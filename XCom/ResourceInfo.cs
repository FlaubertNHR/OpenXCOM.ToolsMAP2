using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	public static class ResourceInfo
	{
		#region Fields (static)
		private static readonly Dictionary<Palette, Dictionary<string, SpriteCollection>> _spritesets_byPalette =
							new Dictionary<Palette, Dictionary<string, SpriteCollection>>();

		public static bool ReloadSprites;

		public static int[,] ScanGufo;
		public static int[,] ScanGtftd;

		public static BitArray LoFTufo;
		public static BitArray LoFTtftd;
		#endregion


		#region Properties (static)
		public static TileGroupManager TileGroupManager
		{ get; private set; }
		#endregion


		#region Methods (static)
		/// <summary>
		/// Initializes/ loads info about XCOM resources.
		/// </summary>
		/// <param name="pathConfig"></param>
		public static void InitializeResources(PathInfo pathConfig)
		{
//			XConsole.Init(20);

			TileGroupManager = new TileGroupManager(new TilesetLoader(pathConfig.Fullpath));
		}

		/// <summary>
		/// Loads a given spriteset for UFO or TFTD. This could go in Descriptor
		/// except the XCOM cursor-sprites load w/out a descriptor. As do the
		/// 'ExtraSprites'.
		/// @note Both UFO and TFTD use 2-byte Tab-offsetLengths for 32x40 terrain pcks
		/// (TFTD unitsprites use 4-byte Tab-offsetLengths although Bigobs 32x48 uses 2-byte)
		/// (the UFO cursor uses 2-byte but the TFTD cursor uses 4-byte)
		/// </summary>
		/// <param name="terrain">the terrain file w/out extension</param>
		/// <param name="dirTerrain">path to the directory of the terrain file</param>
		/// <param name="offsetLength"></param>
		/// <param name="pal"></param>
		/// <param name="noError">true if called by McdView (warn-only if spriteset not found)</param>
		/// <returns>a SpriteCollection containing all the sprites for a given Terrain</returns>
		public static SpriteCollection LoadSpriteset(
				string terrain,
				string dirTerrain,
				int offsetLength,
				Palette pal,
				bool noError = false)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("ResourceInfo.LoadSpriteset");

			if (!String.IsNullOrEmpty(dirTerrain))
			{
				//LogFile.WriteLine(". dirTerrain= " + dirTerrain);
				//LogFile.WriteLine(". terrain= " + terrain);

				var pfSpriteset = Path.Combine(dirTerrain, terrain);
				//LogFile.WriteLine(". pfSpriteset= " + pfSpriteset);

				string pfePck = pfSpriteset + GlobalsXC.PckExt;
				string pfeTab = pfSpriteset + GlobalsXC.TabExt;

				if (File.Exists(pfePck) && File.Exists(pfeTab))
				{
					if (!_spritesets_byPalette.ContainsKey(pal))
						 _spritesets_byPalette.Add(pal, new Dictionary<string, SpriteCollection>());

					var spritesets = _spritesets_byPalette[pal];

					if (ReloadSprites) // used by XCMainWindow.OnPckSavedEvent <- TileView's pck-editor
					{
						//LogFile.WriteLine(". ReloadSprites");

						if (spritesets.ContainsKey(pfSpriteset))
							spritesets.Remove(pfSpriteset);
					}

					if (!spritesets.ContainsKey(pfSpriteset))
					{
						//LogFile.WriteLine(". . key not found in spriteset dictionary -> add new SpriteCollection");

						using (var fsPck = File.OpenRead(pfePck))
						using (var fsTab = File.OpenRead(pfeTab))
						{
							var spriteset = new SpriteCollection(
																fsPck,
																fsTab,
																offsetLength,
																pal);
							if (spriteset.Borked)
							{
								MessageBox.Show(
											"The quantity of sprites in the PCK file does not match the"
												+ " quantity of sprites expected by the TAB file."
												+ Environment.NewLine + Environment.NewLine
												+ pfePck + Environment.NewLine
												+ pfeTab,
											"Error",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error,
											MessageBoxDefaultButton.Button1,
											0);
							}
							spritesets.Add(pfSpriteset, spriteset); // NOTE: Add the spriteset even if it is Borked.
						}
					}
					// else WARN: Spriteset already found in the collection.

					return _spritesets_byPalette[pal][pfSpriteset];
				}

				// error/warn ->
				string title;
				string info = "Can't find files for the spriteset"
								+ Environment.NewLine + Environment.NewLine
								+ pfePck + Environment.NewLine
								+ pfeTab;
				MessageBoxIcon icon;
				if (noError)
				{
					title = "Warning";
					icon = MessageBoxIcon.Warning;
				}
				else
				{
					title = "Error";
					icon = MessageBoxIcon.Error;
					info += Environment.NewLine + Environment.NewLine
						  + "Open the Map in the TilesetEditor and re-assign the basepath"
						  + " for the TERRAIN folder of the .PCK and .TAB files.";
				}
				MessageBox.Show(
							info,
							title,
							MessageBoxButtons.OK,
							icon,
							MessageBoxDefaultButton.Button1,
							0);
			}
			return null;
		}

		/// <summary>
		/// Gets the count of sprites in a sprite-collection.
		/// @note Used only by MapInfoOutputBox.Analyze()
		/// </summary>
		/// <param name="terrain">the terrain file w/out extension</param>
		/// <param name="dirTerrain">path to the directory of the terrain file</param>
		/// <param name="pal"></param>
		/// <returns>count of sprites</returns>
		internal static int GetSpritesetCount(
				string terrain,
				string dirTerrain,
				Palette pal)
		{
			var pfSpriteset = Path.Combine(dirTerrain, terrain);
			return _spritesets_byPalette[pal][pfSpriteset].Count;
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
