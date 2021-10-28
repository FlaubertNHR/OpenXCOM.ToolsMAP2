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
		#region Fields (static)
		/// <summary>
		/// path_file_extension of the file for failure info.
		/// </summary>
		private static string Pfe;

		private static byte[] Id_PNG = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
		private static byte[] Id_GIF = { 0x47, 0x49, 0x46, 0x38 };
		private static byte[] Id_BMP = { 0x42, 0x4D };
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Creates a <c>Bitmap</c> from a specified byte-array of
		/// <c>PNG</c>/<c>GIF</c>/<c>BMP</c> file-data. Checks if it is a
		/// <c>PNG</c> containing palette transparency and if so ensures it
		/// loads correctly.
		/// </summary>
		/// <param name="filedata">byte-array to load</param>
		/// <param name="pfe">path_file_extension of the file for failure info</param>
		/// <returns>a CLONED image - the onus is on the receiver for disposal</returns>
		/// <remarks>The theory on the PNG internals can be found at
		/// http://www.libpng.org/pub/png/book/chapter08.html</remarks>
		public static Bitmap LoadImageData(byte[] filedata, string pfe)
		{
			Pfe = pfe;

			if (!CheckValidHeader(filedata))
			{
				error("File does not have a PNG/GIF/BMP header.", Pfe);
				return null;
			}

			byte[] dataTrns = null;

			if (filedata.Length > Id_PNG.Length)
			{
				var data1 = new byte[Id_PNG.Length];
				Array.Copy(filedata, data1, Id_PNG.Length);

				if (Id_PNG.SequenceEqual(data1)) // check if the image is a PNG
				{
					// check if it contains a palette
					// I'm sure it can be looked up in the header somehow, but meh.

					if (FindChunk(filedata, "PLTE") != -1)
					{
						// check if it contains a palette transparency chunk
						int trnsOffset = FindChunk(filedata, "tRNS");
						if (trnsOffset != -1)
						{
							// get chunk
							int trnsLength = GetChunkLength(filedata, trnsOffset);
							switch (trnsLength)
							{
								default:
									dataTrns = new byte[trnsLength];
									Array.Copy(filedata, trnsOffset + 8, dataTrns, 0, trnsLength);

									// filter out the palette alpha chunk, make new data array
									var data = new byte[filedata.Length - (trnsLength + 12)];
									Array.Copy(filedata, 0, data, 0, trnsOffset);

									int trnsEnd = trnsOffset + trnsLength + 12;
									Array.Copy(filedata, trnsEnd, data, trnsOffset, filedata.Length - trnsEnd);

									filedata = data;
									break;

								case -1:
									error("Bad chunk length in PNG image.", Pfe);
									return null;

								case -2:
									error("Bad chunk endianness in PNG image.", Pfe);
									return null;
							}
						}
//						else error("Chunk not found in PNG image: tRNS", true);
					}
//					else error("Chunk not found in PNG image: PLTE", true);
				}
//				else error("Image is not a PNG.", true);
			}
//			else error("Data length is less than or equal to PNG identifier.", true);


			try
			{
				using (var str = new MemoryStream(filedata))
				using (var b = new Bitmap(str))
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

//					Logfile.Log("b " + b.Width + "x" + b.Height + " " + b.PixelFormat); // b 32x40 Format8bppIndexed
//
//					var b1 = Clone(b);
//					var b2 = ObjectCopier.Clone<Bitmap>(b); // Format32bppArgb wtf.
//
//					// so how does ObjectCopier.Clone<Bitmap>(b) turn Format8bppIndexed into Format32bppArgb
//					// ... it doesn't do that in McdView.TerrainPanel_main.addPart() : 8bpp stays 8bpp
//
//					//Logfile.Log("b1 " + b1.Width + "x" + b1.Height + " " + b1.PixelFormat); // b1 32x40 Format8bppIndexed
//					//Logfile.Log("b2 " + b2.Width + "x" + b2.Height + " " + b2.PixelFormat); // b2 32x40 Format32bppArgb
//
//					return b1;

					return Clone(b);
				}
			}
			catch (Exception e)
			{
				error(".net could not load the Image.", Pfe + Environment.NewLine + Environment.NewLine + e.Message);
				return null;
			}
		}

		/// <summary>
		/// Checks a specified byte-array's initial bytes for an image-format
		/// that is supported as input to PckView -
		/// <c>PNG</c>/<c>GIF</c>/<c>BMP</c>.
		/// </summary>
		/// <param name="data">file data to check</param>
		/// <returns><c>true</c> if valid</returns>
		/// <remarks>This is ofc not entirely robust. Image-formats with their
		/// chunks and subtypes get complicated real fast. So
		/// <c><see cref="LoadImageData()">LoadImageData()</see></c> will
		/// further run a try/catch block to display any .NET <c>Exception</c>
		/// when trying to create the <c>Bitmap</c>.</remarks>
		private static bool CheckValidHeader(byte[] data)
		{
			bool fail = false;
			if (data.Length >= Id_PNG.Length)
			{
				for (int i = 0; i != Id_PNG.Length; ++i)
				if (data[i] != Id_PNG[i])
				{ fail = true; break; }
			}
			else fail = true;

			if (!fail) return true;

			fail = false;
			if (data.Length >= Id_GIF.Length)
			{
				for (int i = 0; i != Id_GIF.Length; ++i)
				if (data[i] != Id_GIF[i])
				{ fail = true; break; }
			}
			else fail = true;

			if (!fail) return true;

			fail = false;
			if (data.Length >= Id_BMP.Length)
			{
				for (int i = 0; i != Id_BMP.Length; ++i)
				if (data[i] != Id_BMP[i])
				{ fail = true; break; }
			}
			else fail = true;

			return !fail;
		}
