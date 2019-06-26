using System.Collections.Generic;
using System.IO;

using MapView.OptionsServices;


namespace MapView.Forms.MainWindow
{
	internal sealed class OptionsManager
	{
		#region Properties
		private readonly Dictionary<string, Options> _options =
					 new Dictionary<string, Options>();
		internal Options this[string key]
		{
			get { return _options[key]; }
			set { _options[key] = value; }
		}
		#endregion Properties


		#region Methods
		internal void Add(string key, Options options)
		{
			_options.Add(key, options);
		}

		/// <summary>
		/// Saves options.
		/// </summary>
		internal void SaveOptions()
		{
			OptionsService.SaveOptions(_options);
		}

		/// <summary>
		/// Loads options specified by the user.
		/// </summary>
		/// <param name="fullpath"></param>
		internal void LoadOptions(string fullpath)
		{
			using (var sr = new StreamReader(fullpath))
			{
				var vars = new Varidia(sr);

				KeyvalPair line;
				while ((line = vars.ReadLine()) != null)
					Options.ReadOptions(vars, line, _options[line.Key]);
			}
		}
		#endregion Methods
	}
}
