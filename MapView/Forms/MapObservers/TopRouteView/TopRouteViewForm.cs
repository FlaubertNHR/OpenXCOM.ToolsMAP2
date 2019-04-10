using System;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MapObservers.TileViews // y, "TileView" thanks for knifing the concept of namespaces in the back.
{
	internal sealed partial class TopRouteViewForm
		:
			Form
	{
		internal TopRouteViewForm()
		{
			InitializeComponent();

			var regInfo = new RegistryInfo(RegistryInfo.TopRouteView, this); // subscribe to Load and Closing events.
			regInfo.RegisterProperties();
		}


		internal TopView ControlTop
		{
			get { return TopViewControl; }
		}

		internal RouteView ControlRoute
		{
			get { return RouteViewControl; }
		}


		#region Events (override)
		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - opens/closes Options on [Ctrl+o] event.
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, TopViewForm, RouteViewForm
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((e.Modifiers & Keys.Control) == Keys.Control
				&& e.KeyCode == Keys.O)
			{
				switch (tabControl.SelectedIndex)
				{
					case 0: ControlTop  .OnOptionsClick(ControlTop  .GetOptionsButton(), EventArgs.Empty); break;
					case 1: ControlRoute.OnOptionsClick(ControlRoute.GetOptionsButton(), EventArgs.Empty); break;
				}
				e.SuppressKeyPress = true;
			}
			else
				MainMenusManager.ViewerKeyDown(e);

			base.OnKeyDown(e);
		}

		/// <summary>
		/// Handles form closing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			WindowState = FormWindowState.Normal; // else causes probls when opening a viewer that was closed while maximized.
			base.OnFormClosing(e);
		}
		#endregion Events (override)
	}
}
