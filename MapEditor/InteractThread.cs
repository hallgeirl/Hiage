using System.Threading;
using System;
using Engine;
using Gtk;

namespace MapEditor
{
	/// <summary>
	/// An instance of this class represents the thread which will do all the interaction with the engine.
	/// This means all the rendering and "engine logic" will be done here, while the GUI will be handled externally.
	/// </summary>
	public class InteractThread
	{
		Game game;
		EditorModel model;
		
		//Initialize 
		public InteractThread(EditorModel model)
		{
			this.model = model;
		}
		
		/// <summary>
		/// Run the main game loop
		/// </summary>
		public void MainLoop()
		{
			game = new Game();
			game.Initialize(1024, 768, false, "HIAGE Map Editor");
			game.PushState(new MapEditorState(model));
			
			
			model.ResourceManager = game.Resources;
			//model.CurrentTileset = model.ResourceManager.GetTileset("grassland");
			
			while (!game.Done && model.Running)
			{
				game.Run();
			}
			
			Application.Quit();			
		}
	}
}
