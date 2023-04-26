using System;


namespace XCom
{
	/// <summary>
	/// A container for 3d Map coordinates.
	/// TODO: override Equals, ==, != (see 'MapSize')
	/// </summary>
	public class MapLocation
	{
		public int Col
		{ get; private set; }

		public int Row
		{ get; private set; }

		public int Lev
		{ get; set; }


		/// <summary>
		/// cTor. Constructs a <c>MapLocation</c> vector.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		public MapLocation(int col, int row, int lev)
		{
			Col = col;
			Row = row;
			Lev = lev;
		}
	}
}
