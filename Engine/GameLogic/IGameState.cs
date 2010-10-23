
using System;

namespace Engine
{
	
	/// <summary>
	/// Interface for game states
	/// </summary>
	public interface IGameState
	{
		void Initialize(Game game);
		
		/// <summary>
		/// Run one iteration of the game. 
		/// </summary>
		/// <param name="frameTime">
		/// The number of milliseconds it took to render the last frame.
		/// </param>
		void Run(double frameTime);
	}
}
