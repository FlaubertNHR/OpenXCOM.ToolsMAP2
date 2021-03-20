using System;
using System.Reflection;


namespace XCom
{
	public static class EmbeddedService
	{
		/// <summary>
		/// Loads the sprites for TopView's blank quads and TileView's eraser.
		/// </summary>
		/// <remarks>See also Tilepart.LoadMonotoneSprites().</remarks>
		public static Spriteset CreateMonotoneSpriteset()
		{
			var ass = Assembly.GetExecutingAssembly();
			using (var strPck = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE.PCK"))
			using (var strTab = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE.TAB"))
			{
				var bytesPck = new byte[strPck.Length];
				var bytesTab = new byte[strTab.Length];

				strPck.Read(bytesPck, 0, (int)strPck.Length);
				strTab.Read(bytesTab, 0, (int)strTab.Length);

				return new Spriteset(
								"Monotone",
								Palette.UfoBattle,
								SpritesetManager.TAB_WORD_LENGTH_2,
								bytesPck,
								bytesTab,
								true);
			}
		}
	}
}
