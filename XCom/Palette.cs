using System;
using System.Collections;
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

		private static readonly Hashtable _palettes = new Hashtable();

		private const string Embedded     = "XCom._Embedded.";

		private const string ufobattle    = "ufo-battle";
		private const string ufogeo       = "ufo-geo";
		private const string ufograph     = "ufo-graph";
		private const string uforesearch  = "ufo-research";

		private const string tftdbattle   = "tftd-battle";
		private const string tftdgeo      = "tftd-geo";
		private const string tftdgraph    = "tftd-graph";
		private const string tftdresearch = "tftd-research";

		private const string PalExt       = ".pal";

		/// <summary>
		/// The suffix for the label (key) of the grayscale version of the palette.
		/// </summary>
		private const string Gray         = "#gray";

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
				if (_palettes[ufobattle] == null)
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
				if (_palettes[ufogeo] == null)
					_palettes[ufogeo] = new Palette(Assembly.GetExecutingAssembly()
										   .GetManifestResourceStream(Embedded + ufogeo + PalExt));

				return _palettes[ufogeo] as Palette;
			}
		}

		public static Palette UfoGraph
		{
			get
			{
				if (_palettes[ufograph] == null)
					_palettes[ufograph] = new Palette(Assembly.GetExecutingAssembly()
											 .GetManifestResourceStream(Embedded + ufograph + PalExt));

				return _palettes[ufograph] as Palette;
			}
		}

		public static Palette UfoResearch
		{
			get
			{
				if (_palettes[uforesearch] == null)
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
				if (_palettes[tftdbattle] == null)
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
				if (_palettes[tftdgeo] == null)
					_palettes[tftdgeo] = new Palette(Assembly.GetExecutingAssembly()
											.GetManifestResourceStream(Embedded + tftdgeo + PalExt));

				return _palettes[tftdgeo] as Palette;
			}
		}

		public static Palette TftdGraph
		{
			get
			{
				if (_palettes[tftdgraph] == null)
					_palettes[tftdgraph] = new Palette(Assembly.GetExecutingAssembly()
											  .GetManifestResourceStream(Embedded + tftdgraph + PalExt));

				return _palettes[tftdgraph] as Palette;
			}
		}

		public static Palette TftdResearch
		{
			get
			{
				if( _palettes[tftdresearch] == null)
					_palettes[tftdresearch] = new Palette(Assembly.GetExecutingAssembly()
												 .GetManifestResourceStream(Embedded + tftdresearch + PalExt));

				return _palettes[tftdresearch] as Palette;
			}
		}
		#endregion Properties (static)


		#region Properties
		/// <summary>
		/// Gets/Sets the label (key) of the palette.
		/// </summary>
		public string Label
		{ get; set; }

		/// <summary>
		/// Gets/Sets the colors in the palette.
		/// </summary>
		public ColorPalette ColorTable
		{ get; private set; }

		/// <summary>
		/// Gets/Sets the color of a given index in the color-table.
		/// </summary>
		public Color this[int id]
		{
			get { return ColorTable.Entries[id]; }
			private set { ColorTable.Entries[id] = value; }
		}

		/// <summary>
		/// Gets a grayscale version of the palette.
		/// </summary>
		public Palette Grayscale
		{
			get
			{
				if (_palettes[Label + Gray] == null)
				{
					var pal = new Palette(Label + Gray);

					_palettes[pal.Label] = pal;

					Color color; int val;
					for (int id = 0; id != ColorTable.Entries.Length; ++id)
					{
						color = this[id];
						val = (int)(color.R * 0.2 + color.G * 0.5 + color.B * 0.3);
						pal[id] = Color.FromArgb(val, val, val);
					}
				}
				return _palettes[Label + Gray] as Palette;
			}
		}

//		/// <summary>
//		/// Gets the color that can be transparent.
//		/// NOTE: was used by 'CuboidSprite'.
//		/// </summary>
//		public Color Transparent
//		{ get { return ColorTable.Entries[TranId]; } }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a palette given a filestream of data.
		/// </summary>
		/// <param name="fs"></param>
		private Palette(Stream fs)
		{
			using (var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
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
			}
//			checkPalette();
		}

		/// <summary>
		/// cTor[1]. Instantiates a grayscale-palette with a given label.
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
			ColorTable.Entries[TranId] = Color.FromArgb(
													tran ? 0 : 255,
													ColorTable.Entries[TranId]);
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
