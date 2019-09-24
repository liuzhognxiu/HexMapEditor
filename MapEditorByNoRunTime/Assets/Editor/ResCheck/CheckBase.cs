using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;   
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckBase<I, C> : ICheck where C : IEnumerable
	{
		public static string GetTransformPath(Transform t)
		{
			if (t.parent == null)
			{
				return t.name;
			}
			return string.Format("{0}/{1}", CheckBase<I, C>.GetTransformPath(t.parent), t.name);
		}

		public virtual bool CheckMatch(string asset)
		{
			return (string.IsNullOrEmpty(this.whiteList) || !Regex.IsMatch(asset, this.whiteList)) && !string.IsNullOrEmpty(this.checkPattern) && Regex.IsMatch(asset, this.checkPattern);
		}

		public virtual bool DoCheck(string asset)
		{
			return true;
		}

		public virtual bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			return true;
		}

		public virtual void DoFixHierarchy(string asset, params GameObject[] rootObjs)
		{
			if (this.DoCheckHierarchy(asset, rootObjs))
			{
				return;
			}
			if (this.FixAsset(asset, rootObjs))
			{
				this.RemoveInfo(asset);
				return;
			}
			Debug.LogError(string.Format("Fix asset {0} error!!! ", asset));
		}

		public virtual void DoFix(string asset)
		{
			if (this.DoCheck(asset))
			{
				return;
			}
			if (this.FixAsset(asset))
			{
				this.RemoveInfo(asset);
			}
		}

		public virtual void Start()
		{
			this.selectedAssetInfo = null;
		}

		public virtual void Stop()
		{
			this.SortInfo();
		}

		public virtual List<Drawing> DrawInfo()
		{
			if (this.IsEmpty())
			{
				return null;
			}
			float num = 0f;
			List<Drawing> list = new List<Drawing>();
			list.Add(this.CreateDrawing(ref num, delegate
			{
				this.DrawCheckName(this.tag);
			}));
			list.Add(this.CreateDrawing(ref num, delegate
			{
				this.DrawTableTitle();
			}));
			int num2 = 1;
            foreach (I item in resultInfo)
            {
                int newInt = 0;
                newInt = num2;
                list.Add(CreateDrawing(ref num, delegate
                {
                    DrawItem(newInt, item);
                }));
                num++;
            }
            list.Add(this.CreateDrawing(ref num, delegate
			{
				GUILayout.Space(40f);
			}));
			return list;
		}

		protected Drawing CreateDrawing(ref float lastHeight, Action Draw)
		{
			Drawing drawing = new Drawing();
			drawing.rect = new Rect(0f, lastHeight + 5f, 1000f, EditorGUIUtility.singleLineHeight + 5f);
			drawing.Draw = Draw;
			lastHeight += EditorGUIUtility.singleLineHeight + 5f + 5f;
			return drawing;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in this.resultInfo)
			{
				I item = (I)((object)obj);
				this.AppendInfosForCsv(stringBuilder, new string[]
				{
					this.tag,
					this.ItemToString(item)
				});
			}
			return stringBuilder.ToString();
		}

		public virtual bool IsEmpty()
		{
			return false;
		}

		public void SaveToFile(string fileName)
		{
			if (this.IsEmpty())
			{
				return;
			}
			this.SaveContentToFile(this.ToString(), fileName);
			this.SaveContentToFile("\n\n\n", fileName);
		}

		public virtual bool FixAsset(string asset)
		{
			return false;
		}

		public virtual bool FixAsset(string asset, params GameObject[] rootObjs)
		{
			return false;
		}

		protected virtual void RemoveInfo(string key)
		{
		}

		protected virtual void SortInfo()
		{
		}

		protected void SaveContentToFile(string content, string fileName)
		{
			StreamWriter streamWriter = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write), Encoding.UTF8);
			streamWriter.Write(content);
			streamWriter.Close();
		}

		protected void AppendInfosForCsv(StringBuilder sb, params string[] infos)
		{
			foreach (string value in infos)
			{
				sb.Append(value);
				sb.Append(",,");
			}
			sb.Append("\n");
		}

		protected void DrawText(string info, int width)
		{
			EditorGUILayout.SelectableLabel(info, EditorStyles.textField, new GUILayoutOption[]
			{
				GUILayout.Width((float)width),
				GUILayout.Height(EditorGUIUtility.singleLineHeight + 5f)
			});
		}

		protected void DrawHorizontalInfo(params object[] infoArray)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (int i = 0; i < infoArray.Length / 2; i++)
			{
				this.DrawText(Convert.ToString(infoArray[i * 2]), Convert.ToInt32(infoArray[i * 2 + 1]));
			}
			string text = null;
			int num = this.AssetPathIndex();
			if (num > 0 && infoArray.Length > num)
			{
				text = Convert.ToString(infoArray[num]);
			}
			if (string.IsNullOrEmpty(text))
			{
				EditorGUILayout.EndHorizontal();
				return;
			}
			if (this.selectedAssetInfo != text && GUILayout.Button("Select", new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			}))
			{
				if (!string.IsNullOrEmpty(text))
				{
					this.selectedAssetInfo = text;
					if (text.IndexOf("Assets") >= 0)
					{
						text = text.Substring(text.IndexOf("Assets"));
						Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(text);
					}
				}
				num = this.TransformPathIndex();
				if (num > 0 && infoArray.Length > num)
				{
					text = Convert.ToString(infoArray[num]);
					GameObject gameObject = GameObject.Find(text);
					if (gameObject != null)
					{
						Selection.activeTransform = gameObject.transform;
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		protected virtual int AssetPathIndex()
		{
			return 2;
		}

		protected virtual int TransformPathIndex()
		{
			return 4;
		}

		protected void DrawCheckName(string name)
		{
			float num = this.layout.x + this.layout.y + this.layout.z + this.layout.w + 40f;
			this.DrawHorizontalInfo(new object[]
			{
				string.Format(this.CHECK_NAME_FMT, name),
				num
			});
		}

		protected virtual void DrawTableTitle()
		{
			this.DrawHorizontalInfo(new object[]
			{
				"ID",
				this.layout.x,
				"Asset",
				this.layout.y
			});
		}

		protected virtual void DrawItem(int index, I item)
		{
			this.DrawHorizontalInfo(new object[]
			{
				index.ToString(),
				this.layout.x,
				item,
				this.layout.y
			});
		}

		protected virtual string ItemToString(I item)
		{
			return item.ToString();
		}

		protected void DrawDashLine(int num)
		{
			float num2 = this.layout.x + this.layout.y + this.layout.z + this.layout.w + 40f;
			for (int i = 0; i < num; i++)
			{
				this.DrawHorizontalInfo(new object[]
				{
					"",
					num2
				});
			}
		}

		protected void DrawDashLine()
		{
			this.DrawDashLine(1);
		}

		protected bool MoveFile(string srcFile, string desFile)
		{
			if (Directory.Exists(desFile))
			{
				Debug.LogError(string.Format("Try to move asset {0} error, {1} is exist", srcFile, desFile));
				return false;
			}
			DirectoryInfo parent = Directory.GetParent(desFile);
			if (!parent.Exists)
			{
				parent.Create();
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			string text = AssetDatabase.MoveAsset(srcFile, desFile);
			if (!string.IsNullOrEmpty(text))
			{
                return false;
			}
			return true;
		}

		protected int GetRenderMeshTriangles(GameObject root)
		{
			int num = 0;
			MeshFilter[] componentsInChildren = root.GetComponentsInChildren<MeshFilter>(true);
			SkinnedMeshRenderer[] componentsInChildren2 = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				num += this.GetMeshTriAngleCount(meshFilter.sharedMesh);
			}
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
			{
				num += this.GetMeshTriAngleCount(skinnedMeshRenderer.sharedMesh);
			}
			return num;
		}

		protected int GetColliderMeshTriangles(MeshCollider mc)
		{
			return this.GetMeshTriAngleCount(mc.sharedMesh);
		}

		protected int GetMeshTriAngleCount(Mesh mesh)
		{
			if (mesh == null)
			{
				return 0;
			}
			uint num = 0u;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				num += mesh.GetIndexCount(i) / 3u;
			}
			return (int)num;
		}

		private readonly string CHECK_NAME_FMT = "====================================【{0}】====================================";

		protected string selectedAssetInfo;

		protected string tag = string.Empty;

		protected string checkPattern = string.Empty;

		protected string whiteList = string.Empty;

		protected C resultInfo;

		protected Vector4 layout = new Vector4(50f, 600f);
	}
}
