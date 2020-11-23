using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using DSShared;

using XCom.Base;


namespace XCom
{
	/// <summary>
	/// Instantiates a Map with its Routes and Terrains.
	/// </summary>
	public static class MapFileService
	{
		#region Fields (static)
		public const int MAX_MCDRECORDS = 254; // cf. MapFileBase.MaxTerrainId=253
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Checks if the Mapfile for a specified Descriptor exists.
		/// @note Check (or ensure) that 'descriptor' is valid before call.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns>the path to the Mapfile else null</returns>
		public static string MapfileExists(Descriptor descriptor)
		{
			string dir = descriptor.Basepath;
			if (!String.IsNullOrEmpty(dir)) // -> the BasePath can be null if resource-type is notconfigured.
			{
					   dir = Path.Combine(dir, GlobalsXC.MapsDir);
				string pfe = Path.Combine(dir, descriptor.Label + GlobalsXC.MapExt);

				if (File.Exists(pfe))
					return pfe;
			}
			return null;
		}

		/// <summary>
		/// Loads all routes and terrains for a Map.
		/// @note Called by MainViewF.LoadSelectedDescriptor().
		/// @note Check (or ensure) that 'descriptor' is valid before call.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="treechanged"></param>
		/// <param name="find">true to force the find file dialog</param>
		/// <param name="ignoreRecordsExceeded">true to bypass a potential
		/// RecordsExceeded warning dialog</param>
		/// <returns>null if things go south</returns>
		public static MapFileBase LoadDescriptor(
				Descriptor descriptor,
				ref bool treechanged,
				bool find,
				bool ignoreRecordsExceeded)
		{
			string pfe = MapfileExists(descriptor);

			if (pfe == null // Open a folderbrowser for user to point to a basepath ->
				&& (find || (Control.ModifierKeys & Keys.Shift) == Keys.Shift) // [Shift] to ask for a MapBrowser dialog.
				&& MessageBox.Show(
								"Files not found for : " + descriptor.Label
									+ Environment.NewLine + Environment.NewLine
									+ "Browse to a basepath for the MAP and RMP files ...",
								" Error",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0) == DialogResult.Yes)
			{
				using (var fbd = new FolderBrowserDialog())
				{
					fbd.Description = String.Format(
												CultureInfo.CurrentCulture,
												"Browse to a basepath folder. A valid basepath folder"
													+ " has the subfolders MAPS and ROUTES.");

					if (Directory.Exists(descriptor.Basepath))
						fbd.SelectedPath = descriptor.Basepath;

					// TODO: Check descriptor's Palette and default to Ufo/Tftd Resource dir instead.


					if (fbd.ShowDialog() == DialogResult.OK)
					{
						string dir = Path.Combine(fbd.SelectedPath, GlobalsXC.MapsDir);
							   pfe = Path.Combine(dir, descriptor.Label + GlobalsXC.MapExt);

						if (File.Exists(pfe))
						{
							descriptor.Basepath = fbd.SelectedPath;
							treechanged = true;
						}
						else
							MessageBox.Show(
										descriptor.Label + GlobalsXC.MapExt
											+ " was not found in that basepath.",
										" Error",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error,
										MessageBoxDefaultButton.Button1,
										0);
					}
				}
			}

			if (File.Exists(pfe))
			{
				var parts = new List<Tilepart>();

				ResourceInfo.Spritesets.Clear();

				for (int i = 0; i != descriptor.Terrains.Count; ++i) // push together the tileparts of all allocated terrains
				{
					Tilepart[] records = descriptor.CreateTerrain(i);	// NOTE: calls
					if (records == null)								//     - TilepartFactory.CreateTileparts()
						return null;									//     - ResourceInfo.LoadSpriteset()

					foreach (Tilepart record in records)
						parts.Add(record);
				}

				if (parts.Count != 0)
				{
					if (!ignoreRecordsExceeded && !descriptor.BypassRecordsExceeded // issue warning ->
						&& parts.Count > MAX_MCDRECORDS)
					{
						string text = String.Empty;

						int lengthTotal = 0;
						for (int i = 0; i != descriptor.Terrains.Count; ++i) // do it again ...
						{
							string terrain = descriptor.Terrains[i].Item1;
							if (terrain.Length > lengthTotal)
								lengthTotal = terrain.Length;
						}

						for (int i = 0; i != descriptor.Terrains.Count; ++i) // do it again ...
						{
							string terrain = descriptor.Terrains[i].Item1;
							string st = terrain;

							int length = terrain.Length;
							while (length++ != lengthTotal)
								st += " ";

							text += st + " - "
								  + descriptor.GetRecordCount(i)
								  + Environment.NewLine;
						}
						text += Environment.NewLine + "total - " + parts.Count;

						McdRecordsExceeded.that.SetTexts(descriptor.Label, text);
						McdRecordsExceeded.that.ShowDialog();
					}

					var nodes = new RouteNodeCollection(descriptor.Label, descriptor.Basepath);
					if (nodes.Fail)
					{
						nodes.Fail = false;
						nodes.Nodes.Clear();
					}
					// if Routes fail try to load the Mapfile regardless ->

					var file = new MapFile(
										descriptor,
										parts,
										nodes);

					if (file.Fail) return null;

					return file;
				}

				MessageBox.Show(
							"There are no terrains allocated or they do not contain MCD records.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}

			return null;
		}
		#endregion Methods (static)
	}
}
