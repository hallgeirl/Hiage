//#define DEBUG_COLLISION_OBJECT_POLYGON
//#define DEBUG_COLLISION_OBJECT_OBJECT

using System;
using System.Collections.Generic;

namespace Engine
{
	//Represents the results from a collision test.
	public class CollisionResult
	{
		// Bounding polygons will intersect at t=(0 <= CollisionTime < 1) into the future
		public bool hasIntersected = false;
		public double collisionTime;
		
		// Intersecting right now
		public bool isIntersecting = false;
		// Pushback vector 
		public Vector minimumTranslationVector;
		// The frame duration used when testing
		public double frameTime;

		//Normal for hit surface, and its owner
		public Vector hitNormal;
		public int normalOwner;
		
		//Object 
		//public CollidableComponent obj2;
		
		//Used internally to determine MTD
		internal double distance;
		
		public static CollisionResult NoCollision
		{
			get 
			{
				CollisionResult res = new CollisionResult();
				res.hasIntersected = false; 
				res.isIntersecting = false;
				return res;
			}
		}
	}
	
	/// <summary>
	/// Tests for collisions.
	/// Usage: Call CollisionManager.PerformCollisionEvents() when the collision events should be performed.
	/// </summary>
	public class CollisionManager
	{
		/// <summary>
		/// Class that describes the collision that occured (time, type of collision, what objects, etc.)
		/// </summary>
		private class CollisionEvent
		{
			enum Type
			{
				OBJECT_POLYGON = 1,  //Collision between an object and a polygon
				OBJECT_OBJECT = 2 //And object vs object
			}
			
			CollisionEvent.Type collisionType;
			
			ICollidable object1, object2;
			BoundingPolygon polygon;
			Vector collisionNormal;
			CollisionResult collisionResult;
			
			public CollisionEvent(ICollidable obj, BoundingPolygon p, Vector normal, CollisionResult collisionResult)
			{
				collisionType = Type.OBJECT_POLYGON;
				object1 = obj;
				polygon = p;
				collisionNormal = normal;
				this.collisionResult = collisionResult;
			}

			public CollisionEvent(ICollidable obj1, ICollidable obj2, Vector normal, CollisionResult collisionResult)
			{
				collisionType = Type.OBJECT_OBJECT;
				object1 = obj1;
				object2 = obj2;
				this.collisionNormal = normal;
				this.collisionResult = collisionResult;
			}
			
			public void Perform()
			{
				switch (collisionType)
				{
				case Type.OBJECT_POLYGON:
					object1.Collide(polygon, collisionNormal, collisionResult);
					break;
				case Type.OBJECT_OBJECT:
					object1.Collide(object2, collisionNormal, collisionResult);
					break;
				}
			}
		}
		
		/// <summary>
		/// Represents the accumulated result from multiple vector projections.
		/// </summary>
		private class Projection
		{
			public Projection(double min, double max, Vector axis)
			{
				Min = min;
				Max = max;
			}
			
			public double Min
			{
				get;
				private set;
			}
			
			public double Max
			{
				get;
				private set;
			}
			
			public double Length
			{
				get { return Max - Min; }
			}
			
			public double GetDistance(Projection p)
			{
				//Return the signed distance between the projections.
				//In case of no overlap: Only one of these are positive, so return that one.
				//In case of overlap: Both are negative, return the one with the smallest magnitude.
				return Math.Max(p.Min-Max, Min-p.Max);
			}
						
			public bool IsOverlapping(Projection p)
			{
				return GetDistance(p) < 0;//((Min >= p.Min && Min <= p.Max) || (Max >= p.Min && Max <= p.Max) || (p.Min >= Min && p.Min <= Max) || (p.Max >= Min && p.Max <= Max));
			}
			
			public override string ToString()
			{
				return "Projection(Min=" + Min + ",Max=" + Max + ",Axis=";// + Axis + ")";
			}
		}
		
		private static List<CollisionEvent> collisionEvents = new List<CollisionEvent>();
		
		/// <summary>
		/// Performs every item in the list of collision events. Should be called once each frame.
		/// </summary>
		public static void PerformCollisionEvents()
		{
			foreach (CollisionEvent e in collisionEvents)
				e.Perform();
			
			collisionEvents.Clear();
		}
		
