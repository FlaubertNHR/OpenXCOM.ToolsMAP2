using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

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
		[Browsable(false), DefaultValue(null)]
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
		/// Satisfies IMapObserver. Used by QuadrantPanel but disabled in
		/// TopPanel.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLocationObserver(SelectLocationEventArgs args)
		{}

		/// <summary>
		/// Satisfies IMapObserver. Used by QuadrantPanel and does not exist in
		/// TopPanel.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLevelObserver(SelectLevelEventArgs args)
		{}
		#endregion IMapObserver requirements


		/// <summary>
		/// Brushes for use in TopPanel and QuadrantPanel.
		/// </summary>
		internal static Dictionary<string, SolidBrush> Brushes
		{ get; set; }

		/// <summary>
		/// Pens for use in TopPanel and QuadrantPanel.
		/// </summary>
		internal static Dictionary<string, Pen> Pens
		{ get; set; }
	}
}
