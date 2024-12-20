using System;
using System.ComponentModel;
using System.Windows.Forms;

using DSShared.Controls;


namespace MapView.Forms.Observers
{
	internal sealed partial class RouteView
	{
		#region Designer
		private IContainer components;

		private ToolStripOneclick ts_Main;

		private ToolStripDropDownButton tsddb_File;
		private ToolStripMenuItem tsmi_Export;
		private ToolStripMenuItem tsmi_Import;

		private ToolStripDropDownButton tsddb_Edit;
		private ToolStripMenuItem tsmi_RaiseNode;
		private ToolStripMenuItem tsmi_LowerNode;
		private ToolStripSeparator tss_1;
		private ToolStripMenuItem tsmi_ClearLinks;
		private ToolStripSeparator tss_2;
		private ToolStripMenuItem tsmi_ZeroUnittypes;
		private ToolStripMenuItem tsmi_ZeroNoderanks;
		private ToolStripMenuItem tsmi_ZeroSpawnweights;
		private ToolStripMenuItem tsmi_ZeroPatrolpriorities;
		private ToolStripMenuItem tsmi_ZeroBaseattacks;
		private ToolStripSeparator tss_3;
		private ToolStripMenuItem tsmi_RecalcDist;

		private ToolStripDropDownButton tsddb_Debug;
		private ToolStripMenuItem tsmi_TestPositions;
		private ToolStripMenuItem tsmi_TestNodeRanks;

		private ToolStripDropDownButton tsddb_Highlights;
		private ToolStripMenuItem tsmi_SpawnHighlight;
		private ToolStripSeparator tss_5;
		private ToolStripMenuItem tsmi_Noderank0;
		private ToolStripMenuItem tsmi_Noderank1;
		private ToolStripMenuItem tsmi_Noderank2;
		private ToolStripMenuItem tsmi_Noderank3;
		private ToolStripMenuItem tsmi_Noderank4;
		private ToolStripMenuItem tsmi_Noderank5;
		private ToolStripMenuItem tsmi_Noderank6;
		private ToolStripMenuItem tsmi_Noderank7;
		private ToolStripMenuItem tsmi_Noderank8;
		private ToolStripSeparator tss_6;
		private ToolStripMenuItem tsmi_NoderankClear;

		private ToolStripSeparator tss_0;

		private ToolStripButton tsb_connect0;
		private ToolStripButton tsb_connect1;
		private ToolStripButton tsb_connect2;

		private ToolStripSeparator tss_4;

		private ToolStripButton tsb_x2;

		private ToolStripSeparator tss_7;

		private ToolStripTextBox tstb_Goto;
		private ToolStripButton tsb_Gotoup;
		private ToolStripButton tsb_Gotodn;

		private ToolStripButton tsb_Options;

		private Panel pa_DataFields;
		private Panel pa_DataFieldsLeft;

		private GroupBox gb_TileData;
		private Label la_Selected;
		private Label la_Over;

		private GroupBox gb_NodeData;
		private Label la_UnitType;
		private Label la_SpawnRank;
		private Label la_SpawnWeight;
		private Label la_Priority;
		private Label la_Attack;
		private ComboBox co_Type;
		private ComboBox co_Rank;
		private ComboBox co_Spawn;
		private ComboBox co_Patrol;
		private ComboBox co_Attack;

		private GroupBox gb_LinkData;
		private Label la_Dest;
		private Label la_UnitInfo;
		private Label la_Dist;
		private Label la_Link1;
		private Label la_Link2;
		private Label la_Link3;
		private Label la_Link4;
		private Label la_Link5;
		private ComboBox co_Link1Dest;
		private ComboBox co_Link2Dest;
		private ComboBox co_Link3Dest;
		private ComboBox co_Link4Dest;
		private ComboBox co_Link5Dest;
		private ComboBox co_Link1UnitType;
		private ComboBox co_Link2UnitType;
		private ComboBox co_Link3UnitType;
		private ComboBox co_Link4UnitType;
		private ComboBox co_Link5UnitType;
		private Label la_Link1Dist;
		private Label la_Link2Dist;
		private Label la_Link3Dist;
		private Label la_Link4Dist;
		private Label la_Link5Dist;
		private Button bu_GoLink1;
		private Button bu_GoLink2;
		private Button bu_GoLink3;
		private Button bu_GoLink4;
		private Button bu_GoLink5;
		private Button bu_Og;

		private Button bu_Tallyho;

		private GroupBox gb_NodeEditor;
		private Button bu_Cut;
		private Button bu_Copy;
		private Button bu_Paste;
		private Button bu_Delete;

		private Button bu_Save;

		private GroupBox gb_NoderankColors;
		private Panel pa_ColorRank0;
		private Label la_ColorRank0;
		private Panel pa_ColorRank1;
		private Label la_ColorRank1;
		private Panel pa_ColorRank2;
		private Label la_ColorRank2;
		private Panel pa_ColorRank3;
		private Label la_ColorRank3;
		private Panel pa_ColorRank4;
		private Label la_ColorRank4;
		private Panel pa_ColorRank5;
		private Label la_ColorRank5;
		private Panel pa_ColorRank6;
		private Label la_ColorRank6;
		private Panel pa_ColorRank7;
		private Label la_ColorRank7;
		private Panel pa_ColorRank8;
		private Label la_ColorRank8;

