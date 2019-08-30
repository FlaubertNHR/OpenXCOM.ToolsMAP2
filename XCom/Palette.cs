using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;


namespace XCom
{
	/// <summary>
	/// A class defining a color array of 256 values.
	/// </summary>
	//see http://support.microsoft.com/default.aspx?scid=kb%3Ben-us%3B319061
	public sealed class Palette
	{
		#region Fields (static)
		public const byte TranId = 0x00;

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

		private const string PalExt = ".pal";

		/// <summary>
		/// The suffix for the (key)label of the grayscaled version of this palette.
		/// </summary>
		private const string GRAYSCALED  = "#gray";
		/// <summary>
		/// The suffix for the (key)label of the redscaled version of this palette.
		/// </summary>
		private const string REDSCALED   = "#red";
		/// <summary>
		/// The suffix for the (key)label of the greenscaled version of this palette.
		/// </summary>
		private const string GREENSCALED = "#green";
		/// <summary>
		/// The suffix for the (key)label of the bluescaled version of this palette.
		/// </summary>
		private const string BLUESCALED  = "#blue";

		public static List<Brush> BrushesUfoBattle  = new List<Brush>(); // used by Mono only
		public static List<Brush> BrushesTftdBattle = new List<Brush>(); // used by Mono only
		#endregion Fields (static)


		#region Properties (static)
		/// <summary>
		/// The UFO Palette(s) embedded in this assembly.
		/// </summary>
		public static Palette UfoBattle
		{
			get
			{
				if (!_palettes.ContainsKey(ufobattle))
				{
					_palettes[ufobattle] = new Palette(Assembly.GetExecutingAssembly()
															   .GetManifestResourceStream(Embedded + ufobattle + PalExt));
					CreateUfoBrushes();
				}
				return _palettes[ufobattle] as Palette;
			}
		}

		public static Palette UfoGeo
		{
			get
			{
				if (!_palettes.ContainsKey(ufogeo))
					_palettes[ufogeo] = new Palette(Assembly.GetExecutingAssembly()
															.GetManifestResourceStream(Embedded + ufogeo + PalExt));
				return _palettes[ufogeo] as Palette;
			}
		}

		public static Palette UfoGraph
		{
			get
			{
				if (!_palettes.ContainsKey(ufograph))
					_palettes[ufograph] = new Palette(Assembly.GetExecutingAssembly()
															  .GetManifestResourceStream(Embedded + ufograph + PalExt));
				return _palettes[ufograph] as Palette;
			}
		}

		public static Palette UfoResearch
		{
			get
			{
				if (!_palettes.ContainsKey(uforesearch))
					_palettes[uforesearch] = new Palette(Assembly.GetExecutingAssembly()
																 .GetManifestResourceStream(Embedded + uforesearch + PalExt));
				return _palettes[uforesearch] as Palette;
			}
		}

		/// <summary>
		/// The TFTD Palette(s) embedded in this assembly.
		/// </summary>
		public static Palette TftdBattle
		{
			get
			{
				if (!_palettes.ContainsKey(tftdbattle))
				{
					_palettes[tftdbattle] = new Palette(Assembly.GetExecutingAssembly()
																.GetManifestResourceStream(Embedded + tftdbattle + PalExt));
					CreateTftdBrushes();
				}
				return _palettes[tftdbattle] as Palette;
			}
		}

		public static Palette TftdGeo
		{
			get
			{
				if (!_palettes.ContainsKey(tftdgeo))
					_palettes[tftdgeo] = new Palette(Assembly.GetExecutingAssembly()
															 .GetManifestResourceStream(Embedded + tftdgeo + PalExt));
				return _palettes[tftdgeo] as Palette;
			}
		}

		public static Palette TftdGraph
		{
			get
			{
				if (!_palettes.ContainsKey(tftdgraph))
					_palettes[tftdgraph] = new Palette(Assembly.GetExecutingAssembly()
															   .GetManifestResourceStream(Embedded + tftdgraph + PalExt));
				return _palettes[tftdgraph] as Palette;
			}
		}

		public static Palette TftdResearch
		{
			get
			{
				if (!_palettes.ContainsKey(tftdresearch))
					_palettes[tftdresearch] = new Palette(Assembly.GetExecutingAssembly()
																  .GetManifestResourceStream(Embedded + tftdresearch + PalExt));
				return _palettes[tftdresearch] as Palette;
			}
		}
		#endregion Properties (static)


		#region Properties
		/// <summary>
		/// Gets/Sets the (key)label of this palette.
		/// </summary>
		public string Label
		{ get; set; }

