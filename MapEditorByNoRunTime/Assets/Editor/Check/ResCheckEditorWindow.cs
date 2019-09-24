using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class ResCheckEditorWindow : OdinMenuEditorWindow
{

    [MenuItem("Tools/Buff Window")]
    private static void Open()
    {
        var window = GetWindow<ResCheckEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        tree.AddAllAssetsAtPath("依赖监测", "Assets/Editor/LoadCSV/CSV", typeof(CSVReader), true, true);

        return tree;
    }
}
