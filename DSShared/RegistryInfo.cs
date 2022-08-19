using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace DSShared
{
	/// <summary>
	/// A class to help facilitate the saving and loading of values into the
	/// registry in a central location.
	/// </summary>
	/// <remarks>MapView2 does not use the Windows Registry at all - this info
	/// is rather for the settings file "MapViewers.yml". That is think of
	/// "settings\MapViewers.yml" as MapView2's registry.</remarks>
	public static class RegistryInfo
	{
		#region Fields (static)
		public  const string MainView      = "MainView";
		public  const string TileView      = "TileView";
		public  const string TopView       = "TopView";
		public  const string RouteView     = "RouteView";
		private const string TopRouteView  = "TopRouteView";

		private const string TilesetEditor = "TilesetEditor";
		private const string ScanG         = "ScanG";
		private const string ColorHelp     = "ColorHelp";
		private const string McdInfo       = "McdInfo";

		private const string Options       = "Options";

		private const string PckView       = "PckView";
		private const string SpriteEditor  = "SpriteEditor";
		private const string PaletteViewer = "PaletteViewer";

		private const string McdView       = "McdView";
		private const string CopyPanel     = "CopyPanel";


		private const string Left   = "left";
		private const string Top    = "top";
		private const string Width  = "width";
		private const string Height = "height";


		/// <summary>
		/// Path-file-extension of "settings/MapViewers.yml".
		/// </summary>
		private static string _pfe;

		private static readonly Dictionary<string, Metric> _trics =
							new Dictionary<string, Metric>();
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Sets the fullpath to "settings/MapViewers.yml" and loads its values to
		/// MapView2.
		/// </summary>
		/// <param name="dir">MapView2/PckView/McdView application directory</param>
		public static void InitializeRegistry(string dir)
		{
			 dir = Path.Combine(dir, PathInfo.DIR_Settings);
			_pfe = Path.Combine(dir, PathInfo.YML_Viewers);

			LoadMetrics();
		}

		/// <summary>
		/// Loads every <c><see cref="Metric"/></c> in "settings/MapViewers.yml"
		/// to MapView2.
		/// </summary>
		/// <remarks>Although there are 4 Options forms only 1 <c>Metric</c> is
		/// saved/loaded for all.</remarks>
		private static void LoadMetrics()
		{
			using (var fs = FileService.OpenFile(_pfe))
			if (fs != null)
			using (var sr = new StreamReader(fs))
			{
				var ys = new YamlStream();
				ys.Load(sr);

				int val;

				var root = ys.Documents[0].RootNode as YamlMappingNode;
				foreach (var node in root.Children)
				{
					var tric = new Metric();

					string label = (node.Key as YamlScalarNode).Value;

					var keyvals = root.Children[new YamlScalarNode(label)] as YamlMappingNode;
					foreach (var keyval in keyvals)
					{
						if (Int32.TryParse(keyval.Value.ToString(), out val)
							&& val > -1)
						{
							switch (keyval.Key.ToString())
							{
								case Left:   tric.left   = val; break;
								case Top:    tric.top    = val; break;
								case Width:  tric.width  = val; break; // TODO: width/height could be 0 if parse fails
								case Height: tric.height = val; break;
							}
						}
					}

					// check to ensure that the form is at least partly onscreen ->
					// NOTE: Windows will instantiate forms that try to open to
					// the left or top of the screen on the screen; so this is
					// really only valid if a form tries to instantiate too far
					// to the right or bottom of a screen.
					var loc = new Point(tric.left + 200, tric.top + 100);
					bool isInsideBounds = false;
					foreach (var screen in Screen.AllScreens)
					{
						if (isInsideBounds = screen.Bounds.Contains(loc))
							break;
					}

					if (!isInsideBounds)
					{
						tric.left = 100;
						tric.top  =  50;
					}

					_trics.Add(label, tric);
				}
			}
		}


		/// <summary>
		/// Gets the registry-label of a specified Form.
		/// </summary>
		/// <param name="f">a form for which to get the registry-label</param>
		/// <returns>the registry-label or null if not found</returns>
		/// <remarks>This function would be unnecessary if each form's Name
		/// variable had been used as its registry-label. But since they weren't
		/// this function maintains backward compatibility with the toplevel
		/// types in "settings/MapViewers.yml".</remarks>
		public static string GetRegistryLabel(Control f)
		{
			// TODO: MCDInfo, MapInfo, McdRecordsExceeded, etc etc. RouteInfo, RouteCheckInfobox ...
			// TODO: McdView's Spriteset, ScanGset, LoFTset viewers.
			// TODO: PckView's BytesViewer

			switch (f.Name) // <- not a wise practice.
			{
				case "MainViewF":        return MainView;		// is in manifest
				case "TileViewForm":     return TileView;		// is in manifest
				case "TopViewForm":      return TopView;		// is in manifest
				case "RouteViewForm":    return RouteView;		// is in manifest
				case "TopRouteViewForm": return TopRouteView;	// is in manifest
				case "OptionsF":         return Options;		// is in manifest
				case "TilesetEditor":    return TilesetEditor;	// is in manifest
				case "ScanGViewer":      return ScanG;
				case "ColorHelp":        return ColorHelp;
				case "McdInfoF":         return McdInfo;

				case "McdviewF":         return McdView;		// is in manifest
				case "CopyPanelF":       return CopyPanel;

				case "PckViewF":         return PckView;		// is in manifest
				case "SpriteEditorF":    return SpriteEditor;
				case "PaletteF":         return PaletteViewer;
			}
			return null;
		}

		/// <summary>
		/// Properties to be assigned.
		/// </summary>
		/// <param name="f">a <c>Form</c> aka viewer</param>
		/// <returns><c>true</c> if form/viewer is found in the dictionary and
		/// its properties get loaded</returns>
		public static bool RegisterProperties(Form f)
		{
			string label = GetRegistryLabel(f);
			if (label != null && _trics.ContainsKey(label))
			{
				Metric tric = _trics[label];

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
		/// </summary>
		/// <param name="f">a Form aka viewer</param>
		/// <param name="bypassShow"><c>true</c> for Options forms when they are
		/// merely changing their visible state; <c>false</c> to force the form
		/// to show before the registry gets finalized</param>
		/// <remarks>Most forms never actually close until the app exits; almost
		/// all of them merely hide.</remarks>
		public static void UpdateRegistry(Form f, bool bypassShow = false)
		{
			string label = GetRegistryLabel(f);
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

				if (!_trics.ContainsKey(label))
				{
					tric = new Metric();
				}
				else
					tric = _trics[label];

				tric.left   = Math.Max(0, f.Left);
				tric.top    = Math.Max(0, f.Top);
				tric.width  = f.ClientSize.Width;
				tric.height = f.ClientSize.Height;

				_trics[label] = tric; // yeeahhhhhh riiiiighhthghttt
			}
		}

		/// <summary>
		/// Writes the <c><see cref="Metric"/></c> of all registered forms to
		/// "settings/MapViewers.yml".
		/// </summary>
		/// <remarks>Although there are 4 Options forms only 1 <c>Metric</c> is
		/// saved/loaded for all.</remarks>
		public static void WriteRegistry()
		{
			string pfeT;
			if (File.Exists(_pfe))
				pfeT = _pfe + GlobalsXC.TEMPExt;
			else
				pfeT = _pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;
				using (var sw = new StreamWriter(fs))
				{
					sw.WriteLine("# delete this file if things go wrong with your window locations and/or sizes.");
					sw.WriteLine("# It will be recreated from a hardcoded manifest the next time the program runs.");
					sw.WriteLine("#");
					sw.WriteLine("# NOTE: Do *not* add extra lines or anything to the format; it's not that robust.");
					sw.WriteLine("#");

					Metric tric;
					foreach (var key in _trics.Keys)
					{
						tric = _trics[key];

						sw.WriteLine(key + ":");
						sw.WriteLine("  " + Left   + ": " + tric.left);
						sw.WriteLine("  " + Top    + ": " + tric.top);
						sw.WriteLine("  " + Width  + ": " + tric.width);
						sw.WriteLine("  " + Height + ": " + tric.height);
					}
				}
			}

			if (!fail && pfeT != _pfe)
				FileService.ReplaceFile(_pfe);
		}


		/// <summary>
		/// Checks if the OS wants this shit to stop instantly.
		/// </summary>
		/// <param name="reason"><c><see cref="CloseReason"/></c></param>
		/// <returns><c>true</c> if Windoze is shutting down [or another
		/// application such as TaskManager sends a <c>WM_CLOSE</c> message]</returns>
		/// <remarks>holy erection batrat
		/// 
		/// https://stackoverflow.com/questions/23872921/how-to-reset-the-close-reason-when-close-is-cancelled#answer-23919394</remarks>
		public static bool FastClose(CloseReason reason)
		{
			switch (reason)
			{
				case CloseReason.WindowsShutDown:		// <- accurate
//				case CloseReason.TaskManagerClosing:	// <- might not be TaskManager but can be anything that sends a WM_CLOSE message
					return true;

//				case CloseReason.ApplicationExitCall:	// <- accurate
//				case CloseReason.FormOwnerClosing:		// <- accurate
//				case CloseReason.UserClosing:			// <- not reliable
//				case CloseReason.None:					// <- whatever
//					break;

//				case CloseReason.MdiFormClosing:		// not used by MapView2.
//					break;
			}
			return false;
		}
		#endregion Methods (static)



		#region Structs
		/// <summary>
		/// Contains window-telemetry.
		/// </summary>
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
