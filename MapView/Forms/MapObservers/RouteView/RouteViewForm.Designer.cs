namespace MapView.Forms.MapObservers.RouteViews
{
	partial class RouteViewForm
	{
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

		/* This may get deleted:

			this.RouteViewControl = new MapView.Forms.MapObservers.RouteViews.RouteView();

		add it to the top of InitializeComponent() */

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.RouteViewControl = new MapView.Forms.MapObservers.RouteViews.RouteView();
			this.SuspendLayout();
			// 
			// RouteViewControl
			// 
			this.RouteViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RouteViewControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RouteViewControl.Location = new System.Drawing.Point(0, 0);
			this.RouteViewControl.Name = "RouteViewControl";
			this.RouteViewControl.Size = new System.Drawing.Size(632, 454);
			this.RouteViewControl.TabIndex = 0;
			this.RouteViewControl.Tag = "ROUTE";
			// 
			// RouteViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.RouteViewControl);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.KeyPreview = true;
			this.Name = "RouteViewForm";
			this.ShowInTaskbar = false;
			this.Text = "RouteView";
			this.ResumeLayout(false);

		}
		#endregion

		private RouteView RouteViewControl;
	}
}
