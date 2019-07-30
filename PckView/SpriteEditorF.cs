using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using XCom.Interfaces;


namespace PckView
{
	internal sealed class SpriteEditorF
		:
			Form
	{
		#region Enums
		internal enum EditMode
		{
			Locked,
			Enabled
		}
		#endregion Enums


		#region Fields
		internal readonly PckViewForm _f;
		internal readonly PaletteForm _fpalette;

		private readonly TrackBar _trackBar = new TrackBar();
		private readonly Label _lblEditMode = new Label();
		#endregion Fields


		#region Properties (static)
		internal static EditMode Mode
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		internal SpritePanel SpritePanel
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal SpriteEditorF(PckViewForm f)
		{
			_f = f;

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


			SpritePanel = new SpritePanel(this);
			SpritePanel.Top = _trackBar.Height + _lblEditMode.Height;


			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			MaximumSize = new Size(0,0); // fu.net

			Controls.Add(SpritePanel);
			Controls.Add(_trackBar);
			Controls.Add(_lblEditMode);

			OnTrackScroll(null, EventArgs.Empty);

			_fpalette = new PaletteForm(this);
			_fpalette.FormClosing += OnPaletteFormClosing;

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let OnLoad() deter width and height.
				Left = _f.Left + 20;
				Top  = _f.Top  + 20;
			}
		}
		#endregion cTor


		#region Events (override)
		protected override void OnResize(EventArgs e)
		{
//			base.OnResize(e);

			_trackBar   .Width  =
			_lblEditMode.Width  =
			SpritePanel .Width  = ClientSize.Width;
			SpritePanel .Height = ClientSize.Height
								- _trackBar.Height
								- _lblEditMode.Height;
		}

		/// <summary>
		/// @note Requires KeyPreview TRUE.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.SuppressKeyPress = true;
				Close();
			}
			base.OnKeyDown(e);
		}

		/// <summary>
		/// Handles form closing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!PckViewForm.Quit)
			{
				e.Cancel = true;
				Hide();
			}
			else
				RegistryInfo.UpdateRegistry(this);

			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Sets the *proper* ClientSize.
		/// @note Also called by PckViewForm.SpritesetChanged()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnLoad(object sender, EventArgs e)
		{
			ClientSize = new Size(
								XCImage.SpriteWidth32 * 10 + SpritePanel.Pad, // <- keep the statusbar at 32px width
								XCImage.SpriteHeight  * 10 + SpritePanel.Pad
									+ _trackBar   .Height
									+ _lblEditMode.Height
									+ ss_Status   .Height);
		}

		private void OnTrackScroll(object sender, EventArgs e)
		{
			SpritePanel.ScaleFactor = _trackBar.Value;
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
				_fpalette.Show();
			}
			else
				_fpalette.BringToFront();
		}

		/// <summary>
		/// @note This fires after the palette's FormClosing event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaletteFormClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;
		}

		private void OnShowGridClick(object sender, EventArgs e)
		{
			SpritePanel.Grid = (miGrid.Checked = !miGrid.Checked);
		}

		private void OnInvertGridColorClick(object sender, EventArgs e)
		{
			SpritePanel.InvertGridColor(miGridInvert.Checked = !miGridInvert.Checked);
		}
		#endregion Events


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
		#endregion Methods


		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		private IContainer components;

		private MainMenu mmMainMenu;
		private MenuItem miPaletteMenu;
		private MenuItem miPalette;
		private MenuItem miGridMenu;
		private MenuItem miGrid;
		private MenuItem miGridInvert;
		private StatusStrip ss_Status;
		private ToolStripStatusLabel tssl_ColorInfo;


		#region Windows Form Designer generated code
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
			this.tssl_ColorInfo.Size = new System.Drawing.Size(274, 22);
			this.tssl_ColorInfo.Spring = true;
			this.tssl_ColorInfo.Text = "colorinfo";
			this.tssl_ColorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SpriteEditorF
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(294, 276);
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
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Windows Form Designer generated code
	}
}
