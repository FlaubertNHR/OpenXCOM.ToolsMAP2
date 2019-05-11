using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	/// <summary>
	/// a SPRITESET: A collection of images that is usually created of PCK/TAB
	/// terrain file data but can also be a ScanG iconset.
	/// </summary>
	public sealed class SpriteCollection
	{
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
		/// Flag used to differentiate between 2-byte and 4-byte tab-files.
		/// TODO: is kludge = TRUE.
		/// </summary>
		public bool Borked
		{ get; private set; }

		/// <summary>
		/// Flag used to indicate a mismatch between the size of bindata and
		/// Bindata when attempting to add a PckImage.
		/// TODO: is kludge = TRUE.
		/// </summary>
		public bool BorkedBigobs
		{ get; internal set; }


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
				return (id > -1 && id < Count) ? Sprites[id]
											   : null;
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


		#region cTors
		/// <summary>
		/// cTor[1]. Creates a quick and dirty blank spriteset.
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

			Borked =
			BorkedBigobs = false;
		}

		/// <summary>
		/// cTor[2]. Parses a PCK-file into a collection of images according to
		/// its TAB-file.
		/// NOTE: a spriteset is loaded by:
		/// 1.
		/// XCMainWindow.LoadSelectedDescriptor() calls
		/// MapFileService.LoadDescriptor() calls
		/// Descriptor.GetTerrainRecords() calls
		/// Descriptor.GetTerrainSpriteset() calls
		/// ResourceInfo.LoadSpriteset() calls
		/// SpriteCollection..cTor.
		/// 2.
		/// PckViewForm.LoadSpriteset()
		/// 3.
		/// Also instantiated by Globals.LoadExtraSprites()
		/// 4.
		/// XCMainWindow..cTor also needs to load the CURSOR.
		/// </summary>
		/// <param name="fsPck">filestream of the PCK file</param>
		/// <param name="fsTab">filestream of the TAB file</param>
		/// <param name="tabwordLength">2 for terrains/bigobs/ufo-units, 4 for tftd-units</param>
		/// <param name="pal">the palette to use (typically Palette.UfoBattle
		/// for UFO sprites or Palette.TftdBattle for TFTD sprites)</param>
		/// <param name="label">file w/out extension or path</param>
		public SpriteCollection(
				Stream fsPck,
				Stream fsTab,
				int tabwordLength,
				Palette pal,
				string label)
		{
			//LogFile.WriteLine("SpriteCollection..cTor[2]");

			TabwordLength = tabwordLength;
			Pal           = pal;
			Label         = label;

			Borked =
			BorkedBigobs = false;

			int tabSprites = 0;
			uint[] offsets;

			if (fsTab != null)
			{
				tabSprites = (int)fsTab.Length / tabwordLength;
				//LogFile.WriteLine(". fsTab.Length= " + fsTab.Length);
				//LogFile.WriteLine(". tabwordLength= " + tabwordLength);
				//LogFile.WriteLine(". tabSprites= " + tabSprites);

				fsTab.Position = 0;

				offsets = new uint[tabSprites + 1]; // NOTE: the last entry will be set to the total length of the input-bindata.
				using (var br = new BinaryReader(fsTab))
				{
					switch (tabwordLength)
					{
						case ResourceInfo.TAB_WORD_LENGTH_2:
							for (int i = 0; i != tabSprites; ++i)
								offsets[i] = br.ReadUInt16();
							break;

						case ResourceInfo.TAB_WORD_LENGTH_4:
							for (int i = 0; i != tabSprites; ++i)
								offsets[i] = br.ReadUInt32();
							break;
					}
				}
			}
			else
			{
				offsets = new uint[2];
				offsets[0] = 0;
			}


			fsPck.Position = 0;

			var bindata = new byte[(int)fsPck.Length];
			fsPck.Read(
					bindata,			// buffer
					0,					// offset
					bindata.Length);	// count


			if (bindata.Length > 1)
			{
				if (fsTab != null)
				{
					int pckSprites = 0; // qty of bytes in 'bindata' w/ value 0xFF (ie. qty of sprites)
					for (int i = 1; i != bindata.Length; ++i)
					{
						if (bindata[i] == 255 && bindata[i - 1] != 254)
							++pckSprites;
					}
					Borked = (pckSprites != tabSprites);
					//LogFile.WriteLine("pckSprites= " + pckSprites + " tabSprites= " + tabSprites);
				}

				if (!Borked) // avoid throwing 1 or 15000 exceptions ...
				{
					offsets[offsets.Length - 1] = (uint)bindata.Length;
					//LogFile.WriteLine("");
					//LogFile.WriteLine(". offsets.Length= " + offsets.Length);

					for (int i = 0; i != offsets.Length - 1; ++i)
					{
						//LogFile.WriteLine(". . sprite #" + i);
						//LogFile.WriteLine(". . offsets[i]=\t\t" + (offsets[i]));
						//LogFile.WriteLine(". . offsets[i+1]=\t" + (offsets[i + 1]));
						//LogFile.WriteLine(". . . val=\t\t\t"    + (offsets[i + 1] - offsets[i]));
						var bindataSprite = new byte[offsets[i + 1] - offsets[i]];

						for (int j = 0; j != bindataSprite.Length; ++j)
							bindataSprite[j] = bindata[offsets[i] + j];

						var sprite = new PckImage(
												bindataSprite,
												Pal,
												i,
												this);
						if (!BorkedBigobs)
						{
							Sprites.Add(sprite);
						}
						else
						{
							Sprites.Clear();
							break;
						}
					}
				}
				// else abort. NOTE: 'Borked' is evaluated on return to PckViewForm.LoadSpriteset()
				// ... but the GetBorked() algorithm is pertinent (and could
				// additionally bork things) whenever any spriteset loads.
			}

			//LogFile.WriteLine(". spritecount= " + Count);
		}

		/// <summary>
		/// cTor[3]. Creates a spriteset of ScanG icons.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="fsScanG">filestream of the SCANG.DAT file</param>
		public SpriteCollection(string label, Stream fsScanG)
		{
			Label = label;

			TabwordLength = ResourceInfo.TAB_WORD_LENGTH_0;
			Pal = null;

			Borked       =
			BorkedBigobs = false;

			fsScanG.Position = 0;

			var bindata = new byte[(int)fsScanG.Length];
			fsScanG.Read(
						bindata,			// buffer
						0,					// offset
						bindata.Length);	// count

			// chop bindata into 16-byte icons (4x4 256-color indexed)
			int iconCount = bindata.Length / 16;

			// TODO: Test that ScanG.Dat is not corrupt (ie. is evenly divisible by 16).

			for (int id = 0; id != iconCount; ++id)
			{
				var icondata = new byte[16];

				for (int i = 0; i != 16; ++i)
					icondata[i] = bindata[id * 16 + i];

				Sprites.Add(new ScanGicon(icondata, id));
			}
		}
		#endregion cTors


		#region Methods Methods (static)
		/// <summary>
		/// Saves a specified spriteset to PCK+TAB.
		/// </summary>
		/// <param name="dir">the directory to save to</param>
		/// <param name="file">the file without extension</param>
		/// <param name="spriteset">pointer to the spriteset</param>
		/// <param name="tabwordLength">2 for terrains/bigobs/ufo-units, 4 for tftd-units</param>
		/// <returns>true if mission was successful</returns>
		public static bool SaveSpriteset(
				string dir,
				string file,
				SpriteCollection spriteset,
				int tabwordLength)
		{
			//LogFile.WriteLine("SpriteCollection.SaveSpriteset");

			string pfePck = Path.Combine(dir, file + GlobalsXC.PckExt);
			string pfeTab = Path.Combine(dir, file + GlobalsXC.TabExt);

			using (var bwPck = new BinaryWriter(File.Create(pfePck)))
			using (var bwTab = new BinaryWriter(File.Create(pfeTab)))
			{
				switch (tabwordLength)
				{
					case ResourceInfo.TAB_WORD_LENGTH_2:
					{
						uint pos = 0;
						for (int id = 0; id != spriteset.Count; ++id)
						{
							//LogFile.WriteLine(". pos[pre]= " + pos);
							if (pos > UInt16.MaxValue) // bork. Psst, happens at ~150 sprites.
							{
								//LogFile.WriteLine(". . UInt16 MaxValue exceeded - ret FALSE");
								return false;
							}

							bwTab.Write((ushort)pos); // TODO: investigate le/be
							pos += PckImage.SaveSpritesetSprite(bwPck, spriteset[id]);
							//LogFile.WriteLine(". pos[pst]= " + pos);
						}
						break;
					}

					case ResourceInfo.TAB_WORD_LENGTH_4:
					{
						uint pos = 0;
						for (int id = 0; id != spriteset.Count; ++id)
						{
							bwTab.Write(pos); // TODO: investigate le/be
							pos += PckImage.SaveSpritesetSprite(bwPck, spriteset[id]);
						}
						break;
					}
				}
			}
			return true;
		}

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
//					result = "Sprite offset is invalid.";
					result = "Only " + id + " of " + spriteset.Count
						   + " sprites can be indexed in the TAB file."
						   + Environment.NewLine
						   + "Failed at position " + pos;
					return false;
				}
				pos += PckImage.TestSprite(spriteset[id]);
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
		/// <param name="after">ref for the TabOffset of the next sprite</param>
		/// <param name="spriteId">default -1 to test the final sprite in the set</param>
		public static void Test2byteSpriteset(
				SpriteCollection spriteset,
				out uint last,
				out uint after,
				int spriteId = -1)
		{
			if (spriteId == -1)
				spriteId = spriteset.Count - 1;

			last = after = 0;

			uint len;
			for (int id = 0; id <= spriteId; ++id)
			{
				len = PckImage.TestSprite(spriteset[id]);
				if (id != spriteId)
					last += len;
				else
					after = last + len;
			}
		}
