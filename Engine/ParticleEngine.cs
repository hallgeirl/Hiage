
using System;
using System.Collections.Generic;

namespace Engine
{
	
	
	/// <summary>
	/// Class to handle particles.
	/// </summary>
	public class ParticleEngine
	{
		private class Particle : IRenderable
		{
			Vector position, velocity, accelleration;
			bool gradualFade;	//Does the particle gradually fade away?
			int timeToLive;		//How many frames do this particle have left to live?
			int totalLifetime;  //How long this particle lives in total.
			Texture texture;
			double red, green, blue, alpha;
			int size;
			
			/// <summary>
			/// Construct a new particle.
			/// </summary>
			/// <param name="t">
			/// A <see cref="Texture"/>. Texture used as a base for the particle.
			/// </param>
			/// <param name="position">
			/// A <see cref="Vector"/>. Current position of the particle.
			/// </param>
			/// <param name="velocity">
			/// A <see cref="Vector"/>. Velocity of the particle.
			/// </param>
			/// <param name="accelleration">
			/// A <see cref="Vector"/>. Accelleration of the particle.
			/// </param>
			/// <param name="red">
			/// A <see cref="System.Double"/>. Represents the amount of red color in the particle. Must be between 0.0 and 1.0 (inclusive).
			/// </param>
			/// <param name="green">
			/// A <see cref="System.Double"/>. Represents the amount of green color in the particle. Must be between 0.0 and 1.0 (inclusive).
			/// </param>
			/// <param name="blue">
			/// A <see cref="System.Double"/>. Represents the amount of blue color in the particle. Must be between 0.0 and 1.0 (inclusive).
			/// </param>
			/// <param name="alpha">
			/// A <see cref="System.Double"/>. Represents the alpha value of the particle. Must be between 0.0 and 1.0 (inclusive). 0.0 means invisible, 1.0 means completely opaque.
			/// </param>
			/// <param name="gradualFade">
			/// A <see cref="System.Boolean"/>
			/// </param>
			public Particle(Texture t, int timeToLive, Vector position, Vector velocity, Vector accelleration, double red, double green, double blue, double alpha, bool gradualFade, int size)
			{
				texture = t;
				this.position = position;
				this.velocity = velocity;
				this.accelleration = accelleration;
				this.timeToLive = timeToLive;
				this.totalLifetime = timeToLive;
				
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
				this.red = red;
				this.blue = blue;
				this.green = green;
				this.alpha = alpha;
				this.gradualFade = gradualFade;
				this.size = size;
			}
				
			/// <summary>
			/// Update particle velocity and position
			/// </summary>
			public void Update()
			{
				velocity += accelleration;
				position += velocity;
				TimeToLive--;
			}
			
			//// <value>
			/// How many frames before the particle dies?
			/// </value>
			public int TimeToLive
			{
				get
				{
					return timeToLive;
				}
				set
				{
					if (value < 0)
					{
						timeToLive = 0;
					}
					else
					{
						timeToLive = value;
					}
				}
			}
			
			public void Render(IRenderer renderer)
			{
				renderer.Render(this, red, green, blue, alpha);
			}
			
			#region IRenderable implementation
			//// <value>
			/// Left edge
			/// </value>
			public double Left
			{
				get
				{
					return position.X;// - size/2;
				}
			}
			
			//// <value>
			/// Right edge
			/// </value>
			public double Right
			{
				get
				{
					return position.X + size;
				}
			}
			
			//// <value>
			/// Top edge
			/// </value>
			public double Top
			{
				get
				{
					return position.Y + size/2;
				}
			}
			
			//// <value>
			/// Bottom edge
			/// </value>
			public double Bottom
			{
				get
				{
					return position.Y - size/2;
				}
			}
			
			//// <value>
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
					return 0;
				}
			}
			
			//// <value>
			/// Right edge of texture
			/// </value>
			public int TextureRight
			{
				get
				{
					return texture.Width;
				}
			}
			
			//// <value>
			/// Top edge of texture
			/// </value>
			public int TextureTop
			{
				get
				{
					return 0;
				}
			}
			
			//// <value>
			/// Bottom edge of texture
			/// </value>
			public int TextureBottom
			{
				get
				{
					return texture.Height;
				}
			}
			
			public double Rotation
			{
				get
				{
					return 0;
				}
			}
			#endregion IRenderable implementation
		}
		
		int maxParticles;
		List<Particle> particles = new List<Particle>();
		Texture texture;
		IRenderer renderer;
		
		public ParticleEngine(Texture texture, int maxParticles, IRenderer renderer)
		{
			this.maxParticles = maxParticles;
			this.texture = texture;
			this.renderer = renderer;
		}
		
		//Spawn a new particle
		public void SpawnParticle(int timeToLive, Vector position, Vector velocity, Vector accelleration, double red, double green, double blue, double alpha, bool gradualFade, int size)
		{
			if (particles.Count < maxParticles)
			{
				particles.Add(new Particle(texture, timeToLive, position, velocity, accelleration, red, green, blue, alpha, gradualFade, size));
			}
		}
		
		/// <summary>
		/// Update the particle engine. Move and render all particles.
		/// </summary>
		public void UpdateAndRender()
		{
			for (int i = 0; i < particles.Count && particles.Count > 0; i++)
			{
				particles[i].Update();
				
				particles[i].Render(renderer);
				
				//Remove the particle if it's expired
				if (particles[i].TimeToLive == 0)
				{
					Particle p = particles[particles.Count-1];
					particles[particles.Count-1] = particles[i];
					particles[i] = p;
					
					particles.RemoveAt(particles.Count-1);
					i--;
				}
				
			}
		}	
	}
}
