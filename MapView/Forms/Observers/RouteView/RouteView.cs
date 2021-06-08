using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Does all the heavy-lifting/node-manipulations in RouteView and
	/// TopRouteView(Route). <see cref="RouteControlParent"/> also handles mouse
	/// events.
	/// </summary>
	/// <remarks>Static objects in this class are shared between the two viewers
	/// - otherwise RouteView and TopRouteView(Route) instantiate separately.</remarks>
	internal sealed partial class RouteView
		:
			ObserverControl // UserControl, IObserver/ObserverControl
	{
		#region Enums
		private enum ConnectNodesType
		{ None, OneWay, TwoWay }

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

		private bool _loadingInfo;

		/// <summary>
		/// Used by <see cref="UpdateNodeInformation"/>.
		/// </summary>
		private readonly List<object> _linksList = new List<object>(); // List req'd.

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
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl"/>.
		/// </summary>
		[Browsable(false)]
		public override MapFile MapFile
		{
			set // TODO: check RouteView/TopRouteView(Route)
			{
				base.MapFile = value;	// TODO: reduce count of pointers to the MapFile.
										// It should be stored in ObserverControl and that's basically it.
				DeselectNode();

				RouteControl.SetMapfile(base.MapFile);

				if (base.MapFile != null)
				{
					co_Rank.Items.Clear();

					if (base.MapFile.Descriptor.GroupType == GameType.Tftd)
						co_Rank.Items.AddRange(RouteNodes.RankTftd);
					else
						co_Rank.Items.AddRange(RouteNodes.RankUfo);

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
				RouteControlParent.SetNodeSelected(_nodeSelected = value);

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
		internal RouteControl RouteControl
		{ get; private set; }

		/// <summary>
		/// Sets the 'MapFile.RoutesChanged' flag. This is only an intermediary
		/// that shows "routes changed" in RouteView; the real 'RoutesChanged'
		/// flag is stored in <see cref="T:XCom.MapFile"/>. reasons.
		/// </summary>
		private bool RoutesChanged // TODO: static
		{
			set { bu_Save.Enabled = (MapFile.RoutesChanged = value); }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiates the RouteView viewer and its components/controls.
		/// </summary>
		/// <remarks><see cref="RouteViewForm"/> and <see cref="TopRouteViewForm"/>
		/// will each invoke and maintain their own instantiations.</remarks>
		public RouteView()
		{
			Optionables = new RouteViewOptionables(this);

			InitializeComponent();

			RouteControl = new RouteControl();
			RouteControl.RouteControlMouseDownEvent += OnRouteControlMouseDown;
			RouteControl.RouteControlMouseUpEvent   += OnRouteControlMouseUp;
			RouteControl.MouseMove                  += OnRouteControlMouseMove;
			RouteControl.MouseLeave                 += OnRouteControlMouseLeave;
			RouteControl.KeyDown                    += OnRouteControlKeyDown;
			_pnlRoutes.Controls.Add(RouteControl);

			// node data ->
			var unitTypes = new object[]
			{
				UnitType.Any,
				UnitType.Small,
				UnitType.Large,
				UnitType.FlyingSmall,
				UnitType.FlyingLarge
			};
			co_Type.Items.AddRange(unitTypes);

			co_Spawn .Items.AddRange(RouteNodes.Spawn);
			co_Patrol.Items.AddRange(RouteNodes.Patrol);
			co_Attack.Items.AddRange(RouteNodes.Attack);

			// link data ->
			co_Link1UnitType.Items.AddRange(unitTypes);
			co_Link2UnitType.Items.AddRange(unitTypes);
			co_Link3UnitType.Items.AddRange(unitTypes);
			co_Link4UnitType.Items.AddRange(unitTypes);
			co_Link5UnitType.Items.AddRange(unitTypes);

			// TODO: change the distance textboxes to labels.

			DeselectNode();
		}
		#endregion cTor


		#region Events (override) inherited from IObserver/ObserverControl
		/// <summary>
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl"/>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>This will fire twice whenever the location changes: once by
		/// RouteView and again by TopRouteView(Route). This is desired behavior
		/// since it updates the selected-location in both viewers.
		/// 
		/// 
		/// A route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to (or press [Enter] when the tile is
		/// selected). This is a design decision that allows the selected node
		/// to stay selected while other tiles get highlighted.</remarks>
		public override void OnLocationSelectedObserver(LocationSelectedArgs args)
		{
			PrintSelectedInfo();
		}

		/// <summary>
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl"/>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>This will fire twice whenever the location changes: once by
		/// RouteView and again by TopRouteView(Route). However only the viewer
		/// that the mousecursor is currently in should have the
		/// location-string's color updated; the condition to allow that update
		/// is (RouteControl._col != -1).
		/// 
		/// 
		/// The route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to (or press [Enter] when the tile is
		/// selected). This is a design decision that allows the selected node
		/// to stay selected while other tiles get highlighted.</remarks>
		public override void OnLevelSelectedObserver(LevelSelectedArgs args)
		{
			//LogFile.WriteLine("RouteView.OnLevelSelectedObserver() " + Tag);

			if (RouteControl._col != -1) // find the Control that the mousecursor is in (if either)
			{
				//LogFile.WriteLine(". do overinfo");

				Color color; int id;

				RouteNode node = MapFile.GetTile(RouteControl._col,
												 RouteControl._row).Node;
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

				la_Over.ForeColor = color;

				PrintOverInfo(id);
			}

			PrintSelectedInfo();

			Refresh(); // required to force the other RouteView panel to redraw.
//			InvalidatePanels();
		}
		#endregion Events (override) inherited from IObserver/ObserverControl


		#region Methods (print TileData)
		/// <summary>
		/// Clears the selected tile-info text when another Map loads.
		/// </summary>
		internal void ClearOveredInfo()
		{
			la_Over.Text = String.Empty;
		}

		/// <summary>
		/// Clears the selected tile-info text when another Map loads.
		/// </summary>
		internal void ClearSelectedInfo()
		{
			la_Selected.Text = String.Empty;
		}

		/// <summary>
		/// Updates the selected node's textcolor when the Option changes.
		/// </summary>
		/// <remarks>Called by Options only.</remarks>
		internal static void SetInfoSelectedColor()
		{
			Color color;
			if (NodeSelected == null)
			{
				color = SystemColors.ControlText;
			}
			else
				color = Optionables.NodeSelectedColor;

			ObserverManager.RouteView   .Control     .la_Selected.ForeColor =
			ObserverManager.TopRouteView.ControlRoute.la_Selected.ForeColor = color;
		}

		/// <summary>
		/// Updates the overed node's textcolor when the Option changes.
		/// </summary>
		/// <remarks>Called by Options only.</remarks>
		internal void SetInfoOverColor()
		{
			if (RouteControl._col != -1) // find the Control that the mousecursor is in (if either)
			{
				Color color;

				RouteNode node = MapFile.GetTile(RouteControl._col,
												 RouteControl._row).Node;
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

				la_Over.ForeColor = color; // set color in the overed viewer only.
			}
		}

		/// <summary>
		/// Updates the overed node's info and textcolor when a node is dragged.
		/// </summary>
		internal void SetInfoOverText()
		{
			if (RouteControl._col != -1) // find the Control that the mousecursor is in (if either)
			{
				Color color; int id;

				RouteNode node = MapFile.GetTile(RouteControl._col,
												 RouteControl._row).Node;
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

				la_Over.ForeColor = color;

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

			if (RouteControl._col != -1)
			{
				info += Environment.NewLine;

				int c = RouteControl._col;
				int r = RouteControl._row;
				int l = MapFile.Levs - MapFile.Level;

				if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
				if (!MainViewF.Optionables.Base1_z) { --l; }

				info += "c " + c + "  r " + r + "  L " + l;
			}

			ObserverManager.RouteView   .Control     .la_Over.Text =
			ObserverManager.TopRouteView.ControlRoute.la_Over.Text = info;
		}

		/// <summary>
		/// Prints mouseovered Go button link-node info to the TileData
		/// groupbox.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="isog"></param>
		private void PrintGoInfo(RouteNode node, bool isog)
		{
			Color color;
			if (node.Spawn == SpawnWeight.None)
			{
				color = Optionables.NodeColor;
			}
			else
				color = Optionables.NodeSpawnColor;

			la_Over.ForeColor = color;

			string info;

			if (isog) info = "Og ";
			else      info = "Link ";

			info += node.Id + Environment.NewLine;

			int c = node.Col;
			int r = node.Row;
			int l = MapFile.Levs - node.Lev;

			if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
			if (!MainViewF.Optionables.Base1_z) { --l; }

			info += "c " + c + "  r " + r + "  L " + l;

			la_Over.Text = info; // only this RouteView.
		}


		/// <summary>
		/// Prints the currently selected tile-info to the TileData groupbox.
		/// NOTE: The displayed level is inverted here.
		/// </summary>
		internal void PrintSelectedInfo()
		{
			if (MainViewOverlay.that.FirstClick)
			{
				string info;
				int level;
				Color color;

				if (NodeSelected == null)
				{
					info  = String.Empty;
					level = MapFile.Level;
					color = SystemColors.ControlText;
				}
				else
				{
					info  = "Selected " + NodeSelected.Id;
					level = NodeSelected.Lev;
					color = Optionables.NodeSelectedColor;
				}

				info += Environment.NewLine;

				int c = MapFile.Location.Col;
				int r = MapFile.Location.Row;
				int l = MapFile.Levs - level;

				if (MainViewF.Optionables.Base1_xy) { ++c; ++r; }
				if (!MainViewF.Optionables.Base1_z) { --l; }

				info += "c " + c + "  r " + r + "  L " + l;

				la_Selected.ForeColor = color;
				la_Selected.Text = info;
				la_Selected.Refresh(); // fast update.
			}
		}
		#endregion Methods (print TileData)


		#region Events (mouse-events for RouteControl)
		/// <summary>
		/// Handler that selects a node on LMB, creates and/or connects nodes on
		/// RMB.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnRouteControlMouseDown(object sender, RouteControlEventArgs args)
		{
			bool updateinfo = false;

			RouteNode node = args.Tile.Node;

			if (NodeSelected == null) // a node is not currently selected ->
			{
				if ((NodeSelected = node) == null
					&& args.MouseButton == MouseButtons.Right)
				{
					RoutesChangedCoordinator = true;
					NodeSelected = MapFile.AddRouteNode(args.Location);
					InvalidatePanels(); // not sure why but that's needed after adding the "ReduceDraws" option
				}
				updateinfo = (NodeSelected != null);
			}
			else if (node == null) // a node is already selected but there's not a node on the current tile ->
			{
				if (args.MouseButton == MouseButtons.Right)
				{
					RoutesChangedCoordinator = true;
					node = MapFile.AddRouteNode(args.Location);
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
		private void OnRouteControlMouseUp(object sender, RouteControlEventArgs args)
		{
			if (Dragnode != null)
			{
				if (args.Tile.Node == null)
				{
					RoutesChangedCoordinator = true;

					MapFile.GetTile(Dragnode.Col, // clear the node from the previous tile
									Dragnode.Row,
									Dragnode.Lev).Node = null;

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
					using (var f = new Infobox(
											"Err...",
											"Cannot move node onto another node.",
											null,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
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
		internal void OnRouteControlMouseMove(object sender, MouseEventArgs args)
		{
			RouteControl.SetOver(new Point(args.X, args.Y));

			SetInfoOverText();

			ObserverManager.RouteView   .Control     .la_Over.Refresh(); // fast update. // NOTE: Only RouteView not TopRouteView(Route)
			ObserverManager.TopRouteView.ControlRoute.la_Over.Refresh(); // fast update. // wants fast update. go figure

			// TODO: if (MainView.Optionables.ShowOverlay)
			RefreshPanels(); // fast update. (else the InfoOverlay on RouteView but not TopRouteView(Route) gets sticky - go figur)
		}

		/// <summary>
		/// Handler that hides the info-overlay when the mouse leaves this
		/// control. See also RouteControlParent.t1_Tick().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRouteControlMouseLeave(object sender, EventArgs e)
		{
			// TODO: perhaps fire RouteControlParent.OnMouseMove()
			RouteControl._col = -1;
			RefreshPanels();
		}
		#endregion Events (mouse-events for RouteControl)


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
							using (var f = new Infobox(
													"Warning",
													Infobox.SplitString("Destination node could not be linked"
															+ " to the source node. Its link-slots are full."),
													null,
													InfoboxType.Warn))
							{
								f.ShowDialog(this);
							}
							// TODO: the message leaves the RouteControl drawn in an awkward state
							// but discovering where to call Refresh() is not trivial.
							// Fortunately a simple mouseover straightens things out for now.
//							RouteControl.Refresh(); // in case of a warning this needs to happen ...
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
							using (var f = new Infobox(
													"Warning",
													Infobox.SplitString("Source node could not be linked to the"
															+ " destination node. Its link-slots are full."),
													null,
													InfoboxType.Warn))
							{
								f.ShowDialog(this);
							}
							// TODO: the message leaves the RouteControl drawn in an awkward state
							// but discovering where to call Refresh() is not trivial.
							// Fortunately a simple mouseover straightens things out for now.
//							RouteControl.Refresh(); // in case of a warning this needs to happen ...
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


		/// <summary>
		/// Enables/disables the node-editor buttons as well as the ClearLinks
		/// it.
		/// </summary>
		private static void EnableEditButtons()
		{
			bool valid = (NodeSelected != null);

			ObserverManager.RouteView   .Control     .tsmi_ClearLinks.Enabled =
			ObserverManager.TopRouteView.ControlRoute.tsmi_ClearLinks.Enabled =

			ObserverManager.RouteView   .Control     .bu_Cut         .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Cut         .Enabled =

			ObserverManager.RouteView   .Control     .bu_Copy        .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Copy        .Enabled =

			ObserverManager.RouteView   .Control     .bu_Delete      .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Delete      .Enabled = valid;

			valid = valid && Clipboard.GetText().Split(NodeCopySeparator)[0] == NodeCopyPrefix;

			ObserverManager.RouteView   .Control     .bu_Paste       .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Paste       .Enabled = valid;
		}


		/// <summary>
		/// Updates distances to and from the currently selected node.
		/// </summary>
		/// <remarks>NodeSelected must be valid before call.</remarks>
		private void UpdateLinkDistances()
		{
			for (int slot = 0; slot != RouteNode.LinkSlots; ++slot) // update distances to selected node's linked nodes ->
			{
				string distance;

				Link link = NodeSelected[slot];
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
						distance = link.Distance + GetDistanceArrow(slot);
						break;
				}

				switch (slot)
				{
					case 0: tb_Link1Dist.Text = distance; break;
					case 1: tb_Link2Dist.Text = distance; break;
					case 2: tb_Link3Dist.Text = distance; break;
					case 3: tb_Link4Dist.Text = distance; break;
					case 4: tb_Link5Dist.Text = distance; break;
				}
			}

			int count = MapFile.Routes.Nodes.Count;
			for (var id = 0; id != count; ++id) // update distances of any links to the selected node ->
			{
				if (id != NodeSelected.Id) // NOTE: a node shall not link to itself.
				{
					var node = MapFile.Routes[id];

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
				bu_Cut       .Enabled =
				bu_Copy      .Enabled =
				bu_Paste     .Enabled =
				bu_Delete    .Enabled =

				gb_NodeData  .Enabled =
				gb_LinkData  .Enabled =
				gb_NodeEditor.Enabled =

				bu_GoLink1   .Enabled =
				bu_GoLink2   .Enabled =
				bu_GoLink3   .Enabled =
				bu_GoLink4   .Enabled =
				bu_GoLink5   .Enabled = false;

				bu_GoLink1.Text =
				bu_GoLink2.Text =
				bu_GoLink3.Text =
				bu_GoLink4.Text =
				bu_GoLink5.Text = String.Empty;


				co_Type.SelectedItem = UnitType.Any;

				if (MapFile.Descriptor.GroupType == GameType.Tftd)
					co_Rank.SelectedItem = RouteNodes.RankTftd[0];	//(byte)NodeRankTftd.CivScout
				else
					co_Rank.SelectedItem = RouteNodes.RankUfo [0];	//(byte)NodeRankUfo.CivScout

				co_Spawn .SelectedItem = RouteNodes.Spawn [0];		//(byte)SpawnWeight.None
				co_Patrol.SelectedItem = RouteNodes.Patrol[0];		//(byte)PatrolPriority.Zero
				co_Attack.SelectedItem = RouteNodes.Attack[0];		//(byte)AttackBase.Zero

				co_Link1Dest.SelectedItem = // TODO: figure out why these show blank and not "NotUsed"
				co_Link2Dest.SelectedItem = // when the app loads its very first Map.
				co_Link3Dest.SelectedItem =
				co_Link4Dest.SelectedItem =
				co_Link5Dest.SelectedItem = LinkType.NotUsed;

				co_Link1UnitType.SelectedItem =
				co_Link2UnitType.SelectedItem =
				co_Link3UnitType.SelectedItem =
				co_Link4UnitType.SelectedItem =
				co_Link5UnitType.SelectedItem = UnitType.Any;

				tb_Link1Dist.Text =
				tb_Link2Dist.Text =
				tb_Link3Dist.Text =
				tb_Link4Dist.Text =
				tb_Link5Dist.Text = String.Empty;

				la_Link1.ForeColor =
				la_Link2.ForeColor =
				la_Link3.ForeColor =
				la_Link4.ForeColor =
				la_Link5.ForeColor = SystemColors.ControlText;
			}
			else // selected node is valid ->
			{
				gb_NodeData  .Enabled =
				gb_LinkData  .Enabled =
				gb_NodeEditor.Enabled = true;

				co_Type.SelectedItem = NodeSelected.Unit;

				if (MapFile.Descriptor.GroupType == GameType.Tftd)
					co_Rank.SelectedItem = RouteNodes.RankTftd[NodeSelected.Rank];
				else
					co_Rank.SelectedItem = RouteNodes.RankUfo [NodeSelected.Rank];

				co_Spawn .SelectedItem = RouteNodes.Spawn [(byte)NodeSelected.Spawn];
				co_Patrol.SelectedItem = RouteNodes.Patrol[(byte)NodeSelected.Patrol];
				co_Attack.SelectedItem = RouteNodes.Attack[(byte)NodeSelected.Attack];

				co_Link1Dest.Items.Clear();
				co_Link2Dest.Items.Clear();
				co_Link3Dest.Items.Clear();
				co_Link4Dest.Items.Clear();
				co_Link5Dest.Items.Clear();

				_linksList.Clear();

				int total = MapFile.Routes.Nodes.Count;
				for (byte id = 0; id != total; ++id)
				{
					if (id != NodeSelected.Id)
						_linksList.Add(id);			// <- add all linkable (ie. other) nodes
				}
				_linksList.AddRange(_linkTypes);	// <- add the four compass-points + link-not-used.

				object[] linkListArray = _linksList.ToArray();

				co_Link1Dest.Items.AddRange(linkListArray);
				co_Link2Dest.Items.AddRange(linkListArray);
				co_Link3Dest.Items.AddRange(linkListArray);
				co_Link4Dest.Items.AddRange(linkListArray);
				co_Link5Dest.Items.AddRange(linkListArray);


				ComboBox co_TypL, co_Dest;
				TextBox tb_Dist;
				Button bu_Go;
				Label la_Text;

				Link link;
				byte dest;

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
				{
					switch (slot)
					{
						case 0:
							co_TypL = co_Link1UnitType;
							co_Dest = co_Link1Dest;
							tb_Dist = tb_Link1Dist;
							bu_Go   = bu_GoLink1;
							la_Text = la_Link1;
							break;

						case 1:
							co_TypL = co_Link2UnitType;
							co_Dest = co_Link2Dest;
							tb_Dist = tb_Link2Dist;
							bu_Go   = bu_GoLink2;
							la_Text = la_Link2;
							break;

						case 2:
							co_TypL = co_Link3UnitType;
							co_Dest = co_Link3Dest;
							tb_Dist = tb_Link3Dist;
							bu_Go   = bu_GoLink3;
							la_Text = la_Link3;
							break;

						case 3:
							co_TypL = co_Link4UnitType;
							co_Dest = co_Link4Dest;
							tb_Dist = tb_Link4Dist;
							bu_Go   = bu_GoLink4;
							la_Text = la_Link4;
							break;

						default: // case 4
							co_TypL = co_Link5UnitType;
							co_Dest = co_Link5Dest;
							tb_Dist = tb_Link5Dist;
							bu_Go   = bu_GoLink5;
							la_Text = la_Link5;
							break;
					}

					link = NodeSelected[slot];

					co_TypL.SelectedItem = link.Unit;
					bu_Go.Enabled = link.IsNodelink();

					dest = link.Destination;
					if (link.IsUsed())
					{
						bu_Go  .Text = Go;
						tb_Dist.Text = link.Distance + GetDistanceArrow(slot);

						if (link.IsNodelink())
						{
							co_Dest.SelectedItem = dest;

							if (RouteNodes.OutsideMapBounds(
														MapFile.Routes[dest],
														MapFile.Cols,
														MapFile.Rows,
														MapFile.Levs))
							{
								la_Text.ForeColor = Color.Chocolate;
							}
							else
								la_Text.ForeColor = SystemColors.ControlText;
						}
						else
						{
							co_Dest.SelectedItem = (LinkType)dest;
							la_Text.ForeColor = SystemColors.ControlText;
						}
					}
					else
					{
						bu_Go  .Text =
						tb_Dist.Text = String.Empty;
						co_Dest.SelectedItem = (LinkType)dest;
						la_Text.ForeColor = SystemColors.ControlText;
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
			if (link.IsNodelink())
			{
				RouteNode dest = MapFile.Routes[link.Destination];
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
		/// <summary>
		/// Handles unit-type changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnUnitTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RoutesChangedCoordinator = true;
				NodeSelected.Unit = (UnitType)co_Type.SelectedItem;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.co_Type.SelectedIndex = co_Type.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.co_Type.SelectedIndex = co_Type.SelectedIndex;
			}
		}

		private bool _bypassRankChanged;
		/// <summary>
		/// Handles node-rank changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNodeRankSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo && !_bypassRankChanged)
			{
				if (co_Rank.SelectedIndex == 9)
				{
					_bypassRankChanged = true;	// because this funct is going to fire again immediately
					co_Rank.SelectedIndex = NodeSelected.Rank;
					_bypassRankChanged = false;	// and I don't want the RoutesChanged flagged.
				}
				else
				{
					RoutesChangedCoordinator = true;
					NodeSelected.Rank = (byte)co_Rank.SelectedIndex;
//					NodeSelected.Rank = (byte)((Pterodactyl)co_Rank.SelectedItem).Case; // <- MapView1-type code.

					if (NodeSelected.Spawn != SpawnWeight.None)
					{
						if (RoutesInfo != null)
							RoutesInfo.UpdateNoderank(_curNoderank, NodeSelected.Rank);

						_curNoderank = NodeSelected.Rank;
					}

					NodeSelected.OobRank = (byte)0;

					if (Tag as String == "ROUTE")
						ObserverManager.TopRouteView.ControlRoute.co_Rank.SelectedIndex = co_Rank.SelectedIndex;
					else //if (Tag == "TOPROUTE")
						ObserverManager.RouteView.Control.co_Rank.SelectedIndex = co_Rank.SelectedIndex;
				}
			}
		}

		/// <summary>
		/// Handles spawn-weight changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSpawnWeightSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RoutesChangedCoordinator = true;
				NodeSelected.Spawn = (SpawnWeight)((Pterodactyl)co_Spawn.SelectedItem).O;

				if (RoutesInfo != null)
					RoutesInfo.ChangedSpawnweight(_curSpawnweight, NodeSelected.Spawn, NodeSelected.Rank);

				_curSpawnweight = NodeSelected.Spawn;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.co_Spawn.SelectedIndex = co_Spawn.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.co_Spawn.SelectedIndex = co_Spawn.SelectedIndex;

				RefreshControls(); // update the importance bar
			}
		}

		/// <summary>
		/// Handles patrol-priority changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPatrolPrioritySelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RoutesChangedCoordinator = true;
				NodeSelected.Patrol = (PatrolPriority)((Pterodactyl)co_Patrol.SelectedItem).O;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.co_Patrol.SelectedIndex = co_Patrol.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.co_Patrol.SelectedIndex = co_Patrol.SelectedIndex;

				RefreshControls(); // update the importance bar
			}
		}

		/// <summary>
		/// Handles base-attack changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBaseAttackSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loadingInfo)
			{
				RoutesChangedCoordinator = true;
				NodeSelected.Attack = (AttackBase)((Pterodactyl)co_Attack.SelectedItem).O;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.co_Attack.SelectedIndex = co_Attack.SelectedIndex;
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.co_Attack.SelectedIndex = co_Attack.SelectedIndex;
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
				Button bu_Go;

				var co = sender as ComboBox;
				if (co == co_Link1Dest)
				{
					slot  = 0;
					tb    = tb_Link1Dist;
					bu_Go = bu_GoLink1;
				}
				else if (co == co_Link2Dest)
				{
					slot  = 1;
					tb    = tb_Link2Dist;
					bu_Go = bu_GoLink2;
				}
				else if (co == co_Link3Dest)
				{
					slot  = 2;
					tb    = tb_Link3Dist;
					bu_Go = bu_GoLink3;
				}
				else if (co == co_Link4Dest)
				{
					slot  = 3;
					tb    = tb_Link4Dist;
					bu_Go = bu_GoLink4;
				}
				else //if (co == co_Link5Dest)
				{
					slot  = 4;
					tb    = tb_Link5Dist;
					bu_Go = bu_GoLink5;
				}

				var dest = co.SelectedItem as byte?; // check for id or compass pt/not used.
				if (!dest.HasValue)
					dest = (byte?)(co.SelectedItem as LinkType?);

				bool enable, text;

				var link = NodeSelected[slot];
				switch (link.Destination = dest.Value)
				{
					case Link.NotUsed:
						link.Unit = UnitType.Any;

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

				bu_Go.Enabled = enable;
				bu_Go.Text = text ? Go : String.Empty;

				RouteControl.SetSpot(new Point(-1,-1));

				if (Tag as String == "ROUTE")
				{
					ObserverManager.TopRouteView.ControlRoute.TransferDestination(
																				slot,
																				co.SelectedIndex,
																				tb.Text,
																				enable,
																				bu_Go.Text);
				}
				else //if (Tag == "TOPROUTE")
				{
					ObserverManager.RouteView.Control.TransferDestination(
																		slot,
																		co.SelectedIndex,
																		tb.Text,
																		enable,
																		bu_Go.Text);
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
			ComboBox co_Dest;
			TextBox tb_Dist;
			Button bu_Go;

			switch (slot)
			{
				case 0:
					co_Dest = co_Link1Dest;
					tb_Dist = tb_Link1Dist;
					bu_Go   = bu_GoLink1;
					break;

				case 1:
					co_Dest = co_Link2Dest;
					tb_Dist = tb_Link2Dist;
					bu_Go   = bu_GoLink2;
					break;

				case 2:
					co_Dest = co_Link3Dest;
					tb_Dist = tb_Link3Dist;
					bu_Go   = bu_GoLink3;
					break;

				case 3:
					co_Dest = co_Link4Dest;
					tb_Dist = tb_Link4Dist;
					bu_Go   = bu_GoLink4;
					break;

				default: //case 4:
					co_Dest = co_Link5Dest;
					tb_Dist = tb_Link5Dist;
					bu_Go   = bu_GoLink5;
					break;
			}

			co_Dest.SelectedIndex = dest;
			tb_Dist.Text          = dist;
			bu_Go.Enabled         = enable;
			bu_Go.Text            = text;
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
				textBox.Text = dist + GetDistanceArrow(slot);

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

				var co = sender as ComboBox;
				if      (co == co_Link1UnitType) slot = 0;
				else if (co == co_Link2UnitType) slot = 1;
				else if (co == co_Link3UnitType) slot = 2;
				else if (co == co_Link4UnitType) slot = 3;
				else                             slot = 4; // co_Link5UnitType

				NodeSelected[slot].Unit = (UnitType)co.SelectedItem;

				if (Tag as String == "ROUTE")
					ObserverManager.TopRouteView.ControlRoute.TransferUnitType(slot, co.SelectedIndex);
				else //if (Tag == "TOPROUTE")
					ObserverManager.RouteView.Control.TransferUnitType(slot, co.SelectedIndex);
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
			ComboBox co_UnitType;
			switch (slot)
			{
				case 0:  co_UnitType = co_Link1UnitType; break;
				case 1:  co_UnitType = co_Link2UnitType; break;
				case 2:  co_UnitType = co_Link3UnitType; break;
				case 3:  co_UnitType = co_Link4UnitType; break;
				default: co_UnitType = co_Link5UnitType; break; //case 4
			}
			co_UnitType.SelectedIndex = type;
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

			var bu = sender as Button;
			if      (bu == bu_GoLink1) slot = 0;
			else if (bu == bu_GoLink2) slot = 1;
			else if (bu == bu_GoLink3) slot = 2;
			else if (bu == bu_GoLink4) slot = 3;
			else                       slot = 4; // bu == bu_GoLink5

			byte dest = NodeSelected[slot].Destination;
			RouteNode node = MapFile.Routes[dest];

			if (RouteNodes.OutsideMapBounds(
										node,
										MapFile.Cols,
										MapFile.Rows,
										MapFile.Levs))
			{
				RouteCheckService.SetBase1_xy(MainViewF.Optionables.Base1_xy); // send the base1-count options to 'XCom' ->
				RouteCheckService.SetBase1_z( MainViewF.Optionables.Base1_z);

				if (RouteCheckService.dialog_InvalidNode(MapFile, node) == DialogResult.Yes)
				{
					RoutesChangedCoordinator = true;

					if (RoutesInfo != null)
						RoutesInfo.DeleteNode(node);

					MapFile.Routes.DeleteNode(node);
					UpdateNodeInfo();
					// TODO: May need _pnlRoutes.Refresh()
				}
			}
			else
			{
				_ogId = NodeSelected.Id; // store the current nodeId for the og-button.

				ObserverManager.RouteView   .Control     .bu_Og.Enabled =
				ObserverManager.TopRouteView.ControlRoute.bu_Og.Enabled = true;

				SelectNode(dest);

				SpotDestination(slot); // highlight back to the startnode.
			}

			RouteControl.Select();
		}

		/// <summary>
		/// Deals with the ramifications of a Go or Og click.
		/// </summary>
		/// <param name="id"></param>
		/// <remarks>Any changes that are done here regarding node-selection
		/// should be reflected in RouteControlParent.OnMouseDown() since that
		/// is an alternate way to select a tile/node.</remarks>
		private void SelectNode(int id)
		{
			RouteNode node = MapFile.Routes[id];
			var loc = new Point(node.Col, node.Row);

			if (node.Lev != MapFile.Level)
				MapFile.Level = node.Lev; // fire LevelSelected

			MapFile.Location = new MapLocation( // fire LocationSelected
											loc.X, loc.Y,
											MapFile.Level);

			MainViewOverlay.that.ProcessSelection(loc,loc);

			var args = new RouteControlEventArgs(
											MouseButtons.Left,
											MapFile.GetTile(loc.X, loc.Y),
											MapFile.Location);
			OnRouteControlMouseDown(null, args);

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
			SpotDestination(slot); // TODO: RouteView/TopRouteView(Route)

			byte dest = NodeSelected[slot].Destination;
			if (dest != Link.NotUsed)
			{
				PrintGoInfo(MapFile.Routes[dest], false); // TODO: ensure that nodes are listed in RouteNodes in consecutive order ...
			}
		}

		/// <summary>
		/// Sets the highlighted destination link-line and node if applicable.
		/// </summary>
		/// <param name="slot">the link-slot whose destination should get
		/// highlighted</param>
		private void SpotDestination(int slot)
		{
			if (NodeSelected != null && NodeSelected[slot] != null) // safety: Go-btn should not be enabled unless a node is selected.
			{
				byte dest = NodeSelected[slot].Destination;
				if (dest != Link.NotUsed)
				{
					int c,r;
					switch (dest)
					{
						case Link.ExitNorth: c = r = -2; break;
						case Link.ExitEast:  c = r = -3; break;
						case Link.ExitSouth: c = r = -4; break;
						case Link.ExitWest:  c = r = -5; break;
	
						default:
							RouteNode node = MapFile.Routes[dest];
							c = node.Col;
							r = node.Row;
							break;
					}
	
					RouteControl.SetSpot(new Point(c,r)); // TODO: static - RouteView/TopRouteView(Route)

					RouteControl.Refresh();
//					RefreshControls();
				}
			}
		}

		/// <summary>
		/// Clears the spot position when the cursor leaves a link-control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkMouseLeave(object sender, EventArgs e)
		{
			RouteControl.SetSpot(new Point(-1,-1));

			RouteControl.Refresh();
//			RefreshControls();
		}

		/// <summary>
		/// Selects the appropriate route-node when the Og button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOgClick(object sender, EventArgs e)
		{
			if (_ogId < MapFile.Routes.Nodes.Count) // in case nodes were deleted.
			{
				if (NodeSelected == null || _ogId != NodeSelected.Id)
					SelectNode(_ogId);
			}
			else
			{
				ObserverManager.RouteView   .Control     .bu_Og.Enabled =
				ObserverManager.TopRouteView.ControlRoute.bu_Og.Enabled = false;
			}

			RouteControl.Select();
		}

		/// <summary>
		/// Spots a route-node when the cursor enters the Og button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOgMouseEnter(object sender, EventArgs e)
		{
			if (_ogId < MapFile.Routes.Nodes.Count) // in case nodes were deleted.
			{
				RouteNode node = MapFile.Routes[_ogId];
				RouteControl.SetSpot(new Point(node.Col, node.Row));

				RouteControl.Refresh();
//				RefreshControls();

				PrintGoInfo(node, true);
			}
		}

		/// <summary>
		/// Disables the Og button when a Map gets loaded.
		/// </summary>
		internal static void DisableOg()
		{
			ObserverManager.RouteView   .Control     .bu_Og.Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Og.Enabled = false;
		}
		#endregion Events (LinkData)


		#region Events (node edit)
		/// <summary>
		/// Prevents two error-dialogs from showing if a key-cut is underway.
		/// </summary>
		private bool _asterisk;

		/// <summary>
		/// Handles keyboard input.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Navigation keys are handled by 'KeyPreview' at the form
		/// level.</remarks>
		private void OnRouteControlKeyDown(object sender, KeyEventArgs e)
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
					 OnCopyClick(null, EventArgs.Empty);
					 break;

				 case Keys.Control | Keys.V:
					 OnPasteClick(null, EventArgs.Empty);
					 break;
			}
		}

		/// <summary>
		/// Handles an edit-cut click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCutClick(object sender, EventArgs e)
		{
			OnCopyClick(  null, EventArgs.Empty);
			OnDeleteClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// Handles an edit-copy click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCopyClick(object sender, EventArgs e)
		{
			RouteControl.Select();

			if (NodeSelected != null)
			{
				ObserverManager.RouteView   .Control     .bu_Paste.Enabled =
				ObserverManager.TopRouteView.ControlRoute.bu_Paste.Enabled = true;

				string nodeCopy = NodeCopyPrefix         + NodeCopySeparator
								+ co_Type  .SelectedIndex + NodeCopySeparator
								+ co_Patrol.SelectedIndex + NodeCopySeparator
								+ co_Attack.SelectedIndex + NodeCopySeparator
								+ co_Rank  .SelectedIndex + NodeCopySeparator
								+ co_Spawn .SelectedIndex;

				// TODO: include Link info ... perhaps.
				// But re-assigning the link node-ids would be difficult, since
				// those nodes could have be deleted, etc.
				Clipboard.SetText(nodeCopy);
			}
			else
				ShowError("A node must be selected.");
		}

		/// <summary>
		/// Handles an edit-paste click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPasteClick(object sender, EventArgs e)
		{
			RouteControl.Select();

			if (NodeSelected != null) // TODO: auto-create a new node
			{
				var nodeData = Clipboard.GetText().Split(NodeCopySeparator);
				if (nodeData[0] == NodeCopyPrefix)
				{
					RoutesChangedCoordinator = true;

					co_Type  .SelectedIndex = Int32.Parse(nodeData[1]);
					co_Patrol.SelectedIndex = Int32.Parse(nodeData[2]);
					co_Attack.SelectedIndex = Int32.Parse(nodeData[3]);
					co_Rank  .SelectedIndex = Int32.Parse(nodeData[4]);
					co_Spawn .SelectedIndex = Int32.Parse(nodeData[5]);

					// TODO: include Link info ... perhaps.
					// But re-assigning the link node-ids would be difficult, since
					// those nodes could have be deleted, etc.
				}
				else // non-node data is on the clipboard.
				{
					ObserverManager.RouteView   .Control     .bu_Paste.Enabled =
					ObserverManager.TopRouteView.ControlRoute.bu_Paste.Enabled = false;

					ShowError("The data on the clipboard is not a node.");
				}
			}
			else
				ShowError("A node must be selected.");
		}

		/// <summary>
		/// Handles an edit-delete click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				RoutesChangedCoordinator = true;

				if (RoutesInfo != null)
					RoutesInfo.DeleteNode(NodeSelected);

				MapFile.GetTile(NodeSelected.Col,
								NodeSelected.Row,
								NodeSelected.Lev).Node = null;
				MapFile.Routes.DeleteNode(NodeSelected);

				ObserverManager.RouteView   .Control     .DeselectNode();
				ObserverManager.TopRouteView.ControlRoute.DeselectNode();

				UpdateNodeInfo();

				gb_NodeData.Enabled =
				gb_LinkData.Enabled = false;

				// TODO: check if the Og-button should be disabled when a node gets deleted or cut.

				RefreshControls();
			}			
			else if (!_asterisk)
				ShowError("A node must be selected.");
		}

		/// <summary>
		/// Wrapper for <see cref="Infobox"/>.
		/// </summary>
		/// <param name="head">the error string to show</param>
		private void ShowError(string head)
		{
			using (var f = new Infobox(
									"Error",
									head,
									null,
									InfoboxType.Error))
			{
				f.ShowDialog(this);
			}
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
				RouteControl.Select();
			}
			else // basically a location is selected in MainView or TopView (even if it's still the node's location)
			{
				UpdateNodeInfo();

				gb_NodeData.Enabled =
				gb_LinkData.Enabled = false;

				RouteControl.Invalidate();
			}

			tsmi_ClearLinks.Enabled = false;
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
			RouteControl.Select();
		}


		private string _lastExportDirectory;

		/// <summary>
		/// Exports nodes to a Routes-file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExportClick(object sender, EventArgs e)
		{
			if (MapFile != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title      = "Export Route file ...";
					sfd.Filter     = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
					sfd.DefaultExt = GlobalsXC.RouteExt;
					sfd.FileName   = MapFile.Descriptor.Label;

					if (!Directory.Exists(_lastExportDirectory))
					{
						string path = Path.Combine(MapFile.Descriptor.Basepath, GlobalsXC.RoutesDir);
						if (Directory.Exists(path))
							sfd.InitialDirectory = path;
					}
					else
						sfd.InitialDirectory = _lastExportDirectory;


					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						_lastExportDirectory = Path.GetDirectoryName(sfd.FileName);
						MapFile.Routes.ExportRoutes(sfd.FileName);
					}
				}
			}
		}

		private string _lastImportDirectory;

		/// <summary>
		/// Imports nodes from a Routes-file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnImportClick(object sender, EventArgs e)
		{
			if (MapFile != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title      = "Import Route file ...";
					ofd.Filter     = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
//					ofd.DefaultExt = GlobalsXC.RouteExt;
					ofd.FileName   = MapFile.Descriptor.Label + GlobalsXC.RouteExt;

					if (!Directory.Exists(_lastImportDirectory))
					{
						string dir = Path.Combine(MapFile.Descriptor.Basepath, GlobalsXC.RoutesDir);
						if (Directory.Exists(dir))
							ofd.InitialDirectory = dir;
					}
					else
						ofd.InitialDirectory = _lastImportDirectory;


					if (ofd.ShowDialog(this) == DialogResult.OK)
					{
						_lastImportDirectory = Path.GetDirectoryName(ofd.FileName);

						var routes = new RouteNodes(ofd.FileName);
						if (!routes.Fail)
						{
							RoutesChangedCoordinator = true;

							ObserverManager.RouteView   .Control     .DeselectNode();
							ObserverManager.TopRouteView.ControlRoute.DeselectNode();

							MapFile.ClearRouteNodes();
							MapFile.Routes = routes;
							MapFile.SetupRouteNodes();

							RouteCheckService.SetBase1_xy(MainViewF.Optionables.Base1_xy); // send the base1-count options to 'XCom' ->
							RouteCheckService.SetBase1_z( MainViewF.Optionables.Base1_z);

							if (RouteCheckService.CheckNodeBounds(MapFile) == DialogResult.Yes)
							{
								foreach (RouteNode node in RouteCheckService.Invalids)
									MapFile.Routes.DeleteNode(node);
							}

							UpdateNodeInfo(); // not sure is necessary ...
							RefreshPanels();

							if (RoutesInfo != null)
								RoutesInfo.Initialize(MapFile);
						}
					}
				}
			}
		}


		/// <summary>
		/// Dis/enables its on the Edit menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditOpening(object sender, EventArgs e)
		{
			tsmi_LowerNode.Enabled = (NodeSelected != null && NodeSelected.Lev != MapFile.Levs - 1);
			tsmi_RaiseNode.Enabled = (NodeSelected != null && NodeSelected.Lev != 0);
		}


		/// <summary>
		/// Raises a node 1 level.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNodeRaise(object sender, EventArgs e)
		{
			Dragnode = NodeSelected;

			var args = new RouteControlEventArgs(
											MouseButtons.None,
											MapFile.GetTile(Dragnode.Col,
															Dragnode.Row,
															Dragnode.Lev - 1),
											new MapLocation(
														Dragnode.Col,
														Dragnode.Row,
														Dragnode.Lev - 1));
			OnRouteControlMouseUp(null, args);

			SelectNode(NodeSelected.Id);
		}

		/// <summary>
		/// Lowers a node 1 level.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNodeLower(object sender, EventArgs e)
		{
			Dragnode = NodeSelected;

			var args = new RouteControlEventArgs(
											MouseButtons.None,
											MapFile.GetTile(Dragnode.Col,
															Dragnode.Row,
															Dragnode.Lev + 1),
											new MapLocation(
														Dragnode.Col,
														Dragnode.Row,
														Dragnode.Lev + 1));
			OnRouteControlMouseUp(null, args);

			SelectNode(NodeSelected.Id);
		}

		/// <summary>
		/// Handler for menuitem that sets all NodeRanks to Civilian/Scout.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRank0Click(object sender, EventArgs e)
		{
			string rank;
			if (MapFile.Descriptor.GroupType == GameType.Tftd)
				rank = ((Pterodactyl)RouteNodes.RankTftd[0]).ToString();
			else
				rank = ((Pterodactyl)RouteNodes.RankUfo [0]).ToString();

			using (var f = new Infobox(
									"Warning",
									"Are you sure you want to change all node ranks to " + rank + " ...",
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
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

					string head;
					if (changed != 0)
					{
						RoutesChangedCoordinator = true;
						UpdateNodeInfo();

						head = changed + ((changed == 1) ? " node was" : " nodes were") + " changed.";
					}
					else
						head = "All nodes are already rank 0.";

					using (var f1 = new Infobox("All nodes rank 0", head))
						f1.ShowDialog(this);
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that clears all link-data of the currently
		/// selected node.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClearLinksClick(object sender, EventArgs e)
		{
			if (NodeSelected != null)
			{
				using (var f = new Infobox(
										"Warning",
										"Are you sure you want to clear the selected node's Link data ...",
										null,
										InfoboxType.Warn,
										InfoboxButtons.CancelOkay))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						RoutesChangedCoordinator = true;

						for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
						{
							NodeSelected[slot].Destination = Link.NotUsed;
							NodeSelected[slot].Distance = 0;

							NodeSelected[slot].Unit = UnitType.Any;
						}

						UpdateNodeInfo();
						RefreshControls();
					}
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that updates all link distances in the RMP.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRecalcDistClick(object sender, EventArgs e)
		{
			RouteNode node;
			Link link;
			byte dist;
			int changed = 0;

			int total = MapFile.Routes.Nodes.Count;
			for (var id = 0; id != total; ++id)
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

			string head;
			if (changed != 0)
			{
				RoutesChangedCoordinator = true;
				UpdateNodeInfo();

				head = ((changed == 1) ? " link has" : " links have") + " been updated.";
			}
			else
				head = "All link distances are already correct.";

			using (var f = new Infobox("Link distances updated", head))
				f.ShowDialog(this);
		}

		/// <summary>
		/// Handler for menuitem that checks if any node's location is outside
		/// the dimensions of the Map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTestPositionsClick(object sender, EventArgs e)
		{
			RouteCheckService.SetBase1_xy(MainViewF.Optionables.Base1_xy); // send the base1-count options to 'XCom' ->
			RouteCheckService.SetBase1_z( MainViewF.Optionables.Base1_z);

			if (RouteCheckService.CheckNodeBounds(MapFile, true) == DialogResult.Yes)
			{
				RoutesChangedCoordinator = true;

				foreach (RouteNode node in RouteCheckService.Invalids)
				{
					if (RoutesInfo != null)
						RoutesInfo.DeleteNode(node);

					MapFile.Routes.DeleteNode(node);
				}

				UpdateNodeInfo();
			}
		}

		/// <summary>
		/// Handler for menuitem that checks if any node's rank is beyond the
		/// array of the combobox. See also RouteNodes..cTor.
		/// TODO: Consolidate these checks to RouteCheckService.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTestNoderanksClick(object sender, EventArgs e)
		{
			var invalids = new List<byte>();
			foreach (RouteNode node in MapFile.Routes)
			{
				if (node.OobRank != (byte)0)
					invalids.Add(node.Id);
			}

			InfoboxType bt;
			string title, head, copyable;

			if (invalids.Count != 0)
			{
				bt    = InfoboxType.Warn;
				title = "Warning";
				head  = "The following " + ((invalids.Count == 1) ? "node has" : "nodes have")
					  + " an invalid NodeRank.";

				copyable = String.Empty;
				foreach (byte id in invalids)
				{
					if (copyable.Length != 0) copyable += Environment.NewLine;
					copyable += id;
				}
			}
			else
			{
				bt       = InfoboxType.Info;
				title    = "Good stuff, Magister Ludi";
				head     = "There are no invalid NodeRanks detected.";
				copyable = null;
			}

			using (var f = new Infobox(
									title,
									head,
									copyable,
									bt))
			{
				f.ShowDialog(this);
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
//				RoutesInfo.Focus();
//				RoutesInfo.Select();
//				RoutesInfo.BringToFront();
//				RoutesInfo.TopMost = true;
//				RoutesInfo.TopMost = false;
			}
		}

		/// <summary>
		/// Saves the routes-file when the Save button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveClick(object sender, EventArgs e)
		{
			MainViewF.that.OnSaveRoutesClick(null, EventArgs.Empty);
			RouteControl.Select();
		}
		#endregion Events


		#region Options
		/// <summary>
		/// Selects one of the connector-buttons when either the RouteView or
		/// the TopRouteView toplevel form(s) is first shown.
		/// </summary>
		/// <remarks>The connector-type is determined by user-options.</remarks>
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
			//LogFile.WriteLine("RouteView.LoadControlDefaultOptions()");
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
											OptionableType.RouteView);
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

		/// <summary>
		/// Checks or unchecks the Options button.
		/// </summary>
		/// <param name="checked"></param>
		private static void setOptionsChecked(bool @checked)
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
			ObserverManager.RouteView   .Control     .RouteControl.Refresh();
			ObserverManager.TopRouteView.ControlRoute.RouteControl.Refresh();
		}

		internal static void InvalidatePanels()
		{
			ObserverManager.RouteView   .Control     .RouteControl.Invalidate();
			ObserverManager.TopRouteView.ControlRoute.RouteControl.Invalidate();
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
		#endregion Update UI (static)
	}
}
