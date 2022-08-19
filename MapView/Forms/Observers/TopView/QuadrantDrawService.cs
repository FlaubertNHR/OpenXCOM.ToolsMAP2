using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	internal static class QuadrantDrawService
	{
		/// <summary>
		/// Disposal isn't necessary since the GraphicsPaths last the lifetime
		/// of the app. But FxCop ca1001 gets antsy ....
		/// </summary>
		internal static void DisposeService()
		{
			//DSShared.Logfile.Log("QuadrantDrawService.DisposeService() static");
			_pathFloor      .Dispose();
			_pathWest       .Dispose();
			_pathNorth      .Dispose();
			_pathContent    .Dispose();
			_pathPart       .Dispose();

			QuadrantFont    .Dispose();
			LocationFont    .Dispose();
			SelectorBrush   .Dispose();
			SelectedBrush   .Dispose();
			QuadrantStandard.Dispose();
			QuadrantSelected.Dispose();
			QuadrantDisabled.Dispose();
			QuadrantBorder  .Dispose();
		}


		#region Fields (static)
		private const int MarginHori = 10;
		private const int MarginVert =  3;

		internal const int Quadwidth = Spriteset.SpriteWidth32 + MarginHori;

		internal const int StartX = 26;
		private  const int StartY =  3;

		private const int SwatchHeight = 5;

		// NOTE: keep the door-string and its placement consistent with
		// TilePanel.OnPaint().
		private const int PrintOffsetY = 2;

		private static Random _rand = new Random();

		private  const  string Door    = "door";
		internal static string Floor   = Punkstring("floor");
		internal static string West    = Punkstring("west");
		internal static string North   = Punkstring("north");
		internal static string Content = Punkstring("content");
		private  static string Part    = Punkstring("part");

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

		internal const int MonoTONE_ERASER  = 0; // cf. Tilepart.MonoTONE_* ->
		private  const int MonoTONE_WEST    = 1;
		private  const int MonoTONE_NORTH   = 2;
		private  const int MonoTONE_FLOOR   = 3;
		private  const int MonoTONE_CONTENT = 4;
		internal const int QuadrantPart     = 5;

		private static TopView TopViewControl;

		private static Graphics _graphics;
		private static bool _init;
		#endregion Fields (static)


		#region Properties (static)
		private static readonly Font QuadrantFont = new Font("Comic Sans MS", 7F);
		private static readonly Font LocationFont = new Font("Verdana", 7F, FontStyle.Bold);

		internal static readonly SolidBrush SelectorBrush    = new SolidBrush(TopViewOptionables.def_PanelForecolor);
		internal static readonly SolidBrush SelectedBrush    = new SolidBrush(TopViewOptionables.def_QuadrantForecolor);
		internal static readonly SolidBrush QuadrantStandard = new SolidBrush(TopViewOptionables.def_QuadrantColor);
		internal static readonly SolidBrush QuadrantSelected = new SolidBrush(TopViewOptionables.def_QuadrantSelected);
		internal static readonly SolidBrush QuadrantDisabled = new SolidBrush(TopViewOptionables.def_QuadrantDisabled);

		internal static readonly Pen QuadrantBorder = new Pen(TopViewOptionables.def_QuadrantBorder);


		/// <summary>
		/// The currently selected tilepart in <c><see cref="TileView"/></c>.
		/// </summary>
		internal static Tilepart SelectedTilepart
		{ get; set; }
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Initializes the graphics paths of each quadrant-slot outline.
		/// </summary>
		internal static void CacheQuadrantPaths()
		{
			Point p0,p1,p2,p3,p4;

			GraphicsPath path;
			for (int quad = 0; quad != MapTile.QUADS; ++quad) // cache each quadrant's rectangular bounding path
			{
				p0 = new Point(
							StartX + Quadwidth * quad - 1,
							StartY);
				p1 = new Point(
							StartX + Quadwidth * quad + Spriteset.SpriteWidth32 + 1,
							StartY);
				p2 = new Point(
							StartX + Quadwidth * quad + Spriteset.SpriteWidth32 + 1,
							StartY + Spriteset.SpriteHeight40 + 1);
				p3 = new Point(
							StartX + Quadwidth * quad,
							StartY + Spriteset.SpriteHeight40 + 1);
				p4 = new Point(
							StartX + Quadwidth * quad,
							StartY);

				switch ((PartType)quad)
				{
					default:               path = _pathFloor;   break; // PartType.Floor
					case PartType.West:    path = _pathWest;    break;
					case PartType.North:   path = _pathNorth;   break;
					case PartType.Content: path = _pathContent; break;
				}

				path.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
				path.AddLine(p1, p2); // does not get drawn.
				path.AddLine(p2, p3); // NOTE: try DrawRectangle() it's even worse.
				path.AddLine(p3, p4); // NOTE: It's due to PixelOffsetMode ...
			}

			// skip a space between the Content quadslot and the Current quadslot
			p0 = new Point(
						StartX + Quadwidth * QuadrantPart - 1,
						StartY);
			p1 = new Point(
						StartX + Quadwidth * QuadrantPart + Spriteset.SpriteWidth32 + 1,
						StartY);
			p2 = new Point(
						StartX + Quadwidth * QuadrantPart + Spriteset.SpriteWidth32 + 1,
						StartY + Spriteset.SpriteHeight40 + 1);
			p3 = new Point(
						StartX + Quadwidth * QuadrantPart,
						StartY + Spriteset.SpriteHeight40 + 1);
			p4 = new Point(
						StartX + Quadwidth * QuadrantPart,
						StartY);

			_pathPart.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
			_pathPart.AddLine(p1, p2); // does not get drawn.
			_pathPart.AddLine(p2, p3); // NOTE: try DrawRectangle() it's even worse.
			_pathPart.AddLine(p3, p4); // NOTE: It's due to PixelOffsetMode ...
		}


		/// <summary>
		/// Changes each character of the quadrant strings to uppercase or
		/// lowercase randomly.
		/// </summary>
		/// <param name="in"></param>
		/// <returns></returns>
		private static string Punkstring(string @in)
		{
			var sb = new StringBuilder();

			for (int i = 0; i != @in.Length; ++i)
			{
				if (_rand.Next() % 2 != 0)
					sb.Append((char)(@in[i] - 32)); // uc
				else
					sb.Append((char)@in[i]);
			}
			return sb.ToString();
		}


		/// <summary>
		/// Sets <c><see cref="TopViewControl"/></c>.
		/// </summary>
		internal static void SetTopViewControl(TopView control)
		{
			TopViewControl = control;
		}


		/// <summary>
		/// Sets the graphics object and stuff.
		/// </summary>
		/// <param name="graphics"></param>
		internal static void SetGraphics(Graphics graphics)
		{
			_graphics = graphics;

			if (!_init)
			{
				_init = true;

				TextWidth_door    = (int)_graphics.MeasureString(Door,    QuadrantFont).Width;
				TextWidth_floor   = (int)_graphics.MeasureString(Floor,   QuadrantFont).Width;
				TextWidth_west    = (int)_graphics.MeasureString(West,    QuadrantFont).Width;
				TextWidth_north   = (int)_graphics.MeasureString(North,   QuadrantFont).Width;
				TextWidth_content = (int)_graphics.MeasureString(Content, QuadrantFont).Width;
				TextWidth_part    = (int)_graphics.MeasureString(Part,    QuadrantFont).Width;
			}
		}

		/// <summary>
		/// Draws a <c><see cref="QuadrantControl"/></c> incl/ sprites.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="partType"></param>
		internal static void Draw(
				MapTile tile,
				PartType partType)
		{
			if (MainViewF.Dontdrawyougits) return;

			// fill the background of the quad-slots
			Brush brush;

			if (TopViewControl.it_Floor.Checked)   brush = QuadrantDisabled;
			else if (partType == PartType.Floor)   brush = QuadrantSelected;
			else                                   brush = QuadrantStandard;
			_graphics.FillPath(brush, _pathFloor);

			if (TopViewControl.it_West.Checked)    brush = QuadrantDisabled;
			else if (partType == PartType.West)    brush = QuadrantSelected;
			else                                   brush = QuadrantStandard;
			_graphics.FillPath(brush, _pathWest);

			if (TopViewControl.it_North.Checked)   brush = QuadrantDisabled;
			else if (partType == PartType.North)   brush = QuadrantSelected;
			else                                   brush = QuadrantStandard;
			_graphics.FillPath(brush, _pathNorth);

			if (TopViewControl.it_Content.Checked) brush = QuadrantDisabled;
			else if (partType == PartType.Content) brush = QuadrantSelected;
			else                                   brush = QuadrantStandard;
			_graphics.FillPath(brush, _pathContent);

			_graphics.FillPath(QuadrantStandard, _pathPart);


			// draw the sprites
			int phase = MainViewUnderlay.Phase;
			Tilepart part;

			// Floor ->
			if (tile != null && (part = tile.Floor) != null)
			{
				DrawSprite(part[phase], 0, part.Record.TileOffset);
				if (part.isDoor) DrawDoorString((int)PartType.Floor);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_FLOOR], 0);

			// West ->
			if (tile != null && (part = tile.West) != null)
			{
				DrawSprite(part[phase], Quadwidth, part.Record.TileOffset);
				if (part.isDoor) DrawDoorString((int)PartType.West);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_WEST], Quadwidth);

			// North ->
			if (tile != null && (part = tile.North) != null)
			{
				DrawSprite(part[phase], Quadwidth * (int)PartType.North, part.Record.TileOffset);
				if (part.isDoor) DrawDoorString((int)PartType.North);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_NORTH], Quadwidth * (int)PartType.North);

			// Content ->
			if (tile != null && (part = tile.Content) != null)
			{
				DrawSprite(part[phase], Quadwidth * (int)PartType.Content, part.Record.TileOffset);
				if (part.isDoor) DrawDoorString((int)PartType.Content);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_CONTENT], Quadwidth * (int)PartType.Content);

			// Current ->
			if (SelectedTilepart != null)
			{
				DrawSprite(SelectedTilepart[phase], Quadwidth * QuadrantPart, SelectedTilepart.Record.TileOffset);
				if (SelectedTilepart.isDoor) DrawDoorString(QuadrantPart);
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[MonoTONE_ERASER], Quadwidth * QuadrantPart);


			// draw each quadrant's bounding rectangle
			_graphics.DrawPath(QuadrantBorder, _pathFloor);
			_graphics.DrawPath(QuadrantBorder, _pathWest);
			_graphics.DrawPath(QuadrantBorder, _pathNorth);
			_graphics.DrawPath(QuadrantBorder, _pathContent);
			_graphics.DrawPath(QuadrantBorder, _pathPart);

			// draw the quad-type label under each quadrant
			DrawTypeString(Floor,   TextWidth_floor,   (int)PartType.Floor);
			DrawTypeString(West,    TextWidth_west,    (int)PartType.West);
			DrawTypeString(North,   TextWidth_north,   (int)PartType.North);
			DrawTypeString(Content, TextWidth_content, (int)PartType.Content);
			DrawTypeString(Part,    TextWidth_part,    QuadrantPart);

			FillSwatchColor(TopControl.TopBrushes[TopViewOptionables.str_FloorColor],   PartType.Floor);
			FillSwatchColor(TopControl.ToolWest .Brush,                                 PartType.West);
			FillSwatchColor(TopControl.ToolNorth.Brush,                                 PartType.North);
			FillSwatchColor(TopControl.TopBrushes[TopViewOptionables.str_ContentColor], PartType.Content);
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
				byte[] bindata = sprite.GetBindata();

				int palid;
				int i = -1;
				for (int y = 0; y != Spriteset.SpriteHeight40; ++y)
				for (int x = 0; x != Spriteset.SpriteWidth32;  ++x)
				{
					if ((palid = bindata[++i]) != Palette.Tid)
					{
						_graphics.FillRectangle(
											Palette.MonoBrushes[palid],
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
								Globals.Ia);
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
				byte[] bindata = sprite.GetBindata();

				int palid;
				int i = -1;
				for (int y = 0; y != Spriteset.SpriteHeight40; ++y)
				for (int x = 0; x != Spriteset.SpriteWidth32;  ++x)
				{
					if ((palid = bindata[++i]) != Palette.Tid)
					{
						_graphics.FillRectangle(
											Palette.MonoBrushes[palid],
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
							QuadrantFont,
							Brushes.Black,
							StartX + (Spriteset.SpriteWidth32 - TextWidth_door) / 2 + Quadwidth * quad + 1,
							StartY +  Spriteset.SpriteHeight40 - QuadrantFont.Height + PrintOffsetY);
		}

		/// <summary>
		/// Draws the type of quadrant under its rectangle.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="width"></param>
		/// <param name="slot"></param>
		private static void DrawTypeString(string type, int width, int slot)
		{
			_graphics.DrawString(
							type,
							QuadrantFont,
							SelectedBrush,
							StartX + (Spriteset.SpriteWidth32 - width) / 2 + Quadwidth * slot + 1,
							StartY +  Spriteset.SpriteHeight40 + MarginVert);
		}

		/// <summary>
		/// Fills the swatch under a given quadrant.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="slot"></param>
		private static void FillSwatchColor(Brush brush, PartType slot)
		{
			_graphics.FillRectangle(
								brush,
								new RectangleF(
											StartX + Quadwidth * (int)slot,
											StartY + Spriteset.SpriteHeight40 + MarginVert + QuadrantFont.Height + 1,
											Spriteset.SpriteWidth32,
											SwatchHeight));
		}

		/// <summary>
		/// Prints the currently selected tile's location.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="panelwidth">the width of QuadrantControl</param>
		/// <remarks>This is called by <see cref="QuadrantControl"/>.</remarks>
		internal static void PrintSelectedLocation(MapLocation location, int panelwidth)
		{
			MapFile file = MainViewF.that.MapFile;

			string loc = Globals.GetLocationString(
												location.Col,
												location.Row,
												file.Level,
												file.Levs);

			int w = TextRenderer.MeasureText(loc, LocationFont).Width;
			if (StartX + Quadwidth * (QuadrantPart + 1) - MarginHori + w < panelwidth)
			{
				_graphics.DrawString(
								loc,
								LocationFont,
								SelectedBrush,
								panelwidth - w, StartY);
			}
		}

		/// <summary>
		/// Prints the selector's current tile location.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="location"></param>
		/// <param name="panelwidth">the width of TopControl</param>
		/// <param name="panelheight">the width of TopControl</param>
		/// <param name="file"></param>
		/// <remarks>This is called by <see cref="TopControl"/>.</remarks>
		internal static void PrintSelectorLocation(
				Graphics graphics,
				Point location,
				int panelwidth,
				int panelheight,
				MapFile file)
		{
			string loc = Globals.GetLocationString(
												location.X,
												location.Y,
												file.Level,
												file.Levs);

			int x = panelwidth - TextRenderer.MeasureText(loc, LocationFont).Width;
			int y = panelheight - 20;
			graphics.DrawString(
							loc,
							LocationFont,
							SelectorBrush,
							x,y);
		}
		#endregion Methods (static)
	}
}
