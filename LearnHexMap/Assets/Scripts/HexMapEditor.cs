using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;

    public HexUnit unitPrefab;

    int activeElevation;
    int activeWaterLevel;
    private int activeUrbanLevel, activeFarmLevel, activePlantLevel;

    public Material terrainMaterial;
    int brushSize;

    int activeTerrainTypeIndex;

    bool applyElevation = true;
    bool applyWaterLevel = true;
    bool applyUrbanLevel = true;
    bool editMode;
    private bool applyFarmLevel, applyPlantLevel;

    /// <summary>
    /// 一次寻路的行动力
    /// </summary>
    public int speed = 24;

    enum OptionalToggle
    {
        Ignore,
        Yes,
        No
    }

    OptionalToggle riverMode, roadMode;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell, searchFromCell, searchToCell;


    void Awake()
    {
        terrainMaterial.DisableKeyword("GRID_ON");
        SetEditMode(false);
    }

    public void SetEditMode(bool toggle)
    {
        // editMode = toggle;
        // hexGrid.ShowUI(!toggle);
        enabled = toggle;
    }

    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            terrainMaterial.EnableKeyword("GRID_ON");
        }

        else
        {
            terrainMaterial.DisableKeyword("GRID_ON");
        }
    }

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

    public void SetApplyFarmLevel(bool toggle)
    {
        applyFarmLevel = toggle;
    }

    public void SetFarmLevel(float level)
    {
        activeFarmLevel = (int)level;
    }

    public void SetApplyPlantLevel(bool toggle)
    {
        applyPlantLevel = toggle;
    }

    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }


    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }


    public void SetUrbanLevel(bool toggle)
    {
        applyUrbanLevel = toggle;
    }


    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle)mode;
    }

    //    public void ShowUI(bool visible)
    //    {
    //        hexGrid.ShowUI(visible);
    //    }

    public void SetApplyUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }

    public void Save()
    {
        Debug.Log(Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (
            BinaryWriter writer =
                new BinaryWriter(File.Open(path, FileMode.Create))
        )
        {
            writer.Write(1);
            hexGrid.Save(writer);
        }
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            ///地图的版本号
            int header = reader.ReadInt32();
            if (header <= 1)
            {
                hexGrid.Load(reader, header);
                HexMapCamera.ValidatePosition();
            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                CreateUnit();
                return;
            }
        }
        previousCell = null;
    }

    void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            //
            // if (editMode)
            // {
                EditCells(currentCell);
            // }
            // else if (Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell)
            // {
            //     if (searchFromCell != currentCell)
            //     {
            //         if (searchFromCell)
            //         {
            //             searchFromCell.DisableHighlight();
            //         }
            //     }
            //
            //     searchFromCell = currentCell;
            //     searchFromCell.EnableHighlight(Color.blue);
            //     if (searchToCell)
            //     {
            //         hexGrid.FindPath(searchFromCell, searchToCell, speed);
            //     }
            // }
            // else if (searchFromCell && searchFromCell != currentCell)
            // {
            //     if (searchToCell != currentCell)
            //     {
            //         searchToCell = currentCell;
            //         hexGrid.FindPath(searchFromCell, searchToCell, speed);
            //     }
            // }
            // else
            // {
            //     hexGrid.FindPath(currentCell, searchFromCell, speed);
            // }

            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    /// <summary>
    /// 获取点击到的Cell
    /// </summary>
    /// <returns></returns>
    HexCell GetCellUnderCursor()
    {
        // Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(inputRay, out hit))
        // {
        //     return hexGrid.GetCell(hit.point);
        // }
        return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    /// <summary>
    /// 根据点击的Cell生成对应的单位
    /// </summary>
    void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.hexUnit)
        {
            // HexUnit unit = Instantiate(hexUnit);
            // unit.transform.SetParent(hexGrid.transform, false);
            // unit.Location = cell;
            // //开始创建为随机方向
            // unit.Orientation = Random.Range(0f, 360f);
            hexGrid.AddUnit(
                Instantiate(unitPrefab), cell, Random.Range(0f, 360f)
            );
        }
    }

    /// <summary>
    /// 销毁单位
    /// </summary>
    void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.hexUnit)
        {
            // Destroy(cell.hexUnit.gameObject);
            hexGrid.RemoveUnit(cell.hexUnit);
        }
    }


    void ValidateDrag(HexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        )
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }

        isDrag = false;
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }

            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }

            if (applyWaterLevel)
            {
                cell.WaterLevel = activeWaterLevel;
            }

            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }

            if (applyUrbanLevel)
            {
                cell.UrbanLevel = activeUrbanLevel;
            }

            if (applyFarmLevel)
            {
                cell.FarmLevel = activeFarmLevel;
            }

            if (applyPlantLevel)
            {
                cell.PlantLevel = activePlantLevel;
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
}