using System;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.Observers
{
	#region Event args
	/// <summary>
	/// Event args for RouteView.
	/// </summary>
	internal sealed class RoutePanelEventArgs
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
		internal RoutePanelEventArgs(
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
	#endregion Event args
}
