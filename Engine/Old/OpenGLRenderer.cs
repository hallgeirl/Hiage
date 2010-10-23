
using System;
using System.Collections.Generic;
using Tao.OpenGl;

namespace Engine
{
	
	
	public class OpenGLRenderer : IRenderer
	{
		int currentTexture = -1;
		
		public OpenGLRenderer()
		{
		}

		/// <summary>
		/// Render a IRenderable object
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		public void Render(IRenderable target)
		{
			Render(target.X, target.Y, target.Width, target.Height, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture, target.Rotation, 1, 1, 1, 1);
		}
		
		/// <summary>
		/// Render and specify color.
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
		public void Render(IRenderable target, double red, double green, double blue, double alpha)
		{
			Render(target.X, target.Y, target.Width, target.Height, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture, target.Rotation, red, green, blue, alpha);
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
				
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, currentTexture);
				
			}

			Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2d(0.0, 1.0);
			Gl.glVertex2d(x1, y1);
			Gl.glTexCoord2d(1.0, 1.0);
			Gl.glVertex2d(x2, y1);
			Gl.glTexCoord2d(1.0, 0.0);
			Gl.glVertex2d(x2, y2);
			Gl.glTexCoord2d(0.0, 0.0);
			Gl.glVertex2d(x1, y2);
			Gl.glEnd();
		}
		
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
		public void Render(double x, double y, Texture texture)
		{
			Render(x, y, x+texture.Width, y+texture.Height, texture);
		}
		
	/// <summary>
		/// "Raw" rendering function
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="width">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="height">
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
		public void Render(double x, double y, double width, double height, int texLeft, int texTop, int texRight, int texBottom, Texture texture, double rotation, double red, double green, double blue, double alpha)
		{
			//Check the bounds of the color values.
			if (red < 0 || red > 1)
			{
				throw new ArgumentOutOfRangeException("Value of 'red' must be between 0.0 and 1.0.");
			}
			if (green < 0 || green > 1)
			{
				throw new ArgumentOutOfRangeException("Value of 'green' must be between 0.0 and 1.0.");
			}
			if (blue < 0 || blue > 1)
			{
				throw new ArgumentOutOfRangeException("Value of 'blue' must be between 0.0 and 1.0.");
			}
			if (alpha < 0 || alpha > 1)
			{
				throw new ArgumentOutOfRangeException("Value of 'alpha' must be between 0.0 and 1.0.");
			}
			
			if (currentTexture != texture.TextureID)
			{
				currentTexture = texture.TextureID;
				
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, currentTexture);
				
			}
			//Rotate and translate
			Gl.glLoadIdentity();
			Gl.glPushMatrix();
			Gl.glTranslated(x, y, 0);
			Gl.glRotated(rotation, 0, 0, 1);
			
			Gl.glColor4d(red,green,blue,alpha);
			Gl.glBegin(Gl.GL_QUADS);
			
			double tleft = (double)texLeft / (double)texture.Width;
			double tright = (double)texRight / (double)texture.Width;
			double ttop = (double)texTop / (double)texture.Height;
			double tbottom = (double)texBottom / (double)texture.Height;
			
			//Console.WriteLine("" + tleft + " " + tright + " " + ttop + " " + tbottom);
			
			double left = -width/2;
			double right = width/2;
			double top = -height/2;
			double bottom = height/2;
			
			//Console.WriteLine("" + left + " " + right + " " + top + " " + bottom);
			
			//Upper left
			Gl.glTexCoord2d(tleft, ttop);
			Gl.glVertex2d(left, top);
			//Upper right
			Gl.glTexCoord2d(tright, ttop);
			Gl.glVertex2d(right, top);
			//Lower right
			Gl.glTexCoord2d(tright, tbottom);
			Gl.glVertex2d(right, bottom);
			//Lower left
			Gl.glTexCoord2d(tleft, tbottom);
			Gl.glVertex2d(left, bottom);
			
			Gl.glEnd();
			
			Gl.glColor4d(1,1,1,1);
			
			Gl.glPopMatrix();
		}
		
		public void DrawLine(double x1, double y1, double x2, double y2)
		{
			Gl.glDisable(Gl.GL_TEXTURE_2D);
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2d(x1,y1);
			Gl.glVertex2d(x2,y2);
			Gl.glEnd();
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			
		}
		
		public void DrawText(string text, double x, double y)
		{
			Gl.glRasterPos2d(x,y);
			
			foreach (char c in text)
			{
				
				
			}
		}
	}
}
