﻿using System;


namespace XCom
{
	public sealed class LoFTicon
		:
			XCImage
	{
		#region Fields (static)
		public   const int Length_LoFT      =  32; // each LoFT icon is 32 bytes [(16x16 bits) / 8 bits per byte]
		internal const int Length_LoFT_bits = 256;
		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// Instantiates a LoFT icon, based on an XCImage.
		/// </summary>
		/// <param name="bindata">the LoFTemps.Dat source data per LoFTicon</param>
		/// <param name="id"></param>
		internal LoFTicon(
				byte[] bindata,
				int id)
			:
				base(
					bindata,
					XCImage.SpriteWidth,
					XCImage.SpriteHeight,
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					id)
		{
			Pal = Palette.Binary;

			Sprite = BitmapService.CreateSprite(
											XCImage.SpriteWidth,
											XCImage.SpriteHeight,
											GetBindata(),
											Pal.Table);
		}
		#endregion cTor
	}
}
