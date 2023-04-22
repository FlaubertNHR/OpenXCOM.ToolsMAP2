using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	internal sealed partial class SpawnInfo
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private Label lbl_Tileset_;
		private Label lbl_Category;
		private Label lbl_Group;
		private GroupBox gb_TreeInfo;
		private GroupBox gb_CountInfo;
		private Label lbl_tsRanks0;
		private Label lbl_tsRanks1;
		private Label lbl_tsRanks8;
		private Label lbl_tsRanks7;
		private Label lbl_tsRanks6;
		private Label lbl_tsRanks5;
		private Label lbl_tsRanks4;
		private Label lbl_tsRanks3;
		private Label lbl_tsRanks2;
		private Label lbl_tsRanks8_out;
		private Label lbl_tsRanks7_out;
		private Label lbl_tsRanks6_out;
		private Label lbl_tsRanks5_out;
		private Label lbl_tsRanks4_out;
		private Label lbl_tsRanks3_out;
		private Label lbl_tsRanks2_out;
		private Label lbl_tsRanks1_out;
		private Label lbl_tsRanks0_out;
		private Label lbl_tsCategoryTotals;
		private Label lbl_tsTilesetTotals;
		private Label lbl_tsRanks8_outcat;
		private Label lbl_tsRanks7_outcat;
		private Label lbl_tsRanks6_outcat;
		private Label lbl_tsRanks5_outcat;
		private Label lbl_tsRanks4_outcat;
		private Label lbl_tsRanks3_outcat;
		private Label lbl_tsRanks2_outcat;
		private Label lbl_tsRanks1_outcat;
		private Label lbl_tsRanks0_outcat;
		private Label lbl_Tileset;
		private ToolTip toolTip1;
		private Label lbl_TotalCategory;
		private Label lbl_TotalTileset;


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
			this.lbl_Tileset_ = new System.Windows.Forms.Label();
			this.lbl_Category = new System.Windows.Forms.Label();
			this.lbl_Group = new System.Windows.Forms.Label();
			this.gb_TreeInfo = new System.Windows.Forms.GroupBox();
			this.lbl_Tileset = new System.Windows.Forms.Label();
			this.gb_CountInfo = new System.Windows.Forms.GroupBox();
			this.lbl_TotalCategory = new System.Windows.Forms.Label();
			this.lbl_TotalTileset = new System.Windows.Forms.Label();
			this.lbl_tsRanks8_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks7_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks6_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks5_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks4_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks3_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks2_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks1_outcat = new System.Windows.Forms.Label();
			this.lbl_tsRanks0_outcat = new System.Windows.Forms.Label();
			this.lbl_tsCategoryTotals = new System.Windows.Forms.Label();
			this.lbl_tsTilesetTotals = new System.Windows.Forms.Label();
			this.lbl_tsRanks8_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks7_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks6_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks5_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks4_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks3_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks2_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks1_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks0_out = new System.Windows.Forms.Label();
			this.lbl_tsRanks8 = new System.Windows.Forms.Label();
			this.lbl_tsRanks7 = new System.Windows.Forms.Label();
			this.lbl_tsRanks6 = new System.Windows.Forms.Label();
			this.lbl_tsRanks5 = new System.Windows.Forms.Label();
			this.lbl_tsRanks4 = new System.Windows.Forms.Label();
			this.lbl_tsRanks3 = new System.Windows.Forms.Label();
			this.lbl_tsRanks2 = new System.Windows.Forms.Label();
			this.lbl_tsRanks1 = new System.Windows.Forms.Label();
			this.lbl_tsRanks0 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.gb_TreeInfo.SuspendLayout();
			this.gb_CountInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbl_Tileset_
			// 
			this.lbl_Tileset_.Location = new System.Drawing.Point(3, 10);
			this.lbl_Tileset_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Tileset_.Name = "lbl_Tileset_";
			this.lbl_Tileset_.Size = new System.Drawing.Size(45, 15);
			this.lbl_Tileset_.TabIndex = 0;
			this.lbl_Tileset_.Text = "Tileset";
			this.lbl_Tileset_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_Category
			// 
			this.lbl_Category.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbl_Category.Location = new System.Drawing.Point(236, 10);
			this.lbl_Category.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Category.Name = "lbl_Category";
			this.lbl_Category.Size = new System.Drawing.Size(180, 15);
			this.lbl_Category.TabIndex = 2;
			this.lbl_Category.Text = "Category";
			this.lbl_Category.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.lbl_Category, "Category");
			// 
			// lbl_Group
			// 
			this.lbl_Group.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbl_Group.Location = new System.Drawing.Point(236, 25);
			this.lbl_Group.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Group.Name = "lbl_Group";
			this.lbl_Group.Size = new System.Drawing.Size(180, 15);
			this.lbl_Group.TabIndex = 3;
			this.lbl_Group.Text = "Group";
			this.lbl_Group.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip1.SetToolTip(this.lbl_Group, "Group");
			// 
			// gb_TreeInfo
			// 
			this.gb_TreeInfo.Controls.Add(this.lbl_Tileset);
			this.gb_TreeInfo.Controls.Add(this.lbl_Group);
			this.gb_TreeInfo.Controls.Add(this.lbl_Category);
			this.gb_TreeInfo.Controls.Add(this.lbl_Tileset_);
			this.gb_TreeInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.gb_TreeInfo.Location = new System.Drawing.Point(0, 0);
			this.gb_TreeInfo.Margin = new System.Windows.Forms.Padding(0);
			this.gb_TreeInfo.Name = "gb_TreeInfo";
			this.gb_TreeInfo.Padding = new System.Windows.Forms.Padding(0);
			this.gb_TreeInfo.Size = new System.Drawing.Size(420, 44);
			this.gb_TreeInfo.TabIndex = 0;
			this.gb_TreeInfo.TabStop = false;
			// 
			// lbl_Tileset
			// 
			this.lbl_Tileset.Location = new System.Drawing.Point(54, 10);
			this.lbl_Tileset.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Tileset.Name = "lbl_Tileset";
			this.lbl_Tileset.Size = new System.Drawing.Size(180, 15);
			this.lbl_Tileset.TabIndex = 1;
			this.lbl_Tileset.Text = "label";
			this.lbl_Tileset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gb_CountInfo
			// 
			this.gb_CountInfo.Controls.Add(this.lbl_TotalCategory);
			this.gb_CountInfo.Controls.Add(this.lbl_TotalTileset);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks8_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks7_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks6_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks5_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks4_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks3_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks2_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks1_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks0_outcat);
			this.gb_CountInfo.Controls.Add(this.lbl_tsCategoryTotals);
			this.gb_CountInfo.Controls.Add(this.lbl_tsTilesetTotals);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks8_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks7_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks6_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks5_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks4_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks3_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks2_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks1_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks0_out);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks8);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks7);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks6);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks5);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks4);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks3);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks2);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks1);
			this.gb_CountInfo.Controls.Add(this.lbl_tsRanks0);
			this.gb_CountInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_CountInfo.Location = new System.Drawing.Point(0, 44);
			this.gb_CountInfo.Margin = new System.Windows.Forms.Padding(0);
			this.gb_CountInfo.Name = "gb_CountInfo";
			this.gb_CountInfo.Size = new System.Drawing.Size(420, 184);
			this.gb_CountInfo.TabIndex = 1;
			this.gb_CountInfo.TabStop = false;
			this.gb_CountInfo.Text = " totals ";
			// 
			// lbl_TotalCategory
			// 
			this.lbl_TotalCategory.Location = new System.Drawing.Point(360, 164);
			this.lbl_TotalCategory.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_TotalCategory.Name = "lbl_TotalCategory";
			this.lbl_TotalCategory.Size = new System.Drawing.Size(56, 15);
			this.lbl_TotalCategory.TabIndex = 30;
			this.lbl_TotalCategory.Text = "tc";
			this.lbl_TotalCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_TotalTileset
			// 
			this.lbl_TotalTileset.Location = new System.Drawing.Point(312, 164);
			this.lbl_TotalTileset.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_TotalTileset.Name = "lbl_TotalTileset";
			this.lbl_TotalTileset.Size = new System.Drawing.Size(43, 15);
			this.lbl_TotalTileset.TabIndex = 29;
			this.lbl_TotalTileset.Text = "tt";
			this.lbl_TotalTileset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks8_outcat
			// 
			this.lbl_tsRanks8_outcat.Location = new System.Drawing.Point(360, 145);
			this.lbl_tsRanks8_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks8_outcat.Name = "lbl_tsRanks8_outcat";
			this.lbl_tsRanks8_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks8_outcat.TabIndex = 28;
			this.lbl_tsRanks8_outcat.Text = "8";
			this.lbl_tsRanks8_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks7_outcat
			// 
			this.lbl_tsRanks7_outcat.Location = new System.Drawing.Point(360, 130);
			this.lbl_tsRanks7_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks7_outcat.Name = "lbl_tsRanks7_outcat";
			this.lbl_tsRanks7_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks7_outcat.TabIndex = 25;
			this.lbl_tsRanks7_outcat.Text = "7";
			this.lbl_tsRanks7_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks6_outcat
			// 
			this.lbl_tsRanks6_outcat.Location = new System.Drawing.Point(360, 115);
			this.lbl_tsRanks6_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks6_outcat.Name = "lbl_tsRanks6_outcat";
			this.lbl_tsRanks6_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks6_outcat.TabIndex = 22;
			this.lbl_tsRanks6_outcat.Text = "6";
			this.lbl_tsRanks6_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks5_outcat
			// 
			this.lbl_tsRanks5_outcat.Location = new System.Drawing.Point(360, 100);
			this.lbl_tsRanks5_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks5_outcat.Name = "lbl_tsRanks5_outcat";
			this.lbl_tsRanks5_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks5_outcat.TabIndex = 19;
			this.lbl_tsRanks5_outcat.Text = "5";
			this.lbl_tsRanks5_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks4_outcat
			// 
			this.lbl_tsRanks4_outcat.Location = new System.Drawing.Point(360, 85);
			this.lbl_tsRanks4_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks4_outcat.Name = "lbl_tsRanks4_outcat";
			this.lbl_tsRanks4_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks4_outcat.TabIndex = 16;
			this.lbl_tsRanks4_outcat.Text = "4";
			this.lbl_tsRanks4_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks3_outcat
			// 
			this.lbl_tsRanks3_outcat.Location = new System.Drawing.Point(360, 70);
			this.lbl_tsRanks3_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks3_outcat.Name = "lbl_tsRanks3_outcat";
			this.lbl_tsRanks3_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks3_outcat.TabIndex = 13;
			this.lbl_tsRanks3_outcat.Text = "3";
			this.lbl_tsRanks3_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks2_outcat
			// 
			this.lbl_tsRanks2_outcat.Location = new System.Drawing.Point(360, 55);
			this.lbl_tsRanks2_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks2_outcat.Name = "lbl_tsRanks2_outcat";
			this.lbl_tsRanks2_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks2_outcat.TabIndex = 10;
			this.lbl_tsRanks2_outcat.Text = "2";
			this.lbl_tsRanks2_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks1_outcat
			// 
			this.lbl_tsRanks1_outcat.Location = new System.Drawing.Point(360, 40);
			this.lbl_tsRanks1_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks1_outcat.Name = "lbl_tsRanks1_outcat";
			this.lbl_tsRanks1_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks1_outcat.TabIndex = 7;
			this.lbl_tsRanks1_outcat.Text = "1";
			this.lbl_tsRanks1_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks0_outcat
			// 
			this.lbl_tsRanks0_outcat.Location = new System.Drawing.Point(360, 25);
			this.lbl_tsRanks0_outcat.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks0_outcat.Name = "lbl_tsRanks0_outcat";
			this.lbl_tsRanks0_outcat.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsRanks0_outcat.TabIndex = 4;
			this.lbl_tsRanks0_outcat.Text = "0";
			this.lbl_tsRanks0_outcat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsCategoryTotals
			// 
			this.lbl_tsCategoryTotals.Location = new System.Drawing.Point(360, 10);
			this.lbl_tsCategoryTotals.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsCategoryTotals.Name = "lbl_tsCategoryTotals";
			this.lbl_tsCategoryTotals.Size = new System.Drawing.Size(56, 15);
			this.lbl_tsCategoryTotals.TabIndex = 1;
			this.lbl_tsCategoryTotals.Text = "category";
			this.lbl_tsCategoryTotals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.lbl_tsCategoryTotals, "counts in tileset\'s Category");
			// 
			// lbl_tsTilesetTotals
			// 
			this.lbl_tsTilesetTotals.Location = new System.Drawing.Point(312, 10);
			this.lbl_tsTilesetTotals.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsTilesetTotals.Name = "lbl_tsTilesetTotals";
			this.lbl_tsTilesetTotals.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsTilesetTotals.TabIndex = 0;
			this.lbl_tsTilesetTotals.Text = "tileset";
			this.lbl_tsTilesetTotals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.lbl_tsTilesetTotals, "counts in tileset");
			// 
			// lbl_tsRanks8_out
			// 
			this.lbl_tsRanks8_out.Location = new System.Drawing.Point(312, 145);
			this.lbl_tsRanks8_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks8_out.Name = "lbl_tsRanks8_out";
			this.lbl_tsRanks8_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks8_out.TabIndex = 27;
			this.lbl_tsRanks8_out.Text = "8";
			this.lbl_tsRanks8_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks7_out
			// 
			this.lbl_tsRanks7_out.Location = new System.Drawing.Point(312, 130);
			this.lbl_tsRanks7_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks7_out.Name = "lbl_tsRanks7_out";
			this.lbl_tsRanks7_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks7_out.TabIndex = 24;
			this.lbl_tsRanks7_out.Text = "7";
			this.lbl_tsRanks7_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks6_out
			// 
			this.lbl_tsRanks6_out.Location = new System.Drawing.Point(312, 115);
			this.lbl_tsRanks6_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks6_out.Name = "lbl_tsRanks6_out";
			this.lbl_tsRanks6_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks6_out.TabIndex = 21;
			this.lbl_tsRanks6_out.Text = "6";
			this.lbl_tsRanks6_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks5_out
			// 
			this.lbl_tsRanks5_out.Location = new System.Drawing.Point(312, 100);
			this.lbl_tsRanks5_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks5_out.Name = "lbl_tsRanks5_out";
			this.lbl_tsRanks5_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks5_out.TabIndex = 18;
			this.lbl_tsRanks5_out.Text = "5";
			this.lbl_tsRanks5_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks4_out
			// 
			this.lbl_tsRanks4_out.Location = new System.Drawing.Point(312, 85);
			this.lbl_tsRanks4_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks4_out.Name = "lbl_tsRanks4_out";
			this.lbl_tsRanks4_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks4_out.TabIndex = 15;
			this.lbl_tsRanks4_out.Text = "4";
			this.lbl_tsRanks4_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks3_out
			// 
			this.lbl_tsRanks3_out.Location = new System.Drawing.Point(312, 70);
			this.lbl_tsRanks3_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks3_out.Name = "lbl_tsRanks3_out";
			this.lbl_tsRanks3_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks3_out.TabIndex = 12;
			this.lbl_tsRanks3_out.Text = "3";
			this.lbl_tsRanks3_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks2_out
			// 
			this.lbl_tsRanks2_out.Location = new System.Drawing.Point(312, 55);
			this.lbl_tsRanks2_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks2_out.Name = "lbl_tsRanks2_out";
			this.lbl_tsRanks2_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks2_out.TabIndex = 9;
			this.lbl_tsRanks2_out.Text = "2";
			this.lbl_tsRanks2_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks1_out
			// 
			this.lbl_tsRanks1_out.Location = new System.Drawing.Point(312, 40);
			this.lbl_tsRanks1_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks1_out.Name = "lbl_tsRanks1_out";
			this.lbl_tsRanks1_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks1_out.TabIndex = 6;
			this.lbl_tsRanks1_out.Text = "1";
			this.lbl_tsRanks1_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks0_out
			// 
			this.lbl_tsRanks0_out.Location = new System.Drawing.Point(312, 25);
			this.lbl_tsRanks0_out.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks0_out.Name = "lbl_tsRanks0_out";
			this.lbl_tsRanks0_out.Size = new System.Drawing.Size(43, 15);
			this.lbl_tsRanks0_out.TabIndex = 3;
			this.lbl_tsRanks0_out.Text = "0";
			this.lbl_tsRanks0_out.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks8
			// 
			this.lbl_tsRanks8.Location = new System.Drawing.Point(15, 145);
			this.lbl_tsRanks8.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks8.Name = "lbl_tsRanks8";
			this.lbl_tsRanks8.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks8.TabIndex = 26;
			this.lbl_tsRanks8.Text = "8";
			this.lbl_tsRanks8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks7
			// 
			this.lbl_tsRanks7.Location = new System.Drawing.Point(15, 130);
			this.lbl_tsRanks7.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks7.Name = "lbl_tsRanks7";
			this.lbl_tsRanks7.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks7.TabIndex = 23;
			this.lbl_tsRanks7.Text = "7";
			this.lbl_tsRanks7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks6
			// 
			this.lbl_tsRanks6.Location = new System.Drawing.Point(15, 115);
			this.lbl_tsRanks6.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks6.Name = "lbl_tsRanks6";
			this.lbl_tsRanks6.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks6.TabIndex = 20;
			this.lbl_tsRanks6.Text = "6";
			this.lbl_tsRanks6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks5
			// 
			this.lbl_tsRanks5.Location = new System.Drawing.Point(15, 100);
			this.lbl_tsRanks5.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks5.Name = "lbl_tsRanks5";
			this.lbl_tsRanks5.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks5.TabIndex = 17;
			this.lbl_tsRanks5.Text = "5";
			this.lbl_tsRanks5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks4
			// 
			this.lbl_tsRanks4.Location = new System.Drawing.Point(15, 85);
			this.lbl_tsRanks4.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks4.Name = "lbl_tsRanks4";
			this.lbl_tsRanks4.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks4.TabIndex = 14;
			this.lbl_tsRanks4.Text = "4";
			this.lbl_tsRanks4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks3
			// 
			this.lbl_tsRanks3.Location = new System.Drawing.Point(15, 70);
			this.lbl_tsRanks3.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks3.Name = "lbl_tsRanks3";
			this.lbl_tsRanks3.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks3.TabIndex = 11;
			this.lbl_tsRanks3.Text = "3";
			this.lbl_tsRanks3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks2
			// 
			this.lbl_tsRanks2.Location = new System.Drawing.Point(15, 55);
			this.lbl_tsRanks2.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks2.Name = "lbl_tsRanks2";
			this.lbl_tsRanks2.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks2.TabIndex = 8;
			this.lbl_tsRanks2.Text = "2";
			this.lbl_tsRanks2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks1
			// 
			this.lbl_tsRanks1.Location = new System.Drawing.Point(15, 40);
			this.lbl_tsRanks1.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks1.Name = "lbl_tsRanks1";
			this.lbl_tsRanks1.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks1.TabIndex = 5;
			this.lbl_tsRanks1.Text = "1";
			this.lbl_tsRanks1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_tsRanks0
			// 
			this.lbl_tsRanks0.Location = new System.Drawing.Point(15, 25);
			this.lbl_tsRanks0.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_tsRanks0.Name = "lbl_tsRanks0";
			this.lbl_tsRanks0.Size = new System.Drawing.Size(292, 15);
			this.lbl_tsRanks0.TabIndex = 2;
			this.lbl_tsRanks0.Text = "0";
			this.lbl_tsRanks0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.UseAnimation = false;
			this.toolTip1.UseFading = false;
			// 
			// SpawnInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(420, 228);
			this.Controls.Add(this.gb_CountInfo);
			this.Controls.Add(this.gb_TreeInfo);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.Name = "SpawnInfo";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Spawn Info";
			this.gb_TreeInfo.ResumeLayout(false);
			this.gb_CountInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
