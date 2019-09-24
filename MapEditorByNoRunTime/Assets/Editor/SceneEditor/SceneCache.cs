using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceneCache : ScriptableObject
{
    private static SceneCache _cache;
    public static SceneCache cache
    {
        get
        {
            if (!SceneCache._cache)
            {
                Debug.LogError("DungeonCache null");
            }
            return SceneCache._cache;
        }
    }

    public string scenePath;

    public static string theScenePath
    {
        get
        {
            return SceneCache.cache.scenePath;
        }
    }

    private static Transform _root;
    internal static void Init()
    {
        if (!SceneCache._root)
        {
            GameObject gameObject = GameObject.Find("DungeonRoot");
            if (gameObject == null)
            {
                SceneCache._root = new GameObject("DungeonRoot").transform;
            }
            else
            {
                SceneCache._root = gameObject.transform;
                for (int i = 0; i < SceneCache._root.childCount; i++)
                {
                    UnityEngine.Object.DestroyImmediate(SceneCache._root.GetChild(i).gameObject);
                }
            }
        }
    }

    public static void LoadDungeon(string assetPath, string scenePath)
    {
        SceneCache.CreatAsset(assetPath);
        SceneCache.cache.scenePath = scenePath;
        SceneCache.OnCacheChange();
    }

    public static void CreatAsset(string path)
    {
        SceneCache._cache = ScriptableObject.CreateInstance<SceneCache>();
        AssetDatabase.CreateAsset(SceneCache.cache, path);
        AssetDatabase.SaveAssets();
    }


    public static void LoadAsset(string assetPath)
    {
        SceneCache._cache = (AssetDatabase.LoadAllAssetsAtPath(assetPath).Cast<ScriptableObject>().ToArray<ScriptableObject>().Single((ScriptableObject obj) => obj as SceneCache != null) as SceneCache);
        if (SceneCache.cache == null)
        {
            Debug.Log("Dungeon: Cannot find serialized dungeon cache, creating new instance");
            SceneCache.CreatAsset(assetPath);
        }
    }

    public static Transform dungeonRoot
    {
        get
        {
            if (!SceneCache._root)
            {
                Debug.LogError("dungeonRoot null");
            }
            return SceneCache._root;
        }
    }
    public static GameObject CreateEntity(UnityEngine.Object entity, string selectStateID = "")
    {
        GameObject gameObject = null;
        if (entity is EditorMonster)
        {
            EditorMonster editorMonster = entity as EditorMonster;
            Debug.Log(editorMonster.name);
            gameObject = SceneUtils.CreateEntityInScene(string.Format("Assets/Resources/{0}.prefab", editorMonster.path), editorMonster.name);
            //SceneCache.AddToList<ScriptableObjectMonster>(ScriptableObjectMonster.Create(gameObject.transform, editorMonster.path, selectStateID, editorMonster.monsterId, editorMonster.name), DungeonCache.MonsterList);
        }
        gameObject.transform.SetParent(SceneCache.dungeonRoot);
        SceneCache.OnCacheChange();
        gameObject.layer = 9;
        return gameObject;
    }
                     
    private static void OnCacheChange()
    {
        if (SceneCache.cache)
        {
            SceneCache.SaveDungeonCache();
            EditorUtility.SetDirty(SceneCache.cache);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static void SaveDungeonCache()
    {

    }

}
