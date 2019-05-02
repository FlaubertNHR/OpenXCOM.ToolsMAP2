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
		private const byte ByteMaximumValue = 0xFF; // ... trying to keep my head straight.
		/// <summary>
		/// A flag that is inserted into the file-data that indicates that an
		/// image's data has ended.
		/// </summary>
		public const byte SpriteStopByte = 0xFF;
		/// <summary>
		/// A flag that is inserted into the file-data that indicates that the
		/// following pixels are transparent.
		/// </summary>
		public const byte SpriteTransparencyByte = 0xFE;	// the PCK-file uses 0xFE to flag a succeeding quantity of pixels
															// as transparent. That is, it is *not* a color-id entry; it's
															// just a flag in the Pck-file. Stop using it as a color-id entry.
		/// <summary>
		/// Tracks the id of an image across all loaded terrainsets. Used only
		/// by 'MapInfoOutputBox'.
		/// </summary>
		private static int _id;
		#endregion


		#region Fields
		private readonly SpriteCollection _spriteset; // currently used only for ToString() override.
		#endregion Fields


		#region Properties
		/// <summary>
		/// MapId is used only by 'MapInfoOutputBox'.
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
		/// <param name="terrainId"></param>
		/// <param name="spriteset"></param>
		internal PckImage(
				byte[] bindata,
				Palette pal,
				int terrainId,
				SpriteCollection spriteset)
			:
				base(
					new byte[XCImage.SpriteWidth * XCImage.SpriteHeight],	// new byte[]{}
					XCImage.SpriteWidth,									// 0
					XCImage.SpriteHeight,									// 0
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					terrainId)
		{
			//LogFile.WriteLine("PckImage..cTor");

			_spriteset = spriteset;	// only for ToString().
			SetId = _id++;			// only for 'MapInfoOutputBox'.

			Pal = pal;

			_spriteset.BorkedBigobs = false;

			for (int id = 0; id != Bindata.Length; ++id)
				Bindata[id] = Palette.TransparentId; // Safety: byte arrays get initialized w/ "0" by default

			int posSrc = 0;
			int posDst = 0;

			if (bindata[0] != SpriteTransparencyByte)
				posDst = bindata[posSrc++] * XCImage.SpriteWidth;

			for (; posSrc != bindata.Length; ++posSrc)
			{
				switch (bindata[posSrc])
				{
					default:
						//LogFile.WriteLine(". Bindata.Length= " + Bindata.Length + " dst= " + dst);
						//LogFile.WriteLine(". bindata.Length= " + bindata.Length + " id= "  + id);

						if (posDst < Bindata.Length)
						{
							Bindata[posDst++] = bindata[posSrc];
							break;
						}

						// probly trying to load a 32x48 Bigobs pck in a 32x40 spriteset.
						// Note that this cannot be resolved absolutely.
						_spriteset.BorkedBigobs = true;
						return;

					case SpriteTransparencyByte: // skip quantity of pixels
						posDst += bindata[++posSrc];
						break;

					case SpriteStopByte: // end of image
						break;
				}
			}

//			if (!_spriteset.BorkedBigobs) // check if trying to load a 32x40 Terrain/Unit pck in a 32x48 spriteset.
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
		/// cTor[2]. For Clone().
		/// </summary>
		/// <param name="spriteset"></param>
		private PckImage(SpriteCollection spriteset)
		{
			// PckImage vars
			_spriteset = spriteset;
			SetId = -1;
		}
		#endregion cTor


		#region Methods (static)
		internal static int SaveSpritesetSprite(BinaryWriter bw, XCImage sprite)
		{
			var binlist = new List<byte>();

			int lenTransparent = 0;
			bool first = true;

			for (int id = 0; id != sprite.Bindata.Length; ++id)
			{
				byte b = sprite.Bindata[id];

				if (b == Palette.TransparentId)
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

						while (lenTransparent >= ByteMaximumValue)
						{
							lenTransparent -= ByteMaximumValue;

							binlist.Add(SpriteTransparencyByte);
							binlist.Add(ByteMaximumValue);
						}

						if (lenTransparent != 0)
						{
							binlist.Add(SpriteTransparencyByte);
							binlist.Add((byte)lenTransparent);
						}
						lenTransparent = 0;
					}
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
			// Note that this algorithm can and will tack on multiple 0xFE,0xFF
			// if there's more than 256 transparent pixels at the end of an image.


//			bool appendStopByte = false;
//			while (lenTransparent >= ByteMaximumValue)
//			{
//				lenTransparent -= ByteMaximumValue;
//
//				binlist.Add(SpriteTransparencyByte);
//				binlist.Add(ByteMaximumValue);
//
//				appendStopByte = true;
//			}
//
//			if (appendStopByte
//				|| (byte)binlist[binlist.Count - 1] != SpriteStopByte)
//			{
			binlist.Add(SpriteStopByte);
//			}

			// Okay. That seems to be the algorithm that was used. Ie, no need
			// to go through that final looping mechanism.
			//
			// In fact I'll bet it's even better than stock, since it no longer
			// appends superfluous 0xFE,0xFF markers at all.

			bw.Write(binlist.ToArray());

			return binlist.Count;
		}
		#endregion Methods (static)


		#region Methods (override)
		public override string ToString()
		{
			string ret = String.Empty;

			if (_spriteset != null)
				ret += _spriteset.ToString();

			ret += Id + Environment.NewLine;

			for (int i = 0; i != Bindata.Length; ++i)
			{
				ret += Bindata[i];

				switch (Bindata[i])
				{
					case SpriteStopByte:
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
		/// <returns></returns>
		public PckImage Clone(SpriteCollection spriteset)
		{
			var sprite = new PckImage(spriteset);

			// XCImage vars
			sprite.Bindata  = Bindata.Clone() as byte[];
			sprite.Sprite   = ObjectCopier.Clone<Bitmap>(Sprite);	// workaround Bitmap clone/copy/new shenanigans
			sprite.SpriteGr = ObjectCopier.Clone<Bitmap>(SpriteGr);	// workaround Bitmap clone/copy/new shenanigans
			sprite.Pal      = Pal;

			sprite.Id       = Id; // only this needs to be changed <- do that after the fact

			return sprite;
		}
		#endregion Methods
	}


	/// <summary>
	/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// Another reference article https://stackoverflow.com/questions/78536/deep-cloning-objects/78612#78612
	/// Provides a method for performing a deep copy of an object.
	/// Binary Serialization is used to perform the copy.
	/// </summary>
	public static class ObjectCopier
	{
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(T source)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "source");
			}

			// Don't serialize a null object, simply return the default for that object.
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}

			IFormatter formatter = new BinaryFormatter();
			using (Stream stream = new MemoryStream())
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
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

//		public int MapId
//		{
//			get { return _mapId; }
//			set { _mapId = value; }
//		}
