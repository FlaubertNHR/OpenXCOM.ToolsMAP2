using System;


namespace XCom
{
	/// <summary>
	/// The <c>SpritesetType</c> is based on the x/y dimensions of the sprites
	/// in a <c><see cref="Spriteset">Spriteset's</see></c>
	/// <c><see cref="Spriteset.Sprites"/></c>.
	/// </summary>
	public enum SpritesetType
	{
		/// <summary>
		/// default
		/// </summary>
		non,

		/// <summary>
		/// a terrain or unit PCK+TAB set is currently loaded. These are
		/// 32x40 w/ 2-byte Tabword (terrain or ufo-unit) or 4-byte Tabword
		/// (tftd-unit).
		/// </summary>
		Pck,

		/// <summary>
		/// a Bigobs PCK+TAB set is currently loaded. Bigobs are 32x48 w/
		/// 2-byte Tabword.
		/// </summary>
		Bigobs,

		/// <summary>
		/// a ScanG iconset is currently loaded. ScanGs are 4x4 w/ 0-byte
		/// Tabword.
		/// </summary>
		ScanG,

		/// <summary>
		/// a LoFT iconset is currently loaded. LoFTs are 16x16 w/ 0-byte
		/// Tabword.
		/// </summary>
		LoFT
	}
}
