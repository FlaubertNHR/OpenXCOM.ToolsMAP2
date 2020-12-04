using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	internal sealed partial class TopViewForm
		:
			Form,
			IMapObserverProvider
	{
		#region Fields
		private TopView TopViewControl;
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets TopViewControl as a child of MapObserverControl.
		/// </summary>
		internal TopView Control
		{
			get { return TopViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl ObserverControl
		{
			get { return TopViewControl; }
		}
		#endregion Properties


		#region cTor
		internal TopViewForm()
		{
			InitializeComponent();
			InitializeTopView();
		}

		private void InitializeTopView()
		{
			TopViewControl = new TopView();

			TopViewControl.Name     = "TopViewControl";
			TopViewControl.Location = new Point(0, 0);
			TopViewControl.Size     = new Size(632, 454);
			TopViewControl.Dock     = DockStyle.Fill;
			TopViewControl.TabIndex = 1;
			TopViewControl.Tag      = "TOP";

			Controls.Add(TopViewControl);
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

			TopViewControl.TopPanel.ClearSelectorLozenge(); // when TestPartslots is closed the selector-lozenge can glitch.
			TopViewControl.TopPanel.Focus();

//			base.OnActivated(e);
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
			if (Control.TopPanel.Focused)
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
		/// Handles KeyDown events at the form level.
		/// - [Esc] focuses the panel else clears the current selection lozenge
		/// - opens/closes Options on [Ctrl+o] event
		/// - checks for and if so processes a viewer F-key
		/// - passes edit-keys to the TopView control's panel's Navigate()
		///   funct
		/// - selects a quadrant
		/// @note Requires 'KeyPreview' true.
		/// @note See also TileViewForm, RouteViewForm, TopRouteViewForm
		/// @note Edit/Save keys are handled by 'TopPanel.OnKeyDown()'.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			PartType slot = PartType.Invalid;

			switch (e.KeyData)
			{
				case Keys.Escape:
					if (!Control.TopPanel.Focused)
					{
						e.SuppressKeyPress = true;
						Control.TopPanel.Focus();
					}
					else
						MainViewOverlay.that.Edit(e);
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
					if (Control.TopPanel.Focused)
					{
						e.SuppressKeyPress = true;
						MainViewOverlay.that.Navigate(e.KeyData, true);
					}
					break;

				case Keys.D1: slot = PartType.Floor;   break;
				case Keys.D2: slot = PartType.West;    break;
				case Keys.D3: slot = PartType.North;   break;
				case Keys.D4: slot = PartType.Content; break;

				default:
					MenuManager.ViewerKeyDown(e); // NOTE: this can suppress the key
					break;
			}

			if (slot != PartType.Invalid)
			{
				e.SuppressKeyPress = true;
				var args = new MouseEventArgs(MouseButtons.Left, 1, 0,0, 0);
				Control.QuadrantPanel.doMouseDown(args, slot);
			}
//			base.OnKeyDown(e);
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
	}
}
