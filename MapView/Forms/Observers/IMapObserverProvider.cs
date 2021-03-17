namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for TileViewForm, TopViewForm, RouteViewForm.
	/// </summary>
	/// <remarks>This interface ensures that derived forms have implemented a
	/// MapObserverControl.</remarks>
	internal interface IMapObserverProvider
	{
		MapObserverControl Observer
		{ get; }
	}
}
