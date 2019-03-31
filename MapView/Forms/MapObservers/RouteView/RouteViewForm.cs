using System;
using System.Windows.Forms;


namespace MapView.Forms.MapObservers.RouteViews
{
	internal sealed partial class RouteViewForm
		:
			Form,
			IMapObserverProvider
	{
		#region Properties
		internal RouteView Control
		{
			get { return RouteViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl0 ObserverControl0
		{
			get { return RouteViewControl; }
		}
		#endregion


		#region cTor
		internal RouteViewForm()
		{
			InitializeComponent();
		}
		#endregion


		#region Events (override)
		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - closes/hides this viewer on [F7] event.
		/// - opens/closes Options on [Ctrl+o] event.
		/// @note Requires 'KeyPreview' true.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F7:
					Close();
					break;

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
		}
		#endregion Events (override)
	}
}
