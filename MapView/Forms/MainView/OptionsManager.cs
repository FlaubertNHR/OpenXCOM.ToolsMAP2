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
		/// A dictionary that indexes <c><see cref="Options"/></c> by viewer.
		/// The type of <c><see cref="Options"/></c> - ie MainView Options, TileView Options, TopView
		/// Options, or RouteView Options.
		/// </summary>
		private static readonly Dictionary<string, Options> _sections =
							new Dictionary<string, Options>();
		#endregion Fields (static)


		#region Properties (static)
		private static IList<Form> _views = new List<Form>();
		/// <summary>
		/// A list of the <c><see cref="OptionsForm">OptionsForms</see></c> that
		/// are instantiated by each viewer respectively.
		/// </summary>
		/// <remarks>The list is used only to close any open forms when MapView
		/// quits - such forms last the lifetime of the app after they are
		/// instantiated.</remarks>
		internal static IList<Form> Viewers
		{
			get { return _views; }
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
		/// <c><see cref="ObserverManager"/>.LoadDefaultOptions()</c> to assign
		/// TileView's, TopView's, and RouteView's options-types.</remarks>
		internal static void SetOptionsSection(string key, Options val)
		{
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
				foreach (string key in _sections.Keys)
					_sections[key].WriteOptions(key, sw);
			}

			if (pfeT != pfe)
				FileService.ReplaceFile(pfe);
		}

		/// <summary>
		/// Closes all the <c><see cref="OptionsForm"/></c> windows.
		/// </summary>
		internal static void CloseOptions()
		{
			foreach (var view in Viewers)
				view.Close();
		}
		#endregion Methods (static)
	}
}
