
using System;

namespace Engine
{
	
	
	public interface IGameState
	{
		void Initialize(Game game);
		void Run();
	}
}
