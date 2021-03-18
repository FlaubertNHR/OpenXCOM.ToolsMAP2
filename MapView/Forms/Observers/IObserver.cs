﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for ObserverControl and ObserverControl_Top.
	/// </summary>
	internal interface IObserver
	{
		[Browsable(false)] // wtf
		MapFile MapFile
		{ set; get;}

		[Browsable(false)]
		Dictionary<string, IObserver> ObserverPanels
		{ get; }

		void OnLocationSelectedObserver(LocationSelectedEventArgs args);
		void OnLevelSelectedObserver(LevelSelectedEventArgs args);
	}
}