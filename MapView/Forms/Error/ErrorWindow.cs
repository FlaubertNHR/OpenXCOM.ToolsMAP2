using System;
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

			lblHead.Text = "Fuck !" + Environment.NewLine + "what did u do";
			tbDetails.Text = exception.ToString();

			btnClose.Select();
		}
		#endregion cTor
	}
}
