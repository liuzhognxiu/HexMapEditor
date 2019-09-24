using System;
using System.Collections.Generic;

namespace FY.Tools.ResCheck
{
	public class CheckRepeatBase : CheckBaseStringList
	{
		public override void Start()
		{
			base.Start();
			this.layout.Set(50f, 100f, 500f, 0f);
		}

		public override void Stop()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.resultInfo)
			{
				if (keyValuePair.Value.Count < 2)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (string key in list)
			{
				this.RemoveInfo(key);
			}
		}

		protected override void DrawTableTitle()
		{
			base.DrawHorizontalInfo(new object[]
			{
				"ID",
				this.layout.x,
				"Key",
				this.layout.y,
				"FullName",
				this.layout.z
			});
		}
	}
}
