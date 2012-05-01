
using System;
using System.Collections.Generic;

namespace Engine
{
	/// <summary>
	/// Used for creating the sprite. A template or "blueprint".
	/// </summary>
	public class SpriteDescriptor
	{
		public class FrameDescriptor
		{
			public string animationName;
			public int x, y, width, height;
			public double delay;
			public int nextFrame;
		}
		
		private string textureName;
		private List<FrameDescriptor> frames = new List<FrameDescriptor>();
		
		public SpriteDescriptor()
		{
		}
		
		public SpriteDescriptor(string texture)
		{
			textureName = texture;
		}
		
		public string TextureName
		{
			get
			{
				return textureName;
			}
			set
			{
				textureName = value;
			}
		}
		
		public void AddFrame(string animationName, int x, int y, int width, int height, double delay, int nextFrame)
		{
			FrameDescriptor f = new FrameDescriptor();
			
			f.animationName = animationName;
			f.x = x;
			f.y = y;
			f.height = height;
			f.width = width;
			f.delay = delay;
			f.nextFrame = nextFrame;
			
			frames.Add(f);
		}
		
		public List<FrameDescriptor> Frames
		{
			get
			{
				return frames;
			}
		}
		
	}
}
