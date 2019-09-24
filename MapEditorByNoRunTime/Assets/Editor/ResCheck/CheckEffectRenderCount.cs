using System;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckEffectRenderCount : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "特效Render数错误";
			this.checkPattern = "Assets/(Res|ResTemp)/Effect/models/.*?\\.(fbx|FBX)$";
			this.layout.Set(50f, 500f, 250f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath(asset, typeof(GameObject))) as GameObject;
			int renderCount = this.GetRenderCount(gameObject);
			int num = 30;
			if (asset.Contains("ef_31"))
			{
				num = 3;
			}
			else if (asset.Contains("ef_33"))
			{
				num = 3;
			}
			else if (asset.Contains("ef_37") || asset.Contains("ef_10"))
			{
				num = 5;
			}
            UnityEngine.Object.DestroyImmediate(gameObject);
			if (renderCount > num)
			{
				this.resultInfo.Add(asset, string.Format("render count {0}/{1}", renderCount, num));
				return false;
			}
			return true;
		}

		private int GetRenderCount(GameObject root)
		{
			int num = 0;
			Renderer component = root.GetComponent<SkinnedMeshRenderer>();
			MeshRenderer component2 = root.GetComponent<MeshRenderer>();
			if (component != null || component2 != null)
			{
				num++;
			}
			foreach (object obj in root.transform)
			{
				Transform transform = (Transform)obj;
				component = transform.gameObject.GetComponent<SkinnedMeshRenderer>();
				component2 = transform.gameObject.GetComponent<MeshRenderer>();
				if (component != null || component2 != null)
				{
					num++;
				}
			}
			return num;
		}
	}
}
