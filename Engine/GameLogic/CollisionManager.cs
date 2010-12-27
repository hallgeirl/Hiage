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
		public bool WillIntersect = false;
		public double CollisionTime;
		
		// Intersecting right now
		public bool IsIntersecting = false;
		// Pushback vector 
		public Vector MinimumTranslationVector;
		// The frame duration used when testing
		public double FrameTime;
		
		public Vector HitNormal;

		//Used internally to determine MTD
		internal double distance;
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
						//if (t < remainingFrameFraction && axis.DotProduct(relativeVelocity) * (axisOwner == 0 ? -1 : 1) > 0)
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
			if ((!result.IsIntersecting && !result.WillIntersect) || //If the result intersection flags are both false, this is the first test.
			    (result.IsIntersecting && (willIntersect || (isIntersecting && result.distance < distance))) || //Previous result was an overlapping one, while the latest result indicate o1 and o2 will collide in the future instead,
                (result.WillIntersect && willIntersect && t > result.CollisionTime)) //Previous result was that o1 and o2 collides in the future, but this result indicates that they collide later.
			{
				result.IsIntersecting = isIntersecting;
				result.WillIntersect = willIntersect;
				result.CollisionTime = t;
				result.distance = distance;
				result.HitNormal = axis;
				
				#if DEBUG_COLLISION_OBJECT_POLYGON || DEBUG_COLLISION_OBJECT_OBJECT
				Log.Write("\tNew best axis", Log.DEBUG);
				#endif
			}
			//No intersection now or in the future.
			else if (!isIntersecting && !willIntersect)
			{
				result.WillIntersect = false; 
				result.IsIntersecting = false;
			}
		}
		
		/// <summary>
		/// Test collision against a series of polygons.
		/// </summary>
		public static void TestCollision(ICollidable o, List<BoundingPolygon> polygons, double frameTime)
		{
			if (polygons.Count == 0) return;
			//Applying Separating Axis theorem
			//First find all the axis. They are the union of the object's edge normals, and the polygon's edge normals.
			//The polygon's edge normals will be retrieved for each polygon that is checked.
			List<Vector>[] edges = {o.BoundingBox.EdgeNormals, null};
			
			double remainingFrameTime = 1;
			int loopCount = 0;

			#if DEBUG_COLLISION_OBJECT_POLYGON
			int collCount = 0;
			Log.Write("Testing collision object vs polygon, polygon count: " + polygons.Count, Log.DEBUG);
			#endif
			
			//As long as we may get another collision in this frame
			while (o.Velocity.DotProduct(o.Velocity) > 0 && remainingFrameTime > 0 && loopCount < 4)
			{
				loopCount++;
				
				//This is the reference to the first edge we're colliding with. If null at the end, we didn't collide.
				BoundingPolygon firstCollisionPolygon = null;
				//The current minimum time until collision
				double minimumCollisionTime = double.PositiveInfinity;
				//Final collision results
				CollisionResult finalResult = new CollisionResult();

				//How far do we actually move this frame?
				Vector velocity = o.Velocity * frameTime;
				
				//Set the minimum translation vector to the longest vector possible during a frame
				finalResult.MinimumTranslationVector = o.Velocity * frameTime;
				finalResult.FrameTime = frameTime;
				int finalNormalOwner = -1;
				
				//Check each edge
				foreach (BoundingPolygon p in polygons)
				{
					if (p.Vertices.Count == 2 && p.EdgeNormals[0].DotProduct(o.Velocity) > 0)
					{
						#if DEBUG_COLLISION_OBJECT_POLYGON
						Log.Write("Polygon has only one edge, which faces the same way as the movement direction. Ignoring.", Log.DEBUG);
						#endif
						continue;
					}
					
					#if DEBUG_COLLISION_OBJECT_POLYGON
					Log.Write("Object bounding polygon: " + o.BoundingBox, Log.DEBUG);
					Log.Write("Testing polygon " + p, Log.DEBUG);
					#endif
					
					//Collision results for the current polygon
					CollisionResult result = new CollisionResult();
					edges[1] = p.EdgeNormals;

					//TODO: Ignore edges facing the same way as we move?
					bool separating = false;
					int normalOwner = -1;
					
					//foreach (List<Vector> poly in edges)
					for (int i = 0; i < edges.Length; i++)
					{
						var poly = edges[i];
						foreach (Vector axis in poly)
						{
							// Do the collision test on the polygons
							testAxis(ProjectPolygon(o.BoundingBox, axis), ProjectPolygon(p, axis), velocity, axis, result, remainingFrameTime, i);
							if (object.ReferenceEquals(axis, result.HitNormal))
								normalOwner = i;
							
							if (!result.WillIntersect && !result.IsIntersecting) 
							{
								separating =  true;
								break;
							}
							if (result.IsIntersecting && double.IsNegativeInfinity(result.distance)) result.IsIntersecting = false;
						}
						if (separating) break;
					}
					
					//Already intersecting
					if (result.IsIntersecting)
					{
						finalResult = result;
						finalNormalOwner = normalOwner;
						minimumCollisionTime = 0;
						firstCollisionPolygon = p;
					}
					//Will intersect with p in the future. 
					//If we're not already overlapping with another polygon, go ahead and update the current minimum collision time.
					else if (result.WillIntersect && !finalResult.IsIntersecting)
					{
						//If the collision time is the smallest so far, 
						if (result.CollisionTime < minimumCollisionTime)
						{
							minimumCollisionTime = result.CollisionTime;
							finalResult = result;
							finalNormalOwner = normalOwner;
							firstCollisionPolygon = p;
						}
					} 
				}
				
				//If we have a first collision, call the collision handler
				if (firstCollisionPolygon != null)
				{
					if (finalResult.IsIntersecting)
						finalResult.MinimumTranslationVector = finalResult.distance * finalResult.HitNormal; //o.Velocity * finalResult.distance * frameTime;

					remainingFrameTime -= minimumCollisionTime;
					//Subtract a small amount to behave correctly when we have small rounding errors.
					finalResult.CollisionTime = minimumCollisionTime - 1e-6;//Constants.MinDouble;

					o.Collide(firstCollisionPolygon, finalNormalOwner == 1 ? finalResult.HitNormal : -finalResult.HitNormal, finalResult);
					#if DEBUG_COLLISION_OBJECT_POLYGON
					Log.Write("COLLISION." + " Time: " + finalResult.CollisionTime + " Normal: " + finalResult.HitNormal + " Remaining: " + remainingFrameTime + " Collision polygon: " + firstCollisionPolygon + " Velocity: " + o.Velocity + " Translation vector: " + finalResult.MinimumTranslationVector, Log.DEBUG);
					collCount++;					
					#endif
				}
				else 
				{
					remainingFrameTime = 0;
					#if DEBUG_COLLISION_OBJECT_POLYGON
					Log.Write("NO COLLISION.", Log.DEBUG);
					#endif
				}
			}
			
			#if DEBUG_COLLISION_OBJECT_POLYGON
			Log.Write("Collision count: " + collCount + "\n", Log.DEBUG);
			#endif
		}
		
		/// <summary>
		/// Test collision between two objects. 
		/// </summary>
		public static void TestCollision(ICollidable o1, ICollidable o2, double frameTime)
		{
			#if DEBUG_COLLISION_OBJECT_OBJECT
			Log.Write("Begin collision test object vs object", Log.DEBUG);
			Log.Write("Bounding box 1: " + o1.BoundingBox.ToString(), Log.DEBUG);
			Log.Write("Bounding box 2: " + o2.BoundingBox.ToString(), Log.DEBUG);
			#endif
			
			//Calculate relative velocity between o1 and o2, as seen from o1
			Vector relativeVelocity  = (o1.Velocity - o2.Velocity)*frameTime;
			#if DEBUG_COLLISION_OBJECT_OBJECT
			Log.Write("Velocity 1 " + o1.Velocity + " Velocity 2 " + o2.Velocity, Log.DEBUG);
			#endif
			CollisionResult result = new CollisionResult();
			result.FrameTime = frameTime;
			bool separating = false;
			int normalOwner = -1;
			
			List<Vector>[] polygons = {o1.BoundingBox.EdgeNormals, o2.BoundingBox.EdgeNormals};
			
			// Find each edge normal in the bounding polygons, which is used as axes.
			//foreach (var poly in polygons)
			for (int i = 0; i < polygons.Length; i++)
			{
				var poly = polygons[i];
				
				// If the result is ever null, we have a separating axis, and we can cancel the search.
				foreach (var axis in poly)
				{
					//Test for collision on one axis
					testAxis(ProjectPolygon(o1.BoundingBox, axis), ProjectPolygon(o2.BoundingBox, axis), relativeVelocity, axis, result, 1, i);
					
					if (object.ReferenceEquals(axis, result.HitNormal))
						normalOwner = i;
					
					//No intersection (now or in the future)
					if (!result.IsIntersecting && !result.WillIntersect)
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
				Log.Write("COLLISION. Normal: " + result.HitNormal + " Time: " + result.CollisionTime, Log.DEBUG);
				#endif
				if (result.IsIntersecting)
					result.MinimumTranslationVector = result.HitNormal * result.distance * frameTime;

				result.FrameTime = frameTime;
				collisionEvents.Add(new CollisionEvent(o1, o2, normalOwner == 1 ? result.HitNormal : -result.HitNormal, result));
				collisionEvents.Add(new CollisionEvent(o2, o1, normalOwner == 0 ? result.HitNormal : -result.HitNormal, result));
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
