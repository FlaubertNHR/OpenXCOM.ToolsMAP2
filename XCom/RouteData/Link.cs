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

		public const byte MaxDestId = 250;
		#endregion Fields (static)


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
		public UnitType Unit
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// Creates a <c>Link</c> object.
		/// </summary>
		/// <param name="id">the id of the destination-node</param>
		/// <param name="distance">the distance to the destination-node</param>
		/// <param name="unit">the type of unit than can use this link</param>
		internal Link(byte id, byte distance, byte unit)
		{
			Destination = id;
			Distance    = distance;
			Unit        = (UnitType)unit;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Checks if this is a node-link rather than an exit-link or a not-used
		/// link.
		/// </summary>
		/// <returns><c>true</c> if node-link</returns>
		public bool IsNodelink()
		{
			return (Destination < ExitWest);
		}
		#endregion Methods
	}
}
