using System;
using System.Globalization;
//using System.Windows.Forms;
using System.IO;


namespace XCom.Resources.Map
{
	public static class TilepartFactory
	{
		#region Fields (static)
		public const int Length = 62; // there are 62 bytes in each MCD record.
		#endregion


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
			if (spriteset != null)
			{
				string pfeMcd = Path.Combine(dirTerrain, terrain + GlobalsXC.McdExt);

				if (!File.Exists(pfeMcd))
				{
					using (var f = new Infobox(
											" File not found",
											"Can't find file with MCD records.",
											pfeMcd))
					{
						f.ShowDialog();
					}
				}
				else
				{
					using (var bs = new BufferedStream(File.OpenRead(pfeMcd)))
					{
						var parts = new Tilepart[(int)bs.Length / Length]; // TODO: Error if this don't work out right.

						for (int id = 0; id != parts.Length; ++id)
						{
							var bindata = new byte[Length];
							bs.Read(bindata, 0, Length);

							parts[id] = new Tilepart(
												id,
												spriteset,
												new McdRecord(bindata));
						}

						Tilepart part;
						for (int id = 0; id != parts.Length; ++id)
						{
							part = parts[id];
							part.Dead = GetDeadPart(terrain, id, part.Record, parts);
							part.Altr = GetAltrPart(terrain, id, part.Record, parts);
						}

						return parts;
					}
				}
			}
			return new Tilepart[0];
		}

		/// <summary>
		/// Gets the dead-part of a given MCD-record.
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="id"></param>
		/// <param name="record"></param>
		/// <param name="parts"></param>
		/// <returns></returns>
		public static Tilepart GetDeadPart(
				string terrain,
				int id,
				McdRecord record,
				Tilepart[] parts)
		{
			if (record.DieTile != 0)
			{
				if (record.DieTile < parts.Length)
					return parts[record.DieTile];

				string warn = String.Format(
										CultureInfo.CurrentCulture,
										"In the MCD file {0}, part #{1} has an invalid death part (id #{2} of {3} records).",
										terrain,
										id,
										record.DieTile,
										parts.Length);
				using (var f = new Infobox(
										" Invalid death part",
										warn,
										null))
				{
					f.ShowDialog();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the altr-part of a given MCD-record.
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="id"></param>
		/// <param name="record"></param>
		/// <param name="parts"></param>
		/// <returns></returns>
		public static Tilepart GetAltrPart(
				string terrain,
				int id,
				McdRecord record,
				Tilepart[] parts)
		{
			if (record.Alt_MCD != 0) // || record.HumanDoor || record.UfoDoor
			{
				if (record.Alt_MCD < parts.Length)
					return parts[record.Alt_MCD];

				string warn = String.Format(
										CultureInfo.CurrentCulture,
										"In the MCD file {0}, part #{1} has an invalid alternate part (id #{2} of {3} records).",
										terrain,
										id,
										record.Alt_MCD,
										parts.Length);
				using (var f = new Infobox(
										" Invalid alternate part",
										warn,
										null))
				{
					f.ShowDialog();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the count of MCD-records in an MCD-file.
		/// @note It's funky to read from disk just to get the count of records
		/// but at present there is no general cache of all available terrains;
		/// even a Map's Descriptor retains only the allocated terrains as
		/// tuples in a dictionary-object.
		/// See ResourceInfo - where the *sprites* of a terrain *are* cached.
		/// </summary>
		/// <param name="terrain">the terrain file w/out extension</param>
		/// <param name="dirTerrain">path to the directory of the terrain file</param>
		/// <param name="suppressError">true to suppress any error</param>
		/// <returns>count of MCD-records or 0 on fail</returns>
		internal static int GetRecordCount(
				string terrain,
				string dirTerrain,
				bool suppressError)
		{
			string pfeMcd = Path.Combine(dirTerrain, terrain + GlobalsXC.McdExt);

			if (File.Exists(pfeMcd))
			{
				using (var bs = new BufferedStream(File.OpenRead(pfeMcd)))
					return (int)bs.Length / Length; // TODO: Error if this don't work out right.
			}

			if (!suppressError)
			{
				using (var f = new Infobox(
										" File not found",
										"Can't find file with MCD records.",
										pfeMcd))
				{
					f.ShowDialog();
				}
			}
			return 0;
		}
		#endregion
	}
}
