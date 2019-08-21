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

					string nodeGroup, nodeCategory, nodeLabel, terr, path, nodeBasepath;

					YamlSequenceNode nodeTerrains;
					YamlScalarNode   nodetry1;
					YamlMappingNode  nodetry2;

					Dictionary<int, Tuple<string,string>> terrains;

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
//					foreach (var node in nodeRoot.Children) // parses YAML document divisions, ie "---"
//					{
					//LogFile.WriteLine(". node.Key(ScalarNode)= " + (YamlScalarNode)node.Key); // "tilesets"


					var nodeTilesets = nodeRoot.Children[new YamlScalarNode(GlobalsXC.TILESETS)] as YamlSequenceNode;
					foreach (YamlMappingNode nodeTileset in nodeTilesets) // iterate over all the tilesets
					{
						//LogFile.WriteLine(". . nodeTilesets= " + nodeTilesets); // lists all data in the tileset

						// IMPORTANT: ensure that tileset-labels (ie, type) and terrain-labels
						// (ie, terrains) are stored and used only as UpperCASE strings.


						// get the Group of the tileset
						nodeGroup = nodeTileset.Children[new YamlScalarNode(GlobalsXC.GROUP)].ToString();
						//LogFile.WriteLine(". . group= " + nodeGroup); // eg. "ufoShips"

						if (!Groups.Contains(nodeGroup))
							Groups.Add(nodeGroup);


						// get the Category of the tileset ->
						nodeCategory = nodeTileset.Children[new YamlScalarNode(GlobalsXC.CATEGORY)].ToString();
						//LogFile.WriteLine(". . category= " + nodeCategory); // eg. "Ufo"


						// get the Label of the tileset ->
						nodeLabel = nodeTileset.Children[new YamlScalarNode(GlobalsXC.TYPE)].ToString();
						nodeLabel = nodeLabel.ToUpperInvariant();
						//LogFile.WriteLine("\n. . type= " + nodeLabel); // eg. "UFO_110"


						// get the Terrains of the tileset ->
						terrains = new Dictionary<int, Tuple<string,string>>();

						nodeTerrains = nodeTileset.Children[new YamlScalarNode(GlobalsXC.TERRAINS)] as YamlSequenceNode;
						if (nodeTerrains != null)
						{
							for (int i = 0; i != nodeTerrains.Children.Count; ++i)
							{
								terr = null;
								path = null; // NOTE: 'path' will *not* be appended w/ "TERRAIN" here.

								nodetry1 = nodeTerrains[i] as YamlScalarNode;
								//LogFile.WriteLine(". . . nodetry1= " + nodetry1); // eg. "U_EXT02"

								if (nodetry1 != null) // ie. ':' not found. Use Configurator basepath ...
								{
									terr = nodetry1.ToString();
									path = String.Empty;
								}
								else // has ':' + path
								{
									nodetry2 = nodeTerrains[i] as YamlMappingNode;
									//LogFile.WriteLine(". . . nodetry2= " + nodetry2); // eg. "{ { U_EXT02, basepath } }"

									foreach (var keyval in nodetry2.Children) // note: there's only one keyval in each terrain-node.
									{
										terr = keyval.Key.ToString();
										path = keyval.Value.ToString();
									}
								}

								//LogFile.WriteLine(". terr= " + terr);
								//LogFile.WriteLine(". path= " + path);

								terrains[i] = new Tuple<string,string>(terr, path);
							}
						}


						// get the BasePath of the tileset ->
						nodeBasepath = String.Empty;
						var basepath = new YamlScalarNode(GlobalsXC.BASEPATH);
						if (nodeTileset.Children.ContainsKey(basepath))
						{
							nodeBasepath = nodeTileset.Children[basepath].ToString();
							//LogFile.WriteLine(". . basepath= " + nodeBasepath);
						}
						//else LogFile.WriteLine(". . basepath not found.");


						var tileset = new Tileset(
												nodeLabel,
												nodeGroup,
												nodeCategory,
												terrains,
												nodeBasepath);
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
