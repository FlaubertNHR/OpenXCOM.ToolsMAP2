using System;
using System.Windows.Forms;


namespace PckView
{
	internal sealed partial class About
	{
		#region Designer
		private Label label1;
		private Label label2;
		private TextBox tb_Ver;
		private Label la_Date;
		private Label la_Architecture;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tb_Ver = new System.Windows.Forms.TextBox();
			this.la_Date = new System.Windows.Forms.Label();
			this.la_Architecture = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 5);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(285, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Main coding and design: Ben Ratzlaff";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 23);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(285, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "revised: kevL";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tb_Ver
			// 
			this.tb_Ver.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tb_Ver.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tb_Ver.HideSelection = false;
			this.tb_Ver.Location = new System.Drawing.Point(5, 44);
			this.tb_Ver.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Ver.Multiline = true;
			this.tb_Ver.Name = "tb_Ver";
			this.tb_Ver.ReadOnly = true;
			this.tb_Ver.Size = new System.Drawing.Size(285, 43);
			this.tb_Ver.TabIndex = 4;
			this.tb_Ver.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_Ver.WordWrap = false;
			// 
			// la_Date
			// 
			this.la_Date.Location = new System.Drawing.Point(5, 95);
			this.la_Date.Margin = new System.Windows.Forms.Padding(0);
			this.la_Date.Name = "la_Date";
			this.la_Date.Size = new System.Drawing.Size(285, 15);
			this.la_Date.TabIndex = 2;
			this.la_Date.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// la_Architecture
			// 
			this.la_Architecture.Location = new System.Drawing.Point(5, 113);
			this.la_Architecture.Margin = new System.Windows.Forms.Padding(0);
			this.la_Architecture.Name = "la_Architecture";
			this.la_Architecture.Size = new System.Drawing.Size(285, 15);
			this.la_Architecture.TabIndex = 3;
			this.la_Architecture.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(294, 134);
			this.Controls.Add(this.tb_Ver);
			this.Controls.Add(this.la_Architecture);
			this.Controls.Add(this.la_Date);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "PckView about";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
