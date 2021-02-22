using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;

using XCom;


namespace McdView
{
	/// <summary>
	/// A panel that displays an entire MCD recordset with each record's Sprite1
	/// sprite. This class is inherited by 'TerrainPanel_main' and
	/// 'TerrainPanel_copier'.
	/// </summary>
	internal class TerrainPanel
		:
			BufferedPanel
	{
		#region Fields (static)
		protected static McdviewF _f;

		protected static readonly List<Tilepart> _partsCopied = new List<Tilepart>();
		protected static string _partsCopiedLabel;
		#endregion Fields (static)


		#region Fields
		private readonly HScrollBar Scroller = new HScrollBar();

		private const int _largeChange = XCImage.SpriteWidth32 + 1;

		private int TableWidth;

		protected bool _bypassScrollZero;

		internal protected readonly SortedSet<int> SubIds = new SortedSet<int>();
		#endregion Fields


		#region Properties
		internal protected CopierF _fcopier
		{ get; private set; }

		protected virtual int SelId
		{ get; set; }

		private Tilepart[] _parts;
		/// <summary>
		/// An array of 'Tilepart'.
		/// !!IMPORTANT: Only set 'Parts' via 'McdviewF.Parts' when
		/// instantiating a 'TerrainPanel_main' object and via 'CopierF.Parts'
		/// when instantiating a 'TerrainPanel_copier' object. IMPORTANT!!
		/// </summary>
		internal protected Tilepart[] Parts
		{
			get { return _parts; }
			set
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

		/// <summary>
		/// The spriteset. Use this var only for drawing; get the spriteset
		/// directly in 'McdviewF' otherwise. (no reason, jic)
		/// !!IMPORTANT: Set Spriteset only via 'McdviewF' or 'CopierF' as
		/// appropriate. IMPORTANT!!
		/// </summary>
		internal protected SpriteCollection Spriteset
		{ private get; set; }


		protected ContextMenuStrip Context
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="fcopier"></param>
		protected TerrainPanel(
				McdviewF f,
				CopierF fcopier = null)
		{
			_f       = f;
			_fcopier = fcopier;	// prevent the Copier from borking out during its initial
								// OnResize events when it tries to get a 'SelId' from 'CopierF'.
								// That is, '_fcopier' is irrelevant to instantiations of
								// 'TerrainPanel_main'; is used only by 'TerrainPanel_copier'.

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
//		{ Context.Dispose(); }


		// NOTE on the edit-operations: Do not clone on copy/cut.
		// Clone only on insert.

		/// <summary>
		/// Copies a currently selected part along with any sub-selected parts
		/// to the copy-array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnCopyClick(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				_partsCopied.Clear();

				if (_fcopier == null)
					_partsCopiedLabel = _f.Label;
				else
					_partsCopiedLabel = _fcopier.Label;

				var sels = new HashSet<int>(SubIds);
				sels.Add(SelId);

				foreach (int sel in sels)
					_partsCopied.Add(Parts[sel]);
			}
		}

		/// <summary>
		/// Selects the last part and sub-selects all other parts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnSelectAllClick(object sender, EventArgs e)
		{
			if (Parts != null && Parts.Length != 0)
			{
				for (int i = 0; i != Parts.Length - 1; ++i)
					SubIds.Add(i);

				if (SelId != Parts.Length - 1)
					SelId  = Parts.Length - 1;
				else
					Invalidate();
			}
		}

		/// <summary>
		/// Deselects a currently selected part as well as any sub-selected
		/// parts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnDeselectAllClick(object sender, EventArgs e)
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
		// constants for vertical align ->
		const int y1_sprite = 0;
		const int y1_fill   = XCImage.SpriteHeight40;
		const int y1_fill_h = 18;
		const int y2_sprite = y1_fill + y1_fill_h;
		const int y2_line   = y2_sprite + XCImage.SpriteHeight40 + 1;
		const int y3_sprite = y2_line;

		private Graphics _graphics;

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

				Bitmap sprite;

				Rectangle rect;

				int offset = -Scroller.Value;

				int i, spriteId, x;
				for (i = 0; i != Parts.Length; ++i)
				{
					x = i * XCImage.SpriteWidth32 + i + offset;

					if (i != 0)
						_graphics.DrawLine( // draw vertical line before each sprite except the first sprite
										Colors.PenControl,
										x, 0,
										x, Height);

					if (Spriteset != null && Spriteset.Count != 0)
					{
						Tilepart part = Parts[i];

						if ((spriteId = part.Record.Sprite1) < Spriteset.Count
							&& (sprite = Spriteset[spriteId].Sprite) != null)
						{
							DrawSprite(
									sprite,
									x,
									y1_sprite,
									part.Record.TileOffset);
						}
						else
							_graphics.FillRectangle(
												Colors.BrushInvalid,
												x,
												y1_sprite,
												XCImage.SpriteWidth32,
												XCImage.SpriteHeight40);
					}
				}

				_graphics.FillRectangle(
									Colors.BrushControl,
									0,     y1_fill,
									Width, y1_fill_h);

				if (SelId != -1)
				{
					_graphics.FillRectangle(
										Colors.BrushHilight,
										SelId * (XCImage.SpriteWidth32 + 1) + offset,
										y1_fill,
										XCImage.SpriteWidth32,
										y1_fill_h);

					foreach (int id in SubIds)
						_graphics.FillRectangle(
											Colors.BrushHilightsubsel,
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

				if (Spriteset != null && Spriteset.Count != 0)
				{
					for (i = 0; i != Parts.Length; ++i) // dead part ->
					{
						Tilepart part = Parts[i];
						if (part.Record.DieTile != 0)
						{
							if (part.Dead != null
								&& (spriteId = part.Dead.Record.Sprite1) < Spriteset.Count
								&& (sprite = Spriteset[spriteId].Sprite) != null)
							{
								DrawSprite(
										sprite,
										i * XCImage.SpriteWidth32 + i + offset,
										y2_sprite,
										part.Dead.Record.TileOffset);
							}
							else
								_graphics.FillRectangle(
													Colors.BrushInvalid,
													i * XCImage.SpriteWidth32 + i + offset,
													y2_sprite,
													XCImage.SpriteWidth32,
													XCImage.SpriteHeight40);
						}
					}
				}

				_graphics.DrawLine(
								Colors.PenControl,
								0,     y2_line,
								Width, y2_line);

				if (Spriteset != null && Spriteset.Count != 0)
				{
					for (i = 0; i != Parts.Length; ++i) // alternate part ->
					{
						Tilepart part = Parts[i];
						if (part.Record.Alt_MCD != 0)
						{
							if (part.Altr != null
								&& (spriteId = part.Altr.Record.Sprite1) < Spriteset.Count
								&& (sprite = Spriteset[spriteId].Sprite) != null)
							{
								DrawSprite(
										sprite,
										i * XCImage.SpriteWidth32 + i + offset,
										y3_sprite,
										part.Altr.Record.TileOffset);
							}
							else
								_graphics.FillRectangle(
													Colors.BrushInvalid,
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
		/// <param name="topcrop"></param>
		private void DrawSprite(
				Image sprite,
				int x,
				int y,
				int topcrop)
		{
			_graphics.DrawImage(
							sprite,
							new Rectangle(
										x, y,
										XCImage.SpriteWidth32,
										XCImage.SpriteHeight40),
							0, topcrop, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
							GraphicsUnit.Pixel,
							_f.Ia);
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

			Scroller.Maximum = range;
			Scroller.Enabled = range != 0;

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
		/// Selects tileparts.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>If user has the openfile dialog open and double-clicks to
		/// open a file that happens to be over the panel a mouse-up event
		/// fires. So use MouseDown here.</remarks>
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
		/// This is required in order to handle arrow-keyboard-input via
		/// McdviewF.OnKeyDown().
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool IsInputKey(Keys keyData)
		{
			//DSShared.LogFile.WriteLine("TerrainPanel.IsInputKey() keyData= " + keyData);

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
