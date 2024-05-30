using System;
using System.Collections.Generic;


namespace XCom
{
	/// <summary>
	/// The various wall- and content-types that will be used to determine how
	/// to draw the wall- and content-blobs in <c>TopView</c> and
	/// <c>RouteView</c>. Also used for blob-preview in McdView.
	/// </summary>
	public enum Blob
	{
		Crippled = -1,			// invalid loftlist in the tilepart's record

		Generic,				// a generic square graphic that's centered on the tile

		Floorlike,				// fill the entire tile with the blob-color

		Westwall,				// lines along the edges of the tile ->
		Northwall,
		Eastwall,
		Southwall,

		WestwallWindow,			// lines along the edges of the tile with a lighter colored gap ->
		NorthwallWindow,

		WestFence,				// lines along the edges of the tile with a lighter color ->
		NorthFence,
		EastFence,
		SouthFence,

		NorthwestCorner,		// triangles tucked into the corner of the tile ->
		NortheastCorner,
		SoutheastCorner,
		SouthwestCorner,

		NorthwestCornerFence,	// triangles tucked into the corner of the tile with a lighter color ->
		NortheastCornerFence,
		SoutheastCornerFence,
		SouthwestCornerFence,

		NorthwestSoutheast,		// diagonal lines across the tile ->
		NortheastSouthwest,
		NorthwestSoutheastFence,
		NortheastSouthwestFence,


		WestDoorLine,			// labels that are used in BlobDrawService.DrawWallOrContent() ->
		NorthDoorLine
	}


	/// <summary>
	/// A static class that determines how wall- and content-parts are drawn as
	/// preview-blobs.
	/// </summary>
	public static class BlobTypeService
	{
		#region Fields (static)
		/// <summary>
		/// 
		/// </summary>
		/// <remarks><c>_loftlist</c> is used only by McdView.
		/// <c><see cref="McdRecord">McdRecords</see></c> in MapView store their
		/// own <c><see cref="McdRecord.LoftList">McdRecord.LoftLists</see></c>.</remarks>
		public static readonly IList<byte> _loftlist = new List<byte>();

		/// <summary>
		/// The length of a LoFT-layers <c>List</c>.
		/// </summary>
		public const int LoftlistLength = 12;


		// These are (arrays of) LoFT entries in the stock UFO/TFTD LOFTEMPS.DAT
		// resource file that are used to deter graphical Blobs in TopView and
		// RouteView as well as the blob-preview in McdView.
		//
		// Stock UFO has 112 entries; stock TFTD has 114.

		private const           byte   Westwall_window   = 37;
		private static readonly byte[] Westwall          = {7,9,11,13,15,17,19,22};
		private static readonly byte[] Westwall_notsolid = {50,51,52,76,111,
															7,9,11,13,15,17,19,22,				// +Westwall
															0,									// +Loftnon
															37,									// +Westwall_window
															39,40,41, 81,     101,102,103,		// +NorthwestCorner
															          79, 87, 104,105,106};		// +SouthwestCorner

		private const           byte   Northwall_window   = 38;
		private static readonly byte[] Northwall          = {8,10,12,14,16,18,20,21};
		private static readonly byte[] Northwall_notsolid = {77,110,
															 8,10,12,14,16,18,20,21,			// +Northwall
															 0,									// +Loftnon
															 38,								// +Northwall_window
															 39,40,41, 81,    101,102,103,		// +NorthwestCorner
															           80, 88, 98, 99,100};		// +NortheastCorner

		private static readonly byte[] Eastwall           = {24,26,28,30,32,34, 47};
		private static readonly byte[] Eastwall_notsolid  = {24,26,28,30,32,34, 47,				// +Eastwall
															 0,									// +Loftnon
															 80, 88,  98, 99,100,				// +NortheastCorner
															 82, 86, 107,108,109};				// +SoutheastCorner
		private static readonly byte[] Southwall          = {23,25,27,29,31,33, 44};
		private static readonly byte[] Southwall_notsolid = {23,25,27,29,31,33, 44,				// +Southwall
															 0,									// +Loftnon
															 82, 86, 107,108,109,				// +SoutheastCorner
															 79, 87, 104,105,106};				// +SouthwestCorner

