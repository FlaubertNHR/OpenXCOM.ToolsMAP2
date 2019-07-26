using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView
{
	/// <summary>
	/// Colors help screen.
	/// </summary>
	internal sealed class ColorHelp
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ColorHelp()
		{
			InitializeComponent();

			var tpTabControl = new TabPageBorder(tabMain);

			label7   .Font =
			label8   .Font =
			label9   .Font =
			label10  .Font =
			label14  .Font =
			label15  .Font =
			label16  .Font =
			lblType00.Font =
			lblType01.Font =
			lblType02.Font =
			lblType03.Font =
			lblType04.Font =
			lblType05.Font =
			lblType06.Font =
			lblType07.Font =
			lblType08.Font =
			lblType09.Font =
			lblType10.Font =
			lblType11.Font =
			lblType12.Font =
			lblType13.Font =
			lblType14.Font = new Font(Font, FontStyle.Bold);

			OnCheckChanged(null, EventArgs.Empty);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Wraps the several color-updates into one call.
		/// </summary>
		internal void UpdateColors()
		{
			UpdateTopViewBlobColors();
			UpdateRouteViewBlobColors();
			UpdateSpecialPropertyColors();
		}

		/// <summary>
		/// Updates the TopView blob colors via an arcane methodology from the
		/// user's custom Options.
		/// </summary>
		private void UpdateTopViewBlobColors()
		{
			Color color;

			string key = TopViewOptionables.str_FloorColor;
			if (TopPanel.Brushes.ContainsKey(key))
			{
				color = TopPanel.Brushes[key].Color;
				label7.BackColor = color;
				label7.ForeColor = GetTextColor(color);
			}

			key = TopViewOptionables.str_WestColor;
			if (TopPanel.Pens.ContainsKey(key))
			{
				color = TopPanel.Pens[key].Color;
				label8.BackColor = color;
				label8.ForeColor = GetTextColor(color);
			}

			key = TopViewOptionables.str_NorthColor;
			if (TopPanel.Pens.ContainsKey(key))
			{
				color = TopPanel.Pens[key].Color;
				label9.BackColor = color;
				label9.ForeColor = GetTextColor(color);
			}

			key = TopViewOptionables.str_ContentColor;
			if (TopPanel.Brushes.ContainsKey(key))
			{
				color = TopPanel.Brushes[key].Color;
				label10.BackColor = color;
				label10.ForeColor = GetTextColor(color);
			}
		}

		/// <summary>
		/// Updates the RouteView blob colors via an arcane methodology from the
		/// user's custom Options.
		/// </summary>
		private void UpdateRouteViewBlobColors()
		{
			Color color;

			string key = RouteViewOptionables.str_WallColor;
			if (RoutePanel.RoutePens.ContainsKey(key))
			{
				color = RoutePanel.RoutePens[key].Color;

				label14.BackColor = color;
				label15.BackColor = color;

				label14.ForeColor = GetTextColor(color);
				label15.ForeColor = GetTextColor(color);
			}

			key = RouteViewOptionables.str_ContentColor;
			if (RoutePanel.RouteBrushes.ContainsKey(key))
			{
				color = RoutePanel.RouteBrushes[key].Color;
				label16.BackColor = color;
				label16.ForeColor = GetTextColor(color);
			}
		}

		/// <summary>
		/// Updates TileView's special property colors via an arcane methodology
		/// from the user's custom Options.
		/// </summary>
		private void UpdateSpecialPropertyColors()
		{
			// TODO: update special-property colors from Options without
			// requiring that the Help screen be reloaded. Neither form (Options
			// or Help) is modal, so the code can't rely on that user-forced
			// effect. A pointer to this ColorHelp needs to be passed to
			// TileViewOptionables.

			Color color;

			color = TilePanel.SpecialBrushes[SpecialType.Standard].Color;
			lblType00.BackColor = color;
			lblType00.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.EntryPoint].Color;
			lblType01.BackColor = color;
			lblType01.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.PowerSource].Color;
			lblType02.BackColor = color;
			lblType02.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Navigation].Color;
			lblType03.BackColor = color;
			lblType03.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Construction].Color;
			lblType04.BackColor = color;
			lblType04.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Food].Color;
			lblType05.BackColor = color;
			lblType05.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Reproduction].Color;
			lblType06.BackColor = color;
			lblType06.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Entertainment].Color;
			lblType07.BackColor = color;
			lblType07.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Surgery].Color;
			lblType08.BackColor = color;
			lblType08.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Examination].Color;
			lblType09.BackColor = color;
			lblType09.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Alloys].Color;
			lblType10.BackColor = color;
			lblType10.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Habitat].Color;
			lblType11.BackColor = color;
			lblType11.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.Destroyed].Color;
			lblType12.BackColor = color;
			lblType12.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.ExitPoint].Color;
			lblType13.BackColor = color;
			lblType13.ForeColor = GetTextColor(color);

			color = TilePanel.SpecialBrushes[SpecialType.MustDestroy].Color;
			lblType14.BackColor = color;
			lblType14.ForeColor = GetTextColor(color);
		}

		/// <summary>
		/// Gets a contrasting color based on the input color.
		/// @note Does not check alpha.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static Color GetTextColor(Color color)
		{
			return ((int)color.R + color.G + color.B > 485) ? Color.DarkSlateBlue
															: Color.Snow;
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Toggles the text descriptions between UFO and TFTD special property
		/// types.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCheckChanged(object sender, EventArgs e)
		{
			if (rbUfo.Checked)
			{
				lblType00.Text = "standard"; // switch to UFO ->
				lblType01.Text = "entry point";
				lblType02.Text = "power source";
				lblType03.Text = "navigation";
				lblType04.Text = "construction";
				lblType05.Text = "food";
				lblType06.Text = "reproduction";
				lblType07.Text = "entertainment";
				lblType08.Text = "surgery";
				lblType09.Text = "examination";
				lblType10.Text = "alloys";
				lblType11.Text = "habitat";
				lblType12.Text = "dead tile";
				lblType13.Text = "exit point";
				lblType14.Text = "must destroy";
			}
			else // rbTftd.Checked
			{
				lblType00.Text = "standard"; // switch to TFTD ->
				lblType01.Text = "entry point";
				lblType02.Text = "ion accelerator";
				lblType03.Text = "navigation";
				lblType04.Text = "construction";
				lblType05.Text = "cryogenics";
				lblType06.Text = "cloning";
				lblType07.Text = "learning arrays";
				lblType08.Text = "implanter";
				lblType09.Text = "examination";
				lblType10.Text = "plastics";
				lblType11.Text = "re-animation";
				lblType12.Text = "dead tile";
				lblType13.Text = "exit point";
				lblType14.Text = "must destroy";
			}
		}

		/// <summary>
		/// Closes the Help screen.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
		}
		#endregion Events



		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			this.tabMain = new DSShared.Windows.CompositedTabControl();
			this.tpTileView = new System.Windows.Forms.TabPage();
			this.label25 = new System.Windows.Forms.Label();
			this.rbTftd = new System.Windows.Forms.RadioButton();
			this.rbUfo = new System.Windows.Forms.RadioButton();
			this.gbTileViewColors = new System.Windows.Forms.GroupBox();
			this.lblType09 = new System.Windows.Forms.Label();
			this.lblType14 = new System.Windows.Forms.Label();
			this.lblType13 = new System.Windows.Forms.Label();
			this.lblType12 = new System.Windows.Forms.Label();
			this.lblType11 = new System.Windows.Forms.Label();
			this.lblType10 = new System.Windows.Forms.Label();
			this.lblType08 = new System.Windows.Forms.Label();
			this.lblType07 = new System.Windows.Forms.Label();
			this.lblType06 = new System.Windows.Forms.Label();
			this.lblType05 = new System.Windows.Forms.Label();
			this.lblType04 = new System.Windows.Forms.Label();
			this.lblType03 = new System.Windows.Forms.Label();
			this.lblType02 = new System.Windows.Forms.Label();
			this.lblType01 = new System.Windows.Forms.Label();
			this.lblType00 = new System.Windows.Forms.Label();
			this.tpTopView = new System.Windows.Forms.TabPage();
			this.label26 = new System.Windows.Forms.Label();
			this.gbTopViewColors = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.tpRouteView = new System.Windows.Forms.TabPage();
			this.gbRouteViewColors = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabMain.SuspendLayout();
			this.tpTileView.SuspendLayout();
			this.gbTileViewColors.SuspendLayout();
			this.tpTopView.SuspendLayout();
			this.gbTopViewColors.SuspendLayout();
			this.tpRouteView.SuspendLayout();
			this.gbRouteViewColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabMain
			// 
			this.tabMain.Controls.Add(this.tpTileView);
			this.tabMain.Controls.Add(this.tpTopView);
			this.tabMain.Controls.Add(this.tpRouteView);
			this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabMain.Location = new System.Drawing.Point(0, 0);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(454, 256);
			this.tabMain.TabIndex = 0;
			// 
			// tpTileView
			// 
			this.tpTileView.Controls.Add(this.label25);
			this.tpTileView.Controls.Add(this.rbTftd);
			this.tpTileView.Controls.Add(this.rbUfo);
			this.tpTileView.Controls.Add(this.gbTileViewColors);
			this.tpTileView.Location = new System.Drawing.Point(4, 21);
			this.tpTileView.Name = "tpTileView";
			this.tpTileView.Size = new System.Drawing.Size(446, 231);
			this.tpTileView.TabIndex = 3;
			this.tpTileView.Text = "TileView";
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(10, 10);
			this.label25.Margin = new System.Windows.Forms.Padding(0);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(315, 15);
			this.label25.TabIndex = 0;
			this.label25.Text = "These are the background colors for special properties.";
			// 
			// rbTftd
			// 
			this.rbTftd.Location = new System.Drawing.Point(20, 50);
			this.rbTftd.Name = "rbTftd";
			this.rbTftd.Size = new System.Drawing.Size(55, 15);
			this.rbTftd.TabIndex = 2;
			this.rbTftd.Text = "TFTD";
			this.rbTftd.UseVisualStyleBackColor = true;
			this.rbTftd.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
			// 
			// rbUfo
			// 
			this.rbUfo.Checked = true;
			this.rbUfo.Location = new System.Drawing.Point(20, 30);
			this.rbUfo.Name = "rbUfo";
			this.rbUfo.Size = new System.Drawing.Size(55, 15);
			this.rbUfo.TabIndex = 1;
			this.rbUfo.TabStop = true;
			this.rbUfo.Text = "UFO";
			this.rbUfo.UseVisualStyleBackColor = true;
			this.rbUfo.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
			// 
			// gbTileViewColors
			// 
			this.gbTileViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbTileViewColors.Controls.Add(this.lblType09);
			this.gbTileViewColors.Controls.Add(this.lblType14);
			this.gbTileViewColors.Controls.Add(this.lblType13);
			this.gbTileViewColors.Controls.Add(this.lblType12);
			this.gbTileViewColors.Controls.Add(this.lblType11);
			this.gbTileViewColors.Controls.Add(this.lblType10);
			this.gbTileViewColors.Controls.Add(this.lblType08);
			this.gbTileViewColors.Controls.Add(this.lblType07);
			this.gbTileViewColors.Controls.Add(this.lblType06);
			this.gbTileViewColors.Controls.Add(this.lblType05);
			this.gbTileViewColors.Controls.Add(this.lblType04);
			this.gbTileViewColors.Controls.Add(this.lblType03);
			this.gbTileViewColors.Controls.Add(this.lblType02);
			this.gbTileViewColors.Controls.Add(this.lblType01);
			this.gbTileViewColors.Controls.Add(this.lblType00);
			this.gbTileViewColors.Location = new System.Drawing.Point(10, 75);
			this.gbTileViewColors.Name = "gbTileViewColors";
			this.gbTileViewColors.Size = new System.Drawing.Size(430, 150);
			this.gbTileViewColors.TabIndex = 3;
			this.gbTileViewColors.TabStop = false;
			this.gbTileViewColors.Text = " Tilepart Colors ";
			// 
			// lblType09
			// 
			this.lblType09.Location = new System.Drawing.Point(10, 95);
			this.lblType09.Name = "lblType09";
			this.lblType09.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType09.Size = new System.Drawing.Size(130, 20);
			this.lblType09.TabIndex = 9;
			this.lblType09.Text = "09";
			this.lblType09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType14
			// 
			this.lblType14.Location = new System.Drawing.Point(290, 120);
			this.lblType14.Name = "lblType14";
			this.lblType14.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType14.Size = new System.Drawing.Size(130, 20);
			this.lblType14.TabIndex = 14;
			this.lblType14.Text = "14";
			this.lblType14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType13
			// 
			this.lblType13.Location = new System.Drawing.Point(150, 120);
			this.lblType13.Name = "lblType13";
			this.lblType13.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType13.Size = new System.Drawing.Size(130, 20);
			this.lblType13.TabIndex = 13;
			this.lblType13.Text = "13";
			this.lblType13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType12
			// 
			this.lblType12.Location = new System.Drawing.Point(10, 120);
			this.lblType12.Name = "lblType12";
			this.lblType12.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType12.Size = new System.Drawing.Size(130, 20);
			this.lblType12.TabIndex = 12;
			this.lblType12.Text = "12";
			this.lblType12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType11
			// 
			this.lblType11.Location = new System.Drawing.Point(290, 95);
			this.lblType11.Name = "lblType11";
			this.lblType11.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType11.Size = new System.Drawing.Size(130, 20);
			this.lblType11.TabIndex = 11;
			this.lblType11.Text = "11";
			this.lblType11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType10
			// 
			this.lblType10.Location = new System.Drawing.Point(150, 95);
			this.lblType10.Name = "lblType10";
			this.lblType10.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType10.Size = new System.Drawing.Size(130, 20);
			this.lblType10.TabIndex = 10;
			this.lblType10.Text = "10";
			this.lblType10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType08
			// 
			this.lblType08.Location = new System.Drawing.Point(290, 70);
			this.lblType08.Name = "lblType08";
			this.lblType08.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType08.Size = new System.Drawing.Size(130, 20);
			this.lblType08.TabIndex = 8;
			this.lblType08.Text = "08";
			this.lblType08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType07
			// 
			this.lblType07.Location = new System.Drawing.Point(150, 70);
			this.lblType07.Name = "lblType07";
			this.lblType07.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType07.Size = new System.Drawing.Size(130, 20);
			this.lblType07.TabIndex = 7;
			this.lblType07.Text = "07";
			this.lblType07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType06
			// 
			this.lblType06.Location = new System.Drawing.Point(10, 70);
			this.lblType06.Name = "lblType06";
			this.lblType06.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType06.Size = new System.Drawing.Size(130, 20);
			this.lblType06.TabIndex = 6;
			this.lblType06.Text = "06";
			this.lblType06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType05
			// 
			this.lblType05.Location = new System.Drawing.Point(290, 45);
			this.lblType05.Name = "lblType05";
			this.lblType05.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType05.Size = new System.Drawing.Size(130, 20);
			this.lblType05.TabIndex = 5;
			this.lblType05.Text = "05";
			this.lblType05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType04
			// 
			this.lblType04.Location = new System.Drawing.Point(150, 45);
			this.lblType04.Name = "lblType04";
			this.lblType04.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType04.Size = new System.Drawing.Size(130, 20);
			this.lblType04.TabIndex = 4;
			this.lblType04.Text = "04";
			this.lblType04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType03
			// 
			this.lblType03.Location = new System.Drawing.Point(10, 45);
			this.lblType03.Name = "lblType03";
			this.lblType03.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType03.Size = new System.Drawing.Size(130, 20);
			this.lblType03.TabIndex = 3;
			this.lblType03.Text = "03";
			this.lblType03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType02
			// 
			this.lblType02.Location = new System.Drawing.Point(290, 20);
			this.lblType02.Name = "lblType02";
			this.lblType02.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType02.Size = new System.Drawing.Size(130, 20);
			this.lblType02.TabIndex = 2;
			this.lblType02.Text = "02";
			this.lblType02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType01
			// 
			this.lblType01.Location = new System.Drawing.Point(150, 20);
			this.lblType01.Name = "lblType01";
			this.lblType01.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType01.Size = new System.Drawing.Size(130, 20);
			this.lblType01.TabIndex = 1;
			this.lblType01.Text = "01";
			this.lblType01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType00
			// 
			this.lblType00.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblType00.Location = new System.Drawing.Point(10, 20);
			this.lblType00.Name = "lblType00";
			this.lblType00.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblType00.Size = new System.Drawing.Size(130, 20);
			this.lblType00.TabIndex = 0;
			this.lblType00.Text = "00";
			this.lblType00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tpTopView
			// 
			this.tpTopView.Controls.Add(this.label26);
			this.tpTopView.Controls.Add(this.gbTopViewColors);
			this.tpTopView.Location = new System.Drawing.Point(4, 21);
			this.tpTopView.Name = "tpTopView";
			this.tpTopView.Size = new System.Drawing.Size(446, 231);
			this.tpTopView.TabIndex = 1;
			this.tpTopView.Text = "TopView";
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(10, 195);
			this.label26.Margin = new System.Windows.Forms.Padding(0);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(425, 25);
			this.label26.TabIndex = 1;
			this.label26.Text = "The Colors viewer must be closed and re-opened to update any colors that have bee" +
	"n changed in Options.";
			// 
			// gbTopViewColors
			// 
			this.gbTopViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbTopViewColors.Controls.Add(this.label7);
			this.gbTopViewColors.Controls.Add(this.label9);
			this.gbTopViewColors.Controls.Add(this.label10);
			this.gbTopViewColors.Controls.Add(this.label8);
			this.gbTopViewColors.Location = new System.Drawing.Point(10, 10);
			this.gbTopViewColors.Name = "gbTopViewColors";
			this.gbTopViewColors.Size = new System.Drawing.Size(430, 55);
			this.gbTopViewColors.TabIndex = 0;
			this.gbTopViewColors.TabStop = false;
			this.gbTopViewColors.Text = " Blob Colors ";
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label7.Location = new System.Drawing.Point(10, 20);
			this.label7.Name = "label7";
			this.label7.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label7.Size = new System.Drawing.Size(95, 25);
			this.label7.TabIndex = 0;
			this.label7.Text = "floor";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label9.Location = new System.Drawing.Point(220, 20);
			this.label9.Name = "label9";
			this.label9.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label9.Size = new System.Drawing.Size(95, 25);
			this.label9.TabIndex = 2;
			this.label9.Text = "north";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label10.Location = new System.Drawing.Point(325, 20);
			this.label10.Name = "label10";
			this.label10.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label10.Size = new System.Drawing.Size(95, 25);
			this.label10.TabIndex = 3;
			this.label10.Text = "content";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label8.Location = new System.Drawing.Point(115, 20);
			this.label8.Name = "label8";
			this.label8.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label8.Size = new System.Drawing.Size(95, 25);
			this.label8.TabIndex = 1;
			this.label8.Text = "west";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tpRouteView
			// 
			this.tpRouteView.Controls.Add(this.label1);
			this.tpRouteView.Controls.Add(this.gbRouteViewColors);
			this.tpRouteView.Location = new System.Drawing.Point(4, 21);
			this.tpRouteView.Name = "tpRouteView";
			this.tpRouteView.Size = new System.Drawing.Size(446, 231);
			this.tpRouteView.TabIndex = 2;
			this.tpRouteView.Text = "RouteView";
			// 
			// gbRouteViewColors
			// 
			this.gbRouteViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbRouteViewColors.Controls.Add(this.label16);
			this.gbRouteViewColors.Controls.Add(this.label15);
			this.gbRouteViewColors.Controls.Add(this.label14);
			this.gbRouteViewColors.Location = new System.Drawing.Point(10, 10);
			this.gbRouteViewColors.Name = "gbRouteViewColors";
			this.gbRouteViewColors.Size = new System.Drawing.Size(325, 55);
			this.gbRouteViewColors.TabIndex = 0;
			this.gbRouteViewColors.TabStop = false;
			this.gbRouteViewColors.Text = " Blob Colors ";
			// 
			// label16
			// 
			this.label16.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label16.Location = new System.Drawing.Point(220, 20);
			this.label16.Name = "label16";
			this.label16.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label16.Size = new System.Drawing.Size(95, 25);
			this.label16.TabIndex = 2;
			this.label16.Text = "content";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label15.Location = new System.Drawing.Point(115, 20);
			this.label15.Name = "label15";
			this.label15.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label15.Size = new System.Drawing.Size(95, 25);
			this.label15.TabIndex = 1;
			this.label15.Text = "north";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label14
			// 
			this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label14.Location = new System.Drawing.Point(10, 20);
			this.label14.Name = "label14";
			this.label14.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label14.Size = new System.Drawing.Size(95, 25);
			this.label14.TabIndex = 0;
			this.label14.Text = "west";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 195);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(425, 25);
			this.label1.TabIndex = 2;
			this.label1.Text = "The Colors viewer must be closed and re-opened to update any colors that have bee" +
	"n changed in Options.";
			// 
			// ColorHelp
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(454, 256);
			this.Controls.Add(this.tabMain);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColorHelp";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = " Colors";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.tabMain.ResumeLayout(false);
			this.tpTileView.ResumeLayout(false);
			this.gbTileViewColors.ResumeLayout(false);
			this.tpTopView.ResumeLayout(false);
			this.gbTopViewColors.ResumeLayout(false);
			this.tpRouteView.ResumeLayout(false);
			this.gbRouteViewColors.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private CompositedTabControl tabMain;
		private TabPage tpTopView;
		private TabPage tpRouteView;
		private TabPage tpTileView;
		private Label label1;
		private Label label7;
		private Label label8;
		private Label label9;
		private Label label10;
		private Label label14;
		private Label label15;
		private Label label16;
		private Label label25;
		private Label label26;
		private GroupBox gbTileViewColors;
		private Label lblType00;
		private Label lblType01;
		private Label lblType02;
		private Label lblType03;
		private Label lblType04;
		private Label lblType05;
		private Label lblType06;
		private Label lblType07;
		private Label lblType08;
		private Label lblType09;
		private Label lblType10;
		private Label lblType11;
		private Label lblType12;
		private Label lblType13;
		private Label lblType14;
		private RadioButton rbTftd;
		private RadioButton rbUfo;
		private GroupBox gbTopViewColors;
		private GroupBox gbRouteViewColors;
		#endregion Designer
	}
}
