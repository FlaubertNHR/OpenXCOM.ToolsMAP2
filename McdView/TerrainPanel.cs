using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace McdView
{
	/// <summary>
	/// A panel that displays an entire MCD recordset with each record's Sprite1
	/// sprite. This class is inherited by 'TerrainPanel_main' and
	/// 'TerrainPanel_copy'.
	/// </summary>
	internal class TerrainPanel
		:
			Panel
	{
		#region Fields (static)
		protected static McdviewF _f;

		protected readonly static Brush BrushHilightsub = new SolidBrush(Color.FromArgb(36, SystemColors.MenuHighlight));
		#endregion Fields (static)


		#region Fields
		protected readonly HScrollBar Scroller = new HScrollBar();

		protected const int _largeChange = XCImage.SpriteWidth32 + 1;

		protected readonly Pen   _penControl   = new Pen(SystemColors.Control, 1);
		protected readonly Brush _brushControl = new SolidBrush(SystemColors.Control);

		protected int TableWidth;

		protected bool _bypassScrollZero;

		internal protected readonly SortedSet<int> SubIds = new SortedSet<int>();
		protected readonly List<Tilepart> _copyparts = new List<Tilepart>();
		protected string _copylabel;

		protected Tilepart[] _parts;
		#endregion Fields


		#region Properties
		protected virtual int SelId
		{ get; set; }

		internal protected Tilepart[] Parts
		{
			get { return _parts; }
			set // IMPORTANT: Set 'Parts' via McdviewF only or via OpenFileDialog for the copypanel.
			{
				_parts = value;

				TableWidth = _parts.Length * (XCImage.SpriteWidth32 + 1) - 1;

				OnResize(null);

				if (!_bypassScrollZero)
					Scroller.Value = 0;
				else
					_bypassScrollZero = false;

				Invalidate();
			}
		}

		protected ContextMenu Context
		{ get; set; }
		#endregion Properties


		#region cTor
		internal TerrainPanel(McdviewF f)
		{
			_f = f;

//			SetStyle(ControlStyles.OptimizedDoubleBuffer
//				   | ControlStyles.AllPaintingInWmPaint
//				   | ControlStyles.UserPaint
//				   | ControlStyles.ResizeRedraw
//				   | ControlStyles.Selectable, true);
			SetStyle(ControlStyles.Selectable, true);

			Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			BackColor = SystemColors.Desktop;
			Margin = new Padding(0);
			Name = "pnl_Collection";
			TabIndex = 0;
			TabStop = true;

			Location = new Point(5, 15);

			Scroller.Dock = DockStyle.Bottom;
			Scroller.LargeChange = _largeChange;
			Scroller.ValueChanged += OnValueChanged_Scroll;
			Controls.Add(Scroller);

			Height = y3_sprite + XCImage.SpriteHeight40 + Scroller.Height;
		}
		#endregion cTor


		#region Events (context)
//		/// <summary>
//		/// Closes the contextmenu.
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void OnIdClick(object sender, EventArgs e)
//		{
//			Context.Dispose();
//		}

		/// <summary>
		/// Copies a currently selected part along with any sub-selected parts
		/// to the copy-array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCopyClick(object sender, EventArgs e)
		{
			_copyparts.Clear();

			_copylabel = _f.Label;

			var sels = new List<int>(SubIds);
			sels.Add(SelId);
			sels.Sort();

			foreach (int id in sels)
				_copyparts.Add(Parts[id].Clone());
		}

		/// <summary>
		/// Selects the last part and sub-selects all other parts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSelectAllClick(object sender, EventArgs e)
		{
			for (int i = 0; i != Parts.Length - 1; ++i)
				SubIds.Add(i);

			if (SelId != Parts.Length - 1)
				SelId  = Parts.Length - 1;
			else
				Invalidate();
		}

		/// <summary>
		/// Deselects a currently selected part as well as any sub-selected
		/// parts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeselectAllClick(object sender, EventArgs e)
		{
			SubIds.Clear();
			SelId = -1;
		}
		#endregion Events (context)


		#region Events
		/// <summary>
		/// Refreshes the PartsPanel when the scrollbar's value changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnValueChanged_Scroll(object sender, EventArgs e)
		{
			Invalidate();
		}
		#endregion Events


		#region Events (override)
		private Graphics _graphics;
		private ImageAttributes _attri;

		// constants for vertical align ->
		const int y1_sprite = 0;
		const int y1_fill   = XCImage.SpriteHeight40;
		const int y1_fill_h = 18;
		const int y2_sprite = y1_fill + y1_fill_h;
		const int y2_line   = y2_sprite + XCImage.SpriteHeight40 + 1;
		const int y3_sprite = y2_line;

		/// <summary>
		/// Paints this TerrainPanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Parts != null && Parts.Length != 0)
			{
				_graphics = e.Graphics;
				_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				_attri = new ImageAttributes();
				if (_f._spriteShadeEnabled)
					_attri.SetGamma(_f.SpriteShadeFloat, ColorAdjustType.Bitmap);

				Bitmap sprite;

				Rectangle rect;

				int offset = -Scroller.Value;

				int i;
				for (i = 0; i != Parts.Length; ++i)
				{
					if (i != 0)
						_graphics.DrawLine( // draw vertical line before each sprite except the first sprite
										_penControl,
										i * XCImage.SpriteWidth32 + i + offset, 0,
										i * XCImage.SpriteWidth32 + i + offset, Height);

					if (_f.Spriteset != null)
					{
						Tilepart part = Parts[i];
						if (part != null // not sure why Tilepart entries are null that aren't null but they are
							&& part.Record.Sprite1 < _f.Spriteset.Count
							&& part[0] != null
							&& (sprite = part[0].Sprite) != null)
						{
							DrawSprite(
									sprite,
									i * XCImage.SpriteWidth32 + i + offset,
									y1_sprite - part.Record.TileOffset);
						}
						else
							_graphics.FillRectangle(
												McdviewF.BrushSpriteInvalid,
												i * XCImage.SpriteWidth32 + i + offset,
												y1_sprite,
												XCImage.SpriteWidth32,
												XCImage.SpriteHeight40);
					}
				}

				_graphics.FillRectangle(
									_brushControl,
									0,     y1_fill,
									Width, y1_fill_h);

				if (SelId != -1)
				{
					_graphics.FillRectangle(
										McdviewF.BrushHilight,
										SelId * (XCImage.SpriteWidth32 + 1) + offset,
										y1_fill,
										XCImage.SpriteWidth32,
										y1_fill_h);

					foreach (int id in SubIds)
						_graphics.FillRectangle(
											BrushHilightsub,
											id * (XCImage.SpriteWidth32 + 1) + offset,
											y1_fill,
											XCImage.SpriteWidth32,
											y1_fill_h);
				}

				for (i = 0; i != Parts.Length; ++i)
				{
					rect = new Rectangle(
									i * XCImage.SpriteWidth32 + i + offset,
									y1_fill,
									XCImage.SpriteWidth32,
									y1_fill_h);

					TextRenderer.DrawText(
										_graphics,
										i.ToString(),
										Font,
										rect,
										SystemColors.ControlText,
										McdviewF.FLAGS);
				}

				if (_f.Spriteset != null)
				{
					for (i = 0; i != Parts.Length; ++i) // dead part ->
					{
						Tilepart part = Parts[i];
						if (part != null)
						{
							if (part.Dead != null
								&& part.Dead.Record.Sprite1 < _f.Spriteset.Count
								&& part.Dead[0] != null
								&& (sprite = part.Dead[0].Sprite) != null)
							{
								DrawSprite(
										sprite,
										i * XCImage.SpriteWidth32 + i + offset,
										y2_sprite - part.Dead.Record.TileOffset);
							}
							else if (part.Record.DieTile != 0)
								_graphics.FillRectangle(
													McdviewF.BrushSpriteInvalid,
													i * XCImage.SpriteWidth32 + i + offset,
													y2_sprite,
													XCImage.SpriteWidth32,
													XCImage.SpriteHeight40);
						}
					}
				}

				_graphics.DrawLine(
								_penControl,
								0,     y2_line,
								Width, y2_line);

				if (_f.Spriteset != null)
				{
					for (i = 0; i != Parts.Length; ++i) // alternate part ->
					{
						Tilepart part = Parts[i];
						if (part != null)
						{
							if (part.Alternate != null
								&& part.Alternate.Record.Sprite1 < _f.Spriteset.Count
								&& part.Alternate[0] != null
								&& (sprite = part.Alternate[0].Sprite) != null)
							{
								DrawSprite(
										sprite,
										i * XCImage.SpriteWidth32 + i + offset,
										y3_sprite - part.Alternate.Record.TileOffset);
							}
							else if (part.Record.Alt_MCD != 0)
								_graphics.FillRectangle(
													McdviewF.BrushSpriteInvalid,
													i * XCImage.SpriteWidth32 + i + offset,
													y3_sprite,
													XCImage.SpriteWidth32,
													XCImage.SpriteHeight40);
						}
					}
				}
			}
		}

		/// <summary>
		/// Helper for OnPaint().
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
										XCImage.SpriteWidth32,
										XCImage.SpriteHeight40),
							0, 0, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
							GraphicsUnit.Pixel,
							_attri);
		}


		/// <summary>
		/// Handles client resizing. Sets the scrollbar's Enabled and Maximum
		/// values.
		/// @note Holy f*ck I hate .NET scrollbars.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			if (eventargs != null) // ie. is *not* Parts load
				base.OnResize(eventargs);

			int range = 0;
			if (Parts != null && Parts.Length != 0)
			{
				range = TableWidth + (_largeChange - 1) - Width;
				if (range < _largeChange)
					range = 0;
			}

			Scroller.Maximum =  range;
			Scroller.Enabled = (range != 0);

			if (Scroller.Enabled
				&& TableWidth < Width + Scroller.Value)
			{
				Scroller.Value = TableWidth - Width;
			}
			ScrollToPart();
		}

		/// <summary>
		/// Scrolls the table by the mousewheel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (Scroller.Enabled)
			{
				if (e.Delta > 0)
				{
					if (Scroller.Value - Scroller.LargeChange < 0)
						Scroller.Value = 0;
					else
						Scroller.Value -= Scroller.LargeChange;
				}
				else if (e.Delta < 0)
				{
					if (Scroller.Value + Scroller.LargeChange + (Scroller.LargeChange - 1) > Scroller.Maximum)
						Scroller.Value = Scroller.Maximum - (Scroller.LargeChange - 1);
					else
						Scroller.Value += Scroller.LargeChange;
				}
			}
		}


		/// <summary>
		/// @note If user has the openfile dialog open and double-clicks to open
		/// a file that happens to be over the panel a mouse-up event fires. So
		/// use MouseDown here.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (e.Button == MouseButtons.Left
				&& Parts != null && Parts.Length != 0
				&& e.Y < Height - Scroller.Height)
			{
				int id = (e.X + Scroller.Value) / (XCImage.SpriteWidth32 + 1);

				if (id < Parts.Length)
				{
					if (id == SelId && SubIds.Count != 0)
					{
						if ((ModifierKeys & (Keys.Control | Keys.Shift)) != 0)
						{
							int idl, idr;

							for (idl = id; idl != -1; --idl) // find subid left ->
							{
								if (SubIds.Contains(idl))
									break;
							}

							for (idr = id; idr != Parts.Length; ++idr) // find subid right ->
							{
								if (SubIds.Contains(idr))
									break;
							}

							if      (idl == -1)                  id = idr; // find closer of subid left/right ->
							else if (idr == Parts.Length)        id = idl;
							else if (idr - SelId <= SelId - idl) id = idr; // bias: right
							else                                 id = idl;

							SubIds.Remove(SelId);
							SelId = id;
						}
						else
						{
							SubIds.Clear();
							Invalidate();
						}
					}
					else if (ModifierKeys == Keys.Control && SelId != -1)
					{
						if (SubIds.Contains(id))
						{
							SubIds.Remove(id);
							Invalidate();
						}
						else if (id != SelId)
						{
							SubIds.Add(SelId);
							SelId = id;
						}
					}
					else if (ModifierKeys == Keys.Shift && SelId != -1)
					{
						SubIds.Clear();

						if (id == SelId)
						{
							id = -1;
						}
						if (id < SelId)
						{
							for (int i = SelId; i != id; --i)
								SubIds.Add(i);
						}
						else // (id > SelId)
						{
							for (int i = SelId; i != id; ++i)
								SubIds.Add(i);
						}
						SelId = id;
					}
					else
					{
						SubIds.Clear();
						SelId = id;
					}
				}
			}
		}

		/// <summary>
		/// This is required in order to accept arrow-keyboard-input via
		/// McdviewF.OnKeyDown().
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Right:
				case Keys.Down:
				case Keys.Shift | Keys.Left:
				case Keys.Shift | Keys.Up:
				case Keys.Shift | Keys.Right:
				case Keys.Shift | Keys.Down:
					return true;
			}
			return base.IsInputKey(keyData);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Scrolls the panel to ensure that the currently selected part is
		/// fully displayed.
		/// </summary>
		internal void ScrollToPart()
		{
			if (SelId != -1 && Scroller.Enabled)
			{
				int x = SelId * (XCImage.SpriteWidth32 + 1);
				if (x < Scroller.Value)
				{
					Scroller.Value = x;
				}
				else if ((x += XCImage.SpriteWidth32) > Width + Scroller.Value)
				{
					Scroller.Value = x - Width;
				}
			}
		}

