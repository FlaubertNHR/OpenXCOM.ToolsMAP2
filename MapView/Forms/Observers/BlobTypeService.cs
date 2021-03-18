using System;
using System.Collections.Generic;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// A class that determines how walls and objects are drawn for TopView and
	/// RouteView.
	/// </summary>
	internal static class BlobTypeService
	{
		#region Enums
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
		#endregion Enums


		#region Fields (static)
		private static List<byte> _loftList;
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
				_loftList = record.GetLoftList();

				// Floor
				if (CheckFloor())
					return BlobType.Floor;


				// East
				if (CheckAllAreInGroup(new[]{ 24, 26 }))//, 28, 30, 32, 34 }))
					return BlobType.EastWall;

				// South
				if (CheckAllAreInGroup(new[]{ 23, 25 }))//, 27, 29, 31, 33 }))
					return BlobType.SouthWall;


				// North ->
				if (CheckAnyIsLoft(38)
					&& CheckAllAreInGroup(new[]{ 8, 10, 12, 14, 38 }))
				{
					return BlobType.NorthWallWindow;
				}

				if (CheckAnyIsLoft(0)
					&& CheckAllAreInGroup(new[]{ 0, 8, 10, 12, 14, 38, 39, 77 })) // 40,41
				{
					return BlobType.NorthWallFence;
				}

				if (CheckAllAreInGroup(new[]{ 8, 10, 12, 14 }))//, 16, 18, 20, 21 }))
					return BlobType.NorthWall;


				// West ->
				if (CheckAnyIsLoft(37)
					&& CheckAllAreInGroup(new[]{ 7, 9, 11, 13, 37 }))
				{
					return BlobType.WestWallWindow;
				}

				if (CheckAnyIsLoft(0)
					&& CheckAllAreInGroup(new[]{ 0, 7, 9, 11, 13, 37, 39, 76 })) // 40,41
				{
					return BlobType.WestWallFence;
				}

				if (CheckAllAreInGroup(new[]{ 7, 9, 11, 13 }))//, 15, 17, 19, 22 }))
					return BlobType.WestWall;


				// diagonals ->
//				if (CheckAllAreLoftExcludeFloor(35))
				if (CheckAllAreLoft(35))
					return BlobType.NorthwestSoutheast;

//				if (CheckAllAreLoftExcludeFloor(36))
				if (CheckAllAreLoft(36))
					return BlobType.NortheastSouthwest;


				// corners ->
				if (CheckAllAreInGroup(new[]{ 39, 40, 41, 103 })) // 102,101
					return BlobType.NorthwestCorner;

				if (CheckAllAreLoft(100)) // 99,98
					return BlobType.NortheastCorner;

				if (CheckAllAreLoft(106)) // 105,104
					return BlobType.SouthwestCorner;

				if (CheckAllAreLoft(109)) // 108,107
					return BlobType.SoutheastCorner;
			}
			return BlobType.Content;
		}

		/// <summary>
		/// Checks if the tilepart is purely Floor-type.
		/// </summary>
		/// <returns></returns>
		private static bool CheckFloor()
		{
			int length = _loftList.Count;
			for (int layer = 0; layer != length; ++layer)
			{
				switch (layer)
				{
					case 0:
					case 1:
						break;

					default:
						if (_loftList[layer] != 0)
							return false;
						break;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if all entries in '_loftList' are among 'contingent'.
		/// </summary>
		/// <param name="contingent"></param>
		/// <returns></returns>
		private static bool CheckAllAreInGroup(int[] contingent)
		{
			foreach (var loft in _loftList)
			{
				bool valid = false;
				foreach (var gottfried in contingent)
					if (gottfried == loft)
					{
						valid = true;
						break;
					}

				if (!valid)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if all entries in '_loftList' match 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool CheckAllAreLoft(int necessary)
		{
			foreach (var loft in _loftList)
				if (loft != necessary)
					return false;

			return true;
		}

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
		/// Checks if any entry in '_loftList' matches 'necessary'.
		/// </summary>
		/// <param name="necessary"></param>
		/// <returns></returns>
		private static bool CheckAnyIsLoft(int necessary)
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
