using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom;


namespace MapView
{
	internal sealed class MapInfoOutputBox
		:
			Form
	{
		internal MapInfoOutputBox()
		{
			InitializeComponent();
		}


		internal void Analyze(MapFile file)
		{
			gbInfo.Text = " " + file.Descriptor.Label + " ";

			int cols = file.MapSize.Cols;
			int rows = file.MapSize.Rows;
			int levs = file.MapSize.Levs;

			lbl2_Dimensions.Text = cols + " x "
								 + rows + " x "
								 + levs;

			int spritesTotal = ResourceInfo.GetTotalSpriteCount();
			int recordsTotal = 0;

			string text = String.Empty;
			for (int i = 0; i != file.Terrains.Count; ++i)
			{
				if (!String.IsNullOrEmpty(text))
					text += " | ";

				text += file.Terrains[i].Item1;
				recordsTotal += file.Descriptor.GetRecordCount(i);
			}
			lbl2_Terrains.Text = text;

			var w = TextRenderer.MeasureText(text, lbl2_Terrains.Font).Width;
			if (w > lbl2_Terrains.Width)
				Width += w - lbl2_Terrains.Width;

			Refresh();

			int slots  = 0;
			int vacant = 0;

			var sprites = new HashSet<int>();
			var records = new HashSet<int>();

			pBar.Maximum = cols * rows * levs;
			pBar.Value = 0;

			for (int col = 0; col != cols; ++col)
			for (int row = 0; row != rows; ++row)
			for (int lev = 0; lev != levs; ++lev)
			{
				var tile = file[row, col, lev] as XCMapTile;
				if (!tile.Vacant)
				{
					if (tile.Floor != null)
					{
						++slots;
						Count(tile.Floor, sprites, records);
					}

					if (tile.West != null)
					{
						++slots;
						Count(tile.West, sprites, records);
					}

					if (tile.North != null)
					{
						++slots;
						Count(tile.North, sprites, records);
					}

					if (tile.Content != null)
					{
						++slots;
						Count(tile.Content, sprites, records);
					}
				}
				else
					++vacant;

				++pBar.Value;
				pBar.Refresh();
			}

			double pct;;
			pct = Math.Round(100.0 * (double)sprites.Count / (double)spritesTotal, 2);
			lbl2_PckSprites.Text = sprites.Count + " of " + spritesTotal       + " - " + pct.ToString("N2") + "%";

			pct = Math.Round(100.0 * (double)records.Count / (double)recordsTotal, 2);
			lbl2_McdRecords.Text = records.Count + " of " + recordsTotal       + " - " + pct.ToString("N2") + "%";

			pct = Math.Round(100.0 * (double)slots / (double)(pBar.Maximum * 4), 2);
			lbl2_SlotsFilled.Text = slots        + " of " + (pBar.Maximum * 4) + " - " + pct.ToString("N2") + "%";

			pct = Math.Round(100.0 * (double)vacant / (double)pBar.Maximum, 2);
			lbl2_TilesVacant.Text = vacant       + " of " + pBar.Maximum       + " - " + pct.ToString("N2") + "%";

			gbAnalyze.Visible = false;
			btnDetail.Visible =
			btnCancel.Visible = true;
		}

		private static void Count(
				Tilepart part,
				ISet<int> sprites,
				ISet<int> records)
		{
			if (part != null && records.Add(part.Record.SetId))
			{
				foreach (PckImage sprite in part.Sprites)
					sprites.Add(sprite.SetId);

				Tilepart dead, altr;
				Count((dead = part.Dead), sprites, records);
				Count((altr = part.Altr), sprites, records);
			}
		}

		private void click_btnDetail(object sender, EventArgs e)
		{
		}

		private void click_btnClose(object sender, EventArgs e)
		{
			Close();
		}


		#region Designer
		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
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
			this.lbl1_Dimensions = new System.Windows.Forms.Label();
			this.lbl2_Dimensions = new System.Windows.Forms.Label();
			this.lbl2_Terrains = new System.Windows.Forms.Label();
			this.lbl1_Terrains = new System.Windows.Forms.Label();
			this.lbl2_PckSprites = new System.Windows.Forms.Label();
			this.lbl1_PckSprites = new System.Windows.Forms.Label();
			this.lbl2_McdRecords = new System.Windows.Forms.Label();
			this.lbl1_McdRecords = new System.Windows.Forms.Label();
			this.lbl2_SlotsFilled = new System.Windows.Forms.Label();
			this.lbl1_SlotsUsed = new System.Windows.Forms.Label();
			this.pBar = new System.Windows.Forms.ProgressBar();
			this.gbAnalyze = new System.Windows.Forms.GroupBox();
			this.btnDetail = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbInfo = new System.Windows.Forms.GroupBox();
			this.lbl1_TilesVacant = new System.Windows.Forms.Label();
			this.lbl2_TilesVacant = new System.Windows.Forms.Label();
			this.gbAnalyze.SuspendLayout();
			this.gbInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbl1_Dimensions
			// 
			this.lbl1_Dimensions.Location = new System.Drawing.Point(10, 15);
			this.lbl1_Dimensions.Name = "lbl1_Dimensions";
			this.lbl1_Dimensions.Size = new System.Drawing.Size(75, 15);
			this.lbl1_Dimensions.TabIndex = 0;
			this.lbl1_Dimensions.Text = "dimensions";
			// 
			// lbl2_Dimensions
			// 
			this.lbl2_Dimensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_Dimensions.Location = new System.Drawing.Point(85, 15);
			this.lbl2_Dimensions.Name = "lbl2_Dimensions";
			this.lbl2_Dimensions.Size = new System.Drawing.Size(250, 15);
			this.lbl2_Dimensions.TabIndex = 1;
			// 
			// lbl2_Terrains
			// 
			this.lbl2_Terrains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_Terrains.Location = new System.Drawing.Point(85, 30);
			this.lbl2_Terrains.Name = "lbl2_Terrains";
			this.lbl2_Terrains.Size = new System.Drawing.Size(250, 15);
			this.lbl2_Terrains.TabIndex = 3;
			// 
			// lbl1_Terrains
			// 
			this.lbl1_Terrains.Location = new System.Drawing.Point(10, 30);
			this.lbl1_Terrains.Name = "lbl1_Terrains";
			this.lbl1_Terrains.Size = new System.Drawing.Size(75, 15);
			this.lbl1_Terrains.TabIndex = 2;
			this.lbl1_Terrains.Text = "terrains";
			// 
			// lbl2_PckSprites
			// 
			this.lbl2_PckSprites.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_PckSprites.Location = new System.Drawing.Point(85, 45);
			this.lbl2_PckSprites.Name = "lbl2_PckSprites";
			this.lbl2_PckSprites.Size = new System.Drawing.Size(250, 15);
			this.lbl2_PckSprites.TabIndex = 5;
			// 
			// lbl1_PckSprites
			// 
			this.lbl1_PckSprites.Location = new System.Drawing.Point(10, 45);
			this.lbl1_PckSprites.Name = "lbl1_PckSprites";
			this.lbl1_PckSprites.Size = new System.Drawing.Size(75, 15);
			this.lbl1_PckSprites.TabIndex = 4;
			this.lbl1_PckSprites.Text = "pck sprites";
			// 
			// lbl2_McdRecords
			// 
			this.lbl2_McdRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_McdRecords.Location = new System.Drawing.Point(85, 60);
			this.lbl2_McdRecords.Name = "lbl2_McdRecords";
			this.lbl2_McdRecords.Size = new System.Drawing.Size(250, 15);
			this.lbl2_McdRecords.TabIndex = 7;
			// 
			// lbl1_McdRecords
			// 
			this.lbl1_McdRecords.Location = new System.Drawing.Point(10, 60);
			this.lbl1_McdRecords.Name = "lbl1_McdRecords";
			this.lbl1_McdRecords.Size = new System.Drawing.Size(75, 15);
			this.lbl1_McdRecords.TabIndex = 6;
			this.lbl1_McdRecords.Text = "mcd records";
			// 
			// lbl2_SlotsFilled
			// 
			this.lbl2_SlotsFilled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_SlotsFilled.Location = new System.Drawing.Point(85, 75);
			this.lbl2_SlotsFilled.Name = "lbl2_SlotsFilled";
			this.lbl2_SlotsFilled.Size = new System.Drawing.Size(250, 15);
			this.lbl2_SlotsFilled.TabIndex = 9;
			// 
			// lbl1_SlotsUsed
			// 
			this.lbl1_SlotsUsed.Location = new System.Drawing.Point(10, 75);
			this.lbl1_SlotsUsed.Name = "lbl1_SlotsUsed";
			this.lbl1_SlotsUsed.Size = new System.Drawing.Size(75, 15);
			this.lbl1_SlotsUsed.TabIndex = 8;
			this.lbl1_SlotsUsed.Text = "slots used";
			// 
			// pBar
			// 
			this.pBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pBar.Location = new System.Drawing.Point(5, 15);
			this.pBar.Name = "pBar";
			this.pBar.Size = new System.Drawing.Size(326, 25);
			this.pBar.TabIndex = 0;
			// 
			// gbAnalyze
			// 
			this.gbAnalyze.Controls.Add(this.pBar);
			this.gbAnalyze.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gbAnalyze.Location = new System.Drawing.Point(3, 110);
			this.gbAnalyze.Margin = new System.Windows.Forms.Padding(0);
			this.gbAnalyze.Name = "gbAnalyze";
			this.gbAnalyze.Padding = new System.Windows.Forms.Padding(5, 3, 5, 4);
			this.gbAnalyze.Size = new System.Drawing.Size(336, 44);
			this.gbAnalyze.TabIndex = 12;
			this.gbAnalyze.TabStop = false;
			// 
			// btnDetail
			// 
			this.btnDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDetail.Location = new System.Drawing.Point(160, 121);
			this.btnDetail.Margin = new System.Windows.Forms.Padding(0);
			this.btnDetail.Name = "btnDetail";
			this.btnDetail.Size = new System.Drawing.Size(85, 30);
			this.btnDetail.TabIndex = 13;
			this.btnDetail.Text = "Detail";
			this.btnDetail.UseVisualStyleBackColor = true;
			this.btnDetail.Visible = false;
			this.btnDetail.Click += new System.EventHandler(this.click_btnDetail);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(250, 121);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 30);
			this.btnCancel.TabIndex = 14;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Visible = false;
			this.btnCancel.Click += new System.EventHandler(this.click_btnClose);
			// 
			// gbInfo
			// 
			this.gbInfo.Controls.Add(this.lbl1_TilesVacant);
			this.gbInfo.Controls.Add(this.lbl2_TilesVacant);
			this.gbInfo.Controls.Add(this.lbl1_Dimensions);
			this.gbInfo.Controls.Add(this.lbl2_Dimensions);
			this.gbInfo.Controls.Add(this.lbl1_Terrains);
			this.gbInfo.Controls.Add(this.lbl2_Terrains);
			this.gbInfo.Controls.Add(this.lbl1_PckSprites);
			this.gbInfo.Controls.Add(this.lbl2_PckSprites);
			this.gbInfo.Controls.Add(this.lbl1_McdRecords);
			this.gbInfo.Controls.Add(this.lbl2_McdRecords);
			this.gbInfo.Controls.Add(this.lbl1_SlotsUsed);
			this.gbInfo.Controls.Add(this.lbl2_SlotsFilled);
			this.gbInfo.Controls.Add(this.btnDetail);
			this.gbInfo.Controls.Add(this.btnCancel);
			this.gbInfo.Controls.Add(this.gbAnalyze);
			this.gbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbInfo.Location = new System.Drawing.Point(0, 2);
			this.gbInfo.Margin = new System.Windows.Forms.Padding(0);
			this.gbInfo.Name = "gbInfo";
			this.gbInfo.Size = new System.Drawing.Size(342, 157);
			this.gbInfo.TabIndex = 0;
			this.gbInfo.TabStop = false;
			this.gbInfo.Text = " label ";
			// 
			// lbl1_TilesVacant
			// 
			this.lbl1_TilesVacant.Location = new System.Drawing.Point(10, 90);
			this.lbl1_TilesVacant.Name = "lbl1_TilesVacant";
			this.lbl1_TilesVacant.Size = new System.Drawing.Size(75, 15);
			this.lbl1_TilesVacant.TabIndex = 10;
			this.lbl1_TilesVacant.Text = "tiles vacant";
			// 
			// lbl2_TilesVacant
			// 
			this.lbl2_TilesVacant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_TilesVacant.Location = new System.Drawing.Point(85, 90);
			this.lbl2_TilesVacant.Name = "lbl2_TilesVacant";
			this.lbl2_TilesVacant.Size = new System.Drawing.Size(250, 15);
			this.lbl2_TilesVacant.TabIndex = 11;
			// 
			// MapInfoOutputBox
			// 
			this.AcceptButton = this.btnDetail;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(342, 159);
			this.Controls.Add(this.gbInfo);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(350, 170);
			this.Name = "MapInfoOutputBox";
			this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = " MapInfo";
			this.gbAnalyze.ResumeLayout(false);
			this.gbInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private System.ComponentModel.Container components = null;

		private GroupBox gbInfo;
		private Label lbl1_Dimensions;
		private Label lbl2_Dimensions;
		private Label lbl1_Terrains;
		private Label lbl2_Terrains;
		private Label lbl1_PckSprites;
		private Label lbl2_PckSprites;
		private Label lbl1_McdRecords;
		private Label lbl2_McdRecords;
		private Label lbl1_SlotsUsed;
		private Label lbl2_SlotsFilled;
		private GroupBox gbAnalyze;
		private ProgressBar pBar;
		private Button btnCancel;
		private Button btnDetail;
		private Label lbl1_TilesVacant;
		private Label lbl2_TilesVacant;
		#endregion Designer
	}
}
