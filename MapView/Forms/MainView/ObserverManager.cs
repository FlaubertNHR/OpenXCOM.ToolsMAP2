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
		internal static ToolstripFactory ToolFactory;

		/// <summary>
		/// A list of forms that shall be closed when MapView quits.
		/// </summary>
		private static readonly IList<Form> _viewers = new List<Form>();
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
		/// Creates the observers (secondary viewers) and adds
		/// toolstrip-controls for <c><see cref="MainViewF"/></c> and
		/// <c><see cref="Observers.TopView"/></c> (<c>TopView</c> and
		/// <c>TopRouteView</c>). Also synchronizes Optionables for
		/// <c>TopRouteView(Top)</c> and <c>TopRouteView(Route)</c> to
		/// <c><see cref="Observers.TopView.Optionables">TopView.Optionables</see></c>
		/// and <c><see cref="Observers.RouteView.Optionables">RouteView.Optionables</see></c>
		/// respectively. Then loads default options for <c><see cref="Observers.TileView"/></c>,
		/// <c><see cref="Observers.TopView"/></c>, and <c><see cref="Observers.RouteView"/></c>.
		/// </summary>
		internal static void CreateViewers()
		{
			TileView     = new TileViewForm();
			TopView      = new TopViewForm();
			RouteView    = new RouteViewForm();
			TopRouteView = new TopRouteViewForm();

			ToolFactory  = new ToolstripFactory();

			TopView     .Control   .AddToolstripControls();
			TopRouteView.ControlTop.AddToolstripControls();

			_viewers.Add(TileView);
			_viewers.Add(TopView);
			_viewers.Add(RouteView);
			_viewers.Add(TopRouteView);

			// Register the subsidiary viewers via this ObserverManager.
			// TODO: Make TopView's and RouteView's Options static.
			TopRouteView.ControlTop  .Options = TopView  .Control.Options;
			TopRouteView.ControlRoute.Options = RouteView.Control.Options;

			TileView .Control.LoadControlDefaultOptions();
			TopView  .Control.LoadControlDefaultOptions();
			RouteView.Control.LoadControlDefaultOptions();

			OptionsManager.SetOptionsType(RegistryInfo.TileView,  TileView .Control.Options);
			OptionsManager.SetOptionsType(RegistryInfo.TopView,   TopView  .Control.Options);
			OptionsManager.SetOptionsType(RegistryInfo.RouteView, RouteView.Control.Options);
		}


		/// <summary>
		/// Sets or resets the <c><see cref="MapFile"/></c> for controls that
		/// need it.
		/// </summary>
		/// <param name="file"></param>
		/// <remarks>Note that
		/// <c><see cref="MainViewUnderlay.MapFile">MainViewUnderlay.MapFile</see></c>
		/// and
		/// <c><see cref="RouteControlParent">RouteControlParent</see>.MapFile</c>
		/// also contain references to the currently loaded <c>MapFile</c>.</remarks>
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
		/// Closes each Observers' parent-Form.
		/// <list>
		/// <item><c><see cref="TileViewForm"/></c></item>
		/// <item><c><see cref="TopViewForm"/></c></item>
		/// <item><c><see cref="RouteViewForm"/></c></item>
		/// <item><c><see cref="TopRouteViewForm"/></c></item>
		/// </list>
		/// </summary>
		/// <remarks>Called by <c>MainViewF.SafeQuit()</c> so this really does
		/// close the forms and update registry values.</remarks>
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
