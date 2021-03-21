using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	#region Delegates
	public delegate void LocationSelectedEvent(LocationSelectedArgs args);
	public delegate void LevelSelectedEvent(LevelSelectedArgs args);
	#endregion Delegates


	/// <summary>
	/// This is the currently loaded Map.
	/// </summary>
	public sealed class MapFile
	{
		#region Events
		public event LocationSelectedEvent LocationSelected;
		public event LevelSelectedEvent LevelSelected;
		#endregion Events


		#region Fields (static)
		public const int MaxTerrainId = 253; // cf. MapFileService.MAX_MCDRECORDS=254

		// bitwise changes for MapResize()
		public const int CHANGED_NOT = 0; // changed not
		public const int CHANGED_MAP = 1; // changed Map
		public const int CHANGED_NOD = 2; // changed Routes

		public const int LEVEL_Dn = +1;
		public const int LEVEL_no =  0;
		public const int LEVEL_Up = -1;

		/// <summary>
		/// The tilepart-id that's stored in a .MAP file's array of tilepart-ids
		/// accounts for the fact that ids #0 and #1 will be assigned the
		/// so-called Blank MCD records (the scorched earth parts that are
		/// instantiated when x-com itself loads). MapView, however, subtracts
		/// the count of blanks-records to assign the id-values in the
		/// <see cref="Parts">Parts</see> list; terrain- and terrainset-ids are
		/// easier to cope with that way. The part-ids will then be incremented
		/// again by the blanks-count when a .MAP file gets saved. Put another
		/// way #0 and #1 are reserved, by x-com but not by MapView, for the 2
		/// BLANKS tiles.
		/// </summary>
		private const int BlanksReservedCount = 2;
		#endregion Fields (static)


		#region Fields
		private string _pfe;
		#endregion Fields


		#region Properties
		public Descriptor Descriptor
		{ get; private set; }

		public MapTileArray Tiles
		{ get; private set; }

		public List<Tilepart> Parts
		{ get; private set; }

		public Dictionary<int, Tuple<string,string>> Terrains
		{ get; private set; }

		public RouteNodes Routes
		{ get; set; }

		private int _level;
		/// <summary>
		/// Gets/Sets the currently selected level.
		/// WARNING: Level 0 is the top level of the displayed Map.
		/// </summary>
		/// <remarks>Setting the level will fire the LevelSelected event.</remarks>
		public int Level // TODO: why is Level distinct from Location.Lev - why is Location.Lev not even set by Level
		{
			get { return _level; }
			set
			{
				_level = Math.Max(0, Math.Min(value, MapSize.Levs - 1));

				if (LevelSelected != null)
					LevelSelected(new LevelSelectedArgs(_level));
			}
		}

		private MapLocation _location;
		/// <summary>
		/// Gets/Sets the currently selected location.
		/// </summary>
		/// <remarks>Setting the location will fire the LocationSelected event.</remarks>
		public MapLocation Location
		{
			get { return _location; }
			set
			{
				if (   value.Col > -1 && value.Col < MapSize.Cols
					&& value.Row > -1 && value.Row < MapSize.Rows)
				{
					_location = value;

					if (LocationSelected != null)
						LocationSelected(new LocationSelectedArgs(
																_location,
																Tiles.GetTile(_location.Col,
																			  _location.Row,
																			   Level)));
				}
			}
		}

		/// <summary>
		/// Gets the current size of the Map.
		/// </summary>
		public MapSize MapSize
		{ get; private set; }

		/// <summary>
		/// Gets a MapTile object using col,row,lev values.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <returns>the corresponding MapTile object</returns>
		public MapTile GetTile(int col, int row, int lev)
		{
			return Tiles.GetTile(col, row, lev);
		}
		/// <summary>
		/// Gets a MapTile object at the current level using col,row values.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <returns>the corresponding MapTile object</returns>
		public MapTile GetTile(int col, int row)
		{
			return Tiles.GetTile(col, row, Level);
		}

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

		public int TerrainsetCountExceeded
		{ get; set; }

		/// <summary>
		/// Set true if a crippled tile was deleted and MainView needs to reload
		/// the Mapfile.
		/// </summary>
		public bool ForceReload
		{ get; set; }

		internal bool Fail
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="parts">the list of parts in all allocated terrains (the terrainset)</param>
		/// <param name="routes"></param>
		internal MapFile(
				Descriptor descriptor,
				List<Tilepart> parts,
				RouteNodes routes)
		{
			//LogFile.WriteLine("MapFile..cTor");

			string dir = Path.Combine(descriptor.Basepath, GlobalsXC.MapsDir);
			string pfe = Path.Combine(dir, descriptor.Label + GlobalsXC.MapExt);

			if (LoadMapfile(pfe, parts))
			{
				_pfe = pfe;

				Descriptor = descriptor;
				Terrains   = descriptor.Terrains;
				Parts      = parts;
				Routes     = routes;

				for (int i = 0; i != Parts.Count; ++i)
					Parts[i].SetId = i;

				SetupRouteNodes();
				CalculateOccultations();
			}
			else
				Fail = true;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Changes the view-level and fires the LevelSelected event.
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
//				MapTile tile;

				for (int lev = MapSize.Levs - 1; lev != 0; --lev)
				for (int row = 0; row != MapSize.Rows - 2; ++row)
				for (int col = 0; col != MapSize.Cols - 2; ++col)
				{
//					if ((tile = Tiles.GetTile(col, row, lev)) != null) // safety. The tile should always be valid.
//					{
//						tile.Occulted = !forceVis
//									 && Tiles.GetTile(col,     row,     lev - 1).Floor != null // above
//
//									 && Tiles.GetTile(col,     row + 1, lev - 1).Floor != null // south
//									 && Tiles.GetTile(col,     row + 2, lev - 1).Floor != null
//
//									 && Tiles.GetTile(col + 1, row,     lev - 1).Floor != null // east
//									 && Tiles.GetTile(col + 2, row,     lev - 1).Floor != null
//
//									 && Tiles.GetTile(col + 1, row + 1, lev - 1).Floor != null // southeast
//									 && Tiles.GetTile(col + 2, row + 1, lev - 1).Floor != null
//									 && Tiles.GetTile(col + 1, row + 2, lev - 1).Floor != null
//									 && Tiles.GetTile(col + 2, row + 2, lev - 1).Floor != null;
//					}
					Tiles.GetTile(col, row, lev).Occulted = !forceVis
														 && Tiles.GetTile(col,     row,     lev - 1).Floor != null // above

														 && Tiles.GetTile(col,     row + 1, lev - 1).Floor != null // south
														 && Tiles.GetTile(col,     row + 2, lev - 1).Floor != null

														 && Tiles.GetTile(col + 1, row,     lev - 1).Floor != null // east
														 && Tiles.GetTile(col + 2, row,     lev - 1).Floor != null

														 && Tiles.GetTile(col + 1, row + 1, lev - 1).Floor != null // southeast
														 && Tiles.GetTile(col + 2, row + 1, lev - 1).Floor != null
														 && Tiles.GetTile(col + 1, row + 2, lev - 1).Floor != null
														 && Tiles.GetTile(col + 2, row + 2, lev - 1).Floor != null;
				}
			}
		}

		/// <summary>
		/// Reads a .MAP file.
		/// </summary>
		/// <param name="pfe">path-file-extension of a Mapfile</param>
		/// <param name="parts">a list of tileparts</param>
		/// <returns>true if read okay</returns>
		private bool LoadMapfile(string pfe, IList<Tilepart> parts)
		{
			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			{
				int rows = fs.ReadByte(); // http://www.ufopaedia.org/index.php/MAPS
				int cols = fs.ReadByte(); // - says this header is "height, width and depth (in that order)"
				int levs = fs.ReadByte(); //   ie. y/x/z

				Tiles   = new MapTileArray(cols, rows, levs);
				MapSize = new MapSize(     cols, rows, levs);

				for (int lev = 0; lev != levs; ++lev) // z-axis (top to bot)
				for (int row = 0; row != rows; ++row) // y-axis
				for (int col = 0; col != cols; ++col) // x-axis
				{
					Tiles.SetTile(col, row, lev, CreateTile(
														parts,
														fs.ReadByte(),		// floor id
														fs.ReadByte(),		// westwall id
														fs.ReadByte(),		// northwall id
														fs.ReadByte()));	// content id
				}

				if (TerrainsetCountExceeded != 0)
				{
					const string label = "partids detected in the Mapfile that exceed"
									   + " the bounds of the allocated terrainset";

					using (var f = new Infobox(
											"Warning",
											label,
											GetCopyableWarning(),
											Infobox.BoxType.Warn))
					{
						f.ShowDialog();
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Creates a tile with its four parts.
		/// </summary>
		/// <param name="parts">a list of total tileparts that can be used</param>
		/// <param name="id_floor">the floor id</param>
		/// <param name="id_west">the westwall id</param>
		/// <param name="id_north">the northwall id</param>
		/// <param name="id_content">the content id</param>
		/// <returns>the MapTile created</returns>
		/// <remarks>If an id in the Mapfile exceeds the maxid of the file's
		/// terrainset a crippled tilepart will be created for it and displayed
		/// in MainView.</remarks>
		private MapTile CreateTile(
				IList<Tilepart> parts,
				int id_floor,
				int id_west,
				int id_north,
				int id_content)
		{
			Tilepart floor, west, north, content;

			// NOTE: ids will be "-1" if "read from the end of the stream".

			if (id_floor < BlanksReservedCount)
			{
				floor = null; // silently fail.
			}
			else if ((id_floor -= BlanksReservedCount) < parts.Count)
			{
				floor = parts[id_floor];
			}
			else
			{
				floor = new Tilepart(id_floor);
				floor.Cripple(PartType.Floor);
				++TerrainsetCountExceeded;
			}

			if (id_west < BlanksReservedCount)
			{
				west = null; // silently fail.
			}
			else if ((id_west -= BlanksReservedCount) < parts.Count)
			{
				west = parts[id_west];
			}
			else
			{
				west = new Tilepart(id_west);
				west.Cripple(PartType.West);
				++TerrainsetCountExceeded;
			}

			if (id_north < BlanksReservedCount)
			{
				north = null; // silently fail.
			}
			else if ((id_north -= BlanksReservedCount) < parts.Count)
			{
				north = parts[id_north];
			}
			else
			{
				north = new Tilepart(id_north);
				north.Cripple(PartType.North);
				++TerrainsetCountExceeded;
			}

			if (id_content < BlanksReservedCount)
			{
				content = null; // silently fail.
			}
			else if ((id_content -= BlanksReservedCount) < parts.Count)
			{
				content = parts[id_content];
			}
			else
			{
				content = new Tilepart(id_content);
				content.Cripple(PartType.Content);
				++TerrainsetCountExceeded;
			}

			return new MapTile(
							floor,
							west,
							north,
							content);
		}

		/// <summary>
		/// Gets the copyable text that is displayed in an Infobox when a
		/// tileset has parts that exceed the terrainset count.
		/// </summary>
		/// <returns></returns>
		private string GetCopyableWarning()
		{
			string L = Environment.NewLine;

			bool singular = (TerrainsetCountExceeded == 1);

			string copyable0 = "There " + (singular ? "is " : "are ") + TerrainsetCountExceeded + " tilepart"
							 + (singular ? String.Empty : "s") + " that exceed" + (singular ? "s" : String.Empty)
							 + " the bounds of the Map's currently allocated MCD records. "
							 + (singular ? "It" : "They") + " will be replaced by" + (singular ? " a" : String.Empty)
							 + " temporary tilepart" + (singular ? String.Empty : "s") + " and displayed on"
							 + " the Map as borked yellow sprites.";
			copyable0 = Infobox.SplitString(copyable0) + L + L;

			string copyable1 = "Note that borked parts that are in floor-slots could"
							 + " get hidden beneath valid content-parts, etc.";
			copyable1 = Infobox.SplitString(copyable1) + L + L;

			string copyable2 = "IMPORTANT: Saving the Map in its current state would forever lose"
							 + " those tilepart references. But if you know what terrain(s) have"
							 + " gone rogue they can be added to the Map's terrainset with the"
							 + " TilesetEditor. Or if you know how many records have been removed"
							 + " from the terrainset the ids of the rogue parts can be shifted"
							 + " down into a valid range.";
			copyable2 = Infobox.SplitString(copyable2) + L + L;

			string copyable3 = "It's recommended to resolve this issue immediately.";
			copyable3 = Infobox.SplitString(copyable3) + L + L;

			string copyable4 = "(a) save the Mapfile hence deleting the rogue tileparts" + L
							 + "(b) add terrains to the terrainset in the TilesetEditor" + L
							 + "(c) add tileparts to allocated terrains externally"      + L
							 + "(d) use TilepartSubstitution to shift ids down"          + L + L;

			string copyable = copyable0
							+ "TopView|Test|Test parts in tileslots" + L + L
							+ copyable1
							+ copyable2
							+ "MainView|Edit|TilepartSubstitution" + L + L
							+ copyable3
							+ copyable4
							+ "Pronto!";
			return copyable;
		}
		#endregion Methods (read/load)


		#region Methods (routenodes)
		/// <summary>
		/// Assigns route-nodes to tiles when this MapFile object is
		/// instantiated or when importing a Routes file.
		/// </summary>
		public void SetupRouteNodes()
		{
			MapTile tile;
			foreach (RouteNode node in Routes)
			{
				if ((tile = Tiles.GetTile(node.Col, node.Row, node.Lev)) != null)
					tile.Node = node;
			}
		}

		/// <summary>
		/// Clears all route-nodes before RouteView.OnImportClick or for a
		/// <see cref="MapFile.MapResize">MapFile.MapResize</see>.
		/// </summary>
		public void ClearRouteNodes()
		{
			for (int lev = 0; lev != MapSize.Levs; ++lev)
			for (int row = 0; row != MapSize.Rows; ++row)
			for (int col = 0; col != MapSize.Cols; ++col)
			{
				Tiles.GetTile(col, row, lev).Node = null;
			}
		}

		/// <summary>
		/// Adds a route-node to the map-tile at a given location.
		/// </summary>
		/// <param name="location"></param>
		/// <returns>the route-node</returns>
		public RouteNode AddRouteNode(MapLocation location)
		{
			RouteNode node = Routes.AddNode(
										(byte)location.Col,
										(byte)location.Row,
										(byte)location.Lev);

			return Tiles.GetTile((int)node.Col,
								 (int)node.Row,
									  node.Lev).Node = node;
		}
		#endregion Methods (routenodes)


		#region Methods (terrain)
		/// <summary>
		/// Gets the terrain-label of a given tile-part.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public string GetTerrainLabel(Tilepart part)
		{
			int id = -1;
			foreach (var part_ in Parts)
			{
				if (part_.TerId == 0)
					++id;

				if (part_ == part)
					break;
			}

			if (id != -1 && id < Terrains.Count)
				return Terrains[id].Item1;

			return null;
		}

		/// <summary>
		/// Gets the terrain of a given tile-part.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public Tuple<string,string> GetTerrain(Tilepart part)
		{
			int id = -1;
			foreach (var part_ in Parts)
			{
				if (part_.TerId == 0)
					++id;

				if (part_ == part)
					break;
			}

			if (id != -1 && id < Terrains.Count)
				return Terrains[id];

			return null;
		}
		#endregion Methods (terrain)


		#region Methods (static)
		/// <summary>
		/// Writes default Map and blank Route files.
		/// IMPORTANT: Call this funct only if the Mapfile does *not* exist.
		/// This funct does *not* create backup files!
		/// </summary>
		/// <param name="pfeMap"></param>
		/// <param name="pfeRoutes"></param>
		/// <returns>true on success</returns>
		public static bool CreateDefault(string pfeMap, string pfeRoutes)
		{
			using (var fs = FileService.CreateFile(pfeMap)) // create a default Map-file and release its handle.
			if (fs != null)
			{
				fs.WriteByte((byte)10); // rows // default new Map size ->
				fs.WriteByte((byte)10); // cols
				fs.WriteByte((byte) 1); // levs

				for (int r = 0; r != 10; ++r)
				for (int c = 0; c != 10; ++c)
				{
					fs.WriteByte((byte)0);
				}

				using (var fsRoutes = FileService.CreateFile(pfeRoutes)) // create a blank Route-file and release its handle.
				{}

				return true; // ie. don't worry too much about successful creation of the Routesfile.
			}
			return false;
		}
		#endregion Methods (static)


		#region Methods (save/write)
		/// <summary>
		/// Saves the current Mapfile.
		/// </summary>
		/// <returns>true on success</returns>
		public bool SaveMap()
		{
			return WriteMapfile(_pfe);
		}

		/// <summary>
		/// Exports the Map to a different file.
		/// </summary>
		/// <param name="pf">path-file w/out extension</param>
		public void ExportMap(string pf)
		{
			WriteMapfile(pf + GlobalsXC.MapExt);
		}

		/// <summary>
		/// Writes a Mapfile.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>true on success</returns>
		private bool WriteMapfile(string pfe)
		{
			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;

				fs.WriteByte((byte)MapSize.Rows); // http://www.ufopaedia.org/index.php/MAPS
				fs.WriteByte((byte)MapSize.Cols); // - says this header is "height, width and depth (in that order)"
				fs.WriteByte((byte)MapSize.Levs); //   ie. y/x/z

				// NOTE: User is disallowed from placing any tilepart with an id
				// greater than MapFile.MaxTerrainId.

				// TODO: Ask user before NOT writing crippled partids.

				MapTile tile;
				for (int lev = 0; lev != MapSize.Levs; ++lev)
				for (int row = 0; row != MapSize.Rows; ++row)
				for (int col = 0; col != MapSize.Cols; ++col)
				{
					tile = Tiles.GetTile(col, row, lev);

					WritePartId(fs, tile.Floor);
					WritePartId(fs, tile.West);
					WritePartId(fs, tile.North);
					WritePartId(fs, tile.Content);
				}
			}

			if (!fail && pfeT != pfe)
				return FileService.ReplaceFile(pfe);

			return !fail;
		}

		/// <summary>
		/// Writes a tilepart's id to the Filestream.
		/// </summary>
		/// <param name="fs"></param>
		/// <param name="part"></param>
		private void WritePartId(Stream fs, Tilepart part)
		{
			int id;

			if (part == null)
			{
				fs.WriteByte((byte)0);
			}
			else if ((id = part.SetId) >= Parts.Count) // wipe crippled part
			{
				fs.WriteByte((byte)0);
				ForceReload = true;
			}
			else if ((id += BlanksReservedCount) > (int)Byte.MaxValue) // NOTE: shall be disallowed by the edit-functs
			{
				fs.WriteByte((byte)0);
			}
			else
				fs.WriteByte((byte)id);
		}

		/// <summary>
		/// Saves the current Routefile.
		/// </summary>
		/// <returns>true on success</returns>
		public bool SaveRoutes()
		{
			return Routes.SaveRoutes();
		}

		/// <summary>
		/// Exports the routes to a different file.
		/// </summary>
		/// <param name="pf">path-file w/out extension</param>
		public void ExportRoutes(string pf)
		{
			Routes.ExportRoutes(pf + GlobalsXC.RouteExt);
		}
		#endregion Methods (save/write)


		#region Methods (resize)
		/// <summary>
		/// Resizes the current Map.
		/// </summary>
		/// <param name="cols">cols for the new Map</param>
		/// <param name="rows">rows for the new Map</param>
		/// <param name="levs">levs for the new Map</param>
		/// <param name="zType">MRZT_TOP to add or subtract delta-levels
		/// starting at the top level, MRZT_BOT to add or subtract delta-levels
		/// starting at the ground level - but only if a height difference is
		/// found for either case</param>
		/// <returns>a bitwise int of changes
		///          0x0 - no changes
		///          0x1 - Map changed
		///          0x2 - Routes changed</returns>
		public int MapResize(
				int cols,
				int rows,
				int levs,
				MapResizeZtype zType)
		{
			int bit = CHANGED_NOT;

			MapTileArray tiles = MapResizeService.GetTileArray(
															cols, rows, levs,
															MapSize,
															Tiles,
															zType);
			if (tiles != null)
			{
				bit |= CHANGED_MAP;

				if (zType == MapResizeZtype.MRZT_TOP // adjust route-nodes ->
					&& Routes.Any())
				{
					bit |= CHANGED_NOD;

					int delta = (levs - MapSize.Levs);	// NOTE: Map levels are inverted so adding or subtracting levels
														// to the top needs to push any existing node-levels down or up.
					foreach (RouteNode node in Routes)
					{
						if (node.Lev < 128) // allow nodes that are OoB to come back into view
						{
							if ((node.Lev += delta) < 0)	// NOTE: node x/y/z are stored as bytes.
								node.Lev += 256;			// -> ie. level -1 = level 255
						}
						else if ((node.Lev += delta - 256) < 0)	// nodes above the highest Maplevel maintain
							node.Lev += 256;					// their relative z-level
					}
				}

				MapSize = new MapSize(cols, rows, levs);
				Tiles = tiles;

				if (RouteCheckService.CheckNodeBounds(this) == DialogResult.Yes)
					bit |= CHANGED_NOD;

				ClearRouteNodes();
				SetupRouteNodes();

				Level = 0; // fire LevelSelected event
			}
			return bit;
		}
		#endregion Methods (resize)
	}
}
