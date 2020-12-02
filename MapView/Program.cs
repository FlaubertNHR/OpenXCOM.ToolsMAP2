using System;
using System.Globalization;
using System.Threading;


namespace MapView
{
	static class Program
	{
		internal static string[] Args;

		[STAThread]
		public static void Main(string[] args)
		{
			Args = args;

			Thread.CurrentThread.CurrentCulture       = //sheesh
			Thread.CurrentThread.CurrentUICulture     =
			CultureInfo.DefaultThreadCurrentCulture   =
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			var start = new Startup();
			start.RunProgram();
		}
	}
}
