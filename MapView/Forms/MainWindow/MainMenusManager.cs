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
		#region Fields (constant)
		private const string Separator = "-";
		#endregion Fields (constant)


		#region Fields
		private static readonly List<MenuItem> _allItems = new List<MenuItem>();
		private static readonly List<Form>     _allForms = new List<Form>();

		private static Options _options;

		private static bool _quit;
		#endregion Fields


		#region Properties
		internal static MenuItem MenuViewers
		{ get; private set; }

		private static MenuItem MenuHelp
		{ get; set; }
		#endregion Properties


		#region Events
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
//				f.Owner = XCMainWindow.Instance;	// NOTE: If MainView is set as the owner of the
													// viewers MainView can no longer be minimized
													// while keeping the other viewers up. Etc.
													// Policy #1008: let the viewers operate as independently as possible.
													// See also: XCMainWindow.OnActivated()

				f.Show();
				f.WindowState = FormWindowState.Normal;

				if (it.Tag is ColorHelp) // update colors that user might have set in TileView's Option-settings.
					ViewerFormsManager.HelpScreen.UpdateColors();
			}
			else
			{
				f.WindowState = FormWindowState.Normal;
				f.Close();
			}
		}

/*		/// <summary>
		/// Helper for OnMenuItemClick() above. It's called only on the
		/// XCMainWindow menu-click, or a TileViewForm/ TopViewForm/
		/// RouteViewForm/ TopRouteViewForm F-key press.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		private static Control FindFocusedControl(Control control)
		{
			var container = control as ContainerControl;
			while (container != null)
			{
				control = container.ActiveControl;
				container = control as ContainerControl;
			}
			return control;
		} */


		/// <summary>
		/// Shows the CHM helpfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnHelpClick(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(XCMainWindow.Instance, "file://" + help);
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Sets the menus to manage.
		/// </summary>
		/// <param name="viewers"></param>
		/// <param name="help"></param>
		internal static void SetMenus(MenuItem viewers, MenuItem help)
		{
			MenuViewers = viewers;
			MenuHelp    = help;
		}

		/// <summary>
		/// Adds menuitems to MapView's "Viewers" and "Help" dropdown lists.
		/// </summary>
//		/// <param name="fconsole">pointer to the console-form</param>
		/// <param name="options">pointer to MV_OptionsFile options</param>
		internal static void PopulateMenus(/*Form fconsole,*/ Options options)
		{
			_options = options;

			// Viewers menuitems ->
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

//			_menuViewers.MenuItems.Add(new MenuItem(Divider));
//			CreateMenuItem(fconsole, RegistryInfo.Console, _menuViewers); // TODO: either use the Console or lose it.

			// Help menuitems ->
			var miHelp = new MenuItem("Help");
			miHelp.Click += OnHelpClick;
			MenuHelp.MenuItems.Add(miHelp);

			CreateMenuItem(ViewerFormsManager.HelpScreen,  "Colors", MenuHelp);
			CreateMenuItem(ViewerFormsManager.AboutScreen, "About",  MenuHelp);


			AddViewersOptions();
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer. See PopulateMenus() above^
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

			_allItems.Add(it);
			_allForms.Add(f);
		}

		/// <summary>
		/// Adds each viewer's opened/closed flag to user Options.
		/// </summary>
		private static void AddViewersOptions()
		{
			foreach (MenuItem it in MenuViewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null)
				{
					string key = it.Text;
					_options.AddOption(
									key,
//									!(it.Tag is MapView.Forms.MapObservers.TileViews.TopRouteViewForm),	// q. why is TopRouteViewForm under 'TileViews'
																										// a. why not.
									(       it.Tag is TopViewForm)	// true to have the viewer open on 1st run.
										|| (it.Tag is RouteViewForm)
										|| (it.Tag is TileViewForm),
									"Open on load - " + key,		// appears as a tip at the bottom of the Options screen.
									"Windows");						// this identifies what Option category the setting appears under.
																	// NOTE: the Console is not technically a viewer
//																	// but it appears under Options like the real viewers.
					f.VisibleChanged += (sender, e) =>
					{
						if (!_quit)
						{
							var fsender = sender as Form;
							if (fsender != null)
								_options[key].Value = fsender.Visible;
						}
					};
				}
			}
		}

		/// <summary>
		/// Opens the subsidiary viewers that are flagged to open when a Map
		/// loads.
		/// </summary>
		internal static void StartViewers()
		{
			foreach (MenuItem it in MenuViewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null && _options[it.Text].IsTrue) // NOTE: All viewers shall be keyed in Options w/ the item-text.
					it.PerformClick();
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
		/// Creates a device that minimizes and restores all subsidiary viewers
		/// when PckView is opened via TileView.
		/// </summary>
		/// <returns></returns>
		internal static ShowHideManager CreateShowHideManager()
		{
			return new ShowHideManager(_allForms, _allItems);
		}


		/// <summary>
		/// Handles clicks on the Viewers|MinimizeAll item. Also F11.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnMinimizeAllClick(object sender, EventArgs e)
		{
			foreach (MenuItem it in MenuViewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null //&& _options[it.Text].IsTrue
					&& (   f.WindowState == FormWindowState.Normal
						|| f.WindowState == FormWindowState.Maximized))
				{
					f.WindowState = FormWindowState.Minimized;
				}
			}

			if (   XCMainWindow.Instance.WindowState == FormWindowState.Normal
				|| XCMainWindow.Instance.WindowState == FormWindowState.Maximized)
			{
				XCMainWindow.Instance.WindowState = FormWindowState.Minimized;
			}
		}

		/// <summary>
		/// Handles clicks on the Viewers|RestoreAll item. Also F12.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnRestoreAllClick(object sender, EventArgs e)
		{
			foreach (MenuItem it in MenuViewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null //&& _options[it.Text].IsTrue
					&& (   f.WindowState == FormWindowState.Minimized
						|| f.WindowState == FormWindowState.Maximized))
				{
					f.WindowState = FormWindowState.Normal;
				}
			}

			if (   XCMainWindow.Instance.WindowState == FormWindowState.Maximized
				|| XCMainWindow.Instance.WindowState == FormWindowState.Minimized)
			{
				XCMainWindow.Instance.WindowState = FormWindowState.Normal;
			}
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
					if (XCMainWindow.Instance.WindowState == FormWindowState.Minimized)
						XCMainWindow.Instance.WindowState =  FormWindowState.Normal;

					XCMainWindow.Instance.Select();
				}
				else
					OnMenuItemClick(
								MenuViewers.MenuItems[it],
								EventArgs.Empty);
			}
			e.SuppressKeyPress = true;
			return true;
		}
		#endregion Methods
	}
}
