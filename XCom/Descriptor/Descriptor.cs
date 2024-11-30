using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DSShared;


namespace XCom
{
	/// <summary>
	/// A <c>Descriptor</c> describes a tileset: a Map, its route-nodes, and
	/// terrain(s). It also holds the path to its files' parent directory.
	/// </summary>
	/// <remarks>A <c>Descriptor</c> is accessed *only* through a
	/// <c><see cref="TileGroup"/></c> and
	/// <c><see cref="TileGroup.Categories">TileGroup.Category</see></c> and is
	/// identified by its <c><see cref="Label"/></c>. This allows multiple
	/// tilesets - ie. with the same <c>Label</c> - to be configured differently
	/// according to <c>Category</c> and <c>TileGroup</c>.</remarks>
	public sealed class Descriptor // *snap*
	{
		#region Fields
		/// <summary>
		/// The Configurator's terrain-path for UFO or TFTD - depends on
		/// <c><see cref="GroupType"/></c>.
		/// </summary>
		private readonly string _dirTerr;
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

		/// <summary>
		/// <c><see cref="GroupType.Ufo">GroupType.Ufo</see></c> or
		/// <c><see cref="GroupType.Tftd">GroupType.Tftd</see></c>.
		/// </summary>
		/// <remarks>Mv2 Policy is to check for TFTD and if not TFTD then
		/// default to UFO.</remarks>
		public GroupType GroupType
		{ get; private set; }

		/// <summary>
		/// The <c><see cref="Palette"/> used to create sprites for this
		/// tileset.</c>
		/// <list type="bullet">
		/// <item><c><see cref="Palette.UfoBattle">Palette.UfoBattle</see></c></item>
		/// <item><c><see cref="Palette.TftdBattle">Palette.TftdBattle</see></c></item>
		/// </list>
		/// </summary>
		/// <remarks>Mv2 Policy is to check for TFTD and if not TFTD then
		/// default to UFO.</remarks>
		public Palette Pal
		{ get; private set; }


		/// <summary>
		/// <c>true</c> if a Mapfile for this <c>Descriptor</c> exists on the
		/// hardrive.
		/// </summary>
		/// <remarks>Used to color text on the Maptree nodes.</remarks>
		public bool FileValid
		{ get; set; }

        /// <summary>
        /// <c>true</c> if a MAP Mapfile for this <c>Descriptor</c> exists on the
        /// hardrive.
        /// </summary>
        /// <remarks>Used to color text on the Maptree nodes.</remarks>
        public bool IsMAP
        { get; set; }

        public bool BypassRecordsExceeded
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label">the label of this tileset</param>
		/// <param name="basepath">the parent directory of the Map and Routes</param>
		/// <param name="terrains">a <c>Dictionary</c> of terrains</param>
		/// <param name="groupType"><c><see cref="GroupType.Ufo">GroupType.Ufo</see></c>
		/// or <c><see cref="GroupType.Tftd">GroupType.Tftd</see></c></param>
		/// <param name="bypassRecordsExceeded"><c>true</c> to not issue a
		/// warning if the terrainset exceeds
		/// <c><see cref="MapFile.MaxMcdRecords">MapFile.MaxMcdRecords</see></c>
		/// parts</param>
		public Descriptor(
				string label,
				string basepath,
				Dictionary<int, Tuple<string,string>> terrains,
				GroupType groupType,
				bool bypassRecordsExceeded)
		{
			//Logfile.Log("Descriptor..cTor label= " + label);

			Label    = label;
			Basepath = basepath;
			Terrains = terrains;

			if ((GroupType = groupType) == GroupType.Tftd)
			{
				Pal = Palette.TftdBattle;
				_dirTerr = SharedSpace.ResourceDirectoryTftd;
			}
			else // GroupType.Ufo <- default
			{
				Pal = Palette.UfoBattle;
				_dirTerr = SharedSpace.ResourceDirectoryUfo;
			}

			if ((_dirTerr = SharedSpace.GetShareString(_dirTerr)) != null)
			{
				_dirTerr = Path.Combine(_dirTerr, GlobalsXC.TerrainDir);
			}
			else // the Share can return null if the resource-type is notconfigured
				_dirTerr = String.Empty;
            FileValid = (GetMapfilePath() != null) || (GetMap2filePath() != null);
			IsMAP = (GetMapfilePath() != null);

            BypassRecordsExceeded = bypassRecordsExceeded;
		}
        #endregion cTor


        #region Methods
        /// <summary>
        /// Gets the fullpath to the Mapfile with MAP extension for this <c>Descriptor</c>.
        /// </summary>
        /// <returns>the path to the Mapfile MAP else <c>null</c></returns>
        public string GetMapfilePath()
		{
			if (!String.IsNullOrEmpty(Basepath)) // the BasePath can be null if resource-type is notconfigured.
			{
				string pfe = Path.Combine(Basepath, GlobalsXC.MapsDir);
					   pfe = Path.Combine(pfe, Label + GlobalsXC.MapExt);

				if (File.Exists(pfe))
					return pfe;
			}
			return null;
		}

