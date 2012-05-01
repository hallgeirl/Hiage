using Engine;
using System;

namespace Mario
{
	public abstract class MarioGameState : IGameState
	{
		public PlayerState PlayerState { get; private set; }
		
		public MarioGameState (PlayerState state)
		{
			PlayerState = state;
		}
		
		public abstract void Initialize (Game game);

		public abstract void Run (double frameTime);
	}
}

