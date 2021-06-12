using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom;


namespace XCom
{
	/// <summary>
	/// The quadrant-slots that a record can be assigned to in an MCD file.
	/// <c>Invalid</c> is for TileView's ALL tabpage only but is also used by
	/// <c>QuadrantControl.OnMouseDown()</c> as None. As well as by crippled tileparts.
	/// </summary>
	/// <remarks>Do not reorder these since they are used by
	/// <c>MainViewF.Screenshot()</c> in the specific order.</remarks>
	public enum PartType
	{
		Invalid = -1,
		Floor   =  0,
		West    =  1,
		North   =  2,
		Content =  3
	};


	public sealed class McdRecord
	{
		// Descriptions of MCD entries are at
		// https://www.ufopaedia.org/index.php/MCD

		#region Fields (static)
		public const int Length = 62; // there are 62 bytes in each MCD record.
		#endregion Fields (static)


		#region Properties
		// kL_note: All values in an MCD record are unsigned bytes except the
		// ScanG ref (little endian unsigned short) and the TerrainOffset
		// (signed byte).

		public byte Sprite1 { get; set; }
		public byte Sprite2 { get; set; }
		public byte Sprite3 { get; set; }
		public byte Sprite4 { get; set; }
		public byte Sprite5 { get; set; }
		public byte Sprite6 { get; set; }
		public byte Sprite7 { get; set; }
		public byte Sprite8 { get; set; }

		public byte Loft1  { get; set; }
		public byte Loft2  { get; set; }
		public byte Loft3  { get; set; }
		public byte Loft4  { get; set; }
		public byte Loft5  { get; set; }
		public byte Loft6  { get; set; }
		public byte Loft7  { get; set; }
		public byte Loft8  { get; set; }
		public byte Loft9  { get; set; }
		public byte Loft10 { get; set; }
		public byte Loft11 { get; set; }
		public byte Loft12 { get; set; }

		public ushort ScanG         { get; set; }
		public ushort ScanG_reduced { get; set; }

		public byte Unknown22 { get; set; }
		public byte Unknown23 { get; set; }
		public byte Unknown24 { get; set; }
		public byte Unknown25 { get; set; }
		public byte Unknown26 { get; set; }
		public byte Unknown27 { get; set; }
		public byte Unknown28 { get; set; }
		public byte Unknown29 { get; set; }

		public bool SlidingDoor { get; set; }
		public bool StopLOS     { get; set; }
		public bool NotFloored  { get; set; }
		public bool BigWall     { get; set; }
		public bool GravLift    { get; set; }
		public bool HingedDoor  { get; set; }
		public bool BlockFire   { get; set; }
		public bool BlockSmoke  { get; set; }

		public byte LeftRightHalf { get; set; }
		public byte TU_Walk       { get; set; }
		public byte TU_Slide      { get; set; }
		public byte TU_Fly        { get; set; }
		public byte Armor         { get; set; }
		public byte HE_Block      { get; set; }
		public byte DieTile       { get; set; }
		public byte FireResist    { get; set; }
		public byte Alt_MCD       { get; set; }
		public byte Unknown47     { get; set; }
		public sbyte StandOffset  { get; set; }
		public byte TileOffset    { get; set; }
		public byte Unknown50     { get; set; }
		public byte LightBlock    { get; set; }
		public byte Footstep      { get; set; }

		public PartType PartType   { get; set; }
		public byte HE_Type        { get; set; }
		public byte HE_Strength    { get; set; }
		public byte SmokeBlockage  { get; set; }
		public byte Fuel           { get; set; }
		public byte LightSource    { get; set; }
		public SpecialType Special { get; set; }
		public bool BaseObject     { get; set; }
		public byte Unknown61      { get; set; }


		// The following strings are used by 'McdInfoF' only.
		public string stSprites { get; set; }
		public string stScanG   { get; set; }
		public string stLoFTs   { get; set; }

		public string ByteTable { get; set; }


