
using System;

namespace Engine
{
	
	/// <summary>
	/// Make a static way to get a random number, so a new Random object doesn't have to be created on all objects using it.
	/// </summary>
	public class Rnd
	{
		static Random rnd = new Random();
		
		/// <summary>
		/// Returns a double between 0 and 1.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Double"/>
		/// </returns>
		public static double Next()
		{
			return rnd.NextDouble();
		}
	}
}
