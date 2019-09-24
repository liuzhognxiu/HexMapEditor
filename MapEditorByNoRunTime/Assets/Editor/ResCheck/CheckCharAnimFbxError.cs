using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckCharAnimFbxError : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Anim导入错误";
			this.checkPattern = "Assets/Res/Character/[0-9]{5}/(ani|models)/.*?\\.(FBX|fbx)$";
			this.layout.Set(50f, 400f, 300f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			ModelImporter modelImporter = AssetImporter.GetAtPath(asset) as ModelImporter;
			if (modelImporter == null)
			{
				return true;
			}
			if (Regex.IsMatch(asset, ".*?/ani/.*?\\.(FBX|fbx)$"))
			{
				if (!modelImporter.importAnimation)
				{
					this.resultInfo.Add("anim:" + asset, "No anim imported");
				}
			}
			else if (modelImporter.importAnimation)
			{
				this.resultInfo.Add("anim:" + asset, "Anim import should be closed");
			}
			if (AssetDatabase.FindAssets("t:avatar", new string[]
			{
				asset
			}).Length != 0)
			{
				if (Regex.IsMatch(asset, ".*?/models/.*?skeleton\\.(FBX|fbx)$"))
				{
					return true;
				}
				this.resultInfo.Add("avatar:" + asset, "Created Avatar");
				return false;
			}
			else
			{
				Avatar sourceAvatar = modelImporter.sourceAvatar;
				if (sourceAvatar == null)
				{
					this.resultInfo.Add("avatar:" + asset, "Avatar Missed");
					return false;
				}
				string name = sourceAvatar.name;
				string value = name.Substring(0, 5);
				if (!asset.Contains(value) || !name.Contains("skeleton"))
				{
					this.resultInfo.Add("avatar:" + asset, "Avatar not match");
					return false;
				}
				return true;
			}
		}

		private const string ANIM_FBX_PATTERN = ".*?/ani/.*?\\.(FBX|fbx)$";

		private const string SKELETON_PATTERN = ".*?/models/.*?skeleton\\.(FBX|fbx)$";
	}
}
