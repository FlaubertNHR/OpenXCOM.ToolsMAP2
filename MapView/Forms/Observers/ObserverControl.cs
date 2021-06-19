using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by <see cref="TileView"/>, <see cref="TopView"/>,
	/// <see cref="RouteView"/>.
	/// </summary>
	internal class ObserverControl
		:
			UserControl,
			IObserver
	{
		#region IObserver requirements
		/// <summary>
		/// The currently loaded <see cref="XCom.MapFile"/>.
		/// </summary>
		/// <remarks>Is overridden by <see cref="TileView.MapFile">TileView.MapFile</see>
		/// and <see cref="RouteView.MapFile">RouteView.MapFile</see>.
		/// <see cref="TopView"/> uses this MapFile.</remarks>
		[Browsable(false)]
		public virtual MapFile MapFile
		{ get; set; }

		/// <summary>
		/// Satisfies <c><see cref="IObserver"/></c>. Is overridden only by
		/// <c><see cref="RouteView.OnLocationSelectedObserver()">RouteView.OnLocationSelectedObserver()</see></c>.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLocationSelectedObserver(LocationSelectedArgs args)
		{}

		/// <summary>
		/// Satisfies <c><see cref="IObserver"/></c>. Is overridden only by
		/// <c><see cref="RouteView.OnLevelSelectedObserver()">RouteView.OnLevelSelectedObserver()</see></c>.
		/// and
		/// <c><see cref="TopView.OnLevelSelectedObserver()">TopView.OnLevelSelectedObserver()</see></c>.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLevelSelectedObserver(LevelSelectedArgs args)
		{}
		#endregion IObserver requirements


		#region Properties
		private readonly List<IObserver> _observerChildControls =
					 new List<IObserver>();
		[Browsable(false)]
		public List<IObserver> ObserverChildControls
		{
			get { return _observerChildControls; }
		}

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
		internal protected virtual void LoadControlDefaultOptions()
		{}
		#endregion Methods (pseudo-abstract)


		#region Events (override)
		/// <summary>
		/// Scrolls the z-axis for <c><see cref="TopView"/></c> and
		/// <c><see cref="RouteView"/></c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks><c><see cref="TileView"/></c> overrides this override to do
		/// nothing.</remarks>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			int delta;
			if (MainViewF.Optionables.InvertMousewheel)
				delta = -e.Delta;
			else
				delta =  e.Delta;

			int dir = MapFile.LEVEL_no;
			if      (delta < 0) dir = MapFile.LEVEL_Up;
			else if (delta > 0) dir = MapFile.LEVEL_Dn;
			MapFile.ChangeLevel(dir);

			ObserverManager.ToolFactory.EnableLevelers(MapFile.Level, MapFile.Levs);
		}
		#endregion Events (override)


		#region Methods
/*		/// <summary>
		/// Simulates a mousewheel event for keyboard navigation by
		/// RouteControlParent.Navigate().
		/// </summary>
		/// <param name="e"></param>
		internal void doMousewheel(MouseEventArgs e)
		{
			OnMouseWheel(e);
		} */
		#endregion Methods
	}
}
