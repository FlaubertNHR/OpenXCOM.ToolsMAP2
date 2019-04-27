using System;
using System.Windows.Forms;
using System.Drawing;

using XCom;


namespace McdView
{
	/// <summary>
	/// A form with a panel that enables the user to copy MCD records from a
	/// different MCD-set than the one that's currently loaded in McdView to the
	/// internal copy-buffer of McdView for pasting into the currently loaded
	/// MCD-set.
	/// </summary>
	internal partial class CopyPanelF
		:
			Form
	{
		#region Fields (static)
		internal static Rectangle Metric = new Rectangle(-1,0, 0,0);
		#endregion Fields (static)


		#region Fields
		private readonly McdviewF _f;

		internal string Label;
		#endregion Fields


		#region Properties
		internal TerrainPanel_copy PartsPanel
		{ get; private set; }

		private Tilepart[] _parts;
		/// <summary>
		/// An array of 'Tileparts'. Each entry's record is referenced w/ 'Record'.
		/// </summary>
		internal Tilepart[] Parts
		{
			get { return _parts; }
			set
			{
				PartsPanel.Parts = (_parts = value);
			}
		}

		private SpriteCollection _spriteset;
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			set
			{
				string text = "Copy panel - ";

				if ((PartsPanel.Spriteset = (_spriteset = value)) != null)
					text += _spriteset.Label;
				else
					text += "spriteset invalid";

				Text = text;
				PartsPanel.Select();
			}
		}

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
						PopulateTextFields();
						PartsPanel.ScrollToPart();
					}
					else
						ClearTextFields();

					PartsPanel.Invalidate();
				}

				if (PartsPanel.SubIds.Remove(_selId)) // safety. The SelId shall never be in the SubIds.
					PartsPanel.Invalidate();
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		internal CopyPanelF(McdviewF f)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;

			Location = new Point(
							_f.Location.X + 20,
							_f.Location.Y + 20);

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

			ClientSize = new Size(
								gb_Overhead     .Width
									+ gb_Tu     .Width
									+ gb_Explode.Width
									+ gb_Loft   .Width,
								ClientSize.Height); // <- that isn't respecting Clientsize.Height (!!surprise!!)

			btn_Open.Location = new Point(5, pnl_bg.Height - btn_Open.Height - 5);


			PartsPanel = new TerrainPanel_copy(_f, this);
			gb_Collection.Controls.Add(PartsPanel);
			PartsPanel.Width = gb_Collection.Width - 10;

			McdviewF.SetDoubleBuffered(PartsPanel);

			PartsPanel.Select();


			foreach (Control control in Controls) // TODO: Do this recursively.
			{
				if ((control as GroupBox) != null)
				{
					control.Click += OnClick_FocusCollection;
					for (int i = 0; i != control.Controls.Count; ++i)
						if ((control.Controls[i] as TextBox) != null)
							(control.Controls[i] as TextBox).ReadOnly = true;
						else //if ((control.Controls[i] as Label) != null)
							control.Controls[i].Click += OnClick_FocusCollection;
				}
				else if ((control as Panel) != null)
				{
					control.Click += OnClick_FocusCollection;
					for (int i = 0; i != control.Controls.Count; ++i)
						if ((control.Controls[i] as GroupBox) != null)
						{
							control.Controls[i].Click += OnClick_FocusCollection;
							for (int j = 0; j != control.Controls[i].Controls.Count; ++j)
								if ((control.Controls[i].Controls[j] as TextBox) != null)
									(control.Controls[i].Controls[j] as TextBox).ReadOnly = true;
								else //if ((control.Controls[i].Controls[j] as Label) != null)
									control.Controls[i].Controls[j].Click += OnClick_FocusCollection;
						}
				}
			}
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Closes (and disposes) this CopyPanelF object.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Metric.X = Location.X;
			Metric.Y = Location.Y;
			Metric.Width  = ClientSize.Width;
			Metric.Height = ClientSize.Height;

			_f.CloseCopyPanel();

			base.OnFormClosing(e);
		}

		/// <summary>
		/// Positions this CopyPanelF wrt/ McdviewF.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			if (Metric.X != -1)
			{
				Location   = new Point(Metric.X, Metric.Y);
				ClientSize = new Size(Metric.Width, Metric.Height);
			}
			base.OnLoad(e);
		}

		/// <summary>
		/// yah whatever. I dislike .NET keyboard gobbledy-gook.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.O))
			{
				_f.OpenCopyPanel();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion Events (override)


		#region Events
		private void OnClick_Open(object sender, EventArgs e)
		{
			_f.OpenCopyPanel();
		}

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
		#endregion Events


		#region Methods
		/// <summary>
		/// Populates all textfields with values of the currently selected
		/// record-id.
		/// </summary>
		internal void PopulateTextFields()
		{
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
		}

		/// <summary>
		/// Clears all textfields.
		/// </summary>
		internal void ClearTextFields()
		{
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

			tb61_.Text = String.Empty;
		}
		#endregion Methods
	}
}
