using System;


namespace MapView.Forms.Error
{
	/// <summary>
	/// nice ... an Adapter
	/// with an Interface ... jic
	/// </summary>
	internal sealed class ErrorWindowAdapter
		:
			IErrorHandler
	{
		public void HandleException(Exception exception)
		{
			using (var f = new ErrorWindow(exception))
				f.ShowDialog();
		}
	}
}
