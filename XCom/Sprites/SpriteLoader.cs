using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using DSShared;


namespace XCom
{
	/// <summary>
	/// Image loading toolset class which corrects the bug that prevents
	/// paletted <c>PNG</c> images with transparency from being loaded as
	/// paletted.
	/// </summary>
	/// <remarks>Handles 8-bpp <c>PNG</c>/<c>GIF</c>/<c>BMP</c>.</remarks>
	/// https://stackoverflow.com/questions/44835726/c-sharp-loading-an-indexed-color-image-file-correctly#answer-45100442
	public static class SpriteLoader
	{
		private static byte[] Id_PNG = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
		private static byte[] Id_GIF = { 0x47, 0x49, 0x46, 0x38 };
		private static byte[] Id_BMP = { 0x42, 0x4D };

		/// <summary>
		/// Checks the byte-array's header for an image-format that is supported
		/// by MapView.
		/// </summary>
		/// <param name="data"></param>
		/// <returns><c>true</c> if valid</returns>
		/// <remarks>This is ofc not entirely robust. Image-formats with their
		/// chunks and subtypes get complicated real fast. So
		/// <c><see cref="LoadBitmap()">LoadBitmap()</see></c> will further run
		/// a try/catch block to display any .NET <c>Exception</c> when trying
		/// to create the <c>Bitmap</c>.</remarks>
		private static bool CheckValidHeader(byte[] data)
		{
			bool fail = false;
			if (data.Length >= Id_PNG.Length)
			{
				for (int i = 0; i != Id_PNG.Length; ++i)
				if (data[i] != Id_PNG[i])
				{
					fail = true;
					break;
				}
			}
			else fail = true;

			if (!fail) return true;

			fail = false;
			if (fail && data.Length >= Id_GIF.Length)
			{
				for (int i = 0; i != Id_GIF.Length; ++i)
				if (data[i] != Id_GIF[i])
				{
					fail = true;
					break;
				}
			}
			else fail = true;

			if (!fail) return true;

			fail = false;
			if (fail && data.Length >= Id_BMP.Length)
			{
				for (int i = 0; i != Id_BMP.Length; ++i)
				if (data[i] != Id_BMP[i])
				{
					fail = true;
					break;
				}
			}
			else fail = true;

			return !fail;
		}

		/// <summary>
		/// Loads an image. Checks if it is a <c>PNG</c> containing palette
		/// transparency and if so ensures it loads correctly.
		/// </summary>
		/// <param name="data">file data to load</param>
		/// <returns>a CLONED image - the onus is on the receiver for disposal</returns>
		/// <remarks>The theory on the PNG internals can be found at
		/// http://www.libpng.org/pub/png/book/chapter08.html</remarks>
		public static Bitmap LoadBitmap(byte[] data)
		{
			if (!CheckValidHeader(data))
			{
				showerror("File does not have a PNG/GIF/BMP header.");
				return null;
			}

			byte[] dataTrns = null;

			if (data.Length > Id_PNG.Length)
			{
				var data1 = new byte[Id_PNG.Length];
				Array.Copy(data, data1, Id_PNG.Length);

				if (Id_PNG.SequenceEqual(data1)) // check if the image is a PNG
				{
					// check if it contains a palette
					// I'm sure it can be looked up in the header somehow, but meh.

					int plteOffset = FindChunk(data, "PLTE");
					if (plteOffset != -1)
					{
						// check if it contains a palette transparency chunk
						int trnsOffset = FindChunk(data, "tRNS");
						if (trnsOffset != -1)
						{
							// get chunk
							int trnsLength = GetChunkLength(data, trnsOffset);
							switch (trnsLength)
							{
								default:
									dataTrns = new byte[trnsLength];
									Array.Copy(data, trnsOffset + 8, dataTrns, 0, trnsLength);

									// filter out the palette alpha chunk, make new data array
									var data2 = new byte[data.Length - (trnsLength + 12)];
									Array.Copy(data, 0, data2, 0, trnsOffset);

									int trnsEnd = trnsOffset + trnsLength + 12;
									Array.Copy(data, trnsEnd, data2, trnsOffset, data.Length - trnsEnd);

									data = data2;
									break;

								case -1:
									showerror("Bad chunk length in PNG image.");
									return null;

								case -2:
									showerror("Bad chunk endianness in PNG image.");
									return null;
							}
						}
//						else showerror("Chunk not found in PNG image: tRNS", true);
					}
//					else showerror("Chunk not found in PNG image: PLTE", true);
				}
//				else showerror("Image is not a PNG.", true);
			}
//			else showerror("Data length is less than or equal to PNG identifier.", true);


			try
			{
				using (var ms = new MemoryStream(data))
				using (var b = new Bitmap(ms))
				{
					if (b.Palette.Entries.Length != 0 && dataTrns != null)
					{
						ColorPalette pal = b.Palette;
						for (int i = 0; i != pal.Entries.Length; ++i)
						{
							if (i >= dataTrns.Length)
								break;

							Color color = pal.Entries[i];
							pal.Entries[i] = Color.FromArgb(dataTrns[i], color.R, color.G, color.B);
						}
						b.Palette = pal;
					}

					// Images in .net often cause odd crashes when their backing
					// resource disappears. This prevents that from happening by
					// copying its inner contents into a new Bitmap object.
					return Copy(b);
				}
			}
			catch (Exception e)
			{
				showerror(".net could not load the Image.", e.Message);
				return null;
			}
		}

