namespace XCom
{
	#region Enums (node characteristics)
	// NOTE: Only 'NodeRankUfo' and 'NodeRankTftd' need to be enumerated as
	// byte-type. Otherwise the Pterodactyl class goes snakey when
	// RouteView.OnNodeRankSelectedIndexChanged() fires. For reasons, it cannot
	// handle the cast automatically like the other enumerated types here appear
	// to. But I left the others as bytes also for consistency.

	// Rules on nodes and node-links (OxC)
	//
	// - unittype is used for spawning and patrolling only; it is not used by
	//   links
	// - noderank affects both spawning and patrolling, but note that noderank
	//   has a fallback mechanic for spawning such that if no node with an
	//   aLien's rank is found, a succession of (all) other ranks will be
	//   investigated (but not XCOM rank ofc)
	// - re. unittypes: small units are allowed to use large nodes but not vice
	//   versa and flying units are allowed to use non-flying nodes but not vice
	//   versa. Thus 'Large' nodes are effectively identical to 'Any' nodes.
	// - link distance is not used
	// - spawnweight 0 disallows spawning at a node, but patrolpriority 0 is
	//   valid for patrolling to a node if a unit is flagged, by OxC, to "scout"
	//   (details tbd) else patrolpriority 0 disallows patrolling the node: the
	//   OxC "scout" flag appears to be, at least in part, another fallback
	//   mechanic - that is, an aLien will check for valid non-scout nodes first
	//   but if none are found, the routine then checks for valid "scout" nodes.
	//   But don't quote me on that; there's more going on between (a)
	//   patrolpriority, (b) noderank, and (c) the "scout" flag ...
	// - it appears that if the OxC "scout" flag is not set, then the aLien to
	//   which it's being applied will not leave the block it's currently in.
	//   More investigation req'd
	//   - quote from the OxC code:
	//       "scouts roam all over while all others shuffle around to adjacent
	//        nodes at most"
	//   I believe, at a guess, that this is designed to keep Commanders in the
	//   command module, eg, or at least increase the chance of aLiens sticking
	//   around their non-CivScout patrol nodes. Long story short: OxC has
	//   hardcoded patrolling behavior beyond what can be determined by the
	//   Route files. (I didn't look at OxCe)
	//
	// 0 = Any, 1 = Flying, 2 = Flying Large, 3 = Large, 4 = Small <- UfoPaedia.Org BZZZT.

	public enum UnitType
		:
			byte // ca1028 - use Int32
	{
		Any,			// 0
		FlyingSmall,	// 1
		Small,			// 2
		FlyingLarge,	// 3
		Large			// 4 - aka. 'Any'
	};

	public enum NodeRankUfo
		:
			byte // ca1028 - use Int32
	{
		CivScout,			// 0
		XCOM,				// 1
		Soldier,			// 2
		Navigator,			// 3
		LeaderCommander,	// 4
		Engineer,			// 5
		Misc1,				// 6
		Medic,				// 7
		Misc2,				// 8
		invalid				// 9 - WORKAROUND.
	};

	public enum NodeRankTftd
		:
			byte // ca1028 - use Int32
	{
		CivScout,			// 0
		XCOM,				// 1
		Soldier,			// 2
		SquadLeader,		// 3
		LeaderCommander,	// 4
		Medic,				// 5
		Misc1,				// 6
		Technician,			// 7
		Misc2,				// 8
		invalid				// 9 - WORKAROUND.
	};

	public enum SpawnWeight
		:
			byte // ca1028 - use Int32
	{
		None,	// 0
		Spawn1,	// 1
		Spawn2,	// 2
		Spawn3,	// 3
		Spawn4,	// 4
		Spawn5,	// 5
		Spawn6,	// 6
		Spawn7,	// 7
		Spawn8,	// 8
		Spawn9,	// 9
		Spawn10	// 10
	};

	public enum PatrolPriority
		:
			byte // ca1028 - use Int32
	{
		Zero,	// 0
		One,	// 1
		Two,	// 2
		Three,	// 3
		Four,	// 4
		Five,	// 5
		Six,	// 6
		Seven,	// 7
		Eight,	// 8
		Nine,	// 9
		Ten		// 10
	};

	public enum AttackBase
		:
			byte // ca1028 - use Int32
	{
		Zero,	// 0
		One,	// 1
		Two,	// 2
		Three,	// 3
		Four,	// 4
		Five,	// 5
		Six,	// 6
		Seven,	// 7
		Eight,	// 8
		Nine,	// 9
		Ten		// 10
	};

	public enum LinkType
		:
			byte // ca1028 - use Int32
	{
		None      = 0x00, // pacify FxCop ca1008 BUT DO NOT USE IT.
		NotUsed   = 0xFF, // since valid route-nodes can and will have a value of 0.
		ExitNorth = 0xFE,
		ExitEast  = 0xFD,
		ExitSouth = 0xFC,
		ExitWest  = 0xFB
	};
	#endregion Enums (node characteristics)
}
