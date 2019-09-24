using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckMaterialLost : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "材质丢失";
			this.checkPattern = ".*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout.Set(50f, 500f, 400f, 0f);
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
			for (int i = 0; i < rootObjs.Length; i++)
			{
				foreach (Component component in rootObjs[i].GetComponentsInChildren<Component>(true))
				{
					if (component is Renderer)
					{
						Renderer renderer = component as Renderer;
						int num = renderer.sharedMaterials.Length;
						int num2 = 0;
						while (num2 < num && (!(renderer is ParticleSystemRenderer) || num2 <= 0))
						{
							if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(renderer.sharedMaterials[num2])))
							{
								result = false;
								list.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(component.transform));
							}
							num2++;
						}
					}
				}
			}
			if (list.Count > 0)
			{
				this.resultInfo[asset] = list;
			}
			return result;
		}
	}
}
