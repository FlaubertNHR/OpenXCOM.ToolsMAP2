using System;
using System.Collections.Generic;
using System.Drawing;

using XCom;


namespace MapView
{
	/// <summary>
	/// Draws sprites (the cuboid and targeter) from standard resource
	/// UFOGRAPH/CURSOR.PCK+TAB.
	/// </summary>
	internal static class CuboidSprite
	{
		public static void DisposeCursorset()
		{
			DSShared.LogFile.WriteLine("CuboidSprite.DisposeCursorset() static");
			if (Cursorset != null)
				Cursorset.Dispose();
		}


		#region Fields (static)
		private const int RED_BACK   = 0; // sprite-ids in CURSOR.PCK ->
		private const int RED_FRONT  = 3;
		private const int BLUE_BACK  = 2;
		private const int BLUE_FRONT = 5;
		private const int TARGETER   = 7; // 1st yellow targeter sprite

		private const int widthfactor  = 2; // the values for width and height
		private const int heightfactor = 5; // are based on a sprite that's 32x40.
		#endregion Fields (static)


		#region Properties (static)
		private static Spriteset Cursorset
		{ get; set; }

		/// <summary>
		/// Sets the <see cref="Cursorset"/> property.
		/// </summary>
		/// <param name="cursorset"></param>
		internal static void SetCursorset(Spriteset cursorset)
		{
			Cursorset = cursorset;
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Draws the cuboid-cursor.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		/// <param name="front">true to draw the front sprite, else back</param>
		/// <param name="toplevel">true to draw the red sprite, else blue</param>
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
							x, y,
							halfWidth  * widthfactor,
							halfHeight * heightfactor);
		}

		/// <summary>
		/// Draws the cuboid-cursor w/ Mono-style.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="front">true to draw the front sprite, else back</param>
		/// <param name="toplevel">true to draw the red sprite, else blue</param>
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

			IList<Brush> brushes = Palette.BrushesUfoBattle;
			int palid;

			int i = -1, h,w;
			for (h = 0; h != XCImage.SpriteHeight40; ++h)
			for (w = 0; w != XCImage.SpriteWidth32;  ++w)
			{
				palid = bindata[++i];
				if (palid != Palette.Tid)
				{
					graphics.FillRectangle(
										brushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										d, d);
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
							x, y,
							halfWidth  * widthfactor,
							halfHeight * heightfactor);
		}

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

			byte[] bindata = Cursorset[7].GetBindata();

			IList<Brush> brushes = Palette.BrushesUfoBattle;
			int palid;

			int i = -1, h,w;
			for (h = 0; h != XCImage.SpriteHeight40; ++h)
			for (w = 0; w != XCImage.SpriteWidth32;  ++w)
			{
				palid = bindata[++i];
				if (palid != Palette.Tid)
				{
					graphics.FillRectangle(
										brushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										d, d);
				}
			}
		}
		#endregion Methods (static)
	}
}
