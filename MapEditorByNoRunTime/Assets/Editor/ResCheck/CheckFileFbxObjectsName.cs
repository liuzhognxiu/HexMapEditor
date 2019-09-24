using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileFbxObjectsName : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Fbx中对象名称错误";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(fbx|FBX)$";
			this.layout.Set(50f, 600f, 150f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			bool result = true;
			List<string> list = new List<string>();
			foreach (UnityEngine.Object @object in AssetDatabase.LoadAllAssetsAtPath(asset))
			{
				if ((@object is Avatar || @object is Mesh || @object is AnimationClip) && @object.name.StartsWith(" "))
				{
					result = false;
					list.Add(@object.name);
				}
			}
			if (list.Count > 0)
			{
				this.resultInfo.Add(asset, list);
			}
			return result;
		}
	}
}
