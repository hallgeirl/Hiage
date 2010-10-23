using System.Collections.Generic;
using Engine;
using System;

namespace Battleships
{
	
	/// <summary>
	/// 
	/// </summary>
	public class Ship
	{
		/// <summary>
		/// This class will handle exhaust (FLAMES!) from the ships.
		/// </param>
		private class ExhaustPort
		{
			int x, y, size, lifetime;
			double spread;
			Ship parent;
			
			/// <summary>
			/// Construct an exhaust port object.
			/// </summary>
			/// <param name="x">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="y">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="size">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="spread">
			/// A <see cref="System.Double"/>
			/// </param>
			/// <param name="lifetime">
			/// A <see cref="System.Int32"/>
			/// </param>
			/// <param name="parent">
			/// A <see cref="Ship"/>
			/// </param>
			public ExhaustPort(int x, int y, int size, double spread, int lifetime, Ship parent)
			{
				this.x = x;
				this.y = y;
				this.size = size;
				this.spread = spread;
				this.parent = parent;
				this.lifetime = lifetime;
			}
			
			/// <summary>
			/// Fire exhaust.
			/// </summary>
			/// <param name="direction">
			/// A <see cref="System.Double"/>. Direction in degrees.
			/// </param>
			public void Fire(double speed)
			{
				double exhaustX, exhaustY;
				
				//Calculate position based on the parent's rotation
				if (x == 0 && y == 0)
				{
					exhaustX = parent.X;
					exhaustY = parent.Y;
				}
				
				double sinRot = Math.Sin(parent.Rotation*Math.PI/180);
				double cosRot = Math.Cos(parent.Rotation*Math.PI/180);
				
				double dCenter = Math.Sqrt(x*x+y*y); //Distance from center
				double vEndX = parent.Rotation*Math.PI/180 + (y<0?-1:1)*Math.Acos(x/dCenter);
				
				exhaustX = Math.Cos(vEndX)*dCenter + parent.X;
				
				double vEndY = (x<0?-1:1)*parent.Rotation*Math.PI/180 + Math.Asin(y/dCenter);
				
				exhaustY = Math.Sin(vEndY)*dCenter + parent.Y;
				
				//Vector to add to the final velocity, based on direction and speed
				Vector directionMod = new Vector(sinRot, -cosRot)*speed;
				

				parent.particles.SpawnParticle(lifetime, 
				                        new Vector(exhaustX, exhaustY), //Position
				                        new Vector(((Rnd.Next()-0.5)*spread), ((Rnd.Next()-0.5)*spread))*1 + parent.Velocity + directionMod, //Velocity
				                        directionMod*-0.015, //Accelleration
				                        1, 					//Red
				                        Rnd.Next(), 	//Green
				                        0, 					//Blue
				                        1, 					//Alpha
				                        true, 				//Fade
				                        size-(int)(Rnd.Next()*(size-1)));	//Size
			}
		}
		
		Vector velocity = new Vector(0, 0), position;
		double rotation;
		
		Game gameref;
		Arena arenaref;
		ParticleEngine particles;
		
		//Components of the ship
		List<Component> components = new List<Component>();
		List<ExhaustPort> exhaustports = new List<ExhaustPort>();
		
		
		public Ship(Vector pos, Game game, ParticleEngine particleEngine, Arena arena)
		{
			position = pos;
			gameref = game;
			particles = particleEngine;
			arenaref = arena;
		}
		
		//Add a new component to this ship and to the arena
		public void AddComponent(Component obj)
		{
			components.Add(obj);
			arenaref.AddObject(obj);
		}
		
		public void RemoveComponent(Component obj)
		{
			if (components.Remove(obj))
			{
				arenaref.Grid.Remove(obj, obj.AbsoluteX, obj.AbsoluteY, obj.Width, obj.Height);
			}
			    
		}
		
		/// <summary>
		/// Add a new exhaust port on the ship.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="size">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="spread">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="lifetime">
		/// A <see cref="System.Int32"/>
		/// </param>
		public void AddExhaustPort(int x, int y, int size, double spread, int lifetime)
		{
			exhaustports.Add(new ExhaustPort(x, y, size, spread, lifetime, this));
		}
		
		/// <summary>
		/// Render all components.
		/// </summary>
		public void Render()
		{
			gameref.Display.Renderer.SetDrawingColor(1,1,1,1);
			foreach (Component c in components)
			{
				c.Render();
			}
		}
	
		
		/// <summary>
		/// Accellerate along a specified vector.
		/// </summary>
		/// <param name="accelvector">
		/// A <see cref="Vector"/>
		/// </param>
		public void Accellerate(Vector accelvector)
		{
			velocity += accelvector;
		}
		
		/// <summary>
		/// Accellerate in the direction the ship is facing.
		/// </summary>
		/// <param name="magnitude">
		/// A <see cref="System.Double"/>
		/// </param>
		public void Accellerate(double magnitude)
		{
			velocity += new Vector(-Math.Sin(rotation*2*Math.PI/360)*magnitude, Math.Cos(rotation*2*Math.PI/360)*magnitude);
		}
		
