using System;
using System.Collections.Generic;


namespace XCom
{
	public static class McdRecordFactory
	{
		// TODO: do some basic checks, like issue a warning if the Die or
		// Alternate MCD entry is outside the range, etc.

		public static McdRecord CreateRecord(IList<byte> bindata)
		{
			var record = new McdRecord();

			record.Sprite1 = bindata[0];
			record.Sprite2 = bindata[1];
			record.Sprite3 = bindata[2];
			record.Sprite4 = bindata[3];
			record.Sprite5 = bindata[4];
			record.Sprite6 = bindata[5];
			record.Sprite7 = bindata[6];
			record.Sprite8 = bindata[7];

			record.Loft1  = bindata[8];
			record.Loft2  = bindata[9];
			record.Loft3  = bindata[10];
			record.Loft4  = bindata[11];
			record.Loft5  = bindata[12];
			record.Loft6  = bindata[13];
			record.Loft7  = bindata[14];
			record.Loft8  = bindata[15];
			record.Loft9  = bindata[16];
			record.Loft10 = bindata[17];
			record.Loft11 = bindata[18];
			record.Loft12 = bindata[19];

			record.ScanG         = (ushort)(bindata[21] * 256 + bindata[20] + 35);
			record.ScanG_reduced = (ushort)(bindata[21] * 256 + bindata[20]);

			record.Unknown22 = bindata[22];
			record.Unknown23 = bindata[23];
			record.Unknown24 = bindata[24];
			record.Unknown25 = bindata[25];
			record.Unknown26 = bindata[26];
			record.Unknown27 = bindata[27];
			record.Unknown28 = bindata[28];
			record.Unknown29 = bindata[29];

			record.SlidingDoor = bindata[30] != 0;
			record.StopLOS     = bindata[31] != 0;
			record.NoGround    = bindata[32] != 0;
			record.BigWall     = bindata[33] != 0; // TODO: store as a byte
			record.GravLift    = bindata[34] != 0;
			record.HingedDoor  = bindata[35] != 0;
			record.BlockFire   = bindata[36] != 0;
			record.BlockSmoke  = bindata[37] != 0;

			record.LeftRightHalf = bindata[38];
			record.TU_Walk       = bindata[39];
			record.TU_Slide      = bindata[40];
			record.TU_Fly        = bindata[41];
			record.Armor         = bindata[42];
			record.HE_Block      = bindata[43];
			record.DieTile       = bindata[44];
			record.Flammable     = bindata[45];
			record.Alt_MCD       = bindata[46];
			record.Unknown47     = bindata[47];
			record.StandOffset   = (sbyte)bindata[48];
			record.TileOffset    = bindata[49];
			record.Unknown50     = bindata[50];
			record.LightBlock    = bindata[51];
			record.Footstep      = bindata[52];

			record.PartType      = (PartType)bindata[53];
			record.HE_Type       = bindata[54];
			record.HE_Strength   = bindata[55];
			record.SmokeBlockage = bindata[56];
			record.Fuel          = bindata[57];
			record.LightSource   = bindata[58];
			record.Special       = (SpecialType)bindata[59];
			record.BaseObject    = bindata[60] != 0;
			record.Unknown61     = bindata[61];


			#region Descript
			record.stSprites = string.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"{0,-20}{1} {2} {3} {4} {5} {6} {7} {8}" + Environment.NewLine,
										"images:",
										record.Sprite1,
										record.Sprite2,
										record.Sprite3,
										record.Sprite4,
										record.Sprite5,
										record.Sprite6,
										record.Sprite7,
										record.Sprite8);

			record.stScanG = string.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"{0,-20}{1} : {2} -> {3} [{4}]" + Environment.NewLine,
										"scang reference:",
										bindata[20],
										bindata[21],
										record.ScanG,
										record.ScanG_reduced);

			record.stLoFTs = string.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"{0,-20}{1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}" + Environment.NewLine,
										"loft references:",
										record.Loft1,
										record.Loft2,
										record.Loft3,
										record.Loft4,
										record.Loft5,
										record.Loft6,
										record.Loft7,
										record.Loft8,
										record.Loft9,
										record.Loft10,
										record.Loft11,
										record.Loft12);
			#endregion

			record.ByteTable = BytesTable(bindata);
			return record;
		}

		/// <summary>
		/// Creates a table of the byte-data for the MCD-info screen.
		/// </summary>
		/// <returns></returns>
		private static string BytesTable(IList<byte> bindata)
		{
			string text = String.Empty;

			const int wrap = 8;
			int wrapCount  = 0;
			int row        = 0;

			foreach (byte b in bindata)
			{
				if (wrapCount % wrap == 0)
				{
					if (++row < 10)
						text += " ";

					text += row + ": ";
				}

				if (b < 10)
					text += "  ";
				else if (b < 100)
					text += " ";

				text += " " + b;
				text += (++wrapCount % wrap == 0) ? Environment.NewLine
												  : " ";
			}
			return text;
		}
	}
}
