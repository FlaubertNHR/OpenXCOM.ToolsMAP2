using System;
using System.Threading;
using System.Windows.Forms;

using MapView.Error;


namespace MapView
{
	/// <summary>
	/// Class that starts application execution.
	/// </summary>
	internal class Start
	{
		#region Fields
		private readonly IErrorHandler _errorHandler;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Initializes a handler for unhandled exceptions.
		/// </summary>
		internal Start()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			_errorHandler = new ErrorAdapter();

			Application.ThreadException += Application_ThreadException;
			Application.Run(new MainViewF()); // fly like the wind.
			Application.ThreadException -= Application_ThreadException;
		}
		#endregion cTor


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
