using System;
using System.Collections.Generic;
using System.Drawing;


namespace MapView
{
	/// <summary>
	/// The <c>Option</c> of an Optionable property. <c>Options</c> are stored
	/// in
	/// <list type="bullet">
	/// <item><c><see cref="Forms.MainView.MainViewOptionables"/></c></item>
	/// <item><c><see cref="Forms.Observers.TileViewOptionables"/></c></item>
	/// <item><c><see cref="Forms.Observers.TopViewOptionables"/></c></item>
	/// <item><c><see cref="Forms.Observers.RouteViewOptionables"/></c></item>
	/// </list>
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
				//DSShared.Logfile.Log("Option.Value.set");

				if (!_value.Equals(value)) // TODO: Investigate that: (true != true) sic.
				{
					Type type = _value.GetType();
					if (_parsers.ContainsKey(type)) // '_parsers' won't contain type of String.
					{
						string val = value as String; // will not be null if 'MapOptions.cfg' - can be null if PropertyGrid
						if (val != null)
						{
							_value = _parsers[type](val);
							return;
						}
					}
					_value = value;
				}
			}
/*			set
			{
				if (value != null)
				{
//					if (!_value.Equals(value))
					if (!value.Equals(_value))
					{
//						Type type = _value.GetType();
						Type type = value.GetType();
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
				else
					OptionsManager.error("n/a", OptionsManager.ERROR_SET);
			} */
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
		/// cTor. Instantiates an <c>Option</c> with its default value.
		/// </summary>
		/// <param name="default"></param>
		internal Option(object @default)
		{
			//DSShared.Logfile.Log("Option.Option() default= " + @default);
			_value = @default; // TODO: Investigate whether that should run set_Value. uh no ...
		}
		#endregion cTor


		#region Methods
		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		/// <summary>
		/// Called by
		/// <c><see cref="OptionsManager">OptionsManager</see>.ReadOptions()</c>
		/// when MapView loads or by
		/// <c><see cref="OptionsPropertyGrid"></see>.OnPropertyValueChanged()</c>
		/// when user changes the value of this <c>Option</c>.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		internal void SetValue(string key, object val)
		{
			//DSShared.Logfile.Log("Option.SetValue() key= " + key);

			Value = val; // TODO: error check not null and value-type and -range are valid for this Option

			if (OptionChanged != null)
				OptionChanged(key, Value);


//			if (!Value.Equals(val)) // TODO: Investigate that: (true != true) sic.
//			{
//				DSShared.Logfile.Log(". Value != val");
//
//				Type type = Value.GetType();
//				if (_parsers.ContainsKey(type))
//				{
//					DSShared.Logfile.Log(". . parsers contains Type");
//
//					string str = val as String;
//					if (str != null)
//					{
//						DSShared.Logfile.Log(". . . val is String");
//
//						Value = _parsers[type](str);
//						OptionChanged(key, Value);
//						return;
//					}
//				}
//
//				DSShared.Logfile.Log(". no parser OR not string");
//				Value = val;
//				OptionChanged(key, Value);
//			}


//			if (val != null) // && val.GetType() == Value.GetType())
//			{
//				Value = val; // TODO: error check not null and value-type and -range are valid for this Option
//
//				if (OptionChanged != null)
//					OptionChanged(key, Value);
//			}
//			else
//				OptionsManager.error(key, OptionsManager.ERROR_SET);
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Adds parsers for types <c>Boolean</c>, <c>Int32</c>, and <c>Color</c>.
		/// </summary>
		internal static void InitializeParsers()
		{
			//DSShared.Logfile.Log("Option.InitializeParsers()");

			_parsers[typeof(Boolean)] = ParseBoolean;
			_parsers[typeof(Int32)]   = ParseInt32;
			_parsers[typeof(Color)]   = ParseColor;
		}

		/// <summary>
		/// Parses out user-defined booleans output by 'MapOptions.cfg'.
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
		/// Parses out user-defined ints output by 'MapOptions.cfg'.
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
		/// Parses out user-defined colors output by 'MapOptions.cfg'.
		/// User-defined colors can be one of three formats.
		/// <list type="bullet">
		/// <item><c>(string)[color]</c></item>
		/// <item><c>(int)r,(int)g,(int)b</c></item>
		/// <item><c>(int)a,(int)r,(int)g,(int)b</c></item>
		/// </list>
		/// </summary>
		/// <param name="val">a <c>string</c> in one of the three accepted
		/// formats</param>
		/// <returns>the <c>Color</c> as an <c>object</c></returns>
		private static object ParseColor(string val)
		{
			string[] vals = val.Split(',');
			switch (vals.Length)
			{
				case 1:
					for (int i = 0; i != KnownColors.Length; ++i)
					{
						if (KnownColors[i] == val)
							return Color.FromName(val);
					}
					break;

				case 3:
					vals = new [] { "255", vals[0], vals[1], vals[2] };
					goto case 4;

				case 4:
				{
					int a = 0, r = 0, g = 0, b = 0;
					if (TryColorArgb(vals, ref a, ref r, ref g, ref b))
						return Color.FromArgb(a,r,g,b);
					break;
				}
			}
			return null; // this better never happen.
		}

		/// <summary>
		/// Helper for <c><see cref="ParseColor()">ParseColor()</see></c>.
		/// </summary>
		/// <param name="vals"></param>
		/// <param name="a"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool TryColorArgb(
				IReadOnlyList<string> vals,
				ref int a,
				ref int r,
				ref int g,
				ref int b) // idiots ought to have put alpha at last pos. But thanks anyway.
		{
			int result;
			for (int i = 0; i != 4; ++i)
			{
				if (Int32.TryParse(vals[i], out result)
					&& result > -1 && result <= Byte.MaxValue)
				{
					switch (i)
					{
						case 0: a = result; break;
						case 1: r = result; break;
						case 2: g = result; break;
						case 3: b = result; break;
					}
				}
				else return false;
			}
			return true;
		}
		#endregion Methods (static)
	}
}
