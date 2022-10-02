using System;
using System.Collections.Generic;
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
	/// TopRouteView(Route). <c><see cref="RouteControlParent"/></c> also
	/// handles mouse events.
	/// </summary>
	/// <remarks>Static objects in this class are shared between the two viewers
	/// - otherwise RouteView and TopRouteView(Route) instantiate separately.</remarks>
	internal sealed partial class RouteView
		:
			UserControl
	{
		#region Enums
		private enum ConnectNodes
		{ None, OneWay, TwoWay }

		private enum LinkSlotResult
		{
			LinkExists   = -1,
			AllSlotsUsed = -2
		}
		#endregion Enums


		#region Fields (static)
		private static ConnectNodes _conType = ConnectNodes.None; // safety - shall be set by Optionables.LoadDefaults()

		private const string Go = "go";

		internal static RouteNode Dragnode;

		/// <summary>
		/// Stores the current node-id when a Go button is clicked. Used to
		/// re-select previous node(s) - which is not equivalent to "Back" (if
		/// there were a Back button) since only nodes that were selected by the
		/// Go button get pushed onto the stack.
		/// </summary>
		private static readonly Stack<int> _ogIds = new Stack<int>();

		private static        byte _selRank;
		private static SpawnWeight _selWeight;

		private static bool _connectoractivated;

		/// <summary>
		/// A <c><see cref="CopyNodeData"/></c> struct that holds copied node
		/// data.
		/// </summary>
		/// <remarks>Is <c>static</c> so that it remains consistent between
		/// <c>RouteView</c> and <c>TopRouteView(Route)</c>.</remarks>
		private static CopyNodeData _copynodedata;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;

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

		/// <summary>
		/// <c>true</c> if this <c>RouteView</c> is in
		/// <c><see cref="TopRouteViewForm"/></c>.
		/// </summary>
		internal bool isToproute;
		#endregion Fields


		#region Properties (static)
		internal static readonly Options Options = new Options();

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
		internal static RouteViewOptionables Optionables = new RouteViewOptionables();


		private static RouteNode _nodeSelected;
		/// <summary>
		/// A <c><see cref="RouteNode"/></c> that is currently selected.
		/// </summary>
		/// <remarks>Use the setter to set
		/// <c><see cref="RouteControlParent"></see>.NodeSelected</c>.</remarks>
		internal static RouteNode NodeSelected
		{
			get { return _nodeSelected; }
			set
			{
				RouteControlParent.SetNodeSelected(_nodeSelected = value);

				if (_nodeSelected != null) // for SpawnInfo ->
				{
					_selRank   = _nodeSelected.Rank;
					_selWeight = _nodeSelected.Spawn;
				}
			}
		}

		/// <summary>
		/// Coordinates the <c><see cref="RoutesChanged"/></c> flag between
		/// RouteView and TopRouteView(Route).
		/// </summary>
		internal static bool RoutesChangedCoordinator
		{
			set
			{
				ObserverManager.RouteView   .Control     .RoutesChanged =
				ObserverManager.TopRouteView.ControlRoute.RoutesChanged = value;
			}
		}

		static bool _ghostnodes;
		/// <summary>
		/// <c>true</c> to render nonspawn nodes in a ghosted color.
		/// </summary>
		internal static bool GhostNodesCoordinator
		{
			get { return _ghostnodes; }
			private set
			{
				ObserverManager.RouteView   .Control     .tsmi_GhostNodes.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_GhostNodes.Checked = (_ghostnodes = value);
			}
		}
		#endregion Properties (static)


		#region Properties
		internal RouteControl RouteControl
		{ get; private set; }

		/// <summary>
		/// Sets the <c><see cref="MapFile"/>.RoutesChanged</c> flag. This is
		/// only an intermediary that shows "routes changed" in
		/// <c>RouteView</c>; the real <c>RoutesChanged</c> flag is stored in
		/// <c>MapFile</c>. reasons.
		/// </summary>
		/// <seealso cref="RoutesChangedCoordinator"><c>RoutesChangedCoordinator</c></seealso>
		private bool RoutesChanged
		{
			set { bu_Save.Enabled = (_file.RoutesChanged = value); }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiates this <c>RouteView</c> control.
		/// </summary>
		/// <remarks><c><see cref="RouteViewForm"/></c> and
		/// <c><see cref="TopRouteViewForm"/></c> will each create and maintain
		/// their own instantiations.</remarks>
		public RouteView()
		{
			InitializeComponent();

			_copynodedata.unittype = -1; // '_copynodedata' has not been filled w/ valid node data.

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

			DeselectNode();
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets <c><see cref="_file"/></c>.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		/// <remarks>I don't believe it is necessary to unsubscribe the handlers
		/// here from events in the old <c>MapFile</c>. The old <c>MapFile</c>
		/// held the references and it goes poof, which ought release these
		/// handlers and this <c>RouteView</c> from any further obligations.</remarks>
		internal void SetMapfile(MapFile file)
		{
			if (_file != null)
			{
				_file.LocationSelected -= OnLocationSelectedObserver;
				_file.LevelSelected    -= OnLevelSelectedObserver;
			}

			DeselectNode();

			_ogIds.Clear();
			EnableOgButton(false);

			if ((_file = file) != null)
			{
				_file.LocationSelected += OnLocationSelectedObserver;
				_file.LevelSelected    += OnLevelSelectedObserver;

				co_Rank.Items.Clear();

				if (_file.Descriptor.GroupType == GroupType.Tftd)
				{
					co_Rank.Items.AddRange(RouteNodes.RankTftd);

					tsmi_Noderank0.Text = "highlight " + RouteNodes.civscout;
					tsmi_Noderank1.Text = "highlight " + RouteNodes.xcom;
					tsmi_Noderank2.Text = "highlight " + RouteNodes.soldier;
					tsmi_Noderank3.Text = "highlight " + RouteNodes.squadldr;
					tsmi_Noderank4.Text = "highlight " + RouteNodes.lc;
					tsmi_Noderank5.Text = "highlight " + RouteNodes.medicTftD;
					tsmi_Noderank6.Text = "highlight " + RouteNodes.ter1;
					tsmi_Noderank7.Text = "highlight " + RouteNodes.techie;
					tsmi_Noderank8.Text = "highlight " + RouteNodes.ter2;

					la_ColorRank0.Text = RouteNodes.civscout;
					la_ColorRank1.Text = RouteNodes.xcom;
					la_ColorRank2.Text = RouteNodes.soldier;
					la_ColorRank3.Text = RouteNodes.squadldr;
					la_ColorRank4.Text = RouteNodes.lc;
					la_ColorRank5.Text = RouteNodes.medicTftD;
					la_ColorRank6.Text = RouteNodes.ter1;
					la_ColorRank7.Text = RouteNodes.techie;
					la_ColorRank8.Text = RouteNodes.ter2;
				}
				else
				{
					co_Rank.Items.AddRange(RouteNodes.RankUfo);

					tsmi_Noderank0.Text = "highlight " + RouteNodes.civscout;
					tsmi_Noderank1.Text = "highlight " + RouteNodes.xcom;
					tsmi_Noderank2.Text = "highlight " + RouteNodes.soldier;
					tsmi_Noderank3.Text = "highlight " + RouteNodes.navigator;
					tsmi_Noderank4.Text = "highlight " + RouteNodes.lc;
					tsmi_Noderank5.Text = "highlight " + RouteNodes.engineer;
					tsmi_Noderank6.Text = "highlight " + RouteNodes.ter1;
					tsmi_Noderank7.Text = "highlight " + RouteNodes.medic;
					tsmi_Noderank8.Text = "highlight " + RouteNodes.ter2;

					la_ColorRank0.Text = RouteNodes.civscout;
					la_ColorRank1.Text = RouteNodes.xcom;
					la_ColorRank2.Text = RouteNodes.soldier;
					la_ColorRank3.Text = RouteNodes.navigator;
					la_ColorRank4.Text = RouteNodes.lc;
					la_ColorRank5.Text = RouteNodes.engineer;
					la_ColorRank6.Text = RouteNodes.ter1;
					la_ColorRank7.Text = RouteNodes.medic;
					la_ColorRank8.Text = RouteNodes.ter2;
				}

				UpdateNodeInformation();
			}
			else
			{
				la_ColorRank0.Text = "rank 0";
				la_ColorRank1.Text = "rank 1";
				la_ColorRank2.Text = "rank 2";
				la_ColorRank3.Text = "rank 3";
				la_ColorRank4.Text = "rank 4";
				la_ColorRank5.Text = "rank 5";
				la_ColorRank6.Text = "rank 6";
				la_ColorRank7.Text = "rank 7";
				la_ColorRank8.Text = "rank 8";
			}

			tsb_x2.Checked = false;

			RouteControl.SetMapFile(_file);
		}

		/// <summary>
		/// Dis/enables the <c>ToolStrip</c>.
		/// </summary>
		/// <param name="enable"><c>true</c> to enable</param>
		internal void Enable(bool enable)
		{
			ts_Main.Enabled = enable;
		}

		/// <summary>
		/// Updates node-info fields below the panel itself.
		/// </summary>
		private void UpdateNodeInformation()
		{
			SuspendLayout();

			PrintSelectedInfo();

			_loadingInfo = true;

			if (NodeSelected == null)
			{
				gb_NodeData  .Enabled =
				gb_LinkData  .Enabled =
				gb_NodeEditor.Enabled = false;

				bu_GoLink1.Text =
				bu_GoLink2.Text =
				bu_GoLink3.Text =
				bu_GoLink4.Text =
				bu_GoLink5.Text = String.Empty;


				co_Type.SelectedItem = UnitType.Any;

				if (_file.Descriptor.GroupType == GroupType.Tftd)
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

				la_Link1Dist.Text =
				la_Link2Dist.Text =
				la_Link3Dist.Text =
				la_Link4Dist.Text =
				la_Link5Dist.Text = String.Empty;
			}
			else // selected node is valid ->
			{
				gb_NodeData  .Enabled =
				gb_LinkData  .Enabled =
				gb_NodeEditor.Enabled = true;

				co_Type.SelectedItem = NodeSelected.Unit;

				if (_file.Descriptor.GroupType == GroupType.Tftd)
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

				int total = _file.Routes.Nodes.Count;
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


				ComboBox co_unit, co_dest;
				Label la_dist;
				Button bu_go;
				Label la_link;

				Link link;
				byte dest;

				for (int slot = 0; slot != RouteNode.LinkSlots; ++slot)
				{
					switch (slot)
					{
						case 0:
							co_unit = co_Link1UnitType;
							co_dest = co_Link1Dest;
							la_dist = la_Link1Dist;
							bu_go   = bu_GoLink1;
							la_link = la_Link1;
							break;

						case 1:
							co_unit = co_Link2UnitType;
							co_dest = co_Link2Dest;
							la_dist = la_Link2Dist;
							bu_go   = bu_GoLink2;
							la_link = la_Link2;
							break;

						case 2:
							co_unit = co_Link3UnitType;
							co_dest = co_Link3Dest;
							la_dist = la_Link3Dist;
							bu_go   = bu_GoLink3;
							la_link = la_Link3;
							break;

						case 3:
							co_unit = co_Link4UnitType;
							co_dest = co_Link4Dest;
							la_dist = la_Link4Dist;
							bu_go   = bu_GoLink4;
							la_link = la_Link4;
							break;

						default: // case 4
							co_unit = co_Link5UnitType;
							co_dest = co_Link5Dest;
							la_dist = la_Link5Dist;
							bu_go   = bu_GoLink5;
							la_link = la_Link5;
							break;
					}

					link = NodeSelected[slot];

					co_unit.SelectedItem = link.Unit;
					bu_go.Enabled = link.IsNodelink();

					if ((dest = link.Destination) != Link.NotUsed)
					{
						bu_go  .Text = Go;
						la_dist.Text = link.Distance + GetDistanceArrow(slot);

						if (link.IsNodelink())
						{
							co_dest.SelectedItem = dest;

							if (RouteCheckService.OutsideBounds(_file.Routes[dest], _file))
							{
								la_link.ForeColor = Optionables.FieldsForecolorHighlight;
							}
							else
								la_link.ForeColor = Optionables.FieldsForecolor;
						}
						else
						{
							co_dest.SelectedItem = (LinkType)dest;
							la_link.ForeColor = Optionables.FieldsForecolor;
						}
					}
					else
					{
						bu_go  .Text =
						la_dist.Text = String.Empty;
						co_dest.SelectedItem = (LinkType)dest;
						la_link.ForeColor = Optionables.FieldsForecolor;
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
				RouteNode dest = _file.Routes[link.Destination];
				if (dest != null) // safety.
				{
					if (NodeSelected.Lev > dest.Lev)
						return "\u2191"; // up arrow
	
					if (NodeSelected.Lev < dest.Lev)
						return "\u2193"; // down arrow
				}
			}
			return String.Empty;
		}
		#endregion Methods


		#region Events (override)
		/// <summary>
		/// Scrolls the z-axis.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Duplicated in <c><see cref="TopView"/></c>.</remarks>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			int delta;
			if (MainViewF.Optionables.InvertMousewheel)
				delta = -e.Delta;
			else
				delta =  e.Delta;

			int dir = MapFile.LEVEL_no;
			if      (delta < 0) dir = MapFile.LEVEL_Up;
			else if (delta > 0) dir = MapFile.LEVEL_Dn;
			_file.ChangeLevel(dir);

			ObserverManager.ToolFactory.EnableLevelers(_file.Level, _file.Levs);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handler for <c><see cref="MapFile"/>.LocationSelected</c>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>This will fire twice whenever the location changes: once by
		/// RouteView and again by TopRouteView(Route). This is desired behavior
		/// since it updates the selected-location in both viewers.
		/// <br/><br/>
		/// A route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to (or press [Enter] when the tile is
		/// selected). This is a design decision that allows the selected node
		/// to stay selected while other tiles get highlighted.</remarks>
		private void OnLocationSelectedObserver(LocationSelectedArgs args)
		{
			PrintSelectedInfo();
		}

		/// <summary>
		/// Handler for <c><see cref="MapFile"/>.LevelSelected</c>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>This will fire twice whenever the location changes: once by
		/// RouteView and again by TopRouteView(Route). However only the viewer
		/// that the mousecursor is currently in should have the
		/// location-string's color updated; the condition to allow that update
		/// is (RouteControl._col != -1).
		/// <br/><br/>
		/// The route-node at location will *not* be selected; only the
		/// tile is selected. To select a node the route-panel needs to be
		/// either clicked or keyboarded to (or press [Enter] when the tile is
		/// selected). This is a design decision that allows the selected node
		/// to stay selected while other tiles get highlighted.</remarks>
		private void OnLevelSelectedObserver(LevelSelectedArgs args)
		{
			//Logfile.Log("RouteView.OnLevelSelectedObserver() " + Tag);

			PrintOverInfo(RouteControl._col, RouteControl._row);
			PrintSelectedInfo();

			Refresh(); // req'd to force the other RouteView panel to redraw.
		}
		#endregion Events


		#region Methods (print TileData)
		/// <summary>
		/// Clears the currently selected tile-info's text in <c>RouteView</c>
		/// and <c>TopRouteView(Route)</c>.
		/// </summary>
		internal static void ClearSelectedInfo()
		{
			ObserverManager.RouteView   .Control     .la_Selected.Text =
			ObserverManager.TopRouteView.ControlRoute.la_Selected.Text = String.Empty;
		}

		/// <summary>
		/// Prints the currently selected tile-info to the TileData groupbox.
		/// </summary>
		/// <remarks>The displayed level is inverted.</remarks>
		internal void PrintSelectedInfo()
		{
			if (MainViewOverlay.that.FirstClick)
			{
				string info;
				int level;

				if (NodeSelected != null)
				{
					info  = "Selected " + NodeSelected.Id;
					level = NodeSelected.Lev;
				}
				else
				{
					info  = String.Empty;
					level = _file.Level;
				}

				info += Environment.NewLine
					  + Globals.GetLocationString(
											_file.Location.Col,
											_file.Location.Row,
											level,
											_file.Levs);

				la_Selected.Text = info;
				la_Selected.ForeColor = GetNodeColor(NodeSelected);
				la_Selected.Refresh(); // fast update.
			}
		}


		/// <summary>
		/// Clears mouseovered tile-info's text in <c>RouteView</c> and
		/// <c>TopRouteView(Route)</c>.
		/// </summary>
		internal static void ClearOverInfo()
		{
			ObserverManager.RouteView   .Control     .la_Over.Text =
			ObserverManager.TopRouteView.ControlRoute.la_Over.Text = String.Empty;
		}

		/// <summary>
		/// Prints mouseovered tile-info to the TileData groupbox.
		/// </summary>
		internal void PrintOverInfo(int c, int r)
		{
			if (c != -1)
			{
				string info;

				RouteNode node = _file.GetTile(c,r).Node;
				if (node != null) info = "Over " + node.Id;
				else              info = String.Empty;

				if (isToproute)
				{
					ObserverManager.TopRouteView.ControlRoute.la_Over.ForeColor = GetNodeColor(node);
					ObserverManager.RouteView   .Control     .la_Over.ForeColor = Optionables.FieldsForecolor;
				}
				else
				{
					ObserverManager.TopRouteView.ControlRoute.la_Over.ForeColor = Optionables.FieldsForecolor;
					ObserverManager.RouteView   .Control     .la_Over.ForeColor = GetNodeColor(node);
				}

				info += Environment.NewLine
					 + Globals.GetLocationString(c,r, _file.Level, _file.Levs);

				ObserverManager.RouteView   .Control     .la_Over.Text =
				ObserverManager.TopRouteView.ControlRoute.la_Over.Text = info;
			}
		}


		/// <summary>
		/// Prints mouseovered Go (or Og) button link-node info to the TileData
		/// groupbox.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="og"></param>
		private void PrintGoInfo(RouteNode node, bool og)
		{
			if (RouteCheckService.OutsideBounds(node, _file))
			{
				la_Over.ForeColor = Optionables.FieldsForecolorHighlight;
			}
			else
				la_Over.ForeColor = GetNodeColor(node);

			string info;

			if (og) info = "Og ";
			else    info = "Go ";

			la_Over.Text = info + node.Id + Environment.NewLine // only this RouteView.
						 + Globals.GetLocationString(node.Col, node.Row, node.Lev, _file.Levs);
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
					NodeSelected = _file.AddRouteNode(args.Location);
					InvalidatePanels(); // not sure why but that's needed after adding the "ReduceDraws" option
				}
				updateinfo = NodeSelected != null;
			}
			else if (node == null) // a node is already selected but there's not a node on the current tile ->
			{
				if (args.MouseButton == MouseButtons.Right)
				{
					RoutesChangedCoordinator = true;
					node = _file.AddRouteNode(args.Location);
					ConnectNode(node);
					InvalidatePanels(); // not sure why but that's needed after adding the "ReduceDraws" option
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

					_file.GetTile(Dragnode.Col, // clear the node from the previous tile
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

					PrintOverInfo(RouteControl._col, RouteControl._row);
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
		private void OnRouteControlMouseMove(object sender, MouseEventArgs args)
		{
			RouteControl.SetOver(new Point(args.X, args.Y));

			PrintOverInfo(RouteControl._col, RouteControl._row);

			ObserverManager.RouteView   .Control     .la_Over.Refresh(); // fast update. // NOTE: Only RouteView not TopRouteView(Route)
			ObserverManager.TopRouteView.ControlRoute.la_Over.Refresh(); // fast update. // wants fast update. go figure

			// TODO: if (MainView.Optionables.ShowOverlay)
			RefreshPanels(); // fast update. (else the InfoOverlay on RouteView but not TopRouteView(Route) gets sticky - go figur)
		}

		/// <summary>
		/// Handler that hides the info-overlay when the mouse leaves this
		/// control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <seealso cref="RouteControlParent"><c>RouteControlParent.t1_Tick()</c></seealso>
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
				case ConnectNodes.TwoWay:
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
					goto case ConnectNodes.OneWay; // fallthrough

				case ConnectNodes.OneWay:
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
			bool nodeselected = NodeSelected != null;

			ObserverManager.RouteView   .Control     .bu_Cut   .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Cut   .Enabled =

			ObserverManager.RouteView   .Control     .bu_Copy  .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Copy  .Enabled =

			ObserverManager.RouteView   .Control     .bu_Delete.Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Delete.Enabled = nodeselected;

			ObserverManager.RouteView   .Control     .bu_Paste .Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Paste .Enabled = nodeselected && _copynodedata.unittype != -1;
		}


		/// <summary>
		/// Updates distances to and from the currently selected node.
		/// </summary>
		/// <remarks><c><see cref="NodeSelected"/></c> must be valid before
		/// call.</remarks>
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
															_file.Routes[link.Destination]);
						distance = link.Distance + GetDistanceArrow(slot);
						break;
				}

				switch (slot)
				{
					case 0: la_Link1Dist.Text = distance; break;
					case 1: la_Link2Dist.Text = distance; break;
					case 2: la_Link3Dist.Text = distance; break;
					case 3: la_Link4Dist.Text = distance; break;
					case 4: la_Link5Dist.Text = distance; break;
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

				if (isToproute)
					ObserverManager.RouteView.Control.co_Type.SelectedIndex = co_Type.SelectedIndex;
				else
					ObserverManager.TopRouteView.ControlRoute.co_Type.SelectedIndex = co_Type.SelectedIndex;
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
						if (SpawnInfo != null)
							SpawnInfo.UpdateNoderank(_selRank, NodeSelected.Rank);

						_selRank = NodeSelected.Rank;
					}

					NodeSelected.OobRank = (byte)0;

					if (isToproute)
						ObserverManager.RouteView.Control.co_Rank.SelectedIndex = co_Rank.SelectedIndex;
					else
						ObserverManager.TopRouteView.ControlRoute.co_Rank.SelectedIndex = co_Rank.SelectedIndex;

					SetSelectedInfoColor();
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

				if (SpawnInfo != null)
					SpawnInfo.ChangedSpawnweight(_selWeight, NodeSelected.Spawn, NodeSelected.Rank);

				_selWeight = NodeSelected.Spawn;

				if (isToproute)
					ObserverManager.RouteView.Control.co_Spawn.SelectedIndex = co_Spawn.SelectedIndex;
				else
					ObserverManager.TopRouteView.ControlRoute.co_Spawn.SelectedIndex = co_Spawn.SelectedIndex;

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

				if (isToproute)
					ObserverManager.RouteView.Control.co_Patrol.SelectedIndex = co_Patrol.SelectedIndex;
				else
					ObserverManager.TopRouteView.ControlRoute.co_Patrol.SelectedIndex = co_Patrol.SelectedIndex;

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

				if (isToproute)
					ObserverManager.RouteView.Control.co_Attack.SelectedIndex = co_Attack.SelectedIndex;
				else
					ObserverManager.TopRouteView.ControlRoute.co_Attack.SelectedIndex = co_Attack.SelectedIndex;
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
				Label la_dist, la_link;
				Button bu_go;

				var co = sender as ComboBox;
				if (co == co_Link1Dest)
				{
					slot    = 0;
					la_dist = la_Link1Dist;
					bu_go   = bu_GoLink1;
					la_link = la_Link1;
				}
				else if (co == co_Link2Dest)
				{
					slot    = 1;
					la_dist = la_Link2Dist;
					bu_go   = bu_GoLink2;
					la_link = la_Link2;
				}
				else if (co == co_Link3Dest)
				{
					slot    = 2;
					la_dist = la_Link3Dist;
					bu_go   = bu_GoLink3;
					la_link = la_Link3;
				}
				else if (co == co_Link4Dest)
				{
					slot    = 3;
					la_dist = la_Link4Dist;
					bu_go   = bu_GoLink4;
					la_link = la_Link4;
				}
				else // co == co_Link5Dest
				{
					slot    = 4;
					la_dist = la_Link5Dist;
					bu_go   = bu_GoLink5;
					la_link = la_Link5;
				}

				var dest = co.SelectedItem as byte?; // check for id or compass pt/not used.
				if (!dest.HasValue)
					dest = (byte?)(co.SelectedItem as LinkType?);

				Link link = NodeSelected[slot];
				switch (link.Destination = dest.Value)
				{
					case Link.NotUsed:
						link.Unit = UnitType.Any;

						la_dist.Text = String.Empty;
						link.Distance = 0;

						bu_go.Enabled = false;
						bu_go.Text = String.Empty;
						la_link.ForeColor = Optionables.FieldsForecolor;
						break;

					case Link.ExitWest:
					case Link.ExitNorth:
					case Link.ExitEast:
					case Link.ExitSouth:
						la_dist.Text = "0";
						link.Distance = 0;

						bu_go.Enabled = false;
						bu_go.Text = Go;
						la_link.ForeColor = Optionables.FieldsForecolor;
						break;

					default:
					{
						RouteNode nodeDest = _file.Routes[link.Destination];
						link.Distance = CalculateLinkDistance(
															NodeSelected,
															nodeDest,
															la_dist,
															slot);
						bu_go.Enabled = true;
						bu_go.Text = Go;

						if (RouteCheckService.OutsideBounds(nodeDest, _file))			// NOTE: 'la_link.ForeColor' changes on both RouteView and
							la_link.ForeColor = Optionables.FieldsForecolorHighlight;	// TopRouteView(Route) without additional facility req'd.
						else
							la_link.ForeColor = Optionables.FieldsForecolor;

						break;
					}
				}

				RouteControl.SetSpot(new Point(-1,-1));

				if (isToproute)
				{
					ObserverManager.RouteView.Control.TransferDestination(
																		slot,
																		co.SelectedIndex,
																		la_dist.Text,
																		bu_go.Enabled,
																		bu_go.Text);
				}
				else
				{
					ObserverManager.TopRouteView.ControlRoute.TransferDestination(
																				slot,
																				co.SelectedIndex,
																				la_dist.Text,
																				bu_go.Enabled,
																				bu_go.Text);
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
			ComboBox co_dest;
			Label la_dist;
			Button bu_go;

			switch (slot)
			{
				case 0:
					co_dest = co_Link1Dest;
					la_dist = la_Link1Dist;
					bu_go   = bu_GoLink1;
					break;

				case 1:
					co_dest = co_Link2Dest;
					la_dist = la_Link2Dist;
					bu_go   = bu_GoLink2;
					break;

				case 2:
					co_dest = co_Link3Dest;
					la_dist = la_Link3Dist;
					bu_go   = bu_GoLink3;
					break;

				case 3:
					co_dest = co_Link4Dest;
					la_dist = la_Link4Dist;
					bu_go   = bu_GoLink4;
					break;

				default: // case 4
					co_dest = co_Link5Dest;
					la_dist = la_Link5Dist;
					bu_go   = bu_GoLink5;
					break;
			}

			co_dest.SelectedIndex = dest;
			la_dist.Text          = dist;
			bu_go.Enabled         = enable;
			bu_go.Text            = text;
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

			if (dist > Byte.MaxValue)
				dist = Byte.MaxValue;

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
				else                             slot = 4; // co == co_Link5UnitType

				NodeSelected[slot].Unit = (UnitType)co.SelectedItem;

				if (isToproute)
					ObserverManager.RouteView.Control.TransferUnitType(slot, co.SelectedIndex);
				else
					ObserverManager.TopRouteView.ControlRoute.TransferUnitType(slot, co.SelectedIndex);
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
				default: co_UnitType = co_Link5UnitType; break; // case 4
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
			if      (sender == bu_GoLink1) slot = 0;
			else if (sender == bu_GoLink2) slot = 1;
			else if (sender == bu_GoLink3) slot = 2;
			else if (sender == bu_GoLink4) slot = 3;
			else                           slot = 4; // sender == bu_GoLink5

			byte dest = NodeSelected[slot].Destination;
			RouteNode node = _file.Routes[dest];

			if (RouteCheckService.OutsideBounds(node, _file))
			{
				RouteCheckService.SetBase1( // send the base1-count options to 'XCom' ->
										MainViewF.Optionables.Base1_xy,
										MainViewF.Optionables.Base1_z);

				if (RouteCheckService.dialog_InvalidDestination(_file, node) == DialogResult.Yes)
				{
					RoutesChangedCoordinator = true;

					if (SpawnInfo != null)
						SpawnInfo.DeleteNode(node);

					_file.Routes.DeleteNode(node);

					UpdateNodeInfo();
					// TODO: May need _pnlRoutes.Refresh()
				}
			}
			else
			{
				if (_ogIds.Count == 0 || _ogIds.Peek() != NodeSelected.Id)
					_ogIds.Push(NodeSelected.Id);

				EnableOgButton(true);

				SelectNode(dest);

				OnLinkMouseEnter(sender, EventArgs.Empty); // update Go info
			}

			RouteControl.Select();
		}

		/// <summary>
		/// Deals with the ramifications of a Go or Og click.
		/// </summary>
		/// <param name="id"></param>
		/// <remarks>Any changes that are done here regarding node-selection
		/// should be reflected in
		/// <c><see cref="RouteControlParent"/>.OnMouseDown()</c> since that is
		/// an alternate way to select a tile/node. Except that
		/// <c>OnMouseDown()</c> never causes a level change.</remarks>
		private void SelectNode(int id)
		{
			RouteNode node = _file.Routes[id];
			var loc = new Point(node.Col, node.Row);

			if (node.Lev != _file.Level)
				_file.Level = node.Lev;											// fire LevelSelected

			_file.Location = new MapLocation(									// fire LocationSelected
										loc.X, loc.Y,
										_file.Level);

			MainViewOverlay.that.ProcessSelection(loc,loc);

			var args = new RouteControlEventArgs(
											MouseButtons.Left,
											_file.GetTile(loc.X, loc.Y),
											_file.Location);
			OnRouteControlMouseDown(null, args);

			InvalidateControls();

			ObserverManager.ToolFactory.EnableLevelers(_file.Level, _file.Levs);
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
				default:   slot = 4; break; // case "L5"
			}
			SpotDestination(slot); // TODO: RouteView/TopRouteView(Route)

			Link link = NodeSelected[slot];
			if (link.IsNodelink())
			{
				PrintGoInfo(_file.Routes[link.Destination], false); // TODO: ensure that nodes are listed in RouteNodes in consecutive order ...
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
							RouteNode node = _file.Routes[dest];
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
			int id = _ogIds.Pop();
			if (id < _file.Routes.Nodes.Count) // in case nodes were deleted. TODO: check deleted nodes against ogIds as a List<int>
			{
				if (NodeSelected == null || id != NodeSelected.Id)
				{
					SelectNode(id);
					OnOgMouseEnter(null, EventArgs.Empty); // update Og info
				}
			}

			if (_ogIds.Count == 0)
				EnableOgButton(false);

			RouteControl.Select();
		}

		/// <summary>
		/// Spots a route-node when the cursor enters the Og button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOgMouseEnter(object sender, EventArgs e)
		{
			if (_ogIds.Count != 0)
			{
				int id = _ogIds.Peek();
				if (id < _file.Routes.Nodes.Count) // in case nodes were deleted.
				{
					RouteNode node = _file.Routes[id];
					RouteControl.SetSpot(new Point(node.Col, node.Row));

					RouteControl.Refresh();
//					RefreshControls();

					PrintGoInfo(node, true);
				}
			}
		}

		/// <summary>
		/// Disables the Og button when a Map gets loaded.
		/// </summary>
		/// <param name="enable"></param>
		private static void EnableOgButton(bool enable)
		{
			ObserverManager.RouteView   .Control     .bu_Og.Enabled =
			ObserverManager.TopRouteView.ControlRoute.bu_Og.Enabled = enable;
		}
		#endregion Events (LinkData)


		#region Events (node edit)
		/// <summary>
		/// Prevents two error-dialogs from showing if a key-cut is underway.
		/// </summary>
		private bool _bypassCutError;

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
					_bypassCutError = true;
					 OnCopyClick(  null, EventArgs.Empty);
					 OnDeleteClick(null, EventArgs.Empty);
					 _bypassCutError = false;
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

				_copynodedata.unittype       = co_Type  .SelectedIndex;
				_copynodedata.noderank       = co_Rank  .SelectedIndex;
				_copynodedata.spawnweight    = co_Spawn .SelectedIndex;
				_copynodedata.patrolpriority = co_Patrol.SelectedIndex;
				_copynodedata.baseattack     = co_Attack.SelectedIndex;
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
				if (_copynodedata.unittype != -1)
				{
					bool changed = false;

					if (co_Type.SelectedIndex != _copynodedata.unittype)
					{
						co_Type.SelectedIndex = _copynodedata.unittype;
						changed = true;
					}

					if (co_Rank.SelectedIndex != _copynodedata.noderank)
					{
						co_Rank.SelectedIndex = _copynodedata.noderank;
						changed = true;

						_selRank = (byte)_copynodedata.noderank;
					}

					if (co_Spawn.SelectedIndex != _copynodedata.spawnweight)
					{
						co_Spawn.SelectedIndex = _copynodedata.spawnweight;
						changed = true;

						_selWeight = (SpawnWeight)_copynodedata.spawnweight;
					}

					if (co_Patrol.SelectedIndex != _copynodedata.patrolpriority)
					{
						co_Patrol.SelectedIndex = _copynodedata.patrolpriority;
						changed = true;
					}

					if (co_Attack.SelectedIndex != _copynodedata.baseattack)
					{
						co_Attack.SelectedIndex = _copynodedata.baseattack;
						changed = true;
					}

					if (changed) RoutesChangedCoordinator = true;
				}
				else
					ShowError("There isn't any node data copied."); // <- needed for [Ctrl+v]
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

				if (SpawnInfo != null)
					SpawnInfo.DeleteNode(NodeSelected);

				_file.GetTile(NodeSelected.Col,
							  NodeSelected.Row,
							  NodeSelected.Lev).Node = null;
				_file.Routes.DeleteNode(NodeSelected);

				DeselectNodeStatic();
				UpdateNodeInfo();

				// TODO: check if the Og-button should be disabled when a node gets deleted or cut.

				RefreshControls();
			}			
			else if (!_bypassCutError)
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
		/// <param name="updatefields"><c>true</c> if a location is selected in
		/// <c>MainView</c> or <c>TopView</c> (even if it's still the node's
		/// location) - <c>false</c> if the node is deleted from
		/// <c>RouteView</c> itself</param>
		/// <seealso cref="DeselectNodeStatic()"><c>DeselectNodeStatic()</c></seealso>
		private void DeselectNode(bool updatefields = false)
		{
			NodeSelected = null;

			if (updatefields)
			{
				UpdateNodeInfo();
				RouteControl.Invalidate();
			}
			else
				RouteControl.Select();
		}

		/// <summary>
		/// Synchs
		/// <c><see cref="DeselectNode()">DeselectNode()</see></c> in
		/// <c>TopView</c> and <c>TopRouteView</c>.
		/// </summary>
		/// <param name="updatefields"><c>true</c> if a location is selected in
		/// <c>MainView</c> or <c>TopView</c> (even if it's still the node's
		/// location) - <c>false</c> if the node is deleted from
		/// <c>RouteView</c> itself</param>
		internal static void DeselectNodeStatic(bool updatefields = false)
		{
			ObserverManager.RouteView   .Control     .DeselectNode(updatefields);
			ObserverManager.TopRouteView.ControlRoute.DeselectNode(updatefields);
		}


		#region Events (toolstrip)
		/// <summary>
		/// Handles clicking on any of the three ConnectType toolstrip buttons.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="tsb_connect0"/></c></item>
		/// <item><c><see cref="tsb_connect1"/></c></item>
		/// <item><c><see cref="tsb_connect2"/></c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnConnectTypeClicked(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (!tsb.Checked)
			{
				RouteView alt;
				if (isToproute)
					alt = ObserverManager.RouteView.Control;
				else
					alt = ObserverManager.TopRouteView.ControlRoute;

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
					_conType = ConnectNodes.None;

						tsb_connect0.Checked =
					alt.tsb_connect0.Checked = true;

						tsb_connect0.Image =
					alt.tsb_connect0.Image = Properties.Resources.connect_0_red;
				}
				else if (tsb == tsb_connect1)
				{
					_conType = ConnectNodes.OneWay;

						tsb_connect1.Checked =
					alt.tsb_connect1.Checked = true;

						tsb_connect1.Image =
					alt.tsb_connect1.Image = Properties.Resources.connect_1_blue;
				}
				else // tsb == tsb_connect2
				{
					_conType = ConnectNodes.TwoWay;

						tsb_connect2.Checked =
					alt.tsb_connect2.Checked = true;
						tsb_connect2.Image =
					alt.tsb_connect2.Image = Properties.Resources.connect_2_green;
				}
			}
			RouteControl.Select();
		}


		/// <summary>
		/// Handles the Scale toolstrip button/toggle.
		/// </summary>
		/// <param name="sender"><c><see cref="tsb_x2"/></c></param>
		/// <param name="e"></param>
		private void OnScaleClick(object sender, EventArgs e)
		{
			RouteControl.doScaleResize((sender as ToolStripButton).Checked, true);
		}


		private string _lastExportDirectory;

		/// <summary>
		/// Exports nodes to a Routes-file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
						string path = Path.Combine(_file.Descriptor.Basepath, GlobalsXC.RoutesDir);
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

		/// <summary>
		/// Imports nodes from a Routes-file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnImportClick(object sender, EventArgs e)
		{
			if (_file != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title    = "Import Route file ...";
					ofd.Filter   = "Route files (*.RMP)|*.RMP|All files (*.*)|*.*";
					ofd.FileName = _file.Descriptor.Label + GlobalsXC.RouteExt;

					if (!Directory.Exists(_lastImportDirectory))
					{
						string dir = Path.Combine(_file.Descriptor.Basepath, GlobalsXC.RoutesDir);
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

							DeselectNodeStatic();

							_file.ClearRouteNodes();
							_file.Routes = routes;
							_file.SetupRouteNodes();

							RouteCheckService.SetBase1( // send the base1-count options to 'XCom' ->
													MainViewF.Optionables.Base1_xy,
													MainViewF.Optionables.Base1_z);

							if (RouteCheckService.CheckNodeBounds(_file) == DialogResult.Yes)
							{
								foreach (RouteNode node in RouteCheckService.Invalids)
									_file.Routes.DeleteNode(node);
							}

							UpdateNodeInfo(); // not sure is necessary ...
							RefreshPanels();

							if (SpawnInfo != null)
								SpawnInfo.Initialize(_file);
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
			bool nodeselected = NodeSelected != null;

			tsmi_RaiseNode.Enabled = nodeselected && NodeSelected.Lev != 0;
			tsmi_LowerNode.Enabled = nodeselected && NodeSelected.Lev != _file.Levs - 1;

			tsmi_ClearLinks.Enabled = nodeselected;
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
											_file.GetTile(Dragnode.Col,
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
											_file.GetTile(Dragnode.Col,
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
		/// Toggles <c><see cref="GhostNodesCoordinator"/></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnGhostNodesClick(object sender, EventArgs e)
		{
			GhostNodesCoordinator = !tsmi_GhostNodes.Checked;
			InvalidatePanels();
		}

		private static byte _noderankHighlighted = Byte.MaxValue;
		/// <summary>
		/// Tracks which noderank (if any) user has chosen to highlight.
		/// </summary>
		/// <remarks><c>Byte.MaxValue</c> if none.</remarks>
		internal static byte NoderankHighlighted
		{
			get { return _noderankHighlighted; }
			private set { _noderankHighlighted = value; }
		}

		/// <summary>
		/// Handles <c>Click</c> on any of the node-color panels in
		/// <c><see cref="gb_NoderankColors"/></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNodecolorClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Keys keyData;
				if      (sender == pa_ColorRank0) keyData = Keys.D0;
				else if (sender == pa_ColorRank1) keyData = Keys.D1;
				else if (sender == pa_ColorRank2) keyData = Keys.D2;
				else if (sender == pa_ColorRank3) keyData = Keys.D3;
				else if (sender == pa_ColorRank4) keyData = Keys.D4;
				else if (sender == pa_ColorRank5) keyData = Keys.D5;
				else if (sender == pa_ColorRank6) keyData = Keys.D6;
				else if (sender == pa_ColorRank7) keyData = Keys.D7;
				else                              keyData = Keys.D8; // pa_ColorRank8

				FireNoderankClick(keyData);
			}
		}

		/// <summary>
		/// Handles hotkeys [0..8] when the RouteView panel has focus.
		/// </summary>
		/// <param name="keyData"></param>
		internal void FireNoderankClick(Keys keyData)
		{
			object it = null;
			switch (keyData)
			{
				case Keys.D0: it = tsmi_Noderank0; break;
				case Keys.D1: it = tsmi_Noderank1; break;
				case Keys.D2: it = tsmi_Noderank2; break;
				case Keys.D3: it = tsmi_Noderank3; break;
				case Keys.D4: it = tsmi_Noderank4; break;
				case Keys.D5: it = tsmi_Noderank5; break;
				case Keys.D6: it = tsmi_Noderank6; break;
				case Keys.D7: it = tsmi_Noderank7; break;
				case Keys.D8: it = tsmi_Noderank8; break;
			}

			OnNoderankClick(it, EventArgs.Empty);
		}

		/// <summary>
		/// Handles its under the Highlights menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNoderankClick(object sender, EventArgs e)
		{
			if (sender == tsmi_Noderank0)
			{
				if (!tsmi_Noderank0.Checked)
				{
					ClearNoderankIts();
					SetNoderank0Checked(true);
				}
				else
					SetNoderank0Checked(false);
			}
			else if (sender == tsmi_Noderank1)
			{
				if (!tsmi_Noderank1.Checked)
				{
					ClearNoderankIts();
					SetNoderank1Checked(true);
				}
				else
					SetNoderank1Checked(false);
			}
			else if (sender == tsmi_Noderank2)
			{
				if (!tsmi_Noderank2.Checked)
				{
					ClearNoderankIts();
					SetNoderank2Checked(true);
				}
				else
					SetNoderank2Checked(false);
			}
			else if (sender == tsmi_Noderank3)
			{
				if (!tsmi_Noderank3.Checked)
				{
					ClearNoderankIts();
					SetNoderank3Checked(true);
				}
				else
					SetNoderank3Checked(false);
			}
			else if (sender == tsmi_Noderank4)
			{
				if (!tsmi_Noderank4.Checked)
				{
					ClearNoderankIts();
					SetNoderank4Checked(true);
				}
				else
					SetNoderank4Checked(false);
			}
			else if (sender == tsmi_Noderank5)
			{
				if (!tsmi_Noderank5.Checked)
				{
					ClearNoderankIts();
					SetNoderank5Checked(true);
				}
				else
					SetNoderank5Checked(false);
			}
			else if (sender == tsmi_Noderank6)
			{
				if (!tsmi_Noderank6.Checked)
				{
					ClearNoderankIts();
					SetNoderank6Checked(true);
				}
				else
					SetNoderank6Checked(false);
			}
			else if (sender == tsmi_Noderank7)
			{
				if (!tsmi_Noderank7.Checked)
				{
					ClearNoderankIts();
					SetNoderank7Checked(true);
				}
				else
					SetNoderank7Checked(false);
			}
			else // tsmi_Noderank8
			{
				if (!tsmi_Noderank8.Checked)
				{
					ClearNoderankIts();
					SetNoderank8Checked(true);
				}
				else
					SetNoderank8Checked(false);
			}

			InvalidatePanels();
		}

		/// <summary>
		/// Preps this <c>Control</c> for noderank highlights. Clears all
		/// Highlights its and ghosts all noderank colors.
		/// </summary>
		private void ClearNoderankIts()
		{
			RouteView ro = ObserverManager.RouteView   .Control;
			RouteView tr = ObserverManager.TopRouteView.ControlRoute;

			ro.tsmi_Noderank0.Checked = tr.tsmi_Noderank0.Checked =
			ro.tsmi_Noderank1.Checked = tr.tsmi_Noderank1.Checked =
			ro.tsmi_Noderank2.Checked = tr.tsmi_Noderank2.Checked =
			ro.tsmi_Noderank3.Checked = tr.tsmi_Noderank3.Checked =
			ro.tsmi_Noderank4.Checked = tr.tsmi_Noderank4.Checked =
			ro.tsmi_Noderank5.Checked = tr.tsmi_Noderank5.Checked =
			ro.tsmi_Noderank6.Checked = tr.tsmi_Noderank6.Checked =
			ro.tsmi_Noderank7.Checked = tr.tsmi_Noderank7.Checked =
			ro.tsmi_Noderank8.Checked = tr.tsmi_Noderank8.Checked = false;

			ro.pa_ColorRank0.BackColor = tr.pa_ColorRank0.BackColor =
			ro.pa_ColorRank1.BackColor = tr.pa_ColorRank1.BackColor =
			ro.pa_ColorRank2.BackColor = tr.pa_ColorRank2.BackColor =
			ro.pa_ColorRank3.BackColor = tr.pa_ColorRank3.BackColor =
			ro.pa_ColorRank4.BackColor = tr.pa_ColorRank4.BackColor =
			ro.pa_ColorRank5.BackColor = tr.pa_ColorRank5.BackColor =
			ro.pa_ColorRank6.BackColor = tr.pa_ColorRank6.BackColor =
			ro.pa_ColorRank7.BackColor = tr.pa_ColorRank7.BackColor =
			ro.pa_ColorRank8.BackColor = tr.pa_ColorRank8.BackColor = Optionables.NodeColorGhosted;
		}

		/// <summary>
		/// Sets all noderank colors to non-ghosted.
		/// </summary>
		private void FillNoderankColors()
		{
			RouteView ro = ObserverManager.RouteView   .Control;
			RouteView tr = ObserverManager.TopRouteView.ControlRoute;

			ro.pa_ColorRank0.BackColor = tr.pa_ColorRank0.BackColor = Optionables.NodeColor0;
			ro.pa_ColorRank1.BackColor = tr.pa_ColorRank1.BackColor = Optionables.NodeColor1;
			ro.pa_ColorRank2.BackColor = tr.pa_ColorRank2.BackColor = Optionables.NodeColor2;
			ro.pa_ColorRank3.BackColor = tr.pa_ColorRank3.BackColor = Optionables.NodeColor3;
			ro.pa_ColorRank4.BackColor = tr.pa_ColorRank4.BackColor = Optionables.NodeColor4;
			ro.pa_ColorRank5.BackColor = tr.pa_ColorRank5.BackColor = Optionables.NodeColor5;
			ro.pa_ColorRank6.BackColor = tr.pa_ColorRank6.BackColor = Optionables.NodeColor6;
			ro.pa_ColorRank7.BackColor = tr.pa_ColorRank7.BackColor = Optionables.NodeColor7;
			ro.pa_ColorRank8.BackColor = tr.pa_ColorRank8.BackColor = Optionables.NodeColor8;
		}

		/// <summary>
		/// Sets noderank 0 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank0Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank0.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank0.Checked = @checked)
			{
				NoderankHighlighted = (byte)0;
				ObserverManager.RouteView   .Control     .pa_ColorRank0.BackColor = Optionables.NodeColor0;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank0.BackColor = Optionables.NodeColor0;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 1 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank1Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank1.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank1.Checked = @checked)
			{
				NoderankHighlighted = (byte)1;
				ObserverManager.RouteView   .Control     .pa_ColorRank1.BackColor = Optionables.NodeColor1;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank1.BackColor = Optionables.NodeColor1;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 2 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank2Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank2.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank2.Checked = @checked)
			{
				NoderankHighlighted = (byte)2;
				ObserverManager.RouteView   .Control     .pa_ColorRank2.BackColor = Optionables.NodeColor2;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank2.BackColor = Optionables.NodeColor2;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 3 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank3Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank3.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank3.Checked = @checked)
			{
				NoderankHighlighted = (byte)3;
				ObserverManager.RouteView   .Control     .pa_ColorRank3.BackColor = Optionables.NodeColor3;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank3.BackColor = Optionables.NodeColor3;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 4 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank4Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank4.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank4.Checked = @checked)
			{
				NoderankHighlighted = (byte)4;
				ObserverManager.RouteView   .Control     .pa_ColorRank4.BackColor = Optionables.NodeColor4;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank4.BackColor = Optionables.NodeColor4;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 5 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank5Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank5.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank5.Checked = @checked)
			{
				NoderankHighlighted = (byte)5;
				ObserverManager.RouteView   .Control     .pa_ColorRank5.BackColor = Optionables.NodeColor5;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank5.BackColor = Optionables.NodeColor5;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 6 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank6Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank6.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank6.Checked = @checked)
			{
				NoderankHighlighted = (byte)6;
				ObserverManager.RouteView   .Control     .pa_ColorRank6.BackColor = Optionables.NodeColor6;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank6.BackColor = Optionables.NodeColor6;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 7 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank7Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank7.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank7.Checked = @checked)
			{
				NoderankHighlighted = (byte)7;
				ObserverManager.RouteView   .Control     .pa_ColorRank7.BackColor = Optionables.NodeColor7;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank7.BackColor = Optionables.NodeColor7;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
			}
		}

		/// <summary>
		/// Sets noderank 8 de/checked.
		/// </summary>
		/// <param name="checked"></param>
		private void SetNoderank8Checked(bool @checked)
		{
			if (ObserverManager.RouteView   .Control     .tsmi_Noderank8.Checked =
				ObserverManager.TopRouteView.ControlRoute.tsmi_Noderank8.Checked = @checked)
			{
				NoderankHighlighted = (byte)8;
				ObserverManager.RouteView   .Control     .pa_ColorRank8.BackColor = Optionables.NodeColor8;
				ObserverManager.TopRouteView.ControlRoute.pa_ColorRank8.BackColor = Optionables.NodeColor8;
			}
			else
			{
				NoderankHighlighted = Byte.MaxValue;
				FillNoderankColors();
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
		/// Handler for menuitem that sets all
		/// <c><see cref="RouteNode">RouteNodes'</see></c> <c>Unit</c> value
		/// to <c><see cref="UnitType"/>.Any</c>.
		/// </summary>
		/// <param name="sender"><c><see cref="tsmi_ZeroUnittypes"/></c></param>
		/// <param name="e"></param>
		private void OnZeroUnittypesClick(object sender, EventArgs e)
		{
			string any = Enum.GetName(typeof(UnitType), UnitType.Any);
//			string any = ((Pterodactyl)RouteNodes.Unit[0]).ToString(); // there is no Pterodactyl for UnitType.

			using (var f = new Infobox(
									"Warning",
									"Are you sure you want to change all unittypes to " + any + " ...",
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					int changed = 0;
					foreach (RouteNode node in _file.Routes)
					{
						if (node.Unit != UnitType.Any)
						{
							node.Unit = UnitType.Any;
							++changed;
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
						head = "All nodes are already type " + any + ".";

					using (var g = new Infobox("Zero all unittypes", head))
						g.ShowDialog(this);
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that sets all
		/// <c><see cref="RouteNode">RouteNodes'</see></c> <c>Rank</c> value to
		/// <c><see cref="NodeRankUfo"/>/<see cref="NodeRankTftd"/>.CivScout</c>.
		/// </summary>
		/// <param name="sender"><c><see cref="tsmi_ZeroNoderanks"/></c></param>
		/// <param name="e"></param>
		private void OnZeroNoderanksClick(object sender, EventArgs e)
		{
			string civscout;
			if (_file.Descriptor.GroupType == GroupType.Tftd)
				civscout = ((Pterodactyl)RouteNodes.RankTftd[0]).ToString(); // these are basically identical but go with it ...
			else
				civscout = ((Pterodactyl)RouteNodes.RankUfo [0]).ToString();

			using (var f = new Infobox(
									"Warning",
									"Are you sure you want to change all noderanks to " + civscout + " ...",
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					_selRank = (byte)0;

					int changed = 0;
					foreach (RouteNode node in _file.Routes)
					{
						if (node.Rank != (byte)0)
						{
							if (SpawnInfo != null && node.Spawn != SpawnWeight.None)
								SpawnInfo.UpdateNoderank(node.Rank, (byte)0);

							node.Rank = (byte)0;
							++changed;
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
						head = "All nodes are already rank " + civscout + ".";

					using (var g = new Infobox("Zero all noderanks", head))
						g.ShowDialog(this);
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that sets all
		/// <c><see cref="RouteNode">RouteNodes'</see></c> <c>Spawn</c> value
		/// to <c><see cref="SpawnWeight"/>.None</c>.
		/// </summary>
		/// <param name="sender"><c><see cref="tsmi_ZeroSpawnweights"/></c></param>
		/// <param name="e"></param>
		private void OnZeroSpawnweightsClick(object sender, EventArgs e)
		{
			string none = ((Pterodactyl)RouteNodes.Spawn[0]).ToString();

			using (var f = new Infobox(
									"Warning",
									"Are you sure you want to change all spawnweights to " + none + " ...",
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					_selWeight = SpawnWeight.None;

					int changed = 0;
					foreach (RouteNode node in _file.Routes)
					{
						if (node.Spawn != SpawnWeight.None)
						{
							if (SpawnInfo != null)
								SpawnInfo.ChangedSpawnweight(node.Spawn, SpawnWeight.None, node.Rank);

							node.Spawn = SpawnWeight.None;
							++changed;
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
						head = "All nodes are already weight " + none + ".";

					using (var g = new Infobox("Zero all spawnweights", head))
						g.ShowDialog(this);
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that sets all
		/// <c><see cref="RouteNode">RouteNodes'</see></c> <c>Patrol</c> value
		/// to <c><see cref="PatrolPriority"/>.Zero</c>.
		/// </summary>
		/// <param name="sender"><c><see cref="tsmi_ZeroPatrolpriorities"/></c></param>
		/// <param name="e"></param>
		private void OnZeroPatrolprioritiesClick(object sender, EventArgs e)
		{
			string lolo = ((Pterodactyl)RouteNodes.Patrol[0]).ToString();

			using (var f = new Infobox(
									"Warning",
									"Are you sure you want to change all patrolpriorities to " + lolo + " ...",
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					int changed = 0;
					foreach (RouteNode node in _file.Routes)
					{
						if (node.Patrol != PatrolPriority.Zero)
						{
							node.Patrol = PatrolPriority.Zero;
							++changed;
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
						head = "All nodes are already priority " + lolo + ".";

					using (var g = new Infobox("Zero all patrolpriorities", head))
						g.ShowDialog(this);
				}
			}
		}

		/// <summary>
		/// Handler for menuitem that sets all
		/// <c><see cref="RouteNode">RouteNodes'</see></c> <c>Attack</c> value
		/// to <c><see cref="AttackBase"/>.Zero</c>.
		/// </summary>
		/// <param name="sender"><c><see cref="tsmi_ZeroBaseattacks"/></c></param>
		/// <param name="e"></param>
		private void OnZeroBaseattacksClick(object sender, EventArgs e)
		{
			string none = ((Pterodactyl)RouteNodes.Attack[0]).ToString();

			using (var f = new Infobox(
									"Warning",
									"Are you sure you want to change all baseattacks to " + none + " ...",
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					int changed = 0;
					foreach (RouteNode node in _file.Routes)
					{
						if (node.Attack != AttackBase.Zero)
						{
							node.Attack = AttackBase.Zero;
							++changed;
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
						head = "All nodes are already attack " + none + ".";

					using (var g = new Infobox("Zero all baseattacks", head))
						g.ShowDialog(this);
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

			int total = _file.Routes.Nodes.Count;
			for (var id = 0; id != total; ++id)
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
			RouteCheckService.SetBase1( // send the base1-count options to 'XCom' ->
									MainViewF.Optionables.Base1_xy,
									MainViewF.Optionables.Base1_z);

			if (RouteCheckService.CheckNodeBounds(_file, true) == DialogResult.Yes)
			{
				RoutesChangedCoordinator = true;

				foreach (RouteNode node in RouteCheckService.Invalids)
				{
					if (SpawnInfo != null)
						SpawnInfo.DeleteNode(node);

					_file.Routes.DeleteNode(node);
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
			foreach (RouteNode node in _file.Routes)
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
					copyable += Path.GetFileNameWithoutExtension(RouteNodes.PfeRoutes).ToUpper() + " - node " + id;
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
		internal static SpawnInfo SpawnInfo
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
			if (SpawnInfo == null)
			{
				SpawnInfo = new SpawnInfo(_file);
				SpawnInfo.Show(this);
			}
			else
			{
				if (SpawnInfo.WindowState == FormWindowState.Minimized)
					SpawnInfo.WindowState  = FormWindowState.Normal;

				SpawnInfo.Activate(); // so what's the diff ->
//				SpawnInfo.Focus();
//				SpawnInfo.Select();
//				SpawnInfo.BringToFront();
//				SpawnInfo.TopMost = true;
//				SpawnInfo.TopMost = false;
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
		/// Selects one of the connector-buttons when either
		/// <c><see cref="RouteViewForm"/></c> or
		/// <c><see cref="TopRouteViewForm"/></c> is first shown.
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


		internal static OptionsF _foptions;	// is static so that it will be used by both
											// RouteView and TopRouteView(Route)
		/// <summary>
		/// Handles a click on the Options button to show or hide an
		/// <c><see cref="OptionsF"/></c>. Instantiates an <c>OptionsF</c> if
		/// one doesn't exist for this viewer. Also subscribes to a
		/// <c>FormClosing</c> handler that will hide the <c>Form</c> unless
		/// MapView is quitting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (tsb.Checked && _foptions.WindowState == FormWindowState.Minimized)
			{
				_foptions.WindowState = FormWindowState.Normal;
			}
			else if (!tsb.Checked)
			{
				setOptionsChecked(true);

				if (_foptions == null)
				{
					_foptions = new OptionsF(
										Optionables,
										Options,
										OptionableType.RouteView);
					_foptions.Text = "RouteView Options";

//					if (MainViewF.Optionables.OptionsOnTop)
//						_foptions.Owner = ObserverManager.RouteView;

					OptionsManager.Options.Add(_foptions);

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


		#region Update UI (options)
		/// <summary>
		/// Sets the color of datafield texts to
		/// <c><see cref="RouteViewOptionables.FieldsForecolor">RouteViewOptionables.FieldsForecolor</see></c>.
		/// </summary>
		internal void SetFieldsForecolor()
		{
			gb_TileData      .ForeColor =
			gb_NodeData      .ForeColor =
			gb_LinkData      .ForeColor =
			gb_NodeEditor    .ForeColor =
			gb_NoderankColors.ForeColor = Optionables.FieldsForecolor;

			Button bu;
			foreach (var control in gb_LinkData.Controls)
			{
				if ((bu = control as Button) != null)
					bu.ForeColor = SystemColors.ControlText;
			}

			foreach (var control in gb_NodeEditor.Controls)
			{
				if ((bu = control as Button) != null)
					bu.ForeColor = SystemColors.ControlText;
			}
		}

		/// <summary>
		/// Sets the noderank colors in <c><see cref="gb_NoderankColors"/></c>.
		/// </summary>
		internal void SetNodeColors()
		{
			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)0)
				pa_ColorRank0.BackColor = Optionables.NodeColor0;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)1)
				pa_ColorRank1.BackColor = Optionables.NodeColor1;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)2)
				pa_ColorRank2.BackColor = Optionables.NodeColor2;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)3)
				pa_ColorRank3.BackColor = Optionables.NodeColor3;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)4)
				pa_ColorRank4.BackColor = Optionables.NodeColor4;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)5)
				pa_ColorRank5.BackColor = Optionables.NodeColor5;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)6)
				pa_ColorRank6.BackColor = Optionables.NodeColor6;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)7)
				pa_ColorRank7.BackColor = Optionables.NodeColor7;

			if (NoderankHighlighted == Byte.MaxValue || NoderankHighlighted == (byte)8)
				pa_ColorRank8.BackColor = Optionables.NodeColor8;
		}
		#endregion Update UI (options)


		#region Update UI (options)(static)
		/// <summary>
		/// Sets the color of the datafields background to
		/// <c><see cref="RouteViewOptionables.FieldsBackcolor">RouteViewOptionables.FieldsBackcolor</see></c>.
		/// </summary>
		internal static void SetFieldsBackcolor()
		{
			ObserverManager.RouteView   .Control     .pa_DataFields.BackColor =
			ObserverManager.TopRouteView.ControlRoute.pa_DataFields.BackColor = Optionables.FieldsBackcolor;
		}

		/// <summary>
		/// Sets the color of highlighted text to
		/// <c><see cref="RouteViewOptionables.FieldsForecolorHighlight">RouteViewOptionables.FieldsForecolorHighlight</see></c>.
		/// </summary>
		internal static void SetFieldsForecolorHighlight()
		{
			ObserverManager.RouteView   .Control     .bu_Save.ForeColor =
			ObserverManager.TopRouteView.ControlRoute.bu_Save.ForeColor = Optionables.FieldsForecolorHighlight;

			if (NodeSelected != null)
				UpdateNodeInfo(); // TODO: update 'link out of bounds' textcolor only
		}

		/// <summary>
		/// Sets the currently selected tile-info's textcolor when the Option
		/// changes.
		/// </summary>
		/// <param name="updatenodeinfo"><c>true</c> to update node-info</param>
		/// <remarks>Called by Options only.</remarks>
		internal static void SetSelectedInfoColor(bool updatenodeinfo = false)
		{
			ObserverManager.RouteView   .Control     .la_Selected.ForeColor =
			ObserverManager.TopRouteView.ControlRoute.la_Selected.ForeColor = GetNodeColor(NodeSelected);

			if (updatenodeinfo && NodeSelected != null)
				UpdateNodeInfo(); // TODO: update link texts' textcolor only
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static Color GetNodeColor(RouteNode node)
		{
			if (node != null)
			{
				switch (node.Rank)
				{
					case 0: return Optionables.NodeColor0;
					case 1: return Optionables.NodeColor1;
					case 2: return Optionables.NodeColor2;
					case 3: return Optionables.NodeColor3;
					case 4: return Optionables.NodeColor4;
					case 5: return Optionables.NodeColor5;
					case 6: return Optionables.NodeColor6;
					case 7: return Optionables.NodeColor7;
					case 8: return Optionables.NodeColor8;

					default: return Optionables.NodeColorInvalid; // case 9 OobRank. See RouteNode.cTor[0]
				}
			}
			return Optionables.FieldsForecolor;
		}
		#endregion Update UI (options)(static)


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
