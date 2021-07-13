using System;
using System.Collections.Generic;
using System.Drawing;

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
			DSShared.Logfile.Log("CuboidSprite.DisposeCursorset() static");
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

			Cursorset = null;
		}


		#region Fields (static)
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
		/// <remarks>Used only in
		/// <c><see cref="MainViewF()">MainViewF()</see></c> for cases (1)
		/// <c>PreferUfoTargeter</c> has not been registered in
		/// "settings/MapOptions.cfg" yet or (2) the user's
		/// <c>UFOGRAPH/CURSOR.PCK+TAB</c> disappeared for whatever reason.</remarks>
		internal static bool AssignCursorset()
		{
			//DSShared.Logfile.Log("CuboidSprite.AssignCursorset()");
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
		/// Changes <c><see cref="Cursorset"/></c> when user changes
		/// <c><see cref="Forms.MainView.MainViewOptionables.PreferTftdTargeter"/></c>.
		/// </summary>
		/// <param name="val"><c>true</c> to prefer the TFTD cursorset</param>
		internal static void PreferTftdTargeter(bool val)
		{
			//DSShared.Logfile.Log("CuboidSprite.PreferTftdTargeter() val= " + val);
			if (Tftdset != null && (val || Ufoset == null))
			{
				Cursorset = Tftdset;
			}
			else if (Ufoset != null)
			{
				Cursorset = Ufoset;
			}
		}


		/// <summary>
		/// Draws the cuboid-cursor.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		/// <param name="front"><c>true</c> to draw the front sprite, else back</param>
		/// <param name="toplevel"><c>true</c> to draw the red sprite, else blue</param>
		internal static void DrawCuboid_Rembrandt(
				Graphics graphics,
				int x, int y,
				int halfWidth,
				int halfHeight,
				bool front,
				bool toplevel)
		{
			int id;
			if (front) id = (toplevel ? RED_FRONT : BLUE_FRONT);
			else       id = (toplevel ? RED_BACK  : BLUE_BACK);

			graphics.DrawImage(
							Cursorset[id].Sprite,
							x,y,
							halfWidth  * widthfactor,
							halfHeight * heightfactor);
		}
//			if (MainViewF.Optionables.SpriteShadeEnabled)
//				_graphics.DrawImage(
//								sprite,
//								rect,
//								0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
//								GraphicsUnit.Pixel,
//								_ia);
//			else
//				_graphics.DrawImage(sprite, rect);

		/// <summary>
		/// Draws the cuboid-cursor w/ Mono-style.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="front"><c>true</c> to draw the front sprite, else back</param>
		/// <param name="toplevel"><c>true</c> to draw the red sprite, else blue</param>
		internal static void DrawCuboid_Picasso(
				Graphics graphics,
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
					graphics.FillRectangle(
										_brushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										d,d);
				}
			}
		}

		/// <summary>
		/// Draws the target-cursor.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		internal static void DrawTargeter_Rembrandt(
				Graphics graphics,
				int x, int y,
				int halfWidth,
				int halfHeight)
		{
			graphics.DrawImage(
							Cursorset[TARGETER].Sprite,
							x,y,
							halfWidth  * widthfactor,
							halfHeight * heightfactor);
		}
//			if (MainViewF.Optionables.SpriteShadeEnabled)
//				_graphics.DrawImage(
//								sprite,
//								rect,
//								0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
//								GraphicsUnit.Pixel,
//								_ia);
//			else
//				_graphics.DrawImage(sprite, rect);

		/// <summary>
		/// Draws the target-cursor w/ Mono.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		internal static void DrawTargeter_Picasso(
				Graphics graphics,
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
					graphics.FillRectangle(
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
