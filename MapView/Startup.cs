using System;
using System.Threading;
using System.Windows.Forms;

using MapView.Error;


namespace MapView
{
	/// <summary>
	/// Class that starts application execution.
	/// </summary>
	public class Startup
	{
		#region Fields
		private readonly IErrorHandler _errorHandler;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Initializes a handler for unhandled exceptions.
		/// </summary>
		public Startup()
		{
			_errorHandler = new ErrorAdapter();
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Let's run this puppy.
		/// </summary>
		public void RunProgram()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += Application_ThreadException;
			try
			{
				Application.Run(new MainViewF()); // fly like the wind.
			}
			catch (Exception ex)
			{
				_errorHandler.HandleException(ex);
				throw;
			}
		}
		#endregion Methods


		#region Events
		/// <summary>
		/// Handler for thread exceptions.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			_errorHandler.HandleException(e.Exception);
		}
		#endregion Events
	}
}
