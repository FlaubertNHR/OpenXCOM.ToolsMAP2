using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace XCom
{
	/// <summary>
	/// Static methods for dealing with <c>Bitmaps</c> and sprites.
	/// </summary>
	public static class SpriteService
	{
		/// <summary>
		/// Creates an 8-bpp indexed <c>Bitmap</c> from a specified byte-array.
		/// </summary>
		/// <param name="width">width of output <c>Bitmap</c></param>
		/// <param name="height">height of output <c>Bitmap</c></param>
		/// <param name="bindata">uncompressed image data</param>
		/// <param name="pal"><c>ColorPalette</c> to color the image with</param>
		/// <returns>a <c>Bitmap</c> image</returns>
		public static Bitmap CreateSprite(
				int width,
				int height,
				byte[] bindata,
				ColorPalette pal)
		{
			var b = new Bitmap(
							width, height,
							PixelFormat.Format8bppIndexed);

			var locked = b.LockBits(
								new Rectangle(0,0, width, height),
								ImageLockMode.WriteOnly,
								PixelFormat.Format8bppIndexed);
			var start = locked.Scan0;

			unsafe
			{
				byte* pos;
				if (locked.Stride > 0)
					pos = (byte*)start.ToPointer();
				else
					pos = (byte*)start.ToPointer() + locked.Stride * (height - 1);

				// if the stride is negative Scan0 points to the last scanline
				// in the buffer; to normalize the loop obtain a pointer to the
				// front of the buffer that is located Height-1 scanlines previous.

				uint stride = (uint)Math.Abs(locked.Stride);


				int i = -1;
				for (uint row = 0; row != height; ++row)
				for (uint col = 0; col != width && i != bindata.Length; ++col)
				{
					byte* pixel = pos + row * stride + col;
					*pixel = bindata[++i];
				}
			}
			b.UnlockBits(locked);

			b.Palette = pal;
			return b;
		}


		/// <summary>
		/// Extracts sprites from a specified spritesheet and adds them to a
		/// list of <c><see cref="XCImage">XCImages</see></c> according to a
		/// specified <c><see cref="Spriteset.SsType">Spriteset.SsType</see></c>.
		/// </summary>
		/// <param name="sprites">the
		/// <c><see cref="Spriteset.Sprites">Spriteset.Sprites</see></c> list of
		/// <c><see cref="XCImage">XCImages</see></c> of a
		/// <c><see cref="Spriteset"/></c></param>
		/// <param name="b">an 8-bpp <c>Bitmap</c> of a spritesheet</param>
		/// <param name="pal">a <c><see cref="Palette"/></c></param>
		/// <param name="width">the width of one sprite in the spritesheet</param>
		/// <param name="height">the height of one sprite in the spritesheet</param>
		/// <param name="setType"><c><see cref="Spriteset.SsType">Spriteset.SsType</see></c>
		/// to pass to
		/// <c><see cref="CreateSanitarySprite()">CreateSanitarySprite()</see></c></param>
		/// <remarks>Called by <c>PckViewF.OnImportSpritesheetClick()</c>.</remarks>
		public static void ImportSpritesheet(
				IList<XCImage> sprites,
				Bitmap b,
				Palette pal,
				int width,
				int height,
				Spriteset.SsType setType)
		{
			int cols = b.Width  / width;
			int rows = b.Height / height;

			int totalpixels = cols * rows;

			int id = -1;
			for (int i = 0; i != totalpixels; ++i)
			{
				sprites.Add(CreateSanitarySprite(
											b,
											++id,
											pal,
											width, height,
											setType,
											(i % cols) * width,
											(i / cols) * height));
			}
		}

		/// <summary>
		/// Creates a sprite as a derivative of <c><see cref="XCImage"/></c>
		/// after ensuring there aren't any
		/// <c><see cref="PckSprite.MarkerEos">PckSprite.MarkerEos</see></c> or
		/// <c><see cref="PckSprite.MarkerEos">PckSprite.MarkerRle</see></c>
		/// entries in the ColorTable of rle-encoded terrain-sprites,
		/// unit-sprites, or bigobs-sprites per
		/// <c><see cref="Spriteset.SsType">Spriteset.SsType</see></c>.
		/// </summary>
		/// <param name="b">an 8-bpp indexed <c>Bitmap</c></param>
		/// <param name="id">an appropriate <c><see cref="Spriteset"/></c> id</param>
		/// <param name="pal">a <c><see cref="Palette"/></c></param>
		/// <param name="width">the width of the output sprite</param>
		/// <param name="height">the height of the output sprite</param>
		/// <param name="setType"><c><see cref="Spriteset.SsType">Spriteset.SsType</see></c></param>
		/// <param name="x">used by spritesheets only</param>
		/// <param name="y">used by spritesheets only</param>
		/// <returns>a sprite derived from <c>XCImage</c></returns>
		/// <remarks>Helper for
		/// <c><see cref="ImportSpritesheet()">ImportSpritesheet()</see></c>.
		/// Also called by <c>PckViewF's</c> contextmenu
		/// <list type="bullet">
		/// <item><c>OnAddSpritesClick()</c></item>
		/// <item><c>OnReplaceSpriteClick()</c></item>
		/// <item><c>InsertSprites()</c></item>
		/// </list></remarks>
		public static XCImage CreateSanitarySprite(
				Bitmap b,
				int id,
				Palette pal,
				int width,
				int height,
				Spriteset.SsType setType,
				int x = 0,
				int y = 0)
		{
			var bindata = new byte[width * height]; // image data in uncompressed 8-bpp (color-indexed) format

			var locked = b.LockBits(
								new Rectangle(x,y, width, height),
								ImageLockMode.ReadOnly,
								PixelFormat.Format8bppIndexed);
			var start = locked.Scan0;

			unsafe
			{
				// kL_note: I suspect any of this negative-stride stuff is redundant.

				byte* pos;
				if (locked.Stride > 0)
					pos = (byte*)start.ToPointer();
				else
					pos = (byte*)start.ToPointer() + locked.Stride * (b.Height - 1);

				uint stride = (uint)Math.Abs(locked.Stride);


				int i = -1;

				switch (setType)
				{
					case Spriteset.SsType.Pck:
					case Spriteset.SsType.Bigobs:
					{
						byte palid;
						for (uint row = 0; row != height; ++row)
						for (uint col = 0; col != width;  ++col)
						{
							palid = *(pos + row * stride + col);

							if (palid > PckSprite.MaxId) // change any palette-indices 0xFF or 0xFE
								palid = PckSprite.MaxId; // to 0xFD if *not* a ScanG or LoFT icon

							bindata[++i] = palid;
						}
						break;
					}

					case Spriteset.SsType.ScanG:
					case Spriteset.SsType.LoFT:
						for (uint row = 0; row != height; ++row)
						for (uint col = 0; col != width;  ++col)
						{
							bindata[++i] = *(pos + row * stride + col);
						}
						break;

					default:
					case Spriteset.SsType.non: // TODO: error out
						break;
				}
			}
			b.UnlockBits(locked);


			XCImage sprite;

			switch (setType)
			{
				case Spriteset.SsType.Pck:
				case Spriteset.SsType.Bigobs:
					sprite = new PckSprite(
										bindata,
										width,
										height,
										id,
										pal);
					break;

				case Spriteset.SsType.ScanG:
					sprite = new ScanGicon(bindata, id);
					break;

				case Spriteset.SsType.LoFT:
					sprite = new LoFTicon(bindata, id);
					break;

				default:
				case Spriteset.SsType.non: // TODO: error out
					sprite = null;
					break;
			}
			return sprite;
		}


		/// <summary>
		/// Saves a sprite to a specified Pngfile.
		/// </summary>
		/// <param name="fullpath">fullpath of the output file</param>
		/// <param name="b">the <c>(Image)Bitmap</c> to export</param>
		public static void ExportSprite(
				string fullpath,
				Image b)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fullpath));
			b.Save(fullpath, ImageFormat.Png);
		}

		/// <summary>
		/// Saves a specified <c><see cref="Spriteset"/></c> as a <c>PNG</c>
		/// spritesheet.
		/// </summary>
		/// <param name="fullpath">fullpath of the output file</param>
		/// <param name="spriteset">a <c>Spriteset</c></param>
		/// <param name="pal">a <c><see cref="Palette"/></c></param>
		/// <param name="cols">width in sprites</param>
		/// <remarks>Check that spriteset is not <c>null</c> or blank before
		/// call. DO NOT PASS IN <c>0</c> COLS idiot.</remarks>
		public static void ExportSpritesheet(
				string fullpath,
				Spriteset spriteset,
				Palette pal,
				int cols = 8)
		{
			if (spriteset.Count < cols)
				cols = spriteset.Count;

			using (var b = CreateTransparent(
										cols * spriteset.SpriteWidth,
										((spriteset.Count + (cols - 1)) / cols) * spriteset.SpriteHeight,
										pal.Table))
			{
				for (int i = 0; i != spriteset.Count; ++i)
				{
					BlitSprite(
							spriteset[i].Sprite,
							b,
							i % cols * spriteset.SpriteWidth,
							i / cols * spriteset.SpriteHeight);
				}
				ExportSprite(fullpath, b);
			}
		}


		/// <summary>
		/// Creates a transparent <c>Bitmap</c> of specified width/height with
		/// a specified <c>ColorPalette</c>.
		/// </summary>
		/// <param name="width">width of output <c>Bitmap</c></param>
		/// <param name="height">height of output <c>Bitmap</c></param>
		/// <param name="pal">a <c>ColorPalette</c> to color the <c>Bitmap</c>
		/// with</param>
		/// <returns>a transparent <c>Bitmap</c></returns>
		/// <remarks>Called by
		/// <list type="bullet">
		/// <item><c><see cref="ExportSpritesheet()">ExportSpritesheet()</see></c></item>
		/// <item><c><see cref="CropTransparentEdges()">CropTransparentEdges()</see></c></item>
		/// <item><c>MainViewF.Screenshot()</c></item>
		/// </list></remarks>
		public static Bitmap CreateTransparent(
				int width,
				int height,
				ColorPalette pal)
		{
			var b = new Bitmap(
							width, height,
							PixelFormat.Format8bppIndexed);

			var locked = b.LockBits(
								new Rectangle(0,0, width, height),
								ImageLockMode.WriteOnly,
								PixelFormat.Format8bppIndexed);
			var start = locked.Scan0;

			unsafe
			{
				byte* pos;
				if (locked.Stride > 0)
					pos = (byte*)start.ToPointer();
				else
					pos = (byte*)start.ToPointer() + locked.Stride * (height - 1);

				uint stride = (uint)Math.Abs(locked.Stride);


				for (uint row = 0; row != height; ++row)
				for (uint col = 0; col != width;  ++col)
				{
					byte* pixel = pos + row * stride + col;
					*pixel = Palette.Tid;
				}
			}
			b.UnlockBits(locked);

			b.Palette = pal;
			return b;
		}

		/// <summary>
		/// Blits a sprite into another sprite.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>Called by
		/// <list type="bullet">
		/// <item><c><see cref="ExportSpritesheet()">ExportSpritesheet()</see></c></item>
		/// <item><c>MainViewF.Screenshot()</c></item>
		/// </list></remarks>
		public static void BlitSprite(
				Bitmap src,
				Bitmap dst,
				int x,
				int y)
		{
			var srcLocked = src.LockBits(
									new Rectangle(0,0, src.Width, src.Height),
									ImageLockMode.ReadOnly,
									PixelFormat.Format8bppIndexed);
			var srcStart = srcLocked.Scan0;

			var dstLocked = dst.LockBits(
									new Rectangle(0,0, dst.Width, dst.Height),
									ImageLockMode.WriteOnly,
									PixelFormat.Format8bppIndexed);
			var dstStart = dstLocked.Scan0;

			unsafe
			{
				byte* srcPos;
				if (srcLocked.Stride > 0)
					srcPos = (byte*)srcStart.ToPointer();
				else
					srcPos = (byte*)srcStart.ToPointer() + srcLocked.Stride * (src.Height - 1);

				uint srcStride = (uint)Math.Abs(srcLocked.Stride);

				byte* dstPos;
				if (dstLocked.Stride > 0)
					dstPos = (byte*)dstStart.ToPointer();
				else
					dstPos = (byte*)dstStart.ToPointer() + dstLocked.Stride * (dst.Height - 1);

				uint dstStride = (uint)Math.Abs(dstLocked.Stride);


				for (uint row = 0; row != src.Height && row + y < dst.Height; ++row)
				for (uint col = 0; col != src.Width  && col + x < dst.Width;  ++col)
				{
					byte* srcPixel = srcPos +  row      * srcStride +  col;
					byte* dstPixel = dstPos + (row + y) * dstStride + (col + x);

					if (*srcPixel != Palette.Tid)
						*dstPixel = *srcPixel;
				}
			}
			src.UnlockBits(srcLocked);
			dst.UnlockBits(dstLocked);
		}


		/// <summary>
		/// Crops any transparent edges around a specified <c>Bitmap</c>.
		/// </summary>
		/// <param name="src"></param>
		/// <returns>the <c>Bitmap</c> cropped or the unchanged <c>Bitmap</c>
		/// itself</returns>
		/// <remarks>Called by <c>MainViewF.Screenshot()</c>.</remarks>
		public static Bitmap CropTransparentEdges(Bitmap src)
		{
			Rectangle rect = GetRectangle(src);
			if (rect.Width < src.Width || rect.Height < src.Height)
			{
				var dst = CreateTransparent(rect.Width, rect.Height, src.Palette);

				var srcLocked = src.LockBits(
										new Rectangle(0,0, src.Width, src.Height),
										ImageLockMode.ReadOnly,
										PixelFormat.Format8bppIndexed);
				var srcStart = srcLocked.Scan0;

				var dstLocked = dst.LockBits(
										new Rectangle(0,0, dst.Width, dst.Height),
										ImageLockMode.WriteOnly,
										PixelFormat.Format8bppIndexed);
				var dstStart = dstLocked.Scan0;

				unsafe
				{
					byte* srcPos;
					if (srcLocked.Stride > 0)
						srcPos = (byte*)srcStart.ToPointer();
					else
						srcPos = (byte*)srcStart.ToPointer() + srcLocked.Stride * (src.Height - 1);

					uint srcStride = (uint)Math.Abs(srcLocked.Stride);

					byte* dstPos;
					if (dstLocked.Stride > 0)
						dstPos = (byte*)dstStart.ToPointer();
					else
						dstPos = (byte*)dstStart.ToPointer() + dstLocked.Stride * (dst.Height - 1);

					uint dstStride = (uint)Math.Abs(dstLocked.Stride);


					for (uint row = 0; row != dst.Height; ++row) // row + rect.Y < src.Height &&
					for (uint col = 0; col != dst.Width;  ++col) // col + rect.X < src.Width  &&
					{
						byte* srcPixel = srcPos + (row + rect.Y) * srcStride + (col + rect.X);
						byte* dstPixel = dstPos +  row           * dstStride +  col;

						if (*srcPixel != Palette.Tid)
							*dstPixel = *srcPixel;
					}
				}
				src.UnlockBits(srcLocked);
				dst.UnlockBits(dstLocked);

				return dst;
			}
			return src;
		}

		/// <summary>
		/// Gets the largest nontransparent rectangle inside a specified
		/// <c>Bitmap</c>.
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		/// <remarks>Called by
		/// <c><see cref="CropTransparentEdges()">CropTransparentEdges()</see></c>.</remarks>
		private static Rectangle GetRectangle(Bitmap b)
		{
			var locked = b.LockBits(
								new Rectangle(0,0, b.Width, b.Height),
								ImageLockMode.ReadOnly,
								PixelFormat.Format8bppIndexed);
			var start = locked.Scan0;

			int x,y, x0,y0, x1,y1;
			unsafe
			{
				byte* pos;
				if (locked.Stride > 0)
					pos = (byte*)start.ToPointer();
				else
					pos = (byte*)start.ToPointer() + locked.Stride * (b.Height - 1);
				
				uint stride = (uint)Math.Abs(locked.Stride);

//			find_y0:
				for (y0 = 0; y0 != b.Height; ++y0)
				for ( x = 0;  x != b.Width;  ++x)
				{
					if (*(pos + y0 * stride + x) != Palette.Tid)
						goto find_x0;
				}

			find_x0:
				for (x0 = 0; x0 != b.Width;  ++x0)
				for ( y = y0; y != b.Height; ++y)
				{
					if (*(pos + y * stride + x0) != Palette.Tid)
						goto find_y1;
				}

			find_y1:
				for (y1 = b.Height - 1; y1 != y0; --y1)
				for ( x = b.Width  - 1;  x != x0; --x)
				{
					if (*(pos + y1 * stride + x) != Palette.Tid)
						goto find_x1;
				}

			find_x1:
				for (x1 = b.Width - 1; x1 != x0; --x1)
				for ( y = y1;           y != y0; --y)
				{
					if (*(pos + y * stride + x1) != Palette.Tid)
						goto finished;
				}
			}
			finished:
			b.UnlockBits(locked);


			return new Rectangle(
							x0,          y0,
							x1 - x0 + 1, y1 - y0 + 1);
		}
	}
}


