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
		#region Fields
		private readonly Dictionary<string, Option> _options =
					 new Dictionary<string, Option>();
		#endregion Fields


		#region Indexers
		/// <summary>
		/// Gets an <c><see cref="Option"/></c> keyed by a specified key.
		/// </summary>
		/// <remarks>Ensure that <paramref name="key"/> is not <c>null</c>
		/// before call.</remarks>
		internal Option this[string key]
		{
			get
			{
				if (_options.ContainsKey(key))
					return _options[key];

				return null;
			}
		}
		#endregion Indexers


		#region Methods (static)
		/// <summary>
		/// Converts an <c>object</c> to a <c>string</c> for output to
		/// 'MapOptions.cfg'.
		/// </summary>
		/// <param name="val">an <c><see cref="Option"/></c> as an <c>object</c></param>
		/// <returns></returns>
		/// <remarks>If <paramref name="val"/> is <c>null</c> this throws when
		/// trying to write 'MapOptions.cfg'.</remarks>
		private static string Convert(object val)
		{
			//DSShared.Logfile.Log("Options.Convert()");

			if (val is Color)
				return GetColorString((Color)val);

			return val.ToString();
		}

		/// <summary>
		/// Converts an <c>(object)Color</c> to a <c>string</c> for output to
		/// 'MapOptions.cfg'.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static string GetColorString(Color color)
		{
			if (!color.IsKnownColor && !color.IsNamedColor && !color.IsSystemColor)
				return color.A + "," + color.R + "," + color.G + "," + color.B;

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
			//DSShared.Logfile.Log("Options.CreateOptionDefault() key= " + key);
			//DSShared.Logfile.Log(". key= " + key);
			//DSShared.Logfile.Log(". default= " + @default);
			//DSShared.Logfile.Log(". changer= " + changer);

			var option = new Option(@default);

//			if (changer != null) // safety. All Options shall subscribe to an OptionChangedEvent
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
			//DSShared.Logfile.Log("Options.WriteOptions()");

			tw.WriteLine(head);
			tw.WriteLine("{");

//			object val; string valstr;
			foreach (string key in _options.Keys)
			{
//				if ((val = this[key].Value) == null)
//				{
//					OptionsManager.error(key, OptionsManager.ERROR_WRITE);
//					valstr = String.Empty;
//				}
//				else
//					valstr = Convert(val);
//
//				tw.WriteLine("\t" + key + ":" + valstr);

				tw.WriteLine("\t" + key + ":" + Convert(this[key].Value));
			}
			tw.WriteLine("}");
		}
		#endregion Methods
	}
}
