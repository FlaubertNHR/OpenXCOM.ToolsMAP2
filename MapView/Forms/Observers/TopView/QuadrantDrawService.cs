using System;
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
			_border0        .Dispose();
			_border1        .Dispose();
			_border2        .Dispose();
			_border3        .Dispose();
			_border5        .Dispose();

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

		private static readonly GraphicsPath _border0 = new GraphicsPath();
		private static readonly GraphicsPath _border1 = new GraphicsPath();
		private static readonly GraphicsPath _border2 = new GraphicsPath();
		private static readonly GraphicsPath _border3 = new GraphicsPath();
		private static readonly GraphicsPath _border5 = new GraphicsPath();

		/// <summary>
		/// The ID for the Eraser sprite in the MonotoneSpriteset.
		/// </summary>
		internal const int Quad_ERASER = 0; // cf. Tilepart.Quad_* ->
		/// <summary>
		/// The ID for the CurrentPart 'slot'.
		/// </summary>
		internal const int Quad_PART = 5;

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
		/// The currently selected <c><see cref="Tilepart"/></c> in
		/// <c><see cref="TileView"/></c>.
		/// </summary>
		internal static Tilepart SelectedTilepart
		{ get; set; }
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Sets <c>GraphicsPaths</c> for each quadrant-slot outline.
		/// </summary>
		internal static void SetQuadrantPaths()
		{
			Point p0,p1,p2,p3,p4;

			GraphicsPath path;
			for (int quadrant = 0; quadrant != MapTile.QUADS; ++quadrant) // cache each quadrant's rectangular bounding path
			{
				p0 = new Point(
							StartX + Quadwidth * quadrant - 1,
							StartY);
				p1 = new Point(
							StartX + Quadwidth * quadrant + Spriteset.SpriteWidth32  + 1,
							StartY);
				p2 = new Point(
							StartX + Quadwidth * quadrant + Spriteset.SpriteWidth32  + 1,
							StartY +                        Spriteset.SpriteHeight40 + 1);
				p3 = new Point(
							StartX + Quadwidth * quadrant,
							StartY +                        Spriteset.SpriteHeight40 + 1);
				p4 = new Point(
							StartX + Quadwidth * quadrant,
							StartY);

				switch (quadrant)
				{
					default: path = _border0; break; // PartType.Floor // case 0:
					case 1:  path = _border1; break; // PartType.West
					case 2:  path = _border2; break; // PartType.North
					case 3:  path = _border3; break; // PartType.Content
				}

				path.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
				path.AddLine(p1, p2); // does not get drawn.
				path.AddLine(p2, p3); // NOTE: try DrawRectangle() it's even worse.
				path.AddLine(p3, p4); // NOTE: It's due to PixelOffsetMode ...
			}

			// skip a space between the Content quadrant-slot and the Current quadrant-slot

			p0 = new Point(
						StartX + Quadwidth * Quad_PART - 1,
						StartY);
			p1 = new Point(
						StartX + Quadwidth * Quad_PART + Spriteset.SpriteWidth32  + 1,
						StartY);
			p2 = new Point(
						StartX + Quadwidth * Quad_PART + Spriteset.SpriteWidth32  + 1,
						StartY +                         Spriteset.SpriteHeight40 + 1);
			p3 = new Point(
						StartX + Quadwidth * Quad_PART,
						StartY +                         Spriteset.SpriteHeight40 + 1);
			p4 = new Point(
						StartX + Quadwidth * Quad_PART,
						StartY);

			_border5.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
			_border5.AddLine(p1, p2); // does not get drawn.
			_border5.AddLine(p2, p3); // NOTE: try DrawRectangle() it's even worse.
			_border5.AddLine(p3, p4); // NOTE: It's due to PixelOffsetMode ...
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
			_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

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
		internal static void Paint(MapTile tile)
		{
			//DSShared.Logfile.Log("QuadrantDrawService.Paint()");
			//DSShared.Logfile.Log(". tile= " + tile);
			//DSShared.Logfile.Log(". partType= " + partType);

			// fill the background of the quadrant-slots
			Brush brush;

			if (TopViewControl.it_Floor.Checked)                           brush = QuadrantDisabled;
			else if (QuadrantControl.SelectedQuadrant == PartType.Floor)   brush = QuadrantSelected;
			else                                                           brush = QuadrantStandard;
			_graphics.FillPath(brush, _border0);

			if (TopViewControl.it_West.Checked)                            brush = QuadrantDisabled;
			else if (QuadrantControl.SelectedQuadrant == PartType.West)    brush = QuadrantSelected;
			else                                                           brush = QuadrantStandard;
			_graphics.FillPath(brush, _border1);

			if (TopViewControl.it_North.Checked)                           brush = QuadrantDisabled;
			else if (QuadrantControl.SelectedQuadrant == PartType.North)   brush = QuadrantSelected;
			else                                                           brush = QuadrantStandard;
			_graphics.FillPath(brush, _border2);

			if (TopViewControl.it_Content.Checked)                         brush = QuadrantDisabled;
			else if (QuadrantControl.SelectedQuadrant == PartType.Content) brush = QuadrantSelected;
			else                                                           brush = QuadrantStandard;
			_graphics.FillPath(brush, _border3);

			_graphics.FillPath(QuadrantStandard, _border5);


			// draw the sprites
			int phase = MainViewUnderlay.Phase;
			Tilepart part;

			// Floor ->
			if (tile != null && (part = tile.Floor) != null)
			{
				//DSShared.Logfile.Log(". . Floor");
				DrawSprite(part[phase], 0, part.Record.SpriteOffset);
				if (part.IsDoor) DrawDoorString(0); // (int)PartType.Floor
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[Tilepart.Quad_FLOOR], 0);

			// West ->
			if (tile != null && (part = tile.West) != null)
			{
				//DSShared.Logfile.Log(". . West");
				DrawSprite(part[phase], Quadwidth, part.Record.SpriteOffset);
				if (part.IsDoor) DrawDoorString(1); // (int)PartType.West
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[Tilepart.Quad_WEST], Quadwidth);

			// North ->
			if (tile != null && (part = tile.North) != null)
			{
				//DSShared.Logfile.Log(". . North");
				DrawSprite(part[phase], Quadwidth * (int)PartType.North, part.Record.SpriteOffset);
				if (part.IsDoor) DrawDoorString(2); // (int)PartType.North
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[Tilepart.Quad_NORTH], Quadwidth * 2);

			// Content ->
			if (tile != null && (part = tile.Content) != null)
			{
				//DSShared.Logfile.Log(". . Content");
				DrawSprite(part[phase], Quadwidth * (int)PartType.Content, part.Record.SpriteOffset);
				if (part.IsDoor) DrawDoorString(3); // (int)PartType.Content
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[Tilepart.Quad_CONTENT], Quadwidth * 3);

			// Current ->
			if (SelectedTilepart != null)
			{
				//DSShared.Logfile.Log(". . Current");
				DrawSprite(SelectedTilepart[phase], Quadwidth * Quad_PART, SelectedTilepart.Record.SpriteOffset);
				if (SelectedTilepart.IsDoor) DrawDoorString(5); // Quad_PART
			}
			else
				DrawSprite(MainViewF.MonotoneSprites[Quad_ERASER], Quadwidth * 5);


			// draw each quadrant's bounding rectangle
			_graphics.DrawPath(QuadrantBorder, _border0);
			_graphics.DrawPath(QuadrantBorder, _border1);
			_graphics.DrawPath(QuadrantBorder, _border2);
			_graphics.DrawPath(QuadrantBorder, _border3);
			_graphics.DrawPath(QuadrantBorder, _border5);

			// draw the quadrant-type label under each quadrant
			DrawTypeString(Floor,   TextWidth_floor,   0); // (int)PartType.Floor
			DrawTypeString(West,    TextWidth_west,    1); // (int)PartType.West
			DrawTypeString(North,   TextWidth_north,   2); // (int)PartType.North
			DrawTypeString(Content, TextWidth_content, 3); // (int)PartType.Content
			DrawTypeString(Part,    TextWidth_part,    5); // Quad_PART

			FillSwatchColor(TopControl.TopBrushes[TopViewOptionables.str_FloorColor],   0); // PartType.Floor
			FillSwatchColor(TopControl.ToolWest .Brush,                                 1); // PartType.West
			FillSwatchColor(TopControl.ToolNorth.Brush,                                 2); // PartType.North
			FillSwatchColor(TopControl.TopBrushes[TopViewOptionables.str_ContentColor], 3); // PartType.Content

			if (SelectedTilepart != null)
			{
				switch (SelectedTilepart.Record.PartType)
				{
					default:               brush = TopControl.TopBrushes[TopViewOptionables.str_FloorColor];   break; // PartType.Floor
					case PartType.West:    brush = TopControl.ToolWest .Brush;                                 break;
					case PartType.North:   brush = TopControl.ToolNorth.Brush;                                 break;
					case PartType.Content: brush = TopControl.TopBrushes[TopViewOptionables.str_ContentColor]; break;
				}
				FillSwatchColor(brush, 5);
			}
		}

		/// <summary>
		/// Draws a terrain-sprite with an x/y-offset.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="offset_x"></param>
		/// <param name="offset_y"></param>
		private static void DrawSprite(XCImage sprite, int offset_x, int offset_y)
		{
			//DSShared.Logfile.Log("QuadrantDrawService.DrawSprite()");
			//DSShared.Logfile.Log(". sprite= " + sprite);

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
				// 'b' can be null when dismissing a SaveAlert to load or reload
				// a tileset; I haven't gotten to the bottom of it (this
				// shouldn't happen) so just put a null-check here ->
				//
				// It could be .NET trying to redraw the QuadrantPanel for the
				// tileset that has just been removed instead of only drawing
				// the panel for the newly loaded tileset - see
				// 'MainViewF.Dontdrawyougits'.
				//
				if (b != null)
				{
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
		/// <param name="quadrant"></param>
		private static void DrawDoorString(int quadrant)
		{
			_graphics.DrawString(
							Door,
							QuadrantFont,
							Brushes.Black,
							StartX + (Spriteset.SpriteWidth32 - TextWidth_door) / 2 + Quadwidth * quadrant + 1,
							StartY +  Spriteset.SpriteHeight40 - QuadrantFont.Height + PrintOffsetY);
		}

		/// <summary>
		/// Draws the type of quadrant under its rectangle.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="width"></param>
		/// <param name="quadrant"></param>
		private static void DrawTypeString(string type, int width, int quadrant)
		{
			_graphics.DrawString(
							type,
							QuadrantFont,
							SelectedBrush,
							StartX + (Spriteset.SpriteWidth32 - width) / 2 + Quadwidth * quadrant + 1,
							StartY +  Spriteset.SpriteHeight40 + MarginVert);
		}

		/// <summary>
		/// Fills the swatch under a given quadrant.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="quadrant"></param>
		private static void FillSwatchColor(Brush brush, int quadrant)
		{
			_graphics.FillRectangle(
								brush,
								new RectangleF(
											StartX + Quadwidth * quadrant,
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
			if (StartX + Quadwidth * (Quad_PART + 1) - MarginHori + w < panelwidth)
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
