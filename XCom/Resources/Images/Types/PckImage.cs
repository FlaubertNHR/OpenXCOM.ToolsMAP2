using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using XCom.Interfaces;


namespace XCom
{
	// kL_note: It appears that RLE-encoded .Pck images cannot use color-indices
	// of 0xFE or 0xFF. Because, in the Pck-packed file, 0xFF is used to denote
	// the end of each image's bindata, and 0xFE is used to flag a quantity of
	// pixels as transparent by using 2 bytes: 0xFE itself, and the next byte is
	// the quantity of pixels that are transparent and hence are not written.

	public sealed class PckImage
		:
			XCImage
	{
		#region Fields (static)
		/// <summary>
		/// A marker that is inserted into the file-data that indicates that an
		/// image's data has ended.
		/// </summary>
		public const byte MarkerEos = 0xFF;

		/// <summary>
		/// A marker that is inserted into the file-data that indicates that the
		/// next byte is a quantity of pixels that are transparent. This is part
		/// of the RLE-compression.
		/// </summary>
		public const byte MarkerRle = 0xFE;	// the PCK-file uses 0xFE to flag a succeeding quantity of pixels
											// as transparent. That is, it is *not* a color-id entry; it's
											// just a marker in the Pck-file. Stop using it as a color-id entry.

		/// <summary>
		/// The maximum valid color-id in a pck-packaged sprite.
		/// </summary>
		public const byte MxId = 0xFD;

		/// <summary>
		/// Tracks the id of an image across all loaded terrainsets. Used only
		/// by 'MapInfoOutputBox'.
		/// </summary>
		private static int _setId;
		#endregion Fields (static)


		#region Fields
		private SpriteCollection Spriteset;
		#endregion Fields


		#region Properties
		/// <summary>
		/// SetId is used only by 'MapInfoOutputBox'.
		/// </summary>
		public int SetId
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[1]. Instantiates a PckImage, based on an XCImage.
		/// </summary>
		/// <param name="bindata">the COMPRESSED source data</param>
		/// <param name="pal"></param>
		/// <param name="pckId">the id of this sprite in its spriteset</param>
		/// <param name="spriteset"></param>
		internal PckImage(
				byte[] bindata,
				Palette pal,
				int pckId,
				SpriteCollection spriteset)
			:
				base(
					new byte[XCImage.SpriteWidth * XCImage.SpriteHeight],	// new byte[]{}
					XCImage.SpriteWidth,									// 0
					XCImage.SpriteHeight,									// 0
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					pckId)
		{
			Spriteset = spriteset;	// only for ToString().
			SetId = _setId++;		// only for 'MapInfoOutputBox'.

			Pal = pal;

			Spriteset.BorkedBigobs = false;

			for (int id = 0; id != Bindata.Length; ++id)
				Bindata[id] = Palette.TranId; // Safety: byte arrays get initialized w/ "0" by default

			int posSrc = 0;
			int posDst = 0;

			posDst = bindata[posSrc] * XCImage.SpriteWidth; // first byte is always count of transparent rows

			for (posSrc = 1; posSrc != bindata.Length; ++posSrc)
			{
				switch (bindata[posSrc])
				{
					case MarkerEos: // end of image
						break;

					case MarkerRle: // skip quantity of pixels
						posDst += bindata[++posSrc];
						break;

					default:
						if (posDst < Bindata.Length)
						{
							Bindata[posDst++] = bindata[posSrc];
							break;
						}

						// could be trying to load a 32x48 Bigobs pck in a 32x40 spriteset.
						// Note that this cannot be resolved absolutely.
						Spriteset.BorkedBigobs = true;
						return;
				}
			}

//			if (!Spriteset.BorkedBigobs) // check if trying to load a 32x40 Terrain/Unit pck in a 32x48 spriteset.
//			{
//				// there's no way to determine this.
//			}
//			else
//			{
			Sprite = BitmapService.CreateColorized(
												XCImage.SpriteWidth,
												XCImage.SpriteHeight,
												Bindata,
												Pal.ColorTable);
			SpriteGr = BitmapService.CreateColorized(
												XCImage.SpriteWidth,
												XCImage.SpriteHeight,
												Bindata,
												Pal.Grayscale.ColorTable);
//			}
		}

		/// <summary>
		/// cTor[2]. Creates a blank sprite for Duplicate().
		/// </summary>
		private PckImage()
		{}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="sprite"></param>
		/// <returns></returns>
		internal static uint SaveSpritesetSprite(BinaryWriter bw, XCImage sprite)
		{
			var binlist = new List<byte>();

			int tran = 0;
			bool first = true;

			for (int id = 0; id != sprite.Bindata.Length; ++id)
			{
				byte b = sprite.Bindata[id];

				if (b == Palette.TranId)
				{
					++tran;
				}
				else
				{
					if (tran != 0)
					{
						if (first)
						{
							binlist.Add((byte)(tran / sprite.Sprite.Width));	// qty of initial transparent rows
							tran      = (byte)(tran % sprite.Sprite.Width);		// qty of transparent pixels starting on the next row
						}

						while (tran >= Byte.MaxValue)
						{
							tran -= Byte.MaxValue;

							binlist.Add(MarkerRle);
							binlist.Add(Byte.MaxValue);
						}

						if (tran != 0)
						{
							binlist.Add(MarkerRle);
							binlist.Add((byte)tran);
						}
						tran = 0;
					}
					else if (first)
					{
						binlist.Add((byte)0);
					}

					first = false;
					binlist.Add(b);
				}
			}

			// So, question. Is one obligated to account for transparent pixels
			// to the end of an image, or can one just assume that the program
			// that reads and decompresses the data will force them to transparent ...
			//
			// It looks like both OpenXcom and MapView will fill the sprite with
			// all transparent pixels when each sprite is initialized. Therefore,
			// it's not *required* to encode any pixels that are transparent to
			// the end of the sprite.
			//
			// And when looking at some of the stock PCK's things look non-standardized.
			// It's sorta like if there's at least one full row of transparent
			// pixels at the end of an image, it gets 0xFE,0xFF tacked on before
			// the final 0xFF (end of image) marker.
			//
			// Obsolete: This algorithm can and will tack on multiple 0xFE,0xFF
			// if there's more than 256 transparent pixels at the end of an image.

//			bool appendStopByte = false;
//			while (lenTransparent >= Byte.MaxValue)
//			{
//				lenTransparent -= Byte.MaxValue;
//
//				binlist.Add(MarkerRle);
//				binlist.Add(Byte.MaxValue);
//
//				appendStopByte = true;
//			}
//
//			if (appendStopByte
//				|| (byte)binlist[binlist.Count - 1] != MarkerEos)
//			{
			binlist.Add(MarkerEos);
//			}

			// Okay. That seems to be the algorithm that was used. Ie, no need
			// to go through that final looping mechanism.
			//
			// In fact I'll bet it's even better than stock, since it no longer
			// appends superfluous 0xFE,0xFF markers at all.

			bw.Write(binlist.ToArray());

			return (uint)binlist.Count;
		}

		/// <summary>
		/// Creates a mockup of an RLE-encoded sprite and returns its length.
		/// </summary>
		/// <param name="sprite"></param>
		/// <returns></returns>
		internal static uint TestSprite(XCImage sprite)
		{
			var binlist = new List<byte>();

			int lenTransparent = 0;
			bool first = true;

			for (int id = 0; id != sprite.Bindata.Length; ++id)
			{
				byte b = sprite.Bindata[id];

				if (b == Palette.TranId)
					++lenTransparent;
				else
				{
					if (lenTransparent != 0)
					{
						if (first)
						{
							first = false;

							binlist     .Add((byte)(lenTransparent / sprite.Sprite.Width));	// qty of initial transparent rows
							lenTransparent = (byte)(lenTransparent % sprite.Sprite.Width);	// qty of transparent pixels starting on the next row
						}

						while (lenTransparent >= Byte.MaxValue)
						{
							lenTransparent -= Byte.MaxValue;

							binlist.Add(MarkerRle);
							binlist.Add(Byte.MaxValue);
						}

						if (lenTransparent != 0)
						{
							binlist.Add(MarkerRle);
							binlist.Add((byte)lenTransparent);
						}
						lenTransparent = 0;
					}
					binlist.Add(b);
				}
			}

			binlist.Add(MarkerEos);
			return (uint)binlist.Count;
		}
		#endregion Methods (static)


		#region Methods (override)
		public override string ToString()
		{
			string ret = String.Empty;

			if (Spriteset != null)
				ret += Spriteset.ToString();

			ret += Id + Environment.NewLine;

			for (int i = 0; i != Bindata.Length; ++i)
			{
				ret += Bindata[i];

				switch (Bindata[i])
				{
					case MarkerEos:
						ret += Environment.NewLine;
						break;

					default:
						ret += " ";
						break;
				}
			}
			return ret;
		}

		public override bool Equals(object obj)
		{
			if (obj is PckImage)
				return ToString().Equals(obj.ToString());

			return false;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		#endregion Methods (override)


		#region Methods
		/// <summary>
		/// Returns a deep clone of this PckImage. Except 'Pal'.
		/// </summary>
		/// <param name="spriteset">the spriteset that the sprite will belong
		/// to; note that the returned sprite has not been transfered to this
		/// other spriteset yet</param>
		/// <param name="id">the id in the destination spriteset</param>
		/// <returns></returns>
		public PckImage Duplicate(
				SpriteCollection spriteset,
				int id)
		{
			var sprite = new PckImage();

			// PckImage vars
			sprite.Spriteset = spriteset;
			sprite.SetId = -1;

			// XCImage vars
			sprite.Bindata  = Bindata.Clone() as byte[];
			sprite.Sprite   = ObjectCopier.Clone<Bitmap>(Sprite);	// workaround for Bitmap's clone/copy/new shenanigans
			sprite.SpriteGr = ObjectCopier.Clone<Bitmap>(SpriteGr);	// workaround for Bitmap's clone/copy/new shenanigans
			sprite.Pal      = Pal;

			sprite.Id = id;

			return sprite;
		}
		#endregion Methods
	}


	/// <summary>
	/// Provides a method for performing a deep copy of an object. Binary
	/// Serialization is used to perform the copy.
	/// Reference articles:
	/// http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// https://stackoverflow.com/questions/78536/deep-cloning-objects/78612#78612
	/// </summary>
	public static class ObjectCopier
	{
		/// <summary>
		/// Performs a deep copy of a specified object.
		/// </summary>
		/// <typeparam name="T">the type of object being copied</typeparam>
		/// <param name="src">the object instance to copy</param>
		/// <returns>the copied object</returns>
		public static T Clone<T>(T src)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "src");
			}

