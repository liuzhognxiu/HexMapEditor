using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneUtils
{
    public static bool CheckFileExist(string path)
    {
        return File.Exists(path);
    }

    public static string CombinePath(string dir, string name)
    {
        return dir + "/" + name;
    }

    public static bool CheckScene(string path)
    {
        return true;
    }

    public static bool CheckAssetFile(string path)
    {
        return true;
    }

    public static string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public static bool LoadFormFile(string path)
    {
        return true;
    }

    public static Texture2D LoadTextureResource(string resPath)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ToolEditor/DungeonEditor/EditorRes/" + resPath);
    }

    public static string GetEditorCacheAssetPath()
    {
        return "";
    }

    public static string GetMonsterPath()
    {
        return "";
    }

    public static string GetFunctionUtilPath()
    {
        return "/ToolEditor/DungeonEditor/Node_Editor/Node_Editor/NodeResources/Saves/";
    }

    public static void OpenScene()
    {
        string theScenePath = SceneCache.theScenePath;
        if (!string.IsNullOrEmpty(theScenePath) && SceneUtils.CheckFileExist(theScenePath))
        {
            AssetDatabase.CopyAsset(theScenePath, "Assets/Copy.unity");
            EditorSceneManager.OpenScene("Assets/Copy.unity");
            return;
        }
    }

    public static GameObject CreateEntityInScene(string path, string name)
    {
        UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        GameObject gameObject;
        if (@object == null)
        {
            Debug.LogWarning("Dungeon: path = " + path + ", asset is null");
            gameObject = GameObject.CreatePrimitive(0);
        }
        else
        {
            gameObject = (UnityEngine.Object.Instantiate(@object) as GameObject);
        }
        gameObject.name = name;
        return gameObject;
    }

    public static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
    {
        if (subAsset == null || mainAsset == null)
        {
            Debug.LogError("subAsset or mainAsset is null");
            return;
        }
        if (!AssetDatabase.Contains(subAsset))
        {
            AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
            subAsset.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            return;
        }
        EditorUtility.SetDirty(subAsset);
    }

    public static T Clone<T>(T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError("cannot clone scriptableObject, 'so' is null ");
            return ScriptableObject.CreateInstance<T>();
        }
        string name = scriptableObject.name;
        T t = UnityEngine.Object.Instantiate<T>(scriptableObject);
        t.name = name;
        return t;
    }
}

