using System;
using System.Windows.Forms;


namespace MapView.Forms.Observers
{
	internal sealed partial class TopView
	{
		#region Designer
		private ToolStrip tsTools;
		private Panel pnlMain;
		private ToolStripContainer tscPanel;
		private ToolStrip tsMain;
		private ToolStripDropDownButton tsddbVisibleQuads;
		private ToolStripButton tsb_Options;
		private ToolStripDropDownButton tsddbTest;
		private ToolStripMenuItem tsddbTestPartslots;


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tscPanel = new System.Windows.Forms.ToolStripContainer();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsddbVisibleQuads = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsddbTest = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsddbTestPartslots = new System.Windows.Forms.ToolStripMenuItem();
			this.tsb_Options = new System.Windows.Forms.ToolStripButton();
			this.tscPanel.ContentPanel.SuspendLayout();
			this.tscPanel.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
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
			this.tscPanel.ContentPanel.Controls.Add(this.pnlMain);
			this.tscPanel.ContentPanel.Size = new System.Drawing.Size(640, 430);
			this.tscPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// tscPanel.LeftToolStripPanel
			// 
			this.tscPanel.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Location = new System.Drawing.Point(0, 25);
			this.tscPanel.Name = "tscPanel";
			// 
			// tscPanel.RightToolStripPanel
			// 
			this.tscPanel.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tscPanel.Size = new System.Drawing.Size(640, 455);
			this.tscPanel.TabIndex = 1;
			// 
			// tscPanel.TopToolStripPanel
			// 
			this.tscPanel.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// pnlMain
			// 
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(640, 430);
			this.pnlMain.TabIndex = 0;
			// 
			// tsTools
			// 
			this.tsTools.Dock = System.Windows.Forms.DockStyle.None;
			this.tsTools.Location = new System.Drawing.Point(0, 3);
			this.tsTools.Name = "tsTools";
			this.tsTools.Padding = new System.Windows.Forms.Padding(0);
			this.tsTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsTools.Size = new System.Drawing.Size(25, 111);
			this.tsTools.Stretch = true;
			this.tsTools.TabIndex = 0;
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbVisibleQuads,
			this.tsddbTest,
			this.tsb_Options});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 0;
			this.tsMain.TabStop = true;
			// 
			// tsddbVisibleQuads
			// 
			this.tsddbVisibleQuads.AutoToolTip = false;
			this.tsddbVisibleQuads.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbVisibleQuads.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbVisibleQuads.Margin = new System.Windows.Forms.Padding(3, 1, 0, 1);
			this.tsddbVisibleQuads.Name = "tsddbVisibleQuads";
			this.tsddbVisibleQuads.Size = new System.Drawing.Size(54, 23);
			this.tsddbVisibleQuads.Text = "&Visible";
			this.tsddbVisibleQuads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsddbTest
			// 
			this.tsddbTest.AutoToolTip = false;
			this.tsddbTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbTest.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbTestPartslots});
			this.tsddbTest.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbTest.Name = "tsddbTest";
			this.tsddbTest.Size = new System.Drawing.Size(42, 22);
			this.tsddbTest.Text = "&Test";
			this.tsddbTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsddbTestPartslots
			// 
			this.tsddbTestPartslots.Name = "tsddbTestPartslots";
			this.tsddbTestPartslots.Size = new System.Drawing.Size(187, 22);
			this.tsddbTestPartslots.Text = "test parts in tileslots";
			this.tsddbTestPartslots.Click += new System.EventHandler(this.OnTestPartslotsClick);
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
			this.tsb_Options.Text = "&Options";
			this.tsb_Options.Click += new System.EventHandler(this.OnOptionsClick);
			// 
			// TopView
			// 
			this.Controls.Add(this.tscPanel);
			this.Controls.Add(this.tsMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "TopView";
			this.Size = new System.Drawing.Size(640, 480);
			this.tscPanel.ContentPanel.ResumeLayout(false);
			this.tscPanel.ResumeLayout(false);
			this.tscPanel.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}
