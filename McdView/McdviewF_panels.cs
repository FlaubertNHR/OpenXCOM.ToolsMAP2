using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;

// RotatingCube ->
//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Windows.Forms;
//using System.Windows.Threading;


namespace McdView
{
	internal partial class McdviewF
	{
		Graphics _graphics;
		ImageAttributes _attri;


		#region Anisprites
		const int SPRITE_ORIGIN_X = 20;
		const int SPRITE_ORIGIN_Y =  0;
		const int SPRITE_OFFSET_X = 80;

		/// <summary>
		/// Spaces the layout of the fields etc. of the anisprites in the
		/// sprite-group and -panel.
		/// </summary>
		private void LayoutSpriteGroup()
		{
			pnl_Sprites.Width = (gb_Sprites.Width - 10);

			int left = pnl_Sprites.Left;
			int offset = XCImage.SpriteWidth32 - (tb0_phase0.Width / 2);

			tb0_phase0.Left = left + SPRITE_ORIGIN_X + offset;
			tb1_phase1.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X;
			tb2_phase2.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 2;
			tb3_phase3.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 3;
			tb4_phase4.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 4;
			tb5_phase5.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 5;
			tb6_phase6.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 6;
			tb7_phase7.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 7;

			lbl0.Left = tb0_phase0.Left + (tb0_phase0.Width / 2) - ((lbl0.Width + lbl0_phase0.Width) / 2);
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

				McdRecord record = Records[SelId].Record;
				int y = SPRITE_ORIGIN_Y;

				int yoffset = record.TileOffset;
				if (yoffset != 0)
				{
					y -= yoffset * 2;

					var rect = new Rectangle(
										0,
										SPRITE_ORIGIN_Y,
										XCImage.SpriteWidth32  * 2,
										XCImage.SpriteHeight40 * 2);

					var brush = new SolidBrush(Color.Black); // actually palette-id #0 Transparent
					for (int i = 0; i != 8; ++i)
					{
						rect.X = SPRITE_ORIGIN_X + SPRITE_OFFSET_X * i;
						_graphics.FillRectangle(brush, rect);
					}
				}

				byte phase;
				for (int i = 0; i != 8; ++i)
				{
					switch (i)
					{
						default: phase = record.Sprite1; break;
						case 1:  phase = record.Sprite2; break;
						case 2:  phase = record.Sprite3; break;
						case 3:  phase = record.Sprite4; break;
						case 4:  phase = record.Sprite5; break;
						case 5:  phase = record.Sprite6; break;
						case 6:  phase = record.Sprite7; break;
						case 7:  phase = record.Sprite8; break;
					}
					DrawSprite(
							Spriteset[phase].Sprite,
							SPRITE_ORIGIN_X + SPRITE_OFFSET_X * i, y);
				}
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
		#endregion Anisprites


		#region ScanG icon
		/// <summary>
		/// Draws a square border around the ScanG icon.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_ScanG_group(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(
								_penBlack,
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

							palid = ScanG[id, (row * 4) + col];
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
									new Rectangle(
												0,0,
												((Panel)sender).Width,
												((Panel)sender).Height),
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

				Records[SelId].Record.ScanG         = (ushort)id;
				Records[SelId].Record.ScanG_reduced = (ushort)id_reduced;

				pnl_ScanGic.Invalidate();
			}
		}
		#endregion ScanG icon


