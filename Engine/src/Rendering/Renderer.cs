
using System;
using System.Collections.Generic;
using Tao.OpenGl;

namespace Engine
{	
	public class Renderer
	{
		#region Variables
		int currentTexture = -1;
		//Display display;
		
		//Font stuff
		public const int BASE_FONT_SIZE = 48;	//The base font size (that is, the size the font will be rendered at). Increasing this will increase the overall font sizes.
		string currentFont;				//The currently selected font
		int currentFontSize = 48;		//Current font size
		ResourceManager resourceManager;
		
		#endregion Variables
		
		public enum TextureMode
		{
			NORMAL_MODE = Gl.GL_TEXTURE_2D,
			TEXTURE_RECTANGLE_EXT = Gl.GL_TEXTURE_RECTANGLE_ARB
		}
		
		private static TextureMode textureMode = TextureMode.NORMAL_MODE;
		public static bool Ready { get; private set; }
		
		static Renderer()
		{
			Ready = false;
		}
		
		/// <summary>
		/// Construct the renderer.
		/// </summary>
		/// <param name="texmode">
		/// A <see cref="TextureMode"/>. Choose "TextureMode.NORMAL_MODE" if your graphics card supports non-power of 2 texture sizes, TextureMode.TEXTURE_RECTANGLE_EXT otherwise.
		/// </param>
		public Renderer(TextureMode texmode, Display display, ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
			textureMode = texmode;
			
			try
			{
				CheckRendererStatus();
			}
			catch (Exception e)
			{
				Log.Write("Could not initialize renderer: " + e.Message);
				throw e;
			}
			
			//Set some OpenGL attributes
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(GlTextureMode);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			
			//Enable alpha blending
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glEnable(Gl.GL_BLEND);
			
			Ready = true;
			
			Gl.glColor4d(1, 1, 1, 1);
			/*Gl.glDrawBuffer(Gl.GL_BACK);
			Gl.glClearAccum(0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glClear(Gl.GL_ACCUM_BUFFER_BIT);*/
			
			
		}
		
		public void CheckRendererStatus()
		{
			int errorCode = Gl.glGetError();
			if (errorCode != 0)
				throw new Exception("OpenGL has encountered an error. Error code: " + errorCode);
		}
		
		#region Rendering functions
		
		/// <summary>
		/// Set the current color for drawing (Applies to both text and textures).
		/// </summary>
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
		public void SetDrawingColor(double red, double green, double blue, double alpha)
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
			
			Gl.glColor4d(red, green, blue, alpha);
		}
		
		/// <summary>
		/// Render a IRenderable object
		/// </summary>
		/// <param name="target">
		/// A <see cref="IRenderable"/>
		/// </param>
		public void Render(IRenderable target)
		{
			Render(target.X, target.Y, target.Width, target.Height, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture, target.Rotation, target.Scaling);
		}
		
		public void Render(IRenderable target, double glowRadius)
		{
			SetDrawingColor(1, 0, 0, 0.8);
			Render(target.X, target.Y, target.Width + glowRadius*2, target.Height + glowRadius*2, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture, target.Rotation, target.Scaling);
			SetDrawingColor(1.0, 0.5, 0.5, 0.8);
			Render(target.X, target.Y, target.Width + glowRadius, target.Height + glowRadius, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture, target.Rotation, target.Scaling);
			SetDrawingColor(1,1,1,1);
			Render(target.X, target.Y, target.Width, target.Height, target.TextureLeft, target.TextureTop, target.TextureRight, target.TextureBottom, target.Texture, target.Rotation, target.Scaling);	
		}
		
