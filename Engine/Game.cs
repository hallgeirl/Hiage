
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine
{
	
	
	public class Game
	{
		OpenGLDisplay 		display;
		ResourceManager 	resourceManager;
		InputManager		input;
		List<IGameState> 	gameStates;
		Stopwatch 			timer = new Stopwatch();
		int 				fps = 60;
		double 				currentFps = 0;

		public Game()
		{
		}

		/// <summary>
		/// Initialize the game: Initialize display, load resources etc.
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="fullscreen">
		/// A <see cref="System.Boolean"/>
		/// </param>
		public void Initialize(int width, int height, bool fullscreen)
		{
			gameStates = new List<IGameState>();
			
			//For now, let's just stick to OpenGL
			display = new OpenGLDisplay();
			display.Initialize(width, height);
			if (fullscreen)
			{
				display.Fullscreen = true;
			}
			
			resourceManager = new ResourceManager();
			//TODO: Don't hardcode this.
			resourceManager.LoadResources("Data/resources.xml");
			
			input = new InputManager();
			
			timer.Start();
		}

		/// <summary>
		/// Push a new game state onto the state stack
		/// </summary>
		/// <param name="state">
		/// A <see cref="IGameState"/>
		/// </param>
		/// <returns>
		/// Previous <see cref="IGameState"/>
		/// </returns>
		public IGameState PushState(IGameState state)
		{
			if (state != null)
			{
				gameStates.Add(state);
				state.Initialize(this);
			}
			else
			{
				throw new NullReferenceException("PushState: state was null.");
			}
			if (gameStates.Count > 1)
			{
				return gameStates[gameStates.Count-2];
			}
			return null;
		}
		
		/// <summary>
		/// Pop (remove) the top game state
		/// </summary>
		public IGameState PopState()
		{
			if (gameStates.Count == 0)
			{
				throw new IndexOutOfRangeException("PopState: No state to pop.");
			}
			IGameState temp = gameStates[gameStates.Count-1];
			gameStates.RemoveAt(gameStates.Count-1);
			
			return temp;
		}
		
		/// <summary>
		/// Run the game states and renderer
		/// </summary>
		public void Run()
		{
			long currentTime = timer.ElapsedMilliseconds;
			
			display.PrepareRender();
			
			if (gameStates.Count > 0)
			{
				gameStates[gameStates.Count-1].Run();
			}
			
			display.Render();
			
			while (timer.ElapsedMilliseconds - currentTime < (1/(double)fps*1000))
			{
			}
			currentFps = 1/((double)timer.ElapsedMilliseconds/1000 - (double)currentTime/1000);
		}
		
		public ResourceManager Resources
		{
			get
			{
				return resourceManager;
			}
		}
		
		public IDisplay Display
		{
			get
			{
				return display;
			}
		}
		
		public InputManager Input
		{
			get
			{
				return input;
			}
		}
		
		//// <value>
		/// FPS cap
		/// </value>
		public int MaxFPS
		{
			get
			{
				return fps;
			}
			set
			{
				fps = value;
			}
		}
		
		/// <value>
		/// Current FPS
		/// </value>
		public double FPS
		{
			get
			{
				return currentFps;
			}
		}
	}
}
