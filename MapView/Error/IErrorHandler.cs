using System;


namespace MapView.Error
{
	internal interface IErrorHandler
	{
		void HandleException(Exception exception);
	}
}
