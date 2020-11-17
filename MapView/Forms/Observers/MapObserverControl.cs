using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;
using XCom.Base;


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

		private MapFileBase _base;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual MapFileBase MapBase
		{
			get { return _base; }
			set { _base = value; Refresh(); }
		}

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			//DSShared.LogFile.WriteLine("MapObserverControl.OnSelectLocationObserver() DOES THIS EVER DO ANYTHING.");
			// TODO: YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
			Refresh();
		}

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			//DSShared.LogFile.WriteLine("MapObserverControl.OnSelectLevelObserver() DOES THIS EVER DO ANYTHING.");
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

			int dir = MapFileBase.LEVEL_no;
			if      (e.Delta < 0) dir = MapFileBase.LEVEL_Up;
			else if (e.Delta > 0) dir = MapFileBase.LEVEL_Dn;
			MapBase.ChangeLevel(dir);

			ObserverManager.ToolFactory.SetLevelButtonsEnabled(MapBase.Level, MapBase.MapSize.Levs);
		}
		#endregion Events (override)


		#region Methods
/*		/// <summary>
		/// Simulates a mousewheel event for keyboard navigation by
		/// RoutePanelParent.Navigate().
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
