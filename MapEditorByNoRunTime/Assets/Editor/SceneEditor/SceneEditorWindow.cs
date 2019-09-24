using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AI;

public class SceneEditorWindow : ScriptableObject
{


    private static SceneEditorWindow instance;
    public static void CreateDungeonEditor()
    {
        if (SceneEditorWindow.instance)
        {
            DestroyImmediate(SceneEditorWindow.instance);
        }
        SceneEditorWindow.instance = ScriptableObject.CreateInstance<SceneEditorWindow>();
        SceneEditorWindow.instance.name = "Dungeon Editor";
        SceneCache.Init();
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

    public SceneEditorWindow()
    {
        SceneEditorWindow.instance = this;
        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.SceneUpdate));
    }

    private void OnDestroy()
    {
        //SceneResData.SaveCache();
        ResWindow.Clear();
        //EntityWindow.Clear();
        //DungeonCache.Destroy();
        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.SceneUpdate));
        this.sceneView.maximized = false;
    }

    private Rect exportButton = new Rect(240f, 0f, 60f, 20f);

    private Rect resButton = new Rect(0f, 0f, 200f, 16f);

    private bool showResWindow;

    private Rect resWindow = new Rect(0f, 0f, 200f, 16f);

    private Rect closeButton = new Rect(320f, 0f, 60f, 20f);

    public static GameObject targetGameObject;

    void SceneUpdate(SceneView SceneView)
    {
        if (EditorApplication.isCompiling)
        {
            DestroyImmediate(this);
            return;
        }
        Handles.BeginGUI();
        this.BuildLayout();
        if (GUI.Button(this.resButton, "资源"))
        {
            this.showResWindow = !this.showResWindow;
        }
        if (GUI.Button(this.closeButton, "关闭"))
        {
            ResWindow.Clear();
            //EntityWindow.Clear();
          
            DestroyImmediate(this);
        }
        if (this.showResWindow)
        {
            GUI.Window(GUIUtility.GetControlID(FocusType.Passive), this.resWindow, new GUI.WindowFunction(ResWindow.OnGUI), "资源");
        }
        Handles.EndGUI();
        this.InputReaction();
    }

    private Rect entityButton = new Rect(0f, 20f, 200f, 16f);
    private void BuildLayout()
    {
        if (Event.current.type != EventType.Layout)
        {
            return;
        }
        this.entityButton = this.resButton;
        if (this.showResWindow)
        {
            this.resWindow = this.resButton;
            this.resWindow.y = this.resWindow.y + 16f;
            this.resWindow.height = this.sceneView.position.height / 2f - 16f;
            this.entityButton.y = this.entityButton.y + this.resWindow.height;
        }
        else
        {
            this.entityButton.y = this.entityButton.y + 16f;
        }
    }

    private void InputReaction()
    {
        Event current = Event.current;
        if (current.type == EventType.DragPerform && DragAndDrop.objectReferences.Length != 0 && DragAndDrop.objectReferences[0] != null && DragAndDrop.objectReferences[0].GetType().BaseType == typeof(EditorEntity))
        {
            if (SceneEditorWindow.targetGameObject == null)
            {
                SceneEditorWindow.targetGameObject = SceneCache.CreateEntity(DragAndDrop.objectReferences[0]);
            }
            SceneEditorWindow.targetGameObject.transform.position = this.CalEntityPosition(current.mousePosition);
            Selection.activeObject = SceneEditorWindow.targetGameObject;
            DragAndDrop.AcceptDrag();
            HandleUtility.Repaint();
            current.Use();
        }
        if (current.type == EventType.DragUpdated && DragAndDrop.objectReferences.Length != 0 && DragAndDrop.objectReferences[0] != null && DragAndDrop.objectReferences[0].GetType().BaseType == typeof(EditorEntity))
        {
            if (SceneEditorWindow.targetGameObject == null)
            {
                SceneEditorWindow.targetGameObject = SceneCache.CreateEntity(DragAndDrop.objectReferences[0]);
            }
            if (SceneEditorWindow.targetGameObject != null)
            {
                SceneEditorWindow.targetGameObject.transform.position = this.CalEntityPosition(current.mousePosition);
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            current.Use();
        }
    }

    private Vector3 CalEntityPosition(Vector2 mousePosition)
    {
        HandleUtility.PushCamera(this.sceneView.camera);
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        object obj = HandleUtility.RaySnap(ray);
        if (obj != null)
        {
            return ((RaycastHit)obj).point;
        }
        Vector3 vector = ray.origin + ray.direction * 10f;
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(vector, out navMeshHit, 999999f, -1))
        {
            return navMeshHit.position;
        }
        return vector;
    }
}
