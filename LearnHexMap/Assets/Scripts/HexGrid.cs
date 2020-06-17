using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    int chunkCountX, chunkCountZ;

    public Color defaultColor = Color.white;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;

    public Texture2D noiseSource;

    HexGridChunk[] chunks;
    HexCell[] cells;
    public int seed;

    public HexUnit unitPrefab;

    List<HexUnit> units = new List<HexUnit>();
    //    public Color[] colors;

    public int cellCountX = 20, cellCountZ = 15;

    HexCellPriorityQueue searchFrontier;

    /// <summary>
    /// 搜索边境
    /// </summary>
    int searchFrontierPhase;

    /// <summary>
    /// 当前已有路径的from和to
    /// </summary>
    HexCell currentPathFrom, currentPathTo;

    bool currentPathExists;


    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
    }

    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        //        HexMetrics.colors = colors;
        // HexUnit.unitPrefab = unitPrefab;
        CreateMap(cellCountX, cellCountZ);
    }

    public bool CreateMap(int x, int z)
    {
        if (
            x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
            z <= 0 || z % HexMetrics.chunkSizeZ != 0
        )
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }


        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                Destroy(chunks[i].gameObject);
            }
        }
        ClearPath();
        ClearUnits();
        cellCountX = x;
        cellCountZ = z;
        chunkCountX = cellCountX / HexMetrics.chunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
        CreateChunks();
        CreateCells();
        return true;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }

        writer.Write(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        ClearPath();
        ClearUnits();
        int x = 20, z = 15;
        if (header >= 1)
        {
            x = reader.ReadInt32();
            z = reader.ReadInt32();
        }

        if (!CreateMap(x, z))
        {
            return;
        }


        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader);
        }

        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }

        //地图创建版本大于功能版本
        if (header >= 2)
        {
            int unitCount = reader.ReadInt32();
            for (int i = 0; i < unitCount; i++)
            {
                HexUnit.Load(reader, this);
            }
        }

    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void OnEnable()
    {
        if (!HexMetrics.noiseSource)
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            HexUnit.unitPrefab = unitPrefab;
            //            HexMetrics.colors = colors;
        }
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index =
            coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }

        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }

        return cells[x + z * cellCountX];
    }

    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return GetCell(hit.point);
        }
        return null;
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        //		cell.Color = defaultColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
        cell.DisableHighlight();
        cell.Elevation = 0;
        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }


    #region A*寻路

    /// <summary>
    /// 寻路
    /// </summary>
    /// <param name="fromCell">从哪里出发</param>
    /// <param name="toCell">目标点</param>
    /// <param name="speed">移动的速度（类似行动力）一次能走多少个cell</param>
    public void FindPath(HexCell fromCell, HexCell toCell, int speed)
    {
        //        StopAllCoroutines();
        //        StartCoroutine(Search(fromCell, toCell, speed));
        ClearPath();
        //检测寻路所消耗的时间
        //        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //        sw.Start();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, speed);
        //        sw.Stop();
        //        Debug.Log(sw.ElapsedMilliseconds);
        if (currentPathExists)
        {
            ShowPath(speed);
        }
    }

    /// <summary>
    /// 寻路算法结束之后，显示路径
    /// </summary>
    /// <param name="speed"></param>
    void ShowPath(int speed)
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                int turn = current.Distance / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }
        currentPathFrom.EnableHighlight(Color.blue);
        currentPathTo.EnableHighlight(Color.red);
    }

    /// <summary>
    /// 清除寻路的路径点
    /// </summary>
    public void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
    }

    /// <summary>
    /// A*寻路
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    /// <returns></returns>
    bool Search(HexCell fromCell, HexCell toCell, int speed)
    {

        searchFrontierPhase += 2;
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        //        for (int i = 0; i < cells.Length; i++)
        //        {
        //            //cells[i].Distance = int.MaxValue;
        //            cells[i].SetLabel(null);
        //            cells[i].DisableHighlight();
        //        }
        //
        //        fromCell.EnableHighlight(Color.blue);

        fromCell.SearchPhase = searchFrontierPhase;

        //toCell.EnableHighlight(Color.red);

        fromCell.Distance = 0;

        searchFrontier.Enqueue(fromCell);

        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            //每当一个cell被检索过之后，格子的搜索阶段+1
            current.SearchPhase += 1;
            ///搜索到目标位置不再搜索
            if (current == toCell)
            {
                return true;
                //下边这些可以在搜索完成之后，另外一个方法调用
                //                while (current != fromCell)
                //                {
                //                    int turn = current.Distance / speed;
                //                    current.SetLabel(turn.ToString());
                //                    current.EnableHighlight(Color.white);
                //                    current = current.PathFrom;
                //                }
                //                toCell.EnableHighlight(Color.red);
                //                break;
            }

            //当前的回合数（逻辑强相关）
            int currentTurn = current.Distance / speed;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);

                //这里可以根据搜索是否完成，避免重复计算
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }

                if (neighbor.IsUnderwater || neighbor.hexUnit)
                {
                    continue;
                }

                //取到当前链接类型
                HexEdgeType edgeType = current.GetEdgeType(neighbor);
                if (current.GetEdgeType(neighbor) == HexEdgeType.Cliff)
                {
                    continue;
                }

                //int distance = current.Distance;
                //移动所消耗的行动力
                int moveCost;

                //如果是有道路的情况距离为1
                if (current.HasRoadThroughEdge(d))
                {
                    moveCost = 1;
                }
                else
                {
                    //如果不是平摊的链接，距离为10
                    moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
                    //加入场景内环境的因素导致距离的问题（游戏逻辑强相关）
                    moveCost += neighbor.UrbanLevel + neighbor.FarmLevel +
                                neighbor.PlantLevel;
                }

                int distance = current.Distance + moveCost;

                //需要行动多少个回合
                int turn = distance / speed;

                if (turn > currentTurn)
                {
                    distance = turn * speed + moveCost;
                }

                //如果没有被检测过的邻居添加到队里中
                //if (neighbor.Distance == int.MaxValue) {
                //如果是搜索阶段小于当前的搜索阶段,这样搜索可以在初始化的时候不再初始化每个cell的初始距离
                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.Distance = distance;
                    neighbor.SearchPhase = searchFrontierPhase;
                    //                    neighbor.SetLabel(turn.ToString());
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic =
                        neighbor.coordinates.DistanceTo(toCell.coordinates);
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    //                    neighbor.SetLabel(turn.ToString());
                    neighbor.PathFrom = current;
                    searchFrontier.Change(neighbor, oldPriority);
                }

            }

        }

        return false;
    }

    #endregion

    #region 启发式搜索

    //     public void FindPath(HexCell fromCell, HexCell toCell)
    //    {
    //        StopAllCoroutines();
    //        StartCoroutine(Search(fromCell, toCell));
    //    }
    //
    //    /// <summary>
    //    /// 启发式搜索算法
    //    /// </summary>
    //    /// <param name="fromCell"></param>
    //    /// <param name="toCell"></param>
    //    /// <returns></returns>
    //    IEnumerator Search(HexCell fromCell, HexCell toCell)
    //    {
    //        for (int i = 0; i < cells.Length; i++)
    //        {
    //            cells[i].Distance = int.MaxValue;
    //            cells[i].DisableHighlight();
    //        }
    //
    //        fromCell.EnableHighlight(Color.blue);
    //        toCell.EnableHighlight(Color.red);
    //
    //        WaitForSeconds delay = new WaitForSeconds(1 / 60f);
    //
    //        List<HexCell> frontier = new List<HexCell>();
    //
    //        fromCell.Distance = 0;
    //
    //        frontier.Add(fromCell);
    //
    //        while (frontier.Count > 0)
    //        {
    //            yield return delay;
    //
    //            HexCell current = frontier[0];
    //            frontier.RemoveAt(0);
    //
    //            ///搜索到目标位置不再搜索
    //            if (current == toCell)
    //            {
    //                current = current.PathFrom;
    //                while (current != fromCell)
    //                {
    //                    current.EnableHighlight(Color.white);
    //                    current = current.PathFrom;
    //                }
    //
    //                break;
    //            }
    //
    //            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
    //            {
    //                HexCell neighbor = current.GetNeighbor(d);
    //
    //                if (neighbor == null || neighbor.Distance != int.MaxValue)
    //                {
    //                    continue;
    //                }
    //
    //                if (neighbor.IsUnderwater)
    //                {
    //                    continue;
    //                }
    //
    //                //取到当前链接类型
    //                HexEdgeType edgeType = current.GetEdgeType(neighbor);
    //                if (current.GetEdgeType(neighbor) == HexEdgeType.Cliff)
    //                {
    //                    continue;
    //                }
    //
    //                int distance = current.Distance;
    //
    //                //如果是有道路的情况距离为1
    //                if (current.HasRoadThroughEdge(d))
    //                {
    //                    distance += 1;
    //                }
    //                else
    //                {
    //                    //如果不是平摊的链接，距离为10
    //                    distance += edgeType == HexEdgeType.Flat ? 5 : 10;
    //                    //加入场景内环境的因素导致距离的问题（游戏逻辑强相关）
    //                    distance += neighbor.UrbanLevel + neighbor.FarmLevel +
    //                                neighbor.PlantLevel;
    //                }
    //
    //                if (neighbor.Distance == int.MaxValue)
    //                {
    //                    neighbor.Distance = distance;
    //                    neighbor.PathFrom = current;
    //                    //当前启发式搜索的值
    //                    neighbor.SearchHeuristic =
    //                        neighbor.coordinates.DistanceTo(toCell.coordinates);
    //                    frontier.Add(neighbor);
    //                }
    //                else if (distance < neighbor.Distance)
    //                {
    //                    neighbor.Distance = distance;
    //                    neighbor.PathFrom = current;
    //                }
    //                
    //                neighbor.Distance = distance;
    //                frontier.Add(neighbor);
    //                frontier.Sort(CompareDistances);
    //            }
    //        }
    //    }
    //
    //    static int CompareDistances(HexCell x, HexCell y)
    //    {
    //        return x.SearchPriority.CompareTo(y.SearchPriority);
    //    }

    #endregion

    #region 广度优先搜索

    /// <summary>
    /// 这里先通过广度优先搜索
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    //    IEnumerator Search(HexCell cell)
    //    {
    //        for (int i = 0; i < cells.Length; i++)
    //        {
    //            cells[i].Distance = int.MaxValue;
    //        }
    //
    //        WaitForSeconds delay = new WaitForSeconds(1 / 60f);
    //
    //        List<HexCell> frontier = new List<HexCell>();
    //
    //        cell.Distance = 0;
    //
    //        frontier.Add(cell);
    //
    //        while (frontier.Count > 0)
    //        {
    //            yield return delay;
    //
    //            HexCell current = frontier[0];
    //            frontier.RemoveAt(0);
    //            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
    //            {
    //                HexCell neighbor = current.GetNeighbor(d);
    //
    //                if (neighbor == null || neighbor.Distance != int.MaxValue)
    //                {
    //                    continue;
    //                }
    //
    //                if (neighbor.IsUnderwater)
    //                {
    //                    continue;
    //                }
    //
    //                //取到当前链接类型
    //                HexEdgeType edgeType = current.GetEdgeType(neighbor);
    //                if (current.GetEdgeType(neighbor) == HexEdgeType.Cliff)
    //                {
    //                    continue;
    //                }
    //
    //                int distance = current.Distance;
    //
    //                //如果是有道路的情况距离为1
    //                if (current.HasRoadThroughEdge(d))
    //                {
    //                    distance += 1;
    //                }
    //                else
    //                {
    //                    //如果不是平摊的链接，距离为10
    //                    distance += edgeType == HexEdgeType.Flat ? 5 : 10;
    //                    //加入场景内环境的因素导致距离的问题（游戏逻辑强相关）
    //                    distance += neighbor.UrbanLevel + neighbor.FarmLevel +
    //                                neighbor.PlantLevel;
    //                }
    //
    //                if (neighbor.Distance == int.MaxValue)
    //                {
    //                    neighbor.Distance = distance;
    //                    frontier.Add(neighbor);
    //                }
    //                else if (distance < neighbor.Distance)
    //                {
    //                    neighbor.Distance = distance;
    //                }
    //
    //                neighbor.Distance = distance;
    //                frontier.Add(neighbor);
    //                frontier.Sort(CompareDistances);
    //            }
    //        }
    //    }


    //    static int CompareDistances(HexCell x, HexCell y)
    //    {
    //        
    //        return x.Distance.CompareTo(y.Distance);
    //    }

    #endregion

    #region units

    void ClearUnits()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Die();
        }
        units.Clear();
    }

    /// <summary>
    /// 添加单位
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="location"></param>
    /// <param name="orientation"></param>
    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    /// <summary>
    /// 销毁目标单位
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(HexUnit unit)
    {
        units.Remove(unit);
        unit.Die();
    }
    #endregion

 
}