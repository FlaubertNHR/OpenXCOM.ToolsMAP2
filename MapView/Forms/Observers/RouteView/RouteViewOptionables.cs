using System;
using System.ComponentModel;
using System.Drawing;

using MapView.Forms.MainView;


namespace MapView.Forms.Observers.RouteViews
{
	/// <summary>
	/// Properties for RouteView that appear in RouteView's Options.
	/// </summary>
	internal sealed class RouteViewOptionables
	{
		#region Fields
		private readonly RouteView RouteView;
		#endregion Fields


		#region cTor
		internal RouteViewOptionables(RouteView routeview)
		{
			RouteView = routeview;
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
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_GridLineWidth].Value =
					_gridLineWidth = value.Clamp(1,6);
				}
				else if ((_gridLineWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_GridLineWidth].Value = _gridLineWidth;
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
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_GridLine10Width].Value =
					_gridLine10Width = value.Clamp(1,6);
				}
				else if ((_gridLine10Width = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_GridLine10Width].Value = _gridLine10Width;
//					foptions.propertyGrid.SetSelectedValue(_gridLine10Width);
				}
			}
		}



		private const string cat_Blobs = "Blobs";

		internal const string str_WallColor = "WallColor";
		private static Color  def_WallColor = Color.BurlyWood;

		private Color _wallColor = def_WallColor;
		[Category(cat_Blobs)]
		[Description("Color of the wall indicators")]
		[DefaultValue(typeof(Color), "BurlyWood")]
		public Color WallColor
		{
			get { return _wallColor; }
			set { _wallColor = value; }
		}

		private const string str_WallWidth = "WallWidth";
		private const int    def_WallWidth = 3;

		private int _wallWidth = def_WallWidth;
		[Category(cat_Blobs)]
		[Description("Width of the wall indicators in pixels (1..9 default 3)")]
		[DefaultValue(def_WallWidth)]
		public int WallWidth
		{
			get { return _wallWidth; }
			set
			{
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_WallWidth].Value =
					_wallWidth = value.Clamp(1,9);
				}
				else if ((_wallWidth = value.Clamp(1,9)) != value) // on user-changed
				{
					RouteView.Options[str_WallWidth].Value = _wallWidth;
//					foptions.propertyGrid.SetSelectedValue(_wallWidth);
				}
			}
		}


		internal const string str_ContentColor = "ContentColor";
		private static Color  def_ContentColor = Color.DarkGoldenrod;

		private Color _contentColor = def_ContentColor;
		[Category(cat_Blobs)]
		[Description("Color of the content indicator")]
		[DefaultValue(typeof(Color), "DarkGoldenrod")]
		public Color ContentColor
		{
			get { return _contentColor; }
			set { _contentColor = value; }
		}



		private const string cat_Nodes = "Nodes";

		internal const string str_NodeColor = "NodeColor";
		private static Color  def_NodeColor = Color.MediumSeaGreen;

		private Color _nodeColor = def_NodeColor;
		[Category(cat_Nodes)]
		[Description("Color of unselected nodes")]
		[DefaultValue(typeof(Color), "MediumSeaGreen")]
		public Color NodeColor
		{
			get { return _nodeColor; }
			set { _nodeColor = value; }
		}


		internal const string str_NodeSpawnColor = "NodeSpawnColor";
		private static Color  def_NodeSpawnColor = Color.GreenYellow;

		private Color _nodeSpawnColor = def_NodeSpawnColor;
		[Category(cat_Nodes)]
		[Description("Color of spawn nodes")]
		[DefaultValue(typeof(Color), "GreenYellow")]
		public Color NodeSpawnColor
		{
			get { return _nodeSpawnColor; }
			set { _nodeSpawnColor = value; }
		}


		internal const string str_NodeSelectedColor = "NodeSelectedColor";
		private static Color  def_NodeSelectedColor = Color.RoyalBlue;

		private Color _nodeSelectedColor = def_NodeSelectedColor;
		[Category(cat_Nodes)]
		[Description("Color of selected nodes")]
		[DefaultValue(typeof(Color), "RoyalBlue")]
		public Color NodeSelectedColor
		{
			get { return _nodeSelectedColor; }
			set { _nodeSelectedColor = value; }
		}


		private const string str_NodeOpacity = "NodeOpacity";
		private const int    def_NodeOpacity = 255;

		private int _nodeOpacity = def_NodeOpacity;
		[Category(cat_Nodes)]
		[Description("Opacity of node colors (0..255 default 255)")]
		[DefaultValue(def_NodeOpacity)]
		public int NodeOpacity
		{
			get { return _nodeOpacity; }
			set
			{
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_NodeOpacity].Value =
					_nodeOpacity = value.Clamp(0,255);
				}
				else if ((_nodeOpacity = value.Clamp(0,255)) != value) // on user-changed
				{
					RouteView.Options[str_NodeOpacity].Value = _nodeOpacity;
//					foptions.propertyGrid.SetSelectedValue(_nodeOpacity);
				}
			}
		}



		private const string cat_Links = "Links";

		internal const string str_LinkColor = "LinkColor";
		private static Color  def_LinkColor = Color.OrangeRed;

		private Color _linkColor = def_LinkColor;
		[Category(cat_Links)]
		[Description("Color of unselected link lines")]
		[DefaultValue(typeof(Color), "OrangeRed")]
		public Color LinkColor
		{
			get { return _linkColor; }
			set { _linkColor = value; }
		}

		private const string str_LinkWidth = "LinkWidth";
		private const int    def_LinkWidth = 2;

		private int _linkWidth = def_LinkWidth;
		[Category(cat_Links)]
		[Description("Width of unselected link lines in pixels (1..6 default 2)")]
		[DefaultValue(def_LinkWidth)]
		public int LinkWidth
		{
			get { return _linkWidth; }
			set
			{
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_LinkWidth].Value =
					_linkWidth = value.Clamp(1,6);
				}
				else if ((_linkWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_LinkWidth].Value = _linkWidth;
//					foptions.propertyGrid.SetSelectedValue(_linkWidth);
				}
			}
		}


		internal const string str_LinkSelectedColor = "LinkSelectedColor";
		private static Color  def_LinkSelectedColor = Color.RoyalBlue;

		private Color _linkSelectedColor = def_LinkSelectedColor;
		[Category(cat_Links)]
		[Description("Color of selected link lines")]
		[DefaultValue(typeof(Color), "RoyalBlue")]
		public Color LinkSelectedColor
		{
			get { return _linkSelectedColor; }
			set { _linkSelectedColor = value; }
		}

		private const string str_LinkSelectedWidth = "LinkSelectedWidth";
		private const int    def_LinkSelectedWidth = 2;

		private int _linkSelectedWidth = def_LinkSelectedWidth;
		[Category(cat_Links)]
		[Description("Width of selected link lines in pixels (1..6 default 2)")]
		[DefaultValue(def_LinkSelectedWidth)]
		public int LinkSelectedWidth
		{
			get { return _linkSelectedWidth; }
			set
			{
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_LinkSelectedWidth].Value =
					_linkSelectedWidth = value.Clamp(1,6);
				}
				else if ((_linkSelectedWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_LinkSelectedWidth].Value = _linkSelectedWidth;
//					foptions.propertyGrid.SetSelectedValue(_linkSelectedWidth);
				}
			}
		}



		private const string cat_Panel = "Panel";

		private const string str_ShowOverlay = "ShowOverlay";
		private const bool   def_ShowOverlay = true;

		private bool _showOverlay = def_ShowOverlay;
		[Category(cat_Panel)]
		[Description("True to display cursor info")]
		[DefaultValue(true)]
		public bool ShowOverlay
		{
			get { return _showOverlay; }
			set { _showOverlay = value; }
		}


		private const string str_ShowPriorityBars = "ShowPriorityBars";
		private const bool   def_ShowPriorityBars = true;

		private bool _showPriorityBars = def_ShowPriorityBars;
		[Category(cat_Panel)]
		[Description("True to display the spawn/patrol bars")]
		[DefaultValue(true)]
		public bool ShowPriorityBars
		{
			get { return _showPriorityBars; }
			set { _showPriorityBars = value; }
		}



