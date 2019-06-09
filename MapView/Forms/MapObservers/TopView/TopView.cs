using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

//using Microsoft.Win32;

using MapView.Forms.MainWindow;

using XCom;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopView
		:
			MapObserverControl0
	{
		#region Fields
		private Dictionary<string, Pen>        _topPens;
		private Dictionary<string, SolidBrush> _topBrushes;
		#endregion


		#region Properties
		internal TopPanel TopPanel
		{ get; private set; }

		internal QuadrantPanel QuadrantsPanel
		{
			get { return quadrants; }
		}

		public bool FloorVisible
		{
			get { return TopPanel.Floor.Checked; }
		}

		public bool WestVisible
		{
			get { return TopPanel.West.Checked; }
		}

		public bool NorthVisible
		{
			get { return TopPanel.North.Checked; }
		}

		public bool ContentVisible
		{
			get { return TopPanel.Content.Checked; }
		}
		#endregion


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


			TopPanel = new TopPanel();
			TopPanel.Dock = DockStyle.Fill;

			pnlMain.Controls.Add(TopPanel);

			pnlMain.Resize += (sender, e) => TopPanel.ResizeObserver(
																	pnlMain.Width,
																	pnlMain.Height);

			var visQuads = tsddbVisibleQuads.DropDown.Items;

			TopPanel.Floor   = new ToolStripMenuItem(QuadrantPanelDrawService.Floor);
			TopPanel.West    = new ToolStripMenuItem(QuadrantPanelDrawService.West);
			TopPanel.North   = new ToolStripMenuItem(QuadrantPanelDrawService.North);
			TopPanel.Content = new ToolStripMenuItem(QuadrantPanelDrawService.Content);

			visQuads.Add(TopPanel.Floor);
			visQuads.Add(TopPanel.West);
			visQuads.Add(TopPanel.North);
			visQuads.Add(TopPanel.Content);

			TopPanel.Floor.ShortcutKeys = Keys.F1;
			TopPanel.Floor.Checked = true;

			TopPanel.West.ShortcutKeys = Keys.F2;
			TopPanel.West.Checked = true;

			TopPanel.North.ShortcutKeys = Keys.F3;
			TopPanel.North.Checked = true;

			TopPanel.Content.ShortcutKeys = Keys.F4;
			TopPanel.Content.Checked = true;

			foreach (ToolStripMenuItem it in visQuads)
				it.Click += OnToggleQuadrantVisibilityClick;

			TopPanel.QuadrantsPanel = QuadrantsPanel;

			Panels.Add("QuadrantsPanel", QuadrantsPanel);
			Panels.Add("TopPanel",       TopPanel);

			ResumeLayout();
		}
		#endregion


		#region Events
		/// <summary>
		/// Handles a click on any of the quadrant-visibility menuitems.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnToggleQuadrantVisibilityClick(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			if (it == TopPanel.Floor)
			{
				ViewerFormsManager.TopView     .Control   .TopPanel.Floor.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.TopPanel.Floor.Checked = !it.Checked;

				((MapFileChild)MapBase).CalculateOccultations(!it.Checked);
			}
			else if (it == TopPanel.West)
			{
				ViewerFormsManager.TopView     .Control   .TopPanel.West.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.TopPanel.West.Checked = !it.Checked;
			}
			else if (it == TopPanel.North)
			{
				ViewerFormsManager.TopView     .Control   .TopPanel.North.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.TopPanel.North.Checked = !it.Checked;
			}
			else //if (it == TopPanel.Content)
			{
				ViewerFormsManager.TopView     .Control   .TopPanel.Content.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.TopPanel.Content.Checked = !it.Checked;
			}

			MainViewUnderlay.that.Refresh();

			ViewerFormsManager.TopView     .Control   .Refresh();
			ViewerFormsManager.TopRouteView.ControlTop.Refresh();
		}
		#endregion


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
		/// Selects a quadrant in the QuadrantsPanel given a selected tiletype.
		/// </summary>
		/// <param name="parttype"></param>
		internal void SelectQuadrant(PartType parttype)
		{
			switch (parttype)
			{
				case PartType.Floor:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.Floor;
					break;

				case PartType.West:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.West;
					break;

				case PartType.North:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.North;
					break;

				case PartType.Content:
					QuadrantsPanel.SelectedQuadrant = QuadrantType.Content;
					break;
			}
		}
		#endregion


		#region Options
		private static Form _foptions; // is static so it will be used by both TopViewOptions
		private static bool _closing;  // and TopRouteView(Top)Options

		/// <summary>
		/// Handles a click on the Options button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (!tsb.Checked)
			{
				ViewerFormsManager.TopView     .Control   .tsb_Options.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.tsb_Options.Checked = true;

				_foptions = new OptionsForm("TopViewOptions", Options);
				_foptions.Text = "TopView Options";

				_foptions.Show();

				_foptions.FormClosing += (sender1, e1) => // a note describing why this is here could be helpful ...
				{
					if (!_closing)
						OnOptionsClick(sender, e);

					_closing = false;
				};
			}
			else
			{
				ViewerFormsManager.TopView     .Control   .tsb_Options.Checked =
				ViewerFormsManager.TopRouteView.ControlTop.tsb_Options.Checked = false;

				_closing = true;
				_foptions.Close();
			}
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
		internal const string NorthColor        = "NorthColor";
		internal const string ContentColor      = "ContentColor";

		private  const string WestWidth         = "WestWidth";
		private  const string NorthWidth        = "NorthWidth";

		internal const string SelectorColor     = "SelectorColor";
		internal const string SelectorWidth     = "SelectorWidth";

		internal const string SelectedColor     = "SelectedColor";
		private  const string SelectedWidth     = "SelectedWidth";

		private  const string SelectedTypeColor = "SelectedTypeColor";

		internal const string GridColor         = "GridColor";
		private  const string GridWidth         = "GridWidth";
		internal const string Grid10Color       = "Grid10Color";
		private  const string Grid10Width       = "Grid10Width";


		/// <summary>
		/// Loads default options for TopView in TopRouteView screens.
		/// </summary>
		protected internal override void LoadControl0Options()
		{
			_topBrushes = new Dictionary<string, SolidBrush>();
			_topPens    = new Dictionary<string, Pen>();

			_topBrushes.Add(FloorColor,   new SolidBrush(Color.BurlyWood));
			_topBrushes.Add(ContentColor, new SolidBrush(Color.MediumSeaGreen));
			_topBrushes.Add(SelectedTypeColor, QuadrantsPanel.SelectColor);

			var penWest = new Pen(Color.Khaki, 4);
			_topPens.Add(WestColor, penWest);
			_topPens.Add(WestWidth, penWest);

			var penNorth = new Pen(Color.Wheat, 4);
			_topPens.Add(NorthColor, penNorth);
			_topPens.Add(NorthWidth, penNorth);

			var penOver = new Pen(Color.Black, 2);
			_topPens.Add(SelectorColor, penOver);
			_topPens.Add(SelectorWidth, penOver);

			var penSelected = new Pen(Color.RoyalBlue, 2);
			_topPens.Add(SelectedColor, penSelected);
			_topPens.Add(SelectedWidth, penSelected);

			var penGrid = new Pen(Color.Black, 1);
			_topPens.Add(GridColor, penGrid);
			_topPens.Add(GridWidth, penGrid);

			var pen10Grid = new Pen(Color.Black, 2);
			_topPens.Add(Grid10Color, pen10Grid);
			_topPens.Add(Grid10Width, pen10Grid);

			OptionChangedEventHandler bc = OnBrushChanged;
			OptionChangedEventHandler pc = OnPenColorChanged;
			OptionChangedEventHandler pw = OnPenWidthChanged;

			Options.AddOption(FloorColor,        Color.BurlyWood,                 "Color of the floor tile indicator",           Tile,     bc);
			Options.AddOption(WestColor,         Color.Khaki,                     "Color of the west tile indicator",            Tile,     pc);
			Options.AddOption(NorthColor,        Color.Wheat,                     "Color of the north tile indicator",           Tile,     pc);
			Options.AddOption(ContentColor,      Color.MediumSeaGreen,            "Color of the content tile indicator",         Tile,     bc);
			Options.AddOption(WestWidth,         3,                               "Width of the west tile indicator in pixels",  Tile,     pw);
			Options.AddOption(NorthWidth,        3,                               "Width of the north tile indicator in pixels", Tile,     pw);

			Options.AddOption(SelectorColor,     Color.Black,                     "Color of the mouse-over indicator",           Selector, pc);
			Options.AddOption(SelectorWidth,     2,                               "Width of the mouse-over indicator in pixels", Selector, pw);
			Options.AddOption(SelectedColor,     Color.RoyalBlue,                 "Color of the selection line",                 Selector, pc);
			Options.AddOption(SelectedWidth,     2,                               "Width of the selection line in pixels",       Selector, pw);
			Options.AddOption(SelectedTypeColor, Color.LightBlue,                 "Background color of the selected tiletype",   Selector, bc);

			Options.AddOption(GridColor,         Color.Black,                     "Color of the grid lines",                     Grid,     pc);
			Options.AddOption(GridWidth,         1,                               "Width of the grid lines in pixels",           Grid,     pw);
			Options.AddOption(Grid10Color,       Color.Black,                     "Color of every tenth grid line",              Grid,     pc);
			Options.AddOption(Grid10Width,       2,                               "Width of every tenth grid line in pixels",    Grid,     pw);

			QuadrantsPanel.Pens =
			TopPanel.TopPens    = _topPens;

			QuadrantsPanel.Brushes =
			TopPanel.TopBrushes    = _topBrushes;

			Invalidate();
		}

		/// <summary>
		/// Fires when a brush-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnBrushChanged(string key, object val)
		{
			_topBrushes[key].Color = (Color)val;

			if (key == SelectedTypeColor)
				QuadrantsPanel.SelectColor = _topBrushes[key];

			Refresh();
		}

		/// <summary>
		/// Fires when a pen-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnPenColorChanged(string key, object val)
		{
			_topPens[key].Color = (Color)val;
			Refresh();
		}

		/// <summary>
		/// Fires when a pen-width changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnPenWidthChanged(string key, object val)
		{
			_topPens[key].Width = (int)val;
			Refresh();
		}

		/// <summary>
		/// Gets the brushes/colors for the Floor and Content blobs.
		/// Used by the Help screen.
		/// </summary>
		/// <returns>a hashtable of the brushes</returns>
		internal Dictionary<string, SolidBrush> GetFloorContentBrushes()
		{
			return _topBrushes;
		}

		/// <summary>
		/// Gets the pens/colors for the Westwall and Northwall blobs.
		/// Used by the Help screen.
		/// </summary>
		/// <returns>a hashtable of the brushes</returns>
		internal Dictionary<string, Pen> GetWallPens()
		{
			return _topPens;
		}
		#endregion
	}
}
