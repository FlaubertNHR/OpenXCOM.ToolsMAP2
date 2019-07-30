using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using MapView.Forms.MainView;

using XCom;
using XCom.Interfaces;


namespace MapView.Forms.Observers
{
	internal static class QuadrantDrawService
//		:
//			IDisposable
	{
		#region Fields (static)
		private const int MarginHori = 5;
		private const int MarginVert = 3;

		internal const int Quadwidth = XCImage.SpriteWidth32 + MarginHori * 2;

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
		private  static string Current = "current";

		private static int TextWidth_door;
		private static int TextWidth_floor;
		private static int TextWidth_west;
		private static int TextWidth_north;
		private static int TextWidth_content;
		private static int TextWidth_current;

		private static readonly GraphicsPath _pathFloor   = new GraphicsPath();
		private static readonly GraphicsPath _pathWest    = new GraphicsPath();
		private static readonly GraphicsPath _pathNorth   = new GraphicsPath();
		private static readonly GraphicsPath _pathContent = new GraphicsPath();
		private static readonly GraphicsPath _pathCurrent = new GraphicsPath();

		internal const int QuadrantTypeCurrent = 5;

		private static TopView TopViewControl;
		#endregion Fields (static)


		#region Properties (static)
		internal static SolidBrush Brush
		{ get; set; }

		internal static Font Font
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
			Font  = new Font("Comic Sans MS", 7);
			Brush = new SolidBrush(Color.LightBlue);

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

			// skip a space between the Content quad and the Current quad
			p0 = new Point(
						StartX + Quadwidth * QuadrantTypeCurrent - 1,
						StartY);
			p1 = new Point(
						StartX + Quadwidth * QuadrantTypeCurrent + XCImage.SpriteWidth32 + 1,
						StartY);
			p2 = new Point(
						StartX + Quadwidth * QuadrantTypeCurrent + XCImage.SpriteWidth32 + 1,
						StartY + XCImage.SpriteHeight40 + 1);
			p3 = new Point(
						StartX + Quadwidth * QuadrantTypeCurrent,
						StartY + XCImage.SpriteHeight40 + 1);
			p4 = new Point(
						StartX + Quadwidth * QuadrantTypeCurrent,
						StartY);

			_pathCurrent.AddLine(p0, p1); // NOTE: 'p4' appears to be needed since the origin of 'p0'
			_pathCurrent.AddLine(p1, p2); // does not get drawn.
			_pathCurrent.AddLine(p2, p3);
			_pathCurrent.AddLine(p3, p4); // NOTE: try DrawRectangle() it's even worse.
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

			Current = Punkstring(Current);
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

		/// <summary>
		/// Draws the QuadrantPanel incl/ sprites.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="tile"></param>
		/// <param name="selectedQuadrant"></param>
		internal static void Draw(
				Graphics graphics,
				MapTile tile,
				QuadrantType selectedQuadrant)
		{
			_graphics = graphics;

			var spriteAttributes = new ImageAttributes();
			if (XCMainWindow.Optionables.SpriteShadeEnabled)
				spriteAttributes.SetGamma(XCMainWindow.Optionables.SpriteShadeFloat, ColorAdjustType.Bitmap);

			if (!_inited) // TODO: break that out ->
			{
				_inited = true;

				TopViewControl = ObserverManager.TopView.Control;

				TextWidth_door    = (int)_graphics.MeasureString(Door,    Font).Width;
				TextWidth_floor   = (int)_graphics.MeasureString(Floor,   Font).Width;
				TextWidth_west    = (int)_graphics.MeasureString(West,    Font).Width;
				TextWidth_north   = (int)_graphics.MeasureString(North,   Font).Width;
				TextWidth_content = (int)_graphics.MeasureString(Content, Font).Width;

				TextWidth_current = (int)_graphics.MeasureString(Current, Font).Width;
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
			Bitmap sprite;
			int anistep = MainViewUnderlay.AniStep;

			// Floor ->
			if (tile != null && tile.Floor != null)
			{
				sprite = tile.Floor[anistep].Sprite;
				_graphics.DrawImage(
								sprite,
								new Rectangle(
											StartX,
											StartY - tile.Floor.Record.TileOffset,
											sprite.Width,
											sprite.Height),
								0,0, sprite.Width, sprite.Height,
								GraphicsUnit.Pixel,
								spriteAttributes);

				if (tile.Floor.Record.HingedDoor || tile.Floor.Record.SlidingDoor)
					DrawDoorString((int)QuadrantType.Floor);
			}
			else
				_graphics.DrawImage(
								Globals.ExtraSprites[3].Sprite,
								StartX, StartY);

			// West ->
			if (tile != null && tile.West != null)
			{
				sprite = tile.West[anistep].Sprite;
				_graphics.DrawImage(
								sprite,
								new Rectangle(
											StartX + Quadwidth,
											StartY - tile.West.Record.TileOffset,
											sprite.Width,
											sprite.Height),
								0,0, sprite.Width, sprite.Height,
								GraphicsUnit.Pixel,
								spriteAttributes);

				if (tile.West.Record.HingedDoor || tile.West.Record.SlidingDoor)
					DrawDoorString((int)QuadrantType.West);
			}
			else
				_graphics.DrawImage(
								Globals.ExtraSprites[1].Sprite,
								StartX + Quadwidth,
								StartY);

			// North ->
			if (tile != null && tile.North != null)
			{
				sprite = tile.North[anistep].Sprite;
				_graphics.DrawImage(
								sprite,
								new Rectangle(
											StartX + Quadwidth * (int)QuadrantType.North,
											StartY - tile.North.Record.TileOffset,
											sprite.Width,
											sprite.Height),
								0,0, sprite.Width, sprite.Height,
								GraphicsUnit.Pixel,
								spriteAttributes);

				if (tile.North.Record.HingedDoor || tile.North.Record.SlidingDoor)
					DrawDoorString((int)QuadrantType.North);
			}
			else
				_graphics.DrawImage(
								Globals.ExtraSprites[2].Sprite,
								StartX + Quadwidth * (int)QuadrantType.North,
								StartY);

			// Content ->
			if (tile != null && tile.Content != null)
			{
				sprite = tile.Content[anistep].Sprite;
				_graphics.DrawImage(
								sprite,
								new Rectangle(
											StartX + Quadwidth * (int)QuadrantType.Content,
											StartY - tile.Content.Record.TileOffset,
											sprite.Width,
											sprite.Height),
								0,0, sprite.Width, sprite.Height,
								GraphicsUnit.Pixel,
								spriteAttributes);

				if (tile.Content.Record.HingedDoor || tile.Content.Record.SlidingDoor)
					DrawDoorString((int)QuadrantType.Content);
			}
			else
				_graphics.DrawImage(
								Globals.ExtraSprites[4].Sprite,
								StartX + Quadwidth * (int)QuadrantType.Content,
								StartY);

			// Current ->
			if (CurrentTilepart != null)
			{
				sprite = CurrentTilepart[anistep].Sprite;
				_graphics.DrawImage(
								sprite,
								new Rectangle(
											StartX + Quadwidth * QuadrantTypeCurrent,
											StartY - CurrentTilepart.Record.TileOffset,
											sprite.Width,
											sprite.Height),
								0,0, sprite.Width, sprite.Height,
								GraphicsUnit.Pixel,
								spriteAttributes);

				if (CurrentTilepart.Record.HingedDoor || CurrentTilepart.Record.SlidingDoor)
					DrawDoorString(QuadrantTypeCurrent);
			}
			else
				_graphics.DrawImage(
								Globals.ExtraSprites[0].Sprite,
								StartX + Quadwidth * QuadrantTypeCurrent,
								StartY);


			// draw each quadrant's bounding rectangle
			_graphics.DrawPath(Pens.Black, _pathFloor);
			_graphics.DrawPath(Pens.Black, _pathWest);
			_graphics.DrawPath(Pens.Black, _pathNorth);
			_graphics.DrawPath(Pens.Black, _pathContent);

			_graphics.DrawPath(Pens.Black, _pathCurrent);

			// draw the quad-type label under each quadrant
			DrawTypeString(Floor,   TextWidth_floor,   (int)QuadrantType.Floor);
			DrawTypeString(West,    TextWidth_west,    (int)QuadrantType.West);
			DrawTypeString(North,   TextWidth_north,   (int)QuadrantType.North);
			DrawTypeString(Content, TextWidth_content, (int)QuadrantType.Content);

			DrawTypeString(Current, TextWidth_current, QuadrantTypeCurrent);

			// fill the color-swatch under each quadrant-label
			FillSwatchColor(               TopPanel.Brushes[TopViewOptionables.str_FloorColor],        (int)QuadrantType.Floor);
			FillSwatchColor(new SolidBrush(TopPanel.Pens   [TopViewOptionables.str_WestColor] .Color), (int)QuadrantType.West);
			FillSwatchColor(new SolidBrush(TopPanel.Pens   [TopViewOptionables.str_NorthColor].Color), (int)QuadrantType.North);
			FillSwatchColor(               TopPanel.Brushes[TopViewOptionables.str_ContentColor],      (int)QuadrantType.Content);
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
