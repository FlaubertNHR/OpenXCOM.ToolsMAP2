using System;


namespace MapView
{
	static class Program
	{
		internal static string[] Args;

		[STAThread]
		public static void Main(string[] args)
		{
			Args = args;

			var start = new Startup();
			start.RunProgram();
		}
	}
}