		/// <summary>
		/// Finds the start of a PNG chunk. This assumes the image is already
		/// identified as PNG. It does not go over the first 8 bytes but starts
		/// at the start of the header chunk.
		/// </summary>
		/// <param name="data">the bytes of the PNG image</param>
		/// <param name="chunkName">the name of the chunk to find</param>
		/// <returns>the offset of the start of the PNG chunk or -1 if the chunk
		/// was not found.</returns>
		private static int FindChunk(byte[] data, string chunkName)
		{
			byte[] chunkNameBytes = Encoding.ASCII.GetBytes(chunkName);
			var test = new byte[4];

			// continue until either the end is reached or there is not enough
			// space behind it for reading a new header

			int offset = Id_PNG.Length;
			while (offset < data.Length && offset + 8 < data.Length) // huh - if (offset + 8 < end) then shirley (offset < end)
			{
				Array.Copy(
						data, offset + 4,	// src,pos
						test, 0,			// dst,pos
						4);					// len

				if (chunkNameBytes.SequenceEqual(test))
					return offset;

				int chunkLength = GetChunkLength(data, offset);
				switch (chunkLength)
				{
					default:
						offset += chunkLength + 12; // chunk size + chunk header + chunk checksum = 12 bytes
						break;

					case -1:
						showerror("Bad chunk length in PNG image.");
						return -1;

					case -2:
						showerror("Bad chunk endianness in PNG image.");
						return -1;
				}
			}
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		private static int GetChunkLength(byte[] data, int offset)
		{
			if (offset + 4 > data.Length)
				return -1;

			// Don't want to use BitConverter; then you have to check platform
			// endianness and all that mess.
			int length =  data[offset + 3]
					   + (data[offset + 2] << 8)
					   + (data[offset + 1] << 16)
					   + (data[offset]     << 24);

			if (length < 0) return -2;

			return length;
		}

		/// <summary>
		/// Clones an image object to free it from any backing resources. Code
		/// taken from http://stackoverflow.com/a/3661892/ with some extra fixes.
		/// </summary>
		/// <param name="src">the image to clone</param>
		/// <returns>the cloned image</returns>
		/// <remarks>It's the responsibility of the caller to dispose the
		/// Bitmap.</remarks>
		private static Bitmap Copy(Bitmap src)
		{
			var rect = new Rectangle(0,0, src.Width, src.Height);
			var dst = new Bitmap(rect.Width, rect.Height, src.PixelFormat);

			dst.SetResolution(src.HorizontalResolution, src.VerticalResolution);

			var srcLocked = src.LockBits(rect, ImageLockMode.ReadOnly,  src.PixelFormat);
			var dstLocked = dst.LockBits(rect, ImageLockMode.WriteOnly, dst.PixelFormat);

			int actualDataWidth = (Image.GetPixelFormatSize(src.PixelFormat) * rect.Width + 7) / 8;
			int height = src.Height;
			int srcStride = srcLocked.Stride;
			int dstStride = dstLocked.Stride;

			var imageData = new byte[actualDataWidth];

			IntPtr srcPos = srcLocked.Scan0;
			IntPtr dstPos = dstLocked.Scan0;

			// Copy line by line, skipping by stride but copying actual data width
			for (int y = 0; y != height; ++y)
			{
				Marshal.Copy(srcPos, imageData, 0, actualDataWidth);
				Marshal.Copy(imageData, 0, dstPos, actualDataWidth);

				srcPos = new IntPtr(srcPos.ToInt64() + srcStride);
				dstPos = new IntPtr(dstPos.ToInt64() + dstStride);
			}

			dst.UnlockBits(dstLocked);
			src.UnlockBits(srcLocked);

			// For indexed images, restore the palette. This is not linking to a
			// referenced object in the original image; the getter of Palette
			// creates a new object when called.
			if (((int)src.PixelFormat & (int)PixelFormat.Indexed) != 0) // Indexed = 65536
				dst.Palette = src.Palette;

			dst.SetResolution(src.HorizontalResolution, src.VerticalResolution); // Restore DPI settings - wtf.

			return dst;
		}


		/// <summary>
		/// Displays an <see cref="Infobox"/>.
		/// </summary>
		/// <param name="head"></param>
		/// <param name="copy"></param>
		/// <param name="info"></param>
		private static void showerror(string head, string copy = null, bool info = false)
		{
			string title; InfoboxType bt;

			if (info)
			{
				title = "Load info";
				bt = InfoboxType.Info;
			}
			else
			{
				title = "Load error";
				bt = InfoboxType.Error;
			}

			using (var f = new Infobox(title, head, copy, bt))
				f.ShowDialog();
		}
	}
}
