
using System;

namespace Engine
{
	
	/// <summary>
	/// Mathematical 2D vector class which can be used for positioning, directions etc.
	/// </summary>
	public class Vector
	{
		double x, y;
		
		public Vector()
		{
		}
		
		public Vector(double x, double y)
		{
			this.x = x;
			this.y = y;
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
			x += v.X;
			y += v.Y;
			
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
			x -= v.X;
			y -= v.Y;
			
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
			x *= scalar;
			y *= scalar;
			
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
			return (x * v.X + y * v.Y);
		}
		
		#region Operator overloads
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
		
		public static double operator *(Vector v1, Vector v2)
		{
			return (v1.X*v2.X+v1.Y*v2.Y);
		}
		
		#endregion Operator overloads
		
		#region Properties
		//// <value>
		/// The X component of the vector.
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
		/// The Y component of the vector.
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
		#endregion Properties
	}
}
