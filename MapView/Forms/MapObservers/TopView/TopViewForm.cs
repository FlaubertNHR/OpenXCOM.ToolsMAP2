using System;
using System.Windows.Forms;

using MapView.Forms.MainWindow;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed partial class TopViewForm
		:
			Form,
			IMapObserverProvider
	{
		#region Properties
		internal TopView Control
		{
			get { return TopViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return TopViewControl; }
		}
		#endregion


		#region cTor
		internal TopViewForm()
		{
			InitializeComponent();
		}
		#endregion


		#region Events (override)
		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - opens/closes Options on [Ctrl+o] event.
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, RouteViewForm, TopRouteViewForm
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((e.Modifiers & Keys.Control) == Keys.Control
				&& e.KeyCode == Keys.O)
			{
				Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
				e.SuppressKeyPress = true;
			}
			else
				MainMenusManager.ViewerKeyDown(e); // note this can also suppress the key

			if (!e.SuppressKeyPress && Control.TopViewPanel.Focused)
			{
				switch (e.KeyCode)
				{
					case Keys.Add:
					case Keys.Subtract:
					case Keys.PageDown:
					case Keys.PageUp:
					case Keys.Home:
					case Keys.End:
						MainViewUnderlay.Instance.MainViewOverlay.Navigate(e.KeyData);
						e.SuppressKeyPress = true; // I wonder if this suppresses only KeyDown or other keyed eventtypes also.
						break;
				}
			}
//			base.OnKeyDown(e);
		}

		/// <summary>
		/// Stops keys that shall be used for navigating the tiles from doing
		/// anything stupid instead.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case (Keys.Shift | Keys.Left):
				case (Keys.Shift | Keys.Right):
				case (Keys.Shift | Keys.Up):
				case (Keys.Shift | Keys.Down):
					MainViewUnderlay.Instance.MainViewOverlay.Navigate(keyData);
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}


		/// <summary>
		/// Handles form closing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			WindowState = FormWindowState.Normal; // else causes probls when opening a viewer that was closed while maximized.
			base.OnFormClosing(e);
		}
		#endregion Events (override)
	}
}
