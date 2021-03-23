using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;


namespace MapView.Forms.MainView
{
	internal static class OptionsManager
	{
		#region Fields (static)
		/// <summary>
		/// A dictionary that indexes Options by viewer. The type of Options, ie
		/// MainView Options, TileView Options, TopView Options, or RouteView
		/// Options.
		/// </summary>
		private static readonly Dictionary<string, Options> _optionsTypes =
							new Dictionary<string, Options>();
		#endregion Fields (static)


		#region Properties (static)
		private static IList<Form> _views = new List<Form>();
		/// <summary>
		/// A list of the <see cref="OptionsForm">OptionsForms</see> that are
		/// instantiated by each viewer.
		/// </summary>
		/// <remarks>The list is used only to close any open forms when MapView
		/// quits. Such forms are instantiated when called to, then they last
		/// the lifetime of the app.</remarks>
		internal static IList<Form> Views
		{
			get { return _views; }
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Adds an Options-object by key (viewer) to the types-dictionary.
		/// </summary>
		/// <param name="key">a viewer by string - see RegistryInfo constants</param>
		/// <param name="val">an Options object</param>
		/// <remarks>Is used by MainViewF..cTor to assign MainView's
		/// options-type and by ObserverManager.LoadDefaultOptions() to assign
		/// TileView's, TopView's, and RouteView's options-types.</remarks>
		internal static void setOptionsType(string key, Options val)
		{
			_optionsTypes[key] = val;
		}

		/// <summary>
		/// Gets the Options for MainView.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Is used by MenuManager.</remarks>
		internal static Options getMainOptions()
		{
			return _optionsTypes[RegistryInfo.MainView];
		}


		/// <summary>
		/// Loads options specified by the user.
		/// </summary>
		/// <param name="pfe">fullpath of the configuration file</param>
		/// <returns>true if the file is found and read</returns>
		internal static bool LoadUserOptions(string pfe)
		{
			//LogFile.WriteLine("OptionsManager.LoadUserOptions() pfe= " + pfe);

			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			using (var sr = new StreamReader(pfe))
			{
				KeyvalPair keyval;
				while ((keyval = Varidia.getKeyvalPair(sr)) != null) // NOTE: These are not keyvals; they are section-headers in the options file.
				{
					if (_optionsTypes.ContainsKey(keyval.Key))
						ReadOptions(sr, _optionsTypes[keyval.Key]); // NOTE: This reads the options as keyvals.
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Helper for LoadUserOptions().
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="options"></param>
		private static void ReadOptions(TextReader tr, Options options)
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
							options[key].SetValue(key, keyval.Value);

						break;
				}
			}
		}

		/// <summary>
		/// Saves options.
		/// </summary>
		internal static void SaveOptions()
		{
			string pfe = ((PathInfo)SharedSpace.GetShareObject(PathInfo.ShareOptions)).Fullpath; // gfl

			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			using (var sw = new StreamWriter(fs))
			{
				foreach (string key in _optionsTypes.Keys)
					_optionsTypes[key].WriteOptions(key, sw);
			}

			if (pfeT != pfe)
				FileService.ReplaceFile(pfe);
		}

		/// <summary>
		/// Closes all the Options views.
		/// </summary>
		internal static void CloseOptions()
		{
			foreach (var view in Views)
				view.Close();
		}
		#endregion Methods (static)
	}
}
