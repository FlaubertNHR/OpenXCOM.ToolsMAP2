using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// @note This is not a Panel. It is a Control shown inside of a Panel.
	/// </summary>
	internal class TopPanel
		:
			MapObserverControl_Top // DoubleBufferedControl, IMapObserver
	{
		#region Fields (static)
		private const int OffsetX = 2; // these are the offsets between the
		private const int OffsetY = 3; // panel border and the lozenge-tip(s).
		#endregion Fields (static)


		#region Fields
		private readonly GraphicsPath _lozSelector = new GraphicsPath(); // mouse-over cursor lozenge
		private readonly GraphicsPath _lozSelected = new GraphicsPath(); // selected tile or tiles being drag-selected

		private int _col = -1; // these track the location of the mouse-cursor
		private int _row = -1;

		private int _originX;	// since the lozenge is drawn with its Origin at 0,0 of the
								// panel, the entire lozenge needs to be displaced to the right.
//		private int _originY;	// But this isn't really used. It's set to 'OffsetY' and stays that way. -> done.
		#endregion Fields


		#region Properties (override)
		[Browsable(false)]
		public override MapFileBase MapBase
		{
			set
			{
				base.MapBase = value;

				_blobService.HalfWidth = 8;

				ResizeObserver(Parent.Width, Parent.Height);
				Refresh();
			}
		}
		#endregion Properties (override)


		#region Properties
		private TopView TopView
		{ get; set; }


		private readonly DrawBlobService _blobService = new DrawBlobService();
		internal protected DrawBlobService BlobService
		{
			get { return _blobService; }
		}

		private static readonly Dictionary<string, Pen> _pens =
							new Dictionary<string, Pen>();
		/// <summary>
		/// Pens for use in TopPanel.
		/// @note The identifier 'Pens' could cause a clash w/ System.Drawing
		/// </summary>
		internal static Dictionary<string, Pen> Pens
		{
			get { return _pens; }
		}

		private static readonly Dictionary<string, SolidBrush> _brushes =
							new Dictionary<string, SolidBrush>();
		/// <summary>
		/// Brushes for use in TopPanel.
		/// @note The identifier 'Brushes' could cause a clash w/ System.Drawing
		/// </summary>
		internal static Dictionary<string, SolidBrush> Brushes
		{
			get { return _brushes; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of TopPanel. Is NOT a panel.
		/// </summary>
		internal protected TopPanel(TopView control)
		{
			TopView = control; // beautiful. This pattern should be iterated throughout MapView.

			MainViewOverlay.that.MouseDrag += PathSelectedLozenge;
		}
		#endregion cTor


		#region Resize
		/// <summary>
		/// Called by TopView's resize event or by a straight MapBase change.
		/// </summary>
		/// <param name="width">the width to resize to</param>
		/// <param name="height">the height to resize to</param>
		internal protected void ResizeObserver(int width, int height)
		{
			if (MapBase != null)
			{
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;
				
				int halfWidthPre = halfWidth;

				width  -= OffsetX * 2; // don't clip the right or bottom tip of the big-lozenge.
				height -= OffsetY * 2;

				if (MapBase.MapSize.Rows > 0 || MapBase.MapSize.Cols > 0) // safety vs. div-by-0
				{
					if (height > width / 2) // use width
					{
						halfWidth = width / (MapBase.MapSize.Rows + MapBase.MapSize.Cols);

						if (halfWidth % 2 != 0)
							--halfWidth;

						halfHeight = halfWidth / 2;
					}
					else // use height
					{
						halfHeight = height / (MapBase.MapSize.Rows + MapBase.MapSize.Cols);
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

				_originX = OffsetX + MapBase.MapSize.Rows * halfWidth;
//				_originY = OffsetY;

				if (halfWidthPre != halfWidth)
				{
					Width  = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * halfWidth;
					Height = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * halfHeight;

					Refresh();
				}
			}
		}

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
		internal protected void PathSelectedLozenge()
		{
			var a = MainViewOverlay.that.GetDragBeg_abs();
			var b = MainViewOverlay.that.GetDragEnd_abs();

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

			Refresh();
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

		internal void ClearSelectorLozenge()
		{
			_col =
			_row = -1;
		}

		/// <summary>
		/// Overrides DoubleBufferedControl.RenderGraphics() - ie, OnPaint().
		/// </summary>
		/// <param name="graphics"></param>
		protected override void RenderGraphics(Graphics graphics)
		{
			graphics.FillRectangle(SystemBrushes.Control, ClientRectangle);

			ControlPaint.DrawBorder3D(graphics, ClientRectangle, Border3DStyle.Etched);

			if (MapBase != null)
			{
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;

				// draw tile-blobs ->
				MapTile tile;
				for (int
						r = 0,
							startX = _originX,
							startY = OffsetY;
						r != MapBase.MapSize.Rows;
						++r,
							startX -= halfWidth,
							startY += halfHeight)
				{
					for (int
							c = 0,
								x = startX,
								y = startY;
							c != MapBase.MapSize.Cols;
							++c,
								x += halfWidth,
								y += halfHeight)
					{
						if ((tile = MapBase[r,c]) != null)
							DrawBlobs(tile, graphics, x,y);
					}
				}

				// draw grid-lines ->
				Pen pen;
				for (int i = 0; i <= MapBase.MapSize.Rows; ++i) // draw horizontal grid-lines (ie. upperleft to lowerright)
				{
					if (i % 10 != 0) pen = TopPanel.Pens[TopViewOptionables.str_GridLineColor];
					else             pen = TopPanel.Pens[TopViewOptionables.str_GridLine10Color];

					graphics.DrawLine(
									pen,
									_originX - i * halfWidth,
									OffsetY  + i * halfHeight,
									_originX + (MapBase.MapSize.Cols - i) * halfWidth,
									OffsetY  + (MapBase.MapSize.Cols + i) * halfHeight);
				}

				for (int i = 0; i <= MapBase.MapSize.Cols; ++i) // draw vertical grid-lines (ie. lowerleft to upperright)
				{
					if (i % 10 != 0) pen = TopPanel.Pens[TopViewOptionables.str_GridLineColor];
					else             pen = TopPanel.Pens[TopViewOptionables.str_GridLine10Color];

					graphics.DrawLine(
									pen,
									_originX + i * halfWidth,
									OffsetY  + i * halfHeight,
									_originX + i * halfWidth  - MapBase.MapSize.Rows * halfWidth,
									OffsetY  + i * halfHeight + MapBase.MapSize.Rows * halfHeight);
				}


				// draw the selector lozenge ->
				if (Focused
					&& _col > -1 && _col < MapBase.MapSize.Cols
					&& _row > -1 && _row < MapBase.MapSize.Rows)
				{
					PathSelectorLozenge(
									_originX + (_col - _row) * halfWidth,
									OffsetY  + (_col + _row) * halfHeight);
					graphics.DrawPath(TopPanel.Pens[TopViewOptionables.str_SelectorColor], _lozSelector);
				}

				// draw tiles-selected lozenge ->
				if (MainViewOverlay.that.FirstClick)
					graphics.DrawPath(TopPanel.Pens[TopViewOptionables.str_SelectedColor], _lozSelected);
			}
		}


		internal static ColorTool ToolWest;
		internal static ColorTool ToolNorth;
		internal static ColorTool ToolContent;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void DrawBlobs(
				MapTile tile,
				Graphics graphics,
				int x, int y)
		{
			if (TopView.Floor.Checked && tile.Floor != null)
				BlobService.DrawFloor(
									graphics,
									TopPanel.Brushes[TopViewOptionables.str_FloorColor],
									x, y);

			if (TopView.Content.Checked && tile.Content != null)
				BlobService.DrawContent(
									graphics,
									ToolContent,
									x, y,
									tile.Content);

			if (TopView.West.Checked && tile.West != null)
				BlobService.DrawContent(
									graphics,
									ToolWest,
									x, y,
									tile.West);

			if (TopView.North.Checked && tile.North != null)
				BlobService.DrawContent(
									graphics,
									ToolNorth,
									x, y,
									tile.North);
		}
		#endregion Draw


		#region Events (override)
		/// <summary>
		/// Forwards edit-operations or a Mapfile-save to MainView. Can also
		/// perform some Quad-panel operations.
		/// @note Navigation keys are handled by 'KeyPreview' at the form level.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			MouseButtons button;
			int clicks;

			switch (e.KeyCode)
			{
				case Keys.Enter: // place selected TileView-part in selected quadrant
				{
					button = MouseButtons.Right;
					clicks = 1;
					break;
				}

				case Keys.T: // select the TileView-part of the selected quadrant
				{
					button = MouseButtons.Left;
					clicks = 2;
					break;
				}

				case Keys.Delete: // delete selected Quadrant-type from a selected tile
					if (e.Shift)
					{
						button = MouseButtons.Right;
						clicks = 2;
					}
					else
						goto default;
					break;

				default:
					MainViewOverlay.that.Edit(e);
					return;
			}

			TopView.QuadrantPanel.doMouseDown(
											new MouseEventArgs(button, clicks, 0,0, 0),
											QuadrantType.None);

//			base.OnKeyDown(e);
		}


		private bool _isMouseDrag;

		/// <summary>
		/// Handles the MouseDown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			switch (e.Button)
			{
				case MouseButtons.Left:
				case MouseButtons.Right:
				{
					var loc = GetTileLocation(e.X, e.Y);
					if (   loc.X > -1 && loc.X < MapBase.MapSize.Cols
						&& loc.Y > -1 && loc.Y < MapBase.MapSize.Rows)
					{
						MainViewOverlay.that._keyDeltaX =
						MainViewOverlay.that._keyDeltaY = 0;

						// as long as MainViewOverlay.OnSelectLocationMain()
						// fires before the secondary viewers' OnSelectLocationObserver()
						// functions fire, FirstClick is set okay by the former.
						//
						// TODO: Make a flag of FirstClick in MapFileBase where Location is really
						// set, and where all these OnLocationSelected events actually fire out of!
//						MainViewOverlay.that.FirstClick = true;

						MapBase.Location = new MapLocation( // fire SelectLocation
														loc.Y, loc.X,
														MapBase.Level);
						_isMouseDrag = true;
						MainViewOverlay.that.ProcessSelection(loc,loc);
					}
					break;
				}
			}

			switch (e.Button)
			{
				case MouseButtons.Left:
					if (e.Clicks == 2)
					{
						ObserverManager.TopView     .Control   .QuadrantPanel.Operate(MouseButtons.Left, 2);
						ObserverManager.TopRouteView.ControlTop.QuadrantPanel.Operate(MouseButtons.Left, 2);
					}
					break;

				case MouseButtons.Right:
					if (MainViewOverlay.that.FirstClick)
					{
						ObserverManager.TopView     .Control   .QuadrantPanel.Operate(MouseButtons.Right, 1);
						ObserverManager.TopRouteView.ControlTop.QuadrantPanel.Operate(MouseButtons.Right, 1);
					}
					break;
			}
//			base.OnMouseDown(e);
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
			var loc = GetTileLocation(e.X, e.Y);
			if (loc.X != _col || loc.Y != _row)
			{
				_col = loc.X;
				_row = loc.Y;

				if (_isMouseDrag)
				{
					var overlay = MainViewOverlay.that;

					overlay._keyDeltaX = loc.X - overlay.DragBeg.X; // these are in case user stops a mouse-drag
					overlay._keyDeltaY = loc.Y - overlay.DragBeg.Y; // and resumes selection using keyboard.

					overlay.ProcessSelection(overlay.DragBeg, loc);
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
		private Point GetTileLocation(int x, int y)
		{
			x -= _originX;
			y -=  OffsetY;

			double halfWidth  = (double)_blobService.HalfWidth;
			double halfHeight = (double)_blobService.HalfHeight;

			double x1 = x / (halfWidth  * 2)
					  + y / (halfHeight * 2);
			double y1 = (y * 2 - x) / (halfWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(y1));
		}
		#endregion Methods
	}
}
