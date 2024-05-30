using System;
using System.ComponentModel;
using System.Drawing;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Properties for
	/// <c><see cref="RouteView.Optionables">RouteView.Optionables</see></c>
	/// that appear in RouteView's <c><see cref="Options"/></c>.
	/// </summary>
	internal sealed class RouteViewOptionables
	{
		internal static void DisposeOptionables()
		{
			//DSShared.Logfile.Log("RouteViewOptionables.DisposeOptionables() static");
			foreach (var pair in RouteControl.RoutePens)
				pair.Value.Dispose();

			foreach (var pair in RouteControl.RouteBrushes)
				pair.Value.Dispose();

			RouteControl.ToolWall   .Dispose();
			RouteControl.ToolContent.Dispose();
		}


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
		private static Color  def_GridLineColor = SystemColors.ControlText;

		private Color _gridLineColor = def_GridLineColor;
		[Category(cat_Grid)]
		[Description("Color of the grid lines (default System.ControlText)")]
		[DefaultValue(typeof(Color), "ControlText")]
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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_GridLineWidth].Value =
					_gridLineWidth = value.Viceroy(1,5);
				}
				else if ((_gridLineWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					RouteView.Options[str_GridLineWidth].Value = _gridLineWidth;
				}
			}
		}


		internal const string str_GridLine10Color = "GridLine10Color";
		private static Color  def_GridLine10Color = SystemColors.ControlText;

		private Color _gridLine10Color = def_GridLine10Color;
		[Category(cat_Grid)]
		[Description("Color of every tenth grid line (default System.ControlText)")]
		[DefaultValue(typeof(Color), "ControlText")]
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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_GridLine10Width].Value =
					_gridLine10Width = value.Viceroy(1,5);
				}
				else if ((_gridLine10Width = value.Viceroy(1,5)) != value) // on user-changed
				{
					RouteView.Options[str_GridLine10Width].Value = _gridLine10Width;
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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_WallWidth].Value =
					_wallWidth = value.Viceroy(1,10);
				}
				else if ((_wallWidth = value.Viceroy(1,10)) != value) // on user-changed
				{
					RouteView.Options[str_WallWidth].Value = _wallWidth;
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

		internal const string str_NodeColor0 = "NodeColor0";
		private static Color  def_NodeColor0 = Color.LightGreen;

		private Color _nodeColor0 = def_NodeColor0;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 0 Civilian/Scout (default LightGreen)")]
		[DefaultValue(typeof(Color), "LightGreen")]
		public Color NodeColor0
		{
			get { return _nodeColor0; }
			set { _nodeColor0 = value; }
		}


		internal const string str_NodeColor1 = "NodeColor1";
		private static Color  def_NodeColor1 = Color.Khaki;

		private Color _nodeColor1 = def_NodeColor1;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 1 XCOM (default Khaki)")]
		[DefaultValue(typeof(Color), "Khaki")]
		public Color NodeColor1
		{
			get { return _nodeColor1; }
			set { _nodeColor1 = value; }
		}


		internal const string str_NodeColor2 = "NodeColor2";
		private static Color  def_NodeColor2 = Color.MediumAquamarine;

		private Color _nodeColor2 = def_NodeColor2;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 2 Soldier (default MediumAquamarine)")]
		[DefaultValue(typeof(Color), "MediumAquamarine")]
		public Color NodeColor2
		{
			get { return _nodeColor2; }
			set { _nodeColor2 = value; }
		}


		internal const string str_NodeColor3 = "NodeColor3";
		private static Color  def_NodeColor3 = Color.PaleTurquoise;

		private Color _nodeColor3 = def_NodeColor3;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 3 Navigator/SquadLeader (default PaleTurquoise)")]
		[DefaultValue(typeof(Color), "PaleTurquoise")]
		public Color NodeColor3
		{
			get { return _nodeColor3; }
			set { _nodeColor3 = value; }
		}


		internal const string str_NodeColor4 = "NodeColor4";
		private static Color  def_NodeColor4 = Color.Violet;

		private Color _nodeColor4 = def_NodeColor4;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 4 Leader/Commander (default Violet)")]
		[DefaultValue(typeof(Color), "Violet")]
		public Color NodeColor4
		{
			get { return _nodeColor4; }
			set { _nodeColor4 = value; }
		}


		internal const string str_NodeColor5 = "NodeColor5";
		private static Color  def_NodeColor5 = Color.Thistle;

		private Color _nodeColor5 = def_NodeColor5;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 5 Engineer/Medic (default Thistle)")]
		[DefaultValue(typeof(Color), "Thistle")]
		public Color NodeColor5
		{
			get { return _nodeColor5; }
			set { _nodeColor5 = value; }
		}


		internal const string str_NodeColor6 = "NodeColor6";
		private static Color  def_NodeColor6 = Color.LightSkyBlue;

		private Color _nodeColor6 = def_NodeColor6;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 6 Terrorist1 (default LightSkyBlue)")]
		[DefaultValue(typeof(Color), "LightSkyBlue")]
		public Color NodeColor6
		{
			get { return _nodeColor6; }
			set { _nodeColor6 = value; }
		}


		internal const string str_NodeColor7 = "NodeColor7";
		private static Color  def_NodeColor7 = Color.Crimson;

		private Color _nodeColor7 = def_NodeColor7;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 7 Medic/Technician (default Crimson)")]
		[DefaultValue(typeof(Color), "Crimson")]
		public Color NodeColor7
		{
			get { return _nodeColor7; }
			set { _nodeColor7 = value; }
		}


		internal const string str_NodeColor8 = "NodeColor8";
		private static Color  def_NodeColor8 = Color.LightSteelBlue;

		private Color _nodeColor8 = def_NodeColor8;
		[Category(cat_Nodes)]
		[Description("Color of nodes of Rank 8 Terrorist2 (default LightSteelBlue)")]
		[DefaultValue(typeof(Color), "LightSteelBlue")]
		public Color NodeColor8
		{
			get { return _nodeColor8; }
			set { _nodeColor8 = value; }
		}


		internal const string str_NodeColorGhosted = "NodeColorGhosted";
		private static Color  def_NodeColorGhosted = Color.Gainsboro;

		private Color _nodeColorGhosted = def_NodeColorGhosted;
		[Category(cat_Nodes)]
		[Description("Color of nonspawn nodes when ghosted (default Gainsboro)")]
		[DefaultValue(typeof(Color), "Gainsboro")]
		public Color NodeColorGhosted
		{
			get { return _nodeColorGhosted; }
			set { _nodeColorGhosted = value; }
		}


		internal const string str_NodeColorInvalid = "NodeColorInvalid";
		private static Color  def_NodeColorInvalid = Color.Indigo;

		private Color _nodeColorInvalid = def_NodeColorInvalid;
		[Category(cat_Nodes)]
		[Description("Color of nodes with an invalid rank (default Indigo)")]
		[DefaultValue(typeof(Color), "Indigo")]
		public Color NodeColorInvalid
		{
			get { return _nodeColorInvalid; }
			set { _nodeColorInvalid = value; }
		}


//		internal const string str_NodeColor = "NodeColor";
//		private static Color  def_NodeColor = Color.MediumSeaGreen;
//
//		private Color _nodeColor = def_NodeColor;
//		[Category(cat_Nodes)]
//		[Description("Color of unselected nodes (default MediumSeaGreen)")]
//		[DefaultValue(typeof(Color), "MediumSeaGreen")]
//		public Color NodeColor
//		{
//			get { return _nodeColor; }
//			set { _nodeColor = value; }
//		}


//		internal const string str_NodeSpawnColor = "NodeSpawnColor";
//		private static Color  def_NodeSpawnColor = Color.GreenYellow;
//
//		private Color _nodeSpawnColor = def_NodeSpawnColor;
//		[Category(cat_Nodes)]
//		[Description("Color of spawn nodes (default GreenYellow)")]
//		[DefaultValue(typeof(Color), "GreenYellow")]
//		public Color NodeSpawnColor
//		{
//			get { return _nodeSpawnColor; }
//			set { _nodeSpawnColor = value; }
//		}


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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_NodeOpacity].Value =
					_nodeOpacity = value.Viceroy(0,255);
				}
				else if ((_nodeOpacity = value.Viceroy(0,255)) != value) // on user-changed
				{
					RouteView.Options[str_NodeOpacity].Value = _nodeOpacity;
				}
			}
		}



		private const string cat_Links = "Links";

		internal const string str_LinkColor = "LinkColor";
		private static Color  def_LinkColor = Color.DarkOrange;

		private Color _linkColor = def_LinkColor;
		[Category(cat_Links)]
		[Description("Color of unselected link lines (default DarkOrange)")]
		[DefaultValue(typeof(Color), "DarkOrange")]
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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_LinkWidth].Value =
					_linkWidth = value.Viceroy(1,5);
				}
				else if ((_linkWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					RouteView.Options[str_LinkWidth].Value = _linkWidth;
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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_LinkSelectedWidth].Value =
					_linkSelectedWidth = value.Viceroy(1,5);
				}
				else if ((_linkSelectedWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					RouteView.Options[str_LinkSelectedWidth].Value = _linkSelectedWidth;
				}
			}
		}



		private const string cat_General = "General";

		private const string str_ShowPriorityBars = "ShowPriorityBars";
		private const bool   def_ShowPriorityBars = true;

		private bool _showPriorityBars = def_ShowPriorityBars;
		[Category(cat_General)]
		[Description("True to display the spawn/patrol bars (default True)")]
		[DefaultValue(true)]
		public bool ShowPriorityBars
		{
			get { return _showPriorityBars; }
			set { _showPriorityBars = value; }
		}


		private const int LinkOff     = 0;