		private static readonly byte[] NorthwestCorner = {39,40,41, 81,     90, 101,102,103};
		private static readonly byte[] NortheastCorner = {          80, 88, 91,  98, 99,100};
		private static readonly byte[] SoutheastCorner = {          82, 86, 89, 107,108,109};
		private static readonly byte[] SouthwestCorner = {          79, 87, 92, 104,105,106};

		private const byte NorthwestSoutheast = 35;
		private const byte NortheastSouthwest = 36;

		private const byte Loftnon     =   0;
		private const byte LoftInvalid = 255;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Gets the <c><see cref="Blob"/></c> of a specified
		/// <c><see cref="Tilepart"/></c> for drawing its blob in <c>TopView</c>
		/// and <c>RouteView</c>.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		/// <remarks>http://www.ufopaedia.org/index.php/LOFTEMPS.DAT</remarks>
		public static Blob GetBlobType(Tilepart part)
		{
			//DSShared.Logfile.Log("BlobTypeService.GetBlobType()");

			McdRecord record = part.Record;
			if (record != null)
			{
				//DSShared.Logfile.Log(". record Valid");
				//DSShared.Logfile.Log(". . LoftList= " + (record.LoftList != null ? "Valid" : "INVALID"));
				//DSShared.Logfile.Log(". . LoftList.Count = " + record.LoftList.Count);

				if (record.LoftList != null && record.LoftList.Count != 0) // crippled tileparts have an invalid 'LoftList'
					return GetBlobType(record.LoftList);//, record.PartType

				return Blob.Crippled;
			}
			return Blob.Generic;
		}

