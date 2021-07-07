using System;


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
		/// Instantiates a LoFT icon derived on <c><see cref="XCImage"/></c>.
		/// </summary>
		/// <param name="bindata">the LoFTemps.Dat source data for this
		/// <c>LoFTicon</c></param>
		/// <param name="id"></param>
		internal LoFTicon(byte[] bindata, int id)
		{
			Id  = id;
			Pal = Palette.Binary;

			_width = _height = Spriteset.LoFTside;
			_bindata = bindata;

			Sprite = SpriteService.CreateSprite(
											_width,
											_height,
											_bindata,
											Pal.Table);
		}
		#endregion cTor
	}
}
