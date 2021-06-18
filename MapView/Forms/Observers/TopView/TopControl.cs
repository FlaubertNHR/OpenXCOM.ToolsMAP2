using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The top region of <c><see cref="Observers.TopView"/></c>.
	/// </summary>
	internal sealed class TopControl
		:
			ObserverControl_Top // DoubleBufferedControl, IObserver
	{
		/// <summary>
		/// Disposes the GraphicsPaths.
		/// </summary>
		public void DisposeControl()
		{
			DSShared.LogFile.WriteLine("TopControl.DisposeControl()");
			_lozSelector.Dispose();
			_lozSelected.Dispose();

			_blobService.Dispose();
		}


		#region Fields (static)
		private const int OffsetX = 2; // these are the offsets between the
		private const int OffsetY = 3; // panel border and the lozenge-tip(s).

		internal static BlobColorTool ToolWest;
		internal static BlobColorTool ToolNorth;
		internal static BlobColorTool ToolContent;
		#endregion Fields (static)


		#region Fields
		private Graphics _graphics;
		private readonly GraphicsPath _lozSelector = new GraphicsPath(); // mouse-over cursor lozenge
		private readonly GraphicsPath _lozSelected = new GraphicsPath(); // selected tile or tiles being drag-selected

		private readonly BlobDrawService _blobService = new BlobDrawService();


		private int _originX;	// since the lozenge is drawn with its Origin at 0,0 of the
								// panel, the entire lozenge needs to be displaced to the right.
//		private int _originY;	// But this isn't really used. It's set to 'OffsetY' and stays that way. -> done.

		private Point _loc;

		private int _col = -1; // these track the location of the mouse-cursor
		private int _row = -1;

		private bool _isMouseDrag;
		#endregion Fields


		#region Properties (override)
		/// <summary>
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl_Top"/>.
		/// </summary>
		[Browsable(false)]
		public override MapFile MapFile
		{
			set
			{
				base.MapFile = value;

				_blobService.HalfWidth = 8;

				ResizeObserver(Parent.Width, Parent.Height);
				Refresh();
			}
		}
		#endregion Properties (override)


		#region Properties
		private TopView TopView
		{ get; set; }


		private static readonly Dictionary<string, Pen> _pens =
							new Dictionary<string, Pen>();
		/// <summary>
		/// Pens for use in TopControl.
		/// </summary>
		internal static Dictionary<string, Pen> TopPens
		{
			get { return _pens; }
		}

		private static readonly Dictionary<string, SolidBrush> _brushes =
							new Dictionary<string, SolidBrush>();
		/// <summary>
		/// Brushes for use in TopControl.
		/// </summary>
		internal static Dictionary<string, SolidBrush> TopBrushes
		{
			get { return _brushes; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="topviewcontrol"></param>
		/// <remarks>There are 2 TopControls - one in TopView and another in
		/// TopRouteView(Top).</remarks>
		internal TopControl(TopView topviewcontrol)
		{
			TopView = topviewcontrol; // beautiful. This pattern should be iterated throughout MapView.

			MainViewOverlay.that.MouseDrag += PathSelectedLozenge;
		}
		#endregion cTor


		#region Resize
		/// <summary>
		/// Called by TopView's resize event or by a straight MapFile change.
		/// </summary>
		/// <param name="width">the width to resize to</param>
		/// <param name="height">the height to resize to</param>
		internal void ResizeObserver(int width, int height)
		{
			if (MapFile != null)
			{
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;
				
				int halfWidthPre = halfWidth;

				width  -= OffsetX * 2; // don't clip the right or bottom tip of the big-lozenge.
				height -= OffsetY * 2;

				if (MapFile.Rows > 0 || MapFile.Cols > 0) // safety vs. div-by-0
				{
					if (height > width / 2) // use width
					{
						halfWidth = width / (MapFile.Rows + MapFile.Cols);

						if (halfWidth % 2 != 0)
							--halfWidth;

						halfHeight = halfWidth / 2;
					}
					else // use height
					{
						halfHeight = height / (MapFile.Rows + MapFile.Cols);
						halfWidth  = halfHeight * 2;
					}
				}

//				if (halfHeight < _lozHeightMin)
//				{
//					halfWidth  = _lozHeightMin * 2;
//					halfHeight = _lozHeightMin;
//				}
				if (halfHeight < 1)
				{
					halfHeight = 1;
					halfWidth  = 2;
				}

				_blobService.HalfWidth  = halfWidth;
				_blobService.HalfHeight = halfHeight;

				_originX = OffsetX + MapFile.Rows * halfWidth;
//				_originY = OffsetY;

				if (halfWidthPre != halfWidth)
				{
					Width  = (MapFile.Rows + MapFile.Cols) * halfWidth;
					Height = (MapFile.Rows + MapFile.Cols) * halfHeight;

					Refresh();
				}
			}
		}

		/// <summary>
		/// Repaths the selected-lozenge on the Resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			PathSelectedLozenge();
		}
		#endregion Resize


		#region Draw
		/// <summary>
		/// Sets the graphics-path for a lozenge-border around all tiles that
		/// are selected or being selected.
		/// </summary>
		private void PathSelectedLozenge()
		{
			Point a = MainViewOverlay.that.GetDragBeg_abs();
			Point b = MainViewOverlay.that.GetDragEnd_abs();

			int halfWidth  = _blobService.HalfWidth;
			int halfHeight = _blobService.HalfHeight;

			var p0 = new Point(
							_originX + (a.X - a.Y) * halfWidth,
							 OffsetY + (a.X + a.Y) * halfHeight);
			var p1 = new Point(
							_originX + (b.X - a.Y) * halfWidth  + halfWidth,
							 OffsetY + (b.X + a.Y) * halfHeight + halfHeight);
			var p2 = new Point(
							_originX + (b.X - b.Y) * halfWidth,
							 OffsetY + (b.X + b.Y) * halfHeight + halfHeight * 2);
			var p3 = new Point(
							_originX + (a.X - b.Y) * halfWidth  - halfWidth,
							 OffsetY + (a.X + b.Y) * halfHeight + halfHeight);

			_lozSelected.Reset();
			_lozSelected.AddLine(p0, p1);
			_lozSelected.AddLine(p1, p2);
			_lozSelected.AddLine(p2, p3);
			_lozSelected.CloseFigure();

			Refresh(); // fast update.
		}

		/// <summary>
		/// Sets the graphics-path for a lozenge-border around the tile that
		/// is currently mouse-overed.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private void PathSelectorLozenge(int x, int y)
		{
			int halfWidth  = _blobService.HalfWidth;
			int halfHeight = _blobService.HalfHeight;

			var p0 = new Point(x,             y);
			var p1 = new Point(x + halfWidth, y + halfHeight);
			var p2 = new Point(x,             y + halfHeight * 2);
			var p3 = new Point(x - halfWidth, y + halfHeight);

			_lozSelector.Reset();
			_lozSelector.AddLine(p0, p1);
			_lozSelector.AddLine(p1, p2);
			_lozSelector.AddLine(p2, p3);
			_lozSelector.CloseFigure();
		}

		/// <summary>
		/// Clears the selector-lozenge.
		/// </summary>
		internal void ClearSelectorLozenge()
		{
			_col =
			_row = -1;
		}

		/// <summary>
		/// Overrides DoubleBufferedControl.OnPaintControl() - ie, OnPaint().
		/// </summary>
		/// <param name="graphics"></param>
		protected override void OnPaintControl(Graphics graphics)
		{
			_graphics = graphics;
			_graphics.FillRectangle(SystemBrushes.Control, ClientRectangle);

			ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);

			if (MapFile != null)
			{
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;

				// draw tile-blobs ->
				MapTile tile;
				for (int
						r = 0,
							startX = _originX,
							startY =  OffsetY;
						r != MapFile.Rows;
						++r,
							startX -= halfWidth,
							startY += halfHeight)
				{
					for (int
							c = 0,
								x = startX,
								y = startY;
							c != MapFile.Cols;
							++c,
								x += halfWidth,
								y += halfHeight)
					{
						if (!(tile = MapFile.GetTile(c,r)).Vacant)
							DrawBlobs(tile, x,y);
					}
				}

				// draw grid-lines ->
				Pen pen;
				for (int i = 0; i <= MapFile.Rows; ++i) // draw horizontal grid-lines (ie. upperleft to lowerright)
				{
					if (i % 10 != 0) pen = TopPens[TopViewOptionables.str_GridLineColor];
					else             pen = TopPens[TopViewOptionables.str_GridLine10Color];

					_graphics.DrawLine(
									pen,
									_originX - i * halfWidth,
									 OffsetY + i * halfHeight,
									_originX + (MapFile.Cols - i) * halfWidth,
									 OffsetY + (MapFile.Cols + i) * halfHeight);
				}

				for (int i = 0; i <= MapFile.Cols; ++i) // draw vertical grid-lines (ie. lowerleft to upperright)
				{
					if (i % 10 != 0) pen = TopPens[TopViewOptionables.str_GridLineColor];
					else             pen = TopPens[TopViewOptionables.str_GridLine10Color];

					_graphics.DrawLine(
									pen,
									_originX + i * halfWidth,
									 OffsetY + i * halfHeight,
									_originX + i * halfWidth  - MapFile.Rows * halfWidth,
									 OffsetY + i * halfHeight + MapFile.Rows * halfHeight);
				}


				// draw the selector lozenge ->
				if (Focused
					&& _col > -1 && _col < MapFile.Cols
					&& _row > -1 && _row < MapFile.Rows)
				{
					PathSelectorLozenge(
									_originX + (_col - _row) * halfWidth,
									 OffsetY + (_col + _row) * halfHeight);
					_graphics.DrawPath(TopPens[TopViewOptionables.str_SelectorColor], _lozSelector);

					// print mouseover location ->
					QuadrantDrawService.SetGraphics(_graphics);
					QuadrantDrawService.PrintSelectorLocation(_loc, Width, Height, MapFile);
				}

				// draw tiles-selected lozenge ->
				if (MainViewOverlay.that.FirstClick)
					_graphics.DrawPath(TopPens[TopViewOptionables.str_SelectedColor], _lozSelected);
			}
		}

		/// <summary>
		/// Draws the floor, westwall, northwall, and content indicator blobs.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void DrawBlobs(
				MapTile tile,
				int x, int y)
		{
			if (TopView.Floor.Checked && tile.Floor != null)
				_blobService.Draw(
								_graphics,
								TopBrushes[TopViewOptionables.str_FloorColor],
								x,y);

			if (TopView.Content.Checked && tile.Content != null)
				_blobService.Draw(
								_graphics,
								ToolContent,
								x,y,
								tile.Content);

			if (TopView.West.Checked && tile.West != null)
				_blobService.Draw(
								_graphics,
								ToolWest,
								x,y,
								tile.West);

			if (TopView.North.Checked && tile.North != null)
				_blobService.Draw(
								_graphics,
								ToolNorth,
								x,y,
								tile.North);
		}
		#endregion Draw


		#region Events (override)
		/// <summary>
		/// Forwards edit-operations or a Mapfile-save to MainView. Can also
		/// perform some Quad-panel operations.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Navigation keys etc. are handled by 'KeyPreview' in
		/// <c><see cref="TopViewForm"/></c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			MouseButtons button;
			int clicks;
			PartType slot;

			switch (e.KeyData)
			{
				case Keys.Q: // select the proper quadrant of the currently selected Tileview-part
					button = MouseButtons.Left;
					clicks = 1;
					slot = (PartType)QuadrantDrawService.QuadrantPart;
					break;

				case Keys.T: // select the TileView-part of the selected quadrant
					button = MouseButtons.Left;
					clicks = 2;
					slot = PartType.Invalid;
					break;

				case Keys.Enter: // place selected TileView-part in selected quadrant
					button = MouseButtons.Right;
					clicks = 1;
					slot = PartType.Invalid;
					break;

				case Keys.Shift | Keys.Delete: // delete part of selected Quadrant-type from a selected tile
					button = MouseButtons.Right;
					clicks = 2;
					slot = PartType.Invalid;
					break;

				default:
					MainViewOverlay.that.Edit(e);
					return;
			}

			TopView.QuadrantControl.doMouseDown(
											new MouseEventArgs(button, clicks, 0,0, 0),
											slot);
