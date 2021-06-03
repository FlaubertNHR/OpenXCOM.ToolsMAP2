using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;


namespace XCom
{
	//see http://support.microsoft.com/default.aspx?scid=kb%3Ben-us%3B319061
	/// <summary>
	/// A class defining a Color array of 256 values.
	/// </summary>
	/// <remarks>Palettes are instantiated only as the 8 palettes for ufo/tftd
	/// and the 4 tone-scaled palettes (for selected tileparts in MainView). The
	/// transparency (alpha-value) of colorid #0 can be toggled but beware that
	/// it appears to require re-assigning the changed ColorPalette to affected
	/// sprites/spritesets ... palettes are difficult to reliably/intuitively
	/// work with in .net - you'd think they would be referenced but they could
	/// be/ are dereferences of a copy instead. .net strikes again!</remarks>
	public sealed class Palette
	{
		/// <summary>
		/// Disposes the UFO and TFTD brushes used by
		/// MainViewF.Optionables.UseMono.
		/// </summary>
		public static void DisposeMonoBrushes()
		{
			DSShared.LogFile.WriteLine("Palette.DisposeMonoBrushes()");
			foreach (var brush in _brushesUfoBattle)
				brush.Dispose();

			foreach (var brush in _brushesTftdBattle)
				brush.Dispose();
		}


		#region Fields (static)
		/// <summary>
		/// The transparent Id in the <see cref="Table">Table.Entries</see>
		/// array.
		/// </summary>
		public const byte Tid = 0x00;

		/// <summary>
		/// LoFT icons use only <see cref="ColorPalette">ColorPalette.Entries</see>
		/// #0 and #1. LoFTclear shall be black for non-solid voxelspace.
		/// </summary>
		public const byte LoFTclear = 0x00;

		/// <summary>
		/// LoFT icons use only <see cref="ColorPalette">ColorPalette.Entries</see>
		/// #0 and #1. LoFTSolid shall be white for solid voxelspace.
		/// </summary>
		public const byte LoFTSolid = 0x01;


		private static readonly Dictionary<string, Palette> _palettes =
							new Dictionary<string, Palette>();

		private const string Embedded     = "XCom._Embedded.";

		private const string ufobattle    = "ufo-battle";
		private const string ufogeo       = "ufo-geo";
		private const string ufograph     = "ufo-graph";
		private const string uforesearch  = "ufo-research";

		private const string tftdbattle   = "tftd-battle";
		private const string tftdgeo      = "tftd-geo";
		private const string tftdgraph    = "tftd-graph";
		private const string tftdresearch = "tftd-research";

		private const string binary       = "binary";

		private const string PalExt = ".pal";

		/// <summary>
		/// The suffix for the (key)label of the grayscaled version of this palette.
		/// </summary>
		private const string GRAY  = "#gray";
		/// <summary>
		/// The suffix for the (key)label of the redscaled version of this palette.
		/// </summary>
		private const string RED   = "#red";
		/// <summary>
		/// The suffix for the (key)label of the greenscaled version of this palette.
		/// </summary>
		private const string GREEN = "#green";
		/// <summary>
		/// The suffix for the (key)label of the bluescaled version of this palette.
		/// </summary>
		private const string BLUE  = "#blue";

		/// <summary>
		/// Bypasses creating tone-scaled subpalettes if this Palette is created
		/// by PckView or McdView.
		/// </summary>
		private static bool _bypassTonescales;
		#endregion Fields (static)


		#region Fields
		private bool _isTransparent;
		#endregion Fields