/*		/// <summary>
		/// Resize the image to the specified width and height.
		/// https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp#24199315
		/// WARNING: System.Exception: A Graphics object cannot be created from
		/// an image that has an indexed pixel format.
		/// </summary>
		/// <param name="image">the image to resize</param>
		/// <param name="width">the width to resize to</param>
		/// <param name="height">the height to resize to</param>
		/// <returns>the resized image</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var rect = new Rectangle(0,0, width, height);
			var dst  = new Bitmap(width, height);

			dst.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(dst))
			{
				graphics.CompositingMode    = CompositingMode   .SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode  = InterpolationMode .HighQualityBicubic;
				graphics.SmoothingMode      = SmoothingMode     .HighQuality;
				graphics.PixelOffsetMode    = PixelOffsetMode   .HighQuality;

				using (var ia = new ImageAttributes())
				{
					ia.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(
									image,
									rect,
									0,0,
									image.Width, image.Height,
									GraphicsUnit.Pixel,
									ia);
				}
			}
			return dst;
		} */

/*		/// <summary>
		/// Saves a sprite to a given path w/ format: MS Windows 3 Bitmap, uncompressed.
		/// </summary>
		/// <param name="fullpath"></param>
		/// <param name="bitmap"></param>
		public static void ExportSprite(string fullpath, Bitmap bitmap)
		{
			using (var bw = new BinaryWriter(new FileStream(fullpath, FileMode.Create)))
			{
				int pad = 0;
				while ((bitmap.Width + pad) % 4 != 0)
					++pad;

				int len = (bitmap.Width + pad) * bitmap.Height;

				bw.Write('B');
				bw.Write('M');
				bw.Write(1078 + len); // 14 + 40 + (4 * 256)
				bw.Write((int)0);
				bw.Write((int)1078);

				bw.Write((int)40);
				bw.Write((int)bitmap.Width);
				bw.Write((int)bitmap.Height);
				bw.Write((short)1);
				bw.Write((short)8);
				bw.Write((int)0);
				bw.Write((int)0);
				bw.Write((int)0);
				bw.Write((int)0);
				bw.Write((int)0);
				bw.Write((int)0);
				bw.Write((int)0);

//				byte[] bArr = new byte[256 * 4];
				var entries = bitmap.Palette.Entries;

				for (int colorId = 1; colorId != 256; ++colorId)
				{
//				for (int i = 0; i < bArr.Length; i += 4)
//				{
//					bArr[i]     = entries[i / 4].B;
//					bArr[i + 1] = entries[i / 4].G;
//					bArr[i + 2] = entries[i / 4].R;
//					bArr[i + 3] = 0;

					bw.Write(entries[colorId].B);
					bw.Write(entries[colorId].G);
					bw.Write(entries[colorId].R);
					bw.Write((byte)0);

//					bw.Write((byte)image.Palette.Entries[i].B);
//					bw.Write((byte)image.Palette.Entries[i].G);
//					bw.Write((byte)image.Palette.Entries[i].R);
//					bw.Write((byte)0);
				}
//				bw.Write(bArr);

				var colorTable = new Dictionary<Color, byte>();

				int id = 0;
				foreach(var colorId in bitmap.Palette.Entries)
					colorTable[colorId] = (byte)id++;

				colorTable[Color.FromArgb(0, 0, 0, 0)] = (byte)255;

				for (int i = bitmap.Height - 1; i != -1; --i)
				{
					for (int j = 0; j != bitmap.Width; ++j)
						bw.Write(colorTable[bitmap.GetPixel(j, i)]);

					for (int j = 0; j != pad; ++j)
						bw.Write((byte)0x00);
				}
			}
		} */


