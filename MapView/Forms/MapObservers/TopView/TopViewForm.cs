using System;
using System.Windows.Forms;

using MapView.Forms.MainWindow;

using XCom;


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
		/// - [Esc] focuses the panel else clears the current selection lozenge
		/// - opens/closes Options on [Ctrl+o] event
		/// - checks for and if so processes a viewer F-key
		/// - passes edit-keys to the TopView control's panel's Navigate()
		///   funct
		/// - selects a quadrant
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, RouteViewForm, TopRouteViewForm
		/// @note Edit/Save keys are handled by 'TopViewPanelParent.OnKeyDown()'.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				if (!Control.TopViewPanel.Focused)
				{
					e.SuppressKeyPress = true;
					Control.TopViewPanel.Focus();
				}
				else
					MainViewUnderlay.Instance.MainViewOverlay.Edit(e);
			}
			else if (e.KeyCode == Keys.O
				&& (e.Modifiers & Keys.Control) == Keys.Control)
			{
				e.SuppressKeyPress = true;
				Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
			}
			else if (!MainMenusManager.ViewerKeyDown(e)) // NOTE: this can suppress the key
			{
				QuadrantType quadType = QuadrantType.None;
				switch (e.KeyCode)
				{
					case Keys.D1: quadType = QuadrantType.Floor;   break;
					case Keys.D2: quadType = QuadrantType.West;    break;
					case Keys.D3: quadType = QuadrantType.North;   break;
					case Keys.D4: quadType = QuadrantType.Content; break;
				}

				if (quadType != QuadrantType.None)
				{
					e.SuppressKeyPress = true;
					var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
					Control.QuadrantsPanel.ForceMouseDown(args, quadType);
				}
				else if (Control.TopViewPanel.Focused)
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
							MainViewUnderlay.Instance.MainViewOverlay.Navigate(e.KeyData);
							break;
					}
				}
			}
//			base.OnKeyDown(e);
		}

		/// <summary>
		/// Handles a so-called command-key at the form level. Stops keys that
		/// shall be used for navigating the tiles from doing anything stupid
		/// instead.
		/// - passes the arrow-keys to the TopView control's panel's Navigate()
		///   funct
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (Control.TopViewPanel.Focused)
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
						MainViewUnderlay.Instance.MainViewOverlay.Navigate(keyData);
						return true;
				}
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
