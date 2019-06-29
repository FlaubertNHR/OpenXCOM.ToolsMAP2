using System.Collections.Generic;
using System.IO;

using DSShared;


namespace MapView.Forms.MainWindow
{
	internal static class OptionsManager
	{
		#region Fields (static)
		private static readonly Dictionary<string, Options> _optionsTypes =
							new Dictionary<string, Options>();
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Adds an Options-object by (viewer)key to the types-dictionary.
		/// @note Is used by 'XCMainWindow..cTor' to assign MainView's
		/// option-type and by 'ViewersManager.SetAsObserver()' to assign
		/// TileView's, TopView's, and RouteView's option-types.
		/// </summary>
		/// <param name="key">a viewer by string - see 'RegistryInfo' constants</param>
		/// <param name="val">an Options object</param>
		internal static void setOptionsType(string key, Options val)
		{
//			_optionsTypes.Add(key, val); <- this would throw if a duplicate is found.
			_optionsTypes[key] = val; // But since this all happens in MainView's cTor it's not a probl.
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
					_optionsTypes[key].WriteOptions(key, sw);
			}
		}
		#endregion Methods (static)
	}
}
