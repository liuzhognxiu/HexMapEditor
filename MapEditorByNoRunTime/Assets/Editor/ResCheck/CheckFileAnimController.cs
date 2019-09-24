using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileAnimController : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "AnimCtroller错误";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(controller|CONTROLLER)$";
		}

		public override bool DoCheck(string asset)
		{
			bool result = true;
			foreach (AnimationClip animationClip in AssetDatabase.LoadAssetAtPath<AnimatorController>(asset).animationClips)
			{
				if (animationClip.name.Contains("preview"))
				{
					result = false;
					this.resultInfo.Add(asset, animationClip.name);
				}
			}
			return result;
		}
	}
}
