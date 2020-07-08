using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour
{

    protected const float rotationSpeed = 180f;
    protected const float travelSpeed = 4f;

    public static HexUnit unitPrefab;

    public bool isFly = false;

    public bool isCanWater = false;

    public HexGrid Grid { get; set; }

    public HexCell Location
    {
        get
        {
            return _location;
        }
        set
        {
            if (_location)
            {
                Grid.DecreaseVisibility(_location, visionRange);
                _location.Unit = null;
            }
            _location = value;
            value.Unit = this;
            Grid.IncreaseVisibility(value, visionRange);
            transform.localPosition = value.Position;
            Grid.MakeChildOfColumn(transform, value.ColumnIndex);
        }
    }

    HexCell _location, _currentTravelLocation;

    public float Orientation
    {
        get
        {
            return _orientation;

        }
        set
        {
            _orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public int speed = 24;

    public int visionRange = 3;

    float _orientation;

    List<HexCell> _pathToTravel;

    public void ValidateLocation()
    {
        transform.localPosition = _location.Position;
    }

    public bool IsValidDestination(HexCell cell)
    {
        return (cell.IsExplored && !cell.IsUnderwater && !cell.Unit) || (this.isCanWater && cell.IsUnderwater) || this.isFly;
    }

    public void Travel(List<HexCell> path)
    {
        _location.Unit = null;
        _location = path[path.Count - 1];
        _location.Unit = this;
        _pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    IEnumerator TravelPath()
    {
        Vector3 a, b, c = _pathToTravel[0].Position;
        yield return LookAt(_pathToTravel[1].Position);

        if (!_currentTravelLocation)
        {
            _currentTravelLocation = _pathToTravel[0];
        }
        Grid.DecreaseVisibility(_currentTravelLocation, visionRange);
        int currentColumn = _currentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < _pathToTravel.Count; i++)
        {
            _currentTravelLocation = _pathToTravel[i];
            a = c;
            b = _pathToTravel[i - 1].Position;

            int nextColumn = _currentTravelLocation.ColumnIndex;
            if (currentColumn != nextColumn)
            {
                if (nextColumn < currentColumn - 1)
                {
                    a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                }
                else if (nextColumn > currentColumn + 1)
                {
                    a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                }
                Grid.MakeChildOfColumn(transform, nextColumn);
                currentColumn = nextColumn;
            }

            c = (b + _currentTravelLocation.Position) * 0.5f;
            Grid.IncreaseVisibility(_pathToTravel[i], visionRange);

            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisibility(_pathToTravel[i], visionRange);
            t -= 1f;
        }
        _currentTravelLocation = null;

        a = c;
        b = _location.Position;
        c = b;
        Grid.IncreaseVisibility(_location, visionRange);
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = _location.Position;
        _orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(_pathToTravel);
        _pathToTravel = null;
    }

    IEnumerator LookAt(Vector3 point)
    {
        if (HexMetrics.Wrapping)
        {
            float xDistance = point.x - transform.localPosition.x;
            if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
        }

        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);

        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime * speed
            )
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        transform.LookAt(point);
        _orientation = transform.localRotation.eulerAngles.y;
    }

    public int GetMoveCost(
        HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (!IsValidDestination(toCell))
        {
            return -1;
        }
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        if (edgeType == HexEdgeType.Cliff)
        {
            return -1;
        }
        int moveCost;
        if (fromCell.HasRoadThroughEdge(direction))
        {
            moveCost = 1;
        }
        else if (fromCell.Walled != toCell.Walled)
        {
            return -1;
        }
        else
        {
            moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            moveCost +=
                toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
        }
        return moveCost;
    }

    public void Die()
    {
        if (_location)
        {
            Grid.DecreaseVisibility(_location, visionRange);
        }
        _location.Unit = null;
        Destroy(gameObject);
    }

    public void Save(BinaryWriter writer)
    {
        _location.coordinates.Save(writer);
        writer.Write(_orientation);
    }

    public static void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(
            Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
        );
    }

    void OnEnable()
    {
        if (_location)
        {
            transform.localPosition = _location.Position;
            if (_currentTravelLocation)
            {
                Grid.IncreaseVisibility(_location, visionRange);
                Grid.DecreaseVisibility(_currentTravelLocation, visionRange);
                _currentTravelLocation = null;
            }
        }
    }

    //	void OnDrawGizmos () {
    //		if (_pathToTravel == null || _pathToTravel.Count == 0) {
    //			return;
    //		}
    //
    //		Vector3 a, b, c = _pathToTravel[0].Position;
    //
    //		for (int i = 1; i < _pathToTravel.Count; i++) {
    //			a = c;
    //			b = _pathToTravel[i - 1].Position;
    //			c = (b + _pathToTravel[i].Position) * 0.5f;
    //			for (float t = 0f; t < 1f; t += 0.1f) {
    //				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //			}
    //		}
    //
    //		a = c;
    //		b = _pathToTravel[_pathToTravel.Count - 1].Position;
    //		c = b;
    //		for (float t = 0f; t < 1f; t += 0.1f) {
    //			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //		}
    //	}
}