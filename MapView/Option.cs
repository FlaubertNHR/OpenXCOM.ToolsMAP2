using System;
using System.Collections.Generic;
using System.Drawing;


namespace MapView
{
	/// <summary>
	/// The option of an Optionable Property.
	/// </summary>
	internal sealed class Option
	{
		#region Delegates
		private delegate object ParseEvent(string st);
		#endregion Delegates


		#region Events
		internal event OptionChangedEvent OptionChanged;
		#endregion Events


		#region Fields (static)
		private static Dictionary<Type, ParseEvent> _parsers =
				   new Dictionary<Type, ParseEvent>();

		private static string[] KnownColors = Enum.GetNames(typeof(KnownColor));
		#endregion Fields (static)


		#region Properties
		private object _value;
		internal object Value
		{
			get { return _value; }
			set
			{
				if (!_value.Equals(value)) // TODO: Investigate that: (true != true) sic.
				{
					Type type = _value.GetType();
					if (_parsers.ContainsKey(type))
					{
						string val = value as String;
						if (val != null)
						{
							_value = _parsers[type](val);
							return;
						}
					}
					_value = value;
				}
			}
		}

		/// <summary>
		/// Gets if Value is a boolean and TRUE. else false.
		/// </summary>
		/// <remarks>Is used only to determine which secondary viewers should be
		/// displayed when MapView loads.</remarks>
		internal bool IsTrue
		{
			get { return (bool)Value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiates an Option with its default value.
		/// </summary>
		/// <param name="default"></param>
		internal Option(object @default)
		{
			_value = @default; // TODO: Investigate whether that should run set_Value. uh no ...
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Adds parsers for types boolean, int32, and color.
		/// </summary>
		internal static void InitializeParsers()
		{
			_parsers[typeof(Boolean)] = ParseBoolean;
			_parsers[typeof(Int32)]   = ParseInt32;
			_parsers[typeof(Color)]   = ParseColor;
		}

		/// <summary>
		/// Parses out user-defined booleans output by MapOptions.Cfg.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		private static object ParseBoolean(string val)
		{
			bool result;
			if (Boolean.TryParse(val, out result))
				return result;

			return null;
		}

		/// <summary>
		/// Parses out user-defined ints output by MapOptions.Cfg.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		private static object ParseInt32(string val)
		{
			int result;
			if (Int32.TryParse(val, out result))
				return result;

			return null;
		}

		/// <summary>
		/// Parses out user-defined colors output by MapOptions.Cfg.
		/// UD-colors can be one of three formats:
		/// - "color"
		/// - (int)r,(int)g,(int)b
		/// - (int)a,(int)r,(int)g,(int)b
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		private static object ParseColor(string val)
		{
			string[] vals = val.Split(',');

			if (vals.Length == 1)
			{
				for (int i = 0; i != KnownColors.Length; ++i)
				{
					if (KnownColors[i] == val)
						return Color.FromName(val);
				}
			}
			else if (vals.Length == 3 || vals.Length == 4)
			{
				if (vals.Length == 3)
				{
					var argb = new string[4] { "255", "0","0","0" };
					for (int i = 1; i != 4; ++i)
						argb[i] = vals[i - 1];

					vals = argb;
				}

				int a = 0, r = 0, g = 0, b = 0;
				if (TryColorValues(vals, ref a, ref r, ref g, ref b))
					return Color.FromArgb(a,r,g,b);
			}
			return null;
		}

		/// <summary>
		/// Helper for ParseColor().
		/// </summary>
		/// <param name="vals"></param>
		/// <param name="a"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool TryColorValues(
				IReadOnlyList<string> vals,
				ref int a,
				ref int r,
				ref int g,
				ref int b) // idiots ought to have put alpha at last pos. But thanks anyway.
		{
			int result;
			if (Int32.TryParse(vals[0], out result)
				&& result > -1 && result < 256)
			{
				a = result;
			}
			else return false;

			if (Int32.TryParse(vals[1], out result)
				&& result > -1 && result < 256)
			{
				r = result;
			}
			else return false;

			if (Int32.TryParse(vals[2], out result)
				&& result > -1 && result < 256)
			{
				g = result;
			}
			else return false;

			if (Int32.TryParse(vals[3], out result)
				&& result > -1 && result < 256)
			{
				b = result;
			}
			else return false;

			return true;
		}
		#endregion Methods (static)


		#region Methods
		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		/// <summary>
		/// Called by OptionsManager.ReadOptions() when an OptionsForm loads or
		/// OptionsForm.OnPropertyValueChanged() when user changes an Option's
		/// value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		internal void SetValue(string key, object val)
		{
			Value = val;

			if (OptionChanged != null)
				OptionChanged(key, Value);
		}
		#endregion Methods
	}
}