//		private const int LinkForward = 1;
		private const int LinkForBac  = 2;

		private const string str_StartConnector = "StartConnector";
		private const int    def_StartConnector = LinkOff;

		private int _startConnector = def_StartConnector;
		[Category(cat_General)]
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
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_StartConnector].Value =
					_startConnector = value.Viceroy(LinkOff, LinkForBac);
				}
				else if ((_startConnector = value.Viceroy(LinkOff, LinkForBac)) != value) // on user-changed
				{
					RouteView.Options[str_StartConnector].Value = _startConnector;
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
				RouteView.Options[str_DescriptionHeight].Value =
				_descriptionHeight = value;
			}
		}



		private const string cat_PanelColors = "PanelColors";

		private const string str_PanelBackcolor = "PanelBackcolor";
		private static Color def_PanelBackcolor = SystemColors.Control;

		private Color _panelBackcolor = def_PanelBackcolor;
		[Category(cat_PanelColors)]
		[Description("Color of the panel background (default System.Control)")]
		[DefaultValue(typeof(Color), "Control")]
		public Color PanelBackcolor
		{
			get { return _panelBackcolor; }
			set { _panelBackcolor = value; }
		}


		private  const string str_PanelForecolor = "PanelForecolor";
		internal static Color def_PanelForecolor = SystemColors.ControlText;

		private Color _panelForecolor = def_PanelForecolor;
		[Category(cat_PanelColors)]
		[Description("Color of the panel font (default System.ControlText)")]
		[DefaultValue(typeof(Color), "ControlText")]
		public Color PanelForecolor
		{
			get { return _panelForecolor; }
			set { _panelForecolor = value; }
		}


		private const string str_FieldsBackcolor = "FieldsBackcolor";
		private static Color def_FieldsBackcolor = SystemColors.Control;

		private Color _fieldsBackcolor = def_FieldsBackcolor;
		[Category(cat_PanelColors)]
		[Description("Color of the fields background (default System.Control)")]
		[DefaultValue(typeof(Color), "Control")]
		public Color FieldsBackcolor
		{
			get { return _fieldsBackcolor; }
			set { _fieldsBackcolor = value; }
		}


		private const string str_FieldsForecolor = "FieldsForecolor";
		private static Color def_FieldsForecolor = SystemColors.ControlText;

		private Color _fieldsForecolor = def_FieldsForecolor;
		[Category(cat_PanelColors)]
		[Description(@"Color of the fields font (default System.ControlText)
This does not affect the color of fields if they are disabled (ie. no node is currently selected)")]
		[DefaultValue(typeof(Color), "ControlText")]
		public Color FieldsForecolor
		{
			get { return _fieldsForecolor; }
			set { _fieldsForecolor = value; }
		}


		private const string str_FieldsForecolorHighlight = "FieldsForecolorHighlight";
		private static Color def_FieldsForecolorHighlight = Color.MediumVioletRed;

		private Color _fieldsForecolorHighlight = def_FieldsForecolorHighlight;
		[Category(cat_PanelColors)]
		[Description(@"Color of the fields font highlight (default MediumVioletRed)
This is the color of the text on the Save button (if enabled) and the color for out-of-bounds link destinations")]
		[DefaultValue(typeof(Color), "MediumVioletRed")]
		public Color FieldsForecolorHighlight
		{
			get { return _fieldsForecolorHighlight; }
			set { _fieldsForecolorHighlight = value; }
		}



		private const string cat_Overlay = "Overlay";

		private const string str_ShowOverlay = "ShowOverlay";
		private const bool   def_ShowOverlay = true;

		private bool _showOverlay = def_ShowOverlay;
		[Category(cat_Overlay)]
		[Description("True to display info overlay (default True)")]
		[DefaultValue(true)]
		public bool ShowOverlay
		{
			get { return _showOverlay; }
			set { _showOverlay = value; }
		}


		private const string str_ReduceDraws = "ReduceDraws";
		private const bool   def_ReduceDraws = false;

		private bool _reduceDraws = def_ReduceDraws;
		[Category(cat_Overlay)]
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


		private  const string str_OverlayForecolor = "OverlayForecolor";
		internal static Color def_OverlayForecolor = Color.Yellow;

		private Color _overlayForecolor = def_OverlayForecolor;
		[Category(cat_Overlay)]
		[Description("Color of the overlay's text (default Yellow)")]
		[DefaultValue(typeof(Color), "Yellow")]
		public Color OverlayForecolor
		{
			get {return _overlayForecolor; }
			set { _overlayForecolor = value; }
		}


		private  const string str_OverlayBorderColor = "OverlayBorderColor";
		internal static Color def_OverlayBorderColor = Color.Black;

		private Color _overlayBorderColor = def_OverlayBorderColor;
		[Category(cat_Overlay)]
		[Description("Color of the overlay's border (default Black)")]
		[DefaultValue(typeof(Color), "Black")]
		public Color OverlayBorderColor
		{
			get {return _overlayBorderColor; }
			set { _overlayBorderColor = value; }
		}


		private  const string str_OverlayFillColor = "OverlayFillColor";
		internal static Color def_OverlayFillColor = Color.DarkSlateBlue;

		private Color _overlayFillColor = def_OverlayFillColor;
		[Category(cat_Overlay)]
		[Description("Color of the overlay (default DarkSlateBlue)")]
		[DefaultValue(typeof(Color), "DarkSlateBlue")]
		public Color OverlayFillColor
		{
			get {return _overlayFillColor; }
			set { _overlayFillColor = value; }
		}


		private  const string str_OverlayFillOpacity = "OverlayFillOpacity";
		internal const int    def_OverlayFillOpacity = 200;

		private int _overlayFillOpacity = def_OverlayFillOpacity;
		[Category(cat_Overlay)]
		[Description("Opacity of the overlay (0..255 default 200)")]
		[DefaultValue(def_OverlayFillOpacity)]
		public int OverlayFillOpacity
		{
			get { return _overlayFillOpacity; }
			set
			{
				if (RouteView._foptions == null) // on load
				{
					RouteView.Options[str_OverlayFillOpacity].Value =
					_overlayFillOpacity = value.Viceroy(0,255);
				}
				else if ((_overlayFillOpacity = value.Viceroy(0,255)) != value) // on user-changed
				{
					RouteView.Options[str_OverlayFillOpacity].Value = _overlayFillOpacity;
				}
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
				if (RouteView._foptions == null) // on load
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
				if (RouteView._foptions == null) // on load
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

 		// + SpotColor
		#endregion Properties (optionable)


		#region Methods
		/// <summary>
		/// Instantiates pens, brushes, and colortools used by RouteView's draw
		/// routines with default values. Adds default keyval pairs to
		/// RouteView's optionables and an option-changer is assigned to each.
		/// The default values were assigned to RouteView's optionable
		/// properties when those properties were instantiated above.
		/// </summary>
		/// <param name="options"><c><see cref="RouteView.Options">RouteView.Options</see></c></param>
		internal void LoadDefaults(Options options)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.LoadDefaults()");

			Pen pen; SolidBrush brush; Color color;

			pen = new Pen(def_GridLineColor, def_GridLineWidth);
			RouteControl.RoutePens[str_GridLineColor] = pen;

			pen = new Pen(def_GridLine10Color, def_GridLine10Width);
			RouteControl.RoutePens[str_GridLine10Color] = pen;

			pen = new Pen(def_WallColor, def_WallWidth);
			RouteControl.RoutePens[str_WallColor] = pen;
			RouteControl.ToolWall = new BlobColorTool(pen);

			brush = new SolidBrush(def_ContentColor);
			RouteControl.RouteBrushes[str_ContentColor] = brush;
			RouteControl.ToolContent = new BlobColorTool(brush, BlobDrawCoordinator.DefaultLinewidthContent);

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor0);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor0] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor1);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor1] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor2);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor2] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor3);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor3] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor4);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor4] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor5);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor5] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor6);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor6] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor7);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor7] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColor8);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColor8] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColorGhosted);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColorGhosted] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeColorInvalid);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeColorInvalid] = brush;

