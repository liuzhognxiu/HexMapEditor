using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Hero;

public class HexUnit : MonoBehaviour
{

    protected const float kRotationSpeed = 180f;
    protected const float kTravelSpeed = 4f;

    public static HexUnit unitPrefab;

    public HexUnit attackHexUnit;

    public List<HexUnit> attackHexUnits;

    public bool isCanSelect = true;

    public bool isFly = false;

    public bool isCanWater = false;

    public bool isMonster = false;

    public Action PathfindOverBack;

    public HeroBase unitBase;

    public HexGrid Grid { get; set; }

    public HexCell Location
    {
        get => m_Location;
        set
        {
            if (m_Location)
            {
                Grid.DecreaseVisibility(m_Location, m_VisionRange);
                m_Location.Unit = null;
            }
            m_Location = value;
            value.Unit = this;
            Grid.IncreaseVisibility(value, m_VisionRange);
            if (this.isFly)
            {
                transform.localPosition = new Vector3(value.Position.x, 10, value.Position.z);
            }
            Grid.MakeChildOfColumn(transform, value.ColumnIndex);
        }
    }

    public HexCell m_Location, m_CurrentTravelLocation;

    private float m_Orientation;

    public float Orientation
    {
        get => m_Orientation;
        set
        {
            m_Orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public int speed = 24;

    private int m_VisionRange = 3;
    public int visionRange
    {
        get => m_VisionRange;
        set => m_VisionRange = value;
    }


    public List<HexCell> _pathToTravel;

    public void ValidateLocation()
    {
        transform.localPosition = m_Location.Position;
        if (this.isFly)
        {
            transform.localPosition = new Vector3(m_Location.Position.x, 10, m_Location.Position.z);
        }
    }

    public bool IsValidDestination(HexCell cell)
    {
        return (cell.IsExplored && !cell.IsUnderwater || (this.isCanWater && cell.IsUnderwater) || this.isFly);
    }

    public bool IsMonster()
    {
        return isMonster;
    }

    public virtual void Travel(List<HexCell> path)
    {
        m_Location.Unit = null;
        m_Location = path[path.Count - 1];
        m_Location.Unit = this;
        _pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }


    public virtual IEnumerator TravelPath()
    {
        Vector3 a, b, c = _pathToTravel[0].Position;
        yield return LookAt(_pathToTravel[1].Position);

        if (!m_CurrentTravelLocation)
        {
            m_CurrentTravelLocation = _pathToTravel[0];
        }
        Grid.DecreaseVisibility(m_CurrentTravelLocation, m_VisionRange);
        int currentColumn = m_CurrentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * kTravelSpeed;
        for (int i = 1; i < _pathToTravel.Count; i++)
        {
            m_CurrentTravelLocation = _pathToTravel[i];
            a = c;
            b = _pathToTravel[i - 1].Position;

            int nextColumn = m_CurrentTravelLocation.ColumnIndex;
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

            c = (b + m_CurrentTravelLocation.Position) * 0.5f;
            Grid.IncreaseVisibility(_pathToTravel[i], m_VisionRange);

            for (; t < 1f; t += Time.deltaTime * kTravelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisibility(_pathToTravel[i], m_VisionRange);
            t -= 1f;
        }
        m_CurrentTravelLocation = null;

        a = c;
        b = m_Location.Position;
        c = b;
        Grid.IncreaseVisibility(m_Location, m_VisionRange);
        for (; t < 1f; t += Time.deltaTime * kTravelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }



        transform.localPosition = m_Location.Position;
        if (this.isFly)
        {
            transform.localPosition = new Vector3(m_Location.Position.x, m_Location.Position.y + 10, m_Location.Position.z);
        }
        m_Orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(_pathToTravel);
        PathfindOverBack.Invoke();
        _pathToTravel = null;
    }

    public IEnumerator LookAt(Vector3 point)
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
            float speed = kRotationSpeed / angle;
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
        m_Orientation = transform.localRotation.eulerAngles.y;
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
        // if (m_Location)
        // {
        //     Grid.DecreaseVisibility(m_Location, m_VisionRange);
        // }
        m_Location.Unit = null;
        Destroy(gameObject);
    }

    public void Save(BinaryWriter writer)
    {
        m_Location.coordinates.Save(writer);
        writer.Write(m_Orientation);
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
        if (m_Location)
        {
            transform.localPosition = m_Location.Position;
            if (m_CurrentTravelLocation)
            {
                Grid.IncreaseVisibility(m_Location, m_VisionRange);
                Grid.DecreaseVisibility(m_CurrentTravelLocation, m_VisionRange);
                m_CurrentTravelLocation = null;
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