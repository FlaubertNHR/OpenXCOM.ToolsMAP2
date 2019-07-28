namespace MapView.Volutar
{
	partial class FindVolutarF
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblNotice;
		private System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.Panel pnl_Buttons;
		private System.Windows.Forms.Panel pnl_Head;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button btnFindFile;


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
			this.lblNotice = new System.Windows.Forms.Label();
			this.tbInput = new System.Windows.Forms.TextBox();
			this.pnl_Buttons = new System.Windows.Forms.Panel();
			this.pnl_Head = new System.Windows.Forms.Panel();
			this.btnFindFile = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pnl_Buttons.SuspendLayout();
			this.pnl_Head.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(110, 0);
			this.btnOk.Margin = new System.Windows.Forms.Padding(0);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(85, 25);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "ok";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(200, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 25);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "cancel";
			// 
			// lblNotice
			// 
			this.lblNotice.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblNotice.Location = new System.Drawing.Point(0, 0);
			this.lblNotice.Margin = new System.Windows.Forms.Padding(0);
			this.lblNotice.Name = "lblNotice";
			this.lblNotice.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.lblNotice.Size = new System.Drawing.Size(392, 25);
			this.lblNotice.TabIndex = 0;
			this.lblNotice.Text = "Enter the path to Volutar\'s MCDEdit.exe in full.";
			this.lblNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbInput
			// 
			this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbInput.Location = new System.Drawing.Point(0, 25);
			this.tbInput.Margin = new System.Windows.Forms.Padding(0);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(361, 19);
			this.tbInput.TabIndex = 1;
			// 
			// pnl_Buttons
			// 
			this.pnl_Buttons.Controls.Add(this.btnOk);
			this.pnl_Buttons.Controls.Add(this.btnCancel);
			this.pnl_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnl_Buttons.Location = new System.Drawing.Point(0, 50);
			this.pnl_Buttons.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Buttons.Name = "pnl_Buttons";
			this.pnl_Buttons.Size = new System.Drawing.Size(392, 29);
			this.pnl_Buttons.TabIndex = 0;
			// 
			// pnl_Head
			// 
			this.pnl_Head.Controls.Add(this.btnFindFile);
			this.pnl_Head.Controls.Add(this.tbInput);
			this.pnl_Head.Controls.Add(this.lblNotice);
			this.pnl_Head.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_Head.Location = new System.Drawing.Point(0, 0);
			this.pnl_Head.Name = "pnl_Head";
			this.pnl_Head.Size = new System.Drawing.Size(392, 50);
			this.pnl_Head.TabIndex = 5;
			// 
			// btnFindFile
			// 
			this.btnFindFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindFile.Location = new System.Drawing.Point(361, 24);
			this.btnFindFile.Margin = new System.Windows.Forms.Padding(0);
			this.btnFindFile.Name = "btnFindFile";
			this.btnFindFile.Size = new System.Drawing.Size(30, 20);
			this.btnFindFile.TabIndex = 2;
			this.btnFindFile.Text = "...";
			this.btnFindFile.UseVisualStyleBackColor = true;
			this.btnFindFile.Click += new System.EventHandler(this.btnFindFile_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Executable files|*.exe|All files|*.*";
			// 
			// FindVolutarF
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(392, 79);
			this.Controls.Add(this.pnl_Head);
			this.Controls.Add(this.pnl_Buttons);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 105);
			this.Name = "FindVolutarF";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Find Volutar";
			this.Load += new System.EventHandler(this.OnLoad_form);
			this.pnl_Buttons.ResumeLayout(false);
			this.pnl_Head.ResumeLayout(false);
			this.pnl_Head.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
