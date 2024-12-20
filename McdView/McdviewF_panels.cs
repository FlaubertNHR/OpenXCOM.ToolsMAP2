﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using DSShared;

using XCom;

// RotatingCube ->
//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Windows.Forms;
//using System.Windows.Threading;


namespace McdView
{
	public sealed partial class McdviewF
	{
		#region Anisprites
		private  static int SPRITE_ORIGIN_X;
		private  const  int SPRITE_ORIGIN_Y =  0;
		internal const  int SPRITE_OFFSET_X = 80;

		/// <summary>
		/// Spaces the layout of the fields etc. of the anisprites in the
		/// sprite-group and -panel.
		/// </summary>
		/// <remarks>See also CopierF.LayoutSpritesGroup().</remarks>
		private void LayoutSpritesGroup()
		{
			SPRITE_ORIGIN_X = gb_Sprites.Width / 2 - SPRITE_OFFSET_X * 4;
			if (SPRITE_ORIGIN_X < 0) SPRITE_ORIGIN_X = 0;

			pnl_Sprites.Width = (gb_Sprites.Width - 10);

			int left = pnl_Sprites.Left;
			int offset = Spriteset.SpriteWidth32 - (tb00_phase1.Width / 2);

			tb00_phase1.Left = left + SPRITE_ORIGIN_X + offset;
			tb01_phase2.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X;
			tb02_phase3.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 2;
			tb03_phase4.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 3;
			tb04_phase5.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 4;
			tb05_phase6.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 5;
			tb06_phase7.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 6;
			tb07_phase8.Left = left + SPRITE_ORIGIN_X + offset + SPRITE_OFFSET_X * 7;

			lbl00.Left = tb00_phase1.Left + (tb00_phase1.Width / 2) - ((lbl00.Width + lbl00_phase1.Width) / 2);
			lbl00_phase1.Left = lbl00.Right;

			lbl01.Left = lbl00.Left + SPRITE_OFFSET_X;
			lbl01_phase2.Left = lbl01.Right;

			lbl02.Left = lbl01.Left + SPRITE_OFFSET_X;
			lbl02_phase3.Left = lbl02.Right;

			lbl03.Left = lbl02.Left + SPRITE_OFFSET_X;
			lbl03_phase4.Left = lbl03.Right;

			lbl04.Left = lbl03.Left + SPRITE_OFFSET_X;
			lbl04_phase5.Left = lbl04.Right;

			lbl05.Left = lbl04.Left + SPRITE_OFFSET_X;
			lbl05_phase6.Left = lbl05.Right;

			lbl06.Left = lbl05.Left + SPRITE_OFFSET_X;
			lbl06_phase7.Left = lbl06.Right;

			lbl07.Left = lbl06.Left + SPRITE_OFFSET_X;
			lbl07_phase8.Left = lbl07.Right;
		}

		/// <summary>
		/// Handles the Paint event for the anisprite groupbox's sprite-panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_Sprites(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;

			if (Selid != -1 && Spriteset != null)
			{
				graphics.PixelOffsetMode   = PixelOffsetMode.Half;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

				McdRecord record = Parts[Selid].Record;
				int y = SPRITE_ORIGIN_Y;

				int yoffset = record.SpriteOffset;
				if (yoffset != 0)
				{
					y -= yoffset * 2;

					var rect = new Rectangle(
										0,
										SPRITE_ORIGIN_Y,
										Spriteset.SpriteWidth32  * 2,
										Spriteset.SpriteHeight40 * 2);

					for (int i = 0; i != 8; ++i)
					{
						rect.X = SPRITE_ORIGIN_X + SPRITE_OFFSET_X * i;
						graphics.FillRectangle(Brushes.Black, rect); // actually palette-id #0 Transparent
					}
				}

				byte id;
				for (int i = 0; i != 8; ++i)
				{
					switch (i)
					{
						default: id = record.Sprite1; break;
						case 1:  id = record.Sprite2; break;
						case 2:  id = record.Sprite3; break;
						case 3:  id = record.Sprite4; break;
						case 4:  id = record.Sprite5; break;
						case 5:  id = record.Sprite6; break;
						case 6:  id = record.Sprite7; break;
						case 7:  id = record.Sprite8; break;
					}

					if (id < Spriteset.Count)
						graphics.DrawImage(
										Spriteset[id].Sprite,
										new Rectangle(
													SPRITE_ORIGIN_X + SPRITE_OFFSET_X * i, y,
													Spriteset.SpriteWidth32  * 2,
													Spriteset.SpriteHeight40 * 2),
										0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
										GraphicsUnit.Pixel,
										Ia);
					else
						graphics.FillRectangle(
											Colors.BrushInvalid,
											SPRITE_ORIGIN_X + SPRITE_OFFSET_X * i,
											SPRITE_ORIGIN_Y,
											Spriteset.SpriteWidth32  * 2,
											Spriteset.SpriteHeight40 * 2);
				}
			}
			else // draw blank rectanges ->
			{
				var rect = new Rectangle(
									0,
									SPRITE_ORIGIN_Y + 1,
									Spriteset.SpriteWidth32  * 2 - 2,
									Spriteset.SpriteHeight40 * 2 - 2);

				for (int i = 0; i != 8; ++i)
				{
					rect.X = SPRITE_ORIGIN_X + SPRITE_OFFSET_X * i + 1;
					graphics.DrawRectangle(Colors.PenText, rect);
				}
			}
		}