//		private static bool validateImage(byte[] data)
//		{
//			try
//			{
//				Stream str = new MemoryStream(data);
//				using (Image i = Image.FromStream(str))
//				{
//					if (   i.RawFormat.Equals(ImageFormat.Bmp)
//						|| i.RawFormat.Equals(ImageFormat.Gif)
//						|| i.RawFormat.Equals(ImageFormat.Jpeg)
//						|| i.RawFormat.Equals(ImageFormat.Png))
//					{
//						return true;
//					}
//				}
//				return false;
//			}
//			catch
//			{
//				return false;
//			}
//		}


		/// <summary>
		/// Finds the start of a <c>PNG</c> chunk. This assumes the image is
		/// already identified as <c>PNG</c>. It does not go over the first 8
		/// bytes but starts at the start of the header chunk.
		/// </summary>
		/// <param name="data">the bytes of the <c>PNG</c> image</param>
		/// <param name="chunkName">the name of the chunk to find</param>
		/// <returns>the offset of the start of the <c>PNG</c> chunk or
		/// <c>-1</c> if the chunk was not found.</returns>
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
						error("Bad chunk length in PNG image.", Pfe);
						return -1;

					case -2:
						error("Bad chunk endianness in PNG image.", Pfe);
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
			if (offset + 4 > data.Length) return -1;

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
		/// Clones a specified <c>Bitmap</c> to free it from any backing
		/// resources. Code taken from http://stackoverflow.com/a/3661892/ with
		/// some extra fixes.
		/// </summary>
		/// <param name="src">the image to clone</param>
		/// <returns>the cloned image</returns>
		/// <remarks>It's the responsibility of the caller to dispose both
		/// source and destination <c>Bitmaps</c>.
		/// 
		/// 
		/// See also <c><see cref="ObjectCopier"/>.Clone()</c> - for whatever
		/// reason they're not interchangeable.
		/// </remarks>
		private static Bitmap Clone(Bitmap src)
		{
			int height = src.Height;

			var rect = new Rectangle(0,0, src.Width, height);
			var dst = new Bitmap(rect.Width, rect.Height, src.PixelFormat);

			dst.SetResolution(src.HorizontalResolution, src.VerticalResolution);

			var srcLocked = src.LockBits(rect, ImageLockMode.ReadOnly,  src.PixelFormat);
			var dstLocked = dst.LockBits(rect, ImageLockMode.WriteOnly, dst.PixelFormat);

			long srcStride = (long)srcLocked.Stride;
			long dstStride = (long)dstLocked.Stride;

			int actualDataWidth = (Image.GetPixelFormatSize(src.PixelFormat) * rect.Width + 7) / 8;
			var imageData = new byte[actualDataWidth];

			IntPtr srcPos = srcLocked.Scan0;
			IntPtr dstPos = dstLocked.Scan0;

			// copy line by line skipping by stride but copying actual data width
			for (int y = 0; y != height; ++y)
			{
				Marshal.Copy(srcPos, imageData, 0, actualDataWidth);
				Marshal.Copy(imageData, 0, dstPos, actualDataWidth);

				srcPos = new IntPtr(srcPos.ToInt64() + srcStride);
				dstPos = new IntPtr(dstPos.ToInt64() + dstStride);
			}

			dst.UnlockBits(dstLocked);
			src.UnlockBits(srcLocked);

			// restore the palette for indexed images.
			// This is not linking to a referenced object in the original image;
			// the getter of Palette creates a new object when called.
			if (((int)src.PixelFormat & (int)PixelFormat.Indexed) != 0)
				dst.Palette = src.Palette;

			dst.SetResolution(src.HorizontalResolution, src.VerticalResolution); // Restore DPI settings - wtf.

			return dst;
		}


		/// <summary>
		/// Displays an <c><see cref="Infobox"/></c>.
		/// </summary>
		/// <param name="head"></param>
		/// <param name="copy"></param>
		/// <param name="info"></param>
		private static void error(
				string head,
				string copy = null,
				bool info = false)
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
		#endregion Methods (static)
	}
}
