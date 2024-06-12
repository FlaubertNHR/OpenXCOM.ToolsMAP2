using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// A separate panel is created for each tab-page in the Tileparts viewer.
	/// </summary>
	internal sealed class TilePanel
		:
			BufferedPanel
	{
		internal static void DisposePanels()
		{
			//DSShared.Logfile.Log("TilePanel.DisposePanels() static");
			GridLineBrush     .Dispose();
			SelectedPartBorder.Dispose();
			Eraser            .Dispose();
		}

/*		#region IDisposable interface
		// https://www.codeproject.com/articles/29534/idisposable-what-your-mother-never-told-you-about
		// https://dave-black.blogspot.com/2011/03/how-do-you-properly-implement.html
		// etc etc etc

		/// <summary>
		/// Gets or sets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value><c>true</c> if this instance is disposed</value>
		/// <remarks>Default initialization for a bool is <c>false</c>.</remarks>
//		private bool IsDisposed { get; set; }	// <- hides inherited member 'System.Windows.Forms.Control.IsDisposed'
//		private bool Disposed { get; set; }		// <- hides inherited member 'System.ComponentModel.Component.Disposed'
		private bool _disposed;					// -> so fuck you too.

		// Look. This is more convoluted, confusing, and time-consuming to
		// figure out how to implement than simply allocating and deallocating
		// memory in totally unmanaged languages like C/C++ (before they decided
		// it would be a good thing to implement it there also).
		//
		// No one even seems to know exactly what "managed" and "unmanaged"
		// means - not to mention wtf a "native resource" is.
		//
		// Dave Black, who has been working professionally with low-level C#,
		// specializing in and trouble-shooting disposal for 15 years, can't
		// describe what to do accurately (although he tries to sound like he
		// does). The wiser folks who are on .NET have learned, rather, to just
		// keep their mouths shut.
		//
		// To be honest, the only categorical reason I can see for overriding
		// Dispose(bool) is so that objects can be instantiated then disposed
		// with a using() statement. But MapView doesn't really do that pattern
		// for major objects like the viewers and their controls.
		//
		// And it doesn't have to: because just write and call a function like
		//   DisposeObject()
		//   FreeResources()
		//   ReleaseHandles()
		//   DestroyStuff()
		//   whatever()
		//
		// That is bypass their confusing-as-fuck "pattern" wherever possible.
		//
		// Treat c#/.net like an 'unmanaged' language. If you new it it's your
		// responsibility to decide whether, when, where, and how to dispose it.
		//
		// And congratulate yourself that no one is going to notice ...

		/// <summary>
		/// Overloaded Implementation of Dispose.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and
		/// unmanaged resources; <c>false</c> to release only unmanaged
		/// resources.</param>
		/// <remarks>
		/// <list type="bulleted">Dispose(bool isDisposing) executes in two
		/// distinct scenarios.
		/// <item>If <paramref name="disposing"/> equals <c>true</c> the
		/// method has been called directly or indirectly by a user's code.
		/// Managed and unmanaged resources can be disposed.</item>
		/// <item>If <paramref name="disposing"/> equals <c>false</c> the
		/// method has been called by the runtime from inside the finalizer and
		/// you should not reference other objects. Only unmanaged resources can
		/// be disposed.</item></list>
		/// </remarks>
		protected override void Dispose(bool disposing)
		{
			DSShared.Logfile.Log("TilePanel.Dispose(" + disposing + ")");
			// TODO If you need thread safety, use a lock around these
			// operations as well as in your methods that use the resource.
			try
			{
				if (!_disposed) // && !IsDisposed
				{
					// Explicitly set root references to null to expressly tell the
					// GarbageCollector that the resources have been disposed of
					// and it's ok to release the memory allocated for them.

					if (disposing)
					{
						// Release all managed resources here
						//
						// Need to unregister/detach yourself from the events. Always make
						// sure the object is not null first before trying to unregister/detach
						// them! Failure to unregister can be a BIG source of memory leaks.

						if (PenRed != null) // static object
						{
							PenRed.Dispose();
//							PenRed = null;
						}

						if (_t1 != null) // static object
						{
//							_t1 -= t1_Tick; // should not be a problem even if already unsubscribed.
							_t1.Dispose();
//							_t1 = null;
						}

						// uncomment this code if this is a WinForm-UI control that has assigned components
//						if (components != null)
//							components.Dispose();
					}

					// release all unmanaged resources here
//					if (someComObject != null && Marshal.IsComObject(someComObject))
//					{
//						Marshal.FinalReleaseComObject(someComObject);
//						someComObject = null;
//					}
				}
			}
			finally
			{
				_disposed = true;
				base.Dispose(disposing);
			}
		}
		#endregion IDisposable interface */


		#region Fields (static)
		internal static TileView TileView;

		private const int SpriteMargin = 2;
		private const int SpriteWidth  = Spriteset.SpriteWidth32  + SpriteMargin * 2;
		private const int SpriteHeight = Spriteset.SpriteHeight40 + SpriteMargin * 2;

		private const int _largeChange = SpriteHeight;	// apparently .NET won't return an accurate value
														// for LargeChange unless the scrollbar is visible.

		internal static readonly Dictionary<SpecialType, SolidBrush> SpecialBrushes =
							 new Dictionary<SpecialType, SolidBrush>();

		internal static readonly Pen GridLineBrush      = new Pen(  TileViewOptionables.def_GridLineColor);
		internal static readonly Pen SelectedPartBorder = new Pen(  TileViewOptionables.def_SelectedPartBorderColor,
																    TileViewOptionables.def_SelectedPartBorderWidth);
		internal static readonly SolidBrush Eraser = new SolidBrush(TileViewOptionables.def_EraserBackcolor);

		internal const string Door = "door";
		internal static int TextWidth;

		private const int TableOffset = 2;
		#endregion Fields (static)


		#region Fields
		private Tilepart[] _parts;

		private readonly VScrollBar _scrollBar;

		private int _tilesX = 1;
		private int _startY;
		private int _id;

		/// <summary>
		/// The <c><see cref="PartType"/></c> of the
		/// <c><see cref="Tilepart">Tileparts</see></c> that this
		/// <c>TilePanel</c> displays.
		/// </summary>
		/// <remarks><c>PartType.Invalid</c> designates all types.</remarks>
		private PartType _partType;

		private bool _resetTrack;
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets the current <c>SelectedTilepart</c>.<br/>
		/// Sets the <c>SelectedTilepart</c> when a valid
		/// <c><see cref="QuadrantControl"/></c> quad gets selected.
		/// </summary>
		/// <remarks>The setter is used only by
		/// <c><see cref="TileView.SelectedTilepart">TileView.SelectedTilepart</see></c>.</remarks>
		internal Tilepart SelectedTilepart
		{
			// TODO: refactor this conglomeration by storing an actual Tilepart
			// and store '_id' separately (setting '_id' can set 'SelectedTilepart' or vice versa)
			get
			{
				if (_parts != null
					&& _id > -1 // TODO: <- is not likely (see setter)
					&& _id < _parts.Length)
				{
					return _parts[_id];
				}
				return null;
			}
			set
			{
				if (value != null						// crippled parts shall not be selected here
//					&& value.SetId > -1					// <- is not likely (perhaps crippled tileparts, but they shouldn't appear in the tilepanels, or perhaps by a bork in TilepartSubstitution).
					&& value.SetId < _parts.Length - 1)	// -1 to account for the null-sprite
				{
					_id = value.SetId + 1;				// +1 to account for the null-sprite.
				}
				else
					_id = 0;							// the null-sprite (ie. the eraser)

				TileView.SelectTilepart(SelectedTilepart); // TODO: not that. <-

				ScrollToTile();
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="partType"></param>
		internal TilePanel(PartType partType)
		{
			_partType = partType;

			SetStyle(ControlStyles.Selectable, true);
			TabStop = true;

			Dock = DockStyle.Fill;

			_scrollBar = new VScrollBar();
			_scrollBar.Dock = DockStyle.Right;
			_scrollBar.LargeChange = _largeChange;
			_scrollBar.SmallChange = 1;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;
			Controls.Add(_scrollBar);

			MainViewUnderlay.PhaseEvent += OnPhaseEvent;
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Clears OverInfo on the statusbar when the cursor is not in a panel.
		/// </summary>
		internal void ElvisHasLeftThePanel()
		{
			if (!Bounds.Contains(PointToClient(Control.MousePosition)))
				TileView.PrintOverInfo(null);
		}

		/// <summary>
		/// Invalidates this <c>TilePanel</c> if tileparts are being animated.
		/// </summary>
		private void OnPhaseEvent()
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
		/// <summary>
		/// Handles client resizing. Sets the scrollbar's Maximum value. And
		/// ensures that the bottom of the tile-table gets snuggled down against
		/// the bottom of the panel's area if required.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			int height = GetTableHeight();

			int range = 0;
			if (_parts != null && _parts.Length != 0)
			{
				if (_resetTrack)
				{
					_resetTrack = false;
					_scrollBar.Value = 0;
				}

				range = height + _largeChange - Height;
				if (range < _largeChange)
					range = 0;
			}
			_scrollBar.Maximum = range;
			_scrollBar.Visible = range != 0;

			if (_scrollBar.Visible
				&& height - _scrollBar.Value < Height)
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
		/// Focuses this <c>TilePanel</c> and selects a
		/// <c><see cref="Tilepart"/></c>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (TileView.GetMapfile() != null)
			{
				switch (e.Button)
				{
					case MouseButtons.Left:
					case MouseButtons.Right:
					{
						int id = GetOverId(e);
						if (id != -1 && id < _parts.Length)
						{
							_id = id;

							TileView.SelectTilepart(SelectedTilepart);

							ScrollToTile();
							Invalidate();
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Navigates the <c><see cref="Tilepart">Tileparts</see></c> of this
		/// <c>TilePanel</c> on keydown events at the Form level.
		/// </summary>
		/// <param name="keyData"></param>
		internal void Navigate(Keys keyData)
		{
			if (TileView.GetMapfile() != null)
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

					case (Keys.Control | Keys.Home):
						id = 0;
						break;

					case (Keys.Control | Keys.End):
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

					TileView.SelectTilepart(SelectedTilepart);

					ScrollToTile();
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Opens the <see cref="McdInfoF"/> dialog when a valid tilepart is
		/// double-left-clicked.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				int id = GetOverId(e);
				if (id != -1 && id < _parts.Length)
				{
					TileView.OnMcdInfoClick(this, EventArgs.Empty); // 'this' prevents McdInfo from hiding on a double-click
				}
			}
		}

		/// <summary>
		/// Opens/shows or hides the <see cref="McdInfoF"/> dialog when [i] is
		/// key-upped.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyData == Keys.I)
				TileView.OnMcdInfoClick(null, EventArgs.Empty);
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

			TileView.PrintOverInfo(part);
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


		/// <summary>
		/// this.Fill(black)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (MainViewF.Dontdrawyougits) return;

			if (_parts != null && _parts.Length != 0)
			{
				Graphics graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				var rectOuter = new Rectangle(0,0, SpriteWidth, SpriteHeight);
				var rectInner = new Rectangle();

				if (!MainViewF.Optionables.UseMono)
				{
					rectInner.Width  = Spriteset.SpriteWidth32;
					rectInner.Height = Spriteset.SpriteHeight40;
				}

				int x = 0;
				int y = 0;
				int L,T; // left,top

				int phase = MainViewUnderlay.Phase;

				XCImage sprite;
				byte[] bindata;

				foreach (var part in _parts)
				{
					rectOuter.X = (L = SpriteWidth  * x + TableOffset);
					rectOuter.Y = (T = SpriteHeight * y + TableOffset + _startY);

					if (part != null) // draw tile-sprite ->
					{
						SpecialType special = part.Record.Special;				// first fill w/ SpecialProperty color
						if (SpecialBrushes.ContainsKey(special))
							graphics.FillRectangle(SpecialBrushes[special], rectOuter);

						if ((sprite = part[phase]) != null)
						{
							if (MainViewF.Optionables.UseMono)
							{
								bindata = sprite.GetBindata();

								int palid;
								int i = -1;
								for (int h = 0; h != Spriteset.SpriteHeight40; ++h)
								for (int w = 0; w != Spriteset.SpriteWidth32;  ++w)
								{
									if ((palid = bindata[++i]) != Palette.Tid)
									{
										graphics.FillRectangle(
															Palette.MonoBrushes[palid],
															L + w + 2,
															T + h + 2,
															1,1);
									}
								}
							}
							else
							{
								rectInner.X = L + SpriteMargin;
								rectInner.Y = T + SpriteMargin - part.Record.SpriteOffset;
								graphics.DrawImage(								// then draw the sprite itself
												sprite.Sprite,
												rectInner,
												0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
												GraphicsUnit.Pixel,
												Globals.Ia);
							}
						}

						// NOTE: keep the door-string and its placement consistent with
						// QuadrantDrawService.Draw().
						if (part.IsDoor)										// finally print "door" if it's a door
							graphics.DrawString(
											Door,
											Font,
											Brushes.Black,
											L + (SpriteWidth  - TextWidth) / 2,
											T +  SpriteHeight - Font.Height);
					}
					else // draw the eraser ->
					{
						graphics.FillRectangle(Eraser, rectOuter);
						graphics.DrawImage(
										MainViewF.MonotoneSprites[QuadrantDrawService.Quad_ERASER].Sprite,
										L,T);
					}

					if ((x = (x + 1) % _tilesX) == 0) ++y;
				}

				if (!_scrollBar.Visible) // indicate the reserved width for scrollbar.
					graphics.DrawLine(
									SystemPens.ControlLight,
									Width - _scrollBar.Width - 1, 0,
									Width - _scrollBar.Width - 1, Height);

				graphics.DrawLine(GridLineBrush, TableOffset - 1, TableOffset + _startY,
												 TableOffset,     TableOffset + _startY);

				int height = GetTableHeight();

				for (int i = 0; i <= _tilesX; ++i)								// draw vertical lines
					graphics.DrawLine(
									GridLineBrush,
									TableOffset + SpriteWidth * i, TableOffset + _startY,
									TableOffset + SpriteWidth * i, /*TableOffset +*/ _startY + height);

				for (int i = 0; i <= height; i += SpriteHeight)					// draw horizontal lines
					graphics.DrawLine(
									GridLineBrush,
									TableOffset,                         TableOffset + _startY + i,
									TableOffset + SpriteWidth * _tilesX, TableOffset + _startY + i);

				graphics.DrawRectangle(											// draw selected rectangle
									SelectedPartBorder,
									TableOffset + _id % _tilesX * SpriteWidth,
									TableOffset + _id / _tilesX * SpriteHeight + _startY,
									SpriteWidth, SpriteHeight);
			}
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Assigns <c><see cref="Tilepart">Tileparts</see></c> to this
		/// <c>TilePanel</c>.
		/// </summary>
		/// <param name="parts">a list of <c>Tileparts</c></param>
		internal void PopulatePanel(IList<Tilepart> parts)
		{
			if (_partType == PartType.Invalid) // is ALL parts panel ->
			{
				_parts = new Tilepart[parts.Count + 1]; // +1 for the null-sprite
				_parts[0] = null;

				for (int i = 0; i != parts.Count; ++i)
					_parts[i + 1] = parts[i];
			}
			else // is Floor,West,North,Content panel ->
			{
				int tiles = 0;

				for (int i = 0; i != parts.Count; ++i)
					if (parts[i].Record.PartType == _partType)
						++tiles;

				_parts = new Tilepart[tiles + 1]; // +1 for the null-sprite
				_parts[0] = null;

				for (int i = 0, j = 1; i != parts.Count; ++i)
					if (parts[i].Record.PartType == _partType)
						_parts[j++] = parts[i];
			}

			if (_id >= _parts.Length)
				_id = 0;

			_resetTrack = true;
			OnResize(null);
		}

		/// <summary>
		/// 
		/// </summary>
		internal void ClearPanel()
		{
			_parts = null;
		}

		/// <summary>
		/// Gets the total height required for the tile-table.
		/// </summary>
		/// <returns></returns>
		/// <remarks>TODO: calculate and cache this value in the OnResize and
		/// loading events.</remarks>
		private int GetTableHeight()
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
