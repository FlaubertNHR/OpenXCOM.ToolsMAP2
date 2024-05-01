using System;
using System.Collections.Generic;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The various wall- and content-types that will be used to determine how
	/// to draw the wall- and content-blobs in <c><see cref="TopView"/></c> and
	/// <c><see cref="RouteView"/></c>.
	/// </summary>
	internal enum BlobType
	{
		Content,
		EastWall,
		SouthWall,
		NorthWall,
		WestWall,
		NorthwestSoutheast,
		NortheastSouthwest,
		NorthWallWindow,
		WestWallWindow,
		Floor,
		NorthWallFence,
		WestWallFence,
		NorthwestCorner,
		NortheastCorner,
		SouthwestCorner,
		SoutheastCorner,
		Crippled
	}


	/// <summary>
	/// A class that determines how walls and objects are drawn for
	/// <c><see cref="TopView"/></c> and <c><see cref="RouteView"/></c>.
	/// </summary>
	internal static class BlobTypeService
	{
		#region Fields (static)
		private static IList<byte> _loftList;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Gets the <c><see cref="BlobType"/></c> of a specified
		/// <c><see cref="Tilepart"/></c> for drawing its blob in
		/// <c><see cref="TopView"/></c> and <c><see cref="RouteView"/></c>.
		/// </summary>
		/// <remarks>http://www.ufopaedia.org/index.php/LOFTEMPS.DAT</remarks>
		/// <param name="part"></param>
		/// <returns></returns>
		/// <remarks>The checks are not robust; the return <c>BlobType</c> is
		/// just a guess based on what LoFTs have been assigned (externally) to
		/// a <c>Tilepart</c>.</remarks>
		internal static BlobType GetBlobType(Tilepart part)
		{
			McdRecord record = part.Record;
			if (record != null)
			{
				if ((_loftList = record.LoftList) != null) // crippled tileparts have an invalid 'LoftList'
				{
					// Floor
					if (isfloor())
						return BlobType.Floor;


					// East
					if (allingroup(new byte[]{24,26})) // 28,30,32,34
						return BlobType.EastWall;

					// South
					if (allingroup(new byte[]{23,25})) // 27,29,31,33
						return BlobType.SouthWall;


					// North ->
					if (anyare(38)
						&& allingroup(new byte[]{8,10,12,14,38}))
					{
						return BlobType.NorthWallWindow;
					}

					if (anyare(0)
						&& allingroup(new byte[]{0,8,10,12,14,38,39,77})) // 40,41
					{
						return BlobType.NorthWallFence;
					}

					if (allingroup(new byte[]{8,10,12,14})) // 16,18,20,21
						return BlobType.NorthWall;


					// West ->
					if (anyare(37)
						&& allingroup(new byte[]{7,9,11,13,37}))
					{
						return BlobType.WestWallWindow;
					}

					if (anyare(0)
						&& allingroup(new byte[]{0,7,9,11,13,37,39,76})) // 40,41
					{
						return BlobType.WestWallFence;
					}

					if (allingroup(new byte[]{7,9,11,13})) // 15,17,19,22
						return BlobType.WestWall;


					// diagonals ->
//					if (CheckAllAreLoftExcludeFloor(35))
					if (allare(35))
						return BlobType.NorthwestSoutheast;

//					if (CheckAllAreLoftExcludeFloor(36))
					if (allare(36))
						return BlobType.NortheastSouthwest;


					// corners ->
					if (allingroup(new byte[]{39,40,41,103})) // 102,101
						return BlobType.NorthwestCorner;

					if (allare(100)) // 99,98
						return BlobType.NortheastCorner;

					if (allare(106)) // 105,104
						return BlobType.SouthwestCorner;

					if (allare(109)) // 108,107
						return BlobType.SoutheastCorner;



					if (allingroup(new byte[]{0,110}))
						return BlobType.NorthWallFence;
	
					if (allingroup(new byte[]{0,111}))
						return BlobType.WestWallFence;
				}
				else
					return BlobType.Crippled;
			}
			return BlobType.Content;
		}

		/// <summary>
		/// Checks if the tilepart is purely Floor-type.
		/// </summary>
		/// <returns></returns>
		private static bool isfloor()
		{
			int length = _loftList.Count;
			for (int layer = 2; layer != length; ++layer)
				if (_loftList[layer] != 0) // that's a stupid check for floor ...
					return false;

			return true;
		}

		/// <summary>
		/// Checks if all entries in <c><see cref="_loftList"/></c> are among
		/// <paramref name="group"/>.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		private static bool allingroup(byte[] @group)
		{
			bool found;
			foreach (var loft in _loftList)
			{
				found = false;
				foreach (byte gottfried in @group)
					if (gottfried == loft)
					{
						found = true;
						break;
					}

				if (!found)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if all entries in <c><see cref="_loftList"/></c> are
		/// <paramref name="necessary"/>.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool allare(byte necessary)
		{
			foreach (byte loft in _loftList)
				if (loft != necessary)
					return false;

			return true;
		}

		/// <summary>
		/// Checks if any entry in <c><see cref="_loftList"/></c> is
		/// <paramref name="necessary"/>.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool anyare(byte necessary)
		{
			foreach (byte loft in _loftList)
				if (loft == necessary)
					return true;

			return false;
		}
//		private static bool CheckAnyIsLoft(int[] necessary)
//		{
//			foreach (byte loft in _loftList)
//				foreach (byte gottfried in necessary)
//					if (gottfried == loft)
//						return true;
//
//			return false;
//		}

//		private static bool CheckAllAreLoftExcludeFloor(int necessary)
//		{
//			int length = _loftList.Count;
//			for (int layer = 0; layer != length; ++layer)
//			{
//				switch (layer)
//				{
//					case 0:
//						break;
//
//					default:
//						if (_loftList[layer] != necessary)
//							return false;
//						break;
//				}
//			}
//			return true;
//		}

		private const int LOFTID_Max_ufo  = 111;
		private const int LOFTID_Max_tftd = 113;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="part"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		internal static bool hasCustomLofts(Tilepart part, GroupType @group)
		{
			McdRecord record = part.Record;
			if (record != null)
			{
				if ((_loftList = record.LoftList) != null) // crippled tileparts have an invalid 'LoftList'
				{
					foreach (byte loft in _loftList)
					{
						if (@group == GroupType.Tftd && loft > LOFTID_Max_tftd)
							return true;

						if (loft > LOFTID_Max_ufo)
							return true;
					}
				}
			}
			return false;
		}
		#endregion Methods (static)
	}
}
