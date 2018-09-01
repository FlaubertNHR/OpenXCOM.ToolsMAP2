namespace XCom
{
	public sealed class Link
	{
		#region Fields (static)
		public const byte NotUsed   = 0xFF;
		public const byte ExitNorth = 0xFE;
		public const byte ExitEast  = 0xFD;
		public const byte ExitSouth = 0xFC;
		public const byte ExitWest  = 0xFB;
		#endregion


		#region Properties
		/// <summary>
		/// Gets/Sets the index of the destination node.
		/// </summary>
		public byte Destination
		{ get; set; }

		/// <summary>
		/// Gets/Sets the distance to the destination node.
		/// </summary>
		public byte Distance
		{ get; set; }

		/// <summary>
		/// Gets/Sets the unit-type that can use this link.
		/// </summary>
		public UnitType Type
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// Creates a Link object.
		/// </summary>
		/// <param name="nodeId">the id of the destination-node</param>
		/// <param name="distance">the distance to the destination-node</param>
		/// <param name="type">the type of units than can use this link</param>
		internal Link(byte nodeId, byte distance, byte type)
		{
			Destination = nodeId;
			Distance    = distance;
			Type        = (UnitType)type;
		}
		#endregion
	}
}
