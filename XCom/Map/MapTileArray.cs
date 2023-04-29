using System;
using System.Collections.Generic;


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
		/// <param name="cols">x-count</param>
		/// <param name="rows">y-count</param>
		/// <param name="levs">z-count</param>
		internal MapTileArray(int cols, int rows, int levs)
		{
			_tiles = new MapTile[(_cols = cols)
							   * (_rows = rows)
							   * (_levs = levs)];
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Gets the <c><see cref="MapTile"/></c> at a specified location.
		/// </summary>
		/// <param name="col">x-position</param>
		/// <param name="row">y-position</param>
		/// <param name="lev">z-position</param>
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
		/// <param name="col">x-position</param>
		/// <param name="row">y-position</param>
		/// <param name="lev">z-position</param>
		/// <param name="tile">a <c>MapTile</c> to place at
		/// <paramref name="col"/>/<paramref name="row"/>/<paramref name="lev"/></param>
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
