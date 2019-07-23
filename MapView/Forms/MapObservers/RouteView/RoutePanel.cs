using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// The derived class for RoutePanel. Handles drawing/painting the panel.
	/// @note This is not a Panel. It is a UserControl inside of a Panel.
	/// </summary>
	internal sealed class RoutePanel
		:
			RoutePanelParent
	{
		#region Fields (static)
		private const int RoseMarginX = 25;
		private const int RoseMarginY = 5;

		private const int NodeValMax = 12;

		private const int OverlayColPad = 2;

		private const string Over   = "id"; // these are for the translucent overlay box ->
		private const string Type   = "type";
		private const string Rank   = "rank";
		private const string Spawn  = "spawn";
		private const string Patrol = "patrol";
		private const string Attack = "attack";

		private const string textTile1 = ""; // "position" or "location" or ... "pos" or "loc" ... ie, undecided
		#endregion Fields (static)


		#region Fields
		private readonly Font _fontOverlay = new Font("Verdana",      7F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
		private readonly Font _fontRose    = new Font("Courier New", 22F, FontStyle.Bold);

		private Graphics _graphics;
		private GraphicsPath _nodeFill = new GraphicsPath();

		private SolidBrush _brushNode;
		private SolidBrush _brushNodeSpawn;
		private SolidBrush _brushNodeSelected;

		private Pen _penLink;
		private Pen _penLinkSelected;
		#endregion Fields


		#region Properties
		private Point _spot = new Point(-1,-1);
		/// <summary>
		/// The location of the tile that is highlighted by a mouse-overed Go
		/// button.
		/// </summary>
		internal Point SpotPosition
		{
			private get { return _spot; }
			set { _spot = value; }
		}
		#endregion Properties


		#region Properties (static)
		private static readonly Dictionary<string, Pen> _pens =
							new Dictionary<string, Pen>();
		internal static Dictionary<string, Pen> RoutePens
		{
			get { return _pens; }
		}

		private static readonly Dictionary<string, SolidBrush> _brushes =
							new Dictionary<string, SolidBrush>();
		internal static Dictionary<string, SolidBrush> RouteBrushes
		{
			get { return _brushes; }
		}
		#endregion Properties (static)


		#region Events (override)
		/// <summary>
		/// You know the drill ... Paint it, Black
		/// black as night
		/// NOTE: Pens and Brushes need to be refreshed each call to draw since
		/// they can be changed in Options. Or not ....
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			_graphics = e.Graphics;
			_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);

			if (MapFile != null)
			{
				BlobService.HalfWidth  = DrawAreaWidth;
				BlobService.HalfHeight = DrawAreaHeight;

				_penLink         = RoutePens[RouteViewOptionables.str_LinkColor];
				_penLinkSelected = RoutePens[RouteViewOptionables.str_LinkSelectedColor];

				DrawBlobs();

				DrawLinks();

				if (NodeSelected != null)
					DrawLinkLines(
							Origin.X + (SelectedLocation.X - SelectedLocation.Y)     * DrawAreaWidth,
							Origin.Y + (SelectedLocation.X + SelectedLocation.Y + 1) * DrawAreaHeight,
							NodeSelected,
							true);

				DrawNodes();

				DrawGridLines();

				if (Focused && _overCol != -1) // draw the selector lozenge
				{
					PathSelectorLozenge(
									Origin.X + (_overCol - _overRow) * DrawAreaWidth,
									Origin.Y + (_overCol + _overRow) * DrawAreaHeight);
					_graphics.DrawPath(
									new Pen( // TODO: make this a separate Option.
											RouteView.Optionables.GridLineColor,
											RouteView.Optionables.GridLineWidth + 1),
									LozSelector);
				}

				if (MainViewOverlay.that.FirstClick)
				{
					_graphics.DrawPath(
									new Pen( // TODO: make this a separate Option.
											RouteView.Optionables.NodeSelectedColor,
											RouteView.Optionables.GridLineWidth + 1),
									LozSelected);

					if (SpotPosition.X > -1)
					{
						PathSpottedLozenge(
										Origin.X + (SpotPosition.X - SpotPosition.Y) * DrawAreaWidth,
										Origin.Y + (SpotPosition.X + SpotPosition.Y) * DrawAreaHeight);
						_graphics.DrawPath(
										new Pen( // TODO: make this a separate Option.
												RouteView.Optionables.NodeSelectedColor,
												RouteView.Optionables.GridLineWidth + 1),
										LozSpotted);
					}
				}

				if (RouteView.Optionables.ShowPriorityBars)
					DrawNodeImportanceMeters();

				DrawRose();

				if (RouteView.Optionables.ShowOverlay && CursorPosition.X != -1)
					DrawInfoOverlay();
			}
		}
		#endregion Events (override)


		#region Methods (draw)
		internal static ColorTool ToolWall;
		internal static ColorTool ToolContent;

		/// <summary>
		/// Draws any wall and/or content indicators.
		/// </summary>
		private void DrawBlobs()
		{
			MapTile tile;
			for (int
					r = 0,
						startX = Origin.X,
						startY = Origin.Y;
					r != MapFile.MapSize.Rows;
					++r,
						startX -= DrawAreaWidth,
						startY += DrawAreaHeight)
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
					if (MapFile[r,c] != null)
					{
						tile = MapFile[r,c] as MapTile;

						if (tile.Content != null)
							BlobService.DrawContent(_graphics, ToolContent, x, y, tile.Content);

						if (tile.West != null)
							BlobService.DrawContent(_graphics, ToolWall, x, y, tile.West);

						if (tile.North != null)
							BlobService.DrawContent(_graphics, ToolWall, x, y, tile.North);
					}
				}
			}
		}

		/// <summary>
		/// Draws unselected link-lines.
		/// </summary>
		private void DrawLinks()
		{
			RouteNode node;
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
					if (MapFile[rSrc, cSrc] != null
						&& (node = ((MapTile)MapFile[rSrc, cSrc]).Node) != null
						&& (NodeSelected == null || node != NodeSelected))
					{
						DrawLinkLines(xSrc, ySrc, node, false);
					}
				}
			}
		}

		/// <summary>
		/// Draws link-lines for a given node.
		/// </summary>
		/// <param name="xSrc"></param>
		/// <param name="ySrc"></param>
		/// <param name="node"></param>
		/// <param name="selected">(default true)</param>
		private void DrawLinkLines(
				int xSrc,
				int ySrc,
				RouteNode node,
				bool selected = true)
		{
			int xDst, yDst;
			RouteNode dest;
			byte destId;

			for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
			{
				var link = node[slot] as Link;
				if ((destId = link.Destination) != Link.NotUsed)
				{
					switch (destId)
					{
						case Link.ExitWest:
							if (node.Lev != MapFile.Level)
								continue;

							xDst = OffsetX + 1;
							yDst = OffsetY + 1;
							dest = null;
							break;

						case Link.ExitNorth:
							if (node.Lev != MapFile.Level)
								continue;

							xDst = Width - OffsetX * 2;
							yDst =         OffsetY + 1;
							dest = null;
							break;

						case Link.ExitEast:
							if (node.Lev != MapFile.Level)
								continue;

							xDst = Width  - OffsetX * 2;
							yDst = Height - OffsetY * 2;
							dest = null;
							break;

						case Link.ExitSouth:
							if (node.Lev != MapFile.Level)
								continue;

							xDst =          OffsetX + 1;
							yDst = Height - OffsetY * 2;
							dest = null;
							break;

						default:
							if ((dest = MapFile.Routes[destId]) == null
								|| dest.Lev != MapFile.Level
								|| (NodeSelected != null && dest == NodeSelected)
								|| RouteNodeCollection.IsNodeOutsideMapBounds(
																			dest,
																			MapFile.MapSize.Cols,
																			MapFile.MapSize.Rows,
																			MapFile.MapSize.Levs))
							{
								continue;
							}

							xDst = Origin.X + (dest.Col - dest.Row)     * DrawAreaWidth;
							yDst = Origin.Y + (dest.Col + dest.Row + 1) * DrawAreaHeight;
							break;
					}

					if (selected) // draw link-lines for a selected node ->
					{
						var pen = _penLinkSelected;

						if (SpotPosition.X != -1)
						{
							if (dest != null)
							{
								if (   SpotPosition.X != dest.Col
									|| SpotPosition.Y != dest.Row)
								{
									pen = _penLink;
								}
							}
							else
							{
								switch (destId)	// see RouteView.SpotGoDestination() for def'n of the following spot-positions
								{
									case Link.ExitNorth:
										if (SpotPosition.X != -2)
											pen = _penLink;
										break;
									case Link.ExitEast:
										if (SpotPosition.X != -3)
											pen = _penLink;
										break;
									case Link.ExitSouth:
										if (SpotPosition.X != -4)
											pen = _penLink;
										break;
									case Link.ExitWest:
										if (SpotPosition.X != -5)
											pen = _penLink;
										break;
								}
							}
						}
						_graphics.DrawLine(
										pen,
										xSrc, ySrc,
										xDst, yDst);
					}
					else // draw link-lines for a non-selected node ->
						_graphics.DrawLine(
										_penLink,
										xSrc, ySrc + DrawAreaHeight, // unselected nodes need an offset
										xDst, yDst);
				}
			}
		}

		/// <summary>
		/// Draws the nodes.
		/// </summary>
		private void DrawNodes()
		{
			_brushNode         = RouteBrushes[RouteViewOptionables.str_NodeColor];
			_brushNodeSpawn    = RouteBrushes[RouteViewOptionables.str_NodeSpawnColor];
			_brushNodeSelected = RouteBrushes[RouteViewOptionables.str_NodeSelectedColor];


			int startX = Origin.X;
			int startY = Origin.Y;

			MapTile tile;
			RouteNode node;

			for (int row = 0; row != MapFile.MapSize.Rows; ++row)
			{
				for (int
						col = 0,
							x = startX,
							y = startY;
						col != MapFile.MapSize.Cols;
						++col,
							x += DrawAreaWidth,
							y += DrawAreaHeight)
				{
					if ((tile = MapFile[row, col] as MapTile) != null)	// NOTE: MapFileBase has the current level stored and uses
					{													// it to return only tiles on the correct level here.
						if ((node = tile.Node) != null)
						{
							_nodeFill.Reset();
							_nodeFill.AddLine(
											x,                 y,
											x + DrawAreaWidth, y + DrawAreaHeight);
							_nodeFill.AddLine(
											x + DrawAreaWidth, y + DrawAreaHeight,
											x,                 y + DrawAreaHeight * 2);
							_nodeFill.AddLine(
											x,                 y + DrawAreaHeight * 2,
											x - DrawAreaWidth, y + DrawAreaHeight);
							_nodeFill.CloseFigure();

							if (NodeSelected != null && MapFile.Level == NodeSelected.Lev
								&& col == SelectedLocation.X
								&& row == SelectedLocation.Y)
							{
								_graphics.FillPath(_brushNodeSelected, _nodeFill);
							}
							else if (node.Spawn != SpawnWeight.None)
							{
								_graphics.FillPath(_brushNodeSpawn, _nodeFill);
							}
							else
								_graphics.FillPath(_brushNode, _nodeFill);


							for (int i = 0; i != RouteNode.LinkSlots; ++i) // check for and if applicable draw the up/down indicators.
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
										if (MapFile.Routes[link.Destination] != null)
										{
											int level = MapFile.Routes[link.Destination].Lev;
											if (level != MapFile.Level)
											{
												if (level < MapFile.Level) // draw arrow up.
												{
													_graphics.DrawLine( // start w/ a vertical line in the tile-lozenge
																	_penLink,
																	x, y,
																	x, y + DrawAreaHeight * 2);
													_graphics.DrawLine( // then lines on the two top edges of the tile
																	_penLink,
																	x + 2,                 y,
																	x + 2 - DrawAreaWidth, y + DrawAreaHeight);
													_graphics.DrawLine(
																	_penLink,
																	x - 2,                 y,
																	x - 2 + DrawAreaWidth, y + DrawAreaHeight);
												}
												else //if (levelDestination > MapFile.Level) // draw arrow down.
												{
													_graphics.DrawLine( // start w/ a horizontal line in the tile-lozenge
																	_penLink,
																	x - DrawAreaWidth, y + DrawAreaHeight,
																	x + DrawAreaWidth, y + DrawAreaHeight);
													_graphics.DrawLine( // then lines on the two bottom edges of the tile
																	_penLink,
																	x + 2,                 y + DrawAreaHeight * 2,
																	x + 2 - DrawAreaWidth, y + DrawAreaHeight);
													_graphics.DrawLine(
																	_penLink,
																	x - 2,                 y + DrawAreaHeight * 2,
																	x - 2 + DrawAreaWidth, y + DrawAreaHeight);
												}
											}
										}
										break;
								}
							}
						}
					}
				}
				startX -= DrawAreaWidth;
				startY += DrawAreaHeight;
			}
		}

		/// <summary>
		/// Draws the grid-lines.
		/// </summary>
		private void DrawGridLines()
		{
			Pen pen;
			for (int i = 0; i <= MapFile.MapSize.Rows; ++i)
			{
				if (i % 10 != 0) pen = RoutePens[RouteViewOptionables.str_GridLineColor];
				else             pen = RoutePens[RouteViewOptionables.str_GridLine10Color];

				_graphics.DrawLine(
								pen,
								Origin.X - i * DrawAreaWidth,
								Origin.Y + i * DrawAreaHeight,
								Origin.X + ((MapFile.MapSize.Cols - i) * DrawAreaWidth),
								Origin.Y + ((MapFile.MapSize.Cols + i) * DrawAreaHeight));
			}

			for (int i = 0; i <= MapFile.MapSize.Cols; ++i)
			{
				if (i % 10 != 0) pen = RoutePens[RouteViewOptionables.str_GridLineColor];
				else             pen = RoutePens[RouteViewOptionables.str_GridLine10Color];

				_graphics.DrawLine(
								pen,
								Origin.X + i * DrawAreaWidth,
								Origin.Y + i * DrawAreaHeight,
							   (Origin.X + i * DrawAreaWidth)  - MapFile.MapSize.Rows * DrawAreaWidth,
							   (Origin.Y + i * DrawAreaHeight) + MapFile.MapSize.Rows * DrawAreaHeight);
			}
		}

		/// <summary>
		/// Draws the node importance bars.
		/// </summary>
		private void DrawNodeImportanceMeters()
		{
			int startX = Origin.X;
			int startY = Origin.Y;

			MapTile tile;
			RouteNode node;

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
					if ((tile = MapFile[r,c] as MapTile) != null)
					{
						if ((node = tile.Node) != null)
						{
							int infoboxX = x - DrawAreaWidth / 2 - 2;			// -2 to prevent drawing over the link-going-up
							int infoboxY = y + DrawAreaHeight - NodeValMax / 2;	// vertical line indicator when panel is small sized.

							DrawImportanceMeter(
											infoboxX,
											infoboxY,
											(int)node.Spawn,
											Brushes.LightCoral);

							DrawImportanceMeter(
											infoboxX + 3,
											infoboxY,
											(int)node.Patrol,
											Brushes.DeepSkyBlue);
						}
					}
				}
				startX -= DrawAreaWidth;
				startY += DrawAreaHeight;
			}
		}

		/// <summary>
		/// Helper for DrawNodeImportanceMeters().
		/// </summary>
		/// <param name="infoboxX"></param>
		/// <param name="infoboxY"></param>
		/// <param name="value"></param>
		/// <param name="color"></param>
		private void DrawImportanceMeter(
				int infoboxX,
				int infoboxY,
				int value,
				Brush color)
		{
			var p0 = new Point(
							infoboxX - 1, // ...
							infoboxY);
			var p1 = new Point(
							infoboxX + 3,
							infoboxY);
			var p2 = new Point(
							infoboxX + 3,
							infoboxY + NodeValMax - 1);
			var p3 = new Point(
							infoboxX,
							infoboxY + NodeValMax - 1);
			var p4 = new Point(
							infoboxX,
							infoboxY);

			var path = new GraphicsPath();

			path.AddLine(p0, p1);
			path.AddLine(p1, p2);
			path.AddLine(p2, p3);
			path.AddLine(p3, p4);

			_graphics.FillPath(Brushes.WhiteSmoke, path); // fill background.

			if (value > 0)
				_graphics.FillRectangle(
									color,
									infoboxX, infoboxY + NodeValMax - value - 2,
									2, value);

			_graphics.DrawPath(Pens.Black, path); // draw borders.
		}

		/// <summary>
		/// Draws the compass-rose.
		/// </summary>
		private void DrawRose()
		{
			_graphics.DrawString(
							"W",
							_fontRose,
							Brushes.Black,
							RoseMarginX,
							RoseMarginY);
			_graphics.DrawString(
							"N",
							_fontRose,
							Brushes.Black,
//							Width - TextRenderer.MeasureText("N", _fontRose).Width - RoseMarginX,
							Width - (int)_graphics.MeasureString("N", _fontRose).Width - RoseMarginX,
							RoseMarginY);
			_graphics.DrawString(
							"S",
							_fontRose,
							Brushes.Black,
							RoseMarginX,
							Height - _fontRose.Height - RoseMarginY);
			_graphics.DrawString(
							"E",
							_fontRose,
							Brushes.Black,
//							Width  - TextRenderer.MeasureText("E", _fontRose).Width - RoseMarginX,
							Width  - (int)_graphics.MeasureString("E", _fontRose).Width - RoseMarginX,
							Height - _fontRose.Height - RoseMarginY);
		}

		/// <summary>
		/// Draws tile/node information in the overlay.
		/// </summary>
		private void DrawInfoOverlay()
		{
			int x = CursorPosition.X;
			int y = CursorPosition.Y;

			MapTile tile = GetTile(ref x, ref y); // x/y -> tile-location
			if (tile != null)
			{
				string textTile2 =   "c " + (x + 1)
								 + "  r " + (y + 1)
								 + "  L " + (MapFile.MapSize.Levs - MapFile.Level); // 1-based count, level is inverted.

				int textWidth1 = (int)_graphics.MeasureString(textTile1, _fontOverlay).Width;
				int textWidth2 = (int)_graphics.MeasureString(textTile2, _fontOverlay).Width;

//				int textWidth1 = TextRenderer.MeasureText(textTile1, font).Width;
//				int textWidth2 = TextRenderer.MeasureText(textTile2, font).Width;

				string textOver1, textType1, textRank1, textSpawn1, textPatrol1, textAttack1;
				string textOver2, textType2, textRank2, textSpawn2, textPatrol2, textAttack2;

				if (tile.Node != null)
				{
					textOver1   = Over;
					textType1   = Type;
					textRank1   = Rank;
					textSpawn1  = Spawn;
					textPatrol1 = Patrol;

					textOver2 = (tile.Node.Index).ToString(CultureInfo.CurrentCulture);
					textType2 = Enum.GetName(typeof(UnitType), tile.Node.Type);

					if (MapFile.Descriptor.Pal == Palette.UfoBattle)
						textRank2 = RouteNodeCollection.RankUfo [tile.Node.Rank].ToString();
					else
						textRank2 = RouteNodeCollection.RankTftd[tile.Node.Rank].ToString();

					textSpawn2  = RouteNodeCollection.Spawn [(byte)tile.Node.Spawn] .ToString();
					textPatrol2 = RouteNodeCollection.Patrol[(byte)tile.Node.Patrol].ToString();

					int width;
					width = (int)_graphics.MeasureString(textOver1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
					width = (int)_graphics.MeasureString(textType1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
					width = (int)_graphics.MeasureString(textRank1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
					width = (int)_graphics.MeasureString(textSpawn1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;
					width = (int)_graphics.MeasureString(textPatrol1, _fontOverlay).Width;
					if (width > textWidth1) textWidth1 = width;

					width = (int)_graphics.MeasureString(textOver2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
					width = (int)_graphics.MeasureString(textType2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
					width = (int)_graphics.MeasureString(textRank2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
					width = (int)_graphics.MeasureString(textSpawn2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;
					width = (int)_graphics.MeasureString(textPatrol2, _fontOverlay).Width;
					if (width > textWidth2) textWidth2 = width;

					if (tile.Node.Attack != 0)
					{
						textAttack1 = Attack;
						textAttack2 = RouteNodeCollection.Attack[(byte)tile.Node.Attack].ToString();

						width = (int)_graphics.MeasureString(textAttack1, _fontOverlay).Width;
						if (width > textWidth1) textWidth1 = width;

						width = (int)_graphics.MeasureString(textAttack2, _fontOverlay).Width;
						if (width > textWidth2) textWidth2 = width;
					}
					else
					{
						textAttack1 =
						textAttack2 = null;
					}

					// time to move to a higher .NET framework.

//					width = TextRenderer.MeasureText(textOver1, font).Width;
//					width = TextRenderer.MeasureText(textPriority1, font).Width;
//					width = TextRenderer.MeasureText(textSpawn1, font).Width;
//					width = TextRenderer.MeasureText(textWeight1, font).Width;
//					width = TextRenderer.MeasureText(textOver2, font).Width;
//					width = TextRenderer.MeasureText(textPriority2, font).Width;
//					width = TextRenderer.MeasureText(textSpawn2, font).Width;
//					width = TextRenderer.MeasureText(textWeight2, font).Width;
				}
				else
				{
					textOver1   =
					textType1   =
					textRank1   =
					textSpawn1  =
					textPatrol1 =
					textAttack1 =

					textOver2   =
					textType2   =
					textRank2   =
					textSpawn2  =
					textPatrol2 =
					textAttack2 = null;
				}

//				int textHeight = TextRenderer.MeasureText("X", font).Height;
				int textHeight = (int)_graphics.MeasureString("X", _fontOverlay).Height + 1; // pad +1
				var rect = new Rectangle(
									CursorPosition.X + 18, CursorPosition.Y,
									textWidth1 + OverlayColPad + textWidth2 + 5, textHeight + 7); // trim right & bottom (else +8 for both w/h)

				if (tile.Node != null)
				{
					rect.Height += textHeight * 5;

					if (tile.Node.Attack != 0)
						rect.Height += textHeight;
				}

				if (rect.X + rect.Width > ClientRectangle.Width)
					rect.X = CursorPosition.X - rect.Width - 8;

				if (rect.Y + rect.Height > ClientRectangle.Height)
					rect.Y = CursorPosition.Y - rect.Height;

				_graphics.FillRectangle(new SolidBrush(Color.FromArgb(205, Color.DarkSlateBlue)), rect);
				_graphics.FillRectangle(
									new SolidBrush(Color.FromArgb(90, Color.AntiqueWhite)),
									rect.X + 2,
									rect.Y + 2,
									rect.Width  - 4,
									rect.Height - 4);

				int textLeft = rect.X + 4;
				int textTop  = rect.Y + 3;

				int colRight = textLeft + textWidth1 + OverlayColPad;

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
								colRight,
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
									colRight,
									textTop  + textHeight);

					_graphics.DrawString(
									textType1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 2);
					_graphics.DrawString(
									textType2,
									_fontOverlay,
									Brushes.Yellow,
									colRight,
									textTop  + textHeight * 2);

					_graphics.DrawString(
									textRank1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 3);
					_graphics.DrawString(
									textRank2,
									_fontOverlay,
									Brushes.Yellow,
									colRight,
									textTop  + textHeight * 3);

					_graphics.DrawString(
									textSpawn1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 4);
					_graphics.DrawString(
									textSpawn2,
									_fontOverlay,
									Brushes.Yellow,
									colRight,
									textTop  + textHeight * 4);

					_graphics.DrawString(
									textPatrol1,
									_fontOverlay,
									Brushes.Yellow,
									textLeft,
									textTop + textHeight * 5);
					_graphics.DrawString(
									textPatrol2,
									_fontOverlay,
									Brushes.Yellow,
									colRight,
									textTop  + textHeight * 5);

					if (tile.Node.Attack != 0)
					{
						_graphics.DrawString(
										textAttack1,
										_fontOverlay,
										Brushes.Yellow,
										textLeft,
										textTop + textHeight * 6);
						_graphics.DrawString(
										textAttack2,
										_fontOverlay,
										Brushes.Yellow,
										colRight,
										textTop  + textHeight * 6);
					}
				}
			}
		}
		#endregion Methods (draw)
	}
}
