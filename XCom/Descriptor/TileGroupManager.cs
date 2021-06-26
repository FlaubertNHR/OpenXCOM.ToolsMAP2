using System;
using System.Collections.Generic;
using System.IO;

using DSShared;
using DSShared.Controls;

using YamlDotNet.RepresentationModel;


namespace XCom
{
	#region Enums
	public enum GameType
	{
		Ufo,
		Tftd
	}
	#endregion Enums



	/// <summary>
	/// Manages <c><see cref="TileGroup">TileGroups</see></c>, loads the
	/// tilesets into <c><see cref="Descriptor">Descriptors</see></c>, and
	/// writes "settings/MapTilesets.yml".
	/// </summary>
	public static class TileGroupManager
	{
		#region Fields (static)
		private const string PrePad       = "#----- ";
		private const int    PrePadLength = 7;
		#endregion Fields (static)


		#region Properties (static)
		private static readonly Dictionary<string, TileGroup> _tilegroups =
							new Dictionary<string, TileGroup>();
		public static Dictionary<string, TileGroup> TileGroups
		{
			get { return _tilegroups; }
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Adds a <c><see cref="TileGroup"/></c>. Called by
		/// <c>MainViewF.OnAddGroupClick()</c>.
		/// </summary>
		/// <param name="labelGroup">the label of the group to add</param>
		/// <remarks>Check if the group already exists first.</remarks>
		public static void AddTileGroup(string labelGroup)
		{
			TileGroups[labelGroup] = new TileGroup(labelGroup);
		}

		/// <summary>
		/// Deletes a <c><see cref="TileGroup"/></c>. Called by
		/// <c>MainViewF.OnDeleteGroupClick()</c>.
		/// </summary>
		/// <param name="labelGroup">the label of the group to delete</param>
		public static void DeleteTileGroup(string labelGroup)
		{
			TileGroups.Remove(labelGroup);
		}

		/// <summary>
		/// Creates a new <c><see cref="TileGroup"/></c> and transfers ownership
		/// of all
		/// <c><see cref="TileGroup.Categories">TileGroup.Categories</see></c>
		/// and <c><see cref="Descriptor">Descriptors</see></c> from their
		/// previous <c>TileGroup</c> to the specified new <c>TileGroup</c>.
		/// Called by <c>MainViewF.OnEditGroupClick()</c>.
		/// </summary>
		/// <param name="labelGroup">the new label for the <c>TileGroup</c></param>
		/// <param name="labelGroup0">the old label of the <c>TileGroup</c></param>
		/// <remarks>Check if the <c>TileGroup</c> and <c>Category</c> already
		/// exist first.</remarks>
		public static void EditTileGroup(string labelGroup, string labelGroup0)
		{
			TileGroups[labelGroup] = new TileGroup(labelGroup);
			//or, AddTileGroup(labelGroup);

			foreach (var labelCategory in TileGroups[labelGroup0].Categories.Keys)
			{
				TileGroups[labelGroup].AddCategory(labelCategory);

				foreach (var descriptor in TileGroups[labelGroup0].Categories[labelCategory].Values)
				{
					TileGroups[labelGroup].Categories[labelCategory][descriptor.Label] = descriptor;
				}
			}
			DeleteTileGroup(labelGroup0);
		}


		/// <summary>
		/// Reads "settings/MapTilesets.yml" and converts all its data to
		/// <c><see cref="Descriptor">Descriptors</see></c>.
		/// </summary>
		/// <param name="fullpath">path-file-extension of
		/// "settings/MapTilesets.yml"</param>
		public static void LoadTileGroups(string fullpath)
		{
			//Logfile.Log();
			//Logfile.Log("TileGroupManager.LoadTileGroups()");

			using (var fs = FileService.OpenFile(fullpath))
			if (fs != null)
			using (var sr = new StreamReader(fs))
			{
				var progress = new ProgressBarF("parsing Tilesets ...");
				progress.Refresh();
				int typeCount = 0;

				string line; // fuck this shit.
				while ((line = sr.ReadLine()) != null)
				{
					if (line.StartsWith("  - type:", StringComparison.Ordinal))
						++typeCount;
				}
				progress.SetTotal(typeCount);
				progress.SetText("loading Tilesets ...");

				fs.Position = 0;
				sr.DiscardBufferedData();


				var str = new YamlStream();
				str.Load(sr);

				IList<YamlDocument> docs = str.Documents;
				if (docs != null && docs.Count != 0)
				{
					string terr, path, basepath;
					bool bypassRe;

					var keyGroup    = new YamlScalarNode(GlobalsXC.GROUP);
					var keyCategory = new YamlScalarNode(GlobalsXC.CATEGORY);
					var keyLabel    = new YamlScalarNode(GlobalsXC.TYPE);
					var keyTerrains = new YamlScalarNode(GlobalsXC.TERRAINS);
					var keyBasepath = new YamlScalarNode(GlobalsXC.BASEPATH);
					var keyBypassRe = new YamlScalarNode(GlobalsXC.BYPASSRE);

					YamlSequenceNode terrains;
					YamlScalarNode   terrainTry1;
					YamlMappingNode  terrainTry2;

					Dictionary<int, Tuple<string,string>> terrainset;

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
//					foreach (var node in nodeRoot.Children) // parses YAML document divisions, ie "---"
//					{
					IDictionary<YamlNode,YamlNode> keyvals;

					var tilesets = nodeRoot.Children[new YamlScalarNode(GlobalsXC.TILESETS)] as YamlSequenceNode;
					foreach (YamlMappingNode tileset in tilesets) // iterate over all the tilesets
					{
						//Logfile.Log();

						keyvals = tileset.Children;

						// get the Terrains of the tileset ->
						terrainset = new Dictionary<int, Tuple<string,string>>();

						terrains = keyvals[keyTerrains] as YamlSequenceNode;
						if (terrains != null)
						{
							for (int i = 0; i != terrains.Children.Count; ++i)
							{
								terr = path = null; // NOTE: 'path' will *not* be appended w/ "TERRAIN" yet.

								terrainTry1 = terrains[i] as YamlScalarNode;
								if (terrainTry1 != null) // ie. ':' not found. Use Configurator basepath ...
								{
									terr = terrainTry1.ToString();
									path = String.Empty;
								}
								else // has ':' + path
								{
									terrainTry2 = terrains[i] as YamlMappingNode;
									foreach (var keyval in terrainTry2.Children) // NOTE: There's only one keyval in each terrain-node.
									{
										terr = keyval.Key  .ToString();
										path = keyval.Value.ToString();
									}
								}
								terrainset[i] = new Tuple<string,string>(terr, path);
								//Logfile.Log(". terrainset[" + i + "]= " + terrainset[i]);
							}
						}

						// get the BypassRecordsExceeded bool
						bypassRe = keyvals.ContainsKey(keyBypassRe)
								&& keyvals[keyBypassRe].ToString().ToUpperInvariant() == "TRUE";


						// IMPORTANT: ensure that tileset-labels (ie, types) and
						// terrain-labels (ie, terrains) are stored and used as
						// UpperCASE strings only.

						TileGroup @group;
						string labelGroup = keyvals[keyGroup].ToString();
						//Logfile.Log(". labelGroup= " + labelGroup);
						if (!TileGroupManager.TileGroups.ContainsKey(labelGroup))
						{
							TileGroupManager.TileGroups[labelGroup] =
							@group = new TileGroup(labelGroup);
						}
						else
							@group = TileGroupManager.TileGroups[labelGroup];

						//Logfile.Log(". group= " + @group);

						string labelCategory = keyvals[keyCategory].ToString();
						//Logfile.Log(". labelCategory= " + labelCategory);
						if (!@group.Categories.ContainsKey(labelCategory))
						{
							@group.AddCategory(labelCategory);
						}

						// get the BasePath of the tileset
						if (keyvals.ContainsKey(keyBasepath))
						{
							basepath = keyvals[keyBasepath].ToString();
						}
						else // assign the Configurator's basepath to the tileset's Descriptor ->
						{
							switch (@group.GroupType)
							{
								default:
									goto case GameType.Ufo; // workaround for c#

								case GameType.Ufo:
									basepath = SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo);
									break;
								case GameType.Tftd:
									basepath = SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd);
									break;
							}
						}
						//Logfile.Log(". basepath= " + basepath);


						var descriptor = new Descriptor(
													keyvals[keyLabel].ToString().ToUpperInvariant(),
													basepath,
													terrainset,
													@group.GroupType,
													bypassRe);

						@group.AddTileset(descriptor, labelCategory);
						//or, TileGroups[labelGroup].Categories[labelCategory][keyvals[keyLabel].ToString().ToUpperInvariant()] = descriptor;

						progress.Step();
					}
//					}
				}
				progress.Close();
			}
		}

