using System;
using System.Collections.Generic;
using System.ComponentModel;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for <c><see cref="ObserverControl"/></c> and
	/// <c><see cref="ObserverControl_Top"/></c>.
	/// </summary>
	internal interface IObserver
	{
		[Browsable(false)] // wtf
		MapFile MapFile
		{ set; get;}

		[Browsable(false)]
		Dictionary<string, IObserver> ObserverControls
		{ get; }

		void OnLocationSelectedObserver(LocationSelectedArgs args);
		void OnLevelSelectedObserver(LevelSelectedArgs args);
	}
}
