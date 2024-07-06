using System;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// This is the <c>Form</c> that contains <c><see cref="TopView"/></c> and
	/// <c><see cref="RouteView"/></c> as <c>TabPages</c> in a
	/// <c>TabControl</c>.
	/// </summary>
	/// <remarks>This is instantiated by
	/// <c><see cref="ObserverManager.CreateObservers()">ObserverManager.CreateObservers()</see></c>
	/// and closed by
	/// <c><see cref="ObserverManager.CloseObservers()">ObserverManager.CloseObservers()</see></c>.</remarks>
	internal sealed partial class TopRouteViewForm
		:
			Form
	{
		#region Fields (static)
		/// <summary>
		/// The index of TopView in <c><see cref="tabControl"/></c>.
		/// </summary>
		internal const int TAB_TOP = 0;

		/// <summary>
		/// The index of RouteView in <c><see cref="tabControl"/></c>.
		/// </summary>
		internal const int TAB_ROT = 1;
		#endregion Fields (static)


		#region Properties
		/// <summary>
		/// Gets/Sets <c><see cref="TopView"/></c>.
		/// </summary>
		internal TopView ControlTop
		{ get; private set; }

		/// <summary>
		/// Gets/Sets <c><see cref="RouteView"/></c>.
		/// </summary>
		internal RouteView ControlRoute
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal TopRouteViewForm()
		{
			InitializeComponent();
			var tpBorder = new TabPageBorder(tabControl);
			tpBorder.TabPageBorder_init();

			ControlTop = new TopView();
			ControlTop.Name       = "TopViewControl";
			ControlTop.Dock       = DockStyle.Fill;
			ControlTop.TabIndex   = 0;
			ControlTop.isToproute = true;
			tp_Top.Controls.Add(ControlTop);

			ControlRoute = new RouteView();
			ControlRoute.Name       = "RouteViewControl";
			ControlRoute.Dock       = DockStyle.Fill;
			ControlRoute.TabIndex   = 0;
			ControlRoute.isToproute = true;
			tp_Route.Controls.Add(ControlRoute);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Sets up the link-connector when this <c>Form</c> is shown.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			ControlRoute.ActivateConnector();
		}

		/// <summary>
		/// Fires when this <c>Form</c> is activated. Maintains the position of
		/// this <c>Form</c> in the z-order List and focuses the panel if
		/// <c><see cref="ControlTop"/></c> is currently selected.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

			if (tabControl.SelectedIndex == TAB_TOP)
			{
				ControlTop.TopControl.ClearSelectorLozenge(); // when PartslotTest is closed the selector-lozenge can glitch.
				ControlTop.TopControl.Focus();
			}
		}

		/// <summary>
		/// Handles a so-called command-key at the <c>Form</c> level. Stops keys
		/// that shall be used for navigating the tiles from doing anything
		/// stupid instead.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		/// <remarks><c><see cref="ControlTop"/></c> passes the arrow-keys to
		/// <c><see cref="MainViewOverlay.Navigate()">MainViewOverlay.Navigate()</see></c>.
		/// <c><see cref="ControlRoute"/></c> passes the arrow-keys to
		/// <c><see cref="RouteControlParent.Navigate()">RouteControlParent.Navigate()</see></c>.</remarks>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (tabControl.SelectedIndex)
			{
				case TAB_TOP:
					if (ControlTop.TopControl.Focused)
					{
						switch (keyData)
						{
							case Keys.Left:
							case Keys.Right:
							case Keys.Up:
							case Keys.Down:
							case Keys.Shift | Keys.Left:
							case Keys.Shift | Keys.Right:
							case Keys.Shift | Keys.Up:
							case Keys.Shift | Keys.Down:
								MainViewOverlay.that.Navigate(keyData, true);
								return true;
						}
					}
					break;

				case TAB_ROT:
					if (ControlRoute.RouteControl.Focused)
					{
						switch (keyData)
						{
							case Keys.Left:
							case Keys.Right:
							case Keys.Up:
							case Keys.Down:
							case Keys.Shift | Keys.Left:
							case Keys.Shift | Keys.Right:
							case Keys.Shift | Keys.Up:
							case Keys.Shift | Keys.Down:
								ControlRoute.RouteControl.Navigate(keyData);
								return true;
						}
					}
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles <c>KeyDown</c> events at the <c>Form</c> level. See notes
		/// for <c><see cref="TopViewForm"/>.OnKeyDown()</c> and
		/// <c><see cref="RouteViewForm"/>.OnKeyDown()</c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c> <c>true</c>.
		/// <br/><br/>
		/// See also <c><see cref="TileViewForm"/></c> /
		/// <c><see cref="TopViewForm"/></c> /
		/// <c><see cref="RouteViewForm"/></c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			PartType quadrant = PartType.Invalid;

			switch (e.KeyData)
			{
				case Keys.Escape:
					switch (tabControl.SelectedIndex)
					{
						case TAB_TOP:
							if (!ControlTop.TopControl.Focused)
							{
								e.SuppressKeyPress = true;
								ControlTop.TopControl.Focus();
							}
							else
								MainViewOverlay.that.Edit(e);
							break;

						case TAB_ROT:
							e.SuppressKeyPress = true;
							if (ControlRoute.RouteControl.Focused)
							{
								RouteView.NodeSelected = null;
								RouteView.Invalidator();
							}
							else
								ControlRoute.RouteControl.Focus();
							break;
					}
					break;

				case Keys.Control | Keys.O:
					e.SuppressKeyPress = true;
					switch (tabControl.SelectedIndex)
					{
						case TAB_TOP:
							ControlTop.OnOptionsClick(ControlTop.GetOptionsButton(), EventArgs.Empty);
							break;
						case TAB_ROT:
							ControlRoute.OnOptionsClick(ControlRoute.GetOptionsButton(), EventArgs.Empty);
							break;
					}
					break;

				case Keys.Control | Keys.Q:
					e.SuppressKeyPress = true;
					MainViewF.that.OnQuitClick(null, EventArgs.Empty);
					break;

				case Keys.D1: quadrant = PartType.Floor;   break;
				case Keys.D2: quadrant = PartType.West;    break;
				case Keys.D3: quadrant = PartType.North;   break;
				case Keys.D4: quadrant = PartType.Content; break;

				case Keys.Add:
				case Keys.Subtract:
				case Keys.Home:
				case Keys.End:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Shift | Keys.Home:
				case Keys.Shift | Keys.End:
				case Keys.Shift | Keys.PageUp:
				case Keys.Shift | Keys.PageDown:
					switch (tabControl.SelectedIndex)
					{
						case TAB_TOP:
							if (ControlTop.TopControl.Focused)
							{
								e.SuppressKeyPress = true;
								MainViewOverlay.that.Navigate(e.KeyData, true);
							}
							break;

						case TAB_ROT:
							if (ControlRoute.RouteControl.Focused) // is Route
							{
								e.SuppressKeyPress = true;
								ControlRoute.RouteControl.Navigate(e.KeyData);
							}
							break;
					}
					break;

				case Keys.Enter:
				case Keys.Shift | Keys.Add:
				case Keys.Shift | Keys.Subtract:
					if (tabControl.SelectedIndex == TAB_ROT && ControlRoute.RouteControl.Focused)
					{
						e.SuppressKeyPress = true;
						ControlRoute.RouteControl.Navigate(e.KeyData);
					}
					break;

				default:
					ViewersMenuManager.ViewerKeyDown(e); // NOTE: this can suppress the key
					break;
			}

			switch (tabControl.SelectedIndex)
			{
				case TAB_TOP:
					if (quadrant != PartType.Invalid)
					{
						e.SuppressKeyPress = true;
						var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
						ControlTop.QuadrantControl.doMouseDown(args, quadrant);
					}
					break;

				case TAB_ROT:
					base.OnKeyDown(e);
					break;
			}
		}

		/// <summary>
		/// Handles the <c>FormClosing</c> event. Ensures that the PartslotTest
		/// dialog and <c><see cref="SpawnInfo"/></c> get closed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				TopView.ClosePartslotTestDialog();

				if (RouteView.SpawnInfo != null)
					RouteView.SpawnInfo.Close();

				if (MainViewF.Quit)
				{
					//Logfile.Log("TopRouteViewForm.OnFormClosing()");
					RegistryInfo.UpdateRegistry(this);
					ControlTop  .TopControl  .DisposeControl();
					ControlRoute.RouteControl.DisposeControl();
				}
				else
				{
					e.Cancel = true;
					Hide();
				}
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Tracks the selected page in
		/// <c><see cref="MainViewF.Optionables">MainViewF.Optionables</see></c>
		/// and prints the Mapfile or Routefile path on the titlebar.
		/// </summary>
		/// <param name="sender"><c><see cref="tabControl"/></c></param>
		/// <param name="e"></param>
		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MainViewF.Optionables.StartTopRoutePage > 1)
			{
				MainViewF.Optionables.StartTopRoutePage = tabControl.SelectedIndex + 2;

				if (MainViewF._foptions != null)
					MainViewF._foptions.propertyGrid.Refresh(); // yes refresh the grid even if it's hidden.
			}


			string title;
			if (MainViewF.that.MapFile != null)
			{
				if (tabControl.SelectedIndex == TAB_ROT)
				{
					// note: 'MapFile' always has a valid 'Routes' I believe
					title = " - " + RouteNodes.PfeRoutes;
				}
				else // TAB_TOP
				{
					string pfe = MainViewF.that.MapFile.Descriptor.GetMapfilePath();
					if (pfe == null) pfe = "error";
					title = " - " + pfe;
				}
			}
			else
				title = String.Empty;

			Text = Globals.TITLE_tr + title;
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Focuses a tabpage when
		/// <c><see cref="MainViewOptionables.StartTopRoutePage">MainViewOptionables.StartTopRoutePage</see></c>
		/// changes.
		/// </summary>
		/// <param name="page"></param>
		internal void SelectTabpage(int page)
		{
			tabControl.SelectedIndex = page;
		}

		/// <summary>
		/// Gets the id of the currently selected tabpage.
		/// </summary>
		/// <returns></returns>
		internal int GetSelectedTabpage()
		{
			return tabControl.SelectedIndex;
		}
		#endregion Methods
	}
}
