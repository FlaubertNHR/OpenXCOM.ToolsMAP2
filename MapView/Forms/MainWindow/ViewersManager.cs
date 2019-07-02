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
			ViewerFormsManager.TopRouteView.ControlTop  .Options = ViewerFormsManager.TopView  .Control.Options;
			ViewerFormsManager.TopRouteView.ControlRoute.Options = ViewerFormsManager.RouteView.Control.Options;

			SetAsObserver(RegistryInfo.TileView,  ViewerFormsManager.TileView);
			SetAsObserver(RegistryInfo.TopView,   ViewerFormsManager.TopView);
			SetAsObserver(RegistryInfo.RouteView, ViewerFormsManager.RouteView);

			_viewers.Add(ViewerFormsManager.TopRouteView);

			_viewers.Add(ViewerFormsManager.ColorsScreen);
			_viewers.Add(ViewerFormsManager.AboutScreen);
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
			control.LoadControlOptions();
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
