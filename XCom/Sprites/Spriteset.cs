using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	/// <summary>
	/// a SPRITESET. A collection of images that is usually created of PCK/TAB
	/// terrain file data but can also be bigobs or a ScanG iconset.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable crap</c>.</remarks>
	public sealed class Spriteset
	{
		#region Methods (disposable)
		/// <summary>
		/// Disposes all <c><see cref="XCImage">XCImages</see></c> in
		/// <c><see cref="Sprites"/></c> and clears the list.
		/// </summary>
		/// <remarks>This <c>Spriteset</c> itself remains valid along with the
		/// cleared <c>Sprites</c> list.</remarks>
		public void Dispose()
		{
			Logfile.Log("Spriteset.Dispose() Label= " + Label);
			foreach (var sprite in Sprites)
				sprite.Dispose();

			Sprites.Clear();
		}
		#endregion Methods (disposable)


		#region Fields (static)
		public const int FAIL_non            = 0x0; // bitflags for Fail states ->
		public const int FAIL_OF_SPRITE      = 0x1; // overflow
		public const int FAIL_OF_OFFSET      = 0x2; // overflow
		public const int FAIL_COUNT_MISMATCH = 0x4; // Pck vs Tab counts-mismatch error
		#endregion Fields (static)


		#region Properties
		private readonly IList<XCImage> _sprites = new List<XCImage>();
		public IList<XCImage> Sprites
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
		/// A bit-flagged <c>int</c> containing <c>Fail</c> states -
		/// <c><see cref="FAIL_non"/></c> on a successful load.
		/// </summary>
		/// <remarks>The caller shall set this <c>Spriteset</c> to <c>null</c>
		/// if any bits are flagged. Only <c><see cref="FAIL_OF_SPRITE"/></c>
		/// needs to call <c><see cref="Dispose()">Dispose()</see></c>.</remarks>
		public int Fail
		{ get; internal set; }

		/// <summary>
		/// Count of sprites detected in a Pckfile. Is used only if this
		/// <c>Spriteset</c> fails to load due to a PCK/TAB mismatch error. It's
		/// printed in the errorbox as an aid for debugging.
		/// </summary>
		public int CountSprites
		{ get; private set; }

		/// <summary>
		/// Count of offsets detected in a Tabfile. Is used only if this
		/// <c>Spriteset</c> fails to load due to a PCK/TAB mismatch error. It's
		/// printed in the errorbox as an aid for debugging.
		/// </summary>
		public int CountOffsets
		{ get; private set; }


		private Palette _pal;
		/// <summary>
		/// This <c>Spriteset's</c> reference to a <c><see cref="Palette"/></c>.
		/// </summary>
		/// <remarks>Changing the palette requires re-assigning the changed
		/// <c><see cref="System.Drawing.Imaging.ColorPalette"/></c> to all
		/// sprites in this <c>Spriteset</c>.</remarks>
		public Palette Pal
		{
			get { return _pal; }
			set
			{
				_pal = value;

				foreach (XCImage sprite in Sprites)
					sprite.Pal = Pal;

				// rant
				// why is the dang palette in every god-dang XCImage.
				// why is 'Palette' EVERYWHERE: For indexed images
				// the palette ought be merely a peripheral.
			}
		}

		/// <summary>
		/// Gets/Sets the <c><see cref="XCImage"/></c> at a specified id in
		/// <c><see cref="Sprites"/></c>. Adds a sprite to the end of the set
		/// if the specified id falls outside the bounds of the list.
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
		/// cTor[0]. Creates a quick and dirty blank <c>Spriteset</c>.
		/// </summary>
		/// <param name="label">file w/out path or extension</param>
		/// <param name="pal">the <c><see cref="Palette"/></c> to use (typically
		/// <c><see cref="Palette.UfoBattle">Palette.UfoBattle</see></c> for
		/// UFO-sprites or <c><see cref="Palette.TftdBattle">Palette.TftdBattle</see></c>
		/// for TFTD-sprites)</param>
		/// <param name="tabwordLength"><c><see cref="SpritesetManager.TAB_WORD_LENGTH_2">SpritesetManager.TAB_WORD_LENGTH_2</see></c>
		/// for terrains/bigobs/ufo-units, <c><see cref="SpritesetManager.TAB_WORD_LENGTH_4">SpritesetManager.TAB_WORD_LENGTH_4</see></c>
		/// for tftd-units</param>
		/// <remarks>A spriteset is created by
		/// <list type="number">
		/// <item><c>PckView.PckViewF.OnCreateClick()</c></item>
		/// <item><c>McdView.TerrainPanel_main.addPart()</c></item>
		/// </list></remarks>
		public Spriteset(
				string label,
				Palette pal,
				int tabwordLength = SpritesetManager.TAB_WORD_LENGTH_2)
		{
			Label         = label;
			Pal           = pal;
			TabwordLength = tabwordLength;
		}

		/// <summary>
		/// cTor[1]. Parses a PCK-file into a collection of sprites according to
		/// its TAB-file.
		/// </summary>
		/// <param name="label">usually the file w/out path or extension but
		/// it's arbitrary here</param>
		/// <param name="pal">the <c><see cref="Palette"/></c> to use (typically
		/// <c><see cref="Palette.UfoBattle">Palette.UfoBattle</see></c> for
		/// UFO-sprites or <c><see cref="Palette.TftdBattle">Palette.TftdBattle</see></c>
		/// for TFTD-sprites)</param>
		/// <param name="tabwordLength"><c><see cref="SpritesetManager.TAB_WORD_LENGTH_2">SpritesetManager.TAB_WORD_LENGTH_2</see></c>
		/// for terrains/bigobs/ufo-units, <c><see cref="SpritesetManager.TAB_WORD_LENGTH_4">SpritesetManager.TAB_WORD_LENGTH_4</see></c>
		/// for tftd-units</param>
		/// <param name="bytesPck">byte array of the PCK file</param>
		/// <param name="bytesTab">byte array of the TAB file</param>
		/// <param name="createToned"><c>true</c> to create
		/// <c><see cref="PckSprite.SpriteToned">PckSprite.SpriteToned</see></c>
		/// sprites for MapView</param>
		/// <returns>a <c>Spriteset</c> containing all the sprites, or null if
		/// the quantity of sprites in the PCK vs TAB files aren't equal</returns>
		/// <remarks>Ensure that <paramref name="bytesPck"/> and <paramref name="bytesTab"/>
		/// are valid before call.
		/// 
		/// 
		/// A spriteset is loaded by
		/// <list type="number">
		/// <item><c>MapView.MainViewF()</c>
		/// 
		/// > <c><see cref="EmbeddedService.CreateMonotoneSpriteset(string)">EmbeddedService.CreateMonotoneSpriteset(string)</see></c> partype icons</item>
		/// <item><c>MapView.MainViewF()</c>
		/// 
		/// > <c><see cref="SpritesetManager.CreateSpriteset(string, string, int, Palette, bool)">SpritesetManager.CreateSpriteset()</see></c> ufo-cursor
		/// 
		/// > <c><see cref="SpritesetManager.CreateSpriteset(string, string, int, Palette, bool)">SpritesetManager.CreateSpriteset()</see></c> tftd-cursor</item>
		/// <item><c>MapView.MainViewF.LoadSelectedDescriptor()</c>
		/// 
		/// > <c><see cref="MapFileService.LoadDescriptor(Descriptor, ref bool, bool, bool, RouteNodes)">MapFileService.LoadDescriptor()</see></c>
		/// 
		/// > <c><see cref="Descriptor.CreateTerrain(int)">Descriptor.CreateTerrain()</see></c>
		/// 
		/// > <c><see cref="SpritesetManager.CreateSpriteset(string, string, int, Palette, bool)">SpritesetManager.CreateSpriteset()</see></c> tilepart sprites</item>
		/// <item><c>MapView.MainViewF.LoadSelectedDescriptor()</c>
		/// 
		/// > <c><see cref="MapFileService.LoadDescriptor(Descriptor, ref bool, bool, bool, RouteNodes)">MapFileService.LoadDescriptor()</see></c>
		/// 
		/// > <c><see cref="MapFile(Descriptor, IList, RouteNodes)">MapFile.MapFile()</see></c>
		/// 
		/// > <c>MapFile.LoadMapfile()</c>
		/// 
		/// > <c>MapFile.CreateTile()</c>
		/// 
		/// > <c><see cref="Tilepart.Cripple(PartType)">Tilepart.Cripple()</see></c>
		/// 
		/// > <c>Tilepart.CreateCrippledSprites()</c>
		/// 
		/// > <c><see cref="EmbeddedService.CreateMonotoneSpriteset(string)">EmbeddedService.CreateMonotoneSpriteset(string)</see></c> crippled sprites</item>
		/// <item><c>PckView.PckViewF.LoadSpriteset(string, bool)</c></item>
		/// <item><c>McdView.McdViewF.OnClick_Create()</c></item>
		/// <item><c>McdView.McdViewF.LoadTerrain()</c></item>
		/// <item><c>McdView.McdViewF.OnClick_Reload()</c></item>
		/// <item><c>McdView.McdViewF.LoadRecords()</c></item>
		/// <item><c>McdView.McdViewF.OpenCopier()</c></item>
		/// </list></remarks>
		public Spriteset(
				string label,
				Palette pal,
				int tabwordLength,
				byte[] bytesPck,
				byte[] bytesTab,
				bool createToned = false)
		{
			//Logfile.Log("Spriteset label= " + label + " pal= " + pal + " tabwordLength= " + tabwordLength);

			Pal           = pal;
			TabwordLength = tabwordLength;

			if (label == SharedSpace.CursorFilePrefix)
			{
				if (TabwordLength == SpritesetManager.TAB_WORD_LENGTH_4) // are you sure ...
					Label = "TargeterTftd";
				else
					Label = "TargeterUfo";
			}
			else
				Label = label;


			bool le = BitConverter.IsLittleEndian; // computer architecture

			CountOffsets = (int)bytesTab.Length / TabwordLength;
			var offsets = new uint[CountOffsets + 1];	// NOTE: the last entry will be set to the total length of
														// the input-bindata to deter the length of the final sprite.
			var buffer = new byte[TabwordLength];
			uint b;
			int pos = 0;

			if (TabwordLength == SpritesetManager.TAB_WORD_LENGTH_4)
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
			else // (TabwordLength == SpritesetManager.TAB_WORD_LENGTH_2)
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
				// - 0 blank initial rows
				// - End_of_Sprite marker
				// - let the decoding algo fill the sprite with palette-id #0 as default

				if (CountSprites == CountOffsets) // avoid throwing 1 or 15000 exceptions ...
				{
					offsets[offsets.Length - 1] = (uint)bytesPck.Length;

					for (int i = 0; i != offsets.Length - 1; ++i)
					{
						var bindata = new byte[offsets[i + 1] - offsets[i]];

						for (int j = 0; j != bindata.Length; ++j)
							bindata[j] = bytesPck[offsets[i] + j];

						//Logfile.Log("sprite #" + i);
						var sprite = new PckSprite(
												bindata,
												Pal,
												i,
												this,
												createToned);

						if ((Fail & FAIL_OF_SPRITE) != FAIL_non)
						{
							Dispose();
							return;
						}
						Sprites.Add(sprite);
					}
				}
				else
				{
					Fail |= FAIL_COUNT_MISMATCH;

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
		/// cTor[2]. Creates a <c>Spriteset</c> of ScanG or LoFT icons.
		/// ScanG chops bindata into 16-byte icons (4x4 256-color indexed).
		/// LoFT chops bindata into 256-bit icons (16x16 2-color true/false bits).
		/// </summary>
		/// <param name="label"></param>
		/// <param name="fs">a <c>Stream</c> of the SCANG.DAT or LOFTEMPS.DAT
		/// file</param>
		/// <param name="isLoFT"><c>true</c> if LoFT data, <c>false</c> if ScanG</param>
		/// <remarks>cf
		/// <list type="bullet">
		/// <item><c><see cref="SpritesetManager.LoadScanGufo()">SpritesetManager.LoadScanGufo()</see></c></item>
		/// <item><c><see cref="SpritesetManager.LoadScanGtftd()">SpritesetManager.LoadScanGtftd()</see></c></item>
		/// <item><c><see cref="SpritesetManager.LoadLoFTufo()">SpritesetManager.LoadLoFTufo()</see></c></item>
		/// <item><c><see cref="SpritesetManager.LoadLoFTtftd()">SpritesetManager.LoadLoFTtftd()</see></c></item>
		/// <item><c>McdviewF.LoadScanGufo()</c></item>
		/// <item><c>McdviewF.LoadScanGtftd()</c></item>
		/// <item><c>McdviewF.LoadLoFTufo()</c></item>
		/// <item><c>McdviewF.LoadLoFTtftd()</c></item>
		/// </list></remarks>
		public Spriteset(string label, Stream fs, bool isLoFT)
		{
			Label         = label;
			TabwordLength = SpritesetManager.TAB_WORD_LENGTH_0;

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
				Pal = Palette.UfoBattle; // <- default
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
				Pal = Palette.Binary;
			}
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Saves a specified <c><see cref="Spriteset"/></c> to PCK+TAB.
		/// </summary>
		/// <param name="pf">the directory to save to</param>
		/// <param name="spriteset">pointer to the <c>Spriteset</c></param>
		/// <returns><c>true</c> if mission was successful; <c>false</c> if a
		/// 2-byte tabword-length offset exceeds <c>UInt16.MaxValue</c>. Also
		/// <c>false</c> if the Pck or Tab file could not be created.</returns>
		public static bool WriteSpriteset(
				string pf,
				Spriteset spriteset)
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
						case SpritesetManager.TAB_WORD_LENGTH_2:
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

						case SpritesetManager.TAB_WORD_LENGTH_4:
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
		/// <param name="spriteset">the Spriteset to test</param>
		/// <param name="result">a ref to hold the result as a string</param>
		/// <returns>true if mission was successful</returns>
		public static bool TestTabOffsets(
				Spriteset spriteset,
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
		/// Deters a specified sprite-id's 2-byte TabOffset for a specified
		/// spriteset as well as the TabOffset of the next sprite.
		/// </summary>
		/// <param name="spriteset">the Spriteset to test</param>
		/// <param name="last">ref for the TabOffset of 'spriteId'</param>
		/// <param name="aftr">ref for the TabOffset of the next sprite</param>
		/// <param name="id">default -1 to test the final sprite in the set</param>
		/// <remarks>Ensure that 'id' is less than the spriteset count before
		/// call.</remarks>
		public static void TestTabOffsets(
				Spriteset spriteset,
				out uint last,
				out uint aftr,
				int id = -1)
		{
			if (id == -1)
				id = spriteset.Count - 1;

			last = aftr = 0;

			uint len;
			for (int i = 0; i <= id; ++i)
			{
				len = PckSprite.Write(spriteset[i]); // test only.
				if (i != id)
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
				Spriteset iconset)
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
					fs.Write(icon.GetBindata(), 0, icon.GetBindata().Length);
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
				Spriteset iconset)
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

					for (int i = 0; i != icon.GetBindata().Length; i += 16)
					{
						// Look don't ask it appears to work ...

						b = 0;
						bits = new BitArray(8);
						for (int j = 15; j != 7; --j, ++b)
						{
							if (icon.GetBindata()[i + j] != 0)
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
							if (icon.GetBindata()[i + j] != 0)
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
		/// Returns 'Label'.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion Methods (override)
	}
}
