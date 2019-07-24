using System;

using XCom.Interfaces.Base;


namespace XCom
{
	/// <summary>
	/// EventArgs with a MapLocation and MapTile object for when a
	/// SelectLocation event fires.
	/// </summary>
	public sealed class SelectLocationEventArgs
	{
		public MapLocation Location
		{ get; private set; }

		public MapTile Tile
		{ get; private set; }

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="tile"></param>
		internal SelectLocationEventArgs(MapLocation location, MapTile tile)
		{
			Location = location;
			Tile     = tile;
		}
	}


	/// <summary>
	/// EventArgs for when a SelectLevel event fires.
	/// </summary>
	public sealed class SelectLevelEventArgs
	{
		public int Level
		{ get; private set; }

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="level">the new level</param>
		internal SelectLevelEventArgs(int level)
		{
			Level = level;
		}
	}
}
