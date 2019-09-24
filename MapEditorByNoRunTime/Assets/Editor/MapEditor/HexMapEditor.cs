using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HexMapEditor : EditorWindow
{
    public static HexGrid HexGrid;

    public static HexCell mapCell;

    public static HexGridChunk chunkPrefab;

    private static Color activeColor = Color.red;

    public static int mapSizeX = 3;

    public static int mapSizeY = 3;

    static Transform parent;

    static HexCell[] cells;

    static Canvas gridCanvas;

    public static Text cellLabelPrefab;

    public static Texture2D noiseSource;
    


    [MenuItem("Tools/MapEditor")]
    static void editorMap()
    {
        HexMapEditor win = (HexMapEditor)EditorWindow.GetWindow(typeof(HexMapEditor), false, "MapEditor", false);
        win.autoRepaintOnSceneChange = true;
        win.Show(true);
        SceneView.onSceneGUIDelegate += SceneGUI;
    }

    private static void SceneGUI(SceneView scene)
    {
        if (Event.current.type == EventType.DragUpdated)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                HexGrid.ColorCell(HexGrid.GetCell(hit.point), activeColor);
            }
        }

        if (Event.current.type == EventType.DragExited)
        {
            HexGrid.AddModelToSelf(SceneEditorWindow.targetGameObject);
        }
        
    }

    private bool isCreate = false;
    void OnGUI()
    {
        EditorGUILayout.Space();
        mapCell = EditorGUILayout.ObjectField(mapCell, typeof(HexCell), true) as HexCell;
        cellLabelPrefab = EditorGUILayout.ObjectField(cellLabelPrefab, typeof(Text), true) as Text;
        chunkPrefab = EditorGUILayout.ObjectField(chunkPrefab, typeof(HexGridChunk), true) as HexGridChunk;
        noiseSource = EditorGUILayout.ObjectField(noiseSource, typeof(Texture), true) as Texture2D;
        mapSizeX = EditorGUILayout.IntField("地图长：", mapSizeX);
        mapSizeY = EditorGUILayout.IntField("地图宽：", mapSizeY);
        EditorGUILayout.Space();

        if (GUILayout.Button("生成地图预览"))
        {
            if (mapCell == null)
            {
                Debug.LogError("没有地图格子预制体");
                return;
            }
            GenerateMap();
            isCreate = true;
        }

        if (isCreate)
        {
            for (int i = 0; i < HexMapEditor.HexGrid.chunks.Length; i++)
            {
                HexMapEditor.HexGrid.chunks[i].LateRefresh();
            }
           
        }
        if (GUILayout.Button("打开怪物资源"))
        {
            SceneEditorWindow.CreateDungeonEditor();
        }
        if (GUILayout.Button("打开地图数据编辑"))
        {
            HexMapCellTypeWindow.CreateCellTypeEditor();

        }
        if (GUILayout.Button("保存当前场景数据到Json"))
        {
            MapEditorData.ExportJSON();
        }

    }

    /// <summary>
    /// 生成初始地图
    /// </summary>
    private static void GenerateMap()
    {
        cells = new HexCell[mapSizeX * mapSizeY];
        if (parent == null) parent = new GameObject("Map").AddComponent<HexGrid>().transform;
        //设置地图数据的layer为第8层，方便数据的保存以及处理
        HexGrid = parent.GetComponent<HexGrid>();
        HexGrid.chunkCountX = mapSizeY;
        HexGrid.chunkCountZ = mapSizeX;
        HexGrid.CellPrefab = mapCell;
        HexGrid.cellLabelPrefab = cellLabelPrefab;
        HexGrid.chunkPrefab = chunkPrefab;
        //HexGrid.gridCanvas = GameObject.Instantiate(GameObject.FindObjectOfType<Canvas>());
        //HexGrid.gridCanvas.transform.parent = parent;
        //HexGrid.hexMesh = GameObject.Instantiate(GameObject.FindObjectOfType<HexMesh>());
        //HexGrid.hexMesh.transform.parent = parent;
        HexGrid.noiseSource = noiseSource;

        HexGrid.GenerateAllHexCell();
    }
}

