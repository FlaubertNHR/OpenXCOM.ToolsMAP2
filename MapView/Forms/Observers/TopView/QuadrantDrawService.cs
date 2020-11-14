using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	internal static class QuadrantDrawService
//		:
//			IDisposable
	{
		#region Fields (static)
		private const int MarginHori = 10;
		private const int MarginVert =  3;

		internal const int Quadwidth = XCImage.SpriteWidth32 + MarginHori;

		internal const int StartX = 26;
		private  const int StartY =  3;

		private const int SwatchHeight = 5;

		// NOTE: keep the door-string and its placement consistent with
		// TilePanel.OnPaint().
		private const int PrintOffsetY = 2;

		private  const  string Door    = "door";
		internal static string Floor   = "floor";
		internal static string West    = "west";
		internal static string North   = "north";
		internal static string Content = "content";
		private  static string Part    = "part";

		private static int TextWidth_door;
		private static int TextWidth_floor;
		private static int TextWidth_west;
		private static int TextWidth_north;
		private static int TextWidth_content;
		private static int TextWidth_part;

		private static readonly GraphicsPath _pathFloor   = new GraphicsPath();
		private static readonly GraphicsPath _pathWest    = new GraphicsPath();
		private static readonly GraphicsPath _pathNorth   = new GraphicsPath();
		private static readonly GraphicsPath _pathContent = new GraphicsPath();
		private static readonly GraphicsPath _pathPart    = new GraphicsPath();

		internal const int MonoTONE_ERASER  = 0;
		private  const int MonoTONE_WEST    = 1;
		private  const int MonoTONE_NORTH   = 2;
		private  const int MonoTONE_FLOOR   = 3;
		private  const int MonoTONE_CONTENT = 4;
		internal const int QuadrantPart     = 5;

		private static TopView TopViewControl;
		#endregion Fields (static)


		#region Properties (static)
		private static Font Font
		{ get; set; }

		internal static SolidBrush Brush
		{ get; set; }

		private static Font FontLocation
		{ get; set; }

		private static SolidBrush BrushLocation
		{ get; set; }


		internal static Tilepart CurrentTilepart
		{ get; set; }
		#endregion Properties (static)


		#region cTor (static)
		/// <summary>
		/// cTor.
		/// </summary>
		static QuadrantDrawService()
		{
			// NOTE: Fonts and Brushes are not disposed; they last the lifetime
			// of the app.
			Font  = new Font("Comic Sans MS", 7);
			Brush = new SolidBrush(Color.LightBlue);

			FontLocation  = new Font("Verdana", 7F, FontStyle.Bold);
			BrushLocation = new SolidBrush(SystemColors.ControlText);

			GraphicsPath path;
			Point p0, p1, p2, p3, p4;

			int quad = 0;
			for (; quad != MapTile.QUADS; ++quad) // cache each quadrant's rectangular bounding path
			{
				p0 = new Point(
							StartX + Quadwidth * quad - 1,
							StartY);
				p1 = new Point(
							StartX + Quadwidth * quad + XCImage.SpriteWidth32 + 1,
							StartY);
				p2 = new Point(
							StartX + Quadwidth * quad + XCImage.SpriteWidth32 + 1,
							StartY + XCImage.SpriteHeight40 + 1);
				p3 = new Point(
							StartX + Quadwidth * quad,
							StartY + XCImage.SpriteHeight40 + 1);
				p4 = new Point(
							StartX + Quadwidth * quad,
							StartY);

				switch (quad)
				{
					default: path = _pathFloor;   break; // case 0
					case  1: path = _pathWest;    break;
					case  2: path = _pathNorth;   break;
					case  3: path = _pathContent; break;
				}

				path.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
				path.AddLine(p1, p2); // does not get drawn.
				path.AddLine(p2, p3);
				path.AddLine(p3, p4); // NOTE: try DrawRectangle() it's even worse.
			}

			// skip a space between the Content quadslot and the Current quadslot
			p0 = new Point(
						StartX + Quadwidth * QuadrantPart - 1,
						StartY);
			p1 = new Point(
						StartX + Quadwidth * QuadrantPart + XCImage.SpriteWidth32 + 1,
						StartY);
			p2 = new Point(
						StartX + Quadwidth * QuadrantPart + XCImage.SpriteWidth32 + 1,
						StartY + XCImage.SpriteHeight40 + 1);
			p3 = new Point(
						StartX + Quadwidth * QuadrantPart,
						StartY + XCImage.SpriteHeight40 + 1);
			p4 = new Point(
						StartX + Quadwidth * QuadrantPart,
						StartY);

			_pathPart.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
			_pathPart.AddLine(p1, p2); // does not get drawn.
			_pathPart.AddLine(p2, p3);
			_pathPart.AddLine(p3, p4); // NOTE: try DrawRectangle() it's even worse.
		}
		#endregion cTor (static)


		#region punc (static)
		private static Random _rnd = new Random();

		/// <summary>
		/// Changes each character of the four quadrant strings to uppercase or
		/// lowercase randomly.
		/// </summary>
		internal static void Punkstrings()
		{
			Floor   = Punkstring(Floor);
			West    = Punkstring(West);
			North   = Punkstring(North);
			Content = Punkstring(Content);
			Part    = Punkstring(Part);
		}

		/// <summary>
		/// Helper for Punkstrings().
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private static string Punkstring(string input)
		{
			string output = String.Empty;
			foreach (var letter in input)
			{
				if (_rnd.Next() % 2 != 0)
					output += Char.ToUpper(Convert.ToChar(letter)).ToString();
				else
					output += letter;
			}
			return output;
		}
		#endregion punc (static)


		#region Methods (static)
		private static Graphics _graphics;
		private static bool _inited;

		private static ImageAttributes _attribs = new ImageAttributes();
		private static List<Brush>     _brushes = new List<Brush>();

		/// <summary>
		/// Sets the graphics object.
		/// </summary>
		/// <param name="graphics"></param>
		internal static void SetGraphics(Graphics graphics)
		{
			_graphics = graphics;
		}

		/// <summary>
		/// Draws the QuadrantPanel incl/ sprites.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="selectedQuadrant"></param>
		internal static void Draw(
				MapTile tile,
				QuadrantType selectedQuadrant)
		{
			if (!MainViewF.Optionables.UseMono && MainViewF.Optionables.SpriteShadeEnabled)
			{
				_attribs.SetGamma(MainViewF.Optionables.SpriteShadeFloat, ColorAdjustType.Bitmap);
			}

			if (!_inited) // TODO: break that out ->
			{
				_inited = true;

				TopViewControl = ObserverManager.TopView.Control;

				TextWidth_door    = (int)_graphics.MeasureString(Door,    Font).Width;
				TextWidth_floor   = (int)_graphics.MeasureString(Floor,   Font).Width;
				TextWidth_west    = (int)_graphics.MeasureString(West,    Font).Width;
				TextWidth_north   = (int)_graphics.MeasureString(North,   Font).Width;
				TextWidth_content = (int)_graphics.MeasureString(Content, Font).Width;
				TextWidth_part    = (int)_graphics.MeasureString(Part,    Font).Width;
			}

			// fill the background of the selected quadrant type
			switch (selectedQuadrant)
			{
				case QuadrantType.Floor:
					if (TopViewControl.Floor.Checked)
						_graphics.FillPath(Brush, _pathFloor);
					break;

				case QuadrantType.West:
					if (TopViewControl.West.Checked)
						_graphics.FillPath(Brush, _pathWest);
					break;

				case QuadrantType.North:
					if (TopViewControl.North.Checked)
						_graphics.FillPath(Brush, _pathNorth);
					break;

				case QuadrantType.Content:
					if (TopViewControl.Content.Checked)
						_graphics.FillPath(Brush, _pathContent);
					break;
			}

			// fill the background of !Visible quads incl/ the selected-quad
			if (!TopViewControl.Floor.Checked)
				_graphics.FillPath(Brushes.DarkGray, _pathFloor);

			if (!TopViewControl.West.Checked)
				_graphics.FillPath(Brushes.DarkGray, _pathWest);

			if (!TopViewControl.North.Checked)
				_graphics.FillPath(Brushes.DarkGray, _pathNorth);

			if (!TopViewControl.Content.Checked)
				_graphics.FillPath(Brushes.DarkGray, _pathContent);


			// draw the Sprites
			int anistep = MainViewUnderlay.AniStep;

			if (MainViewF.Optionables.UseMono)
			{
				if (MainViewUnderlay.that.MapBase.Descriptor.GroupType == GameType.Tftd)
					_brushes = Palette.BrushesTftdBattle;
				else
					_brushes = Palette.BrushesUfoBattle;
			}


			// Floor ->
			if (tile != null && tile.Floor != null)
			{
				McdRecord record = tile.Floor.Record;
				DrawSprite(tile.Floor[anistep], 0, record.TileOffset);

				if (record.HingedDoor || record.SlidingDoor)
					DrawDoorString((int)QuadrantType.Floor);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_FLOOR], 0);


			// West ->
			if (tile != null && tile.West != null)
			{
				McdRecord record = tile.West.Record;
				DrawSprite(tile.West[anistep], Quadwidth, record.TileOffset);

				if (record.HingedDoor || record.SlidingDoor)
					DrawDoorString((int)QuadrantType.West);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_WEST], Quadwidth);


			// North ->
			if (tile != null && tile.North != null)
			{
				McdRecord record = tile.North.Record;
				DrawSprite(tile.North[anistep], Quadwidth * (int)QuadrantType.North, record.TileOffset);

				if (record.HingedDoor || record.SlidingDoor)
					DrawDoorString((int)QuadrantType.North);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_NORTH], Quadwidth * (int)QuadrantType.North);


			// Content ->
			if (tile != null && tile.Content != null)
			{
				McdRecord record = tile.Content.Record;
				DrawSprite(tile.Content[anistep], Quadwidth * (int)QuadrantType.Content, record.TileOffset);

				if (record.HingedDoor || record.SlidingDoor)
					DrawDoorString((int)QuadrantType.Content);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_CONTENT], Quadwidth * (int)QuadrantType.Content);


			// Current ->
			if (CurrentTilepart != null)
			{
				McdRecord record = CurrentTilepart.Record;
				DrawSprite(CurrentTilepart[anistep], Quadwidth * QuadrantPart, record.TileOffset);

				if (record.HingedDoor || record.SlidingDoor)
					DrawDoorString(QuadrantPart);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_ERASER], Quadwidth * QuadrantPart);


			// draw each quadrant's bounding rectangle
			_graphics.DrawPath(Pens.Black, _pathFloor);
			_graphics.DrawPath(Pens.Black, _pathWest);
			_graphics.DrawPath(Pens.Black, _pathNorth);
			_graphics.DrawPath(Pens.Black, _pathContent);
			_graphics.DrawPath(Pens.Black, _pathPart);

			// draw the quad-type label under each quadrant
			DrawTypeString(Floor,   TextWidth_floor,   (int)QuadrantType.Floor);
			DrawTypeString(West,    TextWidth_west,    (int)QuadrantType.West);
			DrawTypeString(North,   TextWidth_north,   (int)QuadrantType.North);
			DrawTypeString(Content, TextWidth_content, (int)QuadrantType.Content);
			DrawTypeString(Part,    TextWidth_part,    QuadrantPart);

			// fill the color-swatch under each quadrant-label
			FillSwatchColor(               TopPanel.Brushes[TopViewOptionables.str_FloorColor],        (int)QuadrantType.Floor);
			FillSwatchColor(new SolidBrush(TopPanel.Pens   [TopViewOptionables.str_WestColor] .Color), (int)QuadrantType.West);
			FillSwatchColor(new SolidBrush(TopPanel.Pens   [TopViewOptionables.str_NorthColor].Color), (int)QuadrantType.North);
			FillSwatchColor(               TopPanel.Brushes[TopViewOptionables.str_ContentColor],      (int)QuadrantType.Content);
		}

		/// <summary>
		/// Draws a terrain-sprite with an x/y-offset.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="offset_x"></param>
		/// <param name="offset_y"></param>
		private static void DrawSprite(XCImage sprite, int offset_x, int offset_y)
		{
			if (MainViewF.Optionables.UseMono)
			{
				byte[] bindata = sprite.Bindata;

				int palid;
				int i = -1;
				for (int y = 0; y != XCImage.SpriteHeight40; ++y)
				for (int x = 0; x != XCImage.SpriteWidth32;  ++x)
				{
					if ((palid = bindata[++i]) != Palette.TranId)
					{
						_graphics.FillRectangle(
											_brushes[palid],
											x + StartX + offset_x,
											y + StartY - offset_y,
											1,1);
					}
				}
			}
			else
			{
				Bitmap b = sprite.Sprite;
				_graphics.DrawImage(
								b,
								new Rectangle(
											StartX + offset_x,
											StartY - offset_y,
											b.Width,
											b.Height),
								0,0, b.Width, b.Height,
								GraphicsUnit.Pixel,
								_attribs);
			}
		}

		/// <summary>
		/// Draws a monotone-sprite with an x-offset.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="offset_x"></param>
		private static void DrawSprite(XCImage sprite, int offset_x)
		{
			if (MainViewF.Optionables.UseMono)
			{
				byte[] bindata = sprite.Bindata;

				int palid;
				int i = -1;
				for (int y = 0; y != XCImage.SpriteHeight40; ++y)
				for (int x = 0; x != XCImage.SpriteWidth32;  ++x)
				{
					if ((palid = bindata[++i]) != Palette.TranId)
					{
						_graphics.FillRectangle(
											_brushes[palid],
											x + StartX + offset_x,
											y + StartY,
											1,1);
					}
				}
			}
			else
				_graphics.DrawImage(
								sprite.Sprite,
								StartX + offset_x,
								StartY);
		}

		/// <summary>
		/// Draws the "door" string if applicable.
		/// </summary>
		/// <param name="quad"></param>
		private static void DrawDoorString(int quad)
		{
			_graphics.DrawString(
							Door,
							Font,
							Brushes.Black,
							StartX + (XCImage.SpriteWidth32 - TextWidth_door) / 2 + Quadwidth * quad + 1,
							StartY +  XCImage.SpriteHeight40 - Font.Height + PrintOffsetY);
		}

		/// <summary>
		/// Draws the type of quadrant under its rectangle.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="width"></param>
		/// <param name="quad"></param>
		private static void DrawTypeString(string type, int width, int quad)
		{
			_graphics.DrawString(
							type,
							Font,
							Brushes.Black,
							StartX + (XCImage.SpriteWidth32 - width) / 2 + Quadwidth * quad + 1,
							StartY +  XCImage.SpriteHeight40 + MarginVert);
		}

		/// <summary>
		/// Fills the swatch under a given quadrant.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="quad"></param>
		private static void FillSwatchColor(Brush brush, int quad)
		{
			_graphics.FillRectangle(
								brush,
								new RectangleF(
											StartX + Quadwidth * quad,
											StartY + XCImage.SpriteHeight40 + MarginVert + Font.Height + 1,
											XCImage.SpriteWidth32,
											SwatchHeight));
		}

		/// <summary>
		/// Prints the currently selected tile's location.
		/// </summary>
		/// <param name="panelwidth"></param>
		/// <param name="loc"></param>
		internal static void PrintLocationString(int panelwidth, MapLocation loc)
		{
			var file = ObserverManager.TopView.Control.TopPanel.MapBase as MapFile;

			int c = loc.Col;
			int r = loc.Row;
			int l = file.MapSize.Levs - file.Level;

			if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
			if (!MainViewF.Optionables.Base1_z) { --l; }

			string location = String.Format(
										CultureInfo.InvariantCulture,
										"c {0}  r {1}  L {2}",
										c,r,l);

			int w = TextRenderer.MeasureText(location, FontLocation).Width;
			if (StartX + Quadwidth * (QuadrantPart + 1) - MarginHori + w < panelwidth)
			{
				_graphics.DrawString(
								location,
								FontLocation,
								BrushLocation,
								panelwidth - w, StartY);
			}
		}


/*		/// <summary>
		/// This isn't really necessary since the GraphicsPaths last the
		/// lifetime of the app. But FxCop gets antsy ....
		/// NOTE: Dispose() is never called. cf ColorTool. cf DrawBlobService.
		/// WARNING: This is NOT a robust implementation perhaps. But it
		/// satisifes the core of the matter and could likely be used for
		/// further development if that's ever required.
		/// </summary>
		public static void Dispose()
		{
			if (_pathFloor   != null) _pathFloor  .Dispose();
			if (_pathWest    != null) _pathWest   .Dispose();
			if (_pathNorth   != null) _pathNorth  .Dispose();
			if (_pathContent != null) _pathContent.Dispose();

			if (Font  != null) Font .Dispose();
			if (Brush != null) Brush.Dispose();

			GC.SuppressFinalize(this);
		} */
		#endregion Methods (static)
	}
}
