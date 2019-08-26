using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using YamlDotNet.RepresentationModel;


namespace RulesetConverter
{
	/// <summary>
	/// The Ruleset Converter.
	/// </summary>
	internal partial class RulesetConverter
		:
			Form
	{
		#region Fields
//		const string PrePad = "#----- ";
//		int PrePadLength = PrePad.Length;

//		string _dir;
//		string[] _buffer;

//		string[] _linesPaths, _linesImages, _linesMapEdit;

//		static StringComparer ignorecase = StringComparer.OrdinalIgnoreCase;
//		Dictionary<string,string> Vars     = new Dictionary<string,string>(ignorecase);
//		Dictionary<string,string> Terrains = new Dictionary<string,string>(ignorecase);
		#endregion Fields


		#region Properties
//		private List<Tileset> _tilesets = new List<Tileset>();
//		internal List<Tileset> Tilesets
//		{
//			get { return _tilesets; }
//		}

//		private readonly List<string> _groups = new List<string>();
//		internal List<string> Groups
//		{
//			get { return _groups; }
//		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates the RulesetConverter.
		/// </summary>
		internal RulesetConverter()
		{
			InitializeComponent();
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Closes the converter when the Cancel button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Opens a file browser when the find button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFindInputClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title      = "Open an OxC ruleset file ...";
				ofd.Filter     = "Ruleset files(*.rul)|*.rul|All files(*.*)|*.*";
//				ofd.DefaultExt = "";
//				ofd.FileName   = "";

				ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);


				if (ofd.ShowDialog() == DialogResult.OK)
				{
					tbInput.Text = ofd.FileName;
					btnConvert.Enabled = true;
				}
			}
		}

		/// <summary>
		/// Runs through the file parsing data into Tilesets. Then writes it to
		/// a YAML file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnConvertClick(object sender, EventArgs e)
		{
			if (!File.Exists(tbInput.Text))
			{
				MessageBox.Show(
							this,
							"File not found.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else
			{
				using (var sw = new StreamWriter(File.Open(
														Path.Combine(
																Path.GetDirectoryName(tbInput.Text),
																"convert.log"),
														FileMode.Create,
														FileAccess.Write,
														FileShare.None)))
				{
					// Read ruleset to get the "terrains".

					// mappings  - will be deserialized as Dictionary<object,object>
					// sequences - will be deserialized as List<object>
					// scalars   - will be deserialized as string

					using (var fs = new FileStream(tbInput.Text, FileMode.Open))
					using (var sr = new StreamReader(fs))
					{
						var str = new YamlStream();
						str.Load(sr);

//						string nodeGroup, nodeCategory, nodeLabel, terr, path, nodeBasepath;
						string nodeCategory, nodeTerrainset, nodeTilesets;

//						Dictionary<int, Tuple<string,string>> terrains;

//						YamlSequenceNode nodeTerrains;
//						YamlScalarNode   nodetry1;
//						YamlMappingNode  nodetry2;
						YamlScalarNode node;

						var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
						//sw.WriteLine("nodeRoot type= " + nodeRoot.NodeType);

						var battlesets = nodeRoot.Children[new YamlScalarNode("terrains")] as YamlSequenceNode;
						foreach (YamlMappingNode battlefield in battlesets)
						{
							//sw.WriteLine(". . battlesets= " + battlesets);
							sw.WriteLine("");


							// get the Category ->
							node = new YamlScalarNode("name");
							if (battlefield.Children.ContainsKey(node))
							{
								nodeCategory = battlefield.Children[node].ToString();
								sw.WriteLine("category= " + nodeCategory);
							}

							// get the terrainset ->
							node = new YamlScalarNode("mapDataSets");
							if (battlefield.Children.ContainsKey(node))
							{
								nodeTerrainset = battlefield.Children[node].ToString();
								sw.WriteLine("terrainset=");// + nodeTerrainset);

								var terrainset = battlefield.Children[node] as YamlSequenceNode;
								foreach (var terrain in terrainset)
								{
									sw.WriteLine(". " + terrain);
								}
							}

							// get the tilesets ->
							node = new YamlScalarNode("mapBlocks");
							if (battlefield.Children.ContainsKey(node))
							{
								nodeTilesets = battlefield.Children[node].ToString();
								sw.WriteLine("tilesets=");// + nodeTilesets);

								var tilesets = battlefield.Children[node] as YamlSequenceNode;
								foreach (var tileset in tilesets)
								{
									//sw.WriteLine(". tileset= " + tileset);

									node = new YamlScalarNode("name");
									var label = tileset[node].ToString();
									sw.WriteLine(". . label= " + label);
								}
							}


/*							// get the Group of the tileset
							nodeGroup = battlefield.Children[new YamlScalarNode(GlobalsXC.GROUP)].ToString();
							//LogFile.WriteLine(". . group= " + nodeGroup); // eg. "ufoShips"

							if (!Groups.Contains(nodeGroup))
								Groups.Add(nodeGroup);

							// get the Category of the tileset ->
							nodeCategory = battlefield.Children[new YamlScalarNode(GlobalsXC.CATEGORY)].ToString();
							//LogFile.WriteLine(". . category= " + nodeCategory); // eg. "Ufo"

							// get the Label of the tileset ->
							nodeLabel = battlefield.Children[new YamlScalarNode(GlobalsXC.TYPE)].ToString();
							nodeLabel = nodeLabel.ToUpperInvariant();
							//LogFile.WriteLine("\n. . type= " + nodeLabel); // eg. "UFO_110"

							// get the Terrains of the tileset ->
							terrains = new Dictionary<int, Tuple<string,string>>();

							nodeTerrains = battlefield.Children[new YamlScalarNode(GlobalsXC.TERRAINS)] as YamlSequenceNode;
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
							if (battlefield.Children.ContainsKey(basepath))
							{
								nodeBasepath = battlefield.Children[basepath].ToString();
								//LogFile.WriteLine(". . basepath= " + nodeBasepath);
							}
							//else LogFile.WriteLine(". . basepath not found.");

							var tileset = new Tileset(
													nodeLabel,
													nodeGroup,
													nodeCategory,
													terrains,
													nodeBasepath);
							Tilesets.Add(tileset); */
						}
					}
				}
			}
		}
		#endregion Events



		#region Structs
		/// <summary>
		/// The Tileset struct is the basic stuff of a tileset.
		/// </summary>
		struct Tileset
		{
			internal string Label
			{ get; private set; }
			internal string Group
			{ get; private set; }
			internal string Category
			{ get; private set; }
			internal List<string> Terrains
			{ get; private set; }

			internal string BasePath
			{ get; private set; }

			internal Tileset(
					string label,
					string @group,
					string category,
					List<string> terrains,
					string basepath)
				:
					this()
			{
				Label    = label;
				Group    = @group;
				Category = category;
				Terrains = terrains;

				BasePath = basepath;
			}
		}
		#endregion Structs
	}
}
