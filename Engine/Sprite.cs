
using System;
using System.Collections.Generic;

namespace Engine
{
	
	/// <summary>
	/// Used for creating the sprite.
	/// </summary>
	public class SpriteDescriptor
	{
		public class FrameDescriptor
		{
			public string animationName;
			public int x, y, width, height;
			public int delay, nextFrame;
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
		
		public void AddFrame(string animationName, int x, int y, int width, int height, int delay, int nextFrame)
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
	
	/// <summary>
	/// Used to hold sprite information, like texture, animations and so on.
	/// </summary>
	public class Sprite : IRenderable
	{
		/// <summary>
		/// Handles animations: Incrementing frames, etc.
		/// </summary>
		private class Animation
		{
			int currentFrame = 0;
			int frameCounter = 0;
			
			/// <summary>
			/// Holds frame information (Position on sprite sheet, delay etc.)
			/// </summary>
			public class Frame
			{
				int x, y, width, height;
				int delay;	//For how many frames (in time) should this frame be drawn? delay <= 0 means "always".
				int nextFrame;
				
				public Frame(int x, int y, int width, int height, int delay, int nextFrame)
				{
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					this.delay = delay;
					this.nextFrame = nextFrame;
				}
				
				//// <value>
				/// Left edge of frame
				/// </value>
				public int Left
				{
					get
					{
						return x;
					}
				}
				
				//// <value>
				/// Top edge of frame
				/// </value>
				public int Top
				{
					get
					{
						return y+height;
					}
				}
				
				//// <value>
				/// Right edge of frame
				/// </value>
				public int Right
				{
					get
					{
						return x+width;
					}
				}
				
				//// <value>
				/// Bottom edge of frame
				/// </value>
				public int Bottom
				{
					get
					{
						return y;
					}
				}
				
				//// <value>
				/// How many frames should this one show?
				/// </value>
				public int Delay
				{
					get
					{
						return delay;
					}
				}
				public int NextFrame
				{
					get
					{
						return nextFrame;
					}
				}
			}
			
			List<Frame> frames; //All the frames of this animation
			
			public Animation()
			{
				frames = new List<Frame>();
			}
			
			public void Reset()
			{
				currentFrame = 0;
			}
			
			/// <summary>
			/// Add a new frame to the animation. Make sure 0 < x < width, 0 < y < height, height > 0 and width > 0.
			/// </summary>
			/// <param name="x">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="y">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="width">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="height">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="delay">
			/// A <see cref="System.Int32"/>
			/// </param>
			public void AddFrame(int x, int y, int width, int height, int delay, int nextFrame)
			{
				if (x < 0 || y < 0 || width < 1 || height < 1)
				{
					Log.Write("Animation frame coordinates out of bounds, or invalid dimensions. Skipping frame. x = " + x + ", y = " + y + ", width = " + width + ", height = " + height, Log.WARNING);
				}
				else
				{
					frames.Add(new Frame(x,y,width,height,delay, nextFrame));
				}
			}
			                     
			/// <summary>
			/// Update the animation (go to next frame if ready)
			/// </summary>
			public void Update()
			{
				if (currentFrame >= 0 && currentFrame < frames.Count && frames[currentFrame].Delay > 0)
				{
					frameCounter++;
					if (frameCounter >= frames[currentFrame].Delay)
					{
						frameCounter = 0;
						currentFrame = frames[currentFrame].NextFrame;
						if (currentFrame >= frames.Count)
						{
							throw new IndexOutOfRangeException("Frame " + currentFrame + " out of bounds on animation with " + frames.Count + " frames.");
						}
					}
				}
			}
			
			public Frame CurrentFrame
			{
				get
				{
					if (currentFrame >= 0 && currentFrame < frames.Count)
					{
						return frames[currentFrame];
					}
					throw new IndexOutOfRangeException("Invalid current frame.");
				}
			}
		}

		Texture texture;	//Texture(sprite sheet) used to render the sprite
		double x, y;			//Where is the sprite rendered?
		double rotation;		//Orientation
		string currentAnimation;
		Dictionary<string, Animation> animations = new Dictionary<string, Animation>(); //All animations for this sprite.
		
		public Sprite(Texture tex)
		{
			texture = tex;
			currentAnimation = "NO_ANIMATION";
			animations.Add(currentAnimation, new Animation());
			animations[currentAnimation].AddFrame(0, 0, texture.Width, texture.Height, 0,0);
		}
		
		/// <summary>
		/// Create the sprite from a SpriteDescriptor object. This requires a reference to the resource manager to load the appropriate resource.
		/// </summary>
		/// <param name="spriteDesc">
		/// A <see cref="SpriteDescriptor"/>
		/// </param>
		/// <param name="m">
		/// A <see cref="ResourceManager"/>
		/// </param>
		public Sprite(SpriteDescriptor spriteDesc, ResourceManager m)
		{
			texture = m.GetTexture(spriteDesc.TextureName);

			currentAnimation = "NO_ANIMATION";
			animations.Add(currentAnimation, new Animation());
			animations[currentAnimation].AddFrame(0, 0, texture.Width, texture.Height, 0, 0);
			
			foreach (SpriteDescriptor.FrameDescriptor frame in spriteDesc.Frames)
			{
				if (!animations.ContainsKey(frame.animationName))
				{
					animations.Add(frame.animationName, new Animation());
				}
				animations[frame.animationName].AddFrame(frame.x, frame.y, frame.width, frame.height, frame.delay, frame.nextFrame);
			}
		}
		
		
		public void PlayAnimation(string name)
		{
			currentAnimation = name;
			animations[currentAnimation].Reset();
		}
		
		public void Update()
		{
			animations[currentAnimation].Update();
		}
		
		

#region Position properties

		/// <value>
		/// Left edge
		/// </value>
		public double Left
		{
			get
			{
				return x;
			}
		}
		
		//// <value>
		/// Right edge
		/// </value>
		public double Right
		{
			get
			{
				return x+(currentAnimation == "NO_ANIMATION"?texture.Width:animations[currentAnimation].CurrentFrame.Right-animations[currentAnimation].CurrentFrame.Left);
			}
		}
		
		//// <value>
		/// Top edge
		/// </value>
		public double Top
		{
			get
			{
				return y;
			}
		}
		
		/// <value>
		/// Bottom edge
		/// </value>
		public double Bottom
		{
			get
			{
				return y - (currentAnimation == "NO_ANIMATION"?texture.Height:animations[currentAnimation].CurrentFrame.Bottom-animations[currentAnimation].CurrentFrame.Top);;
			}
		}
		
		public double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}
		
		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}
		
		
		public double Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
			}
		}
#endregion
		
#region Texture properties
		/// <value>
		/// Texture to use for rendering this object
		/// </value>
		public Texture Texture
		{
			get
			{
				return texture;
			}
		}
	
		//// <value>
		/// Left edge of texture
		/// </value>
		public int TextureLeft
		{
			get
			{
				return animations[currentAnimation].CurrentFrame.Left;
			}
		}
		
		//// <value>
		/// Right edge of texture
		/// </value>
		public int TextureRight
		{
			get
			{
				return animations[currentAnimation].CurrentFrame.Right;
			}
		}
		
		//// <value>
		/// Top edge of texture
		/// </value>
		public int TextureTop
		{
			get
			{
				return animations[currentAnimation].CurrentFrame.Top;
			}
		}
		
		//// <value>
		/// Bottom edge of texture
		/// </value>
		public int TextureBottom
		{
			get
			{
				return animations[currentAnimation].CurrentFrame.Bottom;
			}
		}
#endregion
	}
}
