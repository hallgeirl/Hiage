
using System;
using Tao.OpenGl;
using SdlDotNet.Graphics;

namespace Engine
{	
	/*
	 * A OpenGL display.
	 * */
	public class OpenGLDisplay : IDisplay
	{
		Surface screen;
		int width, height;
		double cameraX, cameraY;
		double zoom;
		OpenGLRenderer renderer;
		
		public OpenGLDisplay()
		{
			cameraX = 0;
			cameraY = 0;
			zoom = 100;
			renderer = new OpenGLRenderer();
		}
		
		~OpenGLDisplay()
		{
			Destroy();
		}
		
		/// <summary>
		/// Initialize the display, i.e. set up OpenGL, SDL, Direct3D, whatever and create a window.
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		public void Initialize(int width, int height)
		{
			//Initialize variables
			this.width = width;
			this.height = height;
			
			//Create the window
			screen = Video.SetVideoMode(width, height, 32, true, true, false);
			
			//Set some OpenGL attributes
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			//Gl.glDisable(Gl.GL_TEXTURE_2D);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			
			//Enable alpha blending
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glAlphaFunc(Gl.GL_GREATER, 0.1f);
			Gl.glEnable(Gl.GL_ALPHA_TEST);
			
			PrepareViewport();
			
		}
		
		public void Destroy()
		{
			//If fullscreen, reset video mode.
			if (Fullscreen)
			{
				Fullscreen = false;
			}
		}
		
		/// <summary>
		/// Resize the display.
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		public void PrepareViewport()
		{
			if (height == 0)
				height = 1;

			if (width == 0)
				width = 1;
			
			//Set the viewport dimensions, and store the current matrix
			Gl.glViewport(0, 0, width, height);
			Gl.glPushMatrix();
			
			//Set the matrix mode to modify the projection matrix
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			
			
			//Set the perspective
			if (width > height)
			{
				Gl.glOrtho(-Zoom * AspectRatio, Zoom * AspectRatio, -Zoom, Zoom, -1.0, 1000.0);
			}
			else
			{
				Gl.glOrtho(-Zoom, Zoom, -Zoom / AspectRatio, Zoom / AspectRatio, -1.0, 1000.0);
			}
			
			//Restore previous matrix
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPopMatrix();
		}
		
		/// <summary>
		/// Prepare the window for rendering, like clearing the screen and so on.
		/// </summary>
		public void PrepareRender()
		{
			Gl.glLoadIdentity();
			Gl.glClearColor(0, 0, 0, 1);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			
			Gl.glTranslated(-CameraX, -CameraY, 0);
			
			Gl.glBegin(Gl.GL_QUADS);
		}
		
		/// <summary>
		/// Flip the surface, the buffer or whatever to make the good stuff show.
		/// </summary>
		public void Render()
		{
			renderer.RenderFrame();
			Gl.glEnd();
			Video.GLSwapBuffers();
		}
		
		/// <summary>
		/// Check wether or not a point (x,y) is inside the viewport or not.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool InViewport(double x, double y)
		{
			return (x >= (cameraX-zoom) && x <= (cameraX+zoom) && y <= (cameraY+zoom) && y >= (cameraY-zoom));
		}
		
#region Properties
		
		public double AspectRatio
		{
			get
			{
				return (double)width / (double)height;
			}
		}
		
		//// <value>
		/// Camera X position.
		/// </value>
		public double CameraX
		{
			get
			{
				return cameraX;
			}
			
			set
			{
				cameraX = value;
				//Gl.glLoadIdentity();
				//Gl.glTranslated(-CameraX, -CameraY, 0);
			}
		}
		
		//// <value>
		/// Camera Y position.
		/// </value>
		public double CameraY
		{
			get
			{
				return cameraY;
			}
			
			set
			{
				cameraY = value;
				//Gl.glLoadIdentity();
				//Gl.glTranslated(-CameraX, -CameraY, 0);
			}
		}
		
		public bool Fullscreen
		{
			get
			{
				return screen.FullScreen;
			}
			
			set
			{
				screen = Video.SetVideoMode(width, height, 32, true, true, value);
			}
		}
		
		public IRenderer Renderer
		{
			get
			{
				return renderer;
			}
		}
		
		//// <value>
		/// Height of drawing area.
		/// </value>
		public int ViewportHeight
		{
			get
			{
				return 0;
			}
		}

		//// <value>
		/// Width of the drawing area. I.e. if ViewportWidth == 100, a square with width 100 will reach across the screen.
		/// </value>
		public int ViewportWidth
		{
			get
			{
				return 0;
			}
		}
		
		//// <value>
		/// Zoom level. How much zoomage.
		/// </value>
		public double Zoom
		{
			get
			{
				return zoom;
			}
			
			set
			{
				if (zoom > 0)
				{
					zoom = value;
				}
			}
		}
		
#endregion Properties
	}
}
