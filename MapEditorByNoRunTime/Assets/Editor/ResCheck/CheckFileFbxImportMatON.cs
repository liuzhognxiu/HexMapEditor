using System;
using UnityEditor;

namespace FY.Tools.ResCheck
{
	public class CheckFileFbxImportMatON : CheckBaseList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "导入材质打开";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(FBX|fbx)$";
		}

		public override bool DoCheck(string asset)
		{
			ModelImporter modelImporter = AssetImporter.GetAtPath(asset) as ModelImporter;
			if (modelImporter == null)
			{
				return true;
			}
			if (modelImporter.importMaterials)
			{
				this.resultInfo.Add(asset);
			}
			return !modelImporter.importMaterials;
		}

		public override bool FixAsset(string asset)
		{
			(AssetImporter.GetAtPath(asset) as ModelImporter).importMaterials = false;
			return true;
		}
	}
}
