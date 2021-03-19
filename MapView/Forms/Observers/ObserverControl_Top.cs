using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Controls;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by TopControl, QuadrantControl.
	/// </summary>
	internal class ObserverControl_Top
		:
			DoubleBufferedControl,
			IObserver
	{
		#region IObserver requirements
		private MapFile _file;
		[Browsable(false)]
		public virtual MapFile MapFile
		{
			get { return _file; }
			set { _file = value; Refresh(); }
		}

		private readonly Dictionary<string, IObserver> _panels =
					 new Dictionary<string, IObserver>();
		[Browsable(false)]
		public Dictionary<string, IObserver> ObserverControls
		{
			get { return _panels; }
		}

		/// <summary>
		/// Satisfies IObserver. Is overridden only by QuadrantControl.
		/// TODO: So either (a) it should not be a requirement in IObserver
		/// or, (b) this should not inherit from IObserver or, (c) it should
		/// not be a virtual function but instead it should be wired differently
		/// for the QuadrantControl.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{}

		/// <summary>
		/// Satisfies IObserver. Is overridden only by QuadrantControl.
		/// TODO: So either (a) it should not be a requirement in IObserver
		/// or, (b) this should not inherit from IObserver or, (c) it should
		/// not be a virtual function but instead it should be wired differently
		/// for the QuadrantControl.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLevelSelectedObserver(LevelSelectedEventArgs args)
		{}
		#endregion IObserver requirements
	}
}
