using System;
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
			if (args.Length != 0)
				PckViewForm._args = args;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PckViewForm());
		}
	}
}
