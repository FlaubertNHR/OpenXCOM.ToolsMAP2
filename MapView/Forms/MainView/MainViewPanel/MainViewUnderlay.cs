using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;

using XCom;


namespace MapView.Forms.MainView
{
	internal sealed class MainViewUnderlay
		:
			BufferedPanel // god I hate these double-panels!!!! cf. MainViewOverlay
	{
		internal static void DisposeUnderlay()
		{
			//DSShared.Logfile.Log("MainViewUnderlay.DisposeUnderlay() static");
			if (_t1 != null)
				_t1.Dispose();
		}

		#region Fields (static)
		// these track the offset between the panel border and the lozenge-tips.
		// NOTE: they are used for both the underlay and the overlay, which
		// currently have the same border sizes; if one or the other changes
		// then their offsets would have to be separated.
		private const int OffsetX = 2;
		private const int OffsetY = 2;
		#endregion Fields (static)


		#region Fields
		private readonly VScrollBar _scrollBarV = new VScrollBar();
		private readonly HScrollBar _scrollBarH = new HScrollBar();

		private MainViewOverlay _overlay;

		private MapFile _file;
		#endregion Fields


		#region Properties (static)
		internal static MainViewUnderlay that
		{ get; private set; }
		#endregion Properties (static)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal MainViewUnderlay()
		{
			that = this;
			_overlay = new MainViewOverlay();

			Dock = DockStyle.Fill;
			BorderStyle = BorderStyle.Fixed3D;

			PhaseEvent += OnPhaseEvent;

			_scrollBarV.Dock = DockStyle.Right;
			_scrollBarV.Scroll += OnScrollVert;

			_scrollBarH.Dock = DockStyle.Bottom;
			_scrollBarH.Scroll += OnScrollHori;

			Controls.AddRange(new Control[]
			{
				_scrollBarV,
				_scrollBarH,
				_overlay
			});


//			DSShared.Logfile.Log("");
//			DSShared.Logfile.Log("MainViewUnderlay cTor");
//			DSShared.Logfile.Log(". underlay.Width= " + Width);
//			DSShared.Logfile.Log(". underlay.Height= " + Height);
//
//			DSShared.Logfile.Log(". underlay client.Width= " + ClientSize.Width);
//			DSShared.Logfile.Log(". underlay client.Height= " + ClientSize.Height);
//
//			DSShared.Logfile.Log(". overlay.Width= " + _mainViewOverlay.Width);
//			DSShared.Logfile.Log(". overlay.Height= " + _mainViewOverlay.Height);
//
//			DSShared.Logfile.Log(". overlay client.Width= " + _mainViewOverlay.ClientSize.Width);
//			DSShared.Logfile.Log(". overlay client.Height= " + _mainViewOverlay.ClientSize.Height);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
			// indicate reserved space for scroll-bars.
			Graphics graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.Half;

			graphics.DrawLine(
							SystemPens.ControlLight,
							Width - _scrollBarV.Width - OffsetX - 1, OffsetY,
							Width - _scrollBarV.Width - OffsetX - 1, Height - _scrollBarH.Height - OffsetY - 1);
			graphics.DrawLine(
							SystemPens.ControlLight,
							OffsetX,                                 Height - _scrollBarH.Height - OffsetY - 1,
							Width - _scrollBarV.Width - OffsetX - 1, Height - _scrollBarH.Height - OffsetY - 1);
		}

		/// <summary>
		/// Forces a <c>Resize</c> event for this Panel. Grants access for
		/// <c><see cref="MainViewF"></see>.OnMapResizeClick()</c>.
		/// </summary>
		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
//			DSShared.Logfile.Log("");
//			DSShared.Logfile.Log("MainViewUnderlay.OnResize");
//
//			DSShared.Logfile.Log("underlay.Width= " + Width);
//			DSShared.Logfile.Log("underlay.Height= " + Height);
//
//			DSShared.Logfile.Log("underlay client.Width= " + ClientSize.Width);
//			DSShared.Logfile.Log("underlay client.Height= " + ClientSize.Height);
//
//			DSShared.Logfile.Log("overlay.Width= " + _overlay.Width);
//			DSShared.Logfile.Log("overlay.Height= " + _overlay.Height);
//
//			DSShared.Logfile.Log("overlay client.Width= " + _overlay.ClientSize.Width);
//			DSShared.Logfile.Log("overlay client.Height= " + _overlay.ClientSize.Height);


			base.OnResize(eventargs);

			if (_file != null && Globals.AutoScale)
				SetScale();

			UpdateScrollers();

			Invalidate(); // updates the reserved scroll indicators.

//			DSShared.Logfile.Log("MainViewUnderlay.OnResize EXIT");
		}

