using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	/// <summary>
	/// a SPRITESET: A collection of images that is usually created of PCK/TAB
	/// terrain file data but can also be bigobs or a ScanG iconset.
	/// </summary>
	public sealed class SpriteCollection
	{
		#region Fields (static)
		public const int FAIL_non            = 0x0; // bitflags for Fail states ->
		public const int FAIL_OF_SPRITE      = 0x1; // overflow
		public const int FAIL_OF_OFFSET      = 0x2; // overflow
		public const int FAIL_COUNT_MISMATCH = 0x4; // Pck vs Tab counts mismatch error
		#endregion Fields (static)


		#region Properties
		private List<XCImage> _sprites = new List<XCImage>();
		public List<XCImage> Sprites
		{
			get { return _sprites; }
		}

		public int Count
		{
			get { return Sprites.Count; }
		}

		public string Label
		{ get; private set; }

		public int TabwordLength
		{ get; private set; }


		/// <summary>
		/// A bitflagged int containing Fail states - "0" on a successful load.
		/// @note The caller shall set this spriteset to null if any bits are
		/// flagged.
		/// </summary>
		public int Fail
		{ get; internal set; }

		/// <summary>
		/// Count of sprites detected in a Pckfile. Is used only if the
		/// spriteset fails to load due to a PCK/TAB mismatch error. It's
		/// printed in the errorbox as an aid for debugging.
		/// </summary>
		public int CountSprites
		{ get; private set; }

		/// <summary>
		/// Count of offsets detected in a Tabfile. Is used only if the
		/// spriteset fails to load due to a PCK/TAB mismatch error. It's
		/// printed in the errorbox as an aid for debugging.
		/// </summary>
		public int CountOffsets
		{ get; private set; }


		private Palette _pal;
		/// <summary>
		/// This SpriteCollection's reference to a <see cref="Palette"/>.
		/// @note Changing the palette requires re-assigning the changed
		/// <see cref="System.Drawing.Imaging.ColorPalette"/>.
		/// to all sprites in this spriteset.
		/// </summary>
		public Palette Pal
		{
			get { return _pal; }
			set
			{
				_pal = value;

				foreach (XCImage sprite in Sprites)
					sprite.Sprite.Palette = _pal.Table;

				// rant
				// why is the dang palette in every god-dang XCImage.
				// why is 'Palette' EVERYWHERE: For indexed images
				// the palette ought be merely a peripheral.
			}
		}

		/// <summary>
		/// Gets/sets the 'XCImage' at a specified id. Adds a sprite to the end
		/// of the set if the specified id falls outside the bounds of the List.
		/// </summary>
		public XCImage this[int id]
		{
			get
			{
				if (id > -1 && id < Count)
					return Sprites[id];

				return null;
			}
			set
			{
				if (id > -1 && id < Count)
					Sprites[id] = value;
				else
				{
					value.Id = Count;
					Sprites.Add(value);
				}
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates a quick and dirty blank spriteset.
		/// </summary>
		/// <param name="label">file w/out path or extension</param>
		/// <param name="pal"></param>
		/// <param name="tabwordLength"></param>
		public SpriteCollection(
				string label,
				Palette pal,
				int tabwordLength = SpritesetsManager.TAB_WORD_LENGTH_2)
		{
			Label         = label;
			Pal           = pal;
			TabwordLength = tabwordLength;
		}

		/// <summary>
		/// cTor[1]. Parses a PCK-file into a collection of images according to
		/// its TAB-file.
		/// @note Ensure that 'bytesPck' and 'bytesTab' are valid before call.
		/// NOTE: a spriteset is loaded by:
		/// 1.
		/// MainViewF.LoadSelectedDescriptor()
		/// calls MapFileService.LoadDescriptor()
		/// calls Descriptor.GetTerrainRecords()
		/// calls Descriptor.GetTerrainSpriteset()
		/// calls SpritesetsManager.LoadSpriteset()
		/// calls SpriteCollection..cTor.
		/// 2.
		/// PckViewF.LoadSpriteset()
		/// 3.
		/// Also instantiated by Globals.LoadExtraSprites()
		/// 4.
		/// MainViewF..cTor also needs to load the CURSOR.
		/// </summary>
		/// <param name="label">file w/out extension or path</param>
		/// <param name="pal">the palette to use (typically Palette.UfoBattle
		/// for UFO sprites or Palette.TftdBattle for TFTD sprites)</param>
		/// <param name="tabwordLength">2 for terrains/bigobs/ufo-units, 4 for tftd-units</param>
		/// <param name="bytesPck">byte array of the PCK file</param>
		/// <param name="bytesTab">byte array of the TAB file</param>
		/// <param name="bypassTonescales">true to not create Tonescaled sprites</param>
		public SpriteCollection(
				string label,
				Palette pal,
				int tabwordLength,
				byte[] bytesPck,
				byte[] bytesTab,
				bool bypassTonescales = false)
		{
			//LogFile.WriteLine("SpriteCollection..cTor label= " + label + " pal= " + pal + " tabwordLength= " + tabwordLength);

			Label         = label;
			Pal           = pal;
			TabwordLength = tabwordLength;


			bool le = BitConverter.IsLittleEndian; // computer architecture

			CountOffsets = (int)bytesTab.Length / TabwordLength;
			var offsets = new uint[CountOffsets + 1];	// NOTE: the last entry will be set to the total length of
														// the input-bindata to deter the length of the final sprite.
			var buffer = new byte[TabwordLength];
			uint b;
			int pos = 0;

			if (TabwordLength == SpritesetsManager.TAB_WORD_LENGTH_4)
			{
				while (pos != bytesTab.Length)
				{
					for (b = 0; b != TabwordLength; ++b)
					{
						if (bytesTab.Length > pos + b)
							buffer[b] = bytesTab[pos + b];
						else
						{
							Fail |= FAIL_OF_OFFSET;
							return;
						}
					}

					if (!le) Array.Reverse(buffer);
					offsets[pos / TabwordLength] = BitConverter.ToUInt32(buffer, 0);

					pos += TabwordLength;
				}
			}
			else //if (TabwordLength == SpritesetsManager.TAB_WORD_LENGTH_2)
			{
				while (pos != bytesTab.Length)
				{
					for (b = 0; b != TabwordLength; ++b)
					{
						if (bytesTab.Length > pos + b)
							buffer[b] = bytesTab[pos + b];
						else
						{
							Fail |= FAIL_OF_OFFSET;
							return;
						}
					}

					if (!le) Array.Reverse(buffer);
					offsets[pos / TabwordLength] = BitConverter.ToUInt16(buffer, 0);

					pos += TabwordLength;
				}
			}


			// TODO: Apparently MCDEdit can and will output a 1-byte Pck sprite
			// w/ only "255" if a blank sprite is in its internal spriteset when
			// a save happens.
//			if (bytesPck.Length == 1)
//			{}
//			else

			if (bytesPck.Length > 1)
			{
				for (int i = 1; i != bytesPck.Length; ++i)
				{
					if (   bytesPck[i]     == PckSprite.MarkerEos
						&& bytesPck[i - 1] != PckSprite.MarkerRle)
					{
						++CountSprites; // qty of bytes in 'bytesPck' w/ value 0xFF (ie. qty of sprites)
					}
				}

				// NOTE: I'd just like to point out that a Tabfile is redundant
				// if the Pckfile is wellformed. But if the Pckfile is *not*
				// wellformed you might as well be screwing the dog anyway.
				//
				// At least under my conception of PCK-RLE-compression.
				// Obviously there are others. Which is what makes PCKs so
				// lovely to work with, although the RLE-compression itself is
				// rather excellent.
				//
				// You just don't need Tabfiles if you follow a few rules ....
				// a) a sprite is a minimum of 2 bytes
				// b) the first byte in a sprite shall not be FE or FF
				// c) the first byte shall always be a quantity of transparent fullrows
				// c) the final byte of a sprite shall be FF
				// d) FE shall be followed by a byte that is a quantity of
				//    transparent pixels (FF is allowed as a quantity)
				// f) do not allow FE or FF to be used as a palette-color
				// e) a decoding program shall initialize the entire buffer of a
				//    sprite with transparent pixels first. Hence a contiguous
				//    sequence of transparent pixels that hit the final FF do
				//    not need to be written.
				//
				// I've seen a fair few whacky PCK files, including ones with
				// "FE 01" <- "make the next pixel transparent". Hint, that's
				// the equivalent of simply "00". Another common occurance is to
				// write a blank sprite as FF only; while this could be valid
				// if it were policy (and ofc not preceeded by FE), decoding
				// problems can occur depending on the decoding application.
				//
				// protip: a Minimal (blank) sprite would/could/should be "00 FF".

				if (CountSprites == CountOffsets) // avoid throwing 1 or 15000 exceptions ...
				{
					offsets[offsets.Length - 1] = (uint)bytesPck.Length;

					for (int i = 0; i != offsets.Length - 1; ++i)
					{
						var bindata = new byte[offsets[i + 1] - offsets[i]];

						for (int j = 0; j != bindata.Length; ++j)
							bindata[j] = bytesPck[offsets[i] + j];

						//LogFile.WriteLine("sprite #" + i);
						var sprite = new PckSprite(
												bindata,
												Pal,
												i,
												this,
												bypassTonescales);

						if ((Fail & FAIL_OF_SPRITE) != FAIL_non)	// NOTE: Instantiating the PckSprite above can set the Fail_Overflo flag
							return;									// which shall be handled by the caller; ie. set the spriteset to null.

						Sprites.Add(sprite);
					}
				}
				else
				{
					Fail |= FAIL_COUNT_MISMATCH; // NOTE: Shall be handled by the caller; ie. set the spriteset to null.

/*					if (true) // rewrite the Tabfile ->
					{
						string dir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
						string pfe = Path.Combine(dir, "tabfile.TAB");

						using (var fsTab = FileService.CreateFile(pfe))
						if (fsTab != null)
						using (var bwTab = new BinaryWriter(fsTab))
						{
							pos = 0;

							uint u = 0;
							for (int id = 0; id != CountSprites; ++id)
							{
								if (u > UInt16.MaxValue) // bork. Psst, happens at ~150 sprites.
								{
									// "The size of the encoded sprite-data has grown too large to
									// be stored correctly in a Tab file. Try deleting sprite(s)
									// or (less effective) using more transparency in the sprites."
									return;
								}

								bwTab.Write((ushort)u);

								while (++pos != bytesPck.Length && bytesPck[pos - 1] != 0xFF) // note does not handle "FE FF"
									++u;

								++u;
							}
						}
					} */
				}
			}
			// else malformed pck file (a proper sprite needs at least 2 bytes:
			// one for the count of transparent lines and another for the EoS
			// marker)
		}

		/// <summary>
		/// cTor[2]. Creates a spriteset of ScanG or LoFT icons.
		/// ScanG chops bindata into 16-byte icons (4x4 256-color indexed).
		/// LoFT chops bindata into 256-bit icons (16x16 2-color true/false bits).
		/// @note Cf
		/// - SpritesetsManager.LoadScanGufo()
		/// - McdviewF.LoadScanGufo()
		/// </summary>
		/// <param name="label"></param>
		/// <param name="fs">filestream of the SCANG.DAT or LOFTEMPS.DAT file</param>
		/// <param name="isLoFT">true if LoFT data, false if ScanG</param>
		public SpriteCollection(string label, Stream fs, bool isLoFT)
		{
			Label         = label;
			Pal           = null;
			TabwordLength = SpritesetsManager.TAB_WORD_LENGTH_0;

			if (!isLoFT) // is ScanG
			{
				var bindata = new byte[(int)fs.Length];
				fs.Read(bindata, 0, bindata.Length);

				int iconCount = bindata.Length / ScanGicon.Length_ScanG;
				for (int i = 0; i != iconCount; ++i)
				{
					var icondata = new byte[ScanGicon.Length_ScanG];

					for (int j = 0; j != ScanGicon.Length_ScanG; ++j)
						icondata[j] = bindata[i * ScanGicon.Length_ScanG + j];

					Sprites.Add(new ScanGicon(icondata, i));
				}
			}
			else // is LoFT
			{
				var bindata = new byte[(int)fs.Length];
				fs.Read(bindata, 0, bindata.Length);

				int posByte = 0;

				int iconCount = bindata.Length / LoFTicon.Length_LoFT;
				for (int id = 0; id != iconCount; ++id)
				{
					// 32 bytes in a loft
					// 256 bits in a loft

					var icondata = new BitArray(LoFTicon.Length_LoFT_bits); // init to Falses

					// read the data as little-endian unsigned shorts - you gotta be kidding
					// who decided to write LoFTemps.dat as SHORTS
					// eg. C0 01 -> 01 C0

					int posBit = -1;
					for (int i = 0; i != LoFTicon.Length_LoFT; i += 2, posByte += 2)
					{
						for (byte j = 0x80; j != 0x00; j >>= 1) // 1000 0000 - iterate over bits in each even byte
						{
							icondata[++posBit] = ((bindata[posByte + 1] & j) != 0);
						}

						for (byte j = 0x80; j != 0x00; j >>= 1) // iterate over bits in each odd byte
						{
							icondata[++posBit] = ((bindata[posByte] & j) != 0);
						}
					}

					// convert and store the data to a byte-array in 'XCImage' ->
					var bytes = new byte[icondata.Length];
					for (int i = 0; i != icondata.Length; ++i)
					{
						if (icondata[i])
							bytes[i] = (byte)1;
						else
							bytes[i] = (byte)0;
					}
					Sprites.Add(new LoFTicon(bytes, id));
				}
			}
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Saves a specified spriteset to PCK+TAB.
		/// </summary>
		/// <param name="pf">the directory to save to</param>
		/// <param name="spriteset">pointer to the spriteset</param>
		/// <returns>true if mission was successful; false if a 2-byte
		/// tabword-length offset exceeds 'UInt16.MaxValue'. Also false if the
		/// Pck or Tab file could not be created.</returns>
		public static bool WriteSpriteset(
				string pf,
				SpriteCollection spriteset)
		{
			string pfePck = pf + GlobalsXC.PckExt;
			string pfeTab = pf + GlobalsXC.TabExt;

			string pfePckT, pfeTabT;
			if (File.Exists(pfePck) || File.Exists(pfeTab))
			{
				pfePckT = pfePck + GlobalsXC.TEMPExt;
				pfeTabT = pfeTab + GlobalsXC.TEMPExt;
			}
			else
			{
				pfePckT = pfePck;
				pfeTabT = pfeTab;
			}


			bool fail = true;
			using (var fsPck = FileService.CreateFile(pfePckT))
			if (fsPck != null)
			using (var fsTab = FileService.CreateFile(pfeTabT))
			if (fsTab != null)
			{
				fail = false;

				using (var bwPck = new BinaryWriter(fsPck))
				using (var bwTab = new BinaryWriter(fsTab))
				{
					switch (spriteset.TabwordLength)
					{
						case SpritesetsManager.TAB_WORD_LENGTH_2:
						{
							uint pos = 0;
							for (int id = 0; id != spriteset.Count; ++id)
							{
								if (pos > UInt16.MaxValue) // bork. Psst, happens at ~150 sprites.
								{
									// "The size of the encoded sprite-data has grown too large to
									// be stored correctly in a Tab file. Try deleting sprite(s)
									// or (less effective) using more transparency in the sprites."
									return false;
								}

								bwTab.Write((ushort)pos);
								pos += PckSprite.Write(spriteset[id], bwPck);
							}
							break;
						}

						case SpritesetsManager.TAB_WORD_LENGTH_4:
						{
							uint pos = 0;
							for (int id = 0; id != spriteset.Count; ++id)
							{
								bwTab.Write(pos);
								pos += PckSprite.Write(spriteset[id], bwPck);
							}
							break;
						}
					}
				}
			}

			if (!fail && pfePckT != pfePck)
			{
				return FileService.ReplaceFile(pfePck)
					&& FileService.ReplaceFile(pfeTab);
			}

			return !fail;
		}


		// NOTE: A possible internal reason that a spriteset is invalid is that
		// if the total length of its compressed PCK-data exceeds 2^16 bits
		// (roughly). That is, the TAB file tracks the offsets and needs to
		// know the total length of the PCK file, but UFO's TAB file stores the
		// offsets in only 2-byte format (2^16 bits) so the arithmetic explodes
		// with an overflow as soon as an offset for one of the sprites becomes
		// too large. (Technically, the total PCK data can exceed 2^16 bits;
		// but the *start offset* for a sprite cannot -- at least that's how it
		// works in MapView I/II. Other apps like XCOM, OpenXcom, MCDEdit will
		// use their own routines.)
		// NOTE: It appears that TFTD's terrain files suffer this limitation
		// also (2-byte TabwordLength).

		/// <summary>
		/// Tests a specified 2-byte TabwordLength spriteset for validity of its
		/// TAB-file.
		/// </summary>
		/// <param name="spriteset">the SpriteCollection to test</param>
		/// <param name="result">a ref to hold the result as a string</param>
		/// <returns>true if mission was successful</returns>
		public static bool Test2byteSpriteset(
				SpriteCollection spriteset,
				out string result)
		{
			uint pos = 0;
			for (int id = 0; id != spriteset.Count; ++id)
			{
				if (pos > UInt16.MaxValue)
				{
					result = "Only " + id + " of " + spriteset.Count
						   + " sprites can be indexed in the TAB file."
						   + Environment.NewLine
						   + "Failed at position " + pos;
					return false;
				}
				pos += PckSprite.Write(spriteset[id]);
			}

			result = "Sprite offsets are valid.";
			return true;
		}

		/// <summary>
		/// Deters a specified spriteId's 2-byte TabIndex for a specified
		/// spriteset as well as the TabIndex for the next sprite.
		/// @note Ensure that 'spriteId' is less than the spriteset count before
		/// call.
		/// </summary>
		/// <param name="spriteset">the SpriteCollection to test</param>
		/// <param name="last">ref for the TabOffset of 'spriteId'</param>
		/// <param name="aftr">ref for the TabOffset of the next sprite</param>
		/// <param name="spriteId">default -1 to test the final sprite in the set</param>
		public static void Test2byteSpriteset(
				SpriteCollection spriteset,
				out uint last,
				out uint aftr,
				int spriteId = -1)
		{
			if (spriteId == -1)
				spriteId = spriteset.Count - 1;

			last = aftr = 0;

			uint len;
			for (int id = 0; id <= spriteId; ++id)
			{
				len = PckSprite.Write(spriteset[id]);
				if (id != spriteId)
					last += len;
				else
					aftr = last + len;
			}
		}


		/// <summary>
		/// Saves a specified iconset to SCANG.DAT.
		/// </summary>
		/// <param name="pfe">the directory to save to</param>
		/// <param name="iconset">pointer to the iconset</param>
		/// <returns>true if mission was successful</returns>
		public static bool WriteScanG(
				string pfe,
				SpriteCollection iconset)
		{
			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;

				XCImage icon;
				for (int id = 0; id != iconset.Count; ++id)
				{
					icon = iconset[id];
					fs.Write(icon.Bindata, 0, icon.Bindata.Length);
				}
			}

			if (!fail && pfeT != pfe)
				return FileService.ReplaceFile(pfe);

			return !fail;
		}

		/// <summary>
		/// Saves a specified iconset to LOFTEMPS.DAT.
		/// </summary>
		/// <param name="pfe">the directory to save to</param>
		/// <param name="iconset">pointer to the iconset</param>
		/// <returns>true if mission was successful</returns>
		public static bool WriteLoFT(
				string pfe,
				SpriteCollection iconset)
		{
			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;

				XCImage icon;
				for (int id = 0; id != iconset.Count; ++id)
				{
					icon = iconset[id];

					var buffer = new byte[1];
					BitArray bits;
					int b;

					for (int i = 0; i != icon.Bindata.Length; i += 16)
					{
						// Look don't ask it appears to work ...

						b = 0;
						bits = new BitArray(8);
						for (int j = 15; j != 7; --j, ++b)
						{
							if (icon.Bindata[i + j] != 0)
								bits[b] = true;
							else
								bits[b] = false;
						}
						bits.CopyTo(buffer, 0);
						fs.Write(buffer, 0, 1);

						b = 0;
						bits = new BitArray(8);
						for (int j = 7; j != -1; --j, ++b)
						{
							if (icon.Bindata[i + j] != 0)
								bits[b] = true;
							else
								bits[b] = false;
						}
						bits.CopyTo(buffer, 0);
						fs.Write(buffer, 0, 1);
					}
				}
			}

			if (!fail && pfeT != pfe)
				return FileService.ReplaceFile(pfe);

			return !fail;
		}
		#endregion Methods (static)


		#region Methods (override)
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion Methods (override)
	}
}
