using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal delegate void SpritesetChangedEventHandler(bool valid);


	internal sealed class PckViewPanel
		:
			Panel
	{
		#region Events
		internal event SpritesetChangedEventHandler SpritesetChangedEvent;
		#endregion


		#region Fields (static)
		private const int SpriteMargin = 2; // the pad between the tile's inner border and its contained sprite's edges

		private static int TileWidth  = 32; // default to 32px (terrain/unit/bigobs width - alt is ScanG 4px)
		private static int TileHeight = 40; // default to 40px (terrain/unit height - alt is Bigobs 48px / ScanG 4px)

		private const int TableOffsetHori = 3; // the pad between the panel's inner border and the table's vertical borders
		private const int TableOffsetVert = 2; // the pad between the panel's inner border and the table's horizontal borders
		#endregion


		#region Fields
		private readonly PckViewForm _f;

		private readonly VScrollBar _scrollBar = new VScrollBar();

		private int HoriCount = 1;
		private int TableHeight;

		private Pen   _penBlack        = new Pen(Brushes.Black, 1);
		private Pen   _penControlLight = new Pen(SystemColors.ControlLight, 1);
		private Brush _brushCrimson    = new SolidBrush(Color.Crimson);

		/// <summary>
		/// The LargeChange value for the scrollbar will return "1" when the bar
		/// isn't visible. Therefore this value needs to be used instead of the
		/// actual LargeValue in order to calculate the panel's various dynamics.
		/// </summary>
		private int _largeChange;
		#endregion


		#region Properties (static)
		internal static PckViewPanel that
		{ get; private set; }
		#endregion


		#region Properties
		private SpriteCollection _spriteset;
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			set
			{
				_spriteset = value;
				_spriteset.Pal = PckViewForm.Pal;

				if (_f.IsScanG)
				{
					TileWidth  = XCImage.SpriteWidth  * 4;
					TileHeight = XCImage.SpriteHeight * 4;
				}
				else
				{
					TileWidth  = XCImage.SpriteWidth;
					TileHeight = XCImage.SpriteHeight;
				}
				TileWidth  += SpriteMargin * 2 + 1;
				TileHeight += SpriteMargin * 2 + 1;

				_largeChange           =
				_scrollBar.LargeChange = TileHeight;

				CalculateScrollRange(true);

				EditorPanel.that.Sprite = null;

				_f.PrintSpritesetLabel();

				SelectedId =
				OverId     = -1;
				_f.PrintTotal();

				if (SpritesetChangedEvent != null)
					SpritesetChangedEvent(_spriteset != null);

				// TODO: update PaletteViewer if the spriteset's palette changes.
				Refresh();
			}
		}

		internal int SelectedId
		{ get; set; }

		internal int OverId
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PckViewPanel(PckViewForm f)
		{
#if DEBUG
			LogFile.SetLogFilePath(System.IO.Path.GetDirectoryName(Application.ExecutablePath)); // creates a logfile/ wipes the old one.
#endif
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;

			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			Controls.Add(_scrollBar);


			SelectedId =
			OverId     = -1;

			PckViewForm.PaletteChangedEvent += OnPaletteChanged; // NOTE: lives the life of the app, so no leak.

			that = this;
		}
		#endregion


		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		#region Eventcalls (override)
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (FindForm().WindowState != FormWindowState.Minimized)
			{
				CalculateScrollRange(false);
				ScrollToTile(SelectedId);

				if (_scrollBar.Visible
					&& _scrollBar.Value + (_scrollBar.LargeChange - 1) + _scrollBar.LargeChange > _scrollBar.Maximum)
				{
					_scrollBar.Value = _scrollBar.Maximum - (_scrollBar.LargeChange - 1);
				}
			}
		}

		/// <summary>
		/// Scrolls the Overlay-panel with the mousewheel after OnSpriteClick
		/// has given it focus (see).
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
//			base.OnMouseWheel(e);

			if (_scrollBar.Visible)
			{
				if (e.Delta > 0)
				{
					if (_scrollBar.Value - _scrollBar.LargeChange < 0)
						_scrollBar.Value = 0;
					else
						_scrollBar.Value -= _scrollBar.LargeChange;
				}
				else if (e.Delta < 0)
				{
					if (_scrollBar.Value + (_scrollBar.LargeChange - 1) + _scrollBar.LargeChange > _scrollBar.Maximum)
						_scrollBar.Value = _scrollBar.Maximum - (_scrollBar.LargeChange - 1);
					else
						_scrollBar.Value += _scrollBar.LargeChange;
				}
			}
		}

		/// <summary>
		/// Selects and shows status-information for a sprite. Overrides core
		/// implementation for the MouseDown event.
		/// NOTE: This fires before PckViewForm.OnSpriteClick().
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
//			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left
				&& Spriteset != null && Spriteset.Count != 0)
			{
				// IMPORTANT: 'SelectedId' is currently allowed only 1 entry.

				int id = GetTileId(e);
				if (id != SelectedId)
				{
					XCImage sprite = null;

					if ((SelectedId = id) != -1)
					{
//						SelectedId = Spriteset[SelectedId].TerId; // use the proper Id of the sprite itself. don't bother
						sprite = Spriteset[SelectedId];

//						if (ModifierKeys == Keys.Control)
//						{
//							SpriteSelected spritePre = null;
//							foreach (var sprite in _selectedSprites)
//								if (sprite.X == tileX && sprite.Y == tileY)
//									spritePre = sprite;
//							if (spritePre != null)
//								_selectedSprites.Remove(spritePre);
//							else
//								_selectedSprites.Add(selected);
//						}
//						else
//							Selected.Add(selected);
					}

					EditorPanel.that.Sprite = sprite;

					_f.PrintSelectedId();
					Refresh();
				}
				ScrollToTile(SelectedId);
			}
		}

		/// <summary>
		/// Shows status-information for a sprite. Overrides core implementation
		/// for the MouseMove event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
