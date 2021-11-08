namespace XCom
{
	/// <summary>
	/// Args for when a
	/// <c><see cref="MapFile.LocationSelected">MapFile.LocationSelected</see></c>
	/// event fires.
	/// </summary>
	public sealed class LocationSelectedArgs
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
		internal LocationSelectedArgs(MapLocation location, MapTile tile)
		{
			Location = location;
			Tile     = tile;
		}
	}



	/// <summary>
	/// Args for when a
	/// <c><see cref="MapFile.LevelSelected">MapFile.LevelSelected</see></c>
	/// event fires.
	/// </summary>
	public sealed class LevelSelectedArgs
	{
		public int Level
		{ get; private set; }

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="level">the level selected</param>
		internal LevelSelectedArgs(int level)
		{
			Level = level;
		}
	}
}