		/// <summary>
		/// Gets/Sets the colortable of this palette.
		/// </summary>
		public ColorPalette ColorTable
		{ get; private set; }

		/// <summary>
		/// Gets/Sets the color of a given index in this palette's color-table.
		/// </summary>
		public Color this[int id]
		{
			get { return ColorTable.Entries[id]; }
			private set { ColorTable.Entries[id] = value; }
		}


		public Palette GrayScaled
		{
			get { return _palettes[Label + GRAYSCALED]; }
		}
		public Palette RedScaled
		{
			get { return _palettes[Label + REDSCALED]; }
		}
		public Palette GreenScaled
		{
			get { return _palettes[Label + GREENSCALED]; }
		}
		public Palette BlueScaled
		{
			get { return _palettes[Label + BLUESCALED]; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a palette given a filestream of data.
		/// </summary>
		/// <param name="fs"></param>
		private Palette(Stream fs)
		{
			using (var b = new Bitmap(1,1, PixelFormat.Format8bppIndexed))
				ColorTable = b.Palette;

			using (var input = new StreamReader(fs))
			{
				Label = input.ReadLine(); // 1st line is the label.

				var rgb = new string[3];

				var invariant = System.Globalization.CultureInfo.InvariantCulture;

				for (int id = 0; id != 256; ++id)
				{
					rgb = input.ReadLine().Split(',');
					ColorTable.Entries[id] = Color.FromArgb(
														Int32.Parse(rgb[0], invariant),
														Int32.Parse(rgb[1], invariant),
														Int32.Parse(rgb[2], invariant));
				}
				CreateTonescaledPalettes(Label);
			}
		}

		/// <summary>
		/// cTor[1]. Instantiates a standard palette with a given label.
		/// </summary>
		/// <param name="label"></param>
		private Palette(string label)
		{
			using (var b = new Bitmap(1,1, PixelFormat.Format8bppIndexed))
				ColorTable = b.Palette;

			Label = label;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Enables or disables transparency on the 'TranId' palette-index.
		/// </summary>
		/// <param name="tran">true to enable transparency</param>
		public void SetTransparent(bool tran)
		{
			int alpha;
			if (tran) alpha =   0;
			else      alpha = 255;

			ColorTable.Entries[TranId] = Color.FromArgb(alpha, ColorTable.Entries[TranId]);
		}

		/// <summary>
		/// Creates the toner-palettes.
		/// </summary>
		/// <param name="baselabel">all your label are belong to us</param>
		private void CreateTonescaledPalettes(string baselabel)
		{
			string label;
			for (int i = 0; i != 4; ++i)
			{
				switch (i)
				{
					default: label = baselabel + GRAYSCALED;  break; // case 0
					case 1:  label = baselabel + REDSCALED;   break;
					case 2:  label = baselabel + GREENSCALED; break;
					case 3:  label = baselabel + BLUESCALED;  break;
				}

				var pal = new Palette(label);

				Color color = this[0];
				int val = GetL(color.R, color.G, color.B);
				pal[0] = Color.FromArgb(0, val,val,val); // id#0 shall always be transparent grayscale

				for (int id = 1; id != ColorTable.Entries.Length; ++id)
				{
					color = this[id];
					val = GetL(color.R, color.G, color.B);

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
		/// Gets a luminance value (roughly).
		/// See also ColorHelp.GetTextColor().
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <returns></returns>
		private int GetL(int red, int green, int blue)
		{
//			return (int)(red * 0.2126 + green * 0.7152 + blue * 0.0722);
			return (int)(red * 0.2990 + green * 0.5870 + blue * 0.1140);
		}
		#endregion Methods


		#region Methods (static)
		private static void CreateUfoBrushes()
		{
			var pal = _palettes[ufobattle] as Palette;
			foreach (var color in pal.ColorTable.Entries)
			{
				BrushesUfoBattle.Add(new SolidBrush(color));
			}
		}

		private static void CreateTftdBrushes()
		{
			var pal = _palettes[tftdbattle] as Palette;
			foreach (var color in pal.ColorTable.Entries)
			{
				BrushesTftdBattle.Add(new SolidBrush(color));
			}
		}
		#endregion Methods (static)


		#region Methods (override)
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
			return (pal != null && ColorTable.Equals(pal.ColorTable));
		}

		public override int GetHashCode()
		{
			return ColorTable.GetHashCode(); // FIX: "Non-readonly field referenced in GetHashCode()."
		}
		#endregion Methods (override)
	}
}
