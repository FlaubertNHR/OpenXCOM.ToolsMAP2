using System;
using System.Drawing;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;
using XCom.Base;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The base class for RoutePanel. Generally handles mousey things and
	/// calculating lozenges.
	/// @note This is not a Panel. It is a UserControl inside of a Panel.
	/// </summary>
	internal class RoutePanelParent
		:
			UserControl
	{
		#region Events
		public event EventHandler<RoutePanelEventArgs> RoutePanelMouseDownEvent;
		public event EventHandler<RoutePanelEventArgs> RoutePanelMouseUpEvent;
		#endregion Events


		#region Fields (static)
		internal protected const int OffsetX = 2; // these track the offset between the panel border
		internal protected const int OffsetY = 3; // and the lozenge-tip.
		#endregion Fields (static)


		#region Fields
		private Point _loc; // tracks tile-location for move/up/down mouse events

		internal protected int _col = -1; // these track the location of the last mouse-overed tile
		internal protected int _row = -1; // NOTE: could be subsumed into 'CursorPosition' except ...
		#endregion Fields


		#region Properties (static)
		/// <summary>
		/// A node that is currently selected. Set its value via RouteView only.
		/// </summary>
		internal protected static RouteNode NodeSelected
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		private Point _pos = new Point(-1,-1);
		/// <summary>
		/// Tracks the position of the mouse cursor. Used to print overinfo,
		/// overlayinfo, and to position the Overlay.
		/// </summary>
		internal protected Point CursorPosition
		{
			get { return _pos; }
			set { _pos = value; }
		}

		private MapFile _file;
		internal protected MapFile MapFile
		{
			get { return _file; }
			set
			{
				_file = value;
				OnResize(EventArgs.Empty);
			}
		}

		/// <summary>
		/// The top-left point of the panel.
		/// </summary>
		internal protected Point Origin
		{ get; set; }

		private int _halfwidth = 8;
		/// <summary>
		/// Half the horizontal width of a tile-lozenge.
		/// </summary>
		internal protected int HalfWidth
		{
			get { return _halfwidth; }
			set { _halfwidth = value; }
		}
		private int _halfheight = 4;
		/// <summary>
		/// Half the vertical height of a tile-lozenge.
		/// </summary>
		internal protected int HalfHeight
		{
			get { return _halfheight; }
			set { _halfheight = value; }
		}


		private readonly DrawBlobService _blobService = new DrawBlobService();
		internal protected DrawBlobService BlobService
		{
			get { return _blobService; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of RoutePanel.
		/// </summary>
		internal protected RoutePanelParent()
		{
			var t1 = new Timer();			// because the mouse OnLeave event doesn't fire when the mouse
			t1.Interval = Globals.PERIOD;	// moves over a different form before actually "leaving" this
			t1.Enabled = true;				// control. btw, this is only to stop the overlay from drawing
			t1.Tick += t1_Tick;				// on both RouteView and TopRouteView(Route) simultaneously.
		}									// so uh yeah it's overkill - Good Lord it works.
		#endregion cTor						// Plus it clears the overed infotext tile-coordinates.


		#region Events
		/// <summary>
		/// A ticker that checks if the mouse has left the building.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_Tick(object sender, EventArgs e)
		{
			if (!Bounds.Contains(PointToClient(Control.MousePosition)))
				CursorPosition = new Point(
										_col = -1,
										_row = -1);
		}
		#endregion Events


		#region Events (override)
		/// <summary>
		/// Ensures that a ticker tick happens pronto.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			t1_Tick(this, e);
		}

		/// <summary>
		/// Fires from (child)RoutePanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			if (MapFile != null) // safety.
			{
				int width  = Width  - OffsetX * 2;
				int height = Height - OffsetY * 2;

				if (height > width / 2) // use width
				{
					HalfWidth = width / (MapFile.MapSize.Rows + MapFile.MapSize.Cols);

					if (HalfWidth % 2 != 0)
						--HalfWidth;

					HalfHeight = HalfWidth / 2;
				}
				else // use height
				{
					HalfHeight = height / (MapFile.MapSize.Rows + MapFile.MapSize.Cols);
					HalfWidth  = HalfHeight * 2;
				}

				Origin = new Point( // offset the left and top edges to account for the 3d panel border
								OffsetX + MapFile.MapSize.Rows * HalfWidth,
								OffsetY);

				BlobService.HalfWidth  = HalfWidth;
				BlobService.HalfHeight = HalfHeight;
			}
		}

		/// <summary>
		/// Selects a tile on the mouse-down event.
		/// IMPORTANT: Any changes that are done here regarding node-selection
		/// should be reflected in RouteView.SelectNode() since that is an
		/// alternate way to select a tile/node.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (_loc.X != -1)
			{
				MainViewOverlay.that._keyDeltaX =
				MainViewOverlay.that._keyDeltaY = 0;

				MapFile.Location = new MapLocation( // fire SelectLocation
												_loc.X, _loc.Y,
												MapFile.Level);

				MainViewOverlay.that.ProcessSelection(_loc, _loc);	// set selected location for other viewers.
																	// NOTE: drag-selection is not allowed here.
				var args = new RoutePanelEventArgs(
												e.Button,
												MapFile[_loc.X, _loc.Y],
												MapFile.Location);
				RoutePanelMouseDownEvent(this, args); // fire RouteView.OnRoutePanelMouseDown()
			}
		}

		/// <summary>
		/// Calls RouteView.OnRoutePanelMouseUp().
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (_loc.X != -1)
			{
				MapFile.Location = new MapLocation( // fire SelectLocation
												_loc.X, _loc.Y,
												MapFile.Level);

				var args = new RoutePanelEventArgs(
												e.Button,
												MapFile[_loc.X, _loc.Y],
												MapFile.Location);
				RoutePanelMouseUpEvent(this, args); // fire RouteView.OnRoutePanelMouseUp()
			}
		}

		/// <summary>
		/// Tracks x/y location for the mouseover lozenge.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			_loc = GetTileLocation(e.X, e.Y);
			if (_loc.X != _col || _loc.Y != _row)
			{
				_col = _loc.X;
				_row = _loc.Y;
			}
			base.OnMouseMove(e); // required to fire RouteView.OnRoutePanelMouseMove()
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Keyboard navigation called by RouteViewForm (form-level) key events
		/// OnKeyDown() and ProcessCmdKey().
		/// </summary>
		/// <param name="keyData"></param>
		internal void Navigate(Keys keyData)
		{
			if (MapFile != null && (keyData & (Keys.Control | Keys.Alt)) == Keys.None) // safety.
			{
				bool invalidate = false;

				MainViewOverlay.that._keyDeltaX =
				MainViewOverlay.that._keyDeltaY = 0;

				if (!MainViewOverlay.that.FirstClick) // allow Shift
				{
					MapFile.Location = new MapLocation(0,0, MapFile.Level); // fire SelectLocation event

					var loc = new Point(0,0);
					MainViewOverlay.that.ProcessSelection(loc,loc);

					var args = new RoutePanelEventArgs(
													MouseButtons.Left,
													MapFile[0,0],
													MapFile.Location);
					RoutePanelMouseDownEvent(this, args); // fire RouteView.OnRoutePanelMouseDown()
					invalidate = true;
				}
				else if (keyData == Keys.Enter)
				{
					var args = new RoutePanelEventArgs(
													MouseButtons.Right,
													MapFile[MapFile.Location.Col,
															MapFile.Location.Row],
													MapFile.Location);
					RoutePanelMouseDownEvent(this, args); // fire RouteView.OnRoutePanelMouseDown()
					invalidate = true;
				}
				else if ((keyData & Keys.Shift) == Keys.None)
				{
					var loc = new Point(0,0);
					int vert = MapFileBase.LEVEL_no;

					switch (keyData)
					{
						case Keys.Up:       loc.X = -1; loc.Y = -1; break;
						case Keys.Right:    loc.X = +1; loc.Y = -1; break;
						case Keys.Down:     loc.X = +1; loc.Y = +1; break;
						case Keys.Left:     loc.X = -1; loc.Y = +1; break;

						case Keys.PageUp:   loc.Y = -1; break;
						case Keys.PageDown: loc.X = +1; break;
						case Keys.End:      loc.Y = +1; break;
						case Keys.Home:     loc.X = -1; break;

						case Keys.Add:      vert = MapFileBase.LEVEL_Dn; break;
						case Keys.Subtract: vert = MapFileBase.LEVEL_Up; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int c = MapFile.Location.Col + loc.X;
						int r = MapFile.Location.Row + loc.Y;
						if (   c > -1 && c < MapFile.MapSize.Cols
							&& r > -1 && r < MapFile.MapSize.Rows)
						{
							MapFile.Location = new MapLocation(c,r, MapFile.Level); // fire SelectLocation event

							loc.X = c; loc.Y = r;
							MainViewOverlay.that.ProcessSelection(loc,loc);

							var args = new RoutePanelEventArgs(
															MouseButtons.Left,
															MapFile[c,r],
															MapFile.Location);
							RoutePanelMouseDownEvent(this, args); // fire RouteView.OnRoutePanelMouseDown()
							invalidate = true;
						}
					}
					else if (vert != MapFileBase.LEVEL_no)
					{
						int level = MapFile.Level + vert;
						if (level > -1 && level < MapFile.MapSize.Levs)
						{
							MapFile.ChangeLevel(vert);			// fire SelectLevel event
							MapFile.Location = new MapLocation(	// fire SelectLocation event
															MapFile.Location.Col,
															MapFile.Location.Row,
															level);
						}
					}
				}
				else if (NodeSelected != null) // Shift = drag node ->
				{
					var loc = new Point(0,0);
					int vert = MapFileBase.LEVEL_no;

					switch (keyData)
					{
						case Keys.Shift | Keys.Up:       loc.X = -1; loc.Y = -1; break;
						case Keys.Shift | Keys.Right:    loc.X = +1; loc.Y = -1; break;
						case Keys.Shift | Keys.Down:     loc.X = +1; loc.Y = +1; break;
						case Keys.Shift | Keys.Left:     loc.X = -1; loc.Y = +1; break;

						case Keys.Shift | Keys.PageUp:   loc.Y = -1; break;
						case Keys.Shift | Keys.PageDown: loc.X = +1; break;
						case Keys.Shift | Keys.End:      loc.Y = +1; break;
						case Keys.Shift | Keys.Home:     loc.X = -1; break;

						case Keys.Shift | Keys.Add:      vert = MapFileBase.LEVEL_Dn; break;
						case Keys.Shift | Keys.Subtract: vert = MapFileBase.LEVEL_Up; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int c = MapFile.Location.Col + loc.X;
						int r = MapFile.Location.Row + loc.Y;
						if (   c > -1 && c < MapFile.MapSize.Cols
							&& r > -1 && r < MapFile.MapSize.Rows
							&& MapFile[c,r].Node == null)
						{
							RouteView.Dragnode = NodeSelected;

							MapFile.Location = new MapLocation(c,r, MapFile.Level); // fire SelectLocation event

							var args = new RoutePanelEventArgs(
															MouseButtons.None,
															MapFile[c,r],
															MapFile.Location);
							RoutePanelMouseUpEvent(this, args); // fire RouteView.OnRoutePanelMouseUp()
							invalidate = true;

							ObserverManager.RouteView   .Control     .SetInfotextOver();
							ObserverManager.TopRouteView.ControlRoute.SetInfotextOver();
						}
					}
					else if (vert != MapFileBase.LEVEL_no)
					{
						int level = MapFile.Level + vert;
						if (level > -1 && level < MapFile.MapSize.Levs
							&& MapFile[MapFile.Location.Col,
									   MapFile.Location.Row,
									   level].Node == null)
						{
							RouteView.Dragnode = NodeSelected;

							MapFile.ChangeLevel(vert);			// fire SelectLevel event
							MapFile.Location = new MapLocation(	// fire SelectLocation event
															MapFile.Location.Col,
															MapFile.Location.Row,
															level);

							var args = new RoutePanelEventArgs(
															MouseButtons.None,
															MapFile[MapFile.Location.Col,
																	MapFile.Location.Row],
															MapFile.Location);
							RoutePanelMouseUpEvent(this, args); // fire RouteView.OnRoutePanelMouseUp()
							invalidate = true;

							ObserverManager.RouteView   .Control     .SetInfotextOver();
							ObserverManager.TopRouteView.ControlRoute.SetInfotextOver();

							ObserverManager.RouteView   .Control     .PrintSelectedInfo();
							ObserverManager.TopRouteView.ControlRoute.PrintSelectedInfo();
						}
					}
				}

				if (invalidate)
				{
					ObserverManager.RouteView   .Control     .RoutePanel.Invalidate();
					ObserverManager.TopRouteView.ControlRoute.RoutePanel.Invalidate();
				}
			}
		}


		/// <summary>
		/// Gets the tile contained at (x,y) wrt client-area in local screen
		/// coordinates.
		/// </summary>
		/// <param name="x">ref to the x-position of the mouse-cursor wrt
		/// Client-area (refout is the tile-x location)</param>
		/// <param name="y">ref to the y-position of the mouse-cursor wrt
		/// Client-area (refout is the tile-y location)</param>
		/// <returns>the corresponding MapTile or null if (x,y) is an invalid
		/// location for a tile</returns>
		internal protected MapTile GetTile(ref int x, ref int y)
		{
			var loc = GetTileLocation(x,y);
			if ((x = loc.X) != -1)
				return MapFile[x, (y = loc.Y)];

			return null;
		}

		/// <summary>
		/// Converts a position from client-coordinates to a tile-location.
		/// </summary>
		/// <param name="x">the x-position of the mouse-cursor wrt Client-area</param>
		/// <param name="y">the y-position of the mouse-cursor wrt Client-area</param>
		/// <returns>the corresponding tile-location or (-1,-1) if the location
		/// is invalid</returns>
		internal protected Point GetTileLocation(int x, int y)
		{
			if (MapFile != null) // safety.
			{
				x -= Origin.X;
				y -= Origin.Y;

				double xd = (double)x / (HalfWidth  * 2)
						  + (double)y / (HalfHeight * 2);
				double yd = ((double)y * 2 - x) / (HalfWidth * 2);

				var point = new Point(
									(int)Math.Floor(xd),
									(int)Math.Floor(yd));

				if (   point.X > -1 && point.X < MapFile.MapSize.Cols
					&& point.Y > -1 && point.Y < MapFile.MapSize.Rows)
				{
					return point;
				}
			}
			return new Point(-1,-1);
		}
		#endregion Methods
	}
}
