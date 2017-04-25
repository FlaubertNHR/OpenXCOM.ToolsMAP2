using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	internal delegate void SelectedTileChangedEventHandler(TileBase tile);


	/// <summary>
	/// A separate panel is created for each tab-page in the Tile viewer.
	/// </summary>
	internal sealed class TilePanel
		:
			Panel
	{
		internal event SelectedTileChangedEventHandler PanelSelectedTileChangedEvent;


		#region Fields & Properties
		private TileBase[] _tiles;

		private readonly VScrollBar _scrollBar;

		private const int SpriteMargin = 2;
		private const int SpriteWidth  = 32 + SpriteMargin * 2;
		private const int SpriteHeight = 40 + SpriteMargin * 2;

		private const int _largeChange = SpriteHeight;	// apparently .NET won't return an accurate value
														// for LargeChange unless the scrollbar is visible.

//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));
		private Pen _penRed = new Pen(Brushes.Red, 3); // TODO: find some happy colors
		private Pen _penControlLight = new Pen(SystemColors.ControlLight, 1);

		private static Hashtable _brushes;

		private int _tilesX = 1;
		private int _startY;
		private int _id;

		private TileType _quadType;

		private int TableHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				if (_tiles != null && _tiles.Length != 0)
				{
					_tilesX = (Width - _scrollBar.Width - 1) / SpriteWidth; // reserve width for the scrollbar.

					if (_tilesX > _tiles.Length)
						_tilesX = _tiles.Length;

					int extra = 0;
					if (_tiles.Length % _tilesX != 0)
						extra = 1;

					return (_tiles.Length / _tilesX + extra) * SpriteHeight;
				}

				_tilesX = 1;
				return 0;
			}
		}

		/// <summary>
		/// Gets the selected-tile-id.
		/// Sets the selected-tile-id when a valid QuadrantPanel quad is
		/// double-clicked.
		/// </summary>
		internal TileBase SelectedTile
		{
			get
			{
				if (_id > -1 && _id < _tiles.Length)
					return _tiles[_id];

				return null;
			}
			set
			{
				if (value != null)
				{
					_id = value.TileListId + 1;

					if (PanelSelectedTileChangedEvent != null)
						PanelSelectedTileChangedEvent(SelectedTile);

					ScrollToTile();
				}
				else
					_id = 0;
			}
		}
		#endregion


		internal static readonly Color[] TileColors =
		{
			Color.Cornsilk,
			Color.Lavender,
			Color.DarkRed,
			Color.Fuchsia,
			Color.Aqua,
			Color.DarkOrange,
			Color.DeepPink,
			Color.LightBlue,
			Color.Lime,
			Color.LightGreen,
			Color.MediumPurple,
			Color.LightCoral,
			Color.LightCyan,
			Color.Yellow,
			Color.Blue
		};

		internal static void SetBrushes(Hashtable table)
		{
			_brushes = table;
		}

