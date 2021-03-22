using System;
using System.Collections.Generic;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The various wall- and content-types that will be used to determine how
	/// to draw the wall- and content-blobs in <see cref="TopView"/> and
	/// <see cref="RouteView"/>.
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
		SoutheastCorner
	}


	/// <summary>
	/// A class that determines how walls and objects are drawn for TopView and
	/// RouteView.
	/// </summary>
	internal static class BlobTypeService
	{
		#region Fields (static)
		private static IList<byte> _loftList;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Gets the BlobType of a given tile for drawing its blob in TopView
		/// and/or RouteView.
		/// </summary>
		/// <remarks>http://www.ufopaedia.org/index.php/LOFTEMPS.DAT</remarks>
		/// <param name="part"></param>
		/// <returns></returns>
		/// <remarks>The checks are not robust; the return BlobType is just a
		/// guess based on what LoFTs have been assigned (externally) to a given
		/// tile.</remarks>
		internal static BlobType GetBlobType(Tilepart part)
		{
			McdRecord record = part.Record;
			if (record != null)
			{
				_loftList = record.LoftList;

				// Floor
				if (isFloor())
					return BlobType.Floor;


				// East
				if (allGroup(new byte[]{24,26})) // 28,30,32,34
					return BlobType.EastWall;

				// South
				if (allGroup(new byte[]{23,25})) // 27,29,31,33
					return BlobType.SouthWall;


				// North ->
				if (anyLoft(38)
					&& allGroup(new byte[]{8,10,12,14,38}))
				{
					return BlobType.NorthWallWindow;
				}

				if (anyLoft(0)
					&& allGroup(new byte[]{0,8,10,12,14,38,39,77})) // 40,41
				{
					return BlobType.NorthWallFence;
				}

				if (allGroup(new byte[]{8,10,12,14})) // 16,18,20,21
					return BlobType.NorthWall;


				// West ->
				if (anyLoft(37)
					&& allGroup(new byte[]{7,9,11,13,37}))
				{
					return BlobType.WestWallWindow;
				}

				if (anyLoft(0)
					&& allGroup(new byte[]{0,7,9,11,13,37,39,76})) // 40,41
				{
					return BlobType.WestWallFence;
				}

				if (allGroup(new byte[]{7,9,11,13})) // 15,17,19,22
					return BlobType.WestWall;


				// diagonals ->
//				if (CheckAllAreLoftExcludeFloor(35))
				if (allLoft(35))
					return BlobType.NorthwestSoutheast;

//				if (CheckAllAreLoftExcludeFloor(36))
				if (allLoft(36))
					return BlobType.NortheastSouthwest;


				// corners ->
				if (allGroup(new byte[]{39,40,41,103})) // 102,101
					return BlobType.NorthwestCorner;

				if (allLoft(100)) // 99,98
					return BlobType.NortheastCorner;

				if (allLoft(106)) // 105,104
					return BlobType.SouthwestCorner;

				if (allLoft(109)) // 108,107
					return BlobType.SoutheastCorner;



				if (allGroup(new byte[]{0,110}))
					return BlobType.NorthWallFence;

				if (allGroup(new byte[]{0,111}))
					return BlobType.WestWallFence;
			}
			return BlobType.Content;
		}

		/// <summary>
		/// Checks if the tilepart is purely Floor-type.
		/// </summary>
		/// <returns></returns>
		private static bool isFloor()
		{
			int length = _loftList.Count;
			for (int layer = 2; layer != length; ++layer)
				if (_loftList[layer] != 0) // that's a stupid check for floor ...
					return false;

			return true;
		}

		/// <summary>
		/// Checks if all entries in '_loftList' are among 'contingent'.
		/// </summary>
		/// <param name="contingent"></param>
		/// <returns></returns>
		private static bool allGroup(byte[] contingent)
		{
			bool found;
			foreach (var loft in _loftList)
			{
				found = false;
				foreach (var gottfried in contingent)
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
		/// Checks if all entries in '_loftList' match 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool allLoft(byte necessary)
		{
			foreach (var loft in _loftList)
				if (loft != necessary)
					return false;

			return true;
		}

		/// <summary>
		/// Checks if any entry in '_loftList' matches 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool anyLoft(byte necessary)
		{
			foreach (var loft in _loftList)
				if (loft == necessary)
					return true;

			return false;
		}
//		private static bool CheckAnyIsLoft(int[] necessary)
//		{
//			foreach (var loft in _loftList)
//				foreach (var gottfried in necessary)
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

		/// <summary>
		/// Checks if a tilepart is either a hinged door or a sliding door.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		internal static bool IsDoor(Tilepart part)
		{
			McdRecord record = part.Record;
			return (record != null
				&& (record.HingedDoor || record.SlidingDoor));
		}
		#endregion Methods (static)
	}
}