//			color = Color.FromArgb(def_NodeOpacity, def_NodeColor);
//			brush = new SolidBrush(color);
//			RouteControl.RouteBrushes[str_NodeColor] = brush;

//			color = Color.FromArgb(def_NodeOpacity, def_NodeSpawnColor);
//			brush = new SolidBrush(color);
//			RouteControl.RouteBrushes[str_NodeSpawnColor] = brush;

			color = Color.FromArgb(def_NodeOpacity, def_NodeSelectedColor);
			brush = new SolidBrush(color);
			RouteControl.RouteBrushes[str_NodeSelectedColor] = brush;

			pen = new Pen(def_LinkColor, def_LinkWidth);
			RouteControl.RoutePens[str_LinkColor] = pen;

			pen = new Pen(def_LinkSelectedColor, def_LinkSelectedWidth);
			RouteControl.RoutePens[str_LinkSelectedColor] = pen;


			OptionChangedEvent changer0 = OnOptionChanged;
			OptionChangedEvent changer1 = OnDescriptionHeightChanged;
			OptionChangedEvent changer2 = OnPanelColorChanged;

			options.CreateOptionDefault(str_PanelBackcolor,           def_PanelBackcolor,           changer2);
			options.CreateOptionDefault(str_PanelForecolor,           def_PanelForecolor,           changer2);
			options.CreateOptionDefault(str_FieldsBackcolor,          def_FieldsBackcolor,          changer2);
			options.CreateOptionDefault(str_FieldsForecolor,          def_FieldsForecolor,          changer2);
			options.CreateOptionDefault(str_FieldsForecolorHighlight, def_FieldsForecolorHighlight, changer2);

			options.CreateOptionDefault(str_ShowOverlay,              def_ShowOverlay,              changer0);
			options.CreateOptionDefault(str_ReduceDraws,              def_ReduceDraws,              changer0);
			options.CreateOptionDefault(str_OverlayForecolor,         def_OverlayForecolor,         changer2);
			options.CreateOptionDefault(str_OverlayBorderColor,       def_OverlayBorderColor,       changer2);
			options.CreateOptionDefault(str_OverlayFillColor,         def_OverlayFillColor,         changer2);
			options.CreateOptionDefault(str_OverlayFillOpacity,       def_OverlayFillOpacity,       changer2);

			options.CreateOptionDefault(str_GridLineColor,            def_GridLineColor,            changer0);
			options.CreateOptionDefault(str_GridLineWidth,            def_GridLineWidth,            changer0);
			options.CreateOptionDefault(str_GridLine10Color,          def_GridLine10Color,          changer0);
			options.CreateOptionDefault(str_GridLine10Width,          def_GridLine10Width,          changer0);

			options.CreateOptionDefault(str_WallColor,                def_WallColor,                changer0);
			options.CreateOptionDefault(str_WallWidth,                def_WallWidth,                changer0);
			options.CreateOptionDefault(str_ContentColor,             def_ContentColor,             changer0);

			options.CreateOptionDefault(str_NodeColor0,               def_NodeColor0,               changer0);
			options.CreateOptionDefault(str_NodeColor1,               def_NodeColor1,               changer0);
			options.CreateOptionDefault(str_NodeColor2,               def_NodeColor2,               changer0);
			options.CreateOptionDefault(str_NodeColor3,               def_NodeColor3,               changer0);
			options.CreateOptionDefault(str_NodeColor4,               def_NodeColor4,               changer0);
			options.CreateOptionDefault(str_NodeColor5,               def_NodeColor5,               changer0);
			options.CreateOptionDefault(str_NodeColor6,               def_NodeColor6,               changer0);
			options.CreateOptionDefault(str_NodeColor7,               def_NodeColor7,               changer0);
			options.CreateOptionDefault(str_NodeColor8,               def_NodeColor8,               changer0);
			options.CreateOptionDefault(str_NodeColorGhosted,         def_NodeColorGhosted,         changer0);
			options.CreateOptionDefault(str_NodeColorInvalid,         def_NodeColorInvalid,         changer0);
