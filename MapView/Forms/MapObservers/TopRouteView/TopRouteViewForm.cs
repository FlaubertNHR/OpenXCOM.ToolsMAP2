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
		/// - shows/hides/minimizes/restores viewers on certain F-key events.
		/// - opens/closes Options on [Ctrl+o] event.
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, TopViewForm, RouteViewForm
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			int it = -1;

			switch (e.KeyCode)
			{
				case Keys.F5: it = 0; break; // show/hide viewers ->
				case Keys.F6: it = 2; break;
				case Keys.F7: it = 3; break;
				case Keys.F8: it = 4; break;

				case Keys.F11:
					MainMenusManager.OnMinimizeAllClick(null, EventArgs.Empty);
					return;
				case Keys.F12:
					MainMenusManager.OnRestoreAllClick(null, EventArgs.Empty);
					return;

				case Keys.O:
					if ((e.Modifiers & Keys.Control) == Keys.Control)
					{
						switch (tabControl.SelectedIndex)
						{
							case 0: ControlTop  .OnOptionsClick(ControlTop  .GetOptionsButton(), EventArgs.Empty); break;
							case 1: ControlRoute.OnOptionsClick(ControlRoute.GetOptionsButton(), EventArgs.Empty); break;
						}
						return;
					}
					goto default;

				default:
					base.OnKeyDown(e);
					return;
			}

			if (e.Shift)
			{
				if (XCMainWindow.Instance.WindowState == FormWindowState.Minimized)
					XCMainWindow.Instance.WindowState =  FormWindowState.Normal;

				XCMainWindow.Instance.Select();
			}
			else
				MainMenusManager.OnMenuItemClick(
											MainMenusManager.MenuViewers.MenuItems[it],
											EventArgs.Empty);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			WindowState = FormWindowState.Normal; // else causes probls when opening a viewer that was closed while maximized.
			base.OnFormClosing(e);
		}
		#endregion Events (override)
	}
}
