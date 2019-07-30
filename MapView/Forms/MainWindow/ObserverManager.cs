using System.Collections.Generic;
using System.Windows.Forms;

using DSShared;

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
		private static readonly List<Form> _viewers = new List<Form>();
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
		#endregion Properties (static)


		#region Methods (static)
		internal static void Initialize()
		{
			TileView     = new TileViewForm();
			TopView      = new TopViewForm();
			RouteView    = new RouteViewForm();
			TopRouteView = new TopRouteViewForm();

			ToolFactory  = new ToolstripFactory(MainViewOverlay.that);

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


			// Register the subsidiary viewers via this ObserverManager.
			// TODO: Make TopView's and RouteView's Options static.
			TopRouteView.ControlTop  .Options = ObserverManager.TopView  .Control.Options;
			TopRouteView.ControlRoute.Options = ObserverManager.RouteView.Control.Options;

			InitializeObserver(RegistryInfo.TileView,  TileView);
			InitializeObserver(RegistryInfo.TopView,   TopView);
			InitializeObserver(RegistryInfo.RouteView, RouteView);

			_viewers.Add(TopRouteView);
		}

		/// <summary>
		/// Sets a viewer as an Observer.
		/// @note 'TileViewForm', 'TopViewForm', 'RouteViewForm' only.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="f"></param>
		private static void InitializeObserver(string key, Form f)
		{
			_viewers.Add(f);

			var control = (f as IMapObserverProvider).ObserverControl; // ie. TileView, TopView, RouteView.
			control.LoadControlDefaultOptions();
			OptionsManager.setOptionsType(key, control.Options);
		}

		internal static void SetObservers(MapFileBase @base)
		{
			foreach (var f in _observers)
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

		/// <summary>
		/// Closes the Observers.
		/// - TileView
		/// - TopView
		/// - RouteView
		/// - TopRouteView
		/// @note Called by XCMainWindow.OnFormClosing() so this really does
		/// close -> update registry vals.
		/// </summary>
		internal static void CloseViewers()
		{
			foreach (var f in _viewers)
				f.Close();
		}
		#endregion Methods (static)
	}
}
