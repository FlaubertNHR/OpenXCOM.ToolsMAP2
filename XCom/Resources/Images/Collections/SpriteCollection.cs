using System;
using System.IO;

using XCom.Interfaces;


namespace XCom
{
	/// <summary>
	/// A spriteset: a collection of images taken from PCK/TAB file data.
	/// </summary>
	public sealed class SpriteCollection
		:
			SpriteCollectionBase
	{
		#region Properties
		public int TabOffset
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
		#endregion


		#region cTors
		/// <summary>
		/// cTor[1]. Creates a quick and dirty blank spriteset for PckView.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="pal"></param>
		/// <param name="tabOffset"></param>
		public SpriteCollection(
				string label,
				Palette pal,
				int tabOffset)
		{
			Label     = label;
			Pal       = pal;
			TabOffset = tabOffset;
		}

		/// <summary>
		/// cTor[2]. Parses a PCK-file into a collection of images according to
		/// its TAB-file.
		/// NOTE: a spriteset is loaded by:
		/// 1.
		/// XCMainWindow.LoadSelectedMap() calls
		/// MapFileService.LoadTileset() calls
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
		/// <param name="tabOffsetLength">the length of a word in bytes of a single
		/// tab-record (ie. 2 for 2-byte UFO/TFTD records, 4 for 4-byte TFTD records)</param>
		/// <param name="pal">the palette to use (typically Palette.UfoBattle
		/// for UFO sprites or Palette.TftdBattle for TFTD sprites)</param>
		public SpriteCollection(
				Stream fsPck,
				Stream fsTab,
				int tabOffsetLength,
				Palette pal)
		{
			//LogFile.WriteLine("SpriteCollection..cTor");
			TabOffset = tabOffsetLength;

			Pal = pal;

			int tabSprites = 0;
			uint[] offsets;

			if (fsTab != null)
			{
				tabSprites = (int)fsTab.Length / tabOffsetLength;
				//LogFile.WriteLine(". fsTab.Length= " + fsTab.Length);
				//LogFile.WriteLine(". tabOffsetLength= " + tabOffsetLength);
				//LogFile.WriteLine(". tabSprites= " + tabSprites);

				fsTab.Position = 0;

				offsets = new uint[tabSprites + 1]; // NOTE: the last entry will be set to the total length of the input-bindata.
				using (var br = new BinaryReader(fsTab))
				{
					switch (tabOffsetLength)
					{
						case 2:
							for (int i = 0; i != tabSprites; ++i)
								offsets[i] = br.ReadUInt16();
							break;

						case 4:
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


			Borked = false;
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
							Add(sprite);
						}
						else
						{
							Clear();
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
		/// <param name="fsScanG">filestream of the PCK file</param>
		public SpriteCollection(Stream fsScanG)
		{
			TabOffset = 0;
			Pal = null;

			Borked       =
			BorkedBigobs = false;

			fsScanG.Position = 0;

			var bindata = new byte[(int)fsScanG.Length];
			fsScanG.Read(
						bindata,			// buffer
						0,					// offset
						bindata.Length);	// count

			// chop bindata into 16-byte icons
			int icons = bindata.Length / 16;

			for (int i = 0; i != icons; ++i)
			{
				var icondata = new byte[16];

				for (int j = 0; j != 16; ++j)
					icondata[j] = bindata[i * 16 + j];

				var icon = new ScanGicon(
									icondata,
									this);
				Add(icon);
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// Saves the current spriteset to PCK+TAB.
		/// </summary>
		/// <param name="dir">the directory to save to</param>
		/// <param name="file">the filename without extension</param>
		/// <param name="spriteset">pointer to the base spriteset</param>
		/// <param name="tabOffsetLength">2 for terrains/bigobs/ufo-units, 4 for tftd-units</param>
		/// <returns>true if mission was successful</returns>
		public static bool SaveSpriteset(
				string dir,
				string file,
				SpriteCollectionBase spriteset,
				int tabOffsetLength)
		{
			//LogFile.WriteLine("SpriteCollection.SaveSpriteset");
			string pfePck = Path.Combine(dir, file + GlobalsXC.PckExt);
			string pfeTab = Path.Combine(dir, file + GlobalsXC.TabExt);

			using (var bwPck = new BinaryWriter(File.Create(pfePck)))
			using (var bwTab = new BinaryWriter(File.Create(pfeTab)))
			{
				switch (tabOffsetLength)
				{
					case 2:
					{
						int pos = 0;
						foreach (XCImage sprite in spriteset)
						{
							//LogFile.WriteLine(". pos[pre]= " + pos);
							if (pos > UInt16.MaxValue) // bork. Psst, happens at ~150 sprites.
							{
								//LogFile.WriteLine(". . UInt16 MaxValue exceeded - ret FALSE");
								return false;
							}

							bwTab.Write((ushort)pos);
							pos += PckImage.SaveSpritesetSprite(bwPck, sprite);
							//LogFile.WriteLine(". pos[pst]= " + pos);
						}
						break;
					}

					case 4:
					{
						uint pos = 0;
						foreach (XCImage sprite in spriteset)
						{
							bwTab.Write(pos);
							pos += (uint)PckImage.SaveSpritesetSprite(bwPck, sprite);
						}
						break;
					}
				}
			}
			return true;
		}
		#endregion
	}
}
