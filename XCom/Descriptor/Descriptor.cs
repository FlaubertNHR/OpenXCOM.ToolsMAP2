using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom;


namespace XCom
{
	/// <summary>
	/// Descriptors describe a tileset: a Map, its route-nodes, and terrain. It
	/// also holds the path to its files' parent directory.
	/// A descriptor is accessed *only* through a Group and Category, and is
	/// identified by its tileset-label. This allows multiple tilesets (ie. with
	/// the same label) to be configured differently according to Category and
	/// Group.
	/// </summary>
	public sealed class Descriptor // *snap*
	{
		#region Fields
		private readonly string _dirTerr; // the Configurator's terrain-path for UFO or TFTD - depends on Palette.
		#endregion Fields


		#region Properties
		public string Label
		{ get; private set; }

		public string Basepath
		{ get; internal set; }

		private Dictionary<int, Tuple<string,string>> _terrains =
			new Dictionary<int, Tuple<string,string>>();
		/// <summary>
		/// A dictionary of this tileset's terrains as IDs that keys a tuple
		/// that pairs terrain-labels with basepath-strings. A basepath-string
		/// can be blank (use config's basepath), "basepath" (use the tileset's
		/// basepath), or the basepath of any TERRAIN directory.
		/// </summary>
		public Dictionary<int, Tuple<string,string>> Terrains
		{
			get { return _terrains; }
			set { _terrains = value; }
		}

		public Palette Pal // TODO: Defining the palette in both a Descriptor and its TileGroup is redundant.
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="tileset"></param>
		/// <param name="terrains"></param>
		/// <param name="basepath"></param>
		/// <param name="palette"></param>
		public Descriptor(
				string tileset,
				Dictionary<int, Tuple<string,string>> terrains,
				string basepath,
				Palette palette)
		{
			//LogFile.WriteLine("Descriptor cTor tileset= " + tileset);
			//LogFile.WriteLine("");

			Label    = tileset;
			Terrains = terrains;
			Basepath = basepath;
			Pal      = palette;

			_dirTerr = (Pal == Palette.UfoBattle) ? SharedSpace.ResourceDirectoryUfo
												  : SharedSpace.ResourceDirectoryTftd;
			_dirTerr = SharedSpace.GetShareString(_dirTerr);
			_dirTerr = (_dirTerr != null) ? _dirTerr = Path.Combine(_dirTerr, GlobalsXC.TerrainDir)
										  : _dirTerr = String.Empty; // -> the Share can return null if the resource-type is notconfigured.
		}
		#endregion cTor


		#region Methods
		public string GetTerrainDirectory(string path)
		{
			if (String.IsNullOrEmpty(path))								// use Configurator's basepath
				return _dirTerr;

			if (path == GlobalsXC.BASEPATH)								// use this Tileset's basepath
				return Path.Combine(Basepath, GlobalsXC.TerrainDir);

			return Path.Combine(path, GlobalsXC.TerrainDir);			// use the path specified.
		}

		/// <summary>
		/// Creates the MCD-records and PCK-spriteset for a given terrain in
		/// this Descriptor and returns an array of Tileparts.
		/// @note The TabwordLength of terrains in UFO and TFTD is 2-bytes.
		/// </summary>
		/// <param name="id">the id of the terrain in this tileset's terrains-list</param>
		/// <returns>an array containing the Tileparts for the terrain, or null
		/// if spriteset creation borks</returns>
		internal Tilepart[] CreateTerrain(int id)
		{
			var terrain = Terrains[id];
			string terr = terrain.Item1;
			string path = terrain.Item2;

			path = GetTerrainDirectory(path);

			var spriteset = ResourceInfo.LoadSpriteset(
													terr, path,
													ResourceInfo.TAB_WORD_LENGTH_2,
													Pal);
			if (spriteset != null)
				return TilepartFactory.CreateTileparts(terr, path, spriteset);

			return null;
		}

		/// <summary>
		/// Gets the count of MCD-records in an MCD-file.
		/// </summary>
		/// <param name="id">the position of the terrain in this tileset's terrains-list</param>
		/// <param name="suppressError">true to suppress any error</param>
		/// <returns>count of MCD-records or 0 on fail</returns>
		public int GetRecordCount(int id, bool suppressError = false)
		{
			var terrain = Terrains[id];
			string terr = terrain.Item1;
			string path = terrain.Item2;

			path = GetTerrainDirectory(path);

			return TilepartFactory.GetRecordCount(terr, path, suppressError);
		}

/*		/// <summary>
		/// Gets the count of sprites in a given Terrain.
		/// @note Used only by MapInfoDialog.Analyze()
		/// </summary>
		/// <param name="id">the position of the terrain in this tileset's terrains-list</param>
		/// <returns>count of sprites</returns>
		public int GetSpriteCount(int id)
		{
			var terrain = Terrains[id];
			string terr = terrain.Item1;
			string path = terrain.Item2;

			path = GetTerrainDirectory(path);

			return ResourceInfo.GetSpritesetCount(terr, path, Pal);
		} */
		#endregion Methods


		#region Methods (override)
		/// <summary>
		/// Overrides Object.ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion Methods (override)
	}
}