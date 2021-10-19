using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared;

using XCom;

using MapView.Forms.Observers;


namespace MapView.Forms.MainView
{
	/// <summary>
	/// Properties for <see cref="MainViewF"/> that appear in MainView's
	/// Options.
	/// </summary>
	internal sealed class MainViewOptionables
	{
		public void DisposeOptionables()
		{
			//Logfile.Log("MainViewOptionables.DisposeOptionables()");
			_overlay.BrushLayer.Dispose();
			_overlay.PenGrid   .Dispose();
			_overlay.PenGrid10 .Dispose();
			_overlay.PenSelect .Dispose();
		}


		#region Fields (static)
		internal const int TONER_NONE  = 0;
		internal const int TONER_GRAY  = 1;
		internal const int TONER_RED   = 2;
		internal const int TONER_GREEN = 3;
		internal const int TONER_BLUE  = 4;
		#endregion Fields (static)


		#region Fields
		private readonly MainViewOverlay _overlay;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal MainViewOptionables()
		{
			_overlay = MainViewOverlay.that;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets a specified viewer's show-on-startup property.
		/// </summary>
		/// <param name="f">the viewer</param>
		/// <param name="val">true to show on startup</param>
		internal void setStartPropertyValue(Form f, bool val)
		{
			if      (f is TileViewForm)     StartTileView     = val;
			else if (f is TopViewForm)      StartTopView      = val;
			else if (f is RouteViewForm)    StartRouteView    = val;
			else if (f is TopRouteViewForm) StartTopRouteView = val;
		}

		/// <summary>
		/// Gets the next tile-toner in a specified direction.
		/// </summary>
		/// <param name="dir">+/- 1</param>
		/// <returns></returns>
		internal int GetNextTileToner(int dir)
		{
			return (SelectedTileToner + dir + 5) % 5;
		}
		#endregion Methods (static)


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

		// NOTE: Observers are added to MainView's Options by
		// ViewersMenuManager.PopulateMenu(). They are used by
		// ViewersMenuManager.StartSecondStageBoosters().
		private const string cat_Observers = "Observers";			// category in the PropertyPanel

		private  const string str_StartTileView = "StartTileView";	// const string key of the Property
		internal const bool   def_StartTileView = true;				// default value of the Property

		private bool _starttileview = def_StartTileView;
		[Category(cat_Observers)]
		[Description("Open on load - TileView (default True)")]
		[DefaultValue(def_StartTileView)]
		public bool StartTileView									// the Property.
		{
			get { return _starttileview; }
			set { _starttileview = value; }
		}

		private  const string str_StartTopView = "StartTopView";
		internal const bool   def_StartTopView = true;

		private bool _starttopview = def_StartTopView;
		[Category(cat_Observers)]
		[Description("Open on load - TopView (default True)")]
		[DefaultValue(def_StartTopView)]
		public bool StartTopView
		{
			get { return _starttopview; }
			set { _starttopview = value; }
		}

		private  const string str_StartRouteView = "StartRouteView";
		internal const bool   def_StartRouteView = true;

		private bool _startrouteview = def_StartRouteView;
		[Category(cat_Observers)]
		[Description("Open on load - RouteView (default True)")]
		[DefaultValue(def_StartRouteView)]
		public bool StartRouteView
		{
			get { return _startrouteview; }
			set { _startrouteview = value; }
		}

		private  const string str_StartTopRouteView = "StartTopRouteView";
		internal const bool   def_StartTopRouteView = false;

		private bool _starttoprouteview = def_StartTopRouteView;
		[Category(cat_Observers)]
		[Description("Open on load - TopRouteView (default False)")]
		[DefaultValue(def_StartTopRouteView)]
		public bool StartTopRouteView
		{
			get { return _starttoprouteview; }
			set { _starttoprouteview = value; }
		}


		private const string str_StartTopRoutePage = "StartTopRoutePage";
		private const int    def_StartTopRoutePage = 0;

		private int _starttoproutepage = def_StartTopRoutePage;
		[Category(cat_Observers)]
		[Description(@"The tab-page to focus when TopRouteView starts
0 - TopView (default)
1 - RouteView
2 - TopView - recall state
3 - RouteView - recall state")]
		[DefaultValue(def_StartTopRoutePage)]
		public int StartTopRoutePage
		{
			get { return _starttoproutepage; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_StartTopRoutePage].Value =
					_starttoproutepage = value.Viceroy(0,3);
				}
				else if ((_starttoproutepage = value.Viceroy(0,3)) != value) // on user-changed
				{
					MainViewF.that.Options[str_StartTopRoutePage].Value = _starttoproutepage;
				}
			}
		}



		private const string cat_Grid = "Grid";

		internal const string str_GridVisible = "GridVisible";
		private  const bool   def_GridVisible = true;

		private bool _gridVisible = def_GridVisible;
		[Category(cat_Grid)]
		[Description(@"If true a grid will display at the current level of editing (F4 - On/Off)
(default True)")]
		[DefaultValue(def_GridVisible)]
		public bool GridVisible
		{
			get { return _gridVisible; }
			set { _gridVisible = value; }
		}


		private const string str_GridLayerColor = "GridLayerColor";
		private static Color def_GridLayerColor = Color.MediumVioletRed;

		private Color _gridLayerColor = def_GridLayerColor;
		[Category(cat_Grid)]
		[Description("Color of the grid (default MediumVioletRed)")]
		[DefaultValue(typeof(Color), "MediumVioletRed")]
		public Color GridLayerColor
		{
			get { return _gridLayerColor; }
			set { _gridLayerColor = value; }
		}

		private const string str_GridLayerOpacity = "GridLayerOpacity";
		private const int    def_GridLayerOpacity = 120;

		private int _gridLayerOpacity = def_GridLayerOpacity;
		[Category(cat_Grid)]
		[Description("Opacity of the grid (0..255 default 120)")]
		[DefaultValue(def_GridLayerOpacity)]
		public int GridLayerOpacity
		{
			get { return _gridLayerOpacity; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_GridLayerOpacity].Value =
					_gridLayerOpacity = value.Viceroy(0,255);
				}
				else if ((_gridLayerOpacity = value.Viceroy(0,255)) != value) // on user-changed
				{
					MainViewF.that.Options[str_GridLayerOpacity].Value = _gridLayerOpacity;
				}
			}
		}


		private const string str_GridLineColor = "GridLineColor";
		private static Color def_GridLineColor = Color.Black;

		private Color _gridLineColor = def_GridLineColor;
		[Category(cat_Grid)]
		[Description("Color of the lines that draw the grid (default Black)")]
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
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_GridLineWidth].Value =
					_gridLineWidth = value.Viceroy(1,5);
				}
				else if ((_gridLineWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					MainViewF.that.Options[str_GridLineWidth].Value = _gridLineWidth;
				}
			}
		}


		private const string str_GridLine10Color = "GridLine10Color";
		private static Color def_GridLine10Color = Color.Black;

		private Color _gridLine10Color = def_GridLine10Color;
		[Category(cat_Grid)]
		[Description("Color of every tenth line on the grid (default Black)")]
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
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_GridLine10Width].Value =
					_gridLine10Width = value.Viceroy(1,5);
				}
				else if ((_gridLine10Width = value.Viceroy(1,5)) != value) // on user-changed
				{
					MainViewF.that.Options[str_GridLine10Width].Value = _gridLine10Width;
				}
			}
		}



		private const string cat_Selection = "Selection";

		private const string str_SelectionBorderColor = "SelectionBorderColor";
		private static Color def_SelectionBorderColor = Color.Tomato;

		private Color _selectionBorderColor = def_SelectionBorderColor;
		[Category(cat_Selection)]
		[Description("Color of the border of selected tiles (default Tomato)")]
		[DefaultValue(typeof(Color), "Tomato")]
		public Color SelectionBorderColor
		{
			get { return _selectionBorderColor; }
			set { _selectionBorderColor = value; }
		}

		private const string str_SelectionBorderOpacity = "SelectionBorderOpacity";
		private const int    def_SelectionBorderOpacity = 255;

		private int _selectionBorderOpacity = def_SelectionBorderOpacity;
		[Category(cat_Selection)]
		[Description("Opacity of the border of selected tiles (0..255 default 255)")]
		[DefaultValue(def_SelectionBorderOpacity)]
		public int SelectionBorderOpacity
		{
			get { return _selectionBorderOpacity; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_SelectionBorderOpacity].Value =
					_selectionBorderOpacity = value.Viceroy(0,255);
				}
				else if ((_selectionBorderOpacity = value.Viceroy(0,255)) != value) // on user-changed
				{
					MainViewF.that.Options[str_SelectionBorderOpacity].Value = _selectionBorderOpacity;
				}
			}
		}

		private const string str_SelectionBorderWidth = "SelectionBorderWidth";
		private const int    def_SelectionBorderWidth = 2;

		private int _selectionBorderWidth = def_SelectionBorderWidth;
		[Category(cat_Selection)]
		[Description("Width of the border of selected tiles in pixels (1..5 default 2)")]
		[DefaultValue(def_SelectionBorderWidth)]
		public int SelectionBorderWidth
		{
			get { return _selectionBorderWidth; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_SelectionBorderWidth].Value =
					_selectionBorderWidth = value.Viceroy(1,5);
				}
				else if ((_selectionBorderWidth = value.Viceroy(1,5)) != value) // on user-changed
				{
					MainViewF.that.Options[str_SelectionBorderWidth].Value = _selectionBorderWidth;
				}
			}
		}


		internal const string str_SelectedTileToner = "SelectedTileToner";
		private  const int    def_SelectedTileToner = TONER_GRAY;

		private int _selectedTileToner = def_SelectedTileToner;
		[Category(cat_Selection)]
		[Description(@"The colortone of tiles that are selected (F10 - cycle; Shift+F10 - reverse cycle)
0 - none
1 - grayscale (default)
2 - redscale
3 - greenscale
4 - bluescale
(only if UseMono is false)")]
		[DefaultValue(def_SelectedTileToner)]
		public int SelectedTileToner
		{
			get { return _selectedTileToner; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_SelectedTileToner].Value =
					_selectedTileToner = value.Viceroy(TONER_NONE, TONER_BLUE);
				}
				else if ((_selectedTileToner = value.Viceroy(TONER_NONE, TONER_BLUE)) != value) // user-changed ->
				{
					MainViewF.that.Options[str_SelectedTileToner].Value = _selectedTileToner;
				}
			}
		}


		internal const string str_SelectedTonerBrightness = "SelectedTonerBrightness";
		private  const int    def_SelectedTonerBrightness = 5;

		private int _selectedTonerBrightness = def_SelectedTonerBrightness;
		[Category(cat_Selection)]
		[Description(@"The brightness of selected (colortoned) tiles (0..10 default 5)
(only if UseMono is false)")]
		[DefaultValue(def_SelectedTonerBrightness)]
		public int SelectedTonerBrightness
		{
			get { return _selectedTonerBrightness; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_SelectedTonerBrightness].Value =
					_selectedTonerBrightness = value.Viceroy(0, 10);
				}
				else if ((_selectedTonerBrightness = value.Viceroy(0, 10)) != value) // user-changed ->
				{
					MainViewF.that.Options[str_SelectedTonerBrightness].Value = _selectedTonerBrightness;
				}
			}
		}


		internal const string str_LayerSelectionBorder = "LayerSelectionBorder";
		private  const int    def_LayerSelectionBorder = 0;

		private int _layerSelectionBorder = def_LayerSelectionBorder;
		[Category(cat_Selection)]
		[Description(@"Where the selection border will be drawn. (F9 - Cycle)
0 - at grid level (default)
1 - at both grid level and the level above
2 - at the level above only")]
		[DefaultValue(def_LayerSelectionBorder)]
		public int LayerSelectionBorder
		{
			get { return _layerSelectionBorder; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_LayerSelectionBorder].Value =
					_layerSelectionBorder = value.Viceroy(0,2);
				}
				else if ((_layerSelectionBorder = value.Viceroy(0,2)) != value) // user-changed ->
				{
					MainViewF.that.Options[str_LayerSelectionBorder].Value = _layerSelectionBorder;
				}
			}
		}

		internal const string str_OneTileDraw = "OneTileDraw";
		private  const bool   def_OneTileDraw = false;

		private bool _oneTileDraw = def_OneTileDraw;
		[Category(cat_Selection)]
		[Description("If true a selection border will be drawn even when there"
			+ " is only 1 tile selected. Typically this should be left false"
			+ " but if particularly large sprites hide the selector then this"
			+ " can help resolve the tile - see also " + str_LayerSelectionBorder
			+ @" (Ctrl+F9 - On/Off)
(default False)")]
		[DefaultValue(def_OneTileDraw)]
		public bool OneTileDraw
		{
			get { return _oneTileDraw; }
			set { _oneTileDraw = value; }
		}



		private const string cat_Sprites_render = "Sprites - render";

		private const string str_SpriteShade = "SpriteShade";
		private const int    def_SpriteShade = 0;

		// NOTE: Options don't like floats, hence this workaround w/ 'SpriteShade' and 'SpriteShadeFloat' ->

		private int _spriteShade = def_SpriteShade;
		[Category(cat_Sprites_render)]
		[Description(@"The darkness of the tile sprites (0..99 default 0 off, 33 is unity)
(only if UseMono is false)")]
		[DefaultValue(def_SpriteShade)]
		public int SpriteShade
		{
			get { return _spriteShade; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_SpriteShade].Value =
					_spriteShade = value.Viceroy(0,99);
				}
				else if ((_spriteShade = value.Viceroy(0,99)) != value) // on user-changed
				{
					MainViewF.that.Options[str_SpriteShade].Value = _spriteShade;
				}

				if (_spriteShade != 0)
				{
					SpriteShadeFloat = (float)_spriteShade * GlobalsXC.SpriteShadeCoefficient;
					Globals.SetSpriteShade();
				}
				else
					Globals.SetSpriteShade(true);
			}
		}

		private float _spriteShadeFloat = 1F;
		internal float SpriteShadeFloat
		{
			get { return _spriteShadeFloat; }
			private set { _spriteShadeFloat = value; }
		}


		private const string str_SpriteShadeCursor = "SpriteShadeCursor";
		private const int    def_SpriteShadeCursor = 0;

		// NOTE: Options don't like floats, hence this workaround w/ 'SpriteShadeCursor' and 'SpriteShadeFloatCursor' ->

		private int _spriteShadeCursor = def_SpriteShadeCursor;
		[Category(cat_Sprites_render)]
		[Description(@"The darkness of the cursor sprites (0..99 default 0 off, 33 is unity)
(only if UseMono is false)")]
		[DefaultValue(def_SpriteShadeCursor)]
		public int SpriteShadeCursor
		{
			get { return _spriteShadeCursor; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_SpriteShadeCursor].Value =
					_spriteShadeCursor = value.Viceroy(0,99);
				}
				else if ((_spriteShadeCursor = value.Viceroy(0,99)) != value) // on user-changed
				{
					MainViewF.that.Options[str_SpriteShadeCursor].Value = _spriteShadeCursor;
				}

				if (_spriteShadeCursor != 0)
				{
					SpriteShadeFloatCursor = (float)_spriteShadeCursor * GlobalsXC.SpriteShadeCoefficient;
					CuboidSprite.SetSpriteShadeCursor();
				}
				else
					CuboidSprite.SetSpriteShadeCursor(true);
			}
		}

		private float _spriteShadeFloatCursor = 1F;
		internal float SpriteShadeFloatCursor
		{
			get { return _spriteShadeFloatCursor; }
			private set { _spriteShadeFloatCursor = value; }
		}


		private const string str_Interpolation = "Interpolation";
		private const int    def_Interpolation = 5;

		// NOTE: Options don't like enums, hence this workaround w/ 'Interpolation' and 'InterpolationE' ->

		private int _interpolation = def_Interpolation;
		[Category(cat_Sprites_render)]
		[Description(@"The technique used for resizing sprites (0..7 default 5)
0 - default
1 - low (default)
2 - high (recommended)
3 - bilinear (defaultiest)
4 - bicubic (very slow fullscreen)
5 - nearest neighbor (fastest - this is really the MapView2 default)
6 - high quality bilinear (smoothest)
7 - high quality bicubic (best in a pig's eye)
(only if UseMono is false)")]
		[DefaultValue(def_Interpolation)]
		public int Interpolation
		{
			get { return _interpolation; }
			set
			{
				if ((MainViewF._foptions as OptionsForm) == null) // on load
				{
					MainViewF.that.Options[str_Interpolation].Value =
					_interpolation = value.Viceroy(0,7);
				}
				else if ((_interpolation = value.Viceroy(0,7)) != value) // on user-changed
				{
					MainViewF.that.Options[str_Interpolation].Value = _interpolation;
				}

				InterpolationE = (InterpolationMode)_interpolation;
			}
		}

		private InterpolationMode _interpolationE = InterpolationMode.NearestNeighbor;
		internal InterpolationMode InterpolationE
		{
			get { return _interpolationE; }
			private set { _interpolationE = value; }
		}


		private const string str_UseMono = "UseMono";
		private const bool   def_UseMono = false;

		private bool _useMono = def_UseMono;
		[Category(cat_Sprites_render)]
		[Description("If true use sprite-drawing algorithms that are designed for Mono."
			+ " This fixes an issue on non-Windows systems where non-transparent"
			+ " black boxes appear around sprites but it bypasses Interpolation"
			+ " and SpriteShade routines. Also selected tiles will not be toned"
			+ " (default False)")]
		[DefaultValue(def_UseMono)]
		public bool UseMono
		{
			get { return _useMono; }
			set { _useMono = value; }
		}



		private const string cat_Sprites = "Sprites";

		internal const string str_AnimateSprites = "AnimateSprites";
		private  const bool   def_AnimateSprites = false;

		private bool _animateSprites = def_AnimateSprites;
		[Category(cat_Sprites)]
		[Description(@"If true the sprites will animate (F2 - On/Off)
(default False)")]
		[DefaultValue(def_AnimateSprites)]
		public bool AnimateSprites
		{
			get { return _animateSprites; }
			set { _animateSprites = value; }
		}


		internal const string str_OpenDoors = "OpenDoors";
		private  const bool   def_OpenDoors = false;

		private bool _openDoors = def_OpenDoors;
		[Category(cat_Sprites)]
		[Description("If true the doors will animate if Animate is also on - if"
			+ " Animate is false the doors will show their alternate tile. This"
			+ @" setting may need to be re-toggled if Animate changes (F3 - On/Off)
(default False)")]
		[DefaultValue(def_OpenDoors)]
		public bool OpenDoors
		{
			get { return _openDoors; }
			set { _openDoors = value; }
		}


		internal const string str_PreferTftdTargeter = "PreferTftdTargeter";
		private  const bool   def_PreferTftdTargeter = false;

		private bool _preferTftdTargeter = def_PreferTftdTargeter;
		[Category(cat_Sprites)]
		[Description("If true MainView will use the TftD targeter sprites for"
			+ " its cursor if both Ufo and TftD are configured in resources and"
			+ @" both spritesets are found.
(default False)")]
		[DefaultValue(def_PreferTftdTargeter)]
		public bool PreferTftdTargeter
		{
			get { return _preferTftdTargeter; }
			set { _preferTftdTargeter = value; }
		}



		private const string cat_Screenshot = "Screenshot";

		private const string str_BackgroundColor = "BackgroundColor";
		private static Color def_BackgroundColor = Color.Transparent;

		private Color _backgroundColor = def_BackgroundColor;
		[Category(cat_Screenshot)]
		[Description("The color for the background of a screenshot (default"
			+ " Transparent)")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set { _backgroundColor = value; }
		}


		private const string str_CropBackground = "CropBackground";
		private const bool   def_CropBackground = false;

		private bool _cropBackground = def_CropBackground;
		[Category(cat_Screenshot)]
		[Description("If true a screenshot will be cropped to its foreground"
			+ " pixels (default False)")]
		[DefaultValue(def_CropBackground)]
		public bool CropBackground
		{
			get { return _cropBackground; }
			set { _cropBackground = value; }
		}


		private const string str_Png_notGif = "Png_notGif";
		private const bool   def_Png_notGif = true;

		private bool _png_notGif = def_Png_notGif;
		[Category(cat_Screenshot)]
		[Description("If true screenshots will be saved to PNG format, if false"
			+ " they will be saved to GIF format (default True)")]
		[DefaultValue(def_Png_notGif)]
		public bool Png_notGif
		{
			get { return _png_notGif; }
			set { _png_notGif = value; }
		}



		private const string cat_Global = "Global";


		private const string str_BringAllToFront = "BringAllToFront";
		private const bool   def_BringAllToFront = false;

		private bool _bringAllToFront = def_BringAllToFront;
		[Category(cat_Global)]
		[Description("If true any open subsidiary viewers will be brought to the"
			+ " top of the desktop whenever MainView takes focus - this implements"
			+ " a workaround that might help circumvent an issue in post Windows 7"
			+ " OS, in which focus refuses to switch to MainView unless the"
			+ " secondary viewers are closed (default False)")]
		[DefaultValue(def_BringAllToFront)]
		public bool BringAllToFront
		{
			get { return _bringAllToFront; }
			set { _bringAllToFront = value; }
		}

		private const string str_Base1_xy = "Base1_xy";
		private const bool   def_Base1_xy = false;

		private bool _base1_xy = def_Base1_xy;
		[Category(cat_Global)]
		[Description("If true the printed position of rows and cols starts at 1"
			+ " instead of 0 (default False)")]
		[DefaultValue(def_Base1_xy)]
		public bool Base1_xy
		{
			get { return _base1_xy; }
			set { _base1_xy = value; }
		}


		private const string str_Base1_z = "Base1_z";
		private const bool   def_Base1_z = false;

		private bool _base1_z = def_Base1_z;
		[Category(cat_Global)]
		[Description("If true the printed position of the level starts at 1"
			+ " instead of 0 (default False)")]
		[DefaultValue(def_Base1_z)]
		public bool Base1_z
		{
			get { return _base1_z; }
			set { _base1_z = value; }
		}


		private const string str_IgnoreRecordsExceeded = "IgnoreRecordsExceeded";
		private const bool   def_IgnoreRecordsExceeded = false;

		private bool _ignoreRecordsExceeded = def_IgnoreRecordsExceeded;
		[Category(cat_Global)]
		[Description("If true do not show warnings when the total MCD (terrain)"
			+ " record count exceeds 254 (default False)")]
		[DefaultValue(def_IgnoreRecordsExceeded)]
		public bool IgnoreRecordsExceeded
		{
			get { return _ignoreRecordsExceeded; }
			set { _ignoreRecordsExceeded = value; }
		}


		private const string str_InvertMousewheel = "InvertMousewheel";
		private const bool   def_InvertMousewheel = false;

		private bool _invertMousewheel = def_InvertMousewheel;
		[Category(cat_Global)]
		[Description("If true the mousewheel changes levels in the opposite direction"
			+ " than it should (default False)")]
		[DefaultValue(def_InvertMousewheel)]
		public bool InvertMousewheel
		{
			get { return _invertMousewheel; }
			set { _invertMousewheel = value; }
		}



		private const string cat_nonBrowsable = "nonBrowsable";

		private const string str_DescriptionHeight = "DescriptionHeight";
		private const int    def_DescriptionHeight = 142; // header(22) + 10 line(12)

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
				MainViewF.that.Options[str_DescriptionHeight].Value =
				_descriptionHeight = value;
			}
		}
		#endregion Properties (optionable)


		#region Methods
		/// <summary>
		/// Instantiates pens/brushes used by MainView's draw routines with
		/// default values. Adds default keyval pairs to MainView's optionables
		/// and an option-changer is assigned to each. The default values were
		/// assigned to MainView's optionable properties when those properties
		/// were instantiated above.
		/// </summary>
		/// <param name="options"><c><see cref="MainViewF.Options">MainViewF.Options</see></c></param>
		internal void LoadDefaults(Options options)
		{
			//DSShared.Logfile.Log("MainViewOptionables.LoadDefaults()");

			Color color;

			color = Color.FromArgb(def_GridLayerOpacity, def_GridLayerColor);
			_overlay.BrushLayer = new SolidBrush(color);

			_overlay.PenGrid   = new Pen(def_GridLineColor,   def_GridLineWidth);
			_overlay.PenGrid10 = new Pen(def_GridLine10Color, def_GridLine10Width);

			color = Color.FromArgb(def_SelectionBorderOpacity, def_SelectionBorderColor);
			_overlay.PenSelect = new Pen(color, def_SelectionBorderWidth);


			OptionChangedEvent changer0 = OnOptionChanged;
			OptionChangedEvent changer1 = OnFlagChanged;
			OptionChangedEvent changer2 = OnSpriteStateChanged;
			OptionChangedEvent changer3 = OnSpriteShadeChanged;
			OptionChangedEvent changer4 = OnBaseCounttypeChanged;
			OptionChangedEvent changer5 = OnDescriptionHeightChanged;

			options.CreateOptionDefault(str_StartTileView,           def_StartTileView,           changer1);
			options.CreateOptionDefault(str_StartTopView,            def_StartTopView,            changer1);
			options.CreateOptionDefault(str_StartRouteView,          def_StartRouteView,          changer1);
			options.CreateOptionDefault(str_StartTopRouteView,       def_StartTopRouteView,       changer1);
			options.CreateOptionDefault(str_StartTopRoutePage,       def_StartTopRoutePage,       changer1);

			options.CreateOptionDefault(str_GridVisible,             def_GridVisible,             changer0);
			options.CreateOptionDefault(str_GridLayerColor,          def_GridLayerColor,          changer0);
			options.CreateOptionDefault(str_GridLayerOpacity,        def_GridLayerOpacity,        changer0);
			options.CreateOptionDefault(str_GridLineColor,           def_GridLineColor,           changer0);
			options.CreateOptionDefault(str_GridLineWidth,           def_GridLineWidth,           changer0);
			options.CreateOptionDefault(str_GridLine10Color,         def_GridLine10Color,         changer0);
			options.CreateOptionDefault(str_GridLine10Width,         def_GridLine10Width,         changer0);

			options.CreateOptionDefault(str_SelectionBorderColor,    def_SelectionBorderColor,    changer0);
			options.CreateOptionDefault(str_SelectionBorderOpacity,  def_SelectionBorderOpacity,  changer0);
			options.CreateOptionDefault(str_SelectionBorderWidth,    def_SelectionBorderWidth,    changer0);
			options.CreateOptionDefault(str_SelectedTileToner,       def_SelectedTileToner,       changer0);
			options.CreateOptionDefault(str_SelectedTonerBrightness, def_SelectedTonerBrightness, changer0);
			options.CreateOptionDefault(str_LayerSelectionBorder,    def_LayerSelectionBorder,    changer0);
			options.CreateOptionDefault(str_OneTileDraw,             def_OneTileDraw,             changer0);

			options.CreateOptionDefault(str_SpriteShade,             def_SpriteShade,             changer3);
			options.CreateOptionDefault(str_SpriteShadeCursor,       def_SpriteShadeCursor,       changer0);
			options.CreateOptionDefault(str_Interpolation,           def_Interpolation,           changer0);

			options.CreateOptionDefault(str_AnimateSprites,          def_AnimateSprites,          changer2);
			options.CreateOptionDefault(str_OpenDoors,               def_OpenDoors,               changer2);
			options.CreateOptionDefault(str_BringAllToFront,         def_BringAllToFront,         changer1);
			options.CreateOptionDefault(str_UseMono,                 def_UseMono,                 changer2);

			options.CreateOptionDefault(str_BackgroundColor,         def_BackgroundColor,         changer1);
			options.CreateOptionDefault(str_CropBackground,          def_CropBackground,          changer1);
			options.CreateOptionDefault(str_Png_notGif,              def_Png_notGif,              changer1);

			options.CreateOptionDefault(str_Base1_xy,                def_Base1_xy,                changer4);
			options.CreateOptionDefault(str_Base1_z,                 def_Base1_z,                 changer4);
			options.CreateOptionDefault(str_IgnoreRecordsExceeded,   def_IgnoreRecordsExceeded,   changer1);
			options.CreateOptionDefault(str_InvertMousewheel,        def_InvertMousewheel,        changer1);

			options.CreateOptionDefault(str_PreferTftdTargeter,      def_PreferTftdTargeter,      changer0);

			options.CreateOptionDefault(str_DescriptionHeight,       def_DescriptionHeight,       changer5);
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Sets the value of an optionable property and invalidates
		/// <c><see cref="MainViewOverlay"/></c>.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		internal void OnOptionChanged(string key, object val)
		{
			switch (key)
			{
				case str_GridVisible: // F4 toggle
					GridVisible = (bool)val;
					break;

				case str_GridLayerColor:
				{
					Color color = Color.FromArgb(GridLayerOpacity, (GridLayerColor = (Color)val));
					_overlay.BrushLayer.Color = color;
					break;
				}

				case str_GridLayerOpacity:
				{
					Color color = Color.FromArgb((GridLayerOpacity = (int)val), GridLayerColor);
					_overlay.BrushLayer.Color = color;
					break;
				}

				case str_GridLineColor:
					_overlay.PenGrid.Color = (GridLineColor = (Color)val);
					break;

				case str_GridLineWidth:
					_overlay.PenGrid.Width = (GridLineWidth = (int)val);
					break;

				case str_GridLine10Color:
					_overlay.PenGrid10.Color = (GridLine10Color = (Color)val);
					break;

				case str_GridLine10Width:
					_overlay.PenGrid10.Width = (GridLine10Width = (int)val);
					break;

				case str_SelectionBorderColor:
				{
					Color color = Color.FromArgb(SelectionBorderOpacity, (SelectionBorderColor = (Color)val));
					_overlay.PenSelect.Color = color;
					break;
				}

				case str_SelectionBorderOpacity:
				{
					Color color = Color.FromArgb((SelectionBorderOpacity = (int)val), SelectionBorderColor);
					_overlay.PenSelect.Color = color;
					break;
				}

				case str_SelectionBorderWidth:
					_overlay.PenSelect.Width = (SelectionBorderWidth = (int)val);
					break;

				case str_SelectedTileToner:
					SelectedTileToner = (int)val;
					MainViewF.that.SelectToner();
					break;

				case str_SelectedTonerBrightness:
					SelectedTonerBrightness = (int)val;

					Palette.UfoBattle .CreateTonescaledPalettes(SelectedTonerBrightness);
					Palette.TftdBattle.CreateTonescaledPalettes(SelectedTonerBrightness);

					MainViewF.that.SetTonedPalette();
					break;

				case str_LayerSelectionBorder:
					LayerSelectionBorder = (int)val;
					break;

				case str_OneTileDraw:
					OneTileDraw = (bool)val;
					break;

				case str_Interpolation:
					Interpolation = (int)val;
					break;


				case str_PreferTftdTargeter: // NOTE: The cuboids need to invalidate but not the targeter.
					PreferTftdTargeter = (bool)val;
					CuboidSprite.SetCursor();
					break;


				case str_SpriteShadeCursor:
					SpriteShadeCursor = (int)val;
					break;
			}

			_overlay.Invalidate();
		}

		/// <summary>
		/// Sets the value of an optionable property but doesn't invalidate
		/// anything.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		internal void OnFlagChanged(string key, object val)
		{
			switch (key)
			{
				case str_StartTileView:
					ViewersMenuManager.SetChecked(
											ViewersMenuManager.MI_TILE,
											(StartTileView = (bool)val));
					break;

				case str_StartTopView:
					ViewersMenuManager.SetChecked(
											ViewersMenuManager.MI_TOP,
											(StartTopView = (bool)val));
					break;

				case str_StartRouteView:
					ViewersMenuManager.SetChecked(
											ViewersMenuManager.MI_ROUTE,
											(StartRouteView = (bool)val));
					break;

				case str_StartTopRouteView:
					ViewersMenuManager.SetChecked(
											ViewersMenuManager.MI_TOPROUTE,
											(StartTopRouteView = (bool)val));
					break;

				case str_StartTopRoutePage:
					StartTopRoutePage = (int)val;

					if (MainViewF._foptions as OptionsForm == null // on load
						|| StartTopRoutePage > 1)
					{
						ObserverManager.TopRouteView.SelectTabpage(StartTopRoutePage % 2); // 0,2 TopView; 1,3 RouteView
					}
					break;


				case str_BringAllToFront:
					BringAllToFront = (bool)val;
					break;


				case str_BackgroundColor:
					BackgroundColor = (Color)val;
					break;

				case str_CropBackground:
					CropBackground = (bool)val;
					break;

				case str_Png_notGif:
					Png_notGif = (bool)val;
					break;


				case str_IgnoreRecordsExceeded:
					IgnoreRecordsExceeded = (bool)val;
					break;

				case str_InvertMousewheel:
					InvertMousewheel = (bool)val;
					break;
			}
		}

		/// <summary>
		/// Sets the value of an optionable property and invalidates
		/// <c><see cref="MainViewOverlay"/></c> as well as the current
		/// <c><see cref="TilePanel"/></c> and the
		/// <c><see cref="QuadrantControl">QuadrantControls</see></c> in
		/// <c>TopView</c> and <c>TopRouteView</c>.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		internal void OnSpriteStateChanged(string key, object val)
		{
			switch (key)
			{
				case str_AnimateSprites:
					MainViewUnderlay.Animate(AnimateSprites = (bool)val); // F2 toggle

					if (!AnimateSprites) // show the doorsprites closed in TileView and QuadrantControl.
					{
						if (OpenDoors) // toggle off doors if general animations stop.
						{
							OpenDoors = false;
							MainViewF.that.SetDoorSpritesFullPhase(false);
						}
					}
					else if (OpenDoors) // doors need to animate if they were already toggled on.
						MainViewF.that.SetDoorSpritesFullPhase(true);

					break;

				case str_OpenDoors:
					OpenDoors = (bool)val; // F3 toggle

					if (AnimateSprites)
					{
						MainViewF.that.SetDoorSpritesFullPhase(OpenDoors);
					}
					else if (OpenDoors) // switch to the doors' alt-tile (whether ufo-door or hinge-door)
					{
						MainViewF.that.SetDoorSpritesAlternate();
					}
					else // switch doors to Sprite1 (closed)
						MainViewF.that.SetDoorSpritesFullPhase(false);

					break;

				case str_UseMono:
					UseMono = (bool)val;
					break;
			}

			Invalidate();
		}

		/// <summary>
		/// Sets the value of an optionable property and invalidates
		/// <c><see cref="MainViewOverlay"/></c> as well as the current
		/// <c><see cref="TilePanel"/></c> and the
		/// <c><see cref="QuadrantControl">QuadrantControls</see></c> in
		/// <c>TopView</c> and <c>TopRouteView(Top)</c>. Also invalidates
		/// <c><see cref="ScanGViewer"/></c>.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnSpriteShadeChanged(string key, object val)
		{
			SpriteShade = (int)val;

			Invalidate();

			if (MainViewF.ScanG != null)
				MainViewF.ScanG.InvalidatePanel();
		}

		/// <summary>
		/// Changes the format of printed location strings between base-0 and
		/// base-1 counttypes.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnBaseCounttypeChanged(string key, object val)
		{
			switch (key)
			{
				case str_Base1_xy: Base1_xy = (bool)val; break;
				case str_Base1_z:  Base1_z  = (bool)val; break;
			}

			// refesh position prints
			MainViewF.that.sb_PrintPosition();

			ObserverManager.RouteView   .Control     .PrintSelectedInfo();
			ObserverManager.TopRouteView.ControlRoute.PrintSelectedInfo();

			ObserverManager.InvalidateQuadrantControls();	// NOTE: Trying to print only the string w/ QuadrantDrawService
															// borks out w/ an obscure "Parameter is invalid" exception.

			// close the PartslotTest dialog (its displayed data has changed)
			if (TopView._fpartslots != null && !TopView._fpartslots.IsDisposed)
			{
				TopView._fpartslots.Close();
				TopView._fpartslots = null;
			}

			if (MainViewF.ScanG != null)
				MainViewF.ScanG.SetTitle();

			// NOTE: Routenode Checks don't need to be handled here because
			// their dialogs are Modal.
		}


		/// <summary>
		/// Stores the property panel's <c>Description</c> area's height when
		/// the user changes it.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private void OnDescriptionHeightChanged(string key, object val)
		{
			DescriptionHeight = (int)val;
		}

		/// <summary>
		/// Invalidates <c><see cref="MainViewOverlay"/></c> along with the
		/// current <c><see cref="TilePanel"/></c> in
		/// <c><see cref="TileView"/></c> and the
		/// <c><see cref="QuadrantControl">QuadrantControls</see></c> in
		/// <c>TopView</c> and <c>TopRouteView(Top)</c>.
		/// </summary>
		private void Invalidate()
		{
			_overlay.Invalidate();

			ObserverManager.TileView.Control.GetSelectedPanel().Invalidate();
			ObserverManager.InvalidateQuadrantControls();
		}
		#endregion Events
	}
}
