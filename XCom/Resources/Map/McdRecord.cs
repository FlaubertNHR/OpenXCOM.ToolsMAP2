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

		private static int _id;


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal McdRecord()
		{
			SetId = _id++;
		}
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
		public byte Flammable     { get; set; }
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
			record.Flammable     = Flammable;
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
