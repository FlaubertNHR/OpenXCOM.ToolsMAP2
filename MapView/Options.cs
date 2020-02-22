using System;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace MapView
{
	internal delegate void OptionChangedEvent(string key, object val);


	/// <summary>
	/// Options objects are for use with the OptionsPropertyGrid. An Options
	/// object is created for each of MainView, TileView, TopView, and
	/// RouteView.
	/// </summary>
	public sealed class Options
	{
		#region Delegates
		private delegate string ConvertEvent(object value);
		#endregion Delegates


		#region Fields (static)
		/// <summary>
		/// These converters are for taking a property as input and outputting
		/// it as a string to MapOptions.Cfg.
		/// </summary>
		private static Dictionary<Type, ConvertEvent> _converters =
				   new Dictionary<Type, ConvertEvent>();
		#endregion Fields (static)


		#region Fields
		private readonly Dictionary<string, Option> _options =
					 new Dictionary<string, Option>();
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets an Option keyed by a specified key.
		/// @note Ensure that 'key' is non-null before call.
		/// </summary>
		internal Option this[string key]
		{
			get
			{
				if (_options.ContainsKey(key))
					return _options[key];

				return null;
			}
		}
		#endregion Properties


		#region Methods (static)
		internal static void InitializeConverters()
		{
			_converters[typeof(Color)] = new ConvertEvent(ConvertColor);
		}

		/// <summary>
		/// Converts an object to a string for output to MapOptions.Cfg.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		private static string Convert(object o)
		{
			var type = o.GetType();
			if (_converters.ContainsKey(type))
				return _converters[type](o);

			return o.ToString();
		}

		/// <summary>
		/// Converts a color-object to a string for output to MapOptions.Cfg.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		private static string ConvertColor(object o)
		{
			var color = (Color)o;
			if (!isFamiliarColor(color))
				return String.Format(
								CultureInfo.InvariantCulture,
								"{0},{1},{2},{3}",
								color.A, color.R, color.G, color.B);

			return color.Name;
		}

		private static bool isFamiliarColor(Color color)
		{
			return color.IsKnownColor || color.IsNamedColor || color.IsSystemColor;
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Adds an Option w/ a default value.
		/// @note There is no error-handling so don't foff it.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="default">default value of the property</param>
		/// <param name="changer">pointer to a handler that subscribes to the
		/// OptionChangedEvent</param>
		internal void AddOptionDefault(
				string key,
				object @default,
				OptionChangedEvent changer)
		{
			var option = new Option(@default);

			if (changer != null)
				option.OptionChanged += changer;

			_options[key] = option;
		}

		/// <summary>
		/// Gets the object tied to the key. If there is no object one will be
		/// created with the value specified.
		/// @note Is used only by VolutarService.
		/// </summary>
		/// <param name="key">the name of the Option object</param>
		/// <param name="val">if there is no Option object tied to the
		/// string, an Option will be created with this as its Value</param>
		/// <returns>the Option object tied to the key</returns>
		internal Option GetOption(string key, object val)
		{
			if (!_options.ContainsKey(key))
				_options.Add(key, new Option(val));

			return _options[key];
		}

		/// <summary>
		/// Writes this Options to 'ShareOptions' = "MV_OptionsFile" -> MapOptions.cfg
		/// And you thought indirection was a bad thing.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="tw"></param>
		internal void WriteOptions(string line, TextWriter tw)
		{
			tw.WriteLine(line);
			tw.WriteLine("{");

			foreach (string key in _options.Keys)
				tw.WriteLine("\t" + key + ":" + Convert(this[key].Value));

			tw.WriteLine("}");
		}
		#endregion Methods
	}



	/// <summary>
	/// The option of an Optionable Property.
	/// </summary>
	public sealed class Option
	{
		#region Delegates
		private delegate object ParseStringEvent(string st);
		#endregion Delegates


		#region Events
		internal event OptionChangedEvent OptionChanged;
		#endregion Events


		#region Fields (static)
		private static Dictionary<Type, ParseStringEvent> _parsers =
				   new Dictionary<Type, ParseStringEvent>();

		private static string[] KnownColors = Enum.GetNames(typeof(KnownColor));
		#endregion Fields (static)


		#region Properties
		private object _value;
		internal object Value
		{
			get { return _value; }
			set
			{
				//DSShared.LogFile.WriteLine("Options set_Value");
				if (!_value.Equals(value)) // TODO: Investigate that: (true != true) sic.
				{
					var type = _value.GetType();
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
		/// @note Is used only to determine which secondary viewers should be
		/// displayed when MapView loads.
		/// </summary>
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
		internal static void InitializeParsers()
		{
			_parsers[typeof(Boolean)] = ParseBoolean;
			_parsers[typeof(Int32)]   = ParseInt32;
			_parsers[typeof(Color)]   = ParseColor;
		}

		/// <summary>
		/// Parses out user-defined booleans output by MapOptions.Cfg.
		/// </summary>
		/// <param name="st"></param>
		/// <returns></returns>
		private static object ParseBoolean(string st)
		{
			bool result;
			if (Boolean.TryParse(st, out result))
				return result;

			return null;
		}

		/// <summary>
		/// Parses out user-defined ints output by MapOptions.Cfg.
		/// </summary>
		/// <param name="st"></param>
		/// <returns></returns>
		private static object ParseInt32(string st)
		{
			int result;
			if (Int32.TryParse(st, out result))
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
		/// <param name="st"></param>
		/// <returns></returns>
		private static object ParseColor(string st)
		{
			string[] vals = st.Split(',');

			if (vals.Length == 1)
			{
				for (int i = 0; i != KnownColors.Length; ++i)
				{
					if (KnownColors[i] == st)
						return Color.FromName(st);
				}
			}
			else if (vals.Length == 3 || vals.Length == 4)
			{
				if (vals.Length == 3)
				{
					var valsT = new string[4] { "255", "0","0","0" };
					for (int i = 1, j = 0; i != 4; ++i, ++j)
						valsT[i] = vals[j];

					vals = valsT;
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
		/// Called by OptionsForm.ReadOptions() when an OptionsForm loads.
		/// </summary>
		/// <param name="key"></param>
		internal void doUpdate(string key)
		{
			//DSShared.LogFile.WriteLine("doUpdate() key= " + key);
//			if (OptionChanged != null)
			OptionChanged(key, Value);
		}

		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		/// <summary>
		/// Called by OptionsForm.OnPropertyValueChanged() when user changes an
		/// Option's val.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		internal void doUpdate(string key, object val)
		{
			//DSShared.LogFile.WriteLine("doUpdate() key= " + key + " val= " + val);
//			if (OptionChanged != null)
			OptionChanged(key, val);
		}
		#endregion Methods
	}
}
