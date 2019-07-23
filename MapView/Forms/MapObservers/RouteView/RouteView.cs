using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MainWindow;

using XCom;
using XCom.Interfaces.Base;
using XCom.Resources.Map.RouteData;


namespace MapView.Forms.MapObservers.RouteViews
{
	/// <summary>
	/// Does all the heavy-lifting/node-manipulations in RouteView and
	/// TopRouteView(Route).
	/// </summary>
	internal sealed partial class RouteView
		:
			MapObserverControl // UserControl, IMapObserver
	{
		#region Enumerations
		private enum ConnectNodesType
		{
			None, OneWay, TwoWay
		}
		#endregion Enumerations


		#region Fields (static)
		private static ConnectNodesType _conType = ConnectNodesType.None; // safety - shall be set by LoadControlDefaultOptions()

		private const string NodeCopyPrefix  = "MVNode"; // TODO: use a struct to copy/paste the info.
		private const char NodeCopySeparator = '|';

		private const string Go = "go";

		internal static RouteNode Dragnode;

		internal static byte _curNoderank;
		internal static SpawnWeight _curSpawnweight;
		#endregion Fields (static)


		#region Fields
		private Panel _pnlRoutes; // NOTE: needs to be here for MapObserver vs Designer stuff.

		private readonly List<object> _linksList = new List<object>();

		private int _col; // these are used only to print the clicked location info.
		private int _row;
		private int _lev;

		private bool _loadingInfo;

		/// <summary>
		/// Used by UpdateNodeInformation().
		/// </summary>
		private readonly object[] _linkTypes =
		{
			LinkType.ExitNorth,
			LinkType.ExitEast,
			LinkType.ExitSouth,
			LinkType.ExitWest,
			LinkType.NotUsed
		};
		#endregion Fields


		#region Properties (override)
		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// </summary>
		[Browsable(false)]
		public override MapFileBase MapBase
		{
			set // TODO: check RouteView/TopRouteView(Route)
			{
				base.MapBase = value;
				MapFile      = value as MapFile;

				DeselectNode();

				if ((RoutePanel.MapFile = MapFile) != null)
				{
					cbRank.Items.Clear();

					if (MapFile.Descriptor.Pal == Palette.TftdBattle) // check TFTD else default to UFO
						cbRank.Items.AddRange(RouteNodeCollection.RankTftd);
					else
						cbRank.Items.AddRange(RouteNodeCollection.RankUfo);

					UpdateNodeInformation();
				}
			}
		}
		#endregion Properties (override)


		#region Properties (static)
		/// <summary>
		/// A class-object that holds RouteView's optionable Properties.
		/// @note C# doesn't allow inheritance of multiple class-objects, which
		/// would have been a way to separate the optionable properties from all
		/// the other properties that are not optionable; they need to be
		/// separate or else all Properties would show up in the Options form's
		/// PropertyGrid. An alternative would have been to give all those other
		/// properties the Browsable(false) attribute but I didn't want to
		/// clutter up the code and also because the Browsable(false) attribute
		/// is used to hide Properties from the designer also - but whether or
		/// not they are accessible in the designer is an entirely different
		/// consideration than whether or not they are Optionable Properties. So
		/// I created an independent class just to hold and handle RouteView's
		/// Optionable Properties ... and wired it up. It's a tedious shitfest
		/// but better than the arcane MapViewI system or screwing around with
		/// equally arcane TypeDescriptors. Both of which had been implemented
		/// but then rejected.
		/// </summary>
		internal static RouteViewOptionables Optionables
		{ get; private set; }

		private static RouteNode _nodeSelected;
		internal static RouteNode NodeSelected
		{
			private get { return _nodeSelected; }
			set
			{
				_nodeSelected = RoutePanelParent.NodeSelected = value;

				if (_nodeSelected != null) // for RoutesInfo ->
				{
					_curNoderank    = _nodeSelected.Rank;
					_curSpawnweight = _nodeSelected.Spawn;
				}
			}
		}

		/// <summary>
		/// Stores the node-id from which a "Go" button is clicked. Used to
		/// re-select the original node - which might not be equivalent to
		/// "Back" (if there were a Back button).
		/// </summary>
		private static int OgnodeId
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		internal RoutePanel RoutePanel
		{ get; private set; }

		private MapFile MapFile
		{ get; set; }

		/// <summary>
		/// Coordinates the 'RoutesChanged' flag between RouteView and
		/// TopRouteView(Route).
		/// </summary>
		private bool RouteChanged
		{
			set
			{
				ObserverManager.RouteView   .Control     .RoutesChanged =
				ObserverManager.TopRouteView.ControlRoute.RoutesChanged = value;
			}
		}

		/// <summary>
		/// Sets the 'RoutesChanged' flag. This is only an intermediary that
		/// shows "routes changed" in RouteView; the real 'RoutesChanged' flag
		/// is stored in XCom..MapFileBase. reasons.
		/// </summary>
		internal bool RoutesChanged
		{
			set { label_RoutesChanged.Visible = (MapFile.RoutesChanged = value); }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiates the RouteView viewer and its components/controls.
		/// IMPORTANT: RouteViewForm and TopRouteViewForm will each invoke and
		/// maintain their own instantiations.
		/// </summary>
		public RouteView()
		{
			Optionables = new RouteViewOptionables(this);

			InitializeComponent();

			RoutePanel = new RoutePanel();
			RoutePanel.Dock = DockStyle.Fill;
			RoutePanel.RoutePanelMouseDownEvent += OnRoutePanelMouseDown;
			RoutePanel.RoutePanelMouseUpEvent   += OnRoutePanelMouseUp;
			RoutePanel.MouseMove                += OnRoutePanelMouseMove;
			RoutePanel.MouseLeave               += OnRoutePanelMouseLeave;
			RoutePanel.KeyDown                  += OnRoutePanelKeyDown;
			_pnlRoutes.Controls.Add(RoutePanel);

			// node data ->
			var unitTypes = new object[]
			{
				UnitType.Any,
				UnitType.Small,
				UnitType.Large,
				UnitType.FlyingSmall,
				UnitType.FlyingLarge
			};
			cbType.Items.AddRange(unitTypes);

			cbSpawn .Items.AddRange(RouteNodeCollection.Spawn);
			cbPatrol.Items.AddRange(RouteNodeCollection.Patrol);
			cbAttack.Items.AddRange(RouteNodeCollection.Attack);

			// link data ->
			cbLink1UnitType.Items.AddRange(unitTypes);
			cbLink2UnitType.Items.AddRange(unitTypes);
			cbLink3UnitType.Items.AddRange(unitTypes);
			cbLink4UnitType.Items.AddRange(unitTypes);
			cbLink5UnitType.Items.AddRange(unitTypes);

			// TODO: change the distance textboxes to labels.

			DeselectNode();
		}
		#endregion cTor


		#region Events (override) inherited from IMapObserver/MapObserverControl
		/// <summary>
		/// A flag that prevents the SelectLocation event from firing in both
		/// RouteView and TopRouteView(Route).
		/// </summary>
		private static bool _fired;

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// @note The route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to. This is a design decision that
		/// allows the selected node to stay selected while other tiles get
		/// highlighted.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			if (!_fired) // fire this funct only once (by either Control is sufficient).
			{
				_fired = true;

				ObserverManager.RouteView   .Control     ._col =
				ObserverManager.TopRouteView.ControlRoute._col = args.Location.Col;
				ObserverManager.RouteView   .Control     ._row =
				ObserverManager.TopRouteView.ControlRoute._row = args.Location.Row;
				ObserverManager.RouteView   .Control     ._lev =
				ObserverManager.TopRouteView.ControlRoute._lev = args.Location.Lev;

				ObserverManager.RouteView   .Control     .PrintSelectedInfo();
				ObserverManager.TopRouteView.ControlRoute.PrintSelectedInfo();
			}
			else
				_fired = false;
		}

		/// <summary>
		/// Inherited from IMapObserver through MapObserverControl.
		/// @note The route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to. This is a design decision that
		/// allows the selected node to stay selected while other tiles get
		/// highlighted.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			if (RoutePanel.CursorPosition.X != -1) // find the Control that the mousecursor is in (if either)
			{
				ObserverManager.RouteView   .Control     ._lev =
				ObserverManager.TopRouteView.ControlRoute._lev = args.Level;

				int overId = -1;
				var loc = RoutePanel.GetTileLocation(
												RoutePanel.CursorPosition.X,
												RoutePanel.CursorPosition.Y);
				if (loc.X != -1)
				{
					RouteNode node = ((MapTile)MapBase[loc.Y, loc.X, _lev]).Node;
					if (node != null)
					{
						overId = node.Index;
						if (node.Spawn == SpawnWeight.None)
						{
							lblOver.ForeColor = Optionables.NodeColor;
						}
						else
							lblOver.ForeColor = Optionables.NodeSpawnColor;
					}
					else
						lblOver.ForeColor = SystemColors.ControlText;

					ObserverManager.RouteView   .Control     .PrintOverInfo(overId, loc);
					ObserverManager.TopRouteView.ControlRoute.PrintOverInfo(overId, loc);
				}
			}
			InvalidatePanels();
		}
		#endregion Events (override) inherited from IMapObserver/MapObserverControl


