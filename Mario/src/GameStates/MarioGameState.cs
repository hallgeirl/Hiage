using Engine;
using System;

namespace Mario
{
	public abstract class MarioGameState : GameState
	{
		public PlayerState PlayerState { get; private set; }
		
		public MarioGameState (Game game, PlayerState state) : base(game)
		{
			PlayerState = state;
		}
		
		//public override void Initialize (Game game);

		//public abstract void Run (double frameTime);
	}
}

