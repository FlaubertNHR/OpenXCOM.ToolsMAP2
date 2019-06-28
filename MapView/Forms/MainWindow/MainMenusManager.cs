using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView.Forms.MainWindow
{
	internal static class MainMenusManager
	{
		#region Fields (static)
		private static Options _optionsMain;

		private static bool _quit;

		private const string Separator = "-";
		#endregion Fields (static)


		#region Properties (static)
		internal static MenuItem MenuViewers
		{ get; private set; }

		private static MenuItem MenuHelp
		{ get; set; }
		#endregion Properties (static)


		#region Events (static)
		/// <summary>
		/// Handles clicking on a MenuItem to open/close a subsidiary form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal static void OnMenuItemClick(object sender, EventArgs e)
		{
			var it = sender as MenuItem;
			var f = it.Tag as Form;

			if (it.Checked && Control.ModifierKeys == Keys.Control)
			{
				if (f.WindowState == FormWindowState.Minimized)
					f.WindowState =  FormWindowState.Normal;

				f.Select();
			}
			else if (it.Checked && f.WindowState == FormWindowState.Minimized)
			{
				f.WindowState = FormWindowState.Normal;
			}
			else if (it.Checked = !it.Checked)
			{
//				f.Owner = XCMainWindow.that;	// NOTE: If MainView is set as the owner of the
												// viewers MainView can no longer be minimized
												// while keeping the other viewers up. Etc.
												// Policy #1008: let the viewers operate as independently as possible.
												// See also: XCMainWindow.OnActivated()

				f.Show();
				f.WindowState = FormWindowState.Normal;

				if (it.Tag is ColorHelp) // update colors that user might have set in TileView's Option-settings.
					ViewerFormsManager.ColorsScreen.UpdateColors();
			}
			else
			{
				f.WindowState = FormWindowState.Normal;
				f.Close();
			}
		}

		/// <summary>
		/// Shows the CHM helpfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnHelpClick(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(XCMainWindow.that, "file://" + help);
		}


		/// <summary>
		/// Handles clicks on the Viewers|MinimizeAll item. Also F11.
		/// @note Ironically this seems to bypass MainView's Activated event but
		/// RestoreAll doesn't. So just bypass that event's handler for safety
		/// regardless of .NET's shenanigans.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnMinimizeAllClick(object sender, EventArgs e)
		{
			XCMainWindow.BypassActivatedEvent = true;

			var zOrder = ShowHideManager.getZorderList();
			foreach (var f in zOrder)
			{
				if (   f.WindowState == FormWindowState.Normal
					|| f.WindowState == FormWindowState.Maximized)
				{
					f.WindowState = FormWindowState.Minimized;
				}
			}

			XCMainWindow.BypassActivatedEvent = false;
		}

		/// <summary>
		/// Handles clicks on the Viewers|RestoreAll item. Also F12.
		/// @note Ironically this seems to trigger MainView's Activated event
		/// but MinimizeAll doesn't. So just bypass that event's handler for
		/// safety regardless of .NET's shenanigans.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnRestoreAllClick(object sender, EventArgs e)
		{
			XCMainWindow.BypassActivatedEvent = true;

			var zOrder = ShowHideManager.getZorderList();
			foreach (var f in zOrder)
			{
				if (   f.WindowState == FormWindowState.Minimized
					|| f.WindowState == FormWindowState.Maximized)
				{
					f.WindowState = FormWindowState.Normal;
				}
			}

			XCMainWindow.BypassActivatedEvent = false;
		}
		#endregion Events (static)


		#region Methods (static)
		/// <summary>
		/// Sets the menus to manage.
		/// </summary>
		/// <param name="itViewers"></param>
		/// <param name="itHelp"></param>
		internal static void SetMenus(MenuItem itViewers, MenuItem itHelp)
		{
			MenuViewers = itViewers;
			MenuHelp    = itHelp;
		}

		/// <summary>
		/// Adds menuitems to MapView's "Viewers" and "Help" dropdown lists.
		/// </summary>
		/// <param name="optionsMain">pointer to MainView's Options (for
		/// subsidiary viewers' visibility only)</param>
		internal static void PopulateMenus(Options optionsMain)
		{
			_optionsMain = optionsMain;

			// "Viewers" menuitems ->
			CreateMenuItem(ViewerFormsManager.TileView,     RegistryInfo.TileView,     MenuViewers, Shortcut.F5);	// id #0

			MenuViewers.MenuItems.Add(new MenuItem(Separator));														// id #1

			CreateMenuItem(ViewerFormsManager.TopView,      RegistryInfo.TopView,      MenuViewers, Shortcut.F6);	// id #2
			CreateMenuItem(ViewerFormsManager.RouteView,    RegistryInfo.RouteView,    MenuViewers, Shortcut.F7);	// id #3
			CreateMenuItem(ViewerFormsManager.TopRouteView, RegistryInfo.TopRouteView, MenuViewers, Shortcut.F8);	// id #4

			MenuViewers.MenuItems.Add(new MenuItem(Separator));														// id #5

			var it6 = new MenuItem("minimize all", OnMinimizeAllClick, Shortcut.F11);								// id #6
			var it7 = new MenuItem("restore all",  OnRestoreAllClick,  Shortcut.F12);								// id #7
			MenuViewers.MenuItems.Add(it6);
			MenuViewers.MenuItems.Add(it7);


			// "Help" menuitems ->
			var miHelp = new MenuItem("Help");
			miHelp.Click += OnHelpClick;
			MenuHelp.MenuItems.Add(miHelp);

			CreateMenuItem(ViewerFormsManager.ColorsScreen, "Colors", MenuHelp);
			CreateMenuItem(ViewerFormsManager.AboutScreen,  "About",  MenuHelp);


			AddVisibilityOptions();
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer.
		/// @note The forms never actually close until MainView closes.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="caption"></param>
		/// <param name="parent"></param>
		/// <param name="shortcut"></param>
		private static void CreateMenuItem(
				Form f,
				string caption,
				Menu parent,
				Shortcut shortcut = Shortcut.None)
		{
			f.Text = caption; // set the titlebar text.

			var it = new MenuItem(caption);
			it.Shortcut = shortcut;
			it.Tag = f;

			parent.MenuItems.Add(it);

			it.Click += OnMenuItemClick;

			f.FormClosing += (sender, e) =>
			{
				it.Checked = false;
				e.Cancel = true;
				f.Hide();
			};
		}

		/// <summary>
		/// Adds each viewer's visibility flag to user Options.
		/// </summary>
		private static void AddVisibilityOptions()
		{
			foreach (MenuItem it in MenuViewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null)
				{
					string key = it.Text;
					_optionsMain.AddOption(
									key,
									(       it.Tag is TopViewForm)	// true to have the viewer open on 1st run.
										|| (it.Tag is RouteViewForm)
										|| (it.Tag is TileViewForm),
									"Open on load - " + key,		// appears as a tip at the bottom of the Options screen.
									"Windows");						// this identifies what Option category the setting appears under.

					f.VisibleChanged += (sender, e) =>
					{
						if (!_quit)
						{
							var fsender = sender as Form;
							if (fsender != null)
								_optionsMain[key].Value = fsender.Visible;
						}
					};
				}
			}
		}

		/// <summary>
		/// Visibles the subsidiary viewers that are flagged when a Map loads.
		/// </summary>
		internal static void StartViewers()
		{
			foreach (MenuItem it in MenuViewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null && _optionsMain[it.Text].IsTrue)	// NOTE: All viewers shall be keyed in Options w/ the item-text.
					it.PerformClick();							// TODO: uhhh ...
			}
			MenuViewers.Enabled = true;
		}


		/// <summary>
		/// Effectively disables the 'VisibleChanged' event for all subsidiary
		/// viewers.
		/// </summary>
		internal static void Quit()
		{
			_quit = true;
		}


		/// <summary>
		/// Processes keydown events that shall be captured and abused at the
		/// Form level.
		/// - shows/hides/minimizes/restores viewers on F-key events
		/// - handles activity by TileViewForm, TopViewForm, RouteViewForm,
		///   and TopRouteViewForm
		/// </summary>
		/// <param name="e"></param>
		/// <returns>true if key is suppressed</returns>
		internal static bool ViewerKeyDown(KeyEventArgs e)
		{
			int it = -1;
			switch (e.KeyCode)
			{
				case Keys.F5: it = 0; break; // show/hide viewers ->
				case Keys.F6: it = 2; break;
				case Keys.F7: it = 3; break;
				case Keys.F8: it = 4; break;

				case Keys.F11: OnMinimizeAllClick(null, EventArgs.Empty); break; // min/rest ->
				case Keys.F12: OnRestoreAllClick( null, EventArgs.Empty); break;

				default: return false; // else do not suppress key-event for any other key
			}

			if (it != -1)
			{
				if (e.Shift)
				{
					if (XCMainWindow.that.WindowState == FormWindowState.Minimized)
						XCMainWindow.that.WindowState =  FormWindowState.Normal;

					XCMainWindow.that.Select();
				}
				else
					OnMenuItemClick(
								MenuViewers.MenuItems[it],
								EventArgs.Empty);
			}
			e.SuppressKeyPress = true;
			return true;
		}
		#endregion Methods (static)
	}
}
