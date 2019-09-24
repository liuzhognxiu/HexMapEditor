using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileTextureSize : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "贴图大小超限";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(TGA|tga|PNG|png|JPG|jpg|exr|psd|PSD|bmp)$";
			this.whiteList = "Assets/Res/GUI";
			this.layout.Set(50f, 600f, 200f, 0f);
			this.texureSizeLimit["Assets/(Res|ResTemp)/Character"] = 512;
			this.texureSizeLimit["Assets/(Res|ResTemp)/Effect"] = 128;
			this.texureSizeLimit["Assets/(Res|ResTemp)/Level"] = 1024;
			this.texureSizeLimit["Assets/(Res|ResTemp)/Scene"] = 1024;
			this.texureSizeLimit["Assets/(Res|ResTemp)/Shader"] = 512;
		}

		public override bool DoCheck(string asset)
		{
			Texture texture = AssetDatabase.LoadAssetAtPath(asset, typeof(Texture)) as Texture;
			foreach (KeyValuePair<string, int> keyValuePair in this.texureSizeLimit)
			{
				if (Regex.IsMatch(asset, keyValuePair.Key))
				{
					int value = keyValuePair.Value;
					if (texture.width > value || texture.height > value)
					{
						string arg = string.Format("{0}x{1}", texture.width, texture.height);
						string arg2 = string.Format("{0}x{0}", value);
						this.resultInfo.Add(asset, string.Format("{0}  (limit:{1})", arg, arg2));
						return false;
					}
				}
			}
			return true;
		}

		private Dictionary<string, int> texureSizeLimit = new Dictionary<string, int>();
	}
}
