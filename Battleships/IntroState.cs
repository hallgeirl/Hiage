
using System;
using Engine;

namespace Battleships
{
	
	/// <summary>
	/// Handles the introduction.
	/// </summary>
	public class IntroState : IGameState
	{
		
		Game gameref;
		Texture authorScreen, titleScreen;
		int state = 0;
		int frameCounter = 0;
		bool transition;
		double rotation;
		double initialZoom = 200;
		const int TransitionTime = 180;
		
		public IntroState()
		{
		}
		
		public void Initialize(Game game)
		{
			gameref = game;
			authorScreen = gameref.Resources.GetTexture("author-background");
			titleScreen = gameref.Resources.GetTexture("title-background");
			
			gameref.Display.CameraX = 0;
			gameref.Display.CameraY = 0;
		}
		
		public void Run(double frameTime)
		{
			//Render the current background
			gameref.Display.Renderer.SetDrawingColor(1, 1, 1, 1.0 - (double)frameCounter/(double)TransitionTime);
			switch (state)
			{
			case 0:
				gameref.Display.Renderer.Render(0, 0, gameref.Display.AspectRatio*initialZoom*2, initialZoom*2, 0, authorScreen.Height, authorScreen.Width, 0, authorScreen, rotation);
				break;
			case 1:
				gameref.Display.Renderer.Render(0, 0, gameref.Display.AspectRatio*initialZoom*2, initialZoom*2, 0, titleScreen.Height, titleScreen.Width, 0, titleScreen, rotation);
				break;
			case 2:
				gameref.PushState(new FlyingState());
				break;
			}
			
			
			//If we're in the transition phase, blur the picture, zoom and rotate.
			if (transition)
			{
				frameCounter++;
				gameref.Display.Renderer.AccumulateFrame(0.8);
				gameref.Display.Renderer.MultiplyFrame(0.9);
				gameref.Display.Renderer.DrawFrame(1);
				rotation+=360/TransitionTime;
				gameref.Display.Zoom-=180/TransitionTime;
			}
			else
			{
				if (frameCounter > 0)
				{
					frameCounter--;
				}
			}
		
			//
			if (frameCounter >= TransitionTime)
			{
			 	state++;
				transition = false;
				gameref.Display.CameraX = 0;
				gameref.Display.CameraY = 0;
				rotation = 0;
				gameref.Display.Zoom = initialZoom;
			}
			
			if (gameref.Input.KeyPressed(HKey.Space))
			{
				transition = true;
			}
		}
	}
}
