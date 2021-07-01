using System;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// This is the form that contains <c><see cref="RouteView"/></c>.
	/// </summary>
	/// <remarks>This is instantiated by
	/// <c><see cref="ObserverManager.CreateObservers">ObserverManager.CreateObservers</see></c>
	/// and closed by
	/// <c><see cref="ObserverManager.CloseObservers">ObserverManager.CloseObservers</see></c>.</remarks>
	internal sealed partial class RouteViewForm
		:
			Form
	{
		#region Properties
		/// <summary>
		/// Gets/Sets <c><see cref="RouteView"/></c>.
		/// </summary>
		internal RouteView Control
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal RouteViewForm()
		{
			InitializeComponent();

			Control = new RouteView();

			Control.Name     = "RouteViewControl";
			Control.Dock     = DockStyle.Fill;
			Control.TabIndex = 0;
			Control.Tag      = "ROUTE";

			Controls.Add(Control);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Sets up the link-connector when this form is shown.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			Control.ActivateConnector();

//			base.OnShown(e);
		}

		/// <summary>
		/// Fires when the form is activated. Maintains the position of this
		/// form in the z-order List.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

//			base.OnActivated(e);
		}

		/// <summary>
		/// Handles a so-called command-key at the form level. Stops keys that
		/// shall be used for navigating the tiles from doing anything stupid
		/// instead.
		/// - passes the arrow-keys to the RouteView control's panel's
		///   Navigate() funct
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (Control.RouteControl.Focused)
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
						Control.RouteControl.Navigate(keyData);
						return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - [Esc] focuses the panel
		/// - opens/closes Options on [Ctrl+o] event
		/// - checks for and if so processes a viewer F-key
		/// - passes edit-keys to the RouteView control's panel's Navigate()
		///   funct
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, TopViewForm, TopRouteViewForm
		/// @note Edit/Save keys are handled by 'RouteView.OnRoutePanelKeyDown()'.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					e.SuppressKeyPress = true;
					if (Control.RouteControl.Focused)
					{
						RouteView.NodeSelected = null;
						RouteView.Invalidator();
					}
					else
						Control.RouteControl.Focus();
					break;

				case Keys.Control | Keys.O:
					e.SuppressKeyPress = true;
					Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
					break;

				case Keys.Control | Keys.Q:
					e.SuppressKeyPress = true;
					MainViewF.that.OnQuitClick(null, EventArgs.Empty);
					break;

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
				case Keys.Enter:
					if (Control.RouteControl.Focused)
					{
						e.SuppressKeyPress = true;
						Control.RouteControl.Navigate(e.KeyData);
					}
					break;

				default:
					ViewersMenuManager.ViewerKeyDown(e); // NOTE: this can suppress the key
					break;
			}
			base.OnKeyDown(e);
		}

		/// <summary>
		/// Handles the FormClosing event. TODO: Close subsidiary dialogs here.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (RouteView.RoutesInfo != null)
					RouteView.RoutesInfo.Close();

				if (MainViewF.Quit)
				{
					Logfile.Log("RouteViewForm.OnFormClosing()");
					RegistryInfo.UpdateRegistry(this);
					Control.RouteControl.DisposeControl();
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
	}
}