//			base.OnKeyDown(e);
		}


		/// <summary>
		/// Handles the MouseDown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (   _col > -1 && _col < MapFile.Cols
				&& _row > -1 && _row < MapFile.Rows)
			{
				SelectMapLocation(); // NOTE: Will select a tile on any mousebutton down.

				switch (e.Button)
				{
					case MouseButtons.Left:
						switch (e.Clicks)
						{
							case 1:
								_isMouseDrag = true;
								break;

							case 2:
								TopView.QuadrantControl.Clicker(MouseButtons.Left, 2);
								break;
						}
						break;

					case MouseButtons.Right:
					{
						int clicks;
						if (TopView.Optionables.EnableRightClickWaitTimer)
						{
							clicks = 1;
						}
						else
							clicks = e.Clicks;

						TopView.QuadrantControl.Clicker(MouseButtons.Right, clicks);
						break;
					}
				}
			}
//			base.OnMouseDown(e);
		}

		/// <summary>
		/// Selects a location when a tile is clicked.
		/// @note Helper for OnMouseDown().
		/// </summary>
		private void SelectMapLocation()
		{
			ObserverManager.RouteView   .Control     .DeselectNode(false);
			ObserverManager.TopRouteView.ControlRoute.DeselectNode(false);

			MainViewOverlay.that._keyDeltaX =
			MainViewOverlay.that._keyDeltaY = 0;

			// IMPORTANT: as long as MainViewOverlay.OnLocationSelectedMain()
			// fires before the secondary viewers' OnLocationSelectedObserver()
			// functions fire, FirstClick is set okay by the former.
			//
			// TODO: Make a flag of FirstClick in MapFile where Location is really
			// set, and where all these OnLocationSelected events actually fire out of!
//			MainViewOverlay.that.FirstClick = true;

			MapFile.Location = new MapLocation( // fire LocationSelected
											_col, _row,
											MapFile.Level);
			MainViewOverlay.that.ProcessSelection(_loc, _loc);
		}

		/// <summary>
		/// Handles the MouseUp event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDrag = false;
