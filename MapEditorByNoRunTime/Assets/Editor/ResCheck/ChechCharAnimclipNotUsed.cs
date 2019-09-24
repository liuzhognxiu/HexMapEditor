using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class ChechCharAnimclipNotUsed : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "AnimClip未使用";
			this.checkPattern = "Assets/Res/Character/.*?\\.(fbx|FBX)$";
			this.layout.Set(50f, 500f, 200f, 0f);
			this.CollectAnimClipsInUse();
		}

		public override bool DoCheck(string asset)
		{
			bool result = true;
			ModelImporterClipAnimation[] clipAnimations = (AssetImporter.GetAtPath(asset) as ModelImporter).clipAnimations;
			List<string> list = new List<string>();
			foreach (ModelImporterClipAnimation modelImporterClipAnimation in clipAnimations)
			{
				if (!this.clipListInUse.Contains(modelImporterClipAnimation.name))
				{
					result = false;
					list.Add(modelImporterClipAnimation.name);
				}
			}
			if (list.Count > 0)
			{
				this.resultInfo.Add(asset, list);
			}
			return result;
		}

		private void CollectAnimClipsInUse()
		{
			if (this.clipListInUse != null)
			{
				return;
			}
			this.clipListInUse = new List<string>();
			foreach (string text in AssetDatabase.GetAllAssetPaths())
			{
				if (Regex.IsMatch(text, "Assets/Res/Character/.*?\\.controller$"))
				{
					foreach (AnimationClip animationClip in ((RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(text, typeof(RuntimeAnimatorController))).animationClips)
					{
						this.clipListInUse.Add(animationClip.name);
					}
				}
			}
		}

		private List<string> clipListInUse;
	}
}
