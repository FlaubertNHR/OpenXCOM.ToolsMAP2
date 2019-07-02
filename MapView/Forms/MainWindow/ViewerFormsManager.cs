using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MainWindow
{
	internal static class ViewerFormsManager
	{
		#region Fields (static)
		internal static ToolstripFactory ToolFactory;

		private static IMapObserver[] _observers;
		#endregion Fields (static)


		#region Properties (static)
		// TODO: Just initialize the fucking forms, instead of doing umpteen ba-
		// zillion checks during the running of the app. It's not that fucking
		// hard. In fact it's harder to untangle the mess after the fact!

		private  static TileViewForm _tileView;
		internal static TileViewForm  TileView
		{
			get { return _tileView ?? (_tileView = new TileViewForm()); }
		}

		private  static TopViewForm _topView;
		internal static TopViewForm  TopView
		{
			get { return _topView ?? (_topView = new TopViewForm()); }
		}

		private  static RouteViewForm _routeView;
		internal static RouteViewForm  RouteView
		{
			get { return _routeView ?? (_routeView = new RouteViewForm()); }
		}

		private  static TopRouteViewForm _topRouteView;
		internal static TopRouteViewForm  TopRouteView
		{
			get { return _topRouteView ?? (_topRouteView = new TopRouteViewForm()); }
		}


		private  static ColorHelp _colorsScreen;
		internal static ColorHelp  ColorsScreen
		{
			get { return _colorsScreen ?? (_colorsScreen = new ColorHelp()); }
		}

		private  static About _aboutWindow;
		internal static About  AboutScreen
		{
			get { return _aboutWindow ?? (_aboutWindow = new About()); }
		}
		#endregion Properties (static)


		#region Events (static)
		/// <summary>
		/// Changes the selected quadrant in the QuadrantPanel when a tilepart
		/// is selected in TileView.
		/// </summary>
		/// <param name="part"></param>
		private static void OnTileSelected_SelectQuadrant(Tilepart part)
		{
			if (part != null && part.Record != null)
			{
				TopView     .Control   .SelectQuadrant(part.Record.PartType);
				TopRouteView.ControlTop.SelectQuadrant(part.Record.PartType);
			}
		}
		#endregion Events (static)


		#region Methods (static)
		internal static void Initialize()
		{
			TopView     .Control   .InitializeToolstrip(ToolFactory);
			TopRouteView.ControlTop.InitializeToolstrip(ToolFactory);

			TileView.Control.TileSelected_SelectQuadrant += OnTileSelected_SelectQuadrant;

			_observers = new IMapObserver[]
			{
				TileView    .Control,
				TopView     .Control,
				RouteView   .Control,
				TopRouteView.ControlTop,
				TopRouteView.ControlRoute
			};
		}

		internal static void SetObservers(MapFileBase @base)
		{
			foreach (var f in _observers)
				if (f != null)
					SetObserver(@base, f);

			MainViewUnderlay.that.MainViewOverlay.Refresh();
		}

		private static void SetObserver(MapFileBase @base, IMapObserver observer)
		{
			if (observer.MapBase != null)
			{
				observer.MapBase.SelectLocationEvent -= observer.OnSelectLocationObserver;
				observer.MapBase.SelectLevelEvent    -= observer.OnSelectLevelObserver;
			}

			if ((observer.MapBase = @base) != null)
			{
				observer.MapBase.SelectLocationEvent += observer.OnSelectLocationObserver;
				observer.MapBase.SelectLevelEvent    += observer.OnSelectLevelObserver;
			}

			foreach (string key in observer.Panels.Keys) // ie. 'TopPanel' and 'QuadrantsPanel'
				SetObserver(observer.MapBase, observer.Panels[key]);
		}
		#endregion Methods (static)
	}
}
