using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	/// <summary>
	/// Instantiates a <c><see cref="MapFile"/></c> with its Routes and
	/// Terrains.
	/// </summary>
	public static class MapFileService
	{
		#region Methods (static)
		/// <summary>
		/// Loads a <c><see cref="MapFile"/></c> along with all routes and
		/// terrains for a tileset. Called by
		/// <c>MainViewF.LoadSelectedDescriptor()</c>.
		/// </summary>
		/// <param name="descriptor">a <c><see cref="Descriptor"/></c></param>
		/// <param name="browseMapfile"><c>true</c> to force a MapBrowserDialog
		/// - returns <c>true</c> if the Maptree changes</param>
		/// <param name="ignoreRecordsExceeded"><c>true</c> to bypass a
		/// potential RecordsExceeded warning dialog</param>
		/// <param name="routes"><c><see cref="RouteNodes"/></c> - use
		/// this only when reloading a Mapfile and want to keep the
		/// <c><see cref="MapFile.Routes">MapFile.Routes</see></c> intact</param>
		/// <param name="selected">a <c>TreeNode</c> for MapBrowserDialog info
		/// only</param>
		/// <param name="floorsvisible"><c>true</c> to calculate occultations -
		/// <c>false</c> to clear all occultations</param>
		/// <returns><c>null</c> if things go south</returns>
		/// <remarks>Check that <paramref name="descriptor"/> is valid before
		/// call.</remarks>
		public static MapFile LoadDescriptor(
				Descriptor descriptor,
				ref bool browseMapfile,
				bool ignoreRecordsExceeded,
				RouteNodes routes,
				TreeNode selected,
				bool floorsvisible)
		{
			//Logfile.Log("MapFileService.LoadDescriptor()");
			//Logfile.Log(". descriptor.Label= " + descriptor.Label);
			//Logfile.Log(". browseMapfile= " + browseMapfile);
			//Logfile.Log(". ignoreRecordsExceeded= " + ignoreRecordsExceeded);

			string pfe = descriptor.GetMapfilePath();

			if (pfe == null
				&& (browseMapfile || (Control.ModifierKeys & Keys.Shift) != 0))
			{
				browseMapfile = false;

				string copyable = "group:    " + selected.Parent.Parent.Text + Environment.NewLine
								+ "category: " + selected.Parent.Text        + Environment.NewLine
								+ "tileset:  " + descriptor.Label            + Environment.NewLine + Environment.NewLine
								+ "basepath: " + descriptor.Basepath;

				using (var f = new Infobox(
										"Files not found",
										"Browse to a basepath for the MAP and RMP files ...",
										copyable,
										InfoboxType.Warn,
										InfoboxButton.CancelOkay))
				{
					if (f.ShowDialog() == DialogResult.OK) // Open a folderbrowser for user to set 'descriptor.Basepath' ->
					{
						using (var fbd = new FolderBrowserDialog())
						{
							fbd.Description = "Browse to a basepath folder. A valid basepath folder"
											+ " has the subfolders MAPS and ROUTES.";

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
									descriptor.FileValid = true;
									browseMapfile = true;

									//Logfile.Log(". . treechanged= " + treechanged);
								}
								else
								{
									using (var f1 = new Infobox(
															"Error",
															"File not found in that basepath.",
															descriptor.Label + GlobalsXC.MapExt,
															InfoboxType.Error))
									{
										f1.ShowDialog();
									}
								}
							}
						}
					}
				}
			}
			else
				browseMapfile = false;

			//Logfile.Log("");

			if (File.Exists(pfe))
			{
				SpritesetManager.Dispose();

				var parts = new List<Tilepart>();
				var partCounts = new List<int>(); // for TerrainSwapDialog.

				//Logfile.Log(". . terraincount= " + descriptor.Terrains.Count);

				PckSprite.ResetOrdinal();
				Tilepart .ResetOrdinal();

				Tilepart[] records;

				int setid = -1;
				for (int terid = 0; terid != descriptor.Terrains.Count; ++terid) // push together the tileparts of all allocated terrains
				{
					records = descriptor.CreateTerrain(terid);	// -> TilepartFactory.CreateTileparts()
					if (records == null)						// -> SpritesetManager.CreateSpriteset()
					{
						//Logfile.Log(". . . . no records ABORT");
						// TODO: dispose any created spritesets
						return null;
					}

					foreach (Tilepart record in records)
					{
						record.SetId = ++setid;
						parts.Add(record);
					}
					partCounts.Add(records.Length);
				}

				//Logfile.Log("");
				//Logfile.Log(". . partscount= " + parts.Count);

				if (parts.Count != 0)
				{
					if (!ignoreRecordsExceeded && !descriptor.BypassRecordsExceeded // issue warning ->
						&& parts.Count > MapFile.MaxMcdRecords)
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

					//Logfile.Log(". . . load Routes");

					if (routes == null
						&& (routes = new RouteNodes(descriptor.Label, descriptor.Basepath)).Fail)
					{
						// if Routes fail load the Mapfile regardless
						// do not null the Routes just clear all nodes

						routes.Fail = false;
						routes.Nodes.Clear();
					}

					var file = new MapFile(
										descriptor,
										parts,
										routes,
										floorsvisible);
					if (!file.Fail)
					{
						//Logfile.Log(". . . ret MapFile");
						file.PartCounts = partCounts.ToArray();
						return file;
					}

					//Logfile.Log(". . . MapFile FAILED");
					return null;
				}

				//Logfile.Log(". . MCD Error");
				using (var f = new Infobox(
										"Error",
										"There are no terrains allocated or they do not contain MCD records.",
										null,
										InfoboxType.Error))
				{
					f.ShowDialog();
				}
			}

			//Logfile.Log(". ret null Descriptor");
			return null;
		}
		#endregion Methods (static)
	}
}
