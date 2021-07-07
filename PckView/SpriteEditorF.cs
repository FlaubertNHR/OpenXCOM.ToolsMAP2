using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using XCom;


namespace PckView
{
	internal sealed class SpriteEditorF
		:
			Form
	{
		#region Fields (static)
		internal static bool BypassActivatedEvent;
		#endregion Fields (static)


		#region Fields
		internal readonly PckViewF _f;
		internal readonly PaletteF _fpalette;
		#endregion Fields


		#region Properties (static)
		internal static EditMode Mode
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
		internal SpritePanel SpritePanel
		{ get; private set; }
		#endregion Properties


		#region Properties (override)
		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}
		#endregion Properties (override)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal SpriteEditorF(PckViewF f)
		{
			InitializeComponent();

			bar_Scale.MouseWheel += trackbar_OnMouseWheel;

			// WORKAROUND: See note in MainViewF cTor.
			MaximumSize = new Size(0,0); // fu.net

			_f = f;

			SpritePanel = new SpritePanel(this);
			Controls.Add(SpritePanel);
			SpritePanel.BringToFront();

			Mode = EditMode.Locked;

			_fpalette = new PaletteF(this);
			_fpalette.FormClosing += OnPaletteFormClosing;

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let OnLoad() deter width and height.
				Left = _f.Left + 20;
				Top  = _f.Top  + 20;
			}

			ss_Status.Renderer = new CustomToolStripRenderer();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Brings PaletteViewer to top when this is activated.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			if (!BypassActivatedEvent)
			{
				BypassActivatedEvent = true;

				if (_fpalette.Visible)
				{
					_fpalette.TopMost = true;
					_fpalette.TopMost = false;
				}

				TopMost = true;		// req'd else this form won't activate at all
				TopMost = false;	// unless user closes the PaletteViewer

				BypassActivatedEvent = false;
			}
			base.OnActivated(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
			{
				e.Handled = e.SuppressKeyPress = true;
				Close();
			}
			base.OnKeyDown(e);
		}

		/// <summary>
		/// Handles the <c>FormClosing</c> event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (!PckViewF.Quit)
				{
					e.Cancel = true;
					Hide();
				}
				else
				{
					RegistryInfo.UpdateRegistry(this);
					SpritePanel.Destroy();
				}
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Sets the *proper* ClientSize.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Also called by
		/// <c><see cref="PckViewF.EnableInterface()">PckViewF.EnableInterface()</see></c>.</remarks>
		internal void OnLoad(object sender, EventArgs e)
		{
			ClientSize = new Size(
								XCImage.SpriteWidth32 * 10 + SpritePanel.Pad, // <- keep the statusbar at 32px sprite-width
								_f.SpriteHeight       * 10 + SpritePanel.Pad
														   + bar_Scale  .Height
														   + la_EditMode.Height
														   + ss_Status  .Height);
		}

		/// <summary>
		/// Sets the scale-factor of the sprite in the panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void trackbar_OnScroll(object sender, EventArgs e)
		{
			SpritePanel.ScaleFactor = bar_Scale.Value;
		}

		/// <summary>
		/// Reverses mousewheel direction on the trackbar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void trackbar_OnMouseWheel(object sender, MouseEventArgs e)
		{
			(e as HandledMouseEventArgs).Handled = true;

			if (e.Delta > 0)
			{
				if (bar_Scale.Value != bar_Scale.Minimum)
					--bar_Scale.Value;
			}
			else if (e.Delta < 0)
			{
				if (bar_Scale.Value != bar_Scale.Maximum)
					++bar_Scale.Value;
			}
			SpritePanel.ScaleFactor = bar_Scale.Value;
		}

		/// <summary>
		/// Locks or enables the sprite for edits.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditModeMouseClick(object sender, EventArgs e)
		{
			switch (Mode)
			{
				case EditMode.Locked:
					Mode = EditMode.Enabled;
					la_EditMode.Text = "Enabled";
					la_EditMode.BackColor = Color.AliceBlue;
					break;

				case EditMode.Enabled:
					Mode = EditMode.Locked;
					la_EditMode.Text = "Locked";
					la_EditMode.BackColor = Control.DefaultBackColor;
					break;
			}
		}


		/// <summary>
		/// Disables the Palette it when editing LoFTs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void popup_Palette(object sender, EventArgs e)
		{
			miPalette.Enabled = (_f.SetType != Spriteset.SpritesetType.LoFT);
			miPalette.Checked = _fpalette.Visible;
		}

		/// <summary>
		/// Shows the palette-viewer or brings it to front if already shown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Has no effect if a LoFTset is loaded.</remarks>
		private void OnShowPaletteClick(object sender, EventArgs e)
		{
			if (_f.SetType != Spriteset.SpritesetType.LoFT) // don't allow the Palette to show if editing LoFTs
			{
				if (!miPalette.Checked)
				{
					miPalette.Checked = true;
					_fpalette.Show();
				}
				else
					_fpalette.BringToFront();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>This fires after the palette's <c>FormClosing</c> event.</remarks>
		private void OnPaletteFormClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;
		}

		/// <summary>
		/// Overlays a dark grid on the sprite.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGridDarkClick(object sender, EventArgs e)
		{
			if ((miGridDark.Checked = !miGridDark.Checked))
			{
				miGridLight.Checked = false;
				SpritePanel.PenGrid = Pens.DarkGray;
			}
			else
				SpritePanel.PenGrid = null;
		}

		/// <summary>
		/// Overlays a light grid on the sprite.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGridLightClick(object sender, EventArgs e)
		{
			if ((miGridLight.Checked = !miGridLight.Checked))
			{
				miGridDark.Checked = false;
				SpritePanel.PenGrid = Pens.LightGray;
			}
			else
				SpritePanel.PenGrid = null;
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Prints color to the statusbar.
		/// </summary>
		/// <param name="color"></param>
		internal void PrintPixelColor(string color)
		{
			tssl_ColorInfo.Text = color;
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



		#region Designer
		private IContainer components;

		private MainMenu mmMainMenu;
		private MenuItem miPaletteMenu;
		private MenuItem miPalette;
		private MenuItem miGridMenu;
		private MenuItem miGridDark;
		private MenuItem miGridLight;
		private StatusStrip ss_Status;
		private ToolStripStatusLabel tssl_ColorInfo;
		private TrackBar bar_Scale;
		private Label la_EditMode;


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
			this.ss_Status = new System.Windows.Forms.StatusStrip();
			this.tssl_ColorInfo = new System.Windows.Forms.ToolStripStatusLabel();
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
			this.miGridMenu});
			// 
			// miPaletteMenu
			// 
			this.miPaletteMenu.Index = 0;
			this.miPaletteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miPalette});
			this.miPaletteMenu.Text = "&Palette";
			this.miPaletteMenu.Popup += popup_Palette;
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
			// ss_Status
			// 
			this.ss_Status.Font = new System.Drawing.Font("Verdana", 7F);
			this.ss_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_ColorInfo});
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
			this.tssl_ColorInfo.Size = new System.Drawing.Size(274, 22);
			this.tssl_ColorInfo.Spring = true;
			this.tssl_ColorInfo.Text = "colorinfo";
			this.tssl_ColorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.ShowInTaskbar = false;
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


	internal enum EditMode
	{
		Locked,
		Enabled
	}
}