		private ToolTip toolTip1;


		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.ts_Main = new DSShared.Controls.ToolStripOneclick();
			this.tsddb_File = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmi_Export = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Import = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Edit = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmi_RaiseNode = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_LowerNode = new System.Windows.Forms.ToolStripMenuItem();
			this.tss_1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmi_ClearLinks = new System.Windows.Forms.ToolStripMenuItem();
			this.tss_2 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmi_ZeroUnittypes = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_ZeroNoderanks = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_ZeroSpawnweights = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_ZeroPatrolpriorities = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_ZeroBaseattacks = new System.Windows.Forms.ToolStripMenuItem();
			this.tss_3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmi_RecalcDist = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Debug = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmi_TestPositions = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_TestNodeRanks = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Highlights = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmi_SpawnHighlight = new System.Windows.Forms.ToolStripMenuItem();
			this.tss_5 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmi_Noderank0 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank1 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank2 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank3 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank4 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank5 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank6 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank7 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Noderank8 = new System.Windows.Forms.ToolStripMenuItem();
			this.tss_6 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmi_NoderankClear = new System.Windows.Forms.ToolStripMenuItem();
			this.tsb_Options = new System.Windows.Forms.ToolStripButton();
			this.tss_0 = new System.Windows.Forms.ToolStripSeparator();
			this.tsb_connect0 = new System.Windows.Forms.ToolStripButton();
			this.tsb_connect1 = new System.Windows.Forms.ToolStripButton();
			this.tsb_connect2 = new System.Windows.Forms.ToolStripButton();
			this.tss_4 = new System.Windows.Forms.ToolStripSeparator();
			this.tsb_x2 = new System.Windows.Forms.ToolStripButton();
			this.tss_7 = new System.Windows.Forms.ToolStripSeparator();
			this.tsb_Gotoup = new System.Windows.Forms.ToolStripButton();
			this.tstb_Goto = new System.Windows.Forms.ToolStripTextBox();
			this.tsb_Gotodn = new System.Windows.Forms.ToolStripButton();
			this._pnlRoutes = new DSShared.Controls.CompositedPanel();
			this.pa_DataFields = new System.Windows.Forms.Panel();
			this.pa_DataFieldsLeft = new System.Windows.Forms.Panel();
			this.gb_NodeData = new System.Windows.Forms.GroupBox();
			this.la_UnitType = new System.Windows.Forms.Label();
			this.la_SpawnRank = new System.Windows.Forms.Label();
			this.la_SpawnWeight = new System.Windows.Forms.Label();
			this.la_Priority = new System.Windows.Forms.Label();
			this.la_Attack = new System.Windows.Forms.Label();
			this.co_Type = new System.Windows.Forms.ComboBox();
			this.co_Rank = new System.Windows.Forms.ComboBox();
			this.co_Spawn = new System.Windows.Forms.ComboBox();
			this.co_Patrol = new System.Windows.Forms.ComboBox();
			this.co_Attack = new System.Windows.Forms.ComboBox();
			this.gb_TileData = new System.Windows.Forms.GroupBox();
			this.la_Selected = new System.Windows.Forms.Label();
			this.la_Over = new System.Windows.Forms.Label();
			this.gb_LinkData = new System.Windows.Forms.GroupBox();
			this.la_Dest = new System.Windows.Forms.Label();
			this.la_UnitInfo = new System.Windows.Forms.Label();
			this.la_Dist = new System.Windows.Forms.Label();
			this.la_Link1 = new System.Windows.Forms.Label();
			this.la_Link2 = new System.Windows.Forms.Label();
			this.la_Link3 = new System.Windows.Forms.Label();
			this.la_Link4 = new System.Windows.Forms.Label();
			this.la_Link5 = new System.Windows.Forms.Label();
			this.co_Link1Dest = new System.Windows.Forms.ComboBox();
			this.co_Link2Dest = new System.Windows.Forms.ComboBox();
			this.co_Link3Dest = new System.Windows.Forms.ComboBox();
			this.co_Link4Dest = new System.Windows.Forms.ComboBox();
			this.co_Link5Dest = new System.Windows.Forms.ComboBox();
			this.co_Link1UnitType = new System.Windows.Forms.ComboBox();
			this.co_Link2UnitType = new System.Windows.Forms.ComboBox();
			this.co_Link3UnitType = new System.Windows.Forms.ComboBox();
			this.co_Link4UnitType = new System.Windows.Forms.ComboBox();
			this.co_Link5UnitType = new System.Windows.Forms.ComboBox();
			this.la_Link1Dist = new System.Windows.Forms.Label();
			this.la_Link2Dist = new System.Windows.Forms.Label();
			this.la_Link3Dist = new System.Windows.Forms.Label();
			this.la_Link4Dist = new System.Windows.Forms.Label();
			this.la_Link5Dist = new System.Windows.Forms.Label();
			this.bu_GoLink1 = new System.Windows.Forms.Button();
			this.bu_GoLink2 = new System.Windows.Forms.Button();
			this.bu_GoLink3 = new System.Windows.Forms.Button();
			this.bu_GoLink4 = new System.Windows.Forms.Button();
			this.bu_GoLink5 = new System.Windows.Forms.Button();
			this.bu_Og = new System.Windows.Forms.Button();
			this.bu_Tallyho = new System.Windows.Forms.Button();
			this.gb_NodeEditor = new System.Windows.Forms.GroupBox();
			this.bu_Cut = new System.Windows.Forms.Button();
			this.bu_Copy = new System.Windows.Forms.Button();
			this.bu_Paste = new System.Windows.Forms.Button();
			this.bu_Delete = new System.Windows.Forms.Button();
			this.bu_Save = new System.Windows.Forms.Button();
			this.gb_NoderankColors = new System.Windows.Forms.GroupBox();
			this.la_ColorRank8 = new System.Windows.Forms.Label();
			this.pa_ColorRank8 = new System.Windows.Forms.Panel();
			this.la_ColorRank7 = new System.Windows.Forms.Label();
			this.pa_ColorRank7 = new System.Windows.Forms.Panel();
			this.la_ColorRank6 = new System.Windows.Forms.Label();
			this.pa_ColorRank6 = new System.Windows.Forms.Panel();
			this.la_ColorRank5 = new System.Windows.Forms.Label();
			this.pa_ColorRank5 = new System.Windows.Forms.Panel();
			this.la_ColorRank4 = new System.Windows.Forms.Label();
			this.pa_ColorRank4 = new System.Windows.Forms.Panel();
			this.la_ColorRank3 = new System.Windows.Forms.Label();
			this.pa_ColorRank3 = new System.Windows.Forms.Panel();
			this.la_ColorRank2 = new System.Windows.Forms.Label();
			this.pa_ColorRank2 = new System.Windows.Forms.Panel();
			this.la_ColorRank1 = new System.Windows.Forms.Label();
			this.pa_ColorRank1 = new System.Windows.Forms.Panel();
			this.la_ColorRank0 = new System.Windows.Forms.Label();
			this.pa_ColorRank0 = new System.Windows.Forms.Panel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.ts_Main.SuspendLayout();
			this.pa_DataFields.SuspendLayout();
			this.pa_DataFieldsLeft.SuspendLayout();
			this.gb_NodeData.SuspendLayout();
			this.gb_TileData.SuspendLayout();
			this.gb_LinkData.SuspendLayout();
			this.gb_NodeEditor.SuspendLayout();
			this.gb_NoderankColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// ts_Main
			// 
			this.ts_Main.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ts_Main.GripMargin = new System.Windows.Forms.Padding(0);
			this.ts_Main.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ts_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddb_File,
			this.tsddb_Edit,
			this.tsddb_Debug,
			this.tsddb_Highlights,
			this.tsb_Options,
			this.tss_0,
			this.tsb_connect0,
			this.tsb_connect1,
			this.tsb_connect2,
			this.tss_4,
			this.tsb_x2,
			this.tss_7,
			this.tsb_Gotoup,
			this.tstb_Goto,
			this.tsb_Gotodn});
			this.ts_Main.Location = new System.Drawing.Point(0, 0);
			this.ts_Main.Name = "ts_Main";
			this.ts_Main.Size = new System.Drawing.Size(800, 25);
			this.ts_Main.TabIndex = 0;
			this.ts_Main.TabStop = true;
			this.ts_Main.Text = "ts_Main";
			// 
			// tsddb_File
			// 
			this.tsddb_File.AutoToolTip = false;
			this.tsddb_File.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmi_Export,
			this.tsmi_Import});
			this.tsddb_File.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_File.Margin = new System.Windows.Forms.Padding(3, 1, 0, 1);
			this.tsddb_File.Name = "tsddb_File";
			this.tsddb_File.Size = new System.Drawing.Size(37, 23);
			this.tsddb_File.Text = "&File";
			this.tsddb_File.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsmi_Export
			// 
			this.tsmi_Export.Name = "tsmi_Export";
			this.tsmi_Export.Size = new System.Drawing.Size(124, 22);
			this.tsmi_Export.Text = "&Export ...";
			this.tsmi_Export.Click += new System.EventHandler(this.OnExportClick);
			// 
			// tsmi_Import
			// 
			this.tsmi_Import.Name = "tsmi_Import";
			this.tsmi_Import.Size = new System.Drawing.Size(124, 22);
			this.tsmi_Import.Text = "&Import ...";
			this.tsmi_Import.Click += new System.EventHandler(this.OnImportClick);
			// 
			// tsddb_Edit
			// 
			this.tsddb_Edit.AutoToolTip = false;
			this.tsddb_Edit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmi_RaiseNode,
			this.tsmi_LowerNode,
			this.tss_1,
			this.tsmi_ClearLinks,
			this.tss_2,
			this.tsmi_ZeroUnittypes,
			this.tsmi_ZeroNoderanks,
			this.tsmi_ZeroSpawnweights,
			this.tsmi_ZeroPatrolpriorities,
			this.tsmi_ZeroBaseattacks,
			this.tss_3,
			this.tsmi_RecalcDist});
			this.tsddb_Edit.Font = new System.Drawing.Font("Verdana", 7F);
			this.tsddb_Edit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_Edit.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsddb_Edit.Name = "tsddb_Edit";
			this.tsddb_Edit.Size = new System.Drawing.Size(38, 23);
			this.tsddb_Edit.Text = "&Edit";
			this.tsddb_Edit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsddb_Edit.DropDownOpening += new System.EventHandler(this.OnEditOpening);
			// 
			// tsmi_RaiseNode
			// 
			this.tsmi_RaiseNode.Name = "tsmi_RaiseNode";
			this.tsmi_RaiseNode.Size = new System.Drawing.Size(211, 22);
			this.tsmi_RaiseNode.Text = "node &up 1 level";
			this.tsmi_RaiseNode.Click += new System.EventHandler(this.OnNodeRaise);
			// 
			// tsmi_LowerNode
			// 
			this.tsmi_LowerNode.Name = "tsmi_LowerNode";
			this.tsmi_LowerNode.Size = new System.Drawing.Size(211, 22);
			this.tsmi_LowerNode.Text = "node &down 1 level";
			this.tsmi_LowerNode.Click += new System.EventHandler(this.OnNodeLower);
			// 
			// tss_1
			// 
			this.tss_1.Name = "tss_1";
			this.tss_1.Size = new System.Drawing.Size(208, 6);
			// 
			// tsmi_ClearLinks
			// 
			this.tsmi_ClearLinks.Name = "tsmi_ClearLinks";
			this.tsmi_ClearLinks.Size = new System.Drawing.Size(211, 22);
			this.tsmi_ClearLinks.Text = "&clear current Link data ...";
			this.tsmi_ClearLinks.Click += new System.EventHandler(this.OnClearLinksClick);
			// 
			// tss_2
			// 
			this.tss_2.Name = "tss_2";
			this.tss_2.Size = new System.Drawing.Size(208, 6);
			// 
			// tsmi_ZeroUnittypes
			// 
			this.tsmi_ZeroUnittypes.Name = "tsmi_ZeroUnittypes";
			this.tsmi_ZeroUnittypes.Size = new System.Drawing.Size(211, 22);
			this.tsmi_ZeroUnittypes.Text = "zero all unittypes ...";
			this.tsmi_ZeroUnittypes.Click += new System.EventHandler(this.OnZeroUnittypesClick);
			// 
			// tsmi_ZeroNoderanks
			// 
			this.tsmi_ZeroNoderanks.Name = "tsmi_ZeroNoderanks";
			this.tsmi_ZeroNoderanks.Size = new System.Drawing.Size(211, 22);
			this.tsmi_ZeroNoderanks.Text = "zero all noderanks ...";
			this.tsmi_ZeroNoderanks.Click += new System.EventHandler(this.OnZeroNoderanksClick);
			// 
			// tsmi_ZeroSpawnweights
			// 
			this.tsmi_ZeroSpawnweights.Name = "tsmi_ZeroSpawnweights";
			this.tsmi_ZeroSpawnweights.Size = new System.Drawing.Size(211, 22);
			this.tsmi_ZeroSpawnweights.Text = "zero all spawnweights ...";
			this.tsmi_ZeroSpawnweights.Click += new System.EventHandler(this.OnZeroSpawnweightsClick);
			// 
			// tsmi_ZeroPatrolpriorities
			// 
			this.tsmi_ZeroPatrolpriorities.Name = "tsmi_ZeroPatrolpriorities";
			this.tsmi_ZeroPatrolpriorities.Size = new System.Drawing.Size(211, 22);
			this.tsmi_ZeroPatrolpriorities.Text = "zero all patrolpriorities ...";
			this.tsmi_ZeroPatrolpriorities.Click += new System.EventHandler(this.OnZeroPatrolprioritiesClick);
			// 
			// tsmi_ZeroBaseattacks
			// 
			this.tsmi_ZeroBaseattacks.Name = "tsmi_ZeroBaseattacks";
			this.tsmi_ZeroBaseattacks.Size = new System.Drawing.Size(211, 22);
			this.tsmi_ZeroBaseattacks.Text = "zero all baseattacks ...";
			this.tsmi_ZeroBaseattacks.Click += new System.EventHandler(this.OnZeroBaseattacksClick);
			// 
			// tss_3
			// 
			this.tss_3.Name = "tss_3";
			this.tss_3.Size = new System.Drawing.Size(208, 6);
			// 
			// tsmi_RecalcDist
			// 
			this.tsmi_RecalcDist.Name = "tsmi_RecalcDist";
			this.tsmi_RecalcDist.Size = new System.Drawing.Size(211, 22);
			this.tsmi_RecalcDist.Text = "&update all Link distances";
			this.tsmi_RecalcDist.Click += new System.EventHandler(this.OnRecalculateDistanceClick);
			// 
			// tsddb_Debug
			// 
			this.tsddb_Debug.AutoToolTip = false;
			this.tsddb_Debug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Debug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmi_TestPositions,
			this.tsmi_TestNodeRanks});
			this.tsddb_Debug.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_Debug.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsddb_Debug.Name = "tsddb_Debug";
			this.tsddb_Debug.Size = new System.Drawing.Size(42, 23);
			this.tsddb_Debug.Text = "&Test";
			this.tsddb_Debug.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsmi_TestPositions
			// 
			this.tsmi_TestPositions.Name = "tsmi_TestPositions";
			this.tsmi_TestPositions.Size = new System.Drawing.Size(177, 22);
			this.tsmi_TestPositions.Text = "test node &positions";
			this.tsmi_TestPositions.Click += new System.EventHandler(this.OnTestPositionsClick);
			// 
			// tsmi_TestNodeRanks
			// 
			this.tsmi_TestNodeRanks.Name = "tsmi_TestNodeRanks";
			this.tsmi_TestNodeRanks.Size = new System.Drawing.Size(177, 22);
			this.tsmi_TestNodeRanks.Text = "test node &ranks";
			this.tsmi_TestNodeRanks.Click += new System.EventHandler(this.OnTestNoderanksClick);
			// 
			// tsddb_Highlights
			// 
			this.tsddb_Highlights.AutoToolTip = false;
			this.tsddb_Highlights.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Highlights.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmi_SpawnHighlight,
			this.tss_5,
			this.tsmi_Noderank0,
			this.tsmi_Noderank1,
			this.tsmi_Noderank2,
			this.tsmi_Noderank3,
			this.tsmi_Noderank4,
			this.tsmi_Noderank5,
			this.tsmi_Noderank6,
			this.tsmi_Noderank7,
			this.tsmi_Noderank8,
			this.tss_6,
			this.tsmi_NoderankClear});
			this.tsddb_Highlights.Font = new System.Drawing.Font("Verdana", 7F);
			this.tsddb_Highlights.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_Highlights.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsddb_Highlights.Name = "tsddb_Highlights";
			this.tsddb_Highlights.Size = new System.Drawing.Size(73, 23);
			this.tsddb_Highlights.Text = "Highlights";
			this.tsddb_Highlights.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsmi_SpawnHighlight
			// 
			this.tsmi_SpawnHighlight.Name = "tsmi_SpawnHighlight";
			this.tsmi_SpawnHighlight.ShortcutKeyDisplayString = "Ctrl+h";
			this.tsmi_SpawnHighlight.Size = new System.Drawing.Size(209, 22);
			this.tsmi_SpawnHighlight.Text = "Spawn nodes only";
			this.tsmi_SpawnHighlight.Click += new System.EventHandler(this.OnSpawnHighlightClick);
			// 
			// tss_5
			// 
			this.tss_5.Name = "tss_5";
			this.tss_5.Size = new System.Drawing.Size(206, 6);
			// 
			// tsmi_Noderank0
			// 
			this.tsmi_Noderank0.Name = "tsmi_Noderank0";
			this.tsmi_Noderank0.ShortcutKeyDisplayString = "0";
			this.tsmi_Noderank0.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank0.Text = "Rank0";
			this.tsmi_Noderank0.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank1
			// 
			this.tsmi_Noderank1.Name = "tsmi_Noderank1";
			this.tsmi_Noderank1.ShortcutKeyDisplayString = "1";
			this.tsmi_Noderank1.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank1.Text = "Rank1";
			this.tsmi_Noderank1.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank2
			// 
			this.tsmi_Noderank2.Name = "tsmi_Noderank2";
			this.tsmi_Noderank2.ShortcutKeyDisplayString = "2";
			this.tsmi_Noderank2.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank2.Text = "Rank2";
			this.tsmi_Noderank2.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank3
			// 
			this.tsmi_Noderank3.Name = "tsmi_Noderank3";
			this.tsmi_Noderank3.ShortcutKeyDisplayString = "3";
			this.tsmi_Noderank3.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank3.Text = "Rank3";
			this.tsmi_Noderank3.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank4
			// 
			this.tsmi_Noderank4.Name = "tsmi_Noderank4";
			this.tsmi_Noderank4.ShortcutKeyDisplayString = "4";
			this.tsmi_Noderank4.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank4.Text = "Rank4";
			this.tsmi_Noderank4.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank5
			// 
			this.tsmi_Noderank5.Name = "tsmi_Noderank5";
			this.tsmi_Noderank5.ShortcutKeyDisplayString = "5";
			this.tsmi_Noderank5.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank5.Text = "Rank5";
			this.tsmi_Noderank5.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank6
			// 
			this.tsmi_Noderank6.Name = "tsmi_Noderank6";
			this.tsmi_Noderank6.ShortcutKeyDisplayString = "6";
			this.tsmi_Noderank6.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank6.Text = "Rank6";
			this.tsmi_Noderank6.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank7
			// 
			this.tsmi_Noderank7.Name = "tsmi_Noderank7";
			this.tsmi_Noderank7.ShortcutKeyDisplayString = "7";
			this.tsmi_Noderank7.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank7.Text = "Rank7";
			this.tsmi_Noderank7.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tsmi_Noderank8
			// 
			this.tsmi_Noderank8.Name = "tsmi_Noderank8";
			this.tsmi_Noderank8.ShortcutKeyDisplayString = "8";
			this.tsmi_Noderank8.Size = new System.Drawing.Size(209, 22);
			this.tsmi_Noderank8.Text = "Rank8";
			this.tsmi_Noderank8.Click += new System.EventHandler(this.OnRankHighlightClick_it);
			// 
			// tss_6
			// 
			this.tss_6.Name = "tss_6";
			this.tss_6.Size = new System.Drawing.Size(206, 6);
			// 
			// tsmi_NoderankClear
			// 
			this.tsmi_NoderankClear.Enabled = false;
			this.tsmi_NoderankClear.Name = "tsmi_NoderankClear";
			this.tsmi_NoderankClear.Size = new System.Drawing.Size(209, 22);
			this.tsmi_NoderankClear.Text = "clear Rank highlights";
			this.tsmi_NoderankClear.Click += new System.EventHandler(this.OnClearRankHighlightsClick);
			// 
			// tsb_Options
			// 
			this.tsb_Options.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.tsb_Options.AutoToolTip = false;
			this.tsb_Options.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsb_Options.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_Options.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this.tsb_Options.Name = "tsb_Options";
			this.tsb_Options.Size = new System.Drawing.Size(52, 23);
			this.tsb_Options.Text = "&Options";
			this.tsb_Options.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// tss_0
			// 
			this.tss_0.Name = "tss_0";
			this.tss_0.Size = new System.Drawing.Size(6, 25);
			// 
			// tsb_connect0
			// 
			this.tsb_connect0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_connect0.Image = global::MapView.Properties.Resources.connect_0;
			this.tsb_connect0.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_connect0.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
			this.tsb_connect0.Name = "tsb_connect0";
			this.tsb_connect0.Size = new System.Drawing.Size(23, 23);
			this.tsb_connect0.Text = "auto-link off";
			this.tsb_connect0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_connect0.ToolTipText = "auto-link off";
			this.tsb_connect0.Click += new System.EventHandler(this.OnConnectTypeClicked);
			// 
			// tsb_connect1
			// 
			this.tsb_connect1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_connect1.Image = global::MapView.Properties.Resources.connect_1;
			this.tsb_connect1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_connect1.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsb_connect1.Name = "tsb_connect1";
			this.tsb_connect1.Size = new System.Drawing.Size(23, 23);
			this.tsb_connect1.Text = "link forward";
			this.tsb_connect1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_connect1.ToolTipText = "link forward";
			this.tsb_connect1.Click += new System.EventHandler(this.OnConnectTypeClicked);
			// 
			// tsb_connect2
			// 
			this.tsb_connect2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_connect2.Image = global::MapView.Properties.Resources.connect_2;
			this.tsb_connect2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_connect2.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsb_connect2.Name = "tsb_connect2";
			this.tsb_connect2.Size = new System.Drawing.Size(23, 23);
			this.tsb_connect2.Text = "link forward and backward";
			this.tsb_connect2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_connect2.ToolTipText = "link forward and back";
			this.tsb_connect2.Click += new System.EventHandler(this.OnConnectTypeClicked);
			// 
			// tss_4
			// 
			this.tss_4.Name = "tss_4";
			this.tss_4.Size = new System.Drawing.Size(6, 25);
			// 
			// tsb_x2
			// 
			this.tsb_x2.CheckOnClick = true;
			this.tsb_x2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsb_x2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_x2.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
			this.tsb_x2.Name = "tsb_x2";
			this.tsb_x2.Size = new System.Drawing.Size(23, 23);
			this.tsb_x2.Text = "x2";
			this.tsb_x2.ToolTipText = "[Ctrl]+LMB - center on cursor";
			this.tsb_x2.Click += new System.EventHandler(this.OnScaleClick);
			// 
			// tss_7
			// 
			this.tss_7.Name = "tss_7";
			this.tss_7.Size = new System.Drawing.Size(6, 25);
			// 
			// tsb_Gotoup
			// 
			this.tsb_Gotoup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_Gotoup.Image = global::MapView.Properties.Resources.up;
			this.tsb_Gotoup.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_Gotoup.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsb_Gotoup.Name = "tsb_Gotoup";
			this.tsb_Gotoup.Size = new System.Drawing.Size(23, 23);
			this.tsb_Gotoup.Click += new System.EventHandler(this.OnClick_Gotobtn);
			// 
			// tstb_Goto
			// 
			this.tstb_Goto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tstb_Goto.Font = new System.Drawing.Font("Consolas", 7.5F);
			this.tstb_Goto.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tstb_Goto.Name = "tstb_Goto";
			this.tstb_Goto.Size = new System.Drawing.Size(27, 23);
			this.tstb_Goto.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tstb_Goto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown_Goto);
			// 
			// tsb_Gotodn
			// 
			this.tsb_Gotodn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_Gotodn.Image = global::MapView.Properties.Resources.down;
			this.tsb_Gotodn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_Gotodn.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
			this.tsb_Gotodn.Name = "tsb_Gotodn";
			this.tsb_Gotodn.Size = new System.Drawing.Size(23, 23);
			this.tsb_Gotodn.Click += new System.EventHandler(this.OnClick_Gotobtn);
			// 
			// _pnlRoutes
			// 
			this._pnlRoutes.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pnlRoutes.Location = new System.Drawing.Point(0, 25);
			this._pnlRoutes.Margin = new System.Windows.Forms.Padding(0);
			this._pnlRoutes.Name = "_pnlRoutes";
			this._pnlRoutes.Size = new System.Drawing.Size(800, 250);
			this._pnlRoutes.TabIndex = 1;
			// 
			// pa_DataFields
			// 
			this.pa_DataFields.Controls.Add(this.pa_DataFieldsLeft);
			this.pa_DataFields.Controls.Add(this.gb_LinkData);
			this.pa_DataFields.Controls.Add(this.bu_Og);
			this.pa_DataFields.Controls.Add(this.bu_Tallyho);
			this.pa_DataFields.Controls.Add(this.gb_NodeEditor);
			this.pa_DataFields.Controls.Add(this.bu_Save);
			this.pa_DataFields.Controls.Add(this.gb_NoderankColors);
			this.pa_DataFields.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pa_DataFields.Location = new System.Drawing.Point(0, 275);
			this.pa_DataFields.Margin = new System.Windows.Forms.Padding(0);
			this.pa_DataFields.Name = "pa_DataFields";
			this.pa_DataFields.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.pa_DataFields.Size = new System.Drawing.Size(800, 205);
			this.pa_DataFields.TabIndex = 2;
			// 
			// pa_DataFieldsLeft
			// 
			this.pa_DataFieldsLeft.Controls.Add(this.gb_NodeData);
			this.pa_DataFieldsLeft.Controls.Add(this.gb_TileData);
			this.pa_DataFieldsLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pa_DataFieldsLeft.Location = new System.Drawing.Point(0, 3);
			this.pa_DataFieldsLeft.Margin = new System.Windows.Forms.Padding(0);
			this.pa_DataFieldsLeft.Name = "pa_DataFieldsLeft";
			this.pa_DataFieldsLeft.Size = new System.Drawing.Size(245, 202);
			this.pa_DataFieldsLeft.TabIndex = 0;
			// 
			// gb_NodeData
			// 
			this.gb_NodeData.Controls.Add(this.la_UnitType);
			this.gb_NodeData.Controls.Add(this.la_SpawnRank);
			this.gb_NodeData.Controls.Add(this.la_SpawnWeight);
			this.gb_NodeData.Controls.Add(this.la_Priority);
			this.gb_NodeData.Controls.Add(this.la_Attack);
			this.gb_NodeData.Controls.Add(this.co_Type);
			this.gb_NodeData.Controls.Add(this.co_Rank);
			this.gb_NodeData.Controls.Add(this.co_Spawn);
			this.gb_NodeData.Controls.Add(this.co_Patrol);
			this.gb_NodeData.Controls.Add(this.co_Attack);
			this.gb_NodeData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_NodeData.Location = new System.Drawing.Point(0, 50);
			this.gb_NodeData.Margin = new System.Windows.Forms.Padding(0);
			this.gb_NodeData.Name = "gb_NodeData";
			this.gb_NodeData.Padding = new System.Windows.Forms.Padding(0);
			this.gb_NodeData.Size = new System.Drawing.Size(245, 152);
			this.gb_NodeData.TabIndex = 1;
			this.gb_NodeData.TabStop = false;
			this.gb_NodeData.Text = " Node data ";
			// 
			// la_UnitType
			// 
			this.la_UnitType.Location = new System.Drawing.Point(10, 20);
			this.la_UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.la_UnitType.Name = "la_UnitType";
			this.la_UnitType.Size = new System.Drawing.Size(85, 15);
			this.la_UnitType.TabIndex = 0;
			this.la_UnitType.Text = "Unit Type";
			this.la_UnitType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.la_UnitType, "characteristics of units that may patrol (or spawn at) the node");
			// 
			// la_SpawnRank
			// 
			this.la_SpawnRank.Location = new System.Drawing.Point(10, 45);
			this.la_SpawnRank.Margin = new System.Windows.Forms.Padding(0);
			this.la_SpawnRank.Name = "la_SpawnRank";
			this.la_SpawnRank.Size = new System.Drawing.Size(85, 15);
			this.la_SpawnRank.TabIndex = 2;
			this.la_SpawnRank.Text = "Node Rank";
			this.la_SpawnRank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.la_SpawnRank, "faction or rank (if aLiens) that may spawn/patrol here. Nodes for aLiens outside " +
		"their UFO or base are usually set to 0");
			// 
			// la_SpawnWeight
			// 
			this.la_SpawnWeight.Location = new System.Drawing.Point(10, 70);
			this.la_SpawnWeight.Margin = new System.Windows.Forms.Padding(0);
			this.la_SpawnWeight.Name = "la_SpawnWeight";
			this.la_SpawnWeight.Size = new System.Drawing.Size(85, 15);
			this.la_SpawnWeight.TabIndex = 4;
			this.la_SpawnWeight.Text = "Spawn Weight";
			this.la_SpawnWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.la_SpawnWeight, "desire for an aLien to spawn here");
			// 
			// la_Priority
			// 
			this.la_Priority.Location = new System.Drawing.Point(10, 95);
			this.la_Priority.Margin = new System.Windows.Forms.Padding(0);
			this.la_Priority.Name = "la_Priority";
			this.la_Priority.Size = new System.Drawing.Size(85, 15);
			this.la_Priority.TabIndex = 6;
			this.la_Priority.Text = "Patrol Priority";
			this.la_Priority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.la_Priority, "desire for an aLien to patrol here");
			// 
			// la_Attack
			// 
			this.la_Attack.Location = new System.Drawing.Point(10, 120);
			this.la_Attack.Margin = new System.Windows.Forms.Padding(0);
			this.la_Attack.Name = "la_Attack";
			this.la_Attack.Size = new System.Drawing.Size(85, 15);
			this.la_Attack.TabIndex = 8;
			this.la_Attack.Text = "Base Attack";
			this.la_Attack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.la_Attack, "attacts an aLien to shoot at XCom base tiles");
			// 
			// co_Type
			// 
			this.co_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Type.Location = new System.Drawing.Point(99, 15);
			this.co_Type.Margin = new System.Windows.Forms.Padding(0);
			this.co_Type.Name = "co_Type";
			this.co_Type.Size = new System.Drawing.Size(140, 20);
			this.co_Type.TabIndex = 1;
			this.toolTip1.SetToolTip(this.co_Type, "characteristics of units that may patrol (or spawn at) the node");
			this.co_Type.SelectedIndexChanged += new System.EventHandler(this.OnUnitTypeSelectedIndexChanged);
			// 
			// co_Rank
			// 
			this.co_Rank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Rank.Location = new System.Drawing.Point(99, 40);
			this.co_Rank.Margin = new System.Windows.Forms.Padding(0);
			this.co_Rank.Name = "co_Rank";
			this.co_Rank.Size = new System.Drawing.Size(140, 20);
			this.co_Rank.TabIndex = 3;
			this.toolTip1.SetToolTip(this.co_Rank, "faction or rank (if aLiens) that may spawn/patrol here. Nodes for aLiens outside " +
		"their UFO or base are usually set to 0");
			this.co_Rank.SelectedIndexChanged += new System.EventHandler(this.OnNodeRankSelectedIndexChanged);
			// 
			// co_Spawn
			// 
			this.co_Spawn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Spawn.Location = new System.Drawing.Point(99, 65);
			this.co_Spawn.Margin = new System.Windows.Forms.Padding(0);
			this.co_Spawn.Name = "co_Spawn";
			this.co_Spawn.Size = new System.Drawing.Size(140, 20);
			this.co_Spawn.TabIndex = 5;
			this.toolTip1.SetToolTip(this.co_Spawn, "desire for an aLien to spawn here");
			this.co_Spawn.SelectedIndexChanged += new System.EventHandler(this.OnSpawnWeightSelectedIndexChanged);
			// 
			// co_Patrol
			// 
			this.co_Patrol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Patrol.Location = new System.Drawing.Point(99, 90);
			this.co_Patrol.Margin = new System.Windows.Forms.Padding(0);
			this.co_Patrol.Name = "co_Patrol";
			this.co_Patrol.Size = new System.Drawing.Size(140, 20);
			this.co_Patrol.TabIndex = 7;
			this.toolTip1.SetToolTip(this.co_Patrol, "desire for an aLien to patrol here");
			this.co_Patrol.SelectedIndexChanged += new System.EventHandler(this.OnPatrolPrioritySelectedIndexChanged);
			// 
			// co_Attack
			// 
			this.co_Attack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Attack.Location = new System.Drawing.Point(99, 115);
			this.co_Attack.Margin = new System.Windows.Forms.Padding(0);
			this.co_Attack.Name = "co_Attack";
			this.co_Attack.Size = new System.Drawing.Size(140, 20);
			this.co_Attack.TabIndex = 9;
			this.toolTip1.SetToolTip(this.co_Attack, "attacts an aLien to shoot at XCom base tiles");
			this.co_Attack.SelectedIndexChanged += new System.EventHandler(this.OnBaseAttackSelectedIndexChanged);
			// 
			// gb_TileData
			// 
			this.gb_TileData.Controls.Add(this.la_Selected);
			this.gb_TileData.Controls.Add(this.la_Over);
			this.gb_TileData.Dock = System.Windows.Forms.DockStyle.Top;
			this.gb_TileData.Location = new System.Drawing.Point(0, 0);
			this.gb_TileData.Margin = new System.Windows.Forms.Padding(0);
			this.gb_TileData.Name = "gb_TileData";
			this.gb_TileData.Padding = new System.Windows.Forms.Padding(0);
			this.gb_TileData.Size = new System.Drawing.Size(245, 50);
			this.gb_TileData.TabIndex = 0;
			this.gb_TileData.TabStop = false;
			this.gb_TileData.Text = " Tile data ";
			// 
			// la_Selected
			// 
			this.la_Selected.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.la_Selected.Location = new System.Drawing.Point(15, 15);
			this.la_Selected.Margin = new System.Windows.Forms.Padding(0);
			this.la_Selected.Name = "la_Selected";
			this.la_Selected.Size = new System.Drawing.Size(110, 30);
			this.la_Selected.TabIndex = 0;
			this.la_Selected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Over
			// 
			this.la_Over.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.la_Over.Location = new System.Drawing.Point(130, 15);
			this.la_Over.Margin = new System.Windows.Forms.Padding(0);
			this.la_Over.Name = "la_Over";
			this.la_Over.Size = new System.Drawing.Size(110, 30);
			this.la_Over.TabIndex = 1;
			this.la_Over.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gb_LinkData
			// 
			this.gb_LinkData.Controls.Add(this.la_Dest);
			this.gb_LinkData.Controls.Add(this.la_UnitInfo);
			this.gb_LinkData.Controls.Add(this.la_Dist);
			this.gb_LinkData.Controls.Add(this.la_Link1);
			this.gb_LinkData.Controls.Add(this.la_Link2);
			this.gb_LinkData.Controls.Add(this.la_Link3);
			this.gb_LinkData.Controls.Add(this.la_Link4);
			this.gb_LinkData.Controls.Add(this.la_Link5);
			this.gb_LinkData.Controls.Add(this.co_Link1Dest);
			this.gb_LinkData.Controls.Add(this.co_Link2Dest);
			this.gb_LinkData.Controls.Add(this.co_Link3Dest);
			this.gb_LinkData.Controls.Add(this.co_Link4Dest);
			this.gb_LinkData.Controls.Add(this.co_Link5Dest);
			this.gb_LinkData.Controls.Add(this.co_Link1UnitType);
			this.gb_LinkData.Controls.Add(this.co_Link2UnitType);
			this.gb_LinkData.Controls.Add(this.co_Link3UnitType);
			this.gb_LinkData.Controls.Add(this.co_Link4UnitType);
			this.gb_LinkData.Controls.Add(this.co_Link5UnitType);
			this.gb_LinkData.Controls.Add(this.la_Link1Dist);
			this.gb_LinkData.Controls.Add(this.la_Link2Dist);
			this.gb_LinkData.Controls.Add(this.la_Link3Dist);
			this.gb_LinkData.Controls.Add(this.la_Link4Dist);
			this.gb_LinkData.Controls.Add(this.la_Link5Dist);
			this.gb_LinkData.Controls.Add(this.bu_GoLink1);
			this.gb_LinkData.Controls.Add(this.bu_GoLink2);
			this.gb_LinkData.Controls.Add(this.bu_GoLink3);
			this.gb_LinkData.Controls.Add(this.bu_GoLink4);
			this.gb_LinkData.Controls.Add(this.bu_GoLink5);
			this.gb_LinkData.Location = new System.Drawing.Point(245, 3);
			this.gb_LinkData.Margin = new System.Windows.Forms.Padding(0);
			this.gb_LinkData.Name = "gb_LinkData";
			this.gb_LinkData.Padding = new System.Windows.Forms.Padding(0);
			this.gb_LinkData.Size = new System.Drawing.Size(290, 150);
			this.gb_LinkData.TabIndex = 1;
			this.gb_LinkData.TabStop = false;
			this.gb_LinkData.Text = " Link data ";
			// 
			// la_Dest
			// 
			this.la_Dest.Location = new System.Drawing.Point(85, 10);
			this.la_Dest.Margin = new System.Windows.Forms.Padding(0);
			this.la_Dest.Name = "la_Dest";
			this.la_Dest.Size = new System.Drawing.Size(30, 15);
			this.la_Dest.TabIndex = 0;
			this.la_Dest.Text = "Dest";
			// 
			// la_UnitInfo
			// 
			this.la_UnitInfo.Location = new System.Drawing.Point(175, 10);
			this.la_UnitInfo.Margin = new System.Windows.Forms.Padding(0);
			this.la_UnitInfo.Name = "la_UnitInfo";
			this.la_UnitInfo.Size = new System.Drawing.Size(30, 15);
			this.la_UnitInfo.TabIndex = 1;
			this.la_UnitInfo.Text = "Unit Type";
			this.toolTip1.SetToolTip(this.la_UnitInfo, "not used in 0penXcom");
			// 
			// la_Dist
			// 
			this.la_Dist.Location = new System.Drawing.Point(219, 10);
			this.la_Dist.Margin = new System.Windows.Forms.Padding(0);
			this.la_Dist.Name = "la_Dist";
			this.la_Dist.Size = new System.Drawing.Size(30, 15);
			this.la_Dist.TabIndex = 2;
			this.la_Dist.Text = "Dist";
			this.toolTip1.SetToolTip(this.la_Dist, "not used in 0penXcom");
			// 
			// la_Link1
			// 
			this.la_Link1.Location = new System.Drawing.Point(5, 30);
			this.la_Link1.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link1.Name = "la_Link1";
			this.la_Link1.Size = new System.Drawing.Size(35, 15);
			this.la_Link1.TabIndex = 3;
			this.la_Link1.Tag = "L1";
			this.la_Link1.Text = "Link1";
			this.la_Link1.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link2
			// 
			this.la_Link2.Location = new System.Drawing.Point(5, 55);
			this.la_Link2.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link2.Name = "la_Link2";
			this.la_Link2.Size = new System.Drawing.Size(35, 15);
			this.la_Link2.TabIndex = 8;
			this.la_Link2.Tag = "L2";
			this.la_Link2.Text = "Link2";
			this.la_Link2.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link3
			// 
			this.la_Link3.Location = new System.Drawing.Point(5, 80);
			this.la_Link3.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link3.Name = "la_Link3";
			this.la_Link3.Size = new System.Drawing.Size(35, 15);
			this.la_Link3.TabIndex = 13;
			this.la_Link3.Tag = "L3";
			this.la_Link3.Text = "Link3";
			this.la_Link3.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link4
			// 
			this.la_Link4.Location = new System.Drawing.Point(5, 105);
			this.la_Link4.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link4.Name = "la_Link4";
			this.la_Link4.Size = new System.Drawing.Size(35, 15);
			this.la_Link4.TabIndex = 18;
			this.la_Link4.Tag = "L4";
			this.la_Link4.Text = "Link4";
			this.la_Link4.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link5
			// 
			this.la_Link5.Location = new System.Drawing.Point(5, 130);
			this.la_Link5.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link5.Name = "la_Link5";
			this.la_Link5.Size = new System.Drawing.Size(35, 15);
			this.la_Link5.TabIndex = 23;
			this.la_Link5.Tag = "L5";
			this.la_Link5.Text = "Link5";
			this.la_Link5.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// co_Link1Dest
			// 
			this.co_Link1Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link1Dest.Location = new System.Drawing.Point(45, 25);
			this.co_Link1Dest.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link1Dest.Name = "co_Link1Dest";
			this.co_Link1Dest.Size = new System.Drawing.Size(75, 20);
			this.co_Link1Dest.TabIndex = 4;
			this.co_Link1Dest.Tag = "L1";
			this.co_Link1Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.co_Link1Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link1Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link2Dest
			// 
			this.co_Link2Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link2Dest.Location = new System.Drawing.Point(45, 50);
			this.co_Link2Dest.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link2Dest.Name = "co_Link2Dest";
			this.co_Link2Dest.Size = new System.Drawing.Size(75, 20);
			this.co_Link2Dest.TabIndex = 9;
			this.co_Link2Dest.Tag = "L2";
			this.co_Link2Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.co_Link2Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link2Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link3Dest
			// 
			this.co_Link3Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link3Dest.Location = new System.Drawing.Point(45, 75);
			this.co_Link3Dest.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link3Dest.Name = "co_Link3Dest";
			this.co_Link3Dest.Size = new System.Drawing.Size(75, 20);
			this.co_Link3Dest.TabIndex = 14;
			this.co_Link3Dest.Tag = "L3";
			this.co_Link3Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.co_Link3Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link3Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link4Dest
			// 
			this.co_Link4Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link4Dest.Location = new System.Drawing.Point(45, 100);
			this.co_Link4Dest.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link4Dest.Name = "co_Link4Dest";
			this.co_Link4Dest.Size = new System.Drawing.Size(75, 20);
			this.co_Link4Dest.TabIndex = 19;
			this.co_Link4Dest.Tag = "L4";
			this.co_Link4Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.co_Link4Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link4Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link5Dest
			// 
			this.co_Link5Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link5Dest.Location = new System.Drawing.Point(45, 125);
			this.co_Link5Dest.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link5Dest.Name = "co_Link5Dest";
			this.co_Link5Dest.Size = new System.Drawing.Size(75, 20);
			this.co_Link5Dest.TabIndex = 24;
			this.co_Link5Dest.Tag = "L5";
			this.co_Link5Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.co_Link5Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link5Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link1UnitType
			// 
			this.co_Link1UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link1UnitType.Location = new System.Drawing.Point(125, 25);
			this.co_Link1UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link1UnitType.Name = "co_Link1UnitType";
			this.co_Link1UnitType.Size = new System.Drawing.Size(85, 20);
			this.co_Link1UnitType.TabIndex = 5;
			this.co_Link1UnitType.Tag = "L1";
			this.co_Link1UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.co_Link1UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link1UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link2UnitType
			// 
			this.co_Link2UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link2UnitType.Location = new System.Drawing.Point(125, 50);
			this.co_Link2UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link2UnitType.Name = "co_Link2UnitType";
			this.co_Link2UnitType.Size = new System.Drawing.Size(85, 20);
			this.co_Link2UnitType.TabIndex = 10;
			this.co_Link2UnitType.Tag = "L2";
			this.co_Link2UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.co_Link2UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link2UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link3UnitType
			// 
			this.co_Link3UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link3UnitType.Location = new System.Drawing.Point(125, 75);
			this.co_Link3UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link3UnitType.Name = "co_Link3UnitType";
			this.co_Link3UnitType.Size = new System.Drawing.Size(85, 20);
			this.co_Link3UnitType.TabIndex = 15;
			this.co_Link3UnitType.Tag = "L3";
			this.co_Link3UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.co_Link3UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link3UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link4UnitType
			// 
			this.co_Link4UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link4UnitType.Location = new System.Drawing.Point(125, 100);
			this.co_Link4UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link4UnitType.Name = "co_Link4UnitType";
			this.co_Link4UnitType.Size = new System.Drawing.Size(85, 20);
			this.co_Link4UnitType.TabIndex = 20;
			this.co_Link4UnitType.Tag = "L4";
			this.co_Link4UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.co_Link4UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link4UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// co_Link5UnitType
			// 
			this.co_Link5UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.co_Link5UnitType.Location = new System.Drawing.Point(125, 125);
			this.co_Link5UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.co_Link5UnitType.Name = "co_Link5UnitType";
			this.co_Link5UnitType.Size = new System.Drawing.Size(85, 20);
			this.co_Link5UnitType.TabIndex = 25;
			this.co_Link5UnitType.Tag = "L5";
			this.co_Link5UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.co_Link5UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.co_Link5UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// la_Link1Dist
			// 
			this.la_Link1Dist.Location = new System.Drawing.Point(218, 25);
			this.la_Link1Dist.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link1Dist.Name = "la_Link1Dist";
			this.la_Link1Dist.Size = new System.Drawing.Size(36, 20);
			this.la_Link1Dist.TabIndex = 6;
			this.la_Link1Dist.Tag = "L1";
			this.la_Link1Dist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_Link1Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link1Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link2Dist
			// 
			this.la_Link2Dist.Location = new System.Drawing.Point(218, 50);
			this.la_Link2Dist.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link2Dist.Name = "la_Link2Dist";
			this.la_Link2Dist.Size = new System.Drawing.Size(36, 20);
			this.la_Link2Dist.TabIndex = 11;
			this.la_Link2Dist.Tag = "L2";
			this.la_Link2Dist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_Link2Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link2Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link3Dist
			// 
			this.la_Link3Dist.Location = new System.Drawing.Point(218, 75);
			this.la_Link3Dist.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link3Dist.Name = "la_Link3Dist";
			this.la_Link3Dist.Size = new System.Drawing.Size(36, 20);
			this.la_Link3Dist.TabIndex = 16;
			this.la_Link3Dist.Tag = "L3";
			this.la_Link3Dist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_Link3Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link3Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link4Dist
			// 
			this.la_Link4Dist.Location = new System.Drawing.Point(218, 100);
			this.la_Link4Dist.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link4Dist.Name = "la_Link4Dist";
			this.la_Link4Dist.Size = new System.Drawing.Size(36, 20);
			this.la_Link4Dist.TabIndex = 21;
			this.la_Link4Dist.Tag = "L4";
			this.la_Link4Dist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_Link4Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link4Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// la_Link5Dist
			// 
			this.la_Link5Dist.Location = new System.Drawing.Point(218, 125);
			this.la_Link5Dist.Margin = new System.Windows.Forms.Padding(0);
			this.la_Link5Dist.Name = "la_Link5Dist";
			this.la_Link5Dist.Size = new System.Drawing.Size(36, 20);
			this.la_Link5Dist.TabIndex = 26;
			this.la_Link5Dist.Tag = "L5";
			this.la_Link5Dist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_Link5Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.la_Link5Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_GoLink1
			// 
			this.bu_GoLink1.Enabled = false;
			this.bu_GoLink1.Location = new System.Drawing.Point(255, 25);
			this.bu_GoLink1.Margin = new System.Windows.Forms.Padding(0);
			this.bu_GoLink1.Name = "bu_GoLink1";
			this.bu_GoLink1.Size = new System.Drawing.Size(30, 20);
			this.bu_GoLink1.TabIndex = 7;
			this.bu_GoLink1.Tag = "L1";
			this.bu_GoLink1.Text = "go";
			this.bu_GoLink1.UseVisualStyleBackColor = true;
			this.bu_GoLink1.Click += new System.EventHandler(this.OnLinkGoClick);
			this.bu_GoLink1.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.bu_GoLink1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_GoLink2
			// 
			this.bu_GoLink2.Enabled = false;
			this.bu_GoLink2.Location = new System.Drawing.Point(255, 50);
			this.bu_GoLink2.Margin = new System.Windows.Forms.Padding(0);
			this.bu_GoLink2.Name = "bu_GoLink2";
			this.bu_GoLink2.Size = new System.Drawing.Size(30, 20);
			this.bu_GoLink2.TabIndex = 12;
			this.bu_GoLink2.Tag = "L2";
			this.bu_GoLink2.Text = "go";
			this.bu_GoLink2.UseVisualStyleBackColor = true;
			this.bu_GoLink2.Click += new System.EventHandler(this.OnLinkGoClick);
			this.bu_GoLink2.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.bu_GoLink2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_GoLink3
			// 
			this.bu_GoLink3.Enabled = false;
			this.bu_GoLink3.Location = new System.Drawing.Point(255, 75);
			this.bu_GoLink3.Margin = new System.Windows.Forms.Padding(0);
			this.bu_GoLink3.Name = "bu_GoLink3";
			this.bu_GoLink3.Size = new System.Drawing.Size(30, 20);
			this.bu_GoLink3.TabIndex = 17;
			this.bu_GoLink3.Tag = "L3";
			this.bu_GoLink3.Text = "go";
			this.bu_GoLink3.UseVisualStyleBackColor = true;
			this.bu_GoLink3.Click += new System.EventHandler(this.OnLinkGoClick);
			this.bu_GoLink3.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.bu_GoLink3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_GoLink4
			// 
			this.bu_GoLink4.Enabled = false;
			this.bu_GoLink4.Location = new System.Drawing.Point(255, 100);
			this.bu_GoLink4.Margin = new System.Windows.Forms.Padding(0);
			this.bu_GoLink4.Name = "bu_GoLink4";
			this.bu_GoLink4.Size = new System.Drawing.Size(30, 20);
			this.bu_GoLink4.TabIndex = 22;
			this.bu_GoLink4.Tag = "L4";
			this.bu_GoLink4.Text = "go";
			this.bu_GoLink4.UseVisualStyleBackColor = true;
			this.bu_GoLink4.Click += new System.EventHandler(this.OnLinkGoClick);
			this.bu_GoLink4.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.bu_GoLink4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_GoLink5
			// 
			this.bu_GoLink5.Enabled = false;
			this.bu_GoLink5.Location = new System.Drawing.Point(255, 125);
			this.bu_GoLink5.Margin = new System.Windows.Forms.Padding(0);
			this.bu_GoLink5.Name = "bu_GoLink5";
			this.bu_GoLink5.Size = new System.Drawing.Size(30, 20);
			this.bu_GoLink5.TabIndex = 27;
			this.bu_GoLink5.Tag = "L5";
			this.bu_GoLink5.Text = "go";
			this.bu_GoLink5.UseVisualStyleBackColor = true;
			this.bu_GoLink5.Click += new System.EventHandler(this.OnLinkGoClick);
			this.bu_GoLink5.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.bu_GoLink5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_Og
			// 
			this.bu_Og.Location = new System.Drawing.Point(540, 28);
			this.bu_Og.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Og.Name = "bu_Og";
			this.bu_Og.Size = new System.Drawing.Size(20, 120);
			this.bu_Og.TabIndex = 2;
			this.bu_Og.Text = "o\r\ng";
			this.bu_Og.UseVisualStyleBackColor = true;
			this.bu_Og.Click += new System.EventHandler(this.OnOgClick);
			this.bu_Og.MouseEnter += new System.EventHandler(this.OnOgMouseEnter);
			this.bu_Og.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// bu_Tallyho
			// 
			this.bu_Tallyho.Location = new System.Drawing.Point(565, 28);
			this.bu_Tallyho.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Tallyho.Name = "bu_Tallyho";
			this.bu_Tallyho.Size = new System.Drawing.Size(20, 120);
			this.bu_Tallyho.TabIndex = 3;
			this.bu_Tallyho.Text = "t\r\na\r\nll\r\ny";
			this.bu_Tallyho.UseVisualStyleBackColor = true;
			this.bu_Tallyho.Click += new System.EventHandler(this.OnTallyhoClick);
			// 
			// gb_NodeEditor
			// 
			this.gb_NodeEditor.Controls.Add(this.bu_Cut);
			this.gb_NodeEditor.Controls.Add(this.bu_Copy);
			this.gb_NodeEditor.Controls.Add(this.bu_Paste);
			this.gb_NodeEditor.Controls.Add(this.bu_Delete);
			this.gb_NodeEditor.Location = new System.Drawing.Point(245, 153);
			this.gb_NodeEditor.Margin = new System.Windows.Forms.Padding(0);
			this.gb_NodeEditor.Name = "gb_NodeEditor";
			this.gb_NodeEditor.Padding = new System.Windows.Forms.Padding(0);
			this.gb_NodeEditor.Size = new System.Drawing.Size(290, 52);
			this.gb_NodeEditor.TabIndex = 4;
			this.gb_NodeEditor.TabStop = false;
			this.gb_NodeEditor.Text = " Node editor ";
			// 
			// bu_Cut
			// 
			this.bu_Cut.Enabled = false;
			this.bu_Cut.Location = new System.Drawing.Point(10, 15);
			this.bu_Cut.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Cut.Name = "bu_Cut";
			this.bu_Cut.Size = new System.Drawing.Size(65, 30);
			this.bu_Cut.TabIndex = 0;
			this.bu_Cut.Text = "Cut";
			this.toolTip1.SetToolTip(this.bu_Cut, "deletes the selected node and copies its Node data (not Link data)");
			this.bu_Cut.UseVisualStyleBackColor = true;
			this.bu_Cut.Click += new System.EventHandler(this.OnCutClick);
			// 
			// bu_Copy
			// 
			this.bu_Copy.Enabled = false;
			this.bu_Copy.Location = new System.Drawing.Point(79, 15);
			this.bu_Copy.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Copy.Name = "bu_Copy";
			this.bu_Copy.Size = new System.Drawing.Size(65, 30);
			this.bu_Copy.TabIndex = 1;
			this.bu_Copy.Text = "Copy";
			this.toolTip1.SetToolTip(this.bu_Copy, "copies Node data of the selected node");
			this.bu_Copy.UseVisualStyleBackColor = true;
			this.bu_Copy.Click += new System.EventHandler(this.OnCopyClick);
			// 
			// bu_Paste
			// 
			this.bu_Paste.Enabled = false;
			this.bu_Paste.Location = new System.Drawing.Point(148, 15);
			this.bu_Paste.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Paste.Name = "bu_Paste";
			this.bu_Paste.Size = new System.Drawing.Size(65, 30);
			this.bu_Paste.TabIndex = 2;
			this.bu_Paste.Text = "Paste";
			this.toolTip1.SetToolTip(this.bu_Paste, "pastes copied Node data to the selected node");
			this.bu_Paste.UseVisualStyleBackColor = true;
			this.bu_Paste.Click += new System.EventHandler(this.OnPasteClick);
			// 
			// bu_Delete
			// 
			this.bu_Delete.Enabled = false;
			this.bu_Delete.Location = new System.Drawing.Point(217, 15);
			this.bu_Delete.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Delete.Name = "bu_Delete";
			this.bu_Delete.Size = new System.Drawing.Size(65, 30);
			this.bu_Delete.TabIndex = 3;
			this.bu_Delete.Text = "Delete";
			this.toolTip1.SetToolTip(this.bu_Delete, "deletes the selected node");
			this.bu_Delete.UseVisualStyleBackColor = true;
			this.bu_Delete.Click += new System.EventHandler(this.OnDeleteClick);
			// 
			// bu_Save
			// 
			this.bu_Save.Enabled = false;
			this.bu_Save.ForeColor = System.Drawing.Color.Chocolate;
			this.bu_Save.Location = new System.Drawing.Point(539, 159);
			this.bu_Save.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Save.Name = "bu_Save";
			this.bu_Save.Size = new System.Drawing.Size(46, 46);
			this.bu_Save.TabIndex = 5;
			this.bu_Save.Text = "save";
			this.bu_Save.UseVisualStyleBackColor = true;
			this.bu_Save.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// gb_NoderankColors
			// 
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank8);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank8);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank7);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank7);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank6);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank6);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank5);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank5);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank4);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank4);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank3);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank3);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank2);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank2);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank1);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank1);
			this.gb_NoderankColors.Controls.Add(this.la_ColorRank0);
			this.gb_NoderankColors.Controls.Add(this.pa_ColorRank0);
			this.gb_NoderankColors.Location = new System.Drawing.Point(590, 3);
			this.gb_NoderankColors.Margin = new System.Windows.Forms.Padding(0);
			this.gb_NoderankColors.Name = "gb_NoderankColors";
			this.gb_NoderankColors.Padding = new System.Windows.Forms.Padding(0);
			this.gb_NoderankColors.Size = new System.Drawing.Size(192, 202);
			this.gb_NoderankColors.TabIndex = 6;
			this.gb_NoderankColors.TabStop = false;
			this.gb_NoderankColors.Text = " Node Rank colors ";
			// 
			// la_ColorRank8
			// 
			this.la_ColorRank8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank8.Location = new System.Drawing.Point(34, 178);
			this.la_ColorRank8.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank8.Name = "la_ColorRank8";
			this.la_ColorRank8.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank8.TabIndex = 17;
			this.la_ColorRank8.Text = "rank 8";
			this.la_ColorRank8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank8
			// 
			this.pa_ColorRank8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank8.Location = new System.Drawing.Point(8, 177);
			this.pa_ColorRank8.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank8.Name = "pa_ColorRank8";
			this.pa_ColorRank8.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank8.TabIndex = 16;
			this.pa_ColorRank8.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank7
			// 
			this.la_ColorRank7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank7.Location = new System.Drawing.Point(34, 158);
			this.la_ColorRank7.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank7.Name = "la_ColorRank7";
			this.la_ColorRank7.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank7.TabIndex = 15;
			this.la_ColorRank7.Text = "rank 7";
			this.la_ColorRank7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank7
			// 
			this.pa_ColorRank7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank7.Location = new System.Drawing.Point(8, 157);
			this.pa_ColorRank7.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank7.Name = "pa_ColorRank7";
			this.pa_ColorRank7.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank7.TabIndex = 14;
			this.pa_ColorRank7.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank6
			// 
			this.la_ColorRank6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank6.Location = new System.Drawing.Point(34, 138);
			this.la_ColorRank6.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank6.Name = "la_ColorRank6";
			this.la_ColorRank6.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank6.TabIndex = 13;
			this.la_ColorRank6.Text = "rank 6";
			this.la_ColorRank6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank6
			// 
			this.pa_ColorRank6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank6.Location = new System.Drawing.Point(8, 137);
			this.pa_ColorRank6.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank6.Name = "pa_ColorRank6";
			this.pa_ColorRank6.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank6.TabIndex = 12;
			this.pa_ColorRank6.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank5
			// 
			this.la_ColorRank5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank5.Location = new System.Drawing.Point(34, 118);
			this.la_ColorRank5.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank5.Name = "la_ColorRank5";
			this.la_ColorRank5.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank5.TabIndex = 11;
			this.la_ColorRank5.Text = "rank 5";
			this.la_ColorRank5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank5
			// 
			this.pa_ColorRank5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank5.Location = new System.Drawing.Point(8, 117);
			this.pa_ColorRank5.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank5.Name = "pa_ColorRank5";
			this.pa_ColorRank5.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank5.TabIndex = 10;
			this.pa_ColorRank5.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank4
			// 
			this.la_ColorRank4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank4.Location = new System.Drawing.Point(34, 98);
			this.la_ColorRank4.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank4.Name = "la_ColorRank4";
			this.la_ColorRank4.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank4.TabIndex = 9;
			this.la_ColorRank4.Text = "rank 4";
			this.la_ColorRank4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank4
			// 
			this.pa_ColorRank4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank4.Location = new System.Drawing.Point(8, 97);
			this.pa_ColorRank4.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank4.Name = "pa_ColorRank4";
			this.pa_ColorRank4.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank4.TabIndex = 8;
			this.pa_ColorRank4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank3
			// 
			this.la_ColorRank3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank3.Location = new System.Drawing.Point(34, 78);
			this.la_ColorRank3.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank3.Name = "la_ColorRank3";
			this.la_ColorRank3.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank3.TabIndex = 7;
			this.la_ColorRank3.Text = "rank 3";
			this.la_ColorRank3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank3
			// 
			this.pa_ColorRank3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank3.Location = new System.Drawing.Point(8, 77);
			this.pa_ColorRank3.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank3.Name = "pa_ColorRank3";
			this.pa_ColorRank3.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank3.TabIndex = 6;
			this.pa_ColorRank3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank2
			// 
			this.la_ColorRank2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank2.Location = new System.Drawing.Point(34, 58);
			this.la_ColorRank2.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank2.Name = "la_ColorRank2";
			this.la_ColorRank2.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank2.TabIndex = 5;
			this.la_ColorRank2.Text = "rank 2";
			this.la_ColorRank2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank2
			// 
			this.pa_ColorRank2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank2.Location = new System.Drawing.Point(8, 57);
			this.pa_ColorRank2.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank2.Name = "pa_ColorRank2";
			this.pa_ColorRank2.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank2.TabIndex = 4;
			this.pa_ColorRank2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank1
			// 
			this.la_ColorRank1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank1.Location = new System.Drawing.Point(34, 38);
			this.la_ColorRank1.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank1.Name = "la_ColorRank1";
			this.la_ColorRank1.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank1.TabIndex = 3;
			this.la_ColorRank1.Text = "rank 1";
			this.la_ColorRank1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank1
			// 
			this.pa_ColorRank1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank1.Location = new System.Drawing.Point(8, 37);
			this.pa_ColorRank1.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank1.Name = "pa_ColorRank1";
			this.pa_ColorRank1.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank1.TabIndex = 2;
			this.pa_ColorRank1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// la_ColorRank0
			// 
			this.la_ColorRank0.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_ColorRank0.Location = new System.Drawing.Point(34, 18);
			this.la_ColorRank0.Margin = new System.Windows.Forms.Padding(0);
			this.la_ColorRank0.Name = "la_ColorRank0";
			this.la_ColorRank0.Size = new System.Drawing.Size(153, 15);
			this.la_ColorRank0.TabIndex = 1;
			this.la_ColorRank0.Text = "rank 0";
			this.la_ColorRank0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_ColorRank0
			// 
			this.pa_ColorRank0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pa_ColorRank0.Location = new System.Drawing.Point(8, 17);
			this.pa_ColorRank0.Margin = new System.Windows.Forms.Padding(0);
			this.pa_ColorRank0.Name = "pa_ColorRank0";
			this.pa_ColorRank0.Size = new System.Drawing.Size(22, 17);
			this.pa_ColorRank0.TabIndex = 0;
			this.pa_ColorRank0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnRankHighlightClick_pa);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.UseAnimation = false;
			this.toolTip1.UseFading = false;
			// 
			// RouteView
			// 
			this.Controls.Add(this._pnlRoutes);
			this.Controls.Add(this.ts_Main);
			this.Controls.Add(this.pa_DataFields);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "RouteView";
			this.Size = new System.Drawing.Size(800, 480);
			this.ts_Main.ResumeLayout(false);
			this.ts_Main.PerformLayout();
			this.pa_DataFields.ResumeLayout(false);
			this.pa_DataFieldsLeft.ResumeLayout(false);
			this.gb_NodeData.ResumeLayout(false);
			this.gb_TileData.ResumeLayout(false);
			this.gb_LinkData.ResumeLayout(false);
			this.gb_NodeEditor.ResumeLayout(false);
			this.gb_NoderankColors.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}
