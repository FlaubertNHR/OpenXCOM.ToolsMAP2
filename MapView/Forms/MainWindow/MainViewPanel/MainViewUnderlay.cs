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
		#region Fields & Properties
		private static MainViewUnderlay _instance;
		internal static MainViewUnderlay Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MainViewUnderlay();

				return _instance;
			}
		}

		private MainViewOverlay _mainViewOverlay;
		internal MainViewOverlay MainViewOverlay
		{
			get { return _mainViewOverlay; }
		}

		private XCMapBase _mapBase;
		internal XCMapBase MapBase
		{
			get { return _mapBase; }
			set
			{
				//XCom.LogFile.WriteLine("MainViewUnderlay.MapBase set");

				MainViewOverlay.MapBase = value;

				if (_mapBase != null)
				{
					_mapBase.LocationSelectedEvent -= MainViewOverlay.OnLocationSelected_Main;	// WARNING: if the overlay ever gets removed from the Control
					_mapBase.LevelChangedEvent     -= MainViewOverlay.OnLevelChanged_Main;		// by the code in the cTor, this will likely go defunct. Or not.
				}

				if ((_mapBase = value) != null)
				{
					_mapBase.LocationSelectedEvent += MainViewOverlay.OnLocationSelected_Main;
					_mapBase.LevelChangedEvent     += MainViewOverlay.OnLevelChanged_Main;

					SetOverlaySize();

//					DragStart = _dragStart;	// this might be how to give drags their legitimate
//					DragEnd   = _dragEnd;	// values after initialization to Point(-1/-1).
				}

				OnResize(EventArgs.Empty);
			}
		}


		private int _halfWidth;
		internal int HalfWidth
		{
			set
			{
				_halfWidth                =
				MainViewOverlay.HalfWidth = value; // pass it on to Overlay.
			}
		}

		private int _halfHeight;
		internal int HalfHeight
		{
			set
			{
				_halfHeight                =
				MainViewOverlay.HalfHeight = value; // pass it on to Overlay.
			}
		}

		private const int OffsetX = 2;	// these track the offset between the panel border
		private const int OffsetY = 2;	// and the lozenge-tips. NOTE: they are used for
										// both the underlay and the overlay, which currently
										// have the same border sizes; if one or the other
										// changes then their offsets would have to be separated.

		private readonly HScrollBar _scrollBarH;
		private readonly VScrollBar _scrollBarV;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		private MainViewUnderlay()
		{
			AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarV = new VScrollBar();
			_scrollBarV.Dock = DockStyle.Right;
			_scrollBarV.Scroll += OnScrollVert;

			_scrollBarH = new HScrollBar();
			_scrollBarH.Dock = DockStyle.Bottom;
			_scrollBarH.Scroll += OnScrollHori;

//			var mainViewOverlay = new MainViewOverlay(); // what's this for.
//			if (_mainViewOverlay != null)
//			{
//				mainViewOverlay.MapBase = _mainViewOverlay.MapBase;
//				Controls.Remove(_mainViewOverlay);
//			}
//			_mainViewOverlay = mainViewOverlay;

			_mainViewOverlay = new MainViewOverlay();
			_mainViewOverlay.SetMainViewUnderlay(this);

			Controls.AddRange(new Control[]
			{
				_scrollBarV,
				_scrollBarH,
				_mainViewOverlay
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
		#endregion


		#region EventCalls
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// indicate reserved space for scroll-bars.
			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
//			graphics.SmoothingMode = SmoothingMode.HighQuality;

			var pen = new Pen(SystemColors.ControlLight, 1);
			graphics.DrawLine(
							pen,
							Width - _scrollBarV.Width - OffsetX - 1, OffsetY,
							Width - _scrollBarV.Width - OffsetX - 1, Height - _scrollBarH.Height - OffsetY - 1);
			graphics.DrawLine(
							pen,
							OffsetX,                                 Height - _scrollBarH.Height - OffsetY - 1,
							Width - _scrollBarV.Width - OffsetX - 1, Height - _scrollBarH.Height - OffsetY - 1);
		}

		/// <summary>
		/// Forces an OnResize event for this Panel. Grants access for
		/// XCMainWindow to place a call or two.
		/// </summary>
		internal void ResizeUnderlay()
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

			if (Globals.AutoScale)
			{
				SetScale();
				SetOverlaySize();
			}
			UpdateScrollers();

			Refresh(); // updates the reserved scroll indicators.

//			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize EXIT");
		}

/*		/// <summary>
		/// A workaround for maximizing the parent-form. See notes at
		/// XCMainWindow.OnResize(). Note that this workaround pertains only to
		/// cases when AutoScale=FALSE.
		/// </summary>
		internal void ResetScrollers()
		{
			// NOTE: if the form is enlarged with scrollbars visible and the
			// new size doesn't need scrollbars but the map was offset, the
			// scrollbars disappear but the map is still offset. So fix it.
			//
			// TODO: this is a workaround.
			// It simply relocates the overlay to the origin, but it should try
			// to maintain focus on a selected tile for cases when the form is
			// enlarged *and the overlay still needs* one of the scrollbars.

			_scrollBarV.Value =
			_scrollBarH.Value = 0;

			MainViewOverlay.Location = new Point(0, 0);
		} */

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

				MainViewOverlay.Location = new Point(0, 0);
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

			var required = GetRequiredOverlaySize(1);
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

				_halfWidth  = (int)(MainViewOverlay.HalfWidthConst  * scale);
				_halfHeight = (int)(MainViewOverlay.HalfHeightConst * scale);

				if (_halfHeight > _halfWidth / 2) // use width
				{
					//XCom.LogFile.WriteLine(". use width");

					if (_halfWidth % 2 != 0)
						--_halfWidth;

					_halfHeight = _halfWidth / 2;
				}
				else // use height
				{
					//XCom.LogFile.WriteLine(". use height");

					_halfWidth = _halfHeight * 2;
				}

				HalfWidth  = _halfWidth; // set half-width/height for the Overlay.
				HalfHeight = _halfHeight;


				MainViewOverlay.Origin = new Point(
												OffsetX + (MapBase.MapSize.Rows - 1) * _halfWidth,
												OffsetY);

				int width  = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * _halfWidth;
				int height = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * _halfHeight
						   +  MapBase.MapSize.Levs * _halfHeight * 3;

				//XCom.LogFile.WriteLine(". width= " + width);
				//XCom.LogFile.WriteLine(". height= " + height);

				Globals.Scale = (double)_halfWidth / MainViewOverlay.HalfWidthConst;
				XCMainWindow.Instance.StatusBarPrintScale();
				//XCom.LogFile.WriteLine(". set scale= " + Globals.Scale);

				return new Size(
							OffsetX * 2 + width,//  + _scrollBarV.Width,
							OffsetY * 2 + height);// + _scrollBarH.Height);
			}

			//XCom.LogFile.WriteLine(". RET size empty.");
			return Size.Empty;
		}


		private void OnScrollVert(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Left= " + MainViewOverlay.Left);
			MainViewOverlay.Location = new Point(
												MainViewOverlay.Left,
												-_scrollBarV.Value);
			MainViewOverlay.Refresh();
		}

		private void OnScrollHori(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Top= " + MainViewOverlay.Top);
			MainViewOverlay.Location = new Point(
												-_scrollBarH.Value,
												MainViewOverlay.Top);
			MainViewOverlay.Refresh();
		}


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

		internal void OnFillSelectedTiles(object sender, EventArgs e)
		{
			MainViewOverlay.FillSelectedTiles();
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			MainViewOverlay.Refresh();
		}
		#endregion


		#region Timer stuff for Animations
		internal static event EventHandler AnimationUpdateEvent;

		private static Timer _timer;
		private static int _anistep;

		// NOTE: Remove suppression for Release cfg.
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

		private static void AnimateStep(object sender, EventArgs e)
		{
			_anistep = ++_anistep % 8;

			if (AnimationUpdateEvent != null)
				AnimationUpdateEvent(null, null);
		}

		internal static bool IsAnimated
		{
			get { return (_timer != null && _timer.Enabled); }
		}

		internal static int AniStep
		{
			get { return _anistep; }
		}
		#endregion
	}
}