//			options.CreateOptionDefault(str_NodeColor,                def_NodeColor,                changer0);
//			options.CreateOptionDefault(str_NodeSpawnColor,           def_NodeSpawnColor,           changer0);
			options.CreateOptionDefault(str_NodeSelectedColor,        def_NodeSelectedColor,        changer0);
			options.CreateOptionDefault(str_NodeOpacity,              def_NodeOpacity,              changer0);

			options.CreateOptionDefault(str_LinkColor,                def_LinkColor,                changer0);
			options.CreateOptionDefault(str_LinkWidth,                def_LinkWidth,                changer0);
			options.CreateOptionDefault(str_LinkSelectedColor,        def_LinkSelectedColor,        changer0);
			options.CreateOptionDefault(str_LinkSelectedWidth,        def_LinkSelectedWidth,        changer0);

			options.CreateOptionDefault(str_ShowPriorityBars,         def_ShowPriorityBars,         changer0);
			options.CreateOptionDefault(str_StartConnector,           def_StartConnector,           changer0);

			options.CreateOptionDefault(str_DescriptionHeight,        def_DescriptionHeight,        changer1);


			ObserverManager.RouteView   .Control     .SetNodeColors();
			ObserverManager.TopRouteView.ControlRoute.SetNodeColors();
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Changes the colors of RouteView's panel.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnPanelColorChanged(string key, object val)
		{
			//Logfile.Log("RouteViewOptionables.OnPanelColorChanged() key= " + key);

			switch (key)
			{
				case str_PanelBackcolor:
					ObserverManager.RouteView   .Control     .RouteControl.BackColor =
					ObserverManager.TopRouteView.ControlRoute.RouteControl.BackColor = (PanelBackcolor = (Color)val);
					break;

				case str_PanelForecolor:
					RouteControl.RosaryBrush.Color = (PanelForecolor = (Color)val);
					RouteView.InvalidatePanels();
					break;


				case str_FieldsBackcolor:
					FieldsBackcolor = (Color)val;
					RouteView.SetFieldsBackcolor();
					break;

				case str_FieldsForecolor:
					FieldsForecolor = (Color)val;
					ObserverManager.RouteView   .Control     .SetFieldsForecolor();
					ObserverManager.TopRouteView.ControlRoute.SetFieldsForecolor();

					RouteView.SetSelectedInfoColor(true);
					break;

				case str_FieldsForecolorHighlight:
					FieldsForecolorHighlight = (Color)val;
					RouteView.SetFieldsForecolorHighlight();
					break;


				case str_OverlayForecolor:
					RouteControl.OverlayForecolor.Color = (OverlayForecolor = (Color)val);
					break;

				case str_OverlayBorderColor:
					RouteControl.OverlayBorder.Color = (OverlayBorderColor = (Color)val);
					break;

				case str_OverlayFillColor:
					RouteControl.OverlayFill.Color = Color.FromArgb(OverlayFillOpacity, (OverlayFillColor = (Color)val));
					break;

				case str_OverlayFillOpacity:
					RouteControl.OverlayFill.Color = Color.FromArgb((OverlayFillOpacity = (int)val), OverlayFillColor);
					break;
			}
		}

		/// <summary>
		/// Sets the value of an optionable property and refreshes the RouteView
		/// and TopRouteView(Route) controls.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnOptionChanged(string key, object val)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.OnOptionChanged() key= " + key);

			switch (key)
			{
				case str_GridLineColor:     GridLineColor     = (Color)val; ChangePenColor(key, val); break;
				case str_GridLineWidth:     GridLineWidth     =   (int)val; ChangePenWidth(key, val); break;
				case str_GridLine10Color:   GridLine10Color   = (Color)val; ChangePenColor(key, val); break;
				case str_GridLine10Width:   GridLine10Width   =   (int)val; ChangePenWidth(key, val); break;

				case str_WallColor:         WallColor         = (Color)val; ChangePenColor(key, val); break;
				case str_WallWidth:         WallWidth         =   (int)val; ChangePenWidth(key, val); break;
				case str_ContentColor:      ContentColor      = (Color)val; ChangeBruColor(key, val); break;

				case str_NodeColor0:        NodeColor0        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor1:        NodeColor1        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor2:        NodeColor2        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor3:        NodeColor3        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor4:        NodeColor4        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor5:        NodeColor5        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor6:        NodeColor6        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor7:        NodeColor7        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColor8:        NodeColor8        = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColorGhosted:  NodeColorGhosted  = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeColorInvalid:  NodeColorInvalid  = (Color)val; ChangeBruColor(key, val); break;
//				case str_NodeColor:         NodeColor         = (Color)val; ChangeBruColor(key, val); break;
//				case str_NodeSpawnColor:    NodeSpawnColor    = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeSelectedColor: NodeSelectedColor = (Color)val; ChangeBruColor(key, val); break;
				case str_NodeOpacity:       NodeOpacity       =   (int)val; ChangeBruOpaci(     val); break;

				case str_LinkColor:         LinkColor         = (Color)val; ChangePenColor(key, val); break;
				case str_LinkWidth:         LinkWidth         =   (int)val; ChangePenWidth(key, val); break;
				case str_LinkSelectedColor: LinkSelectedColor = (Color)val; ChangePenColor(key, val); break;
				case str_LinkSelectedWidth: LinkSelectedWidth =   (int)val; ChangePenWidth(key, val); break;

				case str_ShowOverlay:       ShowOverlay       =  (bool)val; break;
				case str_ShowPriorityBars:  ShowPriorityBars  =  (bool)val; break;
				case str_ReduceDraws:       ReduceDraws       =  (bool)val; return;
				case str_StartConnector:    StartConnector    =   (int)val; return;
			}

			RouteView.RefreshControls();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Fires when a brush-color changes in Options.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void ChangeBruColor(string key, object val)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.ChangeBruColor()");

			var color = (Color)val;

			switch (key)
			{
				case str_ContentColor: // do not apply alpha to ContentColor
					RouteControl.RouteBrushes[str_ContentColor].Color = color;
	
					RouteControl.ToolContent.Dispose();
					RouteControl.ToolContent = new BlobColorTool(
															RouteControl.RouteBrushes[str_ContentColor],
															BlobDrawCoordinator.DefaultLinewidthContent);

					if (MainViewF.that._fcolors != null)
						MainViewF.that._fcolors.UpdateRouteViewBlobColors();
					break;

//				default: // is Node color ->
//				str_NodeColor:
//				str_NodeSpawnColor:
				case str_NodeSelectedColor:
				case str_NodeColorGhosted:
					color = Color.FromArgb(NodeOpacity, color);
					RouteControl.RouteBrushes[key].Color = color;

//					if (key == str_NodeSelectedColor)
//					RouteView.SetSelectedInfoColor();

					break;

				case str_NodeColor0:
				case str_NodeColor1:
				case str_NodeColor2:
				case str_NodeColor3:
				case str_NodeColor4:
				case str_NodeColor5:
				case str_NodeColor6:
				case str_NodeColor7:
				case str_NodeColor8:
				case str_NodeColorInvalid:
					color = Color.FromArgb(NodeOpacity, color);
					RouteControl.RouteBrushes[key].Color = color;

					RouteView.SetSelectedInfoColor();
					break;
			}

			ObserverManager.RouteView   .Control     .SetNodeColors();
			ObserverManager.TopRouteView.ControlRoute.SetNodeColors();
		}

		/// <summary>
		/// Fires when a brush-opacity changes in Options.
		/// </summary>
		/// <param name="val"></param>
		private void ChangeBruOpaci(object val)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.ChangeBruOpaci()");

			Color color = Color.FromArgb((int)val, NodeColor0);
			RouteControl.RouteBrushes[str_NodeColor0].Color = color;

			color = Color.FromArgb((int)val, NodeColor1);
			RouteControl.RouteBrushes[str_NodeColor1].Color = color;

			color = Color.FromArgb((int)val, NodeColor2);
			RouteControl.RouteBrushes[str_NodeColor2].Color = color;

			color = Color.FromArgb((int)val, NodeColor3);
			RouteControl.RouteBrushes[str_NodeColor3].Color = color;

			color = Color.FromArgb((int)val, NodeColor4);
			RouteControl.RouteBrushes[str_NodeColor4].Color = color;

			color = Color.FromArgb((int)val, NodeColor5);
			RouteControl.RouteBrushes[str_NodeColor5].Color = color;

			color = Color.FromArgb((int)val, NodeColor6);
			RouteControl.RouteBrushes[str_NodeColor6].Color = color;

			color = Color.FromArgb((int)val, NodeColor7);
			RouteControl.RouteBrushes[str_NodeColor7].Color = color;

			color = Color.FromArgb((int)val, NodeColor8);
			RouteControl.RouteBrushes[str_NodeColor8].Color = color;

			color = Color.FromArgb((int)val, NodeColorGhosted);
			RouteControl.RouteBrushes[str_NodeColorGhosted].Color = color;

			color = Color.FromArgb((int)val, NodeColorInvalid);
			RouteControl.RouteBrushes[str_NodeColorInvalid].Color = color;

