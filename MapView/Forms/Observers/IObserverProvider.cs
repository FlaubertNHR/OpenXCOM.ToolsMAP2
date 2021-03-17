namespace MapView.Forms.Observers
{
	/// <summary>
	/// Interface for TileViewForm, TopViewForm, RouteViewForm.
	/// </summary>
	/// <remarks>This interface ensures that derived forms have implemented an
	/// ObserverControl.</remarks>
	internal interface IObserverProvider
	{
		ObserverControl Observer
		{ get; }
	}
}
