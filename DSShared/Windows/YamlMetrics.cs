using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace DSShared.Windows
{
	/// <summary>
	/// A static class to Get/Set window locations and sizes from the
	/// /settings subfolder. Also accesses the UFO/TFTD resource paths in the
	/// Configurator.
	/// </summary>
	public static class YamlMetrics
	{
		#region Load/Save 'registry' info
		/// <summary>
		/// Reads a specified form's location and size from YAML.
		/// </summary>
		/// <param name="f"></param>
		public static void Load(Form f)
		{
			string pfe = Path.Combine(
									Path.GetDirectoryName(Application.ExecutablePath),
									PathInfo.SettingsDirectory);
			pfe = Path.Combine(pfe, PathInfo.ConfigViewers); // "MapViewers.yml"

			if (File.Exists(pfe))
			{
				using (var sr = new StreamReader(File.OpenRead(pfe)))
				{
					var str = new YamlStream();
					str.Load(sr);

					string label = RegistryInfo.getRegistryLabel(f);

					var invariant = System.Globalization.CultureInfo.InvariantCulture;

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in nodeRoot.Children)
					{
						string viewer = ((YamlScalarNode)node.Key).Value;
						if (String.Equals(viewer, label, StringComparison.Ordinal))
						{
							int x = 0;
							int y = 0;
							int w = 0;
							int h = 0;

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

							var rectScreen = Screen.GetWorkingArea(new Point(x,y));
							if (!rectScreen.Contains(x + 200, y + 100)) // check to ensure that the form is at least partly onscreen.
							{
								x = 100;
								y =  50;
							}

							f.Left = x;
							f.Top  = y;

							f.ClientSize = new Size(w,h);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves a specified form's location and size to YAML.
		/// </summary>
		/// <param name="f"></param>
		public static void Save(Form f)
		{
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileViewers = Path.Combine(dirSettings, PathInfo.ConfigViewers); // "MapViewers.yml"

			if (File.Exists(fileViewers))
			{
				f.WindowState = FormWindowState.Normal;

				string label = RegistryInfo.getRegistryLabel(f);

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

						if (String.Equals(line, label + ":", StringComparison.Ordinal))
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
							sw.WriteLine("  height: " + f.ClientSize.Height);		// screw up due to the titlebar/border area.
						}
						else
							sw.WriteLine(line);
					}

					if (!found)
					{
						sw.WriteLine(label + ":");

						sw.WriteLine("  left: "   + Math.Max(0, f.Location.X));
						sw.WriteLine("  top: "    + Math.Max(0, f.Location.Y));
						sw.WriteLine("  width: "  + f.ClientSize.Width);
						sw.WriteLine("  height: " + f.ClientSize.Height);
					}
				}
				File.Delete(dst);
			}
		}
		#endregion Load/Save 'registry' info


		/// <summary>
		/// Assigns MapView's Configurator's basepath(s) to 'pathufo' and
		/// 'pathtftd'.
		/// </summary>
		/// <param name="pathufo"></param>
		/// <param name="pathtftd"></param>
		public static void GetResourcePaths(out string pathufo, out string pathtftd)
		{
			pathufo = pathtftd = null;

			// First check the current Terrain's basepath ...
//			string path = Path.GetDirectoryName(_pfeMcd);
//			if (path.EndsWith(GlobalsXC.TerrainDir, StringComparison.InvariantCulture))
//			{
//				path = path.Substring(0, path.Length - GlobalsXC.TerrainDir.Length + 1);
//				return Path.Combine(path, SharedSpace.ScanGfile);
//			}

			// Second check the Configurator's basepath ...
			string dirSettings = Path.Combine(
											Path.GetDirectoryName(Application.ExecutablePath),
											PathInfo.SettingsDirectory);
			string fileResources = Path.Combine(dirSettings, PathInfo.ConfigResources);
			if (File.Exists(fileResources))
			{
				using (var sr = new StreamReader(File.OpenRead(fileResources)))
				{
					var str = new YamlStream();
					str.Load(sr);

					string val;

					var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
					foreach (var node in nodeRoot.Children)
					{
						switch (node.Key.ToString())
						{
							case "ufo":
								if ((val = node.Value.ToString()) != PathInfo.NotConfigured)
									pathufo = val;

								break;

							case "tftd":
								if ((val = node.Value.ToString()) != PathInfo.NotConfigured)
									pathtftd = val;

								break;
						}
					}
				}
			}

			// Third let the user load ScanG.Dat/LoFT.Dat files from menuitems.
		}
	}
}