			// don't serialize a null object - return the default for that object
			if (Object.ReferenceEquals(src, null))
			{
				return default(T);
			}

			IFormatter bf = new BinaryFormatter();
			using (Stream str = new MemoryStream())
			{
				bf.Serialize(str, src);
				str.Seek(0, SeekOrigin.Begin);
				return (T)bf.Deserialize(str);
			}
		}
	}
}

//		public override void Hq2x()
//		{
//			if (Width == 32) // hasn't been done yet
//				base.Hq2x();
//		}

//		public static Type GetCollectionType()
//		{
//			return typeof(SpriteCollection);
//		}

//		public void ReImage()
//		{
//			_image = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Colors);
//			_gray = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Grayscale.Colors);
//		}

//		public void MoveImage(byte offset)
//		{
//			_id[_moveId] = (byte)(_moveVal - offset);
//			int ex = 0;
//			int startIdx = 0;
//			for (int i = 0; i != _expanded.Length; ++i)
//				_expanded[i] = TransparentIndex;
//
//			if (_id[0] != FileTransparencyByte)
//				ex = _id[startIdx++] * Width;
//
//			for (int i = startIdx; i < _id.Length; ++i)
//			{
//				switch (_id[i])
//				{
//					case FileTransparencyByte: // skip quantity of pixels
//						ex += _id[i + 1];
//						++i;
//						break;
//
//					case FileStopByte: // end of image
//						break;
//
//					default:
//						_expanded[ex++] = _id[i];
//						break;
//				}
//			}
//		
//			_image = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Colors);
//			_gray = Bmp.MakeBitmap8(
//								Width,
//								Height,
//								_expanded,
//								Palette.Grayscale.Colors);
//		}
