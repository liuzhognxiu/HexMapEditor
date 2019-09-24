using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    //public int width = 6;

    //public int height = 6;

    public int cellCountX = 4, cellCountZ = 4;

    public HexCell CellPrefab;

    public HexGridChunk chunkPrefab;

    HexCell[] cells;

    public HexGridChunk[] chunks;

    private Color[] dataColors;

    public Text cellLabelPrefab;

    //public Canvas gridCanvas;

    //public HexMesh hexMesh;

    public Texture2D noiseSource;

    public int chunkCountX = 4, chunkCountZ = 3;

    public int brushSize;

    /// <summary>   
    /// 当前激活的高度，编辑地图高度的时候使用
    /// </summary>
    public int activeElevation;

    /// <summary>
    /// 当前激活的水位高度
    /// </summary>
    public int activeWaterLevel;
    
    public bool isDrag;

    public HexDirection dragDirection;

    public HexCell previousCell;

    public OptionalToggle riverMode = OptionalToggle.Ignore;

    public OptionalToggle roadMode = OptionalToggle.Ignore;

    public void GenerateAllHexCell()
    {
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        cells = new HexCell[cellCountX * cellCountZ];
        dataColors = new Color[cellCountX * cellCountZ];

        HexMetrics.noiseSource = noiseSource;

        CreateChunks();
        CreateCells();
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountX; z++)
        {
            for (int x = 0; x < cellCountZ; x++)
            {
                CreateCell(x, z, i++);
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
                chunk.Awake();
            }
        }
    }

    public void RestCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.Color = Color.white;
    }

    private Vector3 position;

    public void EditCells(HexCell center, Color color, int elevation)
    {
        position = center.Position;
        activeElevation = elevation;
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(GetCell(new HexCoordinates(x, z)), color);
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(GetCell(new HexCoordinates(x, z)), color);
            }
        }
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

    void EditCell(HexCell cell, Color applyColor)
    {
        if (cell)
        {
            cell.Color = applyColor;
            cell.Elevation = activeElevation;
            cell.WaterLevel = activeWaterLevel;

            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }

            if (roadMode == OptionalToggle.No)
            {
                cell.RemoveRoads();
            }

            if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    if (riverMode == OptionalToggle.Yes)
                    {
                        otherCell.SetOutgoingRiver(dragDirection);
                    }

                    if (roadMode == OptionalToggle.Yes)
                    {
                        otherCell.AddRoad(dragDirection);
                    }
                }
            }
        }
    }

    private HexCell Curcell;

    //不更新数据颜色转换
    public void ColorCell(HexCell cell, Color color)
    {
        Curcell = cell;
        cell.Color = color;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cell.coordinates != cells[i].coordinates)
            {
                cells[i].Color = dataColors[i];
            }
        }
    }

    //用于显示地图数据，防止怪物强制更新对应的地图数据
    public void ColorCellByData(Vector3 position, Color color, int elevation = 0)
    {
        activeElevation = elevation;
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        dataColors[index] = color;
        EditCells(GetCell(position), color, elevation);
    }

    public void AddModelToSelf(GameObject go)
    {
        go.transform.position = Curcell.transform.position;
        //这里是因为地图模型的原因，地图尺寸正常之后可以完全按照模型比例设置
        go.transform.localScale = new Vector3(10, 10, 10);
        //go.transform.parent = Curcell.transform;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        //每隔一行所有的单元应该向后移动一格。在做乘法之前先减去Z除以2的商
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = -1f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(CellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        dataColors[i] = Color.white;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.uiRect = cellLabelPrefab.rectTransform;
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
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);

        label.text = cell.coordinates.X.ToString() + "\n" + cell.coordinates.Z.ToString();
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

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }
}