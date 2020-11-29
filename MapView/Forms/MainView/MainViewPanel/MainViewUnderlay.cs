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
		#endregion Fields


		#region Properties (static)
		internal static MainViewUnderlay that
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
		private MainViewF MainView;

		private MainViewOverlay MainViewOverlay
		{ get; set; }

		private MapFile _file;
		internal MapFile MapFile
		{
			get { return _file; }
			set
			{
				MainViewOverlay.MapFile = value;

				if (_file != null)
				{
					_file.SelectLocation -= MainViewOverlay.OnSelectLocationMain;
					_file.SelectLevel    -= MainViewOverlay.OnSelectLevelMain;
				}

				if ((_file = value) != null)
				{
					_file.SelectLocation += MainViewOverlay.OnSelectLocationMain;
					_file.SelectLevel    += MainViewOverlay.OnSelectLevelMain;

					SetOverlaySize();
				}

				OnResize(EventArgs.Empty);
			}
		}


		internal bool IsVertbarVisible
		{ get { return _scrollBarV.Visible; } }

		internal bool IsHoribarVisible
		{ get { return _scrollBarH.Visible; } }

		internal int WidthVertbar
		{ get { return _scrollBarV.Width; } }

		internal int HeightHoribar
		{ get { return _scrollBarH.Height; } }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal MainViewUnderlay(MainViewF main)
		{
			MainView = main;

			that = this;
			MainViewOverlay = new MainViewOverlay(MainView);

			Dock = DockStyle.Fill;
			BorderStyle = BorderStyle.Fixed3D;

			AnimationUpdate += OnAnimationUpdate;

			_scrollBarV.Dock = DockStyle.Right;
			_scrollBarV.Scroll += OnScrollVert;

			_scrollBarH.Dock = DockStyle.Bottom;
			_scrollBarH.Scroll += OnScrollHori;

			Controls.AddRange(new Control[]
			{
				_scrollBarV,
				_scrollBarH,
				MainViewOverlay
			});


//			DSShared.LogFile.WriteLine("");
//			DSShared.LogFile.WriteLine("MainViewUnderlay cTor");
//			DSShared.LogFile.WriteLine(". underlay.Width= " + Width);
//			DSShared.LogFile.WriteLine(". underlay.Height= " + Height);
//
//			DSShared.LogFile.WriteLine(". underlay client.Width= " + ClientSize.Width);
//			DSShared.LogFile.WriteLine(". underlay client.Height= " + ClientSize.Height);
//
//			DSShared.LogFile.WriteLine(". overlay.Width= " + _mainViewOverlay.Width);
//			DSShared.LogFile.WriteLine(". overlay.Height= " + _mainViewOverlay.Height);
//
//			DSShared.LogFile.WriteLine(". overlay client.Width= " + _mainViewOverlay.ClientSize.Width);
//			DSShared.LogFile.WriteLine(". overlay client.Height= " + _mainViewOverlay.ClientSize.Height);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			// indicate reserved space for scroll-bars.
			var graphics = e.Graphics;
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
		/// Forces an OnResize event for this Panel. Grants access for
		/// MainViewF.OnMapResizeClick().
		/// </summary>
		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
//			DSShared.LogFile.WriteLine("");
//			DSShared.LogFile.WriteLine("MainViewUnderlay.OnResize");
//
//			DSShared.LogFile.WriteLine("underlay.Width= " + Width);
//			DSShared.LogFile.WriteLine("underlay.Height= " + Height);
//
//			DSShared.LogFile.WriteLine("underlay client.Width= " + ClientSize.Width);
//			DSShared.LogFile.WriteLine("underlay client.Height= " + ClientSize.Height);
//
//			DSShared.LogFile.WriteLine("overlay.Width= " + MainViewOverlay.Width);
//			DSShared.LogFile.WriteLine("overlay.Height= " + MainViewOverlay.Height);
//
//			DSShared.LogFile.WriteLine("overlay client.Width= " + MainViewOverlay.ClientSize.Width);
//			DSShared.LogFile.WriteLine("overlay client.Height= " + MainViewOverlay.ClientSize.Height);


			base.OnResize(eventargs);

			if (MapFile != null && Globals.AutoScale)
			{
				SetScale();
				SetOverlaySize();
			}
			UpdateScrollers();

			Invalidate(); // updates the reserved scroll indicators.

//			DSShared.LogFile.WriteLine("MainViewUnderlay.OnResize EXIT");
		}
		#endregion Events (override)


		#region Events
		private void OnScrollVert(object sender, ScrollEventArgs e)
		{
			//DSShared.LogFile.WriteLine("OnVerticalScroll overlay.Left= " + MainViewOverlay.Left);
			MainViewOverlay.Location = new Point(
											MainViewOverlay.Left,
											-_scrollBarV.Value);
			MainViewOverlay.Invalidate();
		}

		private void OnScrollHori(object sender, ScrollEventArgs e)
		{
			//DSShared.LogFile.WriteLine("OnVerticalScroll overlay.Top= " + MainViewOverlay.Top);
			MainViewOverlay.Location = new Point(
											-_scrollBarH.Value,
											MainViewOverlay.Top);
			MainViewOverlay.Invalidate();
		}

		private void OnAnimationUpdate()
		{
			MainViewOverlay.Invalidate();
		}
		#endregion Events


		#region Methods
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
//			MainViewOverlay.Location = new Point(0, 0);
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

				MainViewOverlay.Location = new Point(0,0);
			}
			else
			{
				// TODO: scrollbars jiggery-pokery needed.
				_scrollBarV.Visible = (MainViewOverlay.Height > ClientSize.Height + OffsetY);
				if (_scrollBarV.Visible)
				{
					_scrollBarV.Maximum = Math.Max(
												MainViewOverlay.Height
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
					MainViewOverlay.Location = new Point(Left, 0);
				}

				_scrollBarH.Visible = (MainViewOverlay.Width > ClientSize.Width + OffsetX);
				if (_scrollBarH.Visible)
				{
					_scrollBarH.Maximum = Math.Max(
												MainViewOverlay.Width
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
					MainViewOverlay.Location = new Point(0, Top);
				}
			}

			MainViewOverlay.Refresh();
		}

		/// <summary>
		/// Sets the scale-factor. Is used only if AutoScale=TRUE.
		/// </summary>
		internal void SetScale()
		{
			//DSShared.LogFile.WriteLine("");
			//DSShared.LogFile.WriteLine("MainViewUnderlay.SetScale");

			var required = GetRequiredOverlaySize(1.0);
			Globals.Scale = Math.Min(
								(double)(Width  - OffsetX) / required.Width,
								(double)(Height - OffsetY) / required.Height);
			Globals.Scale = Globals.Scale.Viceroy(
												Globals.ScaleMinimum,
												Globals.ScaleMaximum);

			//DSShared.LogFile.WriteLine(". scale set to= " + Globals.Scale);
		}

		/// <summary>
		/// Sets this Panel to the size of the current Map including scaling.
		/// </summary>
		internal void SetOverlaySize()
		{
			//DSShared.LogFile.WriteLine("");
			//DSShared.LogFile.WriteLine("MainViewUnderlay.SetOverlaySize");

			if (MapFile != null)
			{
				//DSShared.LogFile.WriteLine(". scale= " + Globals.Scale);
				var required = GetRequiredOverlaySize(Globals.Scale);

				MainViewOverlay.Width  = required.Width;
				MainViewOverlay.Height = required.Height;

				//DSShared.LogFile.WriteLine(". set overlay.Width= " + MainViewOverlay.Width);
				//DSShared.LogFile.WriteLine(". set overlay.Height= " + MainViewOverlay.Height);
			}
		}

		/// <summary>
		/// Gets the required x/y size in pixels for the current MapFile as a
		/// lozenge. Also sets the 'Origin' point and the half-width/height vals.
		/// </summary>
		/// <param name="scale">the current scaling factor</param>
		/// <returns></returns>
		private Size GetRequiredOverlaySize(double scale)
		{
			//DSShared.LogFile.WriteLine("");
			//DSShared.LogFile.WriteLine("MainViewUnderlay.GetRequiredOverlaySize");

			if (MapFile != null)
			{
				//DSShared.LogFile.WriteLine(". scale= " + Globals.Scale);

				int halfWidth  = (int)(MainViewOverlay.HalfWidthConst  * scale);
				int halfHeight = (int)(MainViewOverlay.HalfHeightConst * scale);

				if (halfHeight > halfWidth / 2) // use width
				{
					//DSShared.LogFile.WriteLine(". use width");

					if (halfWidth % 2 != 0)
						--halfWidth;

					halfHeight = halfWidth / 2;
				}
				else // use height
				{
					//DSShared.LogFile.WriteLine(". use height");

					halfWidth = halfHeight * 2;
				}

				MainViewOverlay.HalfWidth  = halfWidth; // set half-width/height for the Overlay.
				MainViewOverlay.HalfHeight = halfHeight;


				MainViewOverlay.Origin = new Point(
												OffsetX + (MapFile.MapSize.Rows - 1) * halfWidth,
												OffsetY);

				int width  = (MapFile.MapSize.Rows + MapFile.MapSize.Cols) * halfWidth;
				int height = (MapFile.MapSize.Rows + MapFile.MapSize.Cols) * halfHeight
						   +  MapFile.MapSize.Levs * halfHeight * 3;

				//DSShared.LogFile.WriteLine(". width= " + width);
				//DSShared.LogFile.WriteLine(". height= " + height);

				Globals.Scale = (double)halfWidth / MainViewOverlay.HalfWidthConst;
				MainView.sb_PrintScale();
				//DSShared.LogFile.WriteLine(". set scale= " + Globals.Scale);

				return new Size(
							OffsetX * 2 + width,  // + _scrollBarV.Width,
							OffsetY * 2 + height);// + _scrollBarH.Height);
			}

			//DSShared.LogFile.WriteLine(". RET size empty.");
			return Size.Empty;
		}
		#endregion Methods


		#region Timer stuff for animations (static)
		internal delegate void AnimationEventHandler();
		internal static AnimationEventHandler AnimationUpdate;	// NOTE: 'AnimationUpdate' uses the delegate directly;
																// it is not an event per se.
		private static Timer _t1;
		private static int _anistep;

		internal static void Animate(bool animate)
		{
			if (animate)
			{
				if (_t1 == null)
				{
					_t1 = new Timer();
					_t1.Interval = Globals.PERIOD;
					_t1.Tick += AnimateStep;
				}

				if (!_t1.Enabled)
					_t1.Start();
			}
			else if (_t1 != null)
			{
				_t1.Stop();
				_anistep = 0;
			}
		}

		/// <summary>
		/// Advances to the next sprite-frame and raises AnimationUpdate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void AnimateStep(object sender, EventArgs e)
		{
			_anistep = ++_anistep % 8;
			AnimationUpdate();
		}

		/// <summary>
		/// Gets which phase [0..7] of the sprite to display.
		/// </summary>
		internal static int AniStep
		{
			get { return _anistep; }
		}
		#endregion Timer stuff for animations (static)
	}
}
