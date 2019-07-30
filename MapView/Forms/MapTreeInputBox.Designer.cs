namespace MapView
{
	partial class MapTreeInputBox
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblInfoTop;
		private System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.Label lblInfoBottom;
		private System.Windows.Forms.Label lblParent;


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
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblInfoTop = new System.Windows.Forms.Label();
			this.tbInput = new System.Windows.Forms.TextBox();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlTop = new System.Windows.Forms.Panel();
			this.lblParent = new System.Windows.Forms.Label();
			this.lblInfoBottom = new System.Windows.Forms.Label();
			this.pnlBottom.SuspendLayout();
			this.pnlTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(116, 0);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 25);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "Ok";
			this.btnOk.Click += new System.EventHandler(this.OnAcceptClick);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(201, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 25);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// lblInfoTop
			// 
			this.lblInfoTop.Location = new System.Drawing.Point(5, 25);
			this.lblInfoTop.Name = "lblInfoTop";
			this.lblInfoTop.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
			this.lblInfoTop.Size = new System.Drawing.Size(385, 50);
			this.lblInfoTop.TabIndex = 1;
			this.lblInfoTop.Text = "lblInfoTop";
			this.lblInfoTop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbInput
			// 
			this.tbInput.Location = new System.Drawing.Point(20, 80);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(355, 19);
			this.tbInput.TabIndex = 2;
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.btnOk);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 147);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(394, 29);
			this.pnlBottom.TabIndex = 1;
			// 
			// pnlTop
			// 
			this.pnlTop.Controls.Add(this.lblParent);
			this.pnlTop.Controls.Add(this.lblInfoBottom);
			this.pnlTop.Controls.Add(this.tbInput);
			this.pnlTop.Controls.Add(this.lblInfoTop);
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlTop.Location = new System.Drawing.Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(394, 147);
			this.pnlTop.TabIndex = 0;
			// 
			// lblParent
			// 
			this.lblParent.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblParent.ForeColor = System.Drawing.SystemColors.Highlight;
			this.lblParent.Location = new System.Drawing.Point(5, 5);
			this.lblParent.Name = "lblParent";
			this.lblParent.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
			this.lblParent.Size = new System.Drawing.Size(385, 15);
			this.lblParent.TabIndex = 0;
			this.lblParent.Text = "lblParent";
			this.lblParent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblInfoBottom
			// 
			this.lblInfoBottom.Location = new System.Drawing.Point(5, 105);
			this.lblInfoBottom.Name = "lblInfoBottom";
			this.lblInfoBottom.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
			this.lblInfoBottom.Size = new System.Drawing.Size(385, 40);
			this.lblInfoBottom.TabIndex = 3;
			this.lblInfoBottom.Text = "lblInfoBottom";
			this.lblInfoBottom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MapTreeInputBox
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(394, 176);
			this.Controls.Add(this.pnlTop);
			this.Controls.Add(this.pnlBottom);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 200);
			this.Name = "MapTreeInputBox";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.pnlBottom.ResumeLayout(false);
			this.pnlTop.ResumeLayout(false);
			this.pnlTop.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
