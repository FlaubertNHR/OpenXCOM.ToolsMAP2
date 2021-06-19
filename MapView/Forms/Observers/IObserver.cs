using System;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for <c><see cref="ObserverControl"/></c> and
	/// <c><see cref="ObserverControl_Top"/></c>.
	/// </summary>
	internal interface IObserver
	{
		MapFile MapFile { set; get;}

		void OnLocationSelectedObserver(LocationSelectedArgs args);
		void OnLevelSelectedObserver(LevelSelectedArgs args);
	}
}
