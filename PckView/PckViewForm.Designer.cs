namespace PckView
{
	partial class PckViewForm
	{
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PckViewForm));
			this.mmMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miFileMenu = new System.Windows.Forms.MenuItem();
			this.miCreate = new System.Windows.Forms.MenuItem();
			this.miCreateTerrain = new System.Windows.Forms.MenuItem();
			this.miCreateBigobs = new System.Windows.Forms.MenuItem();
			this.miCreateUnitUfo = new System.Windows.Forms.MenuItem();
			this.miCreateUnitTftd = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miOpenBigobs = new System.Windows.Forms.MenuItem();
			this.miOpenScanG = new System.Windows.Forms.MenuItem();
			this.miSeparator1 = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSaveAs = new System.Windows.Forms.MenuItem();
			this.miSeparator2 = new System.Windows.Forms.MenuItem();
			this.miExportSprites = new System.Windows.Forms.MenuItem();
			this.miExportSpritesheet = new System.Windows.Forms.MenuItem();
			this.miImportSpritesheet = new System.Windows.Forms.MenuItem();
			this.miSeparator3 = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.miCompare = new System.Windows.Forms.MenuItem();
			this.miHq2x = new System.Windows.Forms.MenuItem();
			this.miPaletteMenu = new System.Windows.Forms.MenuItem();
			this.miTransparentMenu = new System.Windows.Forms.MenuItem();
			this.miTransparent = new System.Windows.Forms.MenuItem();
			this.miBytesMenu = new System.Windows.Forms.MenuItem();
			this.miBytes = new System.Windows.Forms.MenuItem();
			this.miHelpMenu = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miAbout = new System.Windows.Forms.MenuItem();
			this.miConsole = new System.Windows.Forms.MenuItem();
			this.ss_Status = new System.Windows.Forms.StatusStrip();
			this.tssl_TilesTotal = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_TileOver = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_TileSelected = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_SpritesetLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_Offset = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_OffsetLast = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_OffsetAftr = new System.Windows.Forms.ToolStripStatusLabel();
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
			this.miCreate,
			this.miCreateTerrain,
			this.miCreateBigobs,
			this.miCreateUnitUfo,
			this.miCreateUnitTftd,
			this.miOpen,
			this.miOpenBigobs,
			this.miOpenScanG,
			this.miSeparator1,
			this.miSave,
			this.miSaveAs,
			this.miSeparator2,
			this.miExportSprites,
			this.miExportSpritesheet,
			this.miImportSpritesheet,
			this.miSeparator3,
			this.miQuit,
			this.miCompare,
			this.miHq2x});
			this.miFileMenu.Text = "&File";
			// 
			// miCreate
			// 
			this.miCreate.Index = 0;
			this.miCreate.Text = "Create Pck file";
			// 
			// miCreateTerrain
			// 
			this.miCreateTerrain.Index = 1;
			this.miCreateTerrain.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.miCreateTerrain.Text = "&Create terrain ...";
			this.miCreateTerrain.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miCreateBigobs
			// 
			this.miCreateBigobs.Index = 2;
			this.miCreateBigobs.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miCreateBigobs.Text = "Create b&igobs ...";
			this.miCreateBigobs.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miCreateUnitUfo
			// 
			this.miCreateUnitUfo.Index = 3;
			this.miCreateUnitUfo.Shortcut = System.Windows.Forms.Shortcut.CtrlU;
			this.miCreateUnitUfo.Text = "Create &ufo unit ...";
			this.miCreateUnitUfo.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miCreateUnitTftd
			// 
			this.miCreateUnitTftd.Index = 4;
			this.miCreateUnitTftd.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
			this.miCreateUnitTftd.Text = "Create t&ftd unit ...";
			this.miCreateUnitTftd.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// miOpen
			// 
			this.miOpen.Index = 5;
			this.miOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miOpen.Text = "&Open Pck (terrain/unit) file ...";
			this.miOpen.Click += new System.EventHandler(this.OnOpenClick);
			// 
			// miOpenBigobs
			// 
			this.miOpenBigobs.Index = 6;
			this.miOpenBigobs.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.miOpenBigobs.Text = "Open Pck (bi&gobs) file ...";
			this.miOpenBigobs.Click += new System.EventHandler(this.OnOpenBigobsClick);
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
			// miSave
			// 
			this.miSave.Enabled = false;
			this.miSave.Index = 9;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miSave.Text = "&Save";
			this.miSave.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// miSaveAs
			// 
			this.miSaveAs.Enabled = false;
			this.miSaveAs.Index = 10;
			this.miSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.miSaveAs.Text = "Save &As ...";
			this.miSaveAs.Click += new System.EventHandler(this.OnSaveAsClick);
			// 
			// miSeparator2
			// 
			this.miSeparator2.Index = 11;
			this.miSeparator2.Text = "-";
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
			// miSeparator3
			// 
			this.miSeparator3.Index = 15;
			this.miSeparator3.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 16;
			this.miQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnQuitClick);
			// 
			// miCompare
			// 
			this.miCompare.Index = 17;
			this.miCompare.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.miCompare.Text = "Co&mpare";
			this.miCompare.Visible = false;
			this.miCompare.Click += new System.EventHandler(this.OnCompareClick);
			// 
			// miHq2x
			// 
			this.miHq2x.Index = 18;
			this.miHq2x.Text = "Hq&2x";
			this.miHq2x.Visible = false;
			this.miHq2x.Click += new System.EventHandler(this.OnHq2xClick);
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
			this.miHelp,
			this.miAbout,
			this.miConsole});
			this.miHelpMenu.Text = "&Help";
			// 
			// miHelp
			// 
			this.miHelp.Index = 0;
			this.miHelp.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
			this.miHelp.Text = "&Help";
			this.miHelp.Click += new System.EventHandler(this.OnHelpClick);
			// 
			// miAbout
			// 
			this.miAbout.Index = 1;
			this.miAbout.Text = "&About";
			this.miAbout.Click += new System.EventHandler(this.OnAboutClick);
			// 
			// miConsole
			// 
			this.miConsole.Index = 2;
			this.miConsole.Text = "Console";
			this.miConsole.Visible = false;
			this.miConsole.Click += new System.EventHandler(this.OnConsoleClick);
			// 
			// ss_Status
			// 
			this.ss_Status.Font = new System.Drawing.Font("Verdana", 7F);
			this.ss_Status.GripMargin = new System.Windows.Forms.Padding(0);
			this.ss_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_TilesTotal,
			this.tssl_TileOver,
			this.tssl_TileSelected,
			this.tssl_SpritesetLabel,
			this.tssl_Offset,
			this.tssl_OffsetLast,
			this.tssl_OffsetAftr});
			this.ss_Status.Location = new System.Drawing.Point(0, 432);
			this.ss_Status.Name = "ss_Status";
			this.ss_Status.Size = new System.Drawing.Size(632, 22);
			this.ss_Status.TabIndex = 0;
			this.ss_Status.Text = "ss_Status";
			// 
			// tssl_TilesTotal
			// 
			this.tssl_TilesTotal.AutoSize = false;
			this.tssl_TilesTotal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_TilesTotal.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_TilesTotal.Name = "tssl_TilesTotal";
			this.tssl_TilesTotal.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tssl_TilesTotal.Size = new System.Drawing.Size(65, 22);
			this.tssl_TilesTotal.Text = "total";
			this.tssl_TilesTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_TileOver
			// 
			this.tssl_TileOver.AutoSize = false;
			this.tssl_TileOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_TileOver.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_TileOver.Name = "tssl_TileOver";
			this.tssl_TileOver.Size = new System.Drawing.Size(65, 22);
			this.tssl_TileOver.Text = "over";
			this.tssl_TileOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_TileSelected
			// 
			this.tssl_TileSelected.AutoSize = false;
			this.tssl_TileSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_TileSelected.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_TileSelected.Name = "tssl_TileSelected";
			this.tssl_TileSelected.Size = new System.Drawing.Size(75, 22);
			this.tssl_TileSelected.Text = "select";
			this.tssl_TileSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_SpritesetLabel
			// 
			this.tssl_SpritesetLabel.AutoSize = false;
			this.tssl_SpritesetLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_SpritesetLabel.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_SpritesetLabel.Name = "tssl_SpritesetLabel";
			this.tssl_SpritesetLabel.Size = new System.Drawing.Size(287, 22);
			this.tssl_SpritesetLabel.Spring = true;
			this.tssl_SpritesetLabel.Text = "label";
			// 
			// tssl_Offset
			// 
			this.tssl_Offset.AutoSize = false;
			this.tssl_Offset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_Offset.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_Offset.Name = "tssl_Offset";
			this.tssl_Offset.Size = new System.Drawing.Size(45, 22);
			this.tssl_Offset.Text = "Offsets";
			this.tssl_Offset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_OffsetLast
			// 
			this.tssl_OffsetLast.AutoSize = false;
			this.tssl_OffsetLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_OffsetLast.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_OffsetLast.Name = "tssl_OffsetLast";
			this.tssl_OffsetLast.Size = new System.Drawing.Size(40, 22);
			this.tssl_OffsetLast.Text = "last";
			this.tssl_OffsetLast.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_OffsetAftr
			// 
			this.tssl_OffsetAftr.AutoSize = false;
			this.tssl_OffsetAftr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tssl_OffsetAftr.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_OffsetAftr.Name = "tssl_OffsetAftr";
			this.tssl_OffsetAftr.Size = new System.Drawing.Size(40, 22);
			this.tssl_OffsetAftr.Text = "after";
			this.tssl_OffsetAftr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PckViewForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.ss_Status);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(50, 50);
			this.MaximumSize = new System.Drawing.Size(640, 480);
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

		private System.Windows.Forms.MenuItem miCreate;
		private System.Windows.Forms.MenuItem miCreateTerrain;
		private System.Windows.Forms.MenuItem miCreateBigobs;
		private System.Windows.Forms.MenuItem miCreateUnitUfo;
		private System.Windows.Forms.MenuItem miCreateUnitTftd;
		private System.Windows.Forms.MenuItem miFileMenu;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.MenuItem miOpenBigobs;
		private System.Windows.Forms.MenuItem miOpenScanG;
		private System.Windows.Forms.MenuItem miSeparator1;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem miSaveAs;
		private System.Windows.Forms.MenuItem miSeparator2;
		private System.Windows.Forms.MenuItem miExportSprites;
		private System.Windows.Forms.MenuItem miExportSpritesheet;
		private System.Windows.Forms.MenuItem miImportSpritesheet;
		private System.Windows.Forms.MenuItem miSeparator3;
		private System.Windows.Forms.MenuItem miQuit;
		private System.Windows.Forms.MenuItem miCompare;
		private System.Windows.Forms.MenuItem miHq2x;

		private System.Windows.Forms.MenuItem miBytesMenu;
		private System.Windows.Forms.MenuItem miBytes;

		private System.Windows.Forms.MenuItem miPaletteMenu;

		private System.Windows.Forms.MenuItem miTransparentMenu;
		private System.Windows.Forms.MenuItem miTransparent;

		private System.Windows.Forms.MenuItem miHelpMenu;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miAbout;
		private System.Windows.Forms.MenuItem miConsole;

		private System.Windows.Forms.StatusStrip ss_Status;
		private System.Windows.Forms.ToolStripStatusLabel tssl_TilesTotal;
		private System.Windows.Forms.ToolStripStatusLabel tssl_TileOver;
		private System.Windows.Forms.ToolStripStatusLabel tssl_TileSelected;
		private System.Windows.Forms.ToolStripStatusLabel tssl_SpritesetLabel;
		private System.Windows.Forms.ToolStripStatusLabel tssl_Offset;
		private System.Windows.Forms.ToolStripStatusLabel tssl_OffsetLast;
		private System.Windows.Forms.ToolStripStatusLabel tssl_OffsetAftr;
	}
}
