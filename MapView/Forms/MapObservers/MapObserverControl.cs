using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers;

using XCom;
using XCom.Interfaces.Base;


namespace MapView
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
			//XCom.LogFile.WriteLine("MapObserverControl.OnSelectLocationObserver() DOES THIS EVER DO ANYTHING.");
			// TODO: YES IT FIRES A HUNDRED THOUSAND TIMES PER SECOND.
			Refresh();
		}

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			//XCom.LogFile.WriteLine("MapObserverControl.OnSelectLevelObserver() DOES THIS EVER DO ANYTHING.");
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
			if      (e.Delta < 0) MapBase.LevelUp();
			else if (e.Delta > 0) MapBase.LevelDown();

			ViewerFormsManager.ToolFactory.SetLevelButtonsEnabled(MapBase.Level, MapBase.MapSize.Levs);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Simulates a mousewheel event for keyboard navigation by
		/// RoutePanelParent.Navigate().
		/// </summary>
		/// <param name="e"></param>
		internal void doMousewheel(MouseEventArgs e)
		{
			OnMouseWheel(e);
		}
		#endregion Methods


		#region Methods (virtual)
		internal protected virtual void LoadControlDefaultOptions()
		{}
		#endregion Methods (virtual)
	}
}
