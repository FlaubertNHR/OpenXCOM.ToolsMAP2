using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed class RoutePanel
		:
			RoutePanelParent
	{
		private Point _pos = new Point(-1, -1);
		public Point Pos
		{
			set { _pos = value; }
		}

		private readonly DrawContentService _drawContentService = new DrawContentService();

		private readonly Font _fontOverlay = new Font("Verdana", 7F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
		private readonly Font _fontRose    = new Font("Courier New", 22, FontStyle.Bold);

		private ColorTools _penWalls;
		private ColorTools _brushContent;

		private Graphics     _graphics;
		private GraphicsPath _layerFill = new GraphicsPath();


//		public void Calculate()
//		{
//			OnResize(null);
//		}

		/// <summary>
		/// You know the drill ... Paint it, Black
		/// black as night
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			_graphics = e.Graphics;

//			try // TODO: why do i get the impression that many of the try/catch blocks can and should be replaced w/ standard code.
//			{
			if (MapFile != null)
			{
				DrawWallsAndContent();
				DrawUnselectedLink();
				DrawSelectedLink();
				DrawNodes();
				DrawGridLines();
				DrawRose();
				DrawInfoOverlay();
			}
//			}
//			catch (Exception ex)
//			{
//				g.FillRectangle(new SolidBrush(Color.Black), g.ClipBounds);
//				g.DrawString(
//							ex.Message,
//							Font,
//							new SolidBrush(Color.White),
//							8, 8);
//				throw;
//			}
		}

		#region Draw Methods
		/// <summary>
		/// Draws any wall and/or content indicators.
		/// </summary>
		private void DrawWallsAndContent()
		{
			if (_penWalls == null)
				_penWalls = new ColorTools(RoutePens["WallColor"]);

			if (_brushContent == null)
				_brushContent = new ColorTools(RouteBrushes["ContentColor"], _penWalls.Pen.Width);

			_drawContentService.HalfWidth  = DrawAreaWidth;
			_drawContentService.HalfHeight = DrawAreaHeight;

			var map = MapFile;
			for (int
					r = 0,
						startX = Origin.X,
						startY = Origin.Y;
					r != map.MapSize.Rows;
					++r,
						startX -= DrawAreaWidth,
						startY += DrawAreaHeight)
			{
				for (int
						c = 0,
							x = startX,
							y = startY;
						c != map.MapSize.Cols;
						++c,
							x += DrawAreaWidth,
							y += DrawAreaHeight)
				{
					if (map[r, c] != null)
					{
						var tile = (XCMapTile)map[r, c];

						if (tile.North != null)
							_drawContentService.DrawContent(_graphics, _penWalls, x, y, tile.North);

						if (tile.West != null)
							_drawContentService.DrawContent(_graphics, _penWalls, x, y, tile.West);

						if (tile.Content != null)
							_drawContentService.DrawContent(_graphics, _brushContent, x, y, tile.Content);
					}
				}
			}
		}

		/// <summary>
		/// Draws an unselected link-line.
		/// </summary>
		private void DrawUnselectedLink()
		{
			var pen = RoutePens["UnselectedLinkColor"];

			for (int
					rSrc = 0,
						x = Origin.X,
						y = Origin.Y;
					rSrc != MapFile.MapSize.Rows;
					++rSrc,
						x -= DrawAreaWidth,
						y += DrawAreaHeight)
			{
				for (int
						cSrc = 0,
							xSrc = x,
							ySrc = y;
						cSrc != MapFile.MapSize.Cols;
						++cSrc,
							xSrc += DrawAreaWidth,
							ySrc += DrawAreaHeight)
				{
					if (MapFile[rSrc, cSrc] != null)
					{
						var node = ((XCMapTile)MapFile[rSrc, cSrc]).Node;
						if (node != null)
						{
//							_layerFill.Reset();
//							_layerFill.AddLine(
//											x,                 y,
//											x + DrawAreaWidth, y + DrawAreaHeight);
//							_layerFill.AddLine(
//											x + DrawAreaWidth, y + DrawAreaHeight,
//											x,                 y + DrawAreaHeight * 2);
//							_layerFill.AddLine(
//											x,                 y + DrawAreaHeight * 2,
//											x - DrawAreaWidth, y + DrawAreaHeight);
//							_layerFill.CloseFigure();

							DrawLinkLine(xSrc, ySrc, node, pen, false);
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws a selected link-line.
		/// </summary>
		private void DrawSelectedLink()
		{
			int cSrc = ClickPoint.X;
			int rSrc = ClickPoint.Y;

			if (cSrc > -1 && rSrc > -1)
			{
				var node = ((XCMapTile)MapFile[rSrc, cSrc]).Node;
				if (node != null)
				{
					int xSrc = Origin.X + (cSrc - rSrc)     * DrawAreaWidth;
					int ySrc = Origin.Y + (cSrc + rSrc + 1) * DrawAreaHeight;

					var pen = RoutePens["SelectedLinkColor"];

					DrawLinkLine(xSrc, ySrc, node, pen, true);
				}
			}
		}

		private void DrawLinkLine(int xSrc, int ySrc, RouteNode node, Pen pen, bool selected)
		{
			int xDst;
			int yDst;
			int yOffset = (!selected) ? DrawAreaHeight
									  : 0;

			for (int i = 0; i != RouteNode.LinkSlots; ++i)
			{
				xDst = -1;
				yDst =  0;

				var link = node[i] as Link;
				switch (link.Destination)
				{
					case Link.NotUsed:
						break;

					case Link.ExitNorth:
						xDst = Width;
						break;

					case Link.ExitEast:
						xDst = Width;
						yDst = Height;
						break;

					case Link.ExitSouth:
						xDst = 0;
						yDst = Height;
						break;

					case Link.ExitWest:
						xDst = 0;
						break;

					default:
						if (   MapFile.RouteFile[link.Destination] != null
							&& MapFile.RouteFile[link.Destination].Height == MapFile.CurrentHeight)
						{
							int cDst = MapFile.RouteFile[link.Destination].Col;
							int rDst = MapFile.RouteFile[link.Destination].Row;

							xDst = Origin.X + (cDst - rDst)     * DrawAreaWidth;
							yDst = Origin.Y + (cDst + rDst + 1) * DrawAreaHeight;
						}
						break;
				}

				if (xDst != -1)
					_graphics.DrawLine(
									pen,
									xSrc, ySrc + yOffset, // unselected nodes need an offset
									xDst, yDst);
			}
		}


		private const int NODE_VAL_MAX = 12;

		private bool _brushes;
		private SolidBrush _brushSelected;
		private SolidBrush _brushUnselected;
		private SolidBrush _brushSpawn;

		/// <summary>
		/// Draws the nodes.
		/// </summary>
		private void DrawNodes()
		{
			if (!_brushes)
			{
				_brushes = true;

				_brushSelected   = RouteBrushes["SelectedNodeColor"];
				_brushUnselected = RouteBrushes["UnselectedNodeColor"];
				_brushSpawn      = RouteBrushes["SpawnNodeColor"];

//				_brushSelected.Color   = Color.FromArgb(200, _brushSelected.Color);
//				_brushUnselected.Color = Color.FromArgb(200, _brushUnselected.Color);
//				_brushSpawn.Color      = Color.FromArgb(200, _brushSpawn.Color);
			}

			var startX = Origin.X;
			var startY = Origin.Y;

			for (int r = 0; r != MapFile.MapSize.Rows; ++r)
			{
				for (int
						c = 0,
							x = startX,
							y = startY;
						c != MapFile.MapSize.Cols;
						++c,
							x += DrawAreaWidth,
							y += DrawAreaHeight)
				{
					var tile = MapFile[r, c] as XCMapTile;
					if (tile != null)
					{
						var node = tile.Node;
						if (node != null)
						{
							_layerFill.Reset();
							_layerFill.AddLine(
											x,                 y,
											x + DrawAreaWidth, y + DrawAreaHeight);
							_layerFill.AddLine(
											x + DrawAreaWidth, y + DrawAreaHeight,
											x,                 y + DrawAreaHeight * 2);
							_layerFill.AddLine(
											x,                 y + DrawAreaHeight * 2,
											x - DrawAreaWidth, y + DrawAreaHeight);
							_layerFill.CloseFigure();


							if (r == ClickPoint.Y && c == ClickPoint.X)
							{
								_graphics.FillPath(_brushSelected, _layerFill);
							}
							else if (node.Spawn != SpawnUsage.NoSpawn)
							{
								_graphics.FillPath(_brushSpawn, _layerFill);
							}
							else
								_graphics.FillPath(_brushUnselected, _layerFill);


							for (int i = 0; i != RouteNode.LinkSlots; ++i)
							{
								var link = node[i] as Link;
								switch (link.Destination)
								{
									case Link.NotUsed:
									case Link.ExitEast:
									case Link.ExitNorth:
									case Link.ExitSouth:
									case Link.ExitWest:
										break;

									default:
										if (   MapFile.RouteFile[link.Destination] != null
											&& MapFile.RouteFile[link.Destination].Height < MapFile.CurrentHeight)
										{
											_graphics.DrawLine(
															RoutePens["UnselectedLinkColor"],
															x, y,
															x, y + DrawAreaHeight * 2);
										}
										else if (MapFile.RouteFile[link.Destination] != null
											&&   MapFile.RouteFile[link.Destination].Height > MapFile.CurrentHeight)
										{
											_graphics.DrawLine(
															RoutePens["UnselectedLinkColor"],
															x - DrawAreaWidth, y + DrawAreaHeight,
															x + DrawAreaWidth, y + DrawAreaHeight);
										}
										break;
								}
							}

//							if (DrawAreaHeight >= NODE_VAL_MAX)
//							{
							int infoboxX = x - DrawAreaWidth / 2;
							int infoboxY = y + DrawAreaHeight - NODE_VAL_MAX / 2;

							var nodePatrolPriority = (int)node.Priority;
							DrawBox(
									infoboxX,
									infoboxY,
									nodePatrolPriority,
									NODE_VAL_MAX,
									Brushes.CornflowerBlue);

							var nodeSpawnWeight = (int)node.Spawn;
							DrawBox(
									infoboxX + 3,
									infoboxY,
									nodeSpawnWeight,
									NODE_VAL_MAX,
									Brushes.Firebrick);
//							}
						}
					}
				}
				startX -= DrawAreaWidth;
				startY += DrawAreaHeight;
			}
		}

		/// <summary>
		/// Helper for DrawNodes().
		/// </summary>
		/// <param name="boxX"></param>
		/// <param name="boxY"></param>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="color"></param>
		private void DrawBox(
				int boxX,
				int boxY,
				int value,
				int max,
				Brush color)
		{
			_graphics.FillRectangle(
								Brushes.Gray,
								boxX, boxY,
								3, NODE_VAL_MAX);
			_graphics.DrawRectangle(
								Pens.Black,
								boxX, boxY,
								3, NODE_VAL_MAX);

			if (value > 0)
			{
				value = (value > max) ? NODE_VAL_MAX
									  : (int)(Math.Ceiling((double)value / max * NODE_VAL_MAX));
				_graphics.FillRectangle(
									color,
									boxX + 1,
									boxY + (NODE_VAL_MAX - value) - 1,
									2,
									value + 1);
			}
		}

		/// <summary>
		/// Draws the grid-lines.
		/// </summary>
		private void DrawGridLines()
		{
			var map = MapFile;

			for (int i = 0; i <= map.MapSize.Rows; ++i)
				_graphics.DrawLine(
								RoutePens["GridLineColor"],
								Origin.X - i * DrawAreaWidth,
								Origin.Y + i * DrawAreaHeight,
								Origin.X + ((map.MapSize.Cols - i) * DrawAreaWidth),
								Origin.Y + ((map.MapSize.Cols + i) * DrawAreaHeight));

			for (int i = 0; i <= map.MapSize.Cols; ++i)
				_graphics.DrawLine(
								RoutePens["GridLineColor"],
								Origin.X + i * DrawAreaWidth,
								Origin.Y + i * DrawAreaHeight,
							   (Origin.X + i * DrawAreaWidth)  - map.MapSize.Rows * DrawAreaWidth,
							   (Origin.Y + i * DrawAreaHeight) + map.MapSize.Rows * DrawAreaHeight);
		}

		/// <summary>
		/// Draws the compass-rose.
		/// </summary>
		private void DrawRose()
		{
			const int PAD_HORI = 25;
			const int PAD_VERT =  5;
			
			_graphics.DrawString(
							"W",
							_fontRose,
							Brushes.Black,
							PAD_HORI,
							PAD_VERT);
			_graphics.DrawString(
							"N",
							_fontRose,
							Brushes.Black,
//							Width - TextRenderer.MeasureText("N", _fontRose).Width - PAD_HORI,
							Width - (int)_graphics.MeasureString("N", _fontRose).Width - PAD_HORI,
							PAD_VERT);
			_graphics.DrawString(
							"S",
							_fontRose,
							Brushes.Black,
							PAD_HORI,
							Height - _fontRose.Height - PAD_VERT);
			_graphics.DrawString(
							"E",
							_fontRose,
							Brushes.Black,
//							Width  - TextRenderer.MeasureText("E", _fontRose).Width - PAD_HORI,
							Width  - (int)_graphics.MeasureString("E", _fontRose).Width - PAD_HORI,
							Height - _fontRose.Height - PAD_VERT);
		}

		/// <summary>
		/// Draws tile/node information in the overlay.
		/// </summary>
		private void DrawInfoOverlay()
		{
			var tile = GetTile(_pos.X, _pos.Y);
			if (tile != null)
			{
				var pt = GetTileCoordinates(_pos.X, _pos.Y);
				const string textTile1 = "position";
				string textTile2 = "c " + pt.X + " r " + pt.Y;

//				int textWidth1 = TextRenderer.MeasureText(textTile1, font).Width;
				int textWidth1 = (int)_graphics.MeasureString(textTile1, _fontOverlay).Width;
//				int textWidth2 = TextRenderer.MeasureText(textTile2, font).Width;
				int textWidth2 = (int)_graphics.MeasureString(textTile2, _fontOverlay).Width;

				string textOver1     = String.Empty;
				string textPriority1 = String.Empty;
				string textSpawn1    = String.Empty;
				string textWeight1   = String.Empty;

				string textOver2     = String.Empty;
				string textPriority2 = String.Empty;
				string textSpawn2    = String.Empty;
				string textWeight2   = String.Empty;

				if (tile.Node != null)
				{
					textOver1     = "over";
					textPriority1 = "priority";
					textSpawn1    = "spawn";
					textWeight1   = "weight";

					textOver2     = (tile.Node.Index).ToString(System.Globalization.CultureInfo.CurrentCulture);
					textPriority2 = (tile.Node.Priority).ToString();
					textSpawn2    = (RouteNodeCollection.UnitRankUfo[tile.Node.UsableRank]).ToString();
					textWeight2   = (tile.Node.Spawn).ToString();

					int width;
//					width = TextRenderer.MeasureText(textOver1, font).Width;
					width = (int)_graphics.MeasureString(textOver1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
//					width = TextRenderer.MeasureText(textPriority1, font).Width;
					width = (int)_graphics.MeasureString(textPriority1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
//					width = TextRenderer.MeasureText(textSpawn1, font).Width;
					width = (int)_graphics.MeasureString(textSpawn1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
//					width = TextRenderer.MeasureText(textWeight1, font).Width;
					width = (int)_graphics.MeasureString(textWeight1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;

//					width = TextRenderer.MeasureText(textOver2, font).Width;
					width = (int)_graphics.MeasureString(textOver2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
//					width = TextRenderer.MeasureText(textPriority2, font).Width;
					width = (int)_graphics.MeasureString(textPriority2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
//					width = TextRenderer.MeasureText(textSpawn2, font).Width;
					width = (int)_graphics.MeasureString(textSpawn2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
//					width = TextRenderer.MeasureText(textWeight2, font).Width;
					width = (int)_graphics.MeasureString(textWeight2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
					// time to move to a higher .NET framework.
				}

				const int Sep = 0;

//				int textHeight = TextRenderer.MeasureText("X", font).Height;
				int textHeight = (int)_graphics.MeasureString("X", _fontOverlay).Height;
				var overlay = new Rectangle(
										_pos.X + 18,                       _pos.Y,
										textWidth1 + Sep + textWidth2 + 8, textHeight + 8);

				if (tile.Node != null)
					overlay.Height += textHeight * 4;

				if (overlay.X + overlay.Width > ClientRectangle.Width)
					overlay.X = _pos.X - overlay.Width - 8;

				if (overlay.Y + overlay.Height > ClientRectangle.Height)
					overlay.Y = _pos.Y - overlay.Height;

				_graphics.FillRectangle(new SolidBrush(Color.FromArgb(160, 0, 0, 0)), overlay);
				_graphics.FillRectangle(
									new SolidBrush(Color.FromArgb(160, 255, 255, 255)),
									overlay.X + 2,
									overlay.Y + 2,
									overlay.Width  - 4,
									overlay.Height - 4);

				int textLeft = overlay.X + 4;
				int textTop  = overlay.Y + 3;

				_graphics.DrawString(
								textTile1,
								_fontOverlay,
								Brushes.Yellow,
								textLeft,
								textTop);
				_graphics.DrawString(
								textTile2,
								_fontOverlay,
								Brushes.Yellow,
								textLeft + textWidth1 + Sep,
								textTop);

				if (tile.Node != null)
				{
					_graphics.DrawString(
									textOver1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight);
					_graphics.DrawString(
									textOver2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Sep,
									textTop  + textHeight);

					_graphics.DrawString(
									textPriority1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 2);
					_graphics.DrawString(
									textPriority2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Sep,
									textTop  + textHeight * 2);

					_graphics.DrawString(
									textSpawn1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 3);
					_graphics.DrawString(
									textSpawn2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Sep,
									textTop  + textHeight * 3);

					_graphics.DrawString(
									textWeight1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 4);
					_graphics.DrawString(
									textWeight2,
									_fontOverlay,
									Brushes.Yellow,
									textLeft + textWidth1 + Sep,
									textTop  + textHeight * 4);
				}
			}
		}
		#endregion
	}
}
