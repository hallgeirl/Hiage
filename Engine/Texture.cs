
using System;

namespace Engine
{
	
	
	public class Texture
	{
		int width, height, textureId;
		
		public Texture(int width, int height, int textureId)
		{
			this.width = width;
			this.height = height;
			this.textureId = textureId;
		}
		
		public int TextureID
		{
			get
			{
				return textureId;
			}
			
		}
		
		public int Width
		{
			get
			{
				return width;
			}
		}
		
		public int Height
		{
			get
			{
				return height;
			}
		}
	}
}
