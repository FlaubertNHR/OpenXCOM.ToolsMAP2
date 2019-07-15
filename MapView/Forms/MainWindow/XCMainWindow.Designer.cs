namespace MapView
{
	partial class XCMainWindow
	{
		#region Designer
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


/*		These lines are going to be deleted when changes are made in the designer.

#if !__MonoCS__
			tvMaps = new MapView.BufferedTreeView();
#else
			tvMaps = new System.Windows.Forms.TreeView();
#endif

		And this line will be inserted:
			tvMaps = new System.Windows.Forms.TreeView();

		Copy-paste things back to what it should be. NOTE: The instantiation
		cannot be done in the cTor because the cTor isn't run by the designer;
		but changing the code of the class to instantiate the tree in, say, the
		OnLoad event is a bit too involved.
 */
		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XCMainWindow));
#if !__MonoCS__
			tvMaps = new MapView.BufferedTreeView();
#else
			tvMaps = new System.Windows.Forms.TreeView();
#endif
			this.mmMain = new System.Windows.Forms.MainMenu(this.components);
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.miSaveAll = new System.Windows.Forms.MenuItem();
			this.miSeparator1 = new System.Windows.Forms.MenuItem();
			this.miSaveMap = new System.Windows.Forms.MenuItem();
			this.miSaveRoutes = new System.Windows.Forms.MenuItem();
			this.miSaveAs = new System.Windows.Forms.MenuItem();
			this.miSeparator2 = new System.Windows.Forms.MenuItem();
			this.miSaveMaptree = new System.Windows.Forms.MenuItem();
			this.miSeparator3 = new System.Windows.Forms.MenuItem();
			this.miSaveImage = new System.Windows.Forms.MenuItem();
			this.miResize = new System.Windows.Forms.MenuItem();
			this.miInfo = new System.Windows.Forms.MenuItem();
			this.miScanG = new System.Windows.Forms.MenuItem();
			this.miReloadTerrains = new System.Windows.Forms.MenuItem();
			this.miSeparator4 = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.menuEdit = new System.Windows.Forms.MenuItem();
			this.miConfigurator = new System.Windows.Forms.MenuItem();
			this.miOptions = new System.Windows.Forms.MenuItem();
			this.menuAnimation = new System.Windows.Forms.MenuItem();
			this.miAnimate = new System.Windows.Forms.MenuItem();
			this.miDoors = new System.Windows.Forms.MenuItem();
			this.miGrid = new System.Windows.Forms.MenuItem();
			this.menuViewers = new System.Windows.Forms.MenuItem();
			this.menuHelp = new System.Windows.Forms.MenuItem();
			this.sfdSaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.tsslScale = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslMapLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslDimensions = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslPosition = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslSelectionSize = new System.Windows.Forms.ToolStripStatusLabel();
			this.tscPanel = new System.Windows.Forms.ToolStripContainer();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.cmMapTreeMenu = new System.Windows.Forms.ContextMenu();
			this.csSplitter = new DSShared.Windows.CollapsibleSplitter();
			this.ssMain.SuspendLayout();
			this.tscPanel.TopToolStripPanel.SuspendLayout();
			this.tscPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvMaps
			// 
			this.tvMaps.BackColor = System.Drawing.SystemColors.Control;
			this.tvMaps.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvMaps.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.tvMaps.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tvMaps.HideSelection = false;
			this.tvMaps.Indent = 15;
			this.tvMaps.Location = new System.Drawing.Point(0, 0);
			this.tvMaps.Margin = new System.Windows.Forms.Padding(0);
			this.tvMaps.Name = "tvMaps";
			this.tvMaps.Size = new System.Drawing.Size(240, 454);
			this.tvMaps.TabIndex = 0;
			this.tvMaps.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tv_DrawNode);
			this.tvMaps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMapTreeMouseDown);
			// 
			// mmMain
			// 
			this.mmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.menuFile,
			this.menuEdit,
			this.menuAnimation,
			this.menuViewers,
			this.menuHelp});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miSaveAll,
			this.miSeparator1,
			this.miSaveMap,
			this.miSaveRoutes,
			this.miSaveAs,
			this.miSeparator2,
			this.miSaveMaptree,
			this.miSeparator3,
			this.miSaveImage,
			this.miResize,
			this.miInfo,
			this.miScanG,
			this.miReloadTerrains,
			this.miSeparator4,
			this.miQuit});
			this.menuFile.Text = "&File";
			// 
			// miSaveAll
			// 
			this.miSaveAll.Enabled = false;
			this.miSaveAll.Index = 0;
			this.miSaveAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.miSaveAll.Text = "Save &All";
			this.miSaveAll.Click += new System.EventHandler(this.OnSaveAllClick);
			// 
			// miSeparator1
			// 
			this.miSeparator1.Index = 1;
			this.miSeparator1.Text = "-";
			// 
			// miSaveMap
			// 
			this.miSaveMap.Enabled = false;
			this.miSaveMap.Index = 2;
			this.miSaveMap.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.miSaveMap.Text = "Save &Map";
			this.miSaveMap.Click += new System.EventHandler(this.OnSaveMapClick);
			// 
			// miSaveRoutes
			// 
			this.miSaveRoutes.Enabled = false;
			this.miSaveRoutes.Index = 3;
			this.miSaveRoutes.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
			this.miSaveRoutes.Text = "Save &Routes";
			this.miSaveRoutes.Click += new System.EventHandler(this.OnSaveRoutesClick);
			// 
			// miSaveAs
			// 
			this.miSaveAs.Enabled = false;
			this.miSaveAs.Index = 4;
			this.miSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miSaveAs.Text = "Sav&e As ...";
			this.miSaveAs.Click += new System.EventHandler(this.OnSaveAsClick);
			// 
			// miSeparator2
			// 
			this.miSeparator2.Index = 5;
			this.miSeparator2.Text = "-";
			// 
			// miSaveMaptree
			// 
			this.miSaveMaptree.Index = 6;
			this.miSaveMaptree.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
			this.miSaveMaptree.Text = "Save Map&tree";
			this.miSaveMaptree.Click += new System.EventHandler(this.OnSaveMaptreeClick);
			// 
			// miSeparator3
			// 
			this.miSeparator3.Index = 7;
			this.miSeparator3.Text = "-";
			// 
			// miSaveImage
			// 
			this.miSaveImage.Enabled = false;
			this.miSaveImage.Index = 8;
			this.miSaveImage.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miSaveImage.Text = "Save &Image ...";
			this.miSaveImage.Click += new System.EventHandler(this.OnSaveImageClick);
			// 
			// miResize
			// 
			this.miResize.Enabled = false;
			this.miResize.Index = 9;
			this.miResize.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.miResize.Text = "Resi&ze Map";
			this.miResize.Click += new System.EventHandler(this.OnMapResizeClick);
			// 
			// miInfo
			// 
			this.miInfo.Enabled = false;
			this.miInfo.Index = 10;
			this.miInfo.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
			this.miInfo.Text = "Map In&fo";
			this.miInfo.Click += new System.EventHandler(this.OnMapInfoClick);
			// 
			// miScanG
			// 
			this.miScanG.Enabled = false;
			this.miScanG.Index = 11;
			this.miScanG.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.miScanG.Text = "Scan&G view";
			this.miScanG.Click += new System.EventHandler(this.OnScanGClick);
			// 
			// miReloadTerrains
			// 
			this.miReloadTerrains.Enabled = false;
			this.miReloadTerrains.Index = 12;
			this.miReloadTerrains.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miReloadTerrains.Text = "Reload terrai&ns";
			this.miReloadTerrains.Click += new System.EventHandler(this.OnReloadTerrainsClick);
			// 
			// miSeparator4
			// 
			this.miSeparator4.Index = 13;
			this.miSeparator4.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 14;
			this.miQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnQuitClick);
			// 
			// menuEdit
			// 
			this.menuEdit.Index = 1;
			this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miConfigurator,
			this.miOptions});
			this.menuEdit.Text = "&Edit";
			// 
			// miConfigurator
			// 
			this.miConfigurator.Index = 0;
			this.miConfigurator.Text = "&Configurator";
			this.miConfigurator.Click += new System.EventHandler(this.OnConfiguratorClick);
			// 
			// miOptions
			// 
			this.miOptions.Index = 1;
			this.miOptions.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miOptions.Text = "&Options";
			this.miOptions.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// menuAnimation
			// 
			this.menuAnimation.Index = 2;
			this.menuAnimation.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miAnimate,
			this.miDoors,
			this.miGrid});
			this.menuAnimation.Text = "menuAnimation";
			this.menuAnimation.Visible = false;
			// 
			// miAnimate
			// 
			this.miAnimate.Index = 0;
			this.miAnimate.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miAnimate.Text = "miAnimate";
			this.miAnimate.Click += new System.EventHandler(this.OnAnimateClick);
			// 
			// miDoors
			// 
			this.miDoors.Index = 1;
			this.miDoors.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.miDoors.Text = "miDoors";
			this.miDoors.Click += new System.EventHandler(this.OnToggleDoorsClick);
			// 
			// miGrid
			// 
			this.miGrid.Index = 2;
			this.miGrid.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.miGrid.Text = "miGrid";
			this.miGrid.Click += new System.EventHandler(this.OnToggleGridClick);
			// 
			// menuViewers
			// 
			this.menuViewers.Enabled = false;
			this.menuViewers.Index = 3;
			this.menuViewers.Text = "&Viewers";
			// 
			// menuHelp
			// 
			this.menuHelp.Index = 4;
			this.menuHelp.Text = "&Help";
			// 
			// sfdSaveDialog
			// 
			this.sfdSaveDialog.DefaultExt = "PNG";
			this.sfdSaveDialog.Filter = "PNG files|*.PNG|All files (*.*)|*.*";
			this.sfdSaveDialog.RestoreDirectory = true;
			// 
			// ssMain
			// 
			this.ssMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsslScale,
			this.tsslMapLabel,
			this.tsslDimensions,
			this.tsslPosition,
			this.tsslSelectionSize});
			this.ssMain.Location = new System.Drawing.Point(248, 432);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(544, 22);
			this.ssMain.TabIndex = 3;
			// 
			// tsslScale
			// 
			this.tsslScale.AutoSize = false;
			this.tsslScale.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.tsslScale.Name = "tsslScale";
			this.tsslScale.Size = new System.Drawing.Size(70, 17);
			// 
			// tsslMapLabel
			// 
			this.tsslMapLabel.AutoSize = false;
			this.tsslMapLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.tsslMapLabel.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsslMapLabel.Name = "tsslMapLabel";
			this.tsslMapLabel.Size = new System.Drawing.Size(215, 17);
			this.tsslMapLabel.Spring = true;
			// 
			// tsslDimensions
			// 
			this.tsslDimensions.AutoSize = false;
			this.tsslDimensions.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.tsslDimensions.Name = "tsslDimensions";
			this.tsslDimensions.Size = new System.Drawing.Size(80, 17);
			// 
			// tsslPosition
			// 
			this.tsslPosition.AutoSize = false;
			this.tsslPosition.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.tsslPosition.Name = "tsslPosition";
			this.tsslPosition.Size = new System.Drawing.Size(100, 17);
			// 
			// tsslSelectionSize
			// 
			this.tsslSelectionSize.AutoSize = false;
			this.tsslSelectionSize.Margin = new System.Windows.Forms.Padding(0, 3, 2, 2);
			this.tsslSelectionSize.Name = "tsslSelectionSize";
			this.tsslSelectionSize.Size = new System.Drawing.Size(62, 17);
			// 
			// tscPanel
			// 
			// 
			// tscPanel.BottomToolStripPanel
			// 
			this.tscPanel.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tscPanel.ContentPanel
			// 
			this.tscPanel.ContentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.tscPanel.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tscPanel.ContentPanel.Size = new System.Drawing.Size(544, 407);
			this.tscPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// tscPanel.LeftToolStripPanel
			// 
			this.tscPanel.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.LeftToolStripPanelVisible = false;
			this.tscPanel.Location = new System.Drawing.Point(248, 0);
			this.tscPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tscPanel.Name = "tscPanel";
			// 
			// tscPanel.RightToolStripPanel
			// 
			this.tscPanel.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.RightToolStripPanelVisible = false;
			this.tscPanel.Size = new System.Drawing.Size(544, 432);
			this.tscPanel.TabIndex = 2;
			// 
			// tscPanel.TopToolStripPanel
			// 
			this.tscPanel.TopToolStripPanel.Controls.Add(this.tsTools);
			this.tscPanel.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tsTools
			// 
			this.tsTools.Dock = System.Windows.Forms.DockStyle.None;
			this.tsTools.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsTools.GripMargin = new System.Windows.Forms.Padding(0);
			this.tsTools.Location = new System.Drawing.Point(3, 0);
			this.tsTools.Name = "tsTools";
			this.tsTools.Padding = new System.Windows.Forms.Padding(0);
			this.tsTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsTools.Size = new System.Drawing.Size(107, 25);
			this.tsTools.TabIndex = 0;
			// 
			// csSplitter
			// 
			this.csSplitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
			this.csSplitter.ControlToHide = this.tvMaps;
			this.csSplitter.Location = new System.Drawing.Point(240, 0);
			this.csSplitter.MinimumSize = new System.Drawing.Size(5, 5);
			this.csSplitter.Name = "csSplitter";
			this.csSplitter.Size = new System.Drawing.Size(8, 454);
			this.csSplitter.TabIndex = 1;
			this.csSplitter.TabStop = false;
			// 
			// XCMainWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(792, 454);
			this.Controls.Add(this.tscPanel);
			this.Controls.Add(this.ssMain);
			this.Controls.Add(this.csSplitter);
			this.Controls.Add(this.tvMaps);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(800, 480);
			this.Menu = this.mmMain;
			this.MinimumSize = new System.Drawing.Size(800, 480);
			this.Name = "XCMainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Map Editor ||";
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.tscPanel.TopToolStripPanel.ResumeLayout(false);
			this.tscPanel.TopToolStripPanel.PerformLayout();
			this.tscPanel.ResumeLayout(false);
			this.tscPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#if !__MonoCS__
		private MapView.BufferedTreeView tvMaps;