		/// <summary>
		/// Draw a square with no texture.
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
		public void DrawSquare(double x1, double y1, double x2, double y2)
		{
			Gl.glDisable(GlTextureMode);
				
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glVertex2d(x1, y1);
			Gl.glVertex2d(x2, y1);
			Gl.glVertex2d(x2, y2);
			Gl.glVertex2d(x1, y2);
			Gl.glEnd();
				
			Gl.glEnable(GlTextureMode);
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
				
				Gl.glBindTexture(GlTextureMode, currentTexture);
				
			}
			
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glTexCoord2d(TextureCoordinate(0, texture.Width), TextureCoordinate(texture.Height, texture.Height));
			Gl.glVertex2d(x1, y1);
			Gl.glTexCoord2d(TextureCoordinate(texture.Width, texture.Width), TextureCoordinate(texture.Height, texture.Height));
			Gl.glVertex2d(x2, y1);
			Gl.glTexCoord2d(TextureCoordinate(texture.Width, texture.Width), TextureCoordinate(0, texture.Height));
			Gl.glVertex2d(x2, y2);
			Gl.glTexCoord2d(TextureCoordinate(0, texture.Width), TextureCoordinate(0, texture.Height));
			Gl.glVertex2d(x1, y2);
			Gl.glEnd();
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
		public void Render(double x, double y, double width, double height, int texLeft, int texTop, int texRight, int texBottom, Texture texture, double rotation, double scale)
		{
			if (currentTexture != texture.TextureID)
			{
				currentTexture = texture.TextureID;
				
				Gl.glBindTexture(GlTextureMode, currentTexture);
			}
			
			//Rotate and translate
			Gl.glPushMatrix();
			Gl.glTranslated(x, y, 0);
			Gl.glRotated(rotation, 0, 0, 1);
			Gl.glScaled(scale, scale, scale);
			
			Gl.glBegin(Gl.GL_QUADS);
			
			double tleft = TextureCoordinate(texLeft, texture.Width);
			double tright = TextureCoordinate(texRight, texture.Width);
			double ttop = TextureCoordinate(texTop, texture.Height);
			double tbottom = TextureCoordinate(texBottom, texture.Height);
			
			//Console.WriteLine("tc:" + tleft + " " + tright + " " + ttop + " " + tbottom);
			
			double left = -width/2;
			double right = width/2;
			double top = height/2;
			double bottom = -height/2;
			
						
			//Console.WriteLine("sc:" + left + " " + right + " " + top + " " + bottom);
			
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
			
			Gl.glPopMatrix();
		}
		
		/// <summary>
		/// Draw a line with the current drawing color.
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
		public void DrawLine(double x1, double y1, double x2, double y2)
		{
			Gl.glDisable(GlTextureMode);
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2d(x1,y1);
			Gl.glVertex2d(x2,y2);
			Gl.glEnd();
			Gl.glEnable(GlTextureMode);
		}
		
		#endregion Rendering functions
		
		#region Text rendering functions
		/// <summary>
		/// Set the current font
		/// </summary>
		/// <param name="fontName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="fontSize">
		/// A <see cref="System.Int32"/>
		/// </param>
		public void SetFont(string fontName, int fontSize)
		{
			if (fontSize <= 0)
			{
				throw new ArgumentException("Font size is not allowed to be negative or zero.");
			}
						
			currentFont = fontName;
			currentFontSize = fontSize;
		}
		
		public void DrawText(string text, Vector pos)
		{
			DrawText(text, pos.X, pos.Y);
		}
		
		/// <summary>
		/// Draw text to the screen.
		/// </summary>
		/// <param name="text">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="x">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/>
		/// </param>
		public void DrawText(string text, double x, double y)
		{
			if (currentFont == "")
			{
				throw new InvalidValueException("No font is selected!");
			}

			double scaleFactor = (double)currentFontSize / (double)BASE_FONT_SIZE;
			Gl.glPushMatrix();
			Gl.glTranslated(x, y, 0);
			Gl.glScaled(scaleFactor, scaleFactor, 0);
			ISE.FTFont font = resourceManager.GetFont(currentFont);
			
			font.ftBeginFont();
			font.ftWrite(text) ;
			font.ftEndFont();
			Gl.glPopMatrix();
		}
		#endregion Text rendering functions
		
