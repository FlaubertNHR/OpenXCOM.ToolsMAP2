using System;


namespace MapView.Forms.MapObservers
{
	/// <summary>
	/// Interface for 'TileViewForm', 'TopViewForm', 'RouteViewForm'.
	/// @note This interface ensures that 'TileViewForm', 'TopViewForm', and
	/// 'RouteViewForm' have all implemented a 'MapObserverControl'.
	/// </summary>
	internal interface IMapObserverProvider
	{
		MapObserverControl ObserverControl
		{ get; }
	}
}
