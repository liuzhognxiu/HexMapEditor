using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector.Demos.RPGEditor;

public class BulidLoadBuff : OdinMenuEditorWindow
{
    [MenuItem("Tools/Open BuffEditor")]
    private static void Open()
    {
        var window = GetWindow<BulidLoadBuff>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;
        tree.AddAllAssetsAtPath("LoadBuff", "Assets/Editor/BuffEditor/Buff", typeof(CSBuff), true, true);
        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Buff Item")))
            {
                ScriptableObjectCreator.ShowDialog<CSBuff>("Assets/Editor/BuffEditor/Buff",
                    obj =>
                    {
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
