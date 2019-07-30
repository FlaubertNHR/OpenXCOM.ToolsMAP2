using System;
using System.Drawing;
using System.Windows.Forms;


namespace MapView.Error
{
	internal sealed partial class ErrorF
		:
			Form
	{
		#region cTor
		internal ErrorF(Exception exception)
		{
			InitializeComponent();

			lblHead.Text = "!! wtf !!"; // + Environment.NewLine + "did u do";
			tbDetails.Text = exception.ToString();

			btnClose.Select();

#if DEBUG
			ClientSize = new Size(1300, 400);
#endif
		}
		#endregion cTor


		#region Events
		private void click_Terminate(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}
		#endregion Events
	}
}
