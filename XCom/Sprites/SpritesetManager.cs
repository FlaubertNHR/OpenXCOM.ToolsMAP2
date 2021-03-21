using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	public static class SpritesetManager
	{
		#region Fields (static)
		private static int[,] _scanGufo;
		private static int[,] _scanGtftd;
		#endregion Fields (static)


		#region Properties (static)
		private static readonly List<Spriteset> _spritesets =
							new List<Spriteset>();
		/// <summary>
		/// A list of spritesets in the currently loaded tileset or so.
		/// </summary>
		/// <remarks>It has relevance only for MapInfoDialog and
		/// MainViewOptionables.SelectedTileColor/SelectedTileToner.</remarks>
		public static List<Spriteset> Spritesets
		{
			get { return _spritesets; }
		}

		/// <summary>
		/// Gets the total count of sprites in all <see cref="Spritesets"/>.
		/// </summary>
		/// <remarks>Used only by MapInfoDialog.Analyze().</remarks>
		public static int TotalSpriteCount
		{
			get
			{
				int count = 0;
				foreach (var spriteset in Spritesets)
					count += spriteset.Count;

				return count;
			}
		}

		public static BitArray LoFTufo
		{ get; private set; }

		public static BitArray LoFTtftd
		{ get; private set; }

		public const int TAB_WORD_LENGTH_0 = 0; // ie. no Tabfile
		public const int TAB_WORD_LENGTH_2 = 2;
		public const int TAB_WORD_LENGTH_4 = 4;
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Loads a given spriteset for UFO or TFTD. This could go in Descriptor
		/// except the XCOM cursor-sprites load w/out a descriptor. As do the
		/// monotone-sprites - although that's done differently w/
		/// MainViewF.LoadMonotoneSprites().
		/// 
		/// TODO: Each effing tilepart gets a pointer to the Spriteset.
		/// Effectively, at present, every tilepart maintains the Spriteset; the
		/// Spriteset should rather be an independent object maintained by a
		/// MapFile object eg.
		/// </summary>
		/// <param name="label">the file w/out extension</param>
		/// <param name="dir">path to the directory of the file</param>
		/// <param name="tabwordLength"></param>
		/// <param name="pal"></param>
		/// <param name="bypassManager">true to not create Tonescaled sprites
		/// and, if called by McdView, don't screw with the spritesets when
		/// McdView is called via TileView</param>
		/// <returns>a Spriteset containing all the sprites, or null if the
		/// quantity of sprites in the PCK vs TAB files aren't equal</returns>
		/// <remarks>Both UFO and TFTD use 2-byte TabwordLengths for
		/// 
		/// - 32x40 terrain-sprites and 32x48 bigobs-sprites
		/// 
		/// - TFTD unit-sprites use 4-byte TabwordLengths
		/// 
		/// - the UFO cursor uses 2-byte but the TFTD cursor uses 4-byte</remarks>
		public static Spriteset LoadSpriteset(
				string label,
				string dir,
				int tabwordLength,
				Palette pal,
				bool bypassManager = false)
		{
			//LogFile.WriteLine("SpritesetManager.LoadSpriteSet()");

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
						var spriteset = new Spriteset(
													label,
													pal,
													tabwordLength,
													bytesPck,
													bytesTab,
													bypassManager);

						if ((spriteset.Fail & Spriteset.FAIL_COUNT_MISMATCH) != Spriteset.FAIL_non)
						{
							using (var f = new Infobox(
													"Error",
													Infobox.SplitString("The count of sprites in the PCK file does not"
															+ " match the count of sprites expected by the TAB file."),
													null,
													InfoboxType.Error))
							{
								f.ShowDialog();
							}
						}
						// else if ((spriteset.Fail & Spriteset.FAIL_OF_SPRITE) != Spriteset.FAIL_non)
						// {} // too many bytes for a nonbigob sprite - better not happen here.
						else
						{
							if (!bypassManager)
								Spritesets.Add(spriteset);

							return spriteset;
						}
					}
				}
			}
			return null;
		}


		/// <summary>
		/// Loads a ScanG.dat file for UFO.
		/// </summary>
		/// <param name="dirUfo"></param>
		/// <returns></returns>
		/// <remarks>See
		/// 
		/// - McdviewF.LoadScanGufo()
		/// 
		/// - Spriteset(string, Stream, bool)</remarks>
		public static bool LoadScanGufo(string dirUfo)
		{
			if (Directory.Exists(dirUfo))
			{
				string pfe = Path.Combine(dirUfo, SharedSpace.ScanGfile);

				byte[] bytes = FileService.ReadFile(pfe);
				if (bytes != null)
				{
					int d1 = bytes.Length / ScanGicon.Length_ScanG;
					_scanGufo = new int[d1, ScanGicon.Length_ScanG];

					for (int i = 0; i != d1; ++i)
					for (int j = 0; j != ScanGicon.Length_ScanG; ++j)
					{
						_scanGufo[i,j] = bytes[i * ScanGicon.Length_ScanG + j];
					}
					return true;
				}
			}

			_scanGufo = null;
			return false;
		}

		/// <summary>
		/// Loads a ScanG.dat file for TFTD.
		/// </summary>
		/// <param name="dirTftd"></param>
		/// <returns></returns>
		/// <remarks>See
		/// 
		/// - McdviewF.LoadScanGtftd()
		/// 
		/// - Spriteset(string, Stream, bool)</remarks>
		public static bool LoadScanGtftd(string dirTftd)
		{
			if (Directory.Exists(dirTftd))
			{
				string pfe = Path.Combine(dirTftd, SharedSpace.ScanGfile);

				byte[] bytes = FileService.ReadFile(pfe);
				if (bytes != null)
				{
					int d1 = bytes.Length / ScanGicon.Length_ScanG;
					_scanGtftd = new int[d1, ScanGicon.Length_ScanG];

					for (int i = 0; i != d1; ++i)
					for (int j = 0; j != ScanGicon.Length_ScanG; ++j)
					{
						_scanGtftd[i,j] = bytes[i * ScanGicon.Length_ScanG + j];
					}
					return true;
				}
			}

			_scanGtftd = null;
			return false;
		}


		/// <summary>
		/// Good Fucking Lord I want to knife-stab a stuffed Pikachu. Loads a
		/// LoFTemps.dat file for UFO.
		/// </summary>
		/// <param name="dirUfo"></param>
		/// <remarks>See
		/// 
		/// - McdviewF.LoadLoFTufo()
		/// 
		/// - Spriteset(string, Stream, bool)</remarks>
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
		/// </summary>
		/// <param name="dirTftd"></param>
		/// <remarks>See
		/// 
		/// - McdviewF.LoadLoFTtftd()
		/// 
		/// - Spriteset(string, Stream, bool)</remarks>
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

		/// <summary>
		/// Gets ScanG for UFO.
		/// </summary>
		/// <returns></returns>
		public static int[,] GetScanGufo()
		{
			return _scanGufo;
		}

		/// <summary>
		/// Gets ScanG for TFTD.
		/// </summary>
		/// <returns></returns>
		public static int[,] GetScanGtftd()
		{
			return _scanGtftd;
		}
		#endregion Methods (static)
	}
}
