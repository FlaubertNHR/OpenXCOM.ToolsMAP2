﻿using System;
using System.Collections.Generic;		// List
using System.Collections.ObjectModel;	// ReadOnlyCollection
using System.Drawing;					// Pens, Brushes
using System.Windows.Forms;				// Panel

using XCom;								// Palette, XCImageCollection
using XCom.Interfaces;					// XCImage


namespace PckView
{
	internal delegate void SpritePackChangedEventHandler(SpritePackChangedEventArgs e);


	internal sealed class ViewPanel
		:
			Panel
	{
		internal event SpritePackChangedEventHandler SpritePackChangedEvent;


		#region Fields & Properties

		private VScrollBar     _scrollBar;
		private StatusBar      _statusBar;
		private StatusBarPanel _statusTileSelected;
		private StatusBarPanel _statusTileOver;


		private XCImageCollection _spritePack;
		internal XCImageCollection SpritePack
		{
			get { return _spritePack; }
			set
			{
				_spritePack = value;

				_spriteWidth  = value.ImageFile.ImageSize.Width  + SpriteMargin * 2;
				_spriteHeight = value.ImageFile.ImageSize.Height + SpriteMargin * 2;

//				Height = TableHeight; ... nobody cares about the Height. Let .NET deal with it.

//				OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
//				OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

				_selectedSprites.Clear();

				_largeChange           =
				_scrollBar.LargeChange = value.ImageFile.ImageSize.Height + SpriteMargin * 2;

				UpdateScrollbar(true);

				Refresh();
				Focus();

				OnSpriteClick(-1);
				OnSpriteOver(-1);

				if (SpritePackChangedEvent != null)
					SpritePackChangedEvent(new SpritePackChangedEventArgs(value));
			}
		}

		internal Palette Pal
		{ get; set; }


		private const int SpriteMargin = 2;

		private int _spriteWidth;
		private int _spriteHeight;

		private int _tilesX = 1;

		private int _startY;

		private int _idSelected;
		private int _idOver;

		private int _overX = -1;
		private int _overY = -1;

		private const string None = "n/a";


		private readonly List<SpriteSelected> _selectedSprites = new List<SpriteSelected>();
		internal ReadOnlyCollection<SpriteSelected> SelectedSprites
		{
			get { return (SpritePack != null) ? _selectedSprites.AsReadOnly()
											  : null; }
		}

		/// <summary>
		/// Used by UpdateScrollBar() to determine its Maximum value.
		/// </summary>
		internal int TableHeight
		{
			get // TODO: calculate and cache this value in the OnResize and loading events.
			{
				SetTilesX();

				int height = 0;
				if (SpritePack != null)
					height = (SpritePack.Count / _tilesX + 2) * _spriteHeight;

				return height;
			}
		}

		/// <summary>
		/// The LargeChange value for the scrollbar will return "1" when the bar
		/// isn't visible. Therefore this value needs to be used instead of the
		/// actual LargeValue in order to calculate the panel's various dynamics.
		/// </summary>
		private int _largeChange;
		#endregion


		#region cTor
		internal ViewPanel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);


			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.SmallChange = 1;
//			_scrollBar.LargeChange = 44; // NOTE: this won't stick unless Visible, perhaps. else "1"
			_scrollBar.ValueChanged += OnScrollBarValueChanged;

			_statusTileSelected = new StatusBarPanel();
			_statusTileSelected.Width = 100;

			_statusTileOver = new StatusBarPanel();
			_statusTileOver.Width = 75;

			_statusBar = new StatusBar();
			_statusBar.Dock = DockStyle.Bottom;
			_statusBar.ShowPanels = true;
			_statusBar.Panels.Add(_statusTileSelected);
			_statusBar.Panels.Add(_statusTileOver);

			Controls.AddRange(new Control[]
			{
				_scrollBar,
				_statusBar
			});

