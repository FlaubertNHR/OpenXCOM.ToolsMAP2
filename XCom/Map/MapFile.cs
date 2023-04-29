using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	#region Delegates
	public delegate void LocationSelectedEvent(LocationSelectedArgs args);	// -> fxCop ca1009 - wants (object sender, EventArgs e)
	public delegate void LevelSelectedEvent(LevelSelectedArgs args);		// -> ditto
	#endregion Delegates


	/// <summary>
	/// This is the currently loaded Map and associated stuff for it.
	/// </summary>
	public sealed class MapFile
	{
		#region Events
		public event LocationSelectedEvent LocationSelected;
		public event LevelSelectedEvent LevelSelected;
		#endregion Events


		#region Fields (static)
		/// <summary>
		/// The maximum count of <c><see cref="McdRecord">McdRecords</see></c>
		/// that a Mapfile can cope with.
		/// </summary>
		/// <seealso cref="MaxTerrainId"><c>MaxTerrainId</c></seealso>
		public const int MAX_MCDRECORDS = 254;

		/// <summary>
		/// The highest id of an <c><see cref="McdRecord"/></c> that a Mapfile
		/// can cope with.
		/// </summary>
		/// <seealso cref="MAX_MCDRECORDS"><c>MAX_MCDRECORDS</c></seealso>
		public const int MaxTerrainId = 253;


		// bitwise changes for MapResize()
		private const int MAPRESIZERESULT_NONE             = 0; // changed not
		public  const int MAPRESIZERESULT_CHANGEDMAP       = 1; // changed Map
		public  const int MAPRESIZERESULT_CHANGEDROUTES    = 2; // changed Routes
		public  const int MAPRESIZERESULT_DELETEROUTENODES = 4; // changed Routes and user chose to delete Oob nodes

		public const int LEVEL_Dn = +1;
		public const int LEVEL_no =  0;
		public const int LEVEL_Up = -1;

		/// <summary>
		/// The tilepart-id that's stored in a .MAP file's array of tilepart-ids
		/// accounts for the fact that ids #0 and #1 will be assigned the
		/// so-called Blank MCD records (the scorched earth parts that are
		/// instantiated when x-com itself loads). MapView, however, subtracts
		/// the count of blanks-records to assign the id-values in the
		/// <c><see cref="Parts">Parts</see></c> list; terrain- and
		/// terrainset-ids are easier to cope with that way. The part-ids will
		/// then be incremented again by the blanks-count when a .MAP file gets
		/// saved. Put another way #0 and #1 are reserved, by x-com but not by
		/// MapView, for the 2 BLANKS tiles.
		/// </summary>
		private const int BlanksReservedCount = 2;

		/// <summary>
		/// <c>true</c> to bypass <c>RouteControl.OnPaint()</c>.
		/// </summary>
		/// <remarks>If the following conditions are true .NET will try to
		/// draw a level that no longer exists.
		/// <list type="bullet">
		/// <item><c><see cref="MapResize()">MapResize()</see></c> cuts level(s)
		/// off the bottom of the Map</item>
		/// <item>a <c><see cref="RouteNode"/></c> is on a level that gets cut
		/// off</item>
		/// <item><c><see cref="Level"/></c> is at that level</item>
		/// </list><br/><br/>
		/// Note that .NET's undesired call to <c>OnPaint()</c> is caused by the
		/// call to
		/// <c><see cref="RouteCheckService.CheckNodeBounds()">RouteCheckService.CheckNodeBounds()</see></c>.</remarks>
		public static bool BypassRoutePaint;
		#endregion Fields (static)


		#region Fields
		private string _pfe;
		#endregion Fields


		#region Properties
		/// <summary>
		/// The <c><see cref="XCom.Descriptor"/></c> holds all of the metadata
		/// about this <c>MapFile</c>.
		/// </summary>
		public Descriptor Descriptor
		{ get; private set; }

		/// <summary>
		/// A <c><see cref="MapTileArray"/></c> of all
		/// <c><see cref="MapTile">MapTiles</see></c> in this <c>MapFile</c>.
		/// </summary>
		/// <remarks>A <c>MapTile</c> contains pointers to any
		/// <c><see cref="RouteNode"/></c> as well as to any
		/// <c><see cref="Tilepart">Tileparts</see></c>
		/// <list type="bullet">
		/// <item><c><see cref="MapTile.Floor"/></c></item>
		/// <item><c><see cref="MapTile.West"/></c></item>
		/// <item><c><see cref="MapTile.North"/></c></item>
		/// <item><c><see cref="MapTile.Content"/></c></item>
		/// </list></remarks>
		public MapTileArray Tiles
		{ get; private set; }

		/// <summary>
		/// A <c>List</c> of all <c><see cref="Tilepart">Tileparts</see></c>
		/// that are available to build this <c>MapFile</c> with.
		/// </summary>
		public IList<Tilepart> Parts
		{ get; private set; }

		/// <summary>
		/// An array that holds the count of parts in each terrain allocated to
		/// this <c>MapFile</c>.
		/// </summary>
		/// <remarks>Used by the <c>TerrainSwapDialog</c>.</remarks>
		public int[] PartCounts
		{ get; internal set; }

		/// <summary>
		/// The Map's terrainset.
		/// </summary>
		/// <remarks>A Map must have at least one terrain allocated. Ironically
		/// a Map's terrain(s) are not referenced by a Mapfile; each Map needs
		/// a separate configuration file that tells it what terrain(s) to use.
		/// In MapView2 that file is "settings/MapTilesets.yml".</remarks>
		public Dictionary<int, Tuple<string,string>> Terrains
		{ get; private set; }

		public RouteNodes Routes
		{ get; set; }


		public int Cols
		{ get; private set; }
		public int Rows
		{ get; private set; }
		public int Levs
		{ get; private set; }

		private int _level;
		/// <summary>
		/// Gets/Sets the currently selected level.
		/// WARNING: Level 0 is the top level of the displayed Map.
		/// </summary>
		/// <remarks>Setting the level will fire the
		/// <c><see cref="LevelSelected"/></c> event.</remarks>
		public int Level
		{
			get { return _level; } // TODO: why is Level distinct from Location.Lev - why is Location.Lev not even set by Level
			set
			{
				_level = Math.Max(0, Math.Min(value, Levs - 1));

				if (LevelSelected != null)
					LevelSelected(new LevelSelectedArgs(_level));
			}
		}

		private MapLocation _location;
		/// <summary>
		/// Gets/Sets the currently selected location.
		/// </summary>
		/// <remarks>Setting the location will fire the
		/// <c><see cref="LocationSelected"/></c> event.</remarks>
		public MapLocation Location
		{
			get { return _location; }
			set
			{
				if (   value.Col > -1 && value.Col < Cols
					&& value.Row > -1 && value.Row < Rows)
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
		/// User will be shown a dialog asking to save if the Map changed.
		/// </summary>
		/// <remarks>The setter must be mediated by <c>MainViewF.MapChanged</c>
		/// in order to apply/remove an asterisk to/from the file-label in
		/// MainView's statusbar.</remarks>
		public bool MapChanged
		{ get; set; }

		/// <summary>
		/// User will be shown a dialog asking to save if the Routes changed.
		/// </summary>
		/// <remarks>The setter must be mediated by <c>RouteView.RoutesChanged</c>
		/// in order to enable/disable <c>RouteView.bu_Save</c>.</remarks>
		public bool RoutesChanged
		{ get; set; }

		public int TerrainsetCountExceeded
		{ get; set; }

		/// <summary>
		/// Set <c>true</c> if a crippled tile was deleted and MainView needs to
		/// reload the current tileset.
		/// </summary>
		public bool ForceReload
		{ get; set; }

		internal bool Fail
		{ get; private set; }

		/// <summary>
		/// Gets the Map's dimensions as a string to print in MainView's
		/// statusbar.
		/// </summary>
		public string SizeString
		{
			get { return Cols + ", " + Rows + ", " + Levs; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="parts">the list of parts in all allocated terrains
		/// (the terrainset)</param>
		/// <param name="routes"></param>
		/// <remarks>Instantiated by
		/// <c><see cref="MapFileService.LoadDescriptor()">MapFileService.LoadDescriptor()</see></c></remarks>
		internal MapFile(
				Descriptor descriptor,
				IList<Tilepart> parts,
				RouteNodes routes)
		{
			string dir = Path.Combine(descriptor.Basepath, GlobalsXC.MapsDir);
			string pfe = Path.Combine(dir, descriptor.Label + GlobalsXC.MapExt);

			if (LoadMapfile(pfe, parts))
			{
				_pfe = pfe;

				Descriptor = descriptor;
				Terrains   = descriptor.Terrains;
				Parts      = parts;
				Routes     = routes;

				SetupRouteNodes();
				CalculateOccultations();
			}
			else
				Fail = true;
		}
		#endregion cTor


		#region Methods (read/load)
		/// <summary>
		/// Reads a .MAP Mapfile.
		/// </summary>
		/// <param name="pfe">path-file-extension of a Mapfile</param>
		/// <param name="parts">a list of
		/// <c><see cref="Tilepart">Tileparts</see></c></param>
		/// <returns><c>true</c> if read okay</returns>
		private bool LoadMapfile(string pfe, IList<Tilepart> parts)
		{
			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			{
				Rows = fs.ReadByte(); // http://www.ufopaedia.org/index.php/MAPS
				Cols = fs.ReadByte(); // - says this header is "height, width and depth (in that order)"
				Levs = fs.ReadByte(); //   ie. y/x/z

				Tiles = new MapTileArray(Cols, Rows, Levs);

				for (int lev = 0; lev != Levs; ++lev) // z-axis (top to bot)
				for (int row = 0; row != Rows; ++row) // y-axis
				for (int col = 0; col != Cols; ++col) // x-axis
				{
					Tiles.SetTile(col, row, lev, CreateTile(
														parts,
														fs.ReadByte(),		// floor setid
														fs.ReadByte(),		// westwall setid
														fs.ReadByte(),		// northwall setid
														fs.ReadByte()));	// content setid
				}

				if (TerrainsetCountExceeded != 0)
				{
					string head = Infobox.SplitString("Partids detected in the Mapfile that exceed"
													+ " the bounds of the allocated terrainset.", 80);

					using (var f = new Infobox(
											"Warning",
											head,
											GetCopyableWarning(),
											InfoboxType.Warn))
					{
						f.ShowDialog();
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Creates a <c><see cref="MapTile"/></c> with its four parts.
		/// </summary>
		/// <param name="parts">a <c><see cref="Tilepart"/></c> list that can be used</param>
		/// <param name="id_floor">the floor setid</param>
		/// <param name="id_west">the westwall setid</param>
		/// <param name="id_north">the northwall setid</param>
		/// <param name="id_content">the content setid</param>
		/// <returns>the <c>MapTile</c> created</returns>
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
				floor = new Tilepart((int)PartType.Floor, id_floor);
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
				west = new Tilepart((int)PartType.West, id_west);
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
				north = new Tilepart((int)PartType.North, id_north);
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
				content = new Tilepart((int)PartType.Content, id_content);
				content.Cripple(PartType.Content);
				++TerrainsetCountExceeded;
			}

			return new MapTile(
							floor,
							west,
							north,
							content);
		}
		#endregion Methods (read/load)


		#region Methods (static)
		/// <summary>
		/// Writes default Map and blank Route files.
		/// </summary>
		/// <param name="pfeMap">path-file-extension of the Mapfile to create</param>
		/// <param name="pfeRoutes">path-file-extension of the Routefile to
		/// create</param>
		/// <returns><c>true</c> on success</returns>
		/// <remarks>Call this funct only if the Mapfile does *not* exist. This
		/// funct does *not* create backup files!</remarks>
		public static bool CreateDefault(string pfeMap, string pfeRoutes)
		{
			using (var fs = FileService.CreateFile(pfeMap)) // create a default Mapfile and release its handle
			if (fs != null)
			{
				fs.WriteByte((byte)10); // rows // default new Map size ->
				fs.WriteByte((byte)10); // cols
				fs.WriteByte((byte) 1); // levs

				for (int r = 0; r != 10; ++r)
				for (int c = 0; c != 10; ++c)
				{
					fs.WriteByte((byte)0);
					fs.WriteByte((byte)0);
					fs.WriteByte((byte)0);
					fs.WriteByte((byte)0);
				}

				using (var fsRoutes = FileService.CreateFile(pfeRoutes)) // create a blank Routefile and release its handle
				{}

				return true; // don't worry too much about successful creation of the Routefile.
			}
			return false;
		}
		#endregion Methods (static)


		#region Methods (save/write)
		/// <summary>
		/// Saves this <c>MapFile</c>.
		/// </summary>
		/// <returns><c>true</c> on success</returns>
		public bool SaveMap()
		{
			return WriteMapfile(_pfe);
		}

		/// <summary>
		/// Exports this <c>MapFile</c> to a different file.
		/// </summary>
		/// <param name="pf">path-file w/out extension</param>
		public void ExportMap(string pf)
		{
			WriteMapfile(pf + GlobalsXC.MapExt);
		}

		/// <summary>
		/// Writes this <c>MapFile</c> to a given path.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns><c>true</c> on success</returns>
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

				fs.WriteByte((byte)Rows); // http://www.ufopaedia.org/index.php/MAPS
				fs.WriteByte((byte)Cols); // - says this header is "height, width and depth (in that order)"
				fs.WriteByte((byte)Levs); //   ie. y/x/z

				// NOTE: User is disallowed from placing any tilepart with an id
				// greater than MapFile.MaxTerrainId.

				// TODO: Ask user before NOT writing crippled partids.

				MapTile tile;
				for (int lev = 0; lev != Levs; ++lev) // z-axis (top to bot)
				for (int row = 0; row != Rows; ++row) // y-axis
				for (int col = 0; col != Cols; ++col) // x-axis
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
		/// Writes the id of a specified <c><see cref="Tilepart"/></c> to a
		/// specified <c>FileStream</c>.
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
		/// Saves <c><see cref="Routes"/></c> to the current Routefile.
		/// </summary>
		/// <returns><c>true</c> on success</returns>
		public bool SaveRoutes()
		{
			return Routes.SaveRoutes();
		}

		/// <summary>
		/// Exports the Routes to a different Routefile.
		/// </summary>
		/// <param name="pf">path-file w/out extension</param>
		public void ExportRoutes(string pf)
		{
			Routes.ExportRoutes(pf + GlobalsXC.RouteExt);
		}
		#endregion Methods (save/write)


		#region Methods (routenodes)
		/// <summary>
		/// Assigns <c><see cref="RouteNode">RouteNodes</see></c> to
		/// <c><see cref="MapTile">MapTiles</see></c> when this <c>MapFile</c>
		/// is instantiated or for a
		/// <c><see cref="MapResize()">MapResize()</see></c> or a
		/// <c>RouteView.OnImportClick()</c>.
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
		/// Clears all <c><see cref="RouteNode">RouteNodes</see></c> for
		/// <c>RouteView.OnImportClick()</c> or for a
		/// <c><see cref="MapResize()">MapResize()</see></c>.
		/// </summary>
		public void ClearRouteNodes()
		{
			for (int lev = 0; lev != Levs; ++lev)
			for (int row = 0; row != Rows; ++row)
			for (int col = 0; col != Cols; ++col)
			{
				Tiles.GetTile(col, row, lev).Node = null;
			}
		}

		/// <summary>
		/// Adds a <c><see cref="RouteNode"/></c> to a
		/// <c><see cref="MapTile"/></c> at a given
		/// <c><see cref="MapLocation"/></c>.
		/// </summary>
		/// <param name="location">a <c>MapLocation</c></param>
		/// <returns>the <c>RouteNode</c></returns>
		public RouteNode AddRouteNode(MapLocation location)
		{
			return Tiles.GetTile(location.Col,
								 location.Row,
								 location.Lev).Node = Routes.AddNode(location);
		}
		#endregion Methods (routenodes)


		#region Methods (terrain)
		/// <summary>
		/// Gets the terrain-label of a given <c><see cref="Tilepart"/></c>.
		/// </summary>
		/// <param name="tilepart"></param>
		/// <returns><c>null</c> if not found</returns>
		public string GetTerrainLabel(Tilepart tilepart)
		{
			int id = -1;
			foreach (var part in Parts)
			{
				if (part.Id == 0)
					++id;

				if (part == tilepart)
					break;
			}

			if (id != -1 && id < Terrains.Count)
				return Terrains[id].Item1;

			return null;
		}

		/// <summary>
		/// Gets the terrain of a given <c><see cref="Tilepart"/></c>.
		/// </summary>
		/// <param name="tilepart"></param>
		/// <returns><c>null</c> if not found</returns>
		public Tuple<string,string> GetTerrain(Tilepart tilepart)
		{
			int id = -1;
			foreach (var part in Parts)
			{
				if (part.Id == 0)
					++id;

				if (part == tilepart)
					break;
			}

			if (id != -1 && id < Terrains.Count)
				return Terrains[id];

			return null;
		}
		#endregion Methods (terrain)


		#region Methods
		/// <summary>
		/// Gets a <c><see cref="MapTile"/></c> using col,row,lev values.
		/// </summary>
		/// <param name="col">x-position</param>
		/// <param name="row">y-position</param>
		/// <param name="lev">z-position</param>
		/// <returns>the <c>MapTile</c></returns>
		public MapTile GetTile(int col, int row, int lev)
		{
			return Tiles.GetTile(col, row, lev);
		}

		/// <summary>
		/// Gets a <c><see cref="MapTile"/></c> at the current level using
		/// col,row values.
		/// </summary>
		/// <param name="col">x-position</param>
		/// <param name="row">y-position</param>
		/// <returns>the <c>MapTile</c></returns>
		/// <remarks>z-position is assumed to be the currently displayed
		/// <c><see cref="Level"/></c>.</remarks>
		public MapTile GetTile(int col, int row)
		{
			return Tiles.GetTile(col, row, Level);
		}


		/// <summary>
		/// Changes the view-level and fires the
		/// <c><see cref="LevelSelected"/></c> event.
		/// </summary>
		/// <param name="dir">+1 is down, -1 is up</param>
		public void ChangeLevel(int dir)
		{
			switch (dir)
			{
				case LEVEL_Dn:
					if (Level != Levs - 1)
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
		/// <param name="floorsdisabled"><c>true</c> to force visibility for all
		/// tiles</param>
		public void CalculateOccultations(bool floorsdisabled = false)
		{
			if (Levs > 1) // NOTE: Maps shall be at least 10x10x1 ...
			{
				if (!floorsdisabled)
				{
					for (int lev = Levs - 1; lev != 0; --lev)
					for (int row = 0; row != Rows - 2; ++row)
					for (int col = 0; col != Cols - 2; ++col)
					{
						Tiles.GetTile(col, row, lev).Occulted = Tiles.GetTile(col,     row,     lev - 1).Floor != null // above

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
				else
				{
					for (int lev = Levs - 1; lev != 0; --lev)
					for (int row = 0; row != Rows - 2; ++row)
					for (int col = 0; col != Cols - 2; ++col)
					{
						Tiles.GetTile(col, row, lev).Occulted = false;
					}
				}
			}
		}


		/// <summary>
		/// Gets the copyable text that is displayed in an
		/// <c><see cref="Infobox"/></c> when this <c>MapFile</c> has
		/// tilepart-ids that exceed the count of parts in its terrainset.
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
		#endregion Methods


		#region Methods (resize)
		/// <summary>
		/// Resizes the current Map.
		/// </summary>
		/// <param name="cols">cols for the new Map</param>
		/// <param name="rows">rows for the new Map</param>
		/// <param name="levs">levs for the new Map</param>
		/// <param name="zType"><c><see cref="MapResizeZtype.MRZT_TOP">MapResizeZtype.MRZT_TOP</see></c>
		/// to add or subtract delta-levels starting at the top level or
		/// <c><see cref="MapResizeZtype.MRZT_BOT">MapResizeZtype.MRZT_BOT</see></c>
		/// to add or subtract delta-levels starting at the ground level - but
		/// only if a height difference is found for either case</param>
		/// <returns>a bitwise int of changes
		/// <list type="bullet">
		/// <item><c><see cref="MAPRESIZERESULT_NONE"/></c> - no change</item>
		/// <item><c><see cref="MAPRESIZERESULT_CHANGEDMAP"/></c> - Map changed</item>
		/// <item><c><see cref="MAPRESIZERESULT_CHANGEDROUTES"/></c> - Routes changed</item>
		/// <item><c><see cref="MAPRESIZERESULT_DELETEROUTENODES"/></c> - Routes changed and user chose to delete Oob nodes</item>
		/// </list></returns>
		public int MapResize(
				int cols,
				int rows,
				int levs,
				MapResizeZtype zType)
		{
			int ret = MAPRESIZERESULT_NONE;

			MapTileArray tiles = MapResizeService.ResizeTileArray(
																cols, rows, levs,
																Cols, Rows, Levs,
																Tiles,
																zType);
			if (tiles != null)
			{
				ret |= MAPRESIZERESULT_CHANGEDMAP;

				if (zType == MapResizeZtype.MRZT_TOP // adjust route-nodes ->
					&& Routes.Any())
				{
					ret |= MAPRESIZERESULT_CHANGEDROUTES;

					int delta = (levs - Levs);	// NOTE: Map levels are inverted so adding or subtracting levels
												// to the top needs to push any existing node-levels down or up.
					foreach (RouteNode node in Routes)
					{
						if (node.Lev < 128) // allow nodes that are OoB to come back into view ->
						{
							if ((node.Lev += delta) < 0)	// NOTE: node x/y/z are stored as bytes.
								node.Lev += 256;			// -> ie. level -1 = level 255
						}
						else if ((node.Lev += delta - 256) < 0)	// nodes above the highest Maplevel maintain
							node.Lev += 256;					// their relative z-level
					}
				}

				Cols = cols;
				Rows = rows;
				Levs = levs;

				Tiles = tiles;

				BypassRoutePaint = true;
				if (RouteCheckService.CheckNodeBounds(this) == DialogResult.Yes)
					ret |= MAPRESIZERESULT_DELETEROUTENODES;
				BypassRoutePaint = false;

				ClearRouteNodes();
				SetupRouteNodes();

				Level = 0; // fire LevelSelected event
			}
			return ret;
		}
		#endregion Methods (resize)
	}
}
