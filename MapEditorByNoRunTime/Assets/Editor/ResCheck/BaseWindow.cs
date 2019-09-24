using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class BaseWindow : EditorWindow
	{
		public void AddChecker(int id, ICheck checker)
		{
			this.checkerList.Add(id, checker);
		}

		public void ClearChecker()
		{
			this.checkerList.Clear();
		}

		private void OnGUI()
		{
			GUILayout.Space(10f);
			if (GUILayout.Button("Save To File", new GUILayoutOption[]
			{
				GUILayout.Width(100f),
				GUILayout.Height(30f)
			}))
			{
				this.SaveInfos();
			}
			GUILayout.Space(20f);
			float num = 40f;
			if (this.drawings == null)
			{
				this.drawings = new List<Drawing>();
				foreach (KeyValuePair<int, ICheck> keyValuePair in this.checkerList)
				{
					List<Drawing> list = keyValuePair.Value.DrawInfo();
					if (list != null && list.Count != 0)
					{
						list.ForEach(delegate(Drawing c)
						{
							c.rect.y = c.rect.y + this.maxY;
						});
						this.drawings.AddRange(list);
						this.maxY = list[list.Count - 1].rect.y;
					}
				}
			}
			Rect position = base.position;
			position.y = num;
			position.height -= position.y;
			position.x = position.width - 16f;
			position.width = 16f;
			float num2 = base.position.height - num;
			if (this.maxY > num2)
			{
				this.scrollPos = GUI.VerticalScrollbar(position, this.scrollPos, num2, 0f, this.maxY);
			}
			else
			{
				this.scrollPos = 0f;
			}
			GUILayout.BeginArea(new Rect(0f, num, base.position.width - 16f, num2));
			foreach (Drawing drawing in this.drawings)
			{
				Rect rect = drawing.rect;
				rect.y -= this.scrollPos;
				if (rect.y + rect.height > 0f && rect.y < num2)
				{
					GUILayout.BeginArea(rect);
					drawing.Draw();
					GUILayout.EndArea();
				}
			}
			GUILayout.EndArea();
		}

		private void SaveInfos()
		{
			string text = EditorUtility.SaveFolderPanel("ResCheckInfo", "", "") + "/ResCheckInfo";
			if (Directory.Exists(text))
			{
				Directory.Delete(text, true);
			}
			string format = text + "/{0}.csv";
			Directory.CreateDirectory(text);
			foreach (KeyValuePair<int, ICheck> keyValuePair in this.checkerList)
			{
				Debug.Log("save to file " + string.Format(format, Const.RECORD_FILE_NAME[keyValuePair.Key]));
				keyValuePair.Value.SaveToFile(string.Format(format, Const.RECORD_FILE_NAME[keyValuePair.Key]));
			}
		}

		private Dictionary<int, ICheck> checkerList = new Dictionary<int, ICheck>();

		private float scrollPos;

		private float maxY;

		private List<Drawing> drawings;
	}
}
