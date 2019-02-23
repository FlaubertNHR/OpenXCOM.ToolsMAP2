using System;

using XCom.Interfaces;


namespace XCom
{
	internal sealed class ScanGicon
		:
			XCImage
	{
		#region cTor
		/// <summary>
		/// Instantiates a ScanG icon, based on an XCImage.
		/// </summary>
		/// <param name="bindata">the ScanG.Dat source data</param>
		internal ScanGicon(byte[] bindata)
			:
				base(
					bindata,
					XCImage.SpriteWidth,
					XCImage.SpriteHeight,
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					-1)
		{
			Pal = Palette.UfoBattle; // default: icons have no integral palette.

			Sprite = BitmapService.CreateColorized(
												XCImage.SpriteWidth,
												XCImage.SpriteHeight,
												Bindata,
												Pal.ColorTable);
		}
		#endregion
	}
}