//			base.OnMouseMove(e);

			if (Spriteset != null && Spriteset.Count != 0)
			{
				int id = GetTileId(e);
				if (id != OverId)
				{
					OverId = id;
					_f.PrintOverId();
				}
			}
		}

		/// <summary>
		/// Clears the overId in the statusbar when the mouse-cursor leaves the
		/// panel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
//			base.OnMouseLeave(e);

			OverId = -1;
			_f.PrintOverId();
		}

		/// <summary>
		/// Let's draw this puppy.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (Spriteset != null && Spriteset.Count != 0)
			{
				var graphics = e.Graphics;

				graphics.PixelOffsetMode    = PixelOffsetMode.Half;
				graphics.InterpolationMode  = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode      = SmoothingMode.None;
//				graphics.CompositingQuality = CompositingQuality.HighQuality;


				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					graphics.DrawLine(
									_penControlLight,
									Width - _scrollBar.Width - 1, 0,
									Width - _scrollBar.Width - 1, Height);


//				var selectedIds = new List<int>(); // track currently selected spriteIds.
//				foreach (var sprite in Selected)
//					selectedIds.Add(sprite.Sprite.TerrainId);

				for (int id = 0; id != Spriteset.Count; ++id) // fill selected tile(s) and draw sprites.
				{
					int tileX = id % HoriCount;
					int tileY = id / HoriCount;

//					if (selectedIds.Contains(id))
					if (id == SelectedId)
						graphics.FillRectangle(
											_brushCrimson,
											TableOffsetHori + TileWidth  * tileX,
											TableOffsetVert + TileHeight * tileY - _scrollBar.Value,
											TableOffsetHori + TileWidth  - SpriteMargin * 2,
											TableOffsetVert + TileHeight - SpriteMargin - 1);

					if (!_f.IsScanG)
					{
						graphics.DrawImage(
										Spriteset[id].Sprite,
										TableOffsetHori + tileX * TileWidth  + SpriteMargin,
										TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value);
					}
					else
					{
						graphics.DrawImage(
										Spriteset[id].Sprite,
										TableOffsetHori + tileX * TileWidth  + SpriteMargin,
										TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
										Spriteset[id].Sprite.Width  * 4,
										Spriteset[id].Sprite.Height * 4);
					}
				}


				graphics.FillRectangle(
									new SolidBrush(_penBlack.Color),
									TableOffsetHori - 1,
									TableOffsetVert - 1 - _scrollBar.Value,
									1, 1); // so bite me.

				for (int tileX = 0; tileX <= HoriCount; ++tileX) // draw vertical lines
					graphics.DrawLine(
									_penBlack,
									new Point(
											TableOffsetHori + TileWidth * tileX,
											TableOffsetVert - _scrollBar.Value),
									new Point(
											TableOffsetHori + TileWidth * tileX,
											TableOffsetVert - _scrollBar.Value + TableHeight));

				int tilesY = Spriteset.Count / HoriCount;
				if (Spriteset.Count % HoriCount != 0)
					++tilesY;

				for (int tileY = 0; tileY <= tilesY; ++tileY) // draw horizontal lines
					graphics.DrawLine(
									_penBlack,
									new Point(
											TableOffsetHori,
											TableOffsetVert + TileHeight * tileY - _scrollBar.Value),
									new Point(
											TableOffsetHori + TileWidth  * HoriCount,
											TableOffsetVert + TileHeight * tileY - _scrollBar.Value));
			}
		}
		#endregion


		#region Eventcalls
		/// <summary>
		/// Handler for PaletteChangedEvent.
		/// </summary>
		private void OnPaletteChanged()
		{
			Refresh();
		}

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			Refresh();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Calculates the scrollbar-range after a resize event or a spriteset-
		/// changed event.
		/// </summary>
		/// <param name="resetTrack">true to set the thing to the top of the track</param>
		private void CalculateScrollRange(bool resetTrack)
		{
			HoriCount   = 1;
			TableHeight = 0;

			int range = 0;
			if (Spriteset != null && Spriteset.Count != 0)
			{
				if (resetTrack)
					_scrollBar.Value = 0;

				HoriCount = (Width - TableOffsetHori - _scrollBar.Width - 1) / TileWidth;

				if (HoriCount > Spriteset.Count)
					HoriCount = Spriteset.Count;

				TableHeight = (((Spriteset.Count + (HoriCount - 1)) / HoriCount) + 1) * TileHeight;

				range = TableOffsetVert
					  + TableHeight
					  + TileHeight
					  - Height;

				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = (range != 0);
		}

		/// <summary>
		/// Checks if a selected tile is fully visible in the view-panel and
		/// scrolls the table to show it if not.
		/// </summary>
		/// <param name="id"></param>
		private void ScrollToTile(int id)
		{
			if (id != -1 && _scrollBar.Visible)
			{
				int r = id / HoriCount;

				int cutoff = r * TileHeight + TableOffsetVert - 1;
				if (cutoff < _scrollBar.Value)	// <- check cutoff high
				{
					_scrollBar.Value = cutoff;
				}
				else							// <- check cutoff low
				{
					cutoff = (r + 1) * TileHeight - Height + 1;
					if (cutoff > _scrollBar.Value)
					{
						_scrollBar.Value = cutoff;
					}
				}
			}
		}

		/// <summary>
		/// Gets the id of a sprite at coordinates x/y.
		/// </summary>
		/// <param name="e"></param>
		/// <returns>the terrain-id or -1 if out of bounds</returns>
		private int GetTileId(MouseEventArgs e)
		{
			if (e.X < HoriCount * TileWidth + TableOffsetHori - 1) // not out of bounds to right
			{
				int tileX = (e.X - TableOffsetHori + 1)                    / TileWidth;
				int tileY = (e.Y - TableOffsetHori + 1 + _scrollBar.Value) / TileHeight;

				int id = tileY * HoriCount + tileX;
				if (id < Spriteset.Count) // not out of bounds below
					return id;
			}
			return -1;
		}
		#endregion
	}
}

