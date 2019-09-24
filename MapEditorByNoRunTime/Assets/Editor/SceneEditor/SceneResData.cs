using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneResData : ScriptableObject
{
    private static SceneResData _data;

    public List<EditorMonster> monsterList;
    private static SceneResData data
    {
        get
        {
            if (!SceneResData._data)
            {
                SceneResData._data = ScriptableObject.CreateInstance<SceneResData>();
            }
            return SceneResData._data;
        }
    }


    public static List<EditorMonster> MonsterList
    {
        get
        {
            return SceneResData.data.monsterList;
        }
    }

    private void OnEnable()
    {
        this.UpdateMonster();
    }

    public static void Update()
    {
        SceneResData.data.UpdateMonster();
    }

    public static void Destroy()
    {
       // UnityEngine.Object.DestroyImmediate(SceneResData._data);
    }

    private void UpdateMonster()
    {
        this.monsterList = new List<EditorMonster>();

        Object[] gameObjects = Resources.LoadAll("Monster");

        int num = 0;
        foreach (var item in gameObjects)
        {
            num++;
            EditorMonster editorMonster = ScriptableObject.CreateInstance<EditorMonster>();
            editorMonster.dir = "Monster";
            editorMonster.path = "Monster/" + item.name;
            UnityEngine.Object @object = editorMonster;
            @object.name = item.name;
            editorMonster.monsterId = num;
            editorMonster.GO = Resources.Load("woodSword") as Texture;
            this.monsterList.Add(editorMonster);
        }
        this.monsterList.Sort((EditorMonster x, EditorMonster y) => x.monsterId.CompareTo(y.monsterId));
    }
}
