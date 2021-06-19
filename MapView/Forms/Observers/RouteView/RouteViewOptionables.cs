using System;
using System.ComponentModel;
using System.Drawing;

using MapView.Forms.MainView;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Properties for <see cref="RouteView"/> that appear in RouteView's
	/// Options.
	/// </summary>
	internal sealed class RouteViewOptionables
	{
		internal static void DisposeOptionables()
		{
			DSShared.LogFile.WriteLine("RouteViewOptionables.DisposeOptionables() static");
			foreach (var pair in RouteControl.RoutePens)
				pair.Value.Dispose();

			foreach (var pair in RouteControl.RouteBrushes)
				pair.Value.Dispose();

			RouteControl.ToolWall   .Dispose();
			RouteControl.ToolContent.Dispose();
		}


		#region Fields
		private readonly RouteView _routeView;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="routeView"></param>
		internal RouteViewOptionables(RouteView routeView)
		{
			_routeView = routeView;
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
		[Description("Color of the grid lines (default Black)")]
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
		[Description("Width of the grid lines in pixels (1..5 default 1)")]
		[DefaultValue(def_GridLineWidth)]
		public int GridLineWidth
		{
			get { return _gridLineWidth; }
			set
			{
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_GridLineWidth].Value =
					_gridLineWidth = value.Viceroy(1,5);
				}
				else if ((_gridLineWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					_routeView.Options[str_GridLineWidth].Value = _gridLineWidth;
				}
			}
		}


		internal const string str_GridLine10Color = "GridLine10Color";
		private static Color  def_GridLine10Color = Color.Black;

		private Color _gridLine10Color = def_GridLine10Color;
		[Category(cat_Grid)]
		[Description("Color of every tenth grid line (default Black)")]
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
		[Description("Width of every tenth grid line in pixels (1..5 default 2)")]
		[DefaultValue(def_GridLine10Width)]
		public int GridLine10Width
		{
			get { return _gridLine10Width; }
			set
			{
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_GridLine10Width].Value =
					_gridLine10Width = value.Viceroy(1,5);
				}
				else if ((_gridLine10Width = value.Viceroy(1,5)) != value) // on user-changed
				{
					_routeView.Options[str_GridLine10Width].Value = _gridLine10Width;
				}
			}
		}



		private const string cat_Blobs = "Blobs";

		internal const string str_WallColor = "WallColor";
		private static Color  def_WallColor = Color.BurlyWood;

		private Color _wallColor = def_WallColor;
		[Category(cat_Blobs)]
		[Description("Color of the wall indicators (default BurlyWood)")]
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
		[Description("Width of the wall indicators in pixels (1..10 default 3)")]
		[DefaultValue(def_WallWidth)]
		public int WallWidth
		{
			get { return _wallWidth; }
			set
			{
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_WallWidth].Value =
					_wallWidth = value.Viceroy(1,10);
				}
				else if ((_wallWidth = value.Viceroy(1,10)) != value) // on user-changed
				{
					_routeView.Options[str_WallWidth].Value = _wallWidth;
				}
			}
		}


		internal const string str_ContentColor = "ContentColor";
		private static Color  def_ContentColor = Color.DarkGoldenrod;

		private Color _contentColor = def_ContentColor;
		[Category(cat_Blobs)]
		[Description("Color of the content indicator (default DarkGoldenrod)")]
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
		[Description("Color of unselected nodes (default MediumSeaGreen)")]
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
		[Description("Color of spawn nodes (default GreenYellow)")]
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
		[Description("Color of selected nodes (default RoyalBlue)")]
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
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_NodeOpacity].Value =
					_nodeOpacity = value.Viceroy(0,255);
				}
				else if ((_nodeOpacity = value.Viceroy(0,255)) != value) // on user-changed
				{
					_routeView.Options[str_NodeOpacity].Value = _nodeOpacity;
				}
			}
		}



		private const string cat_Links = "Links";

		internal const string str_LinkColor = "LinkColor";
		private static Color  def_LinkColor = Color.OrangeRed;

		private Color _linkColor = def_LinkColor;
		[Category(cat_Links)]
		[Description("Color of unselected link lines (default OrangeRed)")]
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
		[Description("Width of unselected link lines in pixels (1..5 default 2)")]
		[DefaultValue(def_LinkWidth)]
		public int LinkWidth
		{
			get { return _linkWidth; }
			set
			{
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_LinkWidth].Value =
					_linkWidth = value.Viceroy(1,5);
				}
				else if ((_linkWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					_routeView.Options[str_LinkWidth].Value = _linkWidth;
				}
			}
		}


		internal const string str_LinkSelectedColor = "LinkSelectedColor";
		private static Color  def_LinkSelectedColor = Color.RoyalBlue;

		private Color _linkSelectedColor = def_LinkSelectedColor;
		[Category(cat_Links)]
		[Description("Color of selected link lines (default RoyalBlue)")]
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
		[Description("Width of selected link lines in pixels (1..5 default 2)")]
		[DefaultValue(def_LinkSelectedWidth)]
		public int LinkSelectedWidth
		{
			get { return _linkSelectedWidth; }
			set
			{
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_LinkSelectedWidth].Value =
					_linkSelectedWidth = value.Viceroy(1,5);
				}
				else if ((_linkSelectedWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					_routeView.Options[str_LinkSelectedWidth].Value = _linkSelectedWidth;
				}
			}
		}



		private const string cat_Panel = "Panel";

		private const string str_ShowOverlay = "ShowOverlay";
		private const bool   def_ShowOverlay = true;

		private bool _showOverlay = def_ShowOverlay;
		[Category(cat_Panel)]
		[Description("True to display cursor info (default True)")]
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
		[Description("True to display the spawn/patrol bars (default True)")]
		[DefaultValue(true)]
		public bool ShowPriorityBars
		{
			get { return _showPriorityBars; }
			set { _showPriorityBars = value; }
		}


		private const string str_ReduceDraws = "ReduceDraws";
		private const bool   def_ReduceDraws = false;

		private bool _reduceDraws = def_ReduceDraws;
		[Category(cat_Panel)]
		[Description("True to reduce the frequency of draw-calls to the panel."
					+ " If so the InfoOverlay doesn't track exactly with the"
					+ " cursor but the panel feels solid. Note that when option "
					+ str_ShowOverlay + " is false draws will be reduced"
					+ " regardless (default False)")]
		[DefaultValue(false)]
		public bool ReduceDraws
		{
			get { return _reduceDraws; }
			set { _reduceDraws = value; }
		}



		private const string cat_Connector = "Connector";

		private const int LinkOff     = 0;