/*		/// <summary>
		/// Takes keyboard-input from the Form's KeyDown event to select a part
		/// or parts.
		/// </summary>
		/// <param name="e"></param>
		internal void KeyInput(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Back:
					if (SelId != -1)
					{
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == 0)
									Invalidate();
							}
							else if (SelId != 0)
								SubIds.Remove(SelId);
						}
						else if (SelId != 0)
							SubIds.Add(SelId);

						if (SelId != 0)
							SelId -= 1;
					}
					break;

				case Keys.Right:
				case Keys.Down:
					if (SelId != -1)
						goto case Keys.Space;
					break;
				case Keys.Space: // at present only the spacebar can change the selected id from #-1 to #0
					if (!e.Control)
					{
						if (!e.Shift)
						{
							SubIds.Clear();
							if (SelId == Parts.Length - 1)
								Invalidate();
						}
						else if (SelId != Parts.Length - 1)
							SubIds.Remove(SelId);
					}
					else if (SelId != Parts.Length - 1)
						SubIds.Add(SelId);

					if (SelId != Parts.Length - 1)
						SelId += 1;
					break;

				case Keys.PageUp:
					if (SelId != -1)
					{
						int id = SelId - (Width / (XCImage.SpriteWidth32 + 1));
						if (id < 0) id = 0;
	
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == 0)
									Invalidate();
							}
							else if (SelId != 0)
							{
								for (int i = SelId; i != id; --i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != 0)
						{
							for (int i = SelId; i != id; --i)
								SubIds.Add(i);
						}
	
						if (SelId != 0)
							SelId = id;
					}
					break;

				case Keys.PageDown:
					if (SelId != -1)
					{
						int id = SelId + (Width / (XCImage.SpriteWidth32 + 1));
						if (id > Parts.Length - 1) id = Parts.Length - 1;
	
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == Parts.Length - 1)
									Invalidate();
							}
							else if (SelId != Parts.Length - 1)
							{
								for (int i = SelId; i != id; ++i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != Parts.Length - 1)
						{
							for (int i = SelId; i != id; ++i)
								SubIds.Add(i);
						}
	
						if (SelId != Parts.Length - 1)
							SelId = id;
					}
					break;

				case Keys.Home:
					if (SelId != -1)
					{
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == 0)
									Invalidate();
							}
							else if (SelId != 0)
							{
								for (int i = SelId; i != 0; --i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != 0)
						{
							for (int i = SelId; i != 0; --i)
								SubIds.Add(i);
						}

						SelId = 0;
					}
					break;

				case Keys.End:
					if (SelId != -1)
					{
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == Parts.Length - 1)
									Invalidate();
							}
							else if (SelId != Parts.Length - 1)
							{
								for (int i = SelId; i != Parts.Length - 1; ++i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != Parts.Length - 1)
						{
							for (int i = SelId; i != Parts.Length - 1; ++i)
								SubIds.Add(i);
						}

						SelId = Parts.Length - 1;
					}
					break;


				// Edit functions (keyboard) follow ...
				// IMPORTANT: The conditions shall be synched w/ OnPopup_Context().
				case Keys.D:
					if (Parts != null)
					{
						if (!e.Control)											// add
							OnAddClick(null, EventArgs.Empty);
						else													// add range
							OnAddRangeClick(null, EventArgs.Empty);
					}
					break;


				case Keys.X:													// cut
					if (e.Control && SelId != -1)
						OnCutClick(null, EventArgs.Empty);
					break;

				case Keys.C:													// copy
					if (e.Control && SelId != -1)
						OnCopyClick(null, EventArgs.Empty);
					break;

				case Keys.V:													// insert
					if (e.Control && Parts != null && _copyparts.Count != 0)
						OnInsertClick(null, EventArgs.Empty);
					break;

				case Keys.Delete:												// delete
					if (SelId != -1)
						OnDeleteClick(null, EventArgs.Empty);
					break;


				case Keys.OemMinus: // drugs ...
				case Keys.Subtract:												// swap left
					if (SelId > 0)
						OnSwapLeftClick(null, EventArgs.Empty);
					break;

				case Keys.Oemplus: // drugs ...
				case Keys.Add:													// swap right
					if (SelId != -1 && SelId != Parts.Length - 1)
						OnSwapRightClick(null, EventArgs.Empty);
					break;


				case Keys.A:													// select all
					if (e.Control && Parts != null && Parts.Length != 0)
						OnSelectAllClick(null, EventArgs.Empty);
					break;

				// NOTE: Escape for deselect all is handled by the caller: McdviewF.OnKeyDown().

				case Keys.F:													// append file
					if (Parts != null)
						OnFileClick(null, EventArgs.Empty);
					break;
			}
		} */

/*		/// <summary>
		/// Gets the loc of the currently selected tile relative to the table.
		/// </summary>
		/// <returns></returns>
		private int GetTileLeft()
		{
			return SelId * (XCImage.SpriteWidth32 + 1);
		}
		/// <summary>
		/// Gets the loc+width of the currently selected tile relative to the table.
		/// </summary>
		/// <returns></returns>
		private int GetTileRight()
		{
			return SelId * (XCImage.SpriteWidth32 + 1) + XCImage.SpriteWidth32;
		} */
		#endregion Methods
	}
}
