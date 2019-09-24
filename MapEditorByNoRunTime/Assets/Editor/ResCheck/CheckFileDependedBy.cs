using System;
using System.Collections.Generic;

namespace FY.Tools.ResCheck
{
	public class CheckFileDependedBy : CheckFileDependencies
	{
		public override void Start()
		{
			base.Start();
			this.tag = "被引用";
		}

		public override bool DoCheck(string asset)
		{
			Dictionary<string, List<string>> resDependReverse = ResCheckMgr.inst.resDependReverse;
			if (!resDependReverse.ContainsKey(asset))
			{
				return true;
			}
			List<string> list = new List<string>(resDependReverse[asset]);
			list.Remove(asset);
			if (list.Count != 0)
			{
				this.resultInfo.Add(asset, list);
			}
			return true;
		}

		protected override int AssetPathIndex()
		{
			return 4;
		}
	}
}
