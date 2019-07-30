namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for TileViewForm, TopViewForm, RouteViewForm.
	/// @note This interface ensures that derived forms have implemented a
	/// MapObserverControl.
	/// </summary>
	internal interface IMapObserverProvider
	{
		MapObserverControl ObserverControl
		{ get; }
	}
}
