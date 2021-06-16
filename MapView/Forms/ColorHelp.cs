using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;
using MapView.Forms.Observers;

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
		#region Fields
		private Font _fontBold;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ColorHelp()
		{
			InitializeComponent();
			var tpBorder = new TabPageBorder(tabControl);
			tpBorder.TabPageBorder_init();

			_fontBold = new Font(Font, FontStyle.Bold);

			la_TopFloor  .Font = la_TopWest   .Font = la_TopNorth    .Font =
			la_TopContent.Font = la_RouteWalls.Font = la_RouteContent.Font =

			la_Type00.Font = la_Type01.Font = la_Type02.Font = la_Type03.Font =
			la_Type04.Font = la_Type05.Font = la_Type06.Font = la_Type07.Font =
			la_Type08.Font = la_Type09.Font = la_Type10.Font = la_Type11.Font =
			la_Type12.Font = la_Type13.Font = la_Type14.Font = _fontBold;

			UpdateColors();

			rb_OnCheckChanged(null, EventArgs.Empty);

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let ClientSize deter width and height.
				Left = 200;
				Top  = 100;
			}
			Show(); // no owner.
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Handles the activated event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}

		/// <summary>
		/// Handles the formclosing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				RegistryInfo.UpdateRegistry(this);

				_fontBold.Dispose();

				if (!MainViewF.Quit)
					MainViewF.that.DecheckColors();
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Updates colors that user could have changed in Options.
		/// </summary>
		/// <remarks>Wraps the several color-updates into one call.</remarks>
		private void UpdateColors()
		{
			UpdateSpecialPropertyColors();
			UpdateTopViewBlobColors();
			UpdateRouteViewBlobColors();
		}

		/// <summary>
		/// Updates TileView's special property colors via an arcane methodology
		/// from the user's Options. But I got rid of the arcane methodology
		/// even though it was faster.
		/// </summary>
		internal void UpdateSpecialPropertyColors()
		{
			Color color;

			color = TilePanel.SpecialBrushes[SpecialType.Standard].Color;
			la_Type00.ForeColor = GetTextColor(la_Type00.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.EntryPoint].Color;
			la_Type01.ForeColor = GetTextColor(la_Type01.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.PowerSource].Color;
			la_Type02.ForeColor = GetTextColor(la_Type02.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Navigation].Color;
			la_Type03.ForeColor = GetTextColor(la_Type03.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Construction].Color;
			la_Type04.ForeColor = GetTextColor(la_Type04.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Food].Color;
			la_Type05.ForeColor = GetTextColor(la_Type05.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Reproduction].Color;
			la_Type06.ForeColor = GetTextColor(la_Type06.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Entertainment].Color;
			la_Type07.ForeColor = GetTextColor(la_Type07.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Surgery].Color;
			la_Type08.ForeColor = GetTextColor(la_Type08.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Examination].Color;
			la_Type09.ForeColor = GetTextColor(la_Type09.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Alloys].Color;
			la_Type10.ForeColor = GetTextColor(la_Type10.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Habitat].Color;
			la_Type11.ForeColor = GetTextColor(la_Type11.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.Destroyed].Color;
			la_Type12.ForeColor = GetTextColor(la_Type12.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.ExitPoint].Color;
			la_Type13.ForeColor = GetTextColor(la_Type13.BackColor = color);

			color = TilePanel.SpecialBrushes[SpecialType.MustDestroy].Color;
			la_Type14.ForeColor = GetTextColor(la_Type14.BackColor = color);
		}

		/// <summary>
		/// Updates the TopView blob colors via an arcane methodology from the
		/// user's Options. But I got rid of the arcane methodology even though
		/// it was faster.
		/// </summary>
		internal void UpdateTopViewBlobColors()
		{
			Color color;

			color = TopControl.TopBrushes[TopViewOptionables.str_FloorColor].Color;
			la_TopFloor.ForeColor = GetTextColor(la_TopFloor.BackColor = color);

			color = TopControl.TopPens[TopViewOptionables.str_WestColor].Color;
			la_TopWest.ForeColor = GetTextColor(la_TopWest.BackColor = color);

			color = TopControl.TopPens[TopViewOptionables.str_NorthColor].Color;
			la_TopNorth.ForeColor = GetTextColor(la_TopNorth.BackColor = color);

			color = TopControl.TopBrushes[TopViewOptionables.str_ContentColor].Color;
			la_TopContent.ForeColor = GetTextColor(la_TopContent.BackColor = color);
		}

		/// <summary>
		/// Updates the RouteView blob colors via an arcane methodology from the
		/// user's Options. But I got rid of the arcane methodology even though
		/// it was faster.
		/// </summary>
		internal void UpdateRouteViewBlobColors()
		{
			Color color;

			color = RouteControl.RoutePens[RouteViewOptionables.str_WallColor].Color;
			la_RouteWalls.ForeColor = GetTextColor(la_RouteWalls.BackColor = color);

			color = RouteControl.RouteBrushes[RouteViewOptionables.str_ContentColor].Color;
			la_RouteContent.ForeColor = GetTextColor(la_RouteContent.BackColor = color);
		}

		/// <summary>
		/// Gets a contrasting color based on the input color.
		/// See also
		/// <c><see cref="XCom.Palette">Palette.CreateTonescaledPalettes()</see></c>.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		/// <remarks>Does not check alpha.</remarks>
		private static Color GetTextColor(Color color)
		{
			if ((int)color.R + color.G + color.B > 485)
				return Color.DarkSlateBlue;
			return Color.Snow;

//			if ((int)(color.R * 0.2990 + color.G * 0.5870 + color.B * 0.1140) > 150)
/*			if ((int)(color.R * 0.2126 + color.G * 0.7152 + color.B * 0.0722) > 150)
			{
				return Color.DarkSlateBlue;
			}
			return Color.Snow; */ // My eyes are weird -> drop LSD.
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Toggles the text descriptions between UFO and TFTD special property
		/// types.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rb_OnCheckChanged(object sender, EventArgs e)
		{
			if (rbUfo.Checked)
			{
				la_Type00.Text = "standard"; // switch to UFO ->
				la_Type01.Text = "entry point";
				la_Type02.Text = "power source";
				la_Type03.Text = "navigation";
				la_Type04.Text = "construction";
				la_Type05.Text = "food";
				la_Type06.Text = "reproduction";
				la_Type07.Text = "entertainment";
				la_Type08.Text = "surgery";
				la_Type09.Text = "examination";
				la_Type10.Text = "alloys";
				la_Type11.Text = "habitat";
				la_Type12.Text = "dead tile";
				la_Type13.Text = "exit point";
				la_Type14.Text = "must destroy";
			}
			else // rbTftd.Checked
			{
				la_Type00.Text = "standard"; // switch to TFTD ->
				la_Type01.Text = "entry point";
				la_Type02.Text = "ion accelerator";
				la_Type03.Text = "navigation";
				la_Type04.Text = "construction";
				la_Type05.Text = "cryogenics";
				la_Type06.Text = "cloning";
				la_Type07.Text = "learning arrays";
				la_Type08.Text = "implanter";
				la_Type09.Text = "examination";
				la_Type10.Text = "plastics";
				la_Type11.Text = "re-animation";
				la_Type12.Text = "dead tile";
				la_Type13.Text = "exit point";
				la_Type14.Text = "must destroy";
			}
		}
		#endregion Events


		#region Events (override)
		/// <summary>
		/// Closes this <c>ColorHelp</c> dialog.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.Enter:
				case Keys.Control | Keys.H:
					Close();
					break;
			}
		}
		#endregion Events (override)



		#region Designer
		private CompositedTabControl tabControl;

		private TabPage tpTileView;
		private Label la_TileHead;
		private RadioButton rbUfo;
		private RadioButton rbTftd;
		private GroupBox gbTileViewColors;
		private Label la_Type00;
		private Label la_Type01;
		private Label la_Type02;
		private Label la_Type03;
		private Label la_Type04;
		private Label la_Type05;
		private Label la_Type06;
		private Label la_Type07;
		private Label la_Type08;
		private Label la_Type09;
		private Label la_Type10;
		private Label la_Type11;
		private Label la_Type12;
		private Label la_Type13;
		private Label la_Type14;

		private TabPage tpTopRouteView;
		private Label la_TopHead;
		private GroupBox gbTopViewColors;
		private Label la_TopFloor;
		private Label la_TopWest;
		private Label la_TopNorth;
		private Label la_TopContent;
		private Label la_RouteHead;
		private GroupBox gbRouteViewColors;
		private Label la_RouteWalls;
		private Label la_RouteContent;


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl = new DSShared.Controls.CompositedTabControl();
			this.tpTileView = new System.Windows.Forms.TabPage();
			this.la_TileHead = new System.Windows.Forms.Label();
			this.rbUfo = new System.Windows.Forms.RadioButton();
			this.rbTftd = new System.Windows.Forms.RadioButton();
			this.gbTileViewColors = new System.Windows.Forms.GroupBox();
			this.la_Type00 = new System.Windows.Forms.Label();
			this.la_Type01 = new System.Windows.Forms.Label();
			this.la_Type02 = new System.Windows.Forms.Label();
			this.la_Type03 = new System.Windows.Forms.Label();
			this.la_Type04 = new System.Windows.Forms.Label();
			this.la_Type05 = new System.Windows.Forms.Label();
			this.la_Type06 = new System.Windows.Forms.Label();
			this.la_Type07 = new System.Windows.Forms.Label();
			this.la_Type08 = new System.Windows.Forms.Label();
			this.la_Type09 = new System.Windows.Forms.Label();
			this.la_Type10 = new System.Windows.Forms.Label();
			this.la_Type11 = new System.Windows.Forms.Label();
			this.la_Type12 = new System.Windows.Forms.Label();
			this.la_Type13 = new System.Windows.Forms.Label();
			this.la_Type14 = new System.Windows.Forms.Label();
			this.tpTopRouteView = new System.Windows.Forms.TabPage();
			this.la_TopHead = new System.Windows.Forms.Label();
			this.gbTopViewColors = new System.Windows.Forms.GroupBox();
			this.la_TopFloor = new System.Windows.Forms.Label();
			this.la_TopWest = new System.Windows.Forms.Label();
			this.la_TopNorth = new System.Windows.Forms.Label();
			this.la_TopContent = new System.Windows.Forms.Label();
			this.la_RouteHead = new System.Windows.Forms.Label();
			this.gbRouteViewColors = new System.Windows.Forms.GroupBox();
			this.la_RouteWalls = new System.Windows.Forms.Label();
			this.la_RouteContent = new System.Windows.Forms.Label();
			this.tabControl.SuspendLayout();
			this.tpTileView.SuspendLayout();
			this.gbTileViewColors.SuspendLayout();
			this.tpTopRouteView.SuspendLayout();
			this.gbTopViewColors.SuspendLayout();
			this.gbRouteViewColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tpTileView);
			this.tabControl.Controls.Add(this.tpTopRouteView);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Margin = new System.Windows.Forms.Padding(0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(454, 256);
			this.tabControl.TabIndex = 0;
			// 
			// tpTileView
			// 
			this.tpTileView.Controls.Add(this.la_TileHead);
			this.tpTileView.Controls.Add(this.rbUfo);
			this.tpTileView.Controls.Add(this.rbTftd);
			this.tpTileView.Controls.Add(this.gbTileViewColors);
			this.tpTileView.Location = new System.Drawing.Point(4, 21);
			this.tpTileView.Margin = new System.Windows.Forms.Padding(0);
			this.tpTileView.Name = "tpTileView";
			this.tpTileView.Size = new System.Drawing.Size(446, 231);
			this.tpTileView.TabIndex = 0;
			this.tpTileView.Text = "TileView";
			// 
			// la_TileHead
			// 
			this.la_TileHead.Location = new System.Drawing.Point(10, 10);
			this.la_TileHead.Margin = new System.Windows.Forms.Padding(0);
			this.la_TileHead.Name = "la_TileHead";
			this.la_TileHead.Size = new System.Drawing.Size(315, 15);
			this.la_TileHead.TabIndex = 0;
			this.la_TileHead.Text = "These are the background colors for special properties.";
			// 
			// rbUfo
			// 
			this.rbUfo.Checked = true;
			this.rbUfo.Location = new System.Drawing.Point(20, 30);
			this.rbUfo.Margin = new System.Windows.Forms.Padding(0);
			this.rbUfo.Name = "rbUfo";
			this.rbUfo.Size = new System.Drawing.Size(55, 15);
			this.rbUfo.TabIndex = 1;
			this.rbUfo.TabStop = true;
			this.rbUfo.Text = "UFO";
			this.rbUfo.UseVisualStyleBackColor = true;
			this.rbUfo.CheckedChanged += new System.EventHandler(this.rb_OnCheckChanged);
			// 
			// rbTftd
			// 
			this.rbTftd.Location = new System.Drawing.Point(20, 50);
			this.rbTftd.Margin = new System.Windows.Forms.Padding(0);
			this.rbTftd.Name = "rbTftd";
			this.rbTftd.Size = new System.Drawing.Size(55, 15);
			this.rbTftd.TabIndex = 2;
			this.rbTftd.Text = "TFTD";
			this.rbTftd.UseVisualStyleBackColor = true;
			this.rbTftd.CheckedChanged += new System.EventHandler(this.rb_OnCheckChanged);
			// 
			// gbTileViewColors
			// 
			this.gbTileViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbTileViewColors.Controls.Add(this.la_Type00);
			this.gbTileViewColors.Controls.Add(this.la_Type01);
			this.gbTileViewColors.Controls.Add(this.la_Type02);
			this.gbTileViewColors.Controls.Add(this.la_Type03);
			this.gbTileViewColors.Controls.Add(this.la_Type04);
			this.gbTileViewColors.Controls.Add(this.la_Type05);
			this.gbTileViewColors.Controls.Add(this.la_Type06);
			this.gbTileViewColors.Controls.Add(this.la_Type07);
			this.gbTileViewColors.Controls.Add(this.la_Type08);
			this.gbTileViewColors.Controls.Add(this.la_Type09);
			this.gbTileViewColors.Controls.Add(this.la_Type10);
			this.gbTileViewColors.Controls.Add(this.la_Type11);
			this.gbTileViewColors.Controls.Add(this.la_Type12);
			this.gbTileViewColors.Controls.Add(this.la_Type13);
			this.gbTileViewColors.Controls.Add(this.la_Type14);
			this.gbTileViewColors.Location = new System.Drawing.Point(10, 75);
			this.gbTileViewColors.Margin = new System.Windows.Forms.Padding(0);
			this.gbTileViewColors.Name = "gbTileViewColors";
			this.gbTileViewColors.Size = new System.Drawing.Size(430, 150);
			this.gbTileViewColors.TabIndex = 3;
			this.gbTileViewColors.TabStop = false;
			this.gbTileViewColors.Text = " Tilepart Colors ";
			// 
			// la_Type00
			// 
			this.la_Type00.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_Type00.Location = new System.Drawing.Point(10, 20);
			this.la_Type00.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type00.Name = "la_Type00";
			this.la_Type00.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type00.Size = new System.Drawing.Size(130, 20);
			this.la_Type00.TabIndex = 0;
			this.la_Type00.Text = "00";
			this.la_Type00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type01
			// 
			this.la_Type01.Location = new System.Drawing.Point(150, 20);
			this.la_Type01.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type01.Name = "la_Type01";
			this.la_Type01.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type01.Size = new System.Drawing.Size(130, 20);
			this.la_Type01.TabIndex = 1;
			this.la_Type01.Text = "01";
			this.la_Type01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type02
			// 
			this.la_Type02.Location = new System.Drawing.Point(290, 20);
			this.la_Type02.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type02.Name = "la_Type02";
			this.la_Type02.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type02.Size = new System.Drawing.Size(130, 20);
			this.la_Type02.TabIndex = 2;
			this.la_Type02.Text = "02";
			this.la_Type02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type03
			// 
			this.la_Type03.Location = new System.Drawing.Point(10, 45);
			this.la_Type03.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type03.Name = "la_Type03";
			this.la_Type03.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type03.Size = new System.Drawing.Size(130, 20);
			this.la_Type03.TabIndex = 3;
			this.la_Type03.Text = "03";
			this.la_Type03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type04
			// 
			this.la_Type04.Location = new System.Drawing.Point(150, 45);
			this.la_Type04.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type04.Name = "la_Type04";
			this.la_Type04.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type04.Size = new System.Drawing.Size(130, 20);
			this.la_Type04.TabIndex = 4;
			this.la_Type04.Text = "04";
			this.la_Type04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type05
			// 
			this.la_Type05.Location = new System.Drawing.Point(290, 45);
			this.la_Type05.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type05.Name = "la_Type05";
			this.la_Type05.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type05.Size = new System.Drawing.Size(130, 20);
			this.la_Type05.TabIndex = 5;
			this.la_Type05.Text = "05";
			this.la_Type05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type06
			// 
			this.la_Type06.Location = new System.Drawing.Point(10, 70);
			this.la_Type06.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type06.Name = "la_Type06";
			this.la_Type06.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type06.Size = new System.Drawing.Size(130, 20);
			this.la_Type06.TabIndex = 6;
			this.la_Type06.Text = "06";
			this.la_Type06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type07
			// 
			this.la_Type07.Location = new System.Drawing.Point(150, 70);
			this.la_Type07.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type07.Name = "la_Type07";
			this.la_Type07.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type07.Size = new System.Drawing.Size(130, 20);
			this.la_Type07.TabIndex = 7;
			this.la_Type07.Text = "07";
			this.la_Type07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type08
			// 
			this.la_Type08.Location = new System.Drawing.Point(290, 70);
			this.la_Type08.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type08.Name = "la_Type08";
			this.la_Type08.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type08.Size = new System.Drawing.Size(130, 20);
			this.la_Type08.TabIndex = 8;
			this.la_Type08.Text = "08";
			this.la_Type08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type09
			// 
			this.la_Type09.Location = new System.Drawing.Point(10, 95);
			this.la_Type09.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type09.Name = "la_Type09";
			this.la_Type09.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type09.Size = new System.Drawing.Size(130, 20);
			this.la_Type09.TabIndex = 9;
			this.la_Type09.Text = "09";
			this.la_Type09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type10
			// 
			this.la_Type10.Location = new System.Drawing.Point(150, 95);
			this.la_Type10.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type10.Name = "la_Type10";
			this.la_Type10.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type10.Size = new System.Drawing.Size(130, 20);
			this.la_Type10.TabIndex = 10;
			this.la_Type10.Text = "10";
			this.la_Type10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type11
			// 
			this.la_Type11.Location = new System.Drawing.Point(290, 95);
			this.la_Type11.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type11.Name = "la_Type11";
			this.la_Type11.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type11.Size = new System.Drawing.Size(130, 20);
			this.la_Type11.TabIndex = 11;
			this.la_Type11.Text = "11";
			this.la_Type11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type12
			// 
			this.la_Type12.Location = new System.Drawing.Point(10, 120);
			this.la_Type12.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type12.Name = "la_Type12";
			this.la_Type12.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type12.Size = new System.Drawing.Size(130, 20);
			this.la_Type12.TabIndex = 12;
			this.la_Type12.Text = "12";
			this.la_Type12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type13
			// 
			this.la_Type13.Location = new System.Drawing.Point(150, 120);
			this.la_Type13.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type13.Name = "la_Type13";
			this.la_Type13.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type13.Size = new System.Drawing.Size(130, 20);
			this.la_Type13.TabIndex = 13;
			this.la_Type13.Text = "13";
			this.la_Type13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_Type14
			// 
			this.la_Type14.Location = new System.Drawing.Point(290, 120);
			this.la_Type14.Margin = new System.Windows.Forms.Padding(0);
			this.la_Type14.Name = "la_Type14";
			this.la_Type14.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_Type14.Size = new System.Drawing.Size(130, 20);
			this.la_Type14.TabIndex = 14;
			this.la_Type14.Text = "14";
			this.la_Type14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tpTopRouteView
			// 
			this.tpTopRouteView.Controls.Add(this.la_TopHead);
			this.tpTopRouteView.Controls.Add(this.gbTopViewColors);
			this.tpTopRouteView.Controls.Add(this.la_RouteHead);
			this.tpTopRouteView.Controls.Add(this.gbRouteViewColors);
			this.tpTopRouteView.Location = new System.Drawing.Point(4, 22);
			this.tpTopRouteView.Margin = new System.Windows.Forms.Padding(0);
			this.tpTopRouteView.Name = "tpTopRouteView";
			this.tpTopRouteView.Size = new System.Drawing.Size(446, 230);
			this.tpTopRouteView.TabIndex = 1;
			this.tpTopRouteView.Text = "Top/RouteView";
			// 
			// la_TopHead
			// 
			this.la_TopHead.Location = new System.Drawing.Point(10, 10);
			this.la_TopHead.Margin = new System.Windows.Forms.Padding(0);
			this.la_TopHead.Name = "la_TopHead";
			this.la_TopHead.Size = new System.Drawing.Size(235, 15);
			this.la_TopHead.TabIndex = 0;
			this.la_TopHead.Text = "These are the blob colors for TopView.";
			// 
			// gbTopViewColors
			// 
			this.gbTopViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbTopViewColors.Controls.Add(this.la_TopFloor);
			this.gbTopViewColors.Controls.Add(this.la_TopWest);
			this.gbTopViewColors.Controls.Add(this.la_TopNorth);
			this.gbTopViewColors.Controls.Add(this.la_TopContent);
			this.gbTopViewColors.Location = new System.Drawing.Point(10, 30);
			this.gbTopViewColors.Margin = new System.Windows.Forms.Padding(0);
			this.gbTopViewColors.Name = "gbTopViewColors";
			this.gbTopViewColors.Size = new System.Drawing.Size(430, 55);
			this.gbTopViewColors.TabIndex = 1;
			this.gbTopViewColors.TabStop = false;
			this.gbTopViewColors.Text = " Blob Colors ";
			// 
			// la_TopFloor
			// 
			this.la_TopFloor.BackColor = System.Drawing.SystemColors.ControlLight;
			this.la_TopFloor.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_TopFloor.Location = new System.Drawing.Point(10, 20);
			this.la_TopFloor.Margin = new System.Windows.Forms.Padding(0);
			this.la_TopFloor.Name = "la_TopFloor";
			this.la_TopFloor.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_TopFloor.Size = new System.Drawing.Size(95, 25);
			this.la_TopFloor.TabIndex = 0;
			this.la_TopFloor.Text = "floor";
			this.la_TopFloor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_TopWest
			// 
			this.la_TopWest.BackColor = System.Drawing.SystemColors.ControlLight;
			this.la_TopWest.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_TopWest.Location = new System.Drawing.Point(115, 20);
			this.la_TopWest.Margin = new System.Windows.Forms.Padding(0);
			this.la_TopWest.Name = "la_TopWest";
			this.la_TopWest.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_TopWest.Size = new System.Drawing.Size(95, 25);
			this.la_TopWest.TabIndex = 1;
			this.la_TopWest.Text = "west";
			this.la_TopWest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_TopNorth
			// 
			this.la_TopNorth.BackColor = System.Drawing.SystemColors.ControlLight;
			this.la_TopNorth.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_TopNorth.Location = new System.Drawing.Point(220, 20);
			this.la_TopNorth.Margin = new System.Windows.Forms.Padding(0);
			this.la_TopNorth.Name = "la_TopNorth";
			this.la_TopNorth.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_TopNorth.Size = new System.Drawing.Size(95, 25);
			this.la_TopNorth.TabIndex = 2;
			this.la_TopNorth.Text = "north";
			this.la_TopNorth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_TopContent
			// 
			this.la_TopContent.BackColor = System.Drawing.SystemColors.ControlLight;
			this.la_TopContent.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_TopContent.Location = new System.Drawing.Point(325, 20);
			this.la_TopContent.Margin = new System.Windows.Forms.Padding(0);
			this.la_TopContent.Name = "la_TopContent";
			this.la_TopContent.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_TopContent.Size = new System.Drawing.Size(95, 25);
			this.la_TopContent.TabIndex = 3;
			this.la_TopContent.Text = "content";
			this.la_TopContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_RouteHead
			// 
			this.la_RouteHead.Location = new System.Drawing.Point(10, 95);
			this.la_RouteHead.Margin = new System.Windows.Forms.Padding(0);
			this.la_RouteHead.Name = "la_RouteHead";
			this.la_RouteHead.Size = new System.Drawing.Size(235, 15);
			this.la_RouteHead.TabIndex = 2;
			this.la_RouteHead.Text = "These are the blob colors for RouteView.";
			// 
			// gbRouteViewColors
			// 
			this.gbRouteViewColors.BackColor = System.Drawing.SystemColors.ControlLight;
			this.gbRouteViewColors.Controls.Add(this.la_RouteWalls);
			this.gbRouteViewColors.Controls.Add(this.la_RouteContent);
			this.gbRouteViewColors.Location = new System.Drawing.Point(10, 115);
			this.gbRouteViewColors.Name = "gbRouteViewColors";
			this.gbRouteViewColors.Size = new System.Drawing.Size(220, 55);
			this.gbRouteViewColors.TabIndex = 3;
			this.gbRouteViewColors.TabStop = false;
			this.gbRouteViewColors.Text = " Blob Colors ";
			// 
			// la_RouteWalls
			// 
			this.la_RouteWalls.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_RouteWalls.Location = new System.Drawing.Point(10, 20);
			this.la_RouteWalls.Margin = new System.Windows.Forms.Padding(0);
			this.la_RouteWalls.Name = "la_RouteWalls";
			this.la_RouteWalls.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_RouteWalls.Size = new System.Drawing.Size(95, 25);
			this.la_RouteWalls.TabIndex = 0;
			this.la_RouteWalls.Text = "walls";
			this.la_RouteWalls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_RouteContent
			// 
			this.la_RouteContent.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_RouteContent.Location = new System.Drawing.Point(115, 20);
			this.la_RouteContent.Margin = new System.Windows.Forms.Padding(0);
			this.la_RouteContent.Name = "la_RouteContent";
			this.la_RouteContent.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_RouteContent.Size = new System.Drawing.Size(95, 25);
			this.la_RouteContent.TabIndex = 1;
			this.la_RouteContent.Text = "content";
			this.la_RouteContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ColorHelp
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(454, 256);
			this.Controls.Add(this.tabControl);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColorHelp";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Colors";
			this.tabControl.ResumeLayout(false);
			this.tpTileView.ResumeLayout(false);
			this.gbTileViewColors.ResumeLayout(false);
			this.tpTopRouteView.ResumeLayout(false);
			this.gbTopViewColors.ResumeLayout(false);
			this.gbRouteViewColors.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
