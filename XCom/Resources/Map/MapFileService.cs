using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom
{
	/// <summary>
	/// Instantiates a Map with its Routes and Terrains.
	/// </summary>
	public static class MapFileService
	{
		#region Methods
		/// <summary>
		/// Loads a tileset. Called by XCMainWindow.LoadSelectedMap()
		/// @note Check that 'descriptor' is not null before call.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public static MapFileBase LoadTileset(Descriptor descriptor)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("MapFileService.LoadTileset descriptor= " + descriptor);

			string dirMap = Path.Combine(descriptor.BasePath, MapFileChild.MapsDir);
			string pfeMap = Path.Combine(
									dirMap,
									descriptor.Label + MapFileChild.MapExt);
			//LogFile.WriteLine(". pfeMap= " + pfeMap);

			if (File.Exists(pfeMap))
			{
				//LogFile.WriteLine(". . Map file exists");

				var parts = new List<TilepartBase>();

				foreach (string terrain in descriptor.Terrains) // push together the tileparts of all allocated terrains
				{
					var MCD = descriptor.GetTerrainRecords(terrain);
					foreach (Tilepart part in MCD)
						parts.Add(part);
				}

				if (parts.Count != 0)
				{
					if (parts.Count > 256)
					{
						string info = String.Empty;

						int tabsTotal = 0;
						foreach (string terrain in descriptor.Terrains) // do it again ...
						{
							if (terrain.Length > tabsTotal)
								tabsTotal = terrain.Length;
						}
						tabsTotal = (tabsTotal - 1) / 8;

						foreach (string terrain in descriptor.Terrains) // do it again ...
						{
							string stabs = "\t";
							int tabs = (terrain.Length - 1) / 8;
							while (tabs++ < tabsTotal)
								stabs += "\t";

							int records = descriptor.GetRecordCount(terrain);
							info += Environment.NewLine + terrain + stabs + "- " + records;
						}
						info += Environment.NewLine + Environment.NewLine + "total - " + parts.Count;

						MessageBox.Show(
									"MCD records allocated by terrains exceeds 256."
									+ Environment.NewLine
									+ info,
									"Warning",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning,
									MessageBoxDefaultButton.Button1,
									0);
					}

					var RMP = new RouteNodeCollection(descriptor.Label, descriptor.BasePath);
					var MAP = new MapFileChild(
											descriptor,
											parts,
											RMP);
					return MAP;
				}

				//LogFile.WriteLine(". . . descriptor has no terrains");
				MessageBox.Show(
							"There are no terrains allocated or they do not contain MCD records.",
							"Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0);
			}
			else
			{
				//LogFile.WriteLine(". . Mapfile does NOT exist");
				MessageBox.Show(
							"The Mapfile does not exist.",
							"Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0);
			}

			return null;
		}
		#endregion
	}
}
