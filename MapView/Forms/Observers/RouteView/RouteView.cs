using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;
using XCom.Base;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Does all the heavy-lifting/node-manipulations in RouteView and
	/// TopRouteView(Route).
	/// @note Static objects in this class are shared between the two viewers -
	/// otherwise RouteView and TopRouteView(Route) instantiate separately.
	/// @note <see cref="RoutePanelParent"/> also handles mouse events.
	/// </summary>
	internal sealed partial class RouteView
		:
			MapObserverControl // UserControl, IMapObserver/MapObserverControl
	{
		#region Enums
		private enum ConnectNodesType
		{
			None, OneWay, TwoWay
		}

		private enum LinkSlotResult
		{
			LinkExists   = -1,
			AllSlotsUsed = -2
		}
		#endregion Enums


		#region Fields (static)
		private static ConnectNodesType _conType = ConnectNodesType.None; // safety - shall be set by LoadControlDefaultOptions()

		private const string NodeCopyPrefix  = "MVNode"; // TODO: use a struct to copy/paste the info.
		private const char NodeCopySeparator = '|';

		private const string Go = "go";

		internal static RouteNode Dragnode;

		/// <summary>
		/// Stores the node-id from which a "Go" button is clicked. Used to
		/// re-select the original node - which might not be equivalent to
		/// "Back" (if there were a Back button).
		/// </summary>
		private static int _ogId;

		internal static byte _curNoderank;
		internal static SpawnWeight _curSpawnweight;

		private static bool _connectoractivated;
		#endregion Fields (static)


		#region Fields
		private CompositedPanel _pnlRoutes; // NOTE: needs to be here for MapObserver vs Designer stuff.

		private MapFile _file;

		private bool _loadingInfo;

		/// <summary>
		/// Used by <see cref="UpdateNodeInformation"/>.
		/// </summary>
		private readonly List<object> _linksList = new List<object>();

		/// <summary>
		/// Used by <see cref="UpdateNodeInformation"/>.
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
		/// Inherited from <see cref="IMapObserver"/> through <see cref="MapObserverControl"/>.
		/// </summary>
		[Browsable(false)]
		public override MapFileBase MapBase
		{
			set // TODO: check RouteView/TopRouteView(Route)
			{
				_file = (base.MapBase = value) as MapFile;

				DeselectNode();

				if ((RoutePanel.MapFile = _file) != null)
				{
					cbRank.Items.Clear();

					if (_file.Descriptor.GroupType == GameType.Tftd)
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
		/// Coordinates the <see cref="RoutesChanged"/> flag between RouteView
		/// and TopRouteView(Route).
		/// </summary>
		internal static bool RoutesChangedCoordinator
		{
			set
			{
				ObserverManager.RouteView   .Control     .RoutesChanged =
				ObserverManager.TopRouteView.ControlRoute.RoutesChanged = value;
			}
		}
		#endregion Properties (static)


		#region Properties
		internal RoutePanel RoutePanel
		{ get; private set; }

		/// <summary>
		/// Sets the 'MapFileBase.RoutesChanged' flag. This is only an
		/// intermediary that shows "routes changed" in RouteView; the real
		/// 'RoutesChanged' flag is stored in <see cref="T:XCom.Base.MapFileBase"/>.
		/// reasons.
		/// </summary>
		private bool RoutesChanged
		{
			set { btnSave.Enabled = (_file.RoutesChanged = value); }
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
		/// Inherited from <see cref="IMapObserver"/> through <see cref="MapObserverControl"/>.
		/// @note This will fire twice whenever the location changes: once by
		/// RouteView and again by TopRouteView(Route). This is desired behavior
		/// since it updates the selected-location in both viewers.
		/// @note A route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to (or press [Enter] when the tile is
		/// selected). This is a design decision that allows the selected node
		/// to stay selected while other tiles get highlighted.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			PrintSelectedInfo();
		}

		/// <summary>
		/// Inherited from <see cref="IMapObserver"/> through <see cref="MapObserverControl"/>.
		/// @note This will fire twice whenever the location changes: once by
		/// RouteView and again by TopRouteView(Route). However only the viewer
		/// that the mousecursor is currently in should have the
		/// location-string's color updated; the condition to allow that update
		/// is (RoutePanel._col != -1).
		/// @note The route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to (or press [Enter] when the tile is
		/// selected). This is a design decision that allows the selected node
		/// to stay selected while other tiles get highlighted.
		/// </summary>
		/// <param name="args"></param>
		public override void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			//LogFile.WriteLine("RouteView.OnSelectLevelObserver() " + Tag);

			if (RoutePanel._col != -1) // find the Control that the mousecursor is in (if either)
			{
				//LogFile.WriteLine(". do overinfo");

				Color color; int id;

				RouteNode node = MapBase[RoutePanel._col,
										 RoutePanel._row,
										 MapBase.Level].Node;
				if (node == null)
				{
					id = -1;
					color = SystemColors.ControlText;
				}
				else
				{
					id = node.Id;

					if (node.Spawn == SpawnWeight.None)
					{
						color = Optionables.NodeColor;
					}
					else
						color = Optionables.NodeSpawnColor;
				}

				lblOver.ForeColor = color;

				PrintOverInfo(id);
			}

			PrintSelectedInfo();

			Refresh(); // required to force the other RouteView panel to redraw.
//			InvalidatePanels();
		}
		#endregion Events (override) inherited from IMapObserver/MapObserverControl


		#region Methods (print TileData)
		/// <summary>
		/// Clears the selected tile-info text when another Map loads.
		/// </summary>
		internal void ClearOveredInfo()
		{
			lblOver.Text = String.Empty;
		}

		/// <summary>
		/// Clears the selected tile-info text when another Map loads.
		/// </summary>
		internal void ClearSelectedInfo()
		{
			lblSelected.Text = String.Empty;
		}

		/// <summary>
		/// Updates the selected node's textcolor when the Option changes.
		/// @note Called by Options only.
		/// </summary>
		internal void SetInfoSelectedColor()
		{
			Color color;
			if (NodeSelected == null)
			{
				color = SystemColors.ControlText;
			}
			else
				color = Optionables.NodeSelectedColor;

			ObserverManager.RouteView   .Control     .lblSelected.ForeColor =
			ObserverManager.TopRouteView.ControlRoute.lblSelected.ForeColor = color;
		}

		/// <summary>
		/// Updates the overed node's textcolor when the Option changes.
		/// @note Called by Options only.
		/// </summary>
		internal void SetInfoOverColor()
		{
			if (RoutePanel._col != -1) // find the Control that the mousecursor is in (if either)
			{
				Color color;

				RouteNode node = MapBase[RoutePanel._col,
										 RoutePanel._row,
										 MapBase.Level].Node;
				if (node == null)
				{
					color = SystemColors.ControlText;
				}
				else if (node.Spawn == SpawnWeight.None)
				{
					color = Optionables.NodeColor;
				}
				else
					color = Optionables.NodeSpawnColor;

				lblOver.ForeColor = color; // set color in the overed viewer only.
			}
		}

		/// <summary>
		/// Updates the overed node's info and textcolor when a node is dragged.
		/// </summary>
		internal void SetInfoOverText()
		{
			if (RoutePanel._col != -1) // find the Control that the mousecursor is in (if either)
			{
				Color color; int id;

				RouteNode node = MapBase[RoutePanel._col,
										 RoutePanel._row,
										 MapBase.Level].Node;
				if (node == null)
				{
					id = -1;
					color = SystemColors.ControlText;
				}
				else
				{
					id = node.Id;

					if (node.Spawn == SpawnWeight.None)
					{
						color = Optionables.NodeColor;
					}
					else
						color = Optionables.NodeSpawnColor;
				}

				lblOver.ForeColor = color;

				PrintOverInfo(id);
			}
		}

		/// <summary>
		/// Prints the currently mouseovered tile-id and -location to the
		/// TileData groupbox.
		/// </summary>
		/// <param name="id"></param>
		private void PrintOverInfo(int id)
		{
			string info;

			if (id != -1)
				info = "Over " + id;
			else
				info = String.Empty;

			if (RoutePanel._col != -1)
			{
				info += Environment.NewLine;

				int c = RoutePanel._col;
				int r = RoutePanel._row;
				int l = _file.MapSize.Levs - MapBase.Level;

				if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
				if (!MainViewF.Optionables.Base1_z) { --l; }

				info += String.Format(
									CultureInfo.InvariantCulture,
									"c {0}  r {1}  L {2}",
									c,r,l);
			}

			ObserverManager.RouteView   .Control     .lblOver.Text =
			ObserverManager.TopRouteView.ControlRoute.lblOver.Text = info;
		}

		/// <summary>
		/// Prints the currently selected tile-info to the TileData groupbox.
		/// NOTE: The displayed level is inverted here.
		/// </summary>
		internal void PrintSelectedInfo()
		{
			//LogFile.WriteLine("RouteView.PrintSelectedInfo()" + Tag);

			if (MainViewOverlay.that.FirstClick)
			{
				string info;
				int level;
				Color color;

				if (NodeSelected == null)
				{
					info = String.Empty;
					level = MapBase.Level;
					color = SystemColors.ControlText;
				}
				else
				{
					info = "Selected " + NodeSelected.Id;
					level = NodeSelected.Lev;
					color = Optionables.NodeSelectedColor;
				}

				info += Environment.NewLine;

				int c = MapBase.Location.Col;
				int r = MapBase.Location.Row;
				int l = _file.MapSize.Levs - level;

				if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
				if (!MainViewF.Optionables.Base1_z) { --l; }

				info += String.Format(
									CultureInfo.InvariantCulture,
									"c {0}  r {1}  L {2}",
									c,r,l);

				lblSelected.ForeColor = color;
				lblSelected.Text = info;
				lblSelected.Refresh(); // fast update.
			}
		}
		#endregion Methods (print TileData)


		#region Events (mouse-events for RoutePanel)
		/// <summary>
		/// Handler that selects a node on LMB, creates and/or connects nodes on
		/// RMB.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnRoutePanelMouseDown(object sender, RoutePanelEventArgs args)
		{
			bool updateinfo = false;

			RouteNode node = args.Tile.Node;

			if (NodeSelected == null) // a node is not currently selected ->
			{
				if ((NodeSelected = node) == null
					&& args.MouseButton == MouseButtons.Right)
				{
					RoutesChangedCoordinator = true;
					NodeSelected = _file.AddRouteNode(args.Location);
					InvalidatePanels(); // not sure why but that's needed after adding the "ReduceDraws" option
				}
				updateinfo = (NodeSelected != null);
			}
			else if (node == null) // a node is already selected but there's not a node on the current tile ->
			{
				if (args.MouseButton == MouseButtons.Right)
				{
					RoutesChangedCoordinator = true;
					node = _file.AddRouteNode(args.Location);
					ConnectNode(node);
				}
				NodeSelected = node;
				updateinfo = true;
			}
			else if (node != NodeSelected) // a node is already selected and it's not the node on the current tile ->
			{
				if (args.MouseButton == MouseButtons.Right)
					ConnectNode(node);

				NodeSelected = node;
				updateinfo = true;
			}
			// else the selected-node is the node clicked.

			if (updateinfo) UpdateNodeInfo();

			Dragnode = NodeSelected;

			EnableEditButtons();
		}

		/// <summary>
		/// Handler that completes a dragnode operation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnRoutePanelMouseUp(object sender, RoutePanelEventArgs args)
		{
			if (Dragnode != null)
			{
				if (args.Tile.Node == null)
				{
					RoutesChangedCoordinator = true;

					_file[Dragnode.Col, // clear the node from the previous tile
						  Dragnode.Row,
						  Dragnode.Lev].Node = null;

					Dragnode.Col = (byte)args.Location.Col; // reassign the node's x/y/z values
					Dragnode.Row = (byte)args.Location.Row; // these get saved w/ Routes.
					Dragnode.Lev =       args.Location.Lev;

					args.Tile.Node = Dragnode; // assign the node to the tile at the mouse-up location.

					var loc = new Point(Dragnode.Col, Dragnode.Row);
					MainViewOverlay.that.ProcessSelection(loc,loc);

					ObserverManager.RouteView   .Control     .UpdateLinkDistances();
					ObserverManager.TopRouteView.ControlRoute.UpdateLinkDistances();

					SetInfoOverText();
				}
				else if (args.Location.Col != (int)Dragnode.Col
					||   args.Location.Row != (int)Dragnode.Row
					||   args.Location.Lev !=      Dragnode.Lev)
				{
					MessageBox.Show(
								this,
								"Cannot move node onto another node.",
								" Err..",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}
				Dragnode = null;
			}
		}

		/// <summary>
		/// Handler that updates the overed node's info and textcolor. Also sets
		/// the position for the InfoOverlay and refreshes panels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal void OnRoutePanelMouseMove(object sender, MouseEventArgs args)
		{
			//LogFile.WriteLine("RouteView.OnRoutePanelMouseMove()");

			RoutePanel._pos = new Point(args.X, args.Y);

			SetInfoOverText();

			ObserverManager.RouteView   .Control     .lblOver.Refresh(); // fast update. // NOTE: Only RouteView not TopRouteView(Route)
			ObserverManager.TopRouteView.ControlRoute.lblOver.Refresh(); // fast update. // wants fast update. go figure

			// TODO: if (MainView.Optionables.ShowOverlay)
			RefreshPanels(); // fast update. (else the InfoOverlay on RouteView but not TopRouteView(Route) gets sticky - go figur)
		}

		/// <summary>
		/// Handler that hides the info-overlay when the mouse leaves this
		/// control. See also RoutePanelParent.t1_Tick().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRoutePanelMouseLeave(object sender, EventArgs e)
		{
			// TODO: perhaps fire RoutePanelParent.OnMouseMove()
			RoutePanel._col = -1;
			RefreshPanels();
		}
		#endregion Events (mouse-events for RoutePanel)


		#region Methods (mouse-event helpers)
		/// <summary>
		/// Checks connector and connects nodes if applicable.
		/// </summary>
		/// <param name="node">the destination node to try to link the currently
		/// selected node to</param>
		private void ConnectNode(RouteNode node)
		{
			LinkSlotResult result;

			switch (_conType)
			{
				case ConnectNodesType.TwoWay:
					switch (result = GetOpenLinkSlot(node, NodeSelected.Id))
					{
						case LinkSlotResult.AllSlotsUsed:
							MessageBox.Show(
										this,
										"Destination node could not be linked to the source node."
											+ " Its link-slots are full.",
										" Warning",
										MessageBoxButtons.OK,
										MessageBoxIcon.Warning,
										MessageBoxDefaultButton.Button1,
										0);
							// TODO: the message leaves the RoutePanel drawn in an awkward state
							// but discovering where to call Refresh() is not trivial.
							// Fortunately a simple mouseover straightens things out for now.
//							RoutePanel.Refresh(); // in case of a warning this needs to happen ...
							break;

						case LinkSlotResult.LinkExists:
							// don't bother the user
							break;

						default:
							RoutesChangedCoordinator = true;
							node[(int)result].Destination = NodeSelected.Id;
							node[(int)result].Distance = CalculateLinkDistance(node, NodeSelected);
							break;
					}
					goto case ConnectNodesType.OneWay; // fallthrough

				case ConnectNodesType.OneWay:
					switch (result = GetOpenLinkSlot(NodeSelected, node.Id))
					{
						case LinkSlotResult.AllSlotsUsed:
							MessageBox.Show(
										this,
										"Source node could not be linked to the destination node."
											+ " Its link-slots are full.",
										" Warning",
										MessageBoxButtons.OK,
										MessageBoxIcon.Warning,
										MessageBoxDefaultButton.Button1,
										0);
							// TODO: the message leaves the RoutePanel drawn in an awkward state
							// but discovering where to call Refresh() is not trivial.
							// Fortunately a simple mouseover straightens things out for now.
//							RoutePanel.Refresh(); // in case of a warning this needs to happen ...
							break;

						case LinkSlotResult.LinkExists:
							// don't bother the user
							break;

						default:
							RoutesChangedCoordinator = true;
							NodeSelected[(int)result].Destination = node.Id;
							NodeSelected[(int)result].Distance = CalculateLinkDistance(NodeSelected, node);
							break;
					}
					break;
			}
		}

		/// <summary>
		/// Gets the first available link-slot for a given node.
		/// </summary>
		/// <param name="node">the node to check the link-slots of</param>
		/// <param name="dest">the id of the destination node</param>
		/// <returns>id of an available link-slot, or
		/// -1 if the link already exists
		/// -2 if there are no free slots</returns>
		private static LinkSlotResult GetOpenLinkSlot(RouteNode node, int dest)
		{
			// first check if a link to the destination-id already exists
			for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
			{
				if (node[slot].Destination == dest)
					return LinkSlotResult.LinkExists;
			}

			// then check for an open slot
			for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
			{
				if (node[slot].Destination == (byte)LinkType.NotUsed)
					return (LinkSlotResult)slot;
			}
			return LinkSlotResult.AllSlotsUsed;
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

			valid = valid && Clipboard.GetText().Split(NodeCopySeparator)[0] == NodeCopyPrefix;

			ObserverManager.RouteView   .Control     .btnPaste         .Enabled =
			ObserverManager.TopRouteView.ControlRoute.btnPaste         .Enabled = valid;
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
															_file.Routes[link.Destination]);
						distance = link.Distance.ToString(CultureInfo.InvariantCulture)
								 + GetDistanceArrow(slot);
						break;
				}

				switch (slot)
				{
					case 0: tbLink1Dist.Text = distance; break;
					case 1: tbLink2Dist.Text = distance; break;
					case 2: tbLink3Dist.Text = distance; break;
					case 3: tbLink4Dist.Text = distance; break;
					case 4: tbLink5Dist.Text = distance; break;
				}
			}

			int count = _file.Routes.Nodes.Count;
			for (var id = 0; id != count; ++id) // update distances of any links to the selected node ->
			{
				if (id != NodeSelected.Id) // NOTE: a node shall not link to itself.
				{
					var node = _file.Routes[id];

					for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
					{
						var link = node[slot];
						if (link.Destination == NodeSelected.Id)
							link.Distance = CalculateLinkDistance(
																node,
																NodeSelected);
					}
				}
			}
		}
		#endregion Methods (mouse-event helpers)


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

				if (_file.Descriptor.GroupType == GameType.Tftd)
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

				if (_file.Descriptor.GroupType == GameType.Tftd)
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

				int count = _file.Routes.Nodes.Count;
				for (byte id = 0; id != count; ++id)
				{
					if (id != NodeSelected.Id)
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

						default: // case 4
							cbTypL  = cbLink5UnitType;
							cbDest  = cbLink5Dest;
							tbDist  = tbLink5Dist;
							btnGo   = btnGoLink5;
							lblText = labelLink5;
							break;
					}

					link = NodeSelected[slot];

					cbTypL.SelectedItem = link.Type;
					btnGo.Enabled = link.isNodelink();

					dest = link.Destination;
					if (link.isUsed())
					{
						btnGo.Text = Go;
						tbDist.Text = Convert.ToString(
													link.Distance,
													CultureInfo.InvariantCulture)
									+ GetDistanceArrow(slot);

						if (link.isNodelink())
						{
							cbDest.SelectedItem = dest;

							if (RouteNodeCollection.IsNodeOutsideMapBounds(
																		_file.Routes[dest],
																		_file.MapSize.Cols,
																		_file.MapSize.Rows,
																		_file.MapSize.Levs))
							{
								lblText.ForeColor = Color.Chocolate;
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
			if (link.isNodelink())
			{
				var dest = _file.Routes[link.Destination];
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
				RoutesChangedCoordinator = true;
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
					RoutesChangedCoordinator = true;
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
				RoutesChangedCoordinator = true;
				NodeSelected.Spawn = (SpawnWeight)((Pterodactyl)cbSpawn.SelectedItem).O;

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
				RoutesChangedCoordinator = true;
				NodeSelected.Patrol = (PatrolPriority)((Pterodactyl)cbPatrol.SelectedItem).O;

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
				RoutesChangedCoordinator = true;
				NodeSelected.Attack = (AttackBase)((Pterodactyl)cbAttack.SelectedItem).O;

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
				RoutesChangedCoordinator = true;

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
															_file.Routes[link.Destination],
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
				textBox.Text = dist.ToString(CultureInfo.InvariantCulture)
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
				RoutesChangedCoordinator = true;

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
			var node  = _file.Routes[dest];

			if (!RouteNodeCollection.IsNodeOutsideMapBounds(
														node,
														_file.MapSize.Cols,
														_file.MapSize.Rows,
														_file.MapSize.Levs))
			{
				_ogId = NodeSelected.Id; // store the current nodeId for the og-button.

				ObserverManager.RouteView   .Control     .btnOg.Enabled =
				ObserverManager.TopRouteView.ControlRoute.btnOg.Enabled = true;

				SelectNode(dest);

				SpotGoDestination(slot); // highlight back to the startnode.
			}
			else
			{
				RouteCheckService.Base1_xy = MainViewF.Optionables.Base1_xy; // send the base1-count options to 'XCom' ->
				RouteCheckService.Base1_z  = MainViewF.Optionables.Base1_z;

				if (RouteCheckService.dialog_InvalidNode(_file, node) == DialogResult.Yes)
				{
					RoutesChangedCoordinator = true;

					if (RoutesInfo != null)
						RoutesInfo.DeleteNode(node);

					_file.Routes.DeleteNode(node);
					UpdateNodeInfo();
					// TODO: May need _pnlRoutes.Refresh()
				}
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
			var node = _file.Routes[id];
			var loc = new Point(node.Col, node.Row);

			if (node.Lev != _file.Level)
				_file.Level = node.Lev; // fire SelectLevel

			_file.Location = new MapLocation( // fire SelectLocation
										loc.X, loc.Y,
										_file.Level);

			MainViewOverlay.that.ProcessSelection(loc,loc);

			var args = new RoutePanelEventArgs(
											MouseButtons.Left,
											_file[loc.X, loc.Y],
											_file.Location);
			OnRoutePanelMouseDown(null, args);

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
							var node = _file.Routes[dest];
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
			if (_ogId < _file.Routes.Nodes.Count) // in case nodes were deleted.
			{
				if (NodeSelected == null || _ogId != NodeSelected.Id)
					SelectNode(_ogId);
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
			if (_ogId < _file.Routes.Nodes.Count) // in case nodes were deleted.
			{
				var node = _file.Routes[_ogId];
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
			switch (e.KeyData)
			{
				case Keys.Delete:
					OnDeleteClick(null, null);
					break;

				case Keys.Control | Keys.S:
					MainViewF.that.OnSaveRoutesClick(null, EventArgs.Empty);
					break;

				case Keys.Control | Keys.X:
					_asterisk = true;
					 OnCopyClick(  null, EventArgs.Empty);
					 OnDeleteClick(null, EventArgs.Empty);
					 _asterisk = false;
					 break;

				case Keys.Control | Keys.C:
					 OnCopyClick( null, EventArgs.Empty);
					 break;

				 case Keys.Control | Keys.V:
					 OnPasteClick(null, EventArgs.Empty);
					 break;
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
										CultureInfo.InvariantCulture,
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
				ShowErrorDialog("A node must be selected.");
		}

		private void OnPasteClick(object sender, EventArgs e)
		{
			RoutePanel.Select();

			if (NodeSelected != null) // TODO: auto-create a new node
			{
				var nodeData = Clipboard.GetText().Split(NodeCopySeparator);
				if (nodeData[0] == NodeCopyPrefix)
				{
					RoutesChangedCoordinator = true;

					var invariant = CultureInfo.InvariantCulture;

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

					ShowErrorDialog("The data on the clipboard is not a node.");
				}
			}
			else
				ShowErrorDialog("A node must be selected.");
		}

		private void OnDeleteClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				RoutesChangedCoordinator = true;

				if (RoutesInfo != null)
					RoutesInfo.DeleteNode(NodeSelected);

				_file[NodeSelected.Col,
					  NodeSelected.Row,
					  NodeSelected.Lev].Node = null;
				_file.Routes.DeleteNode(NodeSelected);

				ObserverManager.RouteView   .Control     .DeselectNode();
				ObserverManager.TopRouteView.ControlRoute.DeselectNode();

				UpdateNodeInfo();

				gbNodeData.Enabled =
				gbLinkData.Enabled = false;

				// TODO: check if the Og-button should be disabled when a node gets deleted or cut.

				RefreshControls();
			}			
			else if (!_asterisk)
				ShowErrorDialog("A node must be selected.");
		}

		private void ShowErrorDialog(string error)
		{
			MessageBox.Show(
						this,
						error,
						" Err..",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion Events (node edit)


		/// <summary>
		/// Deselects any currently selected node.
		/// </summary>
		/// <param name="clearloc"></param>
		internal void DeselectNode(bool clearloc = true)
		{
			NodeSelected = null;

			if (clearloc) // basically the node is deleted from RouteView itself
			{
				RoutePanel.Select();
			}
			else // basically a location is selected in MainView or TopView (even if it's still the node's location)
			{
				UpdateNodeInfo();

				gbNodeData.Enabled =
				gbLinkData.Enabled = false;

				RoutePanel.Invalidate();
			}

			tsmiClearLinkData.Enabled = false;
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

					tsb_connect0.Checked =
				alt.tsb_connect0.Checked =
					tsb_connect1.Checked =
				alt.tsb_connect1.Checked =
					tsb_connect2.Checked =
				alt.tsb_connect2.Checked = false;

					tsb_connect0.Image =
				alt.tsb_connect0.Image = Properties.Resources.connect_0;
					tsb_connect1.Image =
				alt.tsb_connect1.Image = Properties.Resources.connect_1;
					tsb_connect2.Image =
				alt.tsb_connect2.Image = Properties.Resources.connect_2;


				if (tsb == tsb_connect0)
				{
					_conType = ConnectNodesType.None;

						tsb_connect0.Checked =
					alt.tsb_connect0.Checked = true;

						tsb_connect0.Image =
					alt.tsb_connect0.Image = Properties.Resources.connect_0_red;
				}
				else if (tsb == tsb_connect1)
				{
					_conType = ConnectNodesType.OneWay;

						tsb_connect1.Checked =
					alt.tsb_connect1.Checked = true;

						tsb_connect1.Image =
					alt.tsb_connect1.Image = Properties.Resources.connect_1_blue;
				}
				else //if (tsb == tsb_connect2)
				{
					_conType = ConnectNodesType.TwoWay;

						tsb_connect2.Checked =
					alt.tsb_connect2.Checked = true;
						tsb_connect2.Image =
					alt.tsb_connect2.Image = Properties.Resources.connect_2_green;
				}
			}
			RoutePanel.Select();
		}


		private string _lastExportDirectory;

		private void OnExportClick(object sender, EventArgs e)
		{
			if (_file != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title      = "Export Route file ...";
					sfd.Filter     = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
					sfd.DefaultExt = GlobalsXC.RouteExt;
					sfd.FileName   = _file.Descriptor.Label;

					if (!Directory.Exists(_lastExportDirectory))
					{
						string path = Path.Combine(MapBase.Descriptor.Basepath, GlobalsXC.RoutesDir);
						if (Directory.Exists(path))
							sfd.InitialDirectory = path;
					}
					else
						sfd.InitialDirectory = _lastExportDirectory;


					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						_lastExportDirectory = Path.GetDirectoryName(sfd.FileName);
						_file.Routes.ExportRoutes(sfd.FileName);
					}
				}
			}
		}

		private string _lastImportDirectory;

		private void OnImportClick(object sender, EventArgs e)
		{
			if (_file != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title      = "Import Route file ...";
					ofd.Filter     = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
//					ofd.DefaultExt = GlobalsXC.RouteExt;
					ofd.FileName   = _file.Descriptor.Label + GlobalsXC.RouteExt;

					if (!Directory.Exists(_lastImportDirectory))
					{
						string dir = Path.Combine(MapBase.Descriptor.Basepath, GlobalsXC.RoutesDir);
						if (Directory.Exists(dir))
							ofd.InitialDirectory = dir;
					}
					else
						ofd.InitialDirectory = _lastImportDirectory;


					if (ofd.ShowDialog(this) == DialogResult.OK)
					{
						_lastImportDirectory = Path.GetDirectoryName(ofd.FileName);

						var routes = new RouteNodeCollection(ofd.FileName);
						if (!routes.Fail)
						{
							RoutesChangedCoordinator = true;

							ObserverManager.RouteView   .Control     .DeselectNode();
							ObserverManager.TopRouteView.ControlRoute.DeselectNode();

							_file.ClearRouteNodes();
							_file.Routes = routes;
							_file.SetupRouteNodes();

							RouteCheckService.Base1_xy = MainViewF.Optionables.Base1_xy; // send the base1-count options to 'XCom' ->
							RouteCheckService.Base1_z  = MainViewF.Optionables.Base1_z;

							if (RouteCheckService.CheckNodeBounds(_file) == DialogResult.Yes)
							{
								foreach (RouteNode node in RouteCheckService.Invalids)
									_file.Routes.DeleteNode(node);
							}

							UpdateNodeInfo(); // not sure is necessary ...
							RefreshPanels();

							if (RoutesInfo != null)
								RoutesInfo.Initialize(_file);
						}
					}
				}
			}
		}


		private void OnEditOpening(object sender, EventArgs e)
		{
			tsmi_LowerNode.Enabled = (NodeSelected != null && NodeSelected.Lev != _file.MapSize.Levs - 1);
			tsmi_RaiseNode.Enabled = (NodeSelected != null && NodeSelected.Lev != 0);
		}


		private void OnNodeRaise(object sender, EventArgs e)
		{
			Dragnode = NodeSelected;

			var args = new RoutePanelEventArgs(
											MouseButtons.None,
											_file[Dragnode.Col,
												  Dragnode.Row,
												  Dragnode.Lev - 1],
											new MapLocation(
														Dragnode.Col,
														Dragnode.Row,
														Dragnode.Lev - 1));
			OnRoutePanelMouseUp(null, args);

			SelectNode(NodeSelected.Id);
		}

		private void OnNodeLower(object sender, EventArgs e)
		{
			Dragnode = NodeSelected;

			var args = new RoutePanelEventArgs(
											MouseButtons.None,
											_file[Dragnode.Col,
												  Dragnode.Row,
												  Dragnode.Lev + 1],
											new MapLocation(
														Dragnode.Col,
														Dragnode.Row,
														Dragnode.Lev + 1));
			OnRoutePanelMouseUp(null, args);

			SelectNode(NodeSelected.Id);
		}

		/// <summary>
		/// Handler for menuitem that sets all NodeRanks to Civilian/Scout.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAllNodesRank0Click(object sender, EventArgs e)
		{
			string rank;
			if (_file.Descriptor.GroupType == GameType.Tftd)
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
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button2,
							0) == DialogResult.Yes)
			{
				int changed = 0;
				foreach (RouteNode node in _file.Routes)
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
					RoutesChangedCoordinator = true;
					UpdateNodeInfo();

					MessageBox.Show(
								this,
								changed + " nodes were changed.",
								" All nodes rank 0",
								MessageBoxButtons.OK,
								MessageBoxIcon.None,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
					MessageBox.Show(
								this,
								"All nodes are already rank 0.",
								" All nodes rank 0",
								MessageBoxButtons.OK,
								MessageBoxIcon.None,
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
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button2,
								0) == DialogResult.Yes)
				{
					RoutesChangedCoordinator = true;

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

			int count = _file.Routes.Nodes.Count;
			for (var id = 0; id != count; ++id)
			{
				node = _file.Routes[id];

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
													_file.Routes[link.Destination]);
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
				RoutesChangedCoordinator = true;
				info = String.Format(
								CultureInfo.CurrentCulture,
								"{0} link{1} updated.",
								changed,
								(changed == 1) ? " has been" : "s have been");

				UpdateNodeInfo();
			}
			else
			{
				info = String.Format(
								CultureInfo.CurrentCulture,
								"All link distances are already correct.");
			}

			MessageBox.Show(
						this,
						info,
						" Link distances updated",
						MessageBoxButtons.OK,
						MessageBoxIcon.None,
						MessageBoxDefaultButton.Button1,
						0);
		}

		/// <summary>
		/// Handler for menuitem that checks if any node's location is outside
		/// the dimensions of the Map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTestPositionsClick(object sender, EventArgs e)
		{
			RouteCheckService.Base1_xy = MainViewF.Optionables.Base1_xy; // send the base1-count options to 'XCom' ->
			RouteCheckService.Base1_z  = MainViewF.Optionables.Base1_z;

			if (RouteCheckService.CheckNodeBounds(_file, true) == DialogResult.Yes)
			{
				RoutesChangedCoordinator = true;

				foreach (RouteNode node in RouteCheckService.Invalids)
				{
					if (RoutesInfo != null)
						RoutesInfo.DeleteNode(node);

					_file.Routes.DeleteNode(node);
				}

				UpdateNodeInfo();
			}
		}

		/// <summary>
		/// Handler for menuitem that checks if any node's rank is beyond the
		/// array of the combobox. See also RouteNodeCollection.cTor
		/// TODO: Consolidate these checks to RouteCheckService.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTestNodeRanksClick(object sender, EventArgs e)
		{
			var invalids = new List<byte>();
			foreach (RouteNode node in _file.Routes)
			{
				if (node.OobRank != (byte)0)
					invalids.Add(node.Id);
			}

			string info, title;
			MessageBoxIcon icon;

			if (invalids.Count != 0)
			{
				icon  = MessageBoxIcon.Warning;
				title = " Warning";
				info  = String.Format(
									CultureInfo.CurrentCulture,
									"The following route-{0} an invalid NodeRank.{1}",
									(invalids.Count == 1) ? "node has"
														  : "nodes have",
									Environment.NewLine);

				foreach (byte id in invalids)
					info += Environment.NewLine + id;
			}
			else
			{
				icon  = MessageBoxIcon.None;
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
				RoutesInfo = new RoutesInfo(_file);
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

		private void OnSaveClick(object sender, EventArgs e)
		{
			MainViewF.that.OnSaveRoutesClick(null, EventArgs.Empty);
			RoutePanel.Select();
		}
		#endregion Events


		#region Options
		/// <summary>
		/// Selects one of the connector-buttons when either the RouteView or
		/// the TopRouteView toplevel form(s) is first shown. The connector-type
		/// is determined by user-options.
		/// </summary>
		internal void ActivateConnector()
		{
			if (!_connectoractivated)
			{
				_connectoractivated = true;

				ToolStripButton tsb;
				switch (Optionables.StartConnector)
				{
					default: tsb = tsb_connect0; break; // case 0
					case  1: tsb = tsb_connect1; break;
					case  2: tsb = tsb_connect2; break;
				}
				OnConnectTypeClicked(tsb, EventArgs.Empty); // that handles both RouteViews.
			}
		}


		/// <summary>
		/// Loads default options for RouteView in TopRouteView screens.
		/// </summary>
		internal protected override void LoadControlDefaultOptions()
		{
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
					_foptions.Text = "RouteView Options";

					OptionsManager.Views.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!MainViewF.Quit)
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


		#region Update UI (static)
		internal static void RefreshControls()
		{
			ObserverManager.RouteView   .Control     .Refresh();
			ObserverManager.TopRouteView.ControlRoute.Refresh();
		}

		private static void InvalidateControls()
		{
			ObserverManager.RouteView   .Control     .Invalidate();
			ObserverManager.TopRouteView.ControlRoute.Invalidate();
		}

		private static void RefreshPanels()
		{
			ObserverManager.RouteView   .Control     .RoutePanel.Refresh();
			ObserverManager.TopRouteView.ControlRoute.RoutePanel.Refresh();
		}

		private static void InvalidatePanels()
		{
			ObserverManager.RouteView   .Control     .RoutePanel.Invalidate();
			ObserverManager.TopRouteView.ControlRoute.RoutePanel.Invalidate();
		}

		private static void UpdateNodeInfo()
		{
			ObserverManager.RouteView   .Control     .UpdateNodeInformation();
			ObserverManager.TopRouteView.ControlRoute.UpdateNodeInformation();
		}

		internal static void Invalidator()
		{
			InvalidatePanels();
			UpdateNodeInfo();
		}

//		internal static void InvalidateRoutePanels() // just do InvalidatePanels()
//		{
//			ObserverManager.RouteView   .Control     ._pnlRoutes.Invalidate();
//			ObserverManager.TopRouteView.ControlRoute._pnlRoutes.Invalidate();
//		}
		#endregion Update UI (static)
	}
}
