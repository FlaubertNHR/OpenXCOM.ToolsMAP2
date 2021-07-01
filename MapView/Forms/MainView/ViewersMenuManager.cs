using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared;


namespace MapView.Forms.MainView
{
	/// <summary>
	/// Instantiates and manages items on MainView's "Viewers" menu: TileView,
	/// TopView, RouteView, TopRouteView, and the ScanG viewer. As well as
	/// minimize/restore all.
	/// </summary>
	/// <remarks>See <c><see cref="ObserverManager"/></c> for instantiation of
	/// the viewers.</remarks>
	internal static class ViewersMenuManager
	{
		#region Fields (static)
		private const string PropertyStartObserver = "Start";
		private const string Separator = "-";

		internal const int MI_non      = -1;
		internal const int MI_TILE     =  0;
		private  const int MI_sep1     =  1;
		internal const int MI_TOP      =  2;
		internal const int MI_ROUTE    =  3;
		internal const int MI_TOPROUTE =  4;
		private  const int MI_cutoff   =  5;
		private  const int MI_SCANG    =  9;
		#endregion Fields (static)


		#region Properties (static)
		/// <summary>
		/// The "Viewers" <c>MenuItem</c> is assigned as
		/// <c>(MenuItem)MainViewF.menuViewers</c> which is added to
		/// <c>(MainMenu)MainViewF.mmMain</c> which is assigned as
		/// <c>(MainMenu)MainViewF.Menu</c>. Is the <c>Menu</c> of a Form
		/// disposed auto when the form closes - note that it is *not* added to
		/// <c>(IContainer)components</c> so let's assume it is disposed auto.
		/// </summary>
		private static MenuItem Viewers
		{ get; set; }
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Initializes MainView's menuitem "Viewers".
		/// </summary>
		/// <param name="viewers"></param>
		internal static void Initialize(MenuItem viewers)
		{
			Viewers = viewers;
		}

		/// <summary>
		/// Adds menuitems to MapView's "Viewers" dropdown list.
		/// </summary>
		internal static void PopulateMenu()
		{
			Options options = MainViewF.that.Options;
			OptionChangedEvent changer = MainViewF.Optionables.OnFlagChanged;

			CreateSecondaryViewerMenuitem(ObserverManager.TileView,     Shortcut.F5, options, MainViewOptionables.def_StartTileView,     changer);	// id #0

			Viewers.MenuItems.Add(new MenuItem(Separator));																							// id #1

			CreateSecondaryViewerMenuitem(ObserverManager.TopView,      Shortcut.F6, options, MainViewOptionables.def_StartTopView,      changer);	// id #2
			CreateSecondaryViewerMenuitem(ObserverManager.RouteView,    Shortcut.F7, options, MainViewOptionables.def_StartRouteView,    changer);	// id #3
			CreateSecondaryViewerMenuitem(ObserverManager.TopRouteView, Shortcut.F8, options, MainViewOptionables.def_StartTopRouteView, changer);	// id #4

			Viewers.MenuItems.Add(new MenuItem(Separator));							// id #5

			MenuItem it;
			it = new MenuItem("&minimize all", OnMinimizeAllClick, Shortcut.F11);	// id #6
			Viewers.MenuItems.Add(it);
			it = new MenuItem("&restore all",  OnRestoreAllClick,  Shortcut.F12);	// id #7
			Viewers.MenuItems.Add(it);

			Viewers.MenuItems.Add(new MenuItem(Separator));							// id #8

			it = new MenuItem("Scan&G view", OnScanGClick, Shortcut.CtrlG);			// id #9
			it.Enabled = false;
			Viewers.MenuItems.Add(it);
		}

