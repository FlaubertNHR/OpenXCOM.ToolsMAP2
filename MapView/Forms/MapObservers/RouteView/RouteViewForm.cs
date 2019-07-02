using System;
using System.Windows.Forms;

using MapView.Forms.MainWindow;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed partial class RouteViewForm
		:
			Form,
			IMapObserverProvider
	{
		#region Properties
		/// <summary>
		/// Gets 'RouteViewControl' as a child of 'MapObserverControl'.
		/// </summary>
		internal RouteView Control
		{
			get { return RouteViewControl; }
		}

		/// <summary>
		/// Satisfies 'IMapObserverProvider'.
		/// </summary>
		public MapObserverControl ObserverControl
		{
			get { return RouteViewControl; }
		}
		#endregion Properties


		#region cTor
		internal RouteViewForm()
		{
			InitializeComponent();
		}
		#endregion cTor


		#region Events (override)
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
			if (Control.RoutePanel.Focused)
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
						Control.RoutePanel.Navigate(keyData);
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
			if (e.KeyCode == Keys.Escape)
			{
				e.SuppressKeyPress = true;
				if (Control.RoutePanel.Focused)
				{
					RouteView.NodeSelected = null;

					ViewerFormsManager.RouteView   .Control     .RoutePanel.Invalidate();
					ViewerFormsManager.TopRouteView.ControlRoute.RoutePanel.Invalidate();

					ViewerFormsManager.RouteView   .Control     .UpdateNodeInformation();
					ViewerFormsManager.TopRouteView.ControlRoute.UpdateNodeInformation();
				}
				else
					Control.RoutePanel.Focus();
			}
			else if (e.KeyCode == Keys.O
				&& (e.Modifiers & Keys.Control) == Keys.Control)
			{
				e.SuppressKeyPress = true;
				Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
			}
			else if (!MainMenusManager.ViewerKeyDown(e) // NOTE: this can suppress the key
				&& Control.RoutePanel.Focused)
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
						Control.RoutePanel.Navigate(e.KeyData);
						break;
				}
			}
			base.OnKeyDown(e);
		}
		#endregion Events (override)
	}
}
