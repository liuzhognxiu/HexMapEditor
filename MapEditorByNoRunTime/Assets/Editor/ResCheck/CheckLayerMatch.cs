using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckLayerMatch : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Layer";
			this.checkPattern = ".*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout.Set(50f, 500f, 400f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			List<string> list = new List<string>();
			foreach (GameObject gameObject in rootObjs)
			{
				if (gameObject.layer == CheckLayerMatch.matchLayer)
				{
					list.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(gameObject.transform));
				}
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					if (transform.gameObject.layer == CheckLayerMatch.matchLayer)
					{
						list.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(transform));
					}
				}
			}
			if (list.Count > 0)
			{
				this.resultInfo.Add(asset, list);
				return false;
			}
			return true;
		}

		public static int matchLayer = 8;
	}
}
