using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	internal sealed partial class TopRouteViewForm
		:
			Form
	{
		#region Fields (static)
		private const int TAB_TOP = 0;
		private const int TAB_ROT = 1;
		#endregion Fields (static)


		#region Properties
		private TopView _topView;
		internal TopView ControlTop
		{
			get { return _topView; }
		}

		private RouteView _routeView;
		internal RouteView ControlRoute
		{
			get { return _routeView; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal TopRouteViewForm()
		{
			InitializeComponent();
			InitializeTopRouteViews();

			var tpTabControl = new TabPageBorder(tabControl);
		}

		private void InitializeTopRouteViews()
		{
			_topView = new TopView();

			ControlTop.Name       = "TopViewControl";
			ControlTop.Location   = new Point(3, 3);
			ControlTop.Size       = new Size(618, 423);
			ControlTop.Dock       = DockStyle.Fill;
			ControlTop.TabIndex   = 0;
			ControlTop.Tag        = "TOPROUTE";

			_routeView = new RouteView();

			ControlRoute.Name     = "RouteViewControl";
			ControlRoute.Location = new Point(3, 3);
			ControlRoute.Size     = new Size(618, 423);
			ControlRoute.Dock     = DockStyle.Fill;
			ControlRoute.TabIndex = 0;
			ControlRoute.Tag      = "TOPROUTE";

			tp_Top  .Controls.Add(ControlTop);
			tp_Route.Controls.Add(ControlRoute);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnShown(EventArgs e)
		{
			ControlRoute.ActivateConnector();

//			base.OnShown(e);
		}

		/// <summary>
		/// Fires when the form is activated. Maintains the position of this
		/// form in the z-order List and focuses the panel if the TopViewControl
		/// is currently selected.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

			if (tabControl.SelectedIndex == TAB_TOP)
			{
				ControlTop.TopPanel.ClearSelectorLozenge(); // when TestPartslots is closed the selector-lozenge can glitch.
				ControlTop.TopPanel.Focus();
			}

//			base.OnActivated(e);
		}

		/// <summary>
		/// Handles a so-called command-key at the form level. Stops keys that
		/// shall be used for navigating the tiles from doing anything stupid
		/// instead.
		/// - passes the arrow-keys to the appropriate control's panel's
		///   Navigate() funct
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (tabControl.SelectedIndex)
			{
				case TAB_TOP:
					if (ControlTop.TopPanel.Focused)
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
					if (ControlRoute.RoutePanel.Focused)
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
								ControlRoute.RoutePanel.Navigate(keyData);
								return true;
						}
					}
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - [Esc] focuses the appropriate panel
		/// - opens/closes Options on [Ctrl+o] event
		/// - checks for and if so processes a viewer F-key
		/// - passes edit-keys to the appropriate viewer's control's panel's
		///   Navigate() funct
		/// - selects a quadrant if TopView is the current tabpage
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, TopViewForm, RouteViewForm
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			PartType slot = PartType.Invalid;

			switch (e.KeyData)
			{
				case Keys.Escape:
					switch (tabControl.SelectedIndex)
					{
						case TAB_TOP:
							if (!ControlTop.TopPanel.Focused)
							{
								e.SuppressKeyPress = true;
								ControlTop.TopPanel.Focus();
							}
							else
								MainViewOverlay.that.Edit(e);
							break;

						case TAB_ROT:
							e.SuppressKeyPress = true;
							if (ControlRoute.RoutePanel.Focused)
							{
								RouteView.NodeSelected = null;
								RouteView.Invalidator();
							}
							else
								ControlRoute.RoutePanel.Focus();
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

				case Keys.D1: slot = PartType.Floor;   break;
				case Keys.D2: slot = PartType.West;    break;
				case Keys.D3: slot = PartType.North;   break;
				case Keys.D4: slot = PartType.Content; break;

				case Keys.Subtract:
				case Keys.Add:
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
							if (ControlTop.TopPanel.Focused)
							{
								e.SuppressKeyPress = true;
								MainViewOverlay.that.Navigate(e.KeyData, true);
							}
							break;

						case TAB_ROT:
							if (ControlRoute.RoutePanel.Focused) // is Route
							{
								e.SuppressKeyPress = true;
								ControlRoute.RoutePanel.Navigate(e.KeyData);
							}
							break;
					}
					break;

				case Keys.Shift | Keys.Subtract:
				case Keys.Shift | Keys.Add:
				case Keys.Enter:
					if (tabControl.SelectedIndex == TAB_ROT && ControlRoute.RoutePanel.Focused)
					{
						e.SuppressKeyPress = true;
						ControlRoute.RoutePanel.Navigate(e.KeyData);
					}
					break;

				default:
					MenuManager.ViewerKeyDown(e); // NOTE: this can suppress the key
					break;
			}

			switch (tabControl.SelectedIndex)
			{
				case TAB_TOP:
					if (slot != PartType.Invalid)
					{
						e.SuppressKeyPress = true;
						var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
						ControlTop.QuadrantPanel.doMouseDown(args, slot);
					}
					break;

				case TAB_ROT:
					base.OnKeyDown(e);
					break;
			}
		}

		/// <summary>
		/// Handles the FormClosing event. Ensures that the TestPartslots dialog
		/// gets closed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason)
				&& TopView._finfobox != null && !TopView._finfobox.IsDisposed)
			{
				TopView._finfobox.Close();
				TopView._finfobox = null;
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (MainViewF.Optionables.StartTopRoutePage > 1)
			{
				MainViewF.Optionables.StartTopRoutePage = tabControl.SelectedIndex + 2;

				var foptions = MainViewF._foptions;
				if (foptions != null)
				{
					var grid = (foptions as OptionsForm).propertyGrid;
					grid.Refresh(); // yes refresh the grid even if it's hidden.
				}
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Focuses a tabpage when the MainView option "StartTopRoutePage"
		/// changes.
		/// </summary>
		/// <param name="page"></param>
		internal void SelectTabpage(int page)
		{
			tabControl.SelectedIndex = page;
		}
		#endregion Methods
	}
}
