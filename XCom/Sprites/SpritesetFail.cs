using System;


namespace XCom
{
	/// <summary>
	/// Possible fail-states for
	/// <c><see cref="Spriteset(string, Palette, byte[], byte[], int, int, int, bool)">Spriteset()</see></c>.
	/// </summary>
	public enum SpritesetFail
	{
		/// <summary>
		/// Success.
		/// </summary>
		non,

		/// <summary>
		/// Pck data (uncompressed) overflowed a sprite's length.
		/// </summary>
		ovr,

		/// <summary>
		/// Pck vs Tab count mismatch.
		/// </summary>
		qty,

		/// <summary>
		/// End_of_sprite marker before Pck data's length.
		/// </summary>
		eos,

		/// <summary>
		/// Pck data does not end with End_of_Sprite marker.
		/// </summary>
		pck
	}
}
