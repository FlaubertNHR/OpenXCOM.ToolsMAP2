using System;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;


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
		private delegate string ConvertObjectEvent(object value);
		#endregion Delegates


		#region Fields (static)
		private static Dictionary<Type, ConvertObjectEvent> _converters =
				   new Dictionary<Type, ConvertObjectEvent>();
		#endregion Fields (static)


		#region Fields
		private readonly Dictionary<string, Property> _properties =
					 new Dictionary<string, Property>();

		private Dictionary<string, Option> _dict =
			new Dictionary<string, Option>();
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets the key-collection for this Options' dictionary.
		/// </summary>
		internal Dictionary<string, Option>.KeyCollection Keys
		{
			get { return _dict.Keys; }
		}

		/// <summary>
		/// Gets or sets an Option keyed by a specified key.
		/// @note Ensure that 'key' is non-null before call.
		/// </summary>
		internal Option this[string key]
		{
			get { return (_dict.ContainsKey(key)) ? _dict[key] : null; }
		}
		#endregion Properties


		#region Methods (static)
		internal static void InitializeConverters()
		{
			_converters[typeof(Color)] = new ConvertObjectEvent(ConvertColor);
		}

		internal static void ReadOptions(TextReader tr, Options options)
		{
			string key;

			KeyvalPair keyval;
			while ((keyval = Varidia.getKeyvalPair(tr)) != null)
			{
				switch (keyval.Key)
				{
					case "{": break;  // starting out
					case "}": return; // all done

					default:
						key = keyval.Key;
						if (options[key] != null)
						{
							options[key].Value = keyval.Value;
							options[key].doUpdate(key);
						}
						break;
				}
			}
		}

		private static string Convert(object o)
		{
			return (_converters.ContainsKey(o.GetType())) ? _converters[o.GetType()](o)
														  : o.ToString();
		}

		private static string ConvertColor(object o)
		{
			var color = (Color)o;
			if (!color.IsKnownColor && !color.IsNamedColor && !color.IsSystemColor)
				return string.Format(
								CultureInfo.InvariantCulture,
								"{0},{1},{2},{3}",
								color.A, color.R, color.G, color.B);

			return color.Name;
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Adds an Option to a specified target.
		/// @note There is no error-handling so don't foff it.
		/// @note Only one of 'changer' or 'target' can be specified; 'changer'
		/// takes precedence.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="val">default value of the property</param>
		/// <param name="description">property description</param>
		/// <param name="category">property category</param>
		/// <param name="changer">handler to receive the OptionChangedEvent</param>
		/// <param name="target">the object that will receive the changed
		/// property values: an internal event handler will be created and the
		/// name must be the name of a property of the type that the target is
		/// whatever that meant</param>
		internal void AddOption(
				string key,
				object val,
				string description,
				string category,
				OptionChangedEvent changer = null,
				object target = null)
		{
			var option = new Option(val, description, category);

			if (changer != null)
			{
				option.OptionChanged += changer;
			}
			else if (target != null)
			{
				_properties[key] = new Property(target, key);
				option.OptionChanged += OnOptionChanged;
			}

			_dict[key] = option;
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
			if (!_dict.ContainsKey(key))
				_dict.Add(key, new Option(val));

			return _dict[key];
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

			foreach (string key in _dict.Keys)
				tw.WriteLine("\t" + key + ":" + Convert(this[key].Value));

			tw.WriteLine("}");
		}
		#endregion Methods


		#region Events
		private void OnOptionChanged(string key, object val)
		{
			_properties[key].SetValue(val);
		}
		#endregion Events
	}



	/// <summary>
	/// Stores information to be used in the OptionsPropertyGrid.
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
		#endregion Fields (static)


		#region Properties
		private object _value;
		internal object Value
		{
			get { return _value; }
			set
			{
				if (_value != null)
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
				}
				_value = value;
			}
		}

		/// <summary>
		/// Checks if Value is a boolean and TRUE. else false.
		/// </summary>
		internal bool IsTrue
		{
			get
			{
				if (Value is bool)
					return (bool)Value;

				return false;
			}
		}

		internal string Description
		{ get; set; }

		internal string Category
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="default"></param>
		/// <param name="description"></param>
		/// <param name="category"></param>
		internal Option(
				object @default,
				string description = null,
				string category    = null)
		{
			_value      = @default; // TODO: Investigate whether that should run Value_set.
			Description = description;
			Category    = category;
		}
		#endregion cTor


		#region Methods (static)
		internal static void InitializeParsers()
		{
			_parsers[typeof(int)]   = ParseStringInt;
			_parsers[typeof(Color)] = ParseStringColor;
			_parsers[typeof(bool)]  = ParseStringBool;
		}

		private static object ParseStringBool(string st)
		{
			return bool.Parse(st);
		}

		private static object ParseStringInt(string st)
		{
			return int.Parse(st, CultureInfo.InvariantCulture);
		}

		private static object ParseStringColor(string st)
		{
			string[] vals = st.Split(',');

			switch (vals.Length)
			{
				case 1:
					return Color.FromName(st);

				case 3:
					return Color.FromArgb(
									int.Parse(vals[0], CultureInfo.InvariantCulture),
									int.Parse(vals[1], CultureInfo.InvariantCulture),
									int.Parse(vals[2], CultureInfo.InvariantCulture));

				case 4:
					return Color.FromArgb(
									int.Parse(vals[0], CultureInfo.InvariantCulture),
									int.Parse(vals[1], CultureInfo.InvariantCulture),
									int.Parse(vals[2], CultureInfo.InvariantCulture),
									int.Parse(vals[3], CultureInfo.InvariantCulture));
			}
			return null;
		}
		#endregion Methods (static)


		#region Methods
		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		internal void doUpdate(string key, object val)
		{
			if (OptionChanged != null)
				OptionChanged(key, val);
		}

		// TODO: FxCop CA1030:UseEventsWhereAppropriate
		internal void doUpdate(string key)
		{
			if (OptionChanged != null)
				OptionChanged(key, Value);
		}
		#endregion Methods
	}



	/// <summary>
	/// Property struct.
	/// </summary>
	internal struct Property
	{
		#region Fields
		private readonly PropertyInfo _info;
		private readonly object _o;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="property"></param>
		internal Property(object o, string property)
		{
			_info = (_o = o).GetType().GetProperty(property);
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets the value of this Property to a specified object.
		/// </summary>
		/// <param name="o"></param>
		internal void SetValue(object o)
		{
			_info.SetValue(_o, o, new object[]{});
		}
		#endregion Methods
	}
}
