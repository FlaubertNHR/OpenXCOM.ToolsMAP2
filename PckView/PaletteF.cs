using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;


namespace PckView
{
	/// <summary>
	/// Displays the currently active 256-color palette.
	/// </summary>
	/// <remarks>This palette-viewer/chooser shall be hidden and remain hidden
	/// when a LoFTset is current.</remarks>
	internal sealed class PaletteF
		:
			Form
	{
		#region Fields
		internal readonly SpriteEditorF _feditor;
		#endregion Fields


		#region Properties
		internal PalettePanel PalPanel
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
		/// <param name="f">caller</param>
		internal PaletteF(SpriteEditorF f)
		{
			InitializeComponent();

			_feditor = f;

			PalPanel = new PalettePanel(this);
			Controls.Add(PalPanel);
			PalPanel.BringToFront();

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let ClientSize deter width and height.
				Left = _feditor.Left + 20;
				Top  = _feditor.Top  + 20;
			}

			ClientSize = new Size(
								PalettePanel.Sqrt * 20,
								PalettePanel.Sqrt * 20 + lbl_Palettebar.Height);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Handles the keydown event.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Does not require KeyPreview TRUE because there's not really
		/// a child-control to pass the keydown to. But do KeyPreview TRUE
		/// anyway.</remarks>
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
		/// Handles the FormClosing event. Closes this form if PckView is
		/// quitting - else hide.
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

					PalPanel.Destroy();
					lbl_Palettebar.Destroy();
				}
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Prints color-info to the statusbar.
		/// </summary>
		/// <param name="palid"></param>
		internal void PrintPaletteColor(int palid)
		{
			lbl_Palettebar.Text = _feditor.SpritePanel.GetColorInfo(palid);
		}
		#endregion Methods



		#region Designer
		private PaletteLabelBar lbl_Palettebar;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor. Unless you feel lucky.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbl_Palettebar = new PckView.PaletteLabelBar();
			this.SuspendLayout();
			// 
			// lbl_Palettebar
			// 
			this.lbl_Palettebar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lbl_Palettebar.Location = new System.Drawing.Point(0, 254);
			this.lbl_Palettebar.Name = "lbl_Palettebar";
			this.lbl_Palettebar.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lbl_Palettebar.Size = new System.Drawing.Size(292, 20);
			this.lbl_Palettebar.TabIndex = 0;
			this.lbl_Palettebar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PaletteF
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 274);
			this.Controls.Add(this.lbl_Palettebar);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PaletteF";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
