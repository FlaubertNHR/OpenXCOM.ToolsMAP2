namespace MapView.Forms.Error
{
	partial class ErrorWindow
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
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
			this.lblHead = new System.Windows.Forms.Label();
			this.pnl_Info = new System.Windows.Forms.Panel();
			this.gbDetails = new System.Windows.Forms.GroupBox();
			this.tbDetails = new System.Windows.Forms.TextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.pnl_Bot = new System.Windows.Forms.Panel();
			this.btnTerminate = new System.Windows.Forms.Button();
			this.pnl_Info.SuspendLayout();
			this.gbDetails.SuspendLayout();
			this.pnl_Bot.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblHead
			// 
			this.lblHead.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblHead.Font = new System.Drawing.Font("Comic Sans MS", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHead.Location = new System.Drawing.Point(0, 0);
			this.lblHead.Margin = new System.Windows.Forms.Padding(0);
			this.lblHead.Name = "lblHead";
			this.lblHead.Size = new System.Drawing.Size(792, 80);
			this.lblHead.TabIndex = 0;
			this.lblHead.Text = "lblHead";
			this.lblHead.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pnl_Info
			// 
			this.pnl_Info.Controls.Add(this.gbDetails);
			this.pnl_Info.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_Info.Location = new System.Drawing.Point(0, 80);
			this.pnl_Info.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Info.Name = "pnl_Info";
			this.pnl_Info.Size = new System.Drawing.Size(792, 250);
			this.pnl_Info.TabIndex = 1;
			// 
			// gbDetails
			// 
			this.gbDetails.Controls.Add(this.tbDetails);
			this.gbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbDetails.Location = new System.Drawing.Point(0, 0);
			this.gbDetails.Margin = new System.Windows.Forms.Padding(0);
			this.gbDetails.Name = "gbDetails";
			this.gbDetails.Padding = new System.Windows.Forms.Padding(2);
			this.gbDetails.Size = new System.Drawing.Size(792, 250);
			this.gbDetails.TabIndex = 0;
			this.gbDetails.TabStop = false;
			this.gbDetails.Text = " Error Details ";
			// 
			// tbDetails
			// 
			this.tbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbDetails.Location = new System.Drawing.Point(2, 14);
			this.tbDetails.Multiline = true;
			this.tbDetails.Name = "tbDetails";
			this.tbDetails.ReadOnly = true;
			this.tbDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDetails.Size = new System.Drawing.Size(788, 234);
			this.tbDetails.TabIndex = 1;
			this.tbDetails.Text = "tbDetails";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.btnClose.Location = new System.Drawing.Point(685, 5);
			this.btnClose.Margin = new System.Windows.Forms.Padding(0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(104, 35);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// pnl_Bot
			// 
			this.pnl_Bot.Controls.Add(this.btnTerminate);
			this.pnl_Bot.Controls.Add(this.btnClose);
			this.pnl_Bot.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnl_Bot.Location = new System.Drawing.Point(0, 330);
			this.pnl_Bot.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Bot.Name = "pnl_Bot";
			this.pnl_Bot.Size = new System.Drawing.Size(792, 44);
			this.pnl_Bot.TabIndex = 3;
			// 
			// btnTerminate
			// 
			this.btnTerminate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTerminate.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.btnTerminate.Location = new System.Drawing.Point(575, 5);
			this.btnTerminate.Margin = new System.Windows.Forms.Padding(0);
			this.btnTerminate.Name = "btnTerminate";
			this.btnTerminate.Size = new System.Drawing.Size(104, 35);
			this.btnTerminate.TabIndex = 3;
			this.btnTerminate.Text = "Terminate!";
			this.btnTerminate.UseVisualStyleBackColor = true;
			this.btnTerminate.Click += new System.EventHandler(this.click_Terminate);
			// 
			// ErrorWindow
			// 
			this.AcceptButton = this.btnTerminate;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(792, 374);
			this.Controls.Add(this.pnl_Info);
			this.Controls.Add(this.lblHead);
			this.Controls.Add(this.pnl_Bot);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ErrorWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "oop Exception";
			this.pnl_Info.ResumeLayout(false);
			this.gbDetails.ResumeLayout(false);
			this.gbDetails.PerformLayout();
			this.pnl_Bot.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private System.Windows.Forms.Label lblHead;
		private System.Windows.Forms.Panel pnl_Info;
		private System.Windows.Forms.GroupBox gbDetails;
		private System.Windows.Forms.TextBox tbDetails;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Panel pnl_Bot;
		private System.Windows.Forms.Button btnTerminate;
		#endregion Designer
	}
}
