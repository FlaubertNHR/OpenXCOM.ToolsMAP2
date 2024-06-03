using System;

using System.Drawing;
using System.Drawing.Drawing2D;


namespace XCom
{
	/// <summary>
	/// Draws floor- wall- and content-blobs for <c>TopView</c> and
	/// <c>RouteView</c> and blob previews for <c>McdviewF</c>.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c>
	/// crap.</remarks>
	public static class BlobDrawService
	{
		#region Enums (static)
		private enum Corner
		{ nw,ne,se,sw }
		#endregion Enums (static)


		#region Fields (static)
		/// <summary>
		/// Offsets the blobs from the grid-lines a bit.
		/// </summary>
		private const int Offset = 4;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Configures the specified <paramref name="path"/> for a content-blob.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="path"></param>
		/// <param name="halfwidth"></param>
		/// <param name="halfheight"></param>
		private static void PathContent(
				int x, int y,
				GraphicsPath path,
				int halfwidth, int halfheight)
		{
			//DSShared.Logfile.Log("BlobDrawService.PathContent()");

			int w = halfwidth  / 2;
			int h = halfheight / 2;

			y += h;

			path.Reset();
			path.AddLine(
						x,     y,
						x + w, y + h);
			path.AddLine(
						x + w, y + h,
						x,     y + h * 2);
			path.AddLine(
						x,     y + h * 2,
						x - w, y + h);
			path.CloseFigure();
		}

		/// <summary>
		/// Configures the specified <paramref name="path"/> for a corner-blob.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="corner"></param>
		/// <param name="path"></param>
		/// <param name="halfwidth"></param>
		/// <param name="halfheight"></param>
		private static void PathCorner(
				int x, int y,
				Corner corner,
				GraphicsPath path,
				int halfwidth, int halfheight)
		{
			//DSShared.Logfile.Log("BlobDrawService.PathCorner()");

			int w = halfwidth  / 2;
			int h = halfheight / 2;

			path.Reset();
			switch (corner)
			{
				case Corner.nw:
					path.AddLine(
								x,     y,
								x + w, y + h);
					path.AddLine(
								x + w, y + h,
								x - w, y + h);
					break;

				case Corner.ne:
					path.AddLine(
								x + w,         y + h,
								x + halfwidth, y + halfheight);
					path.AddLine(
								x + halfwidth, y + halfheight,
								x + w,         y + halfheight + h);
					break;

				case Corner.se:
					path.AddLine(
								x + w, y + halfheight + h,
								x,     y + halfheight * 2);
					path.AddLine(
								x,     y + halfheight * 2,
								x - w, y + halfheight + h);
					break;

				case Corner.sw:
					path.AddLine(
								x - w,         y + halfheight + h,
								x - halfwidth, y + halfheight);
					path.AddLine(
								x - halfwidth, y + halfheight,
								x - w,         y + h);
					break;
			}
			path.CloseFigure();
		}


		/// <summary>
		/// Draws floor-blobs for <c>TopView</c> and McdView's preview-blobs
		/// only; floors are not drawn for <c>RouteView</c>.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="loftid">the LoFT id of layer #0</param>
		/// <param name="path"></param>
		/// <param name="halfwidth"></param>
		/// <param name="halfheight"></param>
		public static void DrawFloor(
				Graphics graphics,
				Brush brush,
				int x, int y,
				byte loftid,
				GraphicsPath path,
				int halfwidth, int halfheight)
		{
			//DSShared.Logfile.Log("BlobDrawService.DrawFloor()");

			path.Reset();
			switch (loftid)
			{
//				case 0: return; // blank LoFT, no draw.

				default:
//				case Byte.MaxValue:
//				case 6: // fullfloor
					path.AddLine(
								x,             y,
								x + halfwidth, y + halfheight);
					path.AddLine(
								x + halfwidth, y + halfheight,
								x,             y + halfheight * 2);
					path.AddLine(
								x,             y + halfheight * 2,
								x - halfwidth, y + halfheight);
					break;

				// sw corner
				case 79: case 76: case 87: case 104: case 105: case 106:
					path.AddLine(
								x,             y,
								x,             y + halfheight * 2);
					path.AddLine(
								x,             y + halfheight * 2,
								x - halfwidth, y + halfheight);
					break;

				// ne corner
				case 80: case 77: case 88: case 98: case 99: case 100:
					path.AddLine(
								x,             y,
								x + halfwidth, y + halfheight);
					path.AddLine(
								x + halfwidth, y + halfheight,
								x,             y + halfheight * 2);
					break;

				// nw corner
				case  81: case  39: case  40: case 41: case 50: case 51: case 52:
				case 101: case 102: case 103:
					path.AddLine(
								x,             y,
								x + halfwidth, y + halfheight);
					path.AddLine(
								x + halfwidth, y + halfheight,
								x - halfwidth, y + halfheight);
					break;

				// se corner
				case 82: case 86: case 107: case 108: case 109:
					path.AddLine(
								x + halfwidth, y + halfheight,
								x,             y + halfheight * 2);
					path.AddLine(
								x,             y + halfheight * 2,
								x - halfwidth, y + halfheight);
					break;
			}
			path.CloseFigure();

			graphics.FillPath(brush, path);
		}

