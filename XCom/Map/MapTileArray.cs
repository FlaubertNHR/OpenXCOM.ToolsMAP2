using System;


namespace XCom
{
	public sealed class MapTileArray
	{
		#region Fields
		private readonly MapTile[] _tiles;

		private readonly int _cols;
		private readonly int _rows;
		private readonly int _levs;
		#endregion Fields


		#region cTor
		/// <summary>
		/// Instantiates a <c>MapTileArray</c>.
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="rows"></param>
		/// <param name="levs"></param>
		internal MapTileArray(int cols, int rows, int levs)
		{
			_tiles = new MapTile[cols * rows * levs];

			_cols = cols;
			_rows = rows;
			_levs = levs;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Gets the <c><see cref="MapTile"/></c> at a specified location.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <returns></returns>
		public MapTile GetTile(int col, int row, int lev)
		{
			if (   col > -1 && col < _cols
				&& row > -1 && row < _rows
				&& lev > -1 && lev < _levs)
			{
				return _tiles[id(col, row, lev)];
			}
			return null;
		}

		/// <summary>
		/// Sets a specified <c><see cref="MapTile"/></c> at a specified
		/// location.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <param name="tile"></param>
		internal void SetTile(int col, int row, int lev, MapTile tile)
		{
			if (   col > -1 && col < _cols
				&& row > -1 && row < _rows
				&& lev > -1 && lev < _levs)
			{
				_tiles[id(col, row, lev)] = tile;
			}
		}

		/// <summary>
		/// Gets the ID of a specified tile location in this <c>MapTileArray</c>.
		/// </summary>
		/// <param name="col">x-position</param>
		/// <param name="row">y-position</param>
		/// <param name="lev">z-position</param>
		/// <returns></returns>
		private int id(int col, int row, int lev)
		{
			return col + (row * _cols) + (lev * _cols * _rows);
		}
		#endregion Methods
	}
}
