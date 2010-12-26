using System;
using System.Collections.Generic;

namespace Engine
{
	public class BoundingPolygon : ICloneable
	{
		//Untranslated vertices. That is, centered around (0, 0).
		protected List<Vector> vertices;
		//Translated vertices, centered around some coordinate (x,y).
		protected List<Vector> verticesTranslated;
		protected Vector center = new Vector(0,0);
		
		public BoundingPolygon() 
		{
			vertices = new List<Vector>();
			verticesTranslated = new List<Vector>();
			edgeNormals = new List<Vector>();
		}
		
		/// <summary>
		/// Construct a bounding polygon from a list of vertices
		/// </summary>
		public BoundingPolygon(List<Vector> verts) : this()
		{
			AddVertices(verts);
		}
		
		/// <summary>
		/// Build the edge normals, and updates the boundaries (Left,Right,...).
		/// Assumes that all vertices are in the vertices list, and that some arbitrary items has been added to the edgeNormals list.
		/// </summary>
		protected void BuildNormals()
		{
			edgeNormals.Clear();
			MoveTo(center.X, center.Y);
			edgeNormals = new List<Vector>(vertices.Count - (vertices.Count <= 2 ? 1 : 0));
			left = 0; bottom = 0;
			right = 0; top = 0;
			
			for (int i = 0; i < edgeNormals.Capacity; i++)
			{
				Vector edge, normal;
				
				edge = vertices[i == vertices.Count - 1 ? 0 : i+1] - vertices[i];
				normal = new Vector(-edge.Y, edge.X);
				normal.Normalize();
				
				edgeNormals.Add(normal);
				
				//Update outer boundaries
				if (vertices[i].X < Left) left = i;
				if (vertices[i].X > Right) right = i;
				if (vertices[i].Y < Bottom) bottom = i;
				if (vertices[i].Y > Top) top = i;
			}
			for (int i = edgeNormals.Count; i < vertices.Count; i++)
			{
				//Update outer boundaries
				if (vertices[i].X < Left) left = i;
				if (vertices[i].X > Right) right = i;
				if (vertices[i].Y < Bottom) bottom = i;
				if (vertices[i].Y > Top) top = i;
			}
		}
		
		/// <summary>
		/// Add a new vertex, rebuilding edge normals as neccesary.
		/// </summary>
		public void AddVertex(Vector vertex)
		{
			vertices.Add((Vector)vertex.Clone());
			verticesTranslated.Add(new Vector());
			
			//Recalculate the normals
			BuildNormals();
			MoveTo(center.X, center.Y);
		}
		
		/// <summary>
		/// Add a collection of vertices, rebuilding edge normals as neccesary.
		/// </summary>
		public void AddVertices(List<Vector> verts)
		{
			foreach (var v in verts)
			{
				vertices.Add((Vector)v.Clone());
				verticesTranslated.Add(new Vector());
			}
			
			//Build the normals
			BuildNormals();
			MoveTo(center.X, center.Y);
		}
		
		/// <summary>
		/// Edge normals.
		/// </summary>
		public List<Vector> EdgeNormals
		{
			get { return edgeNormals; }
		}
		
		protected List<Vector> edgeNormals;

		/// <summary>
		/// Vertices of polygon. 
		/// Edges are implicit between consecutive vertices in a clockwise cyclic manner, 
		/// so that there is an edge between the last and first vertex, and edge normals point out to the left.
		/// </summary>
		public List<Vector> Vertices
		{
			get { return verticesTranslated; }
		}
		
		
		/// <summary>
		/// Translate the whole polygon by a translation vector (x,y).
		/// </summary>
		public BoundingPolygon Translate(double x, double y)
		{
			foreach (var vert in verticesTranslated)
			{
				vert.X += x;
				vert.Y += y;
			}
			center.X += x;
			center.Y += y;
			return this;
		}
		
		/// <summary>
		/// Center the polygon around a point (x,y). Same as translating from origin.
		/// </summary>
		public BoundingPolygon MoveTo(double x, double y)
		{
			for (int i = 0; i  < vertices.Count; i++)
			{
				verticesTranslated[i].X = vertices[i].X + x;
				verticesTranslated[i].Y = vertices[i].Y + y;
			}
			
			center.X = x;
			center.Y = y;
			return this;
		}
		
		/// <summary>
		/// Scale the whole polygon by a scalar.
		/// </summary>
		public BoundingPolygon Scale(double s)
		{
			Scale(s, s);
			
			return this;
		}
		
		/// <summary>
		/// Scale the whole polygon by a scalar.
		/// </summary>
		public BoundingPolygon Scale(double sX, double sY)
		{
			foreach (var vert in verticesTranslated)
			{
				vert.X *= sX;
				vert.Y *= sY;
			}
			
			return this;
		}
		
		
		/// <summary>
		/// X-position of leftmost vertex
		/// </summary>
		public double Left
		{
			get { return verticesTranslated[left].X; }
		}
		int left;
		
		/// <summary>
		/// Y-position of topmost vertex
		/// </summary>
		public double Top
		{
			get { return verticesTranslated[top].Y; }
		}
		int top;
		
		/// <summary>
		/// X-position of rightmost vertex
		/// </summary>
		public double Right
		{
			get { return verticesTranslated[right].X; }
		}
		int right;
		
		/// <summary>
		/// Y-position of bottommost vertex
		/// </summary>
		public double Bottom
		{
			get { return verticesTranslated[bottom].Y; }
		}
		int bottom;
		
		/// <summary>
		/// From ICloneable
		/// </summary>
		public object Clone()
		{
			BoundingPolygon p = new BoundingPolygon(vertices);
			p.MoveTo(center.X, center.Y);
			return new BoundingPolygon(vertices);
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
		public BoundingBox() : base()
		{
			List<Vector> verts = new List<Vector>();
			verts.Add(new Vector());
			verts.Add(new Vector());
			verts.Add(new Vector());
			verts.Add(new Vector());
			AddVertices(verts);
		}
		public BoundingBox(double left, double top, double right, double bottom) : base()
		{
			List<Vector> verts = new List<Vector>();
			verts.Add(new Vector(left, top));
			verts.Add(new Vector(right, top));
			verts.Add(new Vector(right, bottom));
			verts.Add(new Vector(left, bottom));
			AddVertices(verts);
		}
		
		public double Width
		{
			get { return Right - Left; }
		}
		
		public double Height
		{
			get { return Top - Bottom; }
		}
		
		public void Set(double left, double top, double right, double bottom)
		{
			verticesTranslated[0].X = left; verticesTranslated[0].Y = top; 
			verticesTranslated[1].X = right; verticesTranslated[1].Y = top; 
			verticesTranslated[2].X = right; verticesTranslated[2].Y = bottom; 
			verticesTranslated[3].X = left; verticesTranslated[3].Y = bottom;
			
		}
		
	}
	/// <summary>
	/// Interface for all objects that may collide.
	/// </summary>
	public interface ICollidable
	{
		//Collision handler for collision vs. a bounding polygon not tied to an object, e.g. a tile.
		void Collide(BoundingPolygon polygon, Vector collisionNormal, CollisionResult collisionResult);
		
		//Collision handler for collision vs another collidable object
		//edgeNormal is the normal vector for the edge of o that this object collided with.
		void Collide(ICollidable o, Vector edgeNormal, CollisionResult collisionResult);
		
		// Bounding box for the collidable object before updating
		BoundingPolygon BoundingBox
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
