using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckColliderTriangles : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "碰撞面数超限";
			this.checkPattern = "Assets/(Res|ResTemp)/(Scene|Level)/.*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout.Set(50f, 400f, 600f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			List<Collider> list = new List<Collider>();
			List<string> list2 = new List<string>();
			foreach (GameObject gameObject in rootObjs)
			{
				list.AddRange(gameObject.GetComponentsInChildren<Collider>(false));
			}
			int num = 0;
			int num2 = 0;
			foreach (Collider collider in list)
			{
				if (this.GetMeshTriangles(collider, ref num, ref num2) && num2 >= 20)
				{
					float num3 = (float)num2 / (float)num;
					if (num3 > 0.1f)
					{
						string transformPath = CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(collider.transform);
						string str = string.Format("{0}/{1} : {2:P}", num2, num, num3);
						list2.Add(transformPath + "     " + str);
					}
				}
			}
			if (list2.Count > 0)
			{
				this.resultInfo.Add(asset, list2);
				return false;
			}
			return true;
		}

		private bool GetMeshTriangles(Collider collider, ref int renderTris, ref int colliderTris)
		{
			if (!(collider is MeshCollider))
			{
				return false;
			}
			if (collider.GetComponent<MeshRenderer>() == null)
			{
				return false;
			}
			renderTris = base.GetRenderMeshTriangles(collider.gameObject);
			colliderTris = base.GetColliderMeshTriangles(collider as MeshCollider);
			return true;
		}
	}
}