        /// <summary>
        /// Gets the fullpath to the Mapfile with MAP2 extension for this <c>Descriptor</c>.
        /// </summary>
        /// <returns>the path to the Mapfile MAP2 else <c>null</c></returns>
        public string GetMap2filePath()
        {
            if (!String.IsNullOrEmpty(Basepath)) // the BasePath can be null if resource-type is notconfigured.
            {
                string pfe = Path.Combine(Basepath, GlobalsXC.MapsDir);
                pfe = Path.Combine(pfe, Label + GlobalsXC.MapExt2);

                if (File.Exists(pfe))
                    return pfe;
            }
            return null;
        }

        /// <summary>
        /// Gets the appropriate TERRAIN directory for this tileset.
        /// </summary>
        /// <param name="path">value2 of the Tuple in the
        /// <c><see cref="Terrains"/></c> property</param>
        /// <returns>the actual TERRAIN directory for this tileset</returns>
        public string GetTerrainDirectory(string path)
		{
			if (String.IsNullOrEmpty(path))								// use Configurator's basepath
				return _dirTerr;

			if (path == GlobalsXC.BASEPATH)								// use this Tileset's basepath
				return Path.Combine(Basepath, GlobalsXC.TerrainDir);

			return Path.Combine(path, GlobalsXC.TerrainDir);			// use the path specified.
		}

		/// <summary>
		/// Gets the path_file without extension of a specified terrain-id.
		/// </summary>
		/// <param name="terid">the id of a terrain in
		/// <c><see cref="Terrains"/></c></param>
		/// <returns>the path_file string</returns>
		/// <remarks>Used when copying tiles in <c>MainViewOverlay</c>.</remarks>
		public string GetTerrainPathfile(int terid)
		{
			return Path.Combine(GetTerrainDirectory(Terrains[terid].Item2),
													Terrains[terid].Item1);
		}

		/// <summary>
		/// Creates the <c><see cref="McdRecord">McdRecords</see></c> and
		/// <c><see cref="Spriteset"/></c> for a given terrain in this
		/// <c>Descriptor</c> and returns an array of
		/// <c><see cref="Tilepart">Tileparts</see></c>.
		/// </summary>
		/// <param name="terid">the id of the terrain in this tileset's
		/// <c><see cref="Terrains"/></c> <c>Dictionary</c>.</param>
		/// <returns>an array containing the <c>Tileparts</c> for the terrain,
		/// or <c>null</c> if creating the <c>Spriteset</c> fails</returns>
		/// <remarks>The TabwordLength of terrains in UFO and TFTD is 2-bytes.</remarks>
		internal Tilepart[] CreateTerrain(int terid)
		{
			Tuple<string,string> terrain = Terrains[terid];
			string ter = terrain.Item1;
			string dir = GetTerrainDirectory(terrain.Item2);

			Spriteset spriteset = SpritesetManager.CreateSpriteset( // a pointer to the spriteset shall be stored in 'SpriteManager.Spritesets'
																ter,
																dir,
																Pal,
																true);
			if (spriteset != null)
			{
				//Logfile.Log(". spriteset Valid - create tileparts");
				return TilepartFactory.CreateTileparts(ter, dir, terid, true);
			}

			//Logfile.Log(". spriteset NOT Valid - ret null");
			return null;
		}

		/// <summary>
		/// Gets the count of <c><see cref="McdRecord">McdRecords</see></c> in
		/// an MCD-file.
		/// </summary>
		/// <param name="id">the position of the terrain in this tileset's
		/// <c><see cref="Terrains"/></c> dictionary</param>
		/// <param name="disregard"><c>true</c> to disregard any error</param>
		/// <returns>count of <c>McdRecords</c> or <c>0</c> on fail</returns>
		/// <remarks>It's a bit funky to have to read from disk just to get the
		/// count of records but at present there is no general cache of all
		/// available terrains; even this <c>Descriptor</c> retains only the
		/// allocated terrains as tuples in the <c><see cref="Terrains"/></c>
		/// dictionary. See <c><see cref="SpritesetManager"/></c> - where all
		/// *sprites* of a terrain *are* cached.</remarks>
		public int GetRecordCount(int id, bool disregard = false)
		{
			Tuple<string,string> terrain = Terrains[id];
			string ter = terrain.Item1;
			string dir = GetTerrainDirectory(terrain.Item2);

			using (var fs = FileService.OpenFile(
											Path.Combine(dir, ter + GlobalsXC.McdExt),
											disregard))
			if (fs != null)
				return (int)fs.Length / McdRecord.Length; // TODO: Error if this don't work out right.

			return 0;
		}
		#endregion Methods


		#region Methods (override)
		/// <summary>
		/// Returns the <c><see cref="Label"/></c> of this <c>Descriptor</c>.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Overrides <c>Object.ToString()</c>.</remarks>
		public override string ToString()
		{
			return Label;
		}
		#endregion Methods (override)
	}
}
