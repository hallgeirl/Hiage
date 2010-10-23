
using System;

namespace Engine
{
	
	/// <summary>
	/// Tiled scrolling background.
	/// </summary>
	public class ParallaxBackground
	{
		double parallaxFactorX;
		double parallaxFactorY;
		Texture texture;
		Display display;
		
		/// <summary>
		/// Constructs a ParallaxBackground object. Needs the display object for the camera position.
		/// </summary>
		/// <param name="texture">
		/// A <see cref="Texture"/>
		/// </param>
		/// <param name="parallaxFactor">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="display">
		/// A <see cref="Display"/>
		/// </param>
		/// <param name="motionBlur">
		/// A <see cref="System.Boolean"/>
		/// </param>
		public ParallaxBackground(Texture texture, double parallaxFactorX, double parallaxFactorY, Display display)
		{
			this.texture = texture;
			this.parallaxFactorX = parallaxFactorX;
			this.parallaxFactorY = parallaxFactorY;
			this.display = display;
		}
		
		public void Render()
		{
			double cameraX = display.RenderedCameraX, cameraY = display.RenderedCameraY;
			double viewWidth = display.ViewportWidth, viewHeight = display.ViewportHeight;
			
			double x = parallaxFactorX*cameraX;
			double y = parallaxFactorY*cameraY;
			
			
			//"Floating point modulo" ftw.
			if (x > 0)
				while (x > texture.Width)
					x -= texture.Width;

			while (y > texture.Height)
				y -= texture.Height;
			
			if (x < 0)
				x *= -1;
			if (y < 0)
				y *= -1;
			/*double x = ((int)Math.Abs(parallaxFactorX*cameraX) % texture.Width);  //Current X position of the parallax shift
			double y = ((int)Math.Abs(parallaxFactorY*cameraY) % texture.Height); //Y position*/
			
			if (cameraX < 0)
			{
				x = texture.Width - x;
			}
			if (cameraY < 0)
			{
				y = texture.Height - y;
			}
			
			double startX = cameraX - viewWidth/2 - x;
			double startY = cameraY - viewHeight/2 - y;
			
			for (double currentX = startX; currentX < cameraX+viewWidth/2; currentX+=texture.Width)
			{
				for (double currentY = startY; currentY < cameraY+viewHeight/2; currentY+=texture.Height)
				{
					display.Renderer.Render(currentX, currentY, currentX+texture.Width, currentY+texture.Height, texture);
				}
			}
		}
	}
}
