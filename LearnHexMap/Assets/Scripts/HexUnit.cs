
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HexUnit : MonoBehaviour
{

    public HexGrid Grid { get; set; }

    public static HexUnit unitPrefab;

    const int visionRange = 3;

    /// <summary>
    /// 移动速度
    /// </summary>
    private const float travelSpeed = 4f;

    /// <summary>
    /// 转向速度
    /// </summary>
    private const float rotationSpeed = 180f;
    /// <summary>
    /// 单位所在的格子
    /// </summary>
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                // location.DecreaseVisibility();
                Grid.DecreaseVisibility(location, visionRange);
                location.hexUnit = null;
            }
            location = value;
            value.hexUnit = this;
            // value.DecreaseVisibility();
            Grid.DecreaseVisibility(location, visionRange);
            transform.localPosition = value.Position;
        }
    }

    private List<HexCell> pathToTravel;
    private HexCell location, currentTravelLocation;

    /// <summary>
    /// 单位的方向
    /// </summary>
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    private float orientation;

    void OnEnable()
    {
        if (location)
        {
            ValidateLocation();
            if (currentTravelLocation)
            {
                Grid.IncreaseVisibility(location, visionRange);
                Grid.DecreaseVisibility(currentTravelLocation, visionRange);
                currentTravelLocation = null;
            }
        }
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    public void Die()
    {
        if (location)
        {
            // location.DecreaseVisibility();
            Grid.DecreaseVisibility(location, visionRange);
        }
        location.hexUnit = null;
        Destroy(gameObject);
    }

    public void Save(BinaryWriter writer)
    {
        location.coordinates.Save(writer);
        writer.Write(orientation);
    }

    public static void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(
            Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
        );
    }

    /// <summary>
    /// 是否无法通行
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && !cell.hexUnit;
    }

    public void Travel(List<HexCell> path)
    {
        //		Location = path[path.Count - 1];
        location.hexUnit = null;
        location = path[path.Count - 1];
        location.hexUnit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    /// <summary>
    /// 根据速度更新物体的位置
    /// </summary>
    /// <returns></returns>
    IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        // transform.localPosition = c;
        //如果是再网格内，先进行转向，后进行移动
        yield return LookAt(pathToTravel[1].Position);
        Grid.DecreaseVisibility(pathToTravel[0], visionRange);
        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            currentTravelLocation = pathToTravel[i];
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + currentTravelLocation.Position) * 0.5f;
            Grid.DecreaseVisibility(currentTravelLocation ? currentTravelLocation : pathToTravel[0],
                visionRange);
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisibility(pathToTravel[0], visionRange);
            t -= 1f;
        }
        currentTravelLocation = null;

        a = c;
        b = location.Position;
        c = b;
        Grid.IncreaseVisibility(location, visionRange);
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }
        transform.localPosition = location.Position;
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
    }

    IEnumerator LookAt(Vector3 point)
    {
        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation = Quaternion.LookRotation(point - transform.localPosition);

        float angle = Quaternion.Angle(fromRotation, toRotation);
        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;

            for (float t = Time.deltaTime * speed; t < 1f; t += Time.deltaTime * speed)
            {
                transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }

            transform.LookAt(point);
            orientation = transform.localRotation.eulerAngles.y;
        }
    }

    /// <summary>
    /// 画出当前的路线行驶
    /// </summary>
    void OnDrawGizmos()
    {
        if (pathToTravel == null || pathToTravel.Count == 0)
        {
            return;
        }

        Vector3 a, b, c = pathToTravel[0].Position;

        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + pathToTravel[i].Position) * 0.5f;
            for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
            }
        }

        a = c;
        b = pathToTravel[pathToTravel.Count - 1].Position;
        c = b;
        for (float t = 0f; t < 1f; t += 0.1f)
        {
            Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
        }
    }

}
