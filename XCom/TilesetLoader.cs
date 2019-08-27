using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using YamlDotNet.RepresentationModel;


namespace XCom
{
	/// <summary>
	/// A TilesetLoader reads and loads all the tileset-data in the user-file
	/// MapTilesets.yml. It's the user-configuration for all the Maps.
	/// NOTE: Tilesets are converted into Descriptors and Tilesets are no longer
	/// used after loading is finished.
	/// </summary>
	public sealed class TilesetLoader
	{
		#region Properties
		private List<Tileset> _tilesets = new List<Tileset>();
		internal List<Tileset> Tilesets
		{
			get { return _tilesets; }
		}

		private readonly List<string> _groups = new List<string>();
		internal List<string> Groups
		{
			get { return _groups; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Reads MapTilesets.yml and imports all its data to a Tileset-
		/// object.
		/// </summary>
		/// <param name="fullpath">path-file-extension of settings/MapTilesets.yml</param>
		public TilesetLoader(string fullpath)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("TilesetLoader cTor");

			var typeCount = 0;
			using (var fs = FileService.OpenFile(fullpath))
			if (fs != null)
			using (var sr = new StreamReader(fs))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (line.StartsWith("  - type:", StringComparison.Ordinal))
						++typeCount;
				}
				fs.Position = 0;
				sr.DiscardBufferedData();

				var str = new YamlStream();
				str.Load(sr);

				var docs = str.Documents;
				if (docs != null && docs.Count != 0)
				{
					var progress = ProgressBarForm.that;
					progress.SetInfo("Parsing YAML ...");
					progress.SetTotal(typeCount);


					// mappings  - will be deserialized as Dictionary<object,object>
					// sequences - will be deserialized as List<object>
					// scalars   - will be deserialized as string

					string @group, category, label, terr, path, basepath;
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
					//LogFile.WriteLine(". node.Key(ScalarNode)= " + (YamlScalarNode)node.Key); // "tilesets"


					IDictionary<YamlNode, YamlNode> keyvals;

					var nodeTilesets = nodeRoot.Children[new YamlScalarNode(GlobalsXC.TILESETS)] as YamlSequenceNode;
					foreach (YamlMappingNode nodeTileset in nodeTilesets) // iterate over all the tilesets
					{
						//LogFile.WriteLine(". . nodeTilesets= " + nodeTilesets); // lists all data in the tileset

						keyvals = nodeTileset.Children;

						// IMPORTANT: ensure that tileset-labels (ie, type) and terrain-labels
						// (ie, terrains) are stored and used only as UpperCASE strings.


						// get the Group of the tileset
						@group = keyvals[keyGroup].ToString();
						//LogFile.WriteLine(". . group= " + @group); // eg. "ufoShips"

						if (!Groups.Contains(@group))
							Groups.Add(@group);


						// get the Category of the tileset ->
						category = keyvals[keyCategory].ToString();
						//LogFile.WriteLine(". . category= " + category); // eg. "Ufo"


						// get the Label of the tileset ->
						label = keyvals[keyLabel].ToString();
						label = label.ToUpperInvariant();
						//LogFile.WriteLine("\n. . type= " + label); // eg. "UFO_110"


						// get the Terrains of the tileset ->
						terrainset = new Dictionary<int, Tuple<string,string>>();

						terrains = keyvals[keyTerrains] as YamlSequenceNode;
						if (terrains != null)
						{
							for (int i = 0; i != terrains.Children.Count; ++i)
							{
								terr = null;
								path = null; // NOTE: 'path' will *not* be appended w/ "TERRAIN" here.

								terrainTry1 = terrains[i] as YamlScalarNode;
								//LogFile.WriteLine(". . . terrainTry1= " + terrainTry1); // eg. "U_EXT02"

								if (terrainTry1 != null) // ie. ':' not found. Use Configurator basepath ...
								{
									terr = terrainTry1.ToString();
									path = String.Empty;
								}
								else // has ':' + path
								{
									terrainTry2 = terrains[i] as YamlMappingNode;
									//LogFile.WriteLine(". . . terrainTry2= " + terrainTry2); // eg. "{ { U_EXT02, basepath } }"

									foreach (var keyval in terrainTry2.Children) // note: there's only one keyval in each terrain-node.
									{
										terr = keyval.Key  .ToString();
										path = keyval.Value.ToString();
									}
								}

								//LogFile.WriteLine(". terr= " + terr);
								//LogFile.WriteLine(". path= " + path);

								terrainset[i] = new Tuple<string,string>(terr, path);
							}
						}


						// get the BasePath of the tileset ->
						if (keyvals.ContainsKey(keyBasepath))
						{
							basepath = keyvals[keyBasepath].ToString();
							//LogFile.WriteLine(". . basepath= " + basepath);
						}
						else
						{
							basepath = String.Empty;
							//LogFile.WriteLine(". . basepath not found.");
						}


						// get the BypassRecordsExceeded bool ->
						bypassRe = keyvals.ContainsKey(keyBypassRe)
								&& keyvals[keyBypassRe].ToString().ToLowerInvariant() == "true";


						var tileset = new Tileset(
												label,
												@group,
												category,
												terrainset,
												basepath,
												bypassRe);
						Tilesets.Add(tileset);

						progress.UpdateProgress();
					}
//					}
					progress.Hide();
				}
			}
		}
		#endregion cTor
	}
}
