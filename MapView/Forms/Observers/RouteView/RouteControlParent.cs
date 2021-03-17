using System;
using System.Drawing;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The base class for <see cref="MapView.Forms.Observers.RouteControl"/>.
	/// Generally handles mousey things and keyboard navigation.
	/// </summary>
	/// <remarks><see cref="MapView.Forms.Observers.RouteView"/> also handles
	/// mouse events.</remarks>
	internal class RouteControlParent
		:
			UserControl
	{
		#region Events
		public event EventHandler<RouteControlEventArgs> RouteControlMouseDownEvent;
		public event EventHandler<RouteControlEventArgs> RouteControlMouseUpEvent;
		#endregion Events


		#region Fields (static)
		internal protected const int OffsetX = 2; // these track the offset between the panel border
		internal protected const int OffsetY = 3; // and the lozenge-tip.
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// '_col' and '_row' track the location of the last mouse-overed tile;
		/// '_col' needs to be set to "-1" when the mouse is not over a tile.
		/// Their values need to be updated only when the mouseovered
		/// tile-location changes via <see cref="OnMouseMove"/>.
		/// </summary>
		internal protected int _col = -1;
		/// <summary>
		/// '_col' and '_row' track the location of the last mouse-overed tile;
		/// '_col' needs to be set to "-1" when the mouse is not over a tile.
		/// Their values need to be updated only when the mouseovered
		/// tile-location changes via <see cref="OnMouseMove"/>.
		/// </summary>
		internal protected int _row = -1;

		/// <summary>
		/// Tracks tile-location for move/up/down mouse events: '_col' and
		/// '_row' in a convenient Point object.
		/// </summary>
		internal protected Point _loc;
		#endregion Fields


		#region Properties (static)
		/// <summary>
		/// A node that is currently selected. Set its value via
		/// <see cref="RouteView.NodeSelected">RouteView.NodeSelected</see>
		/// only.
		/// </summary>
		internal protected static RouteNode NodeSelected
		{ get; set; }
		#endregion Properties (static)


		#region Properties
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
		/// cTor. Instantiated only as the parent of RouteControl.
		/// </summary>
		internal protected RouteControlParent()
		{
			var t1 = new Timer();			// because the mouse OnLeave event doesn't fire when the mouse
			t1.Interval = Globals.PERIOD;	// moves over a different form before actually "leaving" this
			t1.Enabled = true;				// control. btw, this is only to stop the overlay from drawing
			t1.Tick += t1_Tick;				// on both RouteView and TopRouteView(Route) simultaneously.
		}									// so uh yeah it's overkill - Good Lord it works.
		#endregion cTor						// Plus it clears the overed infotext tile-coordinates.


		#region Events
		/// <summary>
		/// A ticker that checks if the mouse has left the building. See also
		/// RouteView.OnRouteControlMouseLeave().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_Tick(object sender, EventArgs e)
		{
			if (!Bounds.Contains(PointToClient(Control.MousePosition)))
			{
				_col = -1;
				// TODO: perhaps fire OnMouseMove()
			}
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
		/// Fires from (child)RouteControl.
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
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Any changes that are done here regarding node-selection
		/// should be reflected in RouteView.SelectNode() since that is an
		/// alternate way to select a tile/node.</remarks>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (_col != -1)
			{
				MainViewOverlay.that._keyDeltaX =
				MainViewOverlay.that._keyDeltaY = 0;

				MapFile.Location = new MapLocation( // fire LocationSelected
												_col, _row,
												MapFile.Level);

				MainViewOverlay.that.ProcessSelection(_loc, _loc);	// set selected location for other viewers.
																	// NOTE: drag-selection is not allowed here.
				var args = new RouteControlEventArgs(
												e.Button,
												MapFile[_col, _row],
												MapFile.Location);
				RouteControlMouseDownEvent(this, args); // fire RouteView.OnRouteControlMouseDown()
			}
		}

		/// <summary>
		/// Calls RouteView.OnRouteControlMouseUp().
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (_col != -1)
			{
				MapFile.Location = new MapLocation( // fire LocationSelected
												_col, _row,
												MapFile.Level);

				var args = new RouteControlEventArgs(
												e.Button,
												MapFile[_col, _row],
												MapFile.Location);
				RouteControlMouseUpEvent(this, args); // fire RouteView.OnRouteControlMouseUp()
			}
		}

		/// <summary>
		/// Tracks col/row location for the mouseover lozenge and mouseover info.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			_loc = GetTileLocation(e.X, e.Y);
			if (_loc.X != _col || _loc.Y != _row)
			{
				_col = _loc.X;
				_row = _loc.Y;

				// this fires panel refreshes only when the cursor moves to another tile
				// The InfoOverlay goes sticky but the panel feels tighter.
				base.OnMouseMove(e); // fire RouteView.OnRouteControlMouseMove()
				return;
			}

			if (RouteView.Optionables.ReduceDraws || !RouteView.Optionables.ShowOverlay)
				return;

			// this fires panel refreshes whenever the mouse moves a single pixel
			// The InfoOverlay moves freely.
			base.OnMouseMove(e); // fire RouteView.OnRouteControlMouseMove()
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
					MapFile.Location = new MapLocation(0,0, MapFile.Level); // fire LocationSelected event

					var loc = new Point(0,0);
					MainViewOverlay.that.ProcessSelection(loc,loc);

					var args = new RouteControlEventArgs(
													MouseButtons.Left,
													MapFile[0,0],
													MapFile.Location);
					RouteControlMouseDownEvent(this, args); // fire RouteView.OnRouteControlMouseDown()
					invalidate = true;
				}
				else if (keyData == Keys.Enter)
				{
					var args = new RouteControlEventArgs(
													MouseButtons.Right,
													MapFile[MapFile.Location.Col,
															MapFile.Location.Row],
													MapFile.Location);
					RouteControlMouseDownEvent(this, args); // fire RouteView.OnRouteControlMouseDown()
					invalidate = true;
				}
				else if ((keyData & Keys.Shift) == Keys.None)
				{
					var loc = new Point(0,0);
					int vert = MapFile.LEVEL_no;

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

						case Keys.Add:      vert = MapFile.LEVEL_Dn; break;
						case Keys.Subtract: vert = MapFile.LEVEL_Up; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int c = MapFile.Location.Col + loc.X;
						int r = MapFile.Location.Row + loc.Y;
						if (   c > -1 && c < MapFile.MapSize.Cols
							&& r > -1 && r < MapFile.MapSize.Rows)
						{
							MapFile.Location = new MapLocation(c,r, MapFile.Level); // fire LocationSelected event

							loc.X = c; loc.Y = r;
							MainViewOverlay.that.ProcessSelection(loc,loc);

							var args = new RouteControlEventArgs(
															MouseButtons.Left,
															MapFile[c,r],
															MapFile.Location);
							RouteControlMouseDownEvent(this, args); // fire RouteView.OnRouteControlMouseDown()
							invalidate = true;
						}
					}
					else if (vert != MapFile.LEVEL_no)
					{
						int level = MapFile.Level + vert;
						if (level > -1 && level < MapFile.MapSize.Levs)
						{
							MapFile.ChangeLevel(vert);			// fire LevelSelected event
							MapFile.Location = new MapLocation(	// fire LocationSelected event
															MapFile.Location.Col,
															MapFile.Location.Row,
															level);
						}
					}
				}
				else if (NodeSelected != null) // Shift = drag node ->
				{
					var loc = new Point(0,0);
					int vert = MapFile.LEVEL_no;

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

						case Keys.Shift | Keys.Add:      vert = MapFile.LEVEL_Dn; break;
						case Keys.Shift | Keys.Subtract: vert = MapFile.LEVEL_Up; break;
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

							MapFile.Location = new MapLocation(c,r, MapFile.Level); // fire LocationSelected event

							var args = new RouteControlEventArgs(
															MouseButtons.None,
															MapFile[c,r],
															MapFile.Location);
							RouteControlMouseUpEvent(this, args); // fire RouteView.OnRouteControlMouseUp()
							invalidate = true;

							ObserverManager.RouteView.Control.SetInfoOverText(); // update both viewers.
						}
					}
					else if (vert != MapFile.LEVEL_no)
					{
						int level = MapFile.Level + vert;
						if (level > -1 && level < MapFile.MapSize.Levs
							&& MapFile[MapFile.Location.Col,
									   MapFile.Location.Row,
									   level].Node == null)
						{
							RouteView.Dragnode = NodeSelected;

							MapFile.ChangeLevel(vert);			// fire LevelSelected event
							MapFile.Location = new MapLocation(	// fire LocationSelected event
															MapFile.Location.Col,
															MapFile.Location.Row,
															level);

							var args = new RouteControlEventArgs(
															MouseButtons.None,
															MapFile[MapFile.Location.Col,
																	MapFile.Location.Row],
															MapFile.Location);
							RouteControlMouseUpEvent(this, args); // fire RouteView.OnRouteControlMouseUp()
							invalidate = true;

							ObserverManager.RouteView.Control.SetInfoOverText(); // update both viewers.

							ObserverManager.RouteView   .Control     .PrintSelectedInfo();
							ObserverManager.TopRouteView.ControlRoute.PrintSelectedInfo();
						}
					}
				}

				if (invalidate)
				{
					ObserverManager.RouteView   .Control     .RouteControl.Invalidate();
					ObserverManager.TopRouteView.ControlRoute.RouteControl.Invalidate();
				}
			}
		}


/*		/// <summary>
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
		} */

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

				var loc = new Point(
								(int)Math.Floor(xd),
								(int)Math.Floor(yd));

				if (   loc.X > -1 && loc.X < MapFile.MapSize.Cols
					&& loc.Y > -1 && loc.Y < MapFile.MapSize.Rows)
				{
					return loc;
				}
			}
			return new Point(-1,-1);
		}
		#endregion Methods
	}
}