using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckBatchmeshValid : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "合批检查";
			this.checkPattern = ".*?\\.(unity|UNITY|fbx|FBX)$";
			this.layout.Set(50f, 500f, 800f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			bool result = true;
			List<string> list;
			if (this.resultInfo.ContainsKey(asset))
			{
				list = this.resultInfo[asset];
			}
			else
			{
				list = new List<string>();
			}
			int num = 2500;
			try
			{
				for (int i = 0; i < rootObjs.Length; i++)
				{
					foreach (Renderer renderer in rootObjs[i].GetComponentsInChildren<Renderer>(true))
					{
						int num2 = renderer.sharedMaterials.Length;
						MeshFilter component = renderer.GetComponent<MeshFilter>();
						if (component && component.sharedMesh)
						{
							string text = "";
							if (component.sharedMesh.vertexCount > 220)
							{
								text = text + "顶点数超标:" + component.sharedMesh.vertexCount;
							}
							if (GameObjectUtility.AreStaticEditorFlagsSet(renderer.gameObject.gameObject, StaticEditorFlags.LightmapStatic))
							{
								text += "开了LightMap静态合批";
							}
							if (!string.IsNullOrEmpty(text))
							{
								for (int k = 0; k < num2; k++)
								{
									Material material = renderer.sharedMaterials[k];
									if (material && material.renderQueue > num)
									{
										list.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(renderer.transform) + "--" + text);
										result = false;
										break;
									}
								}
							}
						}
					}
				}
				if (list.Count > 0)
				{
					this.resultInfo[asset] = list;
				}
			}
			catch
			{
			}
			return result;
		}

		private const string TIP_VERTEX = "顶点数超标:";

		private const string TIP_STATIC = "开了LightMap静态合批";

		private const int PLANTCOUNT_MAX = 220;
	}
}
