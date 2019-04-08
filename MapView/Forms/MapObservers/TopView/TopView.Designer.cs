namespace MapView.Forms.MapObservers.TopViews
{
	partial class TopView
	{
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

		// The #develop designer is going to delete this. Copy it back in at the
		// top of InitializeComponent().
		/*
			this.quadrants = new MapView.Forms.MapObservers.TopViews.QuadrantPanel();
		*/

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.quadrants = new MapView.Forms.MapObservers.TopViews.QuadrantPanel();
			this.tscPanel = new System.Windows.Forms.ToolStripContainer();
			this.pMain = new System.Windows.Forms.Panel();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsddbVisibleQuads = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsb_Options = new System.Windows.Forms.ToolStripButton();
			this.tscPanel.ContentPanel.SuspendLayout();
			this.tscPanel.LeftToolStripPanel.SuspendLayout();
			this.tscPanel.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// quadrants
			// 
			this.quadrants.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.quadrants.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.quadrants.Location = new System.Drawing.Point(0, 410);
			this.quadrants.Name = "quadrants";
			this.quadrants.Size = new System.Drawing.Size(640, 70);
			this.quadrants.TabIndex = 2;
			this.quadrants.TabStop = false;
			this.quadrants.Text = "bottom";
			// 
			// tscPanel
			// 
			// 
			// tscPanel.BottomToolStripPanel
			// 
			this.tscPanel.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// tscPanel.ContentPanel
			// 
			this.tscPanel.ContentPanel.Controls.Add(this.pMain);
			this.tscPanel.ContentPanel.Size = new System.Drawing.Size(615, 360);
			this.tscPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tscPanel.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// tscPanel.LeftToolStripPanel
			// 
			this.tscPanel.LeftToolStripPanel.Controls.Add(this.tsTools);
			this.tscPanel.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Location = new System.Drawing.Point(0, 25);
			this.tscPanel.Name = "tscPanel";
			// 
			// tscPanel.RightToolStripPanel
			// 
			this.tscPanel.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Size = new System.Drawing.Size(640, 385);
			this.tscPanel.TabIndex = 1;
			// 
			// tscPanel.TopToolStripPanel
			// 
			this.tscPanel.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// pMain
			// 
			this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pMain.Location = new System.Drawing.Point(0, 0);
			this.pMain.Name = "pMain";
			this.pMain.Size = new System.Drawing.Size(615, 360);
			this.pMain.TabIndex = 0;
			// 
			// tsTools
			// 
			this.tsTools.Dock = System.Windows.Forms.DockStyle.None;
			this.tsTools.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsTools.Location = new System.Drawing.Point(0, 3);
			this.tsTools.Name = "tsTools";
			this.tsTools.Padding = new System.Windows.Forms.Padding(0);
			this.tsTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsTools.Size = new System.Drawing.Size(25, 111);
			this.tsTools.TabIndex = 0;
			this.tsTools.Text = "tsTools";
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbVisibleQuads,
			this.tsb_Options});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 0;
			this.tsMain.TabStop = true;
			this.tsMain.Text = "tsMain";
			// 
			// tsddbVisibleQuads
			// 
			this.tsddbVisibleQuads.AutoToolTip = false;
			this.tsddbVisibleQuads.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbVisibleQuads.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbVisibleQuads.Margin = new System.Windows.Forms.Padding(3, 1, 0, 1);
			this.tsddbVisibleQuads.Name = "tsddbVisibleQuads";
			this.tsddbVisibleQuads.Size = new System.Drawing.Size(54, 23);
			this.tsddbVisibleQuads.Text = "Visible";
			this.tsddbVisibleQuads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsb_Options
			// 
			this.tsb_Options.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.tsb_Options.AutoToolTip = false;
			this.tsb_Options.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsb_Options.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_Options.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this.tsb_Options.Name = "tsb_Options";
			this.tsb_Options.Size = new System.Drawing.Size(52, 23);
			this.tsb_Options.Text = "Options";
			this.tsb_Options.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// TopView
			// 
			this.Controls.Add(this.tscPanel);
			this.Controls.Add(this.quadrants);
			this.Controls.Add(this.tsMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "TopView";
			this.Size = new System.Drawing.Size(640, 480);
			this.tscPanel.ContentPanel.ResumeLayout(false);
			this.tscPanel.LeftToolStripPanel.ResumeLayout(false);
			this.tscPanel.LeftToolStripPanel.PerformLayout();
			this.tscPanel.ResumeLayout(false);
			this.tscPanel.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.ComponentModel.IContainer components = null;

		private QuadrantPanel quadrants;
		private System.Windows.Forms.ToolStrip tsTools;
		private System.Windows.Forms.Panel pMain;
		private System.Windows.Forms.ToolStripContainer tscPanel;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripDropDownButton tsddbVisibleQuads;
		private System.Windows.Forms.ToolStripButton tsb_Options;
	}
}
