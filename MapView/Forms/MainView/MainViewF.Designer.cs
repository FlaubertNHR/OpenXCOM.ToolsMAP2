using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	partial class MainViewF
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;


		private ContextMenu cmMapTreeMenu;

		internal ToolStripContainer tscPanel;


		private MainMenu mmMain;

		private MenuItem menuFile;
		private MenuItem miSaveAll;
		private MenuItem miSeparator1;
		private MenuItem miSaveMap;
		private MenuItem miSaveRoutes;
		private MenuItem miExport;
		private MenuItem miSeparator2;
		private MenuItem miSaveMaptree;
		private MenuItem miSeparator3;
		private MenuItem miReload;
		private MenuItem miSeparator4;
		private MenuItem miScreenshot;
		private MenuItem miSeparator5;
		private MenuItem miQuit;

		private MenuItem menuEdit;
		private MenuItem miModifySize;
		private MenuItem miTilepartSubstitution;
		private MenuItem miClearRecordsExceeded;
		private MenuItem miSeparator6;
		private MenuItem miConfigurator;
		private MenuItem miOptions;

		private MenuItem menuViewers;

		private MenuItem menuToner;
		private MenuItem miNone;
		private MenuItem miGray;
		private MenuItem miRed;
		private MenuItem miGreen;
		private MenuItem miBlue;


		private MenuItem menuHelp;
		private MenuItem miHelp;
		private MenuItem miColors;
		private MenuItem miAbout;
		private MenuItem miSeparator7;
		private MenuItem miMapInfo;


		private ToolStrip tsTools;

		private StatusStrip ssMain;

		private ToolStripStatusLabel tsslScale;
		private ToolStripStatusLabel tsslMapLabel;
		private ToolStripStatusLabel tsslDimensions;
		private ToolStripStatusLabel tsslPosition;
		private ToolStripStatusLabel tsslSelectionSize;


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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainViewF));
			this.mmMain = new System.Windows.Forms.MainMenu(this.components);
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.miSaveAll = new System.Windows.Forms.MenuItem();
			this.miSeparator1 = new System.Windows.Forms.MenuItem();
			this.miSaveMap = new System.Windows.Forms.MenuItem();
			this.miSaveRoutes = new System.Windows.Forms.MenuItem();
			this.miExport = new System.Windows.Forms.MenuItem();
			this.miSeparator2 = new System.Windows.Forms.MenuItem();
			this.miSaveMaptree = new System.Windows.Forms.MenuItem();
			this.miSeparator3 = new System.Windows.Forms.MenuItem();
			this.miReload = new System.Windows.Forms.MenuItem();
			this.miSeparator4 = new System.Windows.Forms.MenuItem();
			this.miScreenshot = new System.Windows.Forms.MenuItem();
			this.miSeparator5 = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.menuEdit = new System.Windows.Forms.MenuItem();
			this.miModifySize = new System.Windows.Forms.MenuItem();
			this.miTilepartSubstitution = new System.Windows.Forms.MenuItem();
			this.miClearRecordsExceeded = new System.Windows.Forms.MenuItem();
			this.miSeparator6 = new System.Windows.Forms.MenuItem();
			this.miConfigurator = new System.Windows.Forms.MenuItem();
			this.miOptions = new System.Windows.Forms.MenuItem();
			this.menuViewers = new System.Windows.Forms.MenuItem();
			this.menuToner = new System.Windows.Forms.MenuItem();
			this.miNone = new System.Windows.Forms.MenuItem();
			this.miGray = new System.Windows.Forms.MenuItem();
			this.miRed = new System.Windows.Forms.MenuItem();
			this.miGreen = new System.Windows.Forms.MenuItem();
			this.miBlue = new System.Windows.Forms.MenuItem();
			this.menuHelp = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miColors = new System.Windows.Forms.MenuItem();
			this.miAbout = new System.Windows.Forms.MenuItem();
			this.miSeparator7 = new System.Windows.Forms.MenuItem();
			this.miMapInfo = new System.Windows.Forms.MenuItem();
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.tsslScale = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslMapLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslDimensions = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslPosition = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslSelectionSize = new System.Windows.Forms.ToolStripStatusLabel();
			this.tscPanel = new System.Windows.Forms.ToolStripContainer();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.cmMapTreeMenu = new System.Windows.Forms.ContextMenu();
			this.ssMain.SuspendLayout();
			this.tscPanel.TopToolStripPanel.SuspendLayout();
			this.tscPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMain
			// 
			this.mmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.menuFile,
			this.menuEdit,
			this.menuViewers,
			this.menuToner,
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
			this.miExport,
			this.miSeparator2,
			this.miSaveMaptree,
			this.miSeparator3,
			this.miReload,
			this.miSeparator4,
			this.miScreenshot,
			this.miSeparator5,
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
			// miExport
			// 
			this.miExport.Enabled = false;
			this.miExport.Index = 4;
			this.miExport.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miExport.Text = "&Export Map and Routes ...";
			this.miExport.Click += new System.EventHandler(this.OnExportMapRoutesClick);
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
			// miReload
			// 
			this.miReload.Enabled = false;
			this.miReload.Index = 8;
			this.miReload.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.miReload.Text = "Re&load";
			this.miReload.Click += new System.EventHandler(this.OnReloadClick);
			// 
			// miSeparator4
			// 
			this.miSeparator4.Index = 9;
			this.miSeparator4.Text = "-";
			// 
			// miScreenshot
			// 
			this.miScreenshot.Enabled = false;
			this.miScreenshot.Index = 10;
			this.miScreenshot.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miScreenshot.Text = "Scree&nshot ...";
			this.miScreenshot.Click += new System.EventHandler(this.OnScreenshotClick);
			// 
			// miSeparator5
			// 
			this.miSeparator5.Index = 11;
			this.miSeparator5.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 12;
			this.miQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnQuitClick);
			// 
			// menuEdit
			// 
			this.menuEdit.Index = 1;
			this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miModifySize,
			this.miTilepartSubstitution,
			this.miClearRecordsExceeded,
			this.miSeparator6,
			this.miConfigurator,
			this.miOptions});
			this.menuEdit.Text = "&Edit";
			// 
			// miModifySize
			// 
			this.miModifySize.Enabled = false;
			this.miModifySize.Index = 0;
			this.miModifySize.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.miModifySize.Text = "Modify Map Si&ze";
			this.miModifySize.Click += new System.EventHandler(this.OnMapResizeClick);
			// 
			// miTilepartSubstitution
			// 
			this.miTilepartSubstitution.Enabled = false;
			this.miTilepartSubstitution.Index = 1;
			this.miTilepartSubstitution.Shortcut = System.Windows.Forms.Shortcut.CtrlU;
			this.miTilepartSubstitution.Text = "Tilepart S&ubstitution";
			this.miTilepartSubstitution.Click += new System.EventHandler(this.OnTilepartSubstitutionClick);
			// 
			// miClearRecordsExceeded
			// 
			this.miClearRecordsExceeded.Index = 2;
			this.miClearRecordsExceeded.Text = "Clear all BypassRecordsExceeded flags";
			this.miClearRecordsExceeded.Click += new System.EventHandler(this.OnClearRecordsExceededClick);
			// 
			// miSeparator6
			// 
			this.miSeparator6.Index = 3;
			this.miSeparator6.Text = "-";
			// 
			// miConfigurator
			// 
			this.miConfigurator.Index = 4;
			this.miConfigurator.Text = "&Configurator";
			this.miConfigurator.Click += new System.EventHandler(this.OnConfiguratorClick);
			// 
			// miOptions
			// 
			this.miOptions.Index = 5;
			this.miOptions.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miOptions.Text = "&Options";
			this.miOptions.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// menuViewers
			// 
			this.menuViewers.Enabled = false;
			this.menuViewers.Index = 2;
			this.menuViewers.Text = "&Viewers";
			// 
			// menuToner
			// 
			this.menuToner.Index = 3;
			this.menuToner.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miNone,
			this.miGray,
			this.miRed,
			this.miGreen,
			this.miBlue});
			this.menuToner.Text = "Toner";
			// 
			// miNone
			// 
			this.miNone.Index = 0;
			this.miNone.Text = "None";
			this.miNone.Click += new System.EventHandler(this.OnTonerClick);
			// 
			// miGray
			// 
			this.miGray.Index = 1;
			this.miGray.Text = "Gray";
			this.miGray.Click += new System.EventHandler(this.OnTonerClick);
			// 
			// miRed
			// 
			this.miRed.Index = 2;
			this.miRed.Text = "Red";
			this.miRed.Click += new System.EventHandler(this.OnTonerClick);
			// 
			// miGreen
			// 
			this.miGreen.Index = 3;
			this.miGreen.Text = "Green";
			this.miGreen.Click += new System.EventHandler(this.OnTonerClick);
			// 
			// miBlue
			// 
			this.miBlue.Index = 4;
			this.miBlue.Text = "Blue";
			this.miBlue.Click += new System.EventHandler(this.OnTonerClick);
			// 
			// menuHelp
			// 
			this.menuHelp.Index = 4;
			this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miHelp,
			this.miColors,
			this.miAbout,
			this.miSeparator7,
			this.miMapInfo});
			this.menuHelp.Text = "&Help";
			// 
			// miHelp
			// 
			this.miHelp.Index = 0;
			this.miHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.miHelp.Text = "CHM &Help";
			this.miHelp.Click += new System.EventHandler(this.OnHelpClick);
			// 
			// miColors
			// 
			this.miColors.Index = 1;
			this.miColors.Text = "Colors";
			this.miColors.Click += new System.EventHandler(this.OnColorsClick);
			// 
			// miAbout
			// 
			this.miAbout.Index = 2;
			this.miAbout.Text = "About";
			this.miAbout.Click += new System.EventHandler(this.OnAboutClick);
			// 
			// miSeparator7
			// 
			this.miSeparator7.Index = 3;
			this.miSeparator7.Text = "-";
			// 
			// miMapInfo
			// 
			this.miMapInfo.Enabled = false;
			this.miMapInfo.Index = 4;
			this.miMapInfo.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miMapInfo.Text = "Map &Info";
			this.miMapInfo.Click += new System.EventHandler(this.OnMapInfoClick);
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
			this.ssMain.Location = new System.Drawing.Point(0, 432);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(792, 22);
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
			this.tsslMapLabel.Size = new System.Drawing.Size(465, 17);
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
			this.tscPanel.ContentPanel.Size = new System.Drawing.Size(792, 407);
			this.tscPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// tscPanel.LeftToolStripPanel
			// 
			this.tscPanel.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.LeftToolStripPanelVisible = false;
			this.tscPanel.Location = new System.Drawing.Point(0, 0);
			this.tscPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tscPanel.Name = "tscPanel";
			// 
			// tscPanel.RightToolStripPanel
			// 
			this.tscPanel.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.RightToolStripPanelVisible = false;
			this.tscPanel.Size = new System.Drawing.Size(792, 432);
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
			// MainViewF
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(792, 454);
			this.Controls.Add(this.tscPanel);
			this.Controls.Add(this.ssMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(800, 480);
			this.Menu = this.mmMain;
			this.MinimumSize = new System.Drawing.Size(800, 480);
			this.Name = "MainViewF";
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
		#endregion Designer
	}
}
