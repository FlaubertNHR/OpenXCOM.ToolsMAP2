using System;
using System.Windows.Forms;


namespace MapView.Forms.Error
{
	internal sealed partial class ErrorWindow
		:
			Form
	{
		private readonly Exception _exception;


		#region cTor
		internal ErrorWindow(Exception exception)
		{
			_exception = exception;

			InitializeComponent();
		}
		#endregion cTor


		private void OnLoad(object sender, EventArgs e)
		{
			tbDetails.Text = _exception.ToString();
			tbDetails.SelectionStart = 0;
		}
	}
}