		/// <summary>
		/// Draws wall- and content-blobs.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="tool"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="part"></param>
		/// <param name="path"></param>
		/// <param name="halfwidth"></param>
		/// <param name="halfheight"></param>
		public static void DrawWallOrContent(
				Graphics graphics,
				BlobColorTool tool,
				int x, int y,
				Tilepart part,
				GraphicsPath path,
				int halfwidth, int halfheight)
		{
			//DSShared.Logfile.Log("BlobDrawService.DrawWallOrContent()");

			Blob blob;
			if (part.Record.LoftList != null)						// Top and Route
				blob = BlobTypeService.GetBlobType(part);
			else													// for McdView (and crippled parts theoretically)
				blob = BlobTypeService.GetBlobType(BlobTypeService._loftlist);

			//DSShared.Logfile.Log(". blob= " + blob);

			switch (blob)
			{
				// floor ->
				case Blob.Floorlike:
					PathContent(x,y, path, halfwidth, halfheight);
					graphics.FillPath(Brushes.White, path); // prep (underlay color) for 'BrushLight'
					graphics.FillPath(tool.BrushLight, path);
					break;

				// content ->
				case Blob.Generic:
					PathContent(x,y, path, halfwidth, halfheight);
					graphics.FillPath(tool.Brush, path);
					break;

				// walls ->
				case Blob.Westwall:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y), pL(x,y, halfwidth, halfheight));

					if (part.IsDoor) goto case Blob.WestDoorLine;
					break;

				case Blob.WestwallWindow:
					DrawWindow(
							graphics,
							tool,
							pT(x,y), pL(x,y, halfwidth, halfheight));

					if (part.IsDoor) goto case Blob.WestDoorLine;
					break;

