using System;
using System.Globalization;
using System.Threading;


namespace MapView
{
	static class Program
	{
		#region Fields
		internal static string[] Args;
		#endregion Fields


		#region MainMethod
		[STAThread]
		public static void Main(string[] args)
		{
			Args = args;

			Thread.CurrentThread.CurrentCulture       = //sheesh
			Thread.CurrentThread.CurrentUICulture     =
			CultureInfo.DefaultThreadCurrentCulture   =
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			var start = new Start();
		}
		#endregion MainMethod
	}
}
