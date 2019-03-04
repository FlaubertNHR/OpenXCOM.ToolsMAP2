using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Windows;

using XCom;
using XCom.Interfaces;
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

		private RecordsetPanel RecordPanel;
		internal int[,] ScanG;
		internal BitArray LoFT;

		private readonly Pen _penBlack = new Pen(Color.Black, 1);
		#endregion Fields


		#region Properties
		private Tilepart[] _records;
		private Tilepart[] Records
		{
			get { return _records; }
			set
			{
				RecordPanel.Records = (_records = value);
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

		private int _spriteShadeInt = 13;// 33; // unity (default) //-1
		private int SpriteShadeInt
		{
			get { return _spriteShadeInt; }
			set
			{
				if (_spriteShadeEnabled = ((_spriteShadeInt = value) != -1))
					SpriteShadeFloat = ((float)_spriteShadeInt * 0.03f);

				RecordPanel.Invalidate();
				pnl_Sprites.Invalidate();
				pnl_ScanGic.Invalidate();
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
						PopulateTextFields();
					}
					else
						ClearTextFields();

					RecordPanel.Invalidate();
					pnl_Sprites.Invalidate();
					pnl_ScanGic.Invalidate();
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
		internal McdviewF()
		{
#if DEBUG
			LogFile.SetLogFilePath(Path.GetDirectoryName(Application.ExecutablePath)); // creates a logfile/ wipes the old one.
#endif

			InitializeComponent();
			SetDoubleBuffered(pnl_Sprites);

			MaximumSize = new Size(0,0);

			LoadWindowMetrics();

			RecordPanel = new RecordsetPanel(this);
			gb_Collection.Controls.Add(RecordPanel);
			RecordPanel.Width = gb_Collection.Width - 10;

			tb_SpriteShade.Text = SpriteShadeInt.ToString();

			RecordPanel.Select();

			pnl_Sprites.Width = gb_Sprites.Width - 10;
			SpaceSpriteFields();


			string pathufo, pathtftd;
			GetResourcePaths(out pathufo, out pathtftd);
			ResourceInfo.LoadScanGufo(pathufo);		// -> ResourceInfo.ScanGufo
			ResourceInfo.LoadScanGtftd(pathtftd);	// -> ResourceInfo.ScanGtftd
			ScanG = ResourceInfo.ScanGufo;


			ResourceInfo.LoadLoFTufo(pathufo);		// -> ResourceInfo.LoFTufo
			ResourceInfo.LoadLoFTtftd(pathtftd);	// -> ResourceInfo.LoFTtftd
			LoFT = ResourceInfo.LoFTufo;
		}

		/// <summary>
		/// Spaces the layout of the fields under the anisprites in the sprite-
		/// panel.
		/// </summary>
		private void SpaceSpriteFields()
		{
			int left = pnl_Sprites.Left;
			int offset = XCImage.SpriteWidth32 - tb0_phase0.Width / 2;

			tb0_phase0.Left = left + SPRITE_ORIGIN_X + offset;
			tb1_phase1.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X;
			tb2_phase2.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 2;
			tb3_phase3.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 3;
			tb4_phase4.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 4;
			tb5_phase5.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 5;
			tb6_phase6.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 6;
			tb7_phase7.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 7;

			lbl0.Left = tb0_phase0.Left + tb0_phase0.Width / 2 - (lbl0.Width + lbl0_phase0.Width) / 2;
			lbl0_phase0.Left = lbl0.Right;

			lbl1.Left = lbl0.Left + SPRITE_OFFSET_X;
			lbl1_phase1.Left = lbl1.Right;

			lbl2.Left = lbl1.Left + SPRITE_OFFSET_X;
			lbl2_phase2.Left = lbl2.Right;

			lbl3.Left = lbl2.Left + SPRITE_OFFSET_X;
			lbl3_phase3.Left = lbl3.Right;

			lbl4.Left = lbl3.Left + SPRITE_OFFSET_X;
			lbl4_phase4.Left = lbl4.Right;

			lbl5.Left = lbl4.Left + SPRITE_OFFSET_X;
			lbl5_phase5.Left = lbl5.Right;

			lbl6.Left = lbl5.Left + SPRITE_OFFSET_X;
			lbl6_phase6.Left = lbl6.Right;

			lbl7.Left = lbl6.Left + SPRITE_OFFSET_X;
			lbl7_phase7.Left = lbl7.Right;
		}

/*		private string GetScanG_pathufo()
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileResources = Path.Combine(dirSettings, PathInfo.ConfigResources); // "MapResources.yml"
		}
		private string GetScanG_pathtftd()
		{
		} */

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
			string fileResources = Path.Combine(dirSettings, PathInfo.ConfigResources); // "MapResources.yml"
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
								if (//miPaletteUfo.Checked &&
									(val = node.Value.ToString()) != PathInfo.NotConfigured)
								{
									pathufo = val;
//									return Path.Combine(val, SharedSpace.ScanGfile);
								}
								break;

							case "tftd":
								if (//miPaletteTftd.Checked &&
									(val = node.Value.ToString()) != PathInfo.NotConfigured)
								{
									pathtftd = val;
//									return Path.Combine(val, SharedSpace.ScanGfile);
								}
								break;
						}
					}
				}
			}

			// Third let the user load a ScanG.Dat file from a ScanG->menuitem or so.