		/// <summary>
		/// Gets the <c><see cref="Blob"/></c> of a specified
		/// <c><see cref="Tilepart"/></c> for drawing its preview-blob.
		/// </summary>
		/// <param name="loftlist"></param>
//		/// <param name="parttype"></param>
		/// <returns>a <c>Blob</c> type</returns>
		/// <remarks>http://www.ufopaedia.org/index.php/LOFTEMPS.DAT</remarks>
		public static Blob GetBlobType(IList<byte> loftlist) //, PartType parttype)
		{
			//var sb = new System.Text.StringBuilder();
			//foreach (byte id in loftlist) sb = sb.Append(id + ",");
			//DSShared.Logfile.Log("BlobTypeService.GetBlobType() parttype= " + parttype + " loftlist= " + sb);

			// IMPORTANT: The order that follows is critical.
			//
			// Note that a 'fence' (as well as a 'window') is a wall-like
			// loftset that can be shot through; unlike a 'wall' its voxels are
			// not entirely solid to LoF.

			// TODO: if (entire id #6 fullblock) ret Blob.Block

			bool anyLoftnon = anyare(loftlist, Loftnon);

			// corners ->
			if (anyLoftnon)
			{
				// not a corner but hey ... this is where it goes ->
				if (floorlike(loftlist))
					return Blob.Floorlike;

				// corner fences ->
				if (allare(loftlist, NorthwestCorner, Loftnon))// && (parttype == PartType.West || parttype == PartType.North))
					return Blob.NorthwestCornerFence;

				if (allare(loftlist, NortheastCorner, Loftnon))// && (parttype == PartType.North || parttype == PartType.Content))
					return Blob.NortheastCornerFence;

				if (allare(loftlist, SoutheastCorner, Loftnon))// && parttype == PartType.Content)
					return Blob.SoutheastCornerFence;

				if (allare(loftlist, SouthwestCorner, Loftnon))// && (parttype == PartType.West || parttype == PartType.Content))
					return Blob.SouthwestCornerFence;
			}
			else
			{
				// corners ->
				if (allare(loftlist, NorthwestCorner))// && (parttype == PartType.West || parttype == PartType.North))
					return Blob.NorthwestCorner;

				if (allare(loftlist, NortheastCorner))// && (parttype == PartType.North || parttype == PartType.Content))
					return Blob.NortheastCorner;

				if (allare(loftlist, SoutheastCorner))// && parttype == PartType.Content)
					return Blob.SoutheastCorner;

				if (allare(loftlist, SouthwestCorner))// && (parttype == PartType.West || parttype == PartType.Content))
					return Blob.SouthwestCorner;

			}

			// windows and solid walls ->
			if (!anyLoftnon)
			{
				if (anyare(loftlist, Westwall_window) && allare(loftlist, Westwall, Westwall_window))
					return Blob.WestwallWindow;

				if (allare(loftlist, Westwall))
					return Blob.Westwall;

				if (anyare(loftlist, Northwall_window) && allare(loftlist, Northwall, Northwall_window))
					return Blob.NorthwallWindow;

				if (allare(loftlist, Northwall))
					return Blob.Northwall;

				if (allare(loftlist, Eastwall))
					return Blob.Eastwall;

				if (allare(loftlist, Southwall))
					return Blob.Southwall;
			}

			// walls not solid ->
			if (allare(loftlist, Westwall_notsolid))
				return Blob.WestFence;

			if (allare(loftlist, Northwall_notsolid))
				return Blob.NorthFence;

			if (allare(loftlist, Eastwall_notsolid))
				return Blob.EastFence;

			if (allare(loftlist, Southwall_notsolid))
				return Blob.SouthFence;


			// diagonals ->
			if (anyLoftnon)
			{
				// diagonal fences ->
				if (allare(loftlist, NorthwestSoutheast, Loftnon))// && parttype == PartType.Content)
					return Blob.NorthwestSoutheastFence;

				if (allare(loftlist, NortheastSouthwest, Loftnon))// && parttype == PartType.Content)
					return Blob.NortheastSouthwestFence;
			}
			else
			{
				// diagonal walls ->
				if (allare(loftlist, NorthwestSoutheast))// && parttype == PartType.Content)
					return Blob.NorthwestSoutheast;

				if (allare(loftlist, NortheastSouthwest))// && parttype == PartType.Content)
					return Blob.NortheastSouthwest;
			}

			return Blob.Generic;
		}


		/// <summary>
		/// Checks if <paramref name="loftlist"/> has only LoFT id #0 (blank
		/// LoFT) above the first layer.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This function checks LoFTs only of wall- and content-parts
		/// for drawing TopView and RouteView and the blob-preview in McdView
		/// but is NOT actually used for floor-parts.
		/// <br/><br/>
		/// Loftid #6 on layer #0 is the fullfloor LoFT but is not checked for.</remarks>
		private static bool floorlike(IList<byte> loftlist)
		{
			//DSShared.Logfile.Log("BlobTypeService.floorlike()");

			// NOTE: Use this only if/when CustomLoftBlobs is implemented:
//			byte loft = loftlist[0];
//			if (   bypassfloor(loft, Westwall_notsolid)
//				|| bypassfloor(loft, Northwall_notsolid)
//				|| bypassfloor(loft, Eastwall_notsolid)
//				|| bypassfloor(loft, Southwall_notsolid)
//				|| bypassfloor(loft, NorthwestCorner)
//				|| bypassfloor(loft, NortheastCorner)
//				|| bypassfloor(loft, SoutheastCorner)
//				|| bypassfloor(loft, SouthwestCorner)) // TODO: check the 2 window and 2 diagonal LoFTs
//			{
//				return false;
//			}

			switch (loftlist[0]) // bypass wall and corner LoFTs (to allow wall and corner blobs later)
			{
				case   7: case   8: case   9: case  10: case  11: case  12: case  13: case  14:	// west and north walls
				case  15: case  16: case  17: case  18: case  19: case  20: case  21: case  22:

				case  37: case  38:																// west and north windows

				case  39: case  40: case  41:													// nw corners

				case  76: case  77: case 110: case 111:											// west and north walls not solid
				case  50: case  51: case  52:													// west walls not solid

				case  79: case  80: case  81: case  82:											// corners ->
				case  89: case  90: case  91: case  92:											// big rounded corners
				case  86: case  87: case  88:
				case  98: case  99: case 100: case 101: case 102: case 103:
				case 104: case 105: case 106: case 107: case 108: case 109:
					return false;
			}

			for (int layer = 1; layer != BlobTypeService.LoftlistLength; ++layer)
				if (loftlist[layer] != Loftnon) // that's kind of a stupid check for floor ...
					return false;

			return true;
		}

