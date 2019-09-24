using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HexMapCellTypeWindow : ScriptableObject
{
    private static GUISkin guiSkin;

    public static HexMapCellTypeWindow instance;

    public static void CreateCellTypeEditor()
    {
        if (HexMapCellTypeWindow.instance)
        {
            DestroyImmediate(HexMapCellTypeWindow.instance);
        }

        guiSkin = Utility.GetSkin();
        HexMapCellTypeWindow.instance = ScriptableObject.CreateInstance<HexMapCellTypeWindow>();
        SceneView.FrameLastActiveSceneView();
    }

    private SceneView sceneView
    {
        get
        {
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (!sceneView)
            {
                SceneView.FrameLastActiveSceneView();
                sceneView = SceneView.lastActiveSceneView;
            }

            return sceneView;
        }
    }

    public HexMapCellTypeWindow()
    {
        HexMapCellTypeWindow.instance = this;
        SceneView.onSceneGUIDelegate += SceneUpdate;
    }

    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= SceneUpdate;
        this.sceneView.maximized = false;
    }

    public int Elevation = 0;

    public int brushSize = 0;

    public int waterElevation = 0;
    
    public ColorType colorType = ColorType.red;

    public OptionalToggle riverMode = OptionalToggle.Ignore;
    
    public OptionalToggle roadMode = OptionalToggle.Ignore;

    private Rect resWindow = new Rect(3f, 20f, 200f, 500f);

    void SceneUpdate(SceneView SceneView)
    {
        if (EditorApplication.isCompiling)
        {
            DestroyImmediate(this);
            return;
        }

        Handles.BeginGUI();
        if (GUI.Button(new Rect(200, 0f, 100f, 20f), "关闭"))
        {
            DestroyImmediate(this);
            return;
        }

        GUI.Window(GUIUtility.GetControlID(FocusType.Passive), this.resWindow, new GUI.WindowFunction(OnGUI), "地图设置");
        Handles.EndGUI();
        InputReaction();
    }

    void OnGUI(int id)
    {
        GUI.Box(new Rect(5, 20f, 190f, 110f), "设置地图属性");
        if (GUI.Toggle(new Rect(5, 30f, 100f, 32f), colorType == ColorType.red, "red",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            colorType = ColorType.red;
        }

        if (GUI.Toggle(new Rect(5, 60f, 100f, 20f), colorType == ColorType.blue, "blue",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            colorType = ColorType.blue;
        }

        if (GUI.Toggle(new Rect(5, 90f, 100f, 20f), colorType == ColorType.green, "green",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            colorType = ColorType.green;
        }

        GUI.Box(new Rect(5, 133f, 190f, 70f), "设置地图高度");
        Elevation = (int) GUI.HorizontalSlider(new Rect(10, 150f, 80f, 30f), Elevation, 0, 5, guiSkin.horizontalSlider,
            guiSkin.horizontalSliderThumb);

        GUI.Label(new Rect(95f, 150f, 100f, 20f), "高度：" + Elevation);

        brushSize = (int) GUI.HorizontalSlider(new Rect(10, 175f, 80f, 30f), brushSize, 0, 3, guiSkin.horizontalSlider,
            guiSkin.horizontalSliderThumb);

        GUI.Label(new Rect(95f, 175f, 100f, 20f), "刷子大小：" + brushSize);

        GUI.Box(new Rect(5, 206f, 190f, 110f), "设置河流");
        if (GUI.Toggle(new Rect(5, 216f, 100f, 32f), riverMode == OptionalToggle.Ignore, "不设置",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            riverMode = OptionalToggle.Ignore;
        }

        if (GUI.Toggle(new Rect(5, 246, 100f, 20f), riverMode == OptionalToggle.Yes, "添加河流",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            riverMode = OptionalToggle.Yes;
            roadMode = OptionalToggle.Ignore;
        }

        if (GUI.Toggle(new Rect(5, 276f, 100f, 20f), riverMode == OptionalToggle.No, "删除河流",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            riverMode = OptionalToggle.No;
            roadMode = OptionalToggle.Ignore;
        }
        
        GUI.Box(new Rect(5, 320f, 190f, 110f), "设置道路");
        if (GUI.Toggle(new Rect(5, 330f, 100f, 32f), roadMode == OptionalToggle.Ignore, "不设置",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            roadMode = OptionalToggle.Ignore;
        }

        if (GUI.Toggle(new Rect(5, 360, 100f, 20f), roadMode == OptionalToggle.Yes, "添加道路",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            roadMode = OptionalToggle.Yes;
            riverMode = OptionalToggle.Ignore;
        }

        if (GUI.Toggle(new Rect(5, 390f, 100f, 20f), roadMode == OptionalToggle.No, "删除道路",
            new GUIStyle(guiSkin.GetStyle("Toggle"))))
        {
            roadMode = OptionalToggle.No;
            riverMode = OptionalToggle.Ignore;
        }
        GUI.Box(new Rect(5, 410f, 190f, 110f), "设置水面");
        waterElevation = (int) GUI.HorizontalSlider(new Rect(10, 430f, 80f, 30f), waterElevation, 0, 3, guiSkin.horizontalSlider,
            guiSkin.horizontalSliderThumb);

        GUI.Label(new Rect(95f, 430f, 100f, 20f), "水面高度：" + waterElevation);

    }

    private void InputReaction()
    {
        if (Event.current.type == EventType.MouseMove && Event.current.alt)
        {
            if (HexMapEditor.HexGrid == null)
            {
                return;
            }
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                HexMapEditor.HexGrid.brushSize = brushSize;
                HexMapEditor.HexGrid.riverMode = riverMode;           
                HexMapEditor.HexGrid.roadMode = roadMode;
                HexMapEditor.HexGrid.activeWaterLevel = waterElevation; 
                HexCell currentCell = HexMapEditor.HexGrid.GetCell(hit.point);
                if (HexMapEditor.HexGrid.previousCell && HexMapEditor.HexGrid.previousCell != currentCell)
                {
                    ValidateDrag(currentCell);
                }
                else
                {
                    HexMapEditor.HexGrid.isDrag = false;
                }

                HexMapEditor.HexGrid.previousCell = currentCell;
                HexMapEditor.HexGrid.ColorCellByData(hit.point, Utility.GetColorByEnum(colorType), Elevation);
            }
            else
            {
                HexMapEditor.HexGrid.previousCell = null;
            }
        }
    }

    /// <summary>
    /// 验证当前是否有拖拽操作
    /// </summary>
    /// <param name="currentCell"></param>
    void ValidateDrag(HexCell currentCell)
    {
        for (
            HexMapEditor.HexGrid.dragDirection = HexDirection.NE;
            HexMapEditor.HexGrid.dragDirection <= HexDirection.NW;
            HexMapEditor.HexGrid.dragDirection++
        )
        {
            if (HexMapEditor.HexGrid.previousCell.GetNeighbor(HexMapEditor.HexGrid.dragDirection) == currentCell)
            {
                HexMapEditor.HexGrid.isDrag = true;
                return;
            }
        }

        HexMapEditor.HexGrid.isDrag = false;
    }


    private Vector3 CalEntityPosition(Vector2 mousePosition)
    {
        HandleUtility.PushCamera(this.sceneView.camera);
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        object obj = HandleUtility.RaySnap(ray);
        if (obj != null)
        {
            return ((RaycastHit) obj).point;
        }

        Vector3 vector = ray.origin + ray.direction * 10f;
        return vector;
    }
}