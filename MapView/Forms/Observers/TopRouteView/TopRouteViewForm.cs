using System;
using System.Drawing;
using System.Windows.Forms;

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

		#region Fields
		private TopView   TopViewControl;
		private RouteView RouteViewControl;
		#endregion Fields


		#region Properties
		internal TopView ControlTop
		{
			get { return TopViewControl; }
		}

		internal RouteView ControlRoute
		{
			get { return RouteViewControl; }
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
			TopViewControl   = new TopView();
			RouteViewControl = new RouteView();

			TopViewControl.Name       = "TopViewControl";
			TopViewControl.Location   = new Point(3, 3);
			TopViewControl.Size       = new Size(618, 423);
			TopViewControl.Dock       = DockStyle.Fill;
			TopViewControl.TabIndex   = 0;
			TopViewControl.Tag        = "TOPROUTE";

			RouteViewControl.Name     = "RouteViewControl";
			RouteViewControl.Location = new Point(3, 3);
			RouteViewControl.Size     = new Size(618, 423);
			RouteViewControl.Dock     = DockStyle.Fill;
			RouteViewControl.TabIndex = 0;
			RouteViewControl.Tag      = "TOPROUTE";

			tp_Top  .Controls.Add(TopViewControl);
			tp_Route.Controls.Add(RouteViewControl);
		}
		#endregion cTor


		#region Events (override)
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
				TopViewControl.TopPanel.ClearSelectorLozenge(); // when TestPartslots is closed the selector-lozenge can glitch.
				TopViewControl.TopPanel.Focus();
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
			QuadrantType quad = QuadrantType.None;

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
	
								ObserverManager.RouteView   .Control     .RoutePanel.Invalidate();
								ObserverManager.TopRouteView.ControlRoute.RoutePanel.Invalidate();
	
								ObserverManager.RouteView   .Control     .UpdateNodeInformation();
								ObserverManager.TopRouteView.ControlRoute.UpdateNodeInformation();
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

				case Keys.D1: quad = QuadrantType.Floor;   break;
				case Keys.D2: quad = QuadrantType.West;    break;
				case Keys.D3: quad = QuadrantType.North;   break;
				case Keys.D4: quad = QuadrantType.Content; break;

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
					if (quad != QuadrantType.None)
					{
						e.SuppressKeyPress = true;
						var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
						ControlTop.QuadrantPanel.doMouseDown(args, quad);
					}
					break;

				case TAB_ROT:
					base.OnKeyDown(e);
					break;
			}
		}
		#endregion Events (override)
	}
}
