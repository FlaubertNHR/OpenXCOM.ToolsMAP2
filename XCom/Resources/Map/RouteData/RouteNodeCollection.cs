using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace XCom
{
	#region Enumerations
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
			byte
	{
		Any,			// 0
		FlyingSmall,	// 1
		Small,			// 2
		FlyingLarge,	// 3
		Large			// 4 - aka. 'Any'
//		Any
//		FlyingOnlySmallOnly
//		SmallOnlyWalkOrFly
//		FlyingOnlyLargeOrSmall
//		LargeOrSmallWalkOrFly
	};

	public enum NodeRankUfo
		:
			byte
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
			byte
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
			byte
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
			byte
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
			byte
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
			byte
	{
		None      = 0x00, // pacify FxCop CA1008 BUT DO NOT USE IT.
		NotUsed   = 0xFF, // since valid route-nodes can and will have a value of 0.
		ExitNorth = 0xFE,
		ExitEast  = 0xFD,
		ExitSouth = 0xFC,
		ExitWest  = 0xFB
	};
	#endregion Enumerations


	/// <summary>
	/// This class reads, saves, and generally manages all the information in a
	/// .RMP file. It's like the parent of RouteNode.
	/// </summary>
	public sealed class RouteNodeCollection
		:
			IEnumerable<RouteNode>
	{
		#region Fields (static)
		private const string civscout  = "0 : Civilian/Scout";
		private const string xcom      = "1 : XCOM";
		private const string soldier   = "2 : Soldier";
		private const string navigator = "3 : Navigator";
		private const string squadldr  = "3 : Squad Leader";
		private const string lc        = "4 : Leader/Commander";
		private const string engineer  = "5 : Engineer";
		private const string ter1      = "6 : Terrorist1";
		private const string medic     = "7 : Medic";
		private const string techie    = "7 : Technician";
		private const string ter2      = "8 : Terrorist2";

		private const string RankInvalid = "INVALID"; // WORKAROUND for several bugged Route files in TftD.

		private const string none0 =  "0 : None";
		private const string lolo0 =  "0 : LoLo";
		private const string lo1   =  "1 : Lo";
		private const string lo2   =  "2 : Lo";
		private const string lo3   =  "3 : Lo";
		private const string med4  =  "4 : Med";
		private const string med5  =  "5 : Med";
		private const string med6  =  "6 : Med";
		private const string med7  =  "7 : Med";
		private const string hi8   =  "8 : Hi";
		private const string hi9   =  "9 : Hi";
		private const string hi10  = "10 : Hi";

		public static readonly object[] RankUfo =
		{
			new Pterodactyl(civscout,    NodeRankUfo.CivScout),
			new Pterodactyl(xcom,        NodeRankUfo.XCOM),
			new Pterodactyl(soldier,     NodeRankUfo.Soldier),
			new Pterodactyl(navigator,   NodeRankUfo.Navigator),
			new Pterodactyl(lc,          NodeRankUfo.LeaderCommander),
			new Pterodactyl(engineer,    NodeRankUfo.Engineer),
			new Pterodactyl(ter1,        NodeRankUfo.Misc1),
			new Pterodactyl(medic,       NodeRankUfo.Medic),
			new Pterodactyl(ter2,        NodeRankUfo.Misc2),
			new Pterodactyl(RankInvalid, NodeRankUfo.invalid) // WORKAROUND.
		};

		public static readonly object[] RankTftd =
		{
			new Pterodactyl(civscout,    NodeRankTftd.CivScout),
			new Pterodactyl(xcom,        NodeRankTftd.XCOM),
			new Pterodactyl(soldier,     NodeRankTftd.Soldier),
			new Pterodactyl(squadldr,    NodeRankTftd.SquadLeader),
			new Pterodactyl(lc,          NodeRankTftd.LeaderCommander),
			new Pterodactyl(medic,       NodeRankTftd.Medic),
			new Pterodactyl(ter1,        NodeRankTftd.Misc1),
			new Pterodactyl(techie,      NodeRankTftd.Technician),
			new Pterodactyl(ter2,        NodeRankTftd.Misc2),
			new Pterodactyl(RankInvalid, NodeRankTftd.invalid) // WORKAROUND.
		};

		public static readonly object[] Spawn =
		{
			new Pterodactyl(none0, SpawnWeight.None),
			new Pterodactyl(lo1,   SpawnWeight.Spawn1),
			new Pterodactyl(lo2,   SpawnWeight.Spawn2),
			new Pterodactyl(lo3,   SpawnWeight.Spawn3),
			new Pterodactyl(med4,  SpawnWeight.Spawn4),
			new Pterodactyl(med5,  SpawnWeight.Spawn5),
			new Pterodactyl(med6,  SpawnWeight.Spawn6),
			new Pterodactyl(med7,  SpawnWeight.Spawn7),
			new Pterodactyl(hi8,   SpawnWeight.Spawn8),
			new Pterodactyl(hi9,   SpawnWeight.Spawn9),
			new Pterodactyl(hi10,  SpawnWeight.Spawn10)
		};

		public static readonly object[] Patrol =
		{
			new Pterodactyl(lolo0, PatrolPriority.Zero),
			new Pterodactyl(lo1,   PatrolPriority.One),
			new Pterodactyl(lo2,   PatrolPriority.Two),
			new Pterodactyl(lo3,   PatrolPriority.Three),
			new Pterodactyl(med4,  PatrolPriority.Four),
			new Pterodactyl(med5,  PatrolPriority.Five),
			new Pterodactyl(med6,  PatrolPriority.Six),
			new Pterodactyl(med7,  PatrolPriority.Seven),
			new Pterodactyl(hi8,   PatrolPriority.Eight),
			new Pterodactyl(hi9,   PatrolPriority.Nine),
			new Pterodactyl(hi10,  PatrolPriority.Ten)
		};

		public static readonly object[] Attack =
		{
			new Pterodactyl(none0, AttackBase.Zero),
			new Pterodactyl(lo1,   AttackBase.One),
			new Pterodactyl(lo2,   AttackBase.Two),
			new Pterodactyl(lo3,   AttackBase.Three),
			new Pterodactyl(med4,  AttackBase.Four),
			new Pterodactyl(med5,  AttackBase.Five),
			new Pterodactyl(med6,  AttackBase.Six),
			new Pterodactyl(med7,  AttackBase.Seven),
			new Pterodactyl(hi8,   AttackBase.Eight),
			new Pterodactyl(hi9,   AttackBase.Nine),
			new Pterodactyl(hi10,  AttackBase.Ten)
		};
		#endregion Fields (static)


		#region Fields
		private readonly List<RouteNode> _nodes = new List<RouteNode>();
		#endregion Fields


		#region Properties (static)
		/// <summary>
		/// The fullpath of the .RMP file.
		/// Is static to maintain its value when importing a different .RMP file.
		/// </summary>
		private static string Fullpath
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		/// <summary>
		/// Returns the node at id.
		/// </summary>
		public RouteNode this[int id]
		{
			get
			{
				if (id > -1 && id < _nodes.Count)
					return _nodes[id];

				return null;
			}
		}

		/// <summary>
		/// Returns the count of RouteNodes in this collection.
		/// </summary>
		public int Count
		{
			get { return _nodes.Count; }
		}
		#endregion Properties


		#region cTors
		/// <summary>
		/// cTor[1]. Reads the .RMP file and adds its data as RouteNodes to a
		/// List.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="basepath"></param>
		public RouteNodeCollection(string label, string basepath)
		{
			Fullpath = Path.Combine(
								Path.Combine(basepath, GlobalsXC.RoutesDir),
								label + GlobalsXC.RouteExt);

			if (File.Exists(Fullpath))
				Instantiate(Fullpath);
		}

		/// <summary>
		/// cTor[2]. Imports an .RMP file and replaces the RouteNodes-list with
		/// its data.
		/// @note Do *not* replace 'Fullpath' on an import.
		/// </summary>
		/// <param name="pfe"></param>
		public RouteNodeCollection(string pfe)
		{
			Instantiate(pfe);
		}

		/// <summary>
		/// Helper for the cTors. Reads a file into this RouteNodeCollection.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		private void Instantiate(string pfe)
		{
			using (var bs = new BufferedStream(File.OpenRead(pfe)))
			{
				for (byte id = 0; id < bs.Length / 24; ++id)
				{
					var bindata = new byte[24];
					bs.Read(bindata, 0, 24);

					_nodes.Add(new RouteNode(id, bindata));
				}
			}

			var invalids = new List<byte>();	// check for invalid Ranks ->
			foreach (RouteNode node in _nodes)	// See also RouteView.OnCheckNodeRanksClick()
			{
				if (node.OobRank != (byte)0)
					invalids.Add(node.Index);
			}

			if (invalids.Count != 0)
			{
				string info = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"The following route-{0} an invalid NodeRank.{1}",
										(invalids.Count == 1) ? "node has"
															  : "nodes have",
										Environment.NewLine);

				foreach (byte id in invalids)
					info += Environment.NewLine + id;

				MessageBox.Show(
							info,
							" Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}
		#endregion cTors


		#region Interface requirements
		/// <summary>
		/// Gets an enumerator for the node-list.
		/// </summary>
		/// <returns></returns>
		IEnumerator<RouteNode> IEnumerable<RouteNode>.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		/// <summary>
		/// Gets another enumerator for the node-list.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}
		#endregion Interface requirements


		#region Methods
		/// <summary>
		/// Saves the .RMP file.
		/// </summary>
		internal void SaveRoutes()
		{
			SaveNodes(Fullpath);
		}

		/// <summary>
		/// Saves the .RMP file as a different file.
		/// </summary>
		/// <param name="pf">the path+file to save as</param>
		internal void SaveRoutes(string pf)
		{
			string pfe = pf + GlobalsXC.RouteExt;
			Directory.CreateDirectory(Path.GetDirectoryName(pfe));
			SaveNodes(pfe);
		}

		/// <summary>
		/// Saves the .RMP file as a different file.
		/// </summary>
		/// <param name="pfe">the path+file to save as</param>
		public void SaveRoutesExport(string pfe)
		{
			SaveNodes(pfe);
		}

		/// <summary>
		/// Saves the route-nodes to a .RMP file.
		/// </summary>
		/// <param name="pfe">path+file+extension</param>
		private void SaveNodes(string pfe)
		{
			using (var fs = File.Create(pfe))
			{
				for (int id = 0; id != _nodes.Count; ++id)
					_nodes[id].SaveNode(fs); // -> RouteNode.SaveNode() writes each node-data
			}
		}

		/// <summary>
		/// Adds a node to the node-list.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		/// <returns></returns>
		internal RouteNode AddNode(byte row, byte col, byte lev)
		{
			var node = new RouteNode((byte)_nodes.Count, row, col, lev);
			_nodes.Add(node);

			return node;
		}

		/// <summary>
		/// Deletes a node from the node-list.
		/// </summary>
		/// <param name="node">the node to delete</param>
		public void DeleteNode(RouteNode node)
		{
			int id = node.Index;

			_nodes.Remove(node);

			foreach (var node0 in _nodes)
			{
				if (node0.Index > id) // shuffle all higher-indexed nodes down 1
					--node0.Index;

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
				{
					var link = node0[slot];
					if (link.StandardNode())
					{
						if (link.Destination == id)
							link.Destination = Link.NotUsed;
						else if (link.Destination > id)
							--link.Destination;
					}
				}
			}
		}

		/// <summary>
		/// Checks if a given node is outside the Map boundaries.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="cols"></param>
		/// <param name="rows"></param>
		/// <param name="levs"></param>
		/// <returns></returns>
		public static bool IsNodeOutsideMapBounds(
				RouteNode node,
				int cols,
				int rows,
				int levs)
		{
			return node.Col < 0 || node.Col >= cols
				|| node.Row < 0 || node.Row >= rows
				|| node.Lev < 0 || node.Lev >= levs;
		}
		#endregion Methods
	}
}
