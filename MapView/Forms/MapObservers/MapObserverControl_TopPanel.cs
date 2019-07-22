using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Inherited by TopPanel, QuadrantPanel.
	/// </summary>
	internal class MapObserverControl_TopPanel
		:
			DoubleBufferedControl,
			IMapObserver
	{
		#region IMapObserver requirements
		private MapFileBase _mapBase;
		[Browsable(false)]
		public virtual MapFileBase MapBase
		{
			get { return _mapBase; }
			set { _mapBase = value; Refresh(); }
		}

		private readonly Dictionary<string, IMapObserver> _panels =
					 new Dictionary<string, IMapObserver>();
		[Browsable(false)]
		public Dictionary<string, IMapObserver> ObserverPanels
		{
			get { return _panels; }
		}

		/// <summary>
		/// Satisfies IMapObserver. Is overridden only by QuadrantPanel.
		/// TODO: So either (a) it should not be a requirement in IMapObserver
		/// or, (b) this should not inherit from IMapObserver or, (c) it should
		/// not be a virtual function but instead it should be wired differently
		/// for the QuadrantPanel.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLocationObserver(SelectLocationEventArgs args)
		{}

		/// <summary>
		/// Satisfies IMapObserver. Is overridden only by QuadrantPanel.
		/// TODO: So either (a) it should not be a requirement in IMapObserver
		/// or, (b) this should not inherit from IMapObserver or, (c) it should
		/// not be a virtual function but instead it should be wired differently
		/// for the QuadrantPanel.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLevelObserver(SelectLevelEventArgs args)
		{}
		#endregion IMapObserver requirements
	}
}
