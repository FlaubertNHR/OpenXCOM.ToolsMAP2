using System;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// This is the <c>Form</c> that contains <c><see cref="RouteView"/></c>.
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
		{ get; private set; }
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

			Controls.Add(Control);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Sets up the link-connector when this <c>Form</c> is shown.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			Control.ActivateConnector();
		}

		/// <summary>
		/// Fires when this <c>Form</c> is activated. Maintains the position of
		/// this <c>Form</c> in the z-order List.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}

		/// <summary>
		/// Handles a so-called command-key at the <c>Form</c> level. Stops keys
		/// that shall be used for navigating the tiles from doing anything
		/// stupid instead.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		/// <remarks>Passes the arrow-keys to
		/// <c><see cref="RouteControlParent.Navigate()">RouteControlParent.Navigate()</see></c>.</remarks>
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
		/// Handles <c>KeyDown</c> events at the <c>Form</c> level.
		/// <list type="bullet">
		/// <item><c>[Esc]</c> - focuses the panel</item>
		/// <item><c>[Ctrl+o]</c> - opens/closes Options</item>
		/// <item><c>F-key</c> - checks for and processes a viewer</item>
		/// <item>passes non-arrow navigate-keys to
		/// <c><see cref="RouteControlParent.Navigate()">RouteControlParent.Navigate()</see></c>
		/// - the arrow-keys are passed to the same function by
		/// <c><see cref="ProcessCmdKey()">ProcessCmdKey()</see></c> instead of
		/// here ... for no special reason perhaps</item>
		/// </list>
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c> <c>true</c>.
		/// <br/><br/>
		/// See also <c><see cref="TileViewForm"/></c> /
		/// <c><see cref="TopViewForm"/></c> /
		/// <c><see cref="TopRouteViewForm"/></c>.
		/// <br/><br/>
		/// Edit/Save keys are handled by
		/// <c><see cref="RouteView">RouteView</see>.OnRouteControlKeyDown()</c>.</remarks>
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

				case Keys.Enter:
				case Keys.Add:
				case Keys.Subtract:
				case Keys.Home:
				case Keys.End:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Shift | Keys.Add:
				case Keys.Shift | Keys.Subtract:
				case Keys.Shift | Keys.Home:
				case Keys.Shift | Keys.End:
				case Keys.Shift | Keys.PageUp:
				case Keys.Shift | Keys.PageDown:
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
		/// Handles the <c>FormClosing</c> event. Ensures that
		/// <c><see cref="SpawnInfo"/></c> gets closed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (RouteView.SpawnInfo != null)
					RouteView.SpawnInfo.Close();

				if (MainViewF.Quit)
				{
					//Logfile.Log("RouteViewForm.OnFormClosing()");
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
