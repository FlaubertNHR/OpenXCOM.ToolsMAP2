namespace XCom
{
	/// <summary>
	/// EventArgs for when a LocationSelected event fires.
	/// </summary>
	public sealed class LocationSelectedEventArgs
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
		internal LocationSelectedEventArgs(MapLocation location, MapTile tile)
		{
			Location = location;
			Tile     = tile;
		}
	}



	/// <summary>
	/// EventArgs for when a LevelSelected event fires.
	/// </summary>
	public sealed class LevelSelectedEventArgs
	{
		public int Level
		{ get; private set; }

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="level">the new level</param>
		internal LevelSelectedEventArgs(int level)
		{
			Level = level;
		}
	}
}