//		public static void Save24(string path, Bitmap image)
//		{
//			Save24(new FileStream(path, FileMode.Create), image);
//		}

//		public static void Save24(Stream str, Bitmap image)
//		{
//			var bw = new BinaryWriter(str);
//
//			int more = 0;
//			while ((image.Width * 3 + more) % 4 != 0)
//				more++;
//
//			int len = (image.Width * 3 + more) * image.Height;
//
//			bw.Write('B');					// must always be set to 'BM' to declare that this is a .bmp-file.
//			bw.Write('M');
//			bw.Write(14 + 40 + len);		// specifies the size of the file in bytes.
//			bw.Write((int)0);				// zero
//			bw.Write((int)14 + 40);			// specifies the offset from the beginning of the file to the bitmap data.
//
//			bw.Write((int)40);				// specifies the size of the BITMAPINFOHEADER structure, in bytes
//			bw.Write((int)image.Width);
//			bw.Write((int)image.Height);
//			bw.Write((short)1);				// specifies the number of planes of the target device
//			bw.Write((short)24);			// specifies the number of bits per pixel
//			bw.Write((int)0);
//			bw.Write((int)0);
//			bw.Write((int)0);
//			bw.Write((int)0);
//			bw.Write((int)0);
//			bw.Write((int)0);
//
//			for (int i = image.Height - 1; i >= 0; i--)
//			{
//				for (int j = 0; j < image.Width; j++)
//				{
//					var c = image.GetPixel(j, i);
//					bw.Write((byte)c.B);
//					bw.Write((byte)c.G);
//					bw.Write((byte)c.R);
//				}
//
//				for (int j = 0; j < more; j++)
//					bw.Write((byte)0x00);
//			}
//
//			bw.Flush();
//			bw.Close();
//		}

