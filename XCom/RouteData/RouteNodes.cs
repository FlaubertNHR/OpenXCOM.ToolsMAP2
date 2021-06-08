using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	/// <summary>
	/// This class reads, saves, and generally manages all the information in a
	/// .RMP file. It contains <see cref="RouteNode"/>'s.
	/// </summary>
	public sealed class RouteNodes // fxCop ca1710 - wants "RouteNodeCollection"
		:
			IEnumerable<RouteNode>
	{
		#region Fields (static)
		private const int Length_Routenode = 24; // each node has 24 bytes

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

		/// <summary>
		/// Workaround for several bugged Route-files in TftD.
		/// </summary>
		/// <remarks>TODO: Repeat this pattern for the other Route-file vars.</remarks>
		private const string RankInvalid = "INVALID";

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

		/// <summary>
		/// RankUfo <see cref="Pterodactyl">Pterodactyls</see>.
		/// </summary>
		/// <remarks>treat as readonly - ca2105</remarks>
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

		/// <summary>
		/// RankTftd <see cref="Pterodactyl">Pterodactyls</see>.
		/// </summary>
		/// <remarks>treat as readonly - ca2105</remarks>
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

		/// <summary>
		/// Spawn <see cref="Pterodactyl">Pterodactyls</see>.
		/// </summary>
		/// <remarks>treat as readonly - ca2105</remarks>
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

		/// <summary>
		/// Patrol <see cref="Pterodactyl">Pterodactyls</see>.
		/// </summary>
		/// <remarks>treat as readonly - ca2105</remarks>
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

		/// <summary>
		/// Attack <see cref="Pterodactyl">Pterodactyls</see>.
		/// </summary>
		/// <remarks>treat as readonly - ca2105</remarks>
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


		#region Properties (static)
		/// <summary>
		/// The fullpath of the .RMP file.
		/// </summary>
		/// <remarks>Is static to maintain its value when importing a different
		/// .RMP file.</remarks>
		private static string PfeRoutes
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		private readonly IList<RouteNode> _nodes = new List<RouteNode>();
		public IList<RouteNode> Nodes
		{
			get { return _nodes; }
		}

		/// <summary>
		/// Returns the node at id.
		/// </summary>
		public RouteNode this[int id]
		{
			get
			{
				if (id > -1 && id < Nodes.Count) // TODO: Get rid of that.
					return Nodes[id];

				return null;
			}
		}

		public bool Fail
		{ get; internal set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Reads a Routefile and adds its nodes to a list.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="basepath"></param>
		public RouteNodes(string label, string basepath)
		{
			string dir = Path.Combine(basepath, GlobalsXC.RoutesDir);
			PfeRoutes  = Path.Combine(dir, label + GlobalsXC.RouteExt);

			if (!LoadNodes(PfeRoutes))
				Fail = true;
		}

		/// <summary>
		/// cTor[1]. Imports a Routefile and replaces the current nodes-list
		/// with its nodes.
		/// </summary>
		/// <param name="pfe"></param>
		/// <remarks>Do *not* replace 'PfeRoutes' on an import.</remarks>
		public RouteNodes(string pfe)
		{
			if (!LoadNodes(pfe))
				Fail = true;
		}

		/// <summary>
		/// Helper for the cTors. Reads a file into this RouteNodes.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>true if situation normal</returns>
		private bool LoadNodes(string pfe)
		{
			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			{
				int length = (int)fs.Length / Length_Routenode;
				for (byte id = 0; id < length; ++id)
				{
					var bindata = new byte[Length_Routenode];
					fs.Read(bindata, 0, Length_Routenode);

					Nodes.Add(new RouteNode(id, bindata));
				}

				var invalids = new List<byte>();	// check for invalid Ranks ->
				foreach (RouteNode node in Nodes)	// See also RouteView.OnCheckNodeRanksClick()
				{
					if (node.OobRank != (byte)0)
						invalids.Add(node.Id);
				}

				if (invalids.Count != 0)
				{
					string head = "The following route-"
								+ ((invalids.Count == 1) ? "node has" : "nodes have")
								+ " an invalid NodeRank.";

					string copyable = String.Empty;
					foreach (byte id in invalids)
					{
						if (copyable.Length != 0) copyable += Environment.NewLine;
						copyable += id.ToString();
					}

					using (var f = new Infobox(
											"Invalid noderanks",
											head,
											copyable,
											InfoboxType.Warn))
					{
						f.ShowDialog();
					}
				}
				return true;
			}
			return false;
		}
		#endregion cTor


		#region Interface requirements
		/// <summary>
		/// Gets an enumerator for the node-list.
		/// </summary>
		/// <returns></returns>
		IEnumerator<RouteNode> IEnumerable<RouteNode>.GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}

		/// <summary>
		/// Gets another enumerator for the node-list.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}
		#endregion Interface requirements


		#region Methods
		/// <summary>
		/// Saves the current Routefile.
		/// </summary>
		/// <returns>true on success</returns>
		internal bool SaveRoutes()
		{
			return WriteNodes(PfeRoutes);
		}

		/// <summary>
		/// Exports the routes to a different file.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		public void ExportRoutes(string pfe)
		{
			WriteNodes(pfe);
		}

		/// <summary>
		/// Saves the routes to a Routefile.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>true on success</returns>
		private bool WriteNodes(string pfe)
		{
			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;

				for (int id = 0; id != Nodes.Count; ++id)
					Nodes[id].WriteNode(fs); // -> writes a node's data to the filestream
			}

			if (!fail && pfeT != pfe)
				return FileService.ReplaceFile(pfe);

			return !fail;
		}

		/// <summary>
		/// Adds a node to the node-list.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <returns></returns>
		internal RouteNode AddNode(byte col, byte row, byte lev)
		{
			var node = new RouteNode((byte)Nodes.Count, col, row, lev);
			Nodes.Add(node);

			return node;
		}

		/// <summary>
		/// Deletes a node from the node-list.
		/// </summary>
		/// <param name="node">the node to delete</param>
		public void DeleteNode(RouteNode node)
		{
			int id = node.Id;

			Nodes.Remove(node);

			foreach (var node0 in Nodes)
			{
				if (node0.Id > id) // shuffle all higher-indexed nodes down 1
					--node0.Id;

				Link link;

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
				{
					if ((link = node0[slot]).IsNodelink())
					{
						if (link.Destination == id)
						{
							link.Destination = Link.NotUsed;
							link.Distance    = (byte)0;
							link.Unit        = UnitType.Any;
						}
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
		public static bool OutsideMapBounds(
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
