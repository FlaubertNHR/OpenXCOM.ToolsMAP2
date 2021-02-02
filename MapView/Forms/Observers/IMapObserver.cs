using System;
using System.Collections.Generic;
using System.ComponentModel;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for MapObserverControl and MapObserverControl_Top.
	/// </summary>
	internal interface IMapObserver
	{
		[Browsable(false)] // wtf
		MapFile MapFile
		{ set; get;}

		[Browsable(false)]
		Dictionary<string, IMapObserver> ObserverPanels
		{ get; }

		void OnLocationSelectedObserver(LocationSelectedEventArgs args);
		void OnLevelSelectedObserver(LevelSelectedEventArgs args);
	}
}
