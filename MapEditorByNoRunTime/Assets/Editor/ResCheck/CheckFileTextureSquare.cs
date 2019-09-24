using System;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class CheckFileTextureSquare : CheckBaseStringString
	{
		public override void Start()
		{
			base.Start();
			this.tag = "贴图不是正方形";
			this.checkPattern = "Assets/(Res|ResTemp)/.*?\\.(TGA|tga|PNG|png|JPG|jpg|exr|psd|PSD|bmp)$";
			this.whiteList = "Assets/Res/GUI|Assets/Res/Scene/.*?/colorgrading/.*?\\.png";
			this.layout.Set(50f, 600f, 100f, 0f);
		}

		public override bool DoCheck(string asset)
		{
			Texture texture = AssetDatabase.LoadAssetAtPath(asset, typeof(Texture)) as Texture;
			if (texture.width != texture.height)
			{
				this.resultInfo.Add(asset, string.Format("{0}x{1}", texture.width, texture.height));
				return false;
			}
			return true;
		}
	}
}
