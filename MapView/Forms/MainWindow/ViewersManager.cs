using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MapObservers;


namespace MapView.Forms.MainWindow
{
	internal sealed class ViewersManager
	{
		#region Fields
		private readonly List<Form> _viewers = new List<Form>();
		#endregion Fields


		#region Methods
		/// <summary>
		/// Registers all subsidiary viewers via this ViewersManager.
		/// </summary>
		internal void InitializeViewersManager()
		{
			ViewerFormsManager.TopRouteView.ControlTop  .Options = ViewerFormsManager.TopView  .Control.Options;
			ViewerFormsManager.TopRouteView.ControlRoute.Options = ViewerFormsManager.RouteView.Control.Options;

			ViewerFormsManager.TopRouteView.ControlTop  .LoadControl0Options();
			ViewerFormsManager.TopRouteView.ControlRoute.LoadControl0Options();

			SetAsObserver(RegistryInfo.TopView,   ViewerFormsManager.TopView);
			SetAsObserver(RegistryInfo.RouteView, ViewerFormsManager.RouteView);
			SetAsObserver(RegistryInfo.TileView,  ViewerFormsManager.TileView);

			_viewers.Add(ViewerFormsManager.TopRouteView);

			_viewers.Add(ViewerFormsManager.ColorsScreen);
			_viewers.Add(ViewerFormsManager.AboutScreen);
		}

		/// <summary>
		/// Sets a viewer as an Observer.
		/// </summary>
		/// <param name="viewer"></param>
		/// <param name="f"></param>
		private void SetAsObserver(string viewer, Form f)
		{
			_viewers.Add(f);

			var fobserver = f as IMapObserverProvider; // TopViewForm, RouteViewForm, TileViewForm only.
			if (fobserver != null)
			{
				var fcontrol = fobserver.ObserverControl0; // ie. TopView, RouteView, TileView.
				fcontrol.LoadControl0Options();

				var regInfo = new RegistryInfo(viewer, f); // subscribe to Load and Closing events.
				regInfo.RegisterProperties();

				OptionsManager.Add(viewer, fcontrol.Options);
			}
		}

		/// <summary>
		/// Closes the following viewers: TopView, RouteView, TopRouteView,
		/// TileView, Console, Colors, About.
		/// </summary>
		internal void CloseViewers()
		{
			foreach (var viewer in _viewers)
			{
				viewer.WindowState = FormWindowState.Normal;
				viewer.Close();
			}
		}
		#endregion Methods
	}
}
