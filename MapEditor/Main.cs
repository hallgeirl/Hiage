using System;
using Gtk;
using Engine;
using System.Threading;

namespace MapEditor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//The model that is shared between the engine part and GUI part of the map editor
			EditorModel model = new EditorModel();
			

			//Start up the engine interaction thread (rendering and such)
			InteractThread thread = new InteractThread(model);
			Thread engineThread = new Thread(new ThreadStart(thread.MainLoop));
			engineThread.Start();

			//Make sure the resource manager has been set before continuing, and ensure that the opengl context has been set up.
			while ((model.ResourceManager == null || !Renderer.Ready) && model.Running);
			
			if (model.Running)
			{
				//Initialize and run the GUI
				Application.Init ();
				MainWindow win = new MainWindow (model);
				win.Show ();
				Application.Run ();
			}			
		}
	}
}