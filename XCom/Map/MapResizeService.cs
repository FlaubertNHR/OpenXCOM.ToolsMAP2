namespace XCom
{
	public enum MapResizeZtype
	{
		/// <summary>
		/// a simple addition or subtraction of z-levels (increase/decrease)
		/// </summary>
		MRZT_BOT, // 0

		/// <summary>
		/// this needs to create/delete levels at top and push existing
		/// levels down or pull them upward
		/// </summary>
		MRZT_TOP  // 1
	}


	public static class MapResizeService
	{
		/// <summary>
		/// Gets a resized tile-array when changing a Map's dimensions.
		/// </summary>
		/// <param name="cols">cols for the new Map</param>
		/// <param name="rows">rows for the new Map</param>
		/// <param name="levs">levs for the new Map</param>
		/// <param name="size">dimensions of the current Map</param>
		/// <param name="tiles0">array of Tiles in the current tileset</param>
		/// <param name="zType">MRZT_TOP to add or subtract delta-levels
		/// starting at the top level, MRZT_BOT to add or subtract delta-levels
		/// starting at the ground level - but only if a height difference is
		/// found for either case</param>
		/// <returns>a resized MapTileArray or null</returns>
		internal static MapTileArray GetTileArray(
				int cols,
				int rows,
				int levs,
				MapSize size,
				MapTileArray tiles0,
				MapResizeZtype zType)
		{
			if (cols > 0 && rows > 0 && levs > 0)
			{
				var tiles1 = new MapTileArray(cols, rows, levs);

				for (int lev = 0; lev != levs; ++lev)
				for (int row = 0; row != rows; ++row)
				for (int col = 0; col != cols; ++col)
					tiles1.SetTile(col, row, lev, new MapTile());

				switch (zType)
				{
					case MapResizeZtype.MRZT_BOT:
					{
						for (int lev = 0; lev != levs && lev != size.Levs; ++lev)
						for (int row = 0; row != rows && row != size.Rows; ++row)
						for (int col = 0; col != cols && col != size.Cols; ++col)
						{
							tiles1.SetTile(col, row, lev, tiles0.GetTile(col, row, lev));
						}
						break;
					}

					case MapResizeZtype.MRZT_TOP:
					{
						int levels0 = size.Levs - 1;
						int levels1 =      levs - 1;

						for (int lev = 0; lev != levs && lev != size.Levs; ++lev)
						for (int row = 0; row != rows && row != size.Rows; ++row)
						for (int col = 0; col != cols && col != size.Cols; ++col)
						{
							tiles1.SetTile(col, row, levels1 - lev, // copy tiles from bot to top.
							tiles0.GetTile(col, row, levels0 - lev));
						}
						break;
					}
				}
				return tiles1;
			}
			return null;
		}
	}
}
