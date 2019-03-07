using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// The various wall- and content-types that will be used to determine how
	/// to draw the wall- and content-blobs in TopView and RouteView.
	/// </summary>
	internal enum BlobType
	{
		Content,
		EastWall,
		SouthWall,
		NorthWall,
		WestWall,
		NorthwestSoutheast,
		NortheastSouthwest,
		NorthWallWindow,
		WestWallWindow,
		Floor,
		NorthWallFence,
		WestWallFence,
		NorthwestCorner,
		NortheastCorner,
		SouthwestCorner,
		SoutheastCorner
	}


	/// <summary>
	/// Draws floor- and wall- and content- blobs for TopView and RouteView.
	/// </summary>
	internal sealed class DrawBlobService
		:
			IDisposable
	{
		#region Fields & Properties
		private readonly GraphicsPath _floor   = new GraphicsPath();
		private readonly GraphicsPath _content = new GraphicsPath();

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
		#endregion


		#region Methods
		/// <summary>
		/// Draws floor-blobs for TopView only; floors are not drawn for
		/// RouteView.
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
		/// Draws wall- and content- blobs for RouteView and TopView.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="tool"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="part"></param>
		internal void DrawContent(
				Graphics g,
				ColorTools tool,
				int x, int y,
				TilepartBase part)
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
				case BlobType.Content:
					PathContent(x, y);
					g.FillPath(
							tool.Brush,
							_content);
					break;

				// floor ->
				case BlobType.Floor:
					PathContent(x, y);
					g.FillPath(
							tool.BrushLight,
							_content);
					break;

				// walls ->
				case BlobType.NorthWallFence:
					g.DrawLine(
							tool.PenLight,
							ptTop,
							ptRight);
					break;

				case BlobType.NorthWall:
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

				case BlobType.WestWallFence:
					g.DrawLine(
							tool.PenLight,
							ptTop,
							ptLeft);
					break;

				case BlobType.WestWall:
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

				case BlobType.NorthWallWindow:
					DrawWindow(
							g,
							tool,
							ptTop,
							ptRight);
					break;

				case BlobType.WestWallWindow:
					DrawWindow(
							g,
							tool,
							ptTop,
							ptLeft);
					break;

				case BlobType.SouthWall:
					g.DrawLine(
							tool.Pen,
							ptLeft,
							ptBot);
					break;

				case BlobType.EastWall:
					g.DrawLine(
							tool.Pen,
							ptBot,
							ptRight);
					break;

				// diagonals ->
				case BlobType.NorthwestSoutheast:
					g.DrawLine(
							tool.Pen,
							ptTop,
							ptBot);
					break;

				case BlobType.NortheastSouthwest:
					g.DrawLine(
							tool.Pen,
							ptLeft,
							ptRight);
					break;

				// corners ->
				case BlobType.NorthwestCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptTop, new Size(-Offset - Offset / 2, 0)),
							Point.Add(ptTop, new Size( Offset + Offset / 2, 0)));
					break;

				case BlobType.NortheastCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptRight, new Size(0, -Offset)),
							Point.Add(ptRight, new Size(0,  Offset)));
					break;

				case BlobType.SoutheastCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptBot, new Size(-Offset - Offset / 2, 0)),
							Point.Add(ptBot, new Size( Offset + Offset / 2, 0)));
					break;

				case BlobType.SouthwestCorner:
					g.DrawLine(
							tool.Pen,
							Point.Add(ptLeft, new Size(0, -Offset)),
							Point.Add(ptLeft, new Size(0,  Offset)));
					break;
			}
		}

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

		private static void DrawWindow(
				Graphics g,
				ColorTools tool,
				Point start, Point end)
		{
			var pt = Point.Subtract(end, new Size(start));
			var xy = new Size(pt.X / 3, pt.Y / 3);
			pt     = Point.Add(start, Size.Add(xy, xy));

			g.DrawLine(
					tool.Pen,
					start,
					Point.Add(start, xy));
			g.DrawLine(
					tool.PenLight,
					Point.Add(start, xy),
					pt);
			g.DrawLine(
					tool.Pen,
					pt,
					end);
		}
		#endregion


		/// <summary>
		/// This isn't really necessary since the GraphicsPath's last the
		/// lifetime of the app. But FxCop gets antsy ....
		/// NOTE: Dispose() is never called. cf ColorTools. cf QuadrantPanelDrawService.
		/// WARNING: This is NOT a robust implementation perhaps. But it
		/// satisifes the core of the matter and could likely be used for
		/// further development if that's ever required.
		/// </summary>
		public void Dispose()
		{
			if (_floor   != null) _floor  .Dispose();
			if (_content != null) _content.Dispose();

			GC.SuppressFinalize(this);
		}
	}


	/// <summary>
	/// A class that determines how walls and objects are drawn for TopView and
	/// RouteView.
	/// </summary>
	internal static class BlobTypeService
	{
		#region Fields
		private static List<byte> _loftList;
		#endregion


		#region Methods
		/// <summary>
		/// Gets the BlobType of a given tile for drawing its blob in TopView
		/// and/or RouteView.
		/// NOTE: The checks are not robust; the return BlobType is just a guess
		/// based on what LoFTs have been assigned (externally) to a given tile.
		/// </summary>
		/// <remarks>http://www.ufopaedia.org/index.php/LOFTEMPS.DAT</remarks>
		/// <param name="part"></param>
		/// <returns></returns>
		internal static BlobType GetBlobType(TilepartBase part)
		{
			var record = part.Record;
			if (record != null)
			{
				_loftList = record.GetLoftList();

				// Floor
				if (CheckFloor())
					return BlobType.Floor;


				// East
				if (CheckAllAreInGroup(new[]{ 24, 26 }))//, 28, 30, 32, 34 }))
					return BlobType.EastWall;

				// South
				if (CheckAllAreInGroup(new[]{ 23, 25 }))//, 27, 29, 31, 33 }))
					return BlobType.SouthWall;


				// North ->
				if (CheckAnyIsLoft(38)
					&& CheckAllAreInGroup(new[]{ 8, 10, 12, 14, 38 }))
				{
					return BlobType.NorthWallWindow;
				}

				if (CheckAnyIsLoft(0)
					&& CheckAllAreInGroup(new[]{ 0, 8, 10, 12, 14, 38, 39, 77 })) // 40,41
				{
					return BlobType.NorthWallFence;
				}

				if (CheckAllAreInGroup(new[]{ 8, 10, 12, 14 }))//, 16, 18, 20, 21 }))
					return BlobType.NorthWall;


				// West ->
				if (CheckAnyIsLoft(37)
					&& CheckAllAreInGroup(new[]{ 7, 9, 11, 13, 37 }))
				{
					return BlobType.WestWallWindow;
				}

				if (CheckAnyIsLoft(0)
					&& CheckAllAreInGroup(new[]{ 0, 7, 9, 11, 13, 37, 39, 76 })) // 40,41
				{
					return BlobType.WestWallFence;
				}

				if (CheckAllAreInGroup(new[]{ 7, 9, 11, 13 }))//, 15, 17, 19, 22 }))
					return BlobType.WestWall;


				// diagonals ->
//				if (CheckAllAreLoftExcludeFloor(35))
				if (CheckAllAreLoft(35))
					return BlobType.NorthwestSoutheast;

//				if (CheckAllAreLoftExcludeFloor(36))
				if (CheckAllAreLoft(36))
					return BlobType.NortheastSouthwest;


				// corners ->
				if (CheckAllAreInGroup(new[]{ 39, 40, 41, 103 })) // 102,101
					return BlobType.NorthwestCorner;

				if (CheckAllAreLoft(100)) // 99,98
					return BlobType.NortheastCorner;

				if (CheckAllAreLoft(106)) // 105,104
					return BlobType.SouthwestCorner;

				if (CheckAllAreLoft(109)) // 108,107
					return BlobType.SoutheastCorner;
			}
			return BlobType.Content;
		}

		/// <summary>
		/// Checks if the tilepart is purely Floor-type.
		/// </summary>
		/// <returns></returns>
		private static bool CheckFloor()
		{
			int length = _loftList.Count;
			for (int layer = 0; layer != length; ++layer)
			{
				switch (layer)
				{
					case 0:
					case 1:
						break;

					default:
						if (_loftList[layer] != 0)
							return false;
						break;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if all entries in '_loftList' are among 'contingent'.
		/// </summary>
		/// <param name="contingent"></param>
		/// <returns></returns>
		private static bool CheckAllAreInGroup(int[] contingent)
		{
			foreach (var loft in _loftList)
			{
				var valid = false;
				foreach (var gottfried in contingent)
					if (gottfried == loft)
					{
						valid = true;
						break;
					}

				if (!valid)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if all entries in '_loftList' match 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool CheckAllAreLoft(int necessary)
		{
			foreach (var loft in _loftList)
				if (loft != necessary)
					return false;

			return true;
		}

//		private static bool CheckAllAreLoftExcludeFloor(int necessary)
//		{
//			int length = _loftList.Count;
//			for (int layer = 0; layer != length; ++layer)
//			{
//				switch (layer)
//				{
//					case 0:
//						break;
//
//					default:
//						if (_loftList[layer] != necessary)
//							return false;
//						break;
//				}
//			}
//			return true;
//		}

		/// <summary>
		/// Checks if any entry in '_loftList' matches 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool CheckAnyIsLoft(int necessary)
		{
			foreach (var loft in _loftList)
				if (loft == necessary)
					return true;

			return false;
		}
//		private static bool CheckAnyIsLoft(int[] necessary)
//		{
//			foreach (var loft in _loftList)
//				foreach (var gottfried in necessary)
//					if (gottfried == loft)
//						return true;
//
//			return false;
//		}

		/// <summary>
		/// Checks if a tilepart is either a standard door or a ufo door.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		internal static bool IsDoor(TilepartBase part)
		{
			var record = part.Record;
			return (record != null
				&& (record.HingedDoor || record.SlidingDoor));
		}
		#endregion
	}
}