//			base.OnMouseUp(e);
		}

		/// <summary>
		/// Handles the MouseMove event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			SetTileLocation(e.X, e.Y);
			if (_loc.X != _col || _loc.Y != _row)
			{
				_col = _loc.X;
				_row = _loc.Y;

				if (_isMouseDrag)
				{
					MainViewOverlay overlay = MainViewOverlay.that;

					overlay._keyDeltaX = _loc.X - overlay.DragBeg.X; // these are in case user stops a mouse-drag
					overlay._keyDeltaY = _loc.Y - overlay.DragBeg.Y; // and resumes selection using keyboard.

					overlay.ProcessSelection(overlay.DragBeg, _loc);
				}
				else
					Invalidate();
			}
//			base.OnMouseMove(e);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Converts a position from screen-coordinates to tile-location.
		/// </summary>
		/// <param name="x">the x-position of the mouse-cursor in pixels wrt/ this control's area</param>
		/// <param name="y">the y-position of the mouse-cursor in pixels wrt/ this control's area</param>
		/// <returns></returns>
		private void SetTileLocation(int x, int y)
		{
			x -= _originX;
			y -=  OffsetY;

			double halfWidth  = (double)_blobService.HalfWidth;
			double halfHeight = (double)_blobService.HalfHeight;

			double x1 = x / (halfWidth  * 2)
					  + y / (halfHeight * 2);
			double y1 = (y * 2 - x) / (halfWidth * 2);

			_loc = new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(y1));
		}
		#endregion Methods
	}
}