		/// <summary>
		/// Project a convex polygon on a vector.
		/// </summary>
		private static Projection ProjectPolygon(BoundingPolygon p, Vector axis)
		{
			double min = double.PositiveInfinity, max = double.NegativeInfinity;
			
			foreach (Vector v in p.Vertices)
			{
				double dot = v.DotProduct(axis);
				if (dot < min) min = dot;
				if (dot > max) max = dot;
			}
			
			return new Projection(min, max, axis);
		}
		
		private static Projection ProjectLine(Vector p1, Vector p2, Vector axis)
		{
			return new Projection(Math.Min(axis.DotProduct(p1), axis.DotProduct(p2)), Math.Max(axis.DotProduct(p1), axis.DotProduct(p2)), axis);
		}
		
		/// <summary>
		/// Test one axis with SAT.
		/// Updates result if there is a "better" hit
		/// </summary>
		private static void testAxis(Projection prj1, Projection prj2, Vector relativeVelocity, Vector axis, CollisionResult result, double remainingFrameFraction, int axisOwner)
		{
			bool isIntersecting = false, willIntersect = false;
			double t = 0; //Collision time
		
			//Positive distance means we don't have an overlap. Negative means we have an overlap.
			double distance = prj1.GetDistance(prj2);
			
			#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
			Log.Write("\tTesting axis " + axis + ", relative vel: " + relativeVelocity, Log.DEBUG);
			Log.Write("\tProjection 1: " + prj1 + " Projection 2: " + prj2, Log.DEBUG);
			Log.Write("\tDistance " + distance, Log.DEBUG);
			#endif
			
			//Already intersecting?
			if (distance < 0)
			{
				isIntersecting = true;
				#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
				Log.Write("\tIntersecting.", Log.DEBUG);
				#endif
			}
			else
			{
				//Calculate velocity component in direction of axis
				double velAxis = axis.DotProduct(relativeVelocity);

				//if (velAxis < Constants.MinDouble) velAxis = 0;
				
				#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
				Log.Write("\tNot intersecting. Velocity along axis: " + velAxis, Log.DEBUG);
				#endif
				
				//If projection of polygon 2 is to the right of polygon 1, AND we have a positive velocity along the axis
				//OR projection of polygon 1 is to the left of polygon 2 AND we have a negative velocity along axis
				//then we might have a collision in the future. If not, the objects are either moving in separate directions
				//or they are staying still.
				if ((velAxis > 0 && prj2.Min >= prj1.Max) || (velAxis < 0 && prj1.Min >= prj2.Max))
				{
					//If the axis belongs to object 1, and it's facing the opposite direction of the velocity,
					//then ignore it because it can't collide. Also, if the axis belongs to object 2,
					//and the axis faces the same direction as the velocity, also ignore it.
					#if DEBUG_COLLISION_OBJECT_POLYGON
					Log.Write("\tAxis dot Velocity: " + axis.DotProduct(relativeVelocity) * (axisOwner == 0 ? -1 : 1) + " Axis: " + axis, Log.DEBUG);
					#endif

					//Ignore this test if the axis faces the wrong way
					if (axis.DotProduct(relativeVelocity) * (axisOwner == 0 ? -1 : 1) > Constants.MinDouble) 
					{
						#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
						Log.Write("\tIgnoring test because the edge faces the wrong way. Dot: " + axis.DotProduct(relativeVelocity) * (axisOwner == 0 ? -1 : 1) + "Owner: " + axisOwner);
						#endif
						distance = double.NegativeInfinity;
						isIntersecting = true;
					}
					else
					{
						t = distance / Math.Abs(velAxis);
						
						if (t < remainingFrameFraction)
							willIntersect = true;
						
						#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
						Log.Write("\tCollision time: " + t, Log.DEBUG);
						#endif
					}
				}
				#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
				else
				{
					Log.Write("\tMoving the wrong way. No collision.", Log.DEBUG);
				}
				#endif
			}
			
			//Find the "best" guess of HOW the objects collides.
			//That is, what direction, and what normal was intersected first.
			if ((!result.isIntersecting && !result.hasIntersected) || //If the result intersection flags are both false, this is the first test.
			    (result.isIntersecting && (willIntersect || (isIntersecting && result.distance < distance))) || //Previous result was an overlapping one, while the latest result indicate o1 and o2 will collide in the future instead,
                (result.hasIntersected && willIntersect && t > result.collisionTime)) //Previous result was that o1 and o2 collides in the future, but this result indicates that they collide later.
			{
				result.isIntersecting = isIntersecting;
				result.hasIntersected = willIntersect;
				result.collisionTime = t;
				result.distance = distance;
				result.hitNormal = axis;
				
				#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
				Log.Write("\tNew best axis", Log.DEBUG);
				#endif
			}
			//No intersection now or in the future.
			else if (!isIntersecting && !willIntersect)
			{
				result.hasIntersected = false; 
				result.isIntersecting = false;
			}
		}
		
