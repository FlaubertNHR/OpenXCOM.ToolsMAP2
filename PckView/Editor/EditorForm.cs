using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Windows;

using XCom.Interfaces;


namespace PckView
{
	internal sealed class EditorForm
		:
			Form
	{
		#region Enums
		internal enum EditMode
		{
			Locked,
			Enabled
		}
		#endregion


		#region Fields
		private readonly EditorPanel _pnlEditor;

		private readonly PaletteForm _fpalette = new PaletteForm();

		private readonly TrackBar _trackBar = new TrackBar();
		private readonly Label _lblEditMode = new Label();
		#endregion


		#region Properties (static)
		internal static EditMode Mode
		{ get; set; }
		#endregion


		#region Properties
		private bool Inited
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal EditorForm()
		{
			_trackBar.AutoSize    = false;
			_trackBar.Height      = 23;
			_trackBar.Minimum     =  1;
			_trackBar.Maximum     = 10;
			_trackBar.Value       = 10;
			_trackBar.LargeChange =  1;
			_trackBar.BackColor   = Color.Silver;

			_trackBar.Scroll += OnTrackScroll;

			_lblEditMode.Text      = "Locked";
			_lblEditMode.TextAlign = ContentAlignment.MiddleCenter;
			_lblEditMode.Height    = 15;
			_lblEditMode.Top       = _trackBar.Height;

			_lblEditMode.MouseClick += OnEditModeMouseClick;

			Mode = EditMode.Locked;


			_pnlEditor = new EditorPanel(this);
			_pnlEditor.Top = _trackBar.Height + _lblEditMode.Height;


			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			MaximumSize = new Size(0, 0); // fu.net

			Controls.Add(_pnlEditor);
			Controls.Add(_trackBar);
			Controls.Add(_lblEditMode);

			_fpalette.FormClosing += OnPaletteFormClosing;

			OnTrackScroll(null, EventArgs.Empty);


			var regInfo = new RegistryInfo(RegistryInfo.SpriteEditor, this); // subscribe to Load and Closing events.
			regInfo.RegisterProperties();
		}
		#endregion


		#region Eventcalls (override)
		protected override void OnResize(EventArgs e)
		{
//			base.OnResize(e);

			_trackBar   .Width  =
			_lblEditMode.Width  =
			_pnlEditor  .Width  = ClientSize.Width;
			_pnlEditor  .Height = ClientSize.Height
								- _trackBar.Height
								- _lblEditMode.Height;
		}
		#endregion


		#region Eventcalls
		/// <summary>
		/// Sets the *proper* ClientSize.
		/// @note Also called by PckViewForm.SpritesetChanged()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnLoad(object sender, EventArgs e)
		{
			ClientSize = new Size(
								XCImage.SpriteWidth32 * 10 + EditorPanel.Pad, // <- keep the statusbar at 32px width
								XCImage.SpriteHeight  * 10 + EditorPanel.Pad
									+ _trackBar   .Height
									+ _lblEditMode.Height
									+ ss_Status   .Height);
		}

		private void OnTrackScroll(object sender, EventArgs e)
		{
			_pnlEditor.ScaleFactor = _trackBar.Value;
		}

		private void OnEditModeMouseClick(object sender, EventArgs e)
		{
			switch (Mode)
			{
				case EditMode.Locked:
					Mode = EditMode.Enabled;
					_lblEditMode.Text = "Enabled";
					_lblEditMode.BackColor = Color.AliceBlue;
					break;

				case EditMode.Enabled:
					Mode = EditMode.Locked;
					_lblEditMode.Text = "Locked";
					_lblEditMode.BackColor = Control.DefaultBackColor;
					break;
			}
		}

		private void OnShowPaletteClick(object sender, EventArgs e)
		{
			if (!miPalette.Checked)
			{
				miPalette.Checked = true;

				if (!Inited)
				{
					Inited = true;
					_fpalette.Left = Left + 20;
					_fpalette.Top  = Top  + 20;
				}
				_fpalette.Show();
			}
			else
				_fpalette.Close(); // hide, not close -> see OnPaletteFormClosing()
		}

		private void OnPaletteFormClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;

			e.Cancel = true;
			_fpalette.Hide();
		}

