using System.IO;


namespace XCom
{
	public sealed class RouteNode
	{
		#region Fields (static)
		public const int LinkSlots = 5;
		#endregion Fields (static)


		#region Properties
		public byte Col
		{ get; set; }

		public byte Row
		{ get; set; }

		public int Lev
		{ get; set; }


		private readonly Link[] _links;
		/// <summary>
		/// Gets the link at slot.
		/// </summary>
		public Link this[int slot]
		{
			get { return _links[slot]; }
		}

		public UnitType Unit
		{ get; set; }

		public byte Rank
		{ get; set; }

		public PatrolPriority Patrol
		{ get; set; }

		public AttackBase Attack
		{ get; set; }

		public SpawnWeight Spawn
		{ get; set; }

		/// <summary>
		/// Gets/Sets the index of this RouteNode.
		/// </summary>
		public byte Id
		{ get; internal set; }


		/// <summary>
		/// Catches an out-of-bounds Rank value if it tries to load from the .RMP.
		/// TFTD appears to have ~10 Maps that have OobRanks.
		/// </summary>
		public byte OobRank
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates a node from binary data.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="bindata"></param>
		internal RouteNode(byte id, byte[] bindata)
		{
			Id = id;

			Row = bindata[0]; // NOTE: x & y are switched in the RMP-file.
			Col = bindata[1];
			Lev = bindata[2]; // NOTE: auto-converts to int-type.

			// NOTE: 'bindata[3]' is not used.

			_links = new Link[LinkSlots];

			int offset = 4;
			for (int slot = 0; slot != LinkSlots; ++slot)
			{
				_links[slot] = new Link(
									bindata[offset],
									bindata[offset + 1],
									bindata[offset + 2]);
				offset += 3;
			}

			Unit   = (UnitType)bindata[19];
			Rank   = bindata[20];
			Patrol = (PatrolPriority)bindata[21];
			Attack = (AttackBase)bindata[22];
			Spawn  = (SpawnWeight)bindata[23];

			if (Rank > (byte)8) // NodeRanks are 0..8 (if valid.)
			{
				OobRank = Rank;
				Rank = (byte)9; // invalid case appears in the combobox.
			}
			else
				OobRank = (byte)0;
		}

		/// <summary>
		/// cTor[1]. Creates a node based on row/col/level.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		internal RouteNode(byte id, byte col, byte row, byte lev)
		{
			Id = id;

			Col =      col;
			Row =      row;
			Lev = (int)lev; // NOTE: auto-converts to int-type. But do it explicitly.

			_links = new Link[LinkSlots];
			for (int slot = 0; slot != LinkSlots; ++slot)
				_links[slot] = new Link(Link.NotUsed, 0,0);

			Unit   = UnitType.Any;
			Rank   = (byte)0;
			Patrol = PatrolPriority.Zero;
			Attack = AttackBase.Zero;
			Spawn  = SpawnWeight.None;

			OobRank = (byte)0;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Writes data to a filestream provided by RouteNodes.WriteNodes().
		/// </summary>
		/// <param name="fs"></param>
		internal void WriteNode(Stream fs)
		{
			fs.WriteByte(      Row); // NOTE: col and row are reversed in the file.
			fs.WriteByte(      Col);
			fs.WriteByte((byte)Lev);
			fs.WriteByte((byte)0);

			for (int slot = 0; slot != LinkSlots; ++slot)
			{
				fs.WriteByte(      _links[slot].Destination);
				fs.WriteByte(      _links[slot].Distance);
				fs.WriteByte((byte)_links[slot].Unit);
			}

			fs.WriteByte((byte)Unit);

			if (Rank != (byte)9)
			{
				fs.WriteByte(Rank);
				OobRank = (byte)0; // just clear it.
			}
			else
				fs.WriteByte(OobRank); // else retain the bug in user's .RMP file.

			fs.WriteByte((byte)Patrol);
			fs.WriteByte((byte)Attack);
			fs.WriteByte((byte)Spawn);
		}

		/// <summary>
		/// Gets the location of this node as a string. This funct inverts the
		/// z-level for readability (which is the policy in Mapview II).
		/// @note Always update 'Base1_xy' and 'Base1_z' with user's current
		/// MainView options before calling this funct.
		/// </summary>
		/// <param name="levels">the quantity of z-levels of the Map</param>
		/// <returns>the location of this node as a string</returns>
		public string GetLocationString(int levels)
		{
			byte c = Col;
			byte r = Row;
			byte l = (byte)(levels - Lev);

			if (RouteCheckService.Base1_xy) { ++c; ++r; }
			if (!RouteCheckService.Base1_z) { --l; }

			return ("c " + c + "  r " + r + "  L " + l);
		}
		#endregion Methods


		#region Methods (override)
		public override bool Equals(object obj)
		{
			var other = obj as RouteNode;
			return (other != null && other.Id == Id);
		}

		public override int GetHashCode()
		{
			return Id; // nice hashcode ...
		}

		public override string ToString()
		{
			return ("c " + Col + "  r " + Row + "  L " + Lev);
		}
		#endregion Methods (override)
	}
}
