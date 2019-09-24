using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace FY.Tools.ResCheck
{
	public class CheckFileNameCaps : CheckBaseList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "文件名有大写";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?$";
			this.whiteList = "(Assets/Res/Adapter|Assets/Res/Shader|Assets/Res/Level|Assets/ResTemp/Adapter|Assets/ResTemp/Level)";
		}

		public override bool DoCheck(string asset)
		{
			if (Regex.IsMatch(asset, "Assets/(Res|ResTemp)/(Character|Effect|GUI|Item|Scene|Effect)$"))
			{
				return true;
			}
			if (!Regex.IsMatch(Path.GetFileNameWithoutExtension(asset), ".*[A-Z].*"))
			{
				return true;
			}
			this.resultInfo.Add(asset);
			return false;
		}

		public override bool FixAsset(string asset)
		{
			string text = Path.GetFileNameWithoutExtension(asset).ToLower();
			return string.IsNullOrEmpty(AssetDatabase.RenameAsset(asset, text));
		}

		private const string IGNORE_DIRS = "Assets/(Res|ResTemp)/(Character|Effect|GUI|Item|Scene|Effect)$";
	}
}
