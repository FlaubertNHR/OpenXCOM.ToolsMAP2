using System;
using System.Windows.Forms;

using XCom.Base;


namespace MapView
{
	internal sealed class TileslotSubstitutionDialog
		:
			Form
	{
		#region Fields
		MapFileBase _base;

		internal int src;
		internal int dst;
		#endregion Fields


		#region cTor
		internal TileslotSubstitutionDialog(MapFileBase @base)
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
				e.Cancel = String.IsNullOrEmpty(tb_Src.Text)
						|| String.IsNullOrEmpty(tb_Dst.Text)
						|| !Int32.TryParse(tb_Src.Text, out src)
						|| !Int32.TryParse(tb_Dst.Text, out dst)
						|| src < 0 || src >= _base.Parts.Count
						|| dst < 0 || dst >= _base.Parts.Count
						|| src == dst;

				if (e.Cancel)
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
		#endregion Events (override)



		#region Designer
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label la_Src;
		private System.Windows.Forms.TextBox tb_Src;
		private System.Windows.Forms.Label la_Dst;
		private System.Windows.Forms.TextBox tb_Dst;
		private System.Windows.Forms.Button bt_Ok;


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
			this.la_Dst.Location = new System.Drawing.Point(5, 55);
			this.la_Dst.Margin = new System.Windows.Forms.Padding(0);
			this.la_Dst.Name = "la_Dst";
			this.la_Dst.Size = new System.Drawing.Size(170, 20);
			this.la_Dst.TabIndex = 2;
			this.la_Dst.Text = "to setId";
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
			this.bt_Ok.TabIndex = 4;
			this.bt_Ok.Text = "Ok";
			this.bt_Ok.UseVisualStyleBackColor = true;
			// 
			// TileslotSubstitutionDialog
			// 
			this.AcceptButton = this.bt_Ok;
			this.ClientSize = new System.Drawing.Size(194, 106);
			this.Controls.Add(this.bt_Ok);
			this.Controls.Add(this.tb_Dst);
			this.Controls.Add(this.la_Dst);
			this.Controls.Add(this.tb_Src);
			this.Controls.Add(this.la_Src);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TileslotSubstitutionDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Tileslot Substitution";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
