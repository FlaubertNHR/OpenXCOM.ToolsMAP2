using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
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
	/// 
	/// TODO: Stop reading and writing "MapViewers.yml" per viewer. Instead,
	/// load its data to a static class-object and maintain it there; doing this
	/// requires instantiating said object when any of MapView, McdView, and/or
	/// PckView load - and writing its data back to "MapViewers.yml" when the
	/// app that instantited it closes. Special consideration is likely required
	/// if McdView or PckView is invoked via TileView ....
	/// </summary>
	public sealed class RegistryInfo
	{
		#region Fields (static)
		public const string MainWindow    = "MainWindow"; // ... is for old parsing Options routine.

		public const string MainView      = "MainView";
		public const string TileView      = "TileView";
		public const string TopView       = "TopView";
		public const string RouteView     = "RouteView";
		public const string TopRouteView  = "TopRouteView";

		public const string TilesetEditor = "TilesetEditor";

		public const string Options       = "Options";

		public const string PckView       = "PckView";
		public const string SpriteEditor  = "SpriteEditor";
		public const string PaletteViewer = "PaletteViewer";

		public const string McdView       = "McdView";
//		public const string CopyPanel     = "CopyPanel"; // <- TODO


		private const string PropLeft   = "Left";
		private const string PropTop    = "Top";
		private const string PropWidth  = "Width";
		private const string PropHeight = "Height";


		private static string _pfe;
		private static string _pfeT;
		private static bool _inited;
		#endregion Fields (static)


		#region Fields
		private readonly string _label;
		private readonly Form _f;

		private readonly Dictionary<string, PropertyInfo> _info =
					 new Dictionary<string, PropertyInfo>();

//		private bool _saveOnClose = true;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Uses a specified string as the key to its Form.
		/// IMPORTANT: Ensure that affected Forms have a manual startposition
		/// set in their designers.
		/// </summary>
		/// <param name="label">the label of the Form to save/load</param>
		/// <param name="f">the form-object corresponding to the label</param>
		public RegistryInfo(string label, Form f)
		{
			_label = label;
			_f = f;

			_f.Load        += OnLoad;
			_f.FormClosing += OnFormClosing;
		}
		#endregion cTor


		#region Methods (static)
		public static void setStaticPaths(string dir)
		{
			if (!_inited)	// ie. don't let McdView or PckView re-init
			{				// these if they are invoked via TileView.
				_inited = true;

				dir = Path.Combine(
								dir,
								PathInfo.SettingsDirectory);
				_pfe = Path.Combine(
								dir,
								PathInfo.ConfigViewers);
				_pfeT = Path.Combine(
								dir,
								PathInfo.ConfigViewersOld);
			}
		}

		/// <summary>
		/// All this would have been so much simpler/easier if you'd just used
		/// each form's Name variable instead of arbitrary concoctions. Ditto
		/// regarding all that 'PropertyInfo' Reflection jazz ... I mean, yes
		/// it's a good way to learn it but it's really entirely unneeded in
		/// this app. I mean, you often used 2 or 3 classes when 1 could and
		/// would suffice just as well. You put panels inside panels over and
		/// over. You abused namespace-strings to no end. You even instantiated
		/// an ordinary rectangle like this:
		/// 
		/// var r = new Rectangle(new Point(0,0), new Size(0,0));
		/// 
		/// It's like, whenever you wanted a 10, you put 1+9 or 2+3+5 or 1+6+7-4
		/// ... just write "10". That's all you had to do, just ... "10".
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string getRegistryLabel(Form f)
		{
			switch (f.Name)
			{
				case "McdviewF": return McdView;
			}
			return null;
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Adds properties to be saved/loaded.
		/// </summary>
		public void RegisterProperties()
		{
			//DSLogFile.WriteLine("RegisterProperties");
			PropertyInfo prop;

			string[] keys =
			{
				PropLeft,
				PropTop,
				PropWidth,
				PropHeight
			};

			foreach (string key in keys)
			{
				//DSLogFile.WriteLine(". . key= " + key);
				if ((prop = _f.GetType().GetProperty(key)) != null) // safety.
				{
					//DSLogFile.WriteLine(". . . info= " + info.Name);
					_info[prop.Name] = prop; // set a ref to each metric (x,y,w,h) via Reflection.
				}
				// this is so clever I want to barf all over the keyboard of the
				// person who wrote it.
				// JUST STORE THE x,y,w,h VALUES AND ASSIGN THEM TO THE FORM
				// WHEN IT LOADS! ffs.
				//
				// no offense, Ben. In a way I know what you're doing but Christ.
				// It makes me want to jump off a mountain and get speared a
				// hundred times on the way down ...
			}
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Loads location and size values for the subsidiary viewers to
		/// "settings\MapViewers.yml".
		/// - TileView
		/// - TopView
		/// - RouteView
		/// - TopRouteView
		/// - Options
		/// - TilesetEditor
		/// - SpriteEditor
		/// - PaletteViewer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoad(object sender, EventArgs e)
		{
			if (File.Exists(_pfe))
			{
				using (var sr = new StreamReader(File.OpenRead(_pfe)))
				{
					var file = new YamlStream();
					file.Load(sr);

					var root = (YamlMappingNode)file.Documents[0].RootNode; // TODO: Error handling. ->
					foreach (var node in root.Children)
					{
						string label = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(label, _label, StringComparison.Ordinal))
						{
							LoadWindowMetrics((YamlMappingNode)root.Children[new YamlScalarNode(label)]);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Helper for OnLoad().
		/// </summary>
		/// <param name="keyvals">yaml-mapped keyval pairs</param>
		private void LoadWindowMetrics(YamlMappingNode keyvals)
		{
			//DSLogFile.WriteLine("ImportValues");
			string key;
			int val;

			var cultureInfo = CultureInfo.InvariantCulture;

			foreach (var keyval in keyvals)
			{
				key = cultureInfo.TextInfo.ToTitleCase(keyval.Key.ToString());
				//DSLogFile.WriteLine(". key= " + key);

				val = Int32.Parse(keyval.Value.ToString(), cultureInfo);
				//DSLogFile.WriteLine(". val= " + val);

				var rect = Screen.GetWorkingArea(new System.Drawing.Point(val, 0));

				switch (key) // check to ensure that viewer is at least partly onscreen.
				{
					case PropLeft:
						if (!rect.Contains(val + 200, 0))
							val = 100;
						break;

					case PropTop:
						if (!rect.Contains(0, val + 100))
							val = 50;
						break;
				}
				_info[key].SetValue(_f, val, null); // set each metric (x,y,w,h) via Reflection.
			}
		}

		/// <summary>
		/// Saves location and size values for the subsidiary viewers to
		/// "settings\MapViewers.yml".
		/// - TileView
		/// - TopView
		/// - RouteView
		/// - TopRouteView
		/// - Options
		/// - TilesetEditor
		/// - SpriteEditor
		/// - PaletteViewer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormClosing(object sender, EventArgs e)
		{
			//DSLogFile.WriteLine("OnClose _label= " + _label);

			_f.WindowState = FormWindowState.Normal;

//			if (_saveOnClose)
//			{
			if (File.Exists(_pfe))
			{
				File.Copy(_pfe, _pfeT, true);

				using (var sr = new StreamReader(File.OpenRead(_pfeT)))	// but now use dst as src ->
				using (var fs = new FileStream(_pfe, FileMode.Create))	// overwrite previous config.
				using (var sw = new StreamWriter(fs))
				{
					bool found = false;

					while (sr.Peek() != -1)
					{
						string line = sr.ReadLine().TrimEnd();
						//DSLogFile.WriteLine(". line= " + line);

						// At present, MainView and McdView and PckView are the
						// only viewers that roll their own metrics.
						// - see the XCMainWindow cTor & FormClosing eventcalls.
						// - see the McdviewF     cTor & FormClosing eventcalls.
						// - see the PckViewForm  cTor & FormClosing eventcalls.

						if (String.Equals(line, _label + ":", StringComparison.Ordinal))
						{
							found = true;

							//DSLogFile.WriteLine(". . write= " + line);
							sw.WriteLine(line);

							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine(); // heh

							sw.WriteLine("  left: "   + Math.Max(0, _f.Location.X)); // =Left
							sw.WriteLine("  top: "    + Math.Max(0, _f.Location.Y)); // =Top
							sw.WriteLine("  width: "  + _f.Width); // TODO: Use ClientSize.Width/Height instead of form width/height
							sw.WriteLine("  height: " + _f.Height);
						}
						else
							sw.WriteLine(line);
					}

					if (!found)
					{
						sw.WriteLine(_label + ":");

						sw.WriteLine("  left: "   + Math.Max(0, _f.Location.X)); // =Left
						sw.WriteLine("  top: "    + Math.Max(0, _f.Location.Y)); // =Top
						sw.WriteLine("  width: "  + _f.Width); // TODO: Use ClientSize.Width/Height instead of form width/height
						sw.WriteLine("  height: " + _f.Height);
					}
				}
				File.Delete(_pfeT);
			}
//			}
		}
		#endregion Events
	}
}
