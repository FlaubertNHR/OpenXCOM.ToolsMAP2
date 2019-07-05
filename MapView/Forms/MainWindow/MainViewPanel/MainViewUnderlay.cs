using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed class MainViewUnderlay
		:
			Panel // god I hate these double-panels!!!! cf. MainViewOverlay
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
		private MainViewOverlay MainViewOverlay
		{ get; set; }

		private MapFileBase _mapBase;
		internal MapFileBase MapBase
		{
			get { return _mapBase; }
			set
			{
				MainViewOverlay.MapBase = value;

				if (_mapBase != null)
				{
					_mapBase.SelectLocationEvent -= MainViewOverlay.OnSelectLocationMain;
					_mapBase.SelectLevelEvent    -= MainViewOverlay.OnSelectLevelMain;
				}

				if ((_mapBase = value) != null)
				{
					_mapBase.SelectLocationEvent += MainViewOverlay.OnSelectLocationMain;
					_mapBase.SelectLevelEvent    += MainViewOverlay.OnSelectLevelMain;

					SetOverlaySize();
				}

				OnResize(EventArgs.Empty);
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal MainViewUnderlay()
		{
			that = this;
			MainViewOverlay = new MainViewOverlay();

			AnimationUpdateEvent += OnAnimationUpdate;

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


//			XCom.LogFile.WriteLine("");
//			XCom.LogFile.WriteLine("MainViewUnderlay cTor");
//			XCom.LogFile.WriteLine(". underlay.Width= " + Width);
//			XCom.LogFile.WriteLine(". underlayHeight= " + Height);
//
//			XCom.LogFile.WriteLine(". underlay client.Width= " + ClientSize.Width);
//			XCom.LogFile.WriteLine(". underlay client.Height= " + ClientSize.Height);
//
//			XCom.LogFile.WriteLine(". overlay.Width= " + _mainViewOverlay.Width);
//			XCom.LogFile.WriteLine(". overlay.Height= " + _mainViewOverlay.Height);
//
//			XCom.LogFile.WriteLine(". overlay client.Width= " + _mainViewOverlay.ClientSize.Width);
//			XCom.LogFile.WriteLine(". overlay client.Height= " + _mainViewOverlay.ClientSize.Height);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

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
		/// XCMainWindow to place a call or two.
		/// </summary>
		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
//			XCom.LogFile.WriteLine("");
//			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize");
//
//			XCom.LogFile.WriteLine("underlay.Width= " + Width);
//			XCom.LogFile.WriteLine("underlay.Height= " + Height);
//
//			XCom.LogFile.WriteLine("underlay client.Width= " + ClientSize.Width);
//			XCom.LogFile.WriteLine("underlay client.Height= " + ClientSize.Height);
//
//			XCom.LogFile.WriteLine("overlay.Width= " + MainViewOverlay.Width);
//			XCom.LogFile.WriteLine("overlay.Height= " + MainViewOverlay.Height);
//
//			XCom.LogFile.WriteLine("overlay client.Width= " + MainViewOverlay.ClientSize.Width);
//			XCom.LogFile.WriteLine("overlay client.Height= " + MainViewOverlay.ClientSize.Height);


			base.OnResize(eventargs);

			if (MapBase != null && Globals.AutoScale)
			{
				SetScale();
				SetOverlaySize();
			}
			UpdateScrollers();

			Invalidate(); // updates the reserved scroll indicators.

//			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize EXIT");
		}
		#endregion Events (override)


		#region Events
		private void OnScrollVert(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Left= " + MainViewOverlay.Left);
			MainViewOverlay.Location = new Point(
											MainViewOverlay.Left,
											-_scrollBarV.Value);
			MainViewOverlay.Invalidate();
		}

		private void OnScrollHori(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Top= " + MainViewOverlay.Top);
			MainViewOverlay.Location = new Point(
											-_scrollBarH.Value,
											MainViewOverlay.Top);
			MainViewOverlay.Invalidate();
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			MainViewOverlay.Invalidate();
		}


		// The following functs are for subscription to toolstrip Editor buttons.
		internal void OnCut(object sender, EventArgs e)
		{
			MainViewOverlay.Copy();
			MainViewOverlay.ClearSelection();
		}
		internal void OnCopy(object sender, EventArgs e)
		{
			MainViewOverlay.Copy();
		}
		internal void OnPaste(object sender, EventArgs e)
		{
			MainViewOverlay.Paste();
		}
		internal void OnDelete(object sender, EventArgs e)
		{
			MainViewOverlay.ClearSelection();
		}
		internal void OnFill(object sender, EventArgs e)
		{
			MainViewOverlay.FillSelectedTiles();
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
//		/// XCMainWindow.OnResize(). Note that this workaround pertains only to
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
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MainViewUnderlay.SetScale");

			var required = GetRequiredOverlaySize(1.0);
			Globals.Scale = Math.Min(
								(double)(Width  - OffsetX) / required.Width,
								(double)(Height - OffsetY) / required.Height);
			Globals.Scale = Globals.Scale.Clamp(
											Globals.ScaleMinimum,
											Globals.ScaleMaximum);

			//XCom.LogFile.WriteLine(". scale set to= " + Globals.Scale);
		}

		/// <summary>
		/// Sets this Panel to the size of the current Map including scaling.
		/// </summary>
		internal void SetOverlaySize()
		{
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MainViewUnderlay.SetOverlaySize");

			if (MapBase != null)
			{
				//XCom.LogFile.WriteLine(". scale= " + Globals.Scale);
				var required = GetRequiredOverlaySize(Globals.Scale);

				MainViewOverlay.Width  = required.Width;
				MainViewOverlay.Height = required.Height;

				//XCom.LogFile.WriteLine(". set overlay.Width= " + MainViewOverlay.Width);
				//XCom.LogFile.WriteLine(". set overlay.Height= " + MainViewOverlay.Height);
			}
		}

		/// <summary>
		/// Gets the required x/y size in pixels for the current MapBase as a
		/// lozenge. Also sets the 'Origin' point and the half-width/height vals.
		/// </summary>
		/// <param name="scale">the current scaling factor</param>
		/// <returns></returns>
		private Size GetRequiredOverlaySize(double scale)
		{
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MainViewUnderlay.GetRequiredOverlaySize");

			if (MapBase != null)
			{
				//XCom.LogFile.WriteLine(". scale= " + Globals.Scale);

				int halfWidth  = (int)(MainViewOverlay.HalfWidthConst  * scale);
				int halfHeight = (int)(MainViewOverlay.HalfHeightConst * scale);

				if (halfHeight > halfWidth / 2) // use width
				{
					//XCom.LogFile.WriteLine(". use width");

					if (halfWidth % 2 != 0)
						--halfWidth;

					halfHeight = halfWidth / 2;
				}
				else // use height
				{
					//XCom.LogFile.WriteLine(". use height");

					halfWidth = halfHeight * 2;
				}

				MainViewOverlay.HalfWidth  = halfWidth; // set half-width/height for the Overlay.
				MainViewOverlay.HalfHeight = halfHeight;


				MainViewOverlay.Origin = new Point(
												OffsetX + (MapBase.MapSize.Rows - 1) * halfWidth,
												OffsetY);

				int width  = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * halfWidth;
				int height = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * halfHeight
						   +  MapBase.MapSize.Levs * halfHeight * 3;

				//XCom.LogFile.WriteLine(". width= " + width);
				//XCom.LogFile.WriteLine(". height= " + height);

				Globals.Scale = (double)halfWidth / MainViewOverlay.HalfWidthConst;
				XCMainWindow.that.sb_PrintScale();
				//XCom.LogFile.WriteLine(". set scale= " + Globals.Scale);

				return new Size(
							OffsetX * 2 + width,//  + _scrollBarV.Width,
							OffsetY * 2 + height);// + _scrollBarH.Height);
			}

			//XCom.LogFile.WriteLine(". RET size empty.");
			return Size.Empty;
		}
		#endregion Methods


		#region Timer stuff for animations (static)
		internal static event EventHandler AnimationUpdateEvent;

		private static Timer _timer;
		private static int _anistep;

		// NOTE: Remove suppression for Release cfg. .. not workie.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility",
		"CA1601:DoNotUseTimersThatPreventPowerStateChanges",
		Justification = "Because animations at or greater than 1 second ain't gonna cut it.")]
		internal static void Animate(bool animate)
		{
			if (animate)
			{
				if (_timer == null)
				{
					_timer = new Timer();
					_timer.Interval = 250;
					_timer.Tick += AnimateStep;
				}

				if (!_timer.Enabled)
					_timer.Start();
			}
			else if (_timer != null)
			{
				_timer.Stop();
				_anistep = 0;
			}
		}

		/// <summary>
		/// Advances to the next sprite-frame and raises AnimationUpdateEvent.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void AnimateStep(object sender, EventArgs e)
		{
			_anistep = ++_anistep % 8;

			if (AnimationUpdateEvent != null)
				AnimationUpdateEvent(null, EventArgs.Empty);
		}

		/// <summary>
		/// Checks if the sprites are currently animating.
		/// </summary>
		internal static bool IsAnimated
		{
			get { return (_timer != null && _timer.Enabled); }
		}

		/// <summary>
		/// Gets which sequential frame [0..7] of the sprite to display.
		/// </summary>
		internal static int AniStep
		{
			get { return _anistep; }
		}
		#endregion Timer stuff for animations (static)
	}
}