//			return null;
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
		#endregion Events (override)


		/// <summary>
		/// Some controls, such as the DataGridView, do not allow setting the
		/// DoubleBuffered property. It is set as a protected property. This
		/// method is a work-around to allow setting it. Call this in the
		/// constructor just after InitializeComponent().
		/// https://stackoverflow.com/questions/118528/horrible-redraw-performance-of-the-datagridview-on-one-of-my-two-screens#answer-16625788
		/// @note I wonder if this works on Mono. It stops the redraw-flick when
		/// setting the anisprite on return from SpritesetviewF on my system
		/// (Win7-64).
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

				RecordPanel.Invalidate();
				pnl_Sprites.Invalidate();
				pnl_ScanGic.Invalidate();
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

				RecordPanel.Invalidate();
				pnl_Sprites.Invalidate();
				pnl_ScanGic.Invalidate();
			}
		}
		#endregion Menuitems


		#region Events
		private void OnTextChanged_SpriteShade(object sender, EventArgs e)
		{
			// TODO: "SpriteShade does NOT get saved."
			int result;
			if (Int32.TryParse(tb_SpriteShade.Text, out result))
			{
				if      (result <  -1) tb_SpriteShade.Text =  "-1"; // recurse
				else if (result ==  0) tb_SpriteShade.Text =  "-1"; // recurse
				else if (result > 100) tb_SpriteShade.Text = "100"; // recurse
				else
					SpriteShadeInt = result;
			}
			else
				tb_SpriteShade.Text = "-1"; // recurse
		}


		Graphics _graphics;
		ImageAttributes _attri;

		const int SPRITE_ORIGIN_X = 20;
		const int SPRITE_ORIGIN_Y =  0;
		const int SPRITE_OFFSET_X = 80;

		/// <summary>
		/// Handles the Paint event for the anisprite groupbox's sprite-panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_Sprites(object sender, PaintEventArgs e)
		{
			if (SelId != -1)
			{
				_graphics = e.Graphics;
				_graphics.PixelOffsetMode   = PixelOffsetMode.Half;
				_graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

				_attri = new ImageAttributes();
				if (_spriteShadeEnabled)
					_attri.SetGamma(SpriteShadeFloat, ColorAdjustType.Bitmap);

				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite1].Sprite,
						SPRITE_ORIGIN_X,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite2].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite3].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X * 2,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite4].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X * 3,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite5].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X * 4,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite6].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X * 5,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite7].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X * 6,
						SPRITE_ORIGIN_Y);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite8].Sprite,
						SPRITE_ORIGIN_X + SPRITE_OFFSET_X * 7,
						SPRITE_ORIGIN_Y);
			}
		}

		/// <summary>
		/// Helper for OnPaint_Sprites().
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void DrawSprite(
				Image sprite,
				int x,
				int y)
		{
			_graphics.DrawImage(
							sprite,
							new Rectangle(
										x, y,
										XCImage.SpriteWidth32  * 2,
										XCImage.SpriteHeight40 * 2),
							0, 0, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
							GraphicsUnit.Pixel,
							_attri);
		}


		/// <summary>
		/// Opens the spriteset-viewer when an anisprite is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp_SpritePanel(object sender, MouseEventArgs e)
		{
			if (Spriteset != null && SelId != -1
				&& e.Y > -1 && e.Y < pnl_Sprites.Height)
			{
				int phase;
				for (phase = 0; phase != 8; ++phase)
				{
					if (   e.X > SPRITE_ORIGIN_X + (phase * SPRITE_OFFSET_X)
						&& e.X < SPRITE_ORIGIN_X + (phase * SPRITE_OFFSET_X) + (XCImage.SpriteWidth32 * 2))
					{
						break;
					}
				}

				if (phase != 8)
				{
					int id;
					switch (phase)
					{
						default: id = Int32.Parse(tb0_phase0.Text); break; // #0
						case 1:  id = Int32.Parse(tb1_phase1.Text); break;
						case 2:  id = Int32.Parse(tb2_phase2.Text); break;
						case 3:  id = Int32.Parse(tb3_phase3.Text); break;
						case 4:  id = Int32.Parse(tb4_phase4.Text); break;
						case 5:  id = Int32.Parse(tb5_phase5.Text); break;
						case 6:  id = Int32.Parse(tb6_phase6.Text); break;
						case 7:  id = Int32.Parse(tb7_phase7.Text); break;
					}

					using (var f = new SpritesetF(this, phase, id))
					{
						f.Location = new Point(Location.X + 20, Location.Y + 350);
						f.ShowDialog();
					}
				}
			}
		}

		/// <summary>
		/// Sets an anisprite when returning from SpritesetviewF.
		/// </summary>
		/// <param name="phase"></param>
		/// <param name="id"></param>
		internal void SetSprite(int phase, int id)
		{
			bool changed = false;

			switch (phase)
			{
				case 0:
					if (Int32.Parse(tb0_phase0.Text) != id)
					{
						changed = true;

						tb0_phase0.Text = id.ToString();
						Records[SelId].Record.Sprite1 = (byte)id;
					}
					break;

				case 1:
					if (Int32.Parse(tb1_phase1.Text) != id)
					{
						changed = true;

					tb1_phase1.Text = id.ToString();
					Records[SelId].Record.Sprite2 = (byte)id;
					}
					break;

				case 2:
					if (Int32.Parse(tb2_phase2.Text) != id)
					{
						changed = true;

						tb2_phase2.Text = id.ToString();
						Records[SelId].Record.Sprite3 = (byte)id;
					}
					break;

				case 3:
					if (Int32.Parse(tb3_phase3.Text) != id)
					{
						changed = true;

						tb3_phase3.Text = id.ToString();
						Records[SelId].Record.Sprite4 = (byte)id;
					}
					break;

				case 4:
					if (Int32.Parse(tb4_phase4.Text) != id)
					{
						changed = true;

						tb4_phase4.Text = id.ToString();
						Records[SelId].Record.Sprite5 = (byte)id;
					}
					break;

				case 5:
					if (Int32.Parse(tb5_phase5.Text) != id)
					{
						changed = true;

						tb5_phase5.Text = id.ToString();
						Records[SelId].Record.Sprite6 = (byte)id;
					}
					break;

				case 6:
					if (Int32.Parse(tb6_phase6.Text) != id)
					{
						changed = true;

						tb6_phase6.Text = id.ToString();
						Records[SelId].Record.Sprite7 = (byte)id;
					}
					break;

				case 7:
					if (Int32.Parse(tb7_phase7.Text) != id)
					{
						changed = true;

						tb7_phase7.Text = id.ToString();
						Records[SelId].Record.Sprite8 = (byte)id;
					}
					break;
			}

			if (changed)
			{
				Changed = true;
				Records[SelId].Anisprites[phase] = Spriteset[id];

				RecordPanel.Invalidate();
				pnl_Sprites.Invalidate();
			}
		}


		/// <summary>
		/// Draws a square around the ScanG icon.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_ScanG_group(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(
								_penBlack,
								119, 14,
								33, 33);
		}

		/// <summary>
		/// Draws a ScanG icon in the ScanG panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_ScanG_panel(object sender, PaintEventArgs e)
		{
			if (SelId != -1 && ScanG != null)
			{
				int id = Int32.Parse(tb20_scang1.Text);
				if (id > 35 && id < ScanG.Length / 16)
				{
					_graphics = e.Graphics;
					_graphics.PixelOffsetMode   = PixelOffsetMode.Half;
					_graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

					_attri = new ImageAttributes();
					if (_spriteShadeEnabled)
						_attri.SetGamma(SpriteShadeFloat, ColorAdjustType.Bitmap);

					var icon = new Bitmap(
										4,4,
										PixelFormat.Format8bppIndexed);

					var data = icon.LockBits(
										new Rectangle(0,0, icon.Width, icon.Height),
										ImageLockMode.WriteOnly,
										PixelFormat.Format8bppIndexed);
					var start = data.Scan0;

					unsafe
					{
						var pos = (byte*)start.ToPointer();

						int palid;
						for (uint row = 0; row != icon.Height; ++row)
						for (uint col = 0; col != icon.Width;  ++col)
						{
							byte* pixel = pos + col + row * data.Stride;

							palid = ScanG[id, row * 4 + col];
							*pixel = (byte)palid;
						}
					}
					icon.UnlockBits(data);

					if (miPaletteTftd.Checked)
						icon.Palette = Palette.TftdBattle.ColorTable;
					else
						icon.Palette = Palette.UfoBattle.ColorTable;

					ColorPalette pal = icon.Palette; // palettes get copied not referenced ->
					pal.Entries[Palette.TransparentId] = Color.Transparent;
					icon.Palette = pal;

					_graphics.DrawImage(
									icon,
									new Rectangle(0,0, 32,32),
									0,0, icon.Width, icon.Height,
									GraphicsUnit.Pixel,
									_attri);
				}
			}
		}

		/// <summary>
		/// Opens the ScanG viewer when the ScanG icon is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp_ScanGicon(object sender, MouseEventArgs e)
		{
			if (SelId != -1 && ScanG != null
				&& e.X > -1 && e.X < pnl_ScanGic.Width
				&& e.Y > -1 && e.Y < pnl_ScanGic.Height)
			{
				ColorPalette pal;
				if (miPaletteTftd.Checked)
					pal = Palette.TftdBattle.ColorTable;
				else
					pal = Palette.UfoBattle.ColorTable;

				using (var f = new ScanGiconF(this, Int32.Parse(tb20_scang1.Text), pal))
				{
					f.Location = new Point(
										Location.X + gb_Minimap.Width,
										Location.Y + Height - f.Height);
					f.ShowDialog();
				}
			}
		}

		/// <summary>
		/// Sets a ScanG icon when returning from ScanGiconF.
		/// </summary>
		/// <param name="id"></param>
		internal void SetIcon(int id)
		{
			if (Int32.Parse(tb20_scang1.Text) != id)
			{
				Changed = true;

				int id_reduced = id - 35;

				tb20_scang1.Text = id        .ToString();
				tb20_scang2.Text = id_reduced.ToString();

				Records[SelId].Record.ScanG         = (byte)id;
				Records[SelId].Record.ScanG_reduced = (byte)id_reduced;

				pnl_ScanGic.Invalidate();
			}
		}


		/// <summary>
		/// Opens the LoFT viewer when a LoFT icon is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp_LoftPanel(object sender, MouseEventArgs e)
		{
			if (SelId != -1 && LoFT != null)
			{
				var loft = sender as Panel;

				if (   e.X > -1 && e.X < loft.Width
					&& e.Y > -1 && e.Y < loft.Height)
				{
					string id;
					if      (loft == pnl_Loft08) id = tb8_loft00 .Text;
					else if (loft == pnl_Loft09) id = tb9_loft02 .Text;
					else if (loft == pnl_Loft10) id = tb10_loft04.Text;
					else if (loft == pnl_Loft11) id = tb11_loft06.Text;
					else if (loft == pnl_Loft12) id = tb12_loft08.Text;
					else if (loft == pnl_Loft13) id = tb13_loft10.Text;
					else if (loft == pnl_Loft14) id = tb14_loft12.Text;
					else if (loft == pnl_Loft15) id = tb15_loft14.Text;
					else if (loft == pnl_Loft16) id = tb16_loft16.Text;
					else if (loft == pnl_Loft17) id = tb17_loft18.Text;
					else if (loft == pnl_Loft18) id = tb18_loft20.Text;
					else                         id = tb19_loft22.Text; // if (loft == pnl_Loft19)

					using (var f = new LoftF(this, Int32.Parse(id)))
					{
						f.Location = new Point(
											Location.X +  10,
											Location.Y + 400);
						f.ShowDialog();
					}
				}
			}
		}

		/// <summary>
		/// Sets a LoFT when returning from LoftF.
		/// </summary>
		/// <param name="id"></param>
		internal void SetLoft(int id)
		{
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Populates all textfields with values of the currently selected
		/// record-id.
		/// </summary>
		internal void PopulateTextFields()
		{
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

			tb20_scang1.Text = ((int)record.ScanG)        .ToString();
			tb20_scang2.Text = ((int)record.ScanG_reduced).ToString();

			tb22_.Text = ((int)record.Unknown22).ToString();
			tb23_.Text = ((int)record.Unknown23).ToString();
			tb24_.Text = ((int)record.Unknown24).ToString();
			tb25_.Text = ((int)record.Unknown25).ToString();
			tb26_.Text = ((int)record.Unknown26).ToString();
			tb27_.Text = ((int)record.Unknown27).ToString();
			tb28_.Text = ((int)record.Unknown28).ToString();
			tb29_.Text = ((int)record.Unknown29).ToString();

			tb30_isufodoor   .Text = Convert.ToInt32(record.UfoDoor)   .ToString();
			tb31_isblocklos  .Text = Convert.ToInt32(record.StopLOS)   .ToString();
			tb32_isdropthrou .Text = Convert.ToInt32(record.NoGround)  .ToString();
			tb33_isbigwall   .Text = Convert.ToInt32(record.BigWall)   .ToString();
			tb34_isgravlift  .Text = Convert.ToInt32(record.GravLift)  .ToString();
			tb35_ishumandoor .Text = Convert.ToInt32(record.HumanDoor) .ToString();
			tb36_isblockfire .Text = Convert.ToInt32(record.BlockFire) .ToString();
			tb37_isblocksmoke.Text = Convert.ToInt32(record.BlockSmoke).ToString();

			tb38_startphase .Text = ((int)record.StartPhase).ToString();
			tb39_tuwalk     .Text = ((int)record.TU_Walk)   .ToString();
			tb40_tuslide    .Text = ((int)record.TU_Slide)  .ToString();
			tb41_tufly      .Text = ((int)record.TU_Fly)    .ToString();
			tb42_hitpoints  .Text = ((int)record.Armor)     .ToString();
			tb43_heblock    .Text = ((int)record.HE_Block)  .ToString();
			tb44_deathid    .Text = ((int)record.DieTile)   .ToString();
			tb45_flammable  .Text = ((int)record.Flammable) .ToString();
			tb46_alternateid.Text = ((int)record.Alt_MCD)   .ToString();

			tb47_.Text = ((int)record.Unknown47).ToString();

			tb48_unitoffset  .Text = ((int)record.StandOffset).ToString();
			tb49_spriteoffset.Text = ((int)record.TileOffset) .ToString();

			tb50_.Text = ((int)record.Unknown50).ToString();

			tb51_lightblock    .Text = ((int)record.LightBlock)   .ToString();
			tb52_footsound     .Text = ((int)record.Footstep)     .ToString();
			tb53_parttype      .Text = ((int)record.PartType)     .ToString();
			tb54_hetype        .Text = ((int)record.HE_Type)      .ToString();
			tb55_hestrength    .Text = ((int)record.HE_Strength)  .ToString();
			tb56_smokeblock    .Text = ((int)record.SmokeBlockage).ToString();
			tb57_fuel          .Text = ((int)record.Fuel)         .ToString();
			tb58_lightintensity.Text = ((int)record.LightSource)  .ToString();
			tb59_specialtype   .Text = ((int)record.TargetType)   .ToString();
			tb60_isbaseobject  .Text = ((int)record.BaseObject)   .ToString();

			tb61_.Text = ((int)record.Unknown61).ToString();
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

			tb30_isufodoor   .Text =
			tb31_isblocklos  .Text =
			tb32_isdropthrou .Text =
			tb33_isbigwall   .Text =
			tb34_isgravlift  .Text =
			tb35_ishumandoor .Text =
			tb36_isblockfire .Text =
			tb37_isblocksmoke.Text =

			tb38_startphase .Text =
			tb39_tuwalk     .Text =
			tb40_tuslide    .Text =
			tb41_tufly      .Text =
			tb42_hitpoints  .Text =
			tb43_heblock    .Text =
			tb44_deathid    .Text =
			tb45_flammable  .Text =
			tb46_alternateid.Text =

			tb47_.Text =

			tb48_unitoffset  .Text =
			tb49_spriteoffset.Text =

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
