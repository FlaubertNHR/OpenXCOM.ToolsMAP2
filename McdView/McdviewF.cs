using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using XCom;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace McdView
{
	/// <summary>
	/// 
	/// </summary>
	public partial class McdviewF
		:
			Form
	{
		#region Fields (static)
		internal static bool isRunT; // shut the designer up.

		internal const TextFormatFlags FLAGS = TextFormatFlags.HorizontalCenter
											 | TextFormatFlags.VerticalCenter
											 | TextFormatFlags.NoPadding;

		/// <summary>
		/// Half the period of a current ID's text-bg blinker in 'SpritesetF',
		/// 'ScanGiconF', and 'LoftF'.
		/// </summary>
		internal const int PERIOD = 456;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// True if McdView has been invoked via TileView.
		/// </summary>
		private bool IsInvoked;

		internal int[,] ScanG;
		internal BitArray LoFT;

		private bool strict = true;

		/// <summary>
		/// True to prevent the Changed flag when a part is being selected.
		/// </summary>
		private bool InitFields;

		internal CopyF Copier;

		private bool SpritesBorked;
		#endregion Fields


		#region Properties
		internal TerrainPanel_main PartsPanel
		{ get; private set; }

		private Tilepart[] _parts;
		/// <summary>
		/// An array of 'Tileparts'. Each entry's record is referenced w/ 'Record'.
		/// @note Printing the quantity of records on the statusbar relies on
		/// reconstructing/assigning the Tilepart array on the fly (see the file
		/// and panel-edit operations).
		/// </summary>
		internal Tilepart[] Parts
		{
			get { return _parts; }
			set
			{
				if (miPaletteUfo .Enabled =
					miPaletteTftd.Enabled = ((PartsPanel.Parts = (_parts = value)) != null)) // perfect.
				{
					tssl_Records.Text = "Records: " + Parts.Length;
				}
				else
					tssl_Records.Text = "Records: null";
			}
		}

		private SpriteCollection _spriteset;
		/// <summary>
		/// The spriteset that will be used to display any/all sprites.
		/// @note Printing the quantity of sprites on the statusbar is either
		/// handled here or by the Copier's InsertAfterLast operation (which is
		/// the only other way to alter the spriteset).
		/// </summary>
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			set
			{
				PartsPanel.Spriteset = (_spriteset = value);
				statusbar_PrintSpriteInfo();
			}
		}


		internal bool _spriteShadeEnabled;

#if !DEBUG
		private int _spriteShadeInt = -1;
#else
		private int _spriteShadeInt = 10;
#endif
		/// <summary>
		/// The inverse-gamma adjustment for sprites and icons.
		/// </summary>
		private int SpriteShadeInt
		{
			get { return _spriteShadeInt; }
			set
			{
				if (_spriteShadeEnabled = ((_spriteShadeInt = value) != -1))
					SpriteShadeFloat = ((float)_spriteShadeInt * 0.03f); // NOTE: 33 is unity.

				InvalidatePanels(false);

				if (Copier != null)
					Copier.PartsPanel.Invalidate();
			}
		}
		internal float SpriteShadeFloat
		{ get; private set; }


		private int _selId = -1;
		/// <summary>
		/// The currently selected 'Parts' ID.
		/// </summary>
		internal int SelId
		{
			get { return _selId; }
			set
			{
				if (_selId != value)
				{
					if ((_selId = value) != -1)
					{
						bool strict0 = strict; // don't let the STRICT policy screw up populating the textfields
						strict = false;
						PopulateTextFields();
						strict = strict0;

						PartsPanel.ScrollToPart();

						miZeroVals .Enabled =
						miCheckVals.Enabled = true;
					}
					else
					{
						ClearTextFields();
						miZeroVals .Enabled =
						miCheckVals.Enabled = false;
					}

					InvalidatePanels();
				}

				if (PartsPanel.SubIds.Remove(_selId)) // safety. The SelId shall never be in the SubIds.
					PartsPanel.Invalidate();
			}
		}


		private bool _changed;
		/// <summary>
		/// Tracks if state has changed.
		/// </summary>
		internal bool Changed
		{
			private get { return _changed; }
			set
			{
				if (_changed != value)
				{
					if (_changed = value)
						Text += "*";
					else
						Text = Text.Substring(0, Text.Length - 1);
				}
			}
		}

		/// <summary>
		/// For reloading the Map when McdView is invoked via TileView.
		/// @note Reload MapView's Map even if the MCD/PCK+TAB is saved as a
		/// different file; the new terrain-label might also be in the Map's
		/// terrainset.
		/// </summary>
		public bool FireMvReload
		{ get; private set; }


		private string _label = String.Empty;
		/// <summary>
		/// The file w/out extension of the currently loaded terrain.
		/// </summary>
		internal string Label
		{
			get { return _label; }
			set
			{
				_label = value;
				if (Copier != null)
					Copier.cb_IalSprites.Text = "copy Sprites to " + _label + GlobalsXC.PckExt;
			}
		}

		private string _pfeMcd = String.Empty;
		/// <summary>
		/// The fullpath of the currently loaded MCD-file.
		/// </summary>
		internal string PfeMcd
		{
			get { return _pfeMcd; }
			set
			{
				Label = Path.GetFileNameWithoutExtension(_pfeMcd = value);
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates the McdView app.
		/// </summary>
		/// <param name="isInvoked">true if invoked via TileView</param>
		public McdviewF(bool isInvoked = false)
		{
			IsInvoked = isInvoked;

			string dirAppL = Path.GetDirectoryName(Application.ExecutablePath);
#if DEBUG
			LogFile.SetLogFilePath(dirAppL); // creates a logfile/ wipes the old one.
			DSLogFile.CreateLogFile();
#endif

			isRunT = true;
			InitializeComponent();

			MaximumSize = new Size(0,0);

			gb_Overhead    .Location = new Point(0, 0);
			gb_General     .Location = new Point(0, gb_Overhead.Bottom);
			gb_Health      .Location = new Point(0, gb_General .Bottom);
			gb_Door        .Location = new Point(0, gb_Health  .Bottom);

			gb_Tu          .Location = new Point(gb_Overhead.Right, 0);
			gb_Block       .Location = new Point(gb_Overhead.Right, gb_Tu   .Bottom);
			gb_Step        .Location = new Point(gb_Overhead.Right, gb_Block.Bottom);
			gb_Elevation   .Location = new Point(gb_Overhead.Right, gb_Step .Bottom);

			gb_Explode     .Location = new Point(gb_Tu.Right, 0);
			gb_Unused      .Location = new Point(gb_Tu.Right, gb_Explode.Bottom);

			int botDoor = gb_Door.Bottom + 5;

			lbl_Strict     .Location = new Point(5,                botDoor);
			cb_Strict      .Location = new Point(lbl_Strict.Right, botDoor);

			lbl_SpriteShade.Location = new Point(cb_Strict      .Right + 10, botDoor);
			tb_SpriteShade .Location = new Point(lbl_SpriteShade.Right,      botDoor);
			bar_SpriteShade.Location = new Point(tb_SpriteShade .Right,      botDoor);

			pnl_IsoLoft    .Location = new Point(gb_Loft    .Left - 5 - pnl_IsoLoft.Width, 15);
			bar_IsoLoft    .Location = new Point(pnl_IsoLoft.Left - 5 - bar_IsoLoft.Width, 10);

			if (!IsInvoked)
				RegistryInfo.InitializeRegistry(dirAppL);

			if (!RegistryInfo.RegisterProperties(this)) // ought never be true ... Location and ClientSize are in the YAML manifest.
			{
				ClientSize = new Size(
									gb_Overhead      .Width
										+ gb_Tu      .Width
										+ gb_Explode .Width
										+ gb_Loft    .Width
										+ pnl_IsoLoft.Width
										+ bar_IsoLoft.Width
										+ 15,
									ClientSize.Height);
			}

			TagLabels();
			TagLoftPanels();

			RecordLabel  .SetStaticVars(tssl_Overval, lbl_Description, this);
			RecordTextbox.SetStaticVars(tssl_Overval, lbl_Description);
			LoftPanel    .SetStaticVars(this);

			PartsPanel = new TerrainPanel_main(this);
			gb_Collection.Controls.Add(PartsPanel);
			PartsPanel.Width = gb_Collection.Width - 10;

			tb_SpriteShade.Text = SpriteShadeInt.ToString();

			PartsPanel.Select();

			string pathufo, pathtftd;
			GetResourcePaths(out pathufo, out pathtftd);

			ResourceInfo.LoadScanGufo(pathufo);		// -> ResourceInfo.ScanGufo
			ResourceInfo.LoadScanGtftd(pathtftd);	// -> ResourceInfo.ScanGtftd
			ScanG = ResourceInfo.ScanGufo;

			ResourceInfo.LoadLoFTufo(pathufo);		// -> ResourceInfo.LoFTufo
			ResourceInfo.LoadLoFTtftd(pathtftd);	// -> ResourceInfo.LoFTtftd
			LoFT = ResourceInfo.LoFTufo;


/*			// RotatingCube ->
			Scale(100, 100, 100);
			RotateCuboid(Math.PI / 4, Math.Atan(Math.Sqrt(2)));
//			var timer = new DispatcherTimer();
			var timer = new Timer();
			timer.Tick += (s, e) => { RotateCuboid(Math.PI / 180, 0); Refresh(); };
			timer.Interval = 150;//new TimeSpan(0, 0, 0, 0, 17);
			timer.Start(); */

			Isocube               = isocube.GetIsocube();
			CuboidOutlinePath     = isocube.GetCuboidOutline(  pnl_IsoLoft.Width, pnl_IsoLoft.Height);
			CuboidTopAnglePath    = isocube.GetTopAngle(       pnl_IsoLoft.Width, pnl_IsoLoft.Height);
			CuboidBotAnglePath    = isocube.GetBotAngle(       pnl_IsoLoft.Width, pnl_IsoLoft.Height);
			CuboidVertLineTopPath = isocube.GetVerticalLineTop(pnl_IsoLoft.Width, pnl_IsoLoft.Height);
			CuboidVertLineBotPath = isocube.GetVerticalLineBot(pnl_IsoLoft.Width, pnl_IsoLoft.Height);


			foreach (Control control in Controls)
			{
				if ((control as GroupBox) != null)
					control.Click += OnClick_FocusCollection;
				else if ((control as Panel) != null)
				{
					for (int i = 0; i != control.Controls.Count; ++i) // yes they really fucked .NET up that badly.
					{
						if ((control.Controls[i] as GroupBox) != null)
							control.Controls[i].Click += OnClick_FocusCollection;
					}
				}
			}

			tssl_Overval    .Text =
			tssl_Records    .Text =
			tssl_Sprites    .Text =
			tssl_OffsetLast .Text =
			tssl_OffsetAfter.Text = String.Empty;

			var r = new CustomToolStripRenderer();
			ss_Statusbar.Renderer = r;
		}


		/// <summary>
		/// Assigns MapView's Configurator's basepath(s) to 'pathufo' and
		/// 'pathtftd'.
		/// </summary>
		/// <param name="pathufo"></param>
		/// <param name="pathtftd"></param>
		public static void GetResourcePaths(out string pathufo, out string pathtftd)
		{
			pathufo = pathtftd = null;

			// First check the current Terrain's basepath ...
//			string path = Path.GetDirectoryName(_pfeMcd);
//			if (path.EndsWith(GlobalsXC.TerrainDir, StringComparison.InvariantCulture))
//			{
//				path = path.Substring(0, path.Length - GlobalsXC.TerrainDir.Length + 1);
//				return Path.Combine(path, SharedSpace.ScanGfile);
//			}

			// Second check the Configurator's basepath ...
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileResources = Path.Combine(dirSettings, PathInfo.ConfigResources);
			if (File.Exists(fileResources))
			{
				using (var sr = new StreamReader(File.OpenRead(fileResources)))
				{
					var ys = new YamlStream();
					ys.Load(sr);

					string val;

					var nodeRoot = ys.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in nodeRoot.Children)
					{
						switch (node.Key.ToString())
						{
							case "ufo":
								if ((val = node.Value.ToString()) != PathInfo.NotConfigured)
									pathufo = val;

								break;

							case "tftd":
								if ((val = node.Value.ToString()) != PathInfo.NotConfigured)
									pathtftd = val;

								break;
						}
					}
				}
			}

			// Third let the user load ScanG.Dat/LoFT.Dat files from menuitems.
		}
		#endregion cTor


		/// <summary>
		/// Checks if either or both 'Changed' and/or 'SpritesetChanged' has
		/// been flagged.
		/// </summary>
		/// <returns>true if okay to proceed ( more or less ... )</returns>
		private bool CheckChanged()
		{
			if (Changed)
			{
				using (var f = new ChangedBox("MCD has changed"))
				{
					switch (f.ShowDialog(this))
					{
						case DialogResult.Cancel:
							return false;

						case DialogResult.Yes:
							Save(PfeMcd);
							break;

						case DialogResult.No:
							break;
					}
				}
			}

			if (PartsPanel.SpritesChanged)
			{
				using (var f = new ChangedBox("PCK + TAB has changed"))
				{
					switch (f.ShowDialog(this))
					{
						case DialogResult.Cancel:
							return false;

						case DialogResult.Yes:
							SpritesBorked = false;
							OnClick_SaveSpriteset(null, EventArgs.Empty);

							if (SpritesBorked)
								return false;
							break;

						case DialogResult.No:
							break;
					}
				}
			}

			return true;
		}


		#region Events (override)
		/// <summary>
		/// Handles this Form's FormClosing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (CheckChanged())
			{
				RegistryInfo.UpdateRegistry(this);

				if (Copier != null) // this is needed when McdView is
					Copier.Close(); // invoked via TileView

				if (!IsInvoked)
					RegistryInfo.FinalizeRegistry();

				base.OnFormClosing(e);
			}
			else
				e.Cancel = true; // not sure if this does anything w/out the call to 'base' above.
		}

		/// <summary>
		/// Handles this Form's Resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			LayoutSpritesGroup();
			pnl_Sprites.Invalidate();

			gb_Description.Height = pnl_bg.Height - gb_Unused.Bottom;
		}

		/// <summary>
		/// The joys of keyboard events in Winforms. Bypasses forwarding a
		/// keyboard-event to the PartsPanel if a control that should use the
		/// keyboard-input instead currently has focus already. blah blah
		/// @note Requires 'KeyPreview' true.
		/// @note Keys that need to be forwarded: Arrows Up/Down/Left/Right,
		/// PageUp/Down, Home/End ... and Delete when editing an MCD.
		/// @note Holy fuck. I make the PartsPanel selectable w/ TabStop and
		/// - lo & behold - the arrow-keys no longer get forwarded. lovely
		/// So, set IsInputKey() for the arrow-keys in the PartsPanel. lovely
		/// @ IMPORTANT: If any other (types of) controls that can accept focus
		/// are added to this Form they need to be accounted for here.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			//LogFile.WriteLine("McdviewF.OnKeyDown() e.KeyCode= " + e.KeyCode);

			switch (e.KeyData)
			{
				case Keys.Enter:
					e.SuppressKeyPress = true;
					PartsPanel.Select();
					break;

				case Keys.Escape:
					e.SuppressKeyPress = true;
					if ((ActiveControl as TextBox) == null)
						SelId = -1;

					PartsPanel.Select();
					break;

				case Keys.Space: // select record #0
					if ((ActiveControl as TextBox) == null	// <- this control doesn't need spacebar but it's confusing to advance the SelId.
						&& !cb_Strict.Focused				// <- this control needs spacebar so don't pass the key to PartsPanel if it's focused
						&& Parts != null
						&& Parts.Length != 0)
					{
						PartsPanel.Select();
						PartsPanel.KeyInput(e);
					}
					break;

				case Keys.S:
					e.SuppressKeyPress = true; // NOTE: all alphabetic codes can be suppressed ...
					cb_Strict.Checked = !cb_Strict.Checked;
					break;

				default:
				{
					var tb = ActiveControl as TextBox;
					if (tb != null)
					{
						if (SelId != -1)
						{
							// keypad +/- to inc/dec focused val
							int val;
							switch (e.KeyData)
							{
//								case Keys.OemMinus: // on the numeric row -> don't do that; #48 TerrainOffset (sbyte) wants "-" key-input
								case Keys.Subtract: // on the numeric keypad (regardless of NumLock state)
									e.SuppressKeyPress = true;
									val = Int32.Parse(tb.Text);
									tb.Text = (--val).ToString();
									break;

//								case Keys.Oemplus:
								case Keys.Add:
									e.SuppressKeyPress = true;
									val = Int32.Parse(tb.Text);
									tb.Text = (++val).ToString();
									break;
							}
						}
					}
					else if (!bar_IsoLoft.Focused	// these controls need navigation key-input so
						&& !bar_SpriteShade.Focused	// don't pass the keys on if they are focused
						&& Parts != null)
					{
						PartsPanel.Select();
						PartsPanel.KeyInput(e);
					}
					break;
				}
			}
		}
		#endregion Events (override)


		#region Menuitems
		/// <summary>
		/// Handles clicking the File|Create menuitem.
		/// Creates an MCD file. See also OnClick_Open() and OnClick_Reload().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Create(object sender, EventArgs e)
		{
			if (CheckChanged())
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title      = "Create MCD file as ...";
					sfd.DefaultExt = "MCD";
					sfd.Filter     = "MCD files (*.MCD)|*.MCD|All files (*.*)|*.*";

					if (sfd.ShowDialog() == DialogResult.OK)
					{
						// NOTE: Would need to close the currently loaded file to
						// release its handle if trying to move or create that file.

						if (sfd.FileName == PfeMcd)
						{
							MessageBox.Show(
										this,
										"Creating a file that is currently loaded is not allowed."
											+ Environment.NewLine + Environment.NewLine
											+ "Create a different file or delete all records in this file.",
										" punt",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error,
										MessageBoxDefaultButton.Button1,
										0);
						}
						else
						{
							PfeMcd = sfd.FileName;

							if (File.Exists(PfeMcd))
							{
								string bak = Path.Combine(Path.GetDirectoryName(PfeMcd), GlobalsXC.MV_Backup);
								Directory.CreateDirectory(bak);

								bak = Path.Combine(bak, Path.GetFileName(PfeMcd));
								File.Delete(bak);
								File.Move(PfeMcd, bak);
							}

							File.Create(PfeMcd);

							SelId = -1;

							Parts = new Tilepart[0];

							Palette pal;
							if (miPaletteUfo.Checked)
								pal = Palette.UfoBattle;
							else
								pal = Palette.TftdBattle;

							ResourceInfo.Spritesets.Clear();
							Spriteset = ResourceInfo.LoadSpriteset(
																Label,
																Path.GetDirectoryName(PfeMcd),
																ResourceInfo.TAB_WORD_LENGTH_2,
																pal,
																true);

							CacheLoad.SetCache(Parts);
							Changed =
							PartsPanel.SpritesChanged = false;
							Text = "McdView - " + PfeMcd;

							miSave         .Enabled =
							miSaveas       .Enabled =
							miReload       .Enabled =
							miSaveSpriteset.Enabled = true;

							PartsPanel.Select();
						}
					}
				}
			}
		}

		/// <summary>
		/// Handles clicking the File|Open menuitem.
		/// Loads an MCD file. See also OnClick_Create() and OnClick_Reload().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Open(object sender, EventArgs e)
		{
			if (CheckChanged())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title      = "Open an MCD file";
					ofd.DefaultExt = "MCD";
					ofd.Filter     = "MCD files (*.MCD)|*.MCD|All files (*.*)|*.*";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						SelId = -1;

						PfeMcd = ofd.FileName;

						using (var bs = new BufferedStream(File.OpenRead(PfeMcd)))
						{
							var parts = new Tilepart[(int)bs.Length / TilepartFactory.Length]; // TODO: Error if this don't work out right.

							Palette pal;
							if (miPaletteUfo.Checked)
								pal = Palette.UfoBattle;
							else
								pal = Palette.TftdBattle;

							ResourceInfo.Spritesets.Clear();
							Spriteset = ResourceInfo.LoadSpriteset(
																Label,
																Path.GetDirectoryName(PfeMcd),
																ResourceInfo.TAB_WORD_LENGTH_2,
																pal,
																true);

							for (int id = 0; id != parts.Length; ++id)
							{
								var bindata = new byte[TilepartFactory.Length];
								bs.Read(bindata, 0, TilepartFactory.Length);

								parts[id] = new Tilepart(
													id,
													new McdRecord(bindata));
							}

							Tilepart part;
							for (int id = 0; id != parts.Length; ++id)
							{
								part = parts[id];
								part.Dead = TilepartFactory.GetDeadPart(
																	Label,
																	id,
																	part.Record,
																	parts);
								part.Altr = TilepartFactory.GetAltrPart(
																	Label,
																	id,
																	part.Record,
																	parts);
							}

							Parts = parts; // do not assign to 'Parts' until the array is gtg.
						}

						CacheLoad.SetCache(Parts);
						Changed =
						PartsPanel.SpritesChanged = false;
						Text = "McdView - " + PfeMcd;

						miSave         .Enabled =
						miSaveas       .Enabled =
						miReload       .Enabled =
						miSaveSpriteset.Enabled = true;

						PartsPanel.Select();
					}
				}
			}
		}

		/// <summary>
		/// Handles clicking the File|Reload menuitem.
		/// Reloads the currently loaded MCD file. See also OnClick_Create() and
		/// OnClick_Open().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Reload(object sender, EventArgs e)
		{
			if (File.Exists(PfeMcd))
			{
				SelId = -1;

				using (var bs = new BufferedStream(File.OpenRead(PfeMcd)))
				{
					var parts = new Tilepart[(int)bs.Length / TilepartFactory.Length]; // TODO: Error if this don't work out right.

					Palette pal;
					if (miPaletteUfo.Checked)
						pal = Palette.UfoBattle;
					else
						pal = Palette.TftdBattle;

					ResourceInfo.Spritesets.Clear();
					Spriteset = ResourceInfo.LoadSpriteset(
														Label,
														Path.GetDirectoryName(PfeMcd),
														ResourceInfo.TAB_WORD_LENGTH_2,
														pal,
														true);

					for (int id = 0; id != parts.Length; ++id)
					{
						var bindata = new byte[TilepartFactory.Length];
						bs.Read(bindata, 0, TilepartFactory.Length);

						parts[id] = new Tilepart(
											id,
											new McdRecord(bindata));
					}

					Tilepart part;
					for (int id = 0; id != parts.Length; ++id)
					{
						part = parts[id];
						part.Dead = TilepartFactory.GetDeadPart(
															Label,
															id,
															part.Record,
															parts);
						part.Altr = TilepartFactory.GetAltrPart(
															Label,
															id,
															part.Record,
															parts);
					}

					Parts = parts; // do not assign to 'Parts' until the array is gtg.
				}

				CacheLoad.SetCache(Parts);
				Changed =
				PartsPanel.SpritesChanged = false;

				PartsPanel.Select();
			}
			else
			{
				MessageBox.Show(
							this,
							"The file no longer exists.",
							" File invalid",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}

		/// <summary>
		/// Loads a specified Mcdfile as called from TileView.
		/// </summary>
		/// <param name="pfeMcd"></param>
		/// <param name="palette"></param>
		/// <param name="terId"></param>
		public void LoadRecords(
				string pfeMcd,
				string palette,
				int terId)
		{
			PfeMcd = pfeMcd;

			using (var bs = new BufferedStream(File.OpenRead(PfeMcd)))
			{
				var parts = new Tilepart[(int)bs.Length / TilepartFactory.Length]; // TODO: Error if this don't work out right.

				ResourceInfo.Spritesets.Clear();
				Spriteset = ResourceInfo.LoadSpriteset(
													Label,
													Path.GetDirectoryName(PfeMcd),
													ResourceInfo.TAB_WORD_LENGTH_2,
													Palette.UfoBattle,
													true);

				for (int id = 0; id != parts.Length; ++id)
				{
					var bindata = new byte[TilepartFactory.Length];
					bs.Read(bindata, 0, TilepartFactory.Length);

					parts[id] = new Tilepart(
										id,
										new McdRecord(bindata));
				}

				Tilepart part;
				for (int id = 0; id != parts.Length; ++id)
				{
					part = parts[id];
					part.Dead = TilepartFactory.GetDeadPart(
														Label,
														id,
														part.Record,
														parts);
					part.Altr = TilepartFactory.GetAltrPart(
														Label,
														id,
														part.Record,
														parts);
				}

				Parts = parts; // do not assign to 'Parts' until the array is gtg.
			}

			if (palette.Substring(0,4) == "tftd")
				OnClick_PaletteTftd(null, EventArgs.Empty);

			CacheLoad.SetCache(Parts);
			Changed =
			PartsPanel.SpritesChanged = false;
			Text = "McdView - " + PfeMcd;

			miSave         .Enabled =
			miSaveas       .Enabled =
			miReload       .Enabled =
			miSaveSpriteset.Enabled = true;

			PartsPanel.Select();
			SelId = terId;
		}


		/// <summary>
		/// Handles clicking the File|Save menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Save(object sender, EventArgs e)
		{
			Save(PfeMcd);
		}

		/// <summary>
		/// Handles clicking the File|Saveas menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Saveas(object sender, EventArgs e)
		{
			using (var sfd = new SaveFileDialog())
			{
				sfd.Title      = "Save MCD file as ...";
				sfd.DefaultExt = "MCD";
				sfd.Filter     = "MCD files (*.MCD)|*.MCD|All files (*.*)|(*.*)";
				sfd.FileName   = Label + GlobalsXC.McdExt;

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					PfeMcd = sfd.FileName;

					Save(PfeMcd, true);

					Text = "McdView - " + PfeMcd;


					if (Spriteset != null)
					{
						bool writeSpriteset = false;

						string dir = Path.GetDirectoryName(PfeMcd);

						string pfePck = Path.Combine(dir, Label + GlobalsXC.PckExt);
						string pfeTab = Path.Combine(dir, Label + GlobalsXC.TabExt);

						if (File.Exists(pfePck) && File.Exists(pfeTab))
						{
							if (MessageBox.Show(
											this,
											"Sprite files for the terrain detected."
												+ " Do you want to overwrite those PCK + TAB"
												+ " files with the current spriteset?",
											" Sprite files found",
											MessageBoxButtons.YesNo,
											MessageBoxIcon.Question,
											MessageBoxDefaultButton.Button2,
											0) == DialogResult.Yes)
							{
								writeSpriteset = true;
							}
						}
						else if (MessageBox.Show(
											this,
											"Sprite files for the terrain were not found."
												+ " Do you want to write PCK + TAB files"
												+ " for this terrain?",
											" Create sprite files",
											MessageBoxButtons.YesNo,
											MessageBoxIcon.Question,
											MessageBoxDefaultButton.Button2,
											0) == DialogResult.Yes)
						{
							writeSpriteset = true;
						}

						if (writeSpriteset)
							OnClick_SaveSpriteset(null, EventArgs.Empty);
					}
					OnClick_Reload(null, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Conducts the save-procedure.
		/// </summary>
		/// <param name="pfeMcd">path-file-extension to save to</param>
		/// <param name="as">true if doing a SaveAs operation</param>
		private void Save(string pfeMcd, bool @as = false)
		{
			if (Parts.Length <= MapFileService.MAX_MCDRECORDS
				|| MessageBox.Show(
								this,
								"Total MCD records in the terrain exceeds "
									+ MapFileService.MAX_MCDRECORDS + ".",
								" Records exceeded",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button2,
								0) == DialogResult.OK)
			{
				if (!@as)
				{
					WriteMcdData(pfeMcd + ".t");

					string bak = Path.Combine(Path.GetDirectoryName(pfeMcd), GlobalsXC.MV_Backup);
					Directory.CreateDirectory(bak);

					File.Replace(
							pfeMcd + ".t",	// src
							pfeMcd,			// dst
							Path.Combine(bak, Path.GetFileName(pfeMcd)),
							true);			// ignoreMetadataErrors
				}
				else
					WriteMcdData(pfeMcd);

				CacheLoad.SetCache(Parts);
				Changed =
				PartsPanel.SpritesChanged = false;
				FireMvReload = true;
			}
		}

		/// <summary>
		/// Writes/overwrites the specified MCD file.
		/// </summary>
		/// <param name="pfeMcd"></param>
		private void WriteMcdData(string pfeMcd)
		{
			using (var fs = File.Create(pfeMcd))
			{
				McdRecord record;

				foreach (Tilepart part in Parts)
				{
					record = part.Record;

					fs.WriteByte((byte)record.Sprite1);					//  0
					fs.WriteByte((byte)record.Sprite2);					//  1
					fs.WriteByte((byte)record.Sprite3);					//  2
					fs.WriteByte((byte)record.Sprite4);					//  3
					fs.WriteByte((byte)record.Sprite5);					//  4
					fs.WriteByte((byte)record.Sprite6);					//  5
					fs.WriteByte((byte)record.Sprite7);					//  6
					fs.WriteByte((byte)record.Sprite8);					//  7

					fs.WriteByte((byte)record.Loft1);					//  8
					fs.WriteByte((byte)record.Loft2);					//  9
					fs.WriteByte((byte)record.Loft3);					// 10
					fs.WriteByte((byte)record.Loft4);					// 11
					fs.WriteByte((byte)record.Loft5);					// 12
					fs.WriteByte((byte)record.Loft6);					// 13
					fs.WriteByte((byte)record.Loft7);					// 14
					fs.WriteByte((byte)record.Loft8);					// 15
					fs.WriteByte((byte)record.Loft9);					// 16
					fs.WriteByte((byte)record.Loft10);					// 17
					fs.WriteByte((byte)record.Loft11);					// 18
					fs.WriteByte((byte)record.Loft12);					// 19

					ushort u = record.ScanG_reduced;
					if (BitConverter.IsLittleEndian)
					{
						byte b = (byte)(u & 0x00FF);
						fs.WriteByte(b);								// 20
						b = (byte)((u & 0xFF00) >> 8);
						fs.WriteByte(b);								// 21
					}
					else // swap bytes.
					{
						byte b = (byte)((u & 0xFF00) >> 8);
						fs.WriteByte(b);								// 20
						b = (byte)(u & 0x00FF);
						fs.WriteByte(b);								// 21
					}

					fs.WriteByte((byte)record.Unknown22);				// 22
					fs.WriteByte((byte)record.Unknown23);				// 23
					fs.WriteByte((byte)record.Unknown24);				// 24
					fs.WriteByte((byte)record.Unknown25);				// 25
					fs.WriteByte((byte)record.Unknown26);				// 26
					fs.WriteByte((byte)record.Unknown27);				// 27
					fs.WriteByte((byte)record.Unknown28);				// 28
					fs.WriteByte((byte)record.Unknown29);				// 29

					fs.WriteByte(Convert.ToByte(record.SlidingDoor));	// 30 (bool)
					fs.WriteByte(Convert.ToByte(record.StopLOS));		// 31 (bool)
					fs.WriteByte(Convert.ToByte(record.NotFloored));	// 32 (bool)
					fs.WriteByte(Convert.ToByte(record.BigWall));		// 33 (bool)
					fs.WriteByte(Convert.ToByte(record.GravLift));		// 34 (bool)
					fs.WriteByte(Convert.ToByte(record.HingedDoor));	// 35 (bool)
					fs.WriteByte(Convert.ToByte(record.BlockFire));		// 36 (bool)
					fs.WriteByte(Convert.ToByte(record.BlockSmoke));	// 37 (bool)

					fs.WriteByte((byte)record.LeftRightHalf);			// 38
					fs.WriteByte((byte)record.TU_Walk);					// 39
					fs.WriteByte((byte)record.TU_Slide);				// 40
					fs.WriteByte((byte)record.TU_Fly);					// 41
					fs.WriteByte((byte)record.Armor);					// 42
					fs.WriteByte((byte)record.HE_Block);				// 43
					fs.WriteByte((byte)record.DieTile);					// 44
					fs.WriteByte((byte)record.FireResist);				// 45
					fs.WriteByte((byte)record.Alt_MCD);					// 46
					fs.WriteByte((byte)record.Unknown47);				// 47
					fs.WriteByte(unchecked((byte)record.StandOffset));	// 48 (sbyte)
					fs.WriteByte((byte)record.TileOffset);				// 49
					fs.WriteByte((byte)record.Unknown50);				// 50
					fs.WriteByte((byte)record.LightBlock);				// 51
					fs.WriteByte((byte)record.Footstep);				// 52

					fs.WriteByte((byte)record.PartType);				// 53 (PartType)
					fs.WriteByte((byte)record.HE_Type);					// 54
					fs.WriteByte((byte)record.HE_Strength);				// 55
					fs.WriteByte((byte)record.SmokeBlockage);			// 56
					fs.WriteByte((byte)record.Fuel);					// 57
					fs.WriteByte((byte)record.LightSource);				// 58
					fs.WriteByte((byte)record.Special);					// 59 (SpecialType)
					fs.WriteByte(Convert.ToByte(record.BaseObject));	// 60 (bool)
					fs.WriteByte((byte)record.Unknown61);				// 61
				}
			}
		}


		/// <summary>
		/// Handles clicking the File|Save spriteset menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_SaveSpriteset(object sender, EventArgs e)
		{
			if (Spriteset != null)
			{
				string dir = Path.GetDirectoryName(PfeMcd);

				string pfePck = Path.Combine(dir, Label + GlobalsXC.PckExt);
				string pfeTab = Path.Combine(dir, Label + GlobalsXC.TabExt);

				BackupSpritesetFiles(dir, Label);

				if (SpriteCollection.SaveSpriteset(
												dir,
												Label,
												Spriteset,
												ResourceInfo.TAB_WORD_LENGTH_2))
				{
					SpritesBorked =
					PartsPanel.SpritesChanged = false;
				}
				else
				{
					// bork - likely too many sprites. run Edit|Checkvals ...
					SpritesBorked = true;
					string error = String.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"An I/O error occurred.");
					MessageBox.Show(
								error,
								" Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}

				FireMvReload = true;
			}
		}

		/// <summary>
		/// Backs up the PCK+TAB files before trying to save the Spriteset.
		/// @note See also PckView.PckViewForm.OnSaveClick() and OnSaveAsClick().
		/// @note A possible internal reason that a spriteset is invalid is that
		/// if the total length of its compressed PCK-data exceeds 2^16 bits
		/// (roughly). That is, the TAB file tracks the offsets and needs to
		/// know the total length of the PCK file, but UFO's TAB file stores the
		/// offsets in only 2-byte format (2^16 bits) so the arithmetic explodes
		/// with an overflow as soon as an offset for one of the sprites becomes
		/// too large. (Technically, the total PCK data can exceed 2^16 bits;
		/// but the *start offset* for a sprite cannot -- at least that's how it
		/// works in MapView I/II. Other apps like XCOM, OpenXcom, MCDEdit will
		/// use their own routines.)
		/// @note It appears that TFTD's terrain files suffer this limitation
		/// also (2-byte TabwordLength).
		/// </summary>
		/// <param name= "dir">the directory of the files to backup</param>
		/// <param name= "fil">the file w/out path or extension</param>
		private static void BackupSpritesetFiles(string dir, string fil)
		{
			// TODO: Don't be such a nerd; see McdView's safety save routine.

			Directory.CreateDirectory(dir); // in case user deleted the dir.

			string pfePck = Path.Combine(dir, fil + GlobalsXC.PckExt);
			string pfeTab = Path.Combine(dir, fil + GlobalsXC.TabExt);

			string dirB = Path.Combine(dir, GlobalsXC.MV_Backup);

			if (File.Exists(pfePck))
			{
				Directory.CreateDirectory(dirB);

				string pfePckB = Path.Combine(dirB, fil + GlobalsXC.PckExt);
				File.Copy(pfePck, pfePckB, true);
			}

			if (File.Exists(pfeTab))
			{
				Directory.CreateDirectory(dirB);

				string pfeTabB = Path.Combine(dirB, fil + GlobalsXC.TabExt);
				File.Copy(pfeTab, pfeTabB, true);
			}
		}


		/// <summary>
		/// Handles clicking the File|Quit menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Quit(object sender, EventArgs e)
		{
			Close();
		}


		/// <summary>
		/// Handles clicking the Palette|UFO menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_PaletteUfo(object sender, EventArgs e)
		{
			if (!miPaletteUfo.Checked)
			{
				miPaletteUfo .Checked = true;
				miPaletteTftd.Checked = false;

				if (Spriteset != null)
					Spriteset.Pal = Palette.UfoBattle;

				ScanG = ResourceInfo.ScanGufo;
				LoFT  = ResourceInfo.LoFTufo;

				InvalidatePanels();
			}
		}

		/// <summary>
		/// Handles clicking the Palette|TFTD menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_PaletteTftd(object sender, EventArgs e)
		{
			if (!miPaletteTftd.Checked)
			{
				miPaletteTftd.Checked = true;
				miPaletteUfo .Checked = false;

				if (Spriteset != null)
					Spriteset.Pal = Palette.TftdBattle;

				ScanG = ResourceInfo.ScanGtftd;
				LoFT  = ResourceInfo.LoFTtftd;

				InvalidatePanels();
			}
		}


		/// <summary>
		/// Handles clicking the Edit|ZeroVals menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_ZeroVals(object sender, EventArgs e)
		{
			if (MessageBox.Show(
							this,
							"Zero the current record's values",
							" Zero all values",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button2,
							0) == DialogResult.Yes)
			{
				bool strict0 = strict; // don't let the STRICT policy prevent setting LeftRightHalf to "0"
				strict = false;
				ClearTextFields(true);
				strict = strict0;
			}
		}

		/// <summary>
		/// Handles clicking the Edit|CheckVals menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_CheckVals(object sender, EventArgs e)
		{
			bool suppress = false;

			List<string> borks = GetStrictBorks();
			if (borks.Count != 0)
			{
				suppress = true;

				string copyable = String.Empty;
				foreach (var bork in borks)
				{
					if (!String.IsNullOrEmpty(copyable))
						copyable += Environment.NewLine;

					copyable += bork;
				}

				using (var f = new Infobox(
										"Strict test",
										"The following items exhibit anomalies.",
										copyable))
				{
					f.ShowDialog(this);
				}
			}

			if (Spriteset != null)
			{
				string result;
				if (!SpriteCollection.Test2byteSpriteset(Spriteset, out result))
				{
					suppress = true;
					using (var f = new Infobox(
											"Strict test",
											"Sprite offset is invalid.",
											result))
					{
						f.ShowDialog(this);
					}
				}
			}

			if (!suppress)
				MessageBox.Show(
							this,
							"All values appear to be within accepted ranges.",
							" Strict test",
							MessageBoxButtons.OK,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							0);
		}


		/// <summary>
		/// Handles a click to open/close the Copier on the menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnClick_OpenCopyPanel(object sender, EventArgs e)
		{
			if (miCopyPanel.Checked = !miCopyPanel.Checked)
			{
				OpenCopyPanel(true);
			}
			else
			{
				Copier.Close();
				Copier = null;
			}
		}

		/// <summary>
		/// Handles an open/close of the Copier on the menuitem or will open a
		/// different MCD file from the Copier itself (without closing the
		/// Copier).
		/// </summary>
		/// <param name="it">true if handling the menuitem click; ie. can close
		/// the Copier form</param>
		internal void OpenCopyPanel(bool it = false)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title      = "Open an MCD file";
				ofd.DefaultExt = "MCD";
				ofd.Filter     = "MCD files (*.MCD)|*.MCD|All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					if (Copier == null)
					{
						Copier = new CopyF(this);
						Copier.Show();

						Copier.LoadIalOptions();
					}
					Copier.SelId = -1;

					Copier.PfeMcd = ofd.FileName;

					using (var bs = new BufferedStream(File.OpenRead(Copier.PfeMcd)))
					{
						var parts = new Tilepart[(int)bs.Length / TilepartFactory.Length]; // TODO: Error if this don't work out right.

						Palette pal;
						if (miPaletteUfo.Checked)
							pal = Palette.UfoBattle;
						else
							pal = Palette.TftdBattle;

						ResourceInfo.Spritesets.Clear();
						Copier.Spriteset = ResourceInfo.LoadSpriteset(
																	Copier.Label,
																	Path.GetDirectoryName(Copier.PfeMcd),
																	ResourceInfo.TAB_WORD_LENGTH_2,
																	pal,
																	true);

						for (int id = 0; id != parts.Length; ++id)
						{
							var bindata = new byte[TilepartFactory.Length];
							bs.Read(bindata, 0, TilepartFactory.Length);

							parts[id] = new Tilepart(
												id,
												new McdRecord(bindata));
						}

						Tilepart part;
						for (int id = 0; id != parts.Length; ++id)
						{
							part = parts[id];
							part.Dead = TilepartFactory.GetDeadPart(
																Copier.Label,
																id,
																part.Record,
																parts);
							part.Altr = TilepartFactory.GetAltrPart(
																Copier.Label,
																id,
																part.Record,
																parts);
						}

						Copier.Parts = parts; // do not assign to 'Parts' until the array is gtg.
					}

					Copier.cb_IalSprites.Enabled = (Copier.Spriteset != null);
				}
				else
				{
					if (it && Copier != null)
					{
						Copier.Close();
						Copier = null;
					}
					miCopyPanel.Checked = (Copier != null);
				}
			}
		}

		/// <summary>
		/// Closes the copypanel from the CopyF object itself.
		/// </summary>
		internal void CloseCopyPanel()
		{
			Copier = null;
			miCopyPanel.Checked = false;
		}


		/// <summary>
		/// Handles clicking the Help|Help menuitem.
		/// Shows the CHM helpfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Help(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(this, "file://" + help, HelpNavigator.Topic, "html/mcdview.htm");
		}

		/// <summary>
		/// Handles clicking the Help|About menuitem.
		/// Shows the about-box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_About(object sender, EventArgs e)
		{
			var an = Assembly.GetExecutingAssembly().GetName();
			string ver = "Ver "
					   + an.Version.Major + "."
					   + an.Version.Minor + "."
					   + an.Version.Build + "."
					   + an.Version.Revision;
#if DEBUG
			ver += " - debug";
#else
			ver += " - release";
#endif
			using (var f = new Infobox(
									"Version info",
									null,
									ver))
			{
				f.ShowDialog(this);
			}
		}
		#endregion Menuitems


		#region Events
		/// <summary>
		/// Selects the PartsPanel if a group's title (or a blank point inside
		/// of the groupbox), etc is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_FocusCollection(object sender, EventArgs e)
		{
			PartsPanel.Select();
		}

		/// <summary>
		/// Selects the STRICT checkbox if the label is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_FocusStrict(object sender, EventArgs e)
		{
			cb_Strict.Select();
		}

		/// <summary>
		/// Selects the SpriteShade trackbar if the label is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_FocusShade(object sender, EventArgs e)
		{
			bar_SpriteShade.Select();
		}


		/// <summary>
		/// Handles SpriteShade's TextChanged event for its TextBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTextChanged_SpriteShade(object sender, EventArgs e)
		{
			string text = tb_SpriteShade.Text.Trim();
			while (text.StartsWith("0", StringComparison.InvariantCulture))
				text = text.Substring(1);

			if (text != tb_SpriteShade.Text)
			{
				tb_SpriteShade.Text = text; // recurse
			}
			else
			{
				int result;
				if (Int32.TryParse(tb_SpriteShade.Text, out result))
				{
					if      (result <  -1) tb_SpriteShade.Text =  "-1"; // recurse
					else if (result ==  0) tb_SpriteShade.Text =  "-1"; // recurse
					else if (result > 100) tb_SpriteShade.Text = "100"; // recurse
					else
					{
						SpriteShadeInt = result;
						bar_SpriteShade.Value = (result != -1 ? result : 0);
					}
				}
				else
					tb_SpriteShade.Text = "-1"; // recurse
			}
		}

		/// <summary>
		/// Handles SpriteShade's trackbar's ValueChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnValueChanged_SpriteShade(object sender, EventArgs e)
		{
			int val = bar_SpriteShade.Value;
			if (val == 0)
				val = -1;

			tb_SpriteShade.Text = val.ToString();
		}

		/// <summary>
		/// Handles the "SpriteShade" textbox gaining focus as well as
		/// mouseovers on its label and textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEnterSpriteShade(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteShade is an inverse gamma-value only for sprites drawn in this app."
								 + " It has nothing to do with palette-based sprite-shading in XCOM itself."
								 + " This setting is not saved."
								 + Environment.NewLine + Environment.NewLine
								 + "1..100, unity 33, default -1 off";
		}

		/// <summary>
		/// Handles mouseover leaving the "SpriteShade" Label or TextBox.
		/// @note Retains the current description if the TextBox has
		/// input-focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseLeaveSpriteShade(object sender, EventArgs e)
		{
			if (!tb_SpriteShade.Focused)
				lbl_Description.Text = String.Empty;
		}


		/// <summary>
		/// Handles STRICT's CheckChanged event for its CheckBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCheckChanged_Strict(object sender, EventArgs e)
		{
			if (strict = cb_Strict.Checked)
			{
				lbl_Strict.ForeColor = SystemColors.ControlText;
				lbl_Strict.Text = "STRICT";

/*				if (SelId != -1)
				{
					// TODO: test if any values are outside standard limits and issue
					// a warning that checking STRICT will set those values to within
					// regular operating bounds. If user chooses
					//
					//   Ignore: don't reset values
					//   Retry:  reset values
					//   Abort:  set STRICT unchecked.
					//
					// This needs to be done only when STRICT becomes checked;
					// unchecking STRICT shall do nothing here.
				} */
			}
			else
			{
				lbl_Strict.ForeColor = Color.Red;
				lbl_Strict.Text = "LOOSE";
			}
		}

		/// <summary>
		/// Handles the "STRICT" checkbox gaining focus as well as mouseovers on
		/// its label and checkbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEnterStrict(object sender, EventArgs e)
		{
			lbl_Description.Text = "STRICT enforces valid values for XCOM. Unchecked allows values"
								 + " outside what's expected (for expert experts only - ie people"
								 + " who code their own XCOM executable and require extended values)."
								 + " This setting is not saved."
								 + Environment.NewLine + Environment.NewLine
								 + "default checked";
		}

		/// <summary>
		/// Handles mouseover leaving the "STRICT" Label or CheckBox.
		/// @note Retains the current description if the CheckBox has
		/// input-focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseLeaveStrict(object sender, EventArgs e)
		{
			if (!cb_Strict.Focused)
				lbl_Description.Text = String.Empty;
		}

		/// <summary>
		/// Handles the "STRICT" checkbox and the "SpriteShade" textbox losing
		/// focus.
		/// @note Clears the current description disregarding mouseover state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLeave(object sender, EventArgs e)
		{
			lbl_Description.Text = String.Empty;
		}


		/// <summary>
		/// Handles IsoLoFT's trackbar's ValueChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnValueChanged_IsoLoft(object sender, EventArgs e)
		{
			pnl_IsoLoft.Invalidate();

			pnl_Loft08.Invalidate();
			pnl_Loft09.Invalidate();
			pnl_Loft10.Invalidate();
			pnl_Loft11.Invalidate();
			pnl_Loft12.Invalidate();
			pnl_Loft13.Invalidate();
			pnl_Loft14.Invalidate();
			pnl_Loft15.Invalidate();
			pnl_Loft16.Invalidate();
			pnl_Loft17.Invalidate();
			pnl_Loft18.Invalidate();
			pnl_Loft19.Invalidate();
		}
		#endregion Events


		#region Methods
		internal void statusbar_PrintSpriteInfo()
		{
			if (Spriteset != null)
			{
				tssl_Sprites.Text = "Sprites: " + Spriteset.Count;

				uint last, after;
				SpriteCollection.Test2byteSpriteset(Spriteset, out last, out after);

				tssl_OffsetLast .ForeColor = (last  > UInt16.MaxValue) ? Color.Crimson : SystemColors.ControlText;
				tssl_OffsetAfter.ForeColor = (after > UInt16.MaxValue) ? Color.Crimson : SystemColors.ControlText;

				tssl_OffsetLast .Text = "last: "  + last;
				tssl_OffsetAfter.Text = "after: " + after;
			}
			else
			{
				tssl_Sprites.Text = "Sprites: null";
				tssl_OffsetLast .Text =
				tssl_OffsetAfter.Text = String.Empty;
			}
		}

		internal bool isTftd()
		{
			return miPaletteTftd.Checked;
		}

		/// <summary>
		/// Gets the IsoLoFT's trackbar's current value.
		/// </summary>
		/// <returns></returns>
		internal int GetIsoLoftVal()
		{
			return bar_IsoLoft.Value;
		}

		/// <summary>
		/// Invalidates panels.
		/// </summary>
		/// <param name="lofts">incl/ lofts</param>
		internal void InvalidatePanels(bool lofts = true)
		{
			PartsPanel .Invalidate();
			pnl_Sprites.Invalidate();
			pnl_ScanGic.Invalidate();

			if (lofts)
			{
				pnl_Loft08.Invalidate();
				pnl_Loft09.Invalidate();
				pnl_Loft10.Invalidate();
				pnl_Loft11.Invalidate();
				pnl_Loft12.Invalidate();
				pnl_Loft13.Invalidate();
				pnl_Loft14.Invalidate();
				pnl_Loft15.Invalidate();
				pnl_Loft16.Invalidate();
				pnl_Loft17.Invalidate();
				pnl_Loft18.Invalidate();
				pnl_Loft19.Invalidate();

				pnl_IsoLoft.Invalidate();
			}
		}

		/// <summary>
		/// Populates all textfields with values of the currently selected
		/// record-id.
		/// </summary>
		internal void PopulateTextFields()
		{
			InitFields = true;

			McdRecord record = Parts[SelId].Record;

			tb00_phase0.Text = ((int)record.Sprite1).ToString();
			tb01_phase1.Text = ((int)record.Sprite2).ToString();
			tb02_phase2.Text = ((int)record.Sprite3).ToString();
			tb03_phase3.Text = ((int)record.Sprite4).ToString();
			tb04_phase4.Text = ((int)record.Sprite5).ToString();
			tb05_phase5.Text = ((int)record.Sprite6).ToString();
			tb06_phase6.Text = ((int)record.Sprite7).ToString();
			tb07_phase7.Text = ((int)record.Sprite8).ToString();

			tb08_loft00.Text = ((int)record.Loft1) .ToString();
			tb09_loft01.Text = ((int)record.Loft2) .ToString();
			tb10_loft02.Text = ((int)record.Loft3) .ToString();
			tb11_loft03.Text = ((int)record.Loft4) .ToString();
			tb12_loft04.Text = ((int)record.Loft5) .ToString();
			tb13_loft05.Text = ((int)record.Loft6) .ToString();
			tb14_loft06.Text = ((int)record.Loft7) .ToString();
			tb15_loft07.Text = ((int)record.Loft8) .ToString();
			tb16_loft08.Text = ((int)record.Loft9) .ToString();
			tb17_loft09.Text = ((int)record.Loft10).ToString();
			tb18_loft10.Text = ((int)record.Loft11).ToString();
			tb19_loft11.Text = ((int)record.Loft12).ToString();

			string scanG         = ((int)record.ScanG)        .ToString();	// NOTE: Yes, keep this outside the .Text setters.
			string scanG_reduced = ((int)record.ScanG_reduced).ToString();	// else only god knows why the cast from ushort won't work right.
			tb20_scang1.Text = scanG;										// See also the OnChanged mechanism ...
			tb20_scang2.Text = scanG_reduced;

			tb22_.Text = ((int)record.Unknown22).ToString();
			tb23_.Text = ((int)record.Unknown23).ToString();
			tb24_.Text = ((int)record.Unknown24).ToString();
			tb25_.Text = ((int)record.Unknown25).ToString();
			tb26_.Text = ((int)record.Unknown26).ToString();
			tb27_.Text = ((int)record.Unknown27).ToString();
			tb28_.Text = ((int)record.Unknown28).ToString();
			tb29_.Text = ((int)record.Unknown29).ToString();

			tb30_isslidingdoor.Text = Convert.ToInt32(record.SlidingDoor).ToString();
			tb31_isblocklos   .Text = Convert.ToInt32(record.StopLOS)    .ToString();
			tb32_isdropthrou  .Text = Convert.ToInt32(record.NotFloored) .ToString();
			tb33_isbigwall    .Text = Convert.ToInt32(record.BigWall)    .ToString();
			tb34_isgravlift   .Text = Convert.ToInt32(record.GravLift)   .ToString();
			tb35_ishingeddoor .Text = Convert.ToInt32(record.HingedDoor) .ToString();
			tb36_isblockfire  .Text = Convert.ToInt32(record.BlockFire)  .ToString();
			tb37_isblocksmoke .Text = Convert.ToInt32(record.BlockSmoke) .ToString();

			tb38_.Text = ((int)record.LeftRightHalf).ToString();

			tb39_tuwalk       .Text = ((int)record.TU_Walk)   .ToString();
			tb40_tuslide      .Text = ((int)record.TU_Slide)  .ToString();
			tb41_tufly        .Text = ((int)record.TU_Fly)    .ToString();
			tb42_armor        .Text = ((int)record.Armor)     .ToString();
			tb43_heblock      .Text = ((int)record.HE_Block)  .ToString();
			tb44_deathid      .Text = ((int)record.DieTile)   .ToString();
			tb45_fireresist   .Text = ((int)record.FireResist).ToString();
			tb46_alternateid  .Text = ((int)record.Alt_MCD)   .ToString();

			tb47_.Text = ((int)record.Unknown47).ToString();

			tb48_terrainoffset.Text = ((int)record.StandOffset).ToString();
			tb49_spriteoffset .Text = ((int)record.TileOffset) .ToString();

			tb50_.Text = ((int)record.Unknown50).ToString();

			tb51_lightblock    .Text = ((int)record.LightBlock)   .ToString();
			tb52_footsound     .Text = ((int)record.Footstep)     .ToString();
			tb53_parttype      .Text = ((int)record.PartType)     .ToString();
			tb54_hetype        .Text = ((int)record.HE_Type)      .ToString();
			tb55_hestrength    .Text = ((int)record.HE_Strength)  .ToString();
			tb56_smokeblock    .Text = ((int)record.SmokeBlockage).ToString();
			tb57_fuel          .Text = ((int)record.Fuel)         .ToString();
			tb58_lightintensity.Text = ((int)record.LightSource)  .ToString();
			tb59_specialtype   .Text = ((int)record.Special)      .ToString();

			tb60_isbaseobject.Text = Convert.ToInt32(record.BaseObject).ToString();

			tb61_.Text = ((int)record.Unknown61).ToString();

			InitFields = false;
		}

		/// <summary>
		/// Clears all textfields.
		/// </summary>
		/// <param name="zero">true to "0" all textfields</param>
		internal void ClearTextFields(bool zero = false)
		{
			string text;
			if (zero)
				text = "0";
			else
				text = String.Empty;

			tb00_phase0.Text =
			tb01_phase1.Text =
			tb02_phase2.Text =
			tb03_phase3.Text =
			tb04_phase4.Text =
			tb05_phase5.Text =
			tb06_phase6.Text =
			tb07_phase7.Text =

			tb08_loft00.Text =
			tb09_loft01.Text =
			tb10_loft02.Text =
			tb11_loft03.Text =
			tb12_loft04.Text =
			tb13_loft05.Text =
			tb14_loft06.Text =
			tb15_loft07.Text =
			tb16_loft08.Text =
			tb17_loft09.Text =
			tb18_loft10.Text =
			tb19_loft11.Text =

			tb20_scang1.Text =
			tb20_scang2.Text =

			tb22_.Text =
			tb23_.Text =
			tb24_.Text =
			tb25_.Text =
			tb26_.Text =
			tb27_.Text =
			tb28_.Text =
			tb29_.Text =

			tb30_isslidingdoor.Text =
			tb31_isblocklos   .Text =
			tb32_isdropthrou  .Text =
			tb33_isbigwall    .Text =
			tb34_isgravlift   .Text =
			tb35_ishingeddoor .Text =
			tb36_isblockfire  .Text =
			tb37_isblocksmoke .Text =

			tb38_.Text =

			tb39_tuwalk     .Text =
			tb40_tuslide    .Text =
			tb41_tufly      .Text =
			tb42_armor      .Text =
			tb43_heblock    .Text =
			tb44_deathid    .Text =
			tb45_fireresist .Text =
			tb46_alternateid.Text =

			tb47_.Text =

			tb48_terrainoffset.Text =
			tb49_spriteoffset .Text =

			tb50_.Text =

			tb51_lightblock    .Text =
			tb52_footsound     .Text =
			tb53_parttype      .Text =
			tb54_hetype        .Text =
			tb55_hestrength    .Text =
			tb56_smokeblock    .Text =
			tb57_fuel          .Text =
			tb58_lightintensity.Text =
			tb59_specialtype   .Text =
			tb60_isbaseobject  .Text =

			tb61_.Text = text;
		}

		/// <summary>
		/// Tests all values for STRICT.
		/// @note These tests shall stay synched w/ McdviewF_changed events.
		/// @note Tests are done against the values that are currently stored in
		/// the records - not against the values shown in the textboxes.
		/// </summary>
		/// <returns>a list of borks if any</returns>
		private List<string> GetStrictBorks()
		{
			var borks = new List<string>();

			int val, y;

			// Sprites
			if (Spriteset != null)
			{
				if ((y = Spriteset.Count - 1) > 255)
				{
					borks.Add("#The terrain's spriteset count exceeds 256 but an MCD record"
							+ Environment.NewLine
							+ "cannot deal with a sprite ref in excess of 256 because the"
							+ Environment.NewLine
							+ "MCD values of sprite refs are stored in a single byte (0..255)");
				}
			}
			else
				y = 255;

			val = Parts[SelId].Record.Sprite1;
			if (val != Int32.Parse(tb00_phase0.Text))
			{
				borks.Add("#0 phase 1 (record) does not equal phase 1 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#0 phase 1 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite2;
			if (val != Int32.Parse(tb01_phase1.Text))
			{
				borks.Add("#1 phase 2 (record) does not equal phase 2 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#1 phase 2 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite3;
			if (val != Int32.Parse(tb02_phase2.Text))
			{
				borks.Add("#2 phase 3 (record) does not equal phase 3 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#2 phase 3 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite4;
			if (val != Int32.Parse(tb03_phase3.Text))
			{
				borks.Add("#3 phase 4 (record) does not equal phase 4 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#3 phase 4 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite5;
			if (val != Int32.Parse(tb04_phase4.Text))
			{
				borks.Add("#4 phase 5 (record) does not equal phase 5 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#4 phase 5 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite6;
			if (val != Int32.Parse(tb05_phase5.Text))
			{
				borks.Add("#5 phase 6 (record) does not equal phase 6 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#5 phase 6 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite7;
			if (val != Int32.Parse(tb06_phase6.Text))
			{
				borks.Add("#6 phase 7 (record) does not equal phase 7 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#6 phase 7 id exceeds the terrain's spriteset count.");
			}

			val = Parts[SelId].Record.Sprite8;
			if (val != Int32.Parse(tb07_phase7.Text))
			{
				borks.Add("#7 phase 8 (record) does not equal phase 8 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#7 phase 8 id exceeds the terrain's spriteset count.");
			}


			// LoFTs
			if (LoFT != null)
			{
				if ((y = LoFT.Length / 256 - 1) > 255)
				{
					borks.Add("#The LoFTs count exceeds 256 but an MCD record cannot deal"
							+ Environment.NewLine
							+ "with a LoFT ref in excess of 256 because the MCD values"
							+ Environment.NewLine
							+ "of LoFT refs are stored in a single byte (0..255)");
				}
			}
			else
				y = 255;

			val = Parts[SelId].Record.Loft1;
			if (val != Int32.Parse(tb08_loft00.Text))
			{
				borks.Add("#8 loft 1 (record) does not equal loft 1 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#8 loft 1 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft2;
			if (val != Int32.Parse(tb09_loft01.Text))
			{
				borks.Add("#9 loft 2 (record) does not equal loft 2 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#9 loft 2 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft3;
			if (val != Int32.Parse(tb10_loft02.Text))
			{
				borks.Add("#10 loft 3 (record) does not equal loft 3 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#10 loft 3 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft4;
			if (val != Int32.Parse(tb11_loft03.Text))
			{
				borks.Add("#11 loft 4 (record) does not equal loft 4 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#11 loft 4 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft5;
			if (val != Int32.Parse(tb12_loft04.Text))
			{
				borks.Add("#12 loft 5 (record) does not equal loft 5 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#12 loft 5 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft6;
			if (val != Int32.Parse(tb13_loft05.Text))
			{
				borks.Add("#13 loft 6 (record) does not equal loft 6 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#13 loft 6 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft7;
			if (val != Int32.Parse(tb14_loft06.Text))
			{
				borks.Add("#14 loft 7 (record) does not equal loft 7 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#14 loft 7 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft8;
			if (val != Int32.Parse(tb15_loft07.Text))
			{
				borks.Add("#15 loft 8 (record) does not equal loft 8 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#15 loft 8 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft9;
			if (val != Int32.Parse(tb16_loft08.Text))
			{
				borks.Add("#16 loft 9 (record) does not equal loft 9 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#16 loft 9 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft10;
			if (val != Int32.Parse(tb17_loft09.Text))
			{
				borks.Add("#17 loft 10 (record) does not equal loft 10 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#17 loft 10 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft11;
			if (val != Int32.Parse(tb18_loft10.Text))
			{
				borks.Add("#18 loft 11 (record) does not equal loft 11 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#18 loft 11 id exceeds the LoFT count.");
			}

			val = Parts[SelId].Record.Loft12;
			if (val != Int32.Parse(tb19_loft11.Text))
			{
				borks.Add("#19 loft 12 (record) does not equal loft 12 (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#19 loft 12 id exceeds the LoFT count.");
			}


			// ScanG
			if (ScanG != null)
			{
				if ((y = ScanG.Length / 16 - 1) > 65535)
				{
					borks.Add("#The ScanG count exceeds 65536 but an MCD record cannot deal"
							+ Environment.NewLine
							+ "with a ScanG ref in excess of 65536 because the MCD values"
							+ Environment.NewLine
							+ "of ScanG refs are stored in an unsigned short (0..65535)"); // yeah, right.
				}
			}
			else
				y = 65535;

			val = Parts[SelId].Record.ScanG;
			if (val != Int32.Parse(tb20_scang1.Text))
			{
				borks.Add("#20 ScanG (record) does not equal ScanG (text).");
			}
			if (!Test(val, 35, y + 35))
			{
				borks.Add("#20 ScanG id is outside the ScanG limits.");
			}

			val = Parts[SelId].Record.ScanG_reduced;
			if (val != Int32.Parse(tb20_scang2.Text))
			{
				borks.Add("#20 ScanG_reduced (record) does not equal ScanG_reduced (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#20 ScanG_reduced id exceeds the ScanG count.");
			}


			// RAM addresses
			val = Parts[SelId].Record.Unknown22;
			if (val != Int32.Parse(tb22_.Text))
			{
				borks.Add("#22 tab ram (record) does not equal tab ram (text).");
			}
			val = Parts[SelId].Record.Unknown23;
			if (val != Int32.Parse(tb23_.Text))
			{
				borks.Add("#23 tab ram (record) does not equal tab ram (text).");
			}
			val = Parts[SelId].Record.Unknown24;
			if (val != Int32.Parse(tb24_.Text))
			{
				borks.Add("#24 tab ram (record) does not equal tab ram (text).");
			}
			val = Parts[SelId].Record.Unknown25;
			if (val != Int32.Parse(tb25_.Text))
			{
				borks.Add("#25 tab ram (record) does not equal tab ram (text).");
			}
			val = Parts[SelId].Record.Unknown26;
			if (val != Int32.Parse(tb26_.Text))
			{
				borks.Add("#26 pck ram (record) does not equal pck ram (text).");
			}
			val = Parts[SelId].Record.Unknown27;
			if (val != Int32.Parse(tb27_.Text))
			{
				borks.Add("#27 pck ram (record) does not equal pck ram (text).");
			}
			val = Parts[SelId].Record.Unknown28;
			if (val != Int32.Parse(tb28_.Text))
			{
				borks.Add("#28 pck ram (record) does not equal pck ram (text).");
			}
			val = Parts[SelId].Record.Unknown29;
			if (val != Int32.Parse(tb29_.Text))
			{
				borks.Add("#29 pck ram (record) does not equal pck ram (text).");
			}


			// booleans
			bool valB;

			valB = Parts[SelId].Record.SlidingDoor;
			if (    (valB && tb30_isslidingdoor.Text == "0")
				|| (!valB && tb30_isslidingdoor.Text == "1"))
			{
				borks.Add("#30 isSlidingDoor (record) does not equal isSlidingDoor (text).");
			}
			if (valB && tb35_ishingeddoor.Text == "1")
			{
				borks.Add("#30 isSlidingDoor and #35 isHingedDoor are both true.");
			}
			valB = Parts[SelId].Record.StopLOS;
			if (    (valB && tb31_isblocklos.Text == "0")
				|| (!valB && tb31_isblocklos.Text == "1"))
			{
				borks.Add("#31 isBlockLoS (record) does not equal isBlockLoS (text).");
			}
			valB = Parts[SelId].Record.NotFloored;
			if (    (valB && tb32_isdropthrou.Text == "0")
				|| (!valB && tb32_isdropthrou.Text == "1"))
			{
				borks.Add("#32 isDropThrou (record) does not equal isDropThrou (text).");
			}
			valB = Parts[SelId].Record.BigWall;
			if (    (valB && tb33_isbigwall.Text == "0")
				|| (!valB && tb33_isbigwall.Text == "1"))
			{
				borks.Add("#33 isBigWall (record) does not equal isBigWall (text).");
			}
			valB = Parts[SelId].Record.GravLift;
			if (    (valB && tb34_isgravlift.Text == "0")
				|| (!valB && tb34_isgravlift.Text == "1"))
			{
				borks.Add("#34 isGravLift (record) does not equal isGravLift (text).");
			}
			valB = Parts[SelId].Record.HingedDoor;
			if (    (valB && tb35_ishingeddoor.Text == "0")
				|| (!valB && tb35_ishingeddoor.Text == "1"))
			{
				borks.Add("#35 isHingedDoor (record) does not equal isHingedDoor (text).");
			}
			if (valB && tb30_isslidingdoor.Text == "1")
			{
				borks.Add("#35 isHingedDoor and #30 isSlidingDoor are both true.");
			}
			valB = Parts[SelId].Record.BlockFire;
			if (    (valB && tb36_isblockfire.Text == "0")
				|| (!valB && tb36_isblockfire.Text == "1"))
			{
				borks.Add("#36 isBlockFire (record) does not equal isBlockFire (text).");
			}
			valB = Parts[SelId].Record.BlockSmoke;
			if (    (valB && tb37_isblocksmoke.Text == "0")
				|| (!valB && tb37_isblocksmoke.Text == "1"))
			{
				borks.Add("#37 isBlockSmoke (record) does not equal isBlockSmoke (text).");
			}


			// ints
			val = Parts[SelId].Record.LeftRightHalf;
			if (val != Int32.Parse(tb38_.Text))
			{
				borks.Add("#38 LeftRightHalf (record) does not equal LeftRightHalf (text).");
			}
//			if (val != 3)
//			{
//				borks.Add("#38 LeftRightHalf is not \"3\".");
//			}

			val = Parts[SelId].Record.TU_Walk;
			if (val != Int32.Parse(tb39_tuwalk.Text))
			{
				borks.Add("#39 TuWalk (record) does not equal TuWalk (text).");
			}
			val = Parts[SelId].Record.TU_Slide;
			if (val != Int32.Parse(tb40_tuslide.Text))
			{
				borks.Add("#40 TuSlide (record) does not equal TuSlide (text).");
			}
			val = Parts[SelId].Record.TU_Fly;
			if (val != Int32.Parse(tb41_tufly.Text))
			{
				borks.Add("#41 TuFly (record) does not equal TuFly (text).");
			}

			val = Parts[SelId].Record.Armor;
			if (val != Int32.Parse(tb42_armor.Text))
			{
				borks.Add("#42 Armor (record) does not equal Armor (text).");
			}

			val = Parts[SelId].Record.HE_Block;
			if (val != Int32.Parse(tb43_heblock.Text))
			{
				borks.Add("#43 HeBlock (record) does not equal HeBlock (text).");
			}


			y = Parts.Length - 1;

			val = Parts[SelId].Record.DieTile;
			if (val != Int32.Parse(tb44_deathid.Text))
			{
				borks.Add("#44 DeathId (record) does not equal DeathId (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#44 DeathId exceeds the count of the MCD.");
			}
			if (val != 0 && Parts[SelId].Dead == null)
			{
				borks.Add("#44 Dead part is null.");
			}

			val = Parts[SelId].Record.FireResist;
			if (val != Int32.Parse(tb45_fireresist.Text))
			{
				borks.Add("#45 FireResist (record) does not equal FireResist (text).");
			}

			val = Parts[SelId].Record.Alt_MCD;
			if (val != Int32.Parse(tb46_alternateid.Text))
			{
				borks.Add("#46 AlternateId (record) does not equal AlternateId (text).");
			}
			if (!Test(val, 0, y))
			{
				borks.Add("#46 AlternateId exceeds the count of the MCD.");
			}
			if (val != 0 && Parts[SelId].Altr == null)
			{
				borks.Add("#46 Alternate part is null.");
			}

			val = Parts[SelId].Record.Unknown47;
			if (val != Int32.Parse(tb47_.Text))
			{
				borks.Add("#47 CloseDoors (record) does not equal CloseDoors (text).");
			}

			val = Parts[SelId].Record.StandOffset;
			if (val != Int32.Parse(tb48_terrainoffset.Text))
			{
				borks.Add("#48 TerrainOffset (record) does not equal TerrainOffset (text).");
			}
			if (!Test(val, -24, 0))
			{
				borks.Add("#48 TerrainOffset has an unusual value.");
			}

			val = Parts[SelId].Record.TileOffset;
			if (val != Int32.Parse(tb49_spriteoffset.Text))
			{
				borks.Add("#49 SpriteOffset (record) does not equal SpriteOffset (text).");
			}
			if (!Test(val, 0, 24))
			{
				borks.Add("#49 SpriteOffset has an unusual value.");
			}

			val = Parts[SelId].Record.Unknown50;
			if (val != Int32.Parse(tb50_.Text))
			{
				borks.Add("#50 dTypeMod (record) does not equal dTypeMod (text).");
			}

			val = Parts[SelId].Record.LightBlock;
			if (val != Int32.Parse(tb51_lightblock.Text))
			{
				borks.Add("#51 LightBlock (record) does not equal LightBlock (text).");
			}

			val = Parts[SelId].Record.Footstep;
			if (val != Int32.Parse(tb52_footsound.Text))
			{
				borks.Add("#52 FootSound (record) does not equal FootSound (text).");
			}
			if (!Test(val, 0, 6))
			{
				borks.Add("#52 FootSound exceeds expected value.");
			}

			val = (int)Parts[SelId].Record.PartType;
			if (val != Int32.Parse(tb53_parttype.Text))
			{
				borks.Add("#53 PartType (record) does not equal PartType (text).");
			}
			if (!Test(val, 0, 3))
			{
				borks.Add("#53 PartType exceeds expected value.");
			}

			val = Parts[SelId].Record.HE_Type;
			if (val != Int32.Parse(tb54_hetype.Text))
			{
				borks.Add("#54 HeType (record) does not equal HeType (text).");
			}
			if (!Test(val, 0, 1))
			{
				borks.Add("#54 HeType exceeds expected value.");
			}

			val = Parts[SelId].Record.HE_Strength;
			if (val != Int32.Parse(tb55_hestrength.Text))
			{
				borks.Add("#55 HeStrength (record) does not equal HeStrength (text).");
			}

			val = Parts[SelId].Record.SmokeBlockage;
			if (val != Int32.Parse(tb56_smokeblock.Text))
			{
				borks.Add("#56 SmokeBlock (record) does not equal SmokeBlock (text).");
			}

			val = Parts[SelId].Record.Fuel;
			if (val != Int32.Parse(tb57_fuel.Text))
			{
				borks.Add("#57 Fuel (record) does not equal Fuel (text).");
			}

			val = Parts[SelId].Record.LightSource;
			if (val != Int32.Parse(tb58_lightintensity.Text))
			{
				borks.Add("#58 LightIntensity (record) does not equal LightIntensity (text).");
			}

			val = (int)Parts[SelId].Record.Special;
			if (val != Int32.Parse(tb59_specialtype.Text))
			{
				borks.Add("#59 SpecialType (record) does not equal SpecialType (text).");
			}
			if (!Test(val, 0, 14))
			{
				borks.Add("#59 SpecialType exceeds expected value.");
			}

			valB = Parts[SelId].Record.BaseObject;
			if (    (valB && tb60_isbaseobject.Text == "0")
				|| (!valB && tb60_isbaseobject.Text == "1"))
			{
				borks.Add("#60 isBaseObject (record) does not equal isBaseObject (text).");
			}

			val = Parts[SelId].Record.Unknown61;
			if (val != Int32.Parse(tb61_.Text))
			{
				borks.Add("#61 VictoryPoints (record) does not equal VictoryPoints (text).");
			}

			return borks;
		}

		/// <summary>
		/// Checks if a specified value is within specified parameters.
		/// </summary>
		/// <param name="val">a value to test</param>
		/// <param name="x">the minimum acceptable value</param>
		/// <param name="y">the maximum acceptable value</param>
		/// <returns></returns>
		private bool Test(int val, int x, int y)
		{
			return (val >= x && val <= y);
		}
		#endregion Methods
	}
}
