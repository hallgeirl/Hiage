
using System;

namespace Mario
{
	/// <summary>
	/// Contains info about the player
	/// </summary>
	public class PlayerState
	{
		public PlayerState ()
		{
			Lives = 5;
			Score = 0;
			Coins = 0;
		}
		
		public int Lives
		{
			get;
			set;
		}
		public int Score
		{
			get;
			set;
		}
		public int Coins
		{
			get;
			set;
		}
	}
}

