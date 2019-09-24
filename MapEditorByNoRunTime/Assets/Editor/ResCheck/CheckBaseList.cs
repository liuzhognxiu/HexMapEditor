using System;
using System.Collections.Generic;

namespace FY.Tools.ResCheck
{
	public class CheckBaseList : CheckBase<string, List<string>>
	{
		public override void Start()
		{
			base.Start();
			this.resultInfo = new List<string>();
		}

		public override bool IsEmpty()
		{
			return this.resultInfo.Count == 0;
		}

		protected override void RemoveInfo(string key)
		{
			this.resultInfo.Remove(key);
		}

		protected override void SortInfo()
		{
			this.resultInfo.Sort();
		}
	}
}
