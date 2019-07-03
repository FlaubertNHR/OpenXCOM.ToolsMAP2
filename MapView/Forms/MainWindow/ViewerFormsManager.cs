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
			TileView     = new TileViewForm();
			TopView      = new TopViewForm();
			RouteView    = new RouteViewForm();
			TopRouteView = new TopRouteViewForm();

			ColorsScreen = new ColorHelp();
			AboutScreen  = new About();


			ToolFactory = new ToolstripFactory(MainViewUnderlay.that);

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

			MainViewOverlay.that.Refresh();
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
