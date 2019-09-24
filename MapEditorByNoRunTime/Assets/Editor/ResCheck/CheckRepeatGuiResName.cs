using System;
using System.Collections.Generic;
using System.IO;

namespace FY.Tools.ResCheck
{
	public class CheckRepeatGuiResName : CheckRepeatBase
	{
		public override void Start()
		{
			base.Start();
			this.tag = "GUI图片重名";
			this.checkPattern = "Assets/Res/GUI/.*?\\.(png|prefab|anim|mat|fontsettings|otf|ttf)$";
		}

		public override bool DoCheck(string asset)
		{
			string fileName = Path.GetFileName(asset);
			if (this.resultInfo.ContainsKey(fileName))
			{
				this.resultInfo[fileName].Add(asset);
				return false;
			}
			List<string> list = new List<string>();
			list.Add(asset);
			this.resultInfo.Add(fileName, list);
			return true;
		}

		protected override int AssetPathIndex()
		{
			return 4;
		}
	}
}