		/// <summary>
		/// Checks if any entry in <paramref name="loftlist"/> is
		/// <paramref name="loftid"/>.
		/// </summary>
		/// <param name="loftlist">a list of LoFTs to check for
		/// <paramref name="loftid"/></param>
		/// <param name="loftid">the required loftid</param>
		/// <returns><c>true</c> if any LoFT is <paramref name="loftid"/></returns>
		private static bool anyare(IList<byte> loftlist, byte loftid)
		{
			//DSShared.Logfile.Log("BlobTypeService.anyare() loftid= " + loftid);

			for (int layer = 0; layer != BlobTypeService.LoftlistLength; ++layer)
				if (loftlist[layer] == loftid)
					return true;

			return false;
		}

//		private static bool anyare(IList<byte> loftlist, byte[] loftids)
//		{
//			var sb = new System.Text.StringBuilder();
//			foreach (byte id in loftids) sb = sb.Append(id + ",");
//			DSShared.Logfile.Log("BlobTypeService.anyare() loftids= " + sb);
//
//			foreach (byte loft in loftlist)
//				foreach (byte id in loftids)
//					if (id == loft)
//						return true;
//
//			return false;
//		}

		/// <summary>
		/// Checks if all entries in <paramref name="loftlist"/> are
		/// <paramref name="loftid"/>.
		/// </summary>
		/// <param name="loftlist">a list of LoFTs to check against
		/// <paramref name="loftid"/></param>
		/// <param name="loftid">the required loftid</param>
		/// <param name="ignore"></param>
		/// <returns><c>true</c> if all LoFTs are <paramref name="loftid"/></returns>
		/// <remarks>Layer #0 is NOT considered since it is typically a
		/// floorlike LoFT.</remarks>
		private static bool allare(IList<byte> loftlist, byte loftid, byte ignore = LoftInvalid)
		{
			//DSShared.Logfile.Log("BlobTypeService.allare() loftid= " + loftid);

			for (int layer = 1; layer != BlobTypeService.LoftlistLength; ++layer)
				if (loftlist[layer] != ignore && loftlist[layer] != loftid)
					return false;

			return true;
		}

