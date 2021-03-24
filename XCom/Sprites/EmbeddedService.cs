using System;
using System.Reflection;

using DSShared;


namespace XCom
{
	public static class EmbeddedService
	{
		/// <summary>
		/// Creates an embedded monotone spriteset.
		/// </summary>
		/// <param name="label">"Monotone" creates the sprites for TopView's
		/// blank quads and TileView's eraser or "Monotone_crippled" creates the
		/// sprites for MainView's crippled tileparts.</param>
		/// <returns>the monotone spriteset</returns>
		public static Spriteset CreateMonotoneSpriteset(string label)
		{
			string tonetype = String.Empty;
			if      (label == "Monotone")          tonetype = "XCom._Embedded.MONOTONE";
			else if (label == "Monotone_crippled") tonetype = "XCom._Embedded.MONOTONE_D";

			if (tonetype.Length != 0)
			{
				var ass = Assembly.GetExecutingAssembly();
				using (var strPck = ass.GetManifestResourceStream(tonetype + GlobalsXC.PckExt))
				using (var strTab = ass.GetManifestResourceStream(tonetype + GlobalsXC.TabExt))
				{
					var bytesPck = new byte[strPck.Length];
					var bytesTab = new byte[strTab.Length];

					strPck.Read(bytesPck, 0, (int)strPck.Length);
					strTab.Read(bytesTab, 0, (int)strTab.Length);

					return new Spriteset(
									label,
									Palette.UfoBattle,
									SpritesetManager.TAB_WORD_LENGTH_2,
									bytesPck,
									bytesTab,
									true);
				}
			}
			return null;
		}
	}
}
