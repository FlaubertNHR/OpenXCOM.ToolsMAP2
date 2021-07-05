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

	public sealed class PckSprite
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
		/// <remarks>The PCK-file uses 0xFE to flag a succeeding quantity of
		/// pixels as transparent. That is it is *not* a color-id entry; it's
		/// just a marker in the Pck-file. Stop using it as a color-id entry.</remarks>
		public const byte MarkerRle = 0xFE;

		/// <summary>
		/// The maximum valid color-id in a pck-packaged sprite.
		/// </summary>
		internal const byte MaxId = 0xFD;

		/// <summary>
		/// The ID of a sprite across all loaded terrainsets.
		/// </summary>
		/// <remarks>Used only by <c>MapView.MapInfoDialog</c>.</remarks>
		private static int _ordinal = -1;
		internal static void ResetOrdinal()
		{
			_ordinal = -1;
		}
		#endregion Fields (static)


		#region Fields
		private Spriteset _spriteset;
		#endregion Fields


		#region Properties
		/// <summary>
		/// A copy of the image but toned according to the currently selected
		/// tile toner.
		/// </summary>
		public Bitmap SpriteToned
		{ get; internal set; }

		/// <summary>
		/// The ID of a sprite across all loaded terrainsets.
		/// </summary>
		/// <remarks>Used only by <c>MapView.MapInfoDialog</c>.</remarks>
		public int Ordinal
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a <c><see cref="PckSprite"/></c>.
		/// </summary>
		/// <param name="bindata">the COMPRESSED source-data</param>
		/// <param name="pal">the <c><see cref="Palette"/></c></param>
		/// <param name="id">the id of this sprite in its <c>Spriteset</c></param>
		/// <param name="spriteset">the <c><see cref="Spriteset"/></c> this
		/// belongs to</param>
		/// <param name="createToned"><c>true</c> to create
		/// <c><see cref="PckSprite.SpriteToned">PckSprite.SpriteToned</see></c>
		/// sprites for MapView</param>
		internal PckSprite(
				byte[] bindata,
				Palette pal,
				int id,
				Spriteset spriteset,
				bool createToned = false)
			:
				base(
					new byte[XCImage.SpriteWidth
						   * XCImage.SpriteHeight],
					XCImage.SpriteWidth,
					XCImage.SpriteHeight,
					null, // do *not* pass 'pal' in here. See XCImage..cTor
					id)
		{
			_spriteset = spriteset; // only for ToString() and 'Fail'.

			if (createToned)
				Ordinal = ++_ordinal; // only for 'MapInfoDialog'.

			Pal = pal;

			//Logfile.Log("PckSprite spriteset= " + spriteset.Label + " Pal= " + Pal + " Id= " + Id + " Ordinal= " + Ordinal);

			//Logfile.Log("PckSprite..cTor id= " + id + " bindata.Length= " + bindata.Length);

			int dst = bindata[0] * XCImage.SpriteWidth; // first byte is count of transparent rows
			for (int src = 1; src != bindata.Length; ++src)
			{
				switch (bindata[src])
				{
					case MarkerEos: // end of sprite
						//Logfile.Log(". EoS");
						break;

					case MarkerRle: // skip quantity of pixels
						dst += bindata[++src];
						//Logfile.Log(". dst= " + dst);
						break;

					default:
						if (dst >= GetBindata().Length)
						{
							//Logfile.Log(". . FAIL dst= " + dst);
							_spriteset.Fail |= Spriteset.FAIL_pck;
							return;
						}

						GetBindata()[dst++] = bindata[src];
						//Logfile.Log(". dst= " + dst);
						break;
				}
			}

			Sprite = BitmapService.CreateSprite(
											XCImage.SpriteWidth,
											XCImage.SpriteHeight,
											GetBindata(),
											Pal.Table);

			// do NOT create ANY tone-scaled sprites for PckView or McdView nor
			// MapView's MonotoneSprites or UFO/TFTD cursor-sprites
			if (createToned)
				SpriteToned = BitmapService.CreateSprite(
													XCImage.SpriteWidth,
													XCImage.SpriteHeight,
													GetBindata(),
													Pal.GrayScale.Table); // default to grayscale.
		}

		/// <summary>
		/// cTor[1]. Creates a blank sprite for
		/// <c><see cref="Duplicate()">Duplicate()</see></c>.
		/// </summary>
		private PckSprite()
		{}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Compresses and optionally writes a specified sprite to a specified
		/// stream.
		/// </summary>
		/// <param name="sprite">an <c><see cref="XCImage"/></c> sprite</param>
		/// <param name="bw"><c>null</c> for test only</param>
		/// <returns>the length of the sprite in <c>bytes</c> after compression</returns>
		internal static uint Write(XCImage sprite, BinaryWriter bw = null)
		{
			var binlist = new List<byte>();

			int tran = 0;
			bool first = true;

			for (int id = 0; id != sprite.GetBindata().Length; ++id)
			{
				byte b = sprite.GetBindata()[id];

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
		// that reads and decompresses the data will force them to transparent
		// ...
		//
		// It looks like both OpenXcom and MapView will fill the sprite with
		// all transparent pixels when each sprite is initialized. Therefore,
		// it's not *required* to encode any pixels that are transparent to
		// the end of the sprite.
		//
		// And when looking at some of the stock PCK's things look
		// non-standardized. It's sorta like if there's at least one full row of
		// transparent pixels at the end of an image, it gets 0xFE,0xFF tacked
		// on before the final 0xFF (end of image) marker.
		#endregion Methods (static)


		#region Methods (override)
		/// <summary>
		/// Gets a string-representation of this <c>PckSprite</c> incl/ its
		/// bindata.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Not used.</remarks>
		public override string ToString()
		{
			string ret;

			if (_spriteset != null)
				ret = _spriteset + ": ";
			else
				ret = String.Empty;

			ret += Id + Environment.NewLine;

			for (int i = 0; i != GetBindata().Length; ++i)
			{
				ret += GetBindata()[i];

				switch (GetBindata()[i])
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
		#endregion Methods (override)


		#region Methods
		/// <summary>
		/// Returns a deep clone of this <c>PckSprite</c>. Except
		/// <c><see cref="Pal"/></c>.
		/// </summary>
		/// <param name="spriteset">the <c><see cref="Spriteset"/></c> that the
		/// sprite will belong to; note that the returned sprite has not been
		/// transfered to this other spriteset yet</param>
		/// <param name="id">the id in the destination <c>Spriteset</c></param>
		/// <returns></returns>
		public PckSprite Duplicate(
				Spriteset spriteset,
				int id)
		{
			var sprite = new PckSprite();

			// PckSprite vars
			sprite._spriteset = spriteset;
//			sprite.Ordinal = -1; // not used.

			// XCImage vars
			sprite.SetBindata(GetBindata().Clone() as byte[]);

			sprite.Sprite      = ObjectCopier.Clone<Bitmap>(Sprite);		// workaround for Bitmap's clone/copy/new shenanigans
			sprite.SpriteToned = ObjectCopier.Clone<Bitmap>(SpriteToned);	// workaround for Bitmap's clone/copy/new shenanigans
			sprite.Pal         = Pal;

			sprite.Id = id;

			return sprite;
		}
		#endregion Methods
	}
}
