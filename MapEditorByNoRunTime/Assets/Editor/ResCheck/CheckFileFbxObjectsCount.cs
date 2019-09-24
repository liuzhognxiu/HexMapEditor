using System;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileFbxObjectsCount : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Fbx中对象太多";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(fbx|FBX)$";
			this.layout.Set(50f, 500f, 250f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			bool result = true;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (UnityEngine.Object @object in AssetDatabase.LoadAllAssetsAtPath(asset))
			{
				if (@object is Avatar)
				{
					num++;
				}
				else if (@object is Mesh)
				{
					num2++;
				}
				else if (@object is AnimationClip)
				{
					num3++;
				}
			}
			int num4 = num + num2 + num3;
			if (num4 > 10)
			{
				result = false;
				string value = string.Format("Avatar:{0} Mesh:{1} Clip:{2} All:{3} ", new object[]
				{
					num,
					num2,
					num3,
					num4
				});
				this.resultInfo.Add(asset, value);
			}
			return result;
		}

		private const int FBX_OBJ_LIMIT = 10;
	}
}
