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
		#region Fields (static)
		/// <summary>
		/// The maximum count of <c><see cref="McdRecord">McdRecords</see></c>
		/// that a Mapfile can cope with.
		/// </summary>
		/// <seealso cref="MapFile.MaxTerrainId"><c>MapFile.MaxTerrainId</c></seealso>
		public const int MAX_MCDRECORDS = 254;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Loads a <c><see cref="MapFile"/></c> along with all routes and
		/// terrains for a tileset. Called by
		/// <c>MainViewF.LoadSelectedDescriptor()</c>.
		/// </summary>
		/// <param name="descriptor">a <c><see cref="Descriptor"/></c></param>
		/// <param name="browseMapfile"><c>true</c> to force the find Mapfile
		/// dialog - returns <c>true</c> if the Maptree changes</param>
		/// <param name="ignoreRecordsExceeded"><c>true</c> to bypass a
		/// potential RecordsExceeded warning dialog</param>
		/// <param name="routes">current Routes - use this only when reloading
		/// the current Mapfile and want to keep the route-collection as is</param>
		/// <returns><c>null</c> if things go south</returns>
		/// <remarks>Check that <paramref name="descriptor"/> is valid before
		/// call.</remarks>
		public static MapFile LoadDescriptor(
				Descriptor descriptor,
				ref bool browseMapfile,
				bool ignoreRecordsExceeded,
				RouteNodes routes)
		{
			//Logfile.Log("MapFileService.LoadDescriptor()");
			//Logfile.Log(". descriptor.Label= " + descriptor.Label);
			//Logfile.Log(". browseMapfile= " + browseMapfile);
			//Logfile.Log(". ignoreRecordsExceeded= " + ignoreRecordsExceeded);

			string pfe = GetMapfilePath(descriptor);

			if (pfe == null
				&& (browseMapfile || (Control.ModifierKeys & Keys.Shift) == Keys.Shift))	// hold [Shift] to ask for a MapBrowser dialog
			{																				// (only so user doesn't have to click the tree-node twice)
				browseMapfile = false;

				using (var f = new Infobox(
										"Files not found",
										"Browse to a basepath for the MAP and RMP files ...",
										descriptor.Label,
										InfoboxType.Warn,
										InfoboxButtons.CancelOkay))
				{
					if (f.ShowDialog() == DialogResult.OK) // Open a folderbrowser for user to find a basepath ->
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

				//Logfile.Log(". . terraincount= " + descriptor.Terrains.Count);

				PckSprite.ResetOrdinal();
				Tilepart .ResetOrdinal();

				int setid = -1;
				for (int terid = 0; terid != descriptor.Terrains.Count; ++terid) // push together the tileparts of all allocated terrains
				{
					Tilepart[] records = descriptor.CreateTerrain(terid);	// -> TilepartFactory.CreateTileparts()
					if (records == null)									// -> SpritesetManager.LoadSpriteset()
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
				}

				//Logfile.Log("");
				//Logfile.Log(". . partscount= " + parts.Count);

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

					//Logfile.Log(". . . load Routes");

					if (routes == null)
					{
						routes = new RouteNodes(descriptor.Label, descriptor.Basepath);
						if (routes.Fail)
						{
							// if Routes fail load the Mapfile regardless
							// do not null the Routes just clear all nodes

							routes.Fail = false;
							routes.Nodes.Clear();
						}
					}

					var file = new MapFile(
										descriptor,
										parts,
										routes);
					if (!file.Fail)
					{
						//Logfile.Log(". . . ret MapFile");
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

		/// <summary>
		/// Gets the fullpath to the Mapfile for a specified
		/// <c><see cref="Descriptor"/></c>.
		/// </summary>
		/// <param name="descriptor">a <c>Descriptor</c></param>
		/// <returns>the path to the Mapfile else <c>null</c></returns>
		/// <remarks>Check that <paramref name="descriptor"/> is valid before
		/// call.</remarks>
		public static string GetMapfilePath(Descriptor descriptor)
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
		#endregion Methods (static)
	}
}