		/// <summary>
		/// Checks if all entries in <paramref name="loftlist"/> are found in
		/// <paramref name="loftids"/>.
		/// </summary>
		/// <param name="loftlist">a list of LoFTs to check against
		/// <paramref name="loftids"/></param>
		/// <param name="loftids">an array of LoFT ids</param>
		/// <param name="ignore"></param>
		/// <returns><c>true</c>if all LoFTs are included in
		/// <paramref name="loftids"/></returns>
		private static bool allare(
				IList<byte> loftlist,
				byte[] loftids,
				byte ignore = LoftInvalid)
		{
			//var sb = new System.Text.StringBuilder();
			//foreach (byte id in loftids) sb = sb.Append(id + ",");
			//DSShared.Logfile.Log("BlobTypeService.allare() loftids= " + sb + " ignore= " + ignore);

			bool found;
			foreach (byte loft in loftlist)
			{
				if (loft != ignore)
				{
					found = false;
					foreach (byte loftid in loftids)
						if (loftid == loft)
						{
							found = true;
							break;
						}
	
					if (!found)
					{
						//DSShared.Logfile.Log(". ret FALSE");
						return false;
					}
				}
			}
			//DSShared.Logfile.Log(". ret TRUE");
			return true;
		}
/*		private static bool allare(
				IList<byte> loftlist,
				byte[] loftids,
				byte[] excluded = null,
				byte include = LoftInvalid)
		{
			var sb = new System.Text.StringBuilder();
			foreach (byte id in loftids) sb = sb.Append(id + ",");
			DSShared.Logfile.Log("BlobTypeService.allare() loftids= " + sb);
			if (excluded != null)
			{
				sb = sb.Clear();
				foreach (byte id in excluded) sb = sb.Append(id + ",");
				DSShared.Logfile.Log(". excluded= " + sb);
			}
			DSShared.Logfile.Log(". include= " + include);

			bool found;
			foreach (byte loft in loftlist)
			{
				if (excluded != null)
					foreach (byte exclude in excluded)
						if (exclude == loft)
							continue;

				if (!(found = (loft == include)))
					foreach (byte loftid in loftids)
						if (loftid == loft)
						{
							found = true;
							break;
						}

				if (!found)
					return false;
			}
			return true;
		} */
/*		private static bool allare(IList<byte> loftlist, byte[] loftids, byte exclude, byte include)
		{
			bool found;
			foreach (byte loft in loftlist)
				if (loft != exclude)
				{
					if (!(found = loft == include))
					{
						foreach (byte loftid in loftids)
							if (loftid == loft)
							{
								found = true;
								break;
							}
					}

					if (!found)
						return false;
				}
			return true;
		} */
/*		private static bool allare(IList<byte> loftlist, byte[] loftids)
		{
			bool found;
			foreach (byte loft in loftlist)
			{
				found = false;
				foreach (byte loftid in loftids)
					if (loftid == loft)
					{
						found = true;
						break;
					}

				if (!found)
					return false;
			}
			return true;
		} */


		/// <summary>
		/// Checks if any entry in <paramref name="loftlist"/> exceeds
		/// <paramref name="loftid"/>.
		/// </summary>
		/// <param name="loftlist">a list of LoFTs to check against
		/// <paramref name="loftid"/></param>
		/// <param name="loftid"><c>McdviewF.LOFTID_Max_ufo</c>
		/// or <c>McdviewF.LOFTID_Max_tftd</c></param>
		/// <returns><c>true</c> if an extended LoFT is detected</returns>
		public static bool hasExtendedLofts(IList<byte> loftlist, byte loftid)
		{
			foreach (byte loft in loftlist)
				if (loft > loftid)
					return true;

			return false;
		}


		/// <summary>
		/// Adds LoFTs to <c><see cref="_loftlist"/></c> for blob-preview in
		/// McdView.
		/// </summary>
		/// <param name="record"></param>
		/// <remarks><c>UpdateLoftlist()</c> is used only by McdView.
		/// <c><see cref="McdRecord">McdRecords</see></c> in MapView store their
		/// own <c><see cref="McdRecord.LoftList">LoftLists</see></c>.
		/// <br/><br/>
		/// TODO: Look into instantiating the McdRecords with 'extra' par</remarks>
		public static void UpdateLoftlist(McdRecord record)
		{
			_loftlist.Clear();

			_loftlist.Add(record.Loft1);
			_loftlist.Add(record.Loft2);
			_loftlist.Add(record.Loft3);
			_loftlist.Add(record.Loft4);
			_loftlist.Add(record.Loft5);
			_loftlist.Add(record.Loft6);
			_loftlist.Add(record.Loft7);
			_loftlist.Add(record.Loft8);
			_loftlist.Add(record.Loft9);
			_loftlist.Add(record.Loft10);
			_loftlist.Add(record.Loft11);
			_loftlist.Add(record.Loft12);
		}
		#endregion Methods (static)
	}
}
