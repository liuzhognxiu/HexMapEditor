using System;
using System.IO;
using UnityEditor;

namespace FY.Tools.ResCheck
{
	public class CheckFileNameSpace : CheckBaseList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "文件名有空格";
			this.checkPattern = "Assets/(Res|ResTemp|Test)/.*?$";
			this.whiteList = "Assets/Res/Shader/Shaders/builtin_shaders";
		}

		public override bool DoCheck(string asset)
		{
			if (!Path.GetFileNameWithoutExtension(asset).Contains(" "))
			{
				return true;
			}
			this.resultInfo.Add(asset);
			return false;
		}

		public override bool FixAsset(string asset)
		{
			string text = Path.GetFileNameWithoutExtension(asset).Replace(" ", "");
			return string.IsNullOrEmpty(AssetDatabase.RenameAsset(asset, text));
		}
	}
}