		/// <summary>
		/// Focuses <c><see cref="MainViewOverlay"/></c> when the panel is
		/// clicked just outside the border of the Map or if there is no Map.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (MainViewF.that.MapFile != null)
				_overlay.Select();
			else
				MainViewF.that.MapTree.Select();
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollVert(object sender, ScrollEventArgs e)
		{
			//DSShared.Logfile.Log("OnVerticalScroll overlay.Left= " + _overlay.Left);
			_overlay.Location = new Point(
										_overlay.Left,
										-_scrollBarV.Value);
			_overlay.Invalidate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScrollHori(object sender, ScrollEventArgs e)
		{
			//DSShared.Logfile.Log("OnVerticalScroll overlay.Top= " + _overlay.Top);
			_overlay.Location = new Point(
										-_scrollBarH.Value,
										_overlay.Top);
			_overlay.Invalidate();
		}

		/// <summary>
		/// Invalidates <c><see cref="_overlay"/></c> if tileparts are being
		/// animated.
		/// </summary>
		private void OnPhaseEvent()
		{
			_overlay.Invalidate();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Sets <c><see cref="_file"/></c>.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		internal void SetMapFile(MapFile file)
		{
			if ((_file = file) != null)
				SetOverlaySize();

			OnResize(EventArgs.Empty);
		}

		/// <summary>
		/// Gets the width of the vertical scrollbar.
		/// </summary>
		/// <returns></returns>
		internal int GetVertbarWidth()
		{
			if (_scrollBarV.Visible) return _scrollBarV.Width;
			return 0;
		}

		/// <summary>
		/// Gets the height of the horizontal scrollbar.
		/// </summary>
		/// <returns></returns>
		internal int GetHoribarHeight()
		{
			if (_scrollBarH.Visible) return _scrollBarH.Height;
			return 0;
		}

		/// <summary>
		/// Mousegrab pan horizontal.
		/// </summary>
		/// <param name="delta"></param>
		internal void ScrollHori(int delta)
		{
			if (_scrollBarH.Visible)
			{
				int val = _scrollBarH.Value + delta;
				if (val < 0)
					val = 0;
				else if (val > _scrollBarH.Maximum - (_scrollBarH.LargeChange - 1))
						 val = _scrollBarH.Maximum - (_scrollBarH.LargeChange - 1);

				_scrollBarH.Value = val;
				OnScrollHori(null, null);
			}
		}

		/// <summary>
		/// Mousegrab pan vertical.
		/// </summary>
		/// <param name="delta"></param>
		internal void ScrollVert(int delta)
		{
			if (_scrollBarV.Visible)
			{
				int val = _scrollBarV.Value + delta;
				if (val < 0)
					val = 0;
				else if (val > _scrollBarV.Maximum - (_scrollBarV.LargeChange - 1))
						 val = _scrollBarV.Maximum - (_scrollBarV.LargeChange - 1);

				_scrollBarV.Value = val;
				OnScrollVert(null, null);
			}
		}

//		/// <summary>
//		/// A workaround for maximizing the parent-form. See notes at
//		/// MainViewF.OnResize(). Note that this workaround pertains only to
//		/// cases when AutoScale=FALSE.
//		/// </summary>
//		internal void ResetScrollers()
//		{
//			// NOTE: if the form is enlarged with scrollbars visible and the
//			// new size doesn't need scrollbars but the map was offset, the
//			// scrollbars disappear but the map is still offset. So fix it.
//			//
//			// TODO: this is a workaround.
//			// It simply relocates the overlay to the origin, but it should try
//			// to maintain focus on a selected tile for cases when the form is
//			// enlarged *and the overlay still needs* one of the scrollbars.
//
//			_scrollBarV.Value =
//			_scrollBarH.Value = 0;
//
//			_overlay.Location = new Point(0, 0);
//		}

		/// <summary>
		/// Handles the scroll-bars.
		/// </summary>
		internal void UpdateScrollers()
		{
			if (Globals.AutoScale)
			{
				_scrollBarV.Visible =
				_scrollBarH.Visible = false;

				_scrollBarV.Value =
				_scrollBarH.Value = 0;

				_overlay.Location = new Point(0,0);
			}
			else
			{
				// TODO: scrollbars jiggery-pokery needed.
				_scrollBarV.Visible = (_overlay.Height > ClientSize.Height + OffsetY);
				if (_scrollBarV.Visible)
				{
					_scrollBarV.Maximum = Math.Max(
												_overlay.Height
													- ClientSize.Height
													+ _scrollBarH.Height
													+ OffsetY * 4, // <- top & bottom Underlay + top & bottom Overlay borders
												0);
					_scrollBarV.Value = Math.Min(
												_scrollBarV.Value,
												_scrollBarV.Maximum);
					OnScrollVert(null, null);
				}
				else
				{
					_scrollBarV.Value = 0;
					_overlay.Location = new Point(Left, 0);
				}

				_scrollBarH.Visible = (_overlay.Width > ClientSize.Width + OffsetX);
				if (_scrollBarH.Visible)
				{
					_scrollBarH.Maximum = Math.Max(
												_overlay.Width
													- ClientSize.Width
													+ _scrollBarV.Width
													+ OffsetX * 4, // <- left & right Underlay + left & right Overlay borders
												0);
					_scrollBarH.Value = Math.Min(
												_scrollBarH.Value,
												_scrollBarH.Maximum);
					OnScrollHori(null, null);
				}
				else
				{
					_scrollBarH.Value = 0;
					_overlay.Location = new Point(0, Top);
				}
			}

			_overlay.Refresh();
		}

		/// <summary>
		/// Sets the scale-factor. Is used only if AutoScale=TRUE.
		/// </summary>
		internal void SetScale()
		{
			//DSShared.Logfile.Log("");
			//DSShared.Logfile.Log("MainViewUnderlay.SetScale");

			Size required = DeterOverlaySize(1f);
			Globals.Scale = Math.Min(
								(float)(Width  - OffsetX) / required.Width,
								(float)(Height - OffsetY) / required.Height);
			Globals.Scale = Globals.Scale.Viceroy(
												Globals.ScaleMinimum,
												Globals.ScaleMaximum);

			//DSShared.Logfile.Log(". scale set to= " + Globals.Scale);
			SetOverlaySize();
		}

		/// <summary>
		/// Sets this Panel to the size of the current Map including scaling.
		/// </summary>
		internal void SetOverlaySize()
		{
			//DSShared.Logfile.Log("");
			//DSShared.Logfile.Log("MainViewUnderlay.SetOverlaySize");

			if (_file != null)
			{
				//DSShared.Logfile.Log(". scale= " + Globals.Scale);
				Size size = DeterOverlaySize(Globals.Scale);

				_overlay.Width  = size.Width;
				_overlay.Height = size.Height;

				//DSShared.Logfile.Log(". set overlay.Width= " + _overlay.Width);
				//DSShared.Logfile.Log(". set overlay.Height= " + _overlay.Height);
			}
		}

		/// <summary>
		/// Determines the required width/height in pixels for the current Map
		/// as a lozenge. Also sets
		/// <c><see cref="MainViewOverlay.Origin">MainViewOverlay.Origin</see></c>
		/// along with
		/// <c><see cref="MainViewOverlay.HalfWidth">MainViewOverlay.HalfWidth</see></c>
		/// and
		/// <c><see cref="MainViewOverlay.HalfHeight">MainViewOverlay.HalfHeight</see></c>.
		/// </summary>
		/// <param name="scale">the current scaling factor</param>
		/// <returns></returns>
		private Size DeterOverlaySize(float scale)
		{
			//DSShared.Logfile.Log("");
			//DSShared.Logfile.Log("MainViewUnderlay.GetRequiredOverlaySize");

			if (_file != null)
			{
				//DSShared.Logfile.Log(". scale= " + Globals.Scale);

				int halfWidth  = (int)(MainViewOverlay.HalfWidthConst  * scale);
				int halfHeight = (int)(MainViewOverlay.HalfHeightConst * scale);

				if (halfHeight > halfWidth / 2) // use width
				{
					//DSShared.Logfile.Log(". use width");

					if (halfWidth % 2 != 0)
						--halfWidth;

					halfHeight = halfWidth / 2;
				}
				else // use height
				{
					//DSShared.Logfile.Log(". use height");

					halfWidth = halfHeight * 2;
				}

				_overlay.HalfWidth  = halfWidth; // set half-width/height for the Overlay.
				_overlay.HalfHeight = halfHeight;


				_overlay.Origin = new Point(
										OffsetX + (_file.Rows - 1) * halfWidth,
										OffsetY);

				int width  = (_file.Rows + _file.Cols) * halfWidth;
				int height = (_file.Rows + _file.Cols) * halfHeight
						   +  _file.Levs * halfHeight * 3;

				//DSShared.Logfile.Log(". width= " + width);
				//DSShared.Logfile.Log(". height= " + height);

				Globals.Scale = (float)halfWidth / MainViewOverlay.HalfWidthConst;
				MainViewF.that.sb_PrintScale();
				//DSShared.Logfile.Log(". set scale= " + Globals.Scale);

				return new Size(
							OffsetX * 2 + width,  // + _scrollBarV.Width,
							OffsetY * 2 + height);// + _scrollBarH.Height);
			}

			//DSShared.Logfile.Log(". RET size empty.");
			return Size.Empty;
		}
		#endregion Methods


		#region Timer stuff for animations (static)
		internal delegate void PhaseEventHandler();
		internal static PhaseEventHandler PhaseEvent;	// NOTE: 'PhaseEvent' uses the delegate directly;
														// it is not an event per se.
		private  static Timer _t1;
		internal static int Phase; // the current animation phase [0..7] that deters which tilepart-sprite to draw

		internal static void Animate(bool animate)
		{
			if (animate)
			{
				if (_t1 == null)
				{
					_t1 = new Timer();
					_t1.Interval = Globals.PERIOD;
					_t1.Tick += Phaser;
				}

				if (!_t1.Enabled)
					_t1.Start();
			}
			else if (_t1 != null)
			{
				_t1.Stop();
				Phase = 0;
			}
		}

		/// <summary>
		/// Advances to the next sprite-frame and raises AnimationUpdate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Phaser(object sender, EventArgs e)
		{
			Phase = ++Phase % 8;
			PhaseEvent();
		}
		#endregion Timer stuff for animations (static)
	}
}
