using System;
using System.Drawing;
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
		private MapTile _tile;
		private MapLocation _location;
		#endregion Fields


		#region Properties
		private QuadrantType _quadrant;
		internal QuadrantType SelectedQuadrant
		{
			get { return _quadrant; }
			set { _quadrant = value; Refresh(); }
		}
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
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			_tile     = args.Tile as MapTile;
			_location = args.Location;
			Refresh();
		}

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			if (_location != null)
			{
				_tile = MapBase[_location.Row, _location.Col] as MapTile;
				_location.Lev = args.Level;
			}
			Refresh();
		}


		private QuadrantType _keyQuadtype = QuadrantType.None;
		internal void ForceMouseDown(MouseEventArgs e, QuadrantType quadType)
		{
			_keyQuadtype = quadType;
			OnMouseDown(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			ViewerFormsManager.TopView     .Control   .TopPanel.Select();
			ViewerFormsManager.TopRouteView.ControlTop.TopPanel.Select();

			QuadrantType quadType;
			if (_keyQuadtype == QuadrantType.None) // ie. is *not* forced by keyboard-input
			{
				quadType = (QuadrantType)((e.X - QuadrantPanelDrawService.StartX)
											   / QuadrantPanelDrawService.QuadWidthTotal);
			}
			else
			{
				quadType = _keyQuadtype;
				_keyQuadtype = QuadrantType.None;
			}

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
				if (e.Button == MouseButtons.Right) // see SetSelected()
				{
					MainViewOverlay.that.Refresh();

					ViewerFormsManager.TopView     .Refresh();
					ViewerFormsManager.RouteView   .Refresh();
					ViewerFormsManager.TopRouteView.Refresh();

					if (XCMainWindow.ScanG != null)
						XCMainWindow.ScanG.InvalidatePanel();
				}
			}
		}

		/// <summary>
		/// Overrides DoubleBufferedControl.RenderGraphics() - ie, OnPaint().
		/// @note Calls the draw-function in QuadrantPanelDrawService.
		/// </summary>
		/// <param name="graphics"></param>
		protected override void RenderGraphics(Graphics graphics)
		{
			QuadrantPanelDrawService.Draw(graphics, _tile, SelectedQuadrant);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Handles the details of LMB and RMB wrt the QuadrantPanels.
		/// </summary>
		/// <param name="btn"></param>
		/// <param name="clicks"></param>
		internal void SetSelected(MouseButtons btn, int clicks)
		{
			if (_tile != null)
			{
				switch (btn)
				{
					case MouseButtons.Left:
						switch (clicks)
						{
							case 1:
								break;

							case 2:
								var tileView = ViewerFormsManager.TileView.Control;
								tileView.SelectedTilepart = _tile[SelectedQuadrant];
								break;
						}
						break;

					case MouseButtons.Right:
					{
						if (MainViewOverlay.that.FirstClick) // do not set a part in a quad unless a tile is selected.
						{
							switch (clicks)
							{
								case 1:
									var tileView = ViewerFormsManager.TileView.Control;
									_tile[SelectedQuadrant] = tileView.SelectedTilepart;
									_tile.Vacancy();

									MainViewOverlay.that.Refresh();

									ViewerFormsManager.RouteView   .Control     .Refresh();
									ViewerFormsManager.TopRouteView.ControlRoute.Refresh();
									break;

								case 2:
									// TODO: GENERAL - Bypass operations (and the MapChanged flag)
									//       if user does an operation that results in identical state.
									_tile[SelectedQuadrant] = null;
									_tile.Vacancy();
									break;
							}

							XCMainWindow.that.MapChanged = true;

							Refresh();

							ViewerFormsManager.TopView     .Control   .TopPanel.Refresh();
							ViewerFormsManager.TopRouteView.ControlTop.TopPanel.Refresh();
						}
						break;
					}
				}
			}
		}
		#endregion Methods
	}
}
