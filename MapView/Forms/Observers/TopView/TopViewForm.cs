using System;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// This is the form that contains <c><see cref="TopView"/></c>.
	/// </summary>
	/// <remarks>This is instantiated by
	/// <c><see cref="ObserverManager.CreateObservers">ObserverManager.CreateObservers</see></c>
	/// and closed by
	/// <c><see cref="ObserverManager.CloseObservers">ObserverManager.CloseObservers</see></c>.</remarks>
	internal sealed partial class TopViewForm
		:
			Form
	{
		#region Properties
		/// <summary>
		/// Gets/Sets <c><see cref="TopView"/></c>.
		/// </summary>
		internal TopView Control
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal TopViewForm()
		{
			InitializeComponent();

			Control = new TopView();

			Control.Name     = "TopViewControl";
			Control.Dock     = DockStyle.Fill;
			Control.TabIndex = 0;

			Controls.Add(Control);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Fires when the form is activated. Maintains the position of this
		/// form in the z-order List and focuses the panel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

			Control.TopControl.ClearSelectorLozenge(); // when TestPartslots is closed the selector-lozenge can glitch.

			// note that if user clicks on TopControl to activate this
			// TopViewForm then TopControl.WndProc() will have already focused
			// TopControl ->
			Control.TopControl.Focus();
		}

		/// <summary>
		/// Handles a so-called command-key at this <c>Form</c> level. Stops
		/// keys that shall be used for navigating the tiles from doing anything
		/// stupid instead.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		/// <remarks>Passes the arrow-keys to
		/// <c><see cref="MainViewOverlay.Navigate()">MainViewOverlay.Navigate()</see></c>.</remarks>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (Control.TopControl.Focused)
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
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles <c>KeyDown</c> events at this <c>Form</c> level.
		/// <list type="bullet">
		/// <item><c>[Esc]</c> - focuses the panel else clears the current
		/// selection lozenge</item>
		/// <item><c>[Ctrl+o]</c> - opens/closes Options</item>
		/// <item><c>F-key</c> - checks for and processes a viewer</item>
		/// <item>passes non-arrow navigate-keys to
		/// <c><see cref="MainViewOverlay.Navigate()">MainViewOverlay.Navigate()</see></c>
		/// - the arrow-keys are passed to the same function by
		/// <c><see cref="ProcessCmdKey()">ProcessCmdKey()</see></c> instead of
		/// here ... for no special reason perhaps</item>
		/// <item>selects a quadrant</item>
		/// </list>
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c> <c>true</c>.
		/// <br/><br/>
		/// See also <c><see cref="TileViewForm"/></c> /
		/// <c><see cref="RouteViewForm"/></c> /
		/// <c><see cref="TopRouteViewForm"/></c>.
		/// <br/><br/>
		/// Edit keys are handled by
		/// <c><see cref="MainViewOverlay.Edit()">MainViewOverlay.Edit()</see></c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			PartType quadrant = PartType.Invalid;

			switch (e.KeyData)
			{
				case Keys.Escape:
					if (!Control.TopControl.Focused)
					{
						e.SuppressKeyPress = true;
						Control.TopControl.Focus();
					}
					else
						MainViewOverlay.that.Edit(e);
					break;

				case Keys.O | Keys.Control:
					e.SuppressKeyPress = true;
					Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
					break;

				case Keys.Q | Keys.Control:
					e.SuppressKeyPress = true;
					MainViewF.that.OnQuitClick(null, EventArgs.Empty);
					break;

				case Keys.Add:
				case Keys.Subtract:
				case Keys.Home:
				case Keys.Home | Keys.Shift:
				case Keys.End:
				case Keys.End | Keys.Shift:
				case Keys.PageUp:
				case Keys.PageUp | Keys.Shift:
				case Keys.PageDown:
				case Keys.PageDown | Keys.Shift:
					if (Control.TopControl.Focused)
					{
						e.SuppressKeyPress = true;
						MainViewOverlay.that.Navigate(e.KeyData, true);
					}
					break;

				case Keys.D1: quadrant = PartType.Floor;   break;
				case Keys.D2: quadrant = PartType.West;    break;
				case Keys.D3: quadrant = PartType.North;   break;
				case Keys.D4: quadrant = PartType.Content; break;

				default:
					ViewersMenuManager.ViewerKeyDown(e); // NOTE: this can suppress the key
					break;
			}

			if (quadrant != PartType.Invalid)
			{
				e.SuppressKeyPress = true;
				var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
				Control.QuadrantControl.doMouseDown(args, quadrant);
			}
		}

		/// <summary>
		/// Handles the <c>FormClosing</c> event. Ensures that the TestPartslots
		/// dialog gets closed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				TopView.CloseTestPartslotsDialog();

				if (MainViewF.Quit)
				{
					//Logfile.Log("TopViewForm.OnFormClosing()");
					RegistryInfo.UpdateRegistry(this);
					ObserverManager.ToolFactory.DisposeTopviewTools(); // disposes tools for TopRouteView also. but might not be req'd
					Control.TopControl.DisposeControl();
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
