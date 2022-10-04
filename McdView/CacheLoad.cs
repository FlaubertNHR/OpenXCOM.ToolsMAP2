using System;

using XCom;


namespace McdView
{
	/// <summary>
	/// A class that stores the originally loaded/ recently saved values of all
	/// tileparts in an MCD file for later comparisons that determine McdView's
	/// 'Changed' boolean.
	/// </summary>
	internal static class CacheLoad
	{
		#region Fields
		private static int Length;
		private static int[,] Parts;
		#endregion Fields


		#region Methods
		/// <summary>
		/// Sets the Cached values.
		/// </summary>
		/// <param name="parts"></param>
		internal static void SetCacheSaved(Tilepart[] parts)
		{
			Length = parts.Length;
			Parts = new int[Length, McdRecord.Length];

			for (int i = 0; i != Length; ++i)
			{
				Tilepart part = parts[i];

				Parts[i,  0] = part.Record.Sprite1;
				Parts[i,  1] = part.Record.Sprite2;
				Parts[i,  2] = part.Record.Sprite3;
				Parts[i,  3] = part.Record.Sprite4;
				Parts[i,  4] = part.Record.Sprite5;
				Parts[i,  5] = part.Record.Sprite6;
				Parts[i,  6] = part.Record.Sprite7;
				Parts[i,  7] = part.Record.Sprite8;

				Parts[i,  8] = part.Record.Loft1;
				Parts[i,  9] = part.Record.Loft2;
				Parts[i, 10] = part.Record.Loft3;
				Parts[i, 11] = part.Record.Loft4;
				Parts[i, 12] = part.Record.Loft5;
				Parts[i, 13] = part.Record.Loft6;
				Parts[i, 14] = part.Record.Loft7;
				Parts[i, 15] = part.Record.Loft8;
				Parts[i, 16] = part.Record.Loft9;
				Parts[i, 17] = part.Record.Loft10;
				Parts[i, 18] = part.Record.Loft11;
				Parts[i, 19] = part.Record.Loft12;

				Parts[i, 20] = part.Record.ScanG;								// ushort (already has 35 added)
				Parts[i, 21] = part.Record.ScanG_reduced;						// ushort - 35 (is redundant)

				Parts[i, 22] = part.Record.Unknown22;
				Parts[i, 23] = part.Record.Unknown23;
				Parts[i, 24] = part.Record.Unknown24;
				Parts[i, 25] = part.Record.Unknown25;
				Parts[i, 26] = part.Record.Unknown26;
				Parts[i, 27] = part.Record.Unknown27;
				Parts[i, 28] = part.Record.Unknown28;
				Parts[i, 29] = part.Record.Unknown29;

				Parts[i, 30] = Convert.ToInt32(part.Record.SlidingDoor);		// bools ->
				Parts[i, 31] = Convert.ToInt32(part.Record.StopLOS);
				Parts[i, 32] = Convert.ToInt32(part.Record.NotFloored);
				Parts[i, 33] = Convert.ToInt32(part.Record.BigWall);
				Parts[i, 34] = Convert.ToInt32(part.Record.GravLift);
				Parts[i, 35] = Convert.ToInt32(part.Record.HingedDoor);
				Parts[i, 36] = Convert.ToInt32(part.Record.BlockFire);
				Parts[i, 37] = Convert.ToInt32(part.Record.BlockSmoke);

				Parts[i, 38] = part.Record.LeftRightHalf;
				Parts[i, 39] = part.Record.TU_Walk;
				Parts[i, 40] = part.Record.TU_Slide;
				Parts[i, 41] = part.Record.TU_Fly;
				Parts[i, 42] = part.Record.Armor;
				Parts[i, 43] = part.Record.HE_Block;
				Parts[i, 44] = part.Record.DieTile;
				Parts[i, 45] = part.Record.FireResist;
				Parts[i, 46] = part.Record.Alt_MCD;
				Parts[i, 47] = part.Record.Unknown47;
				Parts[i, 48] = part.Record.StandOffset;							// sbyte
				Parts[i, 49] = part.Record.TileOffset;
				Parts[i, 50] = part.Record.Unknown50;
				Parts[i, 51] = part.Record.LightBlock;
				Parts[i, 52] = part.Record.Footstep;

				Parts[i, 53] = (int)part.Record.PartType;						// PartType
				Parts[i, 54] = part.Record.HE_Type;
				Parts[i, 55] = part.Record.HE_Strength;
				Parts[i, 56] = part.Record.SmokeBlockage;
				Parts[i, 57] = part.Record.Fuel;
				Parts[i, 58] = part.Record.LightSource;
				Parts[i, 59] = (int)part.Record.Special;						// SpecialType
				Parts[i, 60] = Convert.ToInt32(part.Record.BaseObject);			// bool
				Parts[i, 61] = part.Record.Unknown61;
			}
		}

		/// <summary>
		/// Checks if a specified recordset has changed.
		/// </summary>
		/// <param name="parts">a recordset to test</param>
		/// <returns></returns>
		internal static bool Changed(Tilepart[] parts)
		{
			if (parts.Length != Length)
				return true;

			McdRecord record;
			for (int i = 0; i != Length; ++i)
			{
				record = parts[i].Record;
				for (int j = 0; j != McdRecord.Length; ++j)
				{
					if (record[j] != Parts[i,j])
						return true;
				}
			}
			return false;
		}

/*		/// <summary>
		/// Checks if a specific value is Cached.
		/// </summary>
		/// <param name="val">a value to check</param>
		/// <param name="i">the recordset-id to check against</param>
		/// <param name="j">the record-id to check against</param>
		/// <returns>true if cached</returns>
		internal static bool Cached(int val, int i, int j)
		{
			return (val == Parts[i,j]);
		} */

/*		/// <summary>
		/// Checks if a part in the Cache is zero'd.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		internal static bool isZero(int i)
		{
			foreach (int j in Parts[i])
			{
				if (i != 20 && j != 0) // skip #20 ScanG+35
					return false;
			}
			return true;
		} */
		#endregion Methods
	}
}
