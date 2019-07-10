using System;
using System.Drawing;

using XCom;
using XCom.Interfaces;


namespace MapView
{
	internal class CuboidSprite
	{
		private readonly SpriteCollection _spriteset;


		#region cTor
		/// <summary>
		/// Constructs a CuboidSprite.
		/// @note The CuboidSprite is the actual cuboid selector in XCOM
		/// resources.
		/// </summary>
		/// <param name="spriteset"></param>
		internal CuboidSprite(SpriteCollection spriteset)
		{
			_spriteset = spriteset;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		/// <param name="front">true to draw the front sprite, else back</param>
		/// <param name="red">true to draw the red sprite, else blue</param>
		internal void DrawCuboid(
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

				var bindata = _spriteset[id].Bindata;

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
								_spriteset[id].Sprite,
								x, y,
								halfWidth  * 2,		// NOTE: the values for width and height
								halfHeight * 5);	// are based on a sprite that's 32x40.
		}

		internal void DrawTargeter(
				Graphics graphics,
				int x, int y,
				int halfWidth,
				int halfHeight)
		{
			if (XCMainWindow.UseMonoDraw)
			{
				var d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value.

				var bindata = _spriteset[7].Bindata;

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
								_spriteset[7].Sprite, // yellow targeter sprite
								x, y,
								halfWidth  * 2,		// NOTE: the values for width and height
								halfHeight * 5);	// are based on a sprite that's 32x40.
		}
		#endregion Methods
	}
}
