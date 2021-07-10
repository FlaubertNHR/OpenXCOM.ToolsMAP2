using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	internal sealed partial class About
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private Label lblVersion;
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
			this.lblVersion = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.t1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// lblVersion
			// 
			this.lblVersion.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblVersion.Location = new System.Drawing.Point(0, 0);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(314, 55);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(305, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "AUTHOR - Ben Ratzlaff aka Daishiva";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(305, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "ASSIST - BladeFireLight / J Farceur";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(305, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "REVISION - pmprog";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(305, 15);
			this.label4.TabIndex = 4;
			this.label4.Text = "REVISION - TheBigSot";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 140);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(305, 15);
			this.label5.TabIndex = 5;
			this.label5.Text = "REVISION - kevL";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 155);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(305, 15);
			this.label6.TabIndex = 6;
			this.label6.Text = "ASSIST - luke83 / et alii multus";
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
			this.ClientSize = new System.Drawing.Size(314, 186);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblVersion);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(320, 210);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 210);
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
