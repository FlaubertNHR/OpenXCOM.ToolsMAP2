using System;
using System.Windows.Forms;


namespace XCom
{
	internal sealed class ProgressBarForm
		:
			Form
	{
		#region Properties (static)
		private static ProgressBarForm _that;
		internal static ProgressBarForm that
		{
			get
			{
				if (_that == null)
					_that = new ProgressBarForm();

				return _that;
			}
		}
		#endregion Properties (static)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ProgressBarForm()
		{
			InitializeComponent();
			Show();
		}
		#endregion cTor


		#region Methods
		internal void SetText(string text)
		{
			lblInfo.Text = text;
		}

		internal void SetTotal(int total)
		{
			pbProgress.Maximum = total;
		}

		internal void UpdateProgress()
		{
			++pbProgress.Value;
			Refresh();
		}

		internal void ResetProgress()
		{
			pbProgress.Value = 0;
		}
		#endregion Methods



		#region Designer
		private System.ComponentModel.Container components = null;

		private ProgressBar pbProgress;
		private Label lblInfo;


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
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.lblInfo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pbProgress
			// 
			this.pbProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pbProgress.Location = new System.Drawing.Point(0, 15);
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(312, 21);
			this.pbProgress.TabIndex = 0;
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInfo.Location = new System.Drawing.Point(0, 0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(312, 15);
			this.lblInfo.TabIndex = 1;
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ProgressBarForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(312, 36);
			this.ControlBox = false;
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.pbProgress);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(320, 44);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 44);
			this.Name = "ProgressBarForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion
	}
}
