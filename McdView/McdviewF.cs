﻿using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Windows;

using XCom;
using XCom.Resources.Map;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace McdView
{
	/// <summary>
	/// 
	/// </summary>
	internal partial class McdviewF
		:
			Form
	{
		#region Fields (static)
		internal readonly static Brush BrushHilight = new SolidBrush(Color.FromArgb(69, SystemColors.MenuHighlight));

		internal const TextFormatFlags FLAGS = TextFormatFlags.HorizontalCenter
											 | TextFormatFlags.VerticalCenter
											 | TextFormatFlags.NoPadding;
		#endregion Fields (static)


		#region Fields
		private string _pfeMcd;
		internal string Label;

		private RecordsetPanel RecordsPanel;
		internal int[,] ScanG;
		internal BitArray LoFT;

		private readonly Pen _penBlack = new Pen(Color.Black, 1);
		private readonly Pen _penGray  = new Pen(Color.LightGray, 1);

		private bool strict = true;
		private bool InitFields;
		#endregion Fields


		#region Properties
		private Tilepart[] _records;
		private Tilepart[] Records
		{
			get { return _records; }
			set
			{
				RecordsPanel.Records = (_records = value);
			}
		}

		private SpriteCollection _spriteset;
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			private set
			{
				miPaletteMenu.Enabled = ((_spriteset = value) != null);
			}
		}


		internal bool _spriteShadeEnabled;

		private int _spriteShadeInt = 11;
		private int SpriteShadeInt
		{
			get { return _spriteShadeInt; }
			set
			{
				if (_spriteShadeEnabled = ((_spriteShadeInt = value) != -1))
					SpriteShadeFloat = ((float)_spriteShadeInt * 0.03f); // NOTE: 33 is unity.

				InvalidatePanels(false);
			}
		}
		internal float SpriteShadeFloat
		{ get; private set; }


		private int _selId = -1;
		internal int SelId
		{
			get { return _selId; }
			set
			{
				if (_selId != value)
				{
					if ((_selId = value) != -1)
					{
						bool strict0 = strict;
						strict = false;
						PopulateTextFields();
						strict = strict0;

						RecordsPanel.ScrollTile();
					}
					else
						ClearTextFields();

					InvalidatePanels();
				}
			}
		}


		private bool _changed;
		private bool Changed
		{
			get { return _changed; }
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
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates the McdView app.
		/// </summary>
		internal McdviewF()
		{
#if DEBUG
			LogFile.SetLogFilePath(Path.GetDirectoryName(Application.ExecutablePath)); // creates a logfile/ wipes the old one.
#endif

			InitializeComponent();
			SetDoubleBuffered(pnl_Sprites);
			SetDoubleBuffered(pnl_IsoLoft);

			MaximumSize = new Size(0,0);

			LoadWindowMetrics();

			RecordsPanel = new RecordsetPanel(this);
			gb_Collection.Controls.Add(RecordsPanel);
			RecordsPanel.Width = gb_Collection.Width - 10;

			tb_SpriteShade.Text = SpriteShadeInt.ToString();

			RecordsPanel.Select();

			LayoutSpriteGroup();

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
		}

		/// <summary>
		/// Some controls, such as the DataGridView, do not allow setting the
		/// DoubleBuffered property. It is set as a protected property. This
		/// method is a work-around to allow setting it. Call this in the
		/// constructor just after InitializeComponent().
		/// https://stackoverflow.com/questions/118528/horrible-redraw-performance-of-the-datagridview-on-one-of-my-two-screens#answer-16625788
		/// @note I wonder if this works on Mono. It stops the redraw-flick when
		/// setting the anisprite on return from SpritesetviewF on my system
		/// (Win7-64). Also stops flicker on the IsoLoft panel.
		/// </summary>
		/// <param name="control">the Control on which to set DoubleBuffered to true</param>
		private static void SetDoubleBuffered(object control)
		{
			// if not remote desktop session then enable double-buffering optimization
			if (!SystemInformation.TerminalServerSession)
			{
				// set instance non-public property with name "DoubleBuffered" to true
				typeof(Control).InvokeMember("DoubleBuffered",
											 System.Reflection.BindingFlags.SetProperty
										   | System.Reflection.BindingFlags.Instance
										   | System.Reflection.BindingFlags.NonPublic,
											 null,
											 control,
											 new object[] { true });
			}
		}

		/// <summary>
		/// Assigns MapView's Configurator's basepath to 'pathufo' and 'pathtftd'.
		/// </summary>
		/// <param name="pathufo"></param>
		/// <param name="pathtftd"></param>
		private void GetResourcePaths(out string pathufo, out string pathtftd)
		{
			pathufo  = null;
			pathtftd = null;

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
					var str = new YamlStream();
					str.Load(sr);

					string val;

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
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


		#region Load/Save 'registry' info
		/// <summary>
		/// Positions the window at user-defined coordinates w/ size.
		/// @note Adapted from PckViewForm.
		/// </summary>
		private void LoadWindowMetrics()
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers); // "MapViewers.yml"
			if (File.Exists(fileViewers))
			{
				using (var sr = new StreamReader(File.OpenRead(fileViewers)))
				{
					var str = new YamlStream();
					str.Load(sr);

					var invariant = System.Globalization.CultureInfo.InvariantCulture;

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in nodeRoot.Children)
					{
						string viewer = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(viewer, RegistryInfo.McdView, StringComparison.Ordinal))
						{
							int x = 0;
							int y = 0;
							int w = 0;
							int h = 0;

							var keyvals = nodeRoot.Children[new YamlScalarNode(viewer)] as YamlMappingNode;
							foreach (var keyval in keyvals) // NOTE: There is a better way to do this. See TilesetLoader..cTor
							{
								switch (keyval.Key.ToString()) // TODO: Error handling. ->
								{
									case "left":
										x = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "top":
										y = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "width":
										w = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "height":
										h = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
								}
							}

							var rectScreen = Screen.GetWorkingArea(new Point(x, y));
							if (!rectScreen.Contains(x + 200, y + 100)) // check to ensure that McdView is at least partly onscreen.
							{
								x = 100;
								y =  50;
							}

							Left = x;
							Top  = y;

							ClientSize = new Size(w, h);
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves the window position and size to YAML.
		/// </summary>
		private void SaveWindowMetrics()
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers); // "MapViewers.yml"

			if (File.Exists(fileViewers))
			{
				WindowState = FormWindowState.Normal;

				string src = Path.Combine(dirSettings, PathInfo.ConfigViewers);
				string dst = Path.Combine(dirSettings, PathInfo.ConfigViewersOld);

				File.Copy(src, dst, true);

				using (var sr = new StreamReader(File.OpenRead(dst))) // but now use dst as src ->

				using (var fs = new FileStream(src, FileMode.Create)) // overwrite previous viewers-config.
				using (var sw = new StreamWriter(fs))
				{
					bool found = false;

					while (sr.Peek() != -1)
					{
						string line = sr.ReadLine().TrimEnd();

						if (String.Equals(line, RegistryInfo.McdView + ":", StringComparison.Ordinal))
						{
							found = true;

							sw.WriteLine(line);

							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine(); // heh

							sw.WriteLine("  left: "   + Math.Max(0, Location.X));	// =Left
							sw.WriteLine("  top: "    + Math.Max(0, Location.Y));	// =Top
							sw.WriteLine("  width: "  + ClientSize.Width);			// <- use ClientSize, since Width and Height
							sw.WriteLine("  height: " + ClientSize.Height);			// screw up due to the titlebar/menubar area.
						}
						else
							sw.WriteLine(line);
					}

					if (!found)
					{
						sw.WriteLine(RegistryInfo.McdView + ":");

						sw.WriteLine("  left: "   + Math.Max(0, Location.X));
						sw.WriteLine("  top: "    + Math.Max(0, Location.Y));
						sw.WriteLine("  width: "  + ClientSize.Width);
						sw.WriteLine("  height: " + ClientSize.Height);
					}
				}
				File.Delete(dst);
			}
		}
		#endregion Load/Save 'registry' info


		#region Events (override)
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			SaveWindowMetrics();
			base.OnFormClosing(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			gb_Description.Height = ClientSize.Height - lbl_Strict.Location.Y - lbl_Strict.Height - 25;
			gb_Description.Location = new Point(
											gb_Description.Location.X,
											lbl_Strict.Location.Y + lbl_Strict.Height + 25);
		}

		/// <summary>
		/// The joys of keyboard events in Winforms. Bypasses forwarding a
		/// keyboard-event to the RecordsPanel if a control that should use the
		/// keyboard-input instead currently has focus already. blah blah blah
		/// @note Requires 'KeyPreview' true.
		/// @note The STRICT CheckBox (focused) will allow all keyboard-input to
		/// forward to the RecordsPanel - except the arrow-keys. foffff
		/// @note Keys that need to be forwarded: Arrows Up/Down/Left/Right,
		/// PageUp/Down, Home/End ... and Delete when editing an MCD.
		/// @note Holy fuck. I make the RecordsPanel selectable w/ TabStop and
		/// - lo && behold - the arrow-keys no longer get forwarded. lovely
		/// So, set IsInputKey() for the arrow-keys in the RecordsPanel. lovely
		/// @ IMPORTANT: If any other (types of) controls that can accept focus
		/// are added to this Form they need to be accounted for here.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (SelId == -1)
			{
				if (e.KeyCode == Keys.Space // select record #0 ->
					&& Records != null
					&& Records.Length != 0)
				{
					if (!cb_Strict.Focused && !bar_SpriteShade.Focused && !bar_IsoLoft.Focused)
					{
						RecordsPanel.Select();
						SelId = 0;
					}
				}
			}
			else if (!cb_Strict.Focused && !bar_IsoLoft.Focused && !bar_SpriteShade.Focused)
			{
				foreach (Control control in Controls)
				{
					if (control.Focused && (control as TextBox) != null)
						return;

//					if ((control as GroupBox) != null)
//					{
					foreach (Control control1 in control.Controls)
					{
						if (control1.Focused && (control1 as TextBox) != null)
							return;
					}
//					}
				}
				RecordsPanel.KeyTile(e);
			}
		}
		#endregion Events (override)


		#region Menuitems
		private void OnClick_Open(object sender, EventArgs e)
		{
			// TODO: Check changed.
//			if (Changed)
//			{
//			}

			using (var ofd = new OpenFileDialog())
			{
				ofd.Title      = "Open an MCD file";
				ofd.DefaultExt = "MCD";
				ofd.Filter     = "MCD files (*.MCD)|*.MCD|All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ResourceInfo.ReloadSprites = true;

					_pfeMcd = ofd.FileName;
					Label = Path.GetFileNameWithoutExtension(_pfeMcd);

					using (var bs = new BufferedStream(File.OpenRead(_pfeMcd)))
					{
						Records = new Tilepart[(int)bs.Length / TilepartFactory.Length]; // TODO: Error if this don't work out right.

						Palette pal;
						if (miPaletteUfo.Checked)
							pal = Palette.UfoBattle;
						else
							pal = Palette.TftdBattle;

						// NOTE: The spriteset is also maintained by a pointer
						// to it that's stored in each tilepart.
						Spriteset = ResourceInfo.LoadSpriteset(
															Label,
															Path.GetDirectoryName(_pfeMcd),
															2,
															pal);

						for (int id = 0; id != Records.Length; ++id)
						{
							var bindata = new byte[TilepartFactory.Length];
							bs.Read(bindata, 0, TilepartFactory.Length);
							McdRecord record = McdRecordFactory.CreateRecord(bindata);

							Records[id] = new Tilepart(id, Spriteset, record);
						}

						for (int id = 0; id != Records.Length; ++id)
						{
							Records[id].Dead      = TilepartFactory.GetDeadPart(     Label, id, Records[id].Record, Records);
							Records[id].Alternate = TilepartFactory.GetAlternatePart(Label, id, Records[id].Record, Records);
						}
					}

					SelId = -1;
					ResourceInfo.ReloadSprites = false;

					_changed = false;
					Text = "McdView - " + _pfeMcd;
				}
			}
		}


		private void OnClick_PaletteUfo(object sender, EventArgs e)
		{
			if (!miPaletteUfo.Checked)
			{
				miPaletteUfo .Checked = true;
				miPaletteTftd.Checked = false;

				Spriteset.Pal = Palette.UfoBattle;
				ScanG = ResourceInfo.ScanGufo;
				LoFT  = ResourceInfo.LoFTufo;

				InvalidatePanels();
			}
		}

		private void OnClick_PaletteTftd(object sender, EventArgs e)
		{
			if (!miPaletteTftd.Checked)
			{
				miPaletteTftd.Checked = true;
				miPaletteUfo .Checked = false;

				Spriteset.Pal = Palette.TftdBattle;
				ScanG = ResourceInfo.ScanGtftd;
				LoFT  = ResourceInfo.LoFTtftd;

				InvalidatePanels();
			}
		}
		#endregion Menuitems


		#region Events
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

		private void OnValueChanged_SpriteShade(object sender, EventArgs e)
		{
			int val = bar_SpriteShade.Value;
			if (val == 0)
				val = -1;

			tb_SpriteShade.Text = val.ToString();
		}

		private void OnCheckChanged_Strict(object sender, EventArgs e)
		{
			if (strict = cb_Strict.Checked)
			{
				lbl_Strict.ForeColor = SystemColors.ControlText;

				if (SelId != -1)
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
				}
			}
			else
				lbl_Strict.ForeColor = Color.Red;
		}

		private void OnValueChanged_IsoLoft(object sender, EventArgs e)
		{
			pnl_IsoLoft.Invalidate();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Invalidates panels.
		/// </summary>
		/// <param name="lofts">incl/ lofts</param>
		private void InvalidatePanels(bool lofts = true)
		{
			RecordsPanel.Invalidate();
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

			// TODO: This is going to cause a lot of problems if/when user loads
			// a nonstandard record that has values that are outside the strict-
			// bounds. Ie, the vals will set to standard defaults.
			// 
			// So don't turn 'strict' on until after the record loads/populates
			// the textfields. See 'SelId' setter

			McdRecord record = Records[SelId].Record;

			tb0_phase0.Text = ((int)record.Sprite1).ToString();
			tb1_phase1.Text = ((int)record.Sprite2).ToString();
			tb2_phase2.Text = ((int)record.Sprite3).ToString();
			tb3_phase3.Text = ((int)record.Sprite4).ToString();
			tb4_phase4.Text = ((int)record.Sprite5).ToString();
			tb5_phase5.Text = ((int)record.Sprite6).ToString();
			tb6_phase6.Text = ((int)record.Sprite7).ToString();
			tb7_phase7.Text = ((int)record.Sprite8).ToString();

			tb8_loft00 .Text = ((int)record.Loft1) .ToString();
			tb9_loft02 .Text = ((int)record.Loft2) .ToString();
			tb10_loft04.Text = ((int)record.Loft3) .ToString();
			tb11_loft06.Text = ((int)record.Loft4) .ToString();
			tb12_loft08.Text = ((int)record.Loft5) .ToString();
			tb13_loft10.Text = ((int)record.Loft6) .ToString();
			tb14_loft12.Text = ((int)record.Loft7) .ToString();
			tb15_loft14.Text = ((int)record.Loft8) .ToString();
			tb16_loft16.Text = ((int)record.Loft9) .ToString();
			tb17_loft18.Text = ((int)record.Loft10).ToString();
			tb18_loft20.Text = ((int)record.Loft11).ToString();
			tb19_loft22.Text = ((int)record.Loft12).ToString();

			//LogFile.WriteLine("record.ScanG= " + record.ScanG);
			//LogFile.WriteLine("record.ScanG_reduced= " + record.ScanG_reduced);
//			tb20_scang1.Text = ((int)record.ScanG)        .ToString(); // no.
//			tb20_scang2.Text = ((int)record.ScanG_reduced).ToString();
			string scanG         = ((int)record.ScanG)        .ToString();	// NOTE: Yes, keep this outside the .Text setters.
			string scanG_reduced = ((int)record.ScanG_reduced).ToString();	// god only knows why else the cast from ushort won't work right.
			tb20_scang1.Text = scanG;										// TODO: There could still be probls in the OnChanged mechanism ...
			tb20_scang2.Text = scanG_reduced;
			//LogFile.WriteLine("tb20_scang1.Text= " + tb20_scang1.Text);
			//LogFile.WriteLine("tb20_scang2.Text= " + tb20_scang2.Text);

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

			tb39_tuwalk       .Text = ((int)record.TU_Walk)      .ToString();
			tb40_tuslide      .Text = ((int)record.TU_Slide)     .ToString();
			tb41_tufly        .Text = ((int)record.TU_Fly)       .ToString();
			tb42_armor        .Text = ((int)record.Armor)        .ToString();
			tb43_heblock      .Text = ((int)record.HE_Block)     .ToString();
			tb44_deathid      .Text = ((int)record.DieTile)      .ToString();
			tb45_fireresist   .Text = ((int)record.Flammable)    .ToString();
			tb46_alternateid  .Text = ((int)record.Alt_MCD)      .ToString();

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
		internal void ClearTextFields()
		{
			tb0_phase0.Text =
			tb1_phase1.Text =
			tb2_phase2.Text =
			tb3_phase3.Text =
			tb4_phase4.Text =
			tb5_phase5.Text =
			tb6_phase6.Text =
			tb7_phase7.Text =

			tb8_loft00 .Text =
			tb9_loft02 .Text =
			tb10_loft04.Text =
			tb11_loft06.Text =
			tb12_loft08.Text =
			tb13_loft10.Text =
			tb14_loft12.Text =
			tb15_loft14.Text =
			tb16_loft16.Text =
			tb17_loft18.Text =
			tb18_loft20.Text =
			tb19_loft22.Text =

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

			tb39_tuwalk       .Text =
			tb40_tuslide      .Text =
			tb41_tufly        .Text =
			tb42_armor        .Text =
			tb43_heblock      .Text =
			tb44_deathid      .Text =
			tb45_fireresist   .Text =
			tb46_alternateid  .Text =

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

			tb61_.Text = String.Empty;
		}
		#endregion Methods
	}
}