		#region Methods (print TileData)
		/// <summary>
		/// Clears the selected tile-info text when another Map loads.
		/// </summary>
		internal void ClearSelectedInfo()
		{
			lblSelected.Text = String.Empty;
		}

		/// <summary>
		/// Prints the currently selected tile-info to the TileData groupbox.
		/// NOTE: The displayed level is inverted here.
		/// </summary>
		private void PrintSelectedInfo()
		{
			if (MainViewOverlay.that.FirstClick)
			{
				string selected;
				int level;

				if (NodeSelected != null)
				{
					selected = "Selected " + NodeSelected.Index;
					level = NodeSelected.Lev;
				}
				else
				{
					selected = String.Empty;
					level = _lev;
				}

				selected += Environment.NewLine;
				selected += String.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"c {0}  r {1}  L {2}",
										_col + 1, _row + 1, MapFile.MapSize.Levs - level); // 1-based count, level is inverted.

				lblSelected.Text = selected;
			}
		}

		/// <summary>
		/// Prints the currently mouseovered tile-info to the TileData groupbox.
		/// </summary>
		/// <param name="overId"></param>
		/// <param name="loc"></param>
		private void PrintOverInfo(int overId, Point loc)
		{
			string info;

			if (overId != -1)
				info = "Over " + overId;
			else
				info = String.Empty;

			if (loc.X != -1)
			{
				info += Environment.NewLine;
				info += String.Format(
									System.Globalization.CultureInfo.InvariantCulture,
									"c {0}  r {1}  L {2}",
									loc.X + 1, loc.Y + 1, MapFile.MapSize.Levs - _lev); // 1-based count, level is inverted.
			}

			lblOver.Text = info;
		}
		#endregion Methods (print TileData)


		#region Events (mouse-events for RoutePanel)
		private void OnRoutePanelMouseMove(object sender, MouseEventArgs args)
		{
			RoutePanel.CursorPosition = new Point(args.X, args.Y);

			int overId;
			int x = args.X; int y = args.Y;

			var tile = RoutePanel.GetTile(ref x, ref y); // x/y -> tile-location
			if (tile != null && tile.Node != null)
			{
				overId = tile.Node.Index;
				if (tile.Node.Spawn == SpawnWeight.None)
				{
					lblOver.ForeColor = Optionables.NodeColor;
				}
				else
					lblOver.ForeColor = Optionables.NodeSpawnColor;
			}
			else
			{
				overId = -1;
				lblOver.ForeColor = SystemColors.ControlText;
			}

			var loc = new Point(x,y);

			ObserverManager.RouteView   .Control     .PrintOverInfo(overId, loc);
			ObserverManager.TopRouteView.ControlRoute.PrintOverInfo(overId, loc);

			InvalidatePanels();
		}

		/// <summary>
		/// Hides the info-overlay when the mouse leaves this control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRoutePanelMouseLeave(object sender, EventArgs e)
		{
			RoutePanel.CursorPosition = new Point(-1,-1);
			RefreshPanels();
		}

		private void OnRoutePanelMouseUp(object sender, RoutePanelEventArgs args)
		{
			if (Dragnode != null)
			{
				if (((MapTile)args.Tile).Node == null)
				{
					RouteChanged = true;

					((MapTile)MapFile[Dragnode.Row, // clear the node from the previous tile
									  Dragnode.Col,
									  Dragnode.Lev]).Node = null;

					Dragnode.Col = (byte)args.Location.Col; // reassign the node's x/y/z values
					Dragnode.Row = (byte)args.Location.Row; // these get saved w/ Routes.
					Dragnode.Lev =       args.Location.Lev;

					((MapTile)args.Tile).Node = Dragnode; // assign the node to the tile at the mouse-up location.

					var loc = new Point(Dragnode.Col, Dragnode.Row);
					RoutePanelParent.SelectedLocation = loc;
					MainViewOverlay.that.ProcessSelection(loc,loc);

					ObserverManager.RouteView   .Control     .UpdateLinkDistances();
					ObserverManager.TopRouteView.ControlRoute.UpdateLinkDistances();
				}
				else if (args.Location.Col != Dragnode.Col
					||   args.Location.Row != Dragnode.Row
					||   args.Location.Lev != Dragnode.Lev)
				{
					MessageBox.Show(
								this,
								"Cannot move node onto another node.",
								"Err..",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}
				Dragnode = null;
			}
		}

		/// <summary>
		/// Updates distances to and from the currently selected node.
		/// @note NodeSelected must be valid before call.
		/// </summary>
		private void UpdateLinkDistances()
		{
			for (int slot = 0; slot != RouteNode.LinkSlots; ++slot) // update distances to selected node's linked nodes ->
			{
				string distance;

				var link = NodeSelected[slot];
				switch (link.Destination)
				{
					case Link.NotUsed: // NOTE: Should not change; is here to help keep distances consistent.
						link.Distance = 0;
						distance = String.Empty;
						break;

					case Link.ExitWest: // NOTE: Should not change; is here to help keep distances consistent.
					case Link.ExitNorth:
					case Link.ExitEast:
					case Link.ExitSouth:
						link.Distance = 0;
						distance = "0";
						break;

					default:
						link.Distance = CalculateLinkDistance(
															NodeSelected,
															MapFile.Routes[link.Destination]);
						distance = link.Distance.ToString(System.Globalization.CultureInfo.InvariantCulture)
								 + GetDistanceArrow(slot);
						break;
				}
				UpdateLinkText(slot, distance);
			}

			for (var id = 0; id != MapFile.Routes.Count; ++id) // update distances of any links to the selected node ->
			{
				if (id != NodeSelected.Index) // NOTE: a node shall not link to itself.
				{
					var node = MapFile.Routes[id];

					for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
					{
						var link = node[slot];
						if (link.Destination == NodeSelected.Index)
							link.Distance = CalculateLinkDistance(
																node,
																NodeSelected);
					}
				}
			}
		}

		private void UpdateLinkText(int slot, string distance)
		{
			switch (slot)
			{
				case 0: tbLink1Dist.Text = distance; break;
				case 1: tbLink2Dist.Text = distance; break;
				case 2: tbLink3Dist.Text = distance; break;
				case 3: tbLink4Dist.Text = distance; break;
				case 4: tbLink5Dist.Text = distance; break;
			}
		}

		/// <summary>
		/// Selects a node on LMB, creates and/or connects nodes on RMB.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnRoutePanelMouseDown(object sender, RoutePanelEventArgs args)
		{
			bool update = false;

			RouteNode node = ((MapTile)args.Tile).Node;

			if (NodeSelected == null)
			{
				if ((NodeSelected = node) == null
					&& args.MouseButton == MouseButtons.Right)
				{
					RouteChanged = true;
					NodeSelected = MapFile.AddRouteNode(args.Location);

					if (RoutesInfo != null)
						RoutesInfo.AddNode(NodeSelected);
				}
				update = (NodeSelected != null);
			}
			else // if a node is already selected ...
			{
				if (node == null)
				{
					if (args.MouseButton == MouseButtons.Right)
					{
						RouteChanged = true;
						node = MapFile.AddRouteNode(args.Location);
						ConnectNode(node);

						if (RoutesInfo != null)
							RoutesInfo.AddNode(node);
					}
//					RoutePanel.Refresh(); don't work.

					NodeSelected = node;
					update = true;
				}
				else if (node != NodeSelected)
				{
					if (args.MouseButton == MouseButtons.Right)
						ConnectNode(node);

//					RoutePanel.Refresh(); don't work.

					NodeSelected = node;
					update = true;
				}
				// else the selected node is the node clicked.
			}

			if (update) UpdateNodeInfo();

			Dragnode = NodeSelected;

			EnableEditButtons();
		}

		private void EnableEditButtons()
		{
			bool valid = (NodeSelected != null);

			ObserverManager.RouteView   .Control     .tsmiClearLinkData.Enabled =
			ObserverManager.TopRouteView.ControlRoute.tsmiClearLinkData.Enabled =

			ObserverManager.RouteView   .Control     .btnCut           .Enabled =
			ObserverManager.TopRouteView.ControlRoute.btnCut           .Enabled =

			ObserverManager.RouteView   .Control     .btnCopy          .Enabled =
			ObserverManager.TopRouteView.ControlRoute.btnCopy          .Enabled =

			ObserverManager.RouteView   .Control     .btnDelete        .Enabled =
			ObserverManager.TopRouteView.ControlRoute.btnDelete        .Enabled = valid;

			valid = valid
				 && Clipboard.GetText().Split(NodeCopySeparator)[0] == NodeCopyPrefix;

			ObserverManager.RouteView   .Control     .btnPaste         .Enabled =
			ObserverManager.TopRouteView.ControlRoute.btnPaste         .Enabled = valid;
		}

		/// <summary>
		/// Checks connector and connects nodes if applicable.
		/// </summary>
		/// <param name="node">the node to try to link the currently selected
		/// node to</param>
		private void ConnectNode(RouteNode node)
		{
			if (_conType != ConnectNodesType.None)
			{
				int slot = GetOpenLinkSlot(NodeSelected, node.Index);
				if (slot > -1)
				{
					RouteChanged = true;
					NodeSelected[slot].Destination = node.Index;
					NodeSelected[slot].Distance = CalculateLinkDistance(NodeSelected, node);
				}
				else if (slot == -3)
				{
					MessageBox.Show(
								this,
								"Source node could not be linked to the destination node."
									+ " Its link-slots are full.",
								" Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button1,
								0);
					// TODO: the message leaves the RoutePanel drawn in an awkward state
					// but discovering where to call Refresh() is not trivial.
//					RoutePanel.Refresh(); // in case of a warning this needs to happen ...
					// Fortunately a simple mouseover straightens things out for now.
				}

				if (_conType == ConnectNodesType.TwoWay)
				{
					slot = GetOpenLinkSlot(node, NodeSelected.Index);
					if (slot > -1)
					{
						RouteChanged = true;
						node[slot].Destination = NodeSelected.Index;
						node[slot].Distance = CalculateLinkDistance(node, NodeSelected);
					}
					else if (slot == -3)
					{
						MessageBox.Show(
									this,
									"Destination node could not be linked to the source node."
										+ " Its link-slots are full.",
									" Warning",
									MessageBoxButtons.OK,
									MessageBoxIcon.Exclamation,
									MessageBoxDefaultButton.Button1,
									0);
						// TODO: the message leaves the RoutePanel drawn in an awkward state
						// but discovering where to call Refresh() is not trivial.
//						RoutePanel.Refresh(); // in case of a warning this needs to happen ...
						// Fortunately a simple mouseover straightens things out for now.
					}
				}
			}
		}

		/// <summary>
		/// Gets the first available link-slot for a given node.
		/// </summary>
		/// <param name="node">the node to check the link-slots of</param>
		/// <param name="dest">the id of the destination node</param>
		/// <returns>id of an available link-slot, or
		/// -1 if the source-node is null (not sure if this ever happens)
		/// -2 if the link already exists
		/// -3 if there are no free slots</returns>
		private static int GetOpenLinkSlot(RouteNode node, int dest)
		{
			if (node != null)
			{
				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot) // first check if destination-id already exists
				{
					if (dest != -1 && node[slot].Destination == dest)
						return -2;
				}

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot) // then check for an open slot
				{
					if (node[slot].Destination == (byte)LinkType.NotUsed)
						return slot;
				}
				return -3;
			}
			return -1;
		}
		#endregion Events (mouse-events for RoutePanel)


		/// <summary>
		/// Updates node-info fields below the panel itself.
		/// </summary>
		internal void UpdateNodeInformation()
		{
			SuspendLayout();

			PrintSelectedInfo();

			_loadingInfo = true;

			if (NodeSelected == null)
			{
				btnCut      .Enabled =
				btnCopy     .Enabled =
				btnPaste    .Enabled =
				btnDelete   .Enabled =

				gbNodeData  .Enabled =
				gbLinkData  .Enabled =
				gbNodeEditor.Enabled =

				btnGoLink1  .Enabled =
				btnGoLink2  .Enabled =
				btnGoLink3  .Enabled =
				btnGoLink4  .Enabled =
				btnGoLink5  .Enabled = false;

				btnGoLink1.Text =
				btnGoLink2.Text =
				btnGoLink3.Text =
				btnGoLink4.Text =
				btnGoLink5.Text = String.Empty;


				cbType.SelectedItem = UnitType.Any;

				if (MapFile.Descriptor.Pal == Palette.TftdBattle)
					cbRank.SelectedItem = RouteNodeCollection.RankTftd[0];	//(byte)NodeRankTftd.CivScout
				else
					cbRank.SelectedItem = RouteNodeCollection.RankUfo [0];	//(byte)NodeRankUfo.CivScout

				cbSpawn .SelectedItem = RouteNodeCollection.Spawn [0];		//(byte)SpawnWeight.None
				cbPatrol.SelectedItem = RouteNodeCollection.Patrol[0];		//(byte)PatrolPriority.Zero
				cbAttack.SelectedItem = RouteNodeCollection.Attack[0];		//(byte)AttackBase.Zero

				cbLink1Dest.SelectedItem = // TODO: figure out why these show blank and not "NotUsed"
				cbLink2Dest.SelectedItem = // when the app loads its very first Map.
				cbLink3Dest.SelectedItem =
				cbLink4Dest.SelectedItem =
				cbLink5Dest.SelectedItem = LinkType.NotUsed;

				cbLink1UnitType.SelectedItem =
				cbLink2UnitType.SelectedItem =
				cbLink3UnitType.SelectedItem =
				cbLink4UnitType.SelectedItem =
				cbLink5UnitType.SelectedItem = UnitType.Any;

				tbLink1Dist.Text =
				tbLink2Dist.Text =
				tbLink3Dist.Text =
				tbLink4Dist.Text =
				tbLink5Dist.Text = String.Empty;

				labelLink1.ForeColor =
				labelLink2.ForeColor =
				labelLink3.ForeColor =
				labelLink4.ForeColor =
				labelLink5.ForeColor = SystemColors.ControlText;
			}
			else // selected node is valid ->
			{
				gbNodeData  .Enabled =
				gbLinkData  .Enabled =
				gbNodeEditor.Enabled = true;

				cbType.SelectedItem = NodeSelected.Type;

				if (MapFile.Descriptor.Pal == Palette.TftdBattle)
					cbRank.SelectedItem = RouteNodeCollection.RankTftd[NodeSelected.Rank];
				else
					cbRank.SelectedItem = RouteNodeCollection.RankUfo [NodeSelected.Rank];

				cbSpawn .SelectedItem = RouteNodeCollection.Spawn [(byte)NodeSelected.Spawn];
				cbPatrol.SelectedItem = RouteNodeCollection.Patrol[(byte)NodeSelected.Patrol];
				cbAttack.SelectedItem = RouteNodeCollection.Attack[(byte)NodeSelected.Attack];

				cbLink1Dest.Items.Clear();
				cbLink2Dest.Items.Clear();
				cbLink3Dest.Items.Clear();
				cbLink4Dest.Items.Clear();
				cbLink5Dest.Items.Clear();

				_linksList.Clear();

				for (byte id = 0; id != MapFile.Routes.Count; ++id)
				{
					if (id != NodeSelected.Index)
						_linksList.Add(id);			// <- add all linkable (ie. other) nodes
				}
				_linksList.AddRange(_linkTypes);	// <- add the four compass-points + link-not-used.

				object[] linkListArray = _linksList.ToArray();

				cbLink1Dest.Items.AddRange(linkListArray);
				cbLink2Dest.Items.AddRange(linkListArray);
				cbLink3Dest.Items.AddRange(linkListArray);
				cbLink4Dest.Items.AddRange(linkListArray);
				cbLink5Dest.Items.AddRange(linkListArray);


				ComboBox cbTypL, cbDest;
				TextBox tbDist;
				Button btnGo;
				Label lblText;

				Link link;
				byte dest;

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
				{
					switch (slot)
					{
						case 0:
							cbTypL  = cbLink1UnitType;
							cbDest  = cbLink1Dest;
							tbDist  = tbLink1Dist;
							btnGo   = btnGoLink1;
							lblText = labelLink1;
							break;

						case 1:
							cbTypL  = cbLink2UnitType;
							cbDest  = cbLink2Dest;
							tbDist  = tbLink2Dist;
							btnGo   = btnGoLink2;
							lblText = labelLink2;
							break;

						case 2:
							cbTypL  = cbLink3UnitType;
							cbDest  = cbLink3Dest;
							tbDist  = tbLink3Dist;
							btnGo   = btnGoLink3;
							lblText = labelLink3;
							break;

						case 3:
							cbTypL  = cbLink4UnitType;
							cbDest  = cbLink4Dest;
							tbDist  = tbLink4Dist;
							btnGo   = btnGoLink4;
							lblText = labelLink4;
							break;

						default: // case 4:
							cbTypL  = cbLink5UnitType;
							cbDest  = cbLink5Dest;
							tbDist  = tbLink5Dist;
							btnGo   = btnGoLink5;
							lblText = labelLink5;
							break;
					}

					link = NodeSelected[slot];

					cbTypL.SelectedItem = link.Type;
					btnGo.Enabled = link.StandardNode();

					dest = link.Destination;
					if (link.Used())
					{
						btnGo.Text = Go;
						tbDist.Text = Convert.ToString(
													link.Distance,
													System.Globalization.CultureInfo.InvariantCulture)
									+ GetDistanceArrow(slot);

						if (link.StandardNode())
						{
							cbDest.SelectedItem = dest;

							if (RouteNodeCollection.IsNodeOutsideMapBounds(
																		MapFile.Routes[dest],
																		MapFile.MapSize.Cols,
																		MapFile.MapSize.Rows,
																		MapFile.MapSize.Levs))
							{
								lblText.ForeColor = Color.MediumVioletRed;
							}
							else
								lblText.ForeColor = SystemColors.ControlText;
						}
						else
						{
							cbDest.SelectedItem = (LinkType)dest;
							lblText.ForeColor = SystemColors.ControlText;
						}
					}
					else
					{
						btnGo .Text =
						tbDist.Text = String.Empty;
						cbDest.SelectedItem = (LinkType)dest;
						lblText.ForeColor = SystemColors.ControlText;
					}
				}
			}

			_loadingInfo = false;

			ResumeLayout();
		}

		/// <summary>
		/// Gets an up/down suffix for the linked distance from the currently
		/// selected node, given the link-slot to the destination node. If the
		/// destination is on the same level as the selected node, a blank
		/// string is returned.
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		private string GetDistanceArrow(int slot)
		{
			var link = NodeSelected[slot];
			if (link.StandardNode())
			{
				var dest = MapFile.Routes[link.Destination];
				if (dest != null) // safety.
				{
					if (NodeSelected.Lev > dest.Lev)
						return " \u2191"; // up arrow
	
					if (NodeSelected.Lev < dest.Lev)
						return " \u2193"; // down arrow
				}
			}
			return String.Empty;
		}


		#region Events (NodeData)
		private void OnUnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RouteChanged = true;
				NodeSelected.Type = (UnitType)cbType.SelectedItem;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.cbType.SelectedIndex = cbType.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.cbType.SelectedIndex = cbType.SelectedIndex;
			}
		}

		private bool _bypassRankChanged;
		private void OnNodeRankSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo && !_bypassRankChanged)
			{
				if (cbRank.SelectedIndex == 9)
				{
					_bypassRankChanged = true;	// because this funct is going to fire again immediately
					cbRank.SelectedIndex = (int)NodeSelected.Rank;
					_bypassRankChanged = false;	// and I don't want the RoutesChanged flagged.
				}
				else
				{
					RouteChanged = true;
					NodeSelected.Rank = (byte)cbRank.SelectedIndex;
//					NodeSelected.Rank = (byte)((Pterodactyl)cbRank.SelectedItem).Case; // <- MapView1-type code.

					if (NodeSelected.Spawn != SpawnWeight.None)
					{
						if (RoutesInfo != null)
							RoutesInfo.UpdateNoderank(_curNoderank, NodeSelected.Rank);

						_curNoderank = NodeSelected.Rank;
					}

					NodeSelected.OobRank = (byte)0;

					if (Tag as String == "ROUTE")
						ObserverManager.TopRouteView.ControlRoute.cbRank.SelectedIndex = cbRank.SelectedIndex;
					else //if (Tag == "TOPROUTE")
						ObserverManager.RouteView.Control.cbRank.SelectedIndex = cbRank.SelectedIndex;
				}
			}
		}

		private void OnSpawnWeightSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RouteChanged = true;
				NodeSelected.Spawn = (SpawnWeight)((Pterodactyl)cbSpawn.SelectedItem).Case;

				if (RoutesInfo != null)
					RoutesInfo.ChangedSpawnweight(_curSpawnweight, NodeSelected.Spawn, NodeSelected.Rank);

				_curSpawnweight = NodeSelected.Spawn;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.cbSpawn.SelectedIndex = cbSpawn.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.cbSpawn.SelectedIndex = cbSpawn.SelectedIndex;

				RefreshControls(); // update the importance bar
			}
		}

		private void OnPatrolPrioritySelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RouteChanged = true;
				NodeSelected.Patrol = (PatrolPriority)((Pterodactyl)cbPatrol.SelectedItem).Case;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.cbPatrol.SelectedIndex = cbPatrol.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.cbPatrol.SelectedIndex = cbPatrol.SelectedIndex;

				RefreshControls(); // update the importance bar
			}
		}

		private void OnBaseAttackSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RouteChanged = true;
				NodeSelected.Attack = (AttackBase)((Pterodactyl)cbAttack.SelectedItem).Case;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.cbAttack.SelectedIndex = cbAttack.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.cbAttack.SelectedIndex = cbAttack.SelectedIndex;
			}
		}
		#endregion Events (NodeData)


		#region Events (LinkData)
		/// <summary>
		/// Changes a link's destination.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkDestSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RouteChanged = true;

				int slot;
				TextBox tb;
				Button btnGo;

				var cb = sender as ComboBox;
				if (cb == cbLink1Dest)
				{
					slot  = 0;
					tb    = tbLink1Dist;
					btnGo = btnGoLink1;
				}
				else if (cb == cbLink2Dest)
				{
					slot  = 1;
					tb    = tbLink2Dist;
					btnGo = btnGoLink2;
				}
				else if (cb == cbLink3Dest)
				{
					slot  = 2;
					tb    = tbLink3Dist;
					btnGo = btnGoLink3;
				}
				else if (cb == cbLink4Dest)
				{
					slot  = 3;
					tb    = tbLink4Dist;
					btnGo = btnGoLink4;
				}
				else //if (cb == cbLink5Dest)
				{
					slot  = 4;
					tb    = tbLink5Dist;
					btnGo = btnGoLink5;
				}

				var dest = cb.SelectedItem as byte?; // check for id or compass pt/not used.
				if (!dest.HasValue)
					dest = (byte?)(cb.SelectedItem as LinkType?);

				bool enable, text;

				var link = NodeSelected[slot];
				switch (link.Destination = dest.Value)
				{
					case Link.NotUsed:
						link.Type = UnitType.Any;

						tb.Text = String.Empty;
						link.Distance = 0;

						enable =
						text   = false;
						break;

					case Link.ExitWest:
					case Link.ExitNorth:
					case Link.ExitEast:
					case Link.ExitSouth:
						tb.Text = "0";
						link.Distance = 0;

						enable = false;
						text   = true;
						break;

					default:
						link.Distance = CalculateLinkDistance(
															NodeSelected,
															MapFile.Routes[link.Destination],
															tb,
															slot);
						enable =
						text   = true;
						break;
				}

				btnGo.Enabled = enable;
				btnGo.Text = text ? Go : String.Empty;

				RoutePanel.SpotPosition = new Point(-1,-1);

				if (Tag as String == "ROUTE")
				{
					ObserverManager.TopRouteView.ControlRoute.TransferDestination(
																				slot,
																				cb.SelectedIndex,
																				tb.Text,
																				enable,
																				btnGo.Text);
				}
				else //if (Tag == "TOPROUTE")
				{
					ObserverManager.RouteView.Control.TransferDestination(
																		slot,
																		cb.SelectedIndex,
																		tb.Text,
																		enable,
																		btnGo.Text);
				}

				RefreshControls();
			}
		}

		/// <summary>
		/// Transfers link-destination values from RouteView to
		/// TopRouteView(Route) or vice versa. The args are passed from the
		/// other viewer.
		/// </summary>
		/// <param name="slot">the link-slot's id</param>
		/// <param name="dest">the selected-index of the destination</param>
		/// <param name="dist">the distance text</param>
		/// <param name="enable">true to enable the Go button</param>
		/// <param name="text">the Go button text</param>
		private void TransferDestination(int slot, int dest, string dist, bool enable, string text)
		{
			ComboBox cbDest;
			TextBox tbDist;
			Button btnGo;

			switch (slot)
			{
				case 0:
					cbDest = cbLink1Dest;
					tbDist = tbLink1Dist;
					btnGo  = btnGoLink1;
					break;

				case 1:
					cbDest = cbLink2Dest;
					tbDist = tbLink2Dist;
					btnGo  = btnGoLink2;
					break;

				case 2:
					cbDest = cbLink3Dest;
					tbDist = tbLink3Dist;
					btnGo  = btnGoLink3;
					break;

				case 3:
					cbDest = cbLink4Dest;
					tbDist = tbLink4Dist;
					btnGo  = btnGoLink4;
					break;

				default: //case 4:
					cbDest = cbLink5Dest;
					tbDist = tbLink5Dist;
					btnGo  = btnGoLink5;
					break;
			}

			cbDest.SelectedIndex = dest;
			tbDist.Text          = dist;
			btnGo.Enabled        = enable;
			btnGo.Text           = text;
		}

		/// <summary>
		/// Calculates the distance between two nodes by Pythagoras.
		/// </summary>
		/// <param name="nodeA">a RouteNode</param>
		/// <param name="nodeB">another RouteNode</param>
		/// <param name="textBox">the textbox that shows the distance (default null)</param>
		/// <param name="slot">the slot of the textbox - not used unless 'textBox'
		/// is specified (default 0)</param>
		/// <returns>the distance as a byte-value</returns>
		private byte CalculateLinkDistance(
				RouteNode nodeA,
				RouteNode nodeB,
				Control textBox = null,
				int slot = 0)
		{
			int dist = (int)Math.Sqrt(
									Math.Pow(nodeA.Col - nodeB.Col, 2) +
									Math.Pow(nodeA.Row - nodeB.Row, 2) +
									Math.Pow(nodeA.Lev - nodeB.Lev, 2));
			if (textBox != null)
				textBox.Text = dist.ToString(System.Globalization.CultureInfo.InvariantCulture)
							 + GetDistanceArrow(slot);

			return (byte)dist;
		}


		/// <summary>
		/// Changes a link's UnitType.
		/// TODO: Since a link's UnitType is not used just give it the value
		/// of the link's destination UnitType.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkUnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RouteChanged = true;

				int slot;

				var cb = sender as ComboBox;
				if (cb == cbLink1UnitType)
					slot = 0;
				else if (cb == cbLink2UnitType)
					slot = 1;
				else if (cb == cbLink3UnitType)
					slot = 2;
				else if (cb == cbLink4UnitType)
					slot = 3;
				else //if (cb == cbLink5UnitType)
					slot = 4;

				NodeSelected[slot].Type = (UnitType)cb.SelectedItem;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.TransferUnitType(slot, cb.SelectedIndex);
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.TransferUnitType(slot, cb.SelectedIndex);
			}
		}

		/// <summary>
		/// Transfers link-unittype values from RouteView to TopRouteView(Route)
		/// or vice versa. The field is actually in the other viewer.
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="type"></param>
		private void TransferUnitType(int slot, int type)
		{
			ComboBox cbUnitType;
			switch (slot)
			{
				case 0:  cbUnitType = cbLink1UnitType; break;
				case 1:  cbUnitType = cbLink2UnitType; break;
				case 2:  cbUnitType = cbLink3UnitType; break;
				case 3:  cbUnitType = cbLink4UnitType; break;
				default: cbUnitType = cbLink5UnitType; break; //case 4
			}
			cbUnitType.SelectedIndex = type;
		}

		/// <summary>
		/// Selects the node at the destination of a link when a Go-button is
		/// clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkGoClick(object sender, EventArgs e)
		{
			int slot;

			var btn = sender as Button;
			if      (btn == btnGoLink1) slot = 0;
			else if (btn == btnGoLink2) slot = 1;
			else if (btn == btnGoLink3) slot = 2;
			else if (btn == btnGoLink4) slot = 3;
			else                        slot = 4; // btn == btnGoLink5

			byte dest = NodeSelected[slot].Destination;
			var node  = MapFile.Routes[dest];

			if (!RouteNodeCollection.IsNodeOutsideMapBounds(
														node,
														MapFile.MapSize.Cols,
														MapFile.MapSize.Rows,
														MapFile.MapSize.Levs))
			{
				OgnodeId = NodeSelected.Index; // store the current nodeId for the og-button.

				ObserverManager.RouteView   .Control     .btnOg.Enabled =
				ObserverManager.TopRouteView.ControlRoute.btnOg.Enabled = true;

				SelectNode(dest);

				SpotGoDestination(slot); // highlight back to the startnode.
			}
			else if (RouteCheckService.ShowInvalid(MapFile, node))
			{
				RouteChanged = true;
				UpdateNodeInfo();
				// TODO: May need _pnlRoutes.Refresh()
			}

			RoutePanel.Select();
		}

		/// <summary>
		/// Deals with the ramifications of a Go or Og click.
		/// IMPORTANT: Any changes that are done here regarding node-selection
		/// should be reflected in RoutePanelParent.OnMouseDown() since that is
		/// an alternate way to select a tile/node.
		/// </summary>
		/// <param name="id"></param>
		private void SelectNode(int id)
		{
			var node = MapFile.Routes[id];
			var loc = new Point(node.Col, node.Row);

			if (node.Lev != MapFile.Level)
				MapFile.Level = node.Lev; // fire SelectLevel

			MapFile.Location = new MapLocation( // fire SelectLocation
											loc.Y, loc.X,
											MapFile.Level);

			MainViewOverlay.that.ProcessSelection(loc,loc);

			var args = new RoutePanelEventArgs(
											MouseButtons.Left,
											MapFile[loc.Y, loc.X],
											MapFile.Location);
			OnRoutePanelMouseDown(null, args);


			RoutePanelParent.SelectedLocation = loc;

			InvalidateControls();
		}

		/// <summary>
		/// Highlights a link-line and destination-tile when the mousecursor
		/// enters or hovers over a link-slot's control object.
		/// NOTE.NET anomaly: After the selected index changes of a combobox
		/// the next mouse-enter event won't fire; but doing a mouse-leave then
		/// another mouse-enter catches.
		/// WORKAROUND: Use the mouse-hover event for the comboboxes instead of
		/// the mouse-enter event. For this the cursor has to be kept stationary
		/// and there is also a slight lag.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkMouseEnter(object sender, EventArgs e)
		{
			int slot;
			switch ((sender as Control).Tag as String)
			{
				case "L1": slot = 0; break;
				case "L2": slot = 1; break;
				case "L3": slot = 2; break;
				case "L4": slot = 3; break;
				default:   slot = 4; break; // tag == "L5"
			}
			SpotGoDestination(slot); // TODO: RouteView/TopRouteView(Route)
		}

		/// <summary>
		/// Sets the highlighted destination link-line and node if applicable.
		/// </summary>
		/// <param name="slot">the link-slot whose destination should get
		/// highlighted</param>
		private void SpotGoDestination(int slot)
		{
			if (NodeSelected != null && NodeSelected[slot] != null) // safety: Go should not be enabled unless a node is selected.
			{
				byte dest = NodeSelected[slot].Destination;
				if (dest != Link.NotUsed)
				{
					int c, r;
					switch (dest)
					{
						case Link.ExitNorth: c = r = -2; break;
						case Link.ExitEast:  c = r = -3; break;
						case Link.ExitSouth: c = r = -4; break;
						case Link.ExitWest:  c = r = -5; break;
	
						default:
							var node = MapFile.Routes[dest];
							c = (int)node.Col;
							r = (int)node.Row;
							break;
					}
	
					RoutePanel.SpotPosition = new Point(c, r); // TODO: static - RouteView/TopRouteView(Route)

					RoutePanel.Refresh();
//					RefreshControls();
				}
			}
		}

		private void OnLinkMouseLeave(object sender, EventArgs e)
		{
			RoutePanel.SpotPosition = new Point(-1, -1);

			RoutePanel.Refresh();
//			RefreshControls();
		}

		private void OnOgClick(object sender, EventArgs e)
		{
			if (OgnodeId < MapFile.Routes.Count) // in case nodes were deleted.
			{
				if (NodeSelected == null || OgnodeId != NodeSelected.Index)
					SelectNode(OgnodeId);
			}
			else
			{
				ObserverManager.RouteView   .Control     .btnOg.Enabled =
				ObserverManager.TopRouteView.ControlRoute.btnOg.Enabled = false;
			}

			RoutePanel.Select();
		}

		private void OnOgMouseEnter(object sender, EventArgs e)
		{
			if (OgnodeId < MapFile.Routes.Count) // in case nodes were deleted.
			{
				var node = MapFile.Routes[OgnodeId];
				RoutePanel.SpotPosition = new Point(node.Col, node.Row);

				RoutePanel.Refresh();
//				RefreshControls();
			}
		}

		/// <summary>
		/// Disables the og-button when a Map gets loaded.
		/// </summary>
		internal void DisableOg()
		{
			ObserverManager.RouteView   .Control     .btnOg.Enabled =
			ObserverManager.TopRouteView.ControlRoute.btnOg.Enabled = false;
		}
		#endregion Events (LinkData)


		#region Events (node edit)
		/// <summary>
		/// Prevents two error-dialogs from showing if a key-cut is underway.
		/// </summary>
		private bool _asterisk;

		/// <summary>
		/// Handles keyboard input.
		/// @note Navigation keys are handled by 'KeyPreview' at the form level.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRoutePanelKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.S:
						XCMainWindow.that.OnSaveRoutesClick(null, EventArgs.Empty);
						break;

					case Keys.X: _asterisk = true;
								 OnCopyClick(  null, EventArgs.Empty);
								 OnDeleteClick(null, EventArgs.Empty);
								 _asterisk = false;
								 break;

					case Keys.C: OnCopyClick( null, EventArgs.Empty); break;
					case Keys.V: OnPasteClick(null, EventArgs.Empty); break;
				}
			}
			else if (e.KeyCode == Keys.Delete)
			{
				OnDeleteClick(null, null);
			}
		}

		private void OnCutClick(object sender, EventArgs e)
		{
			OnCopyClick(  null, EventArgs.Empty);
			OnDeleteClick(null, EventArgs.Empty);
		}

		private void OnCopyClick(object sender, EventArgs e)
		{
			RoutePanel.Select();

			if (NodeSelected != null)
			{
				ObserverManager.RouteView   .Control     .btnPaste.Enabled =
				ObserverManager.TopRouteView.ControlRoute.btnPaste.Enabled = true;

				var nodeText = string.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"{0}{6}{1}{6}{2}{6}{3}{6}{4}{6}{5}",
										NodeCopyPrefix,
										cbType  .SelectedIndex,
										cbPatrol.SelectedIndex,
										cbAttack.SelectedIndex,
										cbRank  .SelectedIndex,
										cbSpawn .SelectedIndex,
										NodeCopySeparator);

				// TODO: include Link info ... perhaps.
				// But re-assigning the link node-ids would be difficult, since
				// those nodes could have be deleted, etc.
				Clipboard.SetText(nodeText);
			}
			else
				ShowDialogAsterisk("A node must be selected.");
		}

		private void OnPasteClick(object sender, EventArgs e)
		{
			RoutePanel.Select();

			if (NodeSelected != null) // TODO: auto-create a new node
			{
				var nodeData = Clipboard.GetText().Split(NodeCopySeparator);
				if (nodeData[0] == NodeCopyPrefix)
				{
					RouteChanged = true;

					var invariant = System.Globalization.CultureInfo.InvariantCulture;

					cbType  .SelectedIndex = Int32.Parse(nodeData[1], invariant);
					cbPatrol.SelectedIndex = Int32.Parse(nodeData[2], invariant);
					cbAttack.SelectedIndex = Int32.Parse(nodeData[3], invariant);
					cbRank  .SelectedIndex = Int32.Parse(nodeData[4], invariant);
					cbSpawn .SelectedIndex = Int32.Parse(nodeData[5], invariant);

					// TODO: include Link info ... perhaps.
					// But re-assigning the link node-ids would be difficult, since
					// those nodes could have be deleted, etc.
				}
				else // non-node data is on the clipboard.
				{
					ObserverManager.RouteView   .Control     .btnPaste.Enabled =
					ObserverManager.TopRouteView.ControlRoute.btnPaste.Enabled = false;

					ShowDialogAsterisk("The data on the clipboard is not a node.");
				}
			}
			else
				ShowDialogAsterisk("A node must be selected.");
		}

		private void OnDeleteClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				RouteChanged = true;

				((MapTile)MapFile[NodeSelected.Row,
								  NodeSelected.Col,
								  NodeSelected.Lev]).Node = null;
				MapFile.Routes.DeleteNode(NodeSelected);

				ObserverManager.RouteView   .Control     .DeselectNode();
				ObserverManager.TopRouteView.ControlRoute.DeselectNode();

				UpdateNodeInfo();

				gbNodeData.Enabled =
				gbLinkData.Enabled = false;

				// TODO: check if the Og-button should be disabled when a node gets deleted or cut.

				RefreshControls();
			}			
			else if (!_asterisk)
				ShowDialogAsterisk("A node must be selected.");
		}

		private void ShowDialogAsterisk(string asterisk)
		{
			MessageBox.Show(
						this,
						asterisk,
						" Err..",
						MessageBoxButtons.OK,
						MessageBoxIcon.Asterisk,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion Events (node edit)


		/// <summary>
		/// Deselects any currently selected node.
		/// </summary>
		private void DeselectNode()
		{
			NodeSelected = null;
			RoutePanelParent.SelectedLocation = new Point(-1,-1);

			tsmiClearLinkData.Enabled = false; // TODO: RouteView/TopRouteView(Route)

			RoutePanel.Select();
		}


		#region Events (toolstrip)
		/// <summary>
		/// Handles clicking on any of the three ConnectType toolstrip buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConnectTypeClicked(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (!tsb.Checked)
			{
				RouteView alt;
				if (Tag as String == "ROUTE")
				{
					alt = ObserverManager.TopRouteView.ControlRoute;
				}
				else //if (Tag as String == "TOPROUTE")
				{
					alt = ObserverManager.RouteView.Control;
				}

				if (tsb == tsb_connect0)
				{
					_conType = ConnectNodesType.None;

						tsb_connect0.Checked =
					alt.tsb_connect0.Checked = true;
						tsb_connect1.Checked =
					alt.tsb_connect1.Checked =
						tsb_connect2.Checked =
					alt.tsb_connect2.Checked = false;

						tsb_connect0.Image =
					alt.tsb_connect0.Image = Properties.Resources.connect_0_red;
						tsb_connect1.Image =
					alt.tsb_connect1.Image = Properties.Resources.connect_1;
						tsb_connect2.Image =
					alt.tsb_connect2.Image = Properties.Resources.connect_2;
				}
				else if (tsb == tsb_connect1)
				{
					_conType = ConnectNodesType.OneWay;

						tsb_connect1.Checked =
					alt.tsb_connect1.Checked = true;
						tsb_connect0.Checked =
					alt.tsb_connect0.Checked =
						tsb_connect2.Checked =
					alt.tsb_connect2.Checked = false;

						tsb_connect1.Image =
					alt.tsb_connect1.Image = Properties.Resources.connect_1_blue;
						tsb_connect0.Image =
					alt.tsb_connect0.Image = Properties.Resources.connect_0;
						tsb_connect2.Image =
					alt.tsb_connect2.Image = Properties.Resources.connect_2;
				}
				else //if (tsb == tsb_connect2)
				{
					_conType = ConnectNodesType.TwoWay;

						tsb_connect2.Checked =
					alt.tsb_connect2.Checked = true;
						tsb_connect0.Checked =
					alt.tsb_connect0.Checked =
						tsb_connect1.Checked =
					alt.tsb_connect1.Checked = false;

						tsb_connect2.Image =
					alt.tsb_connect2.Image = Properties.Resources.connect_2_green;
						tsb_connect0.Image =
					alt.tsb_connect0.Image = Properties.Resources.connect_0;
						tsb_connect1.Image =
					alt.tsb_connect1.Image = Properties.Resources.connect_1;
				}
			}
			RoutePanel.Select();
		}


		private void OnExportClick(object sender, EventArgs e)
		{
			if (MapFile != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title            = "Save Route file as ...";
					sfd.DefaultExt       = GlobalsXC.RouteExt;
					sfd.FileName         = MapFile.Descriptor.Label + GlobalsXC.RouteExt;
					sfd.Filter           = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
					sfd.InitialDirectory = Path.Combine(MapFile.Descriptor.Basepath, GlobalsXC.RoutesDir);

					if (sfd.ShowDialog() == DialogResult.OK)
					{
						MapFile.Routes.SaveRoutesExport(sfd.FileName);
					}
				}
			}
		}

		private void OnImportClick(object sender, EventArgs e)
		{
			if (MapFile != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title            = "Open a Route file ...";
					ofd.DefaultExt       = GlobalsXC.RouteExt;
					ofd.FileName         = MapFile.Descriptor.Label + GlobalsXC.RouteExt;
					ofd.Filter           = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
					ofd.InitialDirectory = Path.Combine(MapFile.Descriptor.Basepath, GlobalsXC.RoutesDir);

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						RouteChanged = true;

						ObserverManager.RouteView   .Control     .DeselectNode();
						ObserverManager.TopRouteView.ControlRoute.DeselectNode();

						MapFile.ClearRouteNodes();
						MapFile.Routes = new RouteNodeCollection(ofd.FileName);
						MapFile.SetupRouteNodes();

						RouteCheckService.CheckNodeBounds(MapFile);

						UpdateNodeInfo(); // not sure is necessary ...
						RefreshPanels();

						if (RoutesInfo != null)
							RoutesInfo.Initialize(MapFile);
					}
				}
			}
		}


		private void OnEditOpening(object sender, EventArgs e)
		{
			tsmi_LowerNode.Enabled = (NodeSelected != null && NodeSelected.Lev != MapFile.MapSize.Levs - 1);
			tsmi_RaiseNode.Enabled = (NodeSelected != null && NodeSelected.Lev != 0);
		}

		private void OnNodeRaise(object sender, EventArgs e)
		{
			Dragnode = NodeSelected;

			var args = new RoutePanelEventArgs(
											MouseButtons.None,
											MapFile[Dragnode.Row,
													Dragnode.Col,
													Dragnode.Lev - 1],
											new MapLocation(
														Dragnode.Row,
														Dragnode.Col,
														Dragnode.Lev - 1));
			OnRoutePanelMouseUp(null, args);

			SelectNode(NodeSelected.Index);
		}

		private void OnNodeLower(object sender, EventArgs e)
		{
			Dragnode = NodeSelected;

			var args = new RoutePanelEventArgs(
											MouseButtons.None,
											MapFile[Dragnode.Row,
													Dragnode.Col,
													Dragnode.Lev + 1],
											new MapLocation(
														Dragnode.Row,
														Dragnode.Col,
														Dragnode.Lev + 1));
			OnRoutePanelMouseUp(null, args);

			SelectNode(NodeSelected.Index);
		}

		/// <summary>
		/// Handler for menuitem that sets all NodeRanks to Civilian/Scout.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAllNodesRank0Click(object sender, EventArgs e)
		{
			string rank;
			if (MapFile.Descriptor.Pal == Palette.TftdBattle)
				rank = ((Pterodactyl)RouteNodeCollection.RankTftd[0]).ToString();
			else
				rank = ((Pterodactyl)RouteNodeCollection.RankUfo [0]).ToString();

			if (MessageBox.Show(
							this,
							"Are you sure you want to change all node ranks to"
								+ Environment.NewLine + Environment.NewLine
								+ rank,
							" Warning",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button2,
							0) == DialogResult.Yes)
			{
				int changed = 0;
				foreach (RouteNode node in MapFile.Routes)
				{
					if (node.Rank != 0)
					{
						if (RoutesInfo != null && node.Spawn != SpawnWeight.None)
							RoutesInfo.UpdateNoderank(node.Rank, 0);

						++changed;
						node.Rank = 0;
					}
				}

				if (changed != 0)
				{
					RouteChanged = true;
					UpdateNodeInfo();

					MessageBox.Show(
								this,
								changed + " nodes were changed.",
								" All nodes rank 0",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
					MessageBox.Show(
								this,
								"All nodes are already rank 0.",
								" All nodes rank 0",
								MessageBoxButtons.OK,
								MessageBoxIcon.Asterisk,
								MessageBoxDefaultButton.Button1,
								0);
			}
		}

		/// <summary>
		/// Handler for menuitem that clears all link-data of the currently
		/// selected node.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClearLinkDataClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				if (MessageBox.Show(
								this,
								"Are you sure you want to clear the selected node's Link data ...",
								" Warning",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button2,
								0) == DialogResult.Yes)
				{
					RouteChanged = true;

					for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
					{
						NodeSelected[slot].Destination = Link.NotUsed;
						NodeSelected[slot].Distance = 0;

						NodeSelected[slot].Type = UnitType.Any;
					}

					UpdateNodeInfo();
					RefreshControls();
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that updates all link distances in the RMP.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnUpdateAllLinkDistances(object sender, EventArgs e)
		{
			RouteNode node;
			Link link;
			byte dist;
			int changed = 0;

			for (var id = 0; id != MapFile.Routes.Count; ++id)
			{
				node = MapFile.Routes[id];

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
				{
					link = node[slot];
					switch (link.Destination)
					{
						case Link.NotUsed:
						case Link.ExitWest:
						case Link.ExitNorth:
						case Link.ExitEast:
						case Link.ExitSouth:
							if (link.Distance != 0)
							{
								link.Distance = 0;
								++changed;
							}
							break;

						default:
							dist = CalculateLinkDistance(
													node,
													MapFile.Routes[link.Destination]);
							if (link.Distance != dist)
							{
								link.Distance = dist;
								++changed;
							}
							break;
					}
				}
			}

			string info;
			if (changed != 0)
			{
				RouteChanged = true;
				info = String.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								"{0} link{1} updated.",
								changed,
								(changed == 1) ? " has been" : "s have been");

				UpdateNodeInfo();
			}
			else
			{
				info = String.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								"All link distances are already correct.");
			}

			MessageBox.Show(
						this,
						info,
						" Link distances updated",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information,
						MessageBoxDefaultButton.Button1,
						0);
		}

		/// <summary>
		/// Handler for menuitem that checks if any node's rank is beyond the
		/// array of the combobox. See also RouteNodeCollection.cTor
		/// TODO: Consolidate these checks to RouteCheckService.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCheckNodeRanksClick(object sender, EventArgs e)
		{
			var invalids = new List<byte>();
			foreach (RouteNode node in MapFile.Routes)
			{
				if (node.OobRank != (byte)0)
					invalids.Add(node.Index);
			}

			string info, title;
			MessageBoxIcon icon;

			if (invalids.Count != 0)
			{
				icon  = MessageBoxIcon.Warning;
				title = " Warning";
				info  = String.Format(
									System.Globalization.CultureInfo.CurrentCulture,
									"The following route-{0} an invalid NodeRank.{1}",
									(invalids.Count == 1) ? "node has"
														  : "nodes have",
									Environment.NewLine);

				foreach (byte id in invalids)
					info += Environment.NewLine + id;
			}
			else
			{
				icon  = MessageBoxIcon.Information;
				title = " Good stuff, Magister Ludi";
				info  = "There are no invalid NodeRanks detected.";
			}

			MessageBox.Show(
						this,
						info,
						title,
						MessageBoxButtons.OK,
						icon,
						MessageBoxDefaultButton.Button1,
						0);
		}

		/// <summary>
		/// Handler for menuitem that checks if any node's location is outside
		/// the dimensions of the Map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCheckOobNodesClick(object sender, EventArgs e)
		{
			if (RouteCheckService.CheckNodeBounds(MapFile, true))
			{
				RouteChanged = true;
				UpdateNodeInfo();
			}
		}
		#endregion Events (toolstrip)


		#region Events
		internal static RoutesInfo RoutesInfo
		{ get; set; }

		/// <summary>
		/// Handler for clicking the Tallyho button. Opens a dialog that
		/// displays info tallied from current RouteNodes in the tileset as well
		/// as totals across the tileset's Category.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTallyhoClick(object sender, EventArgs e)
		{
			if (RoutesInfo == null)
			{
				RoutesInfo = new RoutesInfo(MapFile);
				RoutesInfo.Show(this);
			}
			else
			{
				if (RoutesInfo.WindowState == FormWindowState.Minimized)
					RoutesInfo.WindowState  = FormWindowState.Normal;

				RoutesInfo.Activate(); // so what's the diff ->
//				_routesinfo.Focus();
//				_routesinfo.Select();
//				_routesinfo.BringToFront();
//				_routesinfo.TopMost = true;
//				_routesinfo.TopMost = false;
			}
		}
		#endregion Events


		#region Options
		/// <summary>
		/// Loads default options for RouteView in TopRouteView screens.
		/// </summary>
		protected internal override void LoadControlDefaultOptions()
		{
			OnConnectTypeClicked(tsb_connect0, EventArgs.Empty); // TODO: add to Options perhaps

			Optionables.LoadDefaults(Options);
		}


		internal static Form _foptions;	// is static so that it will be used by both
										// RouteView and TopRouteView(Route)
		/// <summary>
		/// Handles a click on the Options button to show or hide an Options-
		/// form. Instantiates an 'OptionsForm' if one doesn't exist for this
		/// viewer. Also subscribes to a form-closing handler that will hide the
		/// form unless MainView is closing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (!tsb.Checked)
			{
				setOptionsChecked(true);

				if (_foptions == null)
				{
					_foptions = new OptionsForm(
											Optionables,
											Options,
											OptionsForm.OptionableType.RouteView);
					_foptions.Text = " RouteView Options";

					OptionsManager.Views.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!XCMainWindow.Quit)
						{
							setOptionsChecked(false);

							e1.Cancel = true;
							_foptions.Hide();
						}
						else
							RegistryInfo.UpdateRegistry(_foptions);
					};
				}

				_foptions.Show();

				if (_foptions.WindowState == FormWindowState.Minimized)
					_foptions.WindowState  = FormWindowState.Normal;
			}
			else
				_foptions.Close();
		}

		private void setOptionsChecked(bool @checked)
		{
			ObserverManager.RouteView   .Control     .tsb_Options.Checked =
			ObserverManager.TopRouteView.ControlRoute.tsb_Options.Checked = @checked;
		}

		/// <summary>
		/// Gets the Options button on the toolstrip.
		/// </summary>
		/// <returns>either the button in RouteView or TopRouteView(Route) -
		/// doesn't matter as long as they are kept in sync</returns>
		internal ToolStripButton GetOptionsButton()
		{
			return tsb_Options;
		}
		#endregion Options


		#region Update UI
