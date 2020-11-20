using System;
using System.Windows.Forms;

using XCom.Base;


namespace MapView
{
	internal sealed class TileslotSubstitutionD
		:
			Form
	{
		#region Fields
		MapFileBase _base;

		internal int src;
		internal int dst;
		#endregion Fields


		#region cTor
		internal TileslotSubstitutionD(MapFileBase @base)
		{
			InitializeComponent();

			_base = @base;
		}
		#endregion cTor


		#region Events (override)
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				if (e.Cancel =  String.IsNullOrEmpty(tb_Src.Text)
							|| (String.IsNullOrEmpty(tb_Dst.Text) && !cb_Clear.Checked)
							||  !Int32.TryParse(tb_Src.Text, out src)
							|| (!Int32.TryParse(tb_Dst.Text, out dst) && !cb_Clear.Checked)
							||   src < 0 || src >= _base.Parts.Count
							|| ((dst < 0 || dst >= _base.Parts.Count) && !cb_Clear.Checked)
							|| (src == dst && !cb_Clear.Checked))
				{
					MessageBox.Show(
								this,
								"Darth Vader lives.",
								" Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}
			}
		}
		#endregion Events (override)


		#region Events
		private void OnClearChanged(object sender, EventArgs e)
		{
			tb_Dst.Enabled = !cb_Clear.Checked;
		}
		#endregion Events



		#region Designer
		private System.ComponentModel.Container components = null;

		private Label la_Src;
		private TextBox tb_Src;
		private Label la_Dst;
		private TextBox tb_Dst;
		private Button bt_Ok;
		internal CheckBox cb_Clear;


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
			this.la_Src = new System.Windows.Forms.Label();
			this.tb_Src = new System.Windows.Forms.TextBox();
			this.la_Dst = new System.Windows.Forms.Label();
			this.tb_Dst = new System.Windows.Forms.TextBox();
			this.bt_Ok = new System.Windows.Forms.Button();
			this.cb_Clear = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// la_Src
			// 
			this.la_Src.Location = new System.Drawing.Point(5, 5);
			this.la_Src.Margin = new System.Windows.Forms.Padding(0);
			this.la_Src.Name = "la_Src";
			this.la_Src.Size = new System.Drawing.Size(170, 20);
			this.la_Src.TabIndex = 0;
			this.la_Src.Text = "Change all tileparts of setId";
			this.la_Src.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tb_Src
			// 
			this.tb_Src.Location = new System.Drawing.Point(10, 30);
			this.tb_Src.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Src.Name = "tb_Src";
			this.tb_Src.Size = new System.Drawing.Size(100, 19);
			this.tb_Src.TabIndex = 1;
			this.tb_Src.WordWrap = false;
			// 
			// la_Dst
			// 
			this.la_Dst.Location = new System.Drawing.Point(5, 57);
			this.la_Dst.Margin = new System.Windows.Forms.Padding(0);
			this.la_Dst.Name = "la_Dst";
			this.la_Dst.Size = new System.Drawing.Size(70, 15);
			this.la_Dst.TabIndex = 2;
			this.la_Dst.Text = "to setId or";
			this.la_Dst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tb_Dst
			// 
			this.tb_Dst.Location = new System.Drawing.Point(10, 80);
			this.tb_Dst.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Dst.Name = "tb_Dst";
			this.tb_Dst.Size = new System.Drawing.Size(100, 19);
			this.tb_Dst.TabIndex = 3;
			this.tb_Dst.WordWrap = false;
			// 
			// bt_Ok
			// 
			this.bt_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Ok.Location = new System.Drawing.Point(120, 80);
			this.bt_Ok.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Ok.Name = "bt_Ok";
			this.bt_Ok.Size = new System.Drawing.Size(65, 25);
			this.bt_Ok.TabIndex = 5;
			this.bt_Ok.Text = "Ok";
			this.bt_Ok.UseVisualStyleBackColor = true;
			// 
			// cb_Clear
			// 
			this.cb_Clear.Location = new System.Drawing.Point(85, 55);
			this.cb_Clear.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Clear.Name = "cb_Clear";
			this.cb_Clear.Size = new System.Drawing.Size(55, 20);
			this.cb_Clear.TabIndex = 4;
			this.cb_Clear.Text = "clear";
			this.cb_Clear.UseVisualStyleBackColor = true;
			this.cb_Clear.CheckedChanged += new System.EventHandler(this.OnClearChanged);
			// 
			// TileslotSubstitutionD
			// 
			this.AcceptButton = this.bt_Ok;
			this.ClientSize = new System.Drawing.Size(194, 106);
			this.Controls.Add(this.cb_Clear);
			this.Controls.Add(this.bt_Ok);
			this.Controls.Add(this.tb_Dst);
			this.Controls.Add(this.la_Dst);
			this.Controls.Add(this.tb_Src);
			this.Controls.Add(this.la_Src);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TileslotSubstitutionD";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Tileslot Substitution";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