//		/// <summary>
//		/// Deletes the currently selected sprite and selects another one.
//		/// </summary>
//		internal void SpriteDelete()
//		{
//			if (Selected.Count != 0)
//			{
//				var lowestId = Int32.MaxValue;
//
//				var selectedIds = new List<int>();
//				foreach (var sprite in Selected)
//					selectedIds.Add(sprite.Id);
//
//				selectedIds.Sort();
//				selectedIds.Reverse();
//
//				foreach (var id in selectedIds)
//				{
//					if (id < lowestId)
//						lowestId = id;
//
//					Spriteset.Remove(id);
//				}
//
//				if (lowestId > 0 && lowestId == Spriteset.Count)
//					lowestId = Spriteset.Count - 1;
//
//				Selected.Clear();
//	
//				if (Spriteset.Count != 0)
//				{
//					var selected = new SelectedSprite();
//					selected.Y   = lowestId / HoriCount;
//					selected.X   = lowestId - selected.Y;
//					selected.Id  = selected.X + selected.Y * HoriCount;
//	
//					Selected.Add(selected);
//				}
//			}
//		}

//		internal void Hq2x()
//		{
//			_collection.HQ2X();
//		}
//		internal void Hq2x()
//		{
//			_viewPanel.Hq2x();
//		}
