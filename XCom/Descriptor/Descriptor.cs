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
		private readonly string _dirTerr; // the Configurator's terrain-path for UFO or TFTD - depends on GroupType.
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

		public GameType GroupType
		{ get; private set; }

		public Palette Pal
		{ get; private set; }


		public bool BypassRecordsExceeded
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="basepath"></param>
		/// <param name="terrains"></param>
		/// <param name="groupType">UFO or TFTD</param>
		/// <param name="bypassRecordsExceeded"></param>
		public Descriptor(
				string label,
				string basepath,
				Dictionary<int, Tuple<string,string>> terrains,
				GameType groupType,
				bool bypassRecordsExceeded)
		{
			Label    = label;
			Basepath = basepath;
			Terrains = terrains;

			switch (GroupType = groupType)
			{
				default:
//				case GameType.Ufo:
					Pal = Palette.UfoBattle;
					_dirTerr = SharedSpace.ResourceDirectoryUfo;
					break;

				case GameType.Tftd:
					Pal = Palette.TftdBattle;
					_dirTerr = SharedSpace.ResourceDirectoryTftd;
					break;
			}

			if ((_dirTerr = SharedSpace.GetShareString(_dirTerr)) != null)
			{
				_dirTerr = Path.Combine(_dirTerr, GlobalsXC.TerrainDir);
			}
			else  // NOTE: the Share can return null if the resource-type is notconfigured
				_dirTerr = String.Empty;

			BypassRecordsExceeded = bypassRecordsExceeded;
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

			SpriteCollection spriteset = ResourceInfo.LoadSpriteset(
																terr, path,
																ResourceInfo.TAB_WORD_LENGTH_2,
																Pal);
			if (spriteset != null)
				return TilepartFactory.CreateTileparts(terr, path, spriteset);

			return null;
		}

		/// <summary>
		/// Gets the count of MCD-records in an MCD-file.
		/// @note It's funky to read from disk just to get the count of records
		/// but at present there is no general cache of all available terrains;
		/// even a Map's Descriptor retains only the allocated terrains as
		/// tuples in a dictionary-object.
		/// See ResourceInfo - where the *sprites* of a terrain *are* cached.
		/// </summary>
		/// <param name="id">the position of the terrain in this tileset's terrains-list</param>
		/// <param name="disregard">true to disregard any error</param>
		/// <returns>count of MCD-records or 0 on fail</returns>
		public int GetRecordCount(int id, bool disregard = false)
		{
			var terrain = Terrains[id];
			string terr = terrain.Item1;
			string path = terrain.Item2;

			path = GetTerrainDirectory(path);

			using (var fs = FileService.OpenFile(
											Path.Combine(path, terr + GlobalsXC.McdExt),
											disregard))
			if (fs != null)
				return (int)fs.Length / McdRecord.Length; // TODO: Error if this don't work out right.

			return 0;
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
