using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public RectTransform uiRect;

    public HexGridChunk chunk;

    public int Index { get; set; }
    public HexCellShaderData ShaderData { get; set; }
    public bool IsVisible
    {
        get
        {
            return visibility > 0;
        }
    }

    int visibility;



    /// <summary>
    /// Cell上的单位
    /// </summary>
    public HexUnit hexUnit;

    int terrainTypeIndex;

    private int distance;

    public int TerrainTypeIndex
    {
        get { return terrainTypeIndex; }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                // Refresh();
                ShaderData.RefreshTerrain(this);
            }
        }
    }

    public int Elevation
    {
        get { return elevation; }
        set
        {
            if (elevation == value)
            {
                return;
            }

            elevation = value;
            RefreshPosition();
            ValidateRivers();

            //检查方向上道路，高度差过大的时候移除道路
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
        }
    }

    public void IncreaseVisibility()
    {
        visibility += 1;
        if (visibility == 1)
        {
            ShaderData.RefreshVisibility(this);
        }
    }

    public void DecreaseVisibility()
    {
        visibility -= 1;
        if (visibility == 0)
        {
            ShaderData.RefreshVisibility(this);
        }
    }

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    /// <summary>
    /// 一个高度偏移,可以作为河流表面的偏移量
    /// </summary>
    public float RiverSurfaceY
    {
        get
        {
            return
                (elevation + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    /// <summary>
    /// 水表面的偏移量
    /// </summary>
    public float WaterSurfaceY
    {
        get
        {
            return
                (waterLevel + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    Color color;

    int elevation = int.MinValue;

    /// <summary>
    /// 当前格子是否有河流
    /// </summary>
    public bool HasRiver
    {
        get { return hasIncomingRiver || hasOutgoingRiver; }
    }

    /// <summary>
    /// 当前cell是否是河流首尾
    /// </summary>
    public bool HasRiverBeginOrEnd
    {
        get { return hasIncomingRiver != hasOutgoingRiver; }
    }

    /// <summary>
    /// 河流是否通过了cell的某个方向的边
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }


    /// <summary>
    /// 两个bool值表示，这个cell中是否有河流流入或者是流出
    /// </summary>
    bool hasIncomingRiver, hasOutgoingRiver;

    /// <summary>
    /// 河流的方向
    /// </summary>
    HexDirection incomingRiver, outgoingRiver;

    public bool HasIncomingRiver
    {
        get { return hasIncomingRiver; }
    }

    public bool HasOutgoingRiver
    {
        get { return hasOutgoingRiver; }
    }

    public HexDirection IncomingRiver
    {
        get { return incomingRiver; }
    }

    public HexDirection OutgoingRiver
    {
        get { return outgoingRiver; }
    }

    /// <summary>
    /// /水位线
    /// </summary>
    public int WaterLevel
    {
        get { return waterLevel; }
        set
        {
            if (waterLevel == value)
            {
                return;
            }

            waterLevel = value;
            ValidateRivers();
            Refresh();
        }
    }

    int waterLevel;

    public int UrbanLevel
    {
        get { return urbanLevel; }
        set
        {
            if (urbanLevel != value)
            {
                urbanLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int FarmLevel
    {
        get { return farmLevel; }
        set
        {
            if (farmLevel != value)
            {
                farmLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int PlantLevel
    {
        get { return plantLevel; }
        set
        {
            if (plantLevel != value)
            {
                plantLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int Distance
    {
        get { return distance; }
        set
        {
            distance = value;
            //            UpdateDistanceLabel();
        }
    }

    private int urbanLevel, farmLevel, plantLevel;

    /// <summary>
    /// 启发式搜索中间值
    /// </summary>
    public int SearchHeuristic { get; set; }

    /// <summary>
    /// 启发式搜索的优先级
    /// </summary>
    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }

    /// <summary>
    /// 搜索的阶段
    /// </summary>
    public int SearchPhase { get; set; }

    /// <summary>
    /// 记录队列中相同优先级的cell
    /// </summary>
    public HexCell NextWithSamePriority { get; set; }

    [SerializeField] HexCell[] neighbors;

    [SerializeField] bool[] roads;

    public HexCell PathFrom { get; set; }

    //    void UpdateDistanceLabel()
    //    {
    //        Text label = uiRect.GetComponent<Text>();
    //        label.text = distance == int.MaxValue ? "" : distance.ToString();
    //    }

    public void SetLabel(string text)
    {
        Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            if (hexUnit)
            {
                hexUnit.ValidateLocation();
            }
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    /// <summary>
    /// 移除流出河流
    /// </summary>
    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }

        hasOutgoingRiver = false;
        Refresh();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除流入河流
    /// </summary>
    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }

        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除整体条河流
    /// </summary>
    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    /// <summary>
    /// 河床的垂直高度
    /// </summary>
    public float StreamBedY
    {
        get
        {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    /// <summary>
    /// 设置流出的河流
    /// </summary>
    /// <param name="direction"></param>
    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction)
        {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!IsValidRiverDestination(neighbor))
        {
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;
        //      RefreshSelfOnly();

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        //      neighbor.RefreshSelfOnly();
        SetRoad((int)direction, false);
    }

    /// <summary>
    /// 检查cell上是否是有道路通过
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 移除道路
    /// </summary>
    public void RemoveRoads()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
            }
        }
    }

    /// <summary>
    /// 添加道路
    /// </summary>
    /// <param name="direction"></param>
    public void AddRoad(HexDirection direction)
    {
        if (
            !roads[(int)direction] && !HasRiverThroughEdge(direction) &&
            GetElevationDifference(direction) <= 1
        )
        {
            SetRoad((int)direction, true);
        }
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = elevation - GetNeighbor(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    /// <summary>
    /// 河流开始或结束
    /// </summary>
    public HexDirection RiverBeginOrEndDirection
    {
        get { return hasIncomingRiver ? incomingRiver : outgoingRiver; }
    }

    /// <summary>
    /// 判断Cell是否低于水位线
    /// </summary>
    public bool IsUnderwater
    {
        get { return waterLevel > elevation; }
    }

    /// <summary>
    /// 判断河流是否是向高的方向流动
    /// </summary>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (
                   elevation >= neighbor.elevation || waterLevel == neighbor.elevation
               );
    }

    /// <summary>
    /// 验证格子高度或者水位线发生变化，对此格子的河流进行验证
    /// </summary>
    void ValidateRivers()
    {
        if (
            hasOutgoingRiver &&
            !IsValidRiverDestination(GetNeighbor(outgoingRiver))
        )
        {
            RemoveOutgoingRiver();
        }

        if (
            hasIncomingRiver &&
            !GetNeighbor(incomingRiver).IsValidRiverDestination(this)
        )
        {
            RemoveIncomingRiver();
        }
    }

    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        position.y +=
            (HexMetrics.SampleNoise(position).y * 2f - 1f) *
            HexMetrics.elevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)elevation);
        writer.Write((byte)waterLevel);
        writer.Write((byte)urbanLevel);
        writer.Write((byte)farmLevel);
        writer.Write((byte)plantLevel);

        if (hasIncomingRiver)
        {
            writer.Write((byte)(incomingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        if (hasOutgoingRiver)
        {
            writer.Write((byte)(outgoingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }
    }

    public void Load(BinaryReader reader)
    {
        terrainTypeIndex = reader.ReadByte();
        ShaderData.RefreshTerrain(this);
        elevation = reader.ReadByte();
        RefreshPosition();
        waterLevel = reader.ReadByte();
        urbanLevel = reader.ReadByte();
        farmLevel = reader.ReadByte();
        plantLevel = reader.ReadByte();

        byte riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            hasIncomingRiver = true;
            incomingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            hasIncomingRiver = false;
        }

        riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            hasOutgoingRiver = true;
            outgoingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            hasOutgoingRiver = false;
        }
    }


    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }
}