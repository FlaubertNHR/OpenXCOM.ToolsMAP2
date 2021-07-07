using System;


namespace XCom
{
	public sealed class ScanGicon
		:
			XCImage
	{
		#region Fields (static)
		public const int Length_ScanG = 16; // each ScanG icon is 16 bytes

		public const int UNITICON_Max = 35; // icons #0..35 are unit-icons (not terrain-icons)
		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// Instantiates a ScanG icon derived on <c><see cref="XCImage"/></c>.
		/// </summary>
		/// <param name="bindata">the ScanG.Dat source data for this
		/// <c>ScanGicon</c></param>
		/// <param name="id"></param>
		internal ScanGicon(byte[] bindata, int id)
		{
			Id  = id;
			Pal = Palette.UfoBattle; // default: icons have no integral palette.

			_width = _height = Spriteset.ScanGside;
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