		private void OnShowGridClick(object sender, EventArgs e)
		{
			_pnlEditor.Grid = (miGrid.Checked = !miGrid.Checked);
		}

		private void OnInvertGridColorClick(object sender, EventArgs e)
		{
			_pnlEditor.InvertGridColor(miGridInvert.Checked = !miGridInvert.Checked);
		}
		#endregion


		#region Methods
		internal void PrintColorInfo(string info)
		{
			tssl_ColorInfo.Text = info;
		}

		internal void ClearColorInfo()
		{
			tssl_ColorInfo.Text = String.Empty;
		}

		/// <summary>
		/// Closes the palette-form when PckView closes. This is required only
		/// when PckView opens via MapView.
		/// </summary>
		internal void ClosePalette()
		{
			_fpalette.Close();
		}
		#endregion


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.miPaletteMenu = new System.Windows.Forms.MenuItem();
			this.miPalette = new System.Windows.Forms.MenuItem();
			this.miGridMenu = new System.Windows.Forms.MenuItem();
			this.miGrid = new System.Windows.Forms.MenuItem();
			this.miGridInvert = new System.Windows.Forms.MenuItem();
			this.ss_Status = new System.Windows.Forms.StatusStrip();
			this.tssl_ColorInfo = new System.Windows.Forms.ToolStripStatusLabel();
			this.ss_Status.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMainMenu
			// 
			this.mmMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPaletteMenu,
			this.miGridMenu});
			// 
			// miPaletteMenu
			// 
			this.miPaletteMenu.Index = 0;
			this.miPaletteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPalette});
			this.miPaletteMenu.Text = "Palette";
			// 
			// miPalette
			// 
			this.miPalette.Index = 0;
			this.miPalette.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.miPalette.Text = "Show/hide palette";
			this.miPalette.Click += new System.EventHandler(this.OnShowPaletteClick);
			// 
			// miGridMenu
			// 
			this.miGridMenu.Index = 1;
			this.miGridMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miGrid,
			this.miGridInvert});
			this.miGridMenu.Text = "Grid";
			// 
			// miGrid
			// 
			this.miGrid.Index = 0;
			this.miGrid.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.miGrid.Text = "Show/hide grid";
			this.miGrid.Click += new System.EventHandler(this.OnShowGridClick);
			// 
			// miGridInvert
			// 
			this.miGridInvert.Index = 1;
			this.miGridInvert.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miGridInvert.Text = "Invert color";
			this.miGridInvert.Click += new System.EventHandler(this.OnInvertGridColorClick);
			// 
			// ss_Status
			// 
			this.ss_Status.Font = new System.Drawing.Font("Verdana", 7F);
			this.ss_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_ColorInfo});
			this.ss_Status.Location = new System.Drawing.Point(0, 254);
			this.ss_Status.Name = "ss_Status";
			this.ss_Status.Size = new System.Drawing.Size(294, 22);
			this.ss_Status.SizingGrip = false;
			this.ss_Status.TabIndex = 0;
			// 
			// tssl_ColorInfo
			// 
			this.tssl_ColorInfo.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tssl_ColorInfo.Name = "tssl_ColorInfo";
			this.tssl_ColorInfo.Size = new System.Drawing.Size(243, 22);
			this.tssl_ColorInfo.Spring = true;
			this.tssl_ColorInfo.Text = "colorinfo";
			this.tssl_ColorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// EditorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(294, 276);
			this.Controls.Add(this.ss_Status);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(300, 300);
			this.Menu = this.mmMainMenu;
			this.MinimizeBox = false;
			this.Name = "EditorForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Sprite Editor";
			this.Load += new System.EventHandler(this.OnLoad);
			this.ss_Status.ResumeLayout(false);
			this.ss_Status.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private IContainer components;

		private MainMenu mmMainMenu;
		private MenuItem miPaletteMenu;
		private MenuItem miPalette;
		private MenuItem miGridMenu;
		private MenuItem miGrid;
		private MenuItem miGridInvert;
		private StatusStrip ss_Status;
		private ToolStripStatusLabel tssl_ColorInfo;
	}
}
