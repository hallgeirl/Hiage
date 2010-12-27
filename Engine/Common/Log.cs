
using System;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Class to abstract log writing
	/// </summary>
	public class Log
	{
		static Log()
		{
			OutputStreamWriter = Console.Out;
		}
			
		//Log types
		public const int OK = 0;
		public const int ERROR = 1;
		public const int WARNING = 2;
		public const int INFO = 3;
		public const int DEBUG = 4;

		public static TextWriter OutputStreamWriter
		{
			get;
			set;
		}
		
		/// <summary>
		/// Write an "OK" log entry
		/// </summary>
		/// <param name="s">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Write(string s)
		{
			Write(s, INFO);
		}
		
		public static void Write(string s, int type)
		{
			string output;

			switch (type)
			{
			case OK:
				output = "[OK]";
				break;
			case ERROR:
				output = "[ERROR]";
				break;
			case WARNING:
				output = "[WARNING]";
				break;
			case INFO:
				output = "[INFO]";
				break;
			case DEBUG:
				output = "[DEBUG]";
				break;
			default:
				output = "[CODE " + type + "]";
				break;
			}
			
			output += "\t" + s;
			
			OutputStreamWriter.WriteLine(output);
		}
	}
}
