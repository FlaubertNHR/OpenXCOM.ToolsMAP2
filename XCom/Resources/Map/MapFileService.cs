using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using XCom.Base;


namespace XCom
{
	/// <summary>
	/// Instantiates a Map with its Routes and Terrains.
	/// </summary>
	public static class MapFileService
	{
		#region Fields (static)
		public const int MAX_MCDRECORDS = 254;
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
		/// <param name="basepathDialog">true to force the find file dialog</param>
		/// <returns>null if things go south</returns>
		public static MapFileBase LoadDescriptor(
				Descriptor descriptor,
				ref bool treechanged,
				bool basepathDialog = false)
		{
			string pfe = MapfileExists(descriptor);

			if (pfe == null // Open a folderbrowser for user to point to a basepath ->
				&& (basepathDialog || (Control.ModifierKeys & Keys.Shift) == Keys.Shift) // [Shift] to ask for a MapBrowser dialog.
				&& MessageBox.Show(
								"Files were not found for : " + descriptor.Label
									+ Environment.NewLine + Environment.NewLine
									+ "Browse for a basepath to the MAP and RMP files ...",
								" Warning",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button1,
								0) == DialogResult.Yes)
			{
				using (var fbd = new FolderBrowserDialog())
				{
					string basepath = descriptor.Basepath;
					if (!String.IsNullOrEmpty(basepath) && Directory.Exists(basepath))
					{
						fbd.SelectedPath = basepath;
					}
					// TODO: Check descriptor's Palette and default to Ufo/Tftd Resource dir instead.

					fbd.Description = String.Format(
												CultureInfo.CurrentCulture,
												"Browse to a basepath folder. A valid basepath folder"
													+ " has the subfolders MAPS and ROUTES.");

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
				var partset = new List<Tilepart>();

				ResourceInfo.Spritesets.Clear();

				for (int i = 0; i != descriptor.Terrains.Count; ++i) // push together the tileparts of all allocated terrains
				{
					Tilepart[] MCD = descriptor.CreateTerrain(i);	// NOTE: calls
					if (MCD == null)								//     - TilepartFactory.CreateTileparts()
						return null;								//     - ResourceInfo.LoadSpriteset()

					foreach (Tilepart part in MCD)
						partset.Add(part);
				}

				if (partset.Count != 0)
				{
					if (partset.Count > MAX_MCDRECORDS) // issue warning ->
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
						text += Environment.NewLine + "total - " + partset.Count;

						McdRecordsExceeded.that.SetTexts(descriptor.Label, text);
						McdRecordsExceeded.that.ShowDialog();
					}

					var RMP = new RouteNodeCollection(descriptor.Label, descriptor.Basepath);
					if (RMP.Fail)
					{
						RMP.Fail = false;
						RMP.Nodes.Clear();
					}
					// if Routes fail try to load the Mapfile regardless ->

					var MAP = new MapFile(
										descriptor,
										partset,
										RMP);

					if (MAP.Fail) return null;

					return MAP;
				}

				//LogFile.WriteLine(". . . descriptor has no terrains");
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
