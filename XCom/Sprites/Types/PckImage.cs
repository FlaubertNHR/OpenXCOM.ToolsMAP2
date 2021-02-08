using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using DSShared;


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
		internal const byte MaxId = 0xFD;

		/// <summary>
		/// Tracks the id of an image across all loaded terrainsets.
		/// @note Used only by 'MapInfoDialog'.
		/// </summary>
		private static int _setId = -1;
		#endregion Fields (static)


		#region Fields
		private SpriteCollection _spriteset;
		#endregion Fields


		#region Properties
		/// <summary>
		/// A copy of the image but toned according to the currently selected
		/// tile toner.
		/// </summary>
		public Bitmap SpriteToned
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a PckImage, based on an XCImage.
		/// </summary>
		/// <param name="bindata">the COMPRESSED source data</param>
		/// <param name="pal"></param>
		/// <param name="id">the id of this sprite in its spriteset</param>
		/// <param name="spriteset"></param>
		internal PckImage(
				byte[] bindata,
				Palette pal,
				int id,
				SpriteCollection spriteset)
			:
				base(
					new byte[XCImage.SpriteWidth
						   * XCImage.SpriteHeight],
					XCImage.SpriteWidth,
					XCImage.SpriteHeight,
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					id)
		{
			_spriteset = spriteset;	// only for ToString().
			SetId = ++_setId;		// only for 'MapInfoDialog'.

			Pal = pal;

			int posDst = bindata[0] * XCImage.SpriteWidth; // first byte is always count of transparent rows

			for (int posSrc = 1; posSrc != bindata.Length; ++posSrc)
			{
				switch (bindata[posSrc])
				{
					case MarkerEos: // end of image
						//DSShared.LogFile.WriteLine(". EoS");
						break;

					case MarkerRle: // skip quantity of pixels
						posDst += bindata[++posSrc];
						//DSShared.LogFile.WriteLine(". posDst= " + posDst);
						break;

					default:
						if (posDst >= Bindata.Length)
						{
							//DSShared.LogFile.WriteLine(". . FAIL posDst= " + posDst);
							_spriteset.Fail |= SpriteCollection.FAIL_OF_SPRITE;
							return;
						}

						Bindata[posDst++] = bindata[posSrc];
						//DSShared.LogFile.WriteLine(". posDst= " + posDst);
						break;
				}
			}

			Sprite = BitmapService.CreateColored(
											XCImage.SpriteWidth,
											XCImage.SpriteHeight,
											Bindata,
											Pal.Table);

			if (Pal.GrayScale != null) // do NOT create ANY tone-scaled sprites for PckView or McdView
				SpriteToned = BitmapService.CreateColored(
													XCImage.SpriteWidth,
													XCImage.SpriteHeight,
													Bindata,
													Pal.GrayScale.Table); // default to grayscale.
		}

		/// <summary>
		/// cTor[1]. Creates a blank sprite for Duplicate().
		/// </summary>
		private PckImage()
		{}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Compresses and optionally writes a specified sprite to a specified
		/// stream.
		/// </summary>
		/// <param name="sprite">an XCImage sprite</param>
		/// <param name="bw">null for test only</param>
		/// <returns>the length of the sprite (in bytes) after compression</returns>
		internal static uint Write(XCImage sprite, BinaryWriter bw = null)
		{
			var binlist = new List<byte>();

			int tran = 0;
			bool first = true;

			for (int id = 0; id != sprite.Bindata.Length; ++id)
			{
				byte b = sprite.Bindata[id];

				if (b == Palette.Tid)
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
			binlist.Add(MarkerEos);

			if (bw != null) bw.Write(binlist.ToArray());

			return (uint)binlist.Count;
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
		#endregion Methods (static)


		#region Methods (override)
		/// <summary>
		/// Gets a string-representation of this PckImage incl/ its bindata.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string ret;

			if (_spriteset != null)
				ret = _spriteset + ": ";
			else
				ret = String.Empty;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is PckImage)
				return ToString().Equals(obj.ToString());

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
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
			sprite._spriteset = spriteset;
			sprite.SetId = -1;

			// XCImage vars
			sprite.Bindata     = Bindata.Clone() as byte[];
			sprite.Sprite      = ObjectCopier.Clone<Bitmap>(Sprite);		// workaround for Bitmap's clone/copy/new shenanigans
			sprite.SpriteToned = ObjectCopier.Clone<Bitmap>(SpriteToned);	// workaround for Bitmap's clone/copy/new shenanigans
			sprite.Pal         = Pal;

			sprite.Id = id;

			return sprite;
		}
		#endregion Methods
	}
}
