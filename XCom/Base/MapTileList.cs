namespace XCom.Base
{
	public sealed class MapTileList
	{
		#region Fields
		private readonly MapTile[] _tiles;
		private readonly MapLocations _locations;
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets/Sets a MapTile object according to a given location.
		/// </summary>
		internal MapTile this[int col, int row, int lev]
		{
			get
			{
				if (   col > -1 && col < _locations.MaxCols
					&& row > -1 && row < _locations.MaxRows
					&& lev > -1 && lev < _locations.MaxLevs)
				{
					return _tiles[_locations.GetLocationId(col, row, lev)];
				}
				return null;
			}
			set
			{
				_tiles[_locations.GetLocationId(col, row, lev)] = value;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates a MapTileList object.
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="rows"></param>
		/// <param name="levs"></param>
		internal MapTileList(int cols, int rows, int levs)
		{
			_tiles     = new MapTile[cols * rows * levs];
			_locations = new MapLocations(cols, rows, levs);
		}
		#endregion cTor
	}
}
