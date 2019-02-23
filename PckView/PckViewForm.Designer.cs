namespace PckView
{
	partial class PckViewForm
	{
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.mmMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miFileMenu = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miOpenBigobs = new System.Windows.Forms.MenuItem();
			this.miCreate = new System.Windows.Forms.MenuItem();
			this.miNewTerrain = new System.Windows.Forms.MenuItem();
			this.miNewBigobs = new System.Windows.Forms.MenuItem();
			this.miNewUnitUfo = new System.Windows.Forms.MenuItem();
			this.miNewUnitTftd = new System.Windows.Forms.MenuItem();
			this.miOpenScanG = new System.Windows.Forms.MenuItem();
			this.miSeparator1 = new System.Windows.Forms.MenuItem();
			this.miCompare = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSaveAs = new System.Windows.Forms.MenuItem();
			this.miExportSprites = new System.Windows.Forms.MenuItem();
			this.miExportSpritesheet = new System.Windows.Forms.MenuItem();
			this.miImportSpritesheet = new System.Windows.Forms.MenuItem();
			this.miHq2x = new System.Windows.Forms.MenuItem();
			this.miSeparator2 = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.miPaletteMenu = new System.Windows.Forms.MenuItem();
			this.miTransparentMenu = new System.Windows.Forms.MenuItem();
			this.miTransparent = new System.Windows.Forms.MenuItem();
			this.miBytesMenu = new System.Windows.Forms.MenuItem();
			this.miBytes = new System.Windows.Forms.MenuItem();
			this.miHelpMenu = new System.Windows.Forms.MenuItem();
			this.miAbout = new System.Windows.Forms.MenuItem();
			this.miConsole = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.ss_Status = new System.Windows.Forms.StatusStrip();
			this.tssl_TilesTotal = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_TileOver = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_TileSelected = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_SpritesetLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.ss_Status.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMainMenu
			// 
			this.mmMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miFileMenu,
			this.miPaletteMenu,
			this.miTransparentMenu,
			this.miBytesMenu,
			this.miHelpMenu});
			// 
			// miFileMenu
			// 
			this.miFileMenu.Index = 0;
			this.miFileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miOpen,
			this.miOpenBigobs,
			this.miCreate,
			this.miNewTerrain,
			this.miNewBigobs,
			this.miNewUnitUfo,
			this.miNewUnitTftd,
			this.miOpenScanG,
			this.miSeparator1,
			this.miCompare,
			this.miSave,
			this.miSaveAs,
			this.miExportSprites,
			this.miExportSpritesheet,
			this.miImportSpritesheet,
			this.miHq2x,
			this.miSeparator2,
			this.miQuit});
			this.miFileMenu.Text = "&File";
			// 
			// miOpen
			// 
			this.miOpen.Index = 0;
			this.miOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miOpen.Text = "&Open Pck (terrain/unit) file ...";
			this.miOpen.Click += new System.EventHandler(this.OnOpenClick);
			// 
			// miOpenBigobs
			// 
			this.miOpenBigobs.Index = 1;
			this.miOpenBigobs.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.miOpenBigobs.Text = "Open Pck (bi&gobs) file ...";
			this.miOpenBigobs.Click += new System.EventHandler(this.OnOpenBigobsClick);
			// 
			// miCreate
			// 
			this.miCreate.Index = 2;
			this.miCreate.Text = "Create Pck file";
			// 
			// miNewTerrain
			// 
			this.miNewTerrain.Index = 3;
			this.miNewTerrain.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.miNewTerrain.Text = "&Create terrain ...";
			this.miNewTerrain.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miNewBigobs
			// 
			this.miNewBigobs.Index = 4;
			this.miNewBigobs.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miNewBigobs.Text = "Create b&igobs ...";
			this.miNewBigobs.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miNewUnitUfo
			// 
			this.miNewUnitUfo.Index = 5;
			this.miNewUnitUfo.Shortcut = System.Windows.Forms.Shortcut.CtrlU;
			this.miNewUnitUfo.Text = "Create &ufo unit ...";
			this.miNewUnitUfo.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miNewUnitTftd
			// 
			this.miNewUnitTftd.Index = 6;
			this.miNewUnitTftd.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
			this.miNewUnitTftd.Text = "Create t&ftd unit ...";
			this.miNewUnitTftd.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miOpenScanG
			// 
			this.miOpenScanG.Index = 7;
			this.miOpenScanG.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			this.miOpenScanG.Text = "Open ScanG.&Dat ...";
			this.miOpenScanG.Click += new System.EventHandler(this.OnOpenScanGClick);
			// 
			// miSeparator1
			// 
			this.miSeparator1.Index = 8;
			this.miSeparator1.Text = "-";
			// 
			// miCompare
			// 
			this.miCompare.Index = 9;
			this.miCompare.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.miCompare.Text = "Co&mpare";
			this.miCompare.Visible = false;
			this.miCompare.Click += new System.EventHandler(this.OnCompareClick);
			// 
			// miSave
			// 
			this.miSave.Enabled = false;
			this.miSave.Index = 10;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miSave.Text = "&Save";
			this.miSave.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// miSaveAs
			// 
			this.miSaveAs.Enabled = false;
			this.miSaveAs.Index = 11;
			this.miSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.miSaveAs.Text = "Save &As ...";
			this.miSaveAs.Click += new System.EventHandler(this.OnSaveAsClick);
			// 
			// miExportSprites
			// 
			this.miExportSprites.Enabled = false;
			this.miExportSprites.Index = 12;
			this.miExportSprites.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miExportSprites.Text = "&Export Sprites ...";
			this.miExportSprites.Click += new System.EventHandler(this.OnExportSpritesClick);
			// 
			// miExportSpritesheet
			// 
			this.miExportSpritesheet.Enabled = false;
			this.miExportSpritesheet.Index = 13;
			this.miExportSpritesheet.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
			this.miExportSpritesheet.Text = "Expo&rt Spritesheet ...";
			this.miExportSpritesheet.Click += new System.EventHandler(this.OnExportSpritesheetClick);
			// 
			// miImportSpritesheet
			// 
			this.miImportSpritesheet.Enabled = false;
			this.miImportSpritesheet.Index = 14;
			this.miImportSpritesheet.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.miImportSpritesheet.Text = "Im&port Spritesheet ...";
			this.miImportSpritesheet.Click += new System.EventHandler(this.OnImportSpritesheetClick);
			// 
			// miHq2x
			// 
			this.miHq2x.Index = 15;
			this.miHq2x.Text = "Hq&2x";
			this.miHq2x.Visible = false;
			this.miHq2x.Click += new System.EventHandler(this.OnHq2xClick);
			// 
			// miSeparator2
			// 
			this.miSeparator2.Index = 16;
			this.miSeparator2.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 17;
			this.miQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnQuitClick);
			// 
			// miPaletteMenu
			// 
			this.miPaletteMenu.Enabled = false;
			this.miPaletteMenu.Index = 1;
			this.miPaletteMenu.Text = "&Palette";
			// 
			// miTransparentMenu
			// 
			this.miTransparentMenu.Enabled = false;
			this.miTransparentMenu.Index = 2;
			this.miTransparentMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miTransparent});
			this.miTransparentMenu.Text = "&Transparency";
			// 
			// miTransparent
			// 
			this.miTransparent.Checked = true;
			this.miTransparent.Index = 0;
			this.miTransparent.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
			this.miTransparent.Text = "&On/off";
			this.miTransparent.Click += new System.EventHandler(this.OnTransparencyClick);
			// 
			// miBytesMenu
			// 
			this.miBytesMenu.Enabled = false;
			this.miBytesMenu.Index = 3;
			this.miBytesMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miBytes});
			this.miBytesMenu.Text = "&Bytes";
			// 
			// miBytes
			// 
			this.miBytes.Index = 0;
			this.miBytes.Shortcut = System.Windows.Forms.Shortcut.CtrlB;
			this.miBytes.Text = "Show/hide &byte table";
			this.miBytes.Click += new System.EventHandler(this.OnBytesClick);
			// 
			// miHelpMenu
			// 
			this.miHelpMenu.Index = 4;
			this.miHelpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miAbout,
			this.miConsole,
			this.miHelp});
			this.miHelpMenu.Text = "Help";
			// 
			// miAbout
			// 
			this.miAbout.Index = 0;
			this.miAbout.Text = "&About";
			this.miAbout.Click += new System.EventHandler(this.OnAboutClick);
			// 
			// miConsole
			// 
			this.miConsole.Index = 1;
			this.miConsole.Text = "Console";
			this.miConsole.Visible = false;
			this.miConsole.Click += new System.EventHandler(this.OnConsoleClick);
			// 
			// miHelp
			// 
			this.miHelp.Index = 2;
			this.miHelp.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
			this.miHelp.Text = "&Help";
			this.miHelp.Click += new System.EventHandler(this.OnHelpClick);
			// 
			// ss_Status
			// 
			this.ss_Status.Font = new System.Drawing.Font("Consolas", 7F);
			this.ss_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_TilesTotal,
			this.tssl_TileOver,
			this.tssl_TileSelected,
			this.tssl_SpritesetLabel});
			this.ss_Status.Location = new System.Drawing.Point(0, 592);
			this.ss_Status.Name = "ss_Status";
			this.ss_Status.Size = new System.Drawing.Size(472, 22);
			this.ss_Status.TabIndex = 0;
			this.ss_Status.Text = "ss_Status";
			// 
			// tssl_TilesTotal
			// 
			this.tssl_TilesTotal.AutoSize = false;
			this.tssl_TilesTotal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_TilesTotal.Font = new System.Drawing.Font("Verdana", 7F);
			this.tssl_TilesTotal.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_TilesTotal.Name = "tssl_TilesTotal";
			this.tssl_TilesTotal.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tssl_TilesTotal.Size = new System.Drawing.Size(75, 22);
			this.tssl_TilesTotal.Text = "total";
			this.tssl_TilesTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_TileOver
			// 
			this.tssl_TileOver.AutoSize = false;
			this.tssl_TileOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_TileOver.Font = new System.Drawing.Font("Verdana", 7F);
			this.tssl_TileOver.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_TileOver.Name = "tssl_TileOver";
			this.tssl_TileOver.Size = new System.Drawing.Size(80, 22);
			this.tssl_TileOver.Text = "over";
			this.tssl_TileOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_TileSelected
			// 
			this.tssl_TileSelected.AutoSize = false;
			this.tssl_TileSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_TileSelected.Font = new System.Drawing.Font("Verdana", 7F);
			this.tssl_TileSelected.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_TileSelected.Name = "tssl_TileSelected";
			this.tssl_TileSelected.Size = new System.Drawing.Size(100, 22);
			this.tssl_TileSelected.Text = "select";
			this.tssl_TileSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_SpritesetLabel
			// 
			this.tssl_SpritesetLabel.AutoSize = false;
			this.tssl_SpritesetLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_SpritesetLabel.Font = new System.Drawing.Font("Verdana", 7F);
			this.tssl_SpritesetLabel.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_SpritesetLabel.Name = "tssl_SpritesetLabel";
			this.tssl_SpritesetLabel.Size = new System.Drawing.Size(202, 22);
			this.tssl_SpritesetLabel.Spring = true;
			this.tssl_SpritesetLabel.Text = "label";
			// 
			// PckViewForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(472, 614);
			this.Controls.Add(this.ss_Status);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(50, 50);
			this.MaximumSize = new System.Drawing.Size(480, 640);
			this.Menu = this.mmMainMenu;
			this.Name = "PckViewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "PckView";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnPckViewFormClosing);
			this.Shown += new System.EventHandler(this.OnShown);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.ss_Status.ResumeLayout(false);
			this.ss_Status.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.ComponentModel.IContainer components = null;


		private System.Windows.Forms.MainMenu mmMainMenu;

		private System.Windows.Forms.MenuItem miFileMenu;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.MenuItem miOpenBigobs;
		private System.Windows.Forms.MenuItem miCreate;
		private System.Windows.Forms.MenuItem miOpenScanG;
		private System.Windows.Forms.MenuItem miNewTerrain;
		private System.Windows.Forms.MenuItem miNewBigobs;
		private System.Windows.Forms.MenuItem miNewUnitUfo;
		private System.Windows.Forms.MenuItem miNewUnitTftd;
		private System.Windows.Forms.MenuItem miSeparator1;
		private System.Windows.Forms.MenuItem miExportSprites;
		private System.Windows.Forms.MenuItem miExportSpritesheet;
		private System.Windows.Forms.MenuItem miImportSpritesheet;
		private System.Windows.Forms.MenuItem miSaveAs;
		private System.Windows.Forms.MenuItem miHq2x;
		private System.Windows.Forms.MenuItem miSeparator2;
		private System.Windows.Forms.MenuItem miQuit;

		private System.Windows.Forms.MenuItem miBytesMenu;
		private System.Windows.Forms.MenuItem miBytes;

		private System.Windows.Forms.MenuItem miPaletteMenu;

		private System.Windows.Forms.MenuItem miTransparentMenu;
		private System.Windows.Forms.MenuItem miTransparent;

		private System.Windows.Forms.MenuItem miHelpMenu;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miAbout;


		private System.Windows.Forms.MenuItem miConsole;

		private System.Windows.Forms.MenuItem miCompare;

		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.StatusStrip ss_Status;
		private System.Windows.Forms.ToolStripStatusLabel tssl_TilesTotal;
		private System.Windows.Forms.ToolStripStatusLabel tssl_TileOver;
		private System.Windows.Forms.ToolStripStatusLabel tssl_TileSelected;
		private System.Windows.Forms.ToolStripStatusLabel tssl_SpritesetLabel;
	}
}
