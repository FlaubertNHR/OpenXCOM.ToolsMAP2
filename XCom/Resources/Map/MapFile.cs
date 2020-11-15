using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DSShared;

using XCom;
using XCom.Base;


namespace XCom
{
	/// <summary>
	/// This is the currently loaded Map.
	/// </summary>
	public sealed class MapFile
		:
			MapFileBase
	{
		#region Properties
		private string PfeMap
		{ get; set; }

		public Dictionary<int, Tuple<string,string>> Terrains
		{ get; private set; }

		public RouteNodeCollection Routes
		{ get; set; }

		public bool IsLoadChanged
		{ get; private set; }

		internal bool Fail
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="parts">a list of parts in all allocated terrains</param>
		/// <param name="routes"></param>
		internal MapFile(
				Descriptor descriptor,
				List<Tilepart> parts,
				RouteNodeCollection routes)
		{
			string dir = Path.Combine(descriptor.Basepath, GlobalsXC.MapsDir);
			string pfe = Path.Combine(dir, descriptor.Label + GlobalsXC.MapExt);

			if (LoadMapfile(pfe, parts))
			{
				PfeMap     = pfe;
				Descriptor = descriptor;
				Terrains   = Descriptor.Terrains;
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


		#region Methods (read/load)
		/// <summary>
		/// Reads a .MAP file.
		/// </summary>
		/// <param name="pfe">path-file-extension of a Mapfile</param>
		/// <param name="parts">a list of tileset-parts</param>
		/// <returns>true if read okay</returns>
		private bool LoadMapfile(string pfe, List<Tilepart> parts)
		{
			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			{
				int rows = fs.ReadByte(); // http://www.ufopaedia.org/index.php/MAPS
				int cols = fs.ReadByte(); // - says this header is "height, width and depth (in that order)"
				int levs = fs.ReadByte(); //   ie. y/x/z

				Tiles   = new MapTileList(cols, rows, levs);
				MapSize = new MapSize(    cols, rows, levs);

				for (int lev = 0; lev != levs; ++lev) // z-axis (inverted)
				for (int row = 0; row != rows; ++row) // y-axis
				for (int col = 0; col != cols; ++col) // x-axis
				{
					this[col, row, lev] = CreateTile(
												parts,
												fs.ReadByte(),
												fs.ReadByte(),
												fs.ReadByte(),
												fs.ReadByte());
				}
				return true;
			}
			return false;
		}


		private const int IdOffset = 2; // #0 and #1 are reserved for the 2 BLANKS tiles.

		/// <summary>
		/// Creates a tile with its four parts.
		/// </summary>
		/// <param name="parts">a list of total tileparts that can be used</param>
		/// <param name="quad1">the floor</param>
		/// <param name="quad2">the westwall</param>
		/// <param name="quad3">the northwall</param>
		/// <param name="quad4">the content</param>
		/// <returns>the MapTile created</returns>
		private MapTile CreateTile(
				IList<Tilepart> parts,
				int quad1,
				int quad2,
				int quad3,
				int quad4)
		{
			Tilepart floor, west, north, content;

			// NOTE: quads will be "-1" if "read from the end of the stream".

			if (quad1 < IdOffset)
			{
				floor = null;
			}
			else if ((quad1 -= IdOffset) < parts.Count)
			{
				floor = parts[quad1];
			}
			else
			{
				floor = null;
				if (!IsLoadChanged) ShowWarning();
			}

			if (quad2 < IdOffset)
			{
				west = null;
			}
			else if ((quad2 -= IdOffset) < parts.Count)
			{
				west = parts[quad2];
			}
			else
			{
				west = null;
				if (!IsLoadChanged) ShowWarning();
			}

			if (quad3 < IdOffset)
			{
				north = null;
			}
			else if ((quad3 -= IdOffset) < parts.Count)
			{
				north = parts[quad3];
			}
			else
			{
				north = null;
				if (!IsLoadChanged) ShowWarning();
			}

			if (quad4 < IdOffset)
			{
				content = null;
			}
			else if ((quad4 -= IdOffset) < parts.Count)
			{
				content = parts[quad4];
			}
			else
			{
				content = null;
				if (!IsLoadChanged) ShowWarning();
			}

			return new MapTile(
							floor,
							west,
							north,
							content);
		}

		/// <summary>
		/// Issues a warning if a tilepart's id overflows or underflows the
		/// total tileparts-list.
		/// </summary>
		private void ShowWarning()
		{
			IsLoadChanged = true;
			MessageBox.Show(
						"There are tileparts that exceed the bounds of the Map's"
							+ " currently allocated MCD records. They will be"
							+ " nulled so that the rest of the tileset can be"
							+ " displayed."
							+ Environment.NewLine + Environment.NewLine
							+ "WARNING: Saving the Map in its current state would"
							+ " lose those tilepart references. But if you know"
							+ " what terrain(s) have gone rogue they can be added"
							+ " to the Map's terrainset with the TilesetEditor.",
						" Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning,
						MessageBoxDefaultButton.Button1,
						0);
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
				if ((tile = this[node.Col, node.Row, node.Lev]) != null)
					tile.Node = node;
			}
		}

		/// <summary>
		/// Clears all route-nodes before importing a Routes file or when doing
		/// a MapResize.
		/// </summary>
		public void ClearRouteNodes()
		{
			for (int lev = 0; lev != MapSize.Levs; ++lev)
			for (int row = 0; row != MapSize.Rows; ++row)
			for (int col = 0; col != MapSize.Cols; ++col)
			{
				this[col, row, lev].Node = null;
			}
		}

		/// <summary>
		/// Adds a route-node to the map-tile at a given location.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public RouteNode AddRouteNode(MapLocation location)
		{
			RouteNode node = Routes.AddNode(
										(byte)location.Row,
										(byte)location.Col,
										(byte)location.Lev);

			return (this[(int)node.Col,
						 (int)node.Row,
						      node.Lev].Node = node);
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
		public override bool SaveMap()
		{
			return WriteMapfile(PfeMap);
		}

		/// <summary>
		/// Exports the Map to a different file.
		/// </summary>
		/// <param name="pf">path-file w/out extension</param>
		public override void ExportMap(string pf)
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

				int id;

				MapTile tile;

				for (int lev = 0; lev != MapSize.Levs; ++lev)
				for (int row = 0; row != MapSize.Rows; ++row)
				for (int col = 0; col != MapSize.Cols; ++col)
				{
					tile = this[col, row, lev];

					if (tile.Floor == null || (id = tile.Floor.SetId + IdOffset) > (int)byte.MaxValue)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)id);

					if (tile.West == null || (id = tile.West.SetId + IdOffset) > (int)byte.MaxValue)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)id);

