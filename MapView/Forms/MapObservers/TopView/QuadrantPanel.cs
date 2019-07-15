using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// These are not actually "quadrants"; they are tile-part types. But that's
	/// the way this trolls.
	/// @note This is not a Panel. It is a Control.
	/// </summary>
	internal sealed class QuadrantPanel
		:
			MapObserverControl_TopPanel // DoubleBufferedControl, IMapObserver
	{
		#region Fields
		/// <summary>
		/// A timer that delays processing clicks until the user's double-click
		/// duration has elapsed. That is, don't do 1-click processing if
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
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

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
		private QuadrantType _keyQuadtype = QuadrantType.None;

		/// <summary>
		/// Wrapper for OnMouseDown() for use by keyboard-input.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="quadType"></param>
		internal void ForceMouseDown(MouseEventArgs e, QuadrantType quadType)
		{
			_keyQuadtype = quadType;
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

			QuadrantType quadType;
			if (_keyQuadtype == QuadrantType.None) // ie. is *not* keyboard-input
			{
				quadType = (QuadrantType)((e.X - QuadrantDrawService.StartX)
											   / QuadrantDrawService.Quadwidth);
			}
			else // is keyboard-input
				quadType = _keyQuadtype;


			PartType partType = PartType.All;
			switch (quadType)
			{
				case QuadrantType.Floor:   partType = PartType.Floor;   break;
				case QuadrantType.West:    partType = PartType.West;    break;
				case QuadrantType.North:   partType = PartType.North;   break;
				case QuadrantType.Content: partType = PartType.Content; break;
			}

			if (partType != PartType.All)
			{
				ViewerFormsManager.TopView     .Control   .SelectQuadrant(partType);
				ViewerFormsManager.TopRouteView.ControlTop.SelectQuadrant(partType);

				SetSelected(e.Button, e.Clicks);
			}
			_keyQuadtype = QuadrantType.None;
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
		private void OnAnimationUpdate()
		{
			Invalidate();
		}

		private void OnClicksElapsed(object source, ElapsedEventArgs e)
		{
			_t1.Stop();
			switch (_t1Clicks)
			{
				case 1: MainViewOverlay.that.FillSelectedQuads();  break;
				case 2: MainViewOverlay.that.ClearSelectedQuads(); break;
			}
			_t1Clicks = 0;
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Handles the details of LMB and RMB wrt the QuadrantPanels.
		/// TODO: GENERAL - Bypass operations (and the MapChanged flag)
		///       if user does an operation that results in identical state.
		/// </summary>
		/// <param name="btn"></param>
		/// <param name="clicks"></param>
		internal void SetSelected(MouseButtons btn, int clicks)
		{
			if (Tile != null)
			{
				switch (btn)
				{
					case MouseButtons.Left:
						if (clicks == 2)
							ViewerFormsManager.TileView.Control.SelectedTilepart = Tile[SelectedQuadrant];

//((MapView.Forms.MapObservers.TileViews.TileView)ViewerFormsManager.TileView.ObserverControl).SelectedTilepart = _tile[SelectedQuadrant];
// I just want to leave that there so you can ponder the significance of it.
						break;

					case MouseButtons.Right:
						if (MainViewOverlay.that.FirstClick) // do not set a part in a quad unless a tile is selected.
						{
							if (_keyQuadtype == QuadrantType.None) // ie. is *not* keyboard-input
							{
								_t1.Stop();
								++_t1Clicks;
								_t1.Start();
							}
							else // is keyboard-input
							{
								_t1Clicks = clicks;
								OnClicksElapsed(null,null);
							}
						}
						break;
				}
			}
		}
		#endregion Methods
	}
}
