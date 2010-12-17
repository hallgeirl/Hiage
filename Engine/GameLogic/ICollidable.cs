
using System;
using System.Collections.Generic;

namespace Engine
{
	public class BoundingPolygon : ICloneable
	{
		public BoundingPolygon() 
		{
			Vertices = new List<Vector>();
		}
		
		public BoundingPolygon(List<Vector> vertices)
		{
			Vertices = new List<Vector>();
			foreach (var v in vertices)
			{
				Vertices.Add((Vector)v.Clone());
			}
			
		}
		
		public object Clone()
		{
			return new BoundingPolygon(Vertices);
		}

		/// <summary>
		/// Vertices of polygon. 
		/// Edges are implicit between consecutive vertices in a cyclic manner, 
		/// so that there is an edge between the last and first vertex.
		/// </summary>
		public List<Vector> Vertices
		{
			get;
			private set;
		}
		
		public BoundingPolygon Translate(Vector v)
		{
			foreach (var vert in Vertices)
			{
				vert.X += v.X;
				vert.Y += v.Y;
			}
			
			return this;
		}
		
		public override string ToString()
		{
			String result = "BoundingPolygon(vertices=";
			bool first = true;
			foreach (var v in Vertices)
			{
				if (!first)
					result += ",";
				
				first = false;
				result += v;
			}
			result += ")";
			return result;
		}
	}
	
	/// <summary>
	/// Bounding box class
	/// </summary>
	public class BoundingBox : BoundingPolygon
	{
		public BoundingBox(double left, double top, double right, double bottom) : base()
		{
			Vertices.Add(new Vector(left, top));
			Vertices.Add(new Vector(right, top));
			Vertices.Add(new Vector(right, bottom));
			Vertices.Add(new Vector(left, bottom));
		}
		
		public double Left 
		{ 
			get { return Vertices[0].X; } 
		}
		
		public double Right
		{ 
			get { return Vertices[1].X; }
		}
		
		public double Top
		{ 
			get { return Vertices[0].Y; }
		}
		
		public double Bottom
		{ 
			get { return Vertices[2].Y; }
		}
		
		public double Width
		{
			get { return Right - Left; }
		}
		
		public double Height
		{
			get { return Top - Bottom; }
		}
		
		/*public override string ToString()
		{
			return "BoundingBox(left=" + Left + ",top=" + Top + ",right=" + Right + ",bottom=" + Bottom + ")";
		}*/
	}
	/// <summary>
	/// Interface for all objects that may collide.
	/// </summary>
	public interface ICollidable
	{
		//Collision handler for collision vs edge
		void Collide(Edge e, CollisionResult collisionResult);
		
		//Collision handler for collision vs another collidable object
		//edgeNormal is the normal vector for the edge of o that this object collided with.
		void Collide(ICollidable o, Vector edgeNormal, CollisionResult collisionResult);
		
		// Bounding box for the collidable object before updating
		BoundingBox BoundingBox
		{
			get;
		}
		
		//Velocity of object, if any. May return null if it does not apply. 
		Vector Velocity
		{
			get;
		}
	}
}
