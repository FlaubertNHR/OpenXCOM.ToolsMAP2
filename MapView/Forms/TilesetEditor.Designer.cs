﻿using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	internal sealed partial class TilesetEditor
	{
		#region Designer
		private IContainer components;

		private Panel pnl_Top;
		private GroupBox gb_Maptree;
		private GroupBox gb_Tileset;
        private GroupBox gb_Terrains;
        private Panel pnl_Bottom;
		private Button btn_Accept;
		private Button btn_Cancel;
		private Label lbl_Group_;
		private TextBox tb_TilesetCurrent;
		private Label lbl_CategoryCurrent;
		private Label lbl_GroupCurrent;
		private Label lbl_Category_;
		private Button btn_FindTileset;
        private RadioButton rb_MAP;
        private RadioButton rb_MAP2;
        private Label lbl_TilesetBasepath;
		private Label lbl_TilesetBasepath_;
        private ListBox lb_TerrainsAvailable;
        private ListBox lb_TerrainsAllocated;
        private Button btn_MoveDown;
        private Button btn_MoveUp;
        private Button btn_MoveRight;
        private Button btn_MoveLeft;
        private Panel pnl_Spacer;
        private Button btn_FindDirectory;
		private Button btn_CreateDescriptor;
		private ToolTip toolTip1;
        private Label lbl_Available;
        private Label lbl_Allocated;
        private Panel pnl_TerrainsTop;
        private Label lbl_TilesetCurrent;
		private Label lbl_AddType;
        private Label lbl_TerrainChanges;
        private Button btn_TerrainCopy;
        private Button btn_TerrainPaste;
        private Label lbl_McdRecords;
        private TextBox tb_PathAvailable;
        private RadioButton rb_CustomBasepath;
        private RadioButton rb_TilesetBasepath;
        private RadioButton rb_ConfigBasepath;
        private Button btn_FindBasepath;
        private Label lbl_PathAllocated_;
        private Label lbl_PathAllocated;
        private Label lbl_PathAvailable_;
        private Button btn_TerrainClear;
        private Label lbl_ListAvailable_;
        private Label lbl_TilesetCount_;
        private Label lbl_TilesetCount;
        private CheckBox cb_BypassRecordsExceeded;
        private Button btn_GlobalTerrains;
		private Button btn_GlobalTerrainsList;


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
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.gb_Terrains = new System.Windows.Forms.GroupBox();
            this.lb_TerrainsAllocated = new System.Windows.Forms.ListBox();
            this.pnl_Spacer = new System.Windows.Forms.Panel();
            this.btn_TerrainCopy = new System.Windows.Forms.Button();
            this.btn_TerrainPaste = new System.Windows.Forms.Button();
            this.btn_TerrainClear = new System.Windows.Forms.Button();
            this.btn_MoveLeft = new System.Windows.Forms.Button();
            this.btn_MoveRight = new System.Windows.Forms.Button();
            this.btn_MoveUp = new System.Windows.Forms.Button();
            this.btn_MoveDown = new System.Windows.Forms.Button();
            this.cb_BypassRecordsExceeded = new System.Windows.Forms.CheckBox();
            this.lb_TerrainsAvailable = new System.Windows.Forms.ListBox();
            this.pnl_TerrainsTop = new System.Windows.Forms.Panel();
            this.lbl_TerrainChanges = new System.Windows.Forms.Label();
            this.lbl_ListAvailable_ = new System.Windows.Forms.Label();
            this.rb_ConfigBasepath = new System.Windows.Forms.RadioButton();
            this.rb_TilesetBasepath = new System.Windows.Forms.RadioButton();
            this.rb_CustomBasepath = new System.Windows.Forms.RadioButton();
            this.lbl_PathAvailable_ = new System.Windows.Forms.Label();
            this.tb_PathAvailable = new System.Windows.Forms.TextBox();
            this.btn_FindBasepath = new System.Windows.Forms.Button();
            this.lbl_PathAllocated = new System.Windows.Forms.Label();
            this.lbl_PathAllocated_ = new System.Windows.Forms.Label();
            this.lbl_Allocated = new System.Windows.Forms.Label();
            this.lbl_Available = new System.Windows.Forms.Label();
            this.gb_Tileset = new System.Windows.Forms.GroupBox();
            this.rb_MAP2 = new System.Windows.Forms.RadioButton();
            this.rb_MAP = new System.Windows.Forms.RadioButton();
            this.lbl_TilesetBasepath_ = new System.Windows.Forms.Label();
            this.lbl_TilesetBasepath = new System.Windows.Forms.Label();
            this.btn_FindDirectory = new System.Windows.Forms.Button();
            this.tb_TilesetCurrent = new System.Windows.Forms.TextBox();
            this.btn_CreateDescriptor = new System.Windows.Forms.Button();
            this.btn_FindTileset = new System.Windows.Forms.Button();
            this.lbl_TilesetCount = new System.Windows.Forms.Label();
            this.lbl_TilesetCount_ = new System.Windows.Forms.Label();
            this.lbl_AddType = new System.Windows.Forms.Label();
            this.btn_Accept = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.gb_Maptree = new System.Windows.Forms.GroupBox();
            this.lbl_Group_ = new System.Windows.Forms.Label();
            this.lbl_GroupCurrent = new System.Windows.Forms.Label();
            this.lbl_Category_ = new System.Windows.Forms.Label();
            this.lbl_CategoryCurrent = new System.Windows.Forms.Label();
            this.lbl_TilesetCurrent = new System.Windows.Forms.Label();
            this.lbl_McdRecords = new System.Windows.Forms.Label();
            this.pnl_Bottom = new System.Windows.Forms.Panel();
            this.btn_GlobalTerrainsList = new System.Windows.Forms.Button();
            this.btn_GlobalTerrains = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnl_Top.SuspendLayout();
            this.gb_Terrains.SuspendLayout();
            this.pnl_Spacer.SuspendLayout();
            this.pnl_TerrainsTop.SuspendLayout();
            this.gb_Tileset.SuspendLayout();
            this.gb_Maptree.SuspendLayout();
            this.pnl_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_Top
            // 
            this.pnl_Top.Controls.Add(this.gb_Terrains);
            this.pnl_Top.Controls.Add(this.gb_Tileset);
            this.pnl_Top.Controls.Add(this.gb_Maptree);
            this.pnl_Top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Top.Location = new System.Drawing.Point(0, 0);
            this.pnl_Top.Name = "pnl_Top";
            this.pnl_Top.Size = new System.Drawing.Size(833, 612);
            this.pnl_Top.TabIndex = 0;
            // 
            // gb_Terrains
            // 
            this.gb_Terrains.Controls.Add(this.lb_TerrainsAllocated);
            this.gb_Terrains.Controls.Add(this.pnl_Spacer);
            this.gb_Terrains.Controls.Add(this.cb_BypassRecordsExceeded);
            this.gb_Terrains.Controls.Add(this.lb_TerrainsAvailable);
            this.gb_Terrains.Controls.Add(this.pnl_TerrainsTop);
            this.gb_Terrains.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_Terrains.Location = new System.Drawing.Point(0, 199);
            this.gb_Terrains.Margin = new System.Windows.Forms.Padding(0);
            this.gb_Terrains.Name = "gb_Terrains";
            this.gb_Terrains.Size = new System.Drawing.Size(833, 413);
            this.gb_Terrains.TabIndex = 2;
            this.gb_Terrains.TabStop = false;
            this.gb_Terrains.Text = " Terrainset ";
            // 
            // lb_TerrainsAllocated
            // 
            this.lb_TerrainsAllocated.Dock = System.Windows.Forms.DockStyle.Left;
            this.lb_TerrainsAllocated.FormattingEnabled = true;
            this.lb_TerrainsAllocated.ItemHeight = 14;
            this.lb_TerrainsAllocated.Location = new System.Drawing.Point(3, 137);
            this.lb_TerrainsAllocated.Margin = new System.Windows.Forms.Padding(0);
            this.lb_TerrainsAllocated.Name = "lb_TerrainsAllocated";
            this.lb_TerrainsAllocated.Size = new System.Drawing.Size(374, 252);
            this.lb_TerrainsAllocated.TabIndex = 0;
            this.lb_TerrainsAllocated.SelectedIndexChanged += new System.EventHandler(this.OnAllocatedIndexChanged);
            // 
            // pnl_Spacer
            // 
            this.pnl_Spacer.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pnl_Spacer.Controls.Add(this.btn_TerrainCopy);
            this.pnl_Spacer.Controls.Add(this.btn_TerrainPaste);
            this.pnl_Spacer.Controls.Add(this.btn_TerrainClear);
            this.pnl_Spacer.Controls.Add(this.btn_MoveLeft);
            this.pnl_Spacer.Controls.Add(this.btn_MoveRight);
            this.pnl_Spacer.Controls.Add(this.btn_MoveUp);
            this.pnl_Spacer.Controls.Add(this.btn_MoveDown);
            this.pnl_Spacer.Location = new System.Drawing.Point(380, 138);
            this.pnl_Spacer.Margin = new System.Windows.Forms.Padding(0);
            this.pnl_Spacer.Name = "pnl_Spacer";
            this.pnl_Spacer.Size = new System.Drawing.Size(77, 237);
            this.pnl_Spacer.TabIndex = 2;
            // 
            // btn_TerrainCopy
            // 
            this.btn_TerrainCopy.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_TerrainCopy.Location = new System.Drawing.Point(7, 138);
            this.btn_TerrainCopy.Margin = new System.Windows.Forms.Padding(0);
            this.btn_TerrainCopy.Name = "btn_TerrainCopy";
            this.btn_TerrainCopy.Size = new System.Drawing.Size(63, 31);
            this.btn_TerrainCopy.TabIndex = 4;
            this.btn_TerrainCopy.Text = "Copy";
            this.btn_TerrainCopy.UseVisualStyleBackColor = true;
            this.btn_TerrainCopy.Click += new System.EventHandler(this.OnTerrainCopyClick);
            // 
            // btn_TerrainPaste
            // 
            this.btn_TerrainPaste.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_TerrainPaste.Location = new System.Drawing.Point(7, 169);
            this.btn_TerrainPaste.Margin = new System.Windows.Forms.Padding(0);
            this.btn_TerrainPaste.Name = "btn_TerrainPaste";
            this.btn_TerrainPaste.Size = new System.Drawing.Size(63, 31);
            this.btn_TerrainPaste.TabIndex = 5;
            this.btn_TerrainPaste.Text = "Paste";
            this.btn_TerrainPaste.UseVisualStyleBackColor = true;
            this.btn_TerrainPaste.Click += new System.EventHandler(this.OnTerrainPasteClick);
            // 
            // btn_TerrainClear
            // 
            this.btn_TerrainClear.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_TerrainClear.Location = new System.Drawing.Point(7, 206);
            this.btn_TerrainClear.Margin = new System.Windows.Forms.Padding(0);
            this.btn_TerrainClear.Name = "btn_TerrainClear";
            this.btn_TerrainClear.Size = new System.Drawing.Size(63, 32);
            this.btn_TerrainClear.TabIndex = 6;
            this.btn_TerrainClear.Text = "Clear";
            this.btn_TerrainClear.UseVisualStyleBackColor = true;
            this.btn_TerrainClear.Click += new System.EventHandler(this.OnTerrainClearClick);
            //
            // btn_MoveLeft
            // 
            this.btn_MoveLeft.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_MoveLeft.Enabled = false;
            this.btn_MoveLeft.Location = new System.Drawing.Point(7, 0);
            this.btn_MoveLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btn_MoveLeft.Name = "btn_MoveLeft";
            this.btn_MoveLeft.Size = new System.Drawing.Size(63, 31);
            this.btn_MoveLeft.TabIndex = 0;
            this.btn_MoveLeft.Text = "Left";
            this.btn_MoveLeft.UseVisualStyleBackColor = true;
            this.btn_MoveLeft.Click += new System.EventHandler(this.OnTerrainLeftClick);
            // 
            // btn_MoveRight
            // 
            this.btn_MoveRight.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_MoveRight.Enabled = false;
            this.btn_MoveRight.Location = new System.Drawing.Point(7, 31);
            this.btn_MoveRight.Margin = new System.Windows.Forms.Padding(0);
            this.btn_MoveRight.Name = "btn_MoveRight";
            this.btn_MoveRight.Size = new System.Drawing.Size(63, 31);
            this.btn_MoveRight.TabIndex = 1;
            this.btn_MoveRight.Text = "Right";
            this.btn_MoveRight.UseVisualStyleBackColor = true;
            this.btn_MoveRight.Click += new System.EventHandler(this.OnTerrainRightClick);
            // 
            // btn_MoveUp
            // 
            this.btn_MoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_MoveUp.Enabled = false;
            this.btn_MoveUp.Location = new System.Drawing.Point(7, 69);
            this.btn_MoveUp.Margin = new System.Windows.Forms.Padding(0);
            this.btn_MoveUp.Name = "btn_MoveUp";
            this.btn_MoveUp.Size = new System.Drawing.Size(63, 31);
            this.btn_MoveUp.TabIndex = 2;
            this.btn_MoveUp.Text = "Up";
            this.btn_MoveUp.UseVisualStyleBackColor = true;
            this.btn_MoveUp.Click += new System.EventHandler(this.OnTerrainUpClick);
            // 
            // btn_MoveDown
            // 
            this.btn_MoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_MoveDown.Enabled = false;
            this.btn_MoveDown.Location = new System.Drawing.Point(7, 100);
            this.btn_MoveDown.Margin = new System.Windows.Forms.Padding(0);
            this.btn_MoveDown.Name = "btn_MoveDown";
            this.btn_MoveDown.Size = new System.Drawing.Size(63, 31);
            this.btn_MoveDown.TabIndex = 3;
            this.btn_MoveDown.Text = "Down";
            this.btn_MoveDown.UseVisualStyleBackColor = true;
            this.btn_MoveDown.Click += new System.EventHandler(this.OnTerrainDownClick);
            // 
            // cb_BypassRecordsExceeded
            // 
            this.cb_BypassRecordsExceeded.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cb_BypassRecordsExceeded.Location = new System.Drawing.Point(3, 389);
            this.cb_BypassRecordsExceeded.Margin = new System.Windows.Forms.Padding(0);
            this.cb_BypassRecordsExceeded.Name = "cb_BypassRecordsExceeded";
            this.cb_BypassRecordsExceeded.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.cb_BypassRecordsExceeded.Size = new System.Drawing.Size(827, 21);
            this.cb_BypassRecordsExceeded.TabIndex = 3;
            this.cb_BypassRecordsExceeded.Text = "bypass RecordsExceeded";
            this.cb_BypassRecordsExceeded.UseVisualStyleBackColor = true;
            this.cb_BypassRecordsExceeded.CheckedChanged += new System.EventHandler(this.OnBypassRecordsExceededCheckedChanged);
            // 
            // lb_TerrainsAvailable
            // 
            this.lb_TerrainsAvailable.Dock = System.Windows.Forms.DockStyle.Right;
            this.lb_TerrainsAvailable.FormattingEnabled = true;
            this.lb_TerrainsAvailable.ItemHeight = 14;
            this.lb_TerrainsAvailable.Location = new System.Drawing.Point(460, 137);
            this.lb_TerrainsAvailable.Margin = new System.Windows.Forms.Padding(0);
            this.lb_TerrainsAvailable.Name = "lb_TerrainsAvailable";
            this.lb_TerrainsAvailable.Size = new System.Drawing.Size(370, 252);
            this.lb_TerrainsAvailable.TabIndex = 1;
            this.lb_TerrainsAvailable.SelectedIndexChanged += new System.EventHandler(this.OnAvailableIndexChanged);
            // 
            // pnl_TerrainsTop
            // 
            this.pnl_TerrainsTop.Controls.Add(this.lbl_TerrainChanges);
            this.pnl_TerrainsTop.Controls.Add(this.lbl_ListAvailable_);
            this.pnl_TerrainsTop.Controls.Add(this.rb_ConfigBasepath);
            this.pnl_TerrainsTop.Controls.Add(this.rb_TilesetBasepath);
            this.pnl_TerrainsTop.Controls.Add(this.rb_CustomBasepath);
            this.pnl_TerrainsTop.Controls.Add(this.lbl_PathAvailable_);
            this.pnl_TerrainsTop.Controls.Add(this.tb_PathAvailable);
            this.pnl_TerrainsTop.Controls.Add(this.btn_FindBasepath);
            this.pnl_TerrainsTop.Controls.Add(this.lbl_PathAllocated);
            this.pnl_TerrainsTop.Controls.Add(this.lbl_PathAllocated_);
            this.pnl_TerrainsTop.Controls.Add(this.lbl_Allocated);
            this.pnl_TerrainsTop.Controls.Add(this.lbl_Available);
            this.pnl_TerrainsTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_TerrainsTop.Location = new System.Drawing.Point(3, 18);
            this.pnl_TerrainsTop.Name = "pnl_TerrainsTop";
            this.pnl_TerrainsTop.Size = new System.Drawing.Size(827, 119);
            this.pnl_TerrainsTop.TabIndex = 0;
            // 
            // lbl_TerrainChanges
            // 
            this.lbl_TerrainChanges.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_TerrainChanges.ForeColor = System.Drawing.Color.SlateBlue;
            this.lbl_TerrainChanges.Location = new System.Drawing.Point(0, 0);
            this.lbl_TerrainChanges.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_TerrainChanges.Name = "lbl_TerrainChanges";
            this.lbl_TerrainChanges.Size = new System.Drawing.Size(827, 33);
            this.lbl_TerrainChanges.TabIndex = 0;
            this.lbl_TerrainChanges.Text = "Changes to the terrainset take effect immediately.";
            this.lbl_TerrainChanges.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lbl_TerrainChanges.Click += new System.EventHandler(this.lbl_TerrainChanges_Click);
            // 
            // lbl_ListAvailable_
            // 
            this.lbl_ListAvailable_.Location = new System.Drawing.Point(6, 33);
            this.lbl_ListAvailable_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_ListAvailable_.Name = "lbl_ListAvailable_";
            this.lbl_ListAvailable_.Size = new System.Drawing.Size(161, 25);
            this.lbl_ListAvailable_.TabIndex = 1;
            this.lbl_ListAvailable_.Text = "Available terrains in";
            this.lbl_ListAvailable_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rb_ConfigBasepath
            // 
            this.rb_ConfigBasepath.Location = new System.Drawing.Point(220, 33);
            this.rb_ConfigBasepath.Margin = new System.Windows.Forms.Padding(0);
            this.rb_ConfigBasepath.Name = "rb_ConfigBasepath";
            this.rb_ConfigBasepath.Size = new System.Drawing.Size(224, 25);
            this.rb_ConfigBasepath.TabIndex = 2;
            this.rb_ConfigBasepath.TabStop = true;
            this.rb_ConfigBasepath.Text = "Configurator basepath |";
            this.rb_ConfigBasepath.UseVisualStyleBackColor = true;
            this.rb_ConfigBasepath.CheckedChanged += new System.EventHandler(this.OnRadioTerrainChanged);
            // 
            // rb_TilesetBasepath
            // 
            this.rb_TilesetBasepath.Location = new System.Drawing.Point(448, 33);
            this.rb_TilesetBasepath.Margin = new System.Windows.Forms.Padding(0);
            this.rb_TilesetBasepath.Name = "rb_TilesetBasepath";
            this.rb_TilesetBasepath.Size = new System.Drawing.Size(175, 25);
            this.rb_TilesetBasepath.TabIndex = 3;
            this.rb_TilesetBasepath.TabStop = true;
            this.rb_TilesetBasepath.Text = "Tileset basepath |";
            this.rb_TilesetBasepath.UseVisualStyleBackColor = true;
            this.rb_TilesetBasepath.CheckedChanged += new System.EventHandler(this.OnRadioTerrainChanged);
            // 
            // rb_CustomBasepath
            // 
            this.rb_CustomBasepath.Location = new System.Drawing.Point(623, 33);
            this.rb_CustomBasepath.Margin = new System.Windows.Forms.Padding(0);
            this.rb_CustomBasepath.Name = "rb_CustomBasepath";
            this.rb_CustomBasepath.Size = new System.Drawing.Size(175, 25);
            this.rb_CustomBasepath.TabIndex = 4;
            this.rb_CustomBasepath.TabStop = true;
            this.rb_CustomBasepath.Text = "Custom basepath";
            this.rb_CustomBasepath.UseVisualStyleBackColor = true;
            this.rb_CustomBasepath.CheckedChanged += new System.EventHandler(this.OnRadioTerrainChanged);
            // 
            // lbl_PathAvailable_
            // 
            this.lbl_PathAvailable_.Location = new System.Drawing.Point(4, 68);
            this.lbl_PathAvailable_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_PathAvailable_.Name = "lbl_PathAvailable_";
            this.lbl_PathAvailable_.Size = new System.Drawing.Size(196, 25);
            this.lbl_PathAvailable_.TabIndex = 5;
            this.lbl_PathAvailable_.Text = "Path (available terrains)";
            this.lbl_PathAvailable_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_PathAvailable
            // 
            this.tb_PathAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_PathAvailable.Location = new System.Drawing.Point(203, 69);
            this.tb_PathAvailable.Margin = new System.Windows.Forms.Padding(0);
            this.tb_PathAvailable.Name = "tb_PathAvailable";
            this.tb_PathAvailable.Size = new System.Drawing.Size(559, 22);
            this.tb_PathAvailable.TabIndex = 6;
            this.tb_PathAvailable.Text = "tb_PathAvailable";
            this.tb_PathAvailable.TextChanged += new System.EventHandler(this.OnTerrainPathChanged);
            // 
            // btn_FindBasepath
            // 
            this.btn_FindBasepath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_FindBasepath.Location = new System.Drawing.Point(783, 66);
            this.btn_FindBasepath.Margin = new System.Windows.Forms.Padding(0);
            this.btn_FindBasepath.Name = "btn_FindBasepath";
            this.btn_FindBasepath.Size = new System.Drawing.Size(35, 27);
            this.btn_FindBasepath.TabIndex = 7;
            this.btn_FindBasepath.Text = "...";
            this.btn_FindBasepath.UseVisualStyleBackColor = true;
            this.btn_FindBasepath.Visible = false;
            this.btn_FindBasepath.Click += new System.EventHandler(this.OnFindTerrainClick);
            // 
            // lbl_PathAllocated
            // 
            this.lbl_PathAllocated.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_PathAllocated.Location = new System.Drawing.Point(217, 81);
            this.lbl_PathAllocated.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_PathAllocated.Name = "lbl_PathAllocated";
            this.lbl_PathAllocated.Size = new System.Drawing.Size(601, 19);
            this.lbl_PathAllocated.TabIndex = 9;
            this.lbl_PathAllocated.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_PathAllocated_
            // 
            this.lbl_PathAllocated_.Location = new System.Drawing.Point(4, 98);
            this.lbl_PathAllocated_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_PathAllocated_.Name = "lbl_PathAllocated_";
            this.lbl_PathAllocated_.Size = new System.Drawing.Size(196, 19);
            this.lbl_PathAllocated_.TabIndex = 8;
            this.lbl_PathAllocated_.Text = "Path (allocated terrain)";
            this.lbl_PathAllocated_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Allocated
            // 
            this.lbl_Allocated.Location = new System.Drawing.Point(287, 100);
            this.lbl_Allocated.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_Allocated.Name = "lbl_Allocated";
            this.lbl_Allocated.Size = new System.Drawing.Size(77, 19);
            this.lbl_Allocated.TabIndex = 10;
            this.lbl_Allocated.Text = "allocated";
            this.lbl_Allocated.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbl_Available
            // 
            this.lbl_Available.Location = new System.Drawing.Point(462, 100);
            this.lbl_Available.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_Available.Name = "lbl_Available";
            this.lbl_Available.Size = new System.Drawing.Size(77, 19);
            this.lbl_Available.TabIndex = 11;
            this.lbl_Available.Text = "available";
            // 
            // gb_Tileset
            // 
            this.gb_Tileset.Controls.Add(this.lbl_TilesetBasepath_);
            this.gb_Tileset.Controls.Add(this.lbl_TilesetBasepath);
            this.gb_Tileset.Controls.Add(this.btn_FindDirectory);
            this.gb_Tileset.Controls.Add(this.rb_MAP2);
            this.gb_Tileset.Controls.Add(this.rb_MAP);
            this.gb_Tileset.Controls.Add(this.tb_TilesetCurrent);
            this.gb_Tileset.Controls.Add(this.btn_CreateDescriptor);
            this.gb_Tileset.Controls.Add(this.btn_FindTileset);
            this.gb_Tileset.Controls.Add(this.lbl_TilesetCount);
            this.gb_Tileset.Controls.Add(this.lbl_TilesetCount_);
            this.gb_Tileset.Controls.Add(this.lbl_AddType);
            this.gb_Tileset.Controls.Add(this.btn_Accept);
            this.gb_Tileset.Controls.Add(this.btn_Cancel);
            this.gb_Tileset.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb_Tileset.Location = new System.Drawing.Point(0, 62);
            this.gb_Tileset.Margin = new System.Windows.Forms.Padding(0);
            this.gb_Tileset.Name = "gb_Tileset";
            this.gb_Tileset.Size = new System.Drawing.Size(833, 137);
            this.gb_Tileset.TabIndex = 1;
            this.gb_Tileset.TabStop = false;
            this.gb_Tileset.Text = " Tileset ";
            this.gb_Tileset.Enter += new System.EventHandler(this.gb_Tileset_Enter);
            // 
            // rb_MAP2
            // 
            this.rb_MAP2.AutoSize = true;
            this.rb_MAP2.Location = new System.Drawing.Point(6, 63);
            this.rb_MAP2.Name = "rb_MAP2";
            this.rb_MAP2.Size = new System.Drawing.Size(62, 18);
            this.rb_MAP2.TabIndex = 13;
            this.rb_MAP2.Text = "MAP2";
            this.rb_MAP2.UseVisualStyleBackColor = true;
            this.rb_MAP2.CheckedChanged += new System.EventHandler(this.OnRadioFormatSelected);
            // 
            // rb_MAP
            // 
            this.rb_MAP.AutoSize = true;
            this.rb_MAP.Location = new System.Drawing.Point(7, 40);
            this.rb_MAP.Name = "rb_MAP";
            this.rb_MAP.Size = new System.Drawing.Size(54, 18);
            this.rb_MAP.TabIndex = 12;
            this.rb_MAP.Text = "MAP";
            this.rb_MAP.UseVisualStyleBackColor = true;
            this.rb_MAP.CheckedChanged += new System.EventHandler(this.OnRadioFormatSelected);
            // 
            // lbl_TilesetBasepath_
            // 
            this.lbl_TilesetBasepath_.Location = new System.Drawing.Point(14, 19);
            this.lbl_TilesetBasepath_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_TilesetBasepath_.Name = "lbl_TilesetBasepath_";
            this.lbl_TilesetBasepath_.Size = new System.Drawing.Size(84, 19);
            this.lbl_TilesetBasepath_.TabIndex = 0;
            this.lbl_TilesetBasepath_.Text = "Basepath";
            this.lbl_TilesetBasepath_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.lbl_TilesetBasepath_, "path to the .MAP file");
            // 
            // lbl_TilesetBasepath
            // 
            this.lbl_TilesetBasepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_TilesetBasepath.Location = new System.Drawing.Point(105, 19);
            this.lbl_TilesetBasepath.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_TilesetBasepath.Name = "lbl_TilesetBasepath";
            this.lbl_TilesetBasepath.Size = new System.Drawing.Size(683, 19);
            this.lbl_TilesetBasepath.TabIndex = 1;
            this.lbl_TilesetBasepath.Text = "lbl_TilesetBasepathCurrent";
            this.lbl_TilesetBasepath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_FindDirectory
            // 
            this.btn_FindDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_FindDirectory.Location = new System.Drawing.Point(788, 12);
            this.btn_FindDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.btn_FindDirectory.Name = "btn_FindDirectory";
            this.btn_FindDirectory.Size = new System.Drawing.Size(35, 28);
            this.btn_FindDirectory.TabIndex = 2;
            this.btn_FindDirectory.Text = "...";
            this.btn_FindDirectory.UseVisualStyleBackColor = true;
            this.btn_FindDirectory.Click += new System.EventHandler(this.OnFindDirectoryClick);
            // 
            // tb_TilesetCurrent
            // 
            this.tb_TilesetCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_TilesetCurrent.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tb_TilesetCurrent.Location = new System.Drawing.Point(105, 44);
            this.tb_TilesetCurrent.Margin = new System.Windows.Forms.Padding(0);
            this.tb_TilesetCurrent.Name = "tb_TilesetCurrent";
            this.tb_TilesetCurrent.Size = new System.Drawing.Size(599, 22);
            this.tb_TilesetCurrent.TabIndex = 4;
            this.tb_TilesetCurrent.TextChanged += new System.EventHandler(this.OnTilesetTextboxChanged);
            // 
            // btn_CreateDescriptor
            // 
            this.btn_CreateDescriptor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CreateDescriptor.Location = new System.Drawing.Point(711, 40);
            this.btn_CreateDescriptor.Margin = new System.Windows.Forms.Padding(0);
            this.btn_CreateDescriptor.Name = "btn_CreateDescriptor";
            this.btn_CreateDescriptor.Size = new System.Drawing.Size(70, 31);
            this.btn_CreateDescriptor.TabIndex = 5;
            this.btn_CreateDescriptor.Text = "create";
            this.toolTip1.SetToolTip(this.btn_CreateDescriptor, "a Map descriptor must be created before terrains can be added");
            this.btn_CreateDescriptor.UseVisualStyleBackColor = true;
            this.btn_CreateDescriptor.Click += new System.EventHandler(this.OnCreateDescriptorClick);
            // 
            // btn_FindTileset
            // 
            this.btn_FindTileset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_FindTileset.Location = new System.Drawing.Point(788, 40);
            this.btn_FindTileset.Margin = new System.Windows.Forms.Padding(0);
            this.btn_FindTileset.Name = "btn_FindTileset";
            this.btn_FindTileset.Size = new System.Drawing.Size(35, 31);
            this.btn_FindTileset.TabIndex = 6;
            this.btn_FindTileset.Text = "...";
            this.btn_FindTileset.UseVisualStyleBackColor = true;
            this.btn_FindTileset.Click += new System.EventHandler(this.OnFindTilesetClick);
            // 
            // lbl_TilesetCount
            // 
            this.lbl_TilesetCount.Location = new System.Drawing.Point(350, 84);
            this.lbl_TilesetCount.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_TilesetCount.Name = "lbl_TilesetCount";
            this.lbl_TilesetCount.Size = new System.Drawing.Size(133, 19);
            this.lbl_TilesetCount.TabIndex = 8;
            this.lbl_TilesetCount.Text = "lbl_TilesetCount";
            this.lbl_TilesetCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_TilesetCount_
            // 
            this.lbl_TilesetCount_.Location = new System.Drawing.Point(10, 84);
            this.lbl_TilesetCount_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_TilesetCount_.Name = "lbl_TilesetCount_";
            this.lbl_TilesetCount_.Size = new System.Drawing.Size(378, 19);
            this.lbl_TilesetCount_.TabIndex = 7;
            this.lbl_TilesetCount_.Text = "Count of tilesets that are defined by Path+Map";
            this.lbl_TilesetCount_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_AddType
            // 
            this.lbl_AddType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_AddType.Location = new System.Drawing.Point(-14, 116);
            this.lbl_AddType.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_AddType.Name = "lbl_AddType";
            this.lbl_AddType.Size = new System.Drawing.Size(571, 18);
            this.lbl_AddType.TabIndex = 9;
            this.lbl_AddType.Text = "lbl_AddType";
            this.lbl_AddType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_Accept
            // 
            this.btn_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Accept.Enabled = false;
            this.btn_Accept.Location = new System.Drawing.Point(585, 97);
            this.btn_Accept.Margin = new System.Windows.Forms.Padding(0);
            this.btn_Accept.Name = "btn_Accept";
            this.btn_Accept.Size = new System.Drawing.Size(119, 37);
            this.btn_Accept.TabIndex = 10;
            this.btn_Accept.Text = "ACCEPT";
            this.btn_Accept.Click += new System.EventHandler(this.OnAcceptClick);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(712, 97);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(112, 37);
            this.btn_Cancel.TabIndex = 11;
            this.btn_Cancel.Text = "Cancel";
            // 
            // gb_Maptree
            // 
            this.gb_Maptree.Controls.Add(this.lbl_Group_);
            this.gb_Maptree.Controls.Add(this.lbl_GroupCurrent);
            this.gb_Maptree.Controls.Add(this.lbl_Category_);
            this.gb_Maptree.Controls.Add(this.lbl_CategoryCurrent);
            this.gb_Maptree.Controls.Add(this.lbl_TilesetCurrent);
            this.gb_Maptree.Controls.Add(this.lbl_McdRecords);
            this.gb_Maptree.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb_Maptree.Location = new System.Drawing.Point(0, 0);
            this.gb_Maptree.Margin = new System.Windows.Forms.Padding(0);
            this.gb_Maptree.Name = "gb_Maptree";
            this.gb_Maptree.Size = new System.Drawing.Size(833, 62);
            this.gb_Maptree.TabIndex = 0;
            this.gb_Maptree.TabStop = false;
            this.gb_Maptree.Text = " Maptree ";
            // 
            // lbl_Group_
            // 
            this.lbl_Group_.Location = new System.Drawing.Point(14, 19);
            this.lbl_Group_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_Group_.Name = "lbl_Group_";
            this.lbl_Group_.Size = new System.Drawing.Size(91, 19);
            this.lbl_Group_.TabIndex = 0;
            this.lbl_Group_.Text = "GROUP";
            // 
            // lbl_GroupCurrent
            // 
            this.lbl_GroupCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_GroupCurrent.Location = new System.Drawing.Point(126, 19);
            this.lbl_GroupCurrent.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_GroupCurrent.Name = "lbl_GroupCurrent";
            this.lbl_GroupCurrent.Size = new System.Drawing.Size(368, 19);
            this.lbl_GroupCurrent.TabIndex = 1;
            this.lbl_GroupCurrent.Text = "lbl_GroupCurrent";
            // 
            // lbl_Category_
            // 
            this.lbl_Category_.Location = new System.Drawing.Point(14, 38);
            this.lbl_Category_.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_Category_.Name = "lbl_Category_";
            this.lbl_Category_.Size = new System.Drawing.Size(91, 18);
            this.lbl_Category_.TabIndex = 3;
            this.lbl_Category_.Text = "CATEGORY";
            // 
            // lbl_CategoryCurrent
            // 
            this.lbl_CategoryCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_CategoryCurrent.Location = new System.Drawing.Point(126, 38);
            this.lbl_CategoryCurrent.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_CategoryCurrent.Name = "lbl_CategoryCurrent";
            this.lbl_CategoryCurrent.Size = new System.Drawing.Size(368, 18);
            this.lbl_CategoryCurrent.TabIndex = 4;
            this.lbl_CategoryCurrent.Text = "lbl_CategoryCurrent";
            // 
            // lbl_TilesetCurrent
            // 
            this.lbl_TilesetCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_TilesetCurrent.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TilesetCurrent.ForeColor = System.Drawing.Color.DarkSeaGreen;
            this.lbl_TilesetCurrent.Location = new System.Drawing.Point(494, 19);
            this.lbl_TilesetCurrent.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_TilesetCurrent.Name = "lbl_TilesetCurrent";
            this.lbl_TilesetCurrent.Size = new System.Drawing.Size(329, 19);
            this.lbl_TilesetCurrent.TabIndex = 2;
            this.lbl_TilesetCurrent.Text = "lbl_TilesetCurrent";
            this.lbl_TilesetCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbl_McdRecords
            // 
            this.lbl_McdRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_McdRecords.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_McdRecords.ForeColor = System.Drawing.Color.Tan;
            this.lbl_McdRecords.Location = new System.Drawing.Point(494, 38);
            this.lbl_McdRecords.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_McdRecords.Name = "lbl_McdRecords";
            this.lbl_McdRecords.Size = new System.Drawing.Size(329, 18);
            this.lbl_McdRecords.TabIndex = 5;
            this.lbl_McdRecords.Text = "lbl_McdRecords";
            this.lbl_McdRecords.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pnl_Bottom
            // 
            this.pnl_Bottom.Controls.Add(this.btn_GlobalTerrainsList);
            this.pnl_Bottom.Controls.Add(this.btn_GlobalTerrains);
            this.pnl_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl_Bottom.Location = new System.Drawing.Point(0, 612);
            this.pnl_Bottom.Margin = new System.Windows.Forms.Padding(0);
            this.pnl_Bottom.Name = "pnl_Bottom";
            this.pnl_Bottom.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.pnl_Bottom.Size = new System.Drawing.Size(833, 37);
            this.pnl_Bottom.TabIndex = 1;
            // 
            // btn_GlobalTerrainsList
            // 
            this.btn_GlobalTerrainsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_GlobalTerrainsList.Location = new System.Drawing.Point(711, 1);
            this.btn_GlobalTerrainsList.Margin = new System.Windows.Forms.Padding(0);
            this.btn_GlobalTerrainsList.Name = "btn_GlobalTerrainsList";
            this.btn_GlobalTerrainsList.Size = new System.Drawing.Size(119, 34);
            this.btn_GlobalTerrainsList.TabIndex = 1;
            this.btn_GlobalTerrainsList.Text = "List ...";
            this.btn_GlobalTerrainsList.UseVisualStyleBackColor = true;
            this.btn_GlobalTerrainsList.Click += new System.EventHandler(this.OnGlobalTerrainsListClick);
            // 
            // btn_GlobalTerrains
            // 
            this.btn_GlobalTerrains.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_GlobalTerrains.Location = new System.Drawing.Point(3, 1);
            this.btn_GlobalTerrains.Margin = new System.Windows.Forms.Padding(0);
            this.btn_GlobalTerrains.Name = "btn_GlobalTerrains";
            this.btn_GlobalTerrains.Size = new System.Drawing.Size(707, 34);
            this.btn_GlobalTerrains.TabIndex = 0;
            this.btn_GlobalTerrains.Text = "Apply allocated terrains to all tilesets that are defined by Path+Map";
            this.btn_GlobalTerrains.UseVisualStyleBackColor = true;
            this.btn_GlobalTerrains.Click += new System.EventHandler(this.OnGlobalTerrainsClick);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // TilesetEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(833, 649);
            this.Controls.Add(this.pnl_Top);
            this.Controls.Add(this.pnl_Bottom);
            this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(840, 500);
            this.Name = "TilesetEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.pnl_Top.ResumeLayout(false);
            this.gb_Terrains.ResumeLayout(false);
            this.pnl_Spacer.ResumeLayout(false);
            this.pnl_TerrainsTop.ResumeLayout(false);
            this.pnl_TerrainsTop.PerformLayout();
            this.gb_Tileset.ResumeLayout(false);
            this.gb_Tileset.PerformLayout();
            this.gb_Maptree.ResumeLayout(false);
            this.pnl_Bottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
        #endregion Designer
    }
}
