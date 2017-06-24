using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

//using XCom.Services;


namespace XCom.Interfaces.Base
{
	#region Delegates
	public delegate void LocationSelectedEventHandler(LocationSelectedEventArgs e);
	public delegate void LevelChangedEventHandler(LevelChangedEventArgs e);
	#endregion


	/// <summary>
	/// This is basically the currently loaded Map.
	/// </summary>
	public class MapFileBase
	{
		#region Events
		public event LocationSelectedEventHandler LocationSelectedEvent;
		public event LevelChangedEventHandler LevelChangedEvent;
		#endregion


		#region Fields (static)
		private const int HalfWidthConst  = 16;
		private const int HalfHeightConst =  8;
		#endregion


		#region Properties
		public Descriptor Descriptor
		{ get; private set; }

		private int _level;
		/// <summary>
		/// Gets this MapBase's currently displayed level.
		/// Changing level will fire a LevelChanged event.
		/// WARNING: Level 0 is the top level of the displayed Map.
		/// </summary>
		public int Level
		{
			get { return _level; }
			set
			{
				if (value < MapSize.Levs)
				{
					_level = value;

					if (LevelChangedEvent != null)
						LevelChangedEvent(new LevelChangedEventArgs(value));
				}
			}
		}

		/// <summary>
		/// User will be shown a dialog asking to save if the Map changed.
		/// </summary>
		public bool MapChanged
		{ get; set; }

		/// <summary>
		/// User will be shown a dialog asking to save if the Routes changed.
		/// </summary>
		public bool RoutesChanged
		{ get; set; }

		internal MapTileList MapTiles
		{ get; set; }

		public List<TilepartBase> Parts
		{ get; set; }

		private MapLocation _location;
		/// <summary>
		/// Gets/Sets the currently selected location. Setting the location will
		/// fire a LocationSelected event.
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
					var tile = this[_location.Row, _location.Col];
					var args = new LocationSelectedEventArgs(value, tile);

					if (LocationSelectedEvent != null)
						LocationSelectedEvent(args);
				}
			}
		}

		/// <summary>
		/// Gets the current size of the Map.
		/// </summary>
		public MapSize MapSize
		{ get; protected set; }

		/// <summary>
		/// Gets/Sets a MapTile using row,col,height values. No error checking
		/// is done to ensure that the location is valid.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col, int lev]
		{
			get { return (MapTiles != null) ? MapTiles[row, col, lev]
											: null; }
			set { MapTiles[row, col, lev] = value; }
		}
		/// <summary>
		/// Gets/Sets a MapTile at the current height using row,col values.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public MapTileBase this[int row, int col]
		{
			get { return this[row, col, Level]; }
			set { this[row, col, Level] = value; }
		}
//		/// <summary>
//		/// Gets/Sets a MapTile using a MapLocation.
//		/// </summary>
//		public MapTileBase this[MapLocation loc]
//		{
//			get { return this[loc.Row, loc.Col, loc.Lev]; }
//			set { this[loc.Row, loc.Col, loc.Lev] = value; }
//		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of MapFileChild.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="parts"></param>
		protected MapFileBase(Descriptor descriptor, List<TilepartBase> parts)
		{
			Descriptor = descriptor;
			Parts = parts;
		}
		#endregion


		#region Methods (virtual)
		public virtual void SaveMap()
		{}
		public virtual void SaveMap(string pf)
		{}

		public virtual void SaveRoutes()
		{}
		public virtual void SaveRoutes(string pf)
		{}

		public virtual void ClearMapChanged()
		{}

		public virtual void ClearRoutesChanged()
		{}

		/// <summary>
		/// Forwards the call to MapFileChild.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="levs"></param>
		/// <param name="ceiling"></param>
		public virtual void MapResize(
				int rows,
				int cols,
				int levs,
				bool ceiling)
		{}
		#endregion


		#region Methods
		/// <summary>
		/// Changes the 'Level' property and fires a LevelChanged event.
		/// </summary>
		public void LevelUp()
		{
			if (Level > 0)
			{
				--Level;

				if (LevelChangedEvent != null)
					LevelChangedEvent(new LevelChangedEventArgs(Level));
			}
		}

		/// <summary>
		/// Changes the 'Level' property and fires a LevelChanged event.
		/// </summary>
		public void LevelDown()
		{
			if (Level < MapSize.Levs - 1)
			{
				++Level;

				if (LevelChangedEvent != null)
					LevelChangedEvent(new LevelChangedEventArgs(Level));
			}
		}

		/// <summary>
		/// Not generic enough to call with custom derived classes other than
		/// MapFileChild.
		/// </summary>
		/// <param name="fullpath"></param>
		public void SaveGifFile(string fullpath)
		{
			var palette = GetFirstGroundPalette();
			if (palette == null)
				throw new ArgumentNullException("fullpath", "MapFileBase: At least 1 ground tile is required.");
			// TODO: I don't want to see 'ArgumentNullException'. Just say
			// what's wrong and save the technical details for the debugger.

			var rowcols = MapSize.Rows + MapSize.Cols;
			var bitmap = BitmapService.MakeBitmap(
												rowcols * (XCImage.SpriteWidth / 2),
												(MapSize.Levs - Level) * 24 + rowcols * 8,
												palette.ColorTable);

			var start = new Point(
								(MapSize.Rows - 1) * (XCImage.SpriteWidth / 2),
								-(24 * Level));

			int i = 0;
			if (MapTiles != null)
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
							var usedParts = this[row, col, lev].UsedTiles;
							foreach (var usedPart in usedParts)
							{
								var part = usedPart as Tilepart;
								BitmapService.Draw( // NOTE: not actually drawing anything.
												part[0].Image,
												bitmap,
												x,
												y - part.Record.TileOffset);
							}

//							XCBitmap.UpdateProgressBar(i, (MapSize.Levs - Level) * MapSize.Rows * MapSize.Cols);
						}
					}
				}
			}

			try
			{
				var rect = BitmapService.GetNontransparentRectangle(bitmap, Palette.TransparentId);
				bitmap   = BitmapService.Crop(bitmap, rect);
				bitmap.Save(fullpath, ImageFormat.Gif);
			}
			catch
			{
				bitmap.Save(fullpath, ImageFormat.Gif);
				throw;
			}
		}

		private Palette GetFirstGroundPalette()
		{
			for (int lev = 0; lev != MapSize.Levs; ++lev)
			for (int row = 0; row != MapSize.Rows; ++row)
			for (int col = 0; col != MapSize.Cols; ++col)
			{
				var tile = this[row, col, lev] as XCMapTile;
				if (tile.Ground != null)
					return tile.Ground[0].Pal;
			}
			return null;
		}
		#endregion
	}
}
