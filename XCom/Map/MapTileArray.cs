namespace XCom
{
	public sealed class MapTileArray
	{
		#region Fields
		private readonly MapTile[]    _tiles;
		private readonly MapLocations _locations;
		#endregion Fields


		#region cTor
		/// <summary>
		/// Instantiates a MapTileArray object.
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="rows"></param>
		/// <param name="levs"></param>
		internal MapTileArray(int cols, int rows, int levs)
		{
			_tiles     = new MapTile[cols * rows * levs];
			_locations = new MapLocations(cols, rows, levs);
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Gets the tile at a specified location.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <returns></returns>
		public MapTile GetTile(int col, int row, int lev)
		{
			if (   col > -1 && col < _locations.MaxCols
				&& row > -1 && row < _locations.MaxRows
				&& lev > -1 && lev < _locations.MaxLevs)
			{
				return _tiles[_locations.GetLocid(col, row, lev)];
			}
			return null;
		}

		/// <summary>
		/// Sets a specified tile at a specified location.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <param name="tile"></param>
		internal void SetTile(int col, int row, int lev, MapTile tile)
		{
			if (   col > -1 && col < _locations.MaxCols
				&& row > -1 && row < _locations.MaxRows
				&& lev > -1 && lev < _locations.MaxLevs)
			{
				_tiles[_locations.GetLocid(col, row, lev)] = tile;
			}
		}
		#endregion Methods
	}
}
