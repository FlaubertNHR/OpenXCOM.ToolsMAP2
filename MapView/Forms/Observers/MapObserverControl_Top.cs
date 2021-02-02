using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Controls;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by TopPanel, QuadrantPanel.
	/// </summary>
	internal class MapObserverControl_Top
		:
			DoubleBufferedControl,
			IMapObserver
	{
		#region IMapObserver requirements
		private MapFile _file;
		[Browsable(false)]
		public virtual MapFile MapFile
		{
			get { return _file; }
			set { _file = value; Refresh(); }
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
		public virtual void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{}

		/// <summary>
		/// Satisfies IMapObserver. Is overridden only by QuadrantPanel.
		/// TODO: So either (a) it should not be a requirement in IMapObserver
		/// or, (b) this should not inherit from IMapObserver or, (c) it should
		/// not be a virtual function but instead it should be wired differently
		/// for the QuadrantPanel.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLevelSelectedObserver(LevelSelectedEventArgs args)
		{}
		#endregion IMapObserver requirements
	}
}