		/// <summary>
		/// Test collision against a series of polygons.
		/// </summary>
		public static CollisionResult TestCollision(CollidableComponent o, List<BoundingPolygon> polygons, double frameTime, int axis)
		{
			CollisionResult finalResult = CollisionResult.NoCollision;
			
			if (polygons.Count == 0) 
			{
				return finalResult;
			}
			
			Vector velocity;
			if (axis < 0)
				velocity = o.Velocity * frameTime;
			else 
			{
				velocity = new Vector(0,0);
				velocity[axis] = o.Velocity[axis]*frameTime;
			}
			
			
			//Applying Separating Axis theorem
			//First find all the axis. They are the union of the object's edge normals, and the polygon's edge normals.
			//The polygon's edge normals will be retrieved for each polygon that is checked.

			
			List<Vector>[] edges = {o.BoundingPolygon.EdgeNormals, null};
			
			int loopCount = 0;

			#if DEBUG_COLLISION_OBJECT_POLYGON
			int collCount = 0;
			Log.Write("Testing collision object vs polygon, polygon count: " + polygons.Count, Log.DEBUG);
			#endif
			
				
			//As long as we may get another collision in this frame
			BoundingPolygon oldBoundingBox = (BoundingPolygon)o.BoundingPolygon.Clone();
			oldBoundingBox.Translate(-velocity.X, -velocity.Y);
			loopCount++;
			
			//This is the reference to the first edge we're colliding with. If null at the end, we didn't collide.
			BoundingPolygon firstCollisionPolygon = null;
			//The current minimum time until collision
			double minimumCollisionTime = double.PositiveInfinity;

			//How far have we actually moved this frame?
			
			//Set the minimum translation vector to the longest vector possible during a frame
			//finalResult.minimumTranslationVector = o.Velocity * frameTime;
			//finalResult.frameTime = frameTime;
			int finalNormalOwner = -1;
			
			//Check each edge
			foreach (BoundingPolygon p in polygons)
			{
				/*if (p.Vertices.Count == 2 && p.EdgeNormals[0].DotProduct(o.Velocity) > 0)
				{
					#if DEBUG_COLLISION_OBJECT_POLYGON
					Log.Write("Polygon has only one edge, which faces the same way as the movement direction. Ignoring.", Log.DEBUG);
					#endif
					continue;
				}*/
				
				#if DEBUG_COLLISION_OBJECT_POLYGON
				Log.Write("Object bounding polygon: " + o.BoundingPolygon, Log.DEBUG);
				Log.Write("Testing polygon " + p, Log.DEBUG);
				#endif
				
				//Collision results for the current polygon
				CollisionResult result = new CollisionResult();
				result.minimumTranslationVector = velocity;
				result.frameTime = frameTime;
				edges[1] = p.EdgeNormals;

				bool separating = false;
				int normalOwner = -1;
				
				for (int i = 0; i < edges.Length; i++)
				{
					var poly = edges[i];
					foreach (Vector _axis in poly)
					{
						// Do the collision test on the polygons
						testAxis(ProjectPolygon(oldBoundingBox, _axis), ProjectPolygon(p, _axis), velocity, _axis, result, 1, i);
						if (object.ReferenceEquals(_axis, result.hitNormal))
							normalOwner = i;
						
						if (!result.hasIntersected && !result.isIntersecting) 
						{
							separating =  true;
							break;
						}
						if (result.isIntersecting && double.IsNegativeInfinity(result.distance)) result.isIntersecting = false;
					}
					if (separating) break;
				}
				
				//Already intersecting
				if (result.isIntersecting)
				{
					finalResult = result;
					finalNormalOwner = normalOwner;
					minimumCollisionTime = 0;
					firstCollisionPolygon = p;
				}
				//Will intersect with p in the future. 
				//If we're not already overlapping with another polygon, go ahead and update the current minimum collision time.
				else if (result.hasIntersected)
				{
					//If the collision time is the smallest so far, 
					if (result.collisionTime < minimumCollisionTime)
					{
						minimumCollisionTime = result.collisionTime;
						finalResult = result;
						finalNormalOwner = normalOwner;
						firstCollisionPolygon = p;
					}
				} 
			}
			
			//If we have a first collision, call the collision handler
			if (firstCollisionPolygon != null)
			{
				if (finalResult.isIntersecting)
					finalResult.minimumTranslationVector = (finalNormalOwner == 0 ? 1 : -1) * Math.Abs(finalResult.distance) * finalResult.hitNormal; //o.Velocity * finalResult.distance * frameTime;

				//Subtract a small amount to behave correctly when we have small rounding errors.
				finalResult.collisionTime = minimumCollisionTime; // - 1e-6;//Constants.MinDouble;
				if (finalNormalOwner != 1)
					finalResult.hitNormal = -finalResult.hitNormal;
					
				#if DEBUG_COLLISION_OBJECT_POLYGON
				Log.Write("COLLISION." + " Time: " + finalResult.collisionTime + " Normal: " + finalResult.hitNormal + " Remaining: " + remainingFrameTime + " Collision polygon: " + firstCollisionPolygon + " Velocity: " + o.Velocity + " Translation vector: " + finalResult.minimumTranslationVector, Log.DEBUG);
				collCount++;					
				#endif
			}
			else 
			{
				#if DEBUG_COLLISION_OBJECT_POLYGON
				Log.Write("NO COLLISION.", Log.DEBUG);
				#endif
			}
			
			#if DEBUG_COLLISION_OBJECT_POLYGON
			Log.Write("Collision count: " + collCount + "\n", Log.DEBUG);
			#endif
			
			return finalResult;
		}
		
