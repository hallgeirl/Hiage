
using System;
using System.Collections.Generic;

namespace Engine
{
	
	
	
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
			double frameTimer = 0;
			
			/// <summary>
			/// Holds frame information (Position on sprite sheet, delay etc.)
			/// </summary>
			public class Frame
			{
				int x, y, width, height;
				double delay;	//For how many seconds should this frame be drawn? delay <= 0 means "always".
				int nextFrame;
				
				public Frame(int x, int y, int width, int height, double delay, int nextFrame)
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
						return y;
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
						return y+height;
					}
				}
				
				//// <value>
				/// How long should this frame show?
				/// </value>
				public double Delay
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
			public void AddFrame(int x, int y, int width, int height, double delay, int nextFrame)
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
			public void Update(double frameTime)
			{
				if (currentFrame >= 0 && currentFrame < frames.Count && frames[currentFrame].Delay > 0)
				{
					frameTimer += frameTime;
					//if (frameCounter >= frames[currentFrame].Delay)
					if (frameTimer >= frames[currentFrame].Delay)
					{
						frameTimer = 0;
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
			Flipped = false;
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
			Flipped = false;
		}
		
		public bool HasAnimation(string name)
		{
			return animations.ContainsKey(name);
		}
		
		
		public void PlayAnimation(string name, bool reset)
		{
			if (animations.ContainsKey(name))
			{
				currentAnimation = name;
				if (reset)
					animations[currentAnimation].Reset();
			}
		}
		
		public void Update(double frameTime)
		{
			animations[currentAnimation].Update(frameTime);
		}
		
		

		#region Position properties
		
		//// <value>
		/// Right edge
		/// </value>
		public double Width
		{
			get
			{
				return (currentAnimation == "NO_ANIMATION" ? texture.Width 
				        								   : animations[currentAnimation].CurrentFrame.Right - animations[currentAnimation].CurrentFrame.Left);
			}
		}
		
	
		/// <value>
		/// Bottom edge
		/// </value>
		public double Height
		{
			get
			{
				return (currentAnimation == "NO_ANIMATION" ? texture.Height 
				        								   : animations[currentAnimation].CurrentFrame.Bottom-animations[currentAnimation].CurrentFrame.Top);
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
		
		public bool Flipped
		{
			get;
			set;
		}
		
		#endregion Position properties
		
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
				if (Flipped)
					return animations[currentAnimation].CurrentFrame.Right;
				else
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
				if (Flipped)
					return animations[currentAnimation].CurrentFrame.Left;
				else
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
		#endregion Texture properties
	}
}
