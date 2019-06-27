using System;
using System.Threading;
using System.Windows.Forms;

using MapView.Forms.Error;


namespace MapView
{
	/// <summary>
	/// Class that starts application execution.
	/// </summary>
	public class Startup
	{
		private readonly IErrorHandler _errorHandler;

		/// <summary>
		/// Initializes a handler for unhandled exceptions.
		/// </summary>
		public Startup()
		{
			_errorHandler = new ErrorWindowAdapter();
		}


		/// <summary>
		/// Let's run this puppy.
		/// </summary>
		public void RunProgram()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += Application_ThreadException; // FIX: "Subscription to static events without unsubscription may cause memory leaks."
			try
			{
				Application.Run(new XCMainWindow());

				// https://msdn.microsoft.com/en-us/library/system.appdomain.aspx
				// Get this AppDomain's settings and display some of them.
//				var ads = AppDomain.CurrentDomain.SetupInformation;
//				Console.WriteLine(
//								"AppName={0}, AppBase={1}, ConfigFile={2}",
//								ads.ApplicationName,
//								ads.ApplicationBase,
//								ads.ConfigurationFile);
			}
			catch (Exception ex)
			{
				_errorHandler.HandleException(ex);
				throw;
			}
		}

		/// <summary>
		/// Handler for thread exceptions.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			_errorHandler.HandleException(e.Exception);
		}
	}
}
