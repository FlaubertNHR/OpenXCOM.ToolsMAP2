using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using XCom;


namespace XCom.Base
{
	/// <summary>
	/// This is basically the currently loaded Map.
	/// </summary>
	public class MapFileBase
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
		private const int HalfWidthConst  = 16;
		private const int HalfHeightConst =  8;

		public const int MaxTerrainId = 253;

		/// <summary>
		/// A bitwise int of changes for MapResize:
		/// 	0 - no changes
		/// 	1 - Map changed
		/// 	2 - Routes changed
		/// </summary>
		public const int CHANGED_NOT = 0;
		public const int CHANGED_MAP = 1;
		public const int CHANGED_NOD = 2;
		#endregion Fields (static)


		#region Properties
		public Descriptor Descriptor
		{ get; private set; }

		internal MapTileList Tiles
		{ get; set; }

		public List<Tilepart> Parts
		{ get; internal set; }

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
				if (   value.Row > -1 && value.Row < MapSize.Rows
					&& value.Col > -1 && value.Col < MapSize.Cols)
				{
					_location = value;

					if (SelectLocation != null)
						SelectLocation(new SelectLocationEventArgs(
																_location,
																this[_location.Row,
																	 _location.Col]));
				}
			}
		}

		/// <summary>
		/// Gets the current size of the Map.
		/// </summary>
		public MapSize MapSize
		{ get; internal protected set; }

		/// <summary>
		/// Gets/Sets a MapTile object using row,col,lev values.
		/// @note No error checking is done to ensure that the given location is
		/// valid.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		/// <returns>the corresponding MapTile object</returns>
		public MapTile this[int row, int col, int lev]
		{
			get
			{
				if (Tiles != null)
					return Tiles[row, col, lev];
				return null;
			}
			set { Tiles[row, col, lev] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile object at the current level using row,col
		/// values.
		/// @note No error checking is done to ensure that the given location is
		/// valid.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns>the corresponding MapTile object</returns>
		public MapTile this[int row, int col]
		{
			get { return this[row, col, Level]; }
			set { this[row, col, Level] = value; }
		}

//		/// <summary>
//		/// Gets/Sets a MapTile object using a MapLocation.
//		/// @note No error checking is done to ensure that the given location is
//		/// valid.
//		/// </summary>
//		public MapTile this[MapLocation loc]
//		{
//			get { return this[loc.Row, loc.Col, loc.Lev]; }
//			set { this[loc.Row, loc.Col, loc.Lev] = value; }
//		}

		/// <summary>
		/// User will be shown a dialog asking to save if the Map changed.
		/// @note The setter must be mediated by XCMainWindow.MapChanged in
		/// order to apply/remove an asterisk to/from the file-label in
		/// MainView's statusbar.
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
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of MapFile.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="parts"></param>
		internal protected MapFileBase(Descriptor descriptor, List<Tilepart> parts)
		{
			Descriptor = descriptor;
			Parts = parts;
		}
		#endregion cTor


		#region Methods (virtual)
		public virtual void SaveMap()
		{}
		public virtual void SaveMap(string pf)
		{}

		public virtual void SaveRoutes()
		{}
		public virtual void SaveRoutes(string pf)
		{}

		/// <summary>
		/// Forwards the call to MapFile.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="levs"></param>
		/// <param name="zType"></param>
		/// <returns>a bitwise int of changes
		///          0x0 - no changes
		///          0x1 - Map changed
		///          0x2 - Routes changed</returns>
		public virtual int MapResize(
				int rows,
				int cols,
				int levs,
				MapResizeService.MapResizeZtype zType)
		{
			return 0x0;
		}
		#endregion Methods (virtual)


		#region Methods
		/// <summary>
		/// Changes the 'Level' property.
		/// </summary>
		public void LevelUp()
		{
			if (Level > 0)
				--Level; // fire SelectLevel
		}

		/// <summary>
		/// Changes the 'Level' property.
		/// </summary>
		public void LevelDown()
		{
			if (Level < MapSize.Levs - 1)
				++Level; // fire SelectLevel
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
					if ((tile = this[row, col, lev]) != null) // safety. The tile should always be valid.
					{
						tile.Occulted = !forceVis
									 && this[row,     col,     lev - 1].Floor != null // above

									 && this[row + 1, col,     lev - 1].Floor != null // south
									 && this[row + 2, col,     lev - 1].Floor != null

									 && this[row,     col + 1, lev - 1].Floor != null // east
									 && this[row,     col + 2, lev - 1].Floor != null

									 && this[row + 1, col + 1, lev - 1].Floor != null // southeast
									 && this[row + 1, col + 2, lev - 1].Floor != null
									 && this[row + 2, col + 1, lev - 1].Floor != null
									 && this[row + 2, col + 2, lev - 1].Floor != null;
					}
				}
			}
		}

		/// <summary>
		/// Not generic enough to call with custom derived classes other than
		/// MapFile.
		/// </summary>
		/// <param name="fullpath"></param>
		public void Screenshot(string fullpath)
		{
			var width = MapSize.Rows + MapSize.Cols;
			var b = BitmapService.CreateTransparent(
												width * (XCImage.SpriteWidth / 2),
												(MapSize.Levs - Level) * 24 + width * 8,
												Descriptor.Pal.ColorTable);

			var start = new Point(
								(MapSize.Rows - 1) * (XCImage.SpriteWidth / 2),
								-(24 * Level));

			int i = 0;
			if (Tiles != null)
			{
				for (int lev = MapSize.Levs - 1; lev >= Level; --lev)
				{
					for (int
							row = 0,
								startX = start.X,
								startY = start.Y + lev * 24;
							row != MapSize.Rows;
							++row,
								startX -= HalfWidthConst,
								startY += HalfHeightConst)
					{
						for (int
								col = 0,
									x = startX,
									y = startY;
								col != MapSize.Cols;
								++col,
									x += HalfWidthConst,
									y += HalfHeightConst,
									++i)
						{
							var parts = this[row, col, lev].UsedParts;
							foreach (var part in parts)
							{
								BitmapService.Insert(
												part[0].Sprite,
												b,
												x,
												y - part.Record.TileOffset);
							}
						}
					}
				}
			}

			try // TODO: what is this.
			{
				var rect = BitmapService.GetCloseRectangle(b, Palette.TranId);
				b = BitmapService.Crop(b, rect);

				ColorPalette p = b.Palette;
				p.Entries[Palette.TranId] = Color.Transparent;
				b.Palette = p;

				b.Save(fullpath, ImageFormat.Png);
			}
			catch // TODO: Deal with exceptions appropriately.
			{
				ColorPalette p = b.Palette;
				p.Entries[Palette.TranId] = Color.Transparent;
				b.Palette = p;

				b.Save(fullpath, ImageFormat.Png);
				throw;
			}

			if (b != null)
				b.Dispose();
		}
		#endregion Methods
	}
}
