using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;


namespace MapView
{
	internal static class OptionsManager
	{
		#region Fields (static)
		/// <summary>
		/// A dictionary that indexes <c><see cref="Options"/></c> by viewer.
		/// The type of <c><see cref="Options"/></c> - ie MainView Options,
		/// TileView Options, TopView Options, or RouteView Options.
		/// </summary>
		private static readonly Dictionary<string, Options> _sections =
							new Dictionary<string, Options>();
		#endregion Fields (static)


		#region Properties (static)
		private static IList<Form> _options = new List<Form>();
		/// <summary>
		/// A <c>List</c> of <c><see cref="OptionsF"/></c> that are instantiated
		/// by each viewer respectively.
		/// </summary>
		/// <remarks>The <c>List</c> is used only to close any open <c>Forms</c>
		/// when MapView quits - such <c>Forms</c> last the lifetime of the app
		/// after they are instantiated.</remarks>
		internal static IList<Form> Options
		{
			get { return _options; }
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Adds <c><see cref="Options"/></c> by key (viewer) to the
		/// types-dictionary.
		/// </summary>
		/// <param name="key">a viewer by string - see
		/// <c><see cref="RegistryInfo"/></c> constants</param>
		/// <param name="val"><c><see cref="Options"/></c></param>
		/// <remarks>Is used by <c><see cref="MainViewF()"/></c> to assign
		/// MainView's options-type and by
		/// <c><see cref="MapView.Forms.MainView.ObserverManager.CreateObservers()">ObserverManager.CreateObservers()</see></c>
		/// to assign TileView's, TopView's, and RouteView's options-types.</remarks>
		internal static void SetOptionsSection(string key, Options val)
		{
			//Logfile.Log("OptionsManager.SetOptionsSection() key= " + key);
			_sections[key] = val;
		}


		/// <summary>
		/// Loads options specified by the user.
		/// </summary>
		/// <param name="pfe">fullpath of the configuration file</param>
		/// <returns>true if the file is found and read</returns>
		internal static bool LoadUserOptions(string pfe)
		{
			//Logfile.Log("OptionsManager.LoadUserOptions() pfe= " + pfe);

			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			using (var sr = new StreamReader(pfe))
			{
				KeyvalPair keyval;
				while ((keyval = Varidia.getKeyvalPair(sr)) != null) // NOTE: These are not keyvals; they are section-headers in the options file.
				{
					if (_sections.ContainsKey(keyval.Key))
						ReadOptions(sr, _sections[keyval.Key]); // NOTE: This reads the options as keyvals.
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Helper for <c><see cref="LoadUserOptions()">LoadUserOptions()</see></c>.
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="options"></param>
		private static void ReadOptions(TextReader tr, Options options)
		{
			//Logfile.Log("OptionsManager.ReadOptions()");
			string key;

			KeyvalPair keyval;
			while ((keyval = Varidia.getKeyvalPair(tr)) != null)
			{
				//Logfile.Log(". " + keyval.Key + ":" + keyval.Value);

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
			//Logfile.Log("OptionsManager.SaveOptions()");

			string pfe = ((PathInfo)SharedSpace.GetShareObject(SharedSpace.MapOptionsFile)).Fullpath; // gfl

			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;
				using (var sw = new StreamWriter(fs))
				{
					foreach (string key in _sections.Keys)
						_sections[key].WriteOptions(key, sw);
				}
			}

			if (!fail && pfeT != pfe)
				FileService.ReplaceFile(pfe);
		}

		/// <summary>
		/// Closes all the <c><see cref="OptionsF"/></c> windows.
		/// </summary>
		internal static void CloseOptions()
		{
			foreach (var f in Options)
				f.Close();
		}


/*		internal const int ERROR_READ  = 0;
		internal const int ERROR_WRITE = 1;
		internal const int ERROR_SET   = 2;

		/// <summary>
		/// Displays an option-error.
		/// </summary>
		/// <param name="key">the key of the <c><see cref="Option"/></c> that
		/// failed</param>
//		/// <param name="write"><c>true</c> if loading 'MapOptions.cfg' or user
//		/// is changing an <c>Option</c></param>
		/// <param name="et">the error type
		/// <list type="bullet">
		/// <item><c><see cref="ERROR_READ"/></c></item>
		/// <item><c><see cref="ERROR_WRITE"/></c></item>
		/// <item><c><see cref="ERROR_SET"/></c></item>
		/// </list></param>
		internal static void error(string key, int et)
		{
			string head;
			switch (et)
			{
				case ERROR_READ:  head = "Option failed to load.";                            break;
				case ERROR_WRITE: head = "Option failed to write to " + PathInfo.CFG_Options; break;
				default:          head = "Option failed.";                                    break; // case ERROR_SET
			}

			using (var ib = new Infobox("Error",
										head,
										key,
										InfoboxType.Error))
			{
				ib.ShowDialog(MainViewF.that);
			}
		} */
		#endregion Methods (static)
	}
}
