using System;
using System.Drawing;
using System.Reflection;


namespace XCom
{
	public static class EmbeddedService
	{
		private const int MonoTONE_ERASER = 0; // duplicate MapView.Forms.Observers.QuadrantDrawService.MonoTONE_ERASER

		/// <summary>
		/// Loads the sprites for TopView's blank quads and TileView's eraser.
		/// @note See also Tilepart.LoadMonotoneSprites().
		/// </summary>
		public static SpriteCollection CreateMonotoneSpriteset()
		{
			var ass = Assembly.GetExecutingAssembly();
			using (var strPck = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE.PCK"))
			using (var strTab = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE.TAB"))
			{
				var bytesPck = new byte[strPck.Length];
				var bytesTab = new byte[strTab.Length];

				strPck.Read(bytesPck, 0, (int)strPck.Length);
				strTab.Read(bytesTab, 0, (int)strTab.Length);

				return new SpriteCollection(
										"Monotone",
										Palette.UfoBattle,
										SpritesetsManager.TAB_WORD_LENGTH_2,
										bytesPck,
										bytesTab,
										true);
			}
		}

		/// <summary>
		/// Loads the eraser-sprite for use as a blank-sprite in PckView.
		/// </summary>
		public static Bitmap CreateBlankSprite()
		{
			return BitmapService.CreateSprite(
										XCImage.SpriteWidth32,
										XCImage.SpriteHeight40,
										CreateMonotoneSpriteset()[MonoTONE_ERASER].Bindata,
										Palette.UfoBattle.Table);
		}
	}
}
