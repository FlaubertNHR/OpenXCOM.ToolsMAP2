using System;

using MapView.Forms.MainView;



namespace MapView
{
	internal static class Globals
	{
		#region Fields (static)
		internal const float ScaleMinimum = 0.250f;
		internal const float ScaleMaximum = 3.000f;
		internal const float ScaleDelta   = 0.125f;

		internal const string ALLFILES = "*.*";

		internal const int PERIOD = 235; // longest acceptable tic-delay for Timers
		#endregion


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
		#endregion


		#region Methods (static)
		/// <summary>
		/// Clamps a value between min and max inclusively. Note that no check
		/// is done to ensure that min is less than max.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns>min if val is less than min; max if value is greater than
		/// max; else the value itself</returns>
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
		internal static string GetLocationString(
				int c, int r, int l,
				int levels)
		{
			l = levels - l; // invert

			if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
			if (!MainViewF.Optionables.Base1_z) { --l; }

			return "c " + c + "  r " + r + "  L " + l;

		}
		#endregion
	}
}
