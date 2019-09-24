using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckSceneObjName : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "场景根物体名称";
			this.checkPattern = ".*?\\.(unity|UNITY)$";
			this.layout.Set(50f, 400f, 200f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			bool result = true;
			foreach (GameObject gameObject in rootObjs)
			{
				if (gameObject.name.StartsWith(" "))
				{
					result = false;
					if (this.resultInfo.ContainsKey(asset))
					{
						this.resultInfo[asset].Add(gameObject.name);
					}
					else
					{
						List<string> list = new List<string>();
						list.Add(gameObject.name);
						this.resultInfo.Add(asset, list);
					}
				}
			}
			return result;
		}
	}
}
