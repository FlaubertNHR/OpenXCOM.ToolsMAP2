using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	public static class ResourceInfo
	{
		#region Fields (static)
		public static int[,] ScanGufo;
		public static int[,] ScanGtftd;

		public static BitArray LoFTufo;
		public static BitArray LoFTtftd;

		public const int TAB_WORD_LENGTH_0 = 0; // ie. no Tabfile
		public const int TAB_WORD_LENGTH_2 = 2;
		public const int TAB_WORD_LENGTH_4 = 4;
		#endregion


		#region Properties (static)
		public static TileGroupManager TileGroupManager
		{ get; private set; }

		private static readonly List<SpriteCollection> _spritesets =
							new List<SpriteCollection>();
		/// <summary>
		/// A list of spritesets in the currently loaded tileset or so.
		/// @note It has relevance only for 'MapInfoOutputBox'.
		/// </summary>
		public static List<SpriteCollection> Spritesets
		{
			get { return _spritesets; }
		}
		#endregion


		#region Methods (static)
		/// <summary>
		/// Initializes/ loads info about XCOM resources.
		/// </summary>
		/// <param name="pathConfig"></param>
		public static void InitializeResources(PathInfo pathConfig)
		{
			TileGroupManager = new TileGroupManager(new TilesetLoader(pathConfig.Fullpath));
		}

		/// <summary>
		/// Loads a given spriteset for UFO or TFTD. This could go in Descriptor
		/// except the XCOM cursor-sprites load w/out a descriptor. As do the
		/// 'ExtraSprites' in 'Globals' - although that's done differently w/
		/// Globals.LoadExtraSprites().
		/// @note Both UFO and TFTD use 2-byte TabwordLengths for 32x40 terrain sprites
		/// - TFTD unitsprites use 4-byte TabwordLengths although Bigobs 32x48 uses 2-byte
		/// - the UFO cursor uses 2-byte but the TFTD cursor uses 4-byte
		/// TODO: Each effing tilepart gets a pointer to the SpriteCollection.
		/// Effectively, at present, every tilepart maintains the
		/// SpriteCollection; the SpriteCollection should rather be an
		/// independent object maintained by a MapFile object eg.
		/// </summary>
		/// <param name="file">the file w/out extension</param>
		/// <param name="dir">path to the directory of the file</param>
		/// <param name="tabwordLength"></param>
		/// <param name="pal"></param>
		/// <param name="warnonly">true if called by McdView (warn-only if spriteset not found)</param>
		/// <returns>a SpriteCollection containing all the sprites</returns>
		public static SpriteCollection LoadSpriteset(
				string file,
				string dir,
				int tabwordLength,
				Palette pal,
				bool warnonly = false)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("ResourceInfo.LoadSpriteset");

			if (!String.IsNullOrEmpty(dir))
			{
				//LogFile.WriteLine(". dir= " + dir);
				//LogFile.WriteLine(". file= " + file);

				var pf = Path.Combine(dir, file);
				//LogFile.WriteLine(". pf= " + pf);

				string pfePck = pf + GlobalsXC.PckExt;
				string pfeTab = pf + GlobalsXC.TabExt;
				//LogFile.WriteLine(". pfePck= " + pfePck);
				//LogFile.WriteLine(". pfeTab= " + pfeTab);

				if (File.Exists(pfePck) && File.Exists(pfeTab))
				{
					using (var fsPck = File.OpenRead(pfePck))
					using (var fsTab = File.OpenRead(pfeTab))
					{
						var spriteset = new SpriteCollection(
															fsPck,
															fsTab,
															tabwordLength,
															pal,
															file);
						if (spriteset.Borked)
						{
							using (var f = new Infobox(
													" Spriteset borked",
													"The quantity of sprites in the PCK file does not match"
														+ " the quantity of sprites expected by the TAB file.",
													pfePck + Environment.NewLine + pfeTab))
							{
								f.ShowDialog();
							}
						}

						Spritesets.Add(spriteset); // used only by 'MapInfoOutputBox'.
						return spriteset;
					}
				}

				// error/warn ->
				string info;
				if (!warnonly)
				{
					info = Environment.NewLine + Environment.NewLine
						 + "Open the Map in the TilesetEditor and re-assign the basepath"
						 + " for the TERRAIN folder of the .PCK and .TAB files.";
				}
				else
					info = String.Empty;

				using (var f = new Infobox(
										" Spriteset not found",
										"Can't find files for the spriteset." + info,
										pfePck + Environment.NewLine + pfeTab))
				{
					f.ShowDialog();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the total count of sprites in 'Spritesets'.
		/// @note Used only by MapInfoOutputBox.Analyze()
		/// </summary>
		/// <returns>count of sprites</returns>
		public static int GetTotalSpriteCount()
		{
			int count = 0;
			foreach (var spriteset in Spritesets)
				count += spriteset.Count;

			return count;
		}


		public static bool LoadScanGufo(string dirUfo)
		{
			if (!String.IsNullOrEmpty(dirUfo))
			{
				string pfe = Path.Combine(dirUfo, SharedSpace.ScanGfile);
				if (File.Exists(pfe))
				{
					byte[] bytes = File.ReadAllBytes(pfe);
					int d1 = bytes.Length / 16;
					ScanGufo = new int[d1, 16];

					for (int i = 0; i != d1; ++i)
					for (int j = 0; j != 16; ++j)
					{
						ScanGufo[i,j] = bytes[i * 16 + j];
					}
					return true;
				}
			}
			return false;
		}

		public static bool LoadScanGtftd(string dirTftd)
		{
			if (!String.IsNullOrEmpty(dirTftd))
			{
				string pfe = Path.Combine(dirTftd, SharedSpace.ScanGfile);
				if (File.Exists(pfe))
				{
					byte[] bytes = File.ReadAllBytes(pfe);
					int d1 = bytes.Length / 16;
					ScanGtftd = new int[d1, 16];

					for (int i = 0; i != d1; ++i)
					for (int j = 0; j != 16; ++j)
					{
						ScanGtftd[i,j] = bytes[i * 16 + j];
					}
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Good Fucking Lord I want to knife-stab a stuffed Pikachu.
		/// </summary>
		/// <param name="dirUfo"></param>
		public static void LoadLoFTufo(string dirUfo)
		{
			if (!String.IsNullOrEmpty(dirUfo))
			{
				string pfe = Path.Combine(dirUfo, SharedSpace.LoftfileUfo);
				if (File.Exists(pfe))
				{
					// 32 bytes in a loft
					// 256 bits in a loft

					byte[] bytes = File.ReadAllBytes(pfe);
					int length = bytes.Length * 8;

					LoFTufo = new BitArray(length); // init to Falses

					// read the file as little-endian unsigned shorts
					// eg. C0 01 -> 01 C0

					int id = -1;
					for (int i = 0; i != bytes.Length; i += 2)
					{
						for (int j = 0x80; j != 0x00; j >>= 1)
						{
							LoFTufo[++id] = ((bytes[i + 1] & j) != 0);
						}

						for (int j = 0x80; j != 0x00; j >>= 1)
						{
							LoFTufo[++id] = ((bytes[i] & j) != 0);
						}
					}
				}
			}
		}

		/// <summary>
		/// Good Fucking Lord I want to knife-stab a stuffed Pikachu.
		/// </summary>
		/// <param name="dirTftd"></param>
		public static void LoadLoFTtftd(string dirTftd)
		{
			if (!String.IsNullOrEmpty(dirTftd))
			{
				string pfe = Path.Combine(dirTftd, SharedSpace.LoftfileTftd);
				if (File.Exists(pfe))
				{
					// 32 bytes in a loft
					// 256 bits in a loft

					byte[] bytes = File.ReadAllBytes(pfe);
					int length = bytes.Length * 8;

					LoFTtftd = new BitArray(length); // init to Falses

					// read the file as little-endian unsigned shorts
					// eg. C0 01 -> 01 C0

					int id = -1;
					for (int i = 0; i != bytes.Length; i += 2)
					{
						for (int j = 0x80; j != 0x00; j >>= 1)
						{
							LoFTtftd[++id] = ((bytes[i + 1] & j) != 0);
						}

						for (int j = 0x80; j != 0x00; j >>= 1)
						{
							LoFTtftd[++id] = ((bytes[i] & j) != 0);
						}
					}
				}
			}
		}
		#endregion
	}
}
