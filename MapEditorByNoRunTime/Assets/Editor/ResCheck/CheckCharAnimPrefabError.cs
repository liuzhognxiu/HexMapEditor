using System;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckCharAnimPrefabError : CheckBaseList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "SkeletonAnim检查";
			this.checkPattern = "Assets/Res/Character/.*?_skeleton\\.(prefab|PREFAB)$";
		}

		public override bool DoCheck(string asset)
		{
			Animator component = (AssetDatabase.LoadAssetAtPath(asset, typeof(GameObject)) as GameObject).GetComponent<Animator>();
			if (component == null)
			{
				this.resultInfo.Add(asset);
				return false;
			}
			if (component.runtimeAnimatorController == null)
			{
				this.resultInfo.Add(asset);
				return false;
			}
			if (component.avatar == null)
			{
				this.resultInfo.Add(asset);
				return false;
			}
			return true;
		}
	}
}
