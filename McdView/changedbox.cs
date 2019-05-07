using System;
using System.Windows.Forms;


namespace McdView
{
	/// <summary>
	/// A dialog that asks the user what to do when closing a file that has been
	/// changed: Save, Lose changes, or Cancel.
	/// </summary>
	internal sealed class ChangedBox
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ChangedBox(string info)
		{
			InitializeComponent();

			lbl_info.Text = info;
		}
		#endregion cTor


		#region Events (override)
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			btn_Save.Select();
		}
		#endregion Events (override)


		#region Events
		private void OnClick_Save(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
			Close();
		}

		private void OnClick_Lose(object sender, EventArgs e)
		{
			DialogResult = DialogResult.No;
			Close();
		}

		private void OnClick_Cancel(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
		#endregion Events


		#region Designer
		private Label lbl_info;
		private Button btn_Cancel;
		private Button btn_Save;
		private Button btn_Lose;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbl_info = new System.Windows.Forms.Label();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_Save = new System.Windows.Forms.Button();
			this.btn_Lose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbl_info
			// 
			this.lbl_info.Location = new System.Drawing.Point(5, 10);
			this.lbl_info.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_info.Name = "lbl_info";
			this.lbl_info.Size = new System.Drawing.Size(235, 15);
			this.lbl_info.TabIndex = 0;
			this.lbl_info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(5, 30);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(75, 40);
			this.btn_Cancel.TabIndex = 1;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.OnClick_Cancel);
			// 
			// btn_Save
			// 
			this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Save.Location = new System.Drawing.Point(165, 30);
			this.btn_Save.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Save.Name = "btn_Save";
			this.btn_Save.Size = new System.Drawing.Size(75, 40);
			this.btn_Save.TabIndex = 3;
			this.btn_Save.Text = "Save and close";
			this.btn_Save.UseVisualStyleBackColor = true;
			this.btn_Save.Click += new System.EventHandler(this.OnClick_Save);
			// 
			// btn_Lose
			// 
			this.btn_Lose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Lose.Location = new System.Drawing.Point(85, 30);
			this.btn_Lose.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Lose.Name = "btn_Lose";
			this.btn_Lose.Size = new System.Drawing.Size(75, 40);
			this.btn_Lose.TabIndex = 2;
			this.btn_Lose.Text = "Lose changes";
			this.btn_Lose.UseVisualStyleBackColor = true;
			this.btn_Lose.Click += new System.EventHandler(this.OnClick_Lose);
			// 
			// ChangedBox
			// 
			this.AcceptButton = this.btn_Save;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(244, 76);
			this.Controls.Add(this.btn_Lose);
			this.Controls.Add(this.btn_Save);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.lbl_info);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangedBox";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = " Changed";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
