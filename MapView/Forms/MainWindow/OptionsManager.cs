using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Windows;


namespace MapView.Forms.MainWindow
{
	internal static class OptionsManager
	{
		#region Fields (static)
		private static readonly Dictionary<string, Options> _optionsTypes =
							new Dictionary<string, Options>();
		#endregion Fields (static)


		#region Properties (static)
		private static List<Form> _screens = new List<Form>();
		internal static List<Form> Screens
		{
			get { return _screens; }
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Adds an Options-object by (viewer)key to the types-dictionary.
		/// @note Is used by 'XCMainWindow..cTor' to assign MainView's
		/// options-type and by 'ViewersManager.SetAsObserver()' to assign
		/// TileView's, TopView's, and RouteView's options-types.
		/// </summary>
		/// <param name="key">a viewer by string - see 'RegistryInfo' constants</param>
		/// <param name="val">an Options object</param>
		internal static void setOptionsType(string key, Options val)
		{
			_optionsTypes[key] = val;
		}

		/// <summary>
		/// Gets the Options-object for MainView.
		/// @note Is used by 'MainMenusManager'.
		/// </summary>
		/// <returns></returns>
		internal static Options getMainOptions()
		{
			return _optionsTypes[RegistryInfo.MainView];
		}


		/// <summary>
		/// Loads options specified by the user.
		/// </summary>
		/// <param name="fullpath"></param>
		internal static void LoadUserOptions(string fullpath)
		{
			using (var sr = new StreamReader(fullpath))
			{
				KeyvalPair keyval;
				while ((keyval = Varidia.getKeyvalPair(sr)) != null) // NOTE: These are not keyvals; they are headers in the options file.
				{
					if (_optionsTypes.ContainsKey(keyval.Key))
						ReadOptions(sr, _optionsTypes[keyval.Key]); // NOTE: This reads the options as keyvals.
				}
			}
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


		/// <summary>
		/// Closes all the Options screens.
		/// </summary>
		internal static void CloseScreens()
		{
			foreach (var screen in Screens)
				screen.Close();
		}
		#endregion Methods (static)
	}
}
