using System;
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
			if (args.Length != 0)
				McdviewF._args = args;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new McdviewF());
		}
	}
}