		/// <summary>
		/// Saves the <c><see cref="TileGroup">TileGroups</see></c> with their
		/// <c><see cref="TileGroup.Categories">TileGroup.Categories</see></c>
		/// and <c><see cref="Descriptor">Descriptors</see></c> aka tilesets to
		/// "settings/MapTilesets.yml".
		/// </summary>
		/// <returns><c>true</c> if no exception was thrown</returns>
		public static bool WriteTileGroups()
		{
			string dir = SharedSpace.GetShareString(SharedSpace.SettingsDirectory);	// settings
			string pfe = Path.Combine(dir, PathInfo.YML_Tilesets);					// MapTilesets.yml

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

				using (var sw = new StreamWriter(fs))
				{
					sw.WriteLine("# This is MapTilesets for MapViewII.");
					sw.WriteLine("#");
					sw.WriteLine("# 'tilesets' - a list that contains all the blocks.");
					sw.WriteLine("# 'type'     - the label of MAP/RMP files for the block.");
					sw.WriteLine("# 'terrains' - the label(s) of PCK/TAB/MCD files for the block. A terrain may be" + Environment.NewLine
							   + "#              defined in one of three formats:"                                  + Environment.NewLine
							   + "#              - LABEL"                                                           + Environment.NewLine
							   + "#              - LABEL: basepath"                                                 + Environment.NewLine
							   + "#              - LABEL: <basepath>"                                               + Environment.NewLine
							   + "#              The first gets the terrain from the Configurator's basepath. The"  + Environment.NewLine
							   + "#              second gets the terrain from the current Map's basepath. The"      + Environment.NewLine
							   + "#              third gets the terrain from the specified basepath (don't use"     + Environment.NewLine
							   + "#              quotes). A terrain must be in a subdirectory labeled TERRAIN.");
					sw.WriteLine("# 'category' - a header for the tileset, is arbitrary here.");
					sw.WriteLine("# 'group'    - a header for the categories, is arbitrary except that the first"   + Environment.NewLine
							   + "#              letters designate the game-type and must be either 'ufo' or"       + Environment.NewLine
							   + "#              'tftd' (case insensitive, with or without a following space).");
					sw.WriteLine("# 'basepath' - the path to the parent directory of the tileset's Map and Route"   + Environment.NewLine
							   + "#              files (default: the resource directory(s) that was/were specified" + Environment.NewLine
							   + "#              when MapView was installed/configured). Note that Maps are"        + Environment.NewLine
							   + "#              expected to be in a subdir called MAPS, Routes in a subdir called" + Environment.NewLine
							   + "#              ROUTES, but that terrains - PCK/TAB/MCD files - are referenced by" + Environment.NewLine
							   + "#              default in the basepath that is set by the Configurator and have"  + Environment.NewLine
							   + "#              to be in a subdir labeled TERRAIN of that path. But see"           + Environment.NewLine
							   + "#              'terrains' above.");
					sw.WriteLine(String.Empty);


					bool tilesets_written = false;

					bool blankline;
					foreach (string labelGroup in TileGroups.Keys)
					{
						var @group = TileGroups[labelGroup] as TileGroup;	// <- fuck inheritance btw. It's not being used properly and is
						if (@group.Categories.Count != 0)					// largely irrelevant and needlessly confusing in this codebase.
						{													// Relax, it's getting sorted out ... bit by bit.
							bool tileset_exists = false; // test if there's a category with a tileset ->
							foreach (var labelCategory in @group.Categories.Keys)
							{
								if (@group.Categories[labelCategory].Keys.Count != 0)
								{
									tileset_exists = true;
									break;
								}
							}

							if (tileset_exists)
							{
								if (!tilesets_written)
								{
									tilesets_written = true;
									sw.WriteLine(GlobalsXC.TILESETS + ":");
								}

								blankline = true;
								sw.WriteLine(String.Empty);
								sw.WriteLine(PrePad + labelGroup + padder(labelGroup.Length + PrePadLength));

								foreach (var labelCategory in @group.Categories.Keys)
								{
									var category = @group.Categories[labelCategory];
									if (category.Count != 0)
									{
										if (!blankline)
											sw.WriteLine(String.Empty);

										blankline = false;
										sw.WriteLine(PrePad + labelCategory + padder(labelCategory.Length + PrePadLength));

										foreach (var labelTileset in category.Keys)
										{
											var descriptor = category[labelTileset];

											sw.WriteLine("  - " + GlobalsXC.TYPE + ": " + descriptor.Label); // =labelTileset
											sw.WriteLine("    " + GlobalsXC.TERRAINS + ":");

											for (int i = 0; i != descriptor.Terrains.Count; ++i)
											{
												var terrain = descriptor.Terrains[i]; // Dictionary<int id, Tuple<string terrain, string path>>
												string terr = terrain.Item1;
												string path = terrain.Item2;
												if (!String.IsNullOrEmpty(path))
													terr += ": " + path;

												sw.WriteLine("      - " + terr);
											}

											sw.WriteLine("    " + GlobalsXC.CATEGORY + ": " + labelCategory);
											sw.WriteLine("    " + GlobalsXC.GROUP + ": " + labelGroup);

											string keyConfigPath = String.Empty;
											switch (@group.GroupType)
											{
												case GameType.Ufo:  keyConfigPath = SharedSpace.ResourceDirectoryUfo;  break;
												case GameType.Tftd: keyConfigPath = SharedSpace.ResourceDirectoryTftd; break;
											}
											string basepath = descriptor.Basepath;
											if (basepath != SharedSpace.GetShareString(keyConfigPath)) // don't write basepath if it's the (default) Configurator's basepath
												sw.WriteLine("    " + GlobalsXC.BASEPATH + ": " + basepath);


											if (descriptor.BypassRecordsExceeded)
												sw.WriteLine("    " + GlobalsXC.BYPASSRE + ": " + descriptor.BypassRecordsExceeded);
										}
									}
								}
							}
						}
					}
				}

				if (!fail && pfeT != pfe)
					return FileService.ReplaceFile(pfe);
			}
			return !fail;
		}


		/// <summary>
		/// Adds padding such as " ---#" out to 80 characters.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		private static string padder(int length)
		{
			string pad = String.Empty;
			if (length < 79) pad = " ";

			for (int i = 78; i > length; --i)
				pad += "-";

			if (length < 79) pad += "#";
			return pad;
		}
		#endregion Methods (static)
	}
}
