using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.Observers;

using XCom;


namespace MapView
{
	/// <summary>
	/// A dialog that displays the total quantities of spawn-nodes in the
	/// currently loaded tileset as well as that tileset's Category. Quantities
	/// are shown in a table by spawn-rank.
	/// </summary>
	internal sealed partial class RoutesInfo
		:
			Form
	{
		#region Fields
		private MapFile _file;

		private string _group    = String.Empty;
		private string _category = String.Empty;

		private int _nodes, _nodescat;
		private int
			_ranks_0,     _ranks_1,     _ranks_2,     _ranks_3,
			_ranks_4,     _ranks_5,     _ranks_6,     _ranks_7,     _ranks_8,
			_ranks_0_cat, _ranks_1_cat, _ranks_2_cat, _ranks_3_cat,
			_ranks_4_cat, _ranks_5_cat, _ranks_6_cat, _ranks_7_cat, _ranks_8_cat;

		private readonly int _clientheight;
		private int _clientwidth = -1;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Instantiates a RoutesInfo screen.
		/// </summary>
		/// <param name="file"></param>
		internal RoutesInfo(MapFile file)
		{
			InitializeComponent();
			_clientheight = gb_Info.Height + gb_Tileset.Height;

			Initialize(file);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Nulls <c><see cref="RouteView.RoutesInfo">RouteView.RoutesInfo</see></c>
		/// after this dialog closes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
				RouteView.RoutesInfo = null;

			base.OnFormClosed(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Closes this form on [Esc].
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
				Close();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Initializes this RoutesInfo with a MapFile.
		/// </summary>
		/// <param name="file"></param>
		internal void Initialize(MapFile file)
		{
			_file = file;

			object[] pters;
			if (_file.Descriptor.GroupType == GameType.Tftd)
				pters = RouteNodes.RankTftd;
			else
				pters = RouteNodes.RankUfo;

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

			lbl_Tileset .Text = _file.Descriptor.Label;
			lbl_Group   .Text = _group;
			lbl_Category.Text = _category;

			thisLayout();
		}

		/// <summary>
		/// Resets all tallies.
		/// </summary>
		private void ResetTallies()
		{
			_ranks_0     = _ranks_1     = _ranks_2     = _ranks_3     =
			_ranks_4     = _ranks_5     = _ranks_6     = _ranks_7     = _ranks_8     =
			_ranks_0_cat = _ranks_1_cat = _ranks_2_cat = _ranks_3_cat =
			_ranks_4_cat = _ranks_5_cat = _ranks_6_cat = _ranks_7_cat = _ranks_8_cat =

			_nodes    =
			_nodescat = 0;
		}

		/// <summary>
		/// Initializes the ranks for the Tileset.
		/// </summary>
		private void InitRanks()
		{
			ResetTallies();

			foreach (RouteNode node in _file.Routes)
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

			lbl_TotalTileset.Text = _nodes.ToString();
		}

		/// <summary>
		/// Initializes the ranks for the Category.
		/// </summary>
		private void InitRanksCategory()
		{
			KeyValuePair<string, Dictionary<string, Descriptor>> cat = getCategory();
			if (!cat.Equals(new KeyValuePair<string, Dictionary<string, Descriptor>>()))
			{
				RouteNodes routes;
				foreach (var descriptor in cat.Value)
				{
					Descriptor tileset = descriptor.Value;

					if (tileset == _file.Descriptor)
					{
						routes = _file.Routes; // -> not only efficient, is req'd when importing Routes
					}
					else if ((routes = new RouteNodes(tileset.Label, tileset.Basepath)).Fail)
					{
//						routes.Fail = false; -> nobody cares, Marvin.
						routes.Nodes.Clear();
					}

					foreach (RouteNode node in routes)
					{
						if (node.Spawn != SpawnWeight.None)
						{
							++_nodescat;

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

				lbl_TotalCategory.Text = _nodescat.ToString();
			}
		}

		/// <summary>
		/// Gets the current tileset's Category and sets the group and category
		/// strings.
		/// </summary>
		/// <returns></returns>
		private KeyValuePair<string, Dictionary<string, Descriptor>> getCategory()
		{
			Dictionary<string, Dictionary<string, Descriptor>> categories;
			Dictionary<string, Descriptor> descriptors;

			Dictionary<string, TileGroup> tileGroups = TileGroupManager.TileGroups;
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
		/// Lays out this dialog.
		/// </summary>
		private void thisLayout()
		{
			int widthLeft = lbl_Tileset_.Left + lbl_Tileset_.Width;
			lbl_Tileset.Left  = widthLeft;
			lbl_Tileset.Width = TextRenderer.MeasureText(lbl_Tileset.Text, Font).Width + 5;

			widthLeft += lbl_Tileset.Width;


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
			if (widthTop > widthBot) width = widthTop;
			else                     width = widthBot;

			if (_clientwidth == -1)
				_clientwidth = width + 10;
			else
				_clientwidth = Math.Max(width + 10, ClientSize.Width);

			ClientSize = new Size(
								_clientwidth,
								Math.Max(_clientheight, ClientSize.Height));

			lbl_Category.Left =
			lbl_Group   .Left = gb_Info.Width - widthRight - 5;


			int border   = Width  - ClientSize.Width;
			int titlebar = Height - ClientSize.Height - border;
			MinimumSize = new Size(
								 width        + border + 10,
								_clientheight + border + titlebar);
		}


		/// <summary>
		/// de-Tallies a node on its deletion in RouteView.
		/// </summary>
		/// <param name="node"></param>
		internal void DeleteNode(RouteNode node)
		{
			if (node.Spawn != SpawnWeight.None)
			{
				lbl_TotalTileset .Text = (--_nodes)   .ToString();
				lbl_TotalCategory.Text = (--_nodescat).ToString();

				UpdateNoderank(node.Rank, Byte.MaxValue);
			}
		}

		/// <summary>
		/// Increments or decrements total values as applicable.
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
					lbl_TotalTileset .Text = (++_nodes)   .ToString();
					lbl_TotalCategory.Text = (++_nodescat).ToString();

					UpdateNoderank(Byte.MaxValue, rank);
				}
			}
			else if (weightPos == SpawnWeight.None)
			{
				lbl_TotalTileset .Text = (--_nodes)   .ToString();
				lbl_TotalCategory.Text = (--_nodescat).ToString();

				UpdateNoderank(rank, Byte.MaxValue);
			}
		}

		/// <summary>
		/// Updates texts when noderank gets changed.
		/// </summary>
		/// <param name="rankPre"></param>
		/// <param name="rankPos"></param>
		/// <remarks>Check that node is SpawnWeight.None before call.</remarks>
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