		public int this[int id]
		{
			get
			{
				switch (id)
				{
					case  0: return (int)Sprite1;
					case  1: return (int)Sprite2;
					case  2: return (int)Sprite3;
					case  3: return (int)Sprite4;
					case  4: return (int)Sprite5;
					case  5: return (int)Sprite6;
					case  6: return (int)Sprite7;
					case  7: return (int)Sprite8;

					case  8: return (int)Loft1;
					case  9: return (int)Loft2;
					case 10: return (int)Loft3;
					case 11: return (int)Loft4;
					case 12: return (int)Loft5;
					case 13: return (int)Loft6;
					case 14: return (int)Loft7;
					case 15: return (int)Loft8;
					case 16: return (int)Loft9;
					case 17: return (int)Loft10;
					case 18: return (int)Loft11;
					case 19: return (int)Loft12;

					case 20: return (int)ScanG;						// ushort +35
					case 21: return (int)ScanG_reduced;				// ushort

					case 22: return (int)Unknown22;
					case 23: return (int)Unknown23;
					case 24: return (int)Unknown24;
					case 25: return (int)Unknown25;
					case 26: return (int)Unknown26;
					case 27: return (int)Unknown27;
					case 28: return (int)Unknown28;
					case 29: return (int)Unknown29;

					case 30: return Convert.ToInt32(SlidingDoor);	// bools ->
					case 31: return Convert.ToInt32(StopLOS);
					case 32: return Convert.ToInt32(NotFloored);
					case 33: return Convert.ToInt32(BigWall);
					case 34: return Convert.ToInt32(GravLift);
					case 35: return Convert.ToInt32(HingedDoor);
					case 36: return Convert.ToInt32(BlockFire);
					case 37: return Convert.ToInt32(BlockSmoke);

					case 38: return (int)LeftRightHalf;
					case 39: return (int)TU_Walk;
					case 40: return (int)TU_Slide;
					case 41: return (int)TU_Fly;
					case 42: return (int)Armor;
					case 43: return (int)HE_Block;
					case 44: return (int)DieTile;
					case 45: return (int)FireResist;
					case 46: return (int)Alt_MCD;
					case 47: return (int)Unknown47;
					case 48: return (int)StandOffset;				// sbyte
					case 49: return (int)TileOffset;
					case 50: return (int)Unknown50;
					case 51: return (int)LightBlock;
					case 52: return (int)Footstep;

					case 53: return (int)PartType;					// PartType
					case 54: return (int)HE_Type;
					case 55: return (int)HE_Strength;
					case 56: return (int)SmokeBlockage;
					case 57: return (int)Fuel;
					case 58: return (int)LightSource;
					case 59: return (int)Special;					// SpecialType
					case 60: return Convert.ToInt32(BaseObject);	// bool
					case 61: return (int)Unknown61;
				}
				return 0;
			}
		}


