using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
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
		internal protected int _overCol = -1; // these track the location of the last mouse-overed tile
		internal protected int _overRow = -1; // NOTE: could be subsumed into 'CursorPosition' except ...
		#endregion Fields


		#region Properties (static)
		/// <summary>
		/// Stores the x/y-position of the currently selected tile.
		/// </summary>
		internal protected static Point SelectedPosition
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		private Point _pos = new Point(-1, -1);
		/// <summary>
		/// Tracks the tile-position of the mouse cursor. Used to print
		/// over-info, overlay-info, and to position the Overlay.
		/// </summary>
		internal Point CursorPosition
		{
			get { return _pos; }
			set { _pos = value; }
		}

		private MapFileChild _child;
		internal protected MapFileChild MapChild
		{
			get { return _child; }
			set
			{
				_child = value;
				OnResize(EventArgs.Empty);
			}
		}

		/// <summary>
		/// The top-left point of the panel.
		/// </summary>
		internal protected Point Origin
		{ get; set; }

		private int _drawAreaWidth = 8;
		/// <summary>
		/// Half the horizontal width of a tile-lozenge.
		/// </summary>
		internal protected int DrawAreaWidth
		{
			get { return _drawAreaWidth; }
			set { _drawAreaWidth = value; }
		}
		private int _drawAreaHeight = 4;
		/// <summary>
		/// Half the vertical height of a tile-lozenge.
		/// </summary>
		internal protected int DrawAreaHeight
		{
			get { return _drawAreaHeight; }
			set { _drawAreaHeight = value; }
		}


		private readonly GraphicsPath _lozSelector = new GraphicsPath(); // mouse-over lozenge
		internal protected GraphicsPath LozSelector
		{
			get { return _lozSelector; }
		}

		private readonly GraphicsPath _lozSelected = new GraphicsPath(); // click/drag lozenge
		internal protected GraphicsPath LozSelected
		{
			get { return _lozSelected; }
		}

		private readonly GraphicsPath _lozSpotted = new GraphicsPath(); // go-button lozenge
		internal protected GraphicsPath LozSpotted
		{
			get { return _lozSpotted; }
		}

		private readonly DrawBlobService _blobService = new DrawBlobService();
		internal protected DrawBlobService BlobService
		{
			get { return _blobService; }
		}

		private readonly Dictionary<string, Pen> _pens = new Dictionary<string, Pen>();
		internal protected Dictionary<string, Pen> RoutePens
		{
			get { return _pens; }
		}
		private readonly Dictionary<string, SolidBrush> _brushes = new Dictionary<string, SolidBrush>();
		internal protected Dictionary<string, SolidBrush> RouteBrushes
		{
			get { return _brushes; }
		}

		private int _opacity = 255; // cf. RouteView.LoadControl0Options()
		internal protected int Opacity
		{
			get { return _opacity; }
			set { _opacity = value.Clamp(0, 255); }
		}

		private bool _showOverlay = true; // cf. RouteView.LoadControl0Options()
		internal protected bool ShowOverlay
		{
			get { return _showOverlay; }
			set { _showOverlay = value; }
		}

		private bool _showPriorityBars = true; // cf. RouteView.LoadControl0Options()
		internal protected bool ShowPriorityBars
		{
			get { return _showPriorityBars; }
			set { _showPriorityBars = value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiated as the parent of RoutePanel which uses a default
		/// cTor.
		/// </summary>
		internal protected RoutePanelParent()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			MainViewUnderlay.Instance.MainViewOverlay.MouseDragEvent += PathSelectedLozenge;


			var t1 = new Timer();	// because the mouse OnLeave event doesn't fire
			t1.Interval = 250;		// when the mouse moves over a different form before
			t1.Enabled = true;		// actually "leaving" this control.
			t1.Tick += t1_Tick;		// btw, this is only to stop the overlay from drawing
		}							// on both RouteView and TopRouteView(Route) simultaneously.
		#endregion cTor				// so uh yeah it's overkill
									// Good Lord it works.

		#region Events
		private void t1_Tick(object sender, EventArgs e)
		{
			if (!Bounds.Contains(PointToClient(Cursor.Position)))
				CursorPosition = new Point(
										_overCol = -1,
										_overRow = -1);
		}
		#endregion Events


		#region Events (override)
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			t1_Tick(this, e);
		}

		protected override void OnResize(EventArgs e)
		{
			if (MapChild != null)
			{
				int width  = Width  - OffsetX * 2;
				int height = Height - OffsetY * 2;

				if (height > width / 2) // use width
				{
					DrawAreaWidth = width / (MapChild.MapSize.Rows + MapChild.MapSize.Cols);

					if (DrawAreaWidth % 2 != 0)
						--DrawAreaWidth;

					DrawAreaHeight = DrawAreaWidth / 2;
				}
				else // use height
				{
					DrawAreaHeight = height / (MapChild.MapSize.Rows + MapChild.MapSize.Cols);
					DrawAreaWidth  = DrawAreaHeight * 2;
				}

				Origin = new Point( // offset the left and top edges to account for the 3d panel border
								OffsetX + MapChild.MapSize.Rows * DrawAreaWidth,
								OffsetY);

				BlobService.HalfWidth  = DrawAreaWidth;
				BlobService.HalfHeight = DrawAreaHeight;

				PathSelectedLozenge();

				Refresh();
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

			if (MapChild != null) // safety.
			{
				var loc = GetTileLocation(e.X, e.Y);
				if (loc.X != -1)
				{
					MainViewUnderlay.Instance.MainViewOverlay._keyDeltaX =
					MainViewUnderlay.Instance.MainViewOverlay._keyDeltaY = 0;

					MapChild.Location = new MapLocation(						// fire SelectLocationEvent
													loc.Y, loc.X,
													MapChild.Level);

					MainViewUnderlay.Instance.MainViewOverlay.ProcessSelection(loc, loc);	// set selected location for other viewers.
																							// NOTE: drag-selection is not allowed here.
					if (RoutePanelMouseDownEvent != null)
					{
						var args = new RoutePanelEventArgs(
														e.Button,
														MapChild[loc.Y, loc.X],
														MapChild.Location);
						RoutePanelMouseDownEvent(this, args);					// fire RouteView.OnRoutePanelMouseDown()
					}

					SelectedPosition = loc;	// NOTE: if a new 'SelectedPosition' is set before firing the
				}							// RoutePanelMouseDownEvent, OnPaint() will draw a frame with
			}								// incorrectly selected-link lines. So set the 'SelectedPosition'
		}									// *after* the event happens.

		/// <summary>
		/// Tracks x/y location for the mouseover lozenge.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			var loc = GetTileLocation(e.X, e.Y);
			if (loc.X != _overCol || loc.Y != _overRow)
			{
				_overCol = loc.X;
				_overRow = loc.Y;
			}
			base.OnMouseMove(e); // required to fire RouteView.OnRoutePanelMouseMove()
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (MapChild != null // safety.
				&& RoutePanelMouseUpEvent != null)
			{
				var loc = GetTileLocation(e.X, e.Y);
				if (loc.X != -1)
				{
					var args = new RoutePanelEventArgs(
													e.Button,
													MapChild[loc.Y, loc.X],
													new MapLocation(
																loc.Y, loc.X,
																MapChild.Level));
					RoutePanelMouseUpEvent(this, args); // fire RouteView.OnRoutePanelMouseUp()
				}
			}
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
			if (MapChild != null) // safety.
			{
				MainViewUnderlay.Instance.MainViewOverlay._keyDeltaX =
				MainViewUnderlay.Instance.MainViewOverlay._keyDeltaY = 0;

				if (!MainViewUnderlay.Instance.MainViewOverlay.FirstClick)
				{
					MapChild.Location = new MapLocation(0, 0, MapChild.Level);	// fire SelectLocationEvent

					var loc = new Point(0,0);
					MainViewUnderlay.Instance.MainViewOverlay.ProcessSelection(loc,loc);

					if (RoutePanelMouseDownEvent != null)
					{
						var args = new RoutePanelEventArgs(
														MouseButtons.Left,
														MapChild[0,0],
														MapChild.Location);
						RoutePanelMouseDownEvent(this, args);					// fire RouteView.OnRoutePanelMouseDown()
						Invalidate();
					}
					SelectedPosition = loc;
				}
				else if (keyData == Keys.Enter)
				{
//					MapChild.Location = new MapLocation(						// fire SelectLocationEvent
//													MapChild.Location.Row,
//													MapChild.Location.Col,
//													MapChild.Level);

					if (RoutePanelMouseDownEvent != null)
					{
						var args = new RoutePanelEventArgs(
														MouseButtons.Right,
														MapChild[MapChild.Location.Row,
																 MapChild.Location.Col],
														MapChild.Location);
						RoutePanelMouseDownEvent(this, args);					// fire RouteView.OnRoutePanelMouseDown()
						Invalidate();
					}
					SelectedPosition = new Point(
											MapChild.Location.Col,
											MapChild.Location.Row);
				}
				else //if (!keyData.HasFlag(Keys.Shift)) // TODO: implement [Shift] for dragnode
				{
					var loc = new Point(0,0);
					int vert = 0;
					switch (keyData)
					{
						case Keys.Up:    loc.X = -1; loc.Y = -1; break;
						case Keys.Right: loc.X = +1; loc.Y = -1; break;
						case Keys.Down:  loc.X = +1; loc.Y = +1; break;
						case Keys.Left:  loc.X = -1; loc.Y = +1; break;

						case Keys.PageUp:   loc.Y = -1; break;
						case Keys.PageDown: loc.X = +1; break;
						case Keys.End:      loc.Y = +1; break;
						case Keys.Home:     loc.X = -1; break;

//						case Keys.Delete: // oops Delete is delete tile - try [Shift+Insert]
						case Keys.Add:
							ViewerFormsManager.RouteView.Control.ForceMousewheel(new MouseEventArgs(
																								MouseButtons.None,
																								0, 0, 0, (vert = +1)));
							break;

//						case Keys.Insert:
						case Keys.Subtract:
							ViewerFormsManager.RouteView.Control.ForceMousewheel(new MouseEventArgs(
																								MouseButtons.None,
																								0, 0, 0, (vert = -1)));
							break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						MapChild.Location = new MapLocation(					// fire SelectLocationEvent
														MapChild.Location.Row + loc.Y,
														MapChild.Location.Col + loc.X,
														MapChild.Level);
						loc.X = MapChild.Location.Col;
						loc.Y = MapChild.Location.Row;
						MainViewUnderlay.Instance.MainViewOverlay.ProcessSelection(loc, loc);

						if (RoutePanelMouseDownEvent != null)
						{
							var args = new RoutePanelEventArgs(
															MouseButtons.Left,
															MapChild[loc.Y, loc.X],
															MapChild.Location);
							RoutePanelMouseDownEvent(this, args);				// fire RouteView.OnRoutePanelMouseDown()
							Invalidate();
						}
						SelectedPosition = loc;
					}
					else if (vert != 0)
					{
						MapChild.Location = new MapLocation(					// fire SelectLocationEvent
														MapChild.Location.Row,
														MapChild.Location.Col,
														MapChild.Level);
					}
				}
/*				else // [Shift] = drag select ->
				{
					var pt = new Point(0,0);
					switch (keyData)
					{
						case (Keys.Shift | Keys.Up):    pt.X = -1; pt.Y = -1; break;
						case (Keys.Shift | Keys.Right): pt.X = +1; pt.Y = -1; break;
						case (Keys.Shift | Keys.Down):  pt.X = +1; pt.Y = +1; break;
						case (Keys.Shift | Keys.Left):  pt.X = -1; pt.Y = +1; break;

						case (Keys.Shift | Keys.PageUp):   pt.Y = -1; break;
						case (Keys.Shift | Keys.PageDown): pt.X = +1; break;
						case (Keys.Shift | Keys.End):      pt.Y = +1; break;
						case (Keys.Shift | Keys.Home):     pt.X = -1; break;
					}

					if (pt.X != 0 || pt.Y != 0) // safety.
					{
						int pos = DragBeg.X + _keyDeltaX + pt.X;
						if (pos > -1 && pos < MapBase.MapSize.Cols)
							_keyDeltaX += pt.X;

						pos = DragBeg.Y + _keyDeltaY + pt.Y;
						if (pos > -1 && pos < MapBase.MapSize.Rows)
							_keyDeltaY += pt.Y;

						var loc = new Point(
										MapBase.Location.Col + _keyDeltaX,
										MapBase.Location.Row + _keyDeltaY);

						_colOver = loc.X;
						_rowOver = loc.Y;

						ProcessSelection(DragBeg, loc);
					}
				} */
			}
		}


		/// <summary>
		/// Sets the graphics-path for a lozenge-border around the tile that
		/// is currently mouse-overed.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		internal protected void PathSelectorLozenge(int x, int y)
		{
			int halfWidth  = BlobService.HalfWidth;
			int halfHeight = BlobService.HalfHeight;

			var p0 = new Point(x,             y);
			var p1 = new Point(x + halfWidth, y + halfHeight);
			var p2 = new Point(x,             y + halfHeight * 2);
			var p3 = new Point(x - halfWidth, y + halfHeight);

			LozSelector.Reset();
			LozSelector.AddLine(p0, p1);
			LozSelector.AddLine(p1, p2);
			LozSelector.AddLine(p2, p3);
			LozSelector.CloseFigure();
		}

		/// <summary>
		/// Sets the graphics-path for a lozenge-border around all tiles that
		/// are selected or being selected.
		/// </summary>
		private void PathSelectedLozenge()
		{
			var a = MainViewUnderlay.Instance.MainViewOverlay.GetDragBeg_abs();
			var b = MainViewUnderlay.Instance.MainViewOverlay.GetDragEnd_abs();

			int halfWidth  = BlobService.HalfWidth;
			int halfHeight = BlobService.HalfHeight;

			var p0 = new Point(
							Origin.X + (a.X - a.Y) * halfWidth,
							Origin.Y + (a.X + a.Y) * halfHeight);
			var p1 = new Point(
							Origin.X + (b.X - a.Y) * halfWidth  + halfWidth,
							Origin.Y + (b.X + a.Y) * halfHeight + halfHeight);
			var p2 = new Point(
							Origin.X + (b.X - b.Y) * halfWidth,
							Origin.Y + (b.X + b.Y) * halfHeight + halfHeight * 2);
			var p3 = new Point(
							Origin.X + (a.X - b.Y) * halfWidth  - halfWidth,
							Origin.Y + (a.X + b.Y) * halfHeight + halfHeight);

			LozSelected.Reset();
			LozSelected.AddLine(p0, p1);
			LozSelected.AddLine(p1, p2);
			LozSelected.AddLine(p2, p3);
			LozSelected.CloseFigure();

			Refresh();
		}

		internal protected void PathSpottedLozenge(int x, int y)
		{
			int halfWidth  = BlobService.HalfWidth;
			int halfHeight = BlobService.HalfHeight;

			var p0 = new Point(x,             y);
			var p1 = new Point(x + halfWidth, y + halfHeight);
			var p2 = new Point(x,             y + halfHeight * 2);
			var p3 = new Point(x - halfWidth, y + halfHeight);

			LozSpotted.Reset();
			LozSpotted.AddLine(p0, p1);
			LozSpotted.AddLine(p1, p2);
			LozSpotted.AddLine(p2, p3);
			LozSpotted.CloseFigure();
		}

		/// <summary>
		/// Gets the tile contained at (x,y) wrt client-area in local screen
		/// coordinates.
		/// </summary>
		/// <param name="x">ref to the x-position of the mouse-cursor wrt
		/// Client-area (refout is the tile-x location)</param>
		/// <param name="y">ref to the y-position of the mouse-cursor wrt
		/// Client-area (refout is the tile-y location)</param>
		/// <returns>the corresponding XCMapTile or null if (x,y) is an invalid
		/// location for a tile</returns>
		internal protected XCMapTile GetTile(ref int x, ref int y)
		{
			var loc = GetTileLocation(x, y);
			x = loc.X;
			y = loc.Y;
			return (x != -1) ? MapChild[y, x] as XCMapTile
							 : null;
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
			if (MapChild != null)
			{
				x -= Origin.X;
				y -= Origin.Y;

				double xd = (double)x / (DrawAreaWidth  * 2)
						  + (double)y / (DrawAreaHeight * 2);
				double yd = ((double)y * 2 - x) / (DrawAreaWidth * 2);

				var point = new Point(
									(int)Math.Floor(xd),
									(int)Math.Floor(yd));

				if (   point.Y > -1 && point.Y < MapChild.MapSize.Rows
					&& point.X > -1 && point.X < MapChild.MapSize.Cols)
				{
					return point;
				}
			}
			return new Point(-1, -1);
		}
		#endregion Methods
	}
}
