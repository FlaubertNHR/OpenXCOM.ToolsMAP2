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
	internal sealed partial class SpawnInfo
		:
			Form
	{
		#region Fields (static)
		static int _x = Int32.MinValue, _y;

		static int _rankwidthUfo  = Int32.MinValue;
		static int _rankwidthTftd = Int32.MinValue;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;

		private int _nodes, _nodescat;
		private int
			_ranks_0, _ranks_1, _ranks_2, _ranks_3,
			_ranks_4, _ranks_5, _ranks_6, _ranks_7,
			_ranks_8,

			_ranks_0_cat, _ranks_1_cat, _ranks_2_cat, _ranks_3_cat,
			_ranks_4_cat, _ranks_5_cat, _ranks_6_cat, _ranks_7_cat,
			_ranks_8_cat;

		int _rankwidth;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Instantiates this <c>SpawnInfo</c> dialog.
		/// </summary>
		/// <param name="file"></param>
		internal SpawnInfo(MapFile file)
		{
			InitializeComponent();

			if (_x != Int32.MinValue)
			{
				StartPosition = FormStartPosition.Manual;
				Left = _x;
				Top  = _y;
			}

			Initialize(file);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Nulls
		/// <c><see cref="RouteView.SpawnInfo">RouteView.SpawnInfo</see></c>
		/// after this dialog closes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				RouteView.SpawnInfo = null;

				_x = Math.Max(0, Left);
				_y = Math.Max(0, Top);
			}

			base.OnFormClosed(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Closes this <c>SpawnInfo</c> dialog on <c>[Esc]</c>.
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
		/// Initializes this <c>SpawnInfo</c> with a specified
		/// <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="file"></param>
		internal void Initialize(MapFile file)
		{
			_file = file;

			SuspendLayout();

			object[] pters;
			if (_file.Descriptor.GroupType == GroupType.Tftd)
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

			if (_file.Descriptor.GroupType == GroupType.Tftd)
			{
				if (_rankwidthTftd == Int32.MinValue)
					LongestRanktext(GroupType.Tftd);

				_rankwidth = _rankwidthTftd;
			}
			else
			{
				if (_rankwidthUfo == Int32.MinValue)
					LongestRanktext(GroupType.Ufo);

				_rankwidth = _rankwidthUfo;
			}

			ResetTallies();

			InitTilesetRanks();
			InitCategoryRanks();

			lbl_Tileset.Text = _file.Descriptor.Label;

			LayoutRoutesInfo();
		}

		/// <summary>
		/// Caches the longest ranktext width for either Ufo-ranks or
		/// Tftd-ranks.
		/// </summary>
		/// <param name="group"></param>
		private void LongestRanktext(GroupType @group)
		{
			int width = TextRenderer.MeasureText(lbl_tsRanks0.Text, Font).Width;

			int widthtest = TextRenderer.MeasureText(lbl_tsRanks1.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks2.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks3.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks4.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks5.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks6.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks7.Text, Font).Width;
			if (widthtest > width) width = widthtest;
			widthtest = TextRenderer.MeasureText(lbl_tsRanks8.Text, Font).Width;
			if (widthtest > width) width = widthtest;

			if (@group == GroupType.Tftd) _rankwidthTftd = width;
			else                          _rankwidthUfo  = width;
		}

		/// <summary>
		/// Resets all tallies.
		/// </summary>
		private void ResetTallies()
		{
			_ranks_0 = _ranks_1 = _ranks_2 = _ranks_3 =
			_ranks_4 = _ranks_5 = _ranks_6 = _ranks_7 =
			_ranks_8 =
			_ranks_0_cat = _ranks_1_cat = _ranks_2_cat = _ranks_3_cat =
			_ranks_4_cat = _ranks_5_cat = _ranks_6_cat = _ranks_7_cat =
			_ranks_8_cat =

			_nodes    =
			_nodescat = 0;
		}

		/// <summary>
		/// Initializes the <c><see cref="NodeRankUfo">NodeRank*s</see></c> for
		/// the Tileset.
		/// </summary>
		private void InitTilesetRanks()
		{
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
		/// Initializes the <c><see cref="NodeRankUfo">NodeRank*s</see></c> for
		/// the Category in
		/// <c><see cref="TileGroup">TileGroup</see>.Categories</c>.
		/// </summary>
		private void InitCategoryRanks()
		{
			KeyValuePair<string, Dictionary<string, Descriptor>> category = GetCategory();
			if (!category.Equals(new KeyValuePair<string, Dictionary<string, Descriptor>>()))
			{
				FileService.isSpawnInfo = true;

				Descriptor tileset; RouteNodes routes;

				foreach (var descriptor in category.Value)
				{
					if ((tileset = descriptor.Value) == _file.Descriptor)
					{
						routes = _file.Routes; // -> not only efficient, is req'd when importing Routes
					}
					else if ((routes = new RouteNodes(tileset.Label, tileset.Basepath)).Fail)
					{
//						routes.Fail = false; -> nobody cares, Marvin.
						routes.Nodes.Clear();

						if (!FileService.isSpawnInfo)
						{
							lbl_tsRanks0_outcat.Text =
							lbl_tsRanks1_outcat.Text =
							lbl_tsRanks2_outcat.Text =
							lbl_tsRanks3_outcat.Text =
							lbl_tsRanks4_outcat.Text =
							lbl_tsRanks5_outcat.Text =
							lbl_tsRanks6_outcat.Text =
							lbl_tsRanks7_outcat.Text =
							lbl_tsRanks8_outcat.Text =

							lbl_TotalCategory  .Text = "-";
							return;
						}
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

				FileService.isSpawnInfo = false;

				lbl_tsRanks0_outcat.Text = _ranks_0_cat.ToString();
				lbl_tsRanks1_outcat.Text = _ranks_1_cat.ToString();
				lbl_tsRanks2_outcat.Text = _ranks_2_cat.ToString();
				lbl_tsRanks3_outcat.Text = _ranks_3_cat.ToString();
				lbl_tsRanks4_outcat.Text = _ranks_4_cat.ToString();
				lbl_tsRanks5_outcat.Text = _ranks_5_cat.ToString();
				lbl_tsRanks6_outcat.Text = _ranks_6_cat.ToString();
				lbl_tsRanks7_outcat.Text = _ranks_7_cat.ToString();
				lbl_tsRanks8_outcat.Text = _ranks_8_cat.ToString();

				lbl_TotalCategory  .Text = _nodescat.ToString();
			}
		}

		/// <summary>
		/// Gets the current <c><see cref="_file">_file's</see></c> Category in
		/// <c><see cref="TileGroup"></see>.Categories</c> and sets the current
		/// Group and Category strings.
		/// </summary>
		/// <returns></returns>
		private KeyValuePair<string, Dictionary<string, Descriptor>> GetCategory()
		{
			Dictionary<string, Dictionary<string, Descriptor>> categories;
			Dictionary<string, Descriptor> descriptors;

			Dictionary<string, TileGroup> groups = TileGroupManager.TileGroups;
			foreach (var @group in groups)
			{
				categories = @group.Value.Categories;
				foreach (var category in categories)
				{
					descriptors = category.Value;
					foreach (var descriptor in descriptors)
					{
						if (descriptor.Value == _file.Descriptor)
						{
							lbl_Group   .Text = @group.Key;
							lbl_Category.Text = category.Key;

							return category;
						}
					}
				}
			}
			return new KeyValuePair<string, Dictionary<string, Descriptor>>();
		}


		/// <summary>
		/// Right padding for texts.
		/// </summary>
		const int PAD = 3;

		/// <summary>
		/// Lays out this <c>SpawnInfo</c> dialog.
		/// </summary>
		private void LayoutRoutesInfo()
		{
			MinimumSize = new Size(0,0); // jic

			lbl_tsTilesetTotals.Anchor =
			lbl_tsRanks0_out.Anchor = lbl_tsRanks1_out.Anchor = lbl_tsRanks2_out.Anchor = lbl_tsRanks3_out.Anchor = 
			lbl_tsRanks4_out.Anchor = lbl_tsRanks5_out.Anchor = lbl_tsRanks6_out.Anchor = lbl_tsRanks7_out.Anchor =
			lbl_tsRanks8_out.Anchor = lbl_TotalTileset.Anchor =

			lbl_tsCategoryTotals.Anchor =
			lbl_tsRanks0_outcat.Anchor = lbl_tsRanks1_outcat.Anchor = lbl_tsRanks2_outcat.Anchor = lbl_tsRanks3_outcat.Anchor = 
			lbl_tsRanks4_outcat.Anchor = lbl_tsRanks5_outcat.Anchor = lbl_tsRanks6_outcat.Anchor = lbl_tsRanks7_outcat.Anchor =
			lbl_tsRanks8_outcat.Anchor = lbl_TotalCategory.Anchor = AnchorStyles.Top | AnchorStyles.Left;


			// gb_TreeInfo ->
			int widthLeft = lbl_Tileset_.Left + lbl_Tileset_.Width;
			lbl_Tileset.Left  = widthLeft;
			lbl_Tileset.Width = TextRenderer.MeasureText(lbl_Tileset.Text, Font).Width + PAD;

			widthLeft += lbl_Tileset.Width;


			int widthRighttop = TextRenderer.MeasureText(lbl_Category.Text, Font).Width + PAD;
			int widthRightbot = TextRenderer.MeasureText(lbl_Group   .Text, Font).Width + PAD;

			int widthRight;
			if (widthRighttop > widthRightbot) widthRight = widthRighttop;
			else                               widthRight = widthRightbot;

			lbl_Category.Width =
			lbl_Group   .Width = widthRight;

			int widthTop = widthLeft + widthRight;


			// gb_CountInfo ->
			lbl_tsRanks0.Width = lbl_tsRanks1.Width = lbl_tsRanks2.Width = lbl_tsRanks3.Width =
			lbl_tsRanks4.Width = lbl_tsRanks5.Width = lbl_tsRanks6.Width = lbl_tsRanks7.Width =
			lbl_tsRanks8.Width = _rankwidth + PAD;

			widthLeft = lbl_tsRanks0.Left + lbl_tsRanks0.Width + PAD;

			lbl_tsTilesetTotals.Left =
			lbl_tsRanks0_out.Left = lbl_tsRanks1_out.Left = lbl_tsRanks2_out.Left = lbl_tsRanks3_out.Left = 
			lbl_tsRanks4_out.Left = lbl_tsRanks5_out.Left = lbl_tsRanks6_out.Left = lbl_tsRanks7_out.Left =
			lbl_tsRanks8_out.Left = lbl_TotalTileset.Left = widthLeft;

			lbl_tsCategoryTotals.Left =
			lbl_tsRanks0_outcat.Left = lbl_tsRanks1_outcat.Left = lbl_tsRanks2_outcat.Left = lbl_tsRanks3_outcat.Left = 
			lbl_tsRanks4_outcat.Left = lbl_tsRanks5_outcat.Left = lbl_tsRanks6_outcat.Left = lbl_tsRanks7_outcat.Left =
			lbl_tsRanks8_outcat.Left = lbl_TotalCategory.Left = widthLeft + lbl_tsTilesetTotals.Width;

			int widthBot = lbl_tsCategoryTotals.Left + lbl_tsCategoryTotals.Width + 2;


			// total width ->
			int width;
			if (widthTop > widthBot) width = widthTop;
			else                     width = widthBot;

			ClientSize = new Size(width, ClientSize.Height);

			lbl_Category.Left =
			lbl_Group   .Left = gb_TreeInfo.Width - widthRight - PAD;

			MinimumSize = new Size(Width, Height);


			ResumeLayout();

			// after ResumeLayout() ->
			lbl_tsTilesetTotals.Anchor =
			lbl_tsRanks0_out.Anchor = lbl_tsRanks1_out.Anchor = lbl_tsRanks2_out.Anchor = lbl_tsRanks3_out.Anchor = 
			lbl_tsRanks4_out.Anchor = lbl_tsRanks5_out.Anchor = lbl_tsRanks6_out.Anchor = lbl_tsRanks7_out.Anchor =
			lbl_tsRanks8_out.Anchor = lbl_TotalTileset.Anchor =

			lbl_tsCategoryTotals.Anchor =
			lbl_tsRanks0_outcat.Anchor = lbl_tsRanks1_outcat.Anchor = lbl_tsRanks2_outcat.Anchor = lbl_tsRanks3_outcat.Anchor = 
			lbl_tsRanks4_outcat.Anchor = lbl_tsRanks5_outcat.Anchor = lbl_tsRanks6_outcat.Anchor = lbl_tsRanks7_outcat.Anchor =
			lbl_tsRanks8_outcat.Anchor = lbl_TotalCategory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		}


		/// <summary>
		/// de-Tallies a <c><see cref="RouteNode"/></c> on its deletion in
		/// RouteView.
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
		/// <param name="pre"></param>
		/// <param name="pos"></param>
		/// <param name="rank"></param>
		internal void ChangedSpawnweight(SpawnWeight pre, SpawnWeight pos, byte rank)
		{
			if (pre == SpawnWeight.None)
			{
				if (pos != SpawnWeight.None)
				{
					lbl_TotalTileset .Text = (++_nodes)   .ToString();
					lbl_TotalCategory.Text = (++_nodescat).ToString();

					UpdateNoderank(Byte.MaxValue, rank);
				}
			}
			else if (pos == SpawnWeight.None)
			{
				lbl_TotalTileset .Text = (--_nodes)   .ToString();
				lbl_TotalCategory.Text = (--_nodescat).ToString();

				UpdateNoderank(rank, Byte.MaxValue);
			}
		}

		/// <summary>
		/// Updates texts when <c><see cref="NodeRankUfo">NodeRank*</see></c>
		/// gets changed.
		/// </summary>
		/// <param name="pre">previous rank</param>
		/// <param name="pos">current rank</param>
		internal void UpdateNoderank(byte pre, byte pos)
		{
			switch (pre)
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

			switch (pos)
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
