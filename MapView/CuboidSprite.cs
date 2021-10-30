using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using XCom;


namespace MapView
{
	/// <summary>
	/// Draws sprites (the cuboid and targeter) from standard resource
	/// <c>UFOGRAPH/CURSOR.PCK+TAB</c>.
	/// </summary>
	internal static class CuboidSprite
	{
		public static void DisposeCursorset()
		{
			//DSShared.Logfile.Log("CuboidSprite.DisposeCursorset() static");
			if (Ufoset != null)
			{
				Ufoset.Dispose();
				Ufoset = null;
			}

			if (Tftdset != null)
			{
				Tftdset.Dispose();
				Tftdset = null;
			}

			_ia.Dispose();
			_ia = null;

			Cursorset = null;
		}


		#region Fields (static)
		private static Graphics _graphics;
		private static ImageAttributes _ia = new ImageAttributes();

		private const int RED_BACK   = 0; // sprite-ids in CURSOR.PCK ->
		private const int RED_FRONT  = 3;
		private const int BLUE_BACK  = 2;
		private const int BLUE_FRONT = 5;
		private const int TARGETER   = 7; // 1st yellow targeter sprite

		private const int widthfactor  = 2; // the values for width and height
		private const int heightfactor = 5; // are based on a sprite that's 32x40.

		private static IList<Brush> _brushes;
		#endregion Fields (static)


		#region Properties (static)
		private static Spriteset _cursorset;
		internal static Spriteset Cursorset
		{
			get { return _cursorset; }
			private set
			{
				if ((_cursorset = value) == Tftdset)
					_brushes = Palette.BrushesTftdBattle;
				else
					_brushes = Palette.BrushesUfoBattle;
			}
		}

		internal static Spriteset Ufoset
		{ get; set; }

		internal static Spriteset Tftdset
		{ get; set; }
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Assigns the targeter-sprite(s)/cursorset of UFO or TFTD respecting
		/// <c><see cref="Forms.MainView.MainViewOptionables.PreferTftdTargeter">MainViewOptionables.PreferTftdTargeter</see></c>.
		/// </summary>
		/// <returns><c>true</c> if a valid cursorset gets assigned</returns>
		/// <remarks>Used in
		/// <c><see cref="MainViewF()">MainViewF()</see></c> for cases (1)
		/// <c>PreferTftdTargeter</c> has not been registered in
		/// "settings/MapOptions.cfg" yet or (2) user changes the
		/// <c>PreferTftdTargeter</c> option or (3) the user's
		/// <c>UFOGRAPH/CURSOR.PCK+TAB</c> disappeared for whatever reason.</remarks>
		internal static bool SetCursor()
		{
			if (Tftdset != null
				&& (MainViewF.Optionables.PreferTftdTargeter || Ufoset == null))
			{
				Cursorset = Tftdset;
			}
			else if (Ufoset != null)
			{
				Cursorset = Ufoset;
			}
			return (Cursorset != null);
		}

		/// <summary>
		/// Sets or disables spriteshade for the targeter and cuboid sprites.
		/// </summary>
		/// <param name="reset"></param>
		/// <remarks>Called by
		/// <c><see cref="Forms.MainView.MainViewOptionables.SpriteShadeCursor">MainViewOptionables.SpriteShadeCursor</see></c>.</remarks>
		internal static void SetSpriteShadeCursor(bool reset = false)
		{
			if (reset)
			{
				_ia.Dispose();
				_ia = new ImageAttributes();
			}
			else
				_ia.SetGamma(MainViewF.Optionables.SpriteShadeFloatCursor, ColorAdjustType.Bitmap);
		}

		/// <summary>
		/// Sets the <c>Graphics</c> that will be used to draw the targeter and
		/// cuboid sprites.
		/// </summary>
		/// <param name="graphics"></param>
		/// <remarks>The <c>Graphics</c> need to be set near the start of every
		/// call to <c><see cref="Forms.MainView.MainViewOverlay"/>.OnPaint()</c></remarks>
		internal static void SetGraphics(Graphics graphics)
		{
			_graphics = graphics;
		}


		/// <summary>
		/// Draws the cuboid-cursor.
		/// </summary>
//		/// <param name="x"></param>
//		/// <param name="y"></param>
//		/// <param name="halfWidth"></param>
//		/// <param name="halfHeight"></param>
		/// <param name="front"><c>true</c> to draw the front sprite, else back</param>
		/// <param name="gridlevel"><c>true</c> to draw the red sprite, else
		/// blue</param>
		/// <param name="rect">destination rectangle for shaded sprites</param>
		internal static void DrawCuboid_Rembrandt(
//				int x, int y,
//				int halfWidth,
//				int halfHeight,
				bool front,
				bool gridlevel,
				Rectangle rect)
		{
			int id;
			if (front) id = (gridlevel ? RED_FRONT : BLUE_FRONT);
			else       id = (gridlevel ? RED_BACK  : BLUE_BACK);

			_graphics.DrawImage(
							Cursorset[id].Sprite,
							rect,
							0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
							GraphicsUnit.Pixel,
							_ia);
		}

		/// <summary>
		/// Draws the target-cursor.
		/// </summary>
//		/// <param name="x"></param>
//		/// <param name="y"></param>
//		/// <param name="halfWidth"></param>
//		/// <param name="halfHeight"></param>
		/// <param name="rect">destination rectangle for shaded sprites</param>
		internal static void DrawTargeter_Rembrandt(
//				int x, int y,
//				int halfWidth,
//				int halfHeight,
				Rectangle rect)
		{
			_graphics.DrawImage(
							Cursorset[TARGETER].Sprite,
							rect,
							0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
							GraphicsUnit.Pixel,
							_ia);
		}

		/// <summary>
		/// Draws the cuboid-cursor for
		/// <c><see cref="Forms.MainView.MainViewOptionables.UseMono">MainViewOptionables.UseMono</see></c>.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="front"><c>true</c> to draw the front sprite, else back</param>
		/// <param name="toplevel"><c>true</c> to draw the red sprite, else blue</param>
		internal static void DrawCuboid_Picasso(
				int x, int y,
				bool front,
				bool toplevel)
		{
			int id;
			if (front) id = (toplevel ? RED_FRONT : BLUE_FRONT);
			else       id = (toplevel ? RED_BACK  : BLUE_BACK);

			int d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value here.

			byte[] bindata = Cursorset[id].GetBindata();
			int palid;

			int i = -1, h,w;
			for (h = 0; h != Spriteset.SpriteHeight40; ++h)
			for (w = 0; w != Spriteset.SpriteWidth32;  ++w)
			{
				if ((palid = bindata[++i]) != Palette.Tid)
				{
					_graphics.FillRectangle(
										_brushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										d,d);
				}
			}
		}

		/// <summary>
		/// Draws the target-cursor for
		/// <c><see cref="Forms.MainView.MainViewOptionables.UseMono">MainViewOptionables.UseMono</see></c>.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		internal static void DrawTargeter_Picasso(
				int x, int y)
		{
			int d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value here.

			byte[] bindata = Cursorset[TARGETER].GetBindata();
			int palid;

			int i = -1, h,w;
			for (h = 0; h != Spriteset.SpriteHeight40; ++h)
			for (w = 0; w != Spriteset.SpriteWidth32;  ++w)
			{
				if ((palid = bindata[++i]) != Palette.Tid)
				{
					_graphics.FillRectangle(
										_brushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										d,d);
				}
			}
		}
		#endregion Methods (static)
	}
}
