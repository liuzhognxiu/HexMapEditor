using System;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckCharacterHangpoint : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "角色相机挂点丢失";
			this.checkPattern = "Assets/(Res|ResTemp)/Character/[1-3].*[0-9]/prefabs/[1-3].*[0-9]_skeleton\\.(prefab|PREFAB)$";
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			string text = null;
			GameObject gameObject = rootObjs[0];
			if (!gameObject.transform.Find("HP_head1"))
			{
				text = string.Format("{0}-{1}", text, "HP_head1");
			}
			if (!gameObject.transform.Find("HP_head_hit"))
			{
				text = string.Format("{0}-{1}", text, "HP_head_hit");
			}
			if (string.IsNullOrEmpty(text))
			{
				return true;
			}
			this.resultInfo.Add(asset, text);
			return false;
		}

		private const string RESULTFORMAT = "{0}-{1}";

		private const string FILTER1 = "HP_head1";

		private const string FILTER2 = "HP_head_hit";
	}
}
