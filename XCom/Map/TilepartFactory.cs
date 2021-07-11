using System;
using System.IO;

using DSShared;


namespace XCom
{
	public static class TilepartFactory
	{
		#region Methods (static)
		/// <summary>
		/// Creates an array of <c><see cref="Tilepart">Tileparts</see></c>
		/// for a given terrain.
		/// </summary>
		/// <param name="terrain">the terrain label</param>
		/// <param name="dirTerrain">path to the directory of the Mcdfile</param>
		/// <param name="terid">the id of this <c>Tilepart's</c> terrain in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c>; default
		/// <c>-1</c> if McdView is going to handle the
		/// sprites itself and this <c>Tilepart</c> is not part of a terrainset</param>
		/// <param name="info"><c>true</c> if the
		/// <c><see cref="McdRecord">McdRecords</see></c> need to create preset
		/// strings for <c>MapView.McdInfoF</c></param>
		/// <returns>an array of <c>Tileparts</c></returns>
		public static Tilepart[] CreateTileparts(
				string terrain,
				string dirTerrain,
				int terid = -1,
				bool info = false)
		{
			string pfeMcd = Path.Combine(dirTerrain, terrain + GlobalsXC.McdExt);

			using (var fs = FileService.OpenFile(pfeMcd))
			if (fs != null && CheckMcdLength(fs, pfeMcd))
			{
				var parts = new Tilepart[(int)fs.Length / McdRecord.Length];

				byte[] bindata;

				for (int id = 0; id != parts.Length; ++id)
				{
					bindata = new byte[McdRecord.Length];
					fs.Read(bindata, 0, McdRecord.Length);

					parts[id] = new Tilepart(
										id,
										new McdRecord(bindata, info),
										terid);
				}

				Tilepart part;
				for (int id = 0; id != parts.Length; ++id)
				{
					part = parts[id];
					part.Dead = GetDeadPart(part.Record, parts, terrain, id);
					part.Altr = GetAltrPart(part.Record, parts, terrain, id);
				}

				//Logfile.Log(". ret parts array");
				return parts;
			}
			//Logfile.Log(". ret EMPTY parts array");
			return null;
		}

		/// <summary>
		/// Checks if the length of specified filestream is an even multiple of
		/// <c><see cref="McdRecord.Length">McdRecord.Length</see></c>.
		/// </summary>
		/// <param name="fs"></param>
		/// <param name="pfeMcd"></param>
		/// <returns><c>true</c> if the records do not overflow</returns>
		private static bool CheckMcdLength(Stream fs, string pfeMcd)
		{
			if (((int)fs.Length % McdRecord.Length) != 0)
			{
				using (var f = new Infobox(
										"MCD load error",
										Infobox.SplitString("The file appears to be corrupted."
												+ " Its length is not consistent with MCD data."),
										pfeMcd,
										InfoboxType.Error))
				{
					f.ShowDialog();
				}
				return false;
			}
			return true;
		}


		/// <summary>
		/// Gets the dead-part of a given <c><see cref="McdRecord"/></c>.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parts"></param>
		/// <param name="terrain"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private static Tilepart GetDeadPart(
				McdRecord record,
				Tilepart[] parts,
				string terrain,
				int id)
		{
			if (record.DieTile != 0)
			{
				if (record.DieTile < parts.Length)
					return parts[record.DieTile];

				string warn = "In the MCD file " + terrain
							+ " part #" + id
							+ " has an invalid death part (id #" + record.DieTile
							+ " of " + parts.Length + " records).";

				using (var f = new Infobox(
										"Warning",
										"Invalid dead part",
										Infobox.SplitString(warn),
										InfoboxType.Warn))
				{
					f.ShowDialog();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the altr-part of a given <c><see cref="McdRecord"/></c>.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parts"></param>
		/// <param name="terrain"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private static Tilepart GetAltrPart(
				McdRecord record,
				Tilepart[] parts,
				string terrain,
				int id)
		{
			if (record.Alt_MCD != 0)
			{
				if (record.Alt_MCD < parts.Length)
					return parts[record.Alt_MCD];

				string warn = "In the MCD file " + terrain
							+ " part #" + id
							+ " has an invalid alternate part (id #" + record.Alt_MCD
							+ " of " + parts.Length + " records).";

				using (var f = new Infobox(
										"Warning",
										"Invalid alternate part",
										Infobox.SplitString(warn),
										InfoboxType.Warn))
				{
					f.ShowDialog();
				}
			}
			return null;
		}
		#endregion Methods (static)
	}
}
