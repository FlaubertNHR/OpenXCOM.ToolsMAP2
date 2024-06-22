using System;
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
		/// <summary>
		/// A list of <c>Forms</c> that shall be closed when MapView quits.
		/// </summary>
		private static readonly IList<Form> _observers = new List<Form>();
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

		internal static ToolstripFactory ToolFactory
		{ get; private set; }
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Creates the observers (secondary viewers) and adds
		/// toolstrip-controls for <c><see cref="MainViewF"/></c> and
		/// <c><see cref="Observers.TopView"/></c> (TopView and TopRouteView).
		/// Also loads default options for
		/// <c><see cref="Observers.TileView"/></c>,
		/// <c><see cref="Observers.TopView"/></c>, and
		/// <c><see cref="Observers.RouteView"/></c>.
		/// </summary>
		internal static void CreateObservers()
		{
			TileView     = new TileViewForm();
			TopView      = new TopViewForm();
			RouteView    = new RouteViewForm();
			TopRouteView = new TopRouteViewForm();

			ToolFactory  = new ToolstripFactory();

			TopView     .Control   .AddToolstripControls();
			TopRouteView.ControlTop.AddToolstripControls();

			_observers.Add(TileView);
			_observers.Add(TopView);
			_observers.Add(RouteView);
			_observers.Add(TopRouteView);


			Observers.TileView .Optionables.LoadDefaults(Observers.TileView .Options);
			Observers.TopView  .Optionables.LoadDefaults(Observers.TopView  .Options);
			Observers.RouteView.Optionables.LoadDefaults(Observers.RouteView.Options);

			OptionsManager.SetOptionsSection(RegistryInfo.TileView,  Observers.TileView .Options);
			OptionsManager.SetOptionsSection(RegistryInfo.TopView,   Observers.TopView  .Options);
			OptionsManager.SetOptionsSection(RegistryInfo.RouteView, Observers.RouteView.Options);
		}


		/// <summary>
		/// Sets or resets the <c><see cref="MapFile"/></c> for controls that
		/// need it.
		/// </summary>
		/// <param name="file"></param>
		/// <remarks>Note that
		/// <c><see cref="MainViewF.MapFile">MainViewF.MapFile</see></c> and
		/// <c><see cref="RouteControlParent">RouteControlParent</see>.MapFile</c>
		/// also contain pointers to the currently loaded <c>MapFile</c>.</remarks>
		internal static void AssignMapfile(MapFile file)
		{
			TileView.Control                       .SetMapfile(file);

			TopView.Control                        .SetMapfile(file); // level selected handler
			TopView.Control.TopControl             .SetMapfile(file);
			TopView.Control.QuadrantControl        .SetMapfile(file); // location,level selected handlers

			TopRouteView.ControlTop                .SetMapfile(file); // level selected handler
			TopRouteView.ControlTop.TopControl     .SetMapfile(file);
			TopRouteView.ControlTop.QuadrantControl.SetMapfile(file); // location,level selected handlers

			RouteView   .Control                   .SetMapfile(file); // location,level selected handlers
			TopRouteView.ControlRoute              .SetMapfile(file); // location,level selected handlers

			MainViewOverlay.that.Refresh();
		}


		/// <summary>
		/// Closes each Observers' parent <c>Form</c>.
		/// <list>
		/// <item><c><see cref="TileViewForm"/></c></item>
		/// <item><c><see cref="TopViewForm"/></c></item>
		/// <item><c><see cref="RouteViewForm"/></c></item>
		/// <item><c><see cref="TopRouteViewForm"/></c></item>
		/// </list>
		/// </summary>
		/// <remarks>Called by <c><see cref="MainViewF"></see>.SafeQuit()</c> so
		/// this really does close the forms and update registry values.</remarks>
		internal static void CloseObservers()
		{
			foreach (var f in _observers)
				f.Close();
		}
		#endregion Methods (static)


		#region Update UI (static)
		/// <summary>
		/// Invalidates the <c><see cref="TopControl">TopControls</see></c> in
		/// <c>TopView</c> and <c>TopRouteView(Top)</c>.
		/// </summary>
		internal static void InvalidateTopControls()
		{
			TopView     .Control   .TopControl.Invalidate();
			TopRouteView.ControlTop.TopControl.Invalidate();
		}

		/// <summary>
		/// Invalidates the
		/// <c><see cref="QuadrantControl">QuadrantControls</see></c>
		/// in <c>TopView</c> and <c>TopRouteView(Top)</c>.
		/// </summary>
		internal static void InvalidateQuadrantControls()
		{
			TopView     .Control   .QuadrantControl.Invalidate();
			TopRouteView.ControlTop.QuadrantControl.Invalidate();
		}

		/// <summary>
		/// Refreshes the
		/// <c><see cref="QuadrantControl">QuadrantControls</see></c>
		/// in <c>TopView</c> and <c>TopRouteView(Top)</c>.
		/// </summary>
		internal static void RefreshQuadrantControls()
		{
			TopView     .Control   .QuadrantControl.Refresh();
			TopRouteView.ControlTop.QuadrantControl.Refresh();
		}
		#endregion Update UI (static)
	}
}
