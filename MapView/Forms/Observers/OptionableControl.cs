using System;
using System.Windows.Forms;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by <c><see cref="TileView"/></c>, <c><see cref="TopView"/></c>,
	/// <c><see cref="RouteView"/></c> for their
	/// <c><see cref="MapView.Options"/></c>.
	/// </summary>
	internal class OptionableControl
		:
			UserControl
	{
		#region Properties
		private Options _options = new Options();
		internal Options Options
		{
			get { return _options; }
			set { _options = value; }
		}
		#endregion Properties
	}
}
