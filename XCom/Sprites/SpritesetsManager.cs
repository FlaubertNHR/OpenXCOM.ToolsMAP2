using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	public static class SpritesetsManager
	{
		#region Fields (static)
		public static int[,] ScanGufo;
		public static int[,] ScanGtftd;

		public static BitArray LoFTufo;
		public static BitArray LoFTtftd;

		public const int TAB_WORD_LENGTH_0 = 0; // ie. no Tabfile
		public const int TAB_WORD_LENGTH_2 = 2;
		public const int TAB_WORD_LENGTH_4 = 4;
		#endregion Fields (static)


		#region Properties (static)
		private static readonly List<SpriteCollection> _spritesets =
							new List<SpriteCollection>();
		/// <summary>
		/// A list of spritesets in the currently loaded tileset or so.
		/// @note It has relevance only for MapInfoDialog and
		/// MainViewOptionables.SelectedTileColor/SelectedTileToner.
		/// </summary>
		public static List<SpriteCollection> Spritesets
		{
			get { return _spritesets; }
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Loads a given spriteset for UFO or TFTD. This could go in Descriptor
		/// except the XCOM cursor-sprites load w/out a descriptor. As do the
		/// monotone-sprites - although that's done differently w/
		/// MainViewF.LoadMonotoneSprites().
		/// @note Both UFO and TFTD use 2-byte TabwordLengths for
		/// - 32x40 terrain-sprites and 32x48 bigobs-sprites
		/// - TFTD unit-sprites use 4-byte TabwordLengths
		/// - the UFO cursor uses 2-byte but the TFTD cursor uses 4-byte
		/// TODO: Each effing tilepart gets a pointer to the SpriteCollection.
		/// Effectively, at present, every tilepart maintains the
		/// SpriteCollection; the SpriteCollection should rather be an
		/// independent object maintained by a MapFile object eg.
		/// </summary>
		/// <param name="label">the file w/out extension</param>
		/// <param name="dir">path to the directory of the file</param>
		/// <param name="tabwordLength"></param>
		/// <param name="pal"></param>
		/// <param name="preserveStaticSpritesets">true if called by McdView -
		/// dont screw with the spritesets when McdView is called via TileView</param>
		/// <returns>a SpriteCollection containing all the sprites, or null if
		/// the quantity of sprites in the PCK vs TAB files aren't equal</returns>
		public static SpriteCollection LoadSpriteset(
				string label,
				string dir,
				int tabwordLength,
				Palette pal,
				bool preserveStaticSpritesets = false)
		{
			//LogFile.WriteLine("SpritesetsManager.LoadSpriteSet()");

			if (Directory.Exists(dir))
			{
				// TODO: If files not found provide hint to assign a basepath to
				// TERRAIN with the TilesetEditor.

				string pf = Path.Combine(dir, label);

				byte[] bytesPck = FileService.ReadFile(pf + GlobalsXC.PckExt);
				if (bytesPck != null)
				{
					byte[] bytesTab = FileService.ReadFile(pf + GlobalsXC.TabExt);
					if (bytesTab != null)
					{
						var spriteset = new SpriteCollection(
														label,
														pal,
														tabwordLength,
														bytesPck,
														bytesTab);

						if ((spriteset.Fail & SpriteCollection.FAIL_COUNT_MISMATCH) != SpriteCollection.FAIL_non)
						{
							MessageBox.Show(
										"The count of sprites in the PCK file does not match"
											+ " the count of sprites expected by the TAB file.",
										" Error",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error,
										MessageBoxDefaultButton.Button1,
										0);
						}
						// else if ((spriteset.Fail & SpriteCollection.FAIL_OF_SPRITE) != SpriteCollection.FAIL_non)
						// {} // too many bytes for a nonbigob sprite - better not happen here.
						else
						{
							if (!preserveStaticSpritesets)
								Spritesets.Add(spriteset);

							return spriteset;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the total count of sprites in 'Spritesets'.
		/// @note Used only by MapInfoDialog.Analyze()
		/// </summary>
		/// <returns>count of sprites</returns>
		public static int GetTotalSpriteCount()
		{
			int count = 0;
			foreach (var spriteset in Spritesets)
				count += spriteset.Count;

			return count;
		}


		/// <summary>
		/// Loads a ScanG.dat file for UFO.
		/// @note Cf
		/// - McdviewF.LoadScanGufo()
		/// - SpriteCollection(string, Stream, bool)
		/// </summary>
		/// <param name="dirUfo"></param>
		/// <returns></returns>
		public static bool LoadScanGufo(string dirUfo)
		{
			if (Directory.Exists(dirUfo))
			{
				string pfe = Path.Combine(dirUfo, SharedSpace.ScanGfile);

				byte[] bytes = FileService.ReadFile(pfe);
				if (bytes != null)
				{
					int d1 = bytes.Length / ScanGicon.Length_ScanG;
					ScanGufo = new int[d1, ScanGicon.Length_ScanG];

					for (int i = 0; i != d1; ++i)
					for (int j = 0; j != ScanGicon.Length_ScanG; ++j)
					{
						ScanGufo[i,j] = bytes[i * ScanGicon.Length_ScanG + j];
					}
					return true;
				}
			}

			ScanGufo = null;
			return false;
		}

		/// <summary>
		/// Loads a ScanG.dat file for TFTD.
		/// @note Cf
		/// - McdviewF.LoadScanGtftd()
		/// - SpriteCollection(string, Stream, bool)
		/// </summary>
		/// <param name="dirTftd"></param>
		/// <returns></returns>
		public static bool LoadScanGtftd(string dirTftd)
		{
			if (Directory.Exists(dirTftd))
			{
				string pfe = Path.Combine(dirTftd, SharedSpace.ScanGfile);

				byte[] bytes = FileService.ReadFile(pfe);
				if (bytes != null)
				{
					int d1 = bytes.Length / ScanGicon.Length_ScanG;
					ScanGtftd = new int[d1, ScanGicon.Length_ScanG];

					for (int i = 0; i != d1; ++i)
					for (int j = 0; j != ScanGicon.Length_ScanG; ++j)
					{
						ScanGtftd[i,j] = bytes[i * ScanGicon.Length_ScanG + j];
					}
					return true;
				}
			}

			ScanGtftd = null;
			return false;
		}


		/// <summary>
		/// Good Fucking Lord I want to knife-stab a stuffed Pikachu.
		/// Loads a LoFTemps.dat file for UFO.
		/// @note Cf
		/// - McdviewF.LoadLoFTufo()
		/// - SpriteCollection(string, Stream, bool)
		/// </summary>
		/// <param name="dirUfo"></param>
		public static void LoadLoFTufo(string dirUfo)
		{
			if (Directory.Exists(dirUfo))
			{
				string pfe = Path.Combine(dirUfo, SharedSpace.LoftfileUfo);

				byte[] bytes = FileService.ReadFile(pfe);
				if (bytes != null)
				{
					// 32 bytes in a loft
					// 256 bits in a loft

					LoFTufo = new BitArray(bytes.Length * 8); // init to Falses

					// read the data as little-endian unsigned shorts
					// eg. C0 01 -> 01 C0

					int id = -1;
					for (int i = 0; i != bytes.Length; i += 2)
					{
						for (int j = 0x80; j != 0x00; j >>= 1) // 1000 0000
						{
							LoFTufo[++id] = ((bytes[i + 1] & j) != 0);
						}

						for (int j = 0x80; j != 0x00; j >>= 1)
						{
							LoFTufo[++id] = ((bytes[i] & j) != 0);
						}
					}
					return;
				}
			}

			LoFTufo = null;
		}

		/// <summary>
		/// Loads a LoFTemps.dat file for TFTD.
		/// @note Cf
		/// - McdviewF.LoadLoFTtftd()
		/// - SpriteCollection(string, Stream, bool)
		/// </summary>
		/// <param name="dirTftd"></param>
		public static void LoadLoFTtftd(string dirTftd)
		{
			if (Directory.Exists(dirTftd))
			{
				string pfe = Path.Combine(dirTftd, SharedSpace.LoftfileTftd);

				byte[] bytes = FileService.ReadFile(pfe);
				if (bytes != null)
				{
					// 32 bytes in a loft
					// 256 bits in a loft

					LoFTtftd = new BitArray(bytes.Length * 8); // init to Falses

					// read the data as little-endian unsigned shorts
					// eg. C0 01 -> 01 C0

					int id = -1;
					for (int i = 0; i != bytes.Length; i += 2)
					{
						for (int j = 0x80; j != 0x00; j >>= 1) // 1000 0000
						{
							LoFTtftd[++id] = ((bytes[i + 1] & j) != 0);
						}

						for (int j = 0x80; j != 0x00; j >>= 1)
						{
							LoFTtftd[++id] = ((bytes[i] & j) != 0);
						}
					}
					return;
				}
			}

			LoFTtftd = null;
		}
		#endregion Methods (static)
	}
}
