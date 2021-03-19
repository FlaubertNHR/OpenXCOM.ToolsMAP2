using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace MapView
{
	internal delegate void OptionChangedEvent(string key, object val);


	/// <summary>
	/// Options objects are for use with the OptionsPropertyGrid. An Options
	/// object is created for each of <see cref="MainViewF"/>,
	/// <see cref="MapView.Forms.Observers.TileView"/>,
	/// <see cref="MapView.Forms.Observers.TopView"/>, and
	/// <see cref="MapView.Forms.Observers.RouteView"/>.
	/// </summary>
	internal sealed class Options
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
		/// Gets an <see cref="Option"/> keyed by a specified key.
		/// </summary>
		/// <remarks>Ensure that 'key' is non-null before call.</remarks>
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
		/// <summary>
		/// 
		/// </summary>
		internal static void InitializeConverters()
		{
			_converters[typeof(Color)] = new ConvertEvent(GetColorString);
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
		private static string GetColorString(object o)
		{
			var color = (Color)o;
			if (!color.IsKnownColor && !color.IsNamedColor && !color.IsSystemColor)
			{
				return color.A + "," + color.R + "," + color.G + "," + color.B;
			}
			return color.Name;
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Adds an Option w/ a default value.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="default">default value of the property</param>
		/// <param name="changer">pointer to a handler that subscribes to the
		/// OptionChangedEvent</param>
		/// <remarks>There is no error-handling so don't foff it.</remarks>
		internal void AddOptionDefault(
				string key,
				object @default,
				OptionChangedEvent changer)
		{
			//DSShared.LogFile.WriteLine("AddOptionDefault()");
			//DSShared.LogFile.WriteLine(". key= " + key);
			//DSShared.LogFile.WriteLine(". default= " + @default);
			//DSShared.LogFile.WriteLine(". changer= " + changer);

			var option = new Option(@default);

			if (changer != null) // safety. I don't think this will ever be null.
				option.OptionChanged += changer;

			_options[key] = option;
		}

		/// <summary>
		/// Gets the object tied to the key. If there is no object one will be
		/// created with the value specified.
		/// </summary>
		/// <param name="key">the name of the Option object</param>
		/// <param name="val">if there is no Option object tied to the
		/// string, an Option will be created with this as its Value</param>
		/// <returns>the Option object tied to the key</returns>
		/// <remarks>Is used only by VolutarService.</remarks>
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
		/// <param name="head"></param>
		/// <param name="tw"></param>
		internal void WriteOptions(string head, TextWriter tw)
		{
			tw.WriteLine(head);
			tw.WriteLine("{");

			foreach (string key in _options.Keys)
				tw.WriteLine("\t" + key + ":" + Convert(this[key].Value));

			tw.WriteLine("}");
		}
		#endregion Methods
	}
}
