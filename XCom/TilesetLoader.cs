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
		private readonly List<Tileset> _tilesets = new List<Tileset>();
		internal List<Tileset> Tilesets
		{
			get { return _tilesets; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Reads MapTilesets.yml and imports all its data to a Tileset-
		/// object. After a list of Tilesets is created it is sent to
		/// TileGroupManager.LoadTilesets() which converts all the tilesets into
		/// Descriptors.
		/// </summary>
		/// <param name="fullpath">path-file-extension of settings/MapTilesets.yml</param>
		public TilesetLoader(string fullpath)
		{
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
					progress.SetText("Parsing YAML ...");
					progress.SetTotal(typeCount);


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
							}
						}

						// get the BasePath of the tileset
						if (keyvals.ContainsKey(keyBasepath))
							basepath = keyvals[keyBasepath].ToString();
						else
							basepath = String.Empty;

						// get the BypassRecordsExceeded bool
						bypassRe = keyvals.ContainsKey(keyBypassRe)
								&& keyvals[keyBypassRe].ToString().ToLowerInvariant() == "true";


						// IMPORTANT: ensure that tileset-labels (ie, types) and
						// terrain-labels (ie, terrains) are stored and used as
						// UpperCASE strings only.

						Tilesets.Add(new Tileset(
											keyvals[keyLabel].ToString().ToUpperInvariant(),
											keyvals[keyGroup].ToString(),
											keyvals[keyCategory].ToString(),
											terrainset,
											basepath,
											bypassRe));

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