		#region LoFT
		/// <summary>
		/// Draws squares around the LoFT panels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_LoFT_group(object sender, PaintEventArgs e)
		{
			Panel pnlLoFT;
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
									_penBlack,
									pnlLoFT.Location.X - 1,
									pnlLoFT.Location.Y - 1,
									pnlLoFT.Width  + 1,
									pnlLoFT.Height + 1);
			}
		}

		/// <summary>
		/// Draws a LoFT icon in a LoFT panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_LoFT_panel(object sender, PaintEventArgs e)
		{
			if (SelId != -1 && LoFT != null)
			{
				var pnlLoFT = sender as Panel;

				string val;
				if      (pnlLoFT == pnl_Loft08) val = tb8_loft00 .Text;
				else if (pnlLoFT == pnl_Loft09) val = tb9_loft02 .Text;
				else if (pnlLoFT == pnl_Loft10) val = tb10_loft04.Text;
				else if (pnlLoFT == pnl_Loft11) val = tb11_loft06.Text;
				else if (pnlLoFT == pnl_Loft12) val = tb12_loft08.Text;
				else if (pnlLoFT == pnl_Loft13) val = tb13_loft10.Text;
				else if (pnlLoFT == pnl_Loft14) val = tb14_loft12.Text;
				else if (pnlLoFT == pnl_Loft15) val = tb15_loft14.Text;
				else if (pnlLoFT == pnl_Loft16) val = tb16_loft16.Text;
				else if (pnlLoFT == pnl_Loft17) val = tb17_loft18.Text;
				else if (pnlLoFT == pnl_Loft18) val = tb18_loft20.Text;
				else                            val = tb19_loft22.Text; // if (pnlLoFT == pnl_Loft19)

				int id = Int32.Parse(val);
				if (id < LoFT.Length / 256)
				{
					_graphics = e.Graphics;
					_graphics.PixelOffsetMode   = PixelOffsetMode.Half;
					_graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

					var loft = new Bitmap(
										16,16,
										PixelFormat.Format8bppIndexed);	// Format1bppIndexed <- uses only 1 BIT per pixel
																		// - causes probs setting the pixels below.
					var data = loft.LockBits(
										new Rectangle(0,0, loft.Width, loft.Height),
										ImageLockMode.WriteOnly,
										PixelFormat.Format8bppIndexed); // Format1bppIndexed
					var start = data.Scan0;

					unsafe
					{
						var pos = (byte*)start.ToPointer();

						byte palid;
						for (int row = 0; row != loft.Height; ++row)
						for (int col = 0; col != loft.Width;  ++col)
						{
							byte* pixel = pos + (row * data.Stride) + col;

							palid = Convert.ToByte(LoFT[(id * 256) + (row * loft.Width) + col]);
							*pixel = palid;
						}
					}
					loft.UnlockBits(data);

					ColorPalette pal = loft.Palette;
					pal.Entries[0] = SystemColors.ControlDarkDark;
					pal.Entries[1] = SystemColors.ControlLightLight;
					loft.Palette = pal;

					_graphics.DrawImage(
									loft,
									new Rectangle(
												0,0,
												pnlLoFT.Width,
												pnlLoFT.Height));
				}
			}
		}


		Panel _pnlLoFT;

		/// <summary>
		/// Opens the LoFT viewer when a LoFT icon is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp_LoftPanel(object sender, MouseEventArgs e)
		{
			if (SelId != -1 && LoFT != null)
			{
				_pnlLoFT = sender as Panel;

				if (   e.X > -1 && e.X < _pnlLoFT.Width
					&& e.Y > -1 && e.Y < _pnlLoFT.Height)
				{
					string id; int slot;
					if      (_pnlLoFT == pnl_Loft08) { id = tb8_loft00 .Text; slot =  0; }
					else if (_pnlLoFT == pnl_Loft09) { id = tb9_loft02 .Text; slot =  2; }
					else if (_pnlLoFT == pnl_Loft10) { id = tb10_loft04.Text; slot =  4; }
					else if (_pnlLoFT == pnl_Loft11) { id = tb11_loft06.Text; slot =  6; }
					else if (_pnlLoFT == pnl_Loft12) { id = tb12_loft08.Text; slot =  8; }
					else if (_pnlLoFT == pnl_Loft13) { id = tb13_loft10.Text; slot = 10; }
					else if (_pnlLoFT == pnl_Loft14) { id = tb14_loft12.Text; slot = 12; }
					else if (_pnlLoFT == pnl_Loft15) { id = tb15_loft14.Text; slot = 14; }
					else if (_pnlLoFT == pnl_Loft16) { id = tb16_loft16.Text; slot = 16; }
					else if (_pnlLoFT == pnl_Loft17) { id = tb17_loft18.Text; slot = 18; }
					else if (_pnlLoFT == pnl_Loft18) { id = tb18_loft20.Text; slot = 20; }
					else                             { id = tb19_loft22.Text; slot = 22; } // if (_pnlLoFT == pnl_Loft19)

					using (var f = new LoftF(this, slot, Int32.Parse(id)))
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
			TextBox tb;
			if      (_pnlLoFT == pnl_Loft08) tb = tb8_loft00;
			else if (_pnlLoFT == pnl_Loft09) tb = tb9_loft02;
			else if (_pnlLoFT == pnl_Loft10) tb = tb10_loft04;
			else if (_pnlLoFT == pnl_Loft11) tb = tb11_loft06;
			else if (_pnlLoFT == pnl_Loft12) tb = tb12_loft08;
			else if (_pnlLoFT == pnl_Loft13) tb = tb13_loft10;
			else if (_pnlLoFT == pnl_Loft14) tb = tb14_loft12;
			else if (_pnlLoFT == pnl_Loft15) tb = tb15_loft14;
			else if (_pnlLoFT == pnl_Loft16) tb = tb16_loft16;
			else if (_pnlLoFT == pnl_Loft17) tb = tb17_loft18;
			else if (_pnlLoFT == pnl_Loft18) tb = tb18_loft20;
			else                             tb = tb19_loft22; // if (_pnlLoFT == pnl_Loft19)

			if (Int32.Parse(tb.Text) != id)
			{
				Changed = true;

				tb.Text = id.ToString();
				if      (_pnlLoFT == pnl_Loft08) Records[SelId].Record.Loft1  = (byte)id;
				else if (_pnlLoFT == pnl_Loft09) Records[SelId].Record.Loft2  = (byte)id;
				else if (_pnlLoFT == pnl_Loft10) Records[SelId].Record.Loft3  = (byte)id;
				else if (_pnlLoFT == pnl_Loft11) Records[SelId].Record.Loft4  = (byte)id;
				else if (_pnlLoFT == pnl_Loft12) Records[SelId].Record.Loft5  = (byte)id;
				else if (_pnlLoFT == pnl_Loft13) Records[SelId].Record.Loft6  = (byte)id;
				else if (_pnlLoFT == pnl_Loft14) Records[SelId].Record.Loft7  = (byte)id;
				else if (_pnlLoFT == pnl_Loft15) Records[SelId].Record.Loft8  = (byte)id;
				else if (_pnlLoFT == pnl_Loft16) Records[SelId].Record.Loft9  = (byte)id;
				else if (_pnlLoFT == pnl_Loft17) Records[SelId].Record.Loft10 = (byte)id;
				else if (_pnlLoFT == pnl_Loft18) Records[SelId].Record.Loft11 = (byte)id;
				else                             Records[SelId].Record.Loft12 = (byte)id;

				_pnlLoFT.Invalidate();
			}
		}


		internal static Bitmap Isocube;
		private static GraphicsPath CuboidOutlinePath;
		private static GraphicsPath CuboidTopAnglePath;
		private static GraphicsPath CuboidBotAnglePath;
		private static GraphicsPath CuboidVertLineTopPath;
		private static GraphicsPath CuboidVertLineBotPath;

		private Font fontRose = new Font("Courier New", 8);

		/// <summary>
		/// Paints a 3d LoFT representation in the IsoLoft panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_IsoLoft(object sender, PaintEventArgs e)
		{
			var graphics = e.Graphics;

			graphics.DrawRectangle(
								_penBlack,
								0,
								0,
								pnl_IsoLoft.Width  - 1,
								pnl_IsoLoft.Height - 1);

			if (SelId != -1 && LoFT != null)
			{
				string rose;
				var pt = new Point();
				for (int i = 0; i != 4; ++i)
				{
					switch (i)
					{
						default: //case 0:
							rose = "w";
							pt.X = 11;
							pt.Y =  6;
							break;
						case 1:
							rose = "n";
							pt.X = pnl_IsoLoft.Width - 24;
							pt.Y = 6;
							break;
						case 2:
							rose = "e";
							pt.X = pnl_IsoLoft.Width  - 24;
							pt.Y = pnl_IsoLoft.Height - 24;
							break;
						case 3:
							rose = "s";
							pt.X = 11;
							pt.Y = pnl_IsoLoft.Height - 24;
							break;
					}
					TextRenderer.DrawText(
										graphics,
										rose,
										fontRose,
										pt,
										Color.Gray);
				}

				graphics.SmoothingMode = SmoothingMode.AntiAlias;

				graphics.DrawPath(_penGray, CuboidOutlinePath);
				graphics.DrawPath(_penGray, CuboidBotAnglePath);
				graphics.DrawPath(_penGray, CuboidVertLineTopPath);


				int halfwidth  = Isocube.Width  / 2;
				int halfheight = Isocube.Height / 2;

				int x_origin = pnl_IsoLoft.Width / 2 - 3;

				int y_layer, y_cell, x_cell;
				int loftid;

				McdRecord record = Records[SelId].Record;
				for (int layer = 0; layer != bar_IsoLoft.Value; ++layer)
				{
					switch (layer / 2)
					{
						default: loftid = record.Loft1;  break; // case 0
						case  1: loftid = record.Loft2;  break;
						case  2: loftid = record.Loft3;  break;
						case  3: loftid = record.Loft4;  break;
						case  4: loftid = record.Loft5;  break;
						case  5: loftid = record.Loft6;  break;
						case  6: loftid = record.Loft7;  break;
						case  7: loftid = record.Loft8;  break;
						case  8: loftid = record.Loft9;  break;
						case  9: loftid = record.Loft10; break;
						case 10: loftid = record.Loft11; break;
						case 11: loftid = record.Loft12; break;
					}

					y_layer = pnl_IsoLoft.Height - ((layer + 1) * halfheight) - (halfheight * 24);

					for (int r = 0; r != 16; ++r)
					for (int c = 0; c != 16; ++c)
					{
						y_cell = y_layer + (c * halfheight) + (r * halfheight) - (r + c);

						x_cell = x_origin + ((c * halfwidth) - (r * halfwidth));
						if      (x_cell > x_origin) x_cell += (c - r);
						else if (x_cell < x_origin) x_cell -= (r - c);

						if (LoFT[(loftid * 256) + (r * 16) + c])
						{
							graphics.DrawImage(Isocube, x_cell, y_cell);
						}
					}
				}
				graphics.DrawPath(_penGray, CuboidTopAnglePath);
				graphics.DrawPath(_penGray, CuboidVertLineBotPath);
			}
		}

		private void OnClick_IsoLoft(object sender, EventArgs e)
		{
			bar_IsoLoft.Select();
		}

/*		// RotatingCube -->
// OnPaint ->
			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.Clear(Color.Transparent);

			g.TranslateTransform(Width / 2, Height / 2);

			foreach (var edge in edges)
			{
				double[] xy1 = nodes[edge[0]];
				double[] xy2 = nodes[edge[1]];
				g.DrawLine(
						Pens.Black,
						(int)Math.Round(xy1[0]),
						(int)Math.Round(xy1[1]),
						(int)Math.Round(xy2[0]),
						(int)Math.Round(xy2[1]));
			}

			foreach (var node in nodes)
			{
				g.FillEllipse(
						Brushes.Black,
						(int)Math.Round(node[0]) - 4,
						(int)Math.Round(node[1]) - 4,
						8,8);
			} */
// class vars/functs ->
/*		double[][] nodes =
		{
			new double[] {-1, -1, -1},
			new double[] {-1, -1,  1},
			new double[] {-1,  1, -1},
			new double[] {-1,  1,  1},
			new double[] { 1, -1, -1},
			new double[] { 1, -1,  1},
			new double[] { 1,  1, -1},
			new double[] { 1,  1,  1}
		};

		int[][] edges =
		{
			new int[] {0, 1},
			new int[] {1, 3},
			new int[] {3, 2},
			new int[] {2, 0},
			new int[] {4, 5},
			new int[] {5, 7},
			new int[] {7, 6},
			new int[] {6, 4},
			new int[] {0, 4},
			new int[] {1, 5},
			new int[] {2, 6},
			new int[] {3, 7}
		};

		private void RotateCuboid(double angleX, double angleY)
		{
			double sinX = Math.Sin(angleX);
			double cosX = Math.Cos(angleX);

			double sinY = Math.Sin(angleY);
			double cosY = Math.Cos(angleY);

			foreach (var node in nodes)
			{
				double x = node[0];
				double y = node[1];
				double z = node[2];

				node[0] = x * cosX - z * sinX;
				node[2] = z * cosX + x * sinX;

				z = node[2];

				node[1] = y * cosY - z * sinY;
				node[2] = z * cosY + y * sinY;
			}
		}

		private void Scale(int v1, int v2, int v3)
		{
			foreach (var item in nodes)
			{
				item[0] *= v1;
				item[1] *= v2;
				item[2] *= v3;
			}
		} */
		#endregion LoFT
	}
}
