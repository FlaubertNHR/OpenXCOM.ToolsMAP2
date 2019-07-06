using System;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Objects of this class are drawn to the screen in MainView.
	/// </summary>
	public abstract class MapTileBase
	{
		/// <summary>
		/// @note This is used only by MapFileBase.SaveGifFile().
		/// </summary>
		public abstract Tilepart[] UsedParts
		{ get; }

		/// <summary>
		/// A tile is flagged as occulted if it has tiles with ground-parts
		/// above and to the south and east.
		/// </summary>
		public bool Occulted
		{ get; set; }
	}
}
