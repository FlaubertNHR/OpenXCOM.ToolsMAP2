using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using XCom;
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
		#region Fields
		private RecordsetPanel RecordPanel;
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

		private SpriteCollection Spriteset
		{ get; set; }


		internal bool _spriteShadeEnabled;

		private int _spriteShadeInt = 12;// 33; // unity (default) //-1
		private int SpriteShadeInt
		{
			get { return _spriteShadeInt; }
			set
			{
				if (_spriteShadeEnabled = ((_spriteShadeInt = value) != -1))
					SpriteShadeFloat = ((float)_spriteShadeInt * 0.03f);

				RecordPanel.Invalidate();
				// TODO: refresh anisprites
			}
		}
		internal float SpriteShadeFloat
		{ get; private set; }


		private int _selId = -1;
		internal int SelId
		{
			get { return _selId; }
			set { _selId = value; }
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
		}
		#endregion cTor


		#region Menuitems
		private void OnOpenClick(object sender, EventArgs e)
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

					string pfeMcd = ofd.FileName;
					string terrain = Path.GetFileNameWithoutExtension(pfeMcd);

					using (var bs = new BufferedStream(File.OpenRead(pfeMcd)))
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
															terrain,
															Path.GetDirectoryName(pfeMcd),
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
							Records[id].Dead      = TilepartFactory.GetDeadPart(     terrain, id, Records[id].Record, Records);
							Records[id].Alternate = TilepartFactory.GetAlternatePart(terrain, id, Records[id].Record, Records);
						}
					}

					SelId = -1;
					ResourceInfo.ReloadSprites = false;

					Text = "McdView - " + pfeMcd;
				}
			}
		}


		private void OnPaletteUfoClick(object sender, EventArgs e)
		{
			if (!miPaletteUfo.Checked)
			{
				miPaletteUfo .Checked = true;
				miPaletteTftd.Checked = false;

				Spriteset.Pal = Palette.UfoBattle;

				RecordPanel.Invalidate();
				// TODO: refresh anisprites
			}
		}

		private void OnPaletteTftdClick(object sender, EventArgs e)
		{
			if (!miPaletteTftd.Checked)
			{
				miPaletteTftd.Checked = true;
				miPaletteUfo .Checked = false;

				Spriteset.Pal = Palette.TftdBattle;

				RecordPanel.Invalidate();
				// TODO: refresh anisprites
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
		#endregion Events


		#region Methods
		internal void PopulateTextFields()
		{
			//LogFile.WriteLine("records= " + Records.Length);
			//LogFile.WriteLine("SelId= " + SelId);

			if (SelId != -1)
			{
				McdRecord record = Records[SelId].Record;

				tb0_sprite1.Text = ((int)record.Sprite1).ToString();
				tb1_sprite2.Text = ((int)record.Sprite2).ToString();
				tb2_sprite3.Text = ((int)record.Sprite3).ToString();
				tb3_sprite4.Text = ((int)record.Sprite4).ToString();
				tb4_sprite5.Text = ((int)record.Sprite5).ToString();
				tb5_sprite6.Text = ((int)record.Sprite6).ToString();
				tb6_sprite7.Text = ((int)record.Sprite7).ToString();
				tb7_sprite8.Text = ((int)record.Sprite8).ToString();

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
		}
		#endregion Methods
	}
}
