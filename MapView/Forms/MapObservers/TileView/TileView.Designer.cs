namespace MapView.Forms.MapObservers.TileViews
{
	partial class TileView
	{
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
			this.tcTileTypes = new System.Windows.Forms.TabControl();
			this.tpAll = new System.Windows.Forms.TabPage();
			this.tpFloors = new System.Windows.Forms.TabPage();
			this.tpWestwalls = new System.Windows.Forms.TabPage();
			this.tpNorthwalls = new System.Windows.Forms.TabPage();
			this.tpObjects = new System.Windows.Forms.TabPage();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.tsddbExternal = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiEditPck = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiEditMcd = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Sep0 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiVolutarMcdEditor = new System.Windows.Forms.ToolStripMenuItem();
			this.tsb_Options = new System.Windows.Forms.ToolStripButton();
			this.ssStatusbar = new System.Windows.Forms.StatusStrip();
			this.tsslTotal = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslOver = new System.Windows.Forms.ToolStripStatusLabel();
			this.tcTileTypes.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.ssStatusbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcTileTypes
			// 
			this.tcTileTypes.Controls.Add(this.tpAll);
			this.tcTileTypes.Controls.Add(this.tpFloors);
			this.tcTileTypes.Controls.Add(this.tpWestwalls);
			this.tcTileTypes.Controls.Add(this.tpNorthwalls);
			this.tcTileTypes.Controls.Add(this.tpObjects);
			this.tcTileTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcTileTypes.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tcTileTypes.Location = new System.Drawing.Point(0, 25);
			this.tcTileTypes.Name = "tcTileTypes";
			this.tcTileTypes.SelectedIndex = 0;
			this.tcTileTypes.Size = new System.Drawing.Size(640, 433);
			this.tcTileTypes.TabIndex = 0;
			// 
			// tpAll
			// 
			this.tpAll.Location = new System.Drawing.Point(4, 21);
			this.tpAll.Name = "tpAll";
			this.tpAll.Size = new System.Drawing.Size(632, 408);
			this.tpAll.TabIndex = 0;
			this.tpAll.Text = "ALL";
			// 
			// tpFloors
			// 
			this.tpFloors.Location = new System.Drawing.Point(4, 21);
			this.tpFloors.Name = "tpFloors";
			this.tpFloors.Size = new System.Drawing.Size(632, 408);
			this.tpFloors.TabIndex = 1;
			this.tpFloors.Text = "fLoOr";
			// 
			// tpWestwalls
			// 
			this.tpWestwalls.Location = new System.Drawing.Point(4, 21);
			this.tpWestwalls.Name = "tpWestwalls";
			this.tpWestwalls.Size = new System.Drawing.Size(632, 408);
			this.tpWestwalls.TabIndex = 2;
			this.tpWestwalls.Text = "WEst";
			// 
			// tpNorthwalls
			// 
			this.tpNorthwalls.Location = new System.Drawing.Point(4, 21);
			this.tpNorthwalls.Name = "tpNorthwalls";
			this.tpNorthwalls.Size = new System.Drawing.Size(632, 408);
			this.tpNorthwalls.TabIndex = 3;
			this.tpNorthwalls.Text = "noRtH";
			// 
			// tpObjects
			// 
			this.tpObjects.Location = new System.Drawing.Point(4, 21);
			this.tpObjects.Name = "tpObjects";
			this.tpObjects.Size = new System.Drawing.Size(632, 408);
			this.tpObjects.TabIndex = 4;
			this.tpObjects.Text = "ConTeNt";
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbExternal,
			this.tsb_Options});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 1;
			this.tsMain.Text = "tsMain";
			// 
			// tsddbExternal
			// 
			this.tsddbExternal.AutoToolTip = false;
			this.tsddbExternal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddbExternal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiEditPck,
			this.tsmiEditMcd,
			this.tsmi_Sep0,
			this.tsmiVolutarMcdEditor});
			this.tsddbExternal.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsddbExternal.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbExternal.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
			this.tsddbExternal.Name = "tsddbExternal";
			this.tsddbExternal.Size = new System.Drawing.Size(63, 23);
			this.tsddbExternal.Text = "External";
			// 
			// tsmiEditPck
			// 
			this.tsmiEditPck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmiEditPck.Name = "tsmiEditPck";
			this.tsmiEditPck.Size = new System.Drawing.Size(173, 22);
			this.tsmiEditPck.Text = "open in PckView";
			this.tsmiEditPck.Click += new System.EventHandler(this.OnPckEditClick);
			// 
			// tsmiEditMcd
			// 
			this.tsmiEditMcd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmiEditMcd.Name = "tsmiEditMcd";
			this.tsmiEditMcd.Size = new System.Drawing.Size(173, 22);
			this.tsmiEditMcd.Text = "open in McdView";
			this.tsmiEditMcd.Click += new System.EventHandler(this.OnMcdEditClick);
			// 
			// tsmi_Sep0
			// 
			this.tsmi_Sep0.Name = "tsmi_Sep0";
			this.tsmi_Sep0.Size = new System.Drawing.Size(170, 6);
			// 
			// tsmiVolutarMcdEditor
			// 
			this.tsmiVolutarMcdEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmiVolutarMcdEditor.Name = "tsmiVolutarMcdEditor";
			this.tsmiVolutarMcdEditor.Size = new System.Drawing.Size(173, 22);
			this.tsmiVolutarMcdEditor.Text = "Volutar MCD Editor";
			this.tsmiVolutarMcdEditor.Click += new System.EventHandler(this.OnVolutarMcdEditorClick);
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
			// ssStatusbar
			// 
			this.ssStatusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsslTotal,
			this.tsslOver});
			this.ssStatusbar.Location = new System.Drawing.Point(0, 458);
			this.ssStatusbar.Name = "ssStatusbar";
			this.ssStatusbar.Size = new System.Drawing.Size(640, 22);
			this.ssStatusbar.TabIndex = 2;
			this.ssStatusbar.Text = "statusStrip1";
			// 
			// tsslTotal
			// 
			this.tsslTotal.AutoSize = false;
			this.tsslTotal.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tsslTotal.Name = "tsslTotal";
			this.tsslTotal.Size = new System.Drawing.Size(65, 22);
			this.tsslTotal.Text = "Total";
			this.tsslTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsslOver
			// 
			this.tsslOver.AutoSize = false;
			this.tsslOver.Margin = new System.Windows.Forms.Padding(0);
			this.tsslOver.Name = "tsslOver";
			this.tsslOver.Size = new System.Drawing.Size(555, 22);
			this.tsslOver.Spring = true;
			this.tsslOver.Text = "Over";
			this.tsslOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TileView
			// 
			this.Controls.Add(this.tcTileTypes);
			this.Controls.Add(this.ssStatusbar);
			this.Controls.Add(this.tsMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "TileView";
			this.Size = new System.Drawing.Size(640, 480);
			this.tcTileTypes.ResumeLayout(false);
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ssStatusbar.ResumeLayout(false);
			this.ssStatusbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripDropDownButton tsddbExternal;
		private System.Windows.Forms.ToolStripMenuItem tsmiEditPck;
		private System.Windows.Forms.ToolStripMenuItem tsmiVolutarMcdEditor;
		private System.Windows.Forms.TabControl tcTileTypes;
		private System.Windows.Forms.TabPage tpAll;
		private System.Windows.Forms.TabPage tpFloors;
		private System.Windows.Forms.TabPage tpObjects;
		private System.Windows.Forms.TabPage tpNorthwalls;
		private System.Windows.Forms.TabPage tpWestwalls;
		private System.Windows.Forms.StatusStrip ssStatusbar;
		private System.Windows.Forms.ToolStripStatusLabel tsslTotal;
		private System.Windows.Forms.ToolStripStatusLabel tsslOver;
		private System.Windows.Forms.ToolStripButton tsb_Options;
		private System.Windows.Forms.ToolStripMenuItem tsmiEditMcd;
		private System.Windows.Forms.ToolStripSeparator tsmi_Sep0;
	}
}
