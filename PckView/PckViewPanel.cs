using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;

using XCom;


namespace PckView
{
	internal sealed class PckViewPanel
		:
			BufferedPanel
	{
		#region Fields (static)
		private const int SpriteMargin = 2; // the pad between the tile's inner border and its contained sprite's edges

		private static int TileWidth  = 32; // default to 32px (terrain/unit/bigobs width - alt is ScanG 4px)
		private static int TileHeight = 40; // default to 40px (terrain/unit height - alt is Bigobs 48px / ScanG 4px)

		private const int TableOffsetHori = 3; // the pad between the panel's inner border and the table's vertical borders
		private const int TableOffsetVert = 2; // the pad between the panel's inner border and the table's horizontal borders
		#endregion Fields (static)


		#region Fields
		private readonly PckViewF _f;

		private readonly VScrollBar _scrollBar = new VScrollBar();

		private int HoriCount = 1;
		private int TableHeight;

		/// <summary>
		/// The LargeChange value for the scrollbar will return "1" when the bar
		/// isn't visible. Therefore this value needs to be used instead of the
		/// actual LargeValue in order to calculate the panel's various dynamics.
		/// </summary>
		private int _largeChange;

		private Graphics _graphics;
		#endregion Fields


		#region Properties
		private SpriteCollection _spriteset;
		/// <summary>
		/// The currently loaded spriteset or null. Initializes the UI according
		/// to parameters of the spriteset. Also ensures that the spriteset and
		/// its sprites have their palettes set to PckView's current palette.
		/// </summary>
		/// <remarks>LoFTsets don't need their palette set; their palette is set
		/// on creation and don't change.</remarks>
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			set
			{
				if ((_spriteset = value) != null
					&& _f.SetType != PckViewF.Type.LoFT)
				{
					_spriteset.Pal = _f.Pal;
				}

				TileWidth  = XCImage.SpriteWidth;
				TileHeight = XCImage.SpriteHeight;

				if (_f.SetType == PckViewF.Type.ScanG)
				{
					TileWidth  *= 4;
					TileHeight *= 4;
				}
				TileWidth  += SpriteMargin * 2 + 1;
				TileHeight += SpriteMargin * 2 + 1;

				_largeChange           =
				_scrollBar.LargeChange = TileHeight;

				CalculateScrollRange(true);

				Selid = Ovid = -1;

				_f.ResetUi(_spriteset != null);

				// TODO: update PaletteViewer if the spriteset's palette changes.
				Invalidate();
			}
		}

		/// <summary>
		/// The selected id.
		/// </summary>
		internal int Selid
		{ get; set; }

		/// <summary>
		/// The overid.
		/// </summary>
		internal int Ovid
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PckViewPanel(PckViewF f)
		{
			_f = f;

			Dock = DockStyle.Fill;

			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;
			Controls.Add(_scrollBar);

			Ovid = Selid  = -1;

			PckViewF.PaletteChanged += OnPaletteChanged;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (_f.WindowState != FormWindowState.Minimized)
			{
				CalculateScrollRange(false);
				ScrollToTile(Selid);

				if (_scrollBar.Visible
					&& _scrollBar.Value + (_scrollBar.LargeChange - 1) + _scrollBar.LargeChange > _scrollBar.Maximum)
				{
					_scrollBar.Value = _scrollBar.Maximum - (_scrollBar.LargeChange - 1);
				}
			}
		}

		/// <summary>
		/// Forces a call to OnResize().
		/// </summary>
		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		/// <summary>
		/// Scrolls this panel with the mousewheel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
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

