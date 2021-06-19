using System;
using System.Windows.Forms;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by <see cref="TileView"/>, <see cref="TopView"/>,
	/// <see cref="RouteView"/>.
	/// </summary>
	internal class ObserverControl
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


		#region Methods (pseudo-abstract)
		/// <summary>
		/// Loads default options for <c><see cref="TileView"/></c>,
		/// <c><see cref="TopView"/></c>, and <c><see cref="RouteView"/></c>.
		/// </summary>
		/// <remarks>Do not make this abstract because the controls' designers
		/// will foff instead of displaying their controls.</remarks>
		internal virtual void LoadControlDefaultOptions()
		{}
		#endregion Methods (pseudo-abstract)
	}
}
