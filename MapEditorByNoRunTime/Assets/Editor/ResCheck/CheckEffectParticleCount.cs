using System;
using System.Collections;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckEffectParticleCount : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "特效粒子数错误";
			this.checkPattern = "Assets/(Res|ResTemp)/Effect/prefabs/npc/.*?\\.(prefab|PREFAB)$";
			this.layout.Set(50f, 500f, 100f, 0f);
		}

		public override bool DoCheckHierarchy(string asset, params GameObject[] rootObjs)
		{
			int num = 30;
			if (asset.Contains("ef_31"))
			{
				num = 3;
			}
			else if (asset.Contains("ef_33"))
			{
				num = 5;
			}
			else if (asset.Contains("ef_37") || asset.Contains("ef_10"))
			{
				num = 5;
			}
			int num2 = 0;
			foreach (GameObject root in rootObjs)
			{
				num2 += this.GetParticalSystemCount(root);
			}
			if (num2 > num)
			{
				this.resultInfo.Add(asset, string.Format("ps count {0}/{1}", num2, num));
				return false;
			}
			return true;
		}

		private int GetParticalSystemCount(GameObject root)
		{
			int num = 0;
			if (root.GetComponent<ParticleSystem>() != null)
			{
				num++;
			}
            foreach (Transform item in root.transform)
            {
                if (item.GetComponent<ParticleSystem>() != null)
                {
                    num++;
                }
            }
            return num;
		}
	}
}
