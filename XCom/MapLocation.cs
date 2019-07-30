using System;


namespace XCom
{
	/// <summary>
	/// A container for 3d Map coordinates.
	/// TODO: override Equals, ==, != (see 'MapSize')
	/// </summary>
	public class MapLocation
	{
		public int Row
		{ get; private set; }

		public int Col
		{ get; private set; }

		public int Lev
		{ get; set; }


		/// <summary>
		/// cTor. Constructs a MapLocation vector.
		/// TODO: Switch row/col so things are x/y instead of y/x.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		public MapLocation(int row, int col, int lev)
		{
			Row = row;
			Col = col;
			Lev = lev;
		}
	}
}
