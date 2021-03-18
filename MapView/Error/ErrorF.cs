using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using DSShared;


namespace MapView.Error
{
	internal sealed partial class ErrorF
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor. Prints the stacktrace in a custom dialog.
		/// </summary>
		/// <param name="exception">the exception to show</param>
		internal ErrorF(Exception exception)
		{
			InitializeComponent();

			string text = exception.ToString();

			// NOTE: TextRenderer is too long but Graphics is too short.

			int width = 450, test;
			using (Graphics graphics = tbDetails.CreateGraphics())
			{
				string[] lines = text.Split(GlobalsXC.CRandorLF, StringSplitOptions.RemoveEmptyEntries);
				foreach (var line in lines)
				{
					test = (int)graphics.MeasureString(line, tbDetails.Font).Width;
					if (test > width)
						width = test;
				}
			}

			ClientSize = new Size(
								width + SystemInformation.VerticalScrollBarWidth + 75,
								ClientSize.Height);


			tbDetails.Text = text + Environment.NewLine;

			btnClose.Select();
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Terminates the app on button-click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void click_Terminate(object sender, EventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}
		#endregion Events
	}
}
