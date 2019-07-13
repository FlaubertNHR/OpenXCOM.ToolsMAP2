using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;

using XCom;
using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed partial class RoutesInfo
		:
			Form
	{
		#region Fields
		private MapFile _file;

		private string _group    = String.Empty;
		private string _category = String.Empty;

		private int
			_nodes,
			_ranks_0,
			_ranks_1,
			_ranks_2,
			_ranks_3,
			_ranks_4,
			_ranks_5,
			_ranks_6,
			_ranks_7,
			_ranks_8,
			_ranks_0_cat,
			_ranks_1_cat,
			_ranks_2_cat,
			_ranks_3_cat,
			_ranks_4_cat,
			_ranks_5_cat,
			_ranks_6_cat,
			_ranks_7_cat,
			_ranks_8_cat;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Instantiates a RoutesInfo screen.
		/// </summary>
		/// <param name="file"></param>
		internal RoutesInfo(MapFile file)
		{
			InitializeComponent();
			Initialize(file);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			RouteView.RoutesInfo = null;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();

//			base.OnKeyUp(e);
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		internal void Initialize(MapFile file)
		{
			_file = file;

			object[] pters;
			if (_file.Descriptor.Pal == Palette.TftdBattle)
				pters = RouteNodeCollection.NodeRankTftd;
			else
				pters = RouteNodeCollection.NodeRankUfo;

			lbl_tsRanks0.Text = pters[0].ToString();
			lbl_tsRanks1.Text = pters[1].ToString();
			lbl_tsRanks2.Text = pters[2].ToString();
			lbl_tsRanks3.Text = pters[3].ToString();
			lbl_tsRanks4.Text = pters[4].ToString();
			lbl_tsRanks5.Text = pters[5].ToString();
			lbl_tsRanks6.Text = pters[6].ToString();
			lbl_tsRanks7.Text = pters[7].ToString();
			lbl_tsRanks8.Text = pters[8].ToString();

			InitRanks();
			InitRanksCategory();

			lbl_Label        .Text = _file.Descriptor.Label;
			lbl_NodesQuantity.Text = _nodes.ToString();
			lbl_Group        .Text = _group;
			lbl_Category     .Text = _category;

			thisLayout();
		}

		private void ResetTallies()
		{
			_nodes       =
			_ranks_0     =
			_ranks_1     =
			_ranks_2     =
			_ranks_3     =
			_ranks_4     =
			_ranks_5     =
			_ranks_6     =
			_ranks_7     =
			_ranks_8     =
			_ranks_0_cat =
			_ranks_1_cat =
			_ranks_2_cat =
			_ranks_3_cat =
			_ranks_4_cat =
			_ranks_5_cat =
			_ranks_6_cat =
			_ranks_7_cat =
			_ranks_8_cat = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private void InitRanks()
		{
			ResetTallies();

			var routes = _file.Routes;
			foreach (RouteNode node in routes)
			{
				if (node.Spawn != SpawnWeight.None)
				{
					++_nodes;

					switch (node.Rank)
					{
						case 0: ++_ranks_0; break;
						case 1: ++_ranks_1; break;
						case 2: ++_ranks_2; break;
						case 3: ++_ranks_3; break;
						case 4: ++_ranks_4; break;
						case 5: ++_ranks_5; break;
						case 6: ++_ranks_6; break;
						case 7: ++_ranks_7; break;
						case 8: ++_ranks_8; break;
					}
				}
			}

			lbl_tsRanks0_out.Text = _ranks_0.ToString();
			lbl_tsRanks1_out.Text = _ranks_1.ToString();
			lbl_tsRanks2_out.Text = _ranks_2.ToString();
			lbl_tsRanks3_out.Text = _ranks_3.ToString();
			lbl_tsRanks4_out.Text = _ranks_4.ToString();
			lbl_tsRanks5_out.Text = _ranks_5.ToString();
			lbl_tsRanks6_out.Text = _ranks_6.ToString();
			lbl_tsRanks7_out.Text = _ranks_7.ToString();
			lbl_tsRanks8_out.Text = _ranks_8.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitRanksCategory()
		{
			KeyValuePair<string, Dictionary<string, Descriptor>> cat = getCategory();
			if (!cat.Equals(new KeyValuePair<string, Dictionary<string, Descriptor>>()))
			{
				RouteNodeCollection routes;
				foreach (var descriptor in cat.Value)
				{
					Descriptor tileset = descriptor.Value;

					if (tileset == _file.Descriptor)
						routes = _file.Routes; // -> not only convenient, is req'd when importing Routes
					else
						routes = new RouteNodeCollection(tileset.Label, tileset.Basepath);

					foreach (RouteNode node in routes)
					{
						if (node.Spawn != SpawnWeight.None)
						{
							switch (node.Rank)
							{
								case 0: ++_ranks_0_cat; break;
								case 1: ++_ranks_1_cat; break;
								case 2: ++_ranks_2_cat; break;
								case 3: ++_ranks_3_cat; break;
								case 4: ++_ranks_4_cat; break;
								case 5: ++_ranks_5_cat; break;
								case 6: ++_ranks_6_cat; break;
								case 7: ++_ranks_7_cat; break;
								case 8: ++_ranks_8_cat; break;
							}
						}
					}
				}

				lbl_tsRanks0_outcat.Text = _ranks_0_cat.ToString();
				lbl_tsRanks1_outcat.Text = _ranks_1_cat.ToString();
				lbl_tsRanks2_outcat.Text = _ranks_2_cat.ToString();
				lbl_tsRanks3_outcat.Text = _ranks_3_cat.ToString();
				lbl_tsRanks4_outcat.Text = _ranks_4_cat.ToString();
				lbl_tsRanks5_outcat.Text = _ranks_5_cat.ToString();
				lbl_tsRanks6_outcat.Text = _ranks_6_cat.ToString();
				lbl_tsRanks7_outcat.Text = _ranks_7_cat.ToString();
				lbl_tsRanks8_outcat.Text = _ranks_8_cat.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private KeyValuePair<string, Dictionary<string, Descriptor>> getCategory()
		{
			Dictionary<string, Dictionary<string, Descriptor>> categories;
			Dictionary<string, Descriptor> descriptors;

			Dictionary<string, TileGroupBase> tileGroups = ResourceInfo.TileGroupManager.TileGroups;
			foreach (var @group in tileGroups)
			{
				categories = @group.Value.Categories;
				foreach (var category in categories)
				{
					descriptors = category.Value;
					foreach (var descriptor in descriptors)
					{
						if (descriptor.Value == _file.Descriptor)
						{
							_group = @group.Key;
							_category = category.Key;

							return category;
						}
					}
				}
			}
			return new KeyValuePair<string, Dictionary<string, Descriptor>>();
		}

		/// <summary>
		/// 
		/// </summary>
		private void thisLayout()
		{
			int widthtileset = lbl_Tileset.Left + lbl_Tileset.Width;
			lbl_Label        .Left =
			lbl_NodesQuantity.Left = widthtileset;

			lbl_Label        .Width =
			lbl_NodesQuantity.Width = TextRenderer.MeasureText(lbl_Label.Text, Font).Width + 5;

			int widthLeft = widthtileset + lbl_Label.Width;


			int widthRighttop = TextRenderer.MeasureText(lbl_Category.Text, Font).Width + 5;
			int widthRightbot = TextRenderer.MeasureText(lbl_Group.Text,    Font).Width + 5;
			int widthRight;
			if (widthRighttop > widthRightbot) widthRight = widthRighttop;
			else                               widthRight = widthRightbot;

			lbl_Category.Width =
			lbl_Group   .Width = widthRight;


			int widthTop = widthLeft + widthRight;
			int widthBot = lbl_tsCategoryTotals.Left + lbl_tsCategoryTotals.Width + 5;

			int width;
			if (widthTop > widthBot)
			{
				width = widthTop;
				lbl_Category.Left =
				lbl_Group   .Left = widthLeft - 5; // WARNING: Could overlap w/ 'lbl_Label'.
			}
			else
			{
				width = widthBot;
				lbl_Category.Left =
				lbl_Group   .Left = width - widthRight - 5; // WARNING: Could overlap w/ 'lbl_Label'.
			}

			ClientSize = new Size(width, gb_Info.Height + gb_Tileset.Height);

			int border   = Width  - ClientSize.Width;
			int titlebar = Height - ClientSize.Height - border;
			MinimumSize = new Size(
								ClientSize.Width  + border,
								ClientSize.Height + border + titlebar);
		}


		/// <summary>
		/// Tallies a node on its creation in RouteView.
		/// </summary>
		/// <param name="node"></param>
		internal void AddNode(RouteNode node)
		{
			if (node.Spawn != SpawnWeight.None)
			{
				lbl_NodesQuantity.Text = (++_nodes).ToString();
				UpdateNoderank(Byte.MaxValue, node.Rank);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weightPre"></param>
		/// <param name="weightPos"></param>
		/// <param name="rank"></param>
		internal void ChangedSpawnweight(SpawnWeight weightPre, SpawnWeight weightPos, byte rank)
		{
			if (weightPre == SpawnWeight.None)
			{
				if (weightPos != SpawnWeight.None)
				{
					lbl_NodesQuantity.Text = (++_nodes).ToString();
					UpdateNoderank(Byte.MaxValue, rank);
				}
			}
			else if (weightPos == SpawnWeight.None)
			{
				lbl_NodesQuantity.Text = (--_nodes).ToString();
				UpdateNoderank(rank, Byte.MaxValue);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rankPre"></param>
		/// <param name="rankPos"></param>
		internal void UpdateNoderank(byte rankPre, byte rankPos)
		{
			switch (rankPre)
			{
				case 0:
					lbl_tsRanks0_out   .Text = (--_ranks_0)    .ToString();
					lbl_tsRanks0_outcat.Text = (--_ranks_0_cat).ToString();
					break;
				case 1:
					lbl_tsRanks1_out   .Text = (--_ranks_1)    .ToString();
					lbl_tsRanks1_outcat.Text = (--_ranks_1_cat).ToString();
					break;
				case 2:
					lbl_tsRanks2_out   .Text = (--_ranks_2)    .ToString();
					lbl_tsRanks2_outcat.Text = (--_ranks_2_cat).ToString();
					break;
				case 3:
					lbl_tsRanks3_out   .Text = (--_ranks_3)    .ToString();
					lbl_tsRanks3_outcat.Text = (--_ranks_3_cat).ToString();
					break;
				case 4:
					lbl_tsRanks4_out   .Text = (--_ranks_4)    .ToString();
					lbl_tsRanks4_outcat.Text = (--_ranks_4_cat).ToString();
					break;
				case 5:
					lbl_tsRanks5_out   .Text = (--_ranks_5)    .ToString();
					lbl_tsRanks5_outcat.Text = (--_ranks_5_cat).ToString();
					break;
				case 6:
					lbl_tsRanks6_out   .Text = (--_ranks_6)    .ToString();
					lbl_tsRanks6_outcat.Text = (--_ranks_6_cat).ToString();
					break;
				case 7:
					lbl_tsRanks7_out   .Text = (--_ranks_7)    .ToString();
					lbl_tsRanks7_outcat.Text = (--_ranks_7_cat).ToString();
					break;
				case 8:
					lbl_tsRanks8_out   .Text = (--_ranks_8)    .ToString();
					lbl_tsRanks8_outcat.Text = (--_ranks_8_cat).ToString();
					break;
			}

			switch (rankPos)
			{
				case 0:
					lbl_tsRanks0_out   .Text = (++_ranks_0)    .ToString();
					lbl_tsRanks0_outcat.Text = (++_ranks_0_cat).ToString();
					break;
				case 1:
					lbl_tsRanks1_out   .Text = (++_ranks_1)    .ToString();
					lbl_tsRanks1_outcat.Text = (++_ranks_1_cat).ToString();
					break;
				case 2:
					lbl_tsRanks2_out   .Text = (++_ranks_2)    .ToString();
					lbl_tsRanks2_outcat.Text = (++_ranks_2_cat).ToString();
					break;
				case 3:
					lbl_tsRanks3_out   .Text = (++_ranks_3)    .ToString();
					lbl_tsRanks3_outcat.Text = (++_ranks_3_cat).ToString();
					break;
				case 4:
					lbl_tsRanks4_out   .Text = (++_ranks_4)    .ToString();
					lbl_tsRanks4_outcat.Text = (++_ranks_4_cat).ToString();
					break;
				case 5:
					lbl_tsRanks5_out   .Text = (++_ranks_5)    .ToString();
					lbl_tsRanks5_outcat.Text = (++_ranks_5_cat).ToString();
					break;
				case 6:
					lbl_tsRanks6_out   .Text = (++_ranks_6)    .ToString();
					lbl_tsRanks6_outcat.Text = (++_ranks_6_cat).ToString();
					break;
				case 7:
					lbl_tsRanks7_out   .Text = (++_ranks_7)    .ToString();
					lbl_tsRanks7_outcat.Text = (++_ranks_7_cat).ToString();
					break;
				case 8:
					lbl_tsRanks8_out   .Text = (++_ranks_8)    .ToString();
					lbl_tsRanks8_outcat.Text = (++_ranks_8_cat).ToString();
					break;
			}
		}
		#endregion Methods
	}
}
/*
		public UnitType Type
		{ get; set; }

		public byte Rank
		{ get; set; }

		public PatrolPriority Patrol
		{ get; set; }

		public BaseAttack Attack
		{ get; set; }

		public SpawnWeight Spawn
		{ get; set; }
*/
/*
		public static readonly object[] NodeRankUfo =
		{
			new Pterodactyl("0 : Civ/Scout",        XCom.NodeRankUfo.CivScout),
			new Pterodactyl("1 : XCOM",             XCom.NodeRankUfo.XCOM),
			new Pterodactyl("2 : Soldier",          XCom.NodeRankUfo.Soldier),
			new Pterodactyl("3 : Navigator",        XCom.NodeRankUfo.Navigator),
			new Pterodactyl("4 : Leader/Commander", XCom.NodeRankUfo.LeaderCommander),
			new Pterodactyl("5 : Engineer",         XCom.NodeRankUfo.Engineer),
			new Pterodactyl("6 : Terrorist1",       XCom.NodeRankUfo.Misc1),
			new Pterodactyl("7 : Medic",            XCom.NodeRankUfo.Medic),
			new Pterodactyl("8 : Terrorist2",       XCom.NodeRankUfo.Misc2),
			new Pterodactyl(RankInvalid,            XCom.NodeRankUfo.invalid) // WORKAROUND.
		};

		public static readonly object[] NodeRankTftd =
		{
			new Pterodactyl("0 : Civ/Scout",        XCom.NodeRankTftd.CivScout),
			new Pterodactyl("1 : XCOM",             XCom.NodeRankTftd.XCOM),
			new Pterodactyl("2 : Soldier",          XCom.NodeRankTftd.Soldier),
			new Pterodactyl("3 : Squad Leader",     XCom.NodeRankTftd.SquadLeader),
			new Pterodactyl("4 : Leader/Commander", XCom.NodeRankTftd.LeaderCommander),
			new Pterodactyl("5 : Medic",            XCom.NodeRankTftd.Medic),
			new Pterodactyl("6 : Terrorist1",       XCom.NodeRankTftd.Misc1),
			new Pterodactyl("7 : Technician",       XCom.NodeRankTftd.Technician),
			new Pterodactyl("8 : Terrorist2",       XCom.NodeRankTftd.Misc2),
			new Pterodactyl(RankInvalid,            XCom.NodeRankTftd.invalid) // WORKAROUND.
		};

		public static readonly object[] SpawnWeight =
		{
			new Pterodactyl( "0 : None", XCom.SpawnWeight.None),
			new Pterodactyl( "1 : Lo",   XCom.SpawnWeight.Spawn1),
			new Pterodactyl( "2 : Lo",   XCom.SpawnWeight.Spawn2),
			new Pterodactyl( "3 : Lo",   XCom.SpawnWeight.Spawn3),
			new Pterodactyl( "4 : Med",  XCom.SpawnWeight.Spawn4),
			new Pterodactyl( "5 : Med",  XCom.SpawnWeight.Spawn5),
			new Pterodactyl( "6 : Med",  XCom.SpawnWeight.Spawn6),
			new Pterodactyl( "7 : Med",  XCom.SpawnWeight.Spawn7),
			new Pterodactyl( "8 : Hi",   XCom.SpawnWeight.Spawn8),
			new Pterodactyl( "9 : Hi",   XCom.SpawnWeight.Spawn9),
			new Pterodactyl("10 : Hi",   XCom.SpawnWeight.Spawn10)
		};
*/
