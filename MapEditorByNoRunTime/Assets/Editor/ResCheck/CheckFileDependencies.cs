using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileDependencies : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "所有引用";
			this.layout.Set(50f, 400f, 500f, 0f);
			int num = 0;
            UnityEngine.Object[] objects = Selection.objects;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			foreach (UnityEngine.Object @object in objects)
			{
				if (num != 0)
				{
					stringBuilder.Append("|");
				}
				string text = AssetDatabase.GetAssetPath(@object);
				if (!Path.HasExtension(text))
				{
					text += "/";
				}
				stringBuilder.Append(text);
				num++;
			}
			stringBuilder.Append(")");
			this.checkPattern = stringBuilder.ToString();
		}

		public override bool DoCheck(string asset)
		{
			List<string> list = new List<string>(AssetDatabase.GetDependencies(asset));
			list.Remove(asset);
			if (list.Count != 0)
			{
				this.resultInfo.Add(asset, list);
			}
			return true;
		}
	}
}
