using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckMeshReuse : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "Mesh Reuse";
			this.checkPattern = ".*?\\.(unity|UNITY|prefab|PREFAB)$";
			this.layout = new Vector4(50f, 400f, 400f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			List<MeshCollider> list = new List<MeshCollider>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < rootObjs.Length; i++)
			{
				MeshCollider[] componentsInChildren = rootObjs[i].GetComponentsInChildren<MeshCollider>(true);
				list.AddRange(componentsInChildren);
			}
			foreach (MeshCollider meshCollider in list)
			{
				MeshFilter component = meshCollider.GetComponent<MeshFilter>();
				if (component && component.sharedMesh)
				{
					string assetPath = AssetDatabase.GetAssetPath(meshCollider.sharedMesh);
					string assetPath2 = AssetDatabase.GetAssetPath(component.sharedMesh);
					if (assetPath == assetPath2)
					{
						list2.Add(CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>.GetTransformPath(component.transform));
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
	}
}
