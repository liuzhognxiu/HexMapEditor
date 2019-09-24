using System;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckSceneObjCount : CheckBaseStringInt
	{
		public override void Start()
		{
			base.Start();
			this.tag = "场景根物体太多";
			this.checkPattern = ".*?\\.(unity|UNITY)$";
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			if (rootObjs.Length > 10)
			{
				this.resultInfo.Add(asset, rootObjs.Length);
				return false;
			}
			return true;
		}

		private const int ROOT_OBJ_COUNT_LIMIT = 10;
	}
}
