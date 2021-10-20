using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	internal sealed partial class About
	{
		#region Designer
		/// <summary>
		/// (un)Required designer variable.
		/// </summary>
		private IContainer components;

		private Label la_Date;
		private TextBox tb_Ver;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Timer t1;


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
			this.components = new System.ComponentModel.Container();
			this.la_Date = new System.Windows.Forms.Label();
			this.tb_Ver = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.t1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// la_Date
			// 
			this.la_Date.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_Date.Location = new System.Drawing.Point(0, 0);
			this.la_Date.Margin = new System.Windows.Forms.Padding(0);
			this.la_Date.Name = "la_Date";
			this.la_Date.Size = new System.Drawing.Size(294, 19);
			this.la_Date.TabIndex = 0;
			this.la_Date.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// tb_Ver
			// 
			this.tb_Ver.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tb_Ver.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tb_Ver.HideSelection = false;
			this.tb_Ver.Location = new System.Drawing.Point(5, 26);
			this.tb_Ver.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Ver.Multiline = true;
			this.tb_Ver.Name = "tb_Ver";
			this.tb_Ver.ReadOnly = true;
			this.tb_Ver.Size = new System.Drawing.Size(285, 85);
			this.tb_Ver.TabIndex = 1;
			this.tb_Ver.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_Ver.WordWrap = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 117);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(285, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "author Ben Ratzlaff aka Daishiva";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 133);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(285, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "advisors BladeFireLight J Farceur";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 152);
			this.label3.Margin = new System.Windows.Forms.Padding(0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(285, 15);
			this.label3.TabIndex = 4;
			this.label3.Text = "contributors pmprog The BigSot etc";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 171);
			this.label4.Margin = new System.Windows.Forms.Padding(0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(285, 15);
			this.label4.TabIndex = 6;
			this.label4.Text = "maintainer kevL";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 187);
			this.label5.Margin = new System.Windows.Forms.Padding(0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(285, 15);
			this.label5.TabIndex = 7;
			this.label5.Text = "reporters luke83 et alii multus";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 209);
			this.label6.Margin = new System.Windows.Forms.Padding(0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(285, 15);
			this.label6.TabIndex = 8;
			this.label6.Text = "YamlDotNet by Antoine Aubry et al.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// t1
			// 
			this.t1.Interval = 15;
			this.t1.Tick += new System.EventHandler(this.OnTick);
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(294, 232);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tb_Ver);
			this.Controls.Add(this.la_Date);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
