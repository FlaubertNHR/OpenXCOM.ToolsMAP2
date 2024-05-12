using System;
using System.Collections.Generic;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The various content- and wall-types that will be used to determine how
	/// to draw the content- and wall-blobs in <c><see cref="TopView"/></c> and
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
	/// A class that determines how content- and wall-parts are drawn for
	/// <c><see cref="TopView"/></c> and <c><see cref="RouteView"/></c>.
	/// </summary>
	internal static class BlobTypeService
	{
		#region Fields (static)
		private static IList<byte> _loftList;

		private const int LoftListLength = 12;
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
					if (isfloorlike())
						return BlobType.Floor;


					// East
					if (allareincluded(new byte[]{24,26,28,30,32,34,47}))
						return BlobType.EastWall;

					// South
					if (allareincluded(new byte[]{23,25,27,29,31,33,44}))
						return BlobType.SouthWall;


					// North ->
					if (anyare(38)
						&& allareincluded(new byte[]{8,10,12,14,38}))
					{
						return BlobType.NorthWallWindow;
					}

					if (anyare(0)
						&& allareincluded(new byte[]{0,8,10,12,14,38,39,40,41,77,110}))
					{
						return BlobType.NorthWallFence;
					}

					if (allareincluded(new byte[]{8,10,12,14,16,18,20,21}))
						return BlobType.NorthWall;


					// West ->
					if (anyare(37)
						&& allareincluded(new byte[]{7,9,11,13,37}))
					{
						return BlobType.WestWallWindow;
					}

					if (anyare(0)
						&& allareincluded(new byte[]{0,7,9,11,13,37,39,40,41,76,111}))
					{
						return BlobType.WestWallFence;
					}

					if (allareincluded(new byte[]{7,9,11,13,15,17,19,22}))
						return BlobType.WestWall;


					// diagonals ->
					if (allare(35))
						return BlobType.NorthwestSoutheast;

					if (allare(36))
						return BlobType.NortheastSouthwest;


					// corners ->
					if (allareincluded(new byte[]{0,39,40,41,101,102,103}))
						return BlobType.NorthwestCorner;

					if (allareincluded(new byte[]{0,98,99,100}))
						return BlobType.NortheastCorner;

					if (allareincluded(new byte[]{0,104,105,106}))
						return BlobType.SouthwestCorner;

					if (allareincluded(new byte[]{0,107,108,109}))
						return BlobType.SoutheastCorner;
				}
				else
					return BlobType.Crippled;
			}
			return BlobType.Content;
		}

		/// <summary>
		/// Checks if <c><see cref="_loftList"/></c> has only LoFT id #0 (blank
		/// LoFT) above the first layer.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This function checks LoFTs only of content- and wall-parts
		/// for
		/// <c><see cref="BlobDrawService.DrawContentOrWall()">BlobDrawService.DrawContentOrWall()</see></c>
		/// but is not actually used for floor-parts which are instead drawn by
		/// <c><see cref="BlobDrawService.DrawFloor()">BlobDrawService.DrawFloor()</see></c>.
		/// Loftid #6 on layer #0 is the fullfloor LoFT but is not checked for.</remarks>
		private static bool isfloorlike()
		{
			for (int layer = 1; layer != LoftListLength; ++layer)
				if (_loftList[layer] != 0) // that's kind of a stupid check for floor ...
					return false;

			return true;
		}

		/// <summary>
		/// Checks if all entries in <c><see cref="_loftList"/></c> are included
		/// in <paramref name="loftids"/>.
		/// </summary>
		/// <param name="loftids">an array of LoFT ids</param>
		/// <returns><c>true</c>if all LoFTs are included in
		/// <paramref name="loftids"/></returns>
		private static bool allareincluded(byte[] loftids)
		{
			bool found;
			foreach (var loft in _loftList)
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
		}

		/// <summary>
		/// Checks if all entries in <c><see cref="_loftList"/></c> are
		/// <paramref name="loftid"/>.
		/// </summary>
		/// <param name="loftid">the required loftid</param>
		/// <returns><c>true</c> if all LoFTs are <paramref name="loftid"/></returns>
		/// <remarks>Layer #0 is NOT considered.</remarks>
		private static bool allare(byte loftid)
		{
			for (int layer = 1; layer != LoftListLength; ++layer)
				if (_loftList[layer] != loftid)
					return false;

			return true;
		}

		/// <summary>
		/// Checks if any entry in <c><see cref="_loftList"/></c> is
		/// <paramref name="loftid"/>.
		/// </summary>
		/// <param name="loftid">the required loftid</param>
		/// <returns><c>true</c> if any LoFT is <paramref name="loftid"/></returns>
		private static bool anyare(byte loftid)
		{
			for (int layer = 0; layer != LoftListLength; ++layer)
				if (_loftList[layer] == loftid)
					return true;

			return false;
		}

		/// <summary>
		/// Checks if any LoFTS of a specified <c><see cref="Tilepart"/></c>
		/// exceed the stock UFO or TFTD LoFT ids.
		/// </summary>
		/// <param name="part">a <c>Tilepart</c> to check the LoFTs of</param>
		/// <param name="loftid"><c><see cref="TopControl"/>.LOFTID_Max_ufo</c>
		/// or <c>TopControl.LOFTID_Max_tftd</c></param>
		/// <returns><c>true</c> if <paramref name="part"/> has extended LoFTs</returns>
		internal static bool hasExtendedLofts(Tilepart part, byte loftid)
		{
			if (part.Record != null
				&& (_loftList = part.Record.LoftList) != null) // crippled tileparts have an invalid 'LoftList'
			{
				foreach (byte loft in _loftList)
					if (loft > loftid)
						return true;
			}
			return false;
		}
		#endregion Methods (static)
	}
}
