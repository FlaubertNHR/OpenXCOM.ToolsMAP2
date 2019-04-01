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
		/// - closes/hides this viewer on [F6] event.
		/// - opens/closes Options on [Ctrl+o] event.
		/// @note Requires 'KeyPreview' true.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			int it = -1;

			switch (e.KeyCode)
			{
				case Keys.F5: it = 0; break;
				case Keys.F6: it = 2; break;
				case Keys.F7: it = 3; break;
				case Keys.F8: it = 4; break;

				case Keys.O:
					if ((e.Modifiers & Keys.Control) == Keys.Control)
					{
						Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
					}
					else
						goto default;
					break;

				default:
					base.OnKeyDown(e);
					break;
			}

			if (it != -1)
			{
				MainMenusManager.OnMenuItemClick(
											MainMenusManager.ViewerIts.MenuItems[it],
											EventArgs.Empty);
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			WindowState = FormWindowState.Normal; // else causes probls when opening a viewer that was closed while maximized.
			base.OnFormClosing(e);
		}
		#endregion Events (override)
	}
}
