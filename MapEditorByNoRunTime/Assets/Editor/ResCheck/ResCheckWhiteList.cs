using System;

namespace FY.Tools.ResCheck
{
	public static class ResCheckWhiteList
	{
		public static bool IsInWhiteList(string asset)
		{
			foreach (string value in ResCheckWhiteList.WhiteList)
			{
				if (asset.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly string[] WhiteList = new string[]
		{
			"Assets/ResTemp/Scene/Demo/",
			"Assets/ResTemp/Scene/common/terrain_temp/"
		};
	}
}
