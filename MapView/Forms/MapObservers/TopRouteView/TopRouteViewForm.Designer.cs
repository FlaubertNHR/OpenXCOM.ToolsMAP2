namespace MapView.Forms.MapObservers.TileViews
{
	partial class TopRouteViewForm
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.tabControl = new DSShared.Windows.CompositedTabControl();
			this.tp_Top = new System.Windows.Forms.TabPage();
			this.tp_Route = new System.Windows.Forms.TabPage();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tp_Top);
			this.tabControl.Controls.Add(this.tp_Route);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(632, 454);
			this.tabControl.TabIndex = 1;
			// 
			// tp_Top
			// 
			this.tp_Top.Location = new System.Drawing.Point(4, 21);
			this.tp_Top.Name = "tp_Top";
			this.tp_Top.Padding = new System.Windows.Forms.Padding(3);
			this.tp_Top.Size = new System.Drawing.Size(624, 429);
			this.tp_Top.TabIndex = 0;
			this.tp_Top.Text = "TopView";
			// 
			// tp_Route
			// 
			this.tp_Route.Location = new System.Drawing.Point(4, 21);
			this.tp_Route.Name = "tp_Route";
			this.tp_Route.Padding = new System.Windows.Forms.Padding(3);
			this.tp_Route.Size = new System.Drawing.Size(624, 429);
			this.tp_Route.TabIndex = 1;
			this.tp_Route.Text = "RouteView";
			// 
			// TopRouteViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.tabControl);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.KeyPreview = true;
			this.Name = "TopRouteViewForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = " Top/Route Views";
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		DSShared.Windows.CompositedTabControl tabControl;

		private System.Windows.Forms.TabPage tp_Top;
		private System.Windows.Forms.TabPage tp_Route;
		#endregion Designer
	}
}
