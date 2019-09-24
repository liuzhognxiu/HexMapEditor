using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class LayerWindow : EditorWindow
	{
		public static void Open()
		{
			LayerWindow window = EditorWindow.GetWindow<LayerWindow>();
			window.titleContent.text = window.GetType().Name;
			EditorWindow editorWindow = window;
			EditorWindow editorWindow2 = window;
			Vector2 vector = new Vector2(400f, 200f);
			editorWindow2.maxSize = vector;
			editorWindow.minSize = vector;
			window.Show();
		}

		private void OnEnable()
		{
			FieldInfo[] fields = typeof(LayerDefine).GetFields();
			this.list = new GUIContent[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				this.list[i] = new GUIContent(fields[i].Name, "");
				if ((int)fields[i].GetValue(null) == CheckLayerMatch.matchLayer)
				{
					this.select = i;
				}
			}
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			GUILayout.Label(typeof(LayerDefine).Name, new GUILayoutOption[0]);
			if (GUILayout.Button("Check", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				ResCheckMgr inst = ResCheckMgr.inst;
				inst.ResetCheckFlag();
				inst.EnableCheck(new int[]
				{
					37
				});
				inst.StartCheck();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			EditorGUI.BeginChangeCheck();
			this.select = EditorGUILayout.Popup(new GUIContent(typeof(LayerDefine).Name), this.select, this.list, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				string text = this.list[this.select].text;
				foreach (FieldInfo fieldInfo in typeof(LayerDefine).GetFields())
				{
					if (fieldInfo.Name == text)
					{
						CheckLayerMatch.matchLayer = (int)fieldInfo.GetValue(null);
					}
				}
			}
			GUILayout.Space(20f);
			EditorGUILayout.HelpBox("\n\nChoose Layer And Click Check\n\n", MessageType.Info);
		}

		private int select;

		private GUIContent[] list;
	}
}

public static class LayerDefine
{
    public const int LAYER_DEFAULT = 0;

    public const int LAYER_TRANSPARENT_FX = 1;

    public const int LAYER_IGNORE_RAYCAST = 2;

    public const int LAYER_WATER = 4;

    public const int LAYER_UI = 5;

    public const int LAYER_UI_TOPLOGO = 8;

    public const int LAYER_UI_3D_MODEL = 9;

    public const int LAYER_GROUND = 10;

    public const int LAYER_CAMERA_COLLIDER = 11;

    public const int LAYER_SKYBOX = 13;

    public const int LAYER_AIR_WALL = 16;

    public const int LAYER_BOSS = 18;

    public const int LAYER_MAINPLAYERTRIGGER = 19;

    public const int LAYER_MAIN_PLAYER = 20;

    public const int LAYER_PLAYER = 21;

    public const int LAYER_NPC = 22;

    public const int LAYER_TRIGGER = 23;

    public const int LAYER_DYNAMIC_WALL = 30;

    public const int MoveColliderLayers = 1074070545;

    public const int MapHeightColliderLayers = 66577;

    public const int DynamicColliderLayers = 262144;
}