		/// <summary>
		/// Test collision between two objects. 
		/// </summary>
		public static void TestCollision(ICollidable o1, ICollidable o2, double frameTime)
		{
			#if DEBUG_COLLISION_OBJECT_OBJECT
			Log.Write("Begin collision test object vs object", Log.DEBUG);
			Log.Write("Bounding box 1: " + o1.BoundingPolygon.ToString(), Log.DEBUG);
			Log.Write("Bounding box 2: " + o2.BoundingPolygon.ToString(), Log.DEBUG);
			#endif
			
			//Calculate relative velocity between o1 and o2, as seen from o1
			Vector relativeVelocity  = (o1.Velocity - o2.Velocity)*frameTime;
			#if DEBUG_COLLISION_OBJECT_OBJECT
			Log.Write("Velocity 1 " + o1.Velocity + " Velocity 2 " + o2.Velocity, Log.DEBUG);
			#endif
			CollisionResult result = new CollisionResult();
			result.frameTime = frameTime;
			bool separating = false;
			int normalOwner = -1;
			
			List<Vector>[] polygons = {o1.BoundingPolygon.EdgeNormals, o2.BoundingPolygon.EdgeNormals};
			
			// Find each edge normal in the bounding polygons, which is used as axes.
			//foreach (var poly in polygons)
			for (int i = 0; i < polygons.Length; i++)
			{
				var poly = polygons[i];
				
				// If the result is ever null, we have a separating axis, and we can cancel the search.
				foreach (var axis in poly)
				{
					//Test for collision on one axis
					testAxis(ProjectPolygon(o1.BoundingPolygon, axis), ProjectPolygon(o2.BoundingPolygon, axis), relativeVelocity, axis, result, 1, i);
					
					if (object.ReferenceEquals(axis, result.hitNormal))
						normalOwner = i;
					
					//No intersection (now or in the future)
					if (!result.isIntersecting && !result.hasIntersected)
					{
						separating = true;
						break;
					}
				}
				if (separating) break;
			}			

			if (!separating)
			{
				#if DEBUG_COLLISION_OBJECT_OBJECT
				Log.Write("COLLISION. Normal: " + result.hitNormal + " Time: " + result.collisionTime, Log.DEBUG);
				#endif
				if (result.isIntersecting)
					result.minimumTranslationVector = result.hitNormal * result.distance * frameTime;

				result.frameTime = frameTime;
				collisionEvents.Add(new CollisionEvent(o1, o2, normalOwner == 1 ? result.hitNormal : -result.hitNormal, result));
				collisionEvents.Add(new CollisionEvent(o2, o1, normalOwner == 0 ? result.hitNormal : -result.hitNormal, result));
			}
			#if DEBUG_COLLISION_OBJECT_OBJECT
			else
			{
				Log.Write("NO COLLISION", Log.DEBUG);
			}
			Log.Write("");
			#endif
		}
	}
}