		/// <summary>
		/// Creates a menuitem for a specified viewer and tags the item with its
		/// viewer's Form-object, etc.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="shortcut"></param>
		/// <param name="options"></param>
		/// <param name="default"><c>true</c> to have the viewer open on 1st run</param>
		/// <param name="changer"></param>
		/// <remarks>These forms never actually close until MapView closes.</remarks>
		private static void CreateSecondaryViewerMenuitem(
				Form f,
				Shortcut shortcut,
				Options options,
				bool @default,
				OptionChangedEvent changer)
		{
			var it = new MenuItem(f.Text, OnMenuItemClick, shortcut);
			it.Tag = f; // TODO: nice doc not

			Viewers.MenuItems.Add(it);

			RegistryInfo.RegisterProperties(f);

			f.FormClosing += (sender, e) =>
			{
				if (!MainViewF.Quit)
					it.Checked = false;
			};

			 // initialize MainView's Options w/ each viewer's default Start setting ->
			string key = PropertyStartObserver + RegistryInfo.GetRegistryLabel(f);
			options.CreateOptionDefault(
									key,
									@default,
									changer);

			f.VisibleChanged += (sender, e) =>
			{
				var fobserver = sender as Form;
				options[key].Value = fobserver.Visible;
				MainViewF.Optionables.setStartPropertyValue(fobserver, fobserver.Visible);

				var foptions = MainViewF._foptions as OptionsForm;
				if (foptions != null)
				{
					OptionsPropertyGrid grid = foptions.propertyGrid;
					grid.Refresh(); // yes refresh the grid even if it's hidden.
				}
			};
		}

