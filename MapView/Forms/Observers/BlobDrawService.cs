using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Draws floor- and wall- and content-blobs for <see cref="TopView"/> and
	/// <see cref="RouteView"/>.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c> crap.</remarks>
	internal sealed class BlobDrawService
	{
		/// <summary>
		/// Disposal isn't necessary since the GraphicsPaths last the lifetime
		/// of the app. But FxCop ca1001 gets antsy ....
		/// </summary>
		internal void Dispose()
		{
			DSShared.Logfile.Log("BlobDrawService.Dispose()");
			_floor  .Dispose();
			_content.Dispose();
		}


		#region Fields (static)
		internal const int LINEWIDTH_CONTENT = 3;
		#endregion Fields (static)


		#region Fields
		private readonly GraphicsPath _floor   = new GraphicsPath();
		private readonly GraphicsPath _content = new GraphicsPath();
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
		/// Draws a window.
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
		/// Draws floor-blobs for <see cref="TopView"/> only; floors are not
		/// drawn for <see cref="RouteView"/>.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		internal void Draw(
				Graphics graphics,
				Brush brush,
				int x, int y)
		{
			_floor.Reset();
			_floor.AddLine(
						x,             y,
						x + HalfWidth, y + HalfHeight);
			_floor.AddLine(
						x + HalfWidth, y + HalfHeight,
						x,             y + HalfHeight * 2);
			_floor.AddLine(
						x,             y + HalfHeight * 2,
						x - HalfWidth, y + HalfHeight);
			_floor.CloseFigure();

			graphics.FillPath(brush, _floor);
		}


		private const int Offset = 4; // offset the blobs from the grid-lines a bit.

		/// <summary>
		/// Draws wall- and content-blobs for <see cref="RouteView"/> and
		/// <see cref="TopView"/>.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="tool"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="part"></param>
		internal void Draw(
				Graphics graphics,
				BlobColorTool tool,
				int x, int y,
				Tilepart part)
		{
			switch (BlobTypeService.GetBlobType(part))
			{
				// content ->
				case BlobType.Content:
					PathContent(x,y);
					graphics.FillPath(
									tool.Brush,
									_content);
					break;

				// floor ->
				case BlobType.Floor:
					PathContent(x,y);
					graphics.FillPath(
									BlobColorTool.BrushLightPrep,
									_content);
					graphics.FillPath(
									tool.BrushLight,
									_content);
					break;

				// walls ->
				case BlobType.NorthWallFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y),
									pR(x,y));
					break;

				case BlobType.NorthWall:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y),
									pR(x,y));

					if (BlobTypeService.IsDoor(part))
						graphics.DrawLine(
										tool.Pen,
										x + HalfWidth, y,
										x,             y + HalfHeight);
					break;

				case BlobType.WestWallFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y),
									pL(x,y));
					break;

				case BlobType.WestWall:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y),
									pL(x,y));

					if (BlobTypeService.IsDoor(part))
						graphics.DrawLine(
										tool.Pen,
										x - HalfWidth, y,
										x,             y + HalfHeight);
					break;

				case BlobType.NorthWallWindow:
					DrawWindow(
							graphics,
							tool,
							pT(x,y),
							pR(x,y));
					break;

				case BlobType.WestWallWindow:
					DrawWindow(
							graphics,
							tool,
							pT(x,y),
							pL(x,y));
					break;

				case BlobType.SouthWall:
					graphics.DrawLine(
									tool.Pen,
									pL(x,y),
									pB(x,y));
					break;

				case BlobType.EastWall:
					graphics.DrawLine(
									tool.Pen,
									pB(x,y),
									pR(x,y));
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
					graphics.DrawLine(
									tool.Pen,
									Point.Add(pT(x,y), new Size(-Offset - Offset / 2, 0)),
									Point.Add(pT(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case BlobType.NortheastCorner:
					graphics.DrawLine(
									tool.Pen,
									Point.Add(pR(x,y), new Size(0, -Offset)),
									Point.Add(pR(x,y), new Size(0,  Offset)));
					break;

				case BlobType.SoutheastCorner:
					graphics.DrawLine(
									tool.Pen,
									Point.Add(pB(x,y), new Size(-Offset - Offset / 2, 0)),
									Point.Add(pB(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case BlobType.SouthwestCorner:
					graphics.DrawLine(
									tool.Pen,
									Point.Add(pL(x,y), new Size(0, -Offset)),
									Point.Add(pL(x,y), new Size(0,  Offset)));
					break;
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
		/// Sets a GraphicsPath for content-object.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void PathContent(int x, int y)
		{
			int w = HalfWidth  / 2;
			int h = HalfHeight / 2;

			y += h;

			_content.Reset();
			_content.AddLine(
							x,     y,
							x + w, y + h);
			_content.AddLine(
							x + w, y + h,
							x,     y + h * 2);
			_content.AddLine(
							x,     y + h * 2,
							x - w, y + h);
			_content.CloseFigure();
		}
		#endregion Methods
	}
}
