using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Reader : MonoBehaviour
{

    #region 格式
    /// <summary>
    /// {0}：变量名
    /// {1}：变量类型
    /// </summary>
    public const string MEMBER_FORMAT = "private {1} {0} = null;";

    /// <summary>
    /// {0}：变量名
    /// </summary>
    public const string MEMBER_NULL = "{0} = null;";

    /// <summary>
    /// {0}：变量名
    /// {1}：变量类型
    /// </summary>
    public const string MEMBER_ASSIGN =
        "private {1} {0} __LK__ get __LK__ return {3} ?? ({3} = Get<{1}>({2}));__RK____RK__";

    /// <summary>
    /// 存取路径
    /// </summary>
    public const string UIVIEW_PATH = "/TableClass";

    /// <summary>
    /// __LK__：左大括号{
    /// __RK__：右大括号}
    /// {0}：UI名称
    /// {1}：成员变量代码
    /// {2}：变量赋值代码
    /// </summary>
    public const string BEHAVIOUR_FORMAT =
        "using UnityEngine;\r\n" +
        "using System;\r\n" +
        "using System.Collections;\r\n" +
        "using System.Collections.Generic;\r\n\r\n" +
        "[Serializable]\r\n\r\n" +
        "public class {0}\r\n" +
        "__LK__\r\n" +
        "     {1}\r\n" +
        "__RK__\r\n\r\n" +
        "public class {0}List: ScriptableObject\r\n" +
        "__LK__\r\n" +
        "     public List<{0}> {0}_info __LK__ get; set; __RK__\r\n" +
        "__RK__\r\n";
    #endregion
    // FilePath also supports lists and arrays.
    //[InlineButton("LoadCSV")]
    [FolderPath(ParentFolder = "d:/Users/Administrator.DESKTOP-PLMFL1F/Documents/test/Assets/Data")]
    public string ListOfFiles;

    void Start()
    {
        Debug.Log(ListOfFiles);
    }

    private string FilesPath = "d:/Users/Administrator.DESKTOP-PLMFL1F/Documents/test/Assets/Data/";
    void LoadCSV()
    {
       Debug.Log(Utility.LoacCSV(FilesPath + ListOfFiles));
       string result = Utility.LoacCSV(FilesPath + ListOfFiles);
       string FileName = ListOfFiles.Remove(ListOfFiles.Length - 4);
       List<string> CaseList = new List<string>();
       string[,] CSVresult = Utility.SplitCsvGrid(result);
       Utility.DebugOutputGrid(CSVresult);
       for (int i = 0; i < CSVresult.GetUpperBound(0) - 1; i++)
       {
           string ProPertyname = CSVresult[i, 0];
           string ProPerty = CSVresult[i, 1];
           string ProPertyString = "public {1} {0} __LK__ get; set; __RK__ ";
           CaseList.Add("/// <summary>");
           CaseList.Add("///" + ProPertyname);
           CaseList.Add("/// <summary>");
           UnityEngine.Debug.Log(string.Format(ProPertyString, ProPerty, ProPertyname));
           CaseList.Add(string.Format(ProPertyString, ProPerty, "string"));
       }
       string CaseCode = string.Join("\r\n    ", CaseList.ToArray());
       string code = string.Format(BEHAVIOUR_FORMAT, FileName, CaseCode);
       code = code.Replace("__LK__", "{");
       code = code.Replace("__RK__", "}");
       string path = Application.dataPath + UIVIEW_PATH;

       string behaviourCodeFile = Path.Combine(path, FileName) + ".cs";


       Utility.SaveStringToFile(behaviourCodeFile, code);

       AssetDatabase.SaveAssets();
       AssetDatabase.Refresh();
    }
}
