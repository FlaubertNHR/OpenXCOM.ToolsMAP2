using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;


namespace PckView
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application PckView.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			if (args != null)
				PckViewF._args = args;

			Thread.CurrentThread.CurrentCulture       = //sheesh
			Thread.CurrentThread.CurrentUICulture     =
			CultureInfo.DefaultThreadCurrentCulture   =
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PckViewF());
		}
	}
}
