using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Draws floor- and wall- and content-blobs for
	/// <c><see cref="TopView"/></c> and <c><see cref="RouteView"/></c>.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c>
	/// crap.</remarks>
	internal sealed class BlobDrawService
	{
		/// <summary>
		/// Disposal isn't necessary since the GraphicsPaths last the lifetime
		/// of the app. But FxCop ca1001 gets antsy ....
		/// </summary>
		internal void Dispose()
		{
			//DSShared.Logfile.Log("BlobDrawService.Dispose()");
			_path.Dispose();
		}


		#region Enums
		enum Corner
		{ nw,ne,se,sw }
		#endregion Enums


		#region Fields (static)
		internal const int LINEWIDTH_CONTENT = 3;

		/// <summary>
		/// Offset the blobs from the grid-lines a bit.
		/// </summary>
		private const int Offset = 4;
		#endregion Fields (static)


		#region Fields
		private readonly GraphicsPath _path = new GraphicsPath();
		#endregion Fields


		#region Properties
		private int _halfWidth = 8;
		internal int HalfWidth
		{
			get { return _halfWidth; }
			set { _halfWidth = value; }
		}
		private int _halfHeight = 4;
		internal int HalfHeight
		{
			get { return _halfHeight; }
			set { _halfHeight = value; }
		}
		#endregion Properties


		#region Methods (static)
		/// <summary>
		/// Draws a window blob.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="tool"></param>
		/// <param name="beg"></param>
		/// <param name="end"></param>
		private static void DrawWindow(
				Graphics graphics,
				BlobColorTool tool,
				Point beg, Point end)
		{
			graphics.DrawLine(tool.Pen, beg, end);

			Point delta = Point.Subtract(end, new Size(beg));
			graphics.SetClip(new Rectangle(
										beg.X + delta.X / 3, beg  .Y,
										        delta.X / 3, delta.Y));
			graphics.DrawLine(tool.PenLightPrep, beg, end);
			graphics.DrawLine(tool.PenLight,     beg, end);

			graphics.ResetClip();
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Draws floor-blobs for <c><see cref="TopView"/></c> only; floors are
		/// not drawn for <c><see cref="RouteView"/></c>.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="loftid"></param>
		internal void DrawFloor(
				Graphics graphics,
				Brush brush,
				int x, int y,
				byte loftid)
		{
			_path.Reset();
			switch (loftid)
			{
				default:
//				case Byte.MaxValue:
//				case 6: // fullfloor
					_path.AddLine(
								x,             y,
								x + HalfWidth, y + HalfHeight);
					_path.AddLine(
								x + HalfWidth, y + HalfHeight,
								x,             y + HalfHeight * 2);
					_path.AddLine(
								x,             y + HalfHeight * 2,
								x - HalfWidth, y + HalfHeight);
					break;

				case 79: // sw
					_path.AddLine(
								x,             y,
								x,             y + HalfHeight * 2);
					_path.AddLine(
								x,             y + HalfHeight * 2,
								x - HalfWidth, y + HalfHeight);
					break;

				case 80: // ne
					_path.AddLine(
								x,             y,
								x + HalfWidth, y + HalfHeight);
					_path.AddLine(
								x + HalfWidth, y + HalfHeight,
								x,             y + HalfHeight * 2);
					break;

				case 81: // nw
					_path.AddLine(
								x,             y,
								x + HalfWidth, y + HalfHeight);
					_path.AddLine(
								x + HalfWidth, y + HalfHeight,
								x - HalfWidth, y + HalfHeight);
					break;

				case 82: // se
					_path.AddLine(
								x + HalfWidth, y + HalfHeight,
								x,             y + HalfHeight * 2);
					_path.AddLine(
								x,             y + HalfHeight * 2,
								x - HalfWidth, y + HalfHeight);
					break;
			}
			_path.CloseFigure();

			graphics.FillPath(brush, _path);
		}

		/// <summary>
		/// Draws content- and wall-blobs for <c><see cref="RouteView"/></c> and
		/// <c><see cref="TopView"/></c>.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="tool"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="part"></param>
		internal void DrawContentOrWall(
				Graphics graphics,
				BlobColorTool tool,
				int x, int y,
				Tilepart part)
		{
			switch (BlobTypeService.GetBlobType(part))
			{
				// floor ->
				case BlobType.Floor:
					PathContent(x,y);
					graphics.FillPath(
									BlobColorTool.BrushLightPrep,
									_path);
					graphics.FillPath(
									tool.BrushLight,
									_path);
					break;

				// content ->
				case BlobType.Content:
					PathContent(x,y);
					graphics.FillPath(
									tool.Brush,
									_path);
					break;

				// walls ->
				case BlobType.WestWall:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y),
									pL(x,y));

					if (part.IsDoor)
						graphics.DrawLine(
										tool.Pen,
										x - HalfWidth, y,
										x,             y + HalfHeight);
					break;

				case BlobType.WestWallWindow:
					DrawWindow(
							graphics,
							tool,
							pT(x,y),
							pL(x,y));
					break;

				case BlobType.WestWallFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y),
									pL(x,y));
					break;

				case BlobType.NorthWall:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y),
									pR(x,y));

					if (part.IsDoor)
						graphics.DrawLine(
										tool.Pen,
										x + HalfWidth, y,
										x,             y + HalfHeight);
					break;

				case BlobType.NorthWallWindow:
					DrawWindow(
							graphics,
							tool,
							pT(x,y),
							pR(x,y));
					break;

				case BlobType.NorthWallFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y),
									pR(x,y));
					break;

				case BlobType.EastWall:
					graphics.DrawLine(
									tool.Pen,
									pB(x,y),
									pR(x,y));
					break;

				case BlobType.SouthWall:
					graphics.DrawLine(
									tool.Pen,
									pL(x,y),
									pB(x,y));
					break;

				// diagonals ->
				case BlobType.NorthwestSoutheast:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y),
									pB(x,y));
					break;

				case BlobType.NortheastSouthwest:
					graphics.DrawLine(
									tool.Pen,
									pL(x,y),
									pR(x,y));
					break;

				// corners ->
				case BlobType.NorthwestCorner:
					PathCorner(x,y, Corner.nw);
					graphics.FillPath(
									tool.Brush,
									_path);
