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
	internal sealed class BlobDrawService
		:
			IDisposable
	{
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
		/// <param name="g"></param>
		/// <param name="tool"></param>
		/// <param name="beg"></param>
		/// <param name="end"></param>
		private static void DrawWindow(
				Graphics g,
				BlobColorTool tool,
				Point beg, Point end)
		{
			g.DrawLine(tool.Pen, beg, end);

			Point delta = Point.Subtract(end, new Size(beg));
			g.SetClip(new Rectangle(
								beg.X + delta.X / 3, beg  .Y,
								        delta.X / 3, delta.Y));
			g.DrawLine(tool.PenLightPrep, beg, end);
			g.DrawLine(tool.PenLight,     beg, end);

			g.ResetClip();
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Draws floor-blobs for <see cref="TopView"/> only; floors are not
		/// drawn for <see cref="RouteView"/>.
		/// </summary>
		internal void DrawFloor(
				Graphics g,
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

			g.FillPath(brush, _floor);
		}


		private const int Offset = 4; // offset the blobs from the grid-lines a bit.

		/// <summary>
		/// Draws wall- and content-blobs for <see cref="RouteView"/> and
		/// <see cref="TopView"/>.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="tool"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="part"></param>
		internal void DrawContent(
				Graphics g,
				BlobColorTool tool,
				int x, int y,
				Tilepart part)
		{
			var ptTop   = new Point(
								x,
								(y > Int32.MaxValue - Offset) ? Int32.MaxValue : y + Offset); // <- FxCop ...
			var ptBot   = new Point(
								x,
								y + (HalfHeight * 2) - Offset);
			var ptLeft  = new Point(
								x - HalfWidth + (Offset * 2),
								y + HalfHeight);
			var ptRight = new Point(
								x + HalfWidth - (Offset * 2),
								y + HalfHeight);

			switch (BlobTypeService.GetBlobType(part))
			{
				// content ->
				case BlobTypeService.BlobType.Content:
					PathContent(x,y);
					g.FillPath(
							tool.Brush,
							_content);
					break;

				// floor ->
				case BlobTypeService.BlobType.Floor:
					PathContent(x,y);
					g.FillPath(
							tool.BrushLightPrep,
							_content);
					g.FillPath(
							tool.BrushLight,
							_content);
					break;

				// walls ->
				case BlobTypeService.BlobType.NorthWallFence:
					g.DrawLine(
							tool.PenLight,
							ptTop,
							ptRight);
					break;

				case BlobTypeService.BlobType.NorthWall:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptRight);

					if (BlobTypeService.IsDoor(part))
						g.DrawLine(
								tool.Pen,
								x + HalfWidth, y,
								x,             y + HalfHeight);
					break;

				case BlobTypeService.BlobType.WestWallFence:
					g.DrawLine(
							tool.PenLight,
							ptTop,
							ptLeft);
					break;

				case BlobTypeService.BlobType.WestWall:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptLeft);

					if (BlobTypeService.IsDoor(part))
						g.DrawLine(
								tool.Pen,
								x - HalfWidth, y,
								x,             y + HalfHeight);
					break;

				case BlobTypeService.BlobType.NorthWallWindow:
					DrawWindow(
							g,
							tool,
							ptTop,
							ptRight);
					break;

				case BlobTypeService.BlobType.WestWallWindow:
					DrawWindow(
							g,
							tool,
							ptTop,
							ptLeft);
					break;

				case BlobTypeService.BlobType.SouthWall:
					g.DrawLine(
							tool.Pen,
							ptLeft,
							ptBot);
					break;

				case BlobTypeService.BlobType.EastWall:
					g.DrawLine(
							tool.Pen,
							ptBot,
							ptRight);
					break;

				// diagonals ->
				case BlobTypeService.BlobType.NorthwestSoutheast:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptBot);
					break;

				case BlobTypeService.BlobType.NortheastSouthwest:
					g.DrawLine(
							tool.Pen,
							ptLeft,
							ptRight);
					break;

				// corners ->
				case BlobTypeService.BlobType.NorthwestCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptTop, new Size(-Offset - Offset / 2, 0)),
							Point.Add(ptTop, new Size( Offset + Offset / 2, 0)));
					break;

				case BlobTypeService.BlobType.NortheastCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptRight, new Size(0, -Offset)),
							Point.Add(ptRight, new Size(0,  Offset)));
					break;

				case BlobTypeService.BlobType.SoutheastCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptBot, new Size(-Offset - Offset / 2, 0)),
							Point.Add(ptBot, new Size( Offset + Offset / 2, 0)));
					break;

				case BlobTypeService.BlobType.SouthwestCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptLeft, new Size(0, -Offset)),
							Point.Add(ptLeft, new Size(0,  Offset)));
					break;
			}
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


		/// <summary>
		/// Disposal isn't necessary since the GraphicsPaths last the lifetime
		/// of the app. But FxCop gets antsy ....
		/// </summary>
		public void Dispose()
		{
			_floor  .Dispose();
			_content.Dispose();
		}
	}
}