		/// <summary>
		/// Opens the sprite-chooser when a sprite-phase is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>If user has the openfile dialog open and double-clicks to
		/// open a file that happens to be over the panel a <c>MouseUp</c> event
		/// fires. So use <c>MouseDown</c> here.</remarks>
		private void OnMouseDown_SpritePanel(object sender, MouseEventArgs e)
		{
			PartsPanel.Select();

			switch (e.Button)
			{
				case MouseButtons.Left:
				case MouseButtons.Right:
					if (Parts != null && Parts.Length != 0 && Selid != -1)
//						&& e.Y > -1 && e.Y < pnl_Sprites.Height // NOTE: Bypass event if cursor moves off the panel before released.
					{
						int phase, left;
						for (phase = 0; phase != 8; ++phase) // find sprite-phase that was clicked ->
						{
							left = SPRITE_ORIGIN_X + phase * SPRITE_OFFSET_X;
							if (   e.X > left
								&& e.X < left + Spriteset.SpriteWidth32 * 2)
							{
								break;
							}
						}

						if (phase != 8)
						{
							if (Spriteset == null)
							{
								string copyable = Label + Environment.NewLine + Environment.NewLine
												+ Infobox.SplitString("A spriteset for the terrain can be created by"
																	+ " inserting records with the Copier or a spriteset"
																	+ " can be created externally with PckView.")
												+ Environment.NewLine + Environment.NewLine
												+ "Edit|Open Copier panel ...";

								// TODO: add a button to open the Copier

								using (var f = new Infobox(
														"Error",
														"Sprites not found.",
														copyable,
														InfoboxType.Error))
								{
									f.ShowDialog(this);
								}
							}
							else if (Spriteset.Count == 0)
							{
								string copyable = Infobox.SplitString("Sprites can be added by inserting records"
																	+ " with the Copier or externally with PckView.")
												+ Environment.NewLine + Environment.NewLine
												+ "Edit|Open Copier panel ...";

								// TODO: add a button to open the Copier

								using (var f = new Infobox(
														"Warning",
														"The terrain's spriteset has no sprites.",
														copyable,
														InfoboxType.Warn))
								{
									f.ShowDialog(this);
								}
							}
							else
							{
								string id;
								switch (phase)
								{
									default: id = tb00_phase1.Text; break; // #0
									case 1:  id = tb01_phase2.Text; break;
									case 2:  id = tb02_phase3.Text; break;
									case 3:  id = tb03_phase4.Text; break;
									case 4:  id = tb04_phase5.Text; break;
									case 5:  id = tb05_phase6.Text; break;
									case 6:  id = tb06_phase7.Text; break;
									case 7:  id = tb07_phase8.Text; break;
								}

								switch (e.Button)
								{
									case MouseButtons.Left:
										using (var f = new SpriteChooserF(this, phase, Int32.Parse(id)))
											f.ShowDialog(this);
										break;

									case MouseButtons.Right:
										if (CanSetAllSprites(id))
										{
											using (var f = new Infobox(
																	"Set all sprite phases",
																	"Set all sprite phases to #" + id,
																	null,
																	InfoboxType.Warn,
																	InfoboxButton.CancelOkay))
											{
												if (f.ShowDialog(this) == DialogResult.OK)
													SetAllSprites(id);
											}
										}
										break;
								}
							}
						}
					}
					break;
			}
		}

