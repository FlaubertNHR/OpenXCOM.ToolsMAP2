﻿using System;
using System.IO;

using DSShared;


namespace XCom
{
	public static class TilepartFactory
	{
		#region Methods (static)
		/// <summary>
		/// Creates an array of tileparts from a given terrain and spriteset.
		/// </summary>
		/// <param name="terrain">the terrain file w/out extension</param>
		/// <param name="dirTerrain">path to the directory of the terrain file</param>
		/// <param name="spriteset">a SpriteCollection containing the needed sprites</param>
		/// <returns>an array of Tileparts</returns>
		internal static Tilepart[] CreateTileparts(
				string terrain,
				string dirTerrain,
				SpriteCollection spriteset)
		{
			//LogFile.WriteLine("TilepartFactory.CreateTileparts()");

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
										spriteset);
				}

				Tilepart part;
				for (int id = 0; id != parts.Length; ++id)
				{
					part = parts[id];
					part.Dead = GetDeadPart(part.Record, parts, terrain, id);
					part.Altr = GetAltrPart(part.Record, parts, terrain, id);
				}

				//LogFile.WriteLine(". ret parts array");
				return parts;
			}

			//LogFile.WriteLine(". ret EMPTY parts array");
			return new Tilepart[0];
		}

		/// <summary>
		/// Gets the dead-part of a given MCD-record.
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
				warn = Infobox.SplitString(warn);

				using (var f = new Infobox(
										"Warning",
										"Invalid death part",
										warn,
										Infobox.BoxType.Warn))
				{
					f.ShowDialog();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the altr-part of a given MCD-record.
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
			if (record.Alt_MCD != 0) // || record.HumanDoor || record.UfoDoor
			{
				if (record.Alt_MCD < parts.Length)
					return parts[record.Alt_MCD];

				string warn = "In the MCD file " + terrain
							+ " part #" + id
							+ " has an invalid alternate part (id #" + record.Alt_MCD
							+ " of " + parts.Length + " records).";
				warn = Infobox.SplitString(warn);

				using (var f = new Infobox(
										"Warning",
										"Invalid alternate part",
										warn,
										Infobox.BoxType.Warn))
				{
					f.ShowDialog();
				}
			}
			return null;
		}
		#endregion Methods (static)
	}
}
