using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MapObservers.TileViews
{
	partial class TopRouteViewForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		/* The #develop designer is going to delete this:

			this.TopViewControl = new MapView.Forms.MapObservers.TopViews.TopView();
			this.RouteViewControl = new MapView.Forms.MapObservers.RouteViews.RouteView();

		- so copy it back into InitializeComponent() */

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TopViewControl = new MapView.Forms.MapObservers.TopViews.TopView();
			this.RouteViewControl = new MapView.Forms.MapObservers.RouteViews.RouteView();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tp_Top = new System.Windows.Forms.TabPage();
			this.tp_Route = new System.Windows.Forms.TabPage();
			this.tabControl.SuspendLayout();
			this.tp_Top.SuspendLayout();
			this.tp_Route.SuspendLayout();
			this.SuspendLayout();
			// 
			// TopViewControl
			// 
			this.TopViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TopViewControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TopViewControl.Location = new System.Drawing.Point(3, 3);
			this.TopViewControl.Name = "TopViewControl";
			this.TopViewControl.Size = new System.Drawing.Size(618, 423);
			this.TopViewControl.TabIndex = 0;
			this.TopViewControl.Tag = "TOPROUTE";
			// 
			// RouteViewControl
			// 
			this.RouteViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RouteViewControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RouteViewControl.Location = new System.Drawing.Point(3, 3);
			this.RouteViewControl.Name = "RouteViewControl";
			this.RouteViewControl.Size = new System.Drawing.Size(618, 423);
			this.RouteViewControl.TabIndex = 0;
			this.RouteViewControl.Tag = "TOPROUTE";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tp_Top);
			this.tabControl.Controls.Add(this.tp_Route);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(632, 454);
			this.tabControl.TabIndex = 1;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.OnActivated);
			// 
			// tp_Top
			// 
			this.tp_Top.Controls.Add(this.TopViewControl);
			this.tp_Top.Location = new System.Drawing.Point(4, 21);
			this.tp_Top.Name = "tp_Top";
			this.tp_Top.Padding = new System.Windows.Forms.Padding(3);
			this.tp_Top.Size = new System.Drawing.Size(624, 429);
			this.tp_Top.TabIndex = 0;
			this.tp_Top.Text = "TopView";
			// 
			// tp_Route
			// 
			this.tp_Route.Controls.Add(this.RouteViewControl);
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
			this.Text = "Top/Route Views";
			this.Activated += new System.EventHandler(this.OnActivated);
			this.tabControl.ResumeLayout(false);
			this.tp_Top.ResumeLayout(false);
			this.tp_Route.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private TopView TopViewControl;
		private RouteView RouteViewControl;

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tp_Top;
		private System.Windows.Forms.TabPage tp_Route;
	}
}
