using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Windows;


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
		private PalettePanel _pnlPalette = new PalettePanel();
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PaletteForm()
		{
			InitializeComponent();

			ClientSize = new Size(
								PalettePanel.SwatchesPerSide * 20,
								PalettePanel.SwatchesPerSide * 20 + lblStatus.Height);

			_pnlPalette.Dock = DockStyle.Fill;
			_pnlPalette.PaletteIdChangedEvent += OnPaletteIdChanged;

			Controls.Add(_pnlPalette);

			lblStatus.SendToBack();

			var regInfo = new RegistryInfo(RegistryInfo.PaletteViewer, this); // subscribe to Load and Closing events.
			regInfo.RegisterProperties();
		}
		#endregion


		#region Eventcalls
		private void OnPaletteIdChanged(int palId)
		{
			lblStatus.Text = EditorPanel.GetColorInfo(palId);
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
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PaletteForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Palette";
			this.ResumeLayout(false);

		}
		#endregion

		private Container components = null;

		private Label lblStatus;
	}
}