		/// <summary>
		/// Visibles the subsidiary viewers that are flagged to Start when a Map
		/// loads.
		/// </summary>
		/// <remarks>Called by <c>MainViewF.LoadSelectedDescriptor()</c>.</remarks>
		internal static void StartSecondStageBoosters()
		{
			Viewers.Enabled = true;

			Options options = MainViewF.that.Options;
			for (int id = MI_TILE; id != MI_cutoff; ++id)
			{
				if (id == MI_sep1) ++id; // skip the separator

				MenuItem it = Viewers.MenuItems[id];
				if (options[PropertyStartObserver + RegistryInfo.GetRegistryLabel(it.Tag as Form)].IsTrue)
				{
					OnMenuItemClick(it, EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// Processes keydown events that shall be captured and abused at the
		/// <c>Form</c> level.
		/// <list type="bullet">
		/// <item>shows/hides/minimizes/restores viewers on F-key events</item>
		/// <item>handles activity by <c><see cref="Observers.TileViewForm"/></c>,
		/// <c><see cref="Observers.TopViewForm"/></c>,
		/// <c><see cref="Observers.RouteViewForm"/></c>, and
		/// <c><see cref="Observers.TopRouteViewForm"/></c></item>
		/// </list>
		/// </summary>
		/// <param name="e"></param>
		internal static void ViewerKeyDown(KeyEventArgs e)
		{
			int id = MI_non;

			switch (e.KeyData)
			{
				case Keys.F5:
				case Keys.Control | Keys.F5:
					id = MI_TILE;
					break;

				case Keys.F6:
				case Keys.Control | Keys.F6:
					id = MI_TOP;
					break;

				case Keys.F7:
				case Keys.Control | Keys.F7:
					id = MI_ROUTE;
					break;

				case Keys.F8:
				case Keys.Control | Keys.F8:
					id = MI_TOPROUTE;
					break;

				case Keys.Shift | Keys.F5: // focus MainView
				case Keys.Shift | Keys.F6:
				case Keys.Shift | Keys.F7:
				case Keys.Shift | Keys.F8:
					e.SuppressKeyPress = true;

					if (MainViewF.that.WindowState == FormWindowState.Minimized)
						MainViewF.that.WindowState =  FormWindowState.Normal;

					MainViewF.that.Select();
					break;

				case Keys.F11: // MinimizeAll ->
					e.SuppressKeyPress = true;
					OnMinimizeAllClick(null, EventArgs.Empty);
					break;

				case Keys.F12: // RestoreAll ->
					e.SuppressKeyPress = true;
					OnRestoreAllClick(null, EventArgs.Empty);
					break;
			}

			if (id != MI_non)
			{
				e.SuppressKeyPress = true;
				OnMenuItemClick(Viewers.MenuItems[id], EventArgs.Empty);
			}
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
//				f.Owner = MainViewF.that;	// NOTE: If MainView is set as the owner of the
											// viewers MainView can no longer be minimized
											// while keeping the other viewers up. Etc.
											// Policy #1008: let the viewers operate as independently as possible.
											// See also: MainViewF.OnActivated()

				f.Show();
				if (f.WindowState == FormWindowState.Minimized)
					f.WindowState  = FormWindowState.Normal;
			}
			else
				f.Close(); // ie. Hide()
		}

		/// <summary>
		/// Sets state when any of <c><see cref="MainViewOptionables"/></c>'
		/// start-viewer Properties is user-changed.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="val"></param>
		internal static void setMenuChecked(int id, bool val)
		{
			if (Viewers.Enabled)
			{
				MenuItem it = Viewers.MenuItems[id];
				var f = it.Tag as Form;

				if (it.Checked = val)
				{
					f.Show();
					if (f.WindowState == FormWindowState.Minimized)
						f.WindowState  = FormWindowState.Normal;
				}
				else
					f.Close(); // ie. Hide()
			}
		}


		/// <summary>
		/// Handles clicks on the Viewers|MinimizeAll item. Also F11.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Ironically this seems to bypass MainView's Activated event
		/// but RestoreAll doesn't. So just bypass that event's handler for
		/// safety regardless of .NET's shenanigans.</remarks>
		private static void OnMinimizeAllClick(object sender, EventArgs e)
		{
			MainViewF.BypassActivatedEvent = true;

			IList<Form> zOrder = ShowHideManager.getZorderList();
			foreach (var f in zOrder)
			if (   f.WindowState == FormWindowState.Normal
				|| f.WindowState == FormWindowState.Maximized)
			{
				f.WindowState = FormWindowState.Minimized;
			}

			MainViewF.BypassActivatedEvent = false;
		}

		/// <summary>
		/// Handles clicks on the Viewers|RestoreAll item. Also F12.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Ironically setting the windowstate (to 'Normal') seems to
		/// trigger MainView's Activated event but MinimizeAll doesn't. So just
		/// bypass that event's handler for safety regardless of .NET's
		/// shenanigans.</remarks>
		private static void OnRestoreAllClick(object sender, EventArgs e)
		{
			MainViewF.BypassActivatedEvent = true;

			bool bringtofront = !MainViewF.that.Focused
							 || !MainViewF.Optionables.BringAllToFront;

			IList<Form> zOrder = ShowHideManager.getZorderList();
			foreach (var f in zOrder)
			{
				if (   f.WindowState == FormWindowState.Minimized
					|| f.WindowState == FormWindowState.Maximized)
				{
					f.WindowState = FormWindowState.Normal;
				}

				if (bringtofront)
				{
					f.TopMost = true;
					f.TopMost = false;
				}
			}

			MainViewF.BypassActivatedEvent = false;
		}


		/// <summary>
		/// Enables the ScanG viewer menuitem.
		/// </summary>
		/// <param name="enabled"></param>
		internal static void EnableScanG(bool enabled)
		{
			Viewers.MenuItems[MI_SCANG].Enabled = enabled;
		}

		/// <summary>
		/// Handles clicks on the ScanG view item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal static void OnScanGClick(object sender, EventArgs e)
		{
			if (MainViewF.that.MapFile != null)
			{
				MenuItem it = Viewers.MenuItems[MI_SCANG];
				if (!it.Checked)
				{
					it.Checked = true;

					MainViewF.ScanG = new ScanGViewer(MainViewF.that.MapFile);
				}
				else
					MainViewF.ScanG.BringToFront();
			}
		}

		/// <summary>
		/// Unchecks the ScanG viewer item when ScanG viewer closes.
		/// </summary>
		internal static void DecheckScanG()
		{
			Viewers.MenuItems[MI_SCANG].Checked = false;
		}
		#endregion Events (static)
	}
}
