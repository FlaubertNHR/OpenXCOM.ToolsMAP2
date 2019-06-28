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
	/// rather for initialization/configuration files.
	/// </summary>
	public sealed class RegistryInfo
	{
		#region Fields (static)
		// viewer labels (keys)
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

		// viewer property metrics
		private const string PropLeft   = "Left";
		private const string PropTop    = "Top";
		private const string PropWidth  = "Width";
		private const string PropHeight = "Height";
		#endregion Fields (static)


		#region Fields
		private readonly string _viewer;
		private readonly Form _f;

		private readonly Dictionary<string, PropertyInfo> _info =
					 new Dictionary<string, PropertyInfo>();

//		private bool _saveOnClose = true;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Uses the specified string as a key to its Form.
		/// </summary>
		/// <param name="viewer">the label of the viewer to save/load</param>
		/// <param name="f">the form-object corresponding to the label</param>
		public RegistryInfo(string viewer, Form f)
		{
			_viewer = viewer;

			_f = f;
			_f.StartPosition = FormStartPosition.Manual;
			_f.Load         += OnLoad;
			_f.FormClosing  += OnFormClosing;
		}
		#endregion cTor


		#region Methods (static)
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
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string pfeViewers  = Path.Combine(
											dirSettings,
											PathInfo.ConfigViewers);
			if (File.Exists(pfeViewers))
			{
				using (var sr = new StreamReader(File.OpenRead(pfeViewers)))
				{
					var fileViewers = new YamlStream();
					fileViewers.Load(sr);

					var nodeRoot = (YamlMappingNode)fileViewers.Documents[0].RootNode; // TODO: Error handling. ->
					foreach (var node in nodeRoot.Children)
					{
						string viewer = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(viewer, _viewer, StringComparison.Ordinal))
						{
							LoadWindowMetrics((YamlMappingNode)nodeRoot.Children[new YamlScalarNode(viewer)]);
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

				var rectScreen = Screen.GetWorkingArea(new System.Drawing.Point(val, 0));

				switch (key) // check to ensure that viewer is at least partly onscreen.
				{
					case PropLeft:
						if (!rectScreen.Contains(val + 200, 0))
							val = 100;
						break;

					case PropTop:
						if (!rectScreen.Contains(0, val + 100))
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
			//DSLogFile.WriteLine("OnClose _viewer= " + _viewer);

			_f.WindowState = FormWindowState.Normal;

//			if (_saveOnClose)
//			{
			string dirSettings   = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string pfeViewers    = Path.Combine(
											dirSettings,
											PathInfo.ConfigViewers);
			string pfeViewersOld = Path.Combine(
											dirSettings,
											PathInfo.ConfigViewersOld);

			if (File.Exists(pfeViewers))
			{
				File.Copy(pfeViewers, pfeViewersOld, true);

				using (var sr = new StreamReader(File.OpenRead(pfeViewersOld)))	// but now use dst as src ->
				using (var fs = new FileStream(pfeViewers, FileMode.Create))	// overwrite previous config.
				using (var sw = new StreamWriter(fs))
				{
					bool found = false;

					while (sr.Peek() != -1)
					{
						string line = sr.ReadLine().TrimEnd();
						//DSLogFile.WriteLine(". line= " + line);

						// At present, MainView and PckView are the only viewers that roll their own metrics.
						// - see the XCMainWindow cTor & FormClosing eventcalls.
						// - see the PckViewForm  cTor & FormClosing eventcalls.
						// + McdView ...

						if (String.Equals(line, _viewer + ":", StringComparison.Ordinal))
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
							sw.WriteLine("  width: "  + _f.Width);
							sw.WriteLine("  height: " + _f.Height);
						}
						else
							sw.WriteLine(line);
					}

					if (!found)
					{
						sw.WriteLine(_viewer + ":");

						sw.WriteLine("  left: "   + Math.Max(0, _f.Location.X)); // =Left
						sw.WriteLine("  top: "    + Math.Max(0, _f.Location.Y)); // =Top
						sw.WriteLine("  width: "  + _f.Width);
						sw.WriteLine("  height: " + _f.Height);
					}
				}
				File.Delete(pfeViewersOld);
			}
//			}
		}
		#endregion Events
	}
}
