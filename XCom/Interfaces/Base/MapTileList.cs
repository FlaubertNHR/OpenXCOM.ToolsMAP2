namespace XCom.Interfaces.Base
{
	internal sealed class MapTileList
	{
		#region Fields
		private readonly MapTile[] _tiles;
		private readonly MapLocations  _locations;
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets/Sets a MapTile object according to a given location.
		/// </summary>
		internal MapTile this[int row, int col, int lev]
		{
			get
			{
				if (   col > -1 && col < _locations.MaxCols
					&& row > -1 && row < _locations.MaxRows
					&& lev > -1 && lev < _locations.MaxLevs)
				{
					return _tiles[_locations.GetLocationId(row, col, lev)];
				}
				return null;
			}
			set
			{
				_tiles[_locations.GetLocationId(row, col, lev)] = value;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates a MapTileList object.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="levs"></param>
		internal MapTileList(int rows, int cols, int levs)
		{
			_tiles     = new MapTile[rows * cols * levs];
			_locations = new MapLocations(rows, cols, levs);
		}
		#endregion cTor
	}
}
