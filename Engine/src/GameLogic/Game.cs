
using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using SdlDotNet.Core;

namespace Engine
{
	public class Game
	{
		Display 			display;
		ResourceManager 	resourceManager;
		InputManager		input;
		AudioManager		audioManager;
		List<GameState> 	gameStates;
		Stopwatch 			timer = new Stopwatch();
		int 				fps = 60;
		double 				currentFps = 0;
		double				lastFrameTime = 0;
		bool 				done = false;

		public Game()
		{
			Events.Quit += new EventHandler<QuitEventArgs>(OnQuit);
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
		public void Initialize(int width, int height, bool fullscreen, string windowTitle)
		{
			Log.Write("Engine initializing at " + DateTime.Now);
			gameStates = new List<GameState>();

			//Create resource manager and load resources from the main resource file
			resourceManager = new ResourceManager();
			resourceManager.LoadResources("data");
			//resourceManager.LoadResourceXML(ResourceManager.MainResourceFile);

			
			//Create the opengl display
			display = new Display();
			display.Initialize(width, height, resourceManager, windowTitle);
			if (fullscreen)
			{
				display.Fullscreen = true;
			}
			
			
			input = new InputManager();
			audioManager = new AudioManager(resourceManager);
			
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
		public GameState PushState(GameState state)
		{
			if (state != null)
			{
				gameStates.Add(state);
				//state.Initialize(this);
			}
			else
			{
				throw new NullReferenceException("PushState: state was null.");
			}
			
			gameStates[gameStates.Count-1].IssueActivateEvent();
			
			if (gameStates.Count > 1)
			{
				return gameStates[gameStates.Count-2];
			}
			return null;
		}
		
		/// <summary>
		/// Pop (remove) the top game state
		/// </summary>
		public GameState PopState()
		{
			if (gameStates.Count == 0)
			{
				throw new IndexOutOfRangeException("PopState: No state to pop.");
			}
			GameState temp = gameStates[gameStates.Count-1];
			gameStates.RemoveAt(gameStates.Count-1);
			
			if (gameStates.Count > 0)
				gameStates[gameStates.Count-1].IssueActivateEvent();
			
			return temp;
		}
		
		/// <summary>
		/// Run the game states and renderer
		/// </summary>
		public void Run()
		{
			while (!Done)
			{
				//Poll for events (Close, resize etc.)
				while (Events.Poll())
				{
				}
				
				timer.Reset();
				timer.Start();
				
				display.PrepareRender();
				
				if (gameStates.Count > 0)
				{
					//gameStates[gameStates.Count-1].Run(lastFrameTime);
					gameStates[gameStates.Count-1].Run(lastFrameTime);
				}
				
				display.Render();
				
				if (fps > 0) while (timer.Elapsed.Milliseconds < (1/(double)fps*1000)) {}
				else while (timer.Elapsed.Milliseconds == 0) {}
				
				lastFrameTime = (double)timer.Elapsed.Milliseconds / 1000.0;
				//TODO: Remove the following line
				//lastFrameTime = 0.02;
				GameTime += lastFrameTime;
				currentFps = 1.0/lastFrameTime;
			}
		}
		
		/// <summary>
		/// Quit the application.
		/// </summary>
		public void Shutdown()
		{
			done = true;
			Events.QuitApplication();
			audioManager.PlayMusic(null, null);
			Thread.CurrentThread.Abort();
		}
		
		#region Event handlers
		
		/// <summary>
		/// Handle closing of the window, causing the application to exit.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="QuitEventArgs"/>
		/// </param>
		public void OnQuit(object sender, QuitEventArgs args)
		{
			Shutdown();
		}
		
		#endregion Event handlers
		
		public bool Done
		{
			get
			{
				return done;
			}
		}
			
		public ResourceManager Resources
		{
			get
			{
				return resourceManager;
			}
		}
		
		public Display Display
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
		
		public AudioManager Audio
		{
			get
			{
				return audioManager;
			}
		}
		
		
		public double GameTime
		{
			get;
			private set;
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
