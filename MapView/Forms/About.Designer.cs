namespace MapView
{
	partial class About
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.MoveTimer = new System.Windows.Forms.Timer(this.components);
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 75);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(305, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "AUTHOR - Ben Ratzlaff aka DaiShiva";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 90);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(305, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "ASSIST - BladeFireLight / J Farceur";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblVersion
			// 
			this.lblVersion.Location = new System.Drawing.Point(5, 10);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(305, 55);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 130);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(305, 15);
			this.label4.TabIndex = 4;
			this.label4.Text = "REVISION - TheBigSot";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MoveTimer
			// 
			this.MoveTimer.Enabled = true;
			this.MoveTimer.Interval = 1000;
			this.MoveTimer.Tick += new System.EventHandler(this.OnTick);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 145);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(305, 15);
			this.label5.TabIndex = 5;
			this.label5.Text = "REVISION - kevL";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 115);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(305, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "REVISION - pmprog";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 160);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(305, 15);
			this.label6.TabIndex = 6;
			this.label6.Text = "ASSIST - luke83 / et mult al";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(314, 186);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(320, 210);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 210);
			this.Name = "About";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			this.Shown += new System.EventHandler(this.OnShown);
			this.LocationChanged += new System.EventHandler(this.OnLocationChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Timer MoveTimer;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
	}
}
