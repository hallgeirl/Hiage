
using System;

namespace Engine
{
	/// <summary>
	/// Bounding box class
	/// </summary>
	public class BoundingBox : ICloneable
	{
		public BoundingBox(double left, double top, double right, double bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}
		
		public BoundingBox Translate(Vector v)
		{
			Left += v.X;
			Right += v.X;
			Top += v.Y;
			Bottom += v.Y;
			
			return this;
		}
		
		public object Clone()
		{
			return new BoundingBox(Left, Top, Right, Bottom);
		}
		
		public double Left 
		{ 
			get; 
			set; 
		}
		
		public double Right
		{ 
			get; 
			set; 
		}
		
		public double Top
		{ 
			get; 
			set; 
		}
		
		public double Bottom
		{ 
			get; 
			set; 
		}
		
		public double Width
		{
			get { return Right - Left; }
		}
		
		public double Height
		{
			get { return Top - Bottom; }
		}
		
		public override string ToString()
		{
			return "BoundingBox(left=" + Left + ",top=" + Top + ",right=" + Right + ",bottom=" + Bottom + ")";
		}
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
