using MapView.Forms.MapObservers;
using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;

using XCom.Interfaces.Base;


namespace MapView.Forms.MainWindow
{
	internal static class ObserverManager
	{
		#region Fields (static)
		internal static ToolstripFactory ToolFactory;

		private static IMapObserver[] _observers;
		#endregion Fields (static)


		#region Properties (static)
		internal static TileViewForm TileView
		{ get; private set; }

		internal static TopViewForm TopView
		{ get; private set; }

		internal static RouteViewForm RouteView
		{ get; private set; }

		internal static TopRouteViewForm TopRouteView
		{ get; private set; }


		internal static ColorHelp ColorsScreen
		{ get; private set; }

		internal static About AboutScreen
		{ get; private set; }
		#endregion Properties (static)


		#region Methods (static)
		internal static void Initialize()
		{
			TileView     = new TileViewForm();
			TopView      = new TopViewForm();
			RouteView    = new RouteViewForm();
			TopRouteView = new TopRouteViewForm();

			ColorsScreen = new ColorHelp();
			AboutScreen  = new About();


			ToolFactory = new ToolstripFactory(MainViewOverlay.that);

			TopView     .Control   .InitializeToolstrip(ToolFactory);
			TopRouteView.ControlTop.InitializeToolstrip(ToolFactory);

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

			MainViewOverlay.that.Refresh();
		}

		private static void SetObserver(MapFileBase @base, IMapObserver observer)
		{
			if (observer.MapBase != null)
			{
				observer.MapBase.SelectLocation -= observer.OnSelectLocationObserver;
				observer.MapBase.SelectLevel    -= observer.OnSelectLevelObserver;
			}

			if ((observer.MapBase = @base) != null)
			{
				observer.MapBase.SelectLocation += observer.OnSelectLocationObserver;
				observer.MapBase.SelectLevel    += observer.OnSelectLevelObserver;
			}

			foreach (string key in observer.ObserverPanels.Keys) // ie. TopPanel and QuadrantPanel
				SetObserver(observer.MapBase, observer.ObserverPanels[key]);
		}
		#endregion Methods (static)
	}
}