				// update the OverId and print info to the statusbar
				var pt = PointToClient(Control.MousePosition);
				OnMouseMove(new MouseEventArgs(
											MouseButtons.None,
											0, pt.X,pt.Y, 0));
			}
		}

		/// <summary>
		/// Selects and shows status-information for a sprite. Overrides core
		/// implementation for the MouseDown event.
		/// NOTE: This fires before PckViewF.OnSpriteClick().
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left
				&& Spriteset != null && Spriteset.Count != 0)
			{
				// IMPORTANT: 'idSel' is currently allowed only 1 entry.

				int id = GetTileId(e);
				if (id != Selid)
				{
					XCImage sprite;

					if ((Selid = id) != -1)
					{
						sprite = Spriteset[Selid];

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
					else
						sprite = null;

					_f.SpriteEditor.SpritePanel.Sprite = sprite;

					_f.PrintSelectedId();
					Invalidate();
				}
				ScrollToTile(Selid);
			}
		}

		/// <summary>
		/// Shows status-information for a sprite. Overrides core implementation
		/// for the MouseMove event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Spriteset != null && Spriteset.Count != 0)
			{
				int id = GetTileId(e);
				if (id != Ovid)
				{
					Ovid = id;
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
			Ovid = -1;
			_f.PrintOverId();
		}

		/// <summary>
		/// Let's draw this puppy.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Spriteset != null && Spriteset.Count != 0)
			{
				_graphics = e.Graphics;

				_graphics.PixelOffsetMode    = PixelOffsetMode.Half;
				_graphics.InterpolationMode  = InterpolationMode.NearestNeighbor;
				_graphics.SmoothingMode      = SmoothingMode.None;
//				_graphics.CompositingQuality = CompositingQuality.HighQuality;


				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					_graphics.DrawLine(
									SystemPens.ControlLight,
									Width - _scrollBar.Width - 1, 0,
									Width - _scrollBar.Width - 1, Height);

//				var selectedIds = new List<int>(); // track currently selected spriteIds.
//				foreach (var sprite in Selected)
//					selectedIds.Add(sprite.Sprite.TerrainId);

				switch (_f.SetType) // draw sprites
				{
					case PckViewF.Type.Pck:    DrawPck();    break;
					case PckViewF.Type.Bigobs: DrawBigobs(); break;
					case PckViewF.Type.ScanG:  DrawScanG();  break;
					case PckViewF.Type.LoFT:   DrawLoFT();   break;
				}

				_graphics.FillRectangle(
									Brushes.Black,
									TableOffsetHori - 1,
									TableOffsetVert - 1 - _scrollBar.Value,
									1,1); // so bite me.

				for (int tileX = 0; tileX <= HoriCount; ++tileX) // draw vertical lines
					_graphics.DrawLine(
									Pens.Black,
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
					_graphics.DrawLine(
									Pens.Black,
									new Point(
											TableOffsetHori,
											TableOffsetVert + TileHeight * tileY - _scrollBar.Value),
									new Point(
											TableOffsetHori + TileWidth  * HoriCount,
											TableOffsetVert + TileHeight * tileY - _scrollBar.Value));
			}
		}

		/// <summary>
		/// Draws a Pck spriteset.
		/// </summary>
		private void DrawPck()
		{
			for (int id = 0; id != Spriteset.Count; ++id) // fill selected tile(s) and draw sprites.
			{
				int tileX = id % HoriCount;
				int tileY = id / HoriCount;

				if (id == Selid)
					_graphics.FillRectangle(
										Brushes.Crimson,
										TableOffsetHori + TileWidth  * tileX,
										TableOffsetVert + TileHeight * tileY - _scrollBar.Value,
										TableOffsetHori + TileWidth  - SpriteMargin * 2,
										TableOffsetVert + TileHeight - SpriteMargin - 1);


				if (Spriteset[id].Istid())
				{
					_graphics.DrawImage(
									_f.BlankSprite,
									TableOffsetHori + tileX * TileWidth  + SpriteMargin,
									TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value);
				}
				else if (_f.SpriteShade >= PckViewF.SPRITESHADE_ON)
				{
					_graphics.DrawImage(
									Spriteset[id].Sprite,
									new Rectangle(
												TableOffsetHori + tileX * TileWidth  + SpriteMargin,
												TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
												XCImage.SpriteWidth, XCImage.SpriteHeight),
									0,0, XCImage.SpriteWidth, XCImage.SpriteHeight,
									GraphicsUnit.Pixel,
									_f.Ia);
				}
				else
					_graphics.DrawImage(
									Spriteset[id].Sprite,
									TableOffsetHori + tileX * TileWidth  + SpriteMargin,
									TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value);
			}
		}

		/// <summary>
		/// Draws a Bigobs spriteset.
		/// </summary>
		private void DrawBigobs()
		{
			for (int id = 0; id != Spriteset.Count; ++id) // fill selected tile(s) and draw sprites.
			{
				int tileX = id % HoriCount;
				int tileY = id / HoriCount;

				if (id == Selid)
					_graphics.FillRectangle(
										Brushes.Crimson,
										TableOffsetHori + TileWidth  * tileX,
										TableOffsetVert + TileHeight * tileY - _scrollBar.Value,
										TableOffsetHori + TileWidth  - SpriteMargin * 2,
										TableOffsetVert + TileHeight - SpriteMargin - 1);


				if (Spriteset[id].Istid())
				{
					_graphics.DrawImage(
									_f.BlankSprite,
									TableOffsetHori + tileX * TileWidth  + SpriteMargin,
									TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
									XCImage.SpriteWidth32, XCImage.SpriteHeight48);
				}
				else if (_f.SpriteShade >= PckViewF.SPRITESHADE_ON)
				{
					_graphics.DrawImage(
									Spriteset[id].Sprite,
									new Rectangle(
												TableOffsetHori + tileX * TileWidth  + SpriteMargin,
												TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
												XCImage.SpriteWidth, XCImage.SpriteHeight),
									0,0, XCImage.SpriteWidth, XCImage.SpriteHeight,
									GraphicsUnit.Pixel,
									_f.Ia);
				}
				else
					_graphics.DrawImage(
									Spriteset[id].Sprite,
									TableOffsetHori + tileX * TileWidth  + SpriteMargin,
									TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value);
			}
		}

		/// <summary>
		/// Draws a ScanG iconset.
		/// </summary>
		private void DrawScanG()
		{
			for (int id = 0; id != Spriteset.Count; ++id) // fill selected tile(s) and draw sprites.
			{
				int tileX = id % HoriCount;
				int tileY = id / HoriCount;

				if (id == Selid)
					_graphics.FillRectangle(
										Brushes.Crimson,
										TableOffsetHori + TileWidth  * tileX,
										TableOffsetVert + TileHeight * tileY - _scrollBar.Value,
										TableOffsetHori + TileWidth  - SpriteMargin * 2,
										TableOffsetVert + TileHeight - SpriteMargin - 1);


				if (Spriteset[id].Istid())
				{
					_graphics.DrawImage(
									_f.BlankIcon,
									TableOffsetHori + tileX * TileWidth  + SpriteMargin,
									TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
									XCImage.SpriteWidth * 4, XCImage.SpriteHeight * 4);
				}
				else if (_f.SpriteShade >= PckViewF.SPRITESHADE_ON)
				{
					_graphics.DrawImage(
									Spriteset[id].Sprite,
									new Rectangle(
												TableOffsetHori + tileX * TileWidth  + SpriteMargin,
												TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
												XCImage.SpriteWidth * 4, XCImage.SpriteHeight * 4),
									0,0, XCImage.ScanGside, XCImage.ScanGside,
									GraphicsUnit.Pixel,
									_f.Ia);
				}
				else
				{
					_graphics.DrawImage(
									Spriteset[id].Sprite,
									TableOffsetHori + tileX * TileWidth  + SpriteMargin,
									TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value,
									XCImage.SpriteWidth * 4, XCImage.SpriteHeight * 4);
				}
			}
		}

		/// <summary>
		/// Draws a LoFT iconset.
		/// </summary>
		private void DrawLoFT()
		{
			for (int id = 0; id != Spriteset.Count; ++id) // fill selected tile(s) and draw sprites.
			{
				int tileX = id % HoriCount;
				int tileY = id / HoriCount;

				if (id == Selid)
					_graphics.FillRectangle(
										Brushes.Crimson,
										TableOffsetHori + TileWidth  * tileX,
										TableOffsetVert + TileHeight * tileY - _scrollBar.Value,
										TableOffsetHori + TileWidth  - SpriteMargin * 2,
										TableOffsetVert + TileHeight - SpriteMargin - 1);

				_graphics.DrawImage(
								Spriteset[id].Sprite,
								TableOffsetHori + tileX * TileWidth  + SpriteMargin,
								TableOffsetVert + tileY * TileHeight + SpriteMargin - _scrollBar.Value);
			}
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handler for PaletteChanged. Invalidates this panel.
		/// </summary>
		private void OnPaletteChanged()
		{
			if (Spriteset != null) Invalidate();
		}

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			Invalidate();
		}
		#endregion Events


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
		internal void ScrollToTile(int id)
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


		/// <summary>
		/// Selects a sprite after left/right keypress.
		/// </summary>
		/// <param name="dir">-1 left, +1 right</param>
		internal void SelectAdjacentHori(int dir)
		{
			_f.SpriteEditor.SpritePanel.Sprite = Spriteset[Selid += dir];
			ScrollToTile(Selid);
			Invalidate();
		}

		/// <summary>
		/// Selects a sprite after up/down keypress.
		/// </summary>
		/// <param name="dir">-1 up, +1 down</param>
		internal void SelectAdjacentVert(int dir)
		{
			switch (dir)
			{
				case -1:
					if (Selid >= HoriCount)
						Selid -= HoriCount;
					break;

				case +1:
					if (Selid == -1 && Spriteset.Count != 0)
					{
						Selid = Spriteset.Count - 1;
					}
					else if (Selid < Spriteset.Count - HoriCount)
					{
						Selid += HoriCount;
					}
					break;
			}

			_f.SpriteEditor.SpritePanel.Sprite = Spriteset[Selid];
			ScrollToTile(Selid);
			Invalidate();
		}

		/// <summary>
		/// fing jackasses.
		/// </summary>
		internal void Destroy()
		{
			PckViewF.PaletteChanged -= OnPaletteChanged;

			if (Spriteset != null)
				Spriteset.Dispose();

//			base.Dispose(); // <- I *still* don't know if that is a Good Thing or not.
		}
		#endregion Methods
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