			OnSpriteClick(-1);
			OnSpriteOver(-1);
		}
		#endregion


		#region EventCalls

		/// <summary>
		/// Fires when anything changes the Value of the scroll-bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			_startY = -_scrollBar.Value;
			Refresh();
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
			UpdateScrollbar(false);
		}

		/// <summary>
		/// Updates the scrollbar after a resize event or a sprite-pack changed
		/// event.
		/// </summary>
		/// <param name="resetTrack">true to set the thing to the top of the track</param>
		private void UpdateScrollbar(bool resetTrack)
		{
			int range = 0;
			if (SpritePack != null && SpritePack.Count != 0)
			{
				if (resetTrack)
					_scrollBar.Value = 0;

				range = TableHeight + _largeChange - Height - _statusBar.Height;
				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = (range != 0);
		}

		internal void SetTilesX()
		{
			int tilesX = 1;

			if (SpritePack != null && SpritePack.Count != 0)
			{
//				tilesX = (Width - 1) / _spriteWidth; // calculate without widthScroll first

				// On 2nd thought always reserve width for the scrollbar.
				// So user can increase/decrease the Height of the window
				// without the tiles re-arranging.
				tilesX = (Width - _scrollBar.Width - 1) / _spriteWidth;

				if (tilesX > SpritePack.Count)
					tilesX = SpritePack.Count;

				// This was for, if extra width was *not* reserved for the
				// scrollbar, deciding if that width now needs to be injected
				// since the scrollbar is going to appear, after all:
//				if (tilesX * _spriteWidth + _scrollBar.Width > Width - 1
//					&& (SpritePack.Count / tilesX + 2) * _spriteHeight > Height - _statusBar.Height - 1)
//				{
//					--tilesX;
//				}
			}
			_tilesX = tilesX;
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
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
//			base.OnMouseDown(e);

//			Focus();	// also set in SpritePack setter. here in case user tabs away from this Panel.
						// but there's no tabbing atm.

			bool clearSelected = true;

			if (SpritePack != null && SpritePack.Count != 0)
			{
				if (e.X < _spriteWidth * _tilesX) // not out of bounds to right
				{
					int tileX =  e.X            / _spriteWidth;
					int tileY = (e.Y - _startY) / _spriteHeight;

					int id = tileX + tileY * _tilesX;
					if (id < SpritePack.Count) // not out of bounds below
					{
						var selected   = new SpriteSelected();
						selected.X     = tileX;
						selected.Y     = tileY;
						selected.Id    = id;
						selected.Image = SpritePack[id];

//						if (ModifierKeys == Keys.Control)
//						{
//							SpriteSelected spritePre = null;
//
//							foreach (var sprite in _selectedSprites)
//							{
//								if (sprite.X == tileX && sprite.Y == tileY)
//									spritePre = sprite;
//							}
//
//							if (spritePre != null)
//							{
//								_selectedSprites.Remove(spritePre);
//							}
//							else
//								_selectedSprites.Add(selected);
//						}
//						else
//						{
						_selectedSprites.Clear();
						_selectedSprites.Add(selected);
//						}

						OnSpriteClick(id);
						clearSelected = false;
					}
				}
			}

			if (clearSelected)
			{
				_selectedSprites.Clear();
				OnSpriteClick(-1);
			}

			Refresh();
		}

		/// <summary>
		/// Shows status-information for a sprite. Overrides core implementation
		/// for the MouseMove event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
//			base.OnMouseMove(e);

			if (SpritePack != null && SpritePack.Count != 0)
			{
				if ( e.X > _spriteWidth * _tilesX - 1) // out of bounds to right
				{
					OnSpriteOver(-1);
				}
				else
				{
					int tileX =  e.X            / _spriteWidth;
					int tileY = (e.Y - _startY) / _spriteHeight;

					if (tileX != _overX || tileY != _overY)
					{
						_overX = tileX;
						_overY = tileY;

						int id = tileX + tileY * _tilesX;
						if (id >= SpritePack.Count) // out of bounds below
							id = -1;

						OnSpriteOver(id);
					}
				}
			}
		}

		/// <summary>
		/// Updates the status-information for the sprite that the cursor is
		/// currently over.
		/// </summary>
		/// <param name="spriteId">the entry # (id) of the currently mouseovered
		/// sprite in the currently loaded PckPack</param>
		private void OnSpriteOver(int spriteId)
		{
			if (spriteId == -1)
			{
				_overX =
				_overY = -1;
			}

			_idOver = spriteId;
			PrintStatus();
		}

		/// <summary>
		/// Updates the status-information for the sprite that the cursor is
		/// currently over.
		/// </summary>
		/// <param name="spriteId">the entry # (id) of the currently mouseclicked
		/// sprite in the currently loaded PckPack</param>
		private void OnSpriteClick(int spriteId)
		{
			_idSelected = spriteId;
			PrintStatus();
		}

		/// <summary>
		/// Prints the current status for the currently selected and/or
		/// mouseovered sprite(s) in the status-bar.
		/// </summary>
		private void PrintStatus()
		{
			string selected = (_idSelected != -1) ? _idSelected.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : None;
			string over     = (_idOver != -1)     ? _idOver.ToString(System.Globalization.CultureInfo.InvariantCulture)
												  : None;

			_statusTileSelected.Text = String.Format(
												System.Globalization.CultureInfo.InvariantCulture,
												"Selected {0}", selected);
			_statusTileOver.Text     = String.Format(
												System.Globalization.CultureInfo.InvariantCulture,
												"Over {0}", over);
		}

		/// <summary>
		/// Let's draw this puppy.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (SpritePack != null && SpritePack.Count != 0)
			{
				var g = e.Graphics;

				for (int tileX = 0; tileX <= _tilesX; ++tileX) // draw vertical lines
					g.DrawLine(
							Pens.Black,
							new Point(tileX * _spriteWidth,          _startY),
							new Point(tileX * _spriteWidth, Height - _startY));

				int tilesY = SpritePack.Count / _tilesX;
				if (SpritePack.Count % _tilesX != 0)
					++tilesY;

				for (int tileY = 0; tileY <= tilesY; ++tileY) // draw horizontal lines
					g.DrawLine(
							Pens.Black,
							new Point(0,                      tileY * _spriteHeight + _startY),
							new Point(_spriteWidth * _tilesX, tileY * _spriteHeight + _startY));


				var selected = new List<int>();
				foreach (var sprite in _selectedSprites)
					selected.Add(sprite.Id);

				for (int id = 0; id != SpritePack.Count; ++id) // fill selected tiles and draw sprites.
				{
					int tileX = id % _tilesX;
					int tileY = id / _tilesX;

					if (selected.Contains(id))
						g.FillRectangle(
									Brushes.Crimson,
									tileX * _spriteWidth  + 1,
									tileY * _spriteHeight + 1 + _startY,
									_spriteWidth  - 1,
									_spriteHeight - 1);

					g.DrawImage(
							SpritePack[id].Image,
							tileX * _spriteWidth  + SpriteMargin,
							tileY * _spriteHeight + SpriteMargin + _startY);
				}
			}
		}
		#endregion


		#region Methods
		internal void SpriteReplace(int id, XCImage image) // currently disabled in PckViewForm
		{
			SpritePack[id] = image;
		}

		/// <summary>
		/// Deletes the currently selected sprite.
		/// </summary>
		internal void SpriteDelete() // currently disabled in PckViewForm
		{
			if (_selectedSprites.Count != 0)
			{
				var lowestId = int.MaxValue;

				var idList = new List<int>();
				foreach (var sprite in _selectedSprites)
					idList.Add(sprite.Id);

				idList.Sort();
				idList.Reverse();

				foreach (var id in idList)
				{
					if (id < lowestId)
						lowestId = id;

					SpritePack.Remove(id);
				}

				if (lowestId > 0 && lowestId == SpritePack.Count)
					lowestId = SpritePack.Count - 1;

				_selectedSprites.Clear();
	
				if (SpritePack.Count != 0)
				{
					var selected = new SpriteSelected();
					selected.Y   = lowestId / _tilesX;
					selected.X   = lowestId - selected.Y;
					selected.Id  = selected.X + selected.Y * _tilesX;
	
					_selectedSprites.Add(selected);
				}
			}
		}
		#endregion

/*		private void tileChooser_SelectedIndexChanged(object sender, EventArgs e)
		{
//			view.Pck = ImageCollection.GetPckFile(tileChooser.SelectedItem.ToString());
			view.Refresh();
//			_scrollBar.Maximum = Math.Max((view.Height - Height + tileChooser.Height + 50), _scrollBar.Minimum);
			_scrollBar.Value = _scrollBar.Minimum;
			scroll_Scroll(null, null);
		} */

/*		internal void Hq2x()
		{
			_collection.HQ2X();
		} */
/*		internal void Hq2x()
		{
			_viewPanel.Hq2x();
		} */

/*		/// <summary>
		/// Saves a bitmap as an 8-bit image.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="pal"></param>
		public void SaveBMP(string file, Palette pal)
		{
			Bmp.SendToSaver(file, _collection, pal, numAcross(), 1);
		} */
	}



	#region SpritePackChanged event handler & args

	/// <summary>
	/// EventArgs for SpritePackChangedEvent.
	/// </summary>
	internal sealed class SpritePackChangedEventArgs
	{
		internal XCImageCollection Sprites
		{ get; private set; }


		internal SpritePackChangedEventArgs(XCImageCollection sprites)
		{
			Sprites = sprites;
		}
	}
	#endregion
}
