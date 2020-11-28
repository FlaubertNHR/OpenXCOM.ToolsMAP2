using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// These are not actually "quadrants"; they are tile-part types. But that's
	/// the way this trolls.
	/// @note This is not a Panel. It is a Control.
	/// </summary>
	internal sealed class QuadrantPanel
		:
			MapObserverControl_Top // DoubleBufferedControl, IMapObserver
	{
		#region Fields (static)
		/// <summary>
		/// A timer that delays processing clicks until the user's double-click
		/// duration has elapsed. That is, don't do 1-click RMB processing if
		/// 2-clicks are inc.
		/// w/ Thanks to Natxo
		/// https://stackoverflow.com/questions/2086213/how-can-i-catch-both-single-click-and-double-click-events-on-wpf-frameworkelement/2087517#2087517
		/// </summary>
		private static readonly System.Timers.Timer _t1 = new System.Timers.Timer(SystemInformation.DoubleClickTime);

		private static int _t1Clicks;
		private static bool _t1subscribed;
		#endregion Fields (static)


		// TODO: Figure out what else can be made static here ->

		#region Properties
		private QuadrantType _quadrant;
		internal QuadrantType SelectedQuadrant
		{
			get { return _quadrant; }
			set { _quadrant = value; Refresh(); }
		}

		internal MapTile Tile
		{ private get; set; }

		internal MapLocation SelectedLocation
		{ private get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. There are 2 quadpanels: one in TopView and another in
		/// TopRouteView(Top).
		/// TODO: This should be a static class.
		/// </summary>
		internal QuadrantPanel()
		{
			MainViewUnderlay.AnimationUpdate += OnAnimationUpdate;

			if (!_t1subscribed) // only once (for both QuadrantPanels)
			{
				_t1subscribed = true;
				_t1.Elapsed += OnClicksElapsed;
			}
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl_Top.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			Tile             = args.Tile;
			SelectedLocation = args.Location;

			Refresh();
		}

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl_Top.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			if (SelectedLocation != null)
			{
				Tile = MapFile[SelectedLocation.Col, SelectedLocation.Row];
				SelectedLocation.Lev = args.Level;
			}
			Refresh();
		}


		/// <summary>
		/// For use by keyboard-input.
		/// </summary>
		private QuadrantType _quad = QuadrantType.None;

		/// <summary>
		/// Wrapper for OnMouseDown() for use by keyboard-input only.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="quad"></param>
		internal void doMouseDown(MouseEventArgs e, QuadrantType quad)
		{
			if (quad != QuadrantType.None)
				_quad = quad;
			else
				_quad = SelectedQuadrant;

			OnMouseDown(e);
		}

		/// <summary>
		///  Handles mousedown events on this QuadrantPanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			//DSShared.LogFile.WriteLine("QuadrantPanel.OnMouseDown()");

			ObserverManager.TopView     .Control   .TopPanel.Select();
			ObserverManager.TopRouteView.ControlTop.TopPanel.Select();

			bool keySelectQuadrant = _quad !=  QuadrantType.None
								  && _quad != (QuadrantType)QuadrantDrawService.QuadrantPart;

			if (!keySelectQuadrant)
			{
				int x = (e.X - QuadrantDrawService.StartX);
				if (x > -1 && x % QuadrantDrawService.Quadwidth < XCImage.SpriteWidth32) // ignore spaces between sprites
					_quad = (QuadrantType)(x / QuadrantDrawService.Quadwidth);
			}

			bool isCurrentClick = false;

			PartType part = PartType.All;
			switch (_quad)
			{
				case QuadrantType.Floor:   part = PartType.Floor;   break;
				case QuadrantType.West:    part = PartType.West;    break;
				case QuadrantType.North:   part = PartType.North;   break;
				case QuadrantType.Content: part = PartType.Content; break;

				case (QuadrantType)QuadrantDrawService.QuadrantPart:
					isCurrentClick = true;
					if (QuadrantDrawService.CurrentTilepart != null)
						part = QuadrantDrawService.CurrentTilepart.Record.PartType;
					break;
			}

			if (part != PartType.All)
			{
				ObserverManager.TopView     .Control   .SelectQuadrant(part);
				ObserverManager.TopRouteView.ControlTop.SelectQuadrant(part);

				if (!isCurrentClick)
					Clicker(e.Button, e.Clicks, keySelectQuadrant);
			}
			_quad = QuadrantType.None;
		}

		/// <summary>
		/// Handles the details of LMB and RMB wrt the QuadrantPanels.
		/// TODO: GENERAL - Bypass operations (and the MapChanged flag)
		///       if user does an operation that results in identical state.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="clicks"></param>
		/// <param name="keySelectQuadrant">true if invoked by keyboard-input</param>
		internal void Clicker(MouseButtons button, int clicks, bool keySelectQuadrant = false)
		{
			if (Tile != null)
			{
				switch (button)
				{
					case MouseButtons.Left: // NOTE: clicks=1 is handled by caller.
						if (clicks == 2)
							ObserverManager.TileView.Control.SelectedTilepart = Tile[SelectedQuadrant];
						break;

					case MouseButtons.Right:
						if (MainViewOverlay.that.FirstClick) // do not set a part in a quad unless a tile is selected.
						{
							//DSShared.LogFile.WriteLine("QuadrantPanel.Clicker()");

							if (keySelectQuadrant || !TopView.Optionables.EnableRightClickWaitTimer)
							{
								_t1Clicks = clicks;
								OnClicksElapsed(null,null);
							}
							else
							{
								_t1.Stop();
								++_t1Clicks;
								_t1.Start();
							}
						}
						break;
				}
			}
		}


		/// <summary>
		/// Overrides DoubleBufferedControl.RenderGraphics() - ie, OnPaint().
		/// @note Calls the draw-function in QuadrantDrawService.
		/// </summary>
		/// <param name="graphics"></param>
		protected override void RenderGraphics(Graphics graphics)
		{
			QuadrantDrawService.SetGraphics(graphics);
			QuadrantDrawService.Draw(Tile, SelectedQuadrant);

			if (SelectedLocation != null)
				QuadrantDrawService.PrintSelectedLocation(SelectedLocation, Width);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Clever handling of RMB double-click event ...
		/// WARNING: The interaction between this QuadrantPanel, its respective
		/// TopPanel, and the TilePanel in TileView is a little bit fragile.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void OnClicksElapsed(object source, ElapsedEventArgs e)
		{
			//DSShared.LogFile.WriteLine("QuadrantPanel.OnClicksElapsed() _t1Clicks= " + _t1Clicks);

			_t1.Stop();

			if (_t1Clicks == 1)
				MainViewOverlay.that.FillSelectedQuads();
			else // 2+ clicks
				MainViewOverlay.that.ClearSelectedQuads();

			_t1Clicks = 0;
		}

		private void OnAnimationUpdate()
		{
			Invalidate();
		}
		#endregion Events
	}
}
