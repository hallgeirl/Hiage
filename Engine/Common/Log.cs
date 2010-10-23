
using System;

namespace Engine
{
	/// <summary>
	/// Class to abstract log writing
	/// </summary>
	public class Log
	{
		//Log types
		public const int OK = 0;
		public const int ERROR = 1;
		public const int WARNING = 2;
		public const int INFO = 3;
		
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
				output = "[OK]\t";
				break;
			case ERROR:
				output = "[ERROR]";
				break;
			case WARNING:
				output = "[WARNING]";
				break;
			case INFO:
				output = "[INFO]\t";
				break;
			default:
				output = "[UNKNOWN]";
				break;
			}
			
			output += "\t" + s;
			
			Console.WriteLine(output);
		}
	}
}
