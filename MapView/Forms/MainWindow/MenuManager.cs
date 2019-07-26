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
	internal static class MenuManager
	{
		#region Fields (static)
		private const string PropertyStartObserver = "Start";
		private const string Separator = "-";

		internal const int MI_non      = -1;
		internal const int MI_TILE     =  0;
		internal const int MI_sep1     =  1;
		internal const int MI_TOP      =  2;
		internal const int MI_ROUTE    =  3;
		internal const int MI_TOPROUTE =  4;
		internal const int MI_cutoff   =  5;
		internal const int MI_SCANG    =  9;
		#endregion Fields (static)


		#region Properties (static)
		private static MenuItem Viewers
		{ get; set; }

		private static MenuItem Helpers
		{ get; set; }
		#endregion Properties (static)


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
			CreateMenuitem(ObserverManager.TileView,     Viewers, Shortcut.F5);	// id #0

			Viewers.MenuItems.Add(new MenuItem(Separator));						// id #1

			CreateMenuitem(ObserverManager.TopView,      Viewers, Shortcut.F6);	// id #2
			CreateMenuitem(ObserverManager.RouteView,    Viewers, Shortcut.F7);	// id #3
			CreateMenuitem(ObserverManager.TopRouteView, Viewers, Shortcut.F8);	// id #4

			Options options = OptionsManager.getMainOptions();
			OptionChangedEvent changer = XCMainWindow.Optionables.OnFlagChanged;

			Form f; // initialize w/ default start option ->
			bool @default;
			for (int id = MI_TILE; id != MI_cutoff; ++id)
			{
				switch (id)
				{
					default: // MI_TILE
						f = ObserverManager.TileView;
						@default = MainViewOptionables.def_StartTileView;
						break;

					case MI_sep1:
						++id;
						goto case MI_TOP;

					case MI_TOP:
						f = ObserverManager.TopView;
						@default = MainViewOptionables.def_StartTopView;
						break;
					case MI_ROUTE:
						f = ObserverManager.RouteView;
						@default = MainViewOptionables.def_StartRouteView;
						break;
					case MI_TOPROUTE:
						f = ObserverManager.TopRouteView;
						@default = MainViewOptionables.def_StartTopRouteView;
						break;
				}

				string key = PropertyStartObserver + RegistryInfo.getRegistryLabel(f);
				options.AddOptionDefault(
									key,
									@default, // true to have the viewer open on 1st run.
									changer);

				f.VisibleChanged += (sender, e) =>
				{
					var fobserver = sender as Form;
					options[key].Value = fobserver.Visible;
					XCMainWindow.Optionables.setStartPropertyValue(fobserver, fobserver.Visible);

					var foptions = XCMainWindow._foptions;
					if (foptions != null && foptions.Visible)
					{
						var grid = (foptions as OptionsForm).propertyGrid;
//						grid.SetSelectedValue((object)fobserver.Visible);
						grid.Refresh();
					}
				};
			}

			Viewers.MenuItems.Add(new MenuItem(Separator));								// id #5

			var it6 = new MenuItem("&minimize all", OnMinimizeAllClick, Shortcut.F11);	// id #6
			var it7 = new MenuItem("&restore all",  OnRestoreAllClick,  Shortcut.F12);	// id #7
			Viewers.MenuItems.Add(it6);
			Viewers.MenuItems.Add(it7);

			Viewers.MenuItems.Add(new MenuItem(Separator));								// id #8

			var it9 = new MenuItem("Scan&G view", OnScanGClick, Shortcut.CtrlG);		// id #9
			Viewers.MenuItems.Add(it9);


			// "Help" menuitems ->
			var help = new MenuItem("CHM &Help", OnHelpClick, Shortcut.F1);
			Helpers.MenuItems.Add(help);

			CreateMenuitem(ObserverManager.ColorsScreen, Helpers);
			CreateMenuitem(ObserverManager.AboutScreen,  Helpers);
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer and tags the item with its
		/// viewer's Form-object, etc.
		/// @note These forms never actually close until MainView closes.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="menu"></param>
		/// <param name="shortcut"></param>
		private static void CreateMenuitem(
				Form f,
				Menu menu,
				Shortcut shortcut = Shortcut.None)
		{
			var it = new MenuItem(f.Text.Trim());
			it.Shortcut = shortcut;
			it.Tag = f;

			menu.MenuItems.Add(it);

			it.Click += OnMenuItemClick;

			if (   f is TileViewForm
				|| f is TopViewForm
				|| f is RouteViewForm
				|| f is TopRouteViewForm)
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
		/// Visibles the subsidiary viewers that are flagged when a Map loads.
		/// @note Called by 'XCMainWindow.LoadSelectedDescriptor()'.
		/// </summary>
		internal static void StartSecondaryStage()
		{
			Viewers.Enabled = true;

			Options options = OptionsManager.getMainOptions();
			for (int id = MI_TILE; id != MI_cutoff; ++id)
			{
				if (id == MI_sep1) ++id; // skip the separator

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
			int id = MI_non;
			switch (e.KeyCode)
			{
				case Keys.F5: id = MI_TILE;     break; // show/hide viewers ->
				case Keys.F6: id = MI_TOP;      break;
				case Keys.F7: id = MI_ROUTE;    break;
				case Keys.F8: id = MI_TOPROUTE; break;

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


		#region Events (static)
		/// <summary>
		/// Handles clicking on a MenuItem to open/close a subsidiary form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal static void OnMenuItemClick(object sender, EventArgs e)
		{
			var it = sender as MenuItem;
			var f  = it.Tag as Form;

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
		/// Sets state when any of MainViewOptionables' start-viewer Properties
		/// is user-changed.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="f"></param>
		/// <param name="val"></param>
		internal static void setMenuChecked(int id, Form f, bool val)
		{
			if (Viewers.Enabled)
			{
				if (Viewers.MenuItems[id].Checked = val)
				{
					f.Show();
					if (f.WindowState == FormWindowState.Minimized)
						f.WindowState  = FormWindowState.Normal;
				}
				else
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


		internal static void OnScanGClick(object sender, EventArgs e)
		{
			if (MainViewUnderlay.that.MapBase != null)
			{
				if (!Viewers.MenuItems[MI_SCANG].Checked)
				{
					Viewers.MenuItems[MI_SCANG].Checked = true;

					XCMainWindow.ScanG = new ScanGViewer(MainViewUnderlay.that.MapBase);
					XCMainWindow.ScanG.Show(); // no owner.
				}
				else
					XCMainWindow.ScanG.BringToFront();
			}
		}

		internal static void UncheckScanG()
		{
			Viewers.MenuItems[MI_SCANG].Checked = false;
		}
		#endregion Events (static)
	}
}
