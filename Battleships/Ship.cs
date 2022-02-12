using System.Collections.Generic;
using Engine;
using System;

namespace Battleships
{
	
	
	public class Ship
	{
		Vector velocity = new Vector(0, 0), position;
		double direction;
		Vector center; //Central point (rotation point) of the ship
		
		Game gameref;
		List<Component> components = new List<Component>();
				
		public Ship(Vector pos, Game game, Vector center)
		{
			position = pos;
			gameref = game;
			this.center = center;
		}
		
		//Add a new component to this ship
		public void AddComponent(Component obj)
		{
			components.Add(obj);
		}
		
		public void Render()
		{
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
			velocity += new Vector(-Math.Sin(direction*2*Math.PI/360)*magnitude, Math.Cos(direction*2*Math.PI/360)*magnitude);
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
					velocity -= velocity.Copy.Normalize()*magnitude;
				}
			}
		}
		
		/// <summary>
		/// Update the position and all components
		/// </summary>
		public void Update()
		{
			position += velocity;
			foreach (Component c in components)
			{
				
				c.Update();
			}
		}
		
		#region Properties
		public Game GameRef
		{
			get
			{
				return gameref;
			}
		}
		
		public List<Component> Components
		{
			get
			{
				return components;
			}
		}

		public double X
		{
			get
			{
				return position.X;
			}
		}
		
		public double Y
		{
			get
			{
				return position.Y;
			}
		}
		
		public double CenterX
		{
			get
			{
				return center.X;
			}
		}
		public double CenterY
		{
			get
			{
				return center.Y;
			}
		}
		
		public double Rotation
		{
			get
			{
				return direction;
			}
			
			set
			{
				direction = value;
				if (direction > 359)
				{
					direction %= 360;
				}
				if (direction < 0)
				{
					direction += 360 * (Math.Abs((int)(direction/360)));
				}
			}
		}
		
		public Vector Velocity
		{
			get
			{
				return velocity;
			}
		}
		
		#endregion Properties
	}
}
