using System;
using System.Collections.Generic;
using Tao.OpenGl;
using SdlDotNet.Graphics;
using SdlDotNet.Core;

namespace Engine
{	
	/*
	 * A OpenGL display.
	 * */
	public class Display
	{
		Surface screen;
		int width, height;
		double cameraX, cameraY, renderedCameraX, renderedCameraY;
		double zoom, renderedZoom;
		Renderer renderer;
		
		public Display()
		{
			cameraX = 0;
			cameraY = 0;
			zoom = 100;
			renderedZoom = zoom;
		}
		
		~Display()
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
		public void Initialize(int width, int height, ResourceManager resourceManager, string windowTitle)
		{
			//Initialize variables
			this.width = width;
			this.height = height;
			
			//Create the window
			screen = Video.SetVideoMode(width, height, 32, true, true, false, true, true);
			Video.WindowCaption = windowTitle;
			Video.GLDoubleBufferEnabled = true;
			
			renderer = new Renderer(Renderer.TextureMode.TEXTURE_RECTANGLE_EXT, this, resourceManager);
			
			
			PrepareViewport();
			Events.VideoResize += new EventHandler<VideoResizeEventArgs>(OnResize);
			
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
			if (RenderedZoom != Zoom)
			{
				PrepareViewport();
				renderedZoom = zoom;
			}
			
			Gl.glLoadIdentity();
			Gl.glClearColor(0, 0, 0, 1);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glTranslated(-CameraX, -CameraY, 0);
			renderedCameraX = CameraX;
			renderedCameraY = CameraY;
		}
		
		/// <summary>
		/// Flip the surface, the buffer or whatever to make the good stuff show.
		/// </summary>
		public void Render()
		{
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
			return (x >= (cameraX-renderedZoom) && x <= (cameraX+renderedZoom) && y <= (cameraY+renderedZoom) && y >= (cameraY-renderedZoom));
		}
		
		public Vector DisplayToWorldCoordinates(double x, double y)
		{
			return new Vector(RenderedCameraX - ViewportWidth/2 + x, RenderedCameraY - ViewportHeight/2 + y);
		}
		
		public double ViewTop
		{
			get
			{
				return RenderedCameraY + ViewportHeight/2;
			}
		}
		public double ViewBottom
		{
			get
			{
				return RenderedCameraY - ViewportHeight/2;
			}
		}
		public double ViewLeft
		{
			get
			{
				return RenderedCameraX - ViewportWidth/2;
			}
		}
		public double ViewRight
		{
			get
			{
				return RenderedCameraX + ViewportWidth/2;
			}
		}
		
		
		#region Event handlers
		/// <summary>
		/// Handle resizes
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="VideoResizeEventArgs"/>
		/// </param>
		public void OnResize(object sender, VideoResizeEventArgs args)
		{
			width = args.Width;
			height = args.Height;
			
			PrepareViewport();
			Gl.glViewport(0, 0, width, height);
			//screen = Video.SetVideoMode(width, height, 32, true, true, false);
			screen = Video.SetVideoMode(width, height, 32, true, true, false, true, true);
		}
		
		#endregion Event handlers
		
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
				//prerenderActions.Add(delegate { cameraX = value; });
				//Gl.glTranslated(cameraX-value, 0, 0);
				cameraX = value;
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
			}
		}
		
		/// <summary>
		/// The camera values as they were when the scene was rendered
		/// </summary>
		public double RenderedCameraX
		{
			get { return renderedCameraX; }
		}

		/// <summary>
		/// The camera values as they were when the scene was rendered
		/// </summary>
		public double RenderedCameraY
		{
			get { return renderedCameraY; }
		}
		
		//// <value>
		/// Set/get fullscreen
		/// </value>
		public bool Fullscreen
		{
			get
			{
				return screen.FullScreen;
			}
			
			set
			{
				screen = Video.SetVideoMode(width, height, 32, true, true, value, true, true);
			}
		}
		
		//// <value>
		/// Get a reference to the renderer.
		/// </value>
		public Renderer Renderer
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
				if (height > width)
				{
					return (int)(renderedZoom*2*AspectRatio);
				}
				return (int)renderedZoom*2;
			}
		}

		//// <value>
		/// Width of the drawing area. I.e. if ViewportWidth == 100, a square with width 100 will reach across the screen.
		/// </value>
		public int ViewportWidth
		{
			get
			{
				if (width>height)
				{
					return (int)(renderedZoom*2*AspectRatio);
				}
				return (int)renderedZoom*2;
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
				else
				{
					zoom = 1;
				}
			}
		}
		
		public double RenderedZoom
		{
			get { return renderedZoom; }
		}
		
		public int WindowWidth
		{
			get { return width; }
		}

		public int WindowHeight
		{
			get { return height; }
		}
		
#endregion Properties
	}
}
