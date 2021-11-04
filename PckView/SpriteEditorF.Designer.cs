using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace PckView
{
	internal sealed partial class SpriteEditorF
	{
		#region Designer
		private IContainer components;

		private MainMenu mmMainMenu;

		private MenuItem miPaletteMenu;
		private MenuItem miPalette;

		private MenuItem miGridMenu;
		private MenuItem miGridDark;
		private MenuItem miGridLight;

		private MenuItem miMagMenu;
		private MenuItem miMagReset;

		private StatusStrip ss_Status;
		private ToolStripStatusLabel tssl_ColorInfo;

		private TrackBar bar_Scale;
		private Label la_EditMode;

		private ToolStripDropDownButton tsddb_Size;
		private ToolStripMenuItem tsddb_Size_0;
		private ToolStripMenuItem tsddb_Size_1;
		private ToolStripMenuItem tsddb_Size_2;
		private ToolStripMenuItem tsddb_Size_3;
		private ToolStripMenuItem tsddb_Size_4;
		private ToolStripMenuItem tsddb_Size_5;
		private ToolStripMenuItem tsddb_Size_6;
		private ToolStripMenuItem tsddb_Size_7;
		private ToolStripMenuItem tsddb_Size_8;
		private ToolStripMenuItem tsddb_Size_9;
		private ToolStripMenuItem tsddb_Size_10;


		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
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
			this.mmMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miPaletteMenu = new System.Windows.Forms.MenuItem();
			this.miPalette = new System.Windows.Forms.MenuItem();
			this.miGridMenu = new System.Windows.Forms.MenuItem();
			this.miGridDark = new System.Windows.Forms.MenuItem();
			this.miGridLight = new System.Windows.Forms.MenuItem();
			this.miMagMenu = new System.Windows.Forms.MenuItem();
			this.miMagReset = new System.Windows.Forms.MenuItem();
			this.ss_Status = new System.Windows.Forms.StatusStrip();
			this.tssl_ColorInfo = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsddb_Size = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsddb_Size_0 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.tsddb_Size_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.bar_Scale = new System.Windows.Forms.TrackBar();
			this.la_EditMode = new System.Windows.Forms.Label();
			this.ss_Status.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bar_Scale)).BeginInit();
			this.SuspendLayout();
			// 
			// mmMainMenu
			// 
			this.mmMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPaletteMenu,
			this.miGridMenu,
			this.miMagMenu});
			// 
			// miPaletteMenu
			// 
			this.miPaletteMenu.Index = 0;
			this.miPaletteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPalette});
			this.miPaletteMenu.Text = "&Palette";
			this.miPaletteMenu.Popup += new System.EventHandler(this.popup_Palette);
			// 
			// miPalette
			// 
			this.miPalette.Index = 0;
			this.miPalette.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.miPalette.Text = "Show &palette";
			this.miPalette.Click += new System.EventHandler(this.OnShowPaletteClick);
			// 
			// miGridMenu
			// 
			this.miGridMenu.Index = 1;
			this.miGridMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miGridDark,
			this.miGridLight});
			this.miGridMenu.Text = "&Grid";
			// 
			// miGridDark
			// 
			this.miGridDark.Index = 0;
			this.miGridDark.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			this.miGridDark.Text = "grid &dark";
			this.miGridDark.Click += new System.EventHandler(this.OnGridDarkClick);
			// 
			// miGridLight
			// 
			this.miGridLight.Index = 1;
			this.miGridLight.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.miGridLight.Text = "grid &light";
			this.miGridLight.Click += new System.EventHandler(this.OnGridLightClick);
			// 
			// miMagMenu
			// 
			this.miMagMenu.Index = 2;
			this.miMagMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miMagReset});
			this.miMagMenu.Text = "&Magnification";
			this.miMagMenu.Popup += new System.EventHandler(this.popup_MagMenu);
			// 
			// miMagReset
			// 
			this.miMagReset.Index = 0;
			this.miMagReset.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
			this.miMagReset.Text = "&Reset";
			this.miMagReset.Click += new System.EventHandler(this.OnResetMagnification);
			// 
			// ss_Status
			// 
			this.ss_Status.Font = new System.Drawing.Font("Verdana", 7F);
			this.ss_Status.GripMargin = new System.Windows.Forms.Padding(0);
			this.ss_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_ColorInfo,
			this.tsddb_Size});
			this.ss_Status.Location = new System.Drawing.Point(0, 254);
			this.ss_Status.Name = "ss_Status";
			this.ss_Status.Size = new System.Drawing.Size(294, 22);
			this.ss_Status.SizingGrip = false;
			this.ss_Status.TabIndex = 2;
			// 
			// tssl_ColorInfo
			// 
			this.tssl_ColorInfo.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tssl_ColorInfo.Name = "tssl_ColorInfo";
			this.tssl_ColorInfo.Size = new System.Drawing.Size(241, 22);
			this.tssl_ColorInfo.Spring = true;
			this.tssl_ColorInfo.Text = "colorinfo";
			this.tssl_ColorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsddb_Size
			// 
			this.tsddb_Size.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddb_Size_0,
			this.tsddb_Size_1,
			this.tsddb_Size_2,
			this.tsddb_Size_3,
			this.tsddb_Size_4,
			this.tsddb_Size_5,
			this.tsddb_Size_6,
			this.tsddb_Size_7,
			this.tsddb_Size_8,
			this.tsddb_Size_9,
			this.tsddb_Size_10});
			this.tsddb_Size.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_Size.Margin = new System.Windows.Forms.Padding(0);
			this.tsddb_Size.Name = "tsddb_Size";
			this.tsddb_Size.Size = new System.Drawing.Size(33, 22);
			this.tsddb_Size.Text = "+0";
			// 
			// tsddb_Size_0
			// 
			this.tsddb_Size_0.Checked = true;
			this.tsddb_Size_0.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsddb_Size_0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_0.Name = "tsddb_Size_0";
			this.tsddb_Size_0.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_0.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_0.Text = "+0";
			this.tsddb_Size_0.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_1
			// 
			this.tsddb_Size_1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_1.Name = "tsddb_Size_1";
			this.tsddb_Size_1.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_1.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_1.Text = "+1";
			this.tsddb_Size_1.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_2
			// 
			this.tsddb_Size_2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_2.Name = "tsddb_Size_2";
			this.tsddb_Size_2.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_2.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_2.Text = "+2";
			this.tsddb_Size_2.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_3
			// 
			this.tsddb_Size_3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_3.Name = "tsddb_Size_3";
			this.tsddb_Size_3.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_3.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_3.Text = "+3";
			this.tsddb_Size_3.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_4
			// 
			this.tsddb_Size_4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_4.Name = "tsddb_Size_4";
			this.tsddb_Size_4.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_4.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_4.Text = "+4";
			this.tsddb_Size_4.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_5
			// 
			this.tsddb_Size_5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_5.Name = "tsddb_Size_5";
			this.tsddb_Size_5.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_5.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_5.Text = "+5";
			this.tsddb_Size_5.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_6
			// 
			this.tsddb_Size_6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_6.Name = "tsddb_Size_6";
			this.tsddb_Size_6.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_6.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_6.Text = "+6";
			this.tsddb_Size_6.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_7
			// 
			this.tsddb_Size_7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_7.Name = "tsddb_Size_7";
			this.tsddb_Size_7.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_7.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_7.Text = "+7";
			this.tsddb_Size_7.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_8
			// 
			this.tsddb_Size_8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_8.Name = "tsddb_Size_8";
			this.tsddb_Size_8.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_8.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_8.Text = "+8";
			this.tsddb_Size_8.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_9
			// 
			this.tsddb_Size_9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_9.Name = "tsddb_Size_9";
			this.tsddb_Size_9.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_9.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_9.Text = "+9";
			this.tsddb_Size_9.Click += new System.EventHandler(this.size_click);
			// 
			// tsddb_Size_10
			// 
			this.tsddb_Size_10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_Size_10.Name = "tsddb_Size_10";
			this.tsddb_Size_10.Padding = new System.Windows.Forms.Padding(0);
			this.tsddb_Size_10.Size = new System.Drawing.Size(92, 20);
			this.tsddb_Size_10.Text = "+10";
			this.tsddb_Size_10.Click += new System.EventHandler(this.size_click);
			// 
			// bar_Scale
			// 
			this.bar_Scale.AutoSize = false;
			this.bar_Scale.BackColor = System.Drawing.Color.Gainsboro;
			this.bar_Scale.Dock = System.Windows.Forms.DockStyle.Top;
			this.bar_Scale.LargeChange = 1;
			this.bar_Scale.Location = new System.Drawing.Point(0, 0);
			this.bar_Scale.Margin = new System.Windows.Forms.Padding(0);
			this.bar_Scale.Minimum = 1;
			this.bar_Scale.Name = "bar_Scale";
			this.bar_Scale.Size = new System.Drawing.Size(294, 23);
			this.bar_Scale.TabIndex = 0;
			this.bar_Scale.TickStyle = System.Windows.Forms.TickStyle.None;
			this.bar_Scale.Value = 10;
			this.bar_Scale.Scroll += new System.EventHandler(this.trackbar_OnScroll);
			// 
			// la_EditMode
			// 
			this.la_EditMode.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_EditMode.Location = new System.Drawing.Point(0, 23);
			this.la_EditMode.Margin = new System.Windows.Forms.Padding(0);
			this.la_EditMode.Name = "la_EditMode";
			this.la_EditMode.Size = new System.Drawing.Size(294, 15);
			this.la_EditMode.TabIndex = 1;
			this.la_EditMode.Text = "Locked";
			this.la_EditMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.la_EditMode.Click += new System.EventHandler(this.OnEditModeMouseClick);
			// 
			// SpriteEditorF
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(294, 276);
			this.Controls.Add(this.la_EditMode);
			this.Controls.Add(this.bar_Scale);
			this.Controls.Add(this.ss_Status);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(300, 300);
			this.Menu = this.mmMainMenu;
			this.MinimizeBox = false;
			this.Name = "SpriteEditorF";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Sprite Editor";
			this.Load += new System.EventHandler(this.OnLoad);
			this.ss_Status.ResumeLayout(false);
			this.ss_Status.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bar_Scale)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
