using System;
using System.Drawing.Drawing2D;


namespace McdView
{
	/// <summary>
	/// Coordinates the drawing of floor and wall and content preview-blobs
	/// between <c><see cref="McdviewF"/></c> and
	/// <c><see cref="XCom.BlobDrawService">XCom.BlobDrawService</see></c>.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c>
	/// crap.
	/// <br/><br/>
	/// This class is like <c>MapView.Forms.Observers.BlobDrawCoordinator</c>
	/// but is <c>static</c> because only one instantiation is required. The
	/// other difference is that <c><see cref="_halfwidth"/></c> and
	/// <c><see cref="_halfheight"/></c> do not change.</remarks>
	internal static class BlobDrawCoordinator
	{
		/// <summary>
		/// Disposal isn't necessary since the GraphicsPaths last the lifetime
		/// of the app. But FxCop ca1001 gets antsy ....
		/// </summary>
		internal static void Dispose()
		{
			//DSShared.Logfile.Log("McdView.BlobDrawCoordinator.Dispose()");
			_path.Dispose();
		}


		#region Fields (static)
		internal static readonly GraphicsPath _path = new GraphicsPath();

		internal static int _halfwidth;
		internal static int _halfheight;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Sets <c><see cref="_halfwidth"/></c> and
		/// <c><see cref="_halfheight"/></c>.
		/// </summary>
		/// <param name="halfwidth"></param>
		internal static void SetService(int halfwidth)
		{
			_halfwidth  = halfwidth;
			_halfheight = halfwidth / 2;
		}
		#endregion Methods (static)
	}
}