//					graphics.DrawLine(
//									tool.Pen,
//									Point.Add(pT(x,y), new Size(-Offset - Offset / 2, 0)),
//									Point.Add(pT(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case BlobType.NortheastCorner:
					PathCorner(x,y, Corner.ne);
					graphics.FillPath(
									tool.Brush,
									_path);
//					graphics.DrawLine(
//									tool.Pen,
//									Point.Add(pR(x,y), new Size(0, -Offset)),
//									Point.Add(pR(x,y), new Size(0,  Offset)));
					break;

				case BlobType.SoutheastCorner:
					PathCorner(x,y, Corner.se);
					graphics.FillPath(
									tool.Brush,
									_path);
//					graphics.DrawLine(
//									tool.Pen,
//									Point.Add(pB(x,y), new Size(-Offset - Offset / 2, 0)),
//									Point.Add(pB(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case BlobType.SouthwestCorner:
					PathCorner(x,y, Corner.sw);
					graphics.FillPath(
									tool.Brush,
									_path);
//					graphics.DrawLine(
//									tool.Pen,
//									Point.Add(pL(x,y), new Size(0, -Offset)),
//									Point.Add(pL(x,y), new Size(0,  Offset)));
					break;

//				case BlobType.Crippled: break;
			}
		}

		private static Point pT(int x, int y)
		{
			return new Point(x, (y > Int32.MaxValue - Offset) ? Int32.MaxValue : y + Offset);
		}
		private Point pB(int x, int y)
		{
			return new Point(x, y + (HalfHeight * 2) - Offset);
		}
		private Point pL(int x, int y)
		{
			return new Point(x - HalfWidth + (Offset * 2), y + HalfHeight);
		}
		private Point pR(int x, int y)
		{
			return new Point(x + HalfWidth - (Offset * 2), y + HalfHeight);
		}

		/// <summary>
		/// Sets <c><see cref="_path"/></c> for a content blob.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void PathContent(int x, int y)
		{
			int w = HalfWidth  / 2;
			int h = HalfHeight / 2;

			y += h;

			_path.Reset();
			_path.AddLine(
						x,     y,
						x + w, y + h);
			_path.AddLine(
						x + w, y + h,
						x,     y + h * 2);
			_path.AddLine(
						x,     y + h * 2,
						x - w, y + h);
			_path.CloseFigure();
		}

		/// <summary>
		/// Sets <c><see cref="_path"/></c> for a corner blob.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="corner"></param>
		private void PathCorner(int x, int y, Corner corner)
		{
			int w = HalfWidth  / 2;
			int h = HalfHeight / 2;

			_path.Reset();
			switch (corner)
			{
				case Corner.nw:
					_path.AddLine(
								x,     y,
								x + w, y + h);
					_path.AddLine(
								x + w, y + h,
								x - w, y + h);
					break;

				case Corner.ne:
					_path.AddLine(
								x + w,         y + h,
								x + HalfWidth, y + HalfHeight);
					_path.AddLine(
								x + HalfWidth, y + HalfHeight,
								x + w,         y + HalfHeight + h);
					break;

				case Corner.se:
					_path.AddLine(
								x + w, y + HalfHeight + h,
								x,     y + HalfHeight * 2);
					_path.AddLine(
								x,     y + HalfHeight * 2,
								x - w, y + HalfHeight + h);
					break;

				case Corner.sw:
					_path.AddLine(
								x - w,         y + HalfHeight + h,
								x - HalfWidth, y + HalfHeight);
					_path.AddLine(
								x - HalfWidth, y + HalfHeight,
								x - w,         y + h);
					break;
			}
			_path.CloseFigure();
		}
		#endregion Methods
	}
}
