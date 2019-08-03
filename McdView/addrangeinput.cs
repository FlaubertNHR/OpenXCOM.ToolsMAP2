using System;
using System.Windows.Forms;


namespace McdView
{
	internal sealed class AddRangeInput
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal AddRangeInput()
		{
			InitializeComponent();

			tb_records.Text = TerrainPanel_main._add.ToString();
			tb_records.Focus();
		}
		#endregion cTor


		#region Events (override)
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
		#endregion Events (override)


		#region Events
		private void OnClick_Accept(object sender, EventArgs e)
		{
			TerrainPanel_main._add = Int32.Parse(tb_records.Text);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			int result;
			if (!Int32.TryParse(tb_records.Text, out result)
				|| result < 0)
			{
				tb_records.Text = "0";
			}
		}
		#endregion Events


		#region Designer
		private Label lbl_info;
		private TextBox tb_records;
		private Button btn_accept;


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbl_info = new System.Windows.Forms.Label();
			this.tb_records = new System.Windows.Forms.TextBox();
			this.btn_accept = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbl_info
			// 
			this.lbl_info.Location = new System.Drawing.Point(5, 5);
			this.lbl_info.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_info.Name = "lbl_info";
			this.lbl_info.Size = new System.Drawing.Size(85, 20);
			this.lbl_info.TabIndex = 0;
			this.lbl_info.Text = "count";
			this.lbl_info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tb_records
			// 
			this.tb_records.Location = new System.Drawing.Point(5, 30);
			this.tb_records.Margin = new System.Windows.Forms.Padding(0);
			this.tb_records.Name = "tb_records";
			this.tb_records.Size = new System.Drawing.Size(85, 19);
			this.tb_records.TabIndex = 1;
			this.tb_records.Text = "0";
			this.tb_records.TextChanged += new System.EventHandler(this.OnTextChanged);
			// 
			// btn_accept
			// 
			this.btn_accept.Location = new System.Drawing.Point(100, 5);
			this.btn_accept.Margin = new System.Windows.Forms.Padding(0);
			this.btn_accept.Name = "btn_accept";
			this.btn_accept.Size = new System.Drawing.Size(75, 45);
			this.btn_accept.TabIndex = 2;
			this.btn_accept.Text = "accept";
			this.btn_accept.UseVisualStyleBackColor = true;
			this.btn_accept.Click += new System.EventHandler(this.OnClick_Accept);
			// 
			// AddRangeInput
			// 
			this.AcceptButton = this.btn_accept;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(179, 56);
			this.Controls.Add(this.btn_accept);
			this.Controls.Add(this.tb_records);
			this.Controls.Add(this.lbl_info);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddRangeInput";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add range";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