		/// <summary>
		/// Sets an anisprite before returning from 'SpriteChooserF'.
		/// </summary>
		/// <param name="phase"></param>
		/// <param name="id"></param>
		internal void SetSprite(int phase, int id)
		{
			string val = id.ToString();
			switch (phase)
			{
				case 0: tb00_phase1.Text = val; break;
				case 1: tb01_phase2.Text = val; break;
				case 2: tb02_phase3.Text = val; break;
				case 3: tb03_phase4.Text = val; break;
				case 4: tb04_phase5.Text = val; break;
				case 5: tb05_phase6.Text = val; break;
				case 6: tb06_phase7.Text = val; break;
				case 7: tb07_phase8.Text = val; break;
			}
		}

		/// <summary>
		/// Sets all anisprites to a specified id.
		/// </summary>
		/// <param name="id"></param>
		internal void SetAllSprites(string id)
		{
			tb00_phase1.Text =
			tb01_phase2.Text =
			tb02_phase3.Text =
			tb03_phase4.Text =
			tb04_phase5.Text =
			tb05_phase6.Text =
			tb06_phase7.Text =
			tb07_phase8.Text = id;
		}

		/// <summary>
		/// Checks if any phase differs from the current phase.
		/// </summary>
		/// <param name="id">the sprite-id of the currently selected phase</param>
		/// <returns>true if any are different</returns>
		internal bool CanSetAllSprites(string id)
		{
			return tb00_phase1.Text != id
				|| tb01_phase2.Text != id
				|| tb02_phase3.Text != id
				|| tb03_phase4.Text != id
				|| tb04_phase5.Text != id
				|| tb05_phase6.Text != id
				|| tb06_phase7.Text != id
				|| tb07_phase8.Text != id;
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
			if (Selid != -1 && ScanG != null)
			{
				int id = Int32.Parse(tb20_scang1.Text);
				if (id > ScanGicon.UNITICON_Max && id < ScanG.Length / ScanGicon.Length_ScanG)
				{
					Graphics graphics = e.Graphics;
					graphics.PixelOffsetMode   = PixelOffsetMode.Half;
					graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

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

								palid = ScanG[id, (row * 4) + col];
								*pixel = (byte)palid;
							}
						}
						icon.UnlockBits(data);

						icon.Palette = Pal.Table;

						ColorPalette pal = icon.Palette; // palettes get copied not referenced ->
						pal.Entries[Palette.Tid] = Color.Transparent;
						icon.Palette = pal;

						var panel = sender as Panel;
						graphics.DrawImage(
										icon,
										new Rectangle(
													0,0,
													panel.Width,
													panel.Height),
										0,0, icon.Width, icon.Height,
										GraphicsUnit.Pixel,
										Ia);
					}
				}
			}
		}

		/// <summary>
		/// Opens the ScanG viewer when the ScanG icon is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>If user has the openfile dialog open and double-clicks to
		/// open a file that happens to be over the panel a mouse-up event
		/// fires. So use MouseDown here.</remarks>
		private void OnMouseDown_ScanGicon(object sender, MouseEventArgs e)
		{
			PartsPanel.Select();

			if (e.Button == MouseButtons.Left
				&& Parts != null && Parts.Length != 0 && Selid != -1)
//				&& e.X > -1 && e.X < pnl_ScanGic.Width // NOTE: Bypass event if cursor moves off the panel before released.
//				&& e.Y > -1 && e.Y < pnl_ScanGic.Height
			{
				if (ScanG != null)
				{
					using (var f = new ScangChooserF(
												this,
												Int32.Parse(tb20_scang1.Text),
												Pal.Table))
					{
						f.ShowDialog(this);
					}
				}
				else
				{
					using (var f = new Infobox(
											"Error",
											"ScanG icons not loaded.",
											null,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
			}
		}

		/// <summary>
		/// Sets a ScanG icon before returning from 'ScangChooserF'.
		/// </summary>
		/// <param name="id"></param>
		internal void SetIcon(int id)
		{
			tb20_scang1.Text = id.ToString();
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
			LoftPanel pnlLoFT;
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

		internal LoftPanel _pnlLoFT;

		/// <summary>
		/// Sets a LoFT before returning from <c><see cref="LoftChooserF"/></c>.
		/// </summary>
		/// <param name="id"></param>
		internal void SetLoft(int id)
		{
			(_pnlLoFT.Tag as TextBox).Text = id.ToString();
		}

		/// <summary>
		/// Sets all LoFTs to a specified id.
		/// </summary>
		/// <param name="id"></param>
		internal void SetAllLofts(string id)
		{
			tb08_loft01.Text =
			tb09_loft02.Text =
			tb10_loft03.Text =
			tb11_loft04.Text =
			tb12_loft05.Text =
			tb13_loft06.Text =
			tb14_loft07.Text =
			tb15_loft08.Text =
			tb16_loft09.Text =
			tb17_loft10.Text =
			tb18_loft11.Text =
			tb19_loft12.Text = id;
		}

		/// <summary>
		/// Checks if any loft-id differs from the current loft-id.
		/// </summary>
		/// <param name="id">the loft-id of the currently selected loft</param>
		/// <returns>true if any are different</returns>
		internal bool CanSetAllLofts(string id)
		{
			return tb08_loft01.Text != id
				|| tb09_loft02.Text != id
				|| tb10_loft03.Text != id
				|| tb11_loft04.Text != id
				|| tb12_loft05.Text != id
				|| tb13_loft06.Text != id
				|| tb14_loft07.Text != id
				|| tb15_loft08.Text != id
				|| tb16_loft09.Text != id
				|| tb17_loft10.Text != id
				|| tb18_loft11.Text != id
				|| tb19_loft12.Text != id;
		}


		internal static Bitmap Isocube;
		private  static GraphicsPath CuboidOutlinePath;
		private  static GraphicsPath CuboidTopAnglePath;
		private  static GraphicsPath CuboidBotAnglePath;
		private  static GraphicsPath CuboidVertLineTopPath;
		private  static GraphicsPath CuboidVertLineBotPath;

		private Font _fontRose = new Font("Courier New", 8);

		/// <summary>
		/// Paints a 3d LoFT representation in the IsoLoft panel as well as the
		/// LoFT panels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_IsoLoft(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;

			graphics.DrawRectangle(
								Colors.PenText,
								0,0,
								pnl_IsoLoft.Width  - 1,
								pnl_IsoLoft.Height - 1);

			if (Parts != null && Selid != -1 && LoFT != null)
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
										_fontRose,
										pt,
										SystemColors.ControlDark);
				}

				graphics.SmoothingMode = SmoothingMode.AntiAlias;

				graphics.DrawPath(Colors.PenLight, CuboidOutlinePath);
				graphics.DrawPath(Colors.PenLight, CuboidBotAnglePath);
				graphics.DrawPath(Colors.PenLight, CuboidVertLineTopPath);


				int halfwidth  = Isocube.Width  / 2;
				int halfheight = Isocube.Height / 2;

				int x_origin = pnl_IsoLoft.Width / 2 - 3;

				int y_layer, y_cell, x_cell;
				int loftid, pos;

				McdRecord record = Parts[Selid].Record;
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
						pos = (loftid * 256) + (r * 16) + c;
						if (pos < LoFT.Length && LoFT[pos])
						{
							y_cell = y_layer + (c * halfheight) + (r * halfheight) - (r + c);

							x_cell = x_origin + ((c * halfwidth) - (r * halfwidth));
							if      (x_cell > x_origin) x_cell += (c - r);
							else if (x_cell < x_origin) x_cell -= (r - c);

							graphics.DrawImage(Isocube, x_cell, y_cell);
						}
					}
				}
				graphics.DrawPath(Colors.PenLight, CuboidTopAnglePath);
				graphics.DrawPath(Colors.PenLight, CuboidVertLineBotPath);
			}
		}

		/// <summary>
		/// Clicking on a <c><see cref="LoftPanel"/></c> or on the
		/// <c><see cref="pnl_IsoLoft">IsoLoFT panel</see></c> selects the
		/// <c><see cref="bar_IsoLoft">IsoLoFT trackbar</see></c>. A rightclick
		/// on the <c>IsoLoFT panel</c> resets its trackbar to full.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>A mouseclick event does not fire if the cursor moves off
		/// the control before released.</remarks>
		internal void OnMouseClick_IsoLoft(object sender, MouseEventArgs e)
		{
			bar_IsoLoft.Select();

			if (sender != null && e.Button == MouseButtons.Right)
				bar_IsoLoft.Value = bar_IsoLoft.Maximum;
		}

		/// <summary>
		/// Changes the value of the
		/// <c><see cref="bar_IsoLoft">IsoLoFT trackbar</see></c> when the
		/// <c><see cref="pnl_IsoLoft">IsoLoFT panel</see></c> has focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The <c>IsoLoFT panel</c> cannot have focus because it is a
		/// <c>Panel</c> which would require <c>SetControlStyle()</c> to make it
		/// selectable - which could create issues in Win10 - but by calling
		/// <c>Application.AddMessageFilter()</c> in the app-constructor the
		/// panel can be forced to take a mousewheel-message anyway. Without
		/// <c>Application.AddMessageFilter()</c> <c>bar_IsoLoft.Select()</c>
		/// can be called in
		/// <c><see cref="OnMouseClick_IsoLoft()">OnMouseClick_IsoLoft()</see></c>
		/// to focus the <c>IsoLoFT trackbar</c> which will handle the
		/// mousewheel auto.</remarks>
		private void OnMouseWheel_IsoLoft(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				if (bar_IsoLoft.Value != bar_IsoLoft.Maximum)
					++bar_IsoLoft.Value;
			}
			else if (e.Delta < 0)
			{
				if (bar_IsoLoft.Value != bar_IsoLoft.Minimum)
					--bar_IsoLoft.Value;
			}
		}

		/// <summary>
		/// A rightclick on the
		/// <c><see cref="bar_IsoLoft">IsoLoFT trackbar</see></c> resets its
		/// value to full.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Bypass event if cursor moves off the trackbar before
		/// released.</remarks>
		private void OnMouseUp_BarIsoLoft(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right
				&& e.X > -1 && e.X < bar_IsoLoft.Width
				&& e.Y > -1 && e.Y < bar_IsoLoft.Height)
			{
				bar_IsoLoft.Value = bar_IsoLoft.Maximum;
			}
		}


		private static BlobColorTool ToolGray = new BlobColorTool(Color.Gray);

		/// <summary>
		/// Paints a blob based on the current loftset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint_Blob(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;

			graphics.DrawRectangle(
								Colors.PenText,
								0,0,
								pnl_Blob.Width  - 1,
								pnl_Blob.Height - 1);

			if (Selid != -1)
			{
				McdRecord record = Parts[Selid].Record;

				BlobTypeService.UpdateLoftlist(record);

				byte loftid;
				if (miResourcesTftd.Checked) loftid = BlobTypeService.LOFTID_Max_tftd;
				else                         loftid = BlobTypeService.LOFTID_Max_ufo;

				lbl_Extended.Visible = BlobTypeService.hasExtendedLofts(BlobTypeService._loftlist, loftid);


				switch (record.PartType)
				{
//					default:
//					case PartType.Invalid: // -1
//						break;

					case PartType.Floor:
					{
						Brush brush;
						if (record.Loft1 == 0) brush = Brushes.DarkGray; // is lighter than Gray
						else                   brush = Brushes.Gray;

						BlobDrawService.DrawFloor(
											graphics,
											brush,
											pnl_Blob.Width / 2, 0,
											record.Loft1,
											BlobDrawCoordinator._path,
											BlobDrawCoordinator._halfwidth, BlobDrawCoordinator._halfheight);

						if (record.GravLift != 0) // draw GravLift floor as content-part
							goto case PartType.Content;

						break;
					}

					case PartType.West:
					case PartType.North:
					case PartType.Content:
						BlobDrawService.DrawWallOrContent(
													graphics,
													ToolGray,
													pnl_Blob.Width / 2, 0,
													Parts[Selid],
													BlobDrawCoordinator._path,
													BlobDrawCoordinator._halfwidth, BlobDrawCoordinator._halfheight);
						break;
				}

				BlobTypeService._loftlist.Clear();
			}
			else
				lbl_Extended.Visible = false;
		}
		#endregion LoFT
	}
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