#else
		private System.Windows.Forms.TreeView tvMaps;
#endif

		private System.Windows.Forms.ContextMenu cmMapTreeMenu;

		private System.Windows.Forms.ToolStripContainer tscPanel;


		private System.Windows.Forms.MainMenu mmMain;

		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem miSaveAll;
		private System.Windows.Forms.MenuItem miSeparator1;
		private System.Windows.Forms.MenuItem miSaveMap;
		private System.Windows.Forms.MenuItem miSaveRoutes;
		private System.Windows.Forms.MenuItem miSaveAs;
		private System.Windows.Forms.MenuItem miSeparator2;
		private System.Windows.Forms.MenuItem miSaveMaptree;
		private System.Windows.Forms.MenuItem miSeparator3;
		private System.Windows.Forms.MenuItem miSaveImage;
		private System.Windows.Forms.MenuItem miResize;
		private System.Windows.Forms.MenuItem miInfo;
		private System.Windows.Forms.MenuItem miScanG;
		private System.Windows.Forms.MenuItem miReloadTerrains;
		private System.Windows.Forms.MenuItem miSeparator4;
		private System.Windows.Forms.MenuItem miQuit;

		private System.Windows.Forms.MenuItem menuViewers;

		private System.Windows.Forms.MenuItem menuEdit;
		private System.Windows.Forms.MenuItem miOptions;
		private System.Windows.Forms.MenuItem miConfigurator;

		private System.Windows.Forms.MenuItem menuHelp;

		private System.Windows.Forms.MenuItem menuAnimation;
		private System.Windows.Forms.MenuItem miAnimate;
		private System.Windows.Forms.MenuItem miDoors;
		private System.Windows.Forms.MenuItem miGrid; // for lack of a better place to put it.


		private System.Windows.Forms.SaveFileDialog sfdSaveDialog;
		private DSShared.Windows.CollapsibleSplitter csSplitter;

		private System.Windows.Forms.ToolStrip tsTools;

		private System.Windows.Forms.StatusStrip ssMain;

		private System.Windows.Forms.ToolStripStatusLabel tsslScale;
		private System.Windows.Forms.ToolStripStatusLabel tsslMapLabel;
		private System.Windows.Forms.ToolStripStatusLabel tsslDimensions;
		private System.Windows.Forms.ToolStripStatusLabel tsslPosition;
		private System.Windows.Forms.ToolStripStatusLabel tsslSelectionSize;
		#endregion Designer
	}
}
