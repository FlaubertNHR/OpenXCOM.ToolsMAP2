using System;


namespace MapView.Forms.MapObservers
{
	/// <summary>
	/// Interface for TileViewForm, TopViewForm, RouteViewForm.
	/// @note What the effing h*ll is this and why is there a file with 3 LoC.
	/// </summary>
	internal interface IMapObserverProvider
	{
		MapObserverControl0 ObserverControl0
		{ get; }
	}
}
