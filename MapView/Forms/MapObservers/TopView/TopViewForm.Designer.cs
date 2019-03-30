namespace MapView.Forms.MapObservers.TopViews
{
	partial class TopViewForm
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

			TopViewControl = new MapView.Forms.MapObservers.TopViews.TopView();

		- so copy it back into InitializeComponent() */

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			TopViewControl = new MapView.Forms.MapObservers.TopViews.TopView();
			this.SuspendLayout();
			// 
			// TopViewControl
			// 
			this.TopViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TopViewControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TopViewControl.Location = new System.Drawing.Point(0, 0);
			this.TopViewControl.Name = "TopViewControl";
			this.TopViewControl.Size = new System.Drawing.Size(632, 454);
			this.TopViewControl.TabIndex = 1;
			this.TopViewControl.Tag = "TOP";
			// 
			// TopViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.TopViewControl);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.KeyPreview = true;
			this.Name = "TopViewForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "TopView";
			this.ResumeLayout(false);

		}

		#endregion

		private TopView TopViewControl;
	}
}
