
using System;

namespace Engine
{
	/*
	 * Interface to define the rendering class(es). Used as an abstraction layer, making it easy to add more rendering classes.
	 * */
	public interface IRenderer
	{
		/// <summary>
		/// Render a IRenderable object
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		void Render(IRenderable target);
		
		/// <summary>
		/// Render and specify rendering color.
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		/// <param name="red">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="green">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="blue">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="alpha">
		/// A <see cref="System.Double"/>
		/// </param>
		void Render(IRenderable target, double red, double green, double blue, double alpha);
		
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
		/// Render a texture, plain and simple.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="texture">
		/// A <see cref="Texture"/>
		/// </param>
		void Render(double x, double y, Texture texture);
		
		/// <summary>
		/// "Raw" rendering function
		/// </summary>
		/// <param name="x1">
		/// A <see cref="System.Double"/>. Left edge.
		/// </param>
		/// <param name="y1">
		/// A <see cref="System.Double"/>. Top edge.
		/// </param>
		/// <param name="x2">
		/// A <see cref="System.Double"/>. Right edge.
		/// </param>
		/// <param name="y2">
		/// A <see cref="System.Double"/>. Bottom edge.
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
		/// <param name="rotation">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="red">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="green">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="blue">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="alpha">
		/// A <see cref="System.Double"/>
		/// </param>
		void Render(double x, double y, double width, double height, int texLeft, int texTop, int texRight, int texBottom, Texture texture, double rotation, double red, double green, double blue, double alpha);
		
		void DrawLine(double x1, double y1, double x2, double y2);
		
		void DrawText(string text, double x, double y);
		
		/// <summary>
		/// Register a texture with the renderer
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.IntPtr"/>
		/// </param>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		int RegisterTexture(System.IntPtr data, int width, int height);
	}
}
