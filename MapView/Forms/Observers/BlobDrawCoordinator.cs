using System;
using System.Drawing.Drawing2D;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// Coordinates the drawing of floor- and wall- and content-blobs between
	/// <c><see cref="TopView"/></c> or <c><see cref="RouteView"/></c> and
	/// <c><see cref="XCom.BlobDrawService">XCom.BlobDrawService</see></c>.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c>
	/// crap.
	/// <br/><br/>
	/// This class is like <c>McdView.BlobDrawCoordinator</c> but is not
	/// <c>static</c> because multiple instantiations are required - one for
	/// <c><see cref="TopView"/></c> and another for
	/// <c><see cref="RouteView"/></c>. The other difference is that
	/// <c><see cref="HalfWidth"/></c> and <c><see cref="HalfHeight"/></c> need
	/// to change in the <c>Resize</c> event etc.</remarks>
	internal sealed class BlobDrawCoordinator
	{
		/// <summary>
		/// Disposal isn't necessary since the GraphicsPath lasts the lifetime
		/// of the app. But FxCop ca1001 gets antsy ....
		/// </summary>
		internal void Dispose()
		{
			//DSShared.Logfile.Log("MapView.Forms.Observers.BlobDrawCoordinator.Dispose()");
			_path.Dispose();
		}


		#region Fields (static)
		internal const int DefaultLinewidthContent = 3;
		#endregion Fields (static)


		#region Fields
		internal readonly GraphicsPath _path = new GraphicsPath();
		#endregion Fields


		#region Properties
		private int _halfwidth = 8;
		internal int HalfWidth
		{
			get { return _halfwidth; }
			set { _halfwidth = value; }
		}
		private int _halfheight = 4;
		internal int HalfHeight
		{
			get { return _halfheight; }
			set { _halfheight = value; }
		}
		#endregion Properties
	}
}
