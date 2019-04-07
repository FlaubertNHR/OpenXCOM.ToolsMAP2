using System;
using System.Collections.Generic;


namespace XCom
{
	public enum PartType
	{
		All       = -1, // <- for TileView's ALL tabpage only.
		Floor     =  0,
		Westwall  =  1,
		Northwall =  2,
		Content   =  3
	};


	public sealed class McdRecord
	{
		// Descriptions of MCD entries are at
		// https://www.ufopaedia.org/index.php/MCD

		#region Fields (static)
		private static int _id;
		#endregion Fields (static)


		#region Properties
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
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal McdRecord()
		{
			SetId = _id++;	// TODO: Investigate that; 'SetId' should not be incrementing on each record.
		}					//       'SetId' should be incremented only on each recordset/terrainset that loads.
		#endregion


		#region Properties
		/// <summary>
		/// SetId is used only by 'MapInfoOutputBox'.
		/// </summary>
		public int SetId
		{ get; private set; }

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


		// The following strings are used by the McdInfoF only.
		public string stSprites { get; set; }
		public string stScanG   { get; set; }
		public string stLoFTs   { get; set; }

		public string ByteTable { get; set; }
		#endregion


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
			record.SetId = -1;

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


			// The following strings are used by the McdInfoF only.
			record.stSprites = stSprites;
			record.stScanG   = stScanG;
			record.stLoFTs   = stLoFTs;

			record.ByteTable = ByteTable;

			return record;
		}
		#endregion
	}
}
