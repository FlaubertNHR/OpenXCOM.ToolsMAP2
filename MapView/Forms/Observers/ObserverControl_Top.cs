using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Controls;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Inherited by <c><see cref="TopControl"/></c> and
	/// <c><see cref="QuadrantControl"/></c>.
	/// </summary>
	internal class ObserverControl_Top
		:
			DoubleBufferedControl,
			IObserver
	{
		#region IObserver requirements
		/// <summary>
		/// The currently loaded <c><see cref="XCom.MapFile"/></c>.
		/// </summary>
		/// <remarks>Is overridden by
		/// <c><see cref="TopControl.MapFile">TopControl.MapFile</see></c> but
		/// <c><see cref="QuadrantControl"/></c> uses this <c>MapFile</c>.</remarks>
		public virtual MapFile MapFile
		{ get; set; }

		/// <summary>
		/// Satisfies <c><see cref="IObserver"/></c>. Is overridden only by
		/// <c><see cref="QuadrantControl.OnLocationSelectedObserver()">QuadrantControl.OnLocationSelectedObserver()</see></c>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>TODO: So either (a) it should not be a requirement in
		/// <c>IObserver</c> or, (b) this should not inherit from
		/// <c>IObserver</c> or, (c) it should not be a virtual function but
		/// instead it should be wired differently for the
		/// <c>QuadrantControl</c>.</remarks>
		public virtual void OnLocationSelectedObserver(LocationSelectedArgs args)
		{}

		/// <summary>
		/// <summary>
		/// Satisfies <c><see cref="IObserver"/></c>. Is overridden only by
		/// <c><see cref="QuadrantControl.OnLevelSelectedObserver()">QuadrantControl.OnLevelSelectedObserver()</see></c>.
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>TODO: So either (a) it should not be a requirement in
		/// <c>IObserver</c> or, (b) this should not inherit from
		/// <c>IObserver</c> or, (c) it should not be a virtual function but
		/// instead it should be wired differently for the
		/// <c>QuadrantControl</c>.</remarks>
		public virtual void OnLevelSelectedObserver(LevelSelectedArgs args)
		{}
		#endregion IObserver requirements
	}
}
