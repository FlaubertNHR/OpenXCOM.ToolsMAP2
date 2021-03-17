using System.Collections.Generic;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.Observers;

using XCom;


namespace MapView.Forms.MainView
{
	internal static class ObserverManager
	{
		#region Fields (static)
		internal static ToolstripFactory ToolFactory;

		private static IObserver[] _observers;
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
		/// <summary>
		/// Creates the observers (secondary viewers) as well as
		/// toolstrip-controls for <see cref="MainViewF"/>, <see cref="Observers.TopView"/>,
		/// and <see cref="Observers.TopRouteViewForm"/>(Top). Also synchronizes
		/// Optionables for TopRouteView(Top) and TopRouteView(Route) to
		/// <see cref="Observers.TopView.Optionables">TopView.Optionables</see>
		/// and <see cref="Observers.RouteView.Optionables">RouteView.Optionables</see>
		/// respectively. Then loads default options for <see cref="Observers.TileView"/>,
		/// <see cref="Observers.TopView"/>, and <see cref="Observers.RouteView"/>.
		/// </summary>
		internal static void CreateViewers()
		{
			TileView     = new TileViewForm();
			TopView      = new TopViewForm();
			RouteView    = new RouteViewForm();
			TopRouteView = new TopRouteViewForm();

			ToolFactory  = new ToolstripFactory();

			TopView     .Control   .CreateToolstripControls();
			TopRouteView.ControlTop.CreateToolstripControls();

			_observers = new IObserver[]
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

			LoadDefaultOptions(RegistryInfo.TileView,  TileView);
			LoadDefaultOptions(RegistryInfo.TopView,   TopView);
			LoadDefaultOptions(RegistryInfo.RouteView, RouteView);

			_viewers.Add(TopRouteView);
		}

		/// <summary>
		/// Sets an observer as a viewer and loads its default options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="f"></param>
		/// <remarks><see cref="TileViewForm"/>, <see cref="TopViewForm"/>,
		/// <see cref="RouteViewForm"/> only.</remarks>
		private static void LoadDefaultOptions(string key, IObserverProvider f)
		{
			_viewers.Add(f as Form);

			ObserverControl observer = f.Observer; // ie. TileView, TopView, RouteView.
			observer.LoadControlDefaultOptions();
			OptionsManager.setOptionsType(key, observer.Options);
		}

		/// <summary>
		/// Sets or resets the MapFile for each observer.
		/// </summary>
		/// <param name="file"></param>
		internal static void SetMapfile(MapFile file)
		{
			foreach (var f in _observers)
				SubscribeObserver(file, f);

			MainViewOverlay.that.Refresh();
		}

		/// <summary>
		/// Subscribes <see cref="IObserver.OnLocationSelectedObserver"/> and
		/// <see cref="IObserver.OnLevelSelectedObserver"/> events to a
		/// specified MapFile ( TODO: isn't that redundant ) for an observer -
		/// incl/ the panels in TopView.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="observer"></param>
		private static void SubscribeObserver(MapFile file, IObserver observer)
		{
			if (observer.MapFile != null)
			{
				observer.MapFile.LocationSelected -= observer.OnLocationSelectedObserver;
				observer.MapFile.LevelSelected    -= observer.OnLevelSelectedObserver;
			}

			if ((observer.MapFile = file) != null)
			{
				observer.MapFile.LocationSelected += observer.OnLocationSelectedObserver;
				observer.MapFile.LevelSelected    += observer.OnLevelSelectedObserver;
			}

			foreach (string key in observer.ObserverPanels.Keys) // ie. TopControl and QuadrantControl
				SubscribeObserver(observer.MapFile, observer.ObserverPanels[key]);
		}

		/// <summary>
		/// Closes the Observers.
		/// - TileView
		/// - TopView
		/// - RouteView
		/// - TopRouteView
		/// </summary>
		/// <remarks>Called by MainViewF.OnFormClosing() so this really does
		/// close -> update registry vals.</remarks>
		internal static void CloseViewers()
		{
			foreach (var f in _viewers)
				f.Close();
		}
		#endregion Methods (static)


		#region Update UI (static)
		/// <summary>
		/// Invalidates the <see cref="TopControl">TopControls</see> in TopView
		/// and TopRouteView(Top).
		/// </summary>
		internal static void InvalidateTopControls()
		{
			TopView     .Control   .TopControl.Invalidate();
			TopRouteView.ControlTop.TopControl.Invalidate();
		}

		/// <summary>
		/// Invalidates the <see cref="QuadrantControl">QuadrantControls</see>
		/// in TopView and TopRouteView(Top).
		/// </summary>
		internal static void InvalidateQuadrantControls()
		{
			TopView     .Control   .QuadrantControl.Invalidate();
			TopRouteView.ControlTop.QuadrantControl.Invalidate();
		}
		#endregion Update UI (static)
	}
}
