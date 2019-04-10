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
		private readonly Dictionary<string, Form> _viewers =
					 new Dictionary<string, Form>();

		private readonly OptionsManager _optionsManager;
//		private readonly ConsoleSharedSpace _consoleShare;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="optionsManager"></param>
//		/// <param name="shareConsole"></param>
		internal ViewersManager(OptionsManager optionsManager)
//				ConsoleSharedSpace shareConsole)
		{
			_optionsManager = optionsManager;
//			_consoleShare   = shareConsole;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Sets up all subsidiary viewers.
		/// </summary>
		internal void ManageViewers()
		{
			ViewerFormsManager.TopRouteView.ControlTop  .Options = ViewerFormsManager.TopView  .Control.Options;
			ViewerFormsManager.TopRouteView.ControlRoute.Options = ViewerFormsManager.RouteView.Control.Options;

			ViewerFormsManager.TopRouteView.ControlTop  .LoadControl0Options();
			ViewerFormsManager.TopRouteView.ControlRoute.LoadControl0Options();

			SetAsObserver(RegistryInfo.TopView,   ViewerFormsManager.TopView);
			SetAsObserver(RegistryInfo.RouteView, ViewerFormsManager.RouteView);
			SetAsObserver(RegistryInfo.TileView,  ViewerFormsManager.TileView);

			_viewers.Add(RegistryInfo.TopRouteView, ViewerFormsManager.TopRouteView);

//			_viewersDictionary.Add(RegistryInfo.Console, _consoleShare.Console);

			_viewers.Add("Help",  ViewerFormsManager.HelpScreen);
			_viewers.Add("About", ViewerFormsManager.AboutScreen);

//			MainWindowsManager.TopRouteView.ControlTop  .RegistryInfo = MainWindowsManager.TopView  .Control.RegistryInfo;
//			MainWindowsManager.TopRouteView.ControlRoute.RegistryInfo = MainWindowsManager.RouteView.Control.RegistryInfo;
		}

		/// <summary>
		/// Sets a viewer as an Observer.
		/// </summary>
		/// <param name="viewer"></param>
		/// <param name="f"></param>
		private void SetAsObserver(string viewer, Form f)
		{
			_viewers.Add(viewer, f);

			var fobserver = f as IMapObserverProvider; // TopViewForm, RouteViewForm, TileViewForm only.
			if (fobserver != null)
			{
				var fcontrol = fobserver.ObserverControl0; // ie. TopView, RouteView, TileView.
				fcontrol.LoadControl0Options();

				var regInfo = new RegistryInfo(viewer, f); // subscribe to Load and Closing events.
				regInfo.RegisterProperties();

				_optionsManager.Add(viewer, fcontrol.Options);
			}
		}

		/// <summary>
		/// Closes the following viewers: TopView, RouteView, TopRouteView,
		/// TileView, Console, Help, About.
		/// </summary>
		internal void CloseSubsidiaryViewers()
		{
			foreach (string key in _viewers.Keys)
			{
				var f = _viewers[key];
				f.WindowState = FormWindowState.Normal;

				f.Close();
			}
		}
		#endregion
	}
}