//		internal void InvalidateRoutePanels()
//		{
//			ObserverManager.RouteView   .Control     ._pnlRoutes.Invalidate();
//			ObserverManager.TopRouteView.ControlRoute._pnlRoutes.Invalidate();
//		}

		internal void RefreshControls()
		{
			ObserverManager.RouteView   .Control     .Refresh();
			ObserverManager.TopRouteView.ControlRoute.Refresh();
		}

		private void InvalidateControls()
		{
			ObserverManager.RouteView   .Control     .Invalidate();
			ObserverManager.TopRouteView.ControlRoute.Invalidate();
		}

		private void RefreshPanels()
		{
			ObserverManager.RouteView   .Control     .RoutePanel.Refresh();
			ObserverManager.TopRouteView.ControlRoute.RoutePanel.Refresh();
		}

		private void InvalidatePanels()
		{
			ObserverManager.RouteView   .Control     .RoutePanel.Invalidate();
			ObserverManager.TopRouteView.ControlRoute.RoutePanel.Invalidate();
		}

		private void UpdateNodeInfo()
		{
			ObserverManager.RouteView   .Control     .UpdateNodeInformation();
			ObserverManager.TopRouteView.ControlRoute.UpdateNodeInformation();
		}
		#endregion Update UI
	}



	#region Event args
	/// <summary>
	/// Event args for RouteView.
	/// </summary>
	internal sealed class RoutePanelEventArgs
		:
			EventArgs
	{
		#region Properties
		internal MouseButtons MouseButton
		{ get; private set; }

		internal MapTileBase Tile
		{ get; private set; }

		internal MapLocation Location
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="tile"></param>
		/// <param name="location"></param>
		internal RoutePanelEventArgs(
				MouseButtons button,
				MapTileBase tile,
				MapLocation location)
		{
			MouseButton = button;
			Tile        = tile;
			Location    = location;
		}
		#endregion cTor
	}
	#endregion Event args
}
