using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckCameraContains : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "包含相机";
			this.checkPattern = ".*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout.Set(50f, 500f, 400f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			bool result = true;
			List<string> list = new List<string>();
			foreach (GameObject gameObject in rootObjs)
			{
				Camera[] componentsInChildren = gameObject.GetComponentsInChildren<Camera>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j].enabled)
					{
						list.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(gameObject.transform));
						result = false;
					}
				}
			}
			if (list.Count > 0)
			{
				this.resultInfo.Add(asset, list);
			}
			return result;
		}
	}
}
