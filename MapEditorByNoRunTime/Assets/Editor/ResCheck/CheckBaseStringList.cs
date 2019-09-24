using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FY.Tools.ResCheck
{
	public class CheckBaseStringList : CheckBase<KeyValuePair<string, List<string>>, Dictionary<string, List<string>>>
	{
		public override void Start()
		{
			base.Start();
			this.resultInfo = new Dictionary<string, List<string>>();
			this.layout.Set(50f, 100f, 500f, 0f);
		}

		public override bool IsEmpty()
		{
			return this.resultInfo.Count == 0;
		}

		protected override void RemoveInfo(string key)
		{
			this.resultInfo.Remove(key);
		}

		protected override void DrawTableTitle()
		{
			base.DrawHorizontalInfo(new object[]
			{
				"ID",
				this.layout.x,
				"Key",
				this.layout.y,
				"Values",
				this.layout.z
			});
		}

		protected override void DrawItem(int index, KeyValuePair<string, List<string>> pair)
		{
			foreach (string text in pair.Value)
			{
				base.DrawHorizontalInfo(new object[]
				{
					index,
					this.layout.x,
					pair.Key,
					this.layout.y,
					text,
					this.layout.z
				});
			}
			base.DrawHorizontalInfo(new object[]
			{
				index,
				this.layout.x,
				"",
				this.layout.y,
				"",
				this.layout.z
			});
		}

		protected override string ItemToString(KeyValuePair<string, List<string>> item)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string arg in item.Value)
			{
				if (num > 0)
				{
					stringBuilder.Append(string.Format(",,{0},,{1}\n", item.Key, arg));
				}
				else
				{
					stringBuilder.Append(string.Format("{0},,{1}\n", item.Key, arg));
				}
				num++;
			}
			return stringBuilder.ToString();
		}

		protected override void SortInfo()
		{
			this.resultInfo = (from i in this.resultInfo
			orderby i.Key
			select i).ToDictionary((KeyValuePair<string, List<string>> i) => i.Key, (KeyValuePair<string, List<string>> j) => j.Value);
		}

		protected virtual void Add(string key, string info)
		{
			List<string> list;
			if (!this.resultInfo.TryGetValue(key, out list))
			{
				list = new List<string>();
				this.resultInfo.Add(key, list);
			}
			list.Add(info);
		}
	}
}
