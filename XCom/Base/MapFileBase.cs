using System;
using System.Collections.Generic;

using XCom;


namespace XCom.Base
{
	/// <summary>
	/// This is basically the currently loaded Map.
	/// </summary>
	public abstract class MapFileBase
	{
		#region Delegates
		public delegate void SelectLocationEvent(SelectLocationEventArgs e);
		public delegate void SelectLevelEvent(SelectLevelEventArgs e);
		#endregion Delegates


		#region Events
		public event SelectLocationEvent SelectLocation;
		public event SelectLevelEvent SelectLevel;
		#endregion Events


		#region Fields (static)
		public const int MaxTerrainId = 253; // cf. MapFileService.MAX_MCDRECORDS=254

		/// <summary>
		/// A bitwise int of changes for MapResize:
		/// 	0 - no changes
		/// 	1 - Map changed
		/// 	2 - Routes changed
		/// </summary>
		public const int CHANGED_NOT = 0;
		public const int CHANGED_MAP = 1;
		public const int CHANGED_NOD = 2;

		public const int LEVEL_Dn = +1;
		public const int LEVEL_no =  0;
		public const int LEVEL_Up = -1;
		#endregion Fields (static)


		#region Properties
		public Descriptor Descriptor
		{ get; internal protected set; }

		public List<Tilepart> Parts
		{ get; internal protected set; }

		public MapTileList Tiles
		{ get; internal protected set; }


		private int _level;
		/// <summary>
		/// Gets/Sets the currently selected level.
		/// @note Setting the level will fire the SelectLevel event.
		/// WARNING: Level 0 is the top level of the displayed Map.
		/// </summary>
		public int Level // TODO: why is Level distinct from Location.Lev - why is Location.Lev not even set by Level
		{
			get { return _level; }
			set
			{
				_level = Math.Max(0, Math.Min(value, MapSize.Levs - 1));

				if (SelectLevel != null)
					SelectLevel(new SelectLevelEventArgs(_level));
			}
		}

		private MapLocation _location;
		/// <summary>
		/// Gets/Sets the currently selected location.
		/// @note Setting the location will fire the SelectLocation event.
		/// </summary>
		public MapLocation Location
		{
			get { return _location; }
			set
			{
				if (   value.Col > -1 && value.Col < MapSize.Cols
					&& value.Row > -1 && value.Row < MapSize.Rows)
				{
					_location = value;

					if (SelectLocation != null)
						SelectLocation(new SelectLocationEventArgs(
																_location,
																this[_location.Col,
																	 _location.Row]));
				}
			}
		}

		/// <summary>
		/// Gets the current size of the Map.
		/// </summary>
		public MapSize MapSize
		{ get; internal protected set; }

		/// <summary>
		/// Gets/Sets a MapTile object using col,row,lev values.
		/// @note No error checking is done to ensure that the given location is
		/// valid.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <returns>the corresponding MapTile object</returns>
		public MapTile this[int col, int row, int lev]
		{
			get
			{
				if (Tiles != null) // TODO: Get rid of that.
					return Tiles[col, row, lev];
				return null;
			}
			set { Tiles[col, row, lev] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile object at the current level using col,row
		/// values.
		/// @note No error checking is done to ensure that the given location is
		/// valid.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <returns>the corresponding MapTile object</returns>
		public MapTile this[int col, int row]
		{
			get { return this[col, row, Level]; }
			set { this[col, row, Level] = value; }
		}

//		/// <summary>
//		/// Gets/Sets a MapTile object using a MapLocation.
//		/// @note No error checking is done to ensure that the given location is
//		/// valid.
//		/// </summary>
//		public MapTile this[MapLocation loc]
//		{
//			get { return this[loc.Col, loc.Row, loc.Lev]; }
//			set { this[loc.Col, loc.Row, loc.Lev] = value; }
//		}

		/// <summary>
		/// User will be shown a dialog asking to save if the Map changed.
		/// @note The setter must be mediated by MainViewF.MapChanged in order
		/// to apply/remove an asterisk to/from the file-label in MainView's
		/// statusbar.
		/// </summary>
		public bool MapChanged
		{ get; set; }

		/// <summary>
		/// User will be shown a dialog asking to save if the Routes changed.
		/// @note The setter must be mediated by RouteView.RoutesChanged in
		/// order to show/hide a "routes changed" label to/from 'pnlDataFields'
		/// in RouteView.
		/// </summary>
		public bool RoutesChanged
		{ get; set; }

		public bool FunkyPartidsDetected
		{ get; internal protected set; }
		#endregion Properties


		#region Methods (abstract)
		public abstract bool SaveMap();
		public abstract void ExportMap(string pf);

		public abstract bool SaveRoutes();
		public abstract void ExportRoutes(string pf);

		public abstract int MapResize(
				int cols,
				int rows,
				int levs,
				MapResizeService.MapResizeZtype zType);
		#endregion Methods (abstract)


		#region Methods
		/// <summary>
		/// Changes the view-level and fires the SelectLevel event.
		/// </summary>
		/// <param name="dir">+1 is down, -1 is up</param>
		public void ChangeLevel(int dir)
		{
			switch (dir)
			{
				case LEVEL_Dn:
					if (Level != MapSize.Levs - 1)
						++Level;
					break;

				case LEVEL_Up:
					if (Level != 0)
						--Level;
					break;
			}
		}

		/// <summary>
		/// Generates occultation data for all tiles in the Map.
		/// </summary>
		/// <param name="forceVis">true to force visibility</param>
		public void CalculateOccultations(bool forceVis = false)
		{
			if (MapSize.Levs > 1) // NOTE: Maps shall be at least 10x10x1 ...
			{
				MapTile tile;

				for (int lev = MapSize.Levs - 1; lev != 0; --lev)
				for (int row = 0; row != MapSize.Rows - 2; ++row)
				for (int col = 0; col != MapSize.Cols - 2; ++col)
				{
					if ((tile = this[col, row, lev]) != null) // safety. The tile should always be valid.
					{
						tile.Occulted = !forceVis
									 && this[col,     row,     lev - 1].Floor != null // above

									 && this[col,     row + 1, lev - 1].Floor != null // south
									 && this[col,     row + 2, lev - 1].Floor != null

									 && this[col + 1, row,     lev - 1].Floor != null // east
									 && this[col + 2, row,     lev - 1].Floor != null

									 && this[col + 1, row + 1, lev - 1].Floor != null // southeast
									 && this[col + 2, row + 1, lev - 1].Floor != null
									 && this[col + 1, row + 2, lev - 1].Floor != null
									 && this[col + 2, row + 2, lev - 1].Floor != null;
					}
				}
			}
		}
		#endregion Methods
	}
}
