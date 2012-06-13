
using System;
using System.ComponentModel;

namespace Engine
{
	
	/// <summary>
	/// Mathematical 2D vector class which can be used for positioning, directions etc.
	/// </summary>
	public class Vector : ICloneable
	{
		double[] x = new double[2];
		
		public Vector()
		{
			x[0] = 0;
			x[1] = 0;
		}
		
		public Vector(double x, double y)
		{
			X = x; 
			Y = y; 
			//this.x = x;
			//this.y = y;
		}
		
		public Vector(Vector v)
		{
			X = v.X; 
			Y = v.Y; 
			//this.x = v.x;
			//this.y = v.y;
		}
		
		/// <summary>
		/// Add a vector v to this vector, and return the results.
		/// </summary>
		/// <param name="v">
		/// A <see cref="Vector"/>
		/// </param>
		/// <returns>
		/// The resulting <see cref="Vector"/>
		/// </returns>
		public Vector Add(Vector v)
		{
			
			X += v.X;
			Y += v.Y;
			
			return this;
		}
		
		/// <summary>
		/// Subtract a vector v from this vector, and return the results.
		/// </summary>
		/// <param name="v">
		/// A <see cref="Vector"/>
		/// </param>
		/// <returns>
		/// A <see cref="Vector"/>
		/// </returns>
		public Vector Subtract(Vector v)
		{
			x[0] -= v.X;
			x[1] -= v.Y;
			
			return this;
		}
		
		/// <summary>
		/// Multiply both components by scalar.
		/// </summary>
		/// <param name="scalar">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// The resulting <see cref="Vector"/>
		/// </returns>
		public Vector Scale(double scalar)
		{
			x[0] *= scalar;
			x[1] *= scalar;
			
			return this;
		}
		
		//Elementwise multiplication
		public Vector Multiply(Vector v)
		{
			x[0] *= v.X;
			x[1] *= v.Y;
			
			return this;
		}
		
		/// <summary>
		/// Calculates the dot product of this vector and v.
		/// </summary>
		/// <param name="v">
		/// A <see cref="Vector"/>
		/// </param>
		/// <returns>
		/// The dot product (or scalar product).
		/// </returns>
		public double DotProduct(Vector v)
		{
			return (x[0] * v.X + x[1] * v.Y);
		}
		
		public Vector Normalize()
		{
			if (Length > 0)
				Scale(1/Length);
			
			return this;
		}
		
		public object Clone()
		{
			return new Vector(X, Y);
		}

		public Vector Copy()
		{
			return new Vector(X, Y);
		}
		
		public void Set(double x, double y)
		{
			X = x; 
			Y = y;
		}
		
		public void Set(Vector v)
		{
			X = v.x[0]; Y = v.x[1];
		}
		
		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}
		
		#region Operator overloads
		public double this[int key]
		{
			get
			{
				return x[key];
			}
			set
			{
				x[key] = value;
			}
		}
		
		public static Vector operator +(Vector v1, Vector v2)
		{
			return (new Vector(v1.X+v2.X, v1.Y+v2.Y));
		}

		public static Vector operator -(Vector v1, Vector v2)
		{
			return (new Vector(v1.X-v2.X, v1.Y-v2.Y));
		}
		
		public static Vector operator *(Vector v, double scalar)
		{
			return (new Vector(v.X * scalar, v.Y * scalar));
		}
		
		public static Vector operator *(double scalar, Vector v)
		{
			return (new Vector(v.X * scalar, v.Y * scalar));
		}
		
		public static double operator *(Vector v1, Vector v2)
		{
			return (v1.X*v2.X+v1.Y*v2.Y);
		}
		
		public static Vector operator /(Vector v, double scalar)
		{
			return (new Vector(v.X / scalar, v.Y / scalar));
		}

		public static Vector operator /(double scalar, Vector v)
		{
			return (new Vector(v.X / scalar, v.Y / scalar));
		}
		
		public static Vector operator-(Vector v)
		{
			return new Vector(-v.X, -v.Y);
		}

		public static bool operator==(Vector v1, Vector v2)	
		{
			if (object.ReferenceEquals(v1, null) && object.ReferenceEquals(v2, null))
			     return true;
			else if (object.ReferenceEquals(v1, null) || object.ReferenceEquals(v2, null))
			    return false;
			else return v1.X == v2.X && v1.Y == v2.Y;
		}
		
		public static bool operator!=(Vector v1, Vector v2)
		{
			return !(v1 == v2);
		}
		
		//Simply a length check. If v1 is shorter than v2, return true.
		public static bool operator<(Vector v1,  Vector v2)
		{
			return (v1.DotProduct(v1) < v2.DotProduct(v2));
		}
		
		public static bool operator>(Vector v1,  Vector v2)
		{
			return (v1.DotProduct(v1) > v2.DotProduct(v2));
		}
		
		public override bool Equals (object obj)
		{
			return this == (Vector)obj;
		}
		
		
		public override int GetHashCode ()
		{
			return (int)Length;
		}


		
		#endregion Operator overloads
		
		#region Properties
		/// <value>
		/// The X component of the vector.
		/// </value>
		public double X
		{
			get
			{
				return x[0];
			}
			set
			{
				x[0] = value;
			}
		}
		
		//// <value>
		/// The Y component of the vector.
		/// </value>
		public double Y
		{
			get
			{
				return x[1];
			}
			set
			{
				x[1] = value;
			}
		}
		
		public double Length
		{
			get
			{
				return Math.Sqrt(x[0]*x[0]+x[1]*x[1]);
			}
		}
		
		public Vector Normalized
		{
			get
			{
				return Copy().Normalize();
			}
		}

		
		#endregion Properties
		
	}
}