		/// <summary>
		/// Used by MapView.BlobDrawService.
		/// </summary>
		public IList<byte> LoftList
		{
			get
			{
				var lofts = new List<byte>();

				lofts.Add(Loft1);
				lofts.Add(Loft2);
				lofts.Add(Loft3);
				lofts.Add(Loft4);
				lofts.Add(Loft5);
				lofts.Add(Loft6);
				lofts.Add(Loft7);
				lofts.Add(Loft8);
				lofts.Add(Loft9);
				lofts.Add(Loft10);
				lofts.Add(Loft11);
				lofts.Add(Loft12);

				return lofts;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0].
		/// </summary>
		/// <param name="bindata">if null a blank byte-array gets created</param>
		public McdRecord(IList<byte> bindata)
		{
			if (bindata == null)
				bindata = new byte[McdRecord.Length]; // all values in the byte-array default to "0"

			Sprite1 = bindata[0];
			Sprite2 = bindata[1];
			Sprite3 = bindata[2];
			Sprite4 = bindata[3];
			Sprite5 = bindata[4];
			Sprite6 = bindata[5];
			Sprite7 = bindata[6];
			Sprite8 = bindata[7];

			Loft1  = bindata[8];
			Loft2  = bindata[9];
			Loft3  = bindata[10];
			Loft4  = bindata[11];
			Loft5  = bindata[12];
			Loft6  = bindata[13];
			Loft7  = bindata[14];
			Loft8  = bindata[15];
			Loft9  = bindata[16];
			Loft10 = bindata[17];
			Loft11 = bindata[18];
			Loft12 = bindata[19];

			ScanG         = (ushort)(bindata[21] * 256 + bindata[20] + 35);
			ScanG_reduced = (ushort)(bindata[21] * 256 + bindata[20]);

			Unknown22 = bindata[22];
			Unknown23 = bindata[23];
			Unknown24 = bindata[24];
			Unknown25 = bindata[25];
			Unknown26 = bindata[26];
			Unknown27 = bindata[27];
			Unknown28 = bindata[28];
			Unknown29 = bindata[29];

			SlidingDoor = bindata[30] != 0;
			StopLOS     = bindata[31] != 0;
			NotFloored  = bindata[32] != 0;
			BigWall     = bindata[33] != 0; // TODO: store as a byte
			GravLift    = bindata[34] != 0;
			HingedDoor  = bindata[35] != 0;
			BlockFire   = bindata[36] != 0;
			BlockSmoke  = bindata[37] != 0;

			LeftRightHalf = bindata[38];
			TU_Walk       = bindata[39];
			TU_Slide      = bindata[40];
			TU_Fly        = bindata[41];
			Armor         = bindata[42];
			HE_Block      = bindata[43];
			DieTile       = bindata[44];
			FireResist    = bindata[45];
			Alt_MCD       = bindata[46];
			Unknown47     = bindata[47];
			StandOffset   = unchecked((sbyte)bindata[48]);
			TileOffset    = bindata[49];
			Unknown50     = bindata[50];
			LightBlock    = bindata[51];
			Footstep      = bindata[52];

			PartType      = (PartType)bindata[53];
			HE_Type       = bindata[54];
			HE_Strength   = bindata[55];
			SmokeBlockage = bindata[56];
			Fuel          = bindata[57];
			LightSource   = bindata[58];
			Special       = (SpecialType)bindata[59];
			BaseObject    = bindata[60] != 0;
			Unknown61     = bindata[61];


			stSprites = string.Format(
								"{0,-20}{1} {2} {3} {4} {5} {6} {7} {8}",
								"images:",	// 0
								Sprite1,	// 1
								Sprite2,	// 2
								Sprite3,	// 3
								Sprite4,	// 4
								Sprite5,	// 5
								Sprite6,	// 6
								Sprite7,	// 7
								Sprite8);	// 8

			stScanG = string.Format(
								"{0,-20}{1} : {2} -> {3} [{4}]",
								"scang reference:",	// 0
								bindata[20],		// 1
								bindata[21],		// 2
								ScanG,				// 3
								ScanG_reduced);		// 4

			stLoFTs = string.Format(
								"{0,-20}{1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}",
								"loft references:",	// 0
								Loft1,				// 1
								Loft2,				// 2
								Loft3,				// 3
								Loft4,				// 4
								Loft5,				// 5
								Loft6,				// 6
								Loft7,				// 7
								Loft8,				// 8
								Loft9,				// 9
								Loft10,				// 10
								Loft11,				// 11
								Loft12);			// 12

			ByteTable = BytesTable(bindata);
		}

		/// <summary>
		/// cTor[1]. Creates a blank record for Duplicate().
		/// </summary>
		private McdRecord()
		{}
		#endregion cTor


		#region Methods (static)
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

		/// <summary>
		/// Writes/overwrites a specified MCD file.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <param name="parts">an array of tileparts</param>
		/// <returns>true if it looks like the file got written</returns>
		public static bool WriteRecords(string pfe, Tilepart[] parts)
		{
			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;

				McdRecord record;
				ushort u;

				foreach (Tilepart part in parts)
				{
					record = part.Record;

					fs.WriteByte((byte)record.Sprite1);					//  0
					fs.WriteByte((byte)record.Sprite2);					//  1
					fs.WriteByte((byte)record.Sprite3);					//  2
					fs.WriteByte((byte)record.Sprite4);					//  3
					fs.WriteByte((byte)record.Sprite5);					//  4
					fs.WriteByte((byte)record.Sprite6);					//  5
					fs.WriteByte((byte)record.Sprite7);					//  6
					fs.WriteByte((byte)record.Sprite8);					//  7

					fs.WriteByte((byte)record.Loft1);					//  8
					fs.WriteByte((byte)record.Loft2);					//  9
					fs.WriteByte((byte)record.Loft3);					// 10
					fs.WriteByte((byte)record.Loft4);					// 11
					fs.WriteByte((byte)record.Loft5);					// 12
					fs.WriteByte((byte)record.Loft6);					// 13
					fs.WriteByte((byte)record.Loft7);					// 14
					fs.WriteByte((byte)record.Loft8);					// 15
					fs.WriteByte((byte)record.Loft9);					// 16
					fs.WriteByte((byte)record.Loft10);					// 17
					fs.WriteByte((byte)record.Loft11);					// 18
					fs.WriteByte((byte)record.Loft12);					// 19

					u = record.ScanG_reduced;
					fs.WriteByte((byte)( u & 0x00FF));					// 20
					fs.WriteByte((byte)((u & 0xFF00) >> 8));			// 21

					fs.WriteByte((byte)record.Unknown22);				// 22
					fs.WriteByte((byte)record.Unknown23);				// 23
					fs.WriteByte((byte)record.Unknown24);				// 24
					fs.WriteByte((byte)record.Unknown25);				// 25
					fs.WriteByte((byte)record.Unknown26);				// 26
					fs.WriteByte((byte)record.Unknown27);				// 27
					fs.WriteByte((byte)record.Unknown28);				// 28
					fs.WriteByte((byte)record.Unknown29);				// 29

					fs.WriteByte(Convert.ToByte(record.SlidingDoor));	// 30 (bool)
					fs.WriteByte(Convert.ToByte(record.StopLOS));		// 31 (bool)
					fs.WriteByte(Convert.ToByte(record.NotFloored));	// 32 (bool)
					fs.WriteByte(Convert.ToByte(record.BigWall));		// 33 (bool)
					fs.WriteByte(Convert.ToByte(record.GravLift));		// 34 (bool)
					fs.WriteByte(Convert.ToByte(record.HingedDoor));	// 35 (bool)
					fs.WriteByte(Convert.ToByte(record.BlockFire));		// 36 (bool)
					fs.WriteByte(Convert.ToByte(record.BlockSmoke));	// 37 (bool)

					fs.WriteByte((byte)record.LeftRightHalf);			// 38
					fs.WriteByte((byte)record.TU_Walk);					// 39
					fs.WriteByte((byte)record.TU_Slide);				// 40
					fs.WriteByte((byte)record.TU_Fly);					// 41
					fs.WriteByte((byte)record.Armor);					// 42
					fs.WriteByte((byte)record.HE_Block);				// 43
					fs.WriteByte((byte)record.DieTile);					// 44
					fs.WriteByte((byte)record.FireResist);				// 45
					fs.WriteByte((byte)record.Alt_MCD);					// 46
					fs.WriteByte((byte)record.Unknown47);				// 47
					fs.WriteByte(unchecked((byte)record.StandOffset));	// 48 (sbyte)
					fs.WriteByte((byte)record.TileOffset);				// 49
					fs.WriteByte((byte)record.Unknown50);				// 50
					fs.WriteByte((byte)record.LightBlock);				// 51
					fs.WriteByte((byte)record.Footstep);				// 52

					fs.WriteByte((byte)record.PartType);				// 53 (PartType)
					fs.WriteByte((byte)record.HE_Type);					// 54
					fs.WriteByte((byte)record.HE_Strength);				// 55
					fs.WriteByte((byte)record.SmokeBlockage);			// 56
					fs.WriteByte((byte)record.Fuel);					// 57
					fs.WriteByte((byte)record.LightSource);				// 58
					fs.WriteByte((byte)record.Special);					// 59 (SpecialType)
					fs.WriteByte(Convert.ToByte(record.BaseObject));	// 60 (bool)
					fs.WriteByte((byte)record.Unknown61);				// 61
				}
			}

			if (!fail && pfeT != pfe)
				return FileService.ReplaceFile(pfe);

			return !fail;
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Creates an independent copy of this McdRecord.
		/// </summary>
		/// <returns></returns>
		public McdRecord Duplicate()
		{
			var record = new McdRecord();

			record.Sprite1 = Sprite1;
			record.Sprite2 = Sprite2;
			record.Sprite3 = Sprite3;
			record.Sprite4 = Sprite4;
			record.Sprite5 = Sprite5;
			record.Sprite6 = Sprite6;
			record.Sprite7 = Sprite7;
			record.Sprite8 = Sprite8;

			record.Loft1  = Loft1;
			record.Loft2  = Loft2;
			record.Loft3  = Loft3;
			record.Loft4  = Loft4;
			record.Loft5  = Loft5;
			record.Loft6  = Loft6;
			record.Loft7  = Loft7;
			record.Loft8  = Loft8;
			record.Loft9  = Loft9;
			record.Loft10 = Loft10;
			record.Loft11 = Loft11;
			record.Loft12 = Loft12;

			record.ScanG         = ScanG;			// ushort
			record.ScanG_reduced = ScanG_reduced;	// ushort

			record.Unknown22 = Unknown22;
			record.Unknown23 = Unknown23;
			record.Unknown24 = Unknown24;
			record.Unknown25 = Unknown25;
			record.Unknown26 = Unknown26;
			record.Unknown27 = Unknown27;
			record.Unknown28 = Unknown28;
			record.Unknown29 = Unknown29;

			record.SlidingDoor = SlidingDoor;	// bool
			record.StopLOS     = StopLOS;		// bool
			record.NotFloored  = NotFloored;	// bool
			record.BigWall     = BigWall;		// bool
			record.GravLift    = GravLift;		// bool
			record.HingedDoor  = HingedDoor;	// bool
			record.BlockFire   = BlockFire;		// bool
			record.BlockSmoke  = BlockSmoke;	// bool

			record.LeftRightHalf = LeftRightHalf;
			record.TU_Walk       = TU_Walk;
			record.TU_Slide      = TU_Slide;
			record.TU_Fly        = TU_Fly;
			record.Armor         = Armor;
			record.HE_Block      = HE_Block;
			record.DieTile       = DieTile;
			record.FireResist    = FireResist;
			record.Alt_MCD       = Alt_MCD;
			record.Unknown47     = Unknown47;
			record.StandOffset   = StandOffset;	// sbyte
			record.TileOffset    = TileOffset;
			record.Unknown50     = Unknown50;
			record.LightBlock    = LightBlock;
			record.Footstep      = Footstep;

			record.PartType      = PartType;	// PartType
			record.HE_Type       = HE_Type;
			record.HE_Strength   = HE_Strength;
			record.SmokeBlockage = SmokeBlockage;
			record.Fuel          = Fuel;
			record.LightSource   = LightSource;
			record.Special       = Special;		// SpecialType
			record.BaseObject    = BaseObject;	// bool
			record.Unknown61     = Unknown61;


			#region Descript
			// The following strings are used by the McdInfoF only.
			record.stSprites = stSprites;
			record.stScanG   = stScanG;
			record.stLoFTs   = stLoFTs;

			record.ByteTable = ByteTable;
			#endregion Descript

			return record;
		}
		#endregion Methods
	}
}
