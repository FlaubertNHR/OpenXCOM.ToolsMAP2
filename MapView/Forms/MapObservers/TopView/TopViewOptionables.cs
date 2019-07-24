using System;
using System.ComponentModel;
using System.Drawing;

using MapView.Forms.MainWindow;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// Properties for TopView that appear in TopView's Options.
	/// </summary>
	internal sealed class TopViewOptionables
	{
		#region Fields
		private readonly TopView TopView;
		#endregion Fields


		#region cTor
		internal TopViewOptionables(TopView topview)
		{
			TopView = topview;
		}
		#endregion cTor


		#region Properties (optionable)
		// NOTE: The Properties are public for Reflection.

//		[DisplayName(...)]
//		[Description(...)]
//		[Category(...)]
//		[TypeConverter(...)]
//		[ReadOnly(...)]
//		[Browsable(...)]
//		[DefaultValue(...)]
//		[Editor(...)]

		private const string cat_Grid = "Grid";

		internal const string str_GridLineColor = "GridLineColor";
		private static Color  def_GridLineColor = Color.Black;

		private Color _gridLineColor = def_GridLineColor;
		[Category(cat_Grid)]
		[Description("Color of the grid lines")]
		[DefaultValue(typeof(Color), "Black")]
		public Color GridLineColor
		{
			get { return _gridLineColor; }
			set { _gridLineColor = value; }
		}

		private const string str_GridLineWidth = "GridLineWidth";
		private const int    def_GridLineWidth = 1;

		private int _gridLineWidth = def_GridLineWidth;
		[Category(cat_Grid)]
		[Description("Width of the grid lines in pixels (1..6 default 1)")]
		[DefaultValue(def_GridLineWidth)]
		public int GridLineWidth
		{
			get { return _gridLineWidth; }
			set
			{
				var foptions = TopView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					TopView.Options[str_GridLineWidth].Value =
					_gridLineWidth = value.Clamp(1,6);
				}
				else if ((_gridLineWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					TopView.Options[str_GridLineWidth].Value = _gridLineWidth;
//					foptions.propertyGrid.SetSelectedValue(_gridLineWidth);
				}
			}
		}


		internal const string str_GridLine10Color = "GridLine10Color";
		private static Color  def_GridLine10Color = Color.Black;

		private Color _gridLine10Color = def_GridLine10Color;
		[Category(cat_Grid)]
		[Description("Color of every tenth grid line")]
		[DefaultValue(typeof(Color), "Black")]
		public Color GridLine10Color
		{
			get { return _gridLine10Color; }
			set { _gridLine10Color = value; }
		}

		private const string str_GridLine10Width = "GridLine10Width";
		private const int    def_GridLine10Width = 2;

		private int _gridLine10Width = def_GridLine10Width;
		[Category(cat_Grid)]
		[Description("Width of every tenth grid line in pixels (1..6 default 2)")]
		[DefaultValue(def_GridLine10Width)]
		public int GridLine10Width
		{
			get { return _gridLine10Width; }
			set
			{
				var foptions = TopView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					TopView.Options[str_GridLine10Width].Value =
					_gridLine10Width = value.Clamp(1,6);
				}
				else if ((_gridLine10Width = value.Clamp(1,6)) != value) // on user-changed
				{
					TopView.Options[str_GridLine10Width].Value = _gridLine10Width;
//					foptions.propertyGrid.SetSelectedValue(_gridLine10Width);
				}
			}
		}



		private const string cat_Blobs = "Blobs";

		internal const string str_FloorColor = "FloorColor"; // The key to the Option shall be identical to its Property label.
		private static Color  def_FloorColor = Color.BurlyWood;

		private Color _floorColor = def_FloorColor;
		[Category(cat_Blobs)]
		[Description("Color of the floor indicator")]
		[DefaultValue(typeof(Color), "BurlyWood")]
		public Color FloorColor
		{
			get { return _floorColor; }
			set { _floorColor = value; }
		}


		internal const string str_WestColor = "WestColor";
		private static Color  def_WestColor = Color.Khaki;

		private Color _westColor = def_WestColor;
		[Category(cat_Blobs)]
		[Description("Color of the westwall indicator")]
		[DefaultValue(typeof(Color), "Khaki")]
		public Color WestColor
		{
			get { return _westColor; }
			set { _westColor = value; }
		}

		private const string str_WestWidth = "WestWidth";
		private const int    def_WestWidth = 3;

		private int _westWidth = def_WestWidth;
		[Category(cat_Blobs)]
		[Description("Width of the westwall indicator in pixels (1..9 default 3)")]
		[DefaultValue(def_WestWidth)]
		public int WestWidth
		{
			get { return _westWidth; }
			set
			{
				var foptions = TopView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					TopView.Options[str_WestWidth].Value =
					_westWidth = value.Clamp(1,9);
				}
				else if ((_westWidth = value.Clamp(1,9)) != value) // on user-changed
				{
					TopView.Options[str_WestWidth].Value = _westWidth;
//					foptions.propertyGrid.SetSelectedValue(_westWidth);
				}
			}
		}


		internal const string str_NorthColor = "NorthColor";
		private static Color  def_NorthColor = Color.Wheat;

		private Color _northColor = def_NorthColor;
		[Category(cat_Blobs)]
		[Description("Color of the northwall indicator")]
		[DefaultValue(typeof(Color), "Wheat")]
		public Color NorthColor
		{
			get { return _northColor; }
			set { _northColor = value; }
		}

		private const string str_NorthWidth = "NorthWidth";
		private const int    def_NorthWidth = 3;

		private int _northWidth = def_NorthWidth;
		[Category(cat_Blobs)]
		[Description("Width of the northwall indicator in pixels (1..9 default 3)")]
		[DefaultValue(def_NorthWidth)]
		public int NorthWidth
		{
			get { return _northWidth; }
			set
			{
				var foptions = TopView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					TopView.Options[str_NorthWidth].Value =
					_northWidth = value.Clamp(1,9);
				}
				else if ((_northWidth = value.Clamp(1,9)) != value) // on user-changed
				{
					TopView.Options[str_NorthWidth].Value = _northWidth;
//					foptions.propertyGrid.SetSelectedValue(_northWidth);
				}
			}
		}


		internal const string str_ContentColor = "ContentColor";
		private static Color  def_ContentColor = Color.MediumSeaGreen;

		private Color _contentColor = def_ContentColor;
		[Category(cat_Blobs)]
		[Description("Color of the content indicator")]
		[DefaultValue(typeof(Color), "MediumSeaGreen")]
		public Color ContentColor
		{
			get { return _contentColor; }
			set { _contentColor = value; }
		}



		private const string cat_Selects = "Selects";

		internal const string str_SelectorColor = "SelectorColor";
		private static Color  def_SelectorColor = Color.Black;

		private Color _selectorColor = def_SelectorColor;
		[Category(cat_Selects)]
		[Description("Color of the tile selector")]
		[DefaultValue(typeof(Color), "Black")]
		public Color SelectorColor
		{
			get { return _selectorColor; }
			set { _selectorColor = value; }
		}

		private const string str_SelectorWidth = "SelectorWidth";
		private const int    def_SelectorWidth = 2;

		private int _selectorWidth = def_SelectorWidth;
		[Category(cat_Selects)]
		[Description("Width of the tile selector in pixels (1..6 default 2)")]
		[DefaultValue(def_SelectorWidth)]
		public int SelectorWidth
		{
			get { return _selectorWidth; }
			set
			{
				var foptions = TopView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					TopView.Options[str_SelectorWidth].Value =
					_selectorWidth = value.Clamp(1,6);
				}
				else if ((_selectorWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					TopView.Options[str_SelectorWidth].Value = _selectorWidth;
//					foptions.propertyGrid.SetSelectedValue(_selectorWidth);
				}
			}
		}


		internal const string str_SelectedColor = "SelectedColor";
		private static Color  def_SelectedColor = Color.RoyalBlue;

		private Color _selectedColor = def_SelectedColor;
		[Category(cat_Selects)]
		[Description("Color of the selection border")]
		[DefaultValue(typeof(Color), "RoyalBlue")]
		public Color SelectedColor
		{
			get { return _selectedColor; }
			set { _selectedColor = value; }
		}

		private const string str_SelectedWidth = "SelectedWidth";
		private const int    def_SelectedWidth = 2;

		private int _selectedWidth = def_SelectedWidth;
		[Category(cat_Selects)]
		[Description("Width of the selection border in pixels (1..6 default 2)")]
		[DefaultValue(def_SelectedWidth)]
		public int SelectedWidth
		{
			get { return _selectedWidth; }
			set
			{
				var foptions = TopView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					TopView.Options[str_SelectedWidth].Value =
					_selectedWidth = value.Clamp(1,6);
				}
				else if ((_selectedWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					TopView.Options[str_SelectedWidth].Value = _selectedWidth;
//					foptions.propertyGrid.SetSelectedValue(_selectedWidth);
				}
			}
		}


		private const string str_SelectedQuadColor = "SelectedQuadColor";
		private static Color def_SelectedQuadColor = Color.LightBlue;

		private Color _selectedQuadColor = def_SelectedQuadColor;
		[Category(cat_Selects)]
		[Description("Background color of the selected parttype")]
		[DefaultValue(typeof(Color), "LightBlue")]
		public Color SelectedQuadColor
		{
			get { return _selectedQuadColor; }
			set { _selectedQuadColor = value; }
		}
		#endregion Properties (optionable)


		#region Methods
		/// <summary>
		/// Instantiates pens, brushes, and colortools used by TopView's draw
		/// routines with default values. Adds default keyval pairs to
		/// TopView's optionables and an option-changer is assigned to each.
		/// The default values were assigned to TopView's optionable properties
		/// when those properties were instantiated above.
		/// </summary>
		/// <param name="options">TopView's options</param>
		internal void LoadDefaults(Options options)
		{
			var penGrid = new Pen(def_GridLineColor, def_GridLineWidth);
			TopPanel.Pens.Add(str_GridLineColor, penGrid);

			var penGrid10 = new Pen(def_GridLine10Color, def_GridLine10Width);
			TopPanel.Pens.Add(str_GridLine10Color, penGrid10);

			TopPanel.Brushes.Add(str_FloorColor, new SolidBrush(def_FloorColor));

			var penWest = new Pen(def_WestColor, def_WestWidth);
			TopPanel.Pens.Add(str_WestColor, penWest);
			TopPanel.ToolWest = new ColorTool(penWest);

			var penNorth = new Pen(def_NorthColor, def_NorthWidth);
			TopPanel.Pens.Add(str_NorthColor, penNorth);
			TopPanel.ToolNorth = new ColorTool(penNorth);

			var brushContent = new SolidBrush(def_ContentColor);
			TopPanel.Brushes.Add(str_ContentColor, brushContent);
			TopPanel.ToolContent = new ColorTool(brushContent, DrawBlobService.LINEWIDTH_CONTENT);

			var penSelector = new Pen(def_SelectorColor, def_SelectorWidth);
			TopPanel.Pens.Add(str_SelectorColor, penSelector);

			var penSelected = new Pen(def_SelectedColor, def_SelectedWidth);
			TopPanel.Pens.Add(str_SelectedColor, penSelected);

			QuadrantDrawService.Brush = new SolidBrush(def_SelectedQuadColor);


			OptionChangedEvent changer = OnOptionChanged;

			options.AddOptionDefault(str_GridLineColor,     def_GridLineColor,     changer);
			options.AddOptionDefault(str_GridLineWidth,     def_GridLineWidth,     changer);
			options.AddOptionDefault(str_GridLine10Color,   def_GridLine10Color,   changer);
			options.AddOptionDefault(str_GridLine10Width,   def_GridLine10Width,   changer);

			options.AddOptionDefault(str_FloorColor,        def_FloorColor,        changer);
			options.AddOptionDefault(str_WestColor,         def_WestColor,         changer);
			options.AddOptionDefault(str_WestWidth,         def_WestWidth,         changer);
			options.AddOptionDefault(str_NorthColor,        def_NorthColor,        changer);
			options.AddOptionDefault(str_NorthWidth,        def_NorthWidth,        changer);
			options.AddOptionDefault(str_ContentColor,      def_ContentColor,      changer);

			options.AddOptionDefault(str_SelectorColor,     def_SelectorColor,     changer);
			options.AddOptionDefault(str_SelectorWidth,     def_SelectorWidth,     changer);
			options.AddOptionDefault(str_SelectedColor,     def_SelectedColor,     changer);
			options.AddOptionDefault(str_SelectedWidth,     def_SelectedWidth,     changer);
			options.AddOptionDefault(str_SelectedQuadColor, def_SelectedQuadColor, OnQuadColorChanged);
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Sets the value of an optionable property and invalidates the TopView
		/// and TopRouteView(Top) controls.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnOptionChanged(string key, object val)
		{
			switch (key)
			{
				case str_FloorColor:      FloorColor      = (Color)val; ChangeBruColor(key, val); break;
				case str_WestColor:       WestColor       = (Color)val; ChangePenColor(key, val); break;
				case str_WestWidth:       WestWidth       =   (int)val; ChangePenWidth(key, val); break;
				case str_NorthColor:      NorthColor      = (Color)val; ChangePenColor(key, val); break;
				case str_NorthWidth:      NorthWidth      =   (int)val; ChangePenWidth(key, val); break;
				case str_ContentColor:    ContentColor    = (Color)val; ChangeBruColor(key, val); break;
				case str_SelectorColor:   SelectorColor   = (Color)val; ChangePenColor(key, val); break;
				case str_SelectorWidth:   SelectorWidth   =   (int)val; ChangePenWidth(key, val); break;
				case str_SelectedColor:   SelectedColor   = (Color)val; ChangePenColor(key, val); break;
				case str_SelectedWidth:   SelectedWidth   =   (int)val; ChangePenWidth(key, val); break;
				case str_GridLineColor:   GridLineColor   = (Color)val; ChangePenColor(key, val); break;
				case str_GridLineWidth:   GridLineWidth   =   (int)val; ChangePenWidth(key, val); break;
				case str_GridLine10Color: GridLine10Color = (Color)val; ChangePenColor(key, val); break;
				case str_GridLine10Width: GridLine10Width =   (int)val; ChangePenWidth(key, val); break;
			}

			ObserverManager.TopView     .Control   .TopPanel.Invalidate();
			ObserverManager.TopRouteView.ControlTop.TopPanel.Invalidate();
		}

		/// <summary>
		/// Sets the value of SelectedQuadColor and invalidates the TopView
		/// and TopRouteView(Top) controls.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnQuadColorChanged(string key, object val)
		{
			SelectedQuadColor =
			QuadrantDrawService.Brush.Color = (Color)val;

			ObserverManager.TopView     .Control   .QuadrantPanel.Invalidate();
			ObserverManager.TopRouteView.ControlTop.QuadrantPanel.Invalidate();
		}
		#endregion Events


		#region Methods (static)
		/// <summary>
		/// Fires when a brush-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private static void ChangeBruColor(string key, object val)
		{
			TopPanel.Brushes[key].Color = (Color)val;

			if (key == str_ContentColor)
			{
				TopPanel.ToolContent.Dispose();
				TopPanel.ToolContent = new ColorTool(
												TopPanel.Brushes[key],
												DrawBlobService.LINEWIDTH_CONTENT);
			}
		}

		/// <summary>
		/// Fires when a pen-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private static void ChangePenColor(string key, object val)
		{
			TopPanel.Pens[key].Color = (Color)val;

			switch (key)
			{
				case str_WestColor:
					TopPanel.ToolWest.Dispose();
					TopPanel.ToolWest = new ColorTool(TopPanel.Pens[key]);
					break;

				case str_NorthColor:
					TopPanel.ToolNorth.Dispose();
					TopPanel.ToolNorth = new ColorTool(TopPanel.Pens[key]);
					break;
			}
		}

		/// <summary>
		/// Fires when a pen-width changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private static void ChangePenWidth(string key, object val)
		{
			TopPanel.Pens[key = WidthToColor(key)].Width = (int)val;

			switch (key)
			{
				case str_WestColor:
					TopPanel.ToolWest.Dispose();
					TopPanel.ToolWest = new ColorTool(TopPanel.Pens[key]);
					break;

				case str_NorthColor:
					TopPanel.ToolNorth.Dispose();
					TopPanel.ToolNorth = new ColorTool(TopPanel.Pens[key]);
					break;
			}
		}

		/// <summary>
		/// Maps a pen's width-key to its corresponding color-key. Because
		/// TopPens are stored and accessed with their color-keys not their
		/// width-keys.
		/// </summary>
		/// <param name="width">width-key</param>
		/// <returns>the width-keys corresponding color-key</returns>
		private static string WidthToColor(string width)
		{
			switch (width)
			{
				case str_WestWidth:       return str_WestColor;
				case str_NorthWidth:      return str_NorthColor;
				case str_SelectorWidth:   return str_SelectorColor;
				case str_SelectedWidth:   return str_SelectedColor;
				case str_GridLineWidth:   return str_GridLineColor;
				case str_GridLine10Width: return str_GridLine10Color;
			}
			return null;
		}
		#endregion Methods (static)
	}
}
