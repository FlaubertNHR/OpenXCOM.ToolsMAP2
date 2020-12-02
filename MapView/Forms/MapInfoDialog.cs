using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView
{
	internal sealed class MapInfoDialog
		:
			Form
	{
		#region Fields (static)
		private static int _x = -1;
		private static int _y = -1;
		#endregion Fields (static)


		#region Fields
		private readonly MainViewF _f;

		private readonly HashSet<int> _sprites = new HashSet<int>();
		private readonly HashSet<int> _records = new HashSet<int>();

		private readonly HashSet<Tuple<string, int>> _used =
					 new HashSet<Tuple<string, int>>();

		private MapFile _file;

		internal MapInfoDetailDialog _fdetail;
		#endregion Fields


		#region cTor
		internal MapInfoDialog(MainViewF f)
		{
			InitializeComponent();

			_f = f;

			if (_x == -1) _x = _f.Left + _f.Width  / 2 - Width  / 2;
			if (_y == -1) _y = _f.Top  + _f.Height / 2 - Height / 2 - 10;

			Left = _x;
			Top  = _y;

			Visible = true;
			Analyze();

			btnCancel.Select();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Handles this dialog's closing event. Sets the static location and
		/// nulls the dialog in MainViewF.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			_x = Left;
			_y = Top;

			if (_fdetail != null)
				_fdetail.Close();

			_f._finfo = null;
			_f.DecheckMapInfo();

			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		private void click_btnRefresh(object sender, EventArgs e)
		{
			Analyze();
		}

		// TODO:
		// - MapDetail		(Terrains in the Tileset: label + count)
		// - RouteDetail	(routenodes, spawnnodes + ranks, attacknodes in the Tileset or Category)
		// - CategoryDetail	(Tilesets in the Category)
		// - GroupDetail	(Categories + Tilesets in the Group)
		// - TerrainDetail	(all Tilesets that use a Terrain or all Terrains used in the Maptree)
		private void click_btnTerrainDetail(object sender, EventArgs e)
		{
			if (_fdetail == null)
			{
				_fdetail = new MapInfoDetailDialog(this);

				_fdetail.SetTitleText("Detail - " + _file.Descriptor.Label);
				_fdetail.SetHeaderText("MCD Record usage");
				_fdetail.SetCopyableText(GetTerrainDetail());

				_fdetail.Show(this);
			}
			else if (_fdetail.WindowState == FormWindowState.Minimized)
			{
				_fdetail.WindowState = FormWindowState.Normal;
			}
			else
			{
				_fdetail.TopMost = true;
				_fdetail.TopMost = false;
			}
		}

		/// <summary>
		/// Closes this dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void click_btnClose(object sender, EventArgs e)
		{
			Close();
		}
		#endregion Events


		#region Methods (static)
		private static int GetLongestTerrainLabelLength(Dictionary<int, Tuple<string,string>> terrains)
		{
			int length = 0, lengthtest;
			foreach (var terrain in terrains)
			{
				if ((lengthtest = terrain.Value.Item1.Length) > length)
					length = lengthtest;
			}
			return length;
		}
		#endregion Methods (static)


		#region Methods
		private void Analyze()
		{
			if (_fdetail != null)
				_fdetail.Close();

			gbAnalyze .Visible = true;
			btnRefresh.Visible =
			btnDetail .Visible =
			btnCancel .Visible = false;

			_file = MainViewUnderlay.that.MapFile;

			Text = "MapInfo - " + _file.Descriptor.Label;

			gbInfo.Text = " " + _file.Descriptor.Label + " ";

			int cols = _file.MapSize.Cols;
			int rows = _file.MapSize.Rows;
			int levs = _file.MapSize.Levs;

			lbl2_Dimensions.Text = cols + " x "
								 + rows + " x "
								 + levs;

			int spritesTotal = SpritesetsManager.GetTotalSpriteCount();
			int recordsTotal = 0;

			string text = String.Empty;
			for (int i = 0; i != _file.Terrains.Count; ++i)
			{
				if (!String.IsNullOrEmpty(text))
					text += " | ";

				text += _file.Terrains[i].Item1;
				recordsTotal += _file.Descriptor.GetRecordCount(i);
			}
			lbl2_Terrains.Text = text;

			int w = TextRenderer.MeasureText(text, lbl2_Terrains.Font).Width;
			if (w > lbl2_Terrains.Width)
				Width += w - lbl2_Terrains.Width;

			MinimumSize = new Size(Width, Height);

			Refresh();

			int slots  = 0;
			int vacant = 0;

			pBar.Maximum = cols * rows * levs;
			pBar.Value = 0;

			MapTile tile;
			Tilepart part;

			for (int lev = 0; lev != levs; ++lev)
			for (int row = 0; row != rows; ++row)
			for (int col = 0; col != cols; ++col)
			{
				if (!(tile = _file[col, row, lev]).Vacant)
				{
					for (int slot = 0; slot != MapTile.QUADS; ++slot)
					{
						switch (slot)
						{
							default: part = tile.Floor;   break; // case 0
							case  1: part = tile.West;    break;
							case  2: part = tile.North;   break;
							case  3: part = tile.Content; break;
						}

						if (part != null)
						{
							++slots;
							tally(part);
						}
					}
				}
				else
					++vacant;

				++pBar.Value;
				pBar.Refresh();
			}

			double pct;;
			pct = Math.Round(100.0 * (double)_sprites.Count / spritesTotal, 2);
			lbl2_PckSprites.Text = _sprites.Count + " of " + spritesTotal + " - " + pct.ToString("N2") + "%";

			pct = Math.Round(100.0 * (double)_records.Count / recordsTotal, 2);
			lbl2_McdRecords.Text = _records.Count + " of " + recordsTotal + " - " + pct.ToString("N2") + "%";

			pct = Math.Round(100.0 * (double)slots / (pBar.Maximum * 4), 2);
			lbl2_QuadsFilled.Text = slots + " of " + (pBar.Maximum * 4) + " - " + pct.ToString("N2") + "%";

			pct = Math.Round(100.0 * (double)vacant / pBar.Maximum, 2);
			lbl2_TilesVacant.Text = vacant + " of " + pBar.Maximum + " - " + pct.ToString("N2") + "%";

			gbAnalyze .Visible = false;
			btnRefresh.Visible =
			btnDetail .Visible =
			btnCancel .Visible = true;
		}

		private void tally(Tilepart part)
		{
			if (part != null && _records.Add(part.SetId))
			{
				_used.Add(new Tuple<string, int>(_file.GetTerrainLabel(part), part.TerId));

				foreach (var sprite in part.Sprites)
					_sprites.Add(sprite.SetId);
//					_sprites.Add(sprite.Id); // TODO: fix this mess. The id goes borked in MapInfo after the set is opened in McdView

				tally(part.Dead);
				tally(part.Altr);
			}
		}

		/// <summary>
		/// Gets a batch of copyable text describing terrain-details.
		/// </summary>
		/// <returns></returns>
		private string GetTerrainDetail()
		{
			string detail = String.Empty;

			int labelwidth = GetLongestTerrainLabelLength(_file.Terrains);
			string label;

			foreach (var terrain in _file.Terrains)
			{
				if (!String.IsNullOrEmpty(detail))
					detail += Environment.NewLine;

				label = terrain.Value.Item1;
				detail += "[" + terrain.Key + "] " + label + " - " + _file.Descriptor.GetTerrainDirectory(terrain.Value.Item2);

				int terraincount = _file.Descriptor.GetRecordCount(terrain.Key, true);

				for (int i = 0; i != terraincount; ++i)
				{
					if (_used.Contains(new Tuple<string, int>(label, i)))
						detail += Environment.NewLine + "    + " + i;
				}

				for (int i = 0; i != terraincount; ++i)
				{
					if (!_used.Contains(new Tuple<string, int>(label, i)))
						detail += Environment.NewLine + "    - " + i;
				}
				detail += Environment.NewLine;
			}
			return detail;
		}
		#endregion Methods



		#region Designer
		private GroupBox gbInfo;
		private Label lbl1_Dimensions;
		private Label lbl2_Dimensions;
		private Label lbl1_Terrains;
		private Label lbl2_Terrains;
		private Label lbl1_PckSprites;
		private Label lbl2_PckSprites;
		private Label lbl1_McdRecords;
		private Label lbl2_McdRecords;
		private Label lbl1_QuadsUsed;
		private Label lbl2_QuadsFilled;
		private Label lbl1_TilesVacant;
		private Label lbl2_TilesVacant;
		private Button btnRefresh;
		private Button btnDetail;
		private Button btnCancel;

		private ProgressBar pBar;
		private GroupBox gbAnalyze;


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbl1_Dimensions = new System.Windows.Forms.Label();
			this.lbl2_Dimensions = new System.Windows.Forms.Label();
			this.lbl1_Terrains = new System.Windows.Forms.Label();
			this.lbl2_Terrains = new System.Windows.Forms.Label();
			this.lbl1_PckSprites = new System.Windows.Forms.Label();
			this.lbl2_PckSprites = new System.Windows.Forms.Label();
			this.lbl1_McdRecords = new System.Windows.Forms.Label();
			this.lbl2_McdRecords = new System.Windows.Forms.Label();
			this.lbl1_QuadsUsed = new System.Windows.Forms.Label();
			this.lbl2_QuadsFilled = new System.Windows.Forms.Label();
			this.lbl1_TilesVacant = new System.Windows.Forms.Label();
			this.lbl2_TilesVacant = new System.Windows.Forms.Label();
			this.pBar = new System.Windows.Forms.ProgressBar();
			this.gbAnalyze = new System.Windows.Forms.GroupBox();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.btnDetail = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbInfo = new System.Windows.Forms.GroupBox();
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
			// lbl1_Terrains
			// 
			this.lbl1_Terrains.Location = new System.Drawing.Point(10, 30);
			this.lbl1_Terrains.Name = "lbl1_Terrains";
			this.lbl1_Terrains.Size = new System.Drawing.Size(75, 15);
			this.lbl1_Terrains.TabIndex = 2;
			this.lbl1_Terrains.Text = "terrains";
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
			// lbl1_PckSprites
			// 
			this.lbl1_PckSprites.Location = new System.Drawing.Point(10, 45);
			this.lbl1_PckSprites.Name = "lbl1_PckSprites";
			this.lbl1_PckSprites.Size = new System.Drawing.Size(75, 15);
			this.lbl1_PckSprites.TabIndex = 4;
			this.lbl1_PckSprites.Text = "pck sprites";
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
			// lbl1_McdRecords
			// 
			this.lbl1_McdRecords.Location = new System.Drawing.Point(10, 60);
			this.lbl1_McdRecords.Name = "lbl1_McdRecords";
			this.lbl1_McdRecords.Size = new System.Drawing.Size(75, 15);
			this.lbl1_McdRecords.TabIndex = 6;
			this.lbl1_McdRecords.Text = "mcd records";
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
			// lbl1_QuadsUsed
			// 
			this.lbl1_QuadsUsed.Location = new System.Drawing.Point(10, 75);
			this.lbl1_QuadsUsed.Name = "lbl1_QuadsUsed";
			this.lbl1_QuadsUsed.Size = new System.Drawing.Size(75, 15);
			this.lbl1_QuadsUsed.TabIndex = 8;
			this.lbl1_QuadsUsed.Text = "quads used";
			// 
			// lbl2_QuadsFilled
			// 
			this.lbl2_QuadsFilled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2_QuadsFilled.Location = new System.Drawing.Point(85, 75);
			this.lbl2_QuadsFilled.Name = "lbl2_QuadsFilled";
			this.lbl2_QuadsFilled.Size = new System.Drawing.Size(250, 15);
			this.lbl2_QuadsFilled.TabIndex = 9;
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
			// pBar
			// 
			this.pBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pBar.Location = new System.Drawing.Point(5, 12);
			this.pBar.Name = "pBar";
			this.pBar.Size = new System.Drawing.Size(326, 27);
			this.pBar.TabIndex = 0;
			// 
			// gbAnalyze
			// 
			this.gbAnalyze.Controls.Add(this.pBar);
			this.gbAnalyze.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gbAnalyze.Location = new System.Drawing.Point(3, 105);
			this.gbAnalyze.Margin = new System.Windows.Forms.Padding(0);
			this.gbAnalyze.Name = "gbAnalyze";
			this.gbAnalyze.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
			this.gbAnalyze.Size = new System.Drawing.Size(336, 44);
			this.gbAnalyze.TabIndex = 1;
			this.gbAnalyze.TabStop = false;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRefresh.Location = new System.Drawing.Point(7, 126);
			this.btnRefresh.Margin = new System.Windows.Forms.Padding(0);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(85, 20);
			this.btnRefresh.TabIndex = 12;
			this.btnRefresh.Text = "refresh";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Visible = false;
			this.btnRefresh.Click += new System.EventHandler(this.click_btnRefresh);
			// 
			// btnDetail
			// 
			this.btnDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDetail.Location = new System.Drawing.Point(160, 116);
			this.btnDetail.Margin = new System.Windows.Forms.Padding(0);
			this.btnDetail.Name = "btnDetail";
			this.btnDetail.Size = new System.Drawing.Size(85, 30);
			this.btnDetail.TabIndex = 13;
			this.btnDetail.Text = "Detail";
			this.btnDetail.UseVisualStyleBackColor = true;
			this.btnDetail.Visible = false;
			this.btnDetail.Click += new System.EventHandler(this.click_btnTerrainDetail);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(250, 116);
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
			this.gbInfo.Controls.Add(this.lbl1_Dimensions);
			this.gbInfo.Controls.Add(this.lbl2_Dimensions);
			this.gbInfo.Controls.Add(this.lbl1_Terrains);
			this.gbInfo.Controls.Add(this.lbl2_Terrains);
			this.gbInfo.Controls.Add(this.lbl1_PckSprites);
			this.gbInfo.Controls.Add(this.lbl2_PckSprites);
			this.gbInfo.Controls.Add(this.lbl1_McdRecords);
			this.gbInfo.Controls.Add(this.lbl2_McdRecords);
			this.gbInfo.Controls.Add(this.lbl1_QuadsUsed);
			this.gbInfo.Controls.Add(this.lbl2_QuadsFilled);
			this.gbInfo.Controls.Add(this.lbl1_TilesVacant);
			this.gbInfo.Controls.Add(this.lbl2_TilesVacant);
			this.gbInfo.Controls.Add(this.btnRefresh);
			this.gbInfo.Controls.Add(this.btnDetail);
			this.gbInfo.Controls.Add(this.btnCancel);
			this.gbInfo.Controls.Add(this.gbAnalyze);
			this.gbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbInfo.Location = new System.Drawing.Point(0, 2);
			this.gbInfo.Margin = new System.Windows.Forms.Padding(0);
			this.gbInfo.Name = "gbInfo";
			this.gbInfo.Size = new System.Drawing.Size(342, 152);
			this.gbInfo.TabIndex = 0;
			this.gbInfo.TabStop = false;
			this.gbInfo.Text = " label ";
			// 
			// MapInfoDialog
			// 
			this.AcceptButton = this.btnDetail;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(342, 154);
			this.Controls.Add(this.gbInfo);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapInfoDialog";
			this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "MapInfo";
			this.gbAnalyze.ResumeLayout(false);
			this.gbInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
