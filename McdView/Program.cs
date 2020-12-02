using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;


namespace McdView
{
	internal sealed class Program
	{
		/// <summary>
		/// The main entry point for the application McdView.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			if (args != null)
				McdviewF._args = args;

			Thread.CurrentThread.CurrentCulture       = //sheesh
			Thread.CurrentThread.CurrentUICulture     =
			CultureInfo.DefaultThreadCurrentCulture   =
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new McdviewF());
		}
	}
}
