using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace McdView
{
	internal static class isocube
	{
		const int HEIGHT = 8; // pixels. Everything else follows ...

		const int OFFSET_VERT = 2;


		/// <summary>
		/// Gets a graphics-path of the isolofts' outline. The args are used to
		/// position the path in the panel.
		/// </summary>
		/// <param name="pnlwidth"></param>
		/// <param name="pnlheight"></param>
		/// <returns></returns>
		internal static GraphicsPath GetCuboidOutline(int pnlwidth, int pnlheight)
		{
			int width  = McdviewF.Isocube.Width;
			int height = McdviewF.Isocube.Height;

			//     p0
			// p5      p1
			//     .
			// p4      p2
			//     p3

			var p0 = new Point(pnlwidth / 2 - 1,                  pnlheight - (OFFSET_VERT + height * 24));
			var p1 = new Point(pnlwidth / 2 - 1 + width * 10 + 4, pnlheight - (OFFSET_VERT + height * 18));
			var p2 = new Point(pnlwidth / 2 - 1 + width * 10 + 4, pnlheight - (OFFSET_VERT + height *  6));
			var p3 = new Point(pnlwidth / 2 - 1,                  pnlheight - (OFFSET_VERT));
			var p4 = new Point(pnlwidth / 2 - 1 - width * 10 - 3, pnlheight - (OFFSET_VERT + height *  6) + 1);
			var p5 = new Point(pnlwidth / 2 - 1 - width * 10 - 3, pnlheight - (OFFSET_VERT + height * 18) - 1);

			var cuboid = new GraphicsPath();

			cuboid.AddLine(p0, p1);
			cuboid.AddLine(p1, p2);
			cuboid.AddLine(p2, p3);
			cuboid.AddLine(p3, p4);
			cuboid.AddLine(p4, p5);
			cuboid.CloseFigure();

			return cuboid;
		}

		/// <summary>
		/// Gets a graphics-path for the top angle of the isolofts' outline. The
		/// args are used to position the path in the panel.
		/// </summary>
		/// <param name="pnlwidth"></param>
		/// <param name="pnlheight"></param>
		/// <returns></returns>
		internal static GraphicsPath GetTopAngle(int pnlwidth, int pnlheight)
		{
			int width  = McdviewF.Isocube.Width;
			int height = McdviewF.Isocube.Height;

			//     .
			// p0      p2
			//     p1
			// .       .
			//     .

			var p0 = new Point(pnlwidth / 2 - 1 - width * 10 - 3, pnlheight - (OFFSET_VERT + height * 18) - 1);
			var p1 = new Point(pnlwidth / 2 - 1,                  pnlheight - (OFFSET_VERT + height * 12));
			var p2 = new Point(pnlwidth / 2 - 1 + width * 10 + 4, pnlheight - (OFFSET_VERT + height * 18));

			var angle = new GraphicsPath();

			angle.AddLine(p0, p1);
			angle.AddLine(p1, p2);

			return angle;
		}

		/// <summary>
		/// Gets a graphics-path for the bot angle of the isolofts' outline. The
		/// args are used to position the path in the panel.
		/// </summary>
		/// <param name="pnlwidth"></param>
		/// <param name="pnlheight"></param>
		/// <returns></returns>
		internal static GraphicsPath GetBotAngle(int pnlwidth, int pnlheight)
		{
			int width  = McdviewF.Isocube.Width;
			int height = McdviewF.Isocube.Height;

			//     .
			// .       .
			//     p1
			// p0      p2
			//     .

			var p0 = new Point(pnlwidth / 2 - 1 - width * 10 - 3, pnlheight - (OFFSET_VERT + height *  6) + 1);
			var p1 = new Point(pnlwidth / 2 - 1,                  pnlheight - (OFFSET_VERT + height * 12));
			var p2 = new Point(pnlwidth / 2 - 1 + width * 10 + 4, pnlheight - (OFFSET_VERT + height *  6));

			var angle = new GraphicsPath();

			angle.AddLine(p0, p1);
			angle.AddLine(p1, p2);

			return angle;
		}

		/// <summary>
		/// Gets a graphics-path for the vertical of the isolofts' outline. The
		/// args are used to position the path in the panel.
		/// </summary>
		/// <param name="pnlwidth"></param>
		/// <param name="pnlheight"></param>
		/// <returns></returns>
		internal static GraphicsPath GetVerticalLineTop(int pnlwidth, int pnlheight)
		{
			int height = McdviewF.Isocube.Height;

			//     p0
			// .       .
			//     p1
			// .       .
			//     .

			var p0 = new Point(pnlwidth / 2 - 1, pnlheight - (OFFSET_VERT + height * 24));
			var p1 = new Point(pnlwidth / 2 - 1, pnlheight - (OFFSET_VERT + height * 12));

			var line = new GraphicsPath();

			line.AddLine(p0, p1);

			return line;
		}

		/// <summary>
		/// Gets a graphics-path for the vertical of the isolofts' outline. The
		/// args are used to position the path in the panel.
		/// </summary>
		/// <param name="pnlwidth"></param>
		/// <param name="pnlheight"></param>
		/// <returns></returns>
		internal static GraphicsPath GetVerticalLineBot(int pnlwidth, int pnlheight)
		{
			int height = McdviewF.Isocube.Height;

			//     .
			// .       .
			//     p0
			// .       .
			//     p1

			var p0 = new Point(pnlwidth / 2 - 1, pnlheight - (OFFSET_VERT + height * 12));
			var p1 = new Point(pnlwidth / 2 - 1, pnlheight - (OFFSET_VERT));

			var line = new GraphicsPath();

			line.AddLine(p0, p1);

			return line;
		}


		/// <summary>
		/// Instantiates a bitmap of an isometric cube for displaying an
		/// isometric representation of a tileparts' LoFTs.
		/// </summary>
		/// <returns></returns>
		internal static Bitmap GetIsocube()
		{
			int width = (int)(HEIGHT * Math.Cos(30.0 * (Math.PI / 180.0)));


			var b = new Bitmap(width, HEIGHT, PixelFormat.Format32bppArgb);

			Graphics graphics = Graphics.FromImage(b);
			graphics.SmoothingMode     = SmoothingMode    .HighQuality;
			graphics.PixelOffsetMode   = PixelOffsetMode  .HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

			graphics.FillRectangle(Brushes.Transparent, 0,0, width, HEIGHT);


			//     p0
			// p1      p2
			//     p3
			// p4      p5
			//     p6

			var p0 = new Point(width / 2, 0);
			var p1 = new Point(0,         HEIGHT     / 4);
			var p2 = new Point(width,     HEIGHT     / 4);
			var p3 = new Point(width / 2, HEIGHT     / 2);
			var p4 = new Point(0,         HEIGHT * 3 / 4);
			var p5 = new Point(width,     HEIGHT * 3 / 4);
			var p6 = new Point(width / 2, HEIGHT);

			using (var path = new GraphicsPath())
			{
				path.AddLine(p0, p2);
				path.AddLine(p2, p3);
				path.AddLine(p3, p1);
				path.CloseFigure();
				graphics.FillPath(Brushes.DeepSkyBlue, path);

				path.Reset();

				path.AddLine(p1, p3);
				path.AddLine(p3, p6);
				path.AddLine(p6, p4);
				path.CloseFigure();
				graphics.FillPath(Brushes.LawnGreen, path);

				path.Reset();

				path.AddLine(p2, p3);
				path.AddLine(p3, p6);
				path.AddLine(p6, p5);
				path.CloseFigure();
				graphics.FillPath(Brushes.Crimson, path);
			}
			return b;
		}
/*			var b = new Bitmap(
							width, height,
							PixelFormat.Format8bppIndexed);

			var data = b.LockBits(
								new Rectangle(0,0, b.Width, b.Height),
								ImageLockMode.WriteOnly,
								PixelFormat.Format8bppIndexed);
			var start = data.Scan0;

			unsafe
			{
				var pos = (byte*)start.ToPointer();

				for (uint row = 0; row != b.Height; ++row)
				for (uint col = 0; col != b.Width;  ++col)
				{
					byte* pixel = pos + col + row * data.Stride;
					*pixel = 3;//0; // fill w/ transparent.
				}


//				for (uint row = 0; row != b.Height; ++row)
//				for (uint col = 0; col != b.Width;  ++col)
//				{
//					if ()
//					{}
//					byte* pixel = pos + col + row * data.Stride;
//					*pixel = ;
//				}
			}
			b.UnlockBits(data);

			ColorPalette pal = b.Palette;
			pal.Entries[0] = Color.Transparent;
			pal.Entries[1] = Color.Red;
			pal.Entries[2] = Color.Green;
			pal.Entries[3] = Color.Blue;

			b.Palette = pal; */
	}
}
