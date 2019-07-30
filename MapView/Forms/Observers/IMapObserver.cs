using System;
using System.Collections.Generic;
using System.ComponentModel;

using XCom;
using XCom.Base;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for MapObserverControl and MapObserverControl_Top.
	/// </summary>
	internal interface IMapObserver
	{
		[Browsable(false)]
		MapFileBase MapBase
		{ set; get;}

		[Browsable(false)]
		Dictionary<string, IMapObserver> ObserverPanels
		{ get; }

		void OnSelectLocationObserver(SelectLocationEventArgs args);
		void OnSelectLevelObserver(SelectLevelEventArgs args);
	}
}
