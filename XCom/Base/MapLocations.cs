namespace XCom.Base
{
	internal sealed class MapLocations
	{
		#region Properties
		/// <summary>
		/// Gets the total columns.
		/// </summary>
		private readonly int _cols;
		internal int MaxCols
		{
			get { return _cols; }
		}

		/// <summary>
		/// Gets the total rows.
		/// </summary>
		private readonly int _rows;
		internal int MaxRows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the total levels.
		/// </summary>
		private readonly int _levs;
		internal int MaxLevs
		{
			get { return _levs; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Constructs a MapLocations object.
		/// </summary>
		/// <param name="cols">width</param>
		/// <param name="rows">height</param>
		/// <param name="levs">depth</param>
		internal MapLocations(int cols, int rows, int levs)
		{
			_cols = cols;
			_rows = rows;
			_levs = levs;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Gets the Id of a specified location.
		/// </summary>
		/// <param name="col">x-position</param>
		/// <param name="row">y-position</param>
		/// <param name="lev">z-position</param>
		/// <returns></returns>
		internal int GetLocationId(int col, int row, int lev)
		{
			return col + (row * _cols) + (lev * _cols * _rows);
		}
		#endregion Methods
	}
}
