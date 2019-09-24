using System;
using System.IO;

namespace FY.Tools.ResCheck
{
	public class CheckFileTypes : CheckBaseStringInt
	{
		public override void Start()
		{
			base.Start();
			this.tag = "所有资源类型";
		}

		public override bool CheckMatch(string asset)
		{
			return true;
		}

		public override bool DoCheck(string asset)
		{
			if ((File.GetAttributes(asset) & FileAttributes.Directory) == FileAttributes.Directory)
			{
				return true;
			}
			string extension = Path.GetExtension(asset);
			if (this.resultInfo.ContainsKey(extension))
			{
				this.resultInfo[extension] = this.resultInfo[extension] + 1;
			}
			else
			{
				this.resultInfo[extension] = 1;
			}
			return true;
		}
	}
}
