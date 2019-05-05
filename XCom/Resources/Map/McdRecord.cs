using System;
using System.Collections.Generic;

using XCom.Resources.Map;


namespace XCom
{
	public enum PartType
	{
		All     = -1, // <- for TileView's ALL tabpage only. Also used by QuadrantPanel.OnMouseDown() as None.
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
		/// <summary>
		/// True if this record is in a recordset that is used in MapView where
		/// it could be conflated with other terrains. False if this record is
		/// being used in McdView which doesn't require as many variables,
		/// sprite-pointers, etc etc as MapView does. In short 'IsTerrainSet' is
		/// a master flag for MapView vs. McdView; MapView is assumed by default
		/// while the flag is disabled in the constructor of 'McdviewF'.
		/// </summary>
		public static bool IsTerrainSet = true;

		/// <summary>
		/// Tracks the 'SetId' of this record.
		/// </summary>
		private static int _id;
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
		/// SetId is used only by 'MapInfoOutputBox'.
		/// </summary>
		public int SetId
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal McdRecord()
		{
			if (IsTerrainSet)
				SetId = _id++;
			else
				SetId = -1;
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Instantiates an MCD record.
		/// </summary>
		/// <param name="bindata">if null a new byte-array gets created</param>
		/// <returns></returns>
		public static McdRecord CreateRecord(IList<byte> bindata = null)
		{
			if (bindata == null)
				bindata = new byte[TilepartFactory.Length]; // all values in the byte-array default to "0"

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
			record.NotFloored  = bindata[32] != 0;
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
			record.FireResist    = bindata[45];
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
			// The following class-vars are used only by McdInfo in MapView
			// itself so - like 'SetId' - are not required by the McdView app.
			if (McdRecord.IsTerrainSet)
			{
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

				record.ByteTable = BytesTable(bindata);
			}
			#endregion Descript

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
		#endregion Methods (static)


		#region Methods
		public List<byte> GetLoftList()
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

		public McdRecord Clone()
		{
			var record = new McdRecord();

			// SetId is used only by 'MapInfoOutputBox'.
			record.SetId = SetId;

			// kL_note: All values in an MCD record are unsigned bytes except the
			// ScanG ref (little endian unsigned short) and the TerrainOffset
			// (signed byte).

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
