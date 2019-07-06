using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopView
		:
			MapObserverControl // UserControl, IMapObserver
	{
		#region Properties
		internal TopPanel TopPanel
		{ get; private set; }

		internal QuadrantPanel QuadrantPanel
		{
			get { return quadrants; }
		}


		internal ToolStripMenuItem Floor
		{ get; private set; }

		internal ToolStripMenuItem North
		{ get; private set; }

		internal ToolStripMenuItem West
		{ get; private set; }

		internal ToolStripMenuItem Content
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiates the TopView viewer and its components/controls.
		/// IMPORTANT: TopViewForm and TopRouteViewForm will each invoke and
		/// maintain their own instantiations.
		/// </summary>
		internal TopView()
		{
			InitializeComponent();

			SuspendLayout();

			quadrants.SelectedQuadrant = QuadrantType.Floor;


			TopPanel = new TopPanel(this);
			TopPanel.Dock = DockStyle.Fill;

			pnlMain.Controls.Add(TopPanel);

			pnlMain.Resize += (sender, e) => TopPanel.ResizeObserver(
																pnlMain.Width,
																pnlMain.Height);

			var visQuads = tsddbVisibleQuads.DropDown.Items;

			Floor   = new ToolStripMenuItem(QuadrantPanelDrawService.Floor);
			West    = new ToolStripMenuItem(QuadrantPanelDrawService.West);
			North   = new ToolStripMenuItem(QuadrantPanelDrawService.North);
			Content = new ToolStripMenuItem(QuadrantPanelDrawService.Content);

			visQuads.Add(Floor);
			visQuads.Add(West);
			visQuads.Add(North);
			visQuads.Add(Content);

			Floor  .ShortcutKeys = Keys.F1;
			West   .ShortcutKeys = Keys.F2;
			North  .ShortcutKeys = Keys.F3;
			Content.ShortcutKeys = Keys.F4;

			Floor  .Checked =
			West   .Checked =
			North  .Checked =
			Content.Checked = true;

			foreach (ToolStripMenuItem it in visQuads)
				it.Click += OnToggleQuadrantVisibilityClick;

			ObserverPanels.Add("TopPanel",      TopPanel);
			ObserverPanels.Add("QuadrantPanel", QuadrantPanel);

			ResumeLayout();
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Handles a click on any of the quadrant-visibility menuitems.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnToggleQuadrantVisibilityClick(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			if (it == Floor)
			{
				ViewerFormsManager.TopView     .Control   .Floor.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.Floor.Checked = !it.Checked;

				((MapFile)MapBase).CalculateOccultations(!it.Checked);
			}
			else if (it == West)
			{
				ViewerFormsManager.TopView     .Control   .West.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.West.Checked = !it.Checked;
			}
			else if (it == North)
			{
				ViewerFormsManager.TopView     .Control   .North.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.North.Checked = !it.Checked;
			}
			else //if (it == Content)
			{
				ViewerFormsManager.TopView     .Control   .Content.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.Content.Checked = !it.Checked;
			}

			MainViewUnderlay.that.Refresh();

			ViewerFormsManager.TopView     .Control   .Refresh();
			ViewerFormsManager.TopRouteView.ControlTop.Refresh();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Creates the tool-objects in the toolstrip.
		/// </summary>
		/// <param name="tools"></param>
		internal void InitializeToolstrip(ToolstripFactory tools)
		{
			tools.CreateToolstripEditorObjects(tsTools, true);
		}

		/// <summary>
		/// Selects a quadrant in the QuadrantPanel given a selected tiletype.
		/// </summary>
		/// <param name="parttype"></param>
		internal void SelectQuadrant(PartType parttype)
		{
			switch (parttype)
			{
				case PartType.Floor:   QuadrantPanel.SelectedQuadrant = QuadrantType.Floor;   break;
				case PartType.West:    QuadrantPanel.SelectedQuadrant = QuadrantType.West;    break;
				case PartType.North:   QuadrantPanel.SelectedQuadrant = QuadrantType.North;   break;
				case PartType.Content: QuadrantPanel.SelectedQuadrant = QuadrantType.Content; break;
			}
		}
		#endregion Methods


		#region Options
		private static Form _foptions;	// is static so that it will be used by both
										// TopView and TopRouteView(Top)
		/// <summary>
		/// Handles a click on the Options button to show or hide an Options-
		/// form. Instantiates an 'OptionsForm' if one doesn't exist for this
		/// viewer. Also subscribes to a form-closing handler that will hide the
		/// form unless MainView is closing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (!tsb.Checked)
			{
				setOptionsChecked(true);

				if (_foptions == null)
				{
					_foptions = new OptionsForm("TopViewOptions", Options);
					_foptions.Text = " TopView Options";

					OptionsManager.Screens.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!XCMainWindow.Quit)
						{
							setOptionsChecked(false);

							e1.Cancel = true;
							_foptions.Hide();
						}
						else
							RegistryInfo.UpdateRegistry(_foptions);
					};
				}

				_foptions.Show();

				if (_foptions.WindowState == FormWindowState.Minimized)
					_foptions.WindowState  = FormWindowState.Normal;
			}
			else
				_foptions.Close();
		}

		private void setOptionsChecked(bool @checked)
		{
			ViewerFormsManager.TopView     .Control   .tsb_Options.Checked =
			ViewerFormsManager.TopRouteView.ControlTop.tsb_Options.Checked = @checked;
		}

		/// <summary>
		/// Gets the Options button on the toolstrip.
		/// </summary>
		/// <returns>either the button in TopView or TopRouteView(Top)
		/// - doesn't matter as long as they are kept synched</returns>
		internal ToolStripButton GetOptionsButton()
		{
			return tsb_Options;
		}


		// headers
		private const string Tile     = "Tile";
		private const string Selector = "Selector";
		private const string Grid     = "Grid";

		// options
		internal const string FloorColor        = "FloorColor";
		internal const string WestColor         = "WestColor";
		private  const string WestWidth         = "WestWidth";
		internal const string NorthColor        = "NorthColor";
		private  const string NorthWidth        = "NorthWidth";
		internal const string ContentColor      = "ContentColor";

		internal const string SelectorColor     = "SelectorColor";
		internal const string SelectorWidth     = "SelectorWidth";

		internal const string SelectedColor     = "SelectedColor";
		private  const string SelectedWidth     = "SelectedWidth";

		private  const string SelectedTypeColor = "SelectedTypeColor"; // ie. SelectedQuadColor

		internal const string GridColor         = "GridColor";
		private  const string GridWidth         = "GridWidth";
		internal const string Grid10Color       = "Grid10Color";
		private  const string Grid10Width       = "Grid10Width";


		/// <summary>
		/// Loads default options for TopView in TopRouteView screens.
		/// </summary>
		protected internal override void LoadControlOptions()
		{
			TopPanel.Brushes.Add(FloorColor, new SolidBrush(Color.BurlyWood));

			const int widthwall     = 3;
			const int widthselector = 2;
			const int widthselected = 2;
			const int widthgrid     = 1;
			const int widthgrid10   = 2;

			var penWest      = new Pen(Color.Khaki,     widthwall);
			var penNorth     = new Pen(Color.Wheat,     widthwall);
			var penSelector  = new Pen(Color.Black,     widthselector);
			var penSelected  = new Pen(Color.RoyalBlue, widthselected);
			var penGrid      = new Pen(Color.Black,     widthgrid);
			var pen10Grid    = new Pen(Color.Black,     widthgrid10);

			var brushContent = new SolidBrush(Color.MediumSeaGreen);

			TopPanel.Pens.Add(WestColor, penWest);
			TopPanel.Pens.Add(WestWidth, penWest);
			TopPanel.ToolWest = new ColorTool(penWest);

			TopPanel.Pens.Add(NorthColor, penNorth);
			TopPanel.Pens.Add(NorthWidth, penNorth);
			TopPanel.ToolNorth = new ColorTool(penNorth);

			TopPanel.Brushes.Add(ContentColor, brushContent);
			TopPanel.ToolContent = new ColorTool(
											brushContent,
											DrawBlobService.LINEWIDTH_CONTENT);

			TopPanel.Pens.Add(SelectorColor, penSelector);
			TopPanel.Pens.Add(SelectorWidth, penSelector);

			TopPanel.Pens.Add(SelectedColor, penSelected);
			TopPanel.Pens.Add(SelectedWidth, penSelected);

			TopPanel.Pens.Add(GridColor, penGrid);
			TopPanel.Pens.Add(GridWidth, penGrid);

			TopPanel.Pens.Add(Grid10Color, pen10Grid);
			TopPanel.Pens.Add(Grid10Width, pen10Grid);

			OptionChangedEvent bc = OnBrushChanged;
			OptionChangedEvent pc = OnPenColorChanged;
			OptionChangedEvent pw = OnPenWidthChanged;

			Options.AddOption(FloorColor,        Color.BurlyWood,      "Color of the floor tile indicator",           Tile,     bc);
			Options.AddOption(WestColor,         Color.Khaki,          "Color of the west tile indicator",            Tile,     pc);
			Options.AddOption(WestWidth,         widthwall,            "Width of the west tile indicator in pixels",  Tile,     pw);
			Options.AddOption(NorthColor,        Color.Wheat,          "Color of the north tile indicator",           Tile,     pc);
			Options.AddOption(NorthWidth,        widthwall,            "Width of the north tile indicator in pixels", Tile,     pw);
			Options.AddOption(ContentColor,      Color.MediumSeaGreen, "Color of the content tile indicator",         Tile,     bc);

			Options.AddOption(SelectorColor,     Color.Black,          "Color of the mouse-over indicator",           Selector, pc);
			Options.AddOption(SelectorWidth,     widthselector,        "Width of the mouse-over indicator in pixels", Selector, pw);
			Options.AddOption(SelectedColor,     Color.RoyalBlue,      "Color of the selection line",                 Selector, pc);
			Options.AddOption(SelectedWidth,     widthselected,        "Width of the selection line in pixels",       Selector, pw);
			Options.AddOption(SelectedTypeColor, Color.LightBlue,      "Background color of the selected parttype",   Selector, bc);

			Options.AddOption(GridColor,         Color.Black,          "Color of the grid lines",                     Grid,     pc);
			Options.AddOption(GridWidth,         widthgrid,            "Width of the grid lines in pixels",           Grid,     pw);
			Options.AddOption(Grid10Color,       Color.Black,          "Color of every tenth grid line",              Grid,     pc);
			Options.AddOption(Grid10Width,       widthgrid10,          "Width of every tenth grid line in pixels",    Grid,     pw);

			Invalidate();
		}

		/// <summary>
		/// Fires when a brush-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnBrushChanged(string key, object val)
		{
			switch (key)
			{
				case SelectedTypeColor:
					QuadrantPanelDrawService.Brush.Dispose();
					QuadrantPanelDrawService.Brush = new SolidBrush((Color)val);
					break;

				default:
					TopPanel.Brushes[key].Color = (Color)val;

					switch (key)
					{
						case ContentColor:
							TopPanel.ToolContent.Dispose();
							TopPanel.ToolContent = new ColorTool(
															TopPanel.Brushes[key],
															DrawBlobService.LINEWIDTH_CONTENT);
							break;
					}
					break;
			}
			RefreshControls();
		}

		/// <summary>
		/// Fires when a pen-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnPenColorChanged(string key, object val)
		{
			TopPanel.Pens[key].Color = (Color)val;

			switch (key)
			{
				case WestColor:
					TopPanel.ToolWest.Dispose();
					TopPanel.ToolWest = new ColorTool(TopPanel.Pens[key]);
					break;

				case NorthColor:
					TopPanel.ToolNorth.Dispose();
					TopPanel.ToolNorth = new ColorTool(TopPanel.Pens[key]);
					break;
			}
			RefreshControls();
		}

		/// <summary>
		/// Fires when a pen-width changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnPenWidthChanged(string key, object val)
		{
			TopPanel.Pens[key].Width = (int)val;

			switch (key)
			{
				case WestWidth:
					TopPanel.ToolWest.Dispose();
					TopPanel.ToolWest = new ColorTool(TopPanel.Pens[key]);
					break;

				case NorthWidth:
					TopPanel.ToolNorth.Dispose();
					TopPanel.ToolNorth = new ColorTool(TopPanel.Pens[key]);
					break;
			}
			RefreshControls();
		}

		/// <summary>
		/// Refreshes TopView's and TopRouteView(Top)'s controls.
		/// </summary>
		private void RefreshControls()
		{
			ViewerFormsManager.TopView     .Control   .Refresh();
			ViewerFormsManager.TopRouteView.ControlTop.Refresh();
		}
		#endregion Options
	}
}
