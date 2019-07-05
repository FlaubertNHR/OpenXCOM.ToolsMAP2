using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace DSShared.Windows
{
	/// <summary>
	/// A class to help facilitate the saving and loading of values into the
	/// registry in a central location.
	/// NOTE: MapViewII does not use the Windows Registry at all - this info is
	/// rather for the settings file "MapViewers.yml". That is, think of
	/// "settings\MapViewers.yml" as MapViewII's registry.
	/// </summary>
	public static class RegistryInfo
	{
		#region Fields (static)
		public const string MainWindow    = "MainWindow"; // ... is for old parsing Options routine.

		public const string MainView      = "MainView";
		public const string TileView      = "TileView";
		public const string TopView       = "TopView";
		public const string RouteView     = "RouteView";
		public const string TopRouteView  = "TopRouteView";

		public const string TilesetEditor = "TilesetEditor";
		public const string ScanG         = "ScanG";

		public const string Options       = "Options";

		public const string PckView       = "PckView";
		public const string SpriteEditor  = "SpriteEditor";
		public const string PaletteViewer = "PaletteViewer";

		public const string McdView       = "McdView";
		public const string CopyPanel     = "CopyPanel";


		private const string LEFT   = "left";
		private const string TOP    = "top";
		private const string WIDTH  = "width";
		private const string HEIGHT = "height";


		private static string _pfe;
		private static string _pfeT;

		private static readonly Dictionary<string, Metric> _dict =
							new Dictionary<string, Metric>();
		#endregion Fields (static)


		#region Methods (static)
		public static void InitializeRegistry(string dir)
		{
			dir   = Path.Combine(dir, PathInfo.SettingsDirectory);

			_pfe  = Path.Combine(dir, PathInfo.ConfigViewers);
			_pfeT = Path.Combine(dir, PathInfo.ConfigViewersT);

			LoadMetrics();
		}

		/// <summary>
		/// @note Although there are 4 Options forms only 1 metric is saved or
		/// loaded for all.
		/// </summary>
		private static void LoadMetrics()
		{
			if (File.Exists(_pfe))
			{
				using (var sr = new StreamReader(File.OpenRead(_pfe)))
				{
					var invariant = CultureInfo.InvariantCulture;

					var ys = new YamlStream();
					ys.Load(sr);

					var root = ys.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in root.Children)
					{
						string label = (node.Key as YamlScalarNode).Value;

						var tric = new Metric();

						var keyvals = root.Children[new YamlScalarNode(label)] as YamlMappingNode;
						foreach (var keyval in keyvals) // Cf. TilesetLoader..cTor
						{
							int val = Int32.Parse(keyval.Value.ToString(), invariant); // TODO: Error handling.
							switch (keyval.Key.ToString())
							{
								case LEFT:   tric.left   = val; break;
								case TOP:    tric.top    = val; break;
								case WIDTH:  tric.width  = val; break;
								case HEIGHT: tric.height = val; break;
							}
						}

						// check to ensure that the form is at least partly onscreen ->
						var rect = Screen.GetWorkingArea(new Point(tric.left, tric.top));
						if (!rect.Contains(tric.left + 200, tric.top + 100))
						{
							tric.left = 100;
							tric.top  =  50;
						}

						_dict.Add(label, tric);
					}
				}
			}
		}


		/// <summary>
		/// Gets the registry-label of a specified Form.
		/// @note This function would be unnecessary if each form's Name
		/// variable had been used as its registry-label. But since they weren't
		/// this function maintains backward compatibility with the property-
		/// headers in "MapViewers.yml".
		/// TODO: MCDInfo, etc.
		/// TODO: McdView's Spriteset, ScanGset, LoFTset viewers.
		/// TODO: PckView's BytesViewer
		/// </summary>
		/// <param name="f">a form for which to get the registry-label</param>
		/// <returns>the registry-label or null if not found</returns>
		public static string getRegistryLabel(Control f)
		{
			switch (f.Name)
			{
				case "XCMainWindow":     return MainView;		// is in manifest
				case "TileViewForm":     return TileView;		// is in manifest
				case "TopViewForm":      return TopView;		// is in manifest
				case "RouteViewForm":    return RouteView;		// is in manifest
				case "TopRouteViewForm": return TopRouteView;	// is in manifest
				case "OptionsForm":      return Options;		// is in manifest
				case "TilesetEditor":    return TilesetEditor;	// is in manifest
				case "ScanGViewer":      return ScanG;
				case "McdviewF":         return McdView;		// is in manifest
				case "CopyPanelF":       return CopyPanel;
				case "PckViewForm":      return PckView;		// is in manifest
				case "EditorForm":       return SpriteEditor;
				case "PaletteForm":      return PaletteViewer;
			}
			return null;
		}

		/// <summary>
		/// Properties to be assigned.
		/// </summary>
		/// <param name="f">a Form aka viewer</param>
		/// <returns>true if form/viewer is found in the dictionary and its
		/// properties get loaded</returns>
		public static bool RegisterProperties(Form f)
		{
			string label = getRegistryLabel(f);
			if (label != null && _dict.ContainsKey(label))
			{
				Metric tric = _dict[label];

				f.Left = tric.left;
				f.Top  = tric.top;
				f.ClientSize = new Size(
									tric.width,
									tric.height);
				return true;
			}
			return false;
			// There. Now isn't that clever.
		}


		/// <summary>
		/// Updates the dictionary's properties when a specified Form closes.
		/// @note Most forms never actually close until the app exits; almost
		/// all of them merely hide.
		/// </summary>
		/// <param name="f">a Form aka viewer</param>
		/// <param name="bypassShow">true for Options forms when they are merely
		/// changing their visible state; false to force the form to show before
		/// the registry gets finalized</param>
		public static void UpdateRegistry(Form f, bool bypassShow = false)
		{
			string label = getRegistryLabel(f);
			if (label != null)
			{
				if (!bypassShow)
				{
					if (!f.Visible)
						f.Show();	// need that since restoring the WindowState
									// of a non-visible form doesn't stick.

					f.WindowState = FormWindowState.Normal;
				}


				Metric tric;

				if (!_dict.ContainsKey(label))
				{
					tric = new Metric();
				}
				else
					tric = _dict[label];

				tric.left   = Math.Max(0, f.Left);
				tric.top    = Math.Max(0, f.Top);
				tric.width  = f.ClientSize.Width;
				tric.height = f.ClientSize.Height;

				_dict[label] = tric; // yeeahhhhhh riiiiighhthghttt
			}
		}

		/// <summary>
		/// @note Although there are 4 Options forms only 1 metric is saved or
		/// loaded for all.
		/// </summary>
		public static void FinalizeRegistry()
		{
			using (var sw = new StreamWriter(_pfeT))
			{
				sw.WriteLine("# delete this file if things go wrong with your window locations and/or sizes.");
				sw.WriteLine("# It will be recreated from a hardcoded manifest the next time the program runs.");
				sw.WriteLine("#");
				sw.WriteLine("# NOTE: Do *not* add extra lines or anything to the format; it's not that");
				sw.WriteLine("# robust.");
				sw.WriteLine("#");

				foreach (var key in _dict.Keys)
				{
					Metric tric = _dict[key];

					sw.WriteLine(key + ":");
					sw.WriteLine("  " + LEFT   + ": " + tric.left);
					sw.WriteLine("  " + TOP    + ": " + tric.top);
					sw.WriteLine("  " + WIDTH  + ": " + tric.width);
					sw.WriteLine("  " + HEIGHT + ": " + tric.height);
				}
			}

			if (File.Exists(_pfeT))
				File.Replace(_pfeT, _pfe, null);
			// TODO: Else show error.
		}
		#endregion Methods (static)


		#region Structs
		private struct Metric
		{
			internal int left;
			internal int top;
			internal int width;
			internal int height;
		}
		#endregion Structs
	}
}
