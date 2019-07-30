namespace MapView.Forms.MapObservers.RouteViews
{
	partial class RouteView
	{
		#region Designer
		private System.Windows.Forms.ToolStrip tsMain;

		private System.Windows.Forms.ToolStripDropDownButton tsddbFile;
		private System.Windows.Forms.ToolStripMenuItem tsmiExport;
		private System.Windows.Forms.ToolStripMenuItem tsmiImport;

		private System.Windows.Forms.ToolStripDropDownButton tsddbEdit;
		private System.Windows.Forms.ToolStripMenuItem tsmiAllNodesRank0;
		private System.Windows.Forms.ToolStripMenuItem tsmiClearLinkData;
		private System.Windows.Forms.ToolStripMenuItem tsmiUpdateAllLinkDistances;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem tsmi_RaiseNode;
		private System.Windows.Forms.ToolStripMenuItem tsmi_LowerNode;

		private System.Windows.Forms.ToolStripDropDownButton tsddbDebug;
		private System.Windows.Forms.ToolStripMenuItem tsmiCheckNodeRanks;
		private System.Windows.Forms.ToolStripMenuItem tsmiCheckOobNodes;

		private System.Windows.Forms.ToolStripButton tsb_Options;

		private System.Windows.Forms.ToolStripSeparator tss_0;

		private System.Windows.Forms.ToolStripButton tsb_connect0;
		private System.Windows.Forms.ToolStripButton tsb_connect1;
		private System.Windows.Forms.ToolStripButton tsb_connect2;

		private System.Windows.Forms.Panel pnlDataFields;

		private System.Windows.Forms.Panel pnlDataFieldsLeft;

		private System.Windows.Forms.GroupBox gbTileData;
		private System.Windows.Forms.Label lblSelected;
		private System.Windows.Forms.Label lblOver;

		private System.Windows.Forms.GroupBox gbNodeData;
		private System.Windows.Forms.Label labelUnitType;
		private System.Windows.Forms.Label labelSpawnRank;
		private System.Windows.Forms.Label labelSpawnWeight;
		private System.Windows.Forms.Label labelPriority;
		private System.Windows.Forms.Label labelAttack;
		private System.Windows.Forms.ComboBox cbType;
		private System.Windows.Forms.ComboBox cbRank;
		private System.Windows.Forms.ComboBox cbSpawn;
		private System.Windows.Forms.ComboBox cbPatrol;
		private System.Windows.Forms.ComboBox cbAttack;

		private System.Windows.Forms.GroupBox gbLinkData;
		private System.Windows.Forms.Label labelDest;
		private System.Windows.Forms.Label labelUnitInfo;
		private System.Windows.Forms.Label labelDist;
		private System.Windows.Forms.Label labelLink1;
		private System.Windows.Forms.Label labelLink2;
		private System.Windows.Forms.Label labelLink3;
		private System.Windows.Forms.Label labelLink4;
		private System.Windows.Forms.Label labelLink5;
		private System.Windows.Forms.ComboBox cbLink1Dest;
		private System.Windows.Forms.ComboBox cbLink2Dest;
		private System.Windows.Forms.ComboBox cbLink3Dest;
		private System.Windows.Forms.ComboBox cbLink4Dest;
		private System.Windows.Forms.ComboBox cbLink5Dest;
		private System.Windows.Forms.ComboBox cbLink1UnitType;
		private System.Windows.Forms.ComboBox cbLink2UnitType;
		private System.Windows.Forms.ComboBox cbLink3UnitType;
		private System.Windows.Forms.ComboBox cbLink4UnitType;
		private System.Windows.Forms.ComboBox cbLink5UnitType;
		private System.Windows.Forms.TextBox tbLink1Dist;
		private System.Windows.Forms.TextBox tbLink2Dist;
		private System.Windows.Forms.TextBox tbLink3Dist;
		private System.Windows.Forms.TextBox tbLink4Dist;
		private System.Windows.Forms.TextBox tbLink5Dist;
		private System.Windows.Forms.Button btnGoLink1;
		private System.Windows.Forms.Button btnGoLink2;
		private System.Windows.Forms.Button btnGoLink3;
		private System.Windows.Forms.Button btnGoLink4;
		private System.Windows.Forms.Button btnGoLink5;

		private System.Windows.Forms.GroupBox gbNodeEditor;
		private System.Windows.Forms.Button btnCut;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Button btnPaste;
		private System.Windows.Forms.Button btnDelete;

		private System.Windows.Forms.Button btnOg;
		private System.Windows.Forms.Button btnTallyho;
		private System.Windows.Forms.Button btnSave;


		private System.Windows.Forms.ToolTip toolTip1;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsddbFile = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiImport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddbEdit = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmi_RaiseNode = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_LowerNode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiAllNodesRank0 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiClearLinkData = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiUpdateAllLinkDistances = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddbDebug = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiCheckOobNodes = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiCheckNodeRanks = new System.Windows.Forms.ToolStripMenuItem();
			this.tsb_Options = new System.Windows.Forms.ToolStripButton();
			this.tss_0 = new System.Windows.Forms.ToolStripSeparator();
			this.tsb_connect0 = new System.Windows.Forms.ToolStripButton();
			this.tsb_connect1 = new System.Windows.Forms.ToolStripButton();
			this.tsb_connect2 = new System.Windows.Forms.ToolStripButton();
			this._pnlRoutes = new DSShared.Controls.CompositedPanel();
			this.pnlDataFields = new System.Windows.Forms.Panel();
			this.pnlDataFieldsLeft = new System.Windows.Forms.Panel();
			this.gbNodeData = new System.Windows.Forms.GroupBox();
			this.labelUnitType = new System.Windows.Forms.Label();
			this.labelSpawnRank = new System.Windows.Forms.Label();
			this.labelSpawnWeight = new System.Windows.Forms.Label();
			this.labelPriority = new System.Windows.Forms.Label();
			this.labelAttack = new System.Windows.Forms.Label();
			this.cbType = new System.Windows.Forms.ComboBox();
			this.cbRank = new System.Windows.Forms.ComboBox();
			this.cbSpawn = new System.Windows.Forms.ComboBox();
			this.cbPatrol = new System.Windows.Forms.ComboBox();
			this.cbAttack = new System.Windows.Forms.ComboBox();
			this.gbTileData = new System.Windows.Forms.GroupBox();
			this.lblSelected = new System.Windows.Forms.Label();
			this.lblOver = new System.Windows.Forms.Label();
			this.gbLinkData = new System.Windows.Forms.GroupBox();
			this.labelDest = new System.Windows.Forms.Label();
			this.labelUnitInfo = new System.Windows.Forms.Label();
			this.labelDist = new System.Windows.Forms.Label();
			this.labelLink1 = new System.Windows.Forms.Label();
			this.labelLink2 = new System.Windows.Forms.Label();
			this.labelLink3 = new System.Windows.Forms.Label();
			this.labelLink4 = new System.Windows.Forms.Label();
			this.labelLink5 = new System.Windows.Forms.Label();
			this.tbLink1Dist = new System.Windows.Forms.TextBox();
			this.tbLink2Dist = new System.Windows.Forms.TextBox();
			this.tbLink3Dist = new System.Windows.Forms.TextBox();
			this.tbLink4Dist = new System.Windows.Forms.TextBox();
			this.tbLink5Dist = new System.Windows.Forms.TextBox();
			this.cbLink1UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink2UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink3UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink4UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink5UnitType = new System.Windows.Forms.ComboBox();
			this.cbLink1Dest = new System.Windows.Forms.ComboBox();
			this.cbLink2Dest = new System.Windows.Forms.ComboBox();
			this.cbLink3Dest = new System.Windows.Forms.ComboBox();
			this.cbLink4Dest = new System.Windows.Forms.ComboBox();
			this.cbLink5Dest = new System.Windows.Forms.ComboBox();
			this.btnGoLink1 = new System.Windows.Forms.Button();
			this.btnGoLink2 = new System.Windows.Forms.Button();
			this.btnGoLink3 = new System.Windows.Forms.Button();
			this.btnGoLink4 = new System.Windows.Forms.Button();
			this.btnGoLink5 = new System.Windows.Forms.Button();
			this.gbNodeEditor = new System.Windows.Forms.GroupBox();
			this.btnCut = new System.Windows.Forms.Button();
			this.btnCopy = new System.Windows.Forms.Button();
			this.btnPaste = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnOg = new System.Windows.Forms.Button();
			this.btnTallyho = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tsMain.SuspendLayout();
			this.pnlDataFields.SuspendLayout();
			this.pnlDataFieldsLeft.SuspendLayout();
			this.gbNodeData.SuspendLayout();
			this.gbTileData.SuspendLayout();
			this.gbLinkData.SuspendLayout();
			this.gbNodeEditor.SuspendLayout();
			this.SuspendLayout();
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbFile,
			this.tsddbEdit,
			this.tsddbDebug,
			this.tsb_Options,
			this.tss_0,
			this.tsb_connect0,
			this.tsb_connect1,
			this.tsb_connect2});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 0;
			this.tsMain.TabStop = true;
			this.tsMain.Text = "tsMain";
			// 
			// tsddbFile
			// 
			this.tsddbFile.AutoToolTip = false;
			this.tsddbFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiExport,
			this.tsmiImport});
			this.tsddbFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbFile.Margin = new System.Windows.Forms.Padding(3, 1, 0, 1);
			this.tsddbFile.Name = "tsddbFile";
			this.tsddbFile.Size = new System.Drawing.Size(37, 23);
			this.tsddbFile.Text = "&File";
			this.tsddbFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsmiExport
			// 
			this.tsmiExport.Name = "tsmiExport";
			this.tsmiExport.Size = new System.Drawing.Size(124, 22);
			this.tsmiExport.Text = "&Export ...";
			this.tsmiExport.Click += new System.EventHandler(this.OnExportClick);
			// 
			// tsmiImport
			// 
			this.tsmiImport.Name = "tsmiImport";
			this.tsmiImport.Size = new System.Drawing.Size(124, 22);
			this.tsmiImport.Text = "&Import ...";
			this.tsmiImport.Click += new System.EventHandler(this.OnImportClick);
			// 
			// tsddbEdit
			// 
			this.tsddbEdit.AutoToolTip = false;
			this.tsddbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmi_RaiseNode,
			this.tsmi_LowerNode,
			this.toolStripSeparator1,
			this.tsmiAllNodesRank0,
			this.tsmiClearLinkData,
			this.tsmiUpdateAllLinkDistances});
			this.tsddbEdit.Font = new System.Drawing.Font("Verdana", 7F);
			this.tsddbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbEdit.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsddbEdit.Name = "tsddbEdit";
			this.tsddbEdit.Size = new System.Drawing.Size(38, 23);
			this.tsddbEdit.Text = "&Edit";
			this.tsddbEdit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsddbEdit.DropDownOpening += new System.EventHandler(this.OnEditOpening);
			// 
			// tsmi_RaiseNode
			// 
			this.tsmi_RaiseNode.Name = "tsmi_RaiseNode";
			this.tsmi_RaiseNode.Size = new System.Drawing.Size(213, 22);
			this.tsmi_RaiseNode.Text = "node &up 1 level";
			this.tsmi_RaiseNode.Click += new System.EventHandler(this.OnNodeRaise);
			// 
			// tsmi_LowerNode
			// 
			this.tsmi_LowerNode.Name = "tsmi_LowerNode";
			this.tsmi_LowerNode.Size = new System.Drawing.Size(213, 22);
			this.tsmi_LowerNode.Text = "node &down 1 level";
			this.tsmi_LowerNode.Click += new System.EventHandler(this.OnNodeLower);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
			// 
			// tsmiAllNodesRank0
			// 
			this.tsmiAllNodesRank0.Name = "tsmiAllNodesRank0";
			this.tsmiAllNodesRank0.Size = new System.Drawing.Size(213, 22);
			this.tsmiAllNodesRank0.Text = "&Zero all noderanks ...";
			this.tsmiAllNodesRank0.Click += new System.EventHandler(this.OnAllNodesRank0Click);
			// 
			// tsmiClearLinkData
			// 
			this.tsmiClearLinkData.Name = "tsmiClearLinkData";
			this.tsmiClearLinkData.Size = new System.Drawing.Size(213, 22);
			this.tsmiClearLinkData.Text = "&Clear current Link data ...";
			this.tsmiClearLinkData.Click += new System.EventHandler(this.OnClearLinkDataClick);
			// 
			// tsmiUpdateAllLinkDistances
			// 
			this.tsmiUpdateAllLinkDistances.Name = "tsmiUpdateAllLinkDistances";
			this.tsmiUpdateAllLinkDistances.Size = new System.Drawing.Size(213, 22);
			this.tsmiUpdateAllLinkDistances.Text = "&Update all Link distances";
			this.tsmiUpdateAllLinkDistances.Click += new System.EventHandler(this.OnUpdateAllLinkDistances);
			// 
			// tsddbDebug
			// 
			this.tsddbDebug.AutoToolTip = false;
			this.tsddbDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiCheckOobNodes,
			this.tsmiCheckNodeRanks});
			this.tsddbDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbDebug.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this.tsddbDebug.Name = "tsddbDebug";
			this.tsddbDebug.Size = new System.Drawing.Size(42, 23);
			this.tsddbDebug.Text = "&Test";
			this.tsddbDebug.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsmiCheckOobNodes
			// 
			this.tsmiCheckOobNodes.Name = "tsmiCheckOobNodes";
			this.tsmiCheckOobNodes.Size = new System.Drawing.Size(157, 22);
			this.tsmiCheckOobNodes.Text = "Test &positions";
			this.tsmiCheckOobNodes.Click += new System.EventHandler(this.OnCheckOobNodesClick);
			// 
			// tsmiCheckNodeRanks
			// 
			this.tsmiCheckNodeRanks.Name = "tsmiCheckNodeRanks";
			this.tsmiCheckNodeRanks.Size = new System.Drawing.Size(157, 22);
			this.tsmiCheckNodeRanks.Text = "Test node&ranks";
			this.tsmiCheckNodeRanks.Click += new System.EventHandler(this.OnCheckNodeRanksClick);
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
			this.tsb_connect0.Text = "dont link";
			this.tsb_connect0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_connect0.ToolTipText = "dont link";
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
			this.tsb_connect1.Text = "1 way link";
			this.tsb_connect1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_connect1.ToolTipText = "1 way link";
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
			this.tsb_connect2.Text = "2 way link";
			this.tsb_connect2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_connect2.ToolTipText = "2 way link";
			this.tsb_connect2.Click += new System.EventHandler(this.OnConnectTypeClicked);
			// 
			// _pnlRoutes
			// 
			this._pnlRoutes.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pnlRoutes.Location = new System.Drawing.Point(0, 25);
			this._pnlRoutes.Margin = new System.Windows.Forms.Padding(0);
			this._pnlRoutes.Name = "_pnlRoutes";
			this._pnlRoutes.Size = new System.Drawing.Size(640, 250);
			this._pnlRoutes.TabIndex = 1;
			// 
			// pnlDataFields
			// 
			this.pnlDataFields.Controls.Add(this.pnlDataFieldsLeft);
			this.pnlDataFields.Controls.Add(this.gbLinkData);
			this.pnlDataFields.Controls.Add(this.gbNodeEditor);
			this.pnlDataFields.Controls.Add(this.btnOg);
			this.pnlDataFields.Controls.Add(this.btnTallyho);
			this.pnlDataFields.Controls.Add(this.btnSave);
			this.pnlDataFields.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDataFields.Location = new System.Drawing.Point(0, 275);
			this.pnlDataFields.Margin = new System.Windows.Forms.Padding(0);
			this.pnlDataFields.Name = "pnlDataFields";
			this.pnlDataFields.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.pnlDataFields.Size = new System.Drawing.Size(640, 205);
			this.pnlDataFields.TabIndex = 2;
			// 
			// pnlDataFieldsLeft
			// 
			this.pnlDataFieldsLeft.Controls.Add(this.gbNodeData);
			this.pnlDataFieldsLeft.Controls.Add(this.gbTileData);
			this.pnlDataFieldsLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlDataFieldsLeft.Location = new System.Drawing.Point(0, 3);
			this.pnlDataFieldsLeft.Margin = new System.Windows.Forms.Padding(0);
			this.pnlDataFieldsLeft.Name = "pnlDataFieldsLeft";
			this.pnlDataFieldsLeft.Size = new System.Drawing.Size(245, 202);
			this.pnlDataFieldsLeft.TabIndex = 0;
			// 
			// gbNodeData
			// 
			this.gbNodeData.Controls.Add(this.labelUnitType);
			this.gbNodeData.Controls.Add(this.labelSpawnRank);
			this.gbNodeData.Controls.Add(this.labelSpawnWeight);
			this.gbNodeData.Controls.Add(this.labelPriority);
			this.gbNodeData.Controls.Add(this.labelAttack);
			this.gbNodeData.Controls.Add(this.cbType);
			this.gbNodeData.Controls.Add(this.cbRank);
			this.gbNodeData.Controls.Add(this.cbSpawn);
			this.gbNodeData.Controls.Add(this.cbPatrol);
			this.gbNodeData.Controls.Add(this.cbAttack);
			this.gbNodeData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbNodeData.Location = new System.Drawing.Point(0, 50);
			this.gbNodeData.Margin = new System.Windows.Forms.Padding(0);
			this.gbNodeData.Name = "gbNodeData";
			this.gbNodeData.Size = new System.Drawing.Size(245, 152);
			this.gbNodeData.TabIndex = 1;
			this.gbNodeData.TabStop = false;
			this.gbNodeData.Text = " Node data ";
			// 
			// labelUnitType
			// 
			this.labelUnitType.Location = new System.Drawing.Point(10, 20);
			this.labelUnitType.Margin = new System.Windows.Forms.Padding(0);
			this.labelUnitType.Name = "labelUnitType";
			this.labelUnitType.Size = new System.Drawing.Size(85, 15);
			this.labelUnitType.TabIndex = 0;
			this.labelUnitType.Text = "Unit Type";
			this.labelUnitType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelUnitType, "characteristics of units that may patrol (or spawn at) the node");
			// 
			// labelSpawnRank
			// 
			this.labelSpawnRank.Location = new System.Drawing.Point(10, 45);
			this.labelSpawnRank.Margin = new System.Windows.Forms.Padding(0);
			this.labelSpawnRank.Name = "labelSpawnRank";
			this.labelSpawnRank.Size = new System.Drawing.Size(85, 15);
			this.labelSpawnRank.TabIndex = 2;
			this.labelSpawnRank.Text = "Node Rank";
			this.labelSpawnRank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelSpawnRank, "faction or rank (if aLiens) that may spawn/patrol here. Nodes for aLiens outside " +
		"their UFO or base are usually set to 0");
			// 
			// labelSpawnWeight
			// 
			this.labelSpawnWeight.Location = new System.Drawing.Point(10, 70);
			this.labelSpawnWeight.Margin = new System.Windows.Forms.Padding(0);
			this.labelSpawnWeight.Name = "labelSpawnWeight";
			this.labelSpawnWeight.Size = new System.Drawing.Size(85, 15);
			this.labelSpawnWeight.TabIndex = 4;
			this.labelSpawnWeight.Text = "Spawn Weight";
			this.labelSpawnWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelSpawnWeight, "desire for an aLien to spawn here");
			// 
			// labelPriority
			// 
			this.labelPriority.Location = new System.Drawing.Point(10, 95);
			this.labelPriority.Margin = new System.Windows.Forms.Padding(0);
			this.labelPriority.Name = "labelPriority";
			this.labelPriority.Size = new System.Drawing.Size(85, 15);
			this.labelPriority.TabIndex = 6;
			this.labelPriority.Text = "Patrol Priority";
			this.labelPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelPriority, "desire for an aLien to patrol here");
			// 
			// labelAttack
			// 
			this.labelAttack.Location = new System.Drawing.Point(10, 120);
			this.labelAttack.Margin = new System.Windows.Forms.Padding(0);
			this.labelAttack.Name = "labelAttack";
			this.labelAttack.Size = new System.Drawing.Size(85, 15);
			this.labelAttack.TabIndex = 8;
			this.labelAttack.Text = "Base Attack";
			this.labelAttack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelAttack, "attacts an aLien to shoot at XCom base tiles");
			// 
			// cbType
			// 
			this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbType.Location = new System.Drawing.Point(100, 15);
			this.cbType.Margin = new System.Windows.Forms.Padding(0);
			this.cbType.Name = "cbType";
			this.cbType.Size = new System.Drawing.Size(140, 20);
			this.cbType.TabIndex = 1;
			this.toolTip1.SetToolTip(this.cbType, "characteristics of units that may patrol (or spawn at) the node");
			this.cbType.SelectedIndexChanged += new System.EventHandler(this.OnUnitTypeSelectedIndexChanged);
			// 
			// cbRank
			// 
			this.cbRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRank.Location = new System.Drawing.Point(100, 40);
			this.cbRank.Margin = new System.Windows.Forms.Padding(0);
			this.cbRank.Name = "cbRank";
			this.cbRank.Size = new System.Drawing.Size(140, 20);
			this.cbRank.TabIndex = 3;
			this.toolTip1.SetToolTip(this.cbRank, "faction or rank (if aLiens) that may spawn/patrol here. Nodes for aLiens outside " +
		"their UFO or base are usually set to 0");
			this.cbRank.SelectedIndexChanged += new System.EventHandler(this.OnNodeRankSelectedIndexChanged);
			// 
			// cbSpawn
			// 
			this.cbSpawn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSpawn.Location = new System.Drawing.Point(100, 65);
			this.cbSpawn.Margin = new System.Windows.Forms.Padding(0);
			this.cbSpawn.Name = "cbSpawn";
			this.cbSpawn.Size = new System.Drawing.Size(140, 20);
			this.cbSpawn.TabIndex = 5;
			this.toolTip1.SetToolTip(this.cbSpawn, "desire for an aLien to spawn here");
			this.cbSpawn.SelectedIndexChanged += new System.EventHandler(this.OnSpawnWeightSelectedIndexChanged);
			// 
			// cbPatrol
			// 
			this.cbPatrol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPatrol.Location = new System.Drawing.Point(100, 90);
			this.cbPatrol.Margin = new System.Windows.Forms.Padding(0);
			this.cbPatrol.Name = "cbPatrol";
			this.cbPatrol.Size = new System.Drawing.Size(140, 20);
			this.cbPatrol.TabIndex = 7;
			this.toolTip1.SetToolTip(this.cbPatrol, "desire for an aLien to patrol here");
			this.cbPatrol.SelectedIndexChanged += new System.EventHandler(this.OnPatrolPrioritySelectedIndexChanged);
			// 
			// cbAttack
			// 
			this.cbAttack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbAttack.Location = new System.Drawing.Point(100, 115);
			this.cbAttack.Margin = new System.Windows.Forms.Padding(0);
			this.cbAttack.Name = "cbAttack";
			this.cbAttack.Size = new System.Drawing.Size(140, 20);
			this.cbAttack.TabIndex = 9;
			this.toolTip1.SetToolTip(this.cbAttack, "attacts an aLien to shoot at XCom base tiles");
			this.cbAttack.SelectedIndexChanged += new System.EventHandler(this.OnBaseAttackSelectedIndexChanged);
			// 
			// gbTileData
			// 
			this.gbTileData.Controls.Add(this.lblSelected);
			this.gbTileData.Controls.Add(this.lblOver);
			this.gbTileData.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbTileData.Location = new System.Drawing.Point(0, 0);
			this.gbTileData.Margin = new System.Windows.Forms.Padding(0);
			this.gbTileData.Name = "gbTileData";
			this.gbTileData.Size = new System.Drawing.Size(245, 50);
			this.gbTileData.TabIndex = 0;
			this.gbTileData.TabStop = false;
			this.gbTileData.Text = " Tile data ";
			// 
			// lblSelected
			// 
			this.lblSelected.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSelected.Location = new System.Drawing.Point(15, 15);
			this.lblSelected.Margin = new System.Windows.Forms.Padding(0);
			this.lblSelected.Name = "lblSelected";
			this.lblSelected.Size = new System.Drawing.Size(110, 30);
			this.lblSelected.TabIndex = 0;
			this.lblSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblOver
			// 
			this.lblOver.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOver.Location = new System.Drawing.Point(130, 15);
			this.lblOver.Margin = new System.Windows.Forms.Padding(0);
			this.lblOver.Name = "lblOver";
			this.lblOver.Size = new System.Drawing.Size(110, 30);
			this.lblOver.TabIndex = 1;
			this.lblOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gbLinkData
			// 
			this.gbLinkData.Controls.Add(this.labelDest);
			this.gbLinkData.Controls.Add(this.labelUnitInfo);
			this.gbLinkData.Controls.Add(this.labelDist);
			this.gbLinkData.Controls.Add(this.labelLink1);
			this.gbLinkData.Controls.Add(this.labelLink2);
			this.gbLinkData.Controls.Add(this.labelLink3);
			this.gbLinkData.Controls.Add(this.labelLink4);
			this.gbLinkData.Controls.Add(this.labelLink5);
			this.gbLinkData.Controls.Add(this.tbLink1Dist);
			this.gbLinkData.Controls.Add(this.tbLink2Dist);
			this.gbLinkData.Controls.Add(this.tbLink3Dist);
			this.gbLinkData.Controls.Add(this.tbLink4Dist);
			this.gbLinkData.Controls.Add(this.tbLink5Dist);
			this.gbLinkData.Controls.Add(this.cbLink1UnitType);
			this.gbLinkData.Controls.Add(this.cbLink2UnitType);
			this.gbLinkData.Controls.Add(this.cbLink3UnitType);
			this.gbLinkData.Controls.Add(this.cbLink4UnitType);
			this.gbLinkData.Controls.Add(this.cbLink5UnitType);
			this.gbLinkData.Controls.Add(this.cbLink1Dest);
			this.gbLinkData.Controls.Add(this.cbLink2Dest);
			this.gbLinkData.Controls.Add(this.cbLink3Dest);
			this.gbLinkData.Controls.Add(this.cbLink4Dest);
			this.gbLinkData.Controls.Add(this.cbLink5Dest);
			this.gbLinkData.Controls.Add(this.btnGoLink1);
			this.gbLinkData.Controls.Add(this.btnGoLink2);
			this.gbLinkData.Controls.Add(this.btnGoLink3);
			this.gbLinkData.Controls.Add(this.btnGoLink4);
			this.gbLinkData.Controls.Add(this.btnGoLink5);
			this.gbLinkData.Location = new System.Drawing.Point(245, 3);
			this.gbLinkData.Margin = new System.Windows.Forms.Padding(0);
			this.gbLinkData.Name = "gbLinkData";
			this.gbLinkData.Size = new System.Drawing.Size(290, 150);
			this.gbLinkData.TabIndex = 1;
			this.gbLinkData.TabStop = false;
			this.gbLinkData.Text = " Link data ";
			// 
			// labelDest
			// 
			this.labelDest.Location = new System.Drawing.Point(85, 10);
			this.labelDest.Margin = new System.Windows.Forms.Padding(0);
			this.labelDest.Name = "labelDest";
			this.labelDest.Size = new System.Drawing.Size(30, 15);
			this.labelDest.TabIndex = 0;
			this.labelDest.Text = "Dest";
			// 
			// labelUnitInfo
			// 
			this.labelUnitInfo.Location = new System.Drawing.Point(175, 10);
			this.labelUnitInfo.Margin = new System.Windows.Forms.Padding(0);
			this.labelUnitInfo.Name = "labelUnitInfo";
			this.labelUnitInfo.Size = new System.Drawing.Size(30, 15);
			this.labelUnitInfo.TabIndex = 1;
			this.labelUnitInfo.Text = "Unit Type";
			this.toolTip1.SetToolTip(this.labelUnitInfo, "not used in 0penXcom");
			// 
			// labelDist
			// 
			this.labelDist.Location = new System.Drawing.Point(220, 10);
			this.labelDist.Margin = new System.Windows.Forms.Padding(0);
			this.labelDist.Name = "labelDist";
			this.labelDist.Size = new System.Drawing.Size(30, 15);
			this.labelDist.TabIndex = 2;
			this.labelDist.Text = "Dist";
			this.toolTip1.SetToolTip(this.labelDist, "not used in 0penXcom");
			// 
			// labelLink1
			// 
			this.labelLink1.Location = new System.Drawing.Point(5, 30);
			this.labelLink1.Margin = new System.Windows.Forms.Padding(0);
			this.labelLink1.Name = "labelLink1";
			this.labelLink1.Size = new System.Drawing.Size(35, 15);
			this.labelLink1.TabIndex = 3;
			this.labelLink1.Tag = "L1";
			this.labelLink1.Text = "Link1";
			this.labelLink1.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.labelLink1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink2
			// 
			this.labelLink2.Location = new System.Drawing.Point(5, 55);
			this.labelLink2.Margin = new System.Windows.Forms.Padding(0);
			this.labelLink2.Name = "labelLink2";
			this.labelLink2.Size = new System.Drawing.Size(35, 15);
			this.labelLink2.TabIndex = 8;
			this.labelLink2.Tag = "L2";
			this.labelLink2.Text = "Link2";
			this.labelLink2.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.labelLink2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink3
			// 
			this.labelLink3.Location = new System.Drawing.Point(5, 80);
			this.labelLink3.Margin = new System.Windows.Forms.Padding(0);
			this.labelLink3.Name = "labelLink3";
			this.labelLink3.Size = new System.Drawing.Size(35, 15);
			this.labelLink3.TabIndex = 13;
			this.labelLink3.Tag = "L3";
			this.labelLink3.Text = "Link3";
			this.labelLink3.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.labelLink3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink4
			// 
			this.labelLink4.Location = new System.Drawing.Point(5, 105);
			this.labelLink4.Margin = new System.Windows.Forms.Padding(0);
			this.labelLink4.Name = "labelLink4";
			this.labelLink4.Size = new System.Drawing.Size(35, 15);
			this.labelLink4.TabIndex = 18;
			this.labelLink4.Tag = "L4";
			this.labelLink4.Text = "Link4";
			this.labelLink4.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.labelLink4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// labelLink5
			// 
			this.labelLink5.Location = new System.Drawing.Point(5, 130);
			this.labelLink5.Margin = new System.Windows.Forms.Padding(0);
			this.labelLink5.Name = "labelLink5";
			this.labelLink5.Size = new System.Drawing.Size(35, 15);
			this.labelLink5.TabIndex = 23;
			this.labelLink5.Tag = "L5";
			this.labelLink5.Text = "Link5";
			this.labelLink5.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.labelLink5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink1Dist
			// 
			this.tbLink1Dist.Location = new System.Drawing.Point(215, 25);
			this.tbLink1Dist.Margin = new System.Windows.Forms.Padding(0);
			this.tbLink1Dist.Name = "tbLink1Dist";
			this.tbLink1Dist.ReadOnly = true;
			this.tbLink1Dist.Size = new System.Drawing.Size(35, 19);
			this.tbLink1Dist.TabIndex = 6;
			this.tbLink1Dist.Tag = "L1";
			this.tbLink1Dist.WordWrap = false;
			this.tbLink1Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.tbLink1Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink2Dist
			// 
			this.tbLink2Dist.Location = new System.Drawing.Point(215, 50);
			this.tbLink2Dist.Margin = new System.Windows.Forms.Padding(0);
			this.tbLink2Dist.Name = "tbLink2Dist";
			this.tbLink2Dist.ReadOnly = true;
			this.tbLink2Dist.Size = new System.Drawing.Size(35, 19);
			this.tbLink2Dist.TabIndex = 11;
			this.tbLink2Dist.Tag = "L2";
			this.tbLink2Dist.WordWrap = false;
			this.tbLink2Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.tbLink2Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink3Dist
			// 
			this.tbLink3Dist.Location = new System.Drawing.Point(215, 75);
			this.tbLink3Dist.Margin = new System.Windows.Forms.Padding(0);
			this.tbLink3Dist.Name = "tbLink3Dist";
			this.tbLink3Dist.ReadOnly = true;
			this.tbLink3Dist.Size = new System.Drawing.Size(35, 19);
			this.tbLink3Dist.TabIndex = 16;
			this.tbLink3Dist.Tag = "L3";
			this.tbLink3Dist.WordWrap = false;
			this.tbLink3Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.tbLink3Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink4Dist
			// 
			this.tbLink4Dist.Location = new System.Drawing.Point(215, 100);
			this.tbLink4Dist.Margin = new System.Windows.Forms.Padding(0);
			this.tbLink4Dist.Name = "tbLink4Dist";
			this.tbLink4Dist.ReadOnly = true;
			this.tbLink4Dist.Size = new System.Drawing.Size(35, 19);
			this.tbLink4Dist.TabIndex = 21;
			this.tbLink4Dist.Tag = "L4";
			this.tbLink4Dist.WordWrap = false;
			this.tbLink4Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.tbLink4Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// tbLink5Dist
			// 
			this.tbLink5Dist.Location = new System.Drawing.Point(215, 125);
			this.tbLink5Dist.Margin = new System.Windows.Forms.Padding(0);
			this.tbLink5Dist.Name = "tbLink5Dist";
			this.tbLink5Dist.ReadOnly = true;
			this.tbLink5Dist.Size = new System.Drawing.Size(35, 19);
			this.tbLink5Dist.TabIndex = 26;
			this.tbLink5Dist.Tag = "L5";
			this.tbLink5Dist.WordWrap = false;
			this.tbLink5Dist.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.tbLink5Dist.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// cbLink1UnitType
			// 
			this.cbLink1UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink1UnitType.Location = new System.Drawing.Point(125, 25);
			this.cbLink1UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink1UnitType.Name = "cbLink1UnitType";
			this.cbLink1UnitType.Size = new System.Drawing.Size(85, 20);
			this.cbLink1UnitType.TabIndex = 5;
			this.cbLink1UnitType.Tag = "L1";
			this.cbLink1UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.cbLink1UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink1UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink2UnitType
			// 
			this.cbLink2UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink2UnitType.Location = new System.Drawing.Point(125, 50);
			this.cbLink2UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink2UnitType.Name = "cbLink2UnitType";
			this.cbLink2UnitType.Size = new System.Drawing.Size(85, 20);
			this.cbLink2UnitType.TabIndex = 10;
			this.cbLink2UnitType.Tag = "L2";
			this.cbLink2UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.cbLink2UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink2UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink3UnitType
			// 
			this.cbLink3UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink3UnitType.Location = new System.Drawing.Point(125, 75);
			this.cbLink3UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink3UnitType.Name = "cbLink3UnitType";
			this.cbLink3UnitType.Size = new System.Drawing.Size(85, 20);
			this.cbLink3UnitType.TabIndex = 15;
			this.cbLink3UnitType.Tag = "L3";
			this.cbLink3UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.cbLink3UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink3UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink4UnitType
			// 
			this.cbLink4UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink4UnitType.Location = new System.Drawing.Point(125, 100);
			this.cbLink4UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink4UnitType.Name = "cbLink4UnitType";
			this.cbLink4UnitType.Size = new System.Drawing.Size(85, 20);
			this.cbLink4UnitType.TabIndex = 20;
			this.cbLink4UnitType.Tag = "L4";
			this.cbLink4UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.cbLink4UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink4UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink5UnitType
			// 
			this.cbLink5UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink5UnitType.Location = new System.Drawing.Point(125, 125);
			this.cbLink5UnitType.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink5UnitType.Name = "cbLink5UnitType";
			this.cbLink5UnitType.Size = new System.Drawing.Size(85, 20);
			this.cbLink5UnitType.TabIndex = 25;
			this.cbLink5UnitType.Tag = "L5";
			this.cbLink5UnitType.SelectedIndexChanged += new System.EventHandler(this.OnLinkUnitTypeSelectedIndexChanged);
			this.cbLink5UnitType.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink5UnitType.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink1Dest
			// 
			this.cbLink1Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink1Dest.Location = new System.Drawing.Point(45, 25);
			this.cbLink1Dest.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink1Dest.Name = "cbLink1Dest";
			this.cbLink1Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink1Dest.TabIndex = 4;
			this.cbLink1Dest.Tag = "L1";
			this.cbLink1Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.cbLink1Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink1Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink2Dest
			// 
			this.cbLink2Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink2Dest.Location = new System.Drawing.Point(45, 50);
			this.cbLink2Dest.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink2Dest.Name = "cbLink2Dest";
			this.cbLink2Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink2Dest.TabIndex = 9;
			this.cbLink2Dest.Tag = "L2";
			this.cbLink2Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.cbLink2Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink2Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink3Dest
			// 
			this.cbLink3Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink3Dest.Location = new System.Drawing.Point(45, 75);
			this.cbLink3Dest.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink3Dest.Name = "cbLink3Dest";
			this.cbLink3Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink3Dest.TabIndex = 14;
			this.cbLink3Dest.Tag = "L3";
			this.cbLink3Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.cbLink3Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink3Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink4Dest
			// 
			this.cbLink4Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink4Dest.Location = new System.Drawing.Point(45, 100);
			this.cbLink4Dest.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink4Dest.Name = "cbLink4Dest";
			this.cbLink4Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink4Dest.TabIndex = 19;
			this.cbLink4Dest.Tag = "L4";
			this.cbLink4Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.cbLink4Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink4Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// cbLink5Dest
			// 
			this.cbLink5Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLink5Dest.Location = new System.Drawing.Point(45, 125);
			this.cbLink5Dest.Margin = new System.Windows.Forms.Padding(0);
			this.cbLink5Dest.Name = "cbLink5Dest";
			this.cbLink5Dest.Size = new System.Drawing.Size(75, 20);
			this.cbLink5Dest.TabIndex = 24;
			this.cbLink5Dest.Tag = "L5";
			this.cbLink5Dest.SelectedIndexChanged += new System.EventHandler(this.OnLinkDestSelectedIndexChanged);
			this.cbLink5Dest.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			this.cbLink5Dest.MouseHover += new System.EventHandler(this.OnLinkMouseEnter);
			// 
			// btnGoLink1
			// 
			this.btnGoLink1.Enabled = false;
			this.btnGoLink1.Location = new System.Drawing.Point(255, 25);
			this.btnGoLink1.Margin = new System.Windows.Forms.Padding(0);
			this.btnGoLink1.Name = "btnGoLink1";
			this.btnGoLink1.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink1.TabIndex = 7;
			this.btnGoLink1.Tag = "L1";
			this.btnGoLink1.Text = "go";
			this.btnGoLink1.UseVisualStyleBackColor = true;
			this.btnGoLink1.Click += new System.EventHandler(this.OnLinkGoClick);
			this.btnGoLink1.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.btnGoLink1.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink2
			// 
			this.btnGoLink2.Enabled = false;
			this.btnGoLink2.Location = new System.Drawing.Point(255, 50);
			this.btnGoLink2.Margin = new System.Windows.Forms.Padding(0);
			this.btnGoLink2.Name = "btnGoLink2";
			this.btnGoLink2.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink2.TabIndex = 12;
			this.btnGoLink2.Tag = "L2";
			this.btnGoLink2.Text = "go";
			this.btnGoLink2.UseVisualStyleBackColor = true;
			this.btnGoLink2.Click += new System.EventHandler(this.OnLinkGoClick);
			this.btnGoLink2.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.btnGoLink2.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink3
			// 
			this.btnGoLink3.Enabled = false;
			this.btnGoLink3.Location = new System.Drawing.Point(255, 75);
			this.btnGoLink3.Margin = new System.Windows.Forms.Padding(0);
			this.btnGoLink3.Name = "btnGoLink3";
			this.btnGoLink3.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink3.TabIndex = 17;
			this.btnGoLink3.Tag = "L3";
			this.btnGoLink3.Text = "go";
			this.btnGoLink3.UseVisualStyleBackColor = true;
			this.btnGoLink3.Click += new System.EventHandler(this.OnLinkGoClick);
			this.btnGoLink3.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.btnGoLink3.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink4
			// 
			this.btnGoLink4.Enabled = false;
			this.btnGoLink4.Location = new System.Drawing.Point(255, 100);
			this.btnGoLink4.Margin = new System.Windows.Forms.Padding(0);
			this.btnGoLink4.Name = "btnGoLink4";
			this.btnGoLink4.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink4.TabIndex = 22;
			this.btnGoLink4.Tag = "L4";
			this.btnGoLink4.Text = "go";
			this.btnGoLink4.UseVisualStyleBackColor = true;
			this.btnGoLink4.Click += new System.EventHandler(this.OnLinkGoClick);
			this.btnGoLink4.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.btnGoLink4.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnGoLink5
			// 
			this.btnGoLink5.Enabled = false;
			this.btnGoLink5.Location = new System.Drawing.Point(255, 125);
			this.btnGoLink5.Margin = new System.Windows.Forms.Padding(0);
			this.btnGoLink5.Name = "btnGoLink5";
			this.btnGoLink5.Size = new System.Drawing.Size(30, 20);
			this.btnGoLink5.TabIndex = 27;
			this.btnGoLink5.Tag = "L5";
			this.btnGoLink5.Text = "go";
			this.btnGoLink5.UseVisualStyleBackColor = true;
			this.btnGoLink5.Click += new System.EventHandler(this.OnLinkGoClick);
			this.btnGoLink5.MouseEnter += new System.EventHandler(this.OnLinkMouseEnter);
			this.btnGoLink5.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// gbNodeEditor
			// 
			this.gbNodeEditor.Controls.Add(this.btnCut);
			this.gbNodeEditor.Controls.Add(this.btnCopy);
			this.gbNodeEditor.Controls.Add(this.btnPaste);
			this.gbNodeEditor.Controls.Add(this.btnDelete);
			this.gbNodeEditor.Location = new System.Drawing.Point(245, 153);
			this.gbNodeEditor.Margin = new System.Windows.Forms.Padding(0);
			this.gbNodeEditor.Name = "gbNodeEditor";
			this.gbNodeEditor.Size = new System.Drawing.Size(290, 52);
			this.gbNodeEditor.TabIndex = 2;
			this.gbNodeEditor.TabStop = false;
			this.gbNodeEditor.Text = " Node editor ";
			// 
			// btnCut
			// 
			this.btnCut.Enabled = false;
			this.btnCut.Location = new System.Drawing.Point(10, 15);
			this.btnCut.Margin = new System.Windows.Forms.Padding(0);
			this.btnCut.Name = "btnCut";
			this.btnCut.Size = new System.Drawing.Size(65, 30);
			this.btnCut.TabIndex = 0;
			this.btnCut.Text = "Cut";
			this.toolTip1.SetToolTip(this.btnCut, "cuts the selected node with its Patrol and Spawn data (not Link data)");
			this.btnCut.Click += new System.EventHandler(this.OnCutClick);
			// 
			// btnCopy
			// 
			this.btnCopy.Enabled = false;
			this.btnCopy.Location = new System.Drawing.Point(79, 15);
			this.btnCopy.Margin = new System.Windows.Forms.Padding(0);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(65, 30);
			this.btnCopy.TabIndex = 1;
			this.btnCopy.Text = "Copy";
			this.toolTip1.SetToolTip(this.btnCopy, "copies Patrol and Spawn data of the selected node");
			this.btnCopy.Click += new System.EventHandler(this.OnCopyClick);
			// 
			// btnPaste
			// 
			this.btnPaste.Enabled = false;
			this.btnPaste.Location = new System.Drawing.Point(148, 15);
			this.btnPaste.Margin = new System.Windows.Forms.Padding(0);
			this.btnPaste.Name = "btnPaste";
			this.btnPaste.Size = new System.Drawing.Size(65, 30);
			this.btnPaste.TabIndex = 2;
			this.btnPaste.Text = "Paste";
			this.toolTip1.SetToolTip(this.btnPaste, "pastes copied Patrol and Spawn data to the selected node");
			this.btnPaste.Click += new System.EventHandler(this.OnPasteClick);
			// 
			// btnDelete
			// 
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new System.Drawing.Point(217, 15);
			this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(65, 30);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.Text = "Delete";
			this.toolTip1.SetToolTip(this.btnDelete, "deletes the selected node");
			this.btnDelete.Click += new System.EventHandler(this.OnDeleteClick);
			// 
			// btnOg
			// 
			this.btnOg.Location = new System.Drawing.Point(540, 28);
			this.btnOg.Margin = new System.Windows.Forms.Padding(0);
			this.btnOg.Name = "btnOg";
			this.btnOg.Size = new System.Drawing.Size(20, 120);
			this.btnOg.TabIndex = 3;
			this.btnOg.Text = "o\r\ng";
			this.btnOg.UseVisualStyleBackColor = true;
			this.btnOg.Click += new System.EventHandler(this.OnOgClick);
			this.btnOg.MouseEnter += new System.EventHandler(this.OnOgMouseEnter);
			this.btnOg.MouseLeave += new System.EventHandler(this.OnLinkMouseLeave);
			// 
			// btnTallyho
			// 
			this.btnTallyho.Location = new System.Drawing.Point(565, 28);
			this.btnTallyho.Margin = new System.Windows.Forms.Padding(0);
			this.btnTallyho.Name = "btnTallyho";
			this.btnTallyho.Size = new System.Drawing.Size(20, 120);
			this.btnTallyho.TabIndex = 4;
			this.btnTallyho.Text = "tally";
			this.btnTallyho.UseVisualStyleBackColor = true;
			this.btnTallyho.Click += new System.EventHandler(this.OnTallyhoClick);
			// 
			// btnSave
			// 
			this.btnSave.Enabled = false;
			this.btnSave.ForeColor = System.Drawing.Color.Chocolate;
			this.btnSave.Location = new System.Drawing.Point(539, 158);
			this.btnSave.Margin = new System.Windows.Forms.Padding(0);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(46, 46);
			this.btnSave.TabIndex = 5;
			this.btnSave.Text = "save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.OnSaveClick);
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
			this.Controls.Add(this.tsMain);
			this.Controls.Add(this.pnlDataFields);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "RouteView";
			this.Size = new System.Drawing.Size(640, 480);
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.pnlDataFields.ResumeLayout(false);
			this.pnlDataFieldsLeft.ResumeLayout(false);
			this.gbNodeData.ResumeLayout(false);
			this.gbTileData.ResumeLayout(false);
			this.gbLinkData.ResumeLayout(false);
			this.gbLinkData.PerformLayout();
			this.gbNodeEditor.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}
