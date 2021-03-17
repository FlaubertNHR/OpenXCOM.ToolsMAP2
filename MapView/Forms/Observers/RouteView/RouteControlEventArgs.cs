using System;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Event args for RouteView.
	/// </summary>
	internal sealed class RouteControlEventArgs
		:
			EventArgs
	{
		#region Properties
		internal MouseButtons MouseButton
		{ get; private set; }

		internal MapTile Tile
		{ get; private set; }

		internal MapLocation Location
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="tile"></param>
		/// <param name="location"></param>
		internal RouteControlEventArgs(
				MouseButtons button,
				MapTile tile,
				MapLocation location)
		{
			MouseButton = button;
			Tile        = tile;
			Location    = location;
		}
		#endregion cTor
	}
}
