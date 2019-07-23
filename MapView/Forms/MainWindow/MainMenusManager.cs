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
		private const string PropertyStartObserver = "Start";
		private const string Separator = "-";
		#endregion Fields (static)


		#region Properties (static)
		private static MenuItem Viewers
		{ get; set; }

		private static MenuItem Helpers
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
				if (f.WindowState == FormWindowState.Minimized)
					f.WindowState  = FormWindowState.Normal;

				if (it.Tag is ColorHelp) // update colors that user could have changed in TileView's Option-settings.
					ObserverManager.ColorsScreen.UpdateColors();
			}
			else
				f.Close();
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
		/// @note Ironically this - setting the windowstate (to 'Normal') -
		/// seems to trigger MainView's Activated event but MinimizeAll doesn't.
		/// So just bypass that event's handler for safety regardless of .NET's
		/// shenanigans.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnRestoreAllClick(object sender, EventArgs e)
		{
			XCMainWindow.BypassActivatedEvent = true;

			bool bringtofront = !XCMainWindow.that.Focused
							 || !XCMainWindow.Optionables.BringAllToFront;

			var zOrder = ShowHideManager.getZorderList();
			foreach (var f in zOrder)
			{
				if (   f.WindowState == FormWindowState.Minimized
					|| f.WindowState == FormWindowState.Maximized)
				{
					f.WindowState = FormWindowState.Normal;
				}

				if (bringtofront) // tentative ->
				{
					f.TopMost = true;
					f.TopMost = false;
				}
			}

			XCMainWindow.BypassActivatedEvent = false;
		}
		#endregion Events (static)


		#region Methods (static)
		/// <summary>
		/// Sets the menus to manage.
		/// </summary>
		/// <param name="it0"></param>
		/// <param name="it1"></param>
		internal static void SetMenus(MenuItem it0, MenuItem it1)
		{
			Viewers = it0;
			Helpers = it1;
		}

		/// <summary>
		/// Adds menuitems to MapView's "Viewers" and "Help" dropdown lists.
		/// </summary>
		internal static void PopulateMenus()
		{
			// "Viewers" menuitems ->
			CreateMenuitem(ObserverManager.TileView,     RegistryInfo.TileView,     Viewers, Shortcut.F5);	// id #0

			Viewers.MenuItems.Add(new MenuItem(Separator));													// id #1

			CreateMenuitem(ObserverManager.TopView,      RegistryInfo.TopView,      Viewers, Shortcut.F6);	// id #2
			CreateMenuitem(ObserverManager.RouteView,    RegistryInfo.RouteView,    Viewers, Shortcut.F7);	// id #3
			CreateMenuitem(ObserverManager.TopRouteView, RegistryInfo.TopRouteView, Viewers, Shortcut.F8);	// id #4

			Viewers.MenuItems.Add(new MenuItem(Separator));													// id #5

			var it6 = new MenuItem("minimize all", OnMinimizeAllClick, Shortcut.F11);						// id #6
			var it7 = new MenuItem("restore all",  OnRestoreAllClick,  Shortcut.F12);						// id #7
			Viewers.MenuItems.Add(it6);
			Viewers.MenuItems.Add(it7);


			Options options = OptionsManager.getMainOptions();
			OptionChangedEvent changer = XCMainWindow.Optionables.OnFlagChanged;
			foreach (MenuItem it in Viewers.MenuItems)
			{
				var f = it.Tag as Form;
				if (f != null)
				{
					string key = PropertyStartObserver + RegistryInfo.getRegistryLabel(f);
					options.AddOptionDefault(
										key,
										f is TileViewForm || f is TopViewForm // true to have the viewer open on 1st run.
														  || f is RouteViewForm,
										changer);

					f.VisibleChanged += (sender, e) =>
					{
						options[key].Value = (sender as Form).Visible;
					};
				}
			}


			// "Help" menuitems ->
			var help = new MenuItem("CHM Help", OnHelpClick, Shortcut.F1);
			Helpers.MenuItems.Add(help);

			CreateMenuitem(ObserverManager.ColorsScreen, "Colors", Helpers);
			CreateMenuitem(ObserverManager.AboutScreen,  "About",  Helpers);
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer and tags the item with its
		/// viewer's Form-object, etc.
		/// @note These forms never actually close until MainView closes.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="caption"></param>
		/// <param name="parent"></param>
		/// <param name="shortcut"></param>
		private static void CreateMenuitem(
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

			if (secondary(f))
			{
				RegistryInfo.RegisterProperties(f);

				f.FormClosing += (sender, e) =>
				{
					if (!XCMainWindow.Quit)
					{
						it.Checked = false;
						e.Cancel = true;
						f.Hide();
					}
					else
						RegistryInfo.UpdateRegistry(f);
				};
			}
			else
			{
				f.FormClosing += (sender, e) =>
				{
					if (!XCMainWindow.Quit)
					{
						it.Checked = false;
						e.Cancel = true;
						f.Hide();
					}
				};
			}
		}

		/// <summary>
		/// Checks if a specified form is a secondary viewer. Secondary forms
		/// are instantiated on app-start and remain that way for the life of
		/// the app (many tertiary forms do too, but secondary forms shall be
		/// guaranteed).
		/// </summary>
		/// <param name="f">a form to check against</param>
		/// <returns></returns>
		private static bool secondary(Form f)
		{
			return f is TileViewForm
				|| f is TopViewForm
				|| f is RouteViewForm
				|| f is TopRouteViewForm;
		}

		/// <summary>
		/// Visibles the subsidiary viewers that are flagged when a Map loads.
		/// @note Called by 'XCMainWindow.LoadSelectedDescriptor()'.
		/// </summary>
		internal static void StartSecondaryStage()
		{
			Viewers.Enabled = true;

			Options options = OptionsManager.getMainOptions();
			for (int id = 0; id != 5; ++id)
			{
				if (id == 1) ++id; // skip the separator

				var it = Viewers.MenuItems[id];
				if (options[PropertyStartObserver + RegistryInfo.getRegistryLabel(it.Tag as Form)].IsTrue)
				{
					OnMenuItemClick(it, EventArgs.Empty);
				}
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
			int id = -1;
			switch (e.KeyCode)
			{
				case Keys.F5: id = 0; break; // show/hide viewers ->
				case Keys.F6: id = 2; break;
				case Keys.F7: id = 3; break;
				case Keys.F8: id = 4; break;

				case Keys.F11: OnMinimizeAllClick(null, EventArgs.Empty); break; // min/rest ->
				case Keys.F12: OnRestoreAllClick( null, EventArgs.Empty); break;

				default: return false; // else do not suppress key-event for any other key
			}

			if (id != -1)
			{
				if (e.Shift) // focus MainView for any (valid) 'id'
				{
					if (XCMainWindow.that.WindowState == FormWindowState.Minimized)
						XCMainWindow.that.WindowState =  FormWindowState.Normal;

					XCMainWindow.that.Select();
				}
				else
					OnMenuItemClick(
								Viewers.MenuItems[id],
								EventArgs.Empty);
			}
			e.SuppressKeyPress = true;
			return true;
		}
		#endregion Methods (static)
	}
}
