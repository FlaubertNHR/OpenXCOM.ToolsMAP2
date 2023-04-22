using System;
using System.Reflection;

using DSShared;


namespace XCom
{
	public static class EmbeddedService
	{
		/// <summary>
		/// Creates a monotone <c><see cref="Spriteset"/></c> from the embedded
		/// <c>MONOTONE.PCK/TAB</c> files.
		/// </summary>
		/// <param name="label">"Monotone" is the label of the spriteset for
		/// TopView's blank quads/TileView's eraser and "Monotone_crippled" is
		/// the label for MainView's crippled tileparts.</param>
		/// <returns>a monotone <c>Spriteset</c></returns>
		public static Spriteset CreateMonotoneSpriteset(string label)
		{
			var ass = Assembly.GetExecutingAssembly();
			using (var pck = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE" + GlobalsXC.PckExt))
			using (var tab = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE" + GlobalsXC.TabExt))
			{
				var bytesPck = new byte[pck.Length];
				var bytesTab = new byte[tab.Length];

				pck.Read(bytesPck, 0, (int)pck.Length);
				tab.Read(bytesTab, 0, (int)tab.Length);

				return new Spriteset( // bypass error-checking
								label,
								Palette.UfoBattle,
								bytesPck,
								bytesTab);
			}
		}
	}
}
