using System;
using System.Windows.Forms;


namespace DSShared.Controls
{
	public sealed class ProgressBarF
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		public ProgressBarF(string text)
		{
			InitializeComponent();
			lblInfo.Text = text;

			Show();
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets header text.
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
			lblInfo.Text = text;
		}

		/// <summary>
		/// Sets the total value to <c><see cref="Step()">Step()</see> to.</c>
		/// </summary>
		/// <param name="total"></param>
		public void SetTotal(int total)
		{
			pbProgress.Maximum = total;
		}

		/// <summary>
		/// Increments the progressbar value.
		/// </summary>
		public void Step()
		{
			++pbProgress.Value;
			Refresh();
		}
		#endregion Methods



		#region Designer
		private Label lblInfo;
		private ProgressBar pbProgress;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblInfo = new System.Windows.Forms.Label();
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInfo.Location = new System.Drawing.Point(0, 0);
			this.lblInfo.Margin = new System.Windows.Forms.Padding(0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.lblInfo.Size = new System.Drawing.Size(261, 18);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblInfo.UseWaitCursor = true;
			// 
			// pbProgress
			// 
			this.pbProgress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pbProgress.Location = new System.Drawing.Point(0, 18);
			this.pbProgress.Margin = new System.Windows.Forms.Padding(0);
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(261, 18);
			this.pbProgress.TabIndex = 1;
			this.pbProgress.UseWaitCursor = true;
			// 
			// ProgressBarF
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(261, 36);
			this.ControlBox = false;
			this.Controls.Add(this.pbProgress);
			this.Controls.Add(this.lblInfo);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(267, 42);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(267, 42);
			this.Name = "ProgressBarF";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.UseWaitCursor = true;
			this.ResumeLayout(false);

		}
		#endregion
	}
}
