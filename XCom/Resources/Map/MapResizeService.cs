using XCom.Base;


namespace XCom
{
	public static class MapResizeService
	{
		public enum MapResizeZtype
		{
			/// <summary>
			/// a simple addition or subtraction of z-levels (increase/decrease)
			/// </summary>
			MRZT_BOT, // 0

			/// <summary>
			/// this needs to create/delete levels at top and push existing
			/// levels down/up
			/// </summary>
			MRZT_TOP  // 1
		}


		/// <summary>
		/// Gets a resized tilelist when changing a Map's dimensions.
		/// </summary>
		/// <param name="cols">cols for the new Map</param>
		/// <param name="rows">rows for the new Map</param>
		/// <param name="levs">levs for the new Map</param>
		/// <param name="size">dimensions of the current Map</param>
		/// <param name="tileList">list of Tiles in the current Map</param>
		/// <param name="zType">MRZT_TOP to add or subtract delta-levels
		/// starting at the top level, MRZT_BOT to add or subtract delta-levels
		/// starting at the ground level - but only if a height difference is
		/// found for either case</param>
		/// <returns>the resized MapTileList</returns>
		internal static MapTileList GetResizedTileList(
				int cols,
				int rows,
				int levs,
				MapSize size,
				MapTileList tileList,
				MapResizeZtype zType)
		{
			if (   cols > 0
				&& rows > 0
				&& levs > 0)
			{
				var tileListOut = new MapTileList(cols, rows, levs);

				for (int lev = 0; lev != levs; ++lev)
				for (int row = 0; row != rows; ++row)
				for (int col = 0; col != cols; ++col)
					tileListOut[col, row, lev] = MapTile.VacantTile;

				switch (zType)
				{
					case MapResizeZtype.MRZT_BOT:
					{
						for (int lev = 0; lev != levs && lev != size.Levs; ++lev)
						for (int row = 0; row != rows && row != size.Rows; ++row)
						for (int col = 0; col != cols && col != size.Cols; ++col)
						{
							tileListOut[col, row, lev] = tileList[col, row, lev];
						}
						break;
					}

					case MapResizeZtype.MRZT_TOP:
					{
						int levels0 = size.Levs - 1;
						int levels1 = levs      - 1;

						for (int lev = 0; lev != levs && lev != size.Levs; ++lev)
						for (int row = 0; row != rows && row != size.Rows; ++row)
						for (int col = 0; col != cols && col != size.Cols; ++col)
						{
							tileListOut[col, row, levels1 - lev] = // copy tiles from bot to top.
							tileList   [col, row, levels0 - lev];
						}
						break;
					}
				}
				return tileListOut;
			}
			return null;
		}
	}
}
