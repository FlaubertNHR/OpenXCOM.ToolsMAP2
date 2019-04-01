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
	internal sealed class MainMenusManager
	{
		#region Fields (static)
		private const string Separator = "-";
		#endregion


		#region Fields
		private readonly MenuItem _menuViewers;
		private readonly MenuItem _menuHelp;

		private readonly List<MenuItem> _allItems = new List<MenuItem>();
		private readonly List<Form>     _allForms = new List<Form>();

		private Options _options;

		private bool _quit;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="show"></param>
		/// <param name="help"></param>
		internal MainMenusManager(MenuItem show, MenuItem help)
		{
			_menuViewers = show; // why are these MenuItems
			_menuHelp    = help;
		}
		#endregion


		#region Events (static)
		/// <summary>
		/// Handles clicking on a MenuItem to open/close a subsidiary form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnMenuItemClick(object sender, EventArgs e)
		{
			var it = sender as MenuItem;
			var f = it.Tag as Form;

			if (it.Checked && f.WindowState == FormWindowState.Minimized)
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

				f.WindowState = FormWindowState.Normal;
				f.Show();

				if (it.Tag is ColorHelp) // update colors that user might have set in TileView's Option-settings.
					ViewerFormsManager.HelpScreen.UpdateColors();
			}
			else
			{
				f.WindowState = FormWindowState.Normal;
				f.Close();
			}
		}
		#endregion


		#region Events
		/// <summary>
		/// Shows the CHM helpfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHelpClick(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(XCMainWindow.Instance, "file://" + help);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Adds menuitems to MapView's dropdown list.
		/// </summary>
//		/// <param name="fconsole">pointer to the console-form</param>
		/// <param name="options">pointer to MV_OptionsFile options</param>
		internal void PopulateMenus(/*Form fconsole,*/ Options options)
		{
			_options = options;

			// Viewers menuitems ->
			CreateMenuItem(ViewerFormsManager.TileView,     RegistryInfo.TileView,     _menuViewers, Shortcut.F5);

			_menuViewers.MenuItems.Add(new MenuItem(Separator));

			CreateMenuItem(ViewerFormsManager.TopView,      RegistryInfo.TopView,      _menuViewers, Shortcut.F6);
			CreateMenuItem(ViewerFormsManager.RouteView,    RegistryInfo.RouteView,    _menuViewers, Shortcut.F7);
			CreateMenuItem(ViewerFormsManager.TopRouteView, RegistryInfo.TopRouteView, _menuViewers, Shortcut.F8);

//			_menuViewers.MenuItems.Add(new MenuItem(Divider));
//			CreateMenuItem(fconsole, RegistryInfo.Console, _menuViewers); // TODO: either use the Console or lose it.

			// Help menuitems ->
			var miHelp = new MenuItem("Help");
			miHelp.Click += OnHelpClick;
			_menuHelp.MenuItems.Add(miHelp);

			CreateMenuItem(ViewerFormsManager.HelpScreen,  "Colors", _menuHelp);
			CreateMenuItem(ViewerFormsManager.AboutScreen, "About",  _menuHelp);


			AddViewersOptions();
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer. See PopulateMenus() above^
		/// </summary>
		/// <param name="f"></param>
		/// <param name="caption"></param>
		/// <param name="parent"></param>
		/// <param name="shortcut"></param>
		private void CreateMenuItem(
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
		private void AddViewersOptions()
		{
			foreach (MenuItem it in _menuViewers.MenuItems)
			{
				string key = it.Text;
				if (!key.Equals(Separator, StringComparison.Ordinal))
				{
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
					var f = it.Tag as Form;							// but it appears under Options like the real viewers.
					if (f != null)
					{
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
		}

		/// <summary>
		/// Opens the subsidiary viewers that are flagged to open when a Map
		/// loads.
		/// </summary>
		internal void StartViewers()
		{
			foreach (MenuItem it in _menuViewers.MenuItems)
			{
				string key = it.Text;
				if (!key.Equals(Separator, StringComparison.Ordinal)
					&& _options[key].IsTrue)
				{
					it.PerformClick();
				}
			}
			_menuViewers.Enabled = true;
		}

		/// <summary>
		/// Effectively disables the 'VisibleChanged' event for all subsidiary
		/// viewers.
		/// </summary>
		internal void Quit()
		{
			_quit = true;
		}

		/// <summary>
		/// Creates a device that minimizes and restores all subsidiary viewers
		/// when PckView is opened via TileView.
		/// </summary>
		/// <returns></returns>
		internal ShowHideManager CreateShowHideManager()
		{
			return new ShowHideManager(_allForms, _allItems);
		}
		#endregion
	}
}
