using System;
using System.Text.RegularExpressions;
using UnityEditor;

namespace FY.Tools.ResCheck
{
	public class CheckCharAnimTypeError : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Anim类型错误";
			this.checkPattern = "Assets/Res/Character/1[0-9][0-9][0-9][0-9].*?\\.(FBX|fbx)$";
			this.layout.Set(50f, 400f, 300f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			ModelImporterAnimationType modelImporterAnimationType;
			if (Regex.IsMatch(asset, "Assets/Res/Character/[3-4]{1}[0-9]{4}/.*?\\.(FBX|fbx)$"))
			{
				modelImporterAnimationType = ModelImporterAnimationType.Generic;
			}
			else
			{
				modelImporterAnimationType = ModelImporterAnimationType.Human;
			}
			ModelImporter modelImporter = AssetImporter.GetAtPath(asset) as ModelImporter;
			if (modelImporter != null)
			{
				ModelImporterAnimationType animationType = modelImporter.animationType;
				if (animationType == null)
				{
					return true;
				}
				if (modelImporterAnimationType != animationType)
				{
					this.resultInfo.Add(asset, "May be " + modelImporterAnimationType.ToString() + "  |||   now " + modelImporter.animationType.ToString());
					return false;
				}
			}
			return true;
		}

		private const string HUMANOID_PATTERN = "Assets/Res/Character/1[0-9]{4}/.*?\\.(FBX|fbx)$";

		private const string MONSTER_PATTERN = "Assets/Res/Character/[3-4]{1}[0-9]{4}/.*?\\.(FBX|fbx)$";
	}
}
