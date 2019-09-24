using System;
using System.IO;

namespace FY.Tools.ResCheck
{
	public class CheckEmptyDir : CheckBaseList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "空目录";
			this.checkPattern = "Assets/";
		}

		public override bool DoCheck(string asset)
		{
			if ((File.GetAttributes(asset) & FileAttributes.Directory) != FileAttributes.Directory)
			{
				return true;
			}
			if (Directory.GetDirectories(asset).Length == 0 && Directory.GetFiles(asset).Length == 0)
			{
				this.resultInfo.Add(asset);
				return false;
			}
			return true;
		}

		public override bool FixAsset(string asset)
		{
			//Utils.DeletePath(asset);
			return true;
		}
	}
}
