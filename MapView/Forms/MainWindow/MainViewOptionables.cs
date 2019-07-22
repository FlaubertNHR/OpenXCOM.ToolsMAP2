//#define LOCKBITS	// toggle this to change OnPaint routine in standard build.
					// Must be set in MainViewOverlay as well. Purely experimental.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using MapView.Forms.MainWindow;


namespace MapView
{
	/// <summary>
	/// Properties for MainView that appear in MainView's Options.
	/// </summary>
	internal sealed class MainViewOptionables
	{
		#region Fields
		private readonly MainViewOverlay MainViewOverlay;
		#endregion Fields


		#region cTor
		internal MainViewOptionables(MainViewOverlay panel)
		{
			MainViewOverlay = panel;
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

		// NOTE: Observers are added to MainView's Options by
		// MainMenusManager.PopulateMenus(). They are used by
		// MainMenusManager.StartSecondaryStage().
		private const string cat_Observers = "Observers";

		private const string str_StartTileView = "StartTileView";
		private const bool   def_StartTileView = true;

		private bool _starttileview = def_StartTileView;
		[Category(cat_Observers)]
		[Description("Open on load - TileView")]
		[DefaultValue(def_StartTileView)]
		public bool StartTileView
		{
			get { return _starttileview; }
			set { _starttileview = value; }
		}

		private const string str_StartTopView = "StartTopView";
		private const bool   def_StartTopView = true;

		private bool _starttopview = def_StartTopView;
		[Category(cat_Observers)]
		[Description("Open on load - TopView")]
		[DefaultValue(def_StartTopView)]
		public bool StartTopView
		{
			get { return _starttopview; }
			set { _starttopview = value; }
		}

		private const string str_StartRouteView = "StartRouteView";
		private const bool   def_StartRouteView = true;

		private bool _startrouteview = def_StartRouteView;
		[Category(cat_Observers)]
		[Description("Open on load - RouteView")]
		[DefaultValue(def_StartRouteView)]
		public bool StartRouteView
		{
			get { return _startrouteview; }
			set { _startrouteview = value; }
		}

		private const string str_StartTopRouteView = "StartTopRouteView";
		private const bool   def_StartTopRouteView = false;

		private bool _starttoprouteview = def_StartTopRouteView;
		[Category(cat_Observers)]
		[Description("Open on load - TopRouteView")]
		[DefaultValue(def_StartTopRouteView)]
		public bool StartTopRouteView
		{
			get { return _starttoprouteview; }
			set { _starttoprouteview = value; }
		}



		private const string cat_Grid = "Grid";

		internal const string str_GridVisible = "GridVisible";
		private  const bool   def_GridVisible = true;

		private bool _gridVisible = def_GridVisible;
		[Category(cat_Grid)]
		[Description("If true a grid will display at the current level of editing (F4 - On/Off)")]
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
		[Description("Color of the grid")]
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
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_GridLayerOpacity].Value =
					_gridLayerOpacity = value.Clamp(0,255);
				}
				else if ((_gridLayerOpacity = value.Clamp(0,255)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_GridLayerOpacity].Value = _gridLayerOpacity;
					foptions.propertyGrid.SetSelectedValue(_gridLayerOpacity);
				}
			}
		}


		private const string str_GridLineColor = "GridLineColor";
		private static Color def_GridLineColor = Color.Black;

		private Color _gridLineColor = def_GridLineColor;
		[Category(cat_Grid)]
		[Description("Color of the lines that draw the grid")]
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
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_GridLineWidth].Value =
					_gridLineWidth = value.Clamp(1,6);
				}
				else if ((_gridLineWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_GridLineWidth].Value = _gridLineWidth;
					foptions.propertyGrid.SetSelectedValue(_gridLineWidth);
				}
			}
		}


		private const string str_GridLine10Color = "GridLine10Color";
		private static Color def_GridLine10Color = Color.Black;

		private Color _gridLine10Color = def_GridLine10Color;
		[Category(cat_Grid)]
		[Description("Color of every tenth line on the grid")]
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
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_GridLine10Width].Value =
					_gridLine10Width = value.Clamp(1,6);
				}
				else if ((_gridLine10Width = value.Clamp(1,6)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_GridLine10Width].Value = _gridLine10Width;
					foptions.propertyGrid.SetSelectedValue(_gridLine10Width);
				}
			}
		}



		private const string cat_Selects = "Selects";

		private const string str_SelectionBorderColor = "SelectionBorderColor";
		private static Color def_SelectionBorderColor = Color.Tomato;

		private Color _selectionBorderColor = def_SelectionBorderColor;
		[Category(cat_Selects)]
		[Description("Color of the border of selected tiles")]
		[DefaultValue(typeof(Color), "Tomato")]
		public Color SelectionBorderColor
		{
			get { return _selectionBorderColor; }
			set { _selectionBorderColor = value; }
		}

		private const string str_SelectionBorderOpacity = "SelectionBorderOpacity";
		private const int    def_SelectionBorderOpacity = 255;

		private int _selectionBorderOpacity = def_SelectionBorderOpacity;
		[Category(cat_Selects)]
		[Description("Opacity of the border of selected tiles (0..255 default 255)")]
		[DefaultValue(def_SelectionBorderOpacity)]
		public int SelectionBorderOpacity
		{
			get { return _selectionBorderOpacity; }
			set
			{
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_SelectionBorderOpacity].Value =
					_selectionBorderOpacity = value.Clamp(0,255);
				}
				else if ((_selectionBorderOpacity = value.Clamp(0,255)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_SelectionBorderOpacity].Value = _selectionBorderOpacity;
					foptions.propertyGrid.SetSelectedValue(_selectionBorderOpacity);
				}
			}
		}

		private const string str_SelectionBorderWidth = "SelectionBorderWidth";
		private const int    def_SelectionBorderWidth = 2;

		private int _selectionBorderWidth = def_SelectionBorderWidth;
		[Category(cat_Selects)]
		[Description("Width of the border of selected tiles in pixels (1..6 default 2)")]
		[DefaultValue(def_SelectionBorderWidth)]
		public int SelectionBorderWidth
		{
			get { return _selectionBorderWidth; }
			set
			{
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_SelectionBorderWidth].Value =
					_selectionBorderWidth = value.Clamp(1,6);
				}
				else if ((_selectionBorderWidth = value.Clamp(1,6)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_SelectionBorderWidth].Value = _selectionBorderWidth;
					foptions.propertyGrid.SetSelectedValue(_selectionBorderWidth);
				}
			}
		}



		private const string cat_Sprites = "Sprites";

		private const string str_GraySelection = "GraySelection";
		private const bool   def_GraySelection = true;

		private bool _graySelection = def_GraySelection;
		[Category(cat_Sprites)]
		[Description(@"If true the selection area will be drawn in grayscale
(only if UseMonoDraw is false)")]
		[DefaultValue(def_GraySelection)]
		public bool GraySelection
		{
			get { return _graySelection; }
			set { _graySelection = value; }
		}


#if !LOCKBITS
		private bool _spriteShadeEnabled = true;
		internal bool SpriteShadeEnabled
		{
			get { return _spriteShadeEnabled; }
			private set { _spriteShadeEnabled = value; }
		}
#endif
		private const string str_SpriteShade = "SpriteShade";
		private const int    def_SpriteShade = 0;

		// NOTE: Options don't like floats, hence this workaround w/ 'SpriteShade' and 'SpriteShadeFloat' ->

		private int _spriteShade = def_SpriteShade;
		[Category(cat_Sprites)]
		[Description("The darkness of the tile sprites (0..100 default 0 off, unity is 33)"
			+ @" Values outside the range turn sprite shading off
(only if UseMonoDraw is false)")]
		[DefaultValue(def_SpriteShade)]
		public int SpriteShade
		{
			get { return _spriteShade; }
			set
			{
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_SpriteShade].Value =
					_spriteShade = value.Clamp(0,100);
				}
				else if ((_spriteShade = value.Clamp(0,100)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_SpriteShade].Value = _spriteShade;
					foptions.propertyGrid.SetSelectedValue(_spriteShade);
				}

				if (_spriteShade != 0)
				{
#if !LOCKBITS
					SpriteShadeEnabled = true;
#endif
					SpriteShadeFloat = (float)_spriteShade * 0.03F;
				}
#if !LOCKBITS
				else
					SpriteShadeEnabled = false;
#endif
			}
		}

		private float _spriteShadeFloat = 1F;
		internal float SpriteShadeFloat
		{
			get { return _spriteShadeFloat; }
			private set { _spriteShadeFloat = value; }
		}


		private const string str_Interpolation = "Interpolation";
		private const int    def_Interpolation = 0;

		// NOTE: Options don't like enums, hence this workaround w/ 'Interpolation' and 'InterpolationE' ->

		private int _interpolation = def_Interpolation;
		[Category(cat_Sprites)]
		[Description(@"The technique used for resizing sprites (0..7 default 0)
0 - default
1 - low (default)
2 - high (recommended)
3 - bilinear (defaultiest)
4 - bicubic (very slow fullscreen)
5 - nearest neighbor (fastest)
6 - high quality bilinear (smoothest)
7 - high quality bicubic (best in a pig's eye)
(only if UseMonoDraw is false)")]
		[DefaultValue(def_Interpolation)]
		public int Interpolation
		{
			get { return _interpolation; }
			set
			{
				var foptions = XCMainWindow._foptions as OptionsForm;
				if (foptions == null) // on load
				{
					XCMainWindow.that.Options[str_Interpolation].Value =
					_interpolation = value.Clamp(0,7);
				}
				else if ((_interpolation = value.Clamp(0,7)) != value) // on user-changed
				{
					XCMainWindow.that.Options[str_Interpolation].Value = _interpolation;
					foptions.propertyGrid.SetSelectedValue(_interpolation);
				}

				InterpolationE = (InterpolationMode)_interpolation;
			}
		}

		private InterpolationMode _interpolationE = InterpolationMode.Default;
		internal InterpolationMode InterpolationE
		{
			get { return _interpolationE; }
			private set { _interpolationE = value; }
		}



		private const string cat_General = "General";

		internal const string str_AnimateSprites = "AnimateSprites";
		private  const bool   def_AnimateSprites = false;

		private bool _animatesprites = def_AnimateSprites;
		[Category(cat_General)]
		[Description("If true the sprites will animate (F2 - On/Off)")]
		[DefaultValue(def_AnimateSprites)]
		public bool AnimateSprites
		{
			get { return _animatesprites; }
			set { _animatesprites = value; }
		}


		internal const string str_OpenDoors = "OpenDoors";
		private  const bool   def_OpenDoors = false;

		private bool _opendoors = def_OpenDoors;
		[Category(cat_General)]
		[Description("If true the doors will animate if Animate is also on - if"
			+ " Animate is false the doors will show their alternate tile. This"
			+ " setting may need to be re-toggled if Animate changes (F3 - On/Off)")]
		[DefaultValue(def_OpenDoors)]
		public bool OpenDoors
		{
			get { return _opendoors; }
			set { _opendoors = value; }
		}


		private const string str_BringAllToFront = "BringAllToFront";
		private const bool   def_BringAllToFront = false;

		private bool _bringalltofront = def_BringAllToFront;
		[Category(cat_General)]
		[Description("If true any open subsidiary viewers will be brought to the"
			+ " top of the desktop whenever MainView takes focus - this implements"
			+ " a workaround that might help circumvent an issue in post Windows 7"
			+ " OS, in which focus refuses to switch to MainView unless the"
			+ " secondary viewers are closed")]
		[DefaultValue(def_BringAllToFront)]
		public bool BringAllToFront
		{
			get { return _bringalltofront; }
			set { _bringalltofront = value; }
		}


		private const string str_UseMonoDraw = "UseMonoDraw";
		private const bool   def_UseMonoDraw = false;

		private bool _usemonodraw = def_UseMonoDraw;
		[Category(cat_General)]
		[Description("If true use sprite-drawing algorithms that are designed for Mono."
			+ " This fixes an issue on non-Windows systems where non-transparent"
			+ " black boxes appear around sprites but it bypasses Interpolation"
			+ " and SpriteShade routines. Selected tiles will not be grayed")]
		[DefaultValue(def_UseMonoDraw)]
		public bool UseMonoDraw
		{
			get { return _usemonodraw; }
			set { _usemonodraw = value; }
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
		/// <param name="options">MainView's options</param>
		internal void LoadDefaults(Options options)
		{
			Color color = Color.FromArgb(def_GridLayerOpacity, def_GridLayerColor);
			MainViewOverlay.BrushLayer = new SolidBrush(color);

			MainViewOverlay.PenGrid   = new Pen(def_GridLineColor,   def_GridLineWidth);
			MainViewOverlay.PenGrid10 = new Pen(def_GridLine10Color, def_GridLine10Width);

			color = Color.FromArgb(def_SelectionBorderOpacity, def_SelectionBorderColor);
			MainViewOverlay.PenSelect = new Pen(color, def_SelectionBorderWidth);


			OptionChangedEvent changer0 = OnOptionChanged;
			OptionChangedEvent changer1 = OnFlagChanged;
			OptionChangedEvent changer2 = OnSpriteStateChanged;
			OptionChangedEvent changer3 = OnSpriteShadeChanged;

			options.AddOptionDefault(str_StartTileView,          def_StartTileView,          changer1);
			options.AddOptionDefault(str_StartTopView,           def_StartTopView,           changer1);
			options.AddOptionDefault(str_StartRouteView,         def_StartRouteView,         changer1);
			options.AddOptionDefault(str_StartTopRouteView,      def_StartTopRouteView,      changer1);

			options.AddOptionDefault(str_GridVisible,            def_GridVisible,            changer0);
			options.AddOptionDefault(str_GridLayerColor,         def_GridLayerColor,         changer0);
			options.AddOptionDefault(str_GridLayerOpacity,       def_GridLayerOpacity,       changer0);
			options.AddOptionDefault(str_GridLineColor,          def_GridLineColor,          changer0);
			options.AddOptionDefault(str_GridLineWidth,          def_GridLineWidth,          changer0);
			options.AddOptionDefault(str_GridLine10Color,        def_GridLine10Color,        changer0);
			options.AddOptionDefault(str_GridLine10Width,        def_GridLine10Width,        changer0);

			options.AddOptionDefault(str_SelectionBorderColor,   def_SelectionBorderColor,   changer0);
			options.AddOptionDefault(str_SelectionBorderOpacity, def_SelectionBorderOpacity, changer0);
			options.AddOptionDefault(str_SelectionBorderWidth,   def_SelectionBorderWidth,   changer0);

			options.AddOptionDefault(str_GraySelection,          def_GraySelection,          changer0);
			options.AddOptionDefault(str_SpriteShade,            def_SpriteShade,            changer3);
			options.AddOptionDefault(str_Interpolation,          def_Interpolation,          changer0);

			options.AddOptionDefault(str_AnimateSprites,         def_AnimateSprites,         changer2);
			options.AddOptionDefault(str_OpenDoors,              def_OpenDoors,              changer2);
			options.AddOptionDefault(str_BringAllToFront,        def_BringAllToFront,        changer1);
			options.AddOptionDefault(str_UseMonoDraw,            def_UseMonoDraw,            changer0);
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Sets the value of an optionable property and invalidates the
		/// MainViewOverlay panel.
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
					MainViewOverlay.BrushLayer.Color = color;
					break;
				}

				case str_GridLayerOpacity:
				{
					Color color = Color.FromArgb((GridLayerOpacity = (int)val), GridLayerColor);
					MainViewOverlay.BrushLayer.Color = color;
					break;
				}

				case str_GridLineColor:
					MainViewOverlay.PenGrid.Color = (GridLineColor = (Color)val);
					break;

				case str_GridLineWidth:
					MainViewOverlay.PenGrid.Width = (GridLineWidth = (int)val);
					break;

				case str_GridLine10Color:
					MainViewOverlay.PenGrid10.Color = (GridLine10Color = (Color)val);
					break;

				case str_GridLine10Width:
					MainViewOverlay.PenGrid10.Width = (GridLine10Width = (int)val);
					break;

				case str_SelectionBorderColor:
				{
					Color color = Color.FromArgb(SelectionBorderOpacity, (SelectionBorderColor = (Color)val));
					MainViewOverlay.PenSelect.Color = color;
					break;
				}

				case str_SelectionBorderOpacity:
				{
					Color color = Color.FromArgb((SelectionBorderOpacity = (int)val), SelectionBorderColor);
					MainViewOverlay.PenSelect.Color = color;
					break;
				}

				case str_SelectionBorderWidth:
					MainViewOverlay.PenSelect.Width = (SelectionBorderWidth = (int)val);
					break;

				case str_GraySelection:
					GraySelection = (bool)val;
					break;

				case str_Interpolation:
					Interpolation = (int)val;
					break;

				case str_UseMonoDraw:
					UseMonoDraw = (bool)val;
					break;
			}

			MainViewOverlay.Invalidate();
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
				case str_StartTileView:     StartTileView     = (bool)val; break;
				case str_StartTopView:      StartTopView      = (bool)val; break;
				case str_StartRouteView:    StartRouteView    = (bool)val; break;
				case str_StartTopRouteView: StartTopRouteView = (bool)val; break;

				case str_BringAllToFront:   BringAllToFront   = (bool)val; break;
			}
		}

		/// <summary>
		/// Sets the value of an optionable property and invalidates the
		/// MainViewOverlay panel as well as the current TileView panel and
		/// the quadrant-panels in TopView and TopRouteView.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		internal void OnSpriteStateChanged(string key, object val)
		{
			switch (key)
			{
				case str_AnimateSprites: MainViewUnderlay.Animate(AnimateSprites = (bool)val); // F2 toggle
					if (!AnimateSprites) // show the doorsprites closed in TileView and QuadrantPanel.
					{
						if (OpenDoors) // toggle off doors if general animations stop.
						{
							OpenDoors = false;
							XCMainWindow.that.SetDoorSpritesFullPhase(false);
						}
					}
					else if (OpenDoors) // doors need to animate if they were already toggled on.
						XCMainWindow.that.SetDoorSpritesFullPhase(true);

					break;

				case str_OpenDoors: OpenDoors = (bool)val; // F3 toggle
					if (AnimateSprites)
					{
						XCMainWindow.that.SetDoorSpritesFullPhase(OpenDoors);
					}
					else if (OpenDoors) // switch to the doors' alt-tile (whether ufo-door or hinge-door)
					{
						XCMainWindow.that.SetDoorSpritesAlternate();
					}
					else // switch doors to Sprite1 (closed)
						XCMainWindow.that.SetDoorSpritesFullPhase(false);

					break;
			}

			MainViewOverlay.Invalidate();
			InvalidateSecondaryPanels();
		}

		/// <summary>
		/// Sets the value of an optionable property and invalidates the
		/// MainViewOverlay panel as well as the current TileView panel and
		/// the quadrant-panels in TopView and TopRouteView. Also invalidates
		/// the ScanG viewer's panel.
		/// </summary>
		/// <param name="key">one of the standard keys of an optionable</param>
		/// <param name="val">the value to set it to</param>
		private void OnSpriteShadeChanged(string key, object val)
		{
			SpriteShade = (int)val;

			MainViewOverlay.Invalidate();
			InvalidateSecondaryPanels();

			// refresh ScanGViewer panel and current TilePanel and QuadrantPanel(s)
			if (XCMainWindow.ScanG != null)
				XCMainWindow.ScanG.InvalidatePanel();
		}

		/// <summary>
		/// Invalidates panels in secondary viewers.
		/// </summary>
		private void InvalidateSecondaryPanels()
		{
			ViewerFormsManager.TileView    .Control   .GetSelectedPanel().Invalidate();
			ViewerFormsManager.TopView     .Control   .QuadrantPanel     .Invalidate();
			ViewerFormsManager.TopRouteView.ControlTop.QuadrantPanel     .Invalidate();
		}
		#endregion Events
	}
}
