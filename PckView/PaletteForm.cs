using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared;


namespace PckView
{
	/// <summary>
	/// Displays the currently active 256-color palette.
	/// </summary>
	internal sealed class PaletteForm
		:
			Form
	{
		#region Fields
		internal readonly SpriteEditorF _feditor;
		internal readonly PalettePanel _pnlPalette;
		#endregion Fields


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
		internal PaletteForm(SpriteEditorF f)
		{
			InitializeComponent();

			_feditor = f;
			_pnlPalette = new PalettePanel(this);

			Controls.Add(_pnlPalette);

			lblStatus.SendToBack();

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let ClientSize deter width and height.
				Left = _feditor.Left + 20;
				Top  = _feditor.Top  + 20;
			}

			ClientSize = new Size(
								PalettePanel.SwatchesPerSide * 20,
								PalettePanel.SwatchesPerSide * 20 + lblStatus.Height);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// @note Does not require KeyPreview TRUE because there's not really a
		/// child-control to pass the keydown to. But do KeyPreview TRUE anyway.
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


		#region Methods
		internal void PrintPaletteId(int palid)
		{
			lblStatus.Text = _feditor.SpritePanel.GetColorInfo(palid);
		}
		#endregion Methods



		#region Designer
		private Container components = null;

		private Label lblStatus;


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
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblStatus.Location = new System.Drawing.Point(0, 254);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblStatus.Size = new System.Drawing.Size(292, 20);
			this.lblStatus.TabIndex = 0;
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PaletteForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 274);
			this.Controls.Add(this.lblStatus);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PaletteForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
