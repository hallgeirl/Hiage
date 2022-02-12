
using System;
using System.Collections.Generic;
using Tao.OpenGl;

namespace Engine
{
	
	
	public class OpenGLRenderer : IRenderer
	{
		List<IRenderable> renderTargets;
		int currentTexture = -1;
		
		public OpenGLRenderer()
		{
			renderTargets = new List<IRenderable>();
		}

		public void AddTarget(IRenderable target)
		{
			renderTargets.Add(target);
		}
		
		public void RenderFrame()
		{
			//Gl.glBegin(Gl.GL_QUADS);
			
			currentTexture = -1;
			
			for (int i = 0; i < renderTargets.Count; i++)
			{
				IRenderable r = renderTargets[i];
				/*if (r.DeleteFromRenderer)
				{
					renderTargets.Remove(r);
					i--;
					continue;
				}*/
				
				if (r.Texture.TextureID != currentTexture)
				{
					currentTexture = r.Texture.TextureID;
					Gl.glEnd();
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, currentTexture);
					Gl.glBegin(Gl.GL_QUADS);
				}
				
				double x1 = (double)r.TextureLeft / (double)r.Texture.Width;
				double x2 = (double)r.TextureRight / (double)r.Texture.Width;
				double y1 = (double)r.TextureTop / (double)r.Texture.Height;
				double y2 = (double)r.TextureBottom / (double)r.Texture.Height;
				
				Gl.glTexCoord2d(x1, y2);
				Gl.glVertex2d(r.Left, r.Top);
				Gl.glTexCoord2d(x2, y2);
				Gl.glVertex2d(r.Right, r.Top);
				Gl.glTexCoord2d(x2, y1);
				Gl.glVertex2d(r.Right, r.Bottom);
				Gl.glTexCoord2d(x1, y1);
				Gl.glVertex2d(r.Left, r.Bottom);
			}
			//Gl.glEnd();
			
			renderTargets.Clear();
		}
		
		
		/// <summary>
		/// Render a IRenderable object
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		public void Render(IRenderable target)
		{
			Render(target.Left, target.Top, target.Right, target.Bottom, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture);
		}
		
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
		public void Render(double x1, double y1, double x2, double y2, Texture texture)
		{
			if (currentTexture != texture.TextureID)
			{
				currentTexture = texture.TextureID;
				Gl.glEnd();
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, currentTexture);
				Gl.glBegin(Gl.GL_QUADS);
			}

			Gl.glTexCoord2d(0.0, 1.0);
			Gl.glVertex2d(x1, y1);
			Gl.glTexCoord2d(1.0, 1.0);
			Gl.glVertex2d(x2, y1);
			Gl.glTexCoord2d(1.0, 0.0);
			Gl.glVertex2d(x2, y2);
			Gl.glTexCoord2d(0.0, 0.0);
			Gl.glVertex2d(x1, y2);
		}
		
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
		/// <param name="texLeft">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texTop">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texRight">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texBottom">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="texture">
		/// A <see cref="Texture"/>
		/// </param>
		public void Render(double x1, double y1, double x2, double y2, int texLeft, int texTop, int texRight, int texBottom, Texture texture)
		{
			if (currentTexture != texture.TextureID)
			{
				currentTexture = texture.TextureID;
				Gl.glEnd();
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, currentTexture);
				Gl.glBegin(Gl.GL_QUADS);
			}
			
			double tx1 = (double)texLeft / (double)texture.Width;
			double tx2 = (double)texRight / (double)texture.Width;
			double ty1 = (double)texTop / (double)texture.Height;
			double ty2 = (double)texBottom / (double)texture.Height;
			
			//Upper left
			Gl.glTexCoord2d(tx1, ty1);
			Gl.glVertex2d(x1, y1);
			//Upper right
			Gl.glTexCoord2d(tx2, ty1);
			Gl.glVertex2d(x2, y1);
			//Lower right
			Gl.glTexCoord2d(tx2, ty2);
			Gl.glVertex2d(x2, y2);
			//Lower left
			Gl.glTexCoord2d(tx1, ty2);
			Gl.glVertex2d(x1, y2);
		}
	}
}
