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
		/// <param name="cols1">cols for the new Map</param>
		/// <param name="rows1">rows for the new Map</param>
		/// <param name="levs1">levs for the new Map</param>
		/// <param name="cols0">cols of the current Map</param>
		/// <param name="rows0">rows of the current Map</param>
		/// <param name="levs0">levs of the current Map</param>
		/// <param name="tiles0">the array of Tiles in the current tileset</param>
		/// <param name="zType">MRZT_TOP to add or subtract delta-levels
		/// starting at the top level, MRZT_BOT to add or subtract delta-levels
		/// starting at the ground level - but only if a height difference is
		/// found for either case</param>
		/// <returns>a resized MapTileArray or null</returns>
		internal static MapTileArray GetTileArray(
				int cols1, int rows1, int levs1,
				int cols0, int rows0, int levs0,
				MapTileArray tiles0,
				MapResizeZtype zType)
		{
			if (cols1 > 0 && rows1 > 0 && levs1 > 0)
			{
				var tiles1 = new MapTileArray(cols1, rows1, levs1);

				for (int lev = 0; lev != levs1; ++lev)
				for (int row = 0; row != rows1; ++row)
				for (int col = 0; col != cols1; ++col)
					tiles1.SetTile(col, row, lev, new MapTile());

				switch (zType)
				{
					case MapResizeZtype.MRZT_BOT:
					{
						for (int lev = 0; lev != levs1 && lev != levs0; ++lev)
						for (int row = 0; row != rows1 && row != rows0; ++row)
						for (int col = 0; col != cols1 && col != cols0; ++col)
						{
							tiles1.SetTile(col, row, lev, tiles0.GetTile(col, row, lev));
						}
						break;
					}

					case MapResizeZtype.MRZT_TOP:
					{
						int levels0 = levs0 - 1;
						int levels1 = levs1 - 1;

						for (int lev = 0; lev != levs1 && lev != levs0; ++lev)
						for (int row = 0; row != rows1 && row != rows0; ++row)
						for (int col = 0; col != cols1 && col != cols0; ++col)
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