//			color = Color.FromArgb((int)val, NodeColor);
//			RouteControl.RouteBrushes[str_NodeColor].Color = color;

//			color = Color.FromArgb((int)val, NodeSpawnColor);
//			RouteControl.RouteBrushes[str_NodeSpawnColor].Color = color;

			color = Color.FromArgb((int)val, NodeSelectedColor);
			RouteControl.RouteBrushes[str_NodeSelectedColor].Color = color;

//			ObserverManager.RouteView   .Control     .SetNodeColors();	// doesn't appear to change transparency
//			ObserverManager.TopRouteView.ControlRoute.SetNodeColors();	// of backcolors in the RouteView groupbox ...
		}																// ie. backcolors are always solid I guess.


		/// <summary>
		/// Stores the property panel's Description area's height when the user
		/// changes it.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnDescriptionHeightChanged(string key, object val)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.OnDescriptionHeightChanged() key= " + key);
			DescriptionHeight = (int)val;
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Fires when a pen-color changes in Options.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private static void ChangePenColor(string key, object val)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.ChangePenColor()");

			RouteControl.RoutePens[key].Color = (Color)val;

			if (key == str_WallColor)
			{
				RouteControl.ToolWall.Dispose();
				RouteControl.ToolWall = new BlobColorTool(RouteControl.RoutePens[key]);

				if (MainViewF.that._fcolors != null)
					MainViewF.that._fcolors.UpdateRouteViewBlobColors();
			}
		}

		/// <summary>
		/// Fires when a pen-width changes in Options.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private static void ChangePenWidth(string key, object val)
		{
			//DSShared.Logfile.Log("RouteViewOptionables.ChangePenWidth()");

			RouteControl.RoutePens[key = WidthToColor(key)].Width = (int)val;

			if (key == str_WallColor) // doh!
			{
				RouteControl.ToolWall.Dispose();
				RouteControl.ToolWall = new BlobColorTool(RouteControl.RoutePens[key]);
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
