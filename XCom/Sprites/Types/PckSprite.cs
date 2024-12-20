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
		/// <remarks>The PCK-file uses <c>0xFF</c> to flag the end of each
		/// sprite. The only other place that it is allowed is after a
		/// <c><see cref="MarkerRle"/></c> flag. That is it is *not* a color-id
		/// entry; it's just a marker in the Pck-file. Stop using it as a
		/// color-id entry.</remarks>
		public const byte MarkerEos = 0xFF;

		/// <summary>
		/// A marker that is inserted into the file-data that indicates that the
		/// next byte is a quantity of pixels that are transparent. This is part
		/// of the RLE-compression.
		/// </summary>
		/// <remarks>The PCK-file uses <c>0xFE</c> to flag a succeeding quantity
		/// of pixels as transparent. That is it is *not* a color-id entry; it's
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
		/// A copy of the sprite but toned according to the currently selected
		/// tile-toner.
		/// </summary>
		public Bitmap SpriteToned
		{ get; internal set; }

		/// <summary>
		/// The ID of this <c>PckSprite</c> across all loaded terrainsets.
		/// </summary>
		/// <remarks>Used only by <c>MapView.MapInfoDialog</c>.</remarks>
		public int Ordinal
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a <c>PckSprite</c>.
		/// </summary>
		/// <param name="bindata">the COMPRESSED source-data</param>
		/// <param name="width">the width of the <c><see cref="Sprite"/></c></param>
		/// <param name="height">the height of the <c><see cref="Sprite"/></c></param>
		/// <param name="pal">a pointer to its initial
		/// <c><see cref="Palette"/></c></param>
		/// <param name="id">the id of this <c>PckSprite</c> in its
		/// <c><see cref="Spriteset"/></c></param>
		/// <param name="spriteset">the <c><see cref="Spriteset"/></c> this
		/// belongs to</param>
		/// <param name="createToned"><c>true</c> to create
		/// <c><see cref="PckSprite.SpriteToned">PckSprite.SpriteToned</see></c>
		/// sprites for MapView</param>
		internal PckSprite(
				byte[] bindata,
				int width,
				int height,
				Palette pal,
				int id,
				Spriteset spriteset,
				bool createToned)
		{
			_spriteset = spriteset; // only for ToString(), 'Fail', and Duplicate().

			if (createToned)
				Ordinal = ++_ordinal; // only for 'MapInfoDialog'.

			Id  = id;
			Pal = pal;

			//Logfile.Log("PckSprite spriteset= " + spriteset.Label + " Pal= " + Pal + " Id= " + Id + " Ordinal= " + Ordinal);
			//Logfile.Log(". bindata.Length= " + bindata.Length);
			//string data = String.Empty;
			//foreach (var b in bindata) data += (b + ",");
			//Logfile.Log(". bindata= " + data);

			_width   = width;
			_height  = height;
			_bindata = new byte[_width * _height];
			//Logfile.Log(". _bindata.Length= " + _bindata.Length);

			int dst = bindata[0] * _width; // first byte is count of transparent rows
			for (int src = 1; src != bindata.Length; ++src)
			{
				switch (bindata[src])
				{
					case MarkerEos: // end of sprite
						if (src != bindata.Length - 1)
						{
							//Logfile.Log(src + " . FAIL dst= " + dst + " val= " + bindata[src] + " SpritesetFail.eos");
							_spriteset.Failreason = SpritesetFail.eos;
							_spriteset.Failid = Id;
							return;
						}
						//Logfile.Log(src + " . EoS" + " val= " + bindata[src]);
						break;

					case MarkerRle: // skip quantity of pixels
						dst += bindata[++src];
						//Logfile.Log(src + " . dst= " + dst + " - skip " + bindata[src]);
						break;

					default:
						if (dst >= _bindata.Length)
						{
							//Logfile.Log(src + " . . FAIL dst= " + dst + " val= " + bindata[src] + " SpritesetFail.ovr");
							_spriteset.Failreason = SpritesetFail.ovr;
							_spriteset.Failid = Id;
							return;
						}

						//Logfile.Log(src + " . dst= " + dst + " val= " + bindata[src]);
						_bindata[dst++] = bindata[src];
						break;
				}
			}

			Sprite = SpriteService.CreateSprite(
											_width,
											_height,
											_bindata,
											Pal.Table);


			// do NOT create ANY tone-scaled sprites for PckView or McdView nor
			// MapView's MonotoneSprites or CrippledSprites or UFO/TFTD
			// cursor-sprites (nor for the TonedSprites themselves) ->

			if (createToned)
				SpriteToned = SpriteService.CreateSprite(
													_width,
													_height,
													_bindata,
													Pal.GrayScale.Table); // default to grayscale.
		}


		/// <summary>
		/// cTor[1]. Instantiates a <c>PckSprite</c>.
		/// </summary>
		/// <param name="bindata">the UNCOMPRESSED source-data</param>
		/// <param name="width">the width of the <c><see cref="Sprite"/></c></param>
		/// <param name="height">the height of the <c><see cref="Sprite"/></c></param>
		/// <param name="id">the id of this <c>PckSprite</c> in its
		/// <c><see cref="Spriteset"/></c></param>
		/// <param name="pal">a pointer to its initial
		/// <c><see cref="Palette"/></c></param>
		/// <remarks>Used only by PckView - although this <c>PckSprite</c> is
		/// added to a <c><see cref="Spriteset"/></c>
		/// <c><see cref="_spriteset"/></c> can remain <c>null</c>.</remarks>
		internal PckSprite(
				byte[] bindata,
				int width,
				int height,
				int id,
				Palette pal)
		{
			_bindata = bindata;
			_width   = width;
			_height  = height;

			Id  = id;
			Pal = pal;

			Sprite = SpriteService.CreateSprite(
											_width,
											_height,
											_bindata,
											Pal.Table);
		}


		/// <summary>
		/// cTor[2]. Creates a blank sprite for
		/// <c><see cref="Duplicate()">Duplicate()</see></c>.
		/// </summary>
		private PckSprite()
		{}
		#endregion cTor


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

			ret += Id; /* + Environment.NewLine;

			for (int i = 0; i != _bindata.Length; ++i)
			{
				ret += _bindata[i];

				switch (_bindata[i])
				{
					case MarkerEos:
						ret += Environment.NewLine;
						break;

					default:
						ret += " ";
						break;
				}
			} */
			return ret;
		}
		#endregion Methods (override)


		#region Methods
		/// <summary>
		/// Compresses and optionally writes this <c>PckSprite</c> to a
		/// specified stream.
		/// </summary>
		/// <param name="bw"><c>null</c> for test only</param>
		/// <returns>the length of the sprite in <c>bytes</c> after compression</returns>
		internal uint Write(BinaryWriter bw = null)
		{
			//Logfile.Log("PckSprite.Write() Id= " + Id + " _bindata.Length= " + _bindata.Length + (bw == null ? " test" : String.Empty));

			var binlist = new List<byte>();

			if (Istid())
			{
				binlist.Add((byte)0);
			}
			else
			{
				bool start = true;

				int tally = 0; // tally of transparent pixels

				byte b;
				for (int id = 0; id != _bindata.Length; ++id)
				{
					if ((b = _bindata[id]) == Palette.Tid)
					{
						++tally;
					}
					else
					{
						if (tally != 0)
						{
							if (start)
							{
								start = false;
								binlist.Add((byte)(tally / Sprite.Width));	// qty of initial transparent rows
								tally     = (byte)(tally % Sprite.Width);	// qty of transparent pixels starting on the next row
							}

							while (tally >= Byte.MaxValue)
							{
								tally -= Byte.MaxValue;

								binlist.Add(MarkerRle);
								binlist.Add(Byte.MaxValue);
							}

							if (tally != 0)
							{
								if (tally == 1)
								{
									binlist.Add((byte)0);
								}
								else
								{
									binlist.Add(MarkerRle);
									binlist.Add((byte)tally);
								}
							}
							tally = 0;
						}
						else if (start)
						{
							start = false;
							binlist.Add((byte)0);	// always add count of transparent rows at start
						}							// even if the first pixel is not transparent

						binlist.Add(b);
					}
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

		// NOTE: I'd just like to point out that a Tabfile is redundant/ if the
		// Pckfile is wellformed. But if the Pckfile is *not* wellformed you
		// might as well be screwing the dog anyway.
		//
		// At least under my conception of PCK-RLE-compression. Obviously there
		// are others. Which is what makes PCKs so lovely to work with, although
		// the RLE-compression itself is rather excellent.
		//
		// You just don't need Tabfiles if you follow a few rules ....
		// a) a sprite is a minimum of 2 bytes
		// b) the first byte in a sprite shall not be FE or FF (safety - in
		//    practice keep the value of the first byte <= sprite_height)
		// c) the first byte shall always be a quantity of transparent fullrows
		// d) the final byte of a sprite shall be FF
		// e) FE shall be followed by a byte that is a quantity of transparent
		//    pixels (FF is allowed as a quantity)
		// f) do not allow FE or FF to be used as a palette-color
		// g) a decoding program shall initialize the entire buffer of a sprite
		//    with transparent pixels first. Hence a contiguous sequence of
		//    transparent pixels that hit the final FF do not need to be written.
		//
		// I've seen a fair few whacky PCK files, including ones with
		// 'FE 01' <- "make the next pixel transparent". Hint, that's the
		// equivalent of simply '00'. Another common occurance is to write a
		// blank sprite as 'FF' only; while this could be valid if it were
		// policy (and ofc not preceeded by FE), decoding problems can occur
		// depending on the decoding application.
		//
		// protip: a Minimal (blank) sprite would/could/should be '00 FF'.
		// - 0 blank initial rows
		// - End_of_Sprite marker
		// - let the decoding algo fill the sprite with palette-id #0 as default


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

			sprite._spriteset = spriteset;

			sprite.Id = id;
//			sprite.Ordinal = -1; // not used.

			sprite._bindata = _bindata.Clone() as byte[];

			sprite.Sprite      = ObjectCopier.Clone<Bitmap>(Sprite);		// workaround for Bitmap's clone/copy/new shenanigans
			sprite.SpriteToned = ObjectCopier.Clone<Bitmap>(SpriteToned);	// workaround for Bitmap's clone/copy/new shenanigans
			sprite.Pal         = Pal;

			return sprite;
		}
		#endregion Methods
	}
}
