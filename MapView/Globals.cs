using System;
using System.Drawing.Imaging;

using MapView.Forms.MainView;

using XCom;



namespace MapView
{
	internal static class Globals
	{
		#region Fields (static)
		internal const string TITLE_t  = "TopView";
		internal const string TITLE_r  = "RouteView";
		internal const string TITLE_tr = "Top/Route View";

		internal const float ScaleMinimum = 0.250f;
		internal const float ScaleMaximum = 3.000f;
		internal const float ScaleDelta   = 0.125f;

		internal const string ALLFILES = "*.*";

		internal const int PERIOD = 235; // longest acceptable tic-delay for Timers
		#endregion Fields (static)


		#region Properties (static)
		private static float _scale = 1f;
		/// <summary>
		/// The scale-factor for sprites and clicks (etc) in MainView only.
		/// TODO: Then why isn't this in MainViewF.
		/// </summary>
		internal static float Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;

				ObserverManager.ToolFactory.EnableScaleout(Math.Abs(_scale - ScaleMinimum) > 0.001f);
				ObserverManager.ToolFactory.EnableScalein( Math.Abs(_scale - ScaleMaximum) > 0.001f);
			}
		}

		private static bool _autoScale = true;
		internal static bool AutoScale
		{
			get { return _autoScale; }
			set { _autoScale = value; }
		}

		internal static ImageAttributes Ia = new ImageAttributes();
		internal static void SetSpriteShade(bool reset = false)
		{
			if (reset)
			{
				Ia.Dispose();
				Ia = new ImageAttributes();
			}
			else
				Ia.SetGamma(MainViewF.Optionables.SpriteShadeFloat, ColorAdjustType.Bitmap);
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Clamps a value between minimum and maximum values inclusively.
		/// </summary>
		/// <typeparam name="T">A type that inherits from the IComparable interface.</typeparam>
		/// <param name="val">your value</param>
		/// <param name="min">the minimum</param>
		/// <param name="max">the maximum</param>
		/// <returns><paramref name="min"/> if <paramref name="val"/> is less
		/// than <paramref name="min"/>; <paramref name="max"/> if
		/// <paramref name="val"/> is greater than <paramref name="max"/>; else
		/// <paramref name="val"/> itself</returns>
		/// <remarks>No check is done to ensure that <paramref name="min"/> is
		/// less than <paramref name="max"/>. So do that before call ...</remarks>
		internal static T Viceroy<T>(
				this T val,
				T min,
				T max) where T
			:
				IComparable<T>
		{
			if (val.CompareTo(min) < 0)
				return min;

			if (val.CompareTo(max) > 0)
				return max;

			return val;
		}


		/// <summary>
		/// Gets a c/r/L location in standard format.
		/// </summary>
		/// <param name="c">col</param>
		/// <param name="r">row</param>
		/// <param name="l">lev</param>
		/// <param name="levels">total levels of the current Map</param>
		/// <returns>the given location as a string</returns>
		/// <remarks>This function inverts the z-level for readability (which is
		/// the policy in Mapview2).</remarks>
		internal static string GetLocationString(
				int c, int r, int l,
				int levels)
		{
			l = levels - l - 1; // base0 and inverted

			if (l < RouteNode.RouteNodeCutoffLevel) l += 256;

			if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
			if (MainViewF.Optionables.Base1_z)  { ++l; }

			return "c " + c + "  r " + r + "  L " + l;

		}
		#endregion Methods (static)
	}
}
