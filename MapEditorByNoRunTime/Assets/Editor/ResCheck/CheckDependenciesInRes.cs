using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace FY.Tools.ResCheck
{
	public class CheckDependenciesInRes : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Res引用错误";
			this.checkPattern = "Assets/Res/";
			this.layout.Set(50f, 400f, 500f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			List<string> list = new List<string>();
			foreach (string text in AssetDatabase.GetDependencies(asset))
			{
				if (Regex.IsMatch(text, "Assets/(ResTemp|Test)/"))
				{
					list.Add(text);
				}
			}
			if (list.Count > 0)
			{
				this.resultInfo.Add(asset, list);
				return false;
			}
			return true;
		}

		public override bool FixAsset(string asset)
		{
			bool flag = true;
			foreach (string text in AssetDatabase.GetDependencies(asset))
			{
				if (Regex.IsMatch(text, "Assets/ResTemp/"))
				{
					string desFile = Regex.Replace(text, "Assets/ResTemp/", "Assets/Res/");
					flag = (flag && base.MoveFile(text, desFile));
				}
			}
			return flag;
		}

		private const string INVALID_DEPENDENCY_PATTERN = "Assets/(ResTemp|Test)/";

		private const string RES_TEMP_PATTERN = "Assets/ResTemp/";

		private const string RES_PATTERN = "Assets/Res/";
	}
}
