using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckComponentLost : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "脚本丢失";
			this.checkPattern = ".*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout.Set(50f, 500f, 400f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			List<string> list = new List<string>();
			foreach (GameObject gameObject in rootObjs)
			{
				list.AddRange(this.Traval(gameObject.transform));
			}
			if (list.Count > 0)
			{
				this.resultInfo.Add(asset, list);
			}
			return list.Count == 0;
		}

		private List<string> Traval(Transform t)
		{
			List<string> list = new List<string>();
			Component[] components = t.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i] == null)
				{
					list.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(t));
				}
			}
			for (int j = 0; j < t.childCount; j++)
			{
				list.AddRange(this.Traval(t.GetChild(j)));
			}
			return list;
		}
	}
}
