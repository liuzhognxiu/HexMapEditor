using System;

namespace FY.Tools.ResCheck
{
	public class CheckFileAllUnityFiles : CheckBaseList
	{
		public override void Start()
		{
			base.Start();
			this.tag = "所有Unity文件";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(unity|UNITY)$";
		}

		public override bool DoCheck(string asset)
		{
			if (asset.Contains("stream_stream"))
			{
				return true;
			}
			this.resultInfo.Add(asset);
			return false;
		}
	}
}
