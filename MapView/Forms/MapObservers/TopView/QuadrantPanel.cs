using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces;


namespace MapView.Forms.MapObservers.TopViews
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
		#region Fields
		/// <summary>
		/// A timer that delays processing clicks until the user's double-click
		/// duration has elapsed. That is, don't do 1-click RMB processing if
		/// 2-clicks are inc.
		/// w/ Thanks to Natxo
		/// https://stackoverflow.com/questions/2086213/how-can-i-catch-both-single-click-and-double-click-events-on-wpf-frameworkelement/2087517#2087517
		/// </summary>
		private readonly System.Timers.Timer _t1;
		private int _t1Clicks;
		#endregion Fields


		#region Properties
		private QuadrantType _quadrant;
		internal QuadrantType SelectedQuadrant
		{
			get { return _quadrant; }
			set { _quadrant = value; Refresh(); }
		}

		internal MapTile Tile
		{ private get; set; }

		internal MapLocation Loc
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

			_t1 = new System.Timers.Timer(SystemInformation.DoubleClickTime);
			_t1.Elapsed += OnClicksElapsed;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			Tile = args.Tile as MapTile;
			Loc  = args.Location;
			Refresh();
		}

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			if (Loc != null)
			{
				Tile = MapBase[Loc.Row, Loc.Col] as MapTile;
				Loc.Lev = args.Level;
			}
			Refresh();
		}


		/// <summary>
		/// For use by keyboard-input.
		/// </summary>
		private QuadrantType _quadtype = QuadrantType.None;

		/// <summary>
		/// Wrapper for OnMouseDown() for use by keyboard-input only.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="quadtype"></param>
		internal void doMouseDown(MouseEventArgs e, QuadrantType quadtype)
		{
			if (quadtype != QuadrantType.None)
				_quadtype = quadtype;
			else
				_quadtype = SelectedQuadrant;

			OnMouseDown(e);
		}

		/// <summary>
		///  Handles mousedown events on this QuadrantPanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			ViewerFormsManager.TopView     .Control   .TopPanel.Select();
			ViewerFormsManager.TopRouteView.ControlTop.TopPanel.Select();

			bool keyboardInput = (_quadtype != QuadrantType.None);

			if (!keyboardInput)
			{
				int x = (e.X - QuadrantDrawService.StartX);
				if (x > -1 && x % QuadrantDrawService.Quadwidth < XCImage.SpriteWidth32) // ignore spaces between sprites
					_quadtype = (QuadrantType)(x / QuadrantDrawService.Quadwidth);
			}

			bool isTypeCurrent = false;

			PartType parttype = PartType.All;
			switch (_quadtype)
			{
				case QuadrantType.Floor:   parttype = PartType.Floor;   break;
				case QuadrantType.West:    parttype = PartType.West;    break;
				case QuadrantType.North:   parttype = PartType.North;   break;
				case QuadrantType.Content: parttype = PartType.Content; break;

				case (QuadrantType)QuadrantDrawService.QuadrantTypeCurrent:
					isTypeCurrent = true;
					if (QuadrantDrawService.CurrentTilepart != null)
						parttype = QuadrantDrawService.CurrentTilepart.Record.PartType;
					break;
			}

			if (parttype != PartType.All)
			{
				ViewerFormsManager.TopView     .Control   .SelectQuadrant(parttype);
				ViewerFormsManager.TopRouteView.ControlTop.SelectQuadrant(parttype);

				if (!isTypeCurrent)
					Operate(e.Button, e.Clicks, keyboardInput);
			}
			_quadtype = QuadrantType.None;
		}

		/// <summary>
		/// Handles the details of LMB and RMB wrt the QuadrantPanels.
		/// TODO: GENERAL - Bypass operations (and the MapChanged flag)
		///       if user does an operation that results in identical state.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="clicks"></param>
		/// <param name= "keyboardInput"></param>
		internal void Operate(MouseButtons button, int clicks, bool keyboardInput = false)
		{
			if (Tile != null)
			{
				switch (button)
				{
					case MouseButtons.Left:
						// clicks=1 is done by caller.
						if (clicks == 2)
							ViewerFormsManager.TileView.Control.SelectedTilepart = Tile[SelectedQuadrant];
						break;

					case MouseButtons.Right:
						if (MainViewOverlay.that.FirstClick) // do not set a part in a quad unless a tile is selected.
						{
							if (!keyboardInput)
							{
								_t1.Stop();
								++_t1Clicks;
								_t1.Start();
							}
							else
							{
								_t1Clicks = clicks;
								OnClicksElapsed(null,null);
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
			QuadrantDrawService.Draw(graphics, Tile, SelectedQuadrant);
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
