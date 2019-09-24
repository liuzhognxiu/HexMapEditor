using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MapEditor : EditorWindow
{
    [MenuItem("Map/editor")]
    static void editorMap()
    {
        MapEditor win = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor), false, "MapEditor", false);
        win.autoRepaintOnSceneChange = true;
        win.Show(true);
        SceneView.onSceneGUIDelegate += SceneGUI;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    /// <summary>
    /// Scene视图刷新
    /// </summary>
    /// <param name="scene"></param>
    private static void SceneGUI(SceneView scene)
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                string[] pos = hit.transform.name.Split(',');
                MapCell cell = posCache[int.Parse(pos[0])][int.Parse(pos[1])];
            }
        }
    }

    #region 点击事件
    /// <summary>
    /// 初始化事件监听必备组件
    /// </summary>
    private static void initClick()
    {
        GetOrAddComponent<PhysicsRaycaster>(Camera.main.gameObject);
        GetOrAddComponent<EventSystem>(Camera.main.gameObject);
        GetOrAddComponent<StandaloneInputModule>(Camera.main.gameObject);
    }

    delegate void ClickHandler(MapCell cell, BaseEventData data);

    /// <summary>
    /// 运行时添加点击事件  编辑状态下添加点击组件
    /// </summary>
    /// <param name="unityAction">需要添加的事件</param>
    private static void AddClick(MapCell cell, ClickHandler call)
    {
        var collider = GetOrAddComponent<MeshCollider>(cell.GetGo());
        collider.convex = true;

        //unity运行时添加点击事件
        if (Application.isPlaying)
        {
            EventTrigger trigger = GetOrAddComponent<EventTrigger>(cell.GetGo());

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            //添加的匿名函数方法  要手动清空
            UnityAction<BaseEventData> click = (BaseEventData data) => {
                call(cell, data);
            };
            entry.callback.AddListener(click);

            trigger.triggers.Clear();
            trigger.triggers.Add(entry);
        }
    }

    /// <summary>
    /// 点击格子的事件
    /// </summary>
    /// <param name="arg0"></param>
    private static void ClickCell(MapCell cell, BaseEventData data)
    {
        Debug.Log(cell.GetGo().name);
    }

    private static T GetOrAddComponent<T>(GameObject item) where T : Component
    {
        if (item.GetComponent<T>() != null)
            return item.GetComponent<T>();
        else
            item.AddComponent<T>();
        return item.GetComponent<T>();
    }
    #endregion

    void OnGUI()
    {
        EditorGUILayout.Space();
        mapCell = EditorGUILayout.ObjectField(mapCell, typeof(GameObject)) as GameObject;
        mapSize = EditorGUILayout.IntSlider("地图尺寸", mapSize, 1, 20);
        cellBorder = EditorGUILayout.Slider("格子尺寸", cellBorder, 1f, 10f);
        shape = (MapShape)EditorGUILayout.EnumPopup("地图形状", shape);
        EditorGUILayout.Space();

        if (GUILayout.Button("生成地图预览"))
        {
            if (mapCell == null)
            {
                Debug.LogError("没有地图格子预制体");
                return;
            }
            GenerateMap();
        }
        if (GUILayout.Button("退出编辑"))
        {
            Dispose();
        }
    }

    public enum MapShape
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle, //矩形

        /// <summary>
        /// 六边形
        /// </summary>
        Hexagon,

        /// <summary>
        /// 正方形
        /// </summary>
        Square,
    }

    class MapCell:IEquatable<MapCell>
    {
        Vector3 pos;
        GameObject cell;

        public bool Equals(MapCell other)
        {
            return false;
        }

        public void SetPos(Vector3 pos)
        {
            this.pos = pos;
        }

        public void SetPos(float x, float y, float z)
        {
            this.pos = new Vector3(x, y, z);
            if (cell)
            {
                cell.transform.localPosition = this.pos;
            }
        }

        public void SetGo(GameObject go)
        {
            this.cell = go;
        }

        public GameObject GetGo()
        {
            return cell;
        }

        public Vector3 GetPos()
        {
            return pos;
        }

        public void Clear()
        {
            if (cell)
            {
                Editor.DestroyImmediate(cell);
                cell = null;
            }
        }

        public void SetIndex(int i, int j)
        {
            cell.name = i + ", " + j;
        }
    }

    static Transform parent;

    public static GameObject mapCell;

    [Range(0, 20f)]
    public static int mapSize = 5;   //地图尺寸

    [Range(0, 10.0f)]
    public static float cellBorder = 1.0f; //六边形边长

    public static MapShape shape = MapShape.Square;

    private static List<List<MapCell>> posCache;

    /// <summary>
    /// 生成初始地图
    /// </summary>
    private static void GenerateMap()
    {
        if (parent == null) parent = new GameObject("Map").transform;
        //parent.gameObject.AddComponent<MapEditorHelper>();
        ResetMap();

        switch (shape)
        {
            case MapShape.Hexagon: GenearHexagonPos(); break;
            case MapShape.Rectangle: GenearRectanglePos(); break;
            case MapShape.Square: GenearSquarePos(); break;
            default: break;
        }
        GenerateAllCell();

        initClick();
    }

    /// <summary>
    /// 重置地图
    /// </summary>
    private static void ResetMap()
    {
        if (posCache == null)
            posCache = new List<List<MapCell>>();
        else
        {
            DestoryCells();
        }
             
        for (int i = 0; i < mapSize; i++)
        {
            List<MapCell> row = new List<MapCell>();
            posCache.Add(row);
        }
    }

    /// <summary>
    /// 销毁所有地图格子
    /// </summary>
    private static void DestoryCells()
    {
        if (posCache == null) return;
        for (int i = posCache.Count - 1; i >= 0; i--)
        {
            for (int j = posCache[i].Count - 1; j >= 0; j--)
            {
                posCache[i][j].Clear();
                posCache[i].Remove(posCache[i][j]);
            }
            posCache[i].Clear();
        }
        posCache.Clear();
    }

    /// <summary>
    /// 生成所有格子点
    /// </summary>
    private static void GenerateAllCell()
    {
        for (int i = 0; i < posCache.Count; i++)
        {
            for (int j = 0; j < posCache[i].Count; j++)
            {
                GenerateCell(i, j, parent, posCache[i][j].GetPos());
            }
        }
    }

    /// <summary>
    /// 生成单个格子点
    /// </summary>
    private static void GenerateCell(int i, int j, Transform parent, Vector3 pos)
    {
        if (posCache[i][j].GetGo())
        {
            posCache[i][j].GetGo().transform.parent = parent;
            posCache[i][j].SetPos(pos);
            posCache[i][j].GetGo().SetActive(true);
        }
        else posCache[i][j].SetGo(GameObject.Instantiate(mapCell, pos, Quaternion.Euler(0, 90, 0), parent));

        AddClick(posCache[i][j], ClickCell);
        posCache[i][j].SetIndex(i, j);
    }

    /// <summary>
    /// 生成六边形坐标点
    /// </summary>
    private static void GenearHexagonPos()
    {
        //根据
    }

    /// <summary>
    /// 生成矩形坐标点
    /// </summary>
    private static void GenearRectanglePos()
    {

    }

    /// <summary>
    /// 生成正方形坐标点
    /// </summary>
    private static void GenearSquarePos()
    {
        float xPos = 0;
        MapCell cell;
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                xPos = (i % 2 == 0) ? 0 : (Mathf.Sqrt(3) * 0.5f * cellBorder);
                xPos = xPos - j * Mathf.Sqrt(3) * cellBorder;
                cell = new MapCell();
                cell.SetPos(xPos, 0, i * 1.5f * cellBorder);
                posCache[i].Add(cell);
            }
        }
    }

    /// <summary>
    /// 清除预制体
    /// </summary>
    private static void Dispose()
    {
        DestoryCells();
        if (parent)
        {
            Editor.DestroyImmediate(parent.gameObject);
            parent = null;
        }
    }
}
