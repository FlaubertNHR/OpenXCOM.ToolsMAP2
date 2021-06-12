using System;
using System.Reflection;

using DSShared;


namespace XCom
{
	public static class EmbeddedService
	{
		/// <summary>
		/// Creates a monotone spriteset from the embedded MONOTONE.PCK/TAB
		/// files.
		/// </summary>
		/// <param name="label">"Monotone" is the label of the spriteset for
		/// TopView's blank quads/TileView's eraser and "Monotone_crippled" is
		/// the label for MainView's crippled tileparts.</param>
		/// <returns>a monotone spriteset</returns>
		public static Spriteset CreateMonotoneSpriteset(string label)
		{
			var ass = Assembly.GetExecutingAssembly();
			using (var strPck = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE" + GlobalsXC.PckExt))
			using (var strTab = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE" + GlobalsXC.TabExt))
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
	}
}
