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
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Instantiates a RoutesInfo screen.
		/// </summary>
		/// <param name="file"></param>
		internal RoutesInfo(MapFile file)
		{
			InitializeComponent();

			_file = file;

			Initialize();
		}
		#endregion cTor


		#region Events (override)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			RouteView.RoutesInfo = null;
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
		private void Initialize()
		{
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

			int nodes = InitRanks();
			InitRanksCategory();

			lbl_Label        .Text = _file.Descriptor.Label;
			lbl_NodesQuantity.Text = nodes.ToString();
			lbl_Group        .Text = _group;
			lbl_Category     .Text = _category;

			thisLayout();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private int InitRanks()
		{
			int nodes = 0;

			int ranks_0 = 0;
			int ranks_1 = 0;
			int ranks_2 = 0;
			int ranks_3 = 0;
			int ranks_4 = 0;
			int ranks_5 = 0;
			int ranks_6 = 0;
			int ranks_7 = 0;
			int ranks_8 = 0;

			var routes = _file.Routes;
			foreach (RouteNode node in routes)
			{
				if (node.Spawn != SpawnWeight.None)
				{
					++nodes;

					switch (node.Rank)
					{
						case 0: ++ranks_0; break;
						case 1: ++ranks_1; break;
						case 2: ++ranks_2; break;
						case 3: ++ranks_3; break;
						case 4: ++ranks_4; break;
						case 5: ++ranks_5; break;
						case 6: ++ranks_6; break;
						case 7: ++ranks_7; break;
						case 8: ++ranks_8; break;
					}
				}
			}

			lbl_tsRanks0_out.Text = ranks_0.ToString();
			lbl_tsRanks1_out.Text = ranks_1.ToString();
			lbl_tsRanks2_out.Text = ranks_2.ToString();
			lbl_tsRanks3_out.Text = ranks_3.ToString();
			lbl_tsRanks4_out.Text = ranks_4.ToString();
			lbl_tsRanks5_out.Text = ranks_5.ToString();
			lbl_tsRanks6_out.Text = ranks_6.ToString();
			lbl_tsRanks7_out.Text = ranks_7.ToString();
			lbl_tsRanks8_out.Text = ranks_8.ToString();

			return nodes;
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitRanksCategory()
		{
			KeyValuePair<string, Dictionary<string, Descriptor>> cat = getCategory();
			if (!cat.Equals(new KeyValuePair<string, Dictionary<string, Descriptor>>()))
			{
				int ranks_0 = 0;
				int ranks_1 = 0;
				int ranks_2 = 0;
				int ranks_3 = 0;
				int ranks_4 = 0;
				int ranks_5 = 0;
				int ranks_6 = 0;
				int ranks_7 = 0;
				int ranks_8 = 0;

				foreach (var descriptor in cat.Value)
				{
					Descriptor tileset = descriptor.Value;
					var routes = new RouteNodeCollection(tileset.Label, tileset.Basepath);
					foreach (RouteNode node in routes)
					{
						if (node.Spawn != SpawnWeight.None)
						{
							switch (node.Rank)
							{
								case 0: ++ranks_0; break;
								case 1: ++ranks_1; break;
								case 2: ++ranks_2; break;
								case 3: ++ranks_3; break;
								case 4: ++ranks_4; break;
								case 5: ++ranks_5; break;
								case 6: ++ranks_6; break;
								case 7: ++ranks_7; break;
								case 8: ++ranks_8; break;
							}
						}
					}
				}

				lbl_tsRanks0_outcat.Text = ranks_0.ToString();
				lbl_tsRanks1_outcat.Text = ranks_1.ToString();
				lbl_tsRanks2_outcat.Text = ranks_2.ToString();
				lbl_tsRanks3_outcat.Text = ranks_3.ToString();
				lbl_tsRanks4_outcat.Text = ranks_4.ToString();
				lbl_tsRanks5_outcat.Text = ranks_5.ToString();
				lbl_tsRanks6_outcat.Text = ranks_6.ToString();
				lbl_tsRanks7_outcat.Text = ranks_7.ToString();
				lbl_tsRanks8_outcat.Text = ranks_8.ToString();
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
