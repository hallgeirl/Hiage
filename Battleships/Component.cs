using Engine;
using System;

namespace Battleships
{
	
	
	public abstract class Component
	{
		private int id;
		static int idCounter = 0;
		
		private Ship parent;
		private int health, cost;
		private double x,y; //Relative position from parent if absolutePosition is false, absolute position otherwise
		private double oldX, oldY;
		private bool absolutePosition;
		private double rotation;
		private Arena arenaref;
		private int width, height;

		
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
		public Component(Ship parent, double x, double y, bool absolutePosition, Arena arena, int width, int height)
		{
			this.height = height;
			this.width = width;
			this.parent = parent;
			this.x = x;
			this.y = y;
			this.absolutePosition = absolutePosition;
			this.arenaref = arena;
			id = idCounter;
			idCounter++;
		}
		
		public override int GetHashCode()
		{
			return id;
		}
		
		public abstract void Fire();
		public abstract void Destroy();
		public abstract void Render();
		public abstract void Update();
		public abstract void CollideWith(Component obj, Edge edge, double collX1, double collY1, double collX2, double collY2);
		
		#region Properties
		
		public int ID
		{
			get
			{
				return id;
			}
		}
		
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
				//Calculate position based on the parent's rotation
				if (x == 0 && y == 0)
					return parent.X;
				
				double dCenter = Math.Sqrt(x*x+y*y); //Distance from center
				double vEnd = parent.Rotation*Math.PI/180 + (y<0?-1:1)*Math.Acos(x/dCenter);
				
				double temp = Math.Cos(vEnd)*dCenter + parent.X;
				
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
				if (x == 0 && y == 0)
					return parent.Y;
				
				double dCenter = Math.Sqrt(x*x+y*y); //Distance from center
				double vEnd = (x<0?-1:1)*parent.Rotation*Math.PI/180 + Math.Asin(y/dCenter);
				
				double temp = Math.Sin(vEnd)*dCenter + parent.Y;

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
			set
			{
				x = value;
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
			set
			{
				y = value;
			}
		}
		
		public double OldX
		{
			get
			{
				return oldX;
			}
			
			set
			{
				oldX = value;
			}
		}
		
		public double OldY
		{
			get
			{
				return oldY;
			}
			
			set
			{
				oldY = value;
			}
		}

		//// <value>
		/// Left boundary
		/// </value>
		public double Left
		{
			get
			{
				return AbsoluteX - Width/2;
			}
		}

		//// <value>
		/// Right boundary
		/// </value>
		public double Right
		{
			get
			{
				return AbsoluteX + Width/2;
			}
		}
		
		public double Top
		{
			get
			{
				return AbsoluteY + Height/2;
			}
		}

		public double Bottom
		{
			get
			{
				return AbsoluteY - Height/2;
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
		
		public bool AbsolutePosition
		{
			get
			{
				return absolutePosition;
			}
		}
		
		//// <value>
		/// Get the width of the object (when rotation is taken into consideration)
		/// </value>
		public int Width
		{
			get
			{
				double rotation = Rotation; //Take it's own rotation into account
				
				if (!absolutePosition)
				{
					rotation += parent.Rotation; //And the parent rotation if the component is connected to that one
				}
				rotation += 90;
				
				//Normalize to 0 <= rotation < 360
				if (rotation > 359)
				{
					rotation %= 360;
				}
				if (rotation < 0)
				{
					rotation += 360 * (Math.Abs((int)(rotation/360)) + 1);
				}
				
				//Convert to radians
				rotation *= Math.PI / 180;
				
				//If the angle is in the second or fourth quadrant, calculate the x of the upper left corner
				if (rotation >= Math.PI/2 && rotation < Math.PI || rotation >= 3*Math.PI/2 && rotation < 2*Math.PI)
				{
					
					double dCenter = Math.Sqrt(width*width/4+height*height/4); //Distance from center
					double vLeft = rotation - Math.Acos((width/2)/dCenter) + Math.PI/2;						
					double left = Math.Abs(Math.Cos(vLeft)*dCenter);

					return (int)(left*2);
				}
				//Else, calculate the lower left
				else
				{
					double dCenter = Math.Sqrt(width*width/4+height*height/4); //Distance from center
					double vLeft = rotation + Math.Acos((width/2)/dCenter) + Math.PI/2;						
					double left = Math.Abs(Math.Cos(vLeft)*dCenter);

					return (int)(left*2);
				}
			}
		}

		//// <value>
		/// Get the height of the object (when rotation is taken into consideration)
		/// </value>
		public int Height
		{
			get
			{
				double rotation = Rotation; //Take it's own rotation into account
				
				if (!absolutePosition)
				{
					rotation += parent.Rotation; //And the parent rotation if the component is connected to that one
				}
				rotation += 90;
				
				//Normalize to 0 <= rotation < 360
				if (rotation > 359)
				{
					rotation %= 360;
				}
				if (rotation < 0)
				{
					rotation += 360 * (Math.Abs((int)(rotation/360)) + 1);
				}
				
				rotation *= Math.PI / 180;
				
				if (rotation >= Math.PI/2 && rotation < Math.PI || rotation >= 3*Math.PI/2 && rotation < 2*Math.PI)
				{
					double dCenter = Math.Sqrt(width*width/4+height*height/4); //Distance from center
					double vBottom = rotation + Math.Acos((width/2)/dCenter) + Math.PI/2;						
					double bottom = Math.Abs(Math.Sin(vBottom)*dCenter);

					return (int)(bottom*2);
				}
				else
				{
					double dCenter = Math.Sqrt(width*width/4+height*height/4); //Distance from center
					double vBottom = rotation - Math.Acos((width/2)/dCenter) + Math.PI/2;						
					double bottom = Math.Abs(Math.Sin(vBottom)*dCenter);

					return (int)(bottom*2);
				}
			}
		}
		
		protected Arena ArenaRef
		{
			get
			{
				return arenaref;
			}
		}
		#endregion
	}
}
