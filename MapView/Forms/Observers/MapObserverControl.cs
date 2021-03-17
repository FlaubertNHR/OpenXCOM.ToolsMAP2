using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by TileView, TopView, RouteView.
	/// </summary>
	internal class MapObserverControl
		:
			UserControl,
			IMapObserver
	{
		#region IMapObserver requirements
		private readonly Dictionary<string, IMapObserver> _panels =
					 new Dictionary<string, IMapObserver>();
		[Browsable(false)]
		public Dictionary<string, IMapObserver> ObserverPanels
		{
			get { return _panels; }
		}

		private MapFile _file;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual MapFile MapFile
		{
			get { return _file; }
			set { _file = value; Refresh(); }
		}

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{
			//DSShared.LogFile.WriteLine("MapObserverControl.OnLocationSelectedObserver() DOES THIS EVER DO ANYTHING.");
			// TODO: YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
			Refresh();
		}

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLevelSelectedObserver(LevelSelectedEventArgs args)
		{
			//DSShared.LogFile.WriteLine("MapObserverControl.OnLevelSelectedObserver() DOES THIS EVER DO ANYTHING.");
			// TODO: YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
			Refresh();
		}
		#endregion IMapObserver requirements


		#region Properties
		private Options _options = new Options();
		internal Options Options
		{
			get { return _options; }
			set { _options = value; }
		}
		#endregion Properties


		#region Events (override)
		/// <summary>
		/// Scrolls the z-axis for TopView and RouteView.
		/// </summary>
		/// <param name="e"></param>
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


		#region Methods (virtual)
		internal protected virtual void LoadControlDefaultOptions()
		{}
		#endregion Methods (virtual)
	}
}
