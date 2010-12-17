
using System;
using System.Collections.Generic;

namespace Engine
{
	public class BoundingPolygon : ICloneable
	{
		public BoundingPolygon() 
		{
			vertices = new List<Vector>();
			edgeNormals = new List<Vector>();
		}
		
		public BoundingPolygon(List<Vector> vertices)
		{
			vertices = new List<Vector>();
			foreach (var v in vertices)
			{
				vertices.Add((Vector)v.Clone());
			}
			buildNormals();
		}
		
		/// <summary>
		/// Build the edge normals.
		/// </summary>
		private void buildNormals()
		{
			edgeNormals = new List<Vector>();
			
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector edge = vertices[i == vertices.Count - 1 ? 0 : i+1] - vertices[i];
				Vector normal = new Vector(-edge.Y, edge.X);
				normal.Normalize();
				edgeNormals.Add(normal);
			}
		}
		
		/// <summary>
		/// From ICloneable
		/// </summary>
		public object Clone()
		{
			return new BoundingPolygon(Vertices);
		}
		
		/// <summary>
		/// Add a new vertex, rebuilding edge normals as neccesary.
		/// </summary>
		public void AddVertex(Vector vertex)
		{
			vertices.Add(vertex);
			edgeNormals.Add(new Vector());
			
			//Calculate the last two edge normals as they have changed.
			for (int i = vertices.Count - 2; i < vertices.Count; i++)
			{
				Vector edge = vertices[i == vertices.Count - 1 ? 0 : i+1] - vertices[i];
				Vector normal = new Vector(-edge.Y, edge.X);
				normal.Normalize();
				edgeNormals[i] = normal;
			}
		}
		
		public void AddVertices(List<Vector> verts)
		{
			vertices.AddRange(verts);
			
			for (int i = 0; i < verts.Count; i++)
				edgeNormals.Add(new Vector());
			
			//Rebuild last vertices
			for (int i = Math.Max(vertices.Count - 1 - verts.Count, 0); i < vertices.Count; i++)
			{
				Vector edge = vertices[i == vertices.Count - 1 ? 0 : i+1] - vertices[i];
				Vector normal = new Vector(-edge.Y, edge.X);
				normal.Normalize();
				edgeNormals[i] = normal;
			}
		}
		
		/// <summary>
		/// Edge normals.
		/// </summary>
		public List<Vector> EdgeNormals
		{
			get { return edgeNormals; }
		}
		private List<Vector> edgeNormals;

		/// <summary>
		/// Vertices of polygon. 
		/// Edges are implicit between consecutive vertices in a cyclic manner, 
		/// so that there is an edge between the last and first vertex.
		/// </summary>
		public List<Vector> Vertices
		{
			get { return vertices; }
		}
		private List<Vector> vertices;
		
		/// <summary>
		/// Translate the whole polygon by a translation vector.
		/// </summary>
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
			List<Vector> verts = new List<Vector>();
			verts.Add(new Vector(left, top));
			verts.Add(new Vector(right, top));
			verts.Add(new Vector(right, bottom));
			verts.Add(new Vector(left, bottom));
			AddVertices(verts);
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
