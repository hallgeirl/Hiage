
using System;

namespace Engine
{
	
	/// <summary>
	/// Timer for... well, timing stuff. Millisecond precision, hopefully.
	/// </summary>
	public class Timer
	{
		private int startTime;
		private int accumulatedTime = 0;
		
		public Timer()
		{
		}		
		
		/// <summary>
		/// Starts the timer
		/// </summary>
		public void Start()
		{
			if (!Started)
			{
				Started = true;
				startTime = Environment.TickCount;
			}
		}
		
		/// <summary>
		/// Stop the timer, returning the elapsed time. Does not reset the timer.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int Stop()
		{
			if (Started)
			{
				Started = false;
				accumulatedTime += Environment.TickCount - startTime;
			}
			
			return Elapsed;
		}
		
		public void Restart()
		{
			accumulatedTime = 0;
			startTime = Environment.TickCount;
		}
		
		#region Properties
		public bool Started
		{
			get;
			private set;
		}
			
		public int Elapsed
		{
			get { return Started ? accumulatedTime + Environment.TickCount - startTime : accumulatedTime; }
		}
		#endregion
	}
}
