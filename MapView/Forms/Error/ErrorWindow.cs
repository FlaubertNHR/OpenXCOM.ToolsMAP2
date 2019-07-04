using System;
using System.Drawing;
using System.Windows.Forms;


namespace MapView.Forms.Error
{
	internal sealed partial class ErrorWindow
		:
			Form
	{
		#region cTor
		internal ErrorWindow(Exception exception)
		{
			InitializeComponent();

			lblHead.Text = "_ wtf _" + Environment.NewLine + "did u do";
			tbDetails.Text = exception.ToString();

			btnClose.Select();

#if DEBUG
			ClientSize = new Size(1200, 400);
#endif
		}
		#endregion cTor
	}
}
