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
		/// <summary>
		/// The currently loaded <see cref="XCom.MapFile"/>.
		/// </summary>
		/// <remarks>Is overridden by <see cref="TileView.MapFile">TileView.MapFile</see>
		/// and <see cref="RouteView.MapFile">RouteView.MapFile</see>.
		/// <see cref="TopView"/> uses this MapFile.</remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual MapFile MapFile
		{ get; set; }


		private readonly Dictionary<string, IObserver> _panels =
					 new Dictionary<string, IObserver>();
		[Browsable(false)]
		public Dictionary<string, IObserver> ObserverControls
		{
			get { return _panels; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>Satisfies IObserver. Is overridden only by
		/// <see cref="RouteView.OnLocationSelectedObserver">RouteView.OnLocationSelectedObserver</see>.</remarks>
		public virtual void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{
			//DSShared.LogFile.WriteLine("ObserverControl.OnLocationSelectedObserver() DOES THIS EVER DO ANYTHING.");
			// YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
//			Refresh();
			// But Refresh() doesn't appear to be needed.
			// why would you need to refresh TileView ...
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>Satisfies IObserver. Is overridden only by
		/// <see cref="RouteView.OnLevelSelectedObserver">RouteView.OnLevelSelectedObserver</see>
		/// and <see cref="TopView.OnLevelSelectedObserver">TopView.OnLevelSelectedObserver</see>.</remarks>
		public virtual void OnLevelSelectedObserver(LevelSelectedEventArgs args)
		{
			//DSShared.LogFile.WriteLine("ObserverControl.OnLevelSelectedObserver() DOES THIS EVER DO ANYTHING.");
			// YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
//			Refresh();
			// Refresh() appears to be needed for TopView.
			// why would you need to refresh TileView ...
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
