using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	/// <summary>
	/// The possible tileset-edit Modes of the
	/// <c><see cref="TilesetEditor"/></c>.
	/// </summary>
	internal enum TsEditMode
	{
		Create,	// 0
		Exists	// 1
	}


	/// <summary>
	/// This is the Tileset Editor for MapView ii. It replaces the Paths Editor
	/// of MapView i.
	/// </summary>
	internal sealed partial class TilesetEditor
		:
			Form
	{
		/// <summary>
		/// The possible add-types.
		/// </summary>
		private enum AddType
		{
			non,		// 0
			MapExists,	// 1
			MapCreate	// 2
		}


		#region Fields (static)
		private const string AddTileset  = "Add Tileset";
		private const string EditTileset = "Edit Tileset";

		private static string _lastTerrainFolder = String.Empty;

		/// <summary>
		/// Is static to grant access to subsequent instantiations.
		/// </summary>
		private static readonly Dictionary<int, Tuple<string,string>> _copiedTerrains
						  = new Dictionary<int, Tuple<string,string>>();
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// <c>true</c> if the TilesetLabel textbox has been inited.
		/// </summary>
		/// <remarks>Don't let setting the tileset-label fire
		/// <c><see cref="OnTilesetTextboxChanged()">OnTilesetTextboxChanged()</see></c>
		/// until after <c><see cref="TilesetBasepath"/></c> is initialized else
		/// <c><see cref="ListTerrains()">ListTerrains()</see></c> will barf.</remarks>
		private bool _inited_TL;

		/// <summary>
		/// <c>true</c> if the BypassRecordsExceeded checkbox has been inited.
		/// </summary>
		/// <remarks>Don't let initializing the
		/// <c><see cref="Descriptor"/>.BypassRecordsExceeded</c> checkbox fire
		/// <c><see cref="OnBypassRecordsExceededCheckedChanged()">OnBypassRecordsExceededCheckedChanged()</see></c>
		/// and flag the Maptree changed.</remarks>
		private bool _inited_RE;

		/// <summary>
		/// A <c><see cref="Descriptor"/></c> used internally by this
		/// <c>TilesetEditor</c>.
		/// </summary>
		private Descriptor _descriptor;

		private bool _isDescriptor0;

		private bool _warned_MultipleTilesets;
		private bool _bypassTerrainPathChanged;
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets/Sets whether the user is adding a tileset or editing an
		/// existing tileset.
		/// </summary>
		/// <remarks>The <c>InputBoxType</c> is set in the constructor and does
		/// not change.</remarks>
		private TsEditMode InputBoxType
		{ get; set; }

		/// <summary>
		/// Gets/Sets whether this instantiation of the <c>TilesetEditor</c>
		/// needs to deal with an existing tileset or create a new one.
		/// </summary>
		private AddType FileAddType
		{ get; set; }

		/// <summary>
		/// The current <c><see cref="XCom.TileGroup"/></c>.
		/// </summary>
		/// <remarks>The <c>TileGroup</c> is set in the constructor and does not
		/// change.</remarks>
		private TileGroup TileGroup
		{ get; set; }

		/// <summary>
		/// Gets/Sets the group-label on the group-control.
		/// </summary>
		private string GroupLabel
		{
			get { return lbl_GroupCurrent.Text; }
			set { lbl_GroupCurrent.Text = value; }
		}

		/// <summary>
		/// Gets/Sets the category-label on the category-control.
		/// </summary>
		private string CategoryLabel
		{
			get { return lbl_CategoryCurrent.Text; }
			set { lbl_CategoryCurrent.Text = value; }
		}

		/// <summary>
		/// Gets/Sets the tileset-label on the tileset-control.
		/// </summary>
		internal string TilesetLabel
		{
			get { return tb_TilesetCurrent.Text; }
			private set { tb_TilesetCurrent.Text = value; }
		}

		/// <summary>
		/// Stores the original tileset-label.
		/// </summary>
		/// <remarks>Used only for
		/// <c><see cref="TsEditMode.Exists">TsEditMode.Exists</see></c>
		/// in various ways.</remarks>
		private string TilesetLabel_0
		{ get; set; }

		private string _basepath;
		/// <summary>
		/// Gets/Sets the basepath of the tileset. Setter calls
		/// <c><see cref="ListTerrains()">ListTerrains()</see></c> which also
		/// sets the <c><see cref="Descriptor"/></c>.
		/// </summary>
		private string TilesetBasepath
		{
			get { return _basepath; }
			set
			{
				lbl_TilesetBasepath.Text = (_basepath = value);
				ListTerrains();
			}
		}

		/// <summary>
		/// The basepath that the user has set in the
		/// <c><see cref="MapView.ConfigurationForm">Configurator</see></c>.
		/// </summary>
		private string Basepath
		{ get; set; }

		/// <summary>
		/// Stores the original terrainset of a tileset.
		/// </summary>
		/// <remarks>Used only for
		/// <c><see cref="TsEditMode.Exists">TsEditMode.Exists</see></c>
		/// to check if the terrains have changed when user clicks Accept.</remarks>
		private Dictionary<int, Tuple<string,string>> Terrains_0
		{ get; set; }

		/// <summary>
		/// Invalid characters in a file-label.
		/// </summary>
		private char[] Invalids
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// Creates a <c>TilesetEditor</c> dialog.
		/// </summary>
		/// <param name="bt"></param>
		/// <param name="labelGroup"></param>
		/// <param name="labelCategory"></param>
		/// <param name="labelTileset"></param>
		internal TilesetEditor(
				TsEditMode bt,
				string labelGroup,
				string labelCategory,
				string labelTileset)
		{
			//Logfile.Log("");
			//Logfile.Log("TilesetEditor..cTor");
			//Logfile.Log(". labelGroup= " + labelGroup);
			//Logfile.Log(". labelCategory= " + labelCategory);
			//Logfile.Log(". labelTileset= " + labelTileset);

			InitializeComponent();

			RegistryInfo.RegisterProperties(this);

			GroupLabel    = labelGroup;
			CategoryLabel = labelCategory;
			TilesetLabel  = labelTileset;

			_inited_TL = true;

			Invalids = GetInvalids();

			SetPasteTip();

			lb_TerrainsAllocated.DisplayMember = "Terrain";
			lb_TerrainsAvailable.DisplayMember = "Terrain";

			TileGroup = TileGroupManager.TileGroups[GroupLabel];

			string key = null;
			switch (TileGroup.GroupType)
			{
				case GameType.Ufo:  key = SharedSpace.ResourceDirectoryUfo;  break;
				case GameType.Tftd: key = SharedSpace.ResourceDirectoryTftd; break;
			}
			Basepath = SharedSpace.GetShareString(key);
			rb_ConfigBasepath.Checked = true;


			switch (InputBoxType = bt)
			{
				case TsEditMode.Create:
					Text = AddTileset;
					lbl_AddType.Text = "Descriptor invalid";

					lbl_TerrainChanges.Visible =
					lbl_TilesetCurrent.Visible =
					lbl_McdRecords    .Visible = false;

					btn_CreateDescriptor.Enabled =
					btn_TerrainCopy     .Enabled =
					btn_TerrainPaste    .Enabled =
					btn_TerrainClear    .Enabled =
					rb_TilesetBasepath  .Enabled =
					btn_GlobalTerrains  .Enabled = false;

					TilesetBasepath = Basepath;
					break;

				case TsEditMode.Exists:
				{
					Text = EditTileset;
					lbl_AddType.Text = "Modify existing tileset";
					lbl_TilesetCurrent.Text = TilesetLabel;

					btn_FindTileset     .Visible =
					btn_FindDirectory   .Visible =
					btn_CreateDescriptor.Visible = false;

					TilesetLabel_0 = String.Copy(TilesetLabel);

					var descriptor = TileGroup.Categories[CategoryLabel][TilesetLabel];

					int records = 0;

					Terrains_0 = new Dictionary<int, Tuple<string,string>>();
					for (int i = 0; i != descriptor.Terrains.Count; ++i)
					{
						Terrains_0[i] = new Tuple<string,string>(
															String.Copy(descriptor.Terrains[i].Item1),
															String.Copy(descriptor.Terrains[i].Item2));
						records += descriptor.GetRecordCount(i);
					}
					lbl_McdRecords.Text = records + " MCD Records";

					if (records > MapFile.MAX_MCDRECORDS)
						lbl_McdRecords.ForeColor = Color.MediumVioletRed;
					else
						lbl_McdRecords.ForeColor = Color.Tan;

					TilesetBasepath = descriptor.Basepath;

					cb_BypassRecordsExceeded.Checked = descriptor.BypassRecordsExceeded;
					break;
				}
			}
			FileAddType = AddType.non;

			btn_Cancel.Select();

			PrintTilesetCount();

			_inited_RE = true;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Resizes listboxes etc.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			int lbWidth = gb_Terrains.Width / 2 - pnl_Spacer.Width * 2 / 3; // not sure why 2/3 works.
			lb_TerrainsAllocated.Width = lbWidth - SystemInformation.VerticalScrollBarWidth / 2;
			lb_TerrainsAvailable.Width = lbWidth + SystemInformation.VerticalScrollBarWidth / 2;

			lbl_Allocated.Left = lb_TerrainsAllocated.Right - lbl_Allocated.Width - 5;
			lbl_Available.Left = lb_TerrainsAvailable.Left;

			pnl_Spacer.Left = gb_Terrains.Width / 2 - pnl_Spacer.Width / 2 - SystemInformation.VerticalScrollBarWidth / 2;
		}

		/// <summary>
		/// Checks if the box has been closed by Cancel/exit click and if so do
		/// terrain verifications.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Terrains get changed on-the-fly and do not require an
		/// Accept click. But the Map needs to be reloaded when things go back
		/// to <c><see cref="MainViewF">MainViewF</see>.OnAddTilesetClick()</c>
		/// or <c><see cref="MainViewF">MainViewF</see>.OnEditTilesetClick()</c>.</remarks>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				//Logfile.Log("TilesetEditor.OnFormClosing() _isDescriptor0= " + _isDescriptor0 + " DialogResult= " + DialogResult);

				if (DialogResult != DialogResult.OK
					&& _isDescriptor0)
				{
					if (_descriptor.Terrains.Count == 0)
					{
						e.Cancel = true;
						ShowError("The Map must have at least one terrain allocated.");
					}
					else if (!TerrainsEqual(Terrains_0, _descriptor.Terrains))
					{
						//Logfile.Log(". force DialogResult.OK");
						DialogResult = DialogResult.OK; // force reload of the Tileset
					}
				}

				if (!e.Cancel)
					RegistryInfo.UpdateRegistry(this);
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Opens a dialog to browse for a basepath-directory of Maps and Routes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindDirectoryClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = "Browse to a basepath folder. A valid basepath"
								+ " folder has the subfolders MAPS and ROUTES.";

				if (Directory.Exists(TilesetBasepath))
					fbd.SelectedPath = TilesetBasepath;


				if (fbd.ShowDialog(this) == DialogResult.OK)
				{
					TilesetBasepath = fbd.SelectedPath;
					OnTilesetTextboxChanged(null, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Opens a dialog to browse for a <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindTilesetClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = "Select a Map file";
				ofd.Filter = "Map Files (*.MAP)|*.MAP|All Files (*.*)|*.*";

				string dir = Path.Combine(TilesetBasepath, GlobalsXC.MapsDir);
				if (Directory.Exists(dir))
				{
					ofd.InitialDirectory = dir;
				}
				else if (Directory.Exists(TilesetBasepath))
					ofd.InitialDirectory = TilesetBasepath;


				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					string pfe = ofd.FileName;

					dir = Path.GetDirectoryName(pfe);
					if (dir.EndsWith(GlobalsXC.MapsDir, StringComparison.OrdinalIgnoreCase))
					{
						TilesetBasepath = Path.GetDirectoryName(dir);
						TilesetLabel    = Path.GetFileNameWithoutExtension(pfe);

						// NOTE: This will fire OnTilesetTextboxChanged() twice usually but
						// has to be here in case the basepath changed but the label didn't.
						OnTilesetTextboxChanged(null, EventArgs.Empty);
					}
					else
						ShowError("Maps must be in a directory MAPS.");
				}
			}
		}


		/// <summary>
		/// Refreshes the terrains-lists and ensures that the tileset-label is
		/// valid to be a <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The textbox forces UpperCASE.</remarks>
		private void OnTilesetTextboxChanged(object sender, EventArgs e)
		{
			if (_inited_TL) // do not run until the textbox has been initialized.
			{
				if (!ValidateCharacters(TilesetLabel))
				{
					ShowError("Characters detected that are not allowed.");

					TilesetLabel = InvalidateCharacters(TilesetLabel); // recurse after removing invalid chars.
					tb_TilesetCurrent.SelectionStart = tb_TilesetCurrent.TextLength;
				}
				else
				{
					PrintTilesetCount();

					switch (InputBoxType)
					{
						case TsEditMode.Create:
							ListTerrains();

							if (String.IsNullOrEmpty(TilesetLabel))
							{
								btn_CreateDescriptor.Enabled = false;
								btn_Accept          .Enabled = false;

								lbl_AddType.Text = "Descriptor invalid";
								FileAddType = AddType.non;
							}
							else if (_descriptor == null || _descriptor.Label != TilesetLabel)
							{
								btn_CreateDescriptor.Enabled = true;
								btn_Accept          .Enabled = false;

								lbl_AddType.Text = "Create";
								FileAddType = AddType.non;
							}
							else
							{
								btn_CreateDescriptor.Enabled = false;
								btn_Accept          .Enabled = true;

								if (MapfileExists(TilesetLabel))
								{
									lbl_AddType.Text = "Add using existing Map file";
									FileAddType = AddType.MapExists;
								}
								else
								{
									lbl_AddType.Text = "Add by creating a new Map file";
									FileAddType = AddType.MapCreate;
								}
							}
							break;

						case TsEditMode.Exists:
							if (!_warned_MultipleTilesets && TilesetLabel != TilesetLabel_0)
							{
								_warned_MultipleTilesets = true; // only once per instantiation.

								int tilesets = GetTilesetCount(TilesetLabel_0);
								if (tilesets > 1)
								{
									bool singular = (--tilesets == 1);

									string warn = String.Format(
															"There {1} {0} other tileset{2} that {1} defined with the"
														  + " current .MAP and .RMP files. The label{2} of {3} tileset{2}"
														  + " will be changed also if you change the label of this Map.",
															tilesets,						// 0
															singular ? "is" : "are",		// 1
															singular ? String.Empty : "s",	// 2
															singular ? "that" : "those");	// 3

									ShowWarn(Infobox.SplitString(warn));
								}
							}

							btn_Accept.Enabled = (TilesetLabel != TilesetLabel_0);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Lists the allocated and available terrains in their list-boxes. This
		/// function also sets the internal <c><see cref="Descriptor"/></c>,
		/// which is essential to listing the terrains as well as to the proper
		/// functioning of various control-buttons and routines in this
		/// <c>TilesetEditor</c>.
		/// </summary>
		private void ListTerrains()
		{
			lbl_PathAllocated.Text = String.Empty;

			btn_MoveUp   .Enabled =
			btn_MoveDown .Enabled =
			btn_MoveRight.Enabled =
			btn_MoveLeft .Enabled = false;

			lb_TerrainsAllocated.BeginUpdate();
			lb_TerrainsAvailable.BeginUpdate();
			lb_TerrainsAllocated.Items.Clear();
			lb_TerrainsAvailable.Items.Clear();


			_isDescriptor0 = false;

			switch (InputBoxType)
			{
				case TsEditMode.Create:
					if (TilesetExistsInCategory())
						_descriptor = null;
					break;

				case TsEditMode.Exists:
					if (TilesetLabel == TilesetLabel_0
						|| (!TilesetExistsInCategory() && !MapfileExists(TilesetLabel)))
					{
						// use original Descriptor if label hasn't changed
						// or if (label is not in Category AND its Mapfile doesn't exist in the current tileset's basepath on disk)
						// -> spellcast: ConfuseOpponent(OBJECT_SELF)

						_descriptor = TileGroup.Categories[CategoryLabel][TilesetLabel_0];
						_isDescriptor0 = true; // is Original or is Original w/ modified label
					}
					else
						_descriptor = null;
					break;
			}


			int records = 0;

			if (_descriptor != null)
			{
				for (int i = 0; i != _descriptor.Terrains.Count; ++i)
				{
					lb_TerrainsAllocated.Items.Add(new tle(_descriptor.Terrains[i]));
					records += _descriptor.GetRecordCount(i, true);
				}
			}
			lbl_McdRecords.Text = records + " MCD Records";

			if (records > MapFile.MAX_MCDRECORDS)
				lbl_McdRecords.ForeColor = Color.MediumVioletRed;
			else
				lbl_McdRecords.ForeColor = Color.Tan;

			btn_TerrainClear.Enabled =
			btn_TerrainCopy .Enabled = (_descriptor != null && _descriptor.Terrains.Count != 0);
			btn_TerrainPaste.Enabled = (_descriptor != null && _copiedTerrains     .Count != 0);


			// Get the text of 'tb_PathAvailable' (reflects the currently selected radio-button)
			string dirTerrain = tb_PathAvailable.Text;
			if (Directory.Exists(dirTerrain))
			{
				IEnumerable<string> terrains = Directory.GetFiles(
															dirTerrain,
															Globals.ALLFILES,
															SearchOption.TopDirectoryOnly)
														.Where(file => file.EndsWith(
																				GlobalsXC.McdExt,
																				StringComparison.OrdinalIgnoreCase));
				if (terrains.Any())
				{
					string basepath;
					if (rb_CustomBasepath.Checked) // getting terrainlist from a custom basepath
					{
						// delete TERRAIN from the end of 'tb_PathAvailable.Text'
						if (dirTerrain[dirTerrain.Length - 1] == Path.DirectorySeparatorChar) // TODO: Should check for AltDirectorySeparatorChar also.
						{
							dirTerrain = dirTerrain.Substring(0, dirTerrain.Length - 1);
						}

						if (dirTerrain.EndsWith(GlobalsXC.TerrainDir, StringComparison.OrdinalIgnoreCase))
						{
							dirTerrain = dirTerrain.Substring(0, dirTerrain.Length - GlobalsXC.TerrainDir.Length - 1);
						}

						basepath = dirTerrain; // user-specified basepath
					}
					else if (rb_TilesetBasepath.Checked && _descriptor != null) // getting terrainlist from the Descriptor's basepath
					{
						basepath = GlobalsXC.BASEPATH;
					}
					else //if (rb_ConfigBasepath.Checked) // getting terrainlist from the Configurator's basepath
					{
						basepath = String.Empty;
					}

					string terr;
					foreach (var terrain in terrains)
					{
						terr = Path.GetFileNameWithoutExtension(terrain);

						if ((_descriptor == null || !IsTerrainAllocated(terr, basepath))
							&& !terr.Equals("BLANKS", StringComparison.OrdinalIgnoreCase))
						{
							lb_TerrainsAvailable.Items.Add(new tle(new Tuple<string,string>(terr, basepath)));
						}
					}
				}
			}
			lb_TerrainsAllocated.EndUpdate();
			lb_TerrainsAvailable.EndUpdate();

			btn_GlobalTerrains.Enabled = _descriptor != null
									  && lb_TerrainsAllocated.Items.Count != 0;
		}


		/// <summary>
		/// Creates a tileset as a <c><see cref="Descriptor"/></c>. This is
		/// allowed iff this dialog is
		/// <c><see cref="TsEditMode.Create">TsEditMode.Create</see></c> -
		/// <c><see cref="AddType.MapExists">AddType.MapExists</see></c> or
		/// <c><see cref="AddType.MapCreate">AddType.MapCreate</see></c>.
		/// It is disallowed if the mode is
		/// <c><see cref="TsEditMode.Exists">TsEditMode.Exists</see></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>A <c><see cref="Descriptor"/></c> must be created/valid
		/// before terrains can be added.</remarks>
		private void OnCreateDescriptorClick(object sender, EventArgs e)
		{
			if (TilesetExistsInCategory())
			{
				using (var f = new Infobox(
										"Error",
										"The label already exists in the Category.",
										TilesetLabel,
										InfoboxType.Error))
				{
					f.ShowDialog(this);
				}
			}
			else
			{
				_descriptor = new Descriptor(
										TilesetLabel,
										TilesetBasepath,
										new Dictionary<int, Tuple<string,string>>(),
										TileGroup.GroupType,
										cb_BypassRecordsExceeded.Checked);

				if (_descriptor.FileValid)
				{
					lbl_AddType.Text = "Add using existing Map file";
					FileAddType = AddType.MapExists;
				}
				else
				{
					lbl_AddType.Text = "Add by creating a new Map file";
					FileAddType = AddType.MapCreate;
				}

				btn_CreateDescriptor.Enabled = false;
				btn_Accept          .Enabled = true;

				ListTerrains();


				lbl_TilesetCurrent.Text = TilesetLabel;

				lbl_TerrainChanges.Visible =
				lbl_TilesetCurrent.Visible =
				lbl_McdRecords    .Visible = true;

				rb_TilesetBasepath.Enabled = true;

				PrintTilesetCount();
			}
		}


		/// <summary>
		/// If this <c>TilesetEditor</c> is type
		/// <c><see cref="TsEditMode.Create">TsEditMode.Create</see></c>, the
		/// Accept click must check to see if a <c><see cref="Descriptor"/></c>
		/// has been created with the Create button first.
		/// 
		/// 
		/// If this <c>TilesetEditor</c> is type
		/// <c><see cref="TsEditMode.Exists">TsEditMode.Exists</see></c>, the
		/// Accept click will create a <c><see cref="Descriptor"/></c> if the
		/// tileset-label changed and delete the old <c>Descriptor</c>, and add
		/// the new one to the current tilegroup/category. If the tileset-label
		/// didn't change, nothing more need be done since any terrains that
		/// were changed have already been changed by changes to the
		/// Allocated/Available listboxes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The tileset-label shall be checked for validity before the
		/// Accept button is enabled.</remarks>
		private void OnAcceptClick(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(TilesetLabel))
			{
				ShowError("The Map label cannot be blank.");
				tb_TilesetCurrent.Select();
			}
			else if (lb_TerrainsAllocated.Items.Count == 0)
			{
				ShowError("The Map must have at least one terrain allocated.");
				// TODO: Handle cases where user doesn't have any terrains in a
				// valid TERRAIN basepath or otherwise.
			}
			else
			{
				switch (InputBoxType)
				{
					case TsEditMode.Create:

						// TODO: MapfileExists(TilesetLabel) - see TsEditMode.Exists below.

						switch (FileAddType)
						{
							case AddType.MapCreate:
								string pfeMap   = GetFullpathMapfile(TilesetLabel);
								string pfeRoute = GetFullpathRoutefile(TilesetLabel);

								// NOTE: This has to happen now because once the Maptree node
								// is selected it will try to read/load the .MAP file etc.
								if (MapFile.CreateDefault(pfeMap, pfeRoute))
								{
									// NOTE: The descriptor has already been created with the
									// Create descriptor button.
									_descriptor.FileValid = true;

									goto case AddType.MapExists;
								}
								break;

							case AddType.MapExists:
								TileGroup.AddTileset(_descriptor, CategoryLabel);
								DialogResult = DialogResult.OK; // re/load the Tileset in MainView.
								break;
						}
						break;

					case TsEditMode.Exists:
						if (TilesetLabel == TilesetLabel_0) // label didn't change; check if terrains changed ->
						{
							if (TerrainsEqual(Terrains_0, _descriptor.Terrains))
							{
								// NOTE: This shouldn't happen anymore now that the
								// Accept button remains disabled until it isn't.
								ShowError("No changes were made.");
							}
							else
								DialogResult = DialogResult.OK;

							// NOTE: a Save of Map-file is *not* required here.
						}
						else if (TilesetExistsInCategory())
						{
							using (var f = new Infobox(
													"Error",
													"The label already exists in the Category.",
													TilesetLabel,
													InfoboxType.Error))
							{
								f.ShowDialog(this);
							}
						}
						else if (MapfileExists(TilesetLabel))
						{
							// NOTE: user cannot edit a Map-label to be another already existing file.
							// There are other ways to do that: either let the user delete the target-
							// Map-file from his/her disk, or better click Edit on *that* tileset.
							// NOTE: however, if while editing a tileset the user browses to another
							// tileset and edits that tileset's terrains, the changes are effective.
							//
							// ... which is kind of awkward.

							// TODO: Ask user if he/she wants to overwrite the Map-file.
							const string head = "The Map file already exists on disk. The Tileset Editor is"
											  + " not sophisticated enough to deal with this eventuality."
											  + " Either edit that Map directly if it's already in the Maptree,"
											  + " or use Add Tileset to make it editable, or as a last resort"
											  + " remove it from your MAPS folder.";

							using (var f = new Infobox(
													"Error",
													Infobox.SplitString(head, 80),
													GetFullpathMapfile(TilesetLabel),
													InfoboxType.Error))
							{
								f.ShowDialog(this);
							}
						}
						else // label changed; rewrite the descriptor ->
						{
							string src = GetFullpathMapfile(TilesetLabel_0);
							string dst = GetFullpathMapfile(TilesetLabel);

							if (FileService.MoveFile(src, dst))
							{
								src = GetFullpathRoutefile(TilesetLabel_0);
								dst = GetFullpathRoutefile(TilesetLabel);

								if (FileService.MoveFile(src, dst))
								{
									_descriptor = new Descriptor(
															TilesetLabel,
															TilesetBasepath,
															TileGroup.Categories[CategoryLabel][TilesetLabel_0].Terrains,
															TileGroup.GroupType,
															cb_BypassRecordsExceeded.Checked);
									TileGroup.AddTileset(_descriptor, CategoryLabel);
									TileGroup.DeleteTileset(TilesetLabel_0, CategoryLabel);

									GlobalChangeLabels(); // TODO: figure out how to refresh the Maptree ... ie, the tileset labels don't change.

									DialogResult = DialogResult.OK; // reload the Tileset in MainView.
								}
							}
						}
						break;
				}
			}
		}


		/// <summary>
		/// Allocates a terrain.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainLeftClick(object sender, EventArgs e)
		{
			if (!MainViewF.that.MaptreeChanged && InputBoxType == TsEditMode.Exists)
				 MainViewF.that.MaptreeChanged = true;

			int sel = lb_TerrainsAvailable.SelectedIndex;

			var it = lb_TerrainsAvailable.SelectedItem as tle;
			var terrain = new Tuple<string,string>(
												String.Copy(it.Terrain),
												String.Copy(it.Basepath));

			int id = _descriptor.Terrains.Count;
			_descriptor.Terrains[id] = terrain;

			ListTerrains();

			if (sel == lb_TerrainsAvailable.Items.Count)
				--sel;

			lb_TerrainsAvailable.SelectedIndex = sel;
			lb_TerrainsAvailable.Select();

			lb_TerrainsAllocated.SelectedIndex = id;
		}

		/// <summary>
		/// Deallocates a terrain.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainRightClick(object sender, EventArgs e)
		{
			if (!MainViewF.that.MaptreeChanged && InputBoxType == TsEditMode.Exists)
				 MainViewF.that.MaptreeChanged = true;

			string itAvailable = null;
			string itAllocated = null;

			int sel = lb_TerrainsAvailable.SelectedIndex;
			if (sel != -1)
			{
				itAvailable = (lb_TerrainsAvailable.SelectedItem as tle).ToString();
				itAllocated = (lb_TerrainsAllocated.SelectedItem as tle).ToString();
			}

			int id = lb_TerrainsAllocated.SelectedIndex;
			for (int i = id; i != _descriptor.Terrains.Count - 1; ++i)
			{
				_descriptor.Terrains[i] = _descriptor.Terrains[i + 1];
			}
			_descriptor.Terrains.Remove(_descriptor.Terrains.Count - 1);

			ListTerrains();

			if (id == lb_TerrainsAllocated.Items.Count)
				--id;

			lb_TerrainsAllocated.SelectedIndex = id;
			lb_TerrainsAllocated.Select();

			if (sel != -1)
			{
				// NOTE: Since program-entry-point sets the app to the
				// InvariantCulture, ListBox is likely sorting by that culture.
				// So let String.Compare() use that culture also.

				// TODO: Set the listbox Sort() method and this string
				// comparison to use StringComparison.CurrentCultureIgnoreCase.

				if (String.Compare(itAllocated, itAvailable, StringComparison.InvariantCultureIgnoreCase) < 0)
					++sel;

				if (sel >= lb_TerrainsAvailable.Items.Count) // jic
					sel  = lb_TerrainsAvailable.Items.Count - 1;

				lb_TerrainsAvailable.SelectedIndex = sel;
			}
		}

		/// <summary>
		/// Shifts a terrain up in the allocated terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainUpClick(object sender, EventArgs e)
		{
			ShiftTerrainEntry(-1);
		}

		/// <summary>
		/// Shifts a terrain down in the allocated terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainDownClick(object sender, EventArgs e)
		{
			ShiftTerrainEntry(+1);
		}

		/// <summary>
		/// Shifts a terrain up/down in the allocated terrains-list.
		/// </summary>
		/// <param name="dir"></param>
		/// <remarks>Helper for
		/// <c><see cref="OnTerrainUpClick()">OnTerrainUpClick()</see></c> and
		/// <c><see cref="OnTerrainDownClick()">OnTerrainDownClick()</see></c></remarks>
		private void ShiftTerrainEntry(int dir)
		{
			if (!MainViewF.that.MaptreeChanged && InputBoxType == TsEditMode.Exists)
				 MainViewF.that.MaptreeChanged = true;

			var terrains = _descriptor.Terrains;

			int id_0 = lb_TerrainsAllocated.SelectedIndex;
			int id_1 = id_0 + dir;

			var terrain = terrains[id_1];
			terrains[id_1] = terrains[id_0];
			terrains[id_0] = terrain;

			lb_TerrainsAllocated.BeginUpdate();
			lb_TerrainsAllocated.Items.Clear();
			for (id_0 = 0; id_0 != terrains.Count; ++id_0)
			{
				lb_TerrainsAllocated.Items.Add(new tle(terrains[id_0]));
			}
			lb_TerrainsAllocated.EndUpdate();

			lb_TerrainsAllocated.SelectedIndex = id_1;
			lb_TerrainsAllocated.Select();
		}

		/// <summary>
		/// Copies the current tileset's allocated terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainCopyClick(object sender, EventArgs e)
		{
			_copiedTerrains.Clear();

			for (int i = 0; i != _descriptor.Terrains.Count; ++i)
			{
				_copiedTerrains[i] = CloneTerrainTuple(_descriptor.Terrains[i]);
			}

			SetPasteTip();
			btn_TerrainPaste.Enabled = true;
			lb_TerrainsAvailable.Select();
		}

		/// <summary>
		/// Pastes the currently copied allocated terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainPasteClick(object sender, EventArgs e)
		{
			if (!MainViewF.that.MaptreeChanged && InputBoxType == TsEditMode.Exists)
				 MainViewF.that.MaptreeChanged = true;

			_descriptor.Terrains.Clear();

			for (int i = 0; i != _copiedTerrains.Count; ++i)
			{
				_descriptor.Terrains[i] = CloneTerrainTuple(_copiedTerrains[i]);
			}

			ListTerrains();
			lb_TerrainsAvailable.Select();
		}

		/// <summary>
		/// Clears the current tileset's allocated terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainClearClick(object sender, EventArgs e)
		{
			if (!MainViewF.that.MaptreeChanged && InputBoxType == TsEditMode.Exists)
				 MainViewF.that.MaptreeChanged = true;

			_descriptor.Terrains.Clear();
			ListTerrains();
			lb_TerrainsAvailable.Select();
		}

		/// <summary>
		/// Sets the paste-tip.
		/// </summary>
		private void SetPasteTip()
		{
			string tip = String.Empty;
			for (int i = 0; i != _copiedTerrains.Count; ++i)
			{
				if (tip.Length != 0) tip += Environment.NewLine;
				tip += _copiedTerrains[i].Item1;
			}
			toolTip1.SetToolTip(btn_TerrainPaste, tip);
		}

		/// <summary>
		/// Handles a click on the allocated terrains listbox basically.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Does not fire when a selected item is removed; the index
		/// does not change. Read: it should. But it doesn't.</remarks>
		private void OnAllocatedIndexChanged(object sender, EventArgs e)
		{
			int id = lb_TerrainsAllocated.SelectedIndex;
			if (id != -1)
			{
				Color color; string dir;

				var it = lb_TerrainsAllocated.Items[id] as tle;

				string basepath = it.Basepath;
				if (String.IsNullOrEmpty(basepath))
				{
					dir = Path.Combine(Basepath, GlobalsXC.TerrainDir);

					if (TerrainExists(dir, it.Terrain))
					{
						color = SystemColors.ControlText;
						dir = "in Configurator basepath";
					}
					else
					{
						color = Color.MediumVioletRed;
						dir = "NOT FOUND in Configurator basepath";
					}
				}
				else if (basepath == GlobalsXC.BASEPATH)
				{
					dir = Path.Combine(TilesetBasepath, GlobalsXC.TerrainDir);

					if (TerrainExists(dir, it.Terrain))
					{
						color = SystemColors.ControlText;
						dir = "in Tileset basepath";
					}
					else
					{
						color = Color.MediumVioletRed;
						dir = "NOT FOUND in Tileset basepath";
					}
				}
				else
				{
					dir = Path.Combine(basepath, GlobalsXC.TerrainDir);

					if (TerrainExists(dir, it.Terrain))
					{
						color = SystemColors.ControlText;
					}
					else
					{
						color = Color.MediumVioletRed;
						dir = "NOT FOUND in Custom basepath";
					}
				}

				lbl_PathAllocated.ForeColor = color;
				lbl_PathAllocated.Text = dir;

				btn_MoveRight.Enabled = true;

				if (_descriptor != null && _descriptor.Terrains.Count > 1)
				{
					btn_MoveUp  .Enabled = (id != 0);
					btn_MoveDown.Enabled = (id != _descriptor.Terrains.Count - 1);
				}
			}
		}

		/// <summary>
		/// Handles a click on the available terrains listbox basically.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAvailableIndexChanged(object sender, EventArgs e)
		{
			btn_MoveLeft.Enabled = lb_TerrainsAvailable.SelectedIndex != -1
								&& _descriptor != null;
		}


		/// <summary>
		/// Handles changes to the Terrains-basepath radio-buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRadioTerrainChanged(object sender, EventArgs e)
		{
			_bypassTerrainPathChanged = true;

			int sel = lb_TerrainsAllocated.SelectedIndex;

			string basepath;
			if (sender == rb_CustomBasepath)
			{
				tb_PathAvailable.ReadOnly = false;
				btn_FindBasepath.Visible = true;

				if (_lastTerrainFolder.Length == 0)
					_lastTerrainFolder = Basepath;

				basepath = _lastTerrainFolder;
			}
			else
			{
				tb_PathAvailable.ReadOnly = true;
				btn_FindBasepath.Visible = false;

				if (sender == rb_TilesetBasepath)
				{
					basepath = _descriptor.Basepath;
				}
				else //if (sender == rb_ConfigBasepath)
				{
					basepath = Basepath;
				}
			}

			tb_PathAvailable.Text = Path.Combine(basepath, GlobalsXC.TerrainDir);

			ListTerrains();						// -> have to do that so that user can switch a terrain's path-type even if
			_bypassTerrainPathChanged = false;	// their paths are identical (ie. when 'tbTerrainPath.Text' does not change).

			lb_TerrainsAllocated.SelectedIndex = sel;
//			lbTerrainsAvailable.Select();
		}

		/// <summary>
		/// Handles text-changes to the available terrains TextBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainPathChanged(object sender, EventArgs e)
		{
			if (!_bypassTerrainPathChanged)
				ListTerrains();
		}


		/// <summary>
		/// Opens a dialog to browse for a Terrain-basepath.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindTerrainClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = "Browse to a basepath folder. A valid basepath"
								+ " folder has the subfolder TERRAIN.";

				if (Directory.Exists(_lastTerrainFolder))
					fbd.SelectedPath = _lastTerrainFolder;


				if (fbd.ShowDialog() == DialogResult.OK)
				{
					_lastTerrainFolder = fbd.SelectedPath;

					tb_PathAvailable.Text = Path.Combine(_lastTerrainFolder, GlobalsXC.TerrainDir);

					if (!Directory.Exists(tb_PathAvailable.Text))
					{
						ShowWarn("The subfolder TERRAIN does not exist.");
					}
				}
			}
			lb_TerrainsAvailable.Select();
		}


		/// <summary>
		/// Handles the checkedchanged event for the
		/// <c><see cref="Descriptor"/>.BypassRecordsExceeded</c> checkbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBypassRecordsExceededCheckedChanged(object sender, EventArgs e)
		{
			if (_inited_RE)
			{
				_descriptor.BypassRecordsExceeded = cb_BypassRecordsExceeded.Checked;

				if (!MainViewF.that.MaptreeChanged)
					 MainViewF.that.MaptreeChanged = true;
			}
		}

		/// <summary>
		/// Applies the current Allocated terrains-list to all tilesets that
		/// have the current tileset's
		/// <c><see cref="Descriptor"/>.Label</c> and
		/// <c><see cref="Descriptor"/>.Basepath</c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGlobalTerrainsClick(object sender, EventArgs e)
		{
			var terrains = _descriptor.Terrains;

			bool changed = false;

			foreach (var @group in TileGroupManager.TileGroups)
			foreach (var category in @group.Value.Categories)
			if (category.Key != CategoryLabel)
			{
				foreach (var descriptor in category.Value.Values)
				if (   descriptor.Label    == _descriptor.Label
					&& descriptor.Basepath == _descriptor.Basepath)
				{
					changed = true;

					descriptor.Terrains.Clear();
					for (int i = 0; i != terrains.Count; ++i)
						descriptor.Terrains[i] = CloneTerrainTuple(terrains[i]);
				}
			}

			if (changed && !MainViewF.that.MaptreeChanged)
				MainViewF.that.MaptreeChanged = true;
		}

		/// <summary>
		/// Shows a dialog that lists other tilesets that have an identical
		/// <c><see cref="Descriptor"/>.Basepath</c> and
		/// <c><see cref="Descriptor"/>.Label</c> as the current tileset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGlobalTerrainsListClick(object sender, EventArgs e)
		{
			string copyable = String.Empty;

			foreach (var @group in TileGroupManager.TileGroups)
			foreach (var category in @group.Value.Categories)
			if (@group.Value != TileGroup || category.Key != CategoryLabel)
			{
				foreach (var descriptor in category.Value.Values)
				if (   descriptor.Label    == _descriptor.Label
					&& descriptor.Basepath == _descriptor.Basepath)
				{
					if (copyable.Length != 0) copyable += Environment.NewLine;
					copyable += @group.Key + "|" + category.Key + "|" + descriptor.Label;
				}
			}

			if (copyable.Length == 0)
				copyable = "none";

			using (var f = new Infobox(
									"Tileset list",
									"other Tilesets defined by Path+Map",
									copyable))
			{
				f.ShowDialog(this);
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Changes the labels of all tilesets that have the original tileset's
		/// <c><see cref="Descriptor"/>.Basepath</c> and
		/// <c><see cref="Descriptor"/>.Label</c>.
		/// </summary>
		private void GlobalChangeLabels()
		{
			Descriptor d;

			var changes = new List<Tuple<Descriptor, string>>(); // ie. Don't screw up the groups-iterator.
			foreach (var @group in TileGroupManager.TileGroups)
			{
				changes.Clear();

				foreach (var category in @group.Value.Categories)
				foreach (var descriptor in category.Value.Values)
				{
					if (   descriptor.Label    == TilesetLabel_0
						&& descriptor.Basepath == TilesetBasepath)
					{
						var keyCategory = category.Key;
						if (TilesetExistsInCategory(@group.Key, keyCategory))
						{
							// NOTE: In practice this will not fire; it gets superceded by
							// "The Map file already exists on disk. The Tileset Editor is
							// not sophisticated enough to deal with this eventuality. Either
							// edit that Map directly if it's already in the Maptree, or use
							// Add Tileset to make it editable, or as a last resort delete it
							// from your disk."
							//
							// Although in reality I suppose it could/would fire if a user
							// manually inserts a tileset into MapTilesets but there aren't
							// corresponding .MAP/.RMP files for it. Or conversely a user
							// might delete the files from disk but keep the tileset(s) in
							// MapTilesets.
							//
							// TODO: Ask the user if he/she wants to delete the tileset
							// (not the files, there are no files).
							//
							// NOTE: If this happens both the old and new tilesets get left
							// in the Category.

							using (var f = new Infobox(
													"Warning",
													Infobox.SplitString("The tileset already exists in"
															+ " Category. That tileset will not be changed."),
													@group.Key + "|" + keyCategory + "|" + descriptor.Label,
													InfoboxType.Warn))
							{
								f.ShowDialog(this);
							}
						}
						else
						{
							d = new Descriptor(
											TilesetLabel,
											descriptor.Basepath,
											descriptor.Terrains,
											descriptor.GroupType, // ((TileGroup)@group[@group.Key]).GroupType);
											descriptor.BypassRecordsExceeded);
							changes.Add(new Tuple<Descriptor, string>(d, keyCategory));
						}
					}
				}

				foreach (var change in changes)
				{
					@group.Value.AddTileset(change.Item1, change.Item2);
					@group.Value.DeleteTileset(TilesetLabel_0, change.Item2);
				}
			}
		}


		/// <summary>
		/// Checks that a string can be a valid filename for Windows OS.
		/// </summary>
		/// <param name="chars"></param>
		/// <returns>true if no invalid chars are found</returns>
		/// <remarks>Check that 'chars' is not null or blank before call.</remarks>
		private bool ValidateCharacters(string chars)
		{
			return (chars.IndexOfAny(Invalids) == -1);
		}

		/// <summary>
		/// Removes invalid characters from a given string.
		/// </summary>
		/// <param name="chars"></param>
		/// <returns>a sanitized string</returns>
		/// <remarks>Check that 'chars' is not null or blank before call.</remarks>
		private string InvalidateCharacters(string chars)
		{
			int pos;
			while ((pos = chars.IndexOfAny(Invalids)) != -1)
				chars = chars.Remove(pos, 1);

			return chars;
		}

		/// <summary>
		/// Gets the fullpath for a Mapfile.
		/// </summary>
		/// <param name="label">the label w/out extension of a Mapfile to check
		/// for</param>
		/// <returns></returns>
		private string GetFullpathMapfile(string label)
		{
			string dir = Path.Combine(TilesetBasepath, GlobalsXC.MapsDir);
			return Path.Combine(dir, label + GlobalsXC.MapExt);
		}

		/// <summary>
		/// Gets the fullpath for a Routefile.
		/// </summary>
		/// <param name="label">the label w/out extension of a Routefile to
		/// check for</param>
		/// <returns></returns>
		private string GetFullpathRoutefile(string label)
		{
			string dir = Path.Combine(TilesetBasepath, GlobalsXC.RoutesDir);
			return Path.Combine(dir, label + GlobalsXC.RouteExt);
		}

		/// <summary>
		/// Checks if a Mapfile w/ label exists in the current basepath
		/// directory.
		/// </summary>
		/// <param name="label">the label w/out extension of a Mapfile to check
		/// for</param>
		/// <returns><c>true</c> if the Mapfile already exists on the hardrive</returns>
		private bool MapfileExists(string label)
		{
			return !String.IsNullOrEmpty(label)
				 && File.Exists(GetFullpathMapfile(label));
		}

		/// <summary>
		/// Gets the count of a specified tileset in every
		/// <c><see cref="XCom.TileGroup"/></c>.
		/// </summary>
		/// <param name="label">the tileset-label to check against</param>
		/// <returns>the count of extant tilesets</returns>
		private int GetTilesetCount(string label)
		{
			int count = 0;

			foreach (var @group in TileGroupManager.TileGroups)
			foreach (var category in @group.Value.Categories)
			foreach (var descriptor in category.Value.Values)
			{
				if (   descriptor.Label    == label
					&& descriptor.Basepath == TilesetBasepath)
				{
					++count;
				}
			}
			return count;
		}

		/// <summary>
		/// Prints the count of the tileset that are in every
		/// <c><see cref="XCom.TileGroup"/></c>.
		/// </summary>
		private void PrintTilesetCount()
		{
			int count = GetTilesetCount(TilesetLabel);

			if (InputBoxType == TsEditMode.Create
				&& _descriptor != null
				&& _descriptor.Label == TilesetLabel)
			{
				++count;
			}
			lbl_TilesetCount.Text = count.ToString();

			if (count > 1)
				lbl_TilesetCount.ForeColor = Color.MediumVioletRed;
			else
				lbl_TilesetCount.ForeColor = SystemColors.ControlText;
		}

		/// <summary>
		/// Checks if the current tileset-label exists in a specified Group and
		/// Category. The current tileset's Group and Category will be searched
		/// if <paramref name="labelGroup"/> and <paramref name="labelCategory"/>
		/// are null (default).
		/// </summary>
		/// <param name="labelGroup">the group-label of the category-label to check</param>
		/// <param name="labelCategory">the category-label of the tileset-label to check</param>
		/// <returns>true if the tileset-label already exists in the current or
		/// specified Group and Category</returns>
		/// <remarks>A label shall be unique in its Category.</remarks>
		private bool TilesetExistsInCategory(string labelGroup = null, string labelCategory = null)
		{
			if (labelGroup    == null) labelGroup    = GroupLabel;
			if (labelCategory == null) labelCategory = CategoryLabel;

			var category = TileGroupManager.TileGroups[labelGroup].Categories[labelCategory];
			return category.ContainsKey(TilesetLabel);
		}

		/// <summary>
		/// Checks if a specified terrain is listed in the Allocated listbox and
		/// therefore should be bypassed by the Available listbox.
		/// </summary>
		/// <param name="terrain">terrain-file w/out extension</param>
		/// <param name="dirTerrain">pass in a blank string if Config-basepath
		/// is checked or "basepath" if Tileset-basepath is checked; else pass
		/// in the basepath of the TERRAIN directory</param>
		/// <returns></returns>
		private bool IsTerrainAllocated(string terrain, string dirTerrain)
		{
			for (int i = 0; i != _descriptor.Terrains.Count; ++i)
			{
				if (   _descriptor.Terrains[i].Item1 == terrain
					&& _descriptor.Terrains[i].Item2 == dirTerrain)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Wrapper for <c><see cref="Infobox"/></c>.
		/// </summary>
		/// <param name="head">the error string to show</param>
		private void ShowError(string head)
		{
			using (var f = new Infobox(
									"Error",
									head,
									null,
									InfoboxType.Error))
			{
				f.ShowDialog(this);
			}
		}

		/// <summary>
		/// Wrapper for <c><see cref="Infobox"/></c>.
		/// </summary>
		/// <param name="warn">the warn string to show</param>
		private void ShowWarn(string warn)
		{
			using (var f = new Infobox(
									"Warning",
									warn,
									null,
									InfoboxType.Warn))
			{
				f.ShowDialog(this);
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Checks if two terrains-lists are equivalent.
		/// </summary>
		/// <param name="a">first terrains-list</param>
		/// <param name="b">second terrains-list</param>
		/// <returns><c>true</c> if the specified terrains-lists are equal</returns>
		private static bool TerrainsEqual(
				IDictionary<int, Tuple<string,string>> a,
				IDictionary<int, Tuple<string,string>> b)
		{
			if (a.Count != b.Count)
				return false;

			for (int i = 0; i != a.Count; ++i)
			{
				if (   a[i].Item1 != b[i].Item1
					|| a[i].Item2 != b[i].Item2)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if the MCD-file of a terrain exists.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		private static bool TerrainExists(string path, string label)
		{
			string pfe = Path.Combine(path, label + GlobalsXC.McdExt);
			return File.Exists(pfe);
		}

		/// <summary>
		/// Deep clones a given terrain-tuple. jic.
		/// </summary>
		/// <param name="terrain">a terrain-tuple</param>
		/// <returns>deep clone of the specified terrain-tuple</returns>
		private static Tuple<string,string> CloneTerrainTuple(Tuple<string,string> terrain)
		{
			return new Tuple<string,string>(
										String.Copy(terrain.Item1),
										String.Copy(terrain.Item2));
		}


		/// <summary>
		/// Gets an array of chars that are invalid in file-labels.
		/// </summary>
		/// <returns></returns>
		private static char[] GetInvalids()
		{
			var invalids = new List<char>();

			char[] chars = Path.GetInvalidFileNameChars();
			for (int i = 0; i != chars.Length; ++i)
				invalids.Add(chars[i]);

			invalids.Add(' '); // no spaces also.
			invalids.Add('.'); // and not dots.
			// TODO: hell i should just check for alpha-numeric and underscore. Old-school style. guaranteed.
			// Although users might not appreciate their old filenames getting too mangled.

			return invalids.ToArray();
			// TODO: should disallow filenames like 'CON' and 'PRN' etc. also
		}
		#endregion Methods (static)
	}


//		/// <summary>
//		/// Calls OnCreateTilesetClick() if Enter is key-upped in the
//		/// tileset-label textbox.
//		/// NOTE: KeyDown event doesn't work for an Enter key. Be careful 'cause
//		/// the keydown gets intercepted by the form itself.
//		/// TODO: Bypass triggering OnAcceptClick() ... was raised by tbTileset.KeyUp event.
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void OnTilesetKeyUp(object sender, KeyEventArgs e)
//		{
//			//Logfile.Log("");
//			//Logfile.Log("OnTilesetLabelKeyUp");
//
//			if (InputBoxType == TsEditMode.Create	// NOTE: have to remove this. If a user enters an invalid char in the label
//				&& btnCreateMap.Enabled				// then uses Enter to get rid of the error-popup, the KeyDown dismisses the
//				&& e.KeyCode == Keys.Enter)			// error but then the KeyUp will instantiate a descriptor ....
//			{										// Am sick of fighting with WinForms in an already complicated class like this.
//				OnCreateDescriptorClick(null, EventArgs.Empty);
//			}
//		}

// was OnCreateDescriptorClick() checks ->>
//			if (String.IsNullOrEmpty(Tileset)) // TODO: this should be checked before getting here.
//			{
//				Logfile.Log(". The Map label cannot be blank.");
//				ShowError("The Map label cannot be blank.");
//
//				tbTileset.Select();
//			}
//			else if (!ValidateCharacters(Tileset)) // TODO: this should be checked before getting here.
//			{
//				Logfile.Log(". The Map label contains illegal characters.");
//				ShowError("The Map label contains illegal characters.");
//
//				tbTileset.Select();
//				tbTileset.SelectionStart = tbTileset.TextLength;
//			}
//			else if (MapFileExists(Tileset))	// TODO: check to ensure that this Create function (and KeyUp-Enter events)
//			{									// cannot be called if a descriptor and/or a Map-file already exist.
//				Logfile.Log(". The Map file already exists."); // NOTE: Don't worry about it yet; this does not create a Map-file.
//				ShowError("The Map file already exists.");
//			}
//			else if (TileGroup.Categories[Category].ContainsKey(Tileset))	// safety -> TODO: the create map and tileset keyup events should
//			{																// be disabled if a Descriptor w/ tileset-label already exists
//				Logfile.Log(". The Tileset label already exists.");
//				ShowError("The Tileset label already exists.");
//			}
//			else
//			{}

//		// https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows#answer-62888
//		You may use any character in the current code page (Unicode/ANSI above 127), except:
//
//		< > : " / \ | ? *
//		Characters whose integer representations are 0-31 (less than ASCII space)
//		Any other character that the target file system does not allow (say, trailing periods or spaces)
//		Any of the DOS names: CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4,
//		COM5, COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6,
//		LPT7, LPT8, LPT9 (and avoid AUX.txt, etc) and CLOCK$
//		The file name is all periods
//
//		Some optional things to check:
//
//		File paths (including the file name) may not have more than 260 characters (that don't use the \?\ prefix)
//		Unicode file paths (including the file name) with more than 32,000 characters when using \?\
//		(note that prefix may expand directory components and cause it to overflow the 32,000 limit)
//
//		also: https://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
//
//		also: https://stackoverflow.com/questions/422090/in-c-sharp-check-that-filename-is-possibly-valid-not-that-it-exists
//
//		Naming Files, Paths, and Namespaces
//		https://msdn.microsoft.com/en-us/library/aa365247(VS.85).aspx



	/// <summary>
	/// An object for parsing out a terrain-string to show in the terrain-
	/// listboxes while retaining a reference to its terrain-tuple.
	/// </summary>
	internal sealed class tle // TerrainListEntry
	{
		internal string Terrain
		{ get; private set; }

		internal string Basepath
		{ get; private set; }

		internal tle(Tuple<string,string> terrain)
		{
			Terrain  = terrain.Item1;
			Basepath = terrain.Item2;
		}

		/// <summary>
		/// Required for
		/// <list type="bullet">
		/// <item>lb_TerrainsAllocated.DisplayMember = "Terrain";</item>
		/// <item>lb_TerrainsAvailable.DisplayMember = "Terrain";</item>
		/// </list>
		/// 
		/// 
		/// to work correctly.
		/// </summary>
		/// <returns>the <c><see cref="Terrain"/></c> string</returns>
		public override string ToString()
		{
			return Terrain;
		}
	}
}
