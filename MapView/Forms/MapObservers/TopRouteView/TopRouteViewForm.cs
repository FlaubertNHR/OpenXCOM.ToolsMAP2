using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TopViews;

using XCom;


namespace MapView.Forms.MapObservers.TileViews // y, "TileViews" thanks for knifing the concept of namespaces in the butt.
{
	internal sealed partial class TopRouteViewForm
		:
			Form
	{
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

			if (tabControl.SelectedIndex == 0)
				TopViewControl.TopPanel.Focus();

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
				case 0: // Top
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

				case 1: // Route
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
			if (e.KeyCode == Keys.Escape)
			{
				switch (tabControl.SelectedIndex)
				{
					case 0: // Top
						if (!ControlTop.TopPanel.Focused)
						{
							e.SuppressKeyPress = true;
							ControlTop.TopPanel.Focus();
						}
						else
							MainViewOverlay.that.Edit(e);
						break;

					case 1: // Route
						e.SuppressKeyPress = true;
						if (ControlRoute.RoutePanel.Focused)
						{
							RouteView.NodeSelected = null;

							ViewerFormsManager.RouteView   .Control     .RoutePanel.Invalidate();
							ViewerFormsManager.TopRouteView.ControlRoute.RoutePanel.Invalidate();

							ViewerFormsManager.RouteView   .Control     .UpdateNodeInformation();
							ViewerFormsManager.TopRouteView.ControlRoute.UpdateNodeInformation();
						}
						else
							ControlRoute.RoutePanel.Focus();
						break;
				}
			}
			else if (e.KeyCode == Keys.O
				&& (e.Modifiers & Keys.Control) == Keys.Control)
			{
				e.SuppressKeyPress = true;
				switch (tabControl.SelectedIndex)
				{
					case 0: // Top
						ControlTop.OnOptionsClick(ControlTop.GetOptionsButton(), EventArgs.Empty);
						break;
					case 1: // Route
						ControlRoute.OnOptionsClick(ControlRoute.GetOptionsButton(), EventArgs.Empty);
						break;
				}
			}
			else if (!MainMenusManager.ViewerKeyDown(e)) // NOTE: this can suppress the key
			{
				if (tabControl.SelectedIndex == 0) // Top
				{
					QuadrantType quadtype = QuadrantType.None;
					switch (e.KeyCode)
					{
						case Keys.D1: quadtype = QuadrantType.Floor;   break;
						case Keys.D2: quadtype = QuadrantType.West;    break;
						case Keys.D3: quadtype = QuadrantType.North;   break;
						case Keys.D4: quadtype = QuadrantType.Content; break;
					}

					if (quadtype != QuadrantType.None)
					{
						e.SuppressKeyPress = true;
						var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
						ControlTop.QuadrantPanel.doMouseDown(args, quadtype);
					}
					else if (ControlTop.TopPanel.Focused)
					{
						switch (e.KeyCode)
						{
							case Keys.Add:
							case Keys.Subtract:
							case Keys.PageDown:
							case Keys.PageUp:
							case Keys.Home:
							case Keys.End:
								e.SuppressKeyPress = true;
								MainViewOverlay.that.Navigate(e.KeyData, true);
								break;
						}
					}
				}
				else if (ControlRoute.RoutePanel.Focused) // Route
				{
					switch (e.KeyCode)
					{
						case Keys.Add:
						case Keys.Subtract:
						case Keys.PageDown:
						case Keys.PageUp:
						case Keys.Home:
						case Keys.End:
						case Keys.Enter:
							e.SuppressKeyPress = true;
							ControlRoute.RoutePanel.Navigate(e.KeyData);
							break;
					}
				}
			}

			if (tabControl.SelectedIndex == 1) // Route
				base.OnKeyDown(e);
		}
		#endregion Events (override)
	}
}