					if (tile.North == null || (id = tile.North.SetId + IdOffset) > (int)byte.MaxValue)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)id);

					if (tile.Content == null || (id = tile.Content.SetId + IdOffset) > (int)byte.MaxValue)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)id);
				}
			}

			if (!fail && pfeT != pfe)
				return FileService.ReplaceFile(pfe);

			return !fail;
		}

		/// <summary>
		/// Saves the current Routefile.
		/// </summary>
		/// <returns>true on success</returns>
		public override bool SaveRoutes()
		{
			return Routes.SaveRoutes();
		}

		/// <summary>
		/// Exports the routes to a different file.
		/// </summary>
		/// <param name="pf">path-file w/out extension</param>
		public override void ExportRoutes(string pf)
		{
			Routes.ExportRoutes(pf + GlobalsXC.RouteExt);
		}
		#endregion Methods (save/write)


		#region Methods (resize)
		/// <summary>
		/// Resizes the current Map.
		/// </summary>
		/// <param name="cols">total columns in the new Map</param>
		/// <param name="rows">total rows in the new Map</param>
		/// <param name="levs">total levels in the new Map</param>
		/// <param name="zType">MRZT_TOP to add or subtract delta-levels
		/// starting at the top level, MRZT_BOT to add or subtract delta-levels
		/// starting at the ground level - but only if a height difference is
		/// found for either case</param>
		/// <returns>a bitwise int of changes
		///          0x0 - no changes
		///          0x1 - Map changed
		///          0x2 - Routes changed</returns>
		public override int MapResize(
				int cols,
				int rows,
				int levs,
				MapResizeService.MapResizeZtype zType)
		{
			int bit = CHANGED_NOT;

			var tileList = MapResizeService.GetResizedTileList(
															cols, rows, levs,
															MapSize,
															Tiles,
															zType);
			if (tileList != null)
			{
				bit |= CHANGED_MAP;

				int
					preRows = MapSize.Rows,
					preCols = MapSize.Cols,
					preLevs = MapSize.Levs;

				if (zType == MapResizeService.MapResizeZtype.MRZT_TOP // adjust route-nodes ->
					&& Routes.Any())
				{
					bit |= CHANGED_NOD;

					int delta = (levs - preLevs);	// NOTE: map levels are inverted so adding or subtracting levels
													// to the top needs to push any existing node-levels down or up.
					foreach (RouteNode node in Routes)
					{
						if (node.Lev < 128) // allow nodes that are OoB to come back into view
						{
							if ((node.Lev += delta) < 0)	// NOTE: node x/y/z are stored as bytes.
								node.Lev += 256;			// -> ie. level -1 = level 255
						}
						else
						{
							if ((node.Lev += delta - 256) < 0)	// nodes above the highest Maplevel maintain
								node.Lev += 256;				// their relative z-level
						}
					}
				}

				MapSize = new MapSize(cols, rows, levs);
				Tiles = tileList;

				if (RouteCheckService.CheckNodeBounds(this) == DialogResult.Yes)
					bit |= CHANGED_NOD;

				ClearRouteNodes();
				SetupRouteNodes();

				Level = 0; // fire SelectLevel event
			}
			return bit;
		}
		#endregion Methods (resize)
	}
}
