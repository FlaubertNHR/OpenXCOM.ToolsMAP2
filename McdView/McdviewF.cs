using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;
using XCom.Resources.Map;


namespace McdView
{
	/// <summary>
	/// 
	/// </summary>
	internal partial class McdviewF
		:
			Form
	{
		#region Fields (constant)
		internal const TextFormatFlags FLAGS = TextFormatFlags.HorizontalCenter
											 | TextFormatFlags.VerticalCenter
											 | TextFormatFlags.NoPadding;
		#endregion Fields (constant)


		#region Fields
		private string _pfeMcd;
		internal string Label;

		private RecordsetPanel RecordPanel;

		internal readonly static Brush BrushHilight = new SolidBrush(Color.FromArgb(60, SystemColors.MenuHighlight));
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

			MaximumSize = new Size(0,0);

			RecordPanel = new RecordsetPanel(this);
			gb_Collection.Controls.Add(RecordPanel);
			RecordPanel.Width = gb_Collection.Width - 10;

			tb_SpriteShade.Text = SpriteShadeInt.ToString();

			RecordPanel.Select();

			pnl_Sprites.Width = Width - 10;
		}
		#endregion cTor


		#region Menuitems
		private void OnClick_Open(object sender, EventArgs e)
		{
			// TODO: Check changed.

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

				RecordPanel.Invalidate();
				pnl_Sprites.Invalidate();
			}
		}

		private void OnClick_PaletteTftd(object sender, EventArgs e)
		{
			if (!miPaletteTftd.Checked)
			{
				miPaletteTftd.Checked = true;
				miPaletteUfo .Checked = false;

				Spriteset.Pal = Palette.TftdBattle;

				RecordPanel.Invalidate();
				pnl_Sprites.Invalidate();
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

		const int xOrigin = 20;
		const int yOrigin =  0;
		const int xOffset = 80;

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
						xOrigin,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite2].Sprite,
						xOrigin + xOffset,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite3].Sprite,
						xOrigin + xOffset * 2,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite4].Sprite,
						xOrigin + xOffset * 3,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite5].Sprite,
						xOrigin + xOffset * 4,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite6].Sprite,
						xOrigin + xOffset * 5,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite7].Sprite,
						xOrigin + xOffset * 6,
						yOrigin);
				DrawSprite(
						Spriteset[Records[SelId].Record.Sprite8].Sprite,
						xOrigin + xOffset * 7,
						yOrigin);
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
		private void OnMouseDown_SpritePanel(object sender, MouseEventArgs e)
		{
			if (Spriteset != null && SelId != -1)
			{
				int pos;
				for (pos = 0; pos != 8; ++pos)
				{
					if (   e.X > xOrigin + (pos * xOffset)
						&& e.X < xOrigin + (pos * xOffset) + (XCImage.SpriteWidth32 * 2))
					{
						break;
					}
				}

				if (pos != 8)
				{
					int spriteId;
					switch (pos)
					{
						default: spriteId = Int32.Parse(tb0_phase1.Text); break; // #0
						case 1:  spriteId = Int32.Parse(tb1_phase2.Text); break;
						case 2:  spriteId = Int32.Parse(tb2_phase3.Text); break;
						case 3:  spriteId = Int32.Parse(tb3_phase4.Text); break;
						case 4:  spriteId = Int32.Parse(tb4_phase5.Text); break;
						case 5:  spriteId = Int32.Parse(tb5_phase6.Text); break;
						case 6:  spriteId = Int32.Parse(tb6_phase7.Text); break;
						case 7:  spriteId = Int32.Parse(tb7_phase8.Text); break;
					}

					var f = new SpritesetF(this, pos, spriteId);
					f.Location = new Point(Location.X + 20, Location.Y + 350);
					f.ShowDialog();
				}
			}
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

			tb0_phase1.Text = ((int)record.Sprite1).ToString();
			tb1_phase2.Text = ((int)record.Sprite2).ToString();
			tb2_phase3.Text = ((int)record.Sprite3).ToString();
			tb3_phase4.Text = ((int)record.Sprite4).ToString();
			tb4_phase5.Text = ((int)record.Sprite5).ToString();
			tb5_phase6.Text = ((int)record.Sprite6).ToString();
			tb6_phase7.Text = ((int)record.Sprite7).ToString();
			tb7_phase8.Text = ((int)record.Sprite8).ToString();

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

			tb30_isufodoor  .Text = Convert.ToInt32(record.UfoDoor)   .ToString();
			tb31_blocklos   .Text = Convert.ToInt32(record.StopLOS)   .ToString();
			tb32_isdropthrou.Text = Convert.ToInt32(record.NoGround)  .ToString();
			tb33_isbigwall  .Text = Convert.ToInt32(record.BigWall)   .ToString();
			tb34_isgravlift .Text = Convert.ToInt32(record.GravLift)  .ToString();
			tb35_ishumandoor.Text = Convert.ToInt32(record.HumanDoor) .ToString();
			tb36_blockfire  .Text = Convert.ToInt32(record.BlockFire) .ToString();
			tb37_blocksmoke .Text = Convert.ToInt32(record.BlockSmoke).ToString();

			tb38_startphase .Text = ((int)record.StartPhase).ToString();
			tb39_tuwalk     .Text = ((int)record.TU_Walk)   .ToString();
			tb40_tuslide    .Text = ((int)record.TU_Slide)  .ToString();
			tb41_tufly      .Text = ((int)record.TU_Fly)    .ToString();
			tb42_armor      .Text = ((int)record.Armor)     .ToString();
			tb43_heblock    .Text = ((int)record.HE_Block)  .ToString();
			tb44_deathid    .Text = ((int)record.DieTile)   .ToString();
			tb45_flammable  .Text = ((int)record.Flammable) .ToString();
			tb46_alternateid.Text = ((int)record.Alt_MCD)   .ToString();

			tb47_.Text = ((int)record.Unknown47).ToString();

			tb48_unitoffset  .Text = ((int)record.StandOffset).ToString();
			tb49_spriteoffset.Text = ((int)record.TileOffset) .ToString();

			tb50_.Text = ((int)record.Unknown50).ToString();

			tb51_lightblock  .Text = ((int)record.LightBlock)   .ToString();
			tb52_footsound   .Text = ((int)record.Footstep)     .ToString();
			tb53_parttype    .Text = ((int)record.PartType)     .ToString();
			tb54_hetype      .Text = ((int)record.HE_Type)      .ToString();
			tb55_hestrength  .Text = ((int)record.HE_Strength)  .ToString();
			tb56_smokeblock  .Text = ((int)record.SmokeBlockage).ToString();
			tb57_fuel        .Text = ((int)record.Fuel)         .ToString();
			tb58_lightpower  .Text = ((int)record.LightSource)  .ToString();
			tb59_specialtype .Text = ((int)record.TargetType)   .ToString();
			tb60_isbaseobject.Text = ((int)record.BaseObject)   .ToString();

			tb61_.Text = ((int)record.Unknown61).ToString();
		}

		/// <summary>
		/// Clears all textfields.
		/// </summary>
		internal void ClearTextFields()
		{
			tb0_phase1.Text =
			tb1_phase2.Text =
			tb2_phase3.Text =
			tb3_phase4.Text =
			tb4_phase5.Text =
			tb5_phase6.Text =
			tb6_phase7.Text =
			tb7_phase8.Text =

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

			tb30_isufodoor  .Text =
			tb31_blocklos   .Text =
			tb32_isdropthrou.Text =
			tb33_isbigwall  .Text =
			tb34_isgravlift .Text =
			tb35_ishumandoor.Text =
			tb36_blockfire  .Text =
			tb37_blocksmoke .Text =

			tb38_startphase .Text =
			tb39_tuwalk     .Text =
			tb40_tuslide    .Text =
			tb41_tufly      .Text =
			tb42_armor      .Text =
			tb43_heblock    .Text =
			tb44_deathid    .Text =
			tb45_flammable  .Text =
			tb46_alternateid.Text =

			tb47_.Text =

			tb48_unitoffset  .Text =
			tb49_spriteoffset.Text =

			tb50_.Text =

			tb51_lightblock  .Text =
			tb52_footsound   .Text =
			tb53_parttype    .Text =
			tb54_hetype      .Text =
			tb55_hestrength  .Text =
			tb56_smokeblock  .Text =
			tb57_fuel        .Text =
			tb58_lightpower  .Text =
			tb59_specialtype .Text =
			tb60_isbaseobject.Text =

			tb61_.Text = String.Empty;
		}
		#endregion Methods
	}
}
