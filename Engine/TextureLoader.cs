
using System;
using System.IO;
using Tao.DevIl;
using Tao.OpenGl;

namespace Engine
{
	
	/// <summary>
	/// Class for loading textures into the resource manager
	/// </summary>
	public class TextureLoader : IResourceLoader<Texture>
	{

		public TextureLoader()
		{
		}
		
		public Texture LoadResource(string filename)
		{
			int imageId;
			Texture tex;
			
			//Initialize DevIL
			Il.ilInit();
			
			Il.ilGenImages(1, out imageId);
			Il.ilBindImage(imageId);
			
			if (Il.ilLoadImage(filename))
			{
				//Successfully loaded the image. Generate the OpenGL texture
				int texId;
				
				Gl.glGenTextures(1, out texId);
				Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, texId);
				
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
			    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
				
				tex = new Texture(Il.ilGetInteger(Il.IL_IMAGE_WIDTH), Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), texId);
				
				int format;
				switch (Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL))
				{
				case 24:
					format = Gl.GL_RGB;
					break;
				case 32:
					format = Gl.GL_RGBA;
					break;
				default:
					Il.ilDeleteImages(1, ref imageId);
					throw new NotSupportedException("Unsupported color depth: " + Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL));
				}
				
				Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, format, tex.Width, tex.Height, 0, format, Gl.GL_UNSIGNED_BYTE, Il.ilGetData());
				
			}
			else
			{
				//Failure. Clean up loaded image.
				Il.ilDeleteImages(1, ref imageId);
				throw new IOException("Unable to load texture \"" + filename + "\".");
			}
			Il.ilDeleteImages(1, ref imageId);
			
			return tex;
		}
	}
}
