using System.Collections.Generic;
using System.IO;

using DSShared;


namespace MapView.Forms.MainWindow
{
	internal static class OptionsManager
	{
		#region Fields (static)
		internal const string MainWindow = "MainWindow";

		private static readonly Dictionary<string, Options> _optionsTypes =
							new Dictionary<string, Options>();
		#endregion Fields (static)


		#region Methods (static)
		internal static void setOptionsType(string key, Options val)
		{
			_optionsTypes[key] = val;
		}

		/// <summary>
		/// Adds an Options-object by (viewer)key.
		/// </summary>
		/// <param name="key">a Viewer by string: TileView, TopView, RouteView</param>
		/// <param name="val">an Options object</param>
		internal static void Add(string key, Options val)
		{
			_optionsTypes.Add(key, val);
		}


		/// <summary>
		/// Loads options specified by the user.
		/// </summary>
		/// <param name="fullpath"></param>
		internal static void LoadOptions(string fullpath)
		{
			using (var sr = new StreamReader(fullpath))
			{
				KeyvalPair keyval;
				while ((keyval = Varidia.getKeyvalPair(sr)) != null) // NOTE: These are not keyvals; they are headers in the options file.
				{
					Options.ReadOptions(sr, _optionsTypes[keyval.Key]); // NOTE: This reads the options as keyvals.
				}
			}
		}

		/// <summary>
		/// Saves options.
		/// </summary>
		internal static void SaveOptions()
		{
			using (var sw = new StreamWriter(((PathInfo)SharedSpace.GetShareObject(PathInfo.ShareOptions)).Fullpath)) // gfl
			{
				foreach (string key in _optionsTypes.Keys)
					_optionsTypes[key].SaveOptions(key, sw);
			}
		}
		#endregion Methods (static)
	}
}
