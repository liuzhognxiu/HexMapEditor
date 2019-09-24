using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileBuiltInShader : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "引用内置Shader";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(mat|MAT)$";
		}

		public override bool DoCheck(string asset)
		{
			Material material = AssetDatabase.LoadAssetAtPath<Material>(asset);
			if (material == null || material.shader == null)
			{
				Debug.Log("Res check mat or shader is null " + asset);
				return true;
			}
			string assetPath = AssetDatabase.GetAssetPath(material.shader);
			if (Regex.IsMatch(assetPath, "(unity_builtin_extra|unity default resources)$"))
			{
				this.resultInfo.Add(asset, assetPath);
				return false;
			}
			return true;
		}

		public override bool FixAsset(string asset)
		{
			Material material = AssetDatabase.LoadAssetAtPath<Material>(asset);
			material.shader = Shader.Find(material.shader.name);
			return true;
		}

		private const string BUILTIN_SHADER_PATTERN = "(unity_builtin_extra|unity default resources)$";
	}
}
