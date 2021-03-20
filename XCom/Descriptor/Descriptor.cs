using System;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	/// <summary>
	/// Descriptors describe a tileset: a Map, its route-nodes, and terrain. It
	/// also holds the path to its files' parent directory.
	/// </summary>
	/// <remarks>A descriptor is accessed *only* through a Group and Category,
	/// and is identified by its tileset-label. This allows multiple tilesets
	/// (ie. with the same label) to be configured differently according to
	/// Category and Group.</remarks>
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

		/// <summary>
		/// A dictionary of this tileset's terrains as IDs that key a tuple that
		/// pairs terrain-labels with basepath-strings.
		/// </summary>
		/// <remarks>A basepath-string can be blank (use config's basepath),
		/// "basepath" (use the tileset's basepath), or the basepath of any
		/// TERRAIN directory.</remarks>
		public Dictionary<int, Tuple<string,string>> Terrains
		{ get; private set; }

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
		/// <param name="label">the label of this tileset</param>
		/// <param name="basepath">the parent directory of the Map and Routes</param>
		/// <param name="terrains">a dictionary of terrains</param>
		/// <param name="groupType">GameType.Ufo or GameType.Tftd</param>
		/// <param name="bypassRecordsExceeded">true to not issue a warning if
		/// the terrainset exceeds 253 parts</param>
		public Descriptor(
				string label,
				string basepath,
				Dictionary<int, Tuple<string,string>> terrains,
				GameType groupType,
				bool bypassRecordsExceeded)
		{
			//LogFile.WriteLine("Descriptor..cTor label= " + label);

			Label    = label;
			Basepath = basepath;
			Terrains = terrains;

			switch (GroupType = groupType)
			{
				case GameType.Ufo:
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
			else // NOTE: the Share can return null if the resource-type is notconfigured
				_dirTerr = String.Empty;

			BypassRecordsExceeded = bypassRecordsExceeded;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Gets the appropriate terrain-directory for this tileset.
		/// </summary>
		/// <param name="path">value2 of the Tuple in the <see cref="Terrains"/>
		/// property</param>
		/// <returns>the actual terrain-directory for this tileset</returns>
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
		/// </summary>
		/// <param name="id">the id of the terrain in this tileset's terrains-list</param>
		/// <returns>an array containing the Tileparts for the terrain, or null
		/// if spriteset creation borks</returns>
		/// <remarks>The TabwordLength of terrains in UFO and TFTD is 2-bytes.</remarks>
		internal Tilepart[] CreateTerrain(int id)
		{
			//LogFile.WriteLine("Descriptor.CreateTerrain() id= " + id);

			var terrain = Terrains[id];
			string terr = terrain.Item1;
			string path = terrain.Item2;

			path = GetTerrainDirectory(path);

			SpriteCollection spriteset = SpritesetsManager.LoadSpriteset(
																	terr,
																	path,
																	SpritesetsManager.TAB_WORD_LENGTH_2,
																	Pal);
			if (spriteset != null)
			{
				//LogFile.WriteLine(". spriteset Valid - create tileparts");
				return TilepartFactory.CreateTileparts(terr, path, spriteset);
			}

			//LogFile.WriteLine(". spriteset NOT Valid - ret null");
			return null;
		}

		/// <summary>
		/// Gets the count of MCD-records in an MCD-file.
		/// </summary>
		/// <param name="id">the position of the terrain in this tileset's terrains-list</param>
		/// <param name="disregard">true to disregard any error</param>
		/// <returns>count of MCD-records or 0 on fail</returns>
		/// <remarks>It's funky to read from disk just to get the count of
		/// records but at present there is no general cache of all available
		/// terrains; even a Map's Descriptor retains only the allocated
		/// terrains as tuples in a dictionary-object. See
		/// <see cref="SpritesetsManager"/> - where the *sprites* of a terrain
		/// *are* cached.</remarks>
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
		/// </summary>
		/// <param name="id">the position of the terrain in this tileset's terrains-list</param>
		/// <returns>count of sprites</returns>
		/// <remarks>Used only by MapInfoDialog.Analyze().</remarks>
		public int GetSpriteCount(int id)
		{
			var terrain = Terrains[id];
			string terr = terrain.Item1;
			string path = terrain.Item2;

			path = GetTerrainDirectory(path);

			return SpritesetsManager.GetSpritesetCount(terr, path, Pal);
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
