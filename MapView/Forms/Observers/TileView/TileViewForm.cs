using System;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// This is the form that contains <c><see cref="TileView"/></c>.
	/// </summary>
	/// <remarks>This is instantiated by
	/// <c><see cref="ObserverManager.CreateObservers">ObserverManager.CreateObservers</see></c>
	/// and closed by
	/// <c><see cref="ObserverManager.CloseObservers">ObserverManager.CloseObservers</see></c>.</remarks>
	internal sealed partial class TileViewForm
		:
			Form
	{
		#region Properties
		/// <summary>
		/// Gets/Sets <c><see cref="TileView"/></c>.
		/// </summary>
		internal TileView Control
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal TileViewForm()
		{
			InitializeComponent();

			Control = new TileView();
			Controls.Add(Control);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Fires when the form is activated. Maintains the position of this
		/// form in the z-order List and focuses the selected panel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

			Control.GetSelectedPanel().Focus();

//			base.OnActivated(e);
		}

		/// <summary>
		/// Handles a so-called command-key at the form level. Stops keys that
		/// shall be used for navigating the tiles from doing anything stupid
		/// instead.
		/// - passes the arrow-keys to the TileView control's current panel's
		///   Navigate() funct
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			TilePanel panel = Control.GetSelectedPanel();
			if (panel.Focused)
			{
				switch (keyData)
				{
					case Keys.Left:
					case Keys.Right:
					case Keys.Up:
					case Keys.Down:
						panel.Navigate(keyData);
						return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles KeyDown events at the form level.
		/// <list type="bullet">
		/// <item>[Esc] focuses the current panel</item>
		/// <item>opens/closes Options on [Ctrl+o] event</item>
		/// <item>opens/focuses Colorhelp on [Ctrl+h] event</item>
		/// <item>checks for and if so processes a viewer F-key</item>
		/// <item>passes edit-keys to the TileView control's current panel's
		/// Navigate() funct</item>
		/// </list>
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires 'KeyPreview' true. See also <c><see cref="TopViewForm"/></c>,
		/// <c><see cref="RouteViewForm"/></c>, <c><see cref="TopRouteViewForm"/></c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					e.SuppressKeyPress = true;
					Control.GetSelectedPanel().Focus();
					break;

				case Keys.Control | Keys.O:
					e.SuppressKeyPress = true;
					Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
					break;

				case Keys.Control | Keys.H:
					e.SuppressKeyPress = true;
					MainViewF.that.OnColorsClick(null, EventArgs.Empty);
					break;

				case Keys.Control | Keys.Q:
					e.SuppressKeyPress = true;
					MainViewF.that.OnQuitClick(null, EventArgs.Empty);
					break;

				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End:
				case Keys.Control | Keys.Home:
				case Keys.Control | Keys.End:
				{
					TilePanel panel = Control.GetSelectedPanel();
					if (panel.Focused)
					{
						e.SuppressKeyPress = true;
						panel.Navigate(e.KeyData);
					}
					break;
				}

				default:
					ViewersMenuManager.ViewerKeyDown(e); // NOTE: this can suppress the key
					break;
			}
			base.OnKeyDown(e);
		}

		/// <summary>
		/// Handles the FormClosing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (MainViewF.Quit)
				{
					Logfile.Log("TileViewForm.OnFormClosing()");
					RegistryInfo.UpdateRegistry(this);
					Control.DisposeObserver();
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
