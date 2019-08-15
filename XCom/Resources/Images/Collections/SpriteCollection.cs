using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	/// <summary>
	/// a SPRITESET: A collection of images that is usually created of PCK/TAB
	/// terrain file data but can also be a ScanG iconset.
	/// </summary>
	public sealed class SpriteCollection
	{
		#region Fields (static)
		public const int Length_ScanG = 16; // each ScanG icon is 16 bytes
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
		{ get; set; }

		public int TabwordLength
		{ get; private set; }

		/// <summary>
		/// Flag to state that the quantity of sprites in a Pck file don't
		/// jive with the quantity of offsets in its Tab file.
		/// </summary>
		public bool Error_PckTabCount
		{ get; private set; }

		/// <summary>
		/// Flag to state that there was a sprite-buffer overflow in a PckImage.
		/// </summary>
		public bool Error_Overflo
		{ get; internal set; }


//		public int CountPckSprites // TODO ->
//		{ get; set; }
//		public int CountTabOffsets
//		{ get; set; }


		private Palette _pal;
		/// <summary>
		/// TODO: SpriteCollection should not have a pointer to the palette; the
		/// palette should be applied only when drawing. Where palette is used
		/// to determine game-type a game-type enum should be implemented and
		/// checked.
		/// </summary>
		public Palette Pal
		{
			get { return _pal; }
			set
			{
				_pal = value;

				foreach (XCImage sprite in Sprites)
					sprite.Sprite.Palette = _pal.ColorTable;	// why is the dang palette in every god-dang XCImage.
			}													// why is 'Palette' EVERYWHERE: For indexed images
		}														// the palette ought be merely a peripheral.

		/// <summary>
		/// Gets/sets the 'XCImage' at a specified id. Adds a sprite to the end
		/// of the set if the specified id falls outside the bounds of the List.
		/// </summary>
		public XCImage this[int id]
		{
			get
			{
				return (id > -1 && id < Count) ? Sprites[id] : null;
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
				int tabwordLength = ResourceInfo.TAB_WORD_LENGTH_2)
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
		/// calls ResourceInfo.LoadSpriteset()
		/// calls SpriteCollection..cTor.
		/// 2.
		/// PckViewForm.LoadSpriteset()
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
		public SpriteCollection(
				string label,
				Palette pal,
				int tabwordLength,
				byte[] bytesPck,
				byte[] bytesTab)
		{
			Label         = label;
			Pal           = pal;
			TabwordLength = tabwordLength;


			bool le = BitConverter.IsLittleEndian; // computer architecture

			int tabSprites = (int)bytesTab.Length / TabwordLength;
			var offsets = new uint[tabSprites + 1];	// NOTE: the last entry will be set to the total length of
													// the input-bindata to deter the length of the final sprite.
			var buffer = new byte[TabwordLength];
			uint b;
			int pos = 0;

			if (TabwordLength == ResourceInfo.TAB_WORD_LENGTH_4)
			{
				while (pos != bytesTab.Length)
				{
					for (b = 0; b != TabwordLength; ++b)
						buffer[b] = bytesTab[pos + b];

					if (!le) Array.Reverse(buffer);
					offsets[pos / TabwordLength] = BitConverter.ToUInt32(buffer, 0);

					pos += TabwordLength;
				}
			}
			else //if (TabwordLength == ResourceInfo.TAB_WORD_LENGTH_2)
			{
				while (pos != bytesTab.Length)
				{
					for (b = 0; b != TabwordLength; ++b)
						buffer[b] = bytesTab[pos + b];

					if (!le) Array.Reverse(buffer);
					offsets[pos / TabwordLength] = BitConverter.ToUInt16(buffer, 0);

					pos += TabwordLength;
				}
			}


			// TODO: Apparently MCDEdit can and will output a 1-byte Pck sprite
			// w/ "255" only if a blank sprite is in its internal spriteset when
			// a save happens.
//			if (bytesPck.Length == 1)
//			{}
//			else

			if (bytesPck.Length > 1)
			{
				int pckSprites = 0; // qty of bytes in 'bytesPck' w/ value 0xFF (ie. qty of sprites)
				for (int i = 1; i != bytesPck.Length; ++i)
				{
					if (   bytesPck[i]     == PckImage.MarkerEos
						&& bytesPck[i - 1] != PckImage.MarkerRle)
					{
						++pckSprites;
					}
				}

				if (pckSprites == tabSprites) // avoid throwing 1 or 15000 exceptions ...
				{
					offsets[offsets.Length - 1] = (uint)bytesPck.Length;

					for (int i = 0; i != offsets.Length - 1; ++i)
					{
						var bindata = new byte[offsets[i + 1] - offsets[i]];

						for (int j = 0; j != bindata.Length; ++j)
							bindata[j] = bytesPck[offsets[i] + j];

						var sprite = new PckImage(			// NOTE: Instantiating a PckImage can set the 'Error_Overflo' flag
												bindata,	// which shall be handled by the caller; ie. set the spriteset to null.
												Pal,
												i,
												this);

						if (Error_Overflo)
							return;

						Sprites.Add(sprite);
					}
				}
				else
					Error_PckTabCount = true; // NOTE: Shall be handled by the caller; ie. set the spriteset to null.
			}
			// else malformed pck file (a proper sprite needs at least 2 bytes:
			// one for the count of transparent lines and another for the EoS
			// marker)
		}

		/// <summary>
		/// cTor[2]. Creates a spriteset of ScanG icons.
		/// Chops bindata into 16-byte icons (4x4 256-color indexed).
		/// </summary>
		/// <param name="label"></param>
		/// <param name="fsScanG">filestream of the SCANG.DAT file</param>
		public SpriteCollection(string label, Stream fsScanG)
		{
			Label         = label;
			Pal           = null;
			TabwordLength = ResourceInfo.TAB_WORD_LENGTH_0;

			var bindata = new byte[(int)fsScanG.Length];
			fsScanG.Read(bindata, 0, bindata.Length);

			int iconCount = bindata.Length / Length_ScanG;
			for (int i = 0; i != iconCount; ++i)
			{
				var icondata = new byte[Length_ScanG];

				for (int j = 0; j != Length_ScanG; ++j)
					icondata[j] = bindata[i * Length_ScanG + j];

				Sprites.Add(new ScanGicon(icondata, i));
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
			string pfePckT = pf + GlobalsXC.PckExt + GlobalsXC.TEMPExt;
			string pfeTabT = pf + GlobalsXC.TabExt + GlobalsXC.TEMPExt;

			using (var fsPck = FileService.CreateFile(pfePckT))
			if (fsPck != null)
			using (var fsTab = FileService.CreateFile(pfeTabT))
			if (fsTab != null)
			{
				using (var bwPck = new BinaryWriter(fsPck))
				using (var bwTab = new BinaryWriter(fsTab))
				{
					switch (spriteset.TabwordLength)
					{
						case ResourceInfo.TAB_WORD_LENGTH_2:
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
								pos += PckImage.Write(spriteset[id], bwPck);
							}
							break;
						}

						case ResourceInfo.TAB_WORD_LENGTH_4:
						{
							uint pos = 0;
							for (int id = 0; id != spriteset.Count; ++id)
							{
								bwTab.Write(pos);
								pos += PckImage.Write(spriteset[id], bwPck);
							}
							break;
						}
					}
				}
				return true;
			}
			return false;
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
				pos += PckImage.Write(spriteset[id]);
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
				len = PckImage.Write(spriteset[id]);
				if (id != spriteId)
					last += len;
				else
					aftr = last + len;
			}
		}


		/// <summary>
		/// Saves a specified iconset to SCANG.DAT.
		/// </summary>
		/// <param name="pfeScanG">the directory to save to</param>
		/// <param name="iconset">pointer to the iconset</param>
		/// <returns>true if mission was successful</returns>
		public static bool WriteScanG(
				string pfeScanG,
				SpriteCollection iconset)
		{
			using (var fs = FileService.CreateFile(pfeScanG + GlobalsXC.TEMPExt))
			{
				if (fs != null)
				{
					XCImage icon;
					for (int id = 0; id != iconset.Count; ++id)
					{
						icon = iconset[id];
						fs.Write(icon.Bindata, 0, icon.Bindata.Length);
					}
					return true;
				}
			}
			return false;
		}
		#endregion Methods (static)
	}
}