		/// <summary>
		/// Apply negative accelleration until the ship stops.
		/// </summary>
		/// <param name="magnitude">
		/// A <see cref="System.Double"/>
		/// </param>
		public void Brake(double magnitude)
		{
			if (velocity.Length > 0)
			{
				if (velocity.Length < magnitude)
				{
					velocity.X = 0;
					velocity.Y = 0;
				}
				else
				{
					velocity -= ((Vector)velocity.Clone()).Normalize()*magnitude;
				}
			}
		}
		
		/// <summary>
		/// Update the position and all components
		/// </summary>
		public void Update()
		{
			//Increment position
			position += velocity;
			//Console.WriteLine(components.Count);
			
			//Update all components
			for (int i = 0; i < components.Count && components.Count > 0; i++)
			{
				Component c = components[i];
				
				//Store the old dimensions and positions
				int oldWidth = c.Width, oldHeight = c.Height;
				if (c.AbsolutePosition)
				{
					c.OldX = c.AbsoluteX;
					c.OldY = c.AbsoluteY;
				}
				else
				{
					c.OldX = c.AbsoluteX - velocity.X;
					c.OldY = c.AbsoluteY - velocity.Y;
				}
				
				//Update the component
				c.Update();
				try
				{
					//If neccesary, move the object in the grid.
					arenaref.MoveObject(c, c.OldX, c.OldY, c.AbsoluteX, c.AbsoluteY, oldWidth, oldHeight, c.Width, c.Height);
				}
				catch (IndexOutOfRangeException e)
				{
					if (c is SimpleBullet)
					{
						arenaref.Grid.Remove(c, c.OldX, c.OldY, oldWidth, oldHeight);
						components.Remove(c);
						i--;
						continue;
					}
					else 
					{
						throw e;
					}
				}
				
				//Collect a list of all objects in the surrounding grid squares
				IList<Component> near = arenaref.Grid.GetSurroundingObjects(c.AbsoluteX, c.AbsoluteY, c.Width, c.Height, 1);
				foreach (Component comp in near)
				{
					if (comp.Parent != c.Parent)
					{
						//Schedule a collision test if the components doesn't belong to the same parents
						arenaref.AddCollisionTest(c, comp);
					}
				}
			}
		}
		
		/// <summary>
		/// Tell all components to fire their weapons
		/// </summary>
		public void Fire()
		{
			int componentCount = components.Count;
			for (int i = 0; i < componentCount; i++)
			{
				components[i].Fire();
			}
		}
		
		/// <summary>
		/// Fire exhaust from all ports
		/// </summary>
		public void FireExhaust(double speed)
		{
			foreach (ExhaustPort e in exhaustports)
			{
				e.Fire(speed);
			}
		}
		
		#region Properties
		//// <value>
		/// Reference to the game object.
		/// </value>
		public Game GameRef
		{
			get
			{
				return gameref;
			}
		}
		
		//// <value>
		/// Components of this ship.
		/// </value>
		public List<Component> Components
		{
			get
			{
				return components;
			}
		}

		//// <value>
		/// X position.
		/// </value>
		public double X
		{
			get
			{
				return position.X;
			}
			
			set
			{
				position.X = value;
			}
		}
		
		//// <value>
		/// Y position.
		/// </value>
		public double Y
		{
			get
			{
				return position.Y;
			}
			
			set
			{
				position.Y = value;
			}
		}
		
		//// <value>
		/// Rotation (in degrees)
		/// </value>
		public double Rotation
		{
			get
			{
				return rotation;
			}
			
			set
			{
				double oldRot = rotation;
				
				//Make sure all components can be found in the correct squares after rotation
				for (int i = 0; i < components.Count && components.Count > 0; i++)
				{
					Component c = components[i];
					int oldWidth = c.Width, oldHeight = c.Height;
					double oldX = c.AbsoluteX, oldY = c.AbsoluteY;
					rotation = value;
					try
					{
						arenaref.MoveObject(c, oldX, oldY, c.AbsoluteX, c.AbsoluteY, oldWidth, oldHeight, c.Width, c.Height);
					}
					catch (IndexOutOfRangeException e)
					{
						if (c is SimpleBullet)
						{
							
						}
						else
						{
							throw e;
						}
					}
					rotation = oldRot;
				}
				
				rotation = value;
				if (rotation > 359)
				{
					rotation %= 360;
				}
				if (rotation < 0)
				{
					rotation += 360 * (Math.Abs((int)(rotation/360)) + 1);
				}
			}
		}
		
		public Vector Direction
		{
			get
			{
				return new Vector(-Math.Sin(Rotation*Math.PI/180), Math.Cos(Rotation*Math.PI/180));
			}
		}
		
		//// <value>
		/// Velocity.
		/// </value>
		public Vector Velocity
		{
			get
			{
				return velocity;
			}
			set
			{
				velocity = value;
			}
		}
		
		public ParticleEngine Particles
		{
			get
			{
				return particles;
			}
		}
		
		#endregion Properties
	}
}