/*		/// <summary>
		/// Deters the last sprite's 2-byte TabIndex for a specified spriteset
		/// as well as the TabIndex for the next sprite if it were added.
		/// </summary>
		/// <param name="spriteset">the SpriteCollection to test</param>
		/// <param name="last"></param>
		/// <param name="after"></param>
		public static void Test2byteSpriteset(
				SpriteCollection spriteset,
				out uint last,
				out uint after)
		{
			last = after = 0;

			uint len;
			for (int id = 0; id != spriteset.Count; ++id)
			{
				len = PckImage.TestSprite(spriteset[id]);
				if (id != spriteset.Count - 1)
					last += len;
				else
					after = last + len;
			}
		} */
/*		/// <summary>
		/// Gets the next 2-byte TabIndex for a specified spriteset.
		/// </summary>
		/// <param name="spriteset">the SpriteCollection to test</param>
		/// <returns></returns>
		public static uint Test2byteSpriteset(SpriteCollection spriteset)
		{
			uint pos = 0;
			for (int id = 0; id != spriteset.Count; ++id)
				pos += PckImage.TestSprite(spriteset[id]);

			return pos;
		} */
/*		/// <summary>
		/// Gets the lastknowngood 2-byte TabIndex for a specified spriteset.
		/// </summary>
		/// <param name="spriteset">the SpriteCollection to test</param>
		/// <returns></returns>
		public static uint Test2byteSpriteset(SpriteCollection spriteset)
		{
			uint pos = 0, postest = 0;
			for (int id = 0; id != spriteset.Count; ++id)
			{
				if (postest += PckImage.TestSprite(spriteset[id]) > UInt16.MaxValue)
					return pos;

				pos = postest;
			}
			return 0;
		} */

		/// <summary>
		/// Saves a specified iconset to SCANG.DAT.
		/// </summary>
		/// <param name="dir">the directory to save to</param>
		/// <param name="file">the file without extension</param>
		/// <param name="iconset">pointer to the iconset</param>
		/// <returns>true if mission was successful</returns>
		public static bool SaveScanG(
				string dir,
				string file,
				SpriteCollection iconset)
		{
			string pfeScanG = Path.Combine(dir, file + GlobalsXC.DatExt);

			try
			{
				using (var bwDat = new BinaryWriter(File.Create(pfeScanG)))
				{
					XCImage icon;
					for (int id = 0; id != iconset.Count; ++id)
					{
						icon = iconset[id];
						for (int i = 0; i != icon.Bindata.Length; ++i)
						{
							bwDat.Write(icon.Bindata[i]);
						}
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion Methods (static)
	}
}

//		private int _scale = 1;
//		public void HQ2X()
//		{
//			foreach (XCImage image in this)
//				image.HQ2X();
//			_scale *= 2;
//		}