//		public static unsafe Bitmap HQ2X(/*Bitmap image*/)
//		{
//#if hq2xWorks
//			CImage in24 = new CImage();
//			in24.Init(image.Width, image.Height, 24);
//
//			for (int row = 0; row < image.Height; row++)
//				for (int col = 0; col < image.Width; col++)
//				{
//					Color c = image.GetPixel(col,row);
//					*(in24.m_pBitmap + (row * in24.m_Xres * 3) + (col * 3 + 0)) = c.B;
//					*(in24.m_pBitmap + (row * in24.m_Xres * 3) + (col * 3 + 1)) = c.G;
//					*(in24.m_pBitmap + (row * in24.m_Xres * 3) + (col * 3 + 2)) = c.R;
//				}
//
//			in24.ConvertTo16();
//
//			CImage out32 = new CImage();
//			out32.Init(in24.m_Xres * 2, in24.m_Yres * 2, 32);
//
//			CImage.InitLUTs();
//			CImage.hq2x_32(
//						in24.m_pBitmap,
//						out32.m_pBitmap,
//						in24.m_Xres,
//						in24.m_Yres,
//						out32.m_Xres * 4);
//
//			out32.ConvertTo24();
//
//			Bitmap b = new Bitmap(
//								out32.m_Xres, out32.m_Yres,
//								PixelFormat.Format24bppRgb);
//
////			Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
//			BitmapData bitmapData = b.LockBits(
//											new Rectangle(
//														0, 0,
//														b.Width, b.Height),
//											ImageLockMode.WriteOnly,
//											b.PixelFormat);
//
//			IntPtr pixels = bitmapData.Scan0;
//
//			byte* pBits;
//			if (bitmapData.Stride > 0)
//				pBits = (byte*)pixels.ToPointer();
//			else
//				pBits = (byte*)pixels.ToPointer() + bitmapData.Stride * (b.Height - 1);
//
//			byte* srcBits = out32.m_pBitmap;
//
//			for (int i = 0; i < b.Width * b.Height; i++)
//			{
//				*(pBits++) = *(srcBits++);
//				*(pBits++) = *(srcBits++);
//				*(pBits++) = *(srcBits++);
//			}
//
//			b.UnlockBits(bitmapData);
//
//			image.Dispose();
//			in24.__dtor();
//			out32.__dtor();
//
//			return b;
//#else
//			return null;
//#endif
//		}


//		public static XCImageCollection Load(string file, Type collectionType)
//		{
//			Bitmap b = new Bitmap(file);
//
//			MethodInfo mi = collectionType.GetMethod("FromBmp");
//			if (mi == null)
//				return null;
//			else
//				return (XCImageCollection)mi.Invoke(null, new object[]{ b });
//		}
//		public static XCImage LoadSingle(Bitmap src, int num, Palette pal, Type collectionType)
//		{
//			//return Spriteset.FromBmpSingle(src, num, pal);
//
//			MethodInfo mi = collectionType.GetMethod("FromBmpSingle");
//			if (mi == null)
//				return null;
//			else
//				return (XCImage)mi.Invoke(null, new object[]{ src, num, pal });
//		}
