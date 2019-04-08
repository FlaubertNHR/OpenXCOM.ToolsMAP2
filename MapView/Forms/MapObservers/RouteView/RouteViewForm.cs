﻿using System;
using System.Windows.Forms;

using MapView.Forms.MainWindow;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed partial class RouteViewForm
		:
			Form,
			IMapObserverProvider
	{
		#region Properties
		internal RouteView Control
		{
			get { return RouteViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return RouteViewControl; }
		}
		#endregion


		#region cTor
		internal RouteViewForm()
		{
			InitializeComponent();
		}
		#endregion


		#region Events (override)
		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - shows/hides/minimizes/restores viewers on certain F-key events.
		/// - opens/closes Options on [Ctrl+o] event.
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, TopViewForm, TopRouteViewForm
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
						Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
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
