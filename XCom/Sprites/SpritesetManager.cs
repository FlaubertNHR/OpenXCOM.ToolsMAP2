using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	/// <summary>
	/// Creates <c><see cref="Spriteset">Spritesets</see></c> that are required
	/// to display <c><see cref="Tilepart">Tileparts</see></c> in MapView and
	/// maintains them in the <c><see cref="Spritesets"/></c> list.
	/// <c><see cref="LoadSpriteset()">LoadSpriteset()</see></c> is also used to
	/// create the Cursor-sprites in MapView and to load <c>Spritesets</c> in
	/// McdView but pointers to such <c>Spritesets</c> are not stored in the
	/// <c>Spritesets</c> list.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable crap</c>.</remarks>
	public static class SpritesetManager
	{
		#region Methods (disposable)
		/// <summary>
		/// Disposes sprites and clears the <c><see cref="Spritesets"/></c> list.
		/// </summary>
		public static void Dispose()
		{
			LogFile.WriteLine("SpritesetManager.Dispose() static");
			for (int i = 0; i != Spritesets.Count; ++i)
			{
				Spritesets[i].Dispose();
			}
			Spritesets.Clear();
		}
		#endregion Methods (disposable)


		#region Fields (static)
		private static int[,] _scanGufo;
		private static int[,] _scanGtftd;
		#endregion Fields (static)


		#region Properties (static)
		private static readonly IList<Spriteset> _spritesets = new List<Spriteset>();
		/// <summary>
		/// A list of <c><see cref="Spriteset">Spritesets</see></c> in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c> or so.
		/// </summary>
//		/// <remarks>It has relevance only for MapInfoDialog and
//		/// MainViewOptionables.SelectedTileColor/SelectedTileToner.</remarks>
		public static IList<Spriteset> Spritesets
		{
			get { return _spritesets; }
		}

		/// <summary>
		/// Gets the total count of sprites in all
		/// <c><see cref="Spritesets"/></c>.
		/// </summary>
		/// <remarks>Used only by <c>MapInfoDialog.Analyze()</c>.</remarks>
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
		/// Loads a <c><see cref="Spriteset"/></c> for UFO or TFTD. This could
		/// go in <c><see cref="Descriptor"/></c> except the XCOM cursor-sprites
		/// load w/out a <c>Descriptor</c>. As do the monotone-sprites -
		/// although that's done differently in
		/// <c>MainViewF.LoadMonotoneSprites()</c>.
		/// 
		/// 
		/// TODO: Each effing <c><see cref="Tilepart"/></c> gets a pointer to
		/// its <c>Spriteset</c>. Effectively, at present, every <c>Tilepart</c>
		/// maintains its <c>Spriteset</c>; the <c>Spriteset</c> should rather
		/// be an independent object maintained by a
		/// <c><see cref="MapFile"/></c> eg.
		/// </summary>
		/// <param name="label">the file w/out extension</param>
		/// <param name="dir">path to the directory of the file</param>
		/// <param name="tabwordLength">one of the
		/// <c><see cref="TAB_WORD_LENGTH_0">TAB_WORD_LENGTH_*</see></c>
		/// constants</param>
		/// <param name="pal">a freakin <c><see cref="Palette"/></c></param>
		/// <param name="bypassManager"><c>true</c> to not create Tonescaled
		/// sprites and, if called by McdView, don't screw with the spritesets
		/// when McdView is called via TileView</param>
		/// <returns>a <c>Spriteset</c> containing all the sprites, or null if
		/// the quantity of sprites in the PCK vs TAB files aren't equal</returns>
		/// <remarks>
		/// <list type="bullet">
		/// <item>both UFO and TFTD use 2-byte TabwordLengths for 32x40
		/// terrain-sprites and 32x48 bigobs-sprites</item>
		/// <item>TFTD unit-sprites use 4-byte TabwordLengths</item>
		/// <item>the UFO cursor uses 2-byte but the TFTD cursor uses 4-byte</item>
		/// </list></remarks>
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
		/// <remarks>cf
		/// <list type="bullet">
		/// <item><c><see cref="Spriteset(string, Stream, bool)">Spriteset(string, Stream, bool)</see></c></item>
		/// <item><c>McdviewF.LoadScanGufo()</c></item>
		/// </list></remarks>
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
		/// <remarks>cf
		/// <list type="bullet">
		/// <item><c><see cref="Spriteset(string, Stream, bool)">Spriteset(string, Stream, bool)</see></c></item>
		/// <item><c>McdviewF.LoadScanGtftd()</c></item>
		/// </list></remarks>
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
		/// <remarks>cf
		/// <list type="bullet">
		/// <item><c><see cref="Spriteset(string, Stream, bool)">Spriteset(string, Stream, bool)</see></c></item>
		/// <item><c>McdviewF.LoadLoFTufo()</c></item>
		/// </list></remarks>
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
		/// <remarks>cf
		/// <list type="bullet">
		/// <item><c><see cref="Spriteset(string, Stream, bool)">Spriteset(string, Stream, bool)</see></c></item>
		/// <item><c>McdviewF.LoadLoFTtftd()</c></item>
		/// </list></remarks>
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
