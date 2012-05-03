
using System;

namespace Engine
{
	
	/// <summary>
	/// Interface for game states
	/// </summary>
	public abstract class GameState
	{
		protected Game game;
		
		public GameState(Game game)
		{
			this.game = game;
		}
		
		/// <summary>
		/// Run one iteration of the game. 
		/// </summary>
		/// <param name="frameTime">
		/// The number of milliseconds it took to render the last frame.
		/// </param>
		public abstract void Run(double frameTime);
		
		public event EventHandler Activated;
		
		
		internal void IssueActivateEvent()
		{
			if (Activated != null)
				Activated(this, new EventArgs());
		}
		/*protected virtual void OnChanged(EventArgs e)
		{
			if (Activated != null)
				Activated(this, e);
		}*/
	}
}
