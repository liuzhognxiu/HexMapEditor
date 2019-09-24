using System;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckSkybox : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "配置内置天空盒";
			this.checkPattern = ".*?\\.(unity|UNITY)$";
			this.layout = new Vector4(50f, 600f, 200f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			try
			{
				if (!AssetDatabase.IsNativeAsset(RenderSettings.skybox))
				{
					this.resultInfo.Add(asset, "配置内置天空盒");
					return false;
				}
			}
			catch
			{
			}
			return true;
		}
	}
}
