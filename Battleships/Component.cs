using Engine;
using System;

namespace Battleships
{
	
	
	public abstract class Component
	{
		private Ship parent;
		private int health, cost;
		private double x,y; //Relative position from parent if absolutePosition is false, absolute position otherwise
		private bool absolutePosition, center;
		private double rotation;
		
		/// <summary>
		/// Construct a new 
		/// </summary>
		/// <param name="parent">
		/// A <see cref="Ship"/>
		/// </param>
		/// <param name="x">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/>
		/// </param>
		public Component(Ship parent, double x, double y, bool absolutePosition, bool center)
		{
			this.parent = parent;
			this.x = x;
			this.y = y;
			this.absolutePosition = absolutePosition;
			this.center = center;
		}
		
		public abstract void Fire(Vector direction);
		public abstract void Render();
		public abstract void Update();
		
		#region Properties
		/// <value>
		/// Health of the component
		/// </value>
		public int Health
		{
			set				
			{
				health = value;
			}
				
			get
			{
				return health;
			}
		}
		
		//// <value>
		/// Cost of the component
		/// </value>
		public int Cost
		{
			set
			{
				cost = value;
			}
			
			get
			{
				return cost;
			}
		}
		
		//// <value>
		/// Container of this component
		/// </value>
		public Ship Parent
		{
			get
			{
				return parent;
			}
		}
		
		//// <value>
		/// Absolute X position (map position)
		/// </value>
		public double AbsoluteX
		{
			get
			{
				if (absolutePosition)
				{
					return x;
				}
				double temp = parent.X;
				if (!center)
				{
					temp -= Math.Sin(parent.Rotation*Math.PI/180)*(parent.CenterY);
				}
				else
				{
					temp += x;
				}
				return temp;
			}
		}
		
		//// <value>
		/// Absolute Y position
		/// </value>
		public double AbsoluteY
		{
			get
			{
				if (absolutePosition)
				{
					return y;
				}
				//Calculate position based on the parent's rotation
				
				double temp = parent.Y;
				if (!center)
				{
					temp +=  Math.Cos(parent.Rotation*Math.PI/180)*parent.CenterY-parent.CenterY;
				}
				else
				{
					temp += y;
				}
				return temp;
			}
		}
		
		//// <value>
		/// X position relative to the parent if absolutePosition is false, absolute position otherwise.
		/// </value>
		public double X
		{
			get
			{
				return x;
			}
		}
		
		//// <value>
		/// Y position relative to the parent if absolutePosition is false, absolute position otherwise.
		/// </value>
		public double Y
		{
			get
			{
				return y;
			}
		}
		
		//// <value>
		/// Rotation of this component
		/// </value>
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
	}
}
