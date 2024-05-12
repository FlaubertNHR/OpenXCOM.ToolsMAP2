using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The top region of <c><see cref="Observers.TopView"/></c>.
	/// </summary>
	internal sealed class TopControl
		:
			DoubleBufferedControl
	{
		/// <summary>
		/// Disposes this <c>TopControl's</c> disposables.
		/// </summary>
		/// <remarks>Do NOT use <c>public void Dispose()</c> or else you'll have
		/// one Fuck of a time trying to trace usage.
		/// Use <c>public void Dispose()</c> only for Designer code w/
		/// <c>components</c>. Thank yourself for heeding this piece of ornery
		/// advice later.</remarks>
		internal void DisposeControl()
		{
			//DSShared.Logfile.Log("TopControl.DisposeControl()");
			_lozSelector.Dispose();
			_lozSelected.Dispose();

			_blobService.Dispose();

			PanelFill   .Dispose();
		}


		#region Fields (static)
		private const int OffsetX = 2; // these are the offsets between the
		private const int OffsetY = 3; // panel border and the lozenge-tip(s).

		internal static BlobColorTool ToolWest;
		internal static BlobColorTool ToolNorth;
		internal static BlobColorTool ToolContent;

		internal static readonly SolidBrush PanelFill = new SolidBrush(TopViewOptionables.def_PanelBackcolor);

		private const byte LOFTID_Max_ufo  = 111;
		private const byte LOFTID_Max_tftd = 113;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;

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


		#region Properties
		private TopView TopView
		{ get; set; }
		#endregion Properties


		#region Properties (static)
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
		#endregion Properties (static)


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
			if (_file != null)
			{
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;
				
				int halfWidthPre = halfWidth;

				width  -= OffsetX * 2; // don't clip the right or bottom tip of the big-lozenge.
				height -= OffsetY * 2;

				if (_file.Rows > 0 || _file.Cols > 0) // safety vs. div-by-0
				{
					if (height > width / 2) // use width
					{
						halfWidth = width / (_file.Rows + _file.Cols);

						if (halfWidth % 2 != 0)
							--halfWidth;

						halfHeight = halfWidth / 2;
					}
					else // use height
					{
						halfHeight = height / (_file.Rows + _file.Cols);
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

				_originX = OffsetX + _file.Rows * halfWidth;
//				_originY = OffsetY;

				if (halfWidthPre != halfWidth)
				{
					Width  = (_file.Rows + _file.Cols) * halfWidth;
					Height = (_file.Rows + _file.Cols) * halfHeight;

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
		/// <seealso cref="ClearSelectorLozengeStatic()"><c>ClearSelectorLozengeStatic()</c></seealso>
		internal void ClearSelectorLozenge()
		{
			_col = _row = -1;
		}

		/// <summary>
		/// Synchs
		/// <c><see cref="ClearSelectorLozenge()">ClearSelectorLozenge()</see></c>
		/// in <c>TopView</c> and <c>TopRouteView</c>.
		/// </summary>
		internal static void ClearSelectorLozengeStatic()
		{
			ObserverManager.TopView     .Control   .TopControl.ClearSelectorLozenge();
			ObserverManager.TopRouteView.ControlTop.TopControl.ClearSelectorLozenge();
		}


		/// <summary>
		/// Overrides the <c>Paint</c> event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			_graphics = e.Graphics;
			_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			_graphics.FillRectangle(PanelFill, ClientRectangle);

			if (_file != null)
			{
				int halfWidth  = _blobService.HalfWidth;
				int halfHeight = _blobService.HalfHeight;

				// draw tile-blobs ->
				MapTile tile;
				for (int
						r = 0,
							startX = _originX,
							startY =  OffsetY;
						r != _file.Rows;
						++r,
							startX -= halfWidth,
							startY += halfHeight)
				{
					for (int
							c = 0,
								x = startX,
								y = startY;
							c != _file.Cols;
							++c,
								x += halfWidth,
								y += halfHeight)
					{
						if (!(tile = _file.GetTile(c,r)).Vacant)
							DrawBlobs(tile, x,y);
					}
				}

				// draw grid-lines ->
				Pen pen;
				for (int i = 0; i <= _file.Rows; ++i) // draw horizontal grid-lines (ie. upperleft to lowerright)
				{
					if (i % 10 != 0) pen = TopPens[TopViewOptionables.str_GridLineColor];
					else             pen = TopPens[TopViewOptionables.str_GridLine10Color];

					_graphics.DrawLine(
									pen,
									_originX - i * halfWidth,
									 OffsetY + i * halfHeight,
									_originX + (_file.Cols - i) * halfWidth,
									 OffsetY + (_file.Cols + i) * halfHeight);
				}

				for (int i = 0; i <= _file.Cols; ++i) // draw vertical grid-lines (ie. lowerleft to upperright)
				{
					if (i % 10 != 0) pen = TopPens[TopViewOptionables.str_GridLineColor];
					else             pen = TopPens[TopViewOptionables.str_GridLine10Color];

					_graphics.DrawLine(
									pen,
									_originX + i * halfWidth,
									 OffsetY + i * halfHeight,
									_originX + i * halfWidth  - _file.Rows * halfWidth,
									 OffsetY + i * halfHeight + _file.Rows * halfHeight);
				}


				// draw the selector lozenge ->
				if (Focused
					&& _col > -1 && _col < _file.Cols
					&& _row > -1 && _row < _file.Rows)
				{
					PathSelectorLozenge(
									_originX + (_col - _row) * halfWidth,
									 OffsetY + (_col + _row) * halfHeight);
					_graphics.DrawPath(TopPens[TopViewOptionables.str_SelectorColor], _lozSelector);

					// print mouseover location ->
					QuadrantDrawService.PrintSelectorLocation(_graphics, _loc, Width, Height, _file);
				}

				// draw tiles-selected lozenge ->
				if (MainViewOverlay.that.FirstClick)
					_graphics.DrawPath(TopPens[TopViewOptionables.str_SelectedColor], _lozSelected);
			}

			ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);
		}


		private const int ELOFT_n = 0; // bitwise flags for DrawBlobs() ->
		private const int ELOFT_F = 1; // floor
		private const int ELOFT_C = 2; // content
		private const int ELOFT_W = 4; // west
		private const int ELOFT_N = 8; // north

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
			int size = TopView.Optionables.ExtendedLoftIndicators;
			byte cutoff = _file.Descriptor.GroupType == GroupType.Tftd ? LOFTID_Max_tftd
																	   : LOFTID_Max_ufo;
			int elofts = ELOFT_n;

			if (!TopView.it_Floor.Checked && tile.Floor != null)
			{
				byte loftid;
				if (tile.Floor.Record.LoftList != null) // crippled tileparts have an invalid 'LoftList'
				{
					loftid = tile.Floor.Record.LoftList[0];
				}
				else
					loftid = Byte.MaxValue;

				string key;
				if (loftid == 0) // blank LoFT, draw light floor color.
					key = TopViewOptionables.str_FloorColorLight;
				else
					key = TopViewOptionables.str_FloorColor;

				_blobService.DrawFloor(
									_graphics,
									TopBrushes[key],
									x,y,
									loftid);

				if (tile.Floor.Record.GravLift != 0) // draw GravLift floor as content-part
					_blobService.DrawContentOrWall(
												_graphics,
												ToolContent,
												x,y,
												tile.Floor);

				if (size != 0 && BlobTypeService.hasExtendedLofts(tile.Floor, cutoff))
					elofts |= ELOFT_F;
			}

			if (!TopView.it_Content.Checked && tile.Content != null)
			{
				_blobService.DrawContentOrWall(
											_graphics,
											ToolContent,
											x,y,
											tile.Content);
				if (size != 0 && BlobTypeService.hasExtendedLofts(tile.Content, cutoff))
					elofts |= ELOFT_C;
			}

			if (!TopView.it_West.Checked && tile.West != null)
			{
				_blobService.DrawContentOrWall(
											_graphics,
											ToolWest,
											x,y,
											tile.West);
				if (size != 0 && BlobTypeService.hasExtendedLofts(tile.West, cutoff))
					elofts |= ELOFT_W;
			}

			if (!TopView.it_North.Checked && tile.North != null)
			{
				_blobService.DrawContentOrWall(
											_graphics,
											ToolNorth,
											x,y,
											tile.North);
				if (size != 0 && BlobTypeService.hasExtendedLofts(tile.North, cutoff))
					elofts |= ELOFT_N;
			}

			if (size != 0 && elofts != ELOFT_n) // draw a small indicator for each tilepart that has custom LoFT entry(s) ->
			{
				var brush = new SolidBrush(TopView.Optionables.GridLine10Color); // TODO: instantiate this brush on Load

				int pos = size / 2;

				if ((elofts & ELOFT_F) != ELOFT_n)
					_graphics.FillRectangle(brush, new Rectangle(
															x - pos,
															y + _blobService.HalfHeight * 2 - pos - 4,
															size,size));

				if ((elofts & ELOFT_C) != ELOFT_n)
					_graphics.FillRectangle(brush, new Rectangle(
															x - pos,
															y + _blobService.HalfHeight - pos,
															size,size));

				if ((elofts & ELOFT_W) != ELOFT_n)
					_graphics.FillRectangle(brush, new Rectangle(
															x - _blobService.HalfWidth  / 2 + pos + 2,
															y + _blobService.HalfHeight / 2 - pos + 1,
															size,size));

				if ((elofts & ELOFT_N) != ELOFT_n)
					_graphics.FillRectangle(brush, new Rectangle(
															x + _blobService.HalfWidth  / 2 - size - 6,
															y + _blobService.HalfHeight / 2 - pos + 1,
															size,size));

				brush.Dispose();
			}
		}
		#endregion Draw


		#region Events (override)
		const int WM_MOUSEACTIVATE = 0x21;

		/// <summary>
		/// Allows an inactive TopView window to accept click(s) on this
		/// <c>TopControl</c>.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEACTIVATE) // && CanFocus && !Focused
			{
				Focus();

				Point pos = PointToClient(Cursor.Position);
				SetTileLocation(pos.X, pos.Y);

				_col = _loc.X;
				_row = _loc.Y;
			}
			base.WndProc(ref m);
		}

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
		}


		/// <summary>
		/// Handles the MouseDown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (_file != null
				&& _col > -1 && _col < _file.Cols
				&& _row > -1 && _row < _file.Rows)
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
		}

		/// <summary>
		/// Selects a location when a tile is clicked.
		/// </summary>
		/// <remarks>Helper for <c><see cref="OnMouseDown()"/></c>.</remarks>
		private void SelectMapLocation()
		{
			RouteView.DeselectNodeStatic(true);

			MainViewOverlay.that._keyDeltaX =
			MainViewOverlay.that._keyDeltaY = 0;

			// IMPORTANT: as long as MainViewOverlay.OnLocationSelectedMain()
			// fires before the secondary viewers' OnLocationSelectedObserver()
			// functions fire, FirstClick is set okay by the former.
			//
			// TODO: Make a flag of FirstClick in MapFile where Location is really
			// set, and where all these LocationSelected events actually fire out of!
//			MainViewOverlay.that.FirstClick = true;

			_file.Location = new MapLocation( // fire LocationSelected
										_col, _row,
										_file.Level);
			MainViewOverlay.that.ProcessSelection(_loc, _loc);
		}

		/// <summary>
		/// Handles the MouseUp event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDrag = false;
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
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Sets <c><see cref="_file"/></c>.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		internal void SetMapfile(MapFile file)
		{
			if ((_file = file) != null)
			{
				_blobService.HalfWidth = 8;
			}

			ResizeObserver(Parent.Width, Parent.Height);
			Refresh();
		}

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
