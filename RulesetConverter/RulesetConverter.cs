﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using YamlDotNet.RepresentationModel; // deserialize


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
		private const string PrePad = "#----- ";
		private int PrePadLength = PrePad.Length;

		private const string LabelBasepathDefault = "[using Configurator's basepath]";

		private string _basepath = String.Empty;
		#endregion Fields


		#region cTor
		/// <summary>
		/// Instantiates the RulesetConverter.
		/// </summary>
		internal RulesetConverter()
		{
			InitializeComponent();

			lbl_Basepath.Text = LabelBasepathDefault;
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
		/// Opens a file browser when the find Inputfile button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFindInputClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = "Open an OxC ruleset file ...";
				ofd.Filter = "Ruleset files(*.rul)|*.rul|All files(*.*)|*.*";

				ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);


				if (ofd.ShowDialog() == DialogResult.OK)
				{
					tb_Input.Text = ofd.FileName;
					btn_Convert.Enabled = true;
				}
			}
		}


		/// <summary>
		/// Handles the Basepath checkbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBasepathCheckChanged(object sender, EventArgs e)
		{
			if (cb_Basepath.Checked)
			{
				btn_Basepath.Enabled = true;
				lbl_Basepath.Text = _basepath;
			}
			else
			{
				btn_Basepath.Enabled = false;
				lbl_Basepath.Text = LabelBasepathDefault;
			}
		}

		/// <summary>
		/// Opens a file browser when the find Basepath button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFindBasepathClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = "Select a basepath. A valid basepath has the"
								+ " folders MAPS, ROUTES, and preferably TERRAIN.";

				if (!String.IsNullOrEmpty(tb_Input.Text))
				{
					string path = Path.GetDirectoryName(tb_Input.Text);
					fbd.SelectedPath = Path.GetDirectoryName(path);
				}
				else
					fbd.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);


				if (fbd.ShowDialog() == DialogResult.OK)
					lbl_Basepath.Text = (_basepath = fbd.SelectedPath);
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
			if (!File.Exists(tb_Input.Text))
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
			else if (cb_Basepath.Checked && !Directory.Exists(_basepath))
			{
				MessageBox.Show(
							this,
							"Selected basepath directory does not exist.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
				OnFindBasepathClick(null, EventArgs.Empty);
			}
			else if (cb_Basepath.Checked
				&& (   !Directory.Exists(Path.Combine(_basepath, "MAPS"))
					|| !Directory.Exists(Path.Combine(_basepath, "ROUTES")))) // NOTE: Allow nonexistent TERRAIN directory.
			{
				MessageBox.Show(
							this,
							"Selected basepath directory does not contain MAPS and ROUTES folders.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
				OnFindBasepathClick(null, EventArgs.Empty);
			}
			else
			{
				if (cb_Basepath.Checked
					&& !Directory.Exists(Path.Combine(_basepath, "TERRAIN")))
				{
					MessageBox.Show(
								this,
								"Selected basepath directory does not contain TERRAIN folder."
								+ Environment.NewLine + Environment.NewLine
								+ "While this is not invalid it means that the terrainsets"
								+ " of each tileset has to be assigned manually with the"
								+ " TilesetEditor.",
								" Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button1,
								0);
				}


				string dirAppl = Path.GetDirectoryName(Application.ExecutablePath);

//				using (var log = new StreamWriter(File.Open(
//														Path.Combine(dirAppl, "convert.log"),
//														FileMode.Create,
//														FileAccess.Write,
//														FileShare.None)))
//				{
				// Read ruleset to get the "terrains".

				var Tilesets = new List<Tileset>();

				string @group = GetGroupLabel();

				using (var fs = new FileStream(tb_Input.Text, FileMode.Open))
				using (var sr = new StreamReader(fs))
				{
					var str = new YamlStream();
					str.Load(sr);

					IDictionary<YamlNode, YamlNode> keyvals;
					YamlScalarNode keylabel;
					YamlSequenceNode terrainset, tilesets;
					string category;
					var terrains = new List<string>();

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;

					var key = new YamlScalarNode("terrains");
					if (nodeRoot.Children.ContainsKey(key))
					{
						var battlesets = nodeRoot.Children[key] as YamlSequenceNode;
						foreach (YamlMappingNode battlefield in battlesets)
						{
							keyvals = battlefield.Children;
							if (keyvals != null && keyvals.Count != 0)
							{
								// get the category ->
								key = new YamlScalarNode("name");
								if (keyvals.ContainsKey(key))
								{
									category = keyvals[key].ToString();

									if (!String.IsNullOrEmpty(category))
									{
										// get the terrainset ->
										terrains.Clear();

										key = new YamlScalarNode("mapDataSets");
										if (keyvals.ContainsKey(key))
										{
											terrainset = keyvals[key] as YamlSequenceNode;
											foreach (var terrain in terrainset)
											{
												if (terrain.ToString().ToLowerInvariant() != "blanks")
													terrains.Add(terrain.ToString());
											}
										}

										if (terrains.Count != 0)
										{
											// get the tilesets ->
											key = new YamlScalarNode("mapBlocks");
											if (keyvals.ContainsKey(key))
											{
												keylabel = new YamlScalarNode("name");

												tilesets = keyvals[key] as YamlSequenceNode;
												foreach (var tileset in tilesets)
												{
													Tilesets.Add(new Tileset(
																			tileset[keylabel].ToString(),
																			@group,
																			category,
																			new List<string>(terrains))); // copy that, Roger.
												}
											}
										}
									}
								}
							}
						}
					}
				}


				if (Tilesets.Count == 0)
				{
					MessageBox.Show(
								this,
								"No tilesets were found in the ruleset.",
								" Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
				{
					// YAML the tilesets ....
					using (var fs = new FileStream(Path.Combine(dirAppl, "MapTilesets.tpl"), FileMode.Create))
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
						sw.WriteLine("");

						sw.WriteLine("tilesets:");

						string headerGroup    = String.Empty;
						string headerCategory = String.Empty;

						bool blankline;
						foreach (Tileset tileset in Tilesets)
						{
							blankline = false;
							if (headerGroup != tileset.Group)
							{
								headerGroup = tileset.Group;
								blankline = true;

								sw.WriteLine("");
								sw.WriteLine(PrePad + headerGroup + Padder(headerGroup.Length + PrePadLength));
							}

							if (headerCategory != tileset.Category)
							{
								headerCategory = tileset.Category;

								if (!blankline)
									sw.WriteLine("");
								sw.WriteLine(PrePad + headerCategory + Padder(headerCategory.Length + PrePadLength));
							}

							sw.WriteLine("  - type: " + tileset.Label);
							sw.WriteLine("    terrains:");

							foreach (string terrain in tileset.Terrains)
							{
								string terr = "      - " + terrain;
								if (cb_Basepath.Checked)
									terr += ": basepath"; // + _basepath;

								// NOTE: Does not handle a custom terrain basepath.

								sw.WriteLine(terr);
							}

							sw.WriteLine("    category: " + tileset.Category);
							sw.WriteLine("    group: " + tileset.Group);

							if (cb_Basepath.Checked)
								sw.WriteLine("    basepath: " + _basepath);
						}
						//  - type: UFO_110
						//    terrains:
						//      - U_EXT02
						//      - U_WALL02
						//      - U_BITS
						//    category: UFO
						//    group: ufoShips
					}
				}
//				}
			}
		}
		#endregion Events


		#region Methods
		private string GetGroupLabel()
		{
			string @group;
			if (rb_Ufo.Checked) @group = "ufo_";
			else                @group = "tftd_"; // rb_Tftd.Checked

			return @group + Path.GetFileNameWithoutExtension(tb_Input.Text);
		}

		/// <summary>
		/// Adds padding such as " ---#" out to 80 characters.
		/// </summary>
		/// <param name="len"></param>
		/// <returns></returns>
		private string Padder(int len)
		{
			string pad = String.Empty;
			if (len < 79)
				pad = " ";

			for (int i = 78; i > len; --i)
			{
				pad += "-";
			}

			if (len < 79)
				pad += "#";

			return pad;
		}
		#endregion Methods


		#region Structs
		/// <summary>
		/// The Tileset struct is the basic stuff of a tileset.
		/// </summary>
		private struct Tileset
		{
			internal string Label
			{ get; private set; }
			internal string Group
			{ get; private set; }
			internal string Category
			{ get; private set; }
			internal List<string> Terrains
			{ get; private set; }

			internal Tileset(
					string label,
					string @group,
					string category,
					List<string> terrains)
				:
					this()
			{
				Label    = label;
				Group    = @group;
				Category = category;
				Terrains = terrains;
			}
		}
		#endregion Structs
	}
}