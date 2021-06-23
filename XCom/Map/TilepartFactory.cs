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
		/// for a given terrain with a given <c><see cref="Spriteset"/></c>.
		/// </summary>
		/// <param name="terrain">the terrain label</param>
		/// <param name="dirTerrain">path to the directory of the MCD-file</param>
		/// <param name="terid">the id of this <c>Tilepart's</c> terrain in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c></param>
		/// <returns>an array of <c>Tileparts</c></returns>
		internal static Tilepart[] CreateTileparts(
				string terrain,
				string dirTerrain,
				int terid)
		{
			//Logfile.Log("TilepartFactory.CreateTileparts()");

			string pfe = Path.Combine(dirTerrain, terrain + GlobalsXC.McdExt);

			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			{
				var parts = new Tilepart[(int)fs.Length / McdRecord.Length]; // TODO: Error if this don't work out right.

				for (int id = 0; id != parts.Length; ++id)
				{
					var bindata = new byte[McdRecord.Length];
					fs.Read(bindata, 0, McdRecord.Length);

					parts[id] = new Tilepart(
										id,
										new McdRecord(bindata),
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
			return new Tilepart[0];
		}

		/// <summary>
		/// Gets the dead-part of a given <c><see cref="McdRecord"/></c>.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parts"></param>
		/// <param name="terrain"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Tilepart GetDeadPart(
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
										"Invalid death part",
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
		public static Tilepart GetAltrPart(
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
