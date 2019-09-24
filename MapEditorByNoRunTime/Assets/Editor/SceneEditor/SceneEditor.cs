using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneEditor : EditorWindow
{


    [OnOpenAsset(1000)]
    private static bool OpenAsset(int instanceid, int line)
    {
        UnityEngine.Object @object = EditorUtility.InstanceIDToObject(instanceid);
        if (@object is SceneCache && EditorUtility.DisplayDialog("", string.Format("用副本编辑器打开 {0}？", @object.name), "Hm", "Cancle"))
        {
            SceneCache.LoadAsset(AssetDatabase.GetAssetPath(@object));
            SceneUtils.OpenScene();
            SceneEditorWindow.CreateDungeonEditor();
            return true;
        }
        return false;
    }

    [MenuItem("Tools/Dungeon Editor", priority = 120)]
    public static void ShowOpenView()
    {
        SceneEditor sceneEditor = ScriptableObject.CreateInstance<SceneEditor>();
        sceneEditor.name = "DungeonOpenView";
        sceneEditor.minSize = new Vector2(600f, 300f);
        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(sceneEditor.SceneUpdate));
    }

    private void SceneUpdate(SceneView scene)
    {
        Handles.BeginGUI();
        this.OnGUI();
        Handles.EndGUI();
    }

    private void Destroy()
    {
        SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.SceneUpdate));
        UnityEngine.Object.DestroyImmediate(this);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, base.position.height / 3f, base.position.width, base.position.height / 3f));
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.FlexibleSpace();




        if (GUILayout.Button(new GUIContent("New Dungeon"), new GUILayoutOption[]
        {
            GUILayout.Width(100f),
            GUILayout.Height(60f)
        }))
        {
            string text = EditorUtility.OpenFilePanel("New Dungeon", "Assets/Scene/Test", "unity");
            if (!string.IsNullOrEmpty(text) && SceneUtils.CheckScene(text))
            {
                text = text.Replace(Application.dataPath, "Assets");
                string text2 = EditorUtility.SaveFilePanel("Save New Dungeon", "Assets", "NewDungeon", "asset");
                if (!string.IsNullOrEmpty(text2) && !text2.Contains(Application.dataPath))
                {
                    string contents = File.ReadAllText(text2);
                    string arg = text2.Split(new char[]
                    {
                        '/'
                    }).Last<string>();
                    text2 = string.Format("{0}/DungeonTmp/{1}", Application.dataPath, arg);
                    Debug.Log(text2);    
                    File.WriteAllText(text2, contents);    
                }
                text2 = text2.Replace(Application.dataPath, "Assets");
                SceneCache.LoadDungeon(text2, text);
                SceneUtils.OpenScene();
                SceneEditorWindow.CreateDungeonEditor();
                this.Destroy();                                                  
            }
            else
            {
                base.ShowNotification(new GUIContent("Choose .unity"));
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
