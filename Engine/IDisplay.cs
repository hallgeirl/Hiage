
using System;

namespace Engine
{
	/*
	 * Interface used to define displays (that is "a place to render stuff", like an OpenGL-window)
	 * */
	public interface IDisplay
	{
		/// <summary>
		/// Initialize the display, i.e. set up OpenGL, SDL, Direct3D, whatever and create a window.
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		void Initialize(int width, int height);
		
		void Destroy();
		
		/// <summary>
		/// Prepare viewport.
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		void PrepareViewport();
		
		/// <summary>
		/// Prepare the window for rendering, like clearing the screen and so on.
		/// </summary>
		void PrepareRender();
		
		/// <summary>
		/// Flip the surface, the buffer or whatever to make the good stuff show.
		/// </summary>
		void Render();
		
		double AspectRatio
		{
			get;
		}
		
		//// <value>
		/// Camera X position.
		/// </value>
		double CameraX
		{
			get;
			set;
		}
		
		//// <value>
		/// Camera Y position.
		/// </value>
		double CameraY
		{
			get;
			set;
		}
		
		//// <value>
		/// Fullscreen.
		/// </value>
		bool Fullscreen
		{
			get;
			set;
		}
		
		//// <value>
		/// Width of the drawing area. I.e. if ViewportWidth == 100, a square with width 100 will reach across the screen.
		/// </value>
		int ViewportWidth
		{
			get;
		}
		
		//// <value>
		/// Height of drawing area.
		/// </value>
		int ViewportHeight
		{
			get;
		}
		
		//// <value>
		/// Zoom level. How much zoomage.
		/// </value>
		double Zoom
		{
			get;
			set;
		}
		
		IRenderer Renderer
		{
			get;
		}
	}
}
