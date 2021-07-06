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
		internal ScanGicon(
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
			Pal = Palette.UfoBattle; // default: icons have no integral palette.

			Sprite = BitmapService.CreateSprite(
											XCImage.SpriteWidth,
											XCImage.SpriteHeight,
											_bindata,
											Pal.Table);
		}
		#endregion cTor
	}
}
