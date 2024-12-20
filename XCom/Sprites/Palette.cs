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
	/// A class defining a <c>Color</c> array of 256 values.
	/// </summary>
	/// <remarks><c>Palettes</c> are instantiated only as the 8 palettes for
	/// ufo/tftd and the 4 tone-scaled palettes (for selected tileparts in
	/// MainView). The transparency (alpha-value) of colorid #0 can be toggled
	/// but beware that it appears to require re-assigning the changed
	/// <c>ColorPalette</c> to affected sprites/spritesets ... palettes are
	/// difficult to reliably/intuitively work with in .net - you'd think they
	/// would be referenced but they could be/are dereferences of a copy
	/// instead. .net strikes again!</remarks>
	public sealed class Palette
	{
		/// <summary>
		/// Disposes the UFO and TFTD brushes used by
		/// <c>MainViewF.Optionables.UseMono</c>.
		/// </summary>
		public static void DisposeMonoBrushes()
		{
			//DSShared.Logfile.Log("Palette.DisposeMonoBrushes() static");
			foreach (var brush in _brushesUfoBattle)
				brush.Dispose();

			foreach (var brush in _brushesTftdBattle)
				brush.Dispose();
		}


		#region Fields (static)
		/// <summary>
		/// The transparent Id in the
		/// <c><see cref="Table">Table.Entries</see></c> array.
		/// </summary>
		public const byte Tid = 0x00;

		/// <summary>
		/// LoFT icons use only
		/// <c><see cref="ColorPalette">ColorPalette.Entries</see></c> #0 and
		/// #1. LoFTclear shall be black for non-solid voxelspace.
		/// </summary>
		public const byte LoFTclear = 0x00;

		/// <summary>
		/// LoFT icons use only
		/// <c><see cref="ColorPalette">ColorPalette.Entries</see></c> #0 and
		/// #1. LoFTSolid shall be white for solid voxelspace.
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

		private const string PalExt       = ".pal";

		/// <summary>
		/// The suffix for the (key)label of the grayscaled version of this
		/// <c>Palette</c>.
		/// </summary>
		private const string GRAY  = "#gray";
		/// <summary>
		/// The suffix for the (key)label of the redscaled version of this
		/// <c>Palette</c>.
		/// </summary>
		private const string RED   = "#red";
		/// <summary>
		/// The suffix for the (key)label of the greenscaled version of this
		/// <c>Palette</c>.
		/// </summary>
		private const string GREEN = "#green";
		/// <summary>
		/// The suffix for the (key)label of the bluescaled version of this
		/// <c>Palette</c>.
		/// </summary>
		private const string BLUE  = "#blue";
		#endregion Fields (static)


		#region Fields
		private bool _isTransparent;
		#endregion Fields


		#region Properties (static)
		/// <summary>
		/// The "ufo-battle" palette <c>_Embedded</c> in this assembly.
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
															.GetManifestResourceStream(Embedded + ufobattle + PalExt));
				}
				return _palettes[ufobattle];
			}
		}

		/// <summary>
		/// The "ufo-geo" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette UfoGeo
		{
			get
			{
				if (!_palettes.ContainsKey(ufogeo))
					_palettes[ufogeo] = new Palette(
												Assembly.GetExecutingAssembly()
														.GetManifestResourceStream(Embedded + ufogeo + PalExt));
				return _palettes[ufogeo];
			}
		}

		/// <summary>
		/// The "ufo-graph" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette UfoGraph
		{
			get
			{
				if (!_palettes.ContainsKey(ufograph))
					_palettes[ufograph] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + ufograph + PalExt));
				return _palettes[ufograph];
			}
		}

		/// <summary>
		/// The "ufo-research" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette UfoResearch
		{
			get
			{
				if (!_palettes.ContainsKey(uforesearch))
					_palettes[uforesearch] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + uforesearch + PalExt));
				return _palettes[uforesearch];
			}
		}

		/// <summary>
		/// The "tftd-battle" palette <c>_Embedded</c> in this assembly.
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
															.GetManifestResourceStream(Embedded + tftdbattle + PalExt));
				}
				return _palettes[tftdbattle];
			}
		}

		/// <summary>
		/// The "tftd-geo" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette TftdGeo
		{
			get
			{
				if (!_palettes.ContainsKey(tftdgeo))
					_palettes[tftdgeo] = new Palette(
												Assembly.GetExecutingAssembly()
														.GetManifestResourceStream(Embedded + tftdgeo + PalExt));
				return _palettes[tftdgeo];
			}
		}

		/// <summary>
		/// The "tftd-graph" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette TftdGraph
		{
			get
			{
				if (!_palettes.ContainsKey(tftdgraph))
					_palettes[tftdgraph] = new Palette(
													Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + tftdgraph + PalExt));
				return _palettes[tftdgraph];
			}
		}

		/// <summary>
		/// The "tftd-research" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette TftdResearch
		{
			get
			{
				if (!_palettes.ContainsKey(tftdresearch))
					_palettes[tftdresearch] = new Palette(
														Assembly.GetExecutingAssembly()
																.GetManifestResourceStream(Embedded + tftdresearch + PalExt));
				return _palettes[tftdresearch];
			}
		}

		/// <summary>
		/// The "binary" palette <c>_Embedded</c> in this assembly.
		/// </summary>
		/// <remarks>Is used by PckView only.</remarks>
		public static Palette Binary
		{
			get
			{
				if (!_palettes.ContainsKey(binary))
					CreateBinaryTable(_palettes[binary] = new Palette(binary));

				return _palettes[binary];
			}
		}


		private static readonly IList<Brush> _brushesUfoBattle = new List<Brush>();
		/// <summary>
		/// Brushes for <c>MainViewF.Optionables.UseMono</c>.
		/// </summary>
		public static IList<Brush> BrushesUfoBattle
		{
			get { return _brushesUfoBattle; }
		}

		private static readonly IList<Brush> _brushesTftdBattle = new List<Brush>();
		/// <summary>
		/// Brushes for <c>MainViewF.Optionables.UseMono</c>.
		/// </summary>
		public static IList<Brush> BrushesTftdBattle
		{
			get { return _brushesTftdBattle; }
		}

		/// <summary>
		/// Brushes for Mono builds.
		/// </summary>
		/// <remarks>Used by
		/// <list type="bullet">
		/// <item><c>MainViewOverlay</c></item>
		/// <item><c>TilePanel</c></item>
		/// <item><c>QuadrantDrawService</c></item>
		/// </list></remarks>
		public static IList<Brush> MonoBrushes
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		/// <summary>
		/// Gets/Sets the (key)Label of this <c>Palette</c>.
		/// </summary>
		public string Label
		{ get; private set; }

		/// <summary>
		/// Gets/Sets the <c>ColorPalette</c> of this <c>Palette</c> (sic).
		/// </summary>
		public ColorPalette Table
		{ get; private set; }


		// TODO: Move the TonescaledPalettes out to their own Palette(s).
		// Reference them in Tileparts perhaps. The interrelation among
		// Palettes, Tileparts, Sprites, Spritesets, Descriptors, etc. ought be
		// rethought and reworked ...

		/// <summary>
		/// The gray-scaled <c>Palette</c> of this <c>Palette</c>.
		/// </summary>
		public Palette GrayScale
		{
			get { return _palettes[Label + GRAY]; }
		}
		/// <summary>
		/// The red-scaled <c>Palette</c> of this <c>Palette</c>.
		/// </summary>
		public Palette RedScale
		{
			get { return _palettes[Label + RED]; }
		}
		/// <summary>
		/// The green-scaled <c>Palette</c> of this <c>Palette</c>.
		/// </summary>
		public Palette GreenScale
		{
			get { return _palettes[Label + GREEN]; }
		}
		/// <summary>
		/// The blue-scaled <c>Palette</c> of this <c>Palette</c>.
		/// </summary>
		public Palette BlueScale
		{
			get { return _palettes[Label + BLUE]; }
		}
		#endregion Properties


		#region Indexers
		/// <summary>
		/// Gets/Sets the <c>Color</c> of a given index in this <c>Palette's</c>
		/// <c><see cref="Table"/></c>.
		/// </summary>
		public Color this[int id]
		{
			get { return Table.Entries[id]; }
			private set { Table.Entries[id] = value; }
		}
		#endregion Indexers


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates any of the 8 ufo/tftd <c>Palettes</c> given a
		/// filestream of data.
		/// </summary>
		/// <param name="fs"></param>
		private Palette(Stream fs)
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
			}
		}

		/// <summary>
		/// cTor[1]. Instantiates a standard <c>Palette</c> with a given label
		/// for a tone-scaled palette.
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
		/// <param name="brightness"></param>
		/// <remarks>Tonescaled <c>Palettes</c> are required by ufo-battle and
		/// tftd-battle for MapView only.</remarks>
		public void CreateTonescaledPalettes(int brightness)
		{
			string label;
			for (int i = 0; i != 4; ++i)
			{
				switch (i)
				{
					default: label = Label + GRAY;  break; // case 0
					case 1:  label = Label + RED;   break;
					case 2:  label = Label + GREEN; break;
					case 3:  label = Label + BLUE;  break;
				}

				var pal = new Palette(label);

				pal[Tid] = Color.FromArgb(0, 0,0,0); // id#0 shall always be transparent black

				Color color; int val;
				for (int id = 1; id != Table.Entries.Length; ++id)
				{
					color = this[id];
					val = GetBrightness(color.R, color.G, color.B) * brightness / 5;
					val = Math.Max(Byte.MinValue, Math.Min(val, Byte.MaxValue));

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
		private static int GetBrightness(int red, int green, int blue)
		{
			return (int)(Math.Sqrt(Math.Pow(red,   2) * 0.2990
								 + Math.Pow(green, 2) * 0.5870
								 + Math.Pow(blue,  2) * 0.1140));
		}
		// red= 62% blue, green= 127% blue, blue= 100% blue
		//https://howlettstudios.com/articles/2017/5/6/the-problem-with-hsv


		/// <summary>
		/// Enables or disables transparency on the <c><see cref="Tid"/></c>
		/// palette-index.
		/// </summary>
		/// <param name="transparent"><c>true</c> to enable transparency</param>
		/// <remarks>TODO: It would perhaps be worthwhile to create a separate
		/// <c>Palette</c> for ufo-battle and tftd-battle that has id #0
		/// transparent.</remarks>
		public void SetTransparent(bool transparent)
		{
			if (transparent != _isTransparent)
			{
				Table.Entries[Tid] = Color.FromArgb(
												((_isTransparent = transparent) ? 0 : 255),
												Table.Entries[Tid]);
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Creates a binary <c>ColorTable</c> for LoFTicons.
		/// </summary>
		private static void CreateBinaryTable(Palette pal)
		{
			pal.Table.Entries[Tid] = Color.Black;

			for (int id = 1; id != 256; ++id)
				pal.Table.Entries[id] = Color.White;
		}


		/// <summary>
		/// Create brushes for drawing UFO sprites per
		/// <c>MainViewF.Optionables.UseMono</c>.
		/// </summary>
		public static void CreateUfoBrushes()
		{
			Color[] colors = _palettes[ufobattle].Table.Entries;
			foreach (var color in colors)
			{
				BrushesUfoBattle.Add(new SolidBrush(color));
			}
		}

		/// <summary>
		/// Create brushes for drawing TFTD sprites per
		/// <c>MainViewF.Optionables.UseMono</c>.
		/// </summary>
		public static void CreateTftdBrushes()
		{
			Color[] colors = _palettes[tftdbattle].Table.Entries;
			foreach (var color in colors)
			{
				BrushesTftdBattle.Add(new SolidBrush(color));
			}
		}
		#endregion Methods (static)


		#region Methods (override)
		/// <summary>
		/// Returns <c><see cref="Label"/></c>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}

		/// <summary>
		/// Checks this <c>Palette</c> against another <c>Palette</c> for
		/// equality.
		/// </summary>
		/// <param name="obj">another <c>Palette</c></param>
		/// <returns><c>true</c> if the palette names are the same</returns>
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
