using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResWindow : EditorWindow {

    private SceneView sceneView
    {
        get
        {
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (!sceneView)
            {
                sceneView = SceneView.lastActiveSceneView;
            }
            if (!sceneView)
            {
                SceneView.FrameLastActiveSceneView();
                sceneView = SceneView.lastActiveSceneView;
            }
            return sceneView;
        }
    }

    private static ResWindow window;

    private static bool inited;

    public static Vector2 scrollView = Vector2.zero;

    public static void OnGUI(int id)
    {
        if (!ResWindow.inited)
        {
            ResWindow.window = ScriptableObject.CreateInstance<ResWindow>();
            ResWindow.window.Init();
        }
        ResWindow.scrollView = GUILayout.BeginScrollView(ResWindow.scrollView, new GUILayoutOption[0]);
        ResWindow.window.DrawPrefab();
        GUILayout.EndScrollView();
    }

    private static int listLength = 0;
    public void DrawPrefab()
    {
        //GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        ////ResWindow.searchString = GUILayout.TextField(ResWindow.searchString, new GUILayoutOption[0]);
        //GUILayout.EndHorizontal();
        for (int i = 0; i < ResWindow.listLength; i++)
        {
            IEnumerable enumerable = ResWindow.dataList[i];
            bool flag = ResWindow.expendList[i];
            string text = ResWindow.nameList[i];
            if (!string.IsNullOrEmpty(text) && enumerable != null)
            {
                GUIContent guicontent = new GUIContent(text);
                if (GUILayout.Button(guicontent, "BoldLabel", new GUILayoutOption[0]))
                {
                    flag = !flag;
                    ResWindow.expendList[i] = flag;
                }
                if (flag)
                {
                    foreach (object obj in enumerable)
                    {
                        EditorEntity editorEntity = obj as EditorEntity;
                        if (editorEntity && !string.IsNullOrEmpty(editorEntity.name))
                        {
                            guicontent.text = editorEntity.name;
                            //guicontent.image = DungeonStyle.MonsterIcon;
                            GUILayout.Label(guicontent, new GUILayoutOption[]
                            {
                                    GUILayout.ExpandWidth(true)
                            });
                            //this.animGOEditor = Editor.CreateEditor(editorEntity.GO);
                            //if (this.animGOEditor != null)
                            //{
                            //    this.animGOEditor.OnPreviewGUI(GUILayoutUtility.GetRect(30f, 240f), EditorStyles.helpBox);
                            //}
                            Event current = Event.current;
                            if (GUILayoutUtility.GetLastRect().Contains(current.mousePosition))
                            {
                                if (current.type == EventType.MouseDrag)
                                {
                                    DragAndDrop.PrepareStartDrag();
                                    DragAndDrop.objectReferences = new UnityEngine.Object[]
                                    {
                                            UnityEngine.Object.Instantiate<EditorEntity>(editorEntity)
                                    };
                                    DragAndDrop.StartDrag("New Entity");
                                    SceneEditorWindow.targetGameObject = null;
                                }
                                if (current.isMouse)
                                {
                                    current.Use();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void Init()
    {
        ResWindow.dataList.Clear();
        ResWindow.expendList.Clear();
        ResWindow.nameList.Clear();
        this.SetList(SceneResData.MonsterList, "monsters", false);
        SceneResData.Destroy();
        ResWindow.listLength = ResWindow.dataList.Count;
        ResWindow.inited = true;
    }

    public static void Clear()
    {
        ResWindow.dataList.Clear();
        ResWindow.expendList.Clear();
        ResWindow.nameList.Clear();
        ResWindow.listLength = 0;
        ResWindow.inited = false;
        UnityEngine.Object.DestroyImmediate(ResWindow.window);
    }
    private void SetList(IEnumerable data, string name, bool expend = false)
    {
        ResWindow.dataList.Add(data);
        ResWindow.expendList.Add(expend);
        ResWindow.nameList.Add(name);
    }

    public static int selectedObjectid;

    private static List<IEnumerable> dataList = new List<IEnumerable>();

    private static List<bool> expendList = new List<bool>();

    private static List<string> nameList = new List<string>();

    private static string searchString;

    private Editor animGOEditor;
}