//		private const int LinkForward = 1;
		private const int LinkForBac  = 2;

		private const string str_StartConnector = "StartConnector";
		private const int    def_StartConnector = LinkOff;

		private int _startConnector = def_StartConnector;
		[Category(cat_Connector)]
		[Description(@"The selected connector button when Mapview starts.
0 - auto-link off (default)
1 - link forward
2 - link forward and back")]
		[DefaultValue(0)]
		public int StartConnector
		{
			get { return _startConnector; }
			set
			{
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					_routeView.Options[str_StartConnector].Value =
					_startConnector = value.Viceroy(LinkOff, LinkForBac);
				}
				else if ((_startConnector = value.Viceroy(LinkOff, LinkForBac)) != value) // on user-changed
				{
					_routeView.Options[str_StartConnector].Value = _startConnector;
				}
			}
		}



		private const string cat_nonBrowsable = "nonBrowsable";

		private const string str_DescriptionHeight = "DescriptionHeight";
		private const int    def_DescriptionHeight = 70; // header(22) + 4 line(12)

		private int _descriptionHeight = def_DescriptionHeight;
		[Browsable(false)]
		[Category(cat_nonBrowsable)]
		[Description("The height of the Description area at the bottom of Options")]
		[DefaultValue(def_DescriptionHeight)]
		public int DescriptionHeight
		{
			get { return _descriptionHeight; }
			set
			{
				ObserverManager.RouteView.Control.Options[str_DescriptionHeight].Value =
				_descriptionHeight = value;
			}
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
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					RouteView.Options[str_SelectorWidth].Value =
					_selectorWidth = value.Clamp(1,6);
				}
				else if ((_selectorWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_SelectorWidth].Value = _selectorWidth;
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
				if ((RouteView._foptions as OptionsForm) == null) // on load
				{
					RouteView.Options[str_SelectedWidth].Value =
					_selectedWidth = value.Clamp(1,6);
				}
				else if ((_selectedWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					RouteView.Options[str_SelectedWidth].Value = _selectedWidth;
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
			//DSShared.LogFile.WriteLine("RouteViewOptionables.LoadDefaults()");

			Pen pen; SolidBrush brush; Color color;

			pen = new Pen(def_GridLineColor, def_GridLineWidth);
			RouteControl.RoutePens[str_GridLineColor] = pen;

			pen = new Pen(def_GridLine10Color, def_GridLine10Width);
			RouteControl.RoutePens[str_GridLine10Color] = pen;

			pen = new Pen(def_WallColor, def_WallWidth);
			RouteControl.RoutePens[str_WallColor] = pen;
			RouteControl.ToolWall = new BlobColorTool(pen, "RouteToolWall");

			brush = new SolidBrush(def_ContentColor);
			RouteControl.RouteBrushes[str_ContentColor] = brush;
			RouteControl.ToolContent = new BlobColorTool(brush, BlobDrawService.LINEWIDTH_CONTENT, "RouteToolContent");

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeSpawnColor);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeSpawnColor] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeSelectedColor);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeSelectedColor] = brush;

			pen = new Pen(def_LinkColor, def_LinkWidth);
			RouteControl.RoutePens[str_LinkColor] = pen;

			pen = new Pen(def_LinkSelectedColor, def_LinkSelectedWidth);
			RouteControl.RoutePens[str_LinkSelectedColor] = pen;


			OptionChangedEvent changer0 = OnOptionChanged;
			OptionChangedEvent changer1 = OnDescriptionHeightChanged;

			options.CreateOptionDefault(str_GridLineColor,     def_GridLineColor,     changer0);
			options.CreateOptionDefault(str_GridLineWidth,     def_GridLineWidth,     changer0);
			options.CreateOptionDefault(str_GridLine10Color,   def_GridLine10Color,   changer0);
			options.CreateOptionDefault(str_GridLine10Width,   def_GridLine10Width,   changer0);

			options.CreateOptionDefault(str_WallColor,         def_WallColor,         changer0);
			options.CreateOptionDefault(str_WallWidth,         def_WallWidth,         changer0);
			options.CreateOptionDefault(str_ContentColor,      def_ContentColor,      changer0);

			options.CreateOptionDefault(str_NodeColor,         def_NodeColor,         changer0);
			options.CreateOptionDefault(str_NodeSpawnColor,    def_NodeSpawnColor,    changer0);
			options.CreateOptionDefault(str_NodeSelectedColor, def_NodeSelectedColor, changer0);
			options.CreateOptionDefault(str_NodeOpacity,       def_NodeOpacity,       changer0);

			options.CreateOptionDefault(str_LinkColor,         def_LinkColor,         changer0);
			options.CreateOptionDefault(str_LinkWidth,         def_LinkWidth,         changer0);
			options.CreateOptionDefault(str_LinkSelectedColor, def_LinkSelectedColor, changer0);
			options.CreateOptionDefault(str_LinkSelectedWidth, def_LinkSelectedWidth, changer0);

			options.CreateOptionDefault(str_ShowOverlay,       def_ShowOverlay,       changer0);
			options.CreateOptionDefault(str_ShowPriorityBars,  def_ShowPriorityBars,  changer0);
			options.CreateOptionDefault(str_ReduceDraws,       def_ReduceDraws,       changer0);

			options.CreateOptionDefault(str_StartConnector,    def_StartConnector,    changer0);

			options.CreateOptionDefault(str_DescriptionHeight, def_DescriptionHeight, changer1);
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
				case str_ReduceDraws:       ReduceDraws       =  (bool)val;                           return;

				case str_StartConnector:    StartConnector    =   (int)val;                           return;
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

			switch (key)
			{
				case str_ContentColor:
					// Do not apply alpha to ContentColor.
					RouteControl.RouteBrushes[str_ContentColor].Color = color;

					RouteControl.ToolContent.Dispose();
					RouteControl.ToolContent = new BlobColorTool(
															RouteControl.RouteBrushes[str_ContentColor],
															BlobDrawService.LINEWIDTH_CONTENT,
															"RouteToolContent");

					if (MainViewF.that._fcolors != null)
						MainViewF.that._fcolors.UpdateRouteViewBlobColors();
					break;

				default: // is Node color
					color = Color.FromArgb(NodeOpacity, color);
					RouteControl.RouteBrushes[key].Color = color;

					switch (key)
					{
						case str_NodeColor:
						case str_NodeSpawnColor:
							ObserverManager.RouteView   .Control     .SetInfoOverColor();
							ObserverManager.TopRouteView.ControlRoute.SetInfoOverColor();
							break;

						case str_NodeSelectedColor:
							RouteView.SetInfoSelectedColor();
							break;
					}
					break;
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
			RouteControl.RouteBrushes[str_NodeColor].Color = color;

			color = Color.FromArgb((int)val, NodeSpawnColor);
			RouteControl.RouteBrushes[str_NodeSpawnColor].Color = color;

			color = Color.FromArgb((int)val, NodeSelectedColor);
			RouteControl.RouteBrushes[str_NodeSelectedColor].Color = color;
		}


		/// <summary>
		/// Stores the property panel's Description area's height when the user
		/// changes it.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnDescriptionHeightChanged(string key, object val)
		{
			DescriptionHeight = (int)val;
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
			RouteControl.RoutePens[key].Color = (Color)val;

			switch (key)
			{
				case str_WallColor:
					RouteControl.ToolWall.Dispose();
					RouteControl.ToolWall = new BlobColorTool(RouteControl.RoutePens[key], "RouteToolWall");

					if (MainViewF.that._fcolors != null)
						MainViewF.that._fcolors.UpdateRouteViewBlobColors();
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
			RouteControl.RoutePens[key = WidthToColor(key)].Width = (int)val;

			switch (key)
			{
				case str_WallColor: // doh!
					RouteControl.ToolWall.Dispose();
					RouteControl.ToolWall = new BlobColorTool(RouteControl.RoutePens[key], "RouteToolWall");
					break;
			}
		}

		/// <summary>
		/// Maps a pen's width-key to its corresponding color-key. Because
		/// RoutePens are stored and accessed with their color-keys not their
		/// width-keys.
		/// </summary>
		/// <param name="key">width-key</param>
		/// <returns>the width-key's corresponding color-key</returns>
		private static string WidthToColor(string key)
		{
			switch (key)
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