//		private static PckSpriteCollection extraFile;
//		public static PckSpriteCollection ExtraFile
//		{
//			get { return extraFile; }
//			set { extraFile = value; }
//		}


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="quadType"></param>
		internal TilePanel(TileType quadType)
		{
			_quadType = quadType;

			Dock = DockStyle.Fill;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.LargeChange = _largeChange;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			Controls.Add(_scrollBar);

			MainViewPanel.AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			Globals.LoadExtras();
		}
		#endregion


		#region Event Calls
		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			if (_tiles != null && _tiles.Length != 0)
			{
				_startY = -_scrollBar.Value;
				Refresh();
			}
		}

		/// <summary>
		/// Handles client resizing. Sets the scrollbar's Maximum value.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			int range = 0;
			if (_tiles != null && _tiles.Length != 0)
			{
				range = TableHeight + _largeChange - Height;
				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = (range != 0);
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
					if (_scrollBar.Value + (_scrollBar.LargeChange - 1) + _scrollBar.LargeChange > _scrollBar.Maximum)
						_scrollBar.Value = _scrollBar.Maximum - (_scrollBar.LargeChange - 1);
					else
						_scrollBar.Value += _scrollBar.LargeChange;
				}
			}
		}

		/// <summary>
		/// Selects a tile.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();

			if (_tiles != null && _tiles.Length != 0)
			{
				int tileX =  e.X            / SpriteWidth;
				int tileY = (e.Y - _startY) / SpriteHeight;

				if (tileX >= _tilesX)
					tileX  = _tilesX - 1;

				int tile = tileX + tileY * _tilesX;
				if (tile < _tiles.Length)
				{
					_id = tile;

					if (PanelSelectedTileChangedEvent != null)
						PanelSelectedTileChangedEvent(SelectedTile);

					ScrollToTile();
					Refresh();
				}
			}
		}

		/// <summary>
		/// this.Fill(black)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_tiles != null && _tiles.Length != 0)
			{
				var g = e.Graphics;

				int x = 0;
				int y = 0;
				int top;
				int left;

				foreach (var tile in _tiles)
				{
					left = x * SpriteWidth;
					top  = y * SpriteHeight + _startY;

					var rect = new Rectangle(
										left, top,
										SpriteWidth, SpriteHeight);

					if (tile != null)
					{
						string targetType = tile.Record.TargetType.ToString();
						if (_brushes.ContainsKey(targetType))
							g.FillRectangle((SolidBrush)_brushes[targetType], rect);

						g.DrawImage(
								tile[MainViewPanel.AniStep].Image,
								left,
								top - tile.Record.TileOffset);

						if (tile.Record.HumanDoor || tile.Record.UfoDoor)
							g.DrawString(
									"Door",
									Font,
									Brushes.Black,
									left,
									top + PckImage.Height - Font.Height);
					}
					else // draw the eraser ->
					{
						g.FillRectangle(Brushes.AliceBlue, rect);

						if (Globals.ExtraTiles != null)
							g.DrawImage(
									Globals.ExtraTiles[0].Image,
									left, top);
					}

					x = (x + 1) % _tilesX;
					if (x == 0)
						y++;
				}

//				g.DrawRectangle(
//							_brush,
//							(_sel % _across) * (_width + _space),
//							_startY + (_sel / _across) * (_height + _space),
//							_width  + _space,
//							_height + _space)

				int height = TableHeight;

				for (int i = 0; i <= _tilesX; ++i)
					g.DrawLine(
							Pens.Black,
							i * SpriteWidth, _startY,
							i * SpriteWidth, _startY + height);

				for (int i = 0; i <= height; i += SpriteHeight)
					g.DrawLine(
							Pens.Black,
							0,                     _startY + i,
							_tilesX * SpriteWidth, _startY + i);

				g.DrawRectangle(
							_penRed,
							_id % _tilesX * SpriteWidth,
							_startY + _id / _tilesX * SpriteHeight,
							SpriteWidth, SpriteHeight);

				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					g.DrawLine(
							_penControlLight,
							Width - _scrollBar.Width, 0,
							Width - _scrollBar.Width, Height);
			}
		}

		/// <summary>
		/// whee. Handles animations.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			Refresh();
		}
		#endregion


		#region Methods
		internal void SetTiles(IList<TileBase> tiles)
		{
			if (tiles != null)// && _tiles.Length != 0)	// NOTE: This check for Length should be enough
			{											// to cover all other checks for Length==0.
				if (_quadType == TileType.All)			// Except that the eraser needs to be added anyway ....
				{
					_tiles = new TileBase[tiles.Count + 1];
					_tiles[0] = null;

					for (int i = 0; i != tiles.Count; ++i)
						_tiles[i + 1] = tiles[i];
				}
				else
				{
					int qtyTiles = 0;

					for (int i = 0; i != tiles.Count; ++i)
						if (tiles[i].Record.TileType == _quadType)
							++qtyTiles;

					_tiles = new TileBase[qtyTiles + 1];
					_tiles[0] = null;

					for (int i = 0, j = 1; i != tiles.Count; ++i)
						if (tiles[i].Record.TileType == _quadType)
							_tiles[j++] = tiles[i];
				}

				if (_id >= _tiles.Length)
					_id = 0;
			}
			else
			{
				_tiles = null;
				_id = 0;
			}

			OnResize(null);
		}

		/// <summary>
		/// Checks if a selected tile is fully visible in the view-panel and
		/// scrolls the table to show it if not.
		/// </summary>
		private void ScrollToTile()
		{
			int tileY = _id / _tilesX;

			int cutoff = tileY * SpriteHeight;
			if (cutoff < -_startY)		// <- check cutoff high
			{
				_scrollBar.Value = cutoff;
			}
			else						// <- check cutoff low
			{
				cutoff = (tileY + 1) * SpriteHeight - Height;
				if (cutoff > -_startY)
				{
					_scrollBar.Value = cutoff;
				}
			}
		}
		#endregion
	}
}