				case Blob.WestFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y), pL(x,y, halfwidth, halfheight));

					if (part.IsDoor) goto case Blob.WestDoorLine;
					break;

				case Blob.WestDoorLine:
					graphics.DrawLine(
									GetDoorPen(part, tool),
									x - halfwidth, y,
									x,             y + halfheight);
					break;


				case Blob.Northwall:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y), pR(x,y, halfwidth, halfheight));

					if (part.IsDoor) goto case Blob.NorthDoorLine;
					break;

				case Blob.NorthwallWindow:
					DrawWindow(
							graphics,
							tool,
							pT(x,y), pR(x,y, halfwidth, halfheight));

					if (part.IsDoor) goto case Blob.NorthDoorLine;
					break;

				case Blob.NorthFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y), pR(x,y, halfwidth, halfheight));

					if (part.IsDoor) goto case Blob.NorthDoorLine;
					break;

				case Blob.NorthDoorLine:
					graphics.DrawLine(
									GetDoorPen(part, tool),
									x + halfwidth, y,
									x,             y + halfheight);
					break;


				case Blob.Eastwall:
					graphics.DrawLine(
									tool.Pen,
									pB(x,y, halfheight), pR(x,y, halfwidth, halfheight));
					break;

				case Blob.EastFence:
					graphics.DrawLine(
									tool.PenLight,
									pB(x,y, halfheight), pR(x,y, halfwidth, halfheight));
					break;


				case Blob.Southwall:
					graphics.DrawLine(
									tool.Pen,
									pL(x,y, halfwidth, halfheight), pB(x,y, halfheight));
					break;

				case Blob.SouthFence:
					graphics.DrawLine(
									tool.PenLight,
									pL(x,y, halfwidth, halfheight), pB(x,y, halfheight));
					break;

				// diagonals ->
				case Blob.NorthwestSoutheast:
					graphics.DrawLine(
									tool.Pen,
									pT(x,y), pB(x,y, halfheight));
					break;

				case Blob.NorthwestSoutheastFence:
					graphics.DrawLine(
									tool.PenLight,
									pT(x,y), pB(x,y, halfheight));
					break;

				case Blob.NortheastSouthwest:
					graphics.DrawLine(
									tool.Pen,
									pL(x,y, halfwidth, halfheight), pR(x,y, halfwidth, halfheight));
					break;

				case Blob.NortheastSouthwestFence:
					graphics.DrawLine(
									tool.PenLight,
									pL(x,y, halfwidth, halfheight), pR(x,y, halfwidth, halfheight));
					break;

				// corners ->
				case Blob.NorthwestCorner:
					PathCorner(x,y, Corner.nw, path, halfwidth, halfheight);
					graphics.FillPath(tool.Brush, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pT(x,y), new Size(-Offset - Offset / 2, 0)), Point.Add(pT(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case Blob.NorthwestCornerFence:
					PathCorner(x,y, Corner.nw, path, halfwidth, halfheight);
					graphics.FillPath(tool.BrushLight, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pT(x,y), new Size(-Offset - Offset / 2, 0)), Point.Add(pT(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case Blob.NortheastCorner:
					PathCorner(x,y, Corner.ne, path, halfwidth, halfheight);
					graphics.FillPath(tool.Brush, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pR(x,y), new Size(0, -Offset)), Point.Add(pR(x,y), new Size(0,  Offset)));
					break;

				case Blob.NortheastCornerFence:
					PathCorner(x,y, Corner.ne, path, halfwidth, halfheight);
					graphics.FillPath(tool.BrushLight, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pR(x,y), new Size(0, -Offset)), Point.Add(pR(x,y), new Size(0,  Offset)));
					break;

				case Blob.SoutheastCorner:
					PathCorner(x,y, Corner.se, path, halfwidth, halfheight);
					graphics.FillPath(tool.Brush, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pB(x,y), new Size(-Offset - Offset / 2, 0)), Point.Add(pB(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case Blob.SoutheastCornerFence:
					PathCorner(x,y, Corner.se, path, halfwidth, halfheight);
					graphics.FillPath(tool.BrushLight, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pB(x,y), new Size(-Offset - Offset / 2, 0)), Point.Add(pB(x,y), new Size( Offset + Offset / 2, 0)));
					break;

				case Blob.SouthwestCorner:
					PathCorner(x,y, Corner.sw, path, halfwidth, halfheight);
					graphics.FillPath(tool.Brush, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pL(x,y), new Size(0, -Offset)), Point.Add(pL(x,y), new Size(0,  Offset)));
					break;

				case Blob.SouthwestCornerFence:
					PathCorner(x,y, Corner.sw, path, halfwidth, halfheight);
					graphics.FillPath(tool.BrushLight, path);
//					graphics.DrawLine(tool.Pen, Point.Add(pL(x,y), new Size(0, -Offset)), Point.Add(pL(x,y), new Size(0,  Offset)));
					break;

//				case Blob.Crippled: break;
			}
		}

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
			//DSShared.Logfile.Log("BlobDrawService.DrawWindow()");

			graphics.DrawLine(tool.Pen, beg, end);

			Point delta = Point.Subtract(end, new Size(beg));
			graphics.SetClip(new Rectangle(
										beg.X + delta.X / 3, beg  .Y,
										        delta.X / 3, delta.Y));
			graphics.DrawLine(tool.PenLightPrep, beg, end);
			graphics.DrawLine(tool.PenLight,     beg, end);

			graphics.ResetClip();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="part"></param>
		/// <param name="tool"></param>
		/// <returns></returns>
		private static Pen GetDoorPen(Tilepart part, BlobColorTool tool)
		{
			Pen door;
			if (part.Record.LoftList != null)	// Top or Route
				door = tool.Pen;
			else								// for McdView (and crippled parts theoretically)
				door = tool.PenDoor;

			return door;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>point top</returns>
		private static Point pT(int x, int y)
		{
			return new Point(x, (y > Int32.MaxValue - Offset) ? Int32.MaxValue : y + Offset);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfheight"></param>
		/// <returns>point bot</returns>
		private static Point pB(int x, int y, int halfheight)
		{
			return new Point(x, y + (halfheight * 2) - Offset);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfwidth"></param>
		/// <param name="halfheight"></param>
		/// <returns>point left</returns>
		private static Point pL(int x, int y, int halfwidth, int halfheight)
		{
			return new Point(x - halfwidth + (Offset * 2), y + halfheight);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfwidth"></param>
		/// <param name="halfheight"></param>
		/// <returns>point right</returns>
		private static Point pR(int x, int y, int halfwidth, int halfheight)
		{
			return new Point(x + halfwidth - (Offset * 2), y + halfheight);
		}
		#endregion Methods (static)
	}
}
