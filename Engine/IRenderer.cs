
using System;

namespace Engine
{
	/*
	 * Interface to define the rendering class(es). Used as an abstraction layer, making it easy to add more rendering classes.
	 * */
	public interface IRenderer
	{
		/// <summary>
		/// Add a target for rendering
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		void AddTarget(IRenderable target);

		/// <summary>
		/// Renders all objects in the buffer
		/// </summary>
		void RenderFrame();
		
		/// <summary>
		/// Render a IRenderable object
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		void Render(IRenderable target);
		
		/// <summary>
		/// Render an object, ignoring texture coordinates (that is, render a full texture)
		/// </summary>
		/// <param name="x1">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y1">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="x2">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y2">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="texture">
		/// A <see cref="Texture"/>
		/// </param>
		void Render(double x1, double y1, double x2, double y2, Texture texture);
		
		/// <summary>
		/// "Raw" rendering function
		/// </summary>
		/// <param name="x1">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y1">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="x2">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y2">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="texX1">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texY1">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texX2">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texY2">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texture">
		/// A <see cref="Texture"/>
		/// </param>
		void Render(double x1, double y1, double x2, double y2, int texX1, int texY1, int texX2, int texY2, Texture texture);
	}
}
