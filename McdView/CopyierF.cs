using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace McdView
{
	/// <summary>
	/// A form with a panel that enables the user to copy MCD records from a
	/// different MCD-set than the one that's currently loaded in McdView to the
	/// internal copy-buffer of McdView for pasting into the currently loaded
	/// MCD-set.
	/// </summary>
	internal sealed partial class CopierF
		:
			Form
	{
		#region Fields (static)
		private const string TITLE = "Copier";

		private static bool IalDeadpartChecked = true;
		private static bool IalAltrpartChecked = true;
		private static bool IalSpritesChecked  = true;
		#endregion Fields (static)


		#region Fields
		private readonly McdviewF _f;

		/// <summary>
		/// The file w/out extension of the currently loaded terrain.
		/// </summary>
		internal string Label;

		private Graphics _graphics;
		#endregion Fields


		#region Properties
		internal TerrainPanel_copier PartsPanel
		{ get; private set; }

		private Tilepart[] _parts;
		/// <summary>
		/// An array of <see cref="Tilepart">Tileparts</see>.
		/// </summary>
		/// <remarks>Each entry's record is referenced w/ Tilepart.Record.</remarks>
		internal Tilepart[] Parts
		{
			get { return _parts; }
			set
			{
				PartsPanel.Parts = (_parts = value);
			}
		}

		private Spriteset _spriteset;
		internal Spriteset Spriteset
		{
			get { return _spriteset; }
			set
			{
				if (_spriteset != null)
					_spriteset.Dispose();

				string text = TITLE;

				if (!String.IsNullOrEmpty(Label))
					text += GlobalsXC.PADDED_SEPARATOR + Label;

				PartsPanel.SetSpriteset(_spriteset = value);
				if (_spriteset == null)
					text += GlobalsXC.PADDED_SEPARATOR + "spriteset invalid";

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

					PartsPanel .Invalidate();
					pnl_ScanGic.Invalidate();

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

				if (PartsPanel.SubIds.Remove(_selId)) // safety. The SelId shall never be in the SubIds.
					PartsPanel.Invalidate();
			}
		}

		private string _pfeMcd;
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
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		internal CopierF(McdviewF f)
		{
			InitializeComponent();

			_f = f;

			gb_Overhead  .Location = new Point(0, 0);
			gb_General   .Location = new Point(0, gb_Overhead.Bottom);
			gb_Health    .Location = new Point(0, gb_General .Bottom);
			gb_Door      .Location = new Point(0, gb_Health  .Bottom);

			gb_Tu        .Location = new Point(gb_Overhead.Right, 0);
			gb_Block     .Location = new Point(gb_Overhead.Right, gb_Tu   .Bottom);
			gb_Step      .Location = new Point(gb_Overhead.Right, gb_Block.Bottom);
			gb_Elevation .Location = new Point(gb_Overhead.Right, gb_Step .Bottom);

			gb_Explode   .Location = new Point(gb_Tu.Right, 0);
			gb_Unused    .Location = new Point(gb_Tu.Right, gb_Explode.Bottom);

			gb_IalOptions.Location = new Point(0, pnl_bg.ClientSize.Height - gb_IalOptions.Height);

			if (!RegistryInfo.RegisterProperties(this))
			{
				Left = _f.Location.X + 20;
				Top  = _f.Location.Y + 20;
				ClientSize = new Size(
									gb_Overhead     .Width
										+ gb_Tu     .Width
										+ gb_Explode.Width
										+ gb_Loft   .Width,
									ClientSize.Height);
			}

			btn_Open.Location = new Point(gb_Unused.Left, gb_Unused.Bottom);
			btn_Open.Width  = gb_Loft.Location.X - gb_IalOptions.Width;
			btn_Open.Height = pnl_bg.ClientSize.Height - gb_Unused.Bottom;

			cb_IalSprites.Text = "copy Sprites to " + _f.Label + GlobalsXC.PckExt;


			PartsPanel = new TerrainPanel_copier(_f, this);
			gb_Collection.Controls.Add(PartsPanel);
			PartsPanel.Width = gb_Collection.Width - 10;

			TagLoftPanels();
			LoftPanel_copier.SetStaticVars(_f, this);

			PartsPanel.Select();


			foreach (Control control in Controls) // TODO: Do this recursively.
			{
				if ((control as GroupBox) != null)
				{
					control.Click += OnClick_FocusCollection;
					for (int i = 0; i != control.Controls.Count; ++i)
						if ((control.Controls[i] as TextBox) != null)
							(control.Controls[i] as TextBox).ReadOnly = true;
						else if ((control.Controls[i] as Label) != null)
							control.Controls[i].Click += OnClick_FocusCollection;
						else if ((control.Controls[i] as Panel) != null)
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
								else if ((control.Controls[i].Controls[j] as Label) != null)
									control.Controls[i].Controls[j].Click += OnClick_FocusCollection;
								else if ((control.Controls[i].Controls[j] as Panel) != null)
									control.Controls[i].Controls[j].Click += OnClick_FocusCollection;
						}
				}
			}
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Closes (and disposes) this CopierF object.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				RegistryInfo.UpdateRegistry(this);

				if (Spriteset != null)
					Spriteset.Dispose();

				PartsPanel.ContextMenuStrip.Dispose();

				IalDeadpartChecked = cb_IalDeadpart.Checked;
				IalAltrpartChecked = cb_IalAltrpart.Checked;
				IalSpritesChecked  = cb_IalSprites .Checked;

				_f.CloseCopyPanel();
			}
			base.OnFormClosing(e);
		}

		/// <summary>
		/// Handles the Form's Resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			LayoutSpritesGroup();
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
				_f.OpenCopier();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Enter:
					e.Handled = e.SuppressKeyPress = true;
					PartsPanel.Select();
					break;

				case Keys.Escape:
					e.Handled = e.SuppressKeyPress = true;
					if ((ActiveControl as TextBox) == null)
						SelId = -1;

					PartsPanel.Select();
					break;
			}
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick_Open(object sender, EventArgs e)
		{
			_f.OpenCopier();
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
		private static int SPRITE_ORIGIN_X;

		/// <summary>
		/// Spaces the layout of the fields etc. of the labels and textboxes in
		/// the sprite-group.
		/// @note Based on McdviewF_panels.LayoutSpritesGroup().
		/// </summary>
		private void LayoutSpritesGroup()
		{
			SPRITE_ORIGIN_X = gb_Sprites.Width / 2 - McdviewF.SPRITE_OFFSET_X * 4;
			if (SPRITE_ORIGIN_X < 0) SPRITE_ORIGIN_X = 0;

			const int left = 5;
			int offset = XCImage.SpriteWidth32 - (tb00_phase0.Width / 2);

			tb00_phase0.Left = left + SPRITE_ORIGIN_X + offset;
			tb01_phase1.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X;
			tb02_phase2.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X * 2;
			tb03_phase3.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X * 3;
			tb04_phase4.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X * 4;
			tb05_phase5.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X * 5;
			tb06_phase6.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X * 6;
			tb07_phase7.Left = left + SPRITE_ORIGIN_X + offset + McdviewF.SPRITE_OFFSET_X * 7;

			lbl00.Left = tb00_phase0.Left + (tb00_phase0.Width / 2) - ((lbl00.Width + lbl00_phase0.Width) / 2);
			lbl00_phase0.Left = lbl00.Right;

			lbl01.Left = lbl00.Left + McdviewF.SPRITE_OFFSET_X;
			lbl01_phase1.Left = lbl01.Right;

			lbl02.Left = lbl01.Left + McdviewF.SPRITE_OFFSET_X;
			lbl02_phase2.Left = lbl02.Right;

			lbl03.Left = lbl02.Left + McdviewF.SPRITE_OFFSET_X;
			lbl03_phase3.Left = lbl03.Right;

			lbl04.Left = lbl03.Left + McdviewF.SPRITE_OFFSET_X;
			lbl04_phase4.Left = lbl04.Right;

			lbl05.Left = lbl04.Left + McdviewF.SPRITE_OFFSET_X;
			lbl05_phase5.Left = lbl05.Right;

			lbl06.Left = lbl05.Left + McdviewF.SPRITE_OFFSET_X;
			lbl06_phase6.Left = lbl06.Right;

			lbl07.Left = lbl06.Left + McdviewF.SPRITE_OFFSET_X;
			lbl07_phase7.Left = lbl07.Right;
		}


		/// <summary>
		/// Decides if the InsertAfterLast options are checked.
		/// </summary>
		internal void LoadIalOptions()
		{
			cb_IalDeadpart.Checked = IalDeadpartChecked;
			cb_IalAltrpart.Checked = IalAltrpartChecked;
			cb_IalSprites .Checked = IalSpritesChecked;
		}


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


		#region ScanG icon
		/// <summary>
		/// Draws a square border around the ScanG icon.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_ScanG_group(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(
								Colors.PenText,
								pnl_ScanGic.Location.X - 1,
								pnl_ScanGic.Location.Y - 1,
								pnl_ScanGic.Width  + 1,
								pnl_ScanGic.Height + 1);
		}

		/// <summary>
		/// Draws a ScanG icon in the ScanG panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_ScanG_panel(object sender, PaintEventArgs e)
		{
			if (SelId != -1 && _f.ScanG != null)
			{
				int id = Int32.Parse(tb20_scang1.Text);
				if (id > 35 && id < _f.ScanG.Length / 16)
				{
					_graphics = e.Graphics;
					_graphics.PixelOffsetMode   = PixelOffsetMode.Half;
					_graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

					using (var icon = new Bitmap(4,4, PixelFormat.Format8bppIndexed))
					{
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

								palid = _f.ScanG[id, (row * 4) + col];
								*pixel = (byte)palid;
							}
						}
						icon.UnlockBits(data);

						icon.Palette = _f.Pal.Table;

						ColorPalette pal = icon.Palette; // palettes get copied not referenced ->
						pal.Entries[Palette.Tid] = Color.Transparent;
						icon.Palette = pal;

						var panel = sender as Panel;
						_graphics.DrawImage(
										icon,
										new Rectangle(
													0,0,
													panel.Width,
													panel.Height),
										0,0, icon.Width, icon.Height,
										GraphicsUnit.Pixel,
										_f.Ia);
					}
				}
			}
		}
		#endregion ScanG icon


		#region LoFT
		/// <summary>
		/// Tags each LoftPanel with its corresponding TextBox.
		/// @note The tagged TextBoxes are tagged with (string)panelid in the
		/// designer. Thus the loft-panels, loft-textboxes, and panelids are all
		/// synched respectively.
		/// </summary>
		private void TagLoftPanels()
		{
			pnl_Loft08.Tag = tb08_loft00;
			pnl_Loft09.Tag = tb09_loft01;
			pnl_Loft10.Tag = tb10_loft02;
			pnl_Loft11.Tag = tb11_loft03;
			pnl_Loft12.Tag = tb12_loft04;
			pnl_Loft13.Tag = tb13_loft05;
			pnl_Loft14.Tag = tb14_loft06;
			pnl_Loft15.Tag = tb15_loft07;
			pnl_Loft16.Tag = tb16_loft08;
			pnl_Loft17.Tag = tb17_loft09;
			pnl_Loft18.Tag = tb18_loft10;
			pnl_Loft19.Tag = tb19_loft11;
		}

		/// <summary>
		/// Draws squares around the LoFT panels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_LoFT_group(object sender, PaintEventArgs e)
		{
			LoftPanel_copier pnlLoFT;
			for (int i = 0; i != 12; ++i)
			{
				switch (i)
				{
					default: pnlLoFT = pnl_Loft08; break; // case 0
					case  1: pnlLoFT = pnl_Loft09; break;
					case  2: pnlLoFT = pnl_Loft10; break;
					case  3: pnlLoFT = pnl_Loft11; break;
					case  4: pnlLoFT = pnl_Loft12; break;
					case  5: pnlLoFT = pnl_Loft13; break;
					case  6: pnlLoFT = pnl_Loft14; break;
					case  7: pnlLoFT = pnl_Loft15; break;
					case  8: pnlLoFT = pnl_Loft16; break;
					case  9: pnlLoFT = pnl_Loft17; break;
					case 10: pnlLoFT = pnl_Loft18; break;
					case 11: pnlLoFT = pnl_Loft19; break;
				}
				e.Graphics.DrawRectangle(
									Colors.PenText,
									pnlLoFT.Location.X - 1,
									pnlLoFT.Location.Y - 1,
									pnlLoFT.Width  + 1,
									pnlLoFT.Height + 1);
			}
		}
		#endregion LoFT
	}
}
