using System;
using System.Drawing;

using XCom;
using XCom.Interfaces;


namespace MapView
{
	internal static class CuboidSprite
	{
		#region Fields (static)
		internal static SpriteCollection Spriteset;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Draws the cuboid-cursor from resources.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		/// <param name="front">true to draw the front sprite, else back</param>
		/// <param name="red">true to draw the red sprite, else blue</param>
		internal static void DrawCuboid(
				Graphics graphics,
				int x, int y,
				int halfWidth,
				int halfHeight,
				bool front,
				bool red)
		{
			int id = 0;
			if (front)
				id = (red ? 3 : 5);
			else
				id = (red ? 0 : 2);

			if (XCMainWindow.UseMonoDraw)
			{
				var d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value.

				var bindata = Spriteset[id].Bindata;

				int palid;

				int i = -1, h,w;
				for (h = 0; h != XCImage.SpriteHeight40; ++h)
				for (w = 0; w != XCImage.SpriteWidth32;  ++w)
				{
					palid = bindata[++i];
					if (palid != Palette.TranId)
					{
						graphics.FillRectangle(
											Palette.BrushesUfoBattle[palid],
											x + (int)(w * Globals.Scale),
											y + (int)(h * Globals.Scale),
											d, d);
					}
				}
			}
			else
				graphics.DrawImage(
								Spriteset[id].Sprite,
								x, y,
								halfWidth  * 2,		// NOTE: the values for width and height
								halfHeight * 5);	// are based on a sprite that's 32x40.
		}

		/// <summary>
		/// Draws the target-cursor from resources.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		internal static void DrawTargeter(
				Graphics graphics,
				int x, int y,
				int halfWidth,
				int halfHeight)
		{
			if (XCMainWindow.UseMonoDraw)
			{
				var d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value.

				var bindata = Spriteset[7].Bindata;

				int palid;

				int i = -1, h,w;
				for (h = 0; h != XCImage.SpriteHeight40; ++h)
				for (w = 0; w != XCImage.SpriteWidth32;  ++w)
				{
					palid = bindata[++i];
					if (palid != Palette.TranId)
					{
						graphics.FillRectangle(
											Palette.BrushesUfoBattle[palid],
											x + (int)(w * Globals.Scale),
											y + (int)(h * Globals.Scale),
											d, d);
					}
				}
			}
			else
				graphics.DrawImage(
								Spriteset[7].Sprite, // yellow targeter sprite
								x, y,
								halfWidth  * 2,		// NOTE: the values for width and height
								halfHeight * 5);	// are based on a sprite that's 32x40.
		}
		#endregion Methods (static)
	}
}
