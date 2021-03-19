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
	internal abstract class ObserverControl
		:
			UserControl,
			IObserver
	{
		#region IObserver requirements
		private readonly Dictionary<string, IObserver> _panels =
					 new Dictionary<string, IObserver>();
		[Browsable(false)]
		public Dictionary<string, IObserver> ObserverPanels
		{
			get { return _panels; }
		}

		private MapFile _file;
		/// <summary>
		/// The currently loaded <see cref="XCom.Mapfile"/>.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual MapFile MapFile
		{
			get { return _file; }
			set { _file = value; Refresh(); }
		}

		/// <summary>
		/// Refreshes <see cref="TileView"/> and <see cref="TopView"/> when a
		/// location is selected.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>Satisfies IObserver. Is overridden only by
		/// <see cref="RouteView.OnLocationSelectedObserver">RouteView.OnLocationSelectedObserver</see>.</remarks>
		public virtual void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{
			//DSShared.LogFile.WriteLine("ObserverControl.OnLocationSelectedObserver() DOES THIS EVER DO ANYTHING.");
			// TODO: YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
			Refresh();
		}

		/// <summary>
		/// Refreshes <see cref="TileView"/> and <see cref="TopView"/> when a
		/// level is selected.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>Satisfies IObserver. Is overridden only by
		/// <see cref="RouteView.OnLevelSelectedObserver">RouteView.OnLevelSelectedObserver</see>.</remarks>
		public virtual void OnLevelSelectedObserver(LevelSelectedEventArgs args)
		{
			//DSShared.LogFile.WriteLine("ObserverControl.OnLevelSelectedObserver() DOES THIS EVER DO ANYTHING.");
			// TODO: YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
			Refresh();
		}
		#endregion IObserver requirements


		#region Properties
		private Options _options = new Options();
		internal Options Options
		{
			get { return _options; }
			set { _options = value; }
		}
		#endregion Properties


		#region Methods (abstract)
		internal protected abstract void LoadControlDefaultOptions();
		#endregion Methods (abstract)


		#region Events (override)
		/// <summary>
		/// Scrolls the z-axis for <see cref="TopView"/> and <see cref="RouteView"/>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks><see cref="TileView"/> overrides this override to do
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

			ObserverManager.ToolFactory.EnableLevelers(MapFile.Level, MapFile.MapSize.Levs);
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
