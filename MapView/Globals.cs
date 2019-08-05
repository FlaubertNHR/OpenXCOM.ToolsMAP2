using System;
using System.Reflection;

using MapView.Forms.MainView;

using XCom;


namespace MapView
{
	internal static class Globals
	{
		#region Fields (static)
		internal const double ScaleMinimum = 0.25;
		internal const double ScaleMaximum = 3.00;

		internal const string ALLFILES = "*.*";

		internal const int PERIOD = 235; // longest acceptable tic-delay for Timers
		#endregion


		#region Properties (static)
		private static double _scale = 1.0;
		/// <summary>
		/// The scale-factor for sprites and clicks (etc) in MainView only.
		/// TODO: Then why isn't this in MainViewF.
		/// </summary>
		internal static double Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;

				ObserverManager.ToolFactory.SetScaleOutButtonEnabled(Math.Abs(_scale - ScaleMinimum) > 0.001);
				ObserverManager.ToolFactory.SetScaleInButtonEnabled( Math.Abs(_scale - ScaleMaximum) > 0.001);
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
		internal static T Clamp<T>(
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
		#endregion
	}
}