/*		private const string cat_Selects = "Selects";

		internal const string str_SelectorColor = "SelectorColor";
		internal static Color def_SelectorColor = Color.Black;

		private Color _selectorColor = def_SelectorColor;
		[Category(cat_Selects)]
		[Description("Color of the tile selector")]
		[DefaultValue(typeof(Color), "Black")]
		public Color SelectorColor
		{
			get { return _selectorColor; }
			set { _selectorColor = value; }
		}

		internal const string str_SelectorWidth = "SelectorWidth";
		internal const int    def_SelectorWidth = widthselector;

		private int _selectorWidth = def_SelectorWidth;
		[Category(cat_Selects)]
		[Description("Width of the tile selector in pixels (1..6 default 2)")]
		[DefaultValue(def_SelectorWidth)]
		public int SelectorWidth
		{
			get { return _selectorWidth; }
			set
			{
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_SelectorWidth].Value =
					_selectorWidth = value.Clamp(1,6);
				}
				else if ((_selectorWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_SelectorWidth].Value = _selectorWidth;
					foptions.propertyGrid.SetSelectedValue(_selectorWidth);
				}
			}
		}


		internal const string str_SelectedColor = "SelectedColor";
		internal static Color def_SelectedColor = Color.RoyalBlue;

		private Color _selectedColor = def_SelectedColor;
		[Category(cat_Selects)]
		[Description("Color of the selection border")]
		[DefaultValue(typeof(Color), "RoyalBlue")]
		public Color SelectedColor
		{
			get { return _selectedColor; }
			set { _selectedColor = value; }
		}

		internal const string str_SelectedWidth = "SelectedWidth";
		internal const int    def_SelectedWidth = widthselected;

		private int _selectedWidth = def_SelectedWidth;
		[Category(cat_Selects)]
		[Description("Width of the selection border in pixels (1..6 default 2)")]
		[DefaultValue(def_SelectedWidth)]
		public int SelectedWidth
		{
			get { return _selectedWidth; }
			set
			{
				var foptions = RouteView._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					RouteView.Options[str_SelectedWidth].Value =
					_selectedWidth = value.Clamp(1,6);
				}
				else if ((_selectedWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_SelectedWidth].Value = _selectedWidth;
					foptions.propertyGrid.SetSelectedValue(_selectedWidth);
				}
			}
		} */
		#endregion Properties (optionable)


		#region Methods
		/// <summary>
		/// Instantiates pens, brushes, and colortools used by RouteView's draw
		/// routines with default values. Adds default keyval pairs to
		/// RouteView's optionables and an option-changer is assigned to each.
		/// The default values were assigned to RouteView's optionable
		/// properties when those properties were instantiated above.
		/// </summary>
		/// <param name="options">RouteView's options</param>
		internal void LoadDefaults(Options options)
		{
			var penGrid = new Pen(def_GridLineColor, def_GridLineWidth);
			RoutePanel.RoutePens[str_GridLineColor] = penGrid;

			var penGrid10 = new Pen(def_GridLine10Color, def_GridLine10Width);
			RoutePanel.RoutePens[str_GridLine10Color] = penGrid10;

			var penWall = new Pen(def_WallColor, def_WallWidth);
			RoutePanel.RoutePens[str_WallColor] = penWall;
			RoutePanel.ToolWall = new ColorTool(penWall);

			var brushContent = new SolidBrush(def_ContentColor);
			RoutePanel.RouteBrushes[str_ContentColor] = brushContent;
			RoutePanel.ToolContent = new ColorTool(brushContent, DrawBlobService.LINEWIDTH_CONTENT);

			Color color = Color.FromArgb(def_NodeOpacity, def_NodeColor);
			var brushNode = new SolidBrush(color);
			RoutePanel.RouteBrushes[str_NodeColor] = brushNode;

			color = Color.FromArgb(def_NodeOpacity, def_NodeSpawnColor);
			var brushNodeSpawn = new SolidBrush(color);
			RoutePanel.RouteBrushes[str_NodeSpawnColor] = brushNodeSpawn;

			color = Color.FromArgb(def_NodeOpacity, def_NodeSelectedColor);
			var brushNodeSelected = new SolidBrush(color);
			RoutePanel.RouteBrushes[str_NodeSelectedColor] = brushNodeSelected;

			var penLink = new Pen(def_LinkColor, def_LinkWidth);
			RoutePanel.RoutePens[str_LinkColor] = penLink;

			var penLinkSelected = new Pen(def_LinkSelectedColor, def_LinkSelectedWidth);
			RoutePanel.RoutePens[str_LinkSelectedColor] = penLinkSelected;


			OptionChangedEvent changer = OnOptionChanged;

			options.AddOptionDefault(str_GridLineColor,     def_GridLineColor,     changer);
			options.AddOptionDefault(str_GridLineWidth,     def_GridLineWidth,     changer);
			options.AddOptionDefault(str_GridLine10Color,   def_GridLine10Color,   changer);
			options.AddOptionDefault(str_GridLine10Width,   def_GridLine10Width,   changer);

			options.AddOptionDefault(str_WallColor,         def_WallColor,         changer);
			options.AddOptionDefault(str_WallWidth,         def_WallWidth,         changer);
			options.AddOptionDefault(str_ContentColor,      def_ContentColor,      changer);

			options.AddOptionDefault(str_NodeColor,         def_NodeColor,         changer);
			options.AddOptionDefault(str_NodeSpawnColor,    def_NodeSpawnColor,    changer);
			options.AddOptionDefault(str_NodeSelectedColor, def_NodeSelectedColor, changer);
			options.AddOptionDefault(str_NodeOpacity,       def_NodeOpacity,       changer);

			options.AddOptionDefault(str_LinkColor,         def_LinkColor,         changer);
			options.AddOptionDefault(str_LinkWidth,         def_LinkWidth,         changer);
			options.AddOptionDefault(str_LinkSelectedColor, def_LinkSelectedColor, changer);
			options.AddOptionDefault(str_LinkSelectedWidth, def_LinkSelectedWidth, changer);

			options.AddOptionDefault(str_ShowOverlay,       def_ShowOverlay,       changer);
			options.AddOptionDefault(str_ShowPriorityBars,  def_ShowPriorityBars,  changer);
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Sets the value of an optionable property and refreshes the RouteView
		/// and TopRouteView(Route) controls.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnOptionChanged(string key, object val)
		{
			switch (key)
			{
				case str_GridLineColor:     GridLineColor     = (Color)val; ChangePenColor(key, val); break;
				case str_GridLineWidth:     GridLineWidth     =   (int)val; ChangePenWidth(key, val); break;
				case str_GridLine10Color:   GridLine10Color   = (Color)val; ChangePenColor(key, val); break;
				case str_GridLine10Width:   GridLine10Width   =   (int)val; ChangePenWidth(key, val); break;

				case str_WallColor:         WallColor         = (Color)val; ChangePenColor(key, val); break;
				case str_WallWidth:         WallWidth         =   (int)val; ChangePenWidth(key, val); break;
				case str_ContentColor:      ContentColor      = (Color)val; ChangeBruColor(key, val); break;

				case str_NodeColor:         NodeColor         = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeSpawnColor:    NodeSpawnColor    = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeSelectedColor: NodeSelectedColor = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeOpacity:       NodeOpacity       =   (int)val; ChangeBruOpaci(key, val); break;

				case str_LinkColor:         LinkColor         = (Color)val; ChangePenColor(key, val); break;
				case str_LinkWidth:         LinkWidth         =   (int)val; ChangePenWidth(key, val); break;
				case str_LinkSelectedColor: LinkSelectedColor = (Color)val; ChangePenColor(key, val); break;
				case str_LinkSelectedWidth: LinkSelectedWidth =   (int)val; ChangePenWidth(key, val); break;

				case str_ShowOverlay:       ShowOverlay       =  (bool)val;                           break;
				case str_ShowPriorityBars:  ShowPriorityBars  =  (bool)val;                           break;
			}

			RouteView.RefreshControls();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Fires when a brush-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void ChangeBruColor(string key, object val)
		{
			var color = (Color)val;

			if (key == str_ContentColor)
			{
				// Do not apply alpha to ContentColor.
				RoutePanel.RouteBrushes[str_ContentColor].Color = color;

				RoutePanel.ToolContent.Dispose();
				RoutePanel.ToolContent = new ColorTool(
													RoutePanel.RouteBrushes[str_ContentColor],
													DrawBlobService.LINEWIDTH_CONTENT);
			}
			else // is Node color
			{
				color = Color.FromArgb(NodeOpacity, color);
				RoutePanel.RouteBrushes[key].Color = color;

				switch (key)
				{
					case str_NodeSelectedColor:
						RouteView.SetInfotextColor_selected();
						break;

					case str_NodeColor:
					case str_NodeSpawnColor:
						ObserverManager.RouteView   .Control     .SetInfotextColor_over();
						ObserverManager.TopRouteView.ControlRoute.SetInfotextColor_over();
						break;
				}
			}
		}

		/// <summary>
		/// Fires when a brush-opacity changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void ChangeBruOpaci(string key, object val)
		{
			Color color = Color.FromArgb((int)val, NodeColor);
			RoutePanel.RouteBrushes[str_NodeColor].Color = color;

			color = Color.FromArgb((int)val, NodeSpawnColor);
			RoutePanel.RouteBrushes[str_NodeSpawnColor].Color = color;

			color = Color.FromArgb((int)val, NodeSelectedColor);
			RoutePanel.RouteBrushes[str_NodeSelectedColor].Color = color;
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Fires when a pen-color changes in Options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private static void ChangePenColor(string key, object val)
		{
			RoutePanel.RoutePens[key].Color = (Color)val;

			switch (key)
			{
				case str_WallColor:
					RoutePanel.ToolWall.Dispose();
					RoutePanel.ToolWall = new ColorTool(RoutePanel.RoutePens[key]);
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
			RoutePanel.RoutePens[key = WidthToColor(key)].Width = (int)val;

			switch (key)
			{
				case str_WallColor: // doh!
					RoutePanel.ToolWall.Dispose();
					RoutePanel.ToolWall = new ColorTool(RoutePanel.RoutePens[key]);
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
				case str_GridLineWidth:     return str_GridLineColor;
				case str_GridLine10Width:   return str_GridLine10Color;
				case str_WallWidth:         return str_WallColor;
				case str_LinkWidth:         return str_LinkColor;
				case str_LinkSelectedWidth: return str_LinkSelectedColor;
			}
			return null;
		}
		#endregion Methods (static)
	}
}
