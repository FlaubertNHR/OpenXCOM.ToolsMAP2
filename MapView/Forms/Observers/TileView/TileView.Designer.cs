﻿using System;
using System.Windows.Forms;

using DSShared.Controls;


namespace MapView.Forms.Observers
{
	internal sealed partial class TileView
	{
		#region Designer
		private CompositedTabControl tcPartTypes;

		private TabPage tpAll;
		private TabPage tpFloors;
		private TabPage tpWestwalls;
		private TabPage tpNorthwalls;
		private TabPage tpContents;

		private ToolStripOneclick tsMain;
		private ToolStripDropDownButton tsddbExternal;
		private ToolStripMenuItem tsmiEditPck;
		private ToolStripMenuItem tsmiEditMcd;
		private ToolStripSeparator tsmi_Sep0;
		private ToolStripMenuItem tsmiExternalProcess;
		private ToolStripButton tsb_Colorhelp;
		private ToolStripButton tsb_Options;

		private StatusStrip ssStatus;
		private ToolStripStatusLabel tsslTotal;
		private ToolStripStatusLabel tsslOver;


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tcPartTypes = new DSShared.Controls.CompositedTabControl();
			this.tpAll = new System.Windows.Forms.TabPage();
			this.tpFloors = new System.Windows.Forms.TabPage();
			this.tpWestwalls = new System.Windows.Forms.TabPage();
			this.tpNorthwalls = new System.Windows.Forms.TabPage();
			this.tpContents = new System.Windows.Forms.TabPage();
			this.tsMain = new DSShared.Controls.ToolStripOneclick();
			this.tsddbExternal = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiEditPck = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiEditMcd = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmi_Sep0 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiExternalProcess = new System.Windows.Forms.ToolStripMenuItem();
			this.tsb_Colorhelp = new System.Windows.Forms.ToolStripButton();
			this.tsb_Options = new System.Windows.Forms.ToolStripButton();
			this.ssStatus = new System.Windows.Forms.StatusStrip();
			this.tsslTotal = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsslOver = new System.Windows.Forms.ToolStripStatusLabel();
			this.tcPartTypes.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.ssStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcPartTypes
			// 
			this.tcPartTypes.Controls.Add(this.tpAll);
			this.tcPartTypes.Controls.Add(this.tpFloors);
			this.tcPartTypes.Controls.Add(this.tpWestwalls);
			this.tcPartTypes.Controls.Add(this.tpNorthwalls);
			this.tcPartTypes.Controls.Add(this.tpContents);
			this.tcPartTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcPartTypes.Location = new System.Drawing.Point(0, 25);
			this.tcPartTypes.Name = "tcPartTypes";
			this.tcPartTypes.SelectedIndex = 0;
			this.tcPartTypes.Size = new System.Drawing.Size(640, 433);
			this.tcPartTypes.TabIndex = 0;
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
			this.tpFloors.Location = new System.Drawing.Point(4, 22);
			this.tpFloors.Name = "tpFloors";
			this.tpFloors.Size = new System.Drawing.Size(632, 407);
			this.tpFloors.TabIndex = 1;
			this.tpFloors.Text = "floor";
			// 
			// tpWestwalls
			// 
			this.tpWestwalls.Location = new System.Drawing.Point(4, 22);
			this.tpWestwalls.Name = "tpWestwalls";
			this.tpWestwalls.Size = new System.Drawing.Size(632, 407);
			this.tpWestwalls.TabIndex = 2;
			this.tpWestwalls.Text = "west";
			// 
			// tpNorthwalls
			// 
			this.tpNorthwalls.Location = new System.Drawing.Point(4, 22);
			this.tpNorthwalls.Name = "tpNorthwalls";
			this.tpNorthwalls.Size = new System.Drawing.Size(632, 407);
			this.tpNorthwalls.TabIndex = 3;
			this.tpNorthwalls.Text = "north";
			// 
			// tpContents
			// 
			this.tpContents.Location = new System.Drawing.Point(4, 22);
			this.tpContents.Name = "tpContents";
			this.tpContents.Size = new System.Drawing.Size(632, 407);
			this.tpContents.TabIndex = 4;
			this.tpContents.Text = "content";
			// 
			// tsMain
			// 
			this.tsMain.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddbExternal,
			this.tsb_Colorhelp,
			this.tsb_Options});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(640, 25);
			this.tsMain.TabIndex = 1;
			this.tsMain.TabStop = true;
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
			this.tsmiExternalProcess});
			this.tsddbExternal.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddbExternal.Margin = new System.Windows.Forms.Padding(3, 1, 0, 1);
			this.tsddbExternal.Name = "tsddbExternal";
			this.tsddbExternal.Size = new System.Drawing.Size(63, 23);
			this.tsddbExternal.Text = "&External";
			this.tsddbExternal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsmiEditPck
			// 
			this.tsmiEditPck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmiEditPck.Name = "tsmiEditPck";
			this.tsmiEditPck.Size = new System.Drawing.Size(173, 22);
			this.tsmiEditPck.Text = "open in &PckView";
			this.tsmiEditPck.Click += new System.EventHandler(this.OnPckViewClick);
			// 
			// tsmiEditMcd
			// 
			this.tsmiEditMcd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmiEditMcd.Name = "tsmiEditMcd";
			this.tsmiEditMcd.Size = new System.Drawing.Size(173, 22);
			this.tsmiEditMcd.Text = "open in &McdView";
			this.tsmiEditMcd.Click += new System.EventHandler(this.OnMcdViewClick);
			// 
			// tsmi_Sep0
			// 
			this.tsmi_Sep0.Name = "tsmi_Sep0";
			this.tsmi_Sep0.Size = new System.Drawing.Size(170, 6);
			// 
			// tsmiExternalProcess
			// 
			this.tsmiExternalProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsmiExternalProcess.Name = "tsmiExternalProcess";
			this.tsmiExternalProcess.Size = new System.Drawing.Size(173, 22);
			this.tsmiExternalProcess.Text = "e&xternal process ...";
			this.tsmiExternalProcess.Click += new System.EventHandler(this.OnExternalProcessClick);
			// 
			// tsb_Colorhelp
			// 
			this.tsb_Colorhelp.AutoToolTip = false;
			this.tsb_Colorhelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsb_Colorhelp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_Colorhelp.Margin = new System.Windows.Forms.Padding(0);
			this.tsb_Colorhelp.Name = "tsb_Colorhelp";
			this.tsb_Colorhelp.Size = new System.Drawing.Size(62, 25);
			this.tsb_Colorhelp.Text = "&Colorhelp";
			this.tsb_Colorhelp.Click += new System.EventHandler(this.OnColorhelpClick);
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
			// ssStatus
			// 
			this.ssStatus.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsslTotal,
			this.tsslOver});
			this.ssStatus.Location = new System.Drawing.Point(0, 458);
			this.ssStatus.Name = "ssStatus";
			this.ssStatus.Size = new System.Drawing.Size(640, 22);
			this.ssStatus.TabIndex = 2;
			// 
			// tsslTotal
			// 
			this.tsslTotal.AutoSize = false;
			this.tsslTotal.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.tsslTotal.Margin = new System.Windows.Forms.Padding(4, 3, 0, 2);
			this.tsslTotal.Name = "tsslTotal";
			this.tsslTotal.Size = new System.Drawing.Size(65, 17);
			this.tsslTotal.Text = "Total";
			this.tsslTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tsslOver
			// 
			this.tsslOver.AutoSize = false;
			this.tsslOver.Margin = new System.Windows.Forms.Padding(6, 3, 0, 2);
			this.tsslOver.Name = "tsslOver";
			this.tsslOver.Size = new System.Drawing.Size(550, 17);
			this.tsslOver.Spring = true;
			this.tsslOver.Text = "{over}";
			this.tsslOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TileView
			// 
			this.Controls.Add(this.tcPartTypes);
			this.Controls.Add(this.ssStatus);
			this.Controls.Add(this.tsMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "TileView";
			this.Size = new System.Drawing.Size(640, 480);
			this.tcPartTypes.ResumeLayout(false);
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ssStatus.ResumeLayout(false);
			this.ssStatus.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
