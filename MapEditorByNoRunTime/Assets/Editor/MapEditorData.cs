using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class MapEditorData
{
    //将所有游戏场景导出为JSON格式
    //使用之前需要把当前场景加入到Scenes in Build中
    public static void ExportJSON()
    {
        string filepath = Application.dataPath + @"/StreamingAssets/json.txt";
        FileInfo t = new FileInfo(filepath);
        if (!File.Exists(filepath))
        {
            File.Delete(filepath);
        }

        StreamWriter sw = t.CreateText();

        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("GameObjects");
        writer.WriteArrayStart();

        foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path;
                writer.WriteObjectStart();
                writer.WritePropertyName("scenes");
                writer.WriteArrayStart();
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(name);
                writer.WritePropertyName("gameObject");
                writer.WriteArrayStart();

                foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if (obj.layer == 8 || obj.layer == 9)
                    {
                        writer.WriteObjectStart();
                        writer.WritePropertyName("name");
                        writer.Write(obj.name.Replace("(Clone)",""));

                        //收集物体的父物体
                        writer.WritePropertyName("layer");
                        writer.Write(obj.layer.ToString());
                        
                        //收集物体的位置信息
                        writer.WritePropertyName("position");
                        writer.WriteArrayStart();
                        writer.WriteObjectStart();
                        writer.WritePropertyName("x");
                        writer.Write(obj.transform.position.x.ToString("F5"));
                        writer.WritePropertyName("y");
                        writer.Write(obj.transform.position.y.ToString("F5"));
                        writer.WritePropertyName("z");
                        writer.Write(obj.transform.position.z.ToString("F5"));
                        writer.WriteObjectEnd();
                        writer.WriteArrayEnd();

                        writer.WritePropertyName("rotation");
                        writer.WriteArrayStart();
                        writer.WriteObjectStart();
                        writer.WritePropertyName("x");
                        writer.Write(obj.transform.rotation.eulerAngles.x.ToString("F5"));
                        writer.WritePropertyName("y");
                        writer.Write(obj.transform.rotation.eulerAngles.y.ToString("F5"));
                        writer.WritePropertyName("z");
                        writer.Write(obj.transform.rotation.eulerAngles.z.ToString("F5"));
                        writer.WriteObjectEnd();
                        writer.WriteArrayEnd();

                        writer.WritePropertyName("scale");
                        writer.WriteArrayStart();
                        writer.WriteObjectStart();
                        writer.WritePropertyName("x");
                        writer.Write(obj.transform.localScale.x.ToString("F5"));
                        writer.WritePropertyName("y");
                        writer.Write(obj.transform.localScale.y.ToString("F5"));
                        writer.WritePropertyName("z");
                        writer.Write(obj.transform.localScale.z.ToString("F5"));
                        writer.WriteObjectEnd();
                        writer.WriteArrayEnd();

                        writer.WriteObjectEnd();
                    }
                }

                writer.WriteArrayEnd();
                writer.WriteObjectEnd();
                writer.WriteArrayEnd();
                writer.WriteObjectEnd();
            }
        }
        writer.WriteArrayEnd();
        writer.WriteObjectEnd();

        sw.WriteLine(sb.ToString());
        sw.Close();
        sw.Dispose();
        AssetDatabase.Refresh();
    }
}