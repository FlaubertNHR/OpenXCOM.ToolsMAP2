using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using YamlDotNet.RepresentationModel; // deserialize


namespace RulesetConverter
{
	/// <summary>
	/// The Ruleset Converter.
	/// </summary>
	internal sealed partial class RulesetConverter
		:
			Form
	{
		#region Fields (static)
		private const string LabelBasepathDefault = "[use Configurator's basepath]";
		private const string LabelBasepathInvalid = "[basepath needs MAPS and ROUTES folders]";

		private const string PrePad = "#----- ";
		#endregion Fields (static)


		#region Fields
		private readonly string Info = "This app inputs an OpenXcom/E ruleset and converts any terrains found out to"
									 + Environment.NewLine
									 + "MapTilesets.tpl - a YAML template file (.TPL) for tileset configuration.";

		private int PrePadLength = PrePad.Length;

		private string _basepath = String.Empty;

		private string _typetext = String.Empty;
		#endregion Fields


		#region cTor
		/// <summary>
		/// Instantiates this <c>RulesetConverter</c>.
		/// </summary>
		internal RulesetConverter()
		{
			InitializeComponent();

			lbl_Info    .Text = Info;
			lbl_Basepath.Text = LabelBasepathDefault;

			tb_Input.BackColor =
			tb_Label.BackColor = Color.Linen;
		}
		#endregion cTor


		#region Events (override)
		protected override void OnLoad(EventArgs e)
		{
			MinimumSize = new Size(500, Height);
			MaximumSize = new Size(Int32.MaxValue, Height);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Closes the converter when the Cancel button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Opens a file browser when the find Inputfile button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindInputClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = "Open an OxC/e ruleset file ...";
				ofd.Filter = "Ruleset files(*.rul)|*.rul|All files(*.*)|*.*";

				string dir;
				if (!String.IsNullOrEmpty(tb_Input.Text)
					&& Directory.Exists(dir = Path.GetDirectoryName(tb_Input.Text)))
				{
					ofd.InitialDirectory = dir;
				}
				else if (Directory.Exists(_basepath))
				{
					ofd.InitialDirectory = _basepath;
				}
				else
					ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);


				if (ofd.ShowDialog() == DialogResult.OK)
				{
					tb_Input.Text = ofd.FileName;

					if (String.IsNullOrEmpty(tb_Label.Text))
					{
						string text = Path.GetFileNameWithoutExtension(ofd.FileName).Trim();
						for (int i = text.Length - 1; i != -1; --i)
						{
							if (!IsValidAscii((int)text[i]))
								text = text.Remove(i,1);
						}
						tb_Label.Text = text;
					}
				}
			}
			EnableConvert();
		}

		/// <summary>
		/// Handles the Basepath checkbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBasepathCheckChanged(object sender, EventArgs e)
		{
			if (cb_Basepath.Checked)
			{
				btn_Basepath.Enabled = true;

				if (IsBasepathValid(_basepath))
					lbl_Basepath.Text = _basepath;
				else
					lbl_Basepath.Text = LabelBasepathInvalid;
			}
			else
			{
				btn_Basepath.Enabled = false;
				lbl_Basepath.Text = LabelBasepathDefault;
				SetInfo();
			}
			EnableConvert();
		}

		/// <summary>
		/// Opens a file browser when the find Basepath button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindBasepathClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = "Select a basepath. A valid basepath has the"
								+ " folders MAPS, ROUTES, and preferably TERRAIN.";

				string dir;
				if (Directory.Exists(_basepath))
				{
					fbd.SelectedPath = _basepath;
				}
				else if (!String.IsNullOrEmpty(tb_Input.Text)
					&& Directory.Exists(dir = Path.GetDirectoryName(tb_Input.Text)))
				{
					fbd.SelectedPath = dir;
				}
				else
					fbd.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);


				if (fbd.ShowDialog() == DialogResult.OK)
				{
					if (IsBasepathValid(fbd.SelectedPath))
						lbl_Basepath.Text = _basepath = fbd.SelectedPath;
					else
					{
						_basepath = String.Empty;
						lbl_Basepath.Text = LabelBasepathInvalid;
					}
				}
			}
			EnableConvert();
		}


		/// <summary>
		/// Changes the group prefix by UFO/TFTD type.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTypeChanged(object sender, EventArgs e)
		{
			var rb = sender as RadioButton;
			if (rb == rb_Ufo)
			{
				if (rb.Checked) lbl_Label.Text = "ufo_";
				else            lbl_Label.Text = "tftd_";
			}
		}

		/// <summary>
		/// Prevents unwanted chars from appearing in the Group-label.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Can't do this on key-events because text can be pasted.</remarks>
		private void OnTypeTextChanged(object sender, EventArgs e)
		{
			for (int i = 0; i != tb_Label.Text.Length; ++i)
			{
				if (!IsValidAscii((int)tb_Label.Text[i]))
				{
					// fail
					tb_Label.Text = _typetext;
					tb_Label.SelectionLength = 0;
					tb_Label.SelectionStart = tb_Label.Text.Length;
					return;
				}
			}
			_typetext = tb_Label.Text; // success.
		}


		/// <summary>
		/// Runs through the file parsing data into Tilesets. Then writes it to
		/// a YAML file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConvertClick(object sender, EventArgs e)
		{
			if (!File.Exists(tb_Input.Text))
			{
				lbl_Info.BorderStyle = BorderStyle.FixedSingle;
				lbl_Info.BackColor = Color.LightCoral;
				lbl_Info.Text = "File not found.";
				btn_Convert.Enabled = false;
//				OnFindInputClick(null, EventArgs.Empty);
			}
			else if (cb_Basepath.Checked && !IsBasepathValid(_basepath))
			{
//				lbl_Info.BorderStyle = BorderStyle.FixedSingle;
//				lbl_Info.BackColor = Color.LightCoral;
				lbl_Basepath.Text = LabelBasepathInvalid;
				btn_Convert.Enabled = false;
//				OnFindBasepathClick(null, EventArgs.Empty);
			}
			else 
			{
				string dirAppL = Path.GetDirectoryName(Application.ExecutablePath);

#if DEBUG
				var swl = new StreamWriter(File.Open(
												Path.Combine(dirAppL, "convert.log"),
												FileMode.Create,
												FileAccess.Write,
												FileShare.None));
#endif

				// Read ruleset to get the "terrains".

				var Tilesets = new List<Tileset>();

				string @group = lbl_Label.Text + tb_Label.Text.Trim();
#if DEBUG
				swl.WriteLine("input= " + tb_Input.Text);
				swl.WriteLine("basepath= " + _basepath);
				swl.WriteLine("group= " + @group);
#endif
				using (var fs = new FileStream(tb_Input.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
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
#if DEBUG
						swl.WriteLine(". found terrains");
#endif
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
												if (terrain.ToString().ToUpperInvariant() != "BLANKS")
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
#if DEBUG
					else
						swl.WriteLine(". terrains NOT found.");
#endif
				}


				if (Tilesets.Count == 0)
				{
					lbl_Info.BorderStyle = BorderStyle.FixedSingle;
					lbl_Info.BackColor = Color.LightCoral;
					lbl_Info.Text = "No terrains were found in the ruleset.";
				}
				else
				{
					// YAML the tilesets ....
					using (var fs = new FileStream(Path.Combine(dirAppL, "MapTilesets.tpl"), FileMode.Create, FileAccess.Write, FileShare.None))
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

					string result;
					if (Tilesets.Count == 1)
						result = "1 terrain converted to a tileset";
					else
						result = Tilesets.Count + " terrains converted to tilesets";

					lbl_Info.BorderStyle = BorderStyle.FixedSingle;
					lbl_Info.BackColor = Color.PaleGreen;
					lbl_Info.Text = result;
				}
#if DEBUG
				swl.Dispose();
#endif
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Enables/disables the Convert button as detered by valid inputfile
		/// and basepath.
		/// </summary>
		private void EnableConvert()
		{
			btn_Convert.Enabled = File.Exists(tb_Input.Text)
							   && (!cb_Basepath.Checked || IsBasepathValid(_basepath));
		}

		/// <summary>
		/// Checks if the current basepath has both MAPS and ROUTES subdirs.
		/// </summary>
		/// <param name="basepath">the basepath</param>
		/// <returns>true if basepath is valid</returns>
		/// <remarks>Allow nonexistent TERRAIN directory.</remarks>
		private bool IsBasepathValid(string basepath)
		{
			SetInfo();

			if (   Directory.Exists(Path.Combine(basepath, "MAPS"))
				&& Directory.Exists(Path.Combine(basepath, "ROUTES")))
			{
				if (!Directory.Exists(Path.Combine(basepath, "TERRAIN")))
				{
					lbl_Info.BorderStyle = BorderStyle.FixedSingle;
					lbl_Info.BackColor = Color.LemonChiffon;
					lbl_Info.Text = "Basepath does not contain a folder for TERRAIN.";
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resets the info at the top.
		/// </summary>
		private void SetInfo()
		{
			lbl_Info.BorderStyle = BorderStyle.None;
			lbl_Info.BackColor = SystemColors.Control;
			lbl_Info.Text = Info;
		}

		/// <summary>
		/// Checks if val is readable ASCII w/out quotes.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		/// <remarks>really not sure - have to look at how Group-label is used
		/// by the Maptree.</remarks>
		private static bool IsValidAscii(int val)
		{
			return val > 31 && val != 34 && val < 127;
		}

		/// <summary>
		/// Adds padding such as " ---#" out to 80 characters.
		/// </summary>
		/// <param name="len"></param>
		/// <returns></returns>
		private static string Padder(int len)
		{
			string pad = String.Empty;
			if (len < 79)
				pad = " ";

			for (int i = 78; i > len; --i)
				pad += "-";

			if (len < 79)
				pad += "#";

			return pad;
		}
		#endregion Methods


		#region Structs
		/// <summary>
		/// The <c>Tileset</c> struct is the basic stuff of a tileset.
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
