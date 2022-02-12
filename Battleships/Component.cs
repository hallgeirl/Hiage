using Engine;

namespace Battleships
{
	
	
	public abstract class Component
	{
		private Ship parent;
		private int health, cost;
		private double x,y; //Relative position from parent
		
		public Component(Ship parent, double x, double y)
		{
			this.parent = parent;
			this.x = x;
			this.y = y;
		}
		
		public abstract void Fire(Vector direction);
		
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
		
		public Ship Parent
		{
			get
			{
				return parent;
			}
		}
		
		public double AbsoluteX
		{
			get
			{
				return x + parent.X;
			}
		}
		
		public double AbsoluteY
		{
			get
			{
				return y + parent.Y;
			}
		}
		
		public double X
		{
			get
			{
				return x;
			}
		}
		
		public double Y
		{
			get
			{
				return y;
			}
		}
	}
}
