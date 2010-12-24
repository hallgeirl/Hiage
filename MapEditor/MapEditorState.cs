
using System;
using System.Collections.Generic;
using Engine;

namespace MapEditor
{
	public class MapEditorState : IGameState, IEditorListener
	{
		EditorModel model;
		ParallaxBackground background = null;
		
		//Some "shortcuts"
		Display display;
		Renderer renderer;
		InputManager input;
		
		//When the mouse is outside this area while moving an object, the camera should pan
		int boundarySize = 100;
		
		bool lmbDown = false;
		
		public MapEditorState(EditorModel model)
		{
			this.model = model;
			model.AddListener(this);
		}
		
		public void Initialize(Game game)
		{			
			display = game.Display;
			renderer = display.Renderer;
			input = game.Input;
			
			model.Display = display;
			model.Zoom = display.Zoom;
		}
		
		public void ModelChanged(EditorModel.VariableName v, object oldValue, object newValue)
		{
			switch (v)
			{
			case EditorModel.VariableName.Background:
				if (!string.IsNullOrEmpty((string)newValue))
					Schedule(delegate {background = new ParallaxBackground(model.ResourceManager.GetTexture((string)newValue), 1, 0.5, model.Display); });
				break;
			}
		}
		
		/*
		 * This part is a little hack to be able to create maps from outside of this specific thread.
		 * Resource loading sometimes need access to the OpenGL context, which is accessible from this thread only.
		 * So any actions that may invoke the resource loaders must be scheduled here and run from this thread.
		 * */
		public delegate void ScheduleDelegate();
		
		private static List<ScheduleDelegate> schedule = new List<ScheduleDelegate>();
		
		public static void Schedule(ScheduleDelegate d)
		{
			lock(schedule)
			{
				schedule.Add(d);
			}
		}
		
