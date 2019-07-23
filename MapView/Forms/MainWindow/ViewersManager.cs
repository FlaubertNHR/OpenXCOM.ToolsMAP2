using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MapObservers;


namespace MapView.Forms.MainWindow
{
	internal static class ViewersManager
	{
		#region Fields (static)
		private static readonly List<Form> _viewers = new List<Form>();
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Registers all subsidiary viewers via this ViewersManager.
		/// </summary>
		internal static void Initialize()
		{
			// TODO: Make TopView's and RouteView's Options static.
			ObserverManager.TopRouteView.ControlTop  .Options = ObserverManager.TopView  .Control.Options;
			ObserverManager.TopRouteView.ControlRoute.Options = ObserverManager.RouteView.Control.Options;

			SetAsObserver(RegistryInfo.TileView,  ObserverManager.TileView);
			SetAsObserver(RegistryInfo.TopView,   ObserverManager.TopView);
			SetAsObserver(RegistryInfo.RouteView, ObserverManager.RouteView);

			_viewers.Add(ObserverManager.TopRouteView);

			_viewers.Add(ObserverManager.ColorsScreen);
			_viewers.Add(ObserverManager.AboutScreen);
		}

		/// <summary>
		/// Sets a viewer as an Observer.
		/// @note 'TileViewForm', 'TopViewForm', 'RouteViewForm' only.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="f"></param>
		private static void SetAsObserver(string label, Form f)
		{
			_viewers.Add(f);

			var control = (f as IMapObserverProvider).ObserverControl; // ie. 'TileView', 'TopView', 'RouteView'.
			control.LoadControlDefaultOptions();
			OptionsManager.setOptionsType(label, control.Options);
		}

		/// <summary>
		/// Closes the following viewers.
		/// SECONDARY:
		/// - TileView
		/// - TopView
		/// - RouteView
		/// - TopRouteView
		/// TERTIARY:
		/// - ColorsHelp
		/// - About
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
