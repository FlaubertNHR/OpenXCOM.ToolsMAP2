using XCom.Base;


namespace XCom
{
	public static class MapResizeService
	{
		public enum MapResizeZtype
		{
			MRZT_BOT,	// 0 - a simple addition or subtraction of z-levels (increase/decrease)
			MRZT_TOP	// 1 - this needs to create/delete levels at top and push existing levels down/up
		}


		internal static MapTileList GetResizedTileList(
				int cols,
				int rows,
				int levs,
				MapSize sizePre,
				MapTileList tileListPre,
				MapResizeZtype zType)
		{
			if (   cols > 0
				&& rows > 0
				&& levs > 0)
			{
				var tileListPost = new MapTileList(cols, rows, levs);

				for (int lev = 0; lev != levs; ++lev)
				for (int row = 0; row != rows; ++row)
				for (int col = 0; col != cols; ++col)
					tileListPost[col, row, lev] = MapTile.VacantTile;

				switch (zType)
				{
					case MapResizeZtype.MRZT_BOT:
					{
						for (int lev = 0; lev != levs && lev != sizePre.Levs; ++lev)
						for (int row = 0; row != rows && row != sizePre.Rows; ++row)
						for (int col = 0; col != cols && col != sizePre.Cols; ++col)
						{
							tileListPost[col, row, lev] = tileListPre[col, row, lev];
						}
						break;
					}

					case MapResizeZtype.MRZT_TOP:
					{
						int levelsPre  = sizePre.Levs - 1;
						int levelsPost = levs - 1;

						for (int lev = 0; lev != levs && lev != sizePre.Levs; ++lev)
						for (int row = 0; row != rows && row != sizePre.Rows; ++row)
						for (int col = 0; col != cols && col != sizePre.Cols; ++col)
						{
							tileListPost[col, row, levelsPost - lev] = // copy tiles from bot to top.
							tileListPre [col, row, levelsPre  - lev];
						}
						break;
					}
				}
				return tileListPost;
			}
			return null;
		}
	}
}
