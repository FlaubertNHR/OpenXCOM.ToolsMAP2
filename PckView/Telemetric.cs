using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

using DSShared;
using DSShared.Windows;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace PckView
{
	/// <summary>
	/// Loads/saves 'registry' info w/ YAML ala "MapViewers.yml".
	/// TODO: Move this to 'DSShared' and implement it for other Viewers.
	/// </summary>
	internal static class Telemetric
	{
		/// <summary>
		/// Positions the window at user-defined coordinates w/ size.
		/// </summary>
		internal static void LoadTelemetric(Form f)
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers);
			if (File.Exists(fileViewers))
			{
				using (var sr = new StreamReader(File.OpenRead(fileViewers)))
				{
					var str = new YamlStream();
					str.Load(sr);

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in nodeRoot.Children)
					{
						string viewer = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(viewer, RegistryInfo.PckView, StringComparison.Ordinal))
						{
							int x = 0;
							int y = 0;
							int w = 0;
							int h = 0;

							var invariant = CultureInfo.InvariantCulture;

							var keyvals = nodeRoot.Children[new YamlScalarNode(viewer)] as YamlMappingNode;
							foreach (var keyval in keyvals) // NOTE: There is a better way to do this. See TilesetLoader..cTor
							{
								switch (keyval.Key.ToString()) // TODO: Error handling. ->
								{
									case "left":
										x = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "top":
										y = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "width":
										w = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
									case "height":
										h = Int32.Parse(keyval.Value.ToString(), invariant);
										break;
								}
							}

							var rectScreen = Screen.GetWorkingArea(new Point(x, y));
							if (!rectScreen.Contains(x + 200, y + 100)) // check to ensure that PckView is at least partly onscreen.
							{
								x = 100;
								y =  50;
							}

							f.Left = x;
							f.Top  = y;

							f.ClientSize = new Size(w, h);
						}
					}
				}
			}
#if DEBUG
			else
			{
				var rect = Screen.GetWorkingArea(new Point(0,0));
				f.Left = (rect.Width  - f.Width)  / 2;
				f.Top  = (rect.Height - f.Height) / 2 - 25;
			}
#endif
		}

		/// <summary>
		/// Saves the window position and size to YAML.
		/// </summary>
		internal static void SaveTelemetric(Form f)
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers);

			if (File.Exists(fileViewers))
			{
				f.WindowState = FormWindowState.Normal;

				string src = Path.Combine(dirSettings, PathInfo.ConfigViewers);
				string dst = Path.Combine(dirSettings, PathInfo.ConfigViewersOld);

				File.Copy(src, dst, true);

				using (var sr = new StreamReader(File.OpenRead(dst))) // but now use dst as src ->

				using (var fs = new FileStream(src, FileMode.Create)) // overwrite previous viewers-config.
				using (var sw = new StreamWriter(fs))
				{
					bool found = false;

					while (sr.Peek() != -1)
					{
						string line = sr.ReadLine().TrimEnd();

						if (String.Equals(line, RegistryInfo.PckView + ":", StringComparison.Ordinal))
						{
							found = true;

							sw.WriteLine(line);

							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine();
							line = sr.ReadLine(); // heh

							sw.WriteLine("  left: "   + Math.Max(0, f.Location.X));	// =Left
							sw.WriteLine("  top: "    + Math.Max(0, f.Location.Y));	// =Top
							sw.WriteLine("  width: "  + f.ClientSize.Width);		// <- use ClientSize, since Width and Height
							sw.WriteLine("  height: " + f.ClientSize.Height);		// screw up due to the titlebar/menubar area.
						}
						else
							sw.WriteLine(line);
					}

					if (!found)
					{
						sw.WriteLine(RegistryInfo.PckView + ":");

						sw.WriteLine("  left: "   + Math.Max(0, f.Location.X));
						sw.WriteLine("  top: "    + Math.Max(0, f.Location.Y));
						sw.WriteLine("  width: "  + f.ClientSize.Width);
						sw.WriteLine("  height: " + f.ClientSize.Height);
					}
				}
				File.Delete(dst);
			}
		}
	}
}