		public void HandleInput(double frameTime)
		{
			//Update model
			model.MousePosition = input.WindowToWorldPosition(input.MouseWindowPosition, display, false);
			
			
			//Change in mouse cursor position in world coordinates
			Vector change = input.WindowToWorldPosition(input.MouseWindowPositionChange, display, true);
			

			
			//Navigation
			if (input.MouseButtonPressed(HMouseButton.LeftButton) && input.MouseButtonPressed(HMouseButton.RightButton))
			{
				display.Zoom -= change.Y;
				
				if (display.Zoom < 5) display.Zoom = 5;
				else if (display.Zoom > 1000) display.Zoom = 1000;
				model.Zoom = display.Zoom;
			}
			else if (input.MouseButtonPressed(HMouseButton.RightButton))
			{
				display.CameraX -= change.X;
				display.CameraY -= change.Y;
			}
			//Drawing
			else if (input.MouseButtonPressed(HMouseButton.LeftButton))
			{
				//Mouse click
				if (!lmbDown)
				{
					lmbDown = true;

					if (!string.IsNullOrEmpty(model.CurrentObject) && model.CurrentTool == EditorModel.Tool.CreateObject && model.TileMap != null)
					{
						MapObject o = new MapObject(model.ResourceManager.GetObjectDescriptor(model.CurrentObject),model.ResourceManager, renderer, new Vector(model.MousePosition.X, model.MousePosition.Y));
						model.Objects.Add(o);
						model.Changed = true;
						Log.Write("Spawned " + model.CurrentObject + " at " + model.MousePosition);
					}
					else if (model.CurrentTool == EditorModel.Tool.SelectObject)
					{
						bool anythingSelected = false;
						foreach (MapObject o in model.Objects)
						{
							if (model.MousePosition.X >= o.Left && model.MousePosition.X <= o.Right && model.MousePosition.Y >= o.Bottom && model.MousePosition.Y <= o.Top)
							{
								model.SelectedObject = o;
								anythingSelected = true;
							}
						}
						if (!anythingSelected)
							model.SelectedObject = null;
					}
				}
				
				//Mouse button down
				switch (model.CurrentTool)
				{
				case EditorModel.Tool.DrawTile:
					if (model.TileMap != null && model.CurrentTile != null)
					{
						Vector tilePos;
						try
						{
							if (model.DrawToLayer - 1 >= 0 && model.DrawToLayer <= model.TileMap.Layers)
							{
								tilePos = model.TileMap.WorldToTilePosition(input.WindowToWorldPosition(input.MouseWindowPosition,display, false));
								model.TileMap.SetTile((int)tilePos.X, (int)tilePos.Y, model.DrawToLayer-1, model.CurrentTile);
							}
						}
						catch (IndexOutOfRangeException e)
						{
							Log.Write("Attempted to set tile outside the boundaries: " + e.Message, Log.WARNING);
						}
						model.Changed = true;
					}
					break;
				case EditorModel.Tool.SelectObject:
					//Move objects
					if (model.SelectedObject != null)
					{
						model.SelectedObject.Position.X += change.X;
						model.SelectedObject.Position.Y += change.Y;
						model.Changed = true;
						
						//Pan the window if neccesary
						if (input.MouseWindowPosition.X >= display.WindowWidth-boundarySize)
						{
							display.CameraX += (boundarySize-(display.WindowWidth-input.MouseWindowPosition.X))*frameTime;
							if (display.CameraX + display.ViewportWidth / 2 < model.TileMap.Right)
								model.SelectedObject.Position.X += (boundarySize-(display.WindowWidth-input.MouseWindowPosition.X))*frameTime;
						}
						
						if (input.MouseWindowPosition.X <= boundarySize)
						{
							display.CameraX -= (boundarySize - input.MouseWindowPosition.X)*frameTime;
							if (display.CameraX - display.ViewportWidth / 2 > model.TileMap.Left)
								model.SelectedObject.Position.X -= (boundarySize - input.MouseWindowPosition.X)*frameTime;
						}
						
						if (input.MouseWindowPosition.Y >= display.WindowHeight-boundarySize)
						{
							display.CameraY -= (boundarySize-(display.WindowHeight-input.MouseWindowPosition.Y))*frameTime;
							if (display.CameraY - display.ViewportHeight / 2 > model.TileMap.Bottom)
								model.SelectedObject.Position.Y -= (boundarySize-(display.WindowHeight-input.MouseWindowPosition.Y))*frameTime;
						}
						
						if (input.MouseWindowPosition.Y <= boundarySize)
						{
							display.CameraY += (boundarySize - input.MouseWindowPosition.Y)*frameTime;
							if (display.CameraY + display.ViewportHeight / 2 < model.TileMap.Top)
								model.SelectedObject.Position.Y += (boundarySize - input.MouseWindowPosition.Y)*frameTime;
						}
					}
					break;
				}
			}
			else
			{
				lmbDown = false;
			}
			
			//Keypresses
			if (model.CurrentTool == EditorModel.Tool.SelectObject)
			{
				if (input.KeyPressed(HKey.Delete) && model.SelectedObject != null)
				{
					model.Objects.Remove(model.SelectedObject);
					model.SelectedObject = null;
				}
			}
			
			if (model.TileMap != null)
			{
				if (display.CameraY + display.ViewportHeight / 2 > model.TileMap.Top)
					display.CameraY = model.TileMap.Top - display.ViewportHeight/2;

				if (display.CameraY - display.ViewportHeight / 2 < model.TileMap.Bottom)
					display.CameraY = model.TileMap.Bottom + display.ViewportHeight/2;

				
				if (display.CameraX + display.ViewportWidth / 2 > model.TileMap.Right)
					display.CameraX = model.TileMap.Right - display.ViewportWidth/2;

				if (display.CameraX - display.ViewportWidth / 2 < model.TileMap.Left)
					display.CameraX = model.TileMap.Left + display.ViewportWidth/2;
			}

		}
		
		//Update and render the map
		public void Run(double frameTime)
		{
			lock (schedule)
			{
				for (int i = 0; i < schedule.Count; i++)
					schedule[i]();
/*				if (schedule.Count > 0)
				{
					foreach (ScheduleDelegate d in schedule)
					{
						d();
					}
				}*/
				schedule.Clear();
			}
			HandleInput(frameTime);
			
			if (background != null)
				background.Render();
			
			if (model.TileMap != null)
			{
				TileMap tm = model.TileMap;
				renderer.DrawLine(tm.Left, tm.Top, tm.Left, tm.Bottom);
				renderer.DrawLine(tm.Right, tm.Top, tm.Right, tm.Bottom);
				renderer.DrawLine(tm.Left, tm.Top, tm.Right, tm.Top);
				renderer.DrawLine(tm.Left, tm.Bottom, tm.Right, tm.Bottom);
				
				for (int z = 0; z < model.TileMap.Layers; z++)
					model.TileMap.Render(z);
				
			}
			
			foreach (MapObject o in model.Objects)
			{
				if (object.ReferenceEquals(o, model.SelectedObject))
				{
					renderer.DrawLine(o.Left, o.Top, o.Left, o.Bottom);
					renderer.DrawLine(o.Right, o.Top, o.Right, o.Bottom);
					renderer.DrawLine(o.Left, o.Top, o.Right, o.Top);
					renderer.DrawLine(o.Left, o.Bottom, o.Right, o.Bottom);
				}
				o.Update(frameTime);
				o.Render();
			}
		}
	}
}
