using System;
using System.Collections.ObjectModel;


namespace XCom
{
	public class McdRecordCollection
		:
			ReadOnlyCollection<Tilepart>
	{
		#region cTor
		/// <summary>
		/// Instantiates a read-only collection of MCD records.
		/// @note This object will be used for approximately 100ns in
		/// 'MapFileService'.
		/// @note This is not an "McdRecordCollection"; it's a
		/// "TilepartCollection".
		/// </summary>
		/// <param name="parts"></param>
		internal McdRecordCollection(Tilepart[] parts)
			:
				base(parts)
		{}
		#endregion
	}
}