		#region Properties (static)
		/// <summary>
		/// The "ufo-battle" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used throughout MapView.</remarks>
		public static Palette UfoBattle
		{
			get
			{
				if (!_palettes.ContainsKey(ufobattle))
				{
					_palettes[ufobattle] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + ufobattle + PalExt),
													true);
					CreateUfoBrushes();
				}
				return _palettes[ufobattle] as Palette;
			}
		}

		/// <summary>
		/// The "ufo-geo" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette UfoGeo
		{
			get
			{
				if (!_palettes.ContainsKey(ufogeo))
					_palettes[ufogeo] = new Palette(
												Assembly.GetExecutingAssembly()
														.GetManifestResourceStream(Embedded + ufogeo + PalExt),
												false);
				return _palettes[ufogeo] as Palette;
			}
		}

		/// <summary>
		/// The "ufo-graph" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette UfoGraph
		{
			get
			{
				if (!_palettes.ContainsKey(ufograph))
					_palettes[ufograph] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + ufograph + PalExt),
													false);
				return _palettes[ufograph] as Palette;
			}
		}

		/// <summary>
		/// The "ufo-research" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette UfoResearch
		{
			get
			{
				if (!_palettes.ContainsKey(uforesearch))
					_palettes[uforesearch] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + uforesearch + PalExt),
													false);
				return _palettes[uforesearch] as Palette;
			}
		}

		/// <summary>
		/// The "tftd-battle" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used throughout MapView.</remarks>
		public static Palette TftdBattle
		{
			get
			{
				if (!_palettes.ContainsKey(tftdbattle))
				{
					_palettes[tftdbattle] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + tftdbattle + PalExt),
													true);
					CreateTftdBrushes();
				}
				return _palettes[tftdbattle] as Palette;
			}
		}

		/// <summary>
		/// The "tftd-geo" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette TftdGeo
		{
			get
			{
				if (!_palettes.ContainsKey(tftdgeo))
					_palettes[tftdgeo] = new Palette(
												Assembly.GetExecutingAssembly()
														.GetManifestResourceStream(Embedded + tftdgeo + PalExt),
												false);
				return _palettes[tftdgeo] as Palette;
			}
		}

		/// <summary>
		/// The "tftd-graph" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette TftdGraph
		{
			get
			{
				if (!_palettes.ContainsKey(tftdgraph))
					_palettes[tftdgraph] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + tftdgraph + PalExt),
													false);
				return _palettes[tftdgraph] as Palette;
			}
		}

		/// <summary>
		/// The "tftd-research" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette TftdResearch
		{
			get
			{
				if (!_palettes.ContainsKey(tftdresearch))
					_palettes[tftdresearch] = new Palette(
														Assembly.GetExecutingAssembly()
																.GetManifestResourceStream(Embedded + tftdresearch + PalExt),
														false);
				return _palettes[tftdresearch] as Palette;
			}
		}

		/// <summary>
		/// The "binary" palette _Embedded in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette Binary
		{
			get
			{
				if (!_palettes.ContainsKey(binary))
					CreateBinaryTable(_palettes[binary] = new Palette(binary));

				return _palettes[binary] as Palette;
			}
		}


		private static readonly IList<Brush> _brushesUfoBattle = new List<Brush>();
		/// <summary>
		/// Brushes for MainViewF.Optionables.UseMono.
		/// </summary>
		public static IList<Brush> BrushesUfoBattle
		{
			get { return _brushesUfoBattle; }
		}

		private static readonly IList<Brush> _brushesTftdBattle = new List<Brush>();
		/// <summary>
		/// Brushes for MainViewF.Optionables.UseMono.
		/// </summary>
		public static IList<Brush> BrushesTftdBattle
		{
			get { return _brushesTftdBattle; }
		}
		#endregion Properties (static)


		#region Properties
		/// <summary>
		/// Gets/Sets the (key)Label of this Palette.
		/// </summary>
		public string Label
		{ get; private set; }

		/// <summary>
		/// Gets/Sets the ColorPalette of this Palette (sic).
		/// </summary>
		public ColorPalette Table
		{ get; private set; }

		/// <summary>
		/// Gets/Sets the Color of a given index in this Palette's
		/// <see cref="Table"/>.
		/// </summary>
		public Color this[int id]
		{
			get { return Table.Entries[id]; }
			private set { Table.Entries[id] = value; }
		}


		// TODO: Move the TonescaledPalettes out to their own Palette(s).
		// Reference them in Tileparts perhaps. The interrelation among
		// Palettes, Tileparts, Sprites, Spritesets, Descriptors, etc. ought be
		// rethought and reworked ...

		/// <summary>
		/// The gray-scaled Palette of this Palette.
		/// </summary>
		public Palette GrayScale
		{
			get
			{
				if (_palettes.ContainsKey(Label + GRAY))
					return _palettes[Label + GRAY];

				return null;
			}
		}
		/// <summary>
		/// The red-scaled Palette of this Palette.
		/// </summary>
		public Palette RedScale
		{
			get { return _palettes[Label + RED]; }
		}
		/// <summary>
		/// The green-scaled Palette of this Palette.
		/// </summary>
		public Palette GreenScale
		{
			get { return _palettes[Label + GREEN]; }
		}
		/// <summary>
		/// The blue-scaled Palette of this Palette.
		/// </summary>
		public Palette BlueScale
		{
			get { return _palettes[Label + BLUE]; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates any of the 8 ufo/tftd palettes given a
		/// filestream of data.
		/// </summary>
		/// <param name="fs"></param>
		/// <param name="createTonescales">tonescaled palettes are required by
		/// ufo-battle and tftd-battle for MapView only</param>
		private Palette(Stream fs, bool createTonescales)
		{
			using (var b = new Bitmap(1,1, PixelFormat.Format8bppIndexed))
				Table = b.Palette;

			using (var sr = new StreamReader(fs))
			{
				Label = sr.ReadLine(); // 1st line is the label

				var rgb = new string[3];
				for (int id = 0; id != 256; ++id)
				{
					rgb = sr.ReadLine().Split(',');
					Table.Entries[id] = Color.FromArgb(
													Int32.Parse(rgb[0]),
													Int32.Parse(rgb[1]),
													Int32.Parse(rgb[2]));
				}

				if (createTonescales && !_bypassTonescales)
					CreateTonescaledPalettes(Label);
			}
		}

		/// <summary>
		/// cTor[1]. Instantiates a standard palette with a given label for a
		/// tone-scaled palette.
		/// </summary>
		/// <param name="label"></param>
		private Palette(string label)
		{
			using (var b = new Bitmap(1,1, PixelFormat.Format8bppIndexed))
				Table = b.Palette;

			Label = label;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Creates the toner-palettes.
		/// </summary>
		/// <param name="baselabel">all your label are belong to us</param>
		/// <remarks>Tonescaled palettes are required by ufo-battle and
		/// tftd-battle for MapView only.</remarks>
		private void CreateTonescaledPalettes(string baselabel)
		{
			string label;
			for (int i = 0; i != 4; ++i)
			{
				switch (i)
				{
					default: label = baselabel + GRAY;  break; // case 0
					case 1:  label = baselabel + RED;   break;
					case 2:  label = baselabel + GREEN; break;
					case 3:  label = baselabel + BLUE;  break;
				}

				var pal = new Palette(label);

				pal[Tid] = Color.FromArgb(0, 0,0,0); // id#0 shall always be transparent black

				Color color; int val;
				for (int id = Tid + 1; id != Table.Entries.Length; ++id)
				{
					color = this[id];
					val = GetMonochromaticBrightness(color.R, color.G, color.B);

					switch (i)
					{
						case 0: pal[id] = Color.FromArgb(val,val,val); break; // grayscale
						case 1: pal[id] = Color.FromArgb(val,  0,  0); break; // redscale
						case 2: pal[id] = Color.FromArgb(  0,val,  0); break; // greenscale
						case 3: pal[id] = Color.FromArgb(  0,  0,val); break; // bluescale
					}
				}
				_palettes[label] = pal;
			}
		}

		/// <summary>
		/// Gets a brightness value (roughly).
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <returns></returns>
		/// <remarks>See also ColorHelp.GetTextColor().</remarks>
		private static int GetMonochromaticBrightness(int red, int green, int blue)
		{
			return (int)(Math.Sqrt(Math.Pow(red,   2) * 0.2990
								 + Math.Pow(green, 2) * 0.5870
								 + Math.Pow(blue,  2) * 0.1140));
		}
		// red= 62% blue, green= 127% blue, blue= 100% blue
		//https://howlettstudios.com/articles/2017/5/6/the-problem-with-hsv


		/// <summary>
		/// Enables or disables transparency on the 'Tid' palette-index.
		/// TODO: It would perhaps be worthwhile to create a separate Palette
		/// for ufo-battle and tftd-battle that has id #0 transparent.
		/// </summary>
		/// <param name="transparent">true to enable transparency</param>
		public void SetTransparent(bool transparent)
		{
			if (transparent != _isTransparent)
			{
				_isTransparent = transparent;

				int alpha;
				if (transparent) alpha =   0;
				else             alpha = 255;

				Table.Entries[Tid] = Color.FromArgb(alpha, Table.Entries[Tid]);
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Creates a binary ColorTable for LoFTicons.
		/// </summary>
		private static void CreateBinaryTable(Palette pal)
		{
			pal.Table.Entries[Tid] = Color.Black;

			for (int id = Tid + 1; id != 256; ++id)
				pal.Table.Entries[id] = Color.White;
		}


		/// <summary>
		/// Create brushes for drawing UFO sprites per
		/// MainViewF.Optionables.UseMono.
		/// </summary>
		private static void CreateUfoBrushes()
		{
			var pal = _palettes[ufobattle] as Palette;
			foreach (var color in pal.Table.Entries)
			{
				BrushesUfoBattle.Add(new SolidBrush(color));
			}
		}

		/// <summary>
		/// Create brushes for drawing TFTD sprites per
		/// MainViewF.Optionables.UseMono.
		/// </summary>
		private static void CreateTftdBrushes()
		{
			var pal = _palettes[tftdbattle] as Palette;
			foreach (var color in pal.Table.Entries)
			{
				BrushesTftdBattle.Add(new SolidBrush(color));
			}
		}

		/// <summary>
		/// Bypasses creating tone-scaled subpalettes if this Palette is created
		/// by PckView or McdView.
		/// </summary>
		/// <param name="bypass"></param>
		public static void BypassTonescales(bool bypass)
		{
			_bypassTonescales = bypass;
		}
		#endregion Methods (static)


		#region Methods (override)
		/// <summary>
		/// Returns <see cref="Label"/>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}

		/// <summary>
		/// Checks for palette equality.
		/// </summary>
		/// <param name="obj">another palette</param>
		/// <returns>true if the palette names are the same</returns>
		public override bool Equals(Object obj)
		{
			var pal = obj as Palette;
			return (pal != null && Table.Equals(pal.Table));
		}

		/// <summary>
		/// Gets an untrustable hashcode but it's a hashcode.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Table.GetHashCode(); // FIX: "Non-readonly field referenced in GetHashCode()."
		}
		#endregion Methods (override)
	}
}
