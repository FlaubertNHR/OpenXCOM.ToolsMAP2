using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace MapView
{
	internal delegate void OptionChangedEvent(string key, object val);


	/// <summary>
	/// <c>Options</c> are for use with the <c><see cref="OptionsPropertyGrid"/></c>.
	/// <c>Options</c> is instantiated separately for
	/// <list type="bullet">
	/// <item><c><see cref="MainViewF"/></c></item>
	/// <item><c><see cref="MapView.Forms.Observers.TileView"/></c></item>
	/// <item><c><see cref="MapView.Forms.Observers.TopView"/></c></item>
	/// <item><c><see cref="MapView.Forms.Observers.RouteView"/></c></item>
	/// </list>
	/// </summary>
	internal sealed class Options
	{
		#region Delegates
		private delegate string ConvertEvent(object value);
		#endregion Delegates


		#region Fields (static)
		/// <summary>
		/// These converters are for taking a property as input and outputting
		/// it as a string to "MapOptions.Cfg".
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
		/// Gets an <c><see cref="Option"/></c> keyed by a specified key.
		/// </summary>
		/// <remarks>Ensure that <paramref name="key"/> is non-null before call.</remarks>
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
		/// Adds an <c><see cref="ConvertEvent"/></c> for <c>Color</c> to
		/// <c><see cref="_converters"/></c>.
		/// </summary>
		internal static void InitializeConverters()
		{
			_converters[typeof(Color)] = new ConvertEvent(GetColorString);
		}

		/// <summary>
		/// Converts an <c>object</c> to a <c>string</c> for output to
		/// "MapOptions.Cfg".
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
		/// Converts an <c>(object)Color</c> to a <c>string</c> for output to
		/// "MapOptions.Cfg".
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
		/// Creates an <c><see cref="Option"/></c> w/ a specified (default)
		/// value.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="default">default value of the property</param>
		/// <param name="changer">pointer to a handler that subscribes to the
		/// <c><see cref="OptionChangedEvent"/></c> - do not pass <c>null</c></param>
		/// <remarks>There is no error-handling so don't foff it.</remarks>
		internal void CreateOptionDefault(
				string key,
				object @default,
				OptionChangedEvent changer)
		{
			//DSShared.Logfile.Log("Options.CreateOptionDefault()");
			//DSShared.Logfile.Log(". key= " + key);
			//DSShared.Logfile.Log(". default= " + @default);
			//DSShared.Logfile.Log(". changer= " + changer);

			var option = new Option(@default);

//			if (changer != null) // safety.
			option.OptionChanged += changer;

			_options[key] = option;
		}

		/// <summary>
		/// Writes this Options to 'MapOptionsFile' = "MapOptionsFile" -> MapOptions.cfg
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
