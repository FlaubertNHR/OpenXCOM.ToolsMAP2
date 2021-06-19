using System;
using System.Windows.Forms;


namespace MapView.ExternalProcess
{
	/// <summary>
	/// This could be turned easily into a generic form providing a textbox for
	/// user-input.
	/// </summary>
	internal sealed class ExternalProcessF
		:
			Form
	{
		#region Properties
		/// <summary>
		/// Gets the text in the textbox.
		/// </summary>
		internal string InputString
		{
			get { return tb_Input.Text; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ExternalProcessF()
		{
			InitializeComponent();
			ActiveControl = btn_Cancel;
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Opens an <c><see cref="OpenFileDialog"/></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnFindFile_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title = "Browse to an application or file";

				if (ofd.ShowDialog(this) == DialogResult.OK)
					tb_Input.Text = ofd.FileName;
			}
		}
		#endregion Events


		#region Designer
		private Button btn_Ok;
		private Button btn_Cancel;
		private Label lbl_Head;
		private TextBox tb_Input;
		private Panel pnl_Buttons;
		private Panel pnl_Head;
		private Button btn_FindFile;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn_Ok = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.lbl_Head = new System.Windows.Forms.Label();
			this.tb_Input = new System.Windows.Forms.TextBox();
			this.pnl_Buttons = new System.Windows.Forms.Panel();
			this.pnl_Head = new System.Windows.Forms.Panel();
			this.btn_FindFile = new System.Windows.Forms.Button();
			this.pnl_Buttons.SuspendLayout();
			this.pnl_Head.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_Ok
			// 
			this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_Ok.Location = new System.Drawing.Point(110, 0);
			this.btn_Ok.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new System.Drawing.Size(85, 25);
			this.btn_Ok.TabIndex = 0;
			this.btn_Ok.Text = "ok";
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(200, 0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(85, 25);
			this.btn_Cancel.TabIndex = 1;
			this.btn_Cancel.Text = "cancel";
			// 
			// lbl_Head
			// 
			this.lbl_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Head.Location = new System.Drawing.Point(0, 0);
			this.lbl_Head.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Head.Name = "lbl_Head";
			this.lbl_Head.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.lbl_Head.Size = new System.Drawing.Size(392, 25);
			this.lbl_Head.TabIndex = 0;
			this.lbl_Head.Text = "Enter the path to an application or file.";
			this.lbl_Head.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tb_Input
			// 
			this.tb_Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Input.Location = new System.Drawing.Point(0, 25);
			this.tb_Input.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Input.Name = "tb_Input";
			this.tb_Input.Size = new System.Drawing.Size(361, 19);
			this.tb_Input.TabIndex = 1;
			// 
			// pnl_Buttons
			// 
			this.pnl_Buttons.Controls.Add(this.btn_Ok);
			this.pnl_Buttons.Controls.Add(this.btn_Cancel);
			this.pnl_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnl_Buttons.Location = new System.Drawing.Point(0, 50);
			this.pnl_Buttons.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Buttons.Name = "pnl_Buttons";
			this.pnl_Buttons.Size = new System.Drawing.Size(392, 29);
			this.pnl_Buttons.TabIndex = 1;
			// 
			// pnl_Head
			// 
			this.pnl_Head.Controls.Add(this.btn_FindFile);
			this.pnl_Head.Controls.Add(this.tb_Input);
			this.pnl_Head.Controls.Add(this.lbl_Head);
			this.pnl_Head.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_Head.Location = new System.Drawing.Point(0, 0);
			this.pnl_Head.Name = "pnl_Head";
			this.pnl_Head.Size = new System.Drawing.Size(392, 50);
			this.pnl_Head.TabIndex = 0;
			// 
			// btn_FindFile
			// 
			this.btn_FindFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_FindFile.Location = new System.Drawing.Point(361, 24);
			this.btn_FindFile.Margin = new System.Windows.Forms.Padding(0);
			this.btn_FindFile.Name = "btn_FindFile";
			this.btn_FindFile.Size = new System.Drawing.Size(30, 20);
			this.btn_FindFile.TabIndex = 2;
			this.btn_FindFile.Text = "...";
			this.btn_FindFile.UseVisualStyleBackColor = true;
			this.btn_FindFile.Click += new System.EventHandler(this.btnFindFile_Click);
			// 
			// ExternalProcessF
			// 
			this.AcceptButton = this.btn_Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(392, 79);
			this.Controls.Add(this.pnl_Head);
			this.Controls.Add(this.pnl_Buttons);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 105);
			this.Name = "ExternalProcessF";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "External process";
			this.pnl_Buttons.ResumeLayout(false);
			this.pnl_Head.ResumeLayout(false);
			this.pnl_Head.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