		#region Texture functions		
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
		/// <param name="bpp">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>. The ID of the texture.
		/// </returns>
		public static Texture GenerateTexture(IntPtr data, int width, int height, int bpp)
		{
			if (!Ready)
			{
				throw new Exception("Renderer has not yet been initialized.");
			}
			int texId;

			int errorCode = Gl.glGetError();
			if (errorCode != 0)
			{
				throw new TextureException("Unable to load texture due to error status. Error code: " + errorCode);
			}
			
			Gl.glGenTextures(1, out texId);
			errorCode = Gl.glGetError();
			if (errorCode != 0)
			{
				throw new TextureException("Unable to generate texture due to error status. Error code: " + errorCode);
			}

			Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
			Gl.glBindTexture(GlTextureMode, texId);
			
			Gl.glTexParameteri(GlTextureMode, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
			Gl.glTexParameteri(GlTextureMode, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
		    Gl.glTexParameteri(GlTextureMode, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
		    Gl.glTexParameteri(GlTextureMode, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
		    //Gl.glTexParameteri(GlTextureMode, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
		    //Gl.glTexParameteri(GlTextureMode, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

			//tex = new Texture(Il.ilGetInteger(Il.IL_IMAGE_WIDTH), Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), texId);
			
			int format;
			switch (bpp)
			{
			case 3:
				format = Gl.GL_RGB;
				break;
			case 24:
				format = Gl.GL_RGB;
				break;
			case 4:
				format = Gl.GL_RGBA;
				break;
			case 32:
				format = Gl.GL_RGBA;
				break;
			default:
				throw new NotSupportedException("Unsupported color depth: " + bpp);
			}
			
			Gl.glTexImage2D(GlTextureMode, 0, format, width, height, 0, format, Gl.GL_UNSIGNED_BYTE, data);
			
			errorCode = Gl.glGetError();
			if (errorCode != 0)
			{
				throw new TextureException("Unable to load texture. Error code: " + errorCode);
			}

			return new Texture(width, height, texId);
		}
		
		/// <summary>
		/// Convert a specified texture coordinate (in pixels from the left or bottom edge of the texture) to the appropriate value for the specified texture mode.
		/// </summary>
		/// <param name="c">
		/// A <see cref="System.Int32"/>. The coordinate (for instance "53" means 53 pixels from the left, or bottom of the texture).
		/// </param>
		/// <param name="size">
		/// A <see cref="System.Int32"/>. The size of the texture for the dimension used for c.
		/// </param>
		/// <returns>
		/// A <see cref="System.Double"/>. The texture coordinate used for rendering.
		/// </returns>
		public static double TextureCoordinate(int c, int size)
		{
			switch (textureMode)
			{
			case TextureMode.NORMAL_MODE:
				return (double)c / (double)size;
			case TextureMode.TEXTURE_RECTANGLE_EXT:
				return (double)c;
			}
			
			return 0;
		}
		
		//// <value>
		/// What kind of texture are we drawing? GL_TEXTURE_2D or GL_TEXTURE_RECTANGLE_ARB?
		/// </value>
		public static int GlTextureMode
		{
			get
			{
				switch (textureMode)
				{
				case TextureMode.NORMAL_MODE:
					return Gl.GL_TEXTURE_2D;
				case TextureMode.TEXTURE_RECTANGLE_EXT:
					return Gl.GL_TEXTURE_RECTANGLE_ARB;
				}
				return Gl.GL_TEXTURE_2D;
			}			
		}
		
		#endregion Texture functions
		
		#region Framebuffer functions
		
		/// <summary>
		/// Accumulate a frame into the accumulation buffer.
		/// </summary>
		/// <param name="amount">
		/// A <see cref="System.Double"/>
		/// </param>
		public void AccumulateFrame(double amount)
		{
			Gl.glAccum(Gl.GL_ACCUM, (float)amount);
		}
		
		/// <summary>
		/// Multiply the current content of the accumulation buffer by a factor.
		/// </summary>
		/// <param name="factor">
		/// A <see cref="System.Double"/>
		/// </param>
		public void MultiplyFrame(double factor)
		{
			Gl.glAccum(Gl.GL_MULT, (float)factor);
		}
		
		/// <summary>
		/// Draw the content of the accumulation buffer to the screen, replacing its current content.
		/// </summary>
		/// <param name="fraction">
		/// A <see cref="System.Double"/>
		/// </param>
		public void DrawFrame(double fraction)
		{
			Gl.glAccum(Gl.GL_RETURN, (float)fraction);
		}
		
		#endregion Framebuffer functions
	}
}
