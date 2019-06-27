using System;


namespace MapView.Forms.Error
{
	internal interface IErrorHandler
	{
		void HandleException(Exception exception);
	}
}
