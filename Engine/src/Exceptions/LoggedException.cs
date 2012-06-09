using System;

namespace Engine
{
	public class LoggedException : Exception
	{
		public LoggedException () : base()
		{
			logError("(no message)");
		}
		
		public LoggedException (string msg) : base(msg)
		{
			logError(msg);
		}
		
		public LoggedException (string msg, Exception innerException) : base(msg, innerException)
		{
			logError(msg);
		}
		
		private void logError(string msg)
		{
			Log.Write("Exception: " + msg, Log.ERROR);
		}
	}
}

