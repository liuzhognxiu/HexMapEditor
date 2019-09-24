using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckColliderHierarchy : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Collider Hierarchy";
			this.checkPattern = "Assets/(Res|ResTemp)/(Scene|Level)/.*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout = new Vector4(50f, 400f, 400f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			List<Collider> list = new List<Collider>();
			List<string> list2 = new List<string>();
			foreach (GameObject gameObject in rootObjs)
			{
				list.AddRange(gameObject.GetComponentsInChildren<Collider>(false));
			}
			foreach (Collider collider in list)
			{
				if (!this.CheckColliderHiearchy(collider))
				{
					list2.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(collider.transform));
				}
			}
			if (list2.Count > 0)
			{
				this.resultInfo.Add(asset, list2);
				return false;
			}
			return true;
		}

		private bool CheckColliderHiearchy(Collider collider)
		{
			return collider is TerrainCollider || ((collider is BoxCollider || collider is CapsuleCollider) && collider.gameObject.layer == 16) || collider.GetComponent<MeshRenderer>() != null;
		}
	}
}
