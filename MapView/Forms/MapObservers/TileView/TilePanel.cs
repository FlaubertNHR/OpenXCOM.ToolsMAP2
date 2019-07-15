using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace MapView.Forms.MapObservers.TileViews
{
	internal delegate void TilepartSelectedEvent(Tilepart part);


	/// <summary>
	/// A separate panel is created for each tab-page in the Tileparts viewer.
	/// </summary>
	internal sealed class TilePanel
		:
			Panel
	{
		internal event TilepartSelectedEvent TilepartSelected;


		#region Fields (static)
		internal static TileView Chaparone;

		private const int SpriteMargin = 2;
		private const int SpriteWidth  = XCImage.SpriteWidth32  + SpriteMargin * 2;
		private const int SpriteHeight = XCImage.SpriteHeight40 + SpriteMargin * 2;

		private const int _largeChange = SpriteHeight;	// apparently .NET won't return an accurate value
														// for LargeChange unless the scrollbar is visible.

		internal static readonly Dictionary<string, SolidBrush> SpecialBrushes =
							 new Dictionary<string, SolidBrush>();

		private static Timer _t1 = new Timer();
		#endregion Fields (static)


		#region Fields
		private Tilepart[] _parts;

		private readonly VScrollBar _scrollBar;

		private readonly Pen _penRed = new Pen(Color.Red, 3);

		private int _tilesX = 1;
		private int _startY;
		private int _id;

		private PartType _quadType;
		#endregion Fields


		#region Properties
		private int TableHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				if (_parts != null && _parts.Length != 0)
				{
					_tilesX = (Width - TableOffset - _scrollBar.Width - 1) / SpriteWidth;	// reserve width for the scrollbar.
					if (_tilesX != 0)														// <- happens when minimizing the TileView form.
					{																		// NOTE: that could be intercepted and disallowed w/
						if (_tilesX > _parts.Length)										// 'if (WindowState != FormWindowState.Minimized)'
							_tilesX = _parts.Length;										// in the OnResize().

						int extra = 0;
						if (_parts.Length % _tilesX != 0)
							extra = 1;

						return (_parts.Length / _tilesX + extra) * SpriteHeight + TableOffset;
					}
				}
				_tilesX = 1;
				return 0;
			}
		}

		/// <summary>
		/// Gets the selected-tilepart.
		/// Sets the selected-tilepart when a valid QuadrantPanel quad is
		/// double-clicked.
		/// @note The setter is invoked only by TileView.SelectedTilepart.
		/// </summary>
		internal Tilepart PartSelected
		{
			get
			{
				if (_id > -1 && _id < _parts.Length)
					return _parts[_id];

				return null;
			}
			set
			{
				if (value != null)
				{
					_id = value.SetId + 1; // +1 to account for the nullpart.

					if (TilepartSelected != null)
						TilepartSelected(PartSelected);

					ScrollToTile();
				}
				else
					_id = 0;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="quadType"></param>
		internal TilePanel(PartType quadType)
		{
			_quadType = quadType;

			Dock = DockStyle.Fill;
			SetStyle(ControlStyles.Selectable, true);
			TabStop = true;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.LargeChange = _largeChange;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			Controls.Add(_scrollBar);

			ContextMenu = CreateContext();

			MainViewUnderlay.AnimationUpdate += OnAnimationUpdate;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);


			_t1.Interval = 250;	// because the mouse OnLeave event doesn't fire
			_t1.Enabled = true;	// when the mouse moves out of the panel directly
		}						// from a tilepart's part-icon.

		/// <summary>
		/// Builds a ContextMenu for RMB on this TilePanel.
		/// </summary>
		/// <returns>the ContextMenu</returns>
		private ContextMenu CreateContext()
		{
			var context = new ContextMenu();

			context.MenuItems.Add(new MenuItem("open in PckView", OnClick_OpenPckview, Shortcut.F9));	// 0
			context.MenuItems.Add(new MenuItem("open in McdView", OnClick_OpenMcdview, Shortcut.F10));	// 1
			context.MenuItems.Add(new MenuItem("-"));													// 2
			context.MenuItems.Add(new MenuItem("Tilepart Info"  , OnClick_OpenMcdinfo));				// 3

			return context;
		}
		#endregion cTor


		#region Events
		private void OnClick_OpenPckview(object sender, EventArgs e)
		{
			Chaparone.OnPckEditClick(sender, e);
		}

		private void OnClick_OpenMcdview(object sender, EventArgs e)
		{
			Chaparone.OnMcdEditClick(sender, e);
		}

		private void OnClick_OpenMcdinfo(object sender, EventArgs e)
		{
			Chaparone.OnMcdInfoClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// Ensures that any OverInfo on the statusbar is cleared when the
		/// mouse-cursor leaves this panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_Tick(object sender, EventArgs e)
		{
			if (!Bounds.Contains(PointToClient(Cursor.Position)))
			{
				Chaparone.StatbarOverInfo(null);
			}
		}

		/// <summary>
		/// Subscribes or unsubscribes this panel's ticker-handler to the static
		/// timer-object; prevents all 5 panels' handlers from firing needlessly.
		/// </summary>
		/// <param name="subscribe"></param>
		internal void SetTickerSubscription(bool subscribe)
		{
			if (subscribe) _t1.Tick += t1_Tick;
			else           _t1.Tick -= t1_Tick;
		}

		/// <summary>
		/// whee. Handles animations.
		/// </summary>
		private void OnAnimationUpdate()
		{
			Invalidate();
		}

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			if (_parts != null && _parts.Length != 0)
			{
				_startY = -_scrollBar.Value;
				Refresh();
			}
		}
		#endregion Events


		#region Events (override)
		private bool _resetTrack;

		/// <summary>
		/// Handles client resizing. Sets the scrollbar's Maximum value. And
		/// ensures that the bottom of the tile-table gets snuggled down against
		/// the bottom of the panel's area if required.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			int range = 0;
			if (_parts != null && _parts.Length != 0)
			{
				if (_resetTrack)
				{
					_resetTrack = false;
					_scrollBar.Value = 0;
				}

				range = TableHeight + _largeChange - Height;
				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = range != 0;

			if (_scrollBar.Visible
				&& TableHeight - _scrollBar.Value < Height)
			{
				_scrollBar.Value = _scrollBar.Maximum - _largeChange - 1 + TableOffset;
			}
		}

		/// <summary>
		/// Scrolls the table by the mousewheel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var args = e as HandledMouseEventArgs;
			if (args != null)
				args.Handled = true;

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
					if (_scrollBar.Value + _scrollBar.LargeChange + (_scrollBar.LargeChange - 1) > _scrollBar.Maximum)
						_scrollBar.Value = _scrollBar.Maximum - (_scrollBar.LargeChange - 1);
					else
						_scrollBar.Value += _scrollBar.LargeChange;
				}
			}

			if (Bounds.Contains(PointToClient(MousePosition)))
				OnMouseMove(e); // update OverInfo on the statusbar.
		}

		/// <summary>
		/// Focuses this panel and selects a tilepart.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();

			int id = GetOverId(e);
			if (id != -1 && id < _parts.Length)
			{
				_id = id;

				if (TilepartSelected != null)
					TilepartSelected(PartSelected);

				ScrollToTile();
				Invalidate();
			}
		}

		/// <summary>
		/// Navigates the tileparts of this panel on keydown events at the Form
		/// level.
		/// </summary>
		/// <param name="keyData"></param>
		internal void Navigate(Keys keyData)
		{
			int id = -1;

			switch (keyData)
			{
				case Keys.Left:
					id = _id - 1;
					break;

				case Keys.Right:
					id = _id + 1;
					break;

				case Keys.Up:
					id = _id - _tilesX;
					break;

				case Keys.Down:
					id = _id + _tilesX;
					break;

				case Keys.Home:
					id = _id / _tilesX * _tilesX;
					break;

				case Keys.End:
					id = _id / _tilesX * _tilesX + _tilesX - 1;
					if (id >= _parts.Length)
						id =  _parts.Length - 1;
					break;

				case (Keys.Home | Keys.Control):
					id = 0;
					break;

				case (Keys.End | Keys.Control):
					id = _parts.Length - 1;
					break;

				case Keys.PageUp:
					if (_id >= _tilesX)
					{
						int vert = Height / SpriteHeight * _tilesX;
						if (vert < _tilesX)
							vert = _tilesX;

						int tileX = _id % _tilesX;
						id = _id / _tilesX * _tilesX + tileX - vert;

						if (id < tileX)
							id = tileX;
					}
					break;

				case Keys.PageDown:
					if (_id < _parts.Length / _tilesX * _tilesX)
					{
						int vert = Height / SpriteHeight * _tilesX;
						if (vert < _tilesX)
							vert = _tilesX;

						int tileX = _id % _tilesX;
						id = _id / _tilesX * _tilesX + tileX + vert;
						if (id >= _parts.Length)
						{
							id = _parts.Length / _tilesX * _tilesX + tileX;
							if (id >= _parts.Length)
								id = (_parts.Length / _tilesX - 1) * _tilesX + tileX;
						}
					}
					break;
			}

			if (id > -1 && id < _parts.Length)
			{
				_id = id;

				if (TilepartSelected != null)
					TilepartSelected(PartSelected);

				ScrollToTile();
				Invalidate();
			}
		}

		/// <summary>
		/// Opens the MCD-info screen when a valid tilepart is
		/// double-left-clicked.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			int id = GetOverId(e);
			if (id != -1 && id < _parts.Length)
			{
				switch (e.Button)
				{
					case MouseButtons.Left:
						Chaparone.OnMcdInfoClick(null, EventArgs.Empty);
						break;
				}
			}
		}

		/// <summary>
		/// Opens the MCD-info screen when [i] is key-upped.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.I)
			{
				Chaparone.OnMcdInfoClick(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Prints info about a mouseovered tilepart to the statusbar.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			Tilepart part = null;

			int id = GetOverId(e);
			if (id != -1 && id < _parts.Length)
				part = _parts[id];

			Chaparone.StatbarOverInfo(part);
		}


		/// <summary>
		/// Gets the ID of a tilepart at a mouse-position.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		private int GetOverId(MouseEventArgs e)
		{
			if (_parts != null && _parts.Length != 0
				&& e.X < SpriteWidth * _tilesX + TableOffset - 1) // not out of bounds to right
			{
				int tileX = (e.X - TableOffset + 1)           / SpriteWidth;
				int tileY = (e.Y - TableOffset + 1 - _startY) / SpriteHeight;

				return tileX + tileY * _tilesX;
			}
			return -1;
		}


		private const string Door = "door";
		private static int TextWidth;

		private const int TableOffset = 2;

		/// <summary>
		/// this.Fill(black)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_parts != null && _parts.Length != 0)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				var spriteAttributes = new ImageAttributes();
				if (MainViewOverlay.that._spriteShadeEnabled)
					spriteAttributes.SetGamma(MainViewOverlay.that.SpriteShadeLocal, ColorAdjustType.Bitmap);

				int x = 0;
				int y = 0;
				int top;
				int left;

				if (TextWidth == 0) // init.
				{
//					TextWidth = TextRenderer.MeasureText(Door, Font).Width;		// =30
					TextWidth = (int)graphics.MeasureString(Door, Font).Width;	// =24
				}

				XCImage sprite;

				foreach (var part in _parts)
				{
					left = SpriteWidth  * x + TableOffset;
					top  = SpriteHeight * y + TableOffset + _startY;

					var rect = new Rectangle(
										left, top,
										SpriteWidth, SpriteHeight);

					if (part != null) // draw tile-sprite ->
					{
						string special = part.Record.Special.ToString();		// first fill w/ SpecialProperty color
						if (SpecialBrushes.ContainsKey(special))
							graphics.FillRectangle(SpecialBrushes[special], rect);

						if ((sprite = part[MainViewUnderlay.AniStep]) != null)
						{
//							graphics.DrawImage(									// then draw the sprite itself
//											sprite.Sprite,
//											left + SpriteMargin,
//											top  + SpriteMargin - part.Record.TileOffset);
							graphics.DrawImage(
											sprite.Sprite,
											new Rectangle(
														left + SpriteMargin,
														top  + SpriteMargin - part.Record.TileOffset,
														sprite.Sprite.Width,
														sprite.Sprite.Height),
											0,0, sprite.Sprite.Width, sprite.Sprite.Height,
											GraphicsUnit.Pixel,
											spriteAttributes);
						}

						// NOTE: keep the door-string and its placement consistent with
						// QuadrantDrawService.Draw().
						if (part.Record.HingedDoor || part.Record.SlidingDoor)	// finally print "door" if it's a door
							graphics.DrawString(
											Door,
											Font,
											Brushes.Black,
											left + (SpriteWidth  - TextWidth) / 2,
											top  +  SpriteHeight - Font.Height);
					}
					else // draw the eraser ->
					{
						graphics.FillRectangle(Brushes.AliceBlue, rect);

						if (Globals.ExtraSprites != null)
							graphics.DrawImage(
											Globals.ExtraSprites[0].Sprite,
											left, top);
					}

					x = (x + 1) % _tilesX;
					if (x == 0)
						y++;
				}

				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					graphics.DrawLine(
									SystemPens.ControlLight,
									Width - _scrollBar.Width - 1, 0,
									Width - _scrollBar.Width - 1, Height);

				graphics.FillRectangle(
									Brushes.Black,
									TableOffset - 1,
									TableOffset + _startY - 1,
									1,1); // so bite me.

				int height = TableHeight;

				for (int i = 0; i <= _tilesX; ++i)								// draw vertical lines
					graphics.DrawLine(
									Pens.Black,
									TableOffset + SpriteWidth * i, TableOffset + _startY,
									TableOffset + SpriteWidth * i, /*TableOffset +*/ _startY + height);

				for (int i = 0; i <= height; i += SpriteHeight)					// draw horizontal lines
					graphics.DrawLine(
									Pens.Black,
									TableOffset,                         TableOffset + _startY + i,
									TableOffset + SpriteWidth * _tilesX, TableOffset + _startY + i);

				graphics.DrawRectangle(											// draw selected rectangle
									_penRed,
									TableOffset + _id % _tilesX * SpriteWidth,
									TableOffset + _id / _tilesX * SpriteHeight + _startY,
									SpriteWidth, SpriteHeight);
			}
		}
		#endregion Events (override)


		#region Methods
		internal void SetTiles(IList<Tilepart> parts)
		{
			if (parts != null) //&& _tiles.Length != 0)	// NOTE: This check for Length should be enough
			{											// to cover all other checks for Length==0.
				if (_quadType == PartType.All)			// Except that the eraser needs to be added anyway ....
				{
					_parts = new Tilepart[parts.Count + 1];
					_parts[0] = null;

					for (int i = 0; i != parts.Count; ++i)
						_parts[i + 1] = parts[i];
				}
				else
				{
					int qtyTiles = 0;

					for (int i = 0; i != parts.Count; ++i)
						if (parts[i].Record.PartType == _quadType)
							++qtyTiles;

					_parts = new Tilepart[qtyTiles + 1];
					_parts[0] = null;

					for (int i = 0, j = 1; i != parts.Count; ++i)
						if (parts[i].Record.PartType == _quadType)
							_parts[j++] = parts[i];
				}

				if (_id >= _parts.Length)
					_id = 0;
			}
			else
			{
				_parts = null;
				_id = 0;
			}

			_resetTrack = true;
			OnResize(null);
		}

		/// <summary>
		/// Checks if a selected tilepart is fully visible in the view-panel and
		/// scrolls the table to show it if not.
		/// </summary>
		private void ScrollToTile()
		{
			int tileY = _id / _tilesX;

			int cutoff = SpriteHeight * tileY;
			if (cutoff < -_startY)		// <- check cutoff high
			{
				_scrollBar.Value = cutoff;
			}
			else						// <- check cutoff low
			{
				cutoff = SpriteHeight * (tileY + 1) - Height + TableOffset + 1;
				if (cutoff > -_startY)
				{
					_scrollBar.Value = cutoff;
				}
			}
		}
		#endregion Methods
	}
}
