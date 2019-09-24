using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckTextureRef : CheckBaseStringInt
	{
		public override void Start()
		{
			base.Start();
			this.tag = "贴图引用";
			this.checkPattern = ".*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.texPattern = ".*?\\.(png|PNG|tga|TGA)$";
			this.layout.Set(50f, 500f, 400f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			bool result = true;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
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
							foreach (string text in AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(renderer.sharedMaterials[num2])))
							{
								if (Regex.IsMatch(text, this.texPattern))
								{
									result = false;
									if (dictionary.ContainsKey(text))
									{
										Dictionary<string, int> dictionary2 = dictionary;
										string key = text;
										int num3 = dictionary2[key];
										dictionary2[key] = num3 + 1;
									}
									else
									{
										dictionary.Add(text, 1);
									}
								}
							}
							num2++;
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				this.resultInfo = dictionary;
			}
			return result;
		}

		private string texPattern = string.Empty;
	}
}
