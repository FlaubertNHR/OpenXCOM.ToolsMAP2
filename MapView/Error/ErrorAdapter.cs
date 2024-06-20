using System;


namespace MapView.Error
{
	/// <summary>
	/// nice ... an Adapter<br/>
	/// with an Interface ... jic
	/// </summary>
	internal sealed class ErrorAdapter
		:
			IErrorHandler
	{
		public void HandleException(Exception exception)
		{
			using (var f = new ErrorF(exception))
				f.ShowDialog();
		}
	}
}
