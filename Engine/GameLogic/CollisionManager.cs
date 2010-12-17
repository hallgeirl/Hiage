//#define DEBUG_COLLISION

using System;
using System.Collections.Generic;


namespace Engine
{
	//Represents the results from a collision test.
	public class CollisionResult
	{
		public bool WillIntersect = false;
		public bool IsIntersecting = false;
		public double CollisionTime;
		public Vector MinimumTranslationVector;
		public double FrameTime;
		
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
				OBJECT_EDGE = 1,  //Collision between an object and an edge
				OBJECT_OBJECT = 2 //And object vs object
			}
			
			CollisionEvent.Type collisionType;
			
			ICollidable object1, object2;
			Edge edge;
			Vector collisionNormal;
			CollisionResult collisionResult;
			
			public CollisionEvent(ICollidable obj, Edge e, CollisionResult collisionResult)
			{
				collisionType = Type.OBJECT_EDGE;
				object1 = obj;
				edge = e;
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
				case Type.OBJECT_EDGE:
					object1.Collide(edge, collisionResult);
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
			
			//Extend the projection by adding to one of the ends.
			public void Extend(Vector v, Vector axis)
			{
				double dot = axis.DotProduct(v);
				if (dot < 0) Min += dot;
				else Max += dot;
				
				/*if (a < 0) Min += a;
				else Max += a;
				Min -= Math.Abs(a);
				Max += Math.Abs(a);*/
			}
			
			/*public Vector Axis
			{
				get;
				private set;
			}*/
			
			public double Length
			{
				get { return Max - Min; }
			}
			
			public double GetDistance(Projection p)
			{

				if (Min < p.Min && Max < p.Max)
					return p.Min - Max;
				else if (p.Min < Min && p.Max < Max)
					return Min - p.Max;
				else if (Min < p.Min && Max > p.Max)
					return Math.Max(Min - p.Max, p.Min - Max);
				else if (p.Min < Min && p.Max > Max)
					return Math.Max(p.Min - Max, Min - p.Max);
				
				return 0;
			}
						
			public bool IsOverlapping(Projection p)
			{
				return ((Min >= p.Min && Min <= p.Max) || (Max >= p.Min && Max <= p.Max) || (p.Min >= Min && p.Min <= Max) || (p.Max >= Min && p.Max <= Max));
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
		
		/*private static Projection ProjectPolygon(BoundingBox b, Vector axis)
		{
			double min = double.PositiveInfinity, max = double.NegativeInfinity;
			
			double dot1 = (new Vector(b.Left, b.Bottom).DotProduct(axis)),
			dot2 = (new Vector(b.Right, b.Bottom).DotProduct(axis)),
			dot3 = (new Vector(b.Left, b.Top).DotProduct(axis)),
			dot4 = (new Vector(b.Right, b.Top).DotProduct(axis));

			
			min = dot1;
			max = dot1;
			
			if (dot2 < min) min = dot2;
			else if (dot2 > max) max = dot2;
			if (dot3 < min) min = dot3;
			else if (dot3 > max) max = dot3;
			if (dot4 < min) min = dot4;
			else if (dot4 > max) max = dot4;
			
			
			return new Projection(min, max, axis);
		}*/
		
		private static Projection ProjectLine(Vector p1, Vector p2, Vector axis)
		{
			return new Projection(Math.Min(axis.DotProduct(p1), axis.DotProduct(p2)), Math.Max(axis.DotProduct(p1), axis.DotProduct(p2)), axis);
		}
		
		/// <summary>
		/// Test collision against a series of edges
		/// </summary>
		public static void TestCollision(ICollidable o, List<Edge> edges, double frameTime)
		{
			bool outputDebug = false;
			
			//Applying Separating Axis theorem
			//First find all the axis
			List<Vector> axis = new List<Vector>();
			axis.Add(new Vector(1,0)); // Bounding box axis 1
			axis.Add(new Vector(0,1)); // Bounding box axis 2
			axis.Add(new Vector());    //Line segment axis 1
			axis.Add(new Vector());	   //Line segment axis 2
			
			double remainingFrameTime = 1;
			int collCount = 0;			
			int loopCount = 0;
			
			//As long as we may get another collision in this frame
			while (o.Velocity.Length > 0 && remainingFrameTime > 0 && loopCount < 10)
			{
				if (loopCount > 10)
					outputDebug = true;
				if (loopCount > 10)
					Log.Write("Collision test looped " + loopCount + " times. Stuck?", Log.WARNING);
				loopCount++;
				//This is the reference to the first edge we're colliding with. If null at the end, we didn't collide.
				Edge firstCollisionEdge = null;
				//The current minimum time untill collision
				double minimumCollisionTime = double.PositiveInfinity;
				//Final collision results
				CollisionResult finalResult = new CollisionResult();

				//How far do we actually move this frame?
				Vector velocity = o.Velocity * frameTime;
				
				//Set the minimum translation vector to the longest vector possible during a frame
				finalResult.MinimumTranslationVector = o.Velocity * frameTime;
				finalResult.FrameTime = frameTime;
				
				if (outputDebug)
					Log.Write("Checking " + edges.Count + " edges");
				
				//Check each edge
				foreach (Edge e in edges)
				{
					//Collision results for the current edge
					CollisionResult result = new CollisionResult();
					//Assume we are colliding at first
					//result.IsIntersecting = false;
					result.WillIntersect = true;
					result.FrameTime = frameTime;
					//Time untill collision with this edge
					double collisionTime = double.NegativeInfinity;
										

					if (outputDebug)
						Log.Write("Dot product: " + e.Normal.DotProduct(velocity) + " Velocity: " + velocity + " Normal: " + e.Normal);

					//Ignore edges facing the same way as we move					
					if (e.Normal.DotProduct(o.Velocity) >= -1e-13) continue;
					
					
					//Calculate the two axis of the edge
					Vector v = (e.P2-e.P1).Normalize();
					axis[2] = v;
					axis[3] = new Vector(-v.Y, v.X);

					foreach (Vector a in axis)
					{
						//Project line and box
						Projection prj1 = ProjectPolygon(o.BoundingBox, a);
						Projection prj2 = ProjectLine(e.P1, e.P2, a);
	
						if (outputDebug)
							Log.Write("prj1:" + prj1 + " prj2:" + prj2);
						
						//Check if projections overlap in the first place. Positive distance means no, otherwise it's yes.
						double distance = prj1.GetDistance(prj2);
						
						if (outputDebug)
							Log.Write("Distance (before correction): " + distance);
						
						if (Math.Abs(distance) <= 1e-12)
							distance = 0;

						//If the distance is negative, we have an overlap.
						if (distance < 0)
						{
							//If the projection of the line has zero length, we know that if we intersect in the future, it will be perpendicular to the edge.
							//We don't want that. Else, continue.
							if (prj2.Min == prj2.Max) result.WillIntersect = false;
							else continue;
						}
						
						if (distance >= 0)
						{
							//Find the velocity along the axis and see if we'll ever intersect
							double velAxis = a.DotProduct(velocity);
							
							if (Math.Abs(velAxis) < 1e-12)
								velAxis = 0;
							
							if (outputDebug)
								Log.Write("Velocity along axis: " + velAxis);
	
							//Calculate the time it takes to travel the distance
							if (result.WillIntersect)
							{
								if (prj1.Max > prj2.Min)
									distance *= -1;
								
								double t = (Math.Abs(distance) < 1e-12 ? 0 : distance / velAxis);
								
								if (t > collisionTime) collisionTime = t;
								
								if (outputDebug)
									Log.Write("t: " + t + " collisionTime: " + collisionTime + " Remaining frame time: " + remainingFrameTime);
								
								//If it takes more than the remaining frame time (or less than 0) to collide, it won't happen
								if ((t > remainingFrameTime || t < 0) && distance > 0)
								{
									result.WillIntersect = false;
								}
							}
						}

						//if (!result.IsIntersecting && !result.WillIntersect) break;
						if (!result.WillIntersect) break;
					}
					
					if (collisionTime > remainingFrameTime || collisionTime < 0)
						result.WillIntersect = false;
					
					if (result.WillIntersect)
					{
						result.MinimumTranslationVector = velocity * collisionTime;

						if (Math.Abs(result.MinimumTranslationVector.X) < 1e-8)
							result.MinimumTranslationVector.X = 0;
						if (Math.Abs(result.MinimumTranslationVector.Y) < 1e-8)
							result.MinimumTranslationVector.Y = 0;
						
						//if (result.MinimumTranslationVector.Length < finalResult.MinimumTranslationVector.Length)
						if (collisionTime < minimumCollisionTime)
						{
							minimumCollisionTime = collisionTime;
							finalResult = result;
							firstCollisionEdge = e;
						}
					} 
				}
				
				
				//If we have a first collision, call the collision handler
				if (firstCollisionEdge != null)
				{
					remainingFrameTime -= minimumCollisionTime;
					finalResult.CollisionTime = minimumCollisionTime;

					o.Collide(firstCollisionEdge, finalResult);
					if (outputDebug)
						Log.Write("Collision. Remaining: " + remainingFrameTime + " Collision edge: " + firstCollisionEdge + "V: " + o.Velocity + " Translation vector: " + finalResult.MinimumTranslationVector);
					collCount++;					
				}
				else remainingFrameTime = 0;
			}
			
			if (outputDebug)
				Log.Write("Collision count: " + collCount + "\n");
		}
		
		/// <summary>
		/// Test one axis with SAT.
		/// Returns null if no overlap was detected (and no overlap will occur within one frame).
		/// </summary>
		private static CollisionResult testAxis(BoundingPolygon p1, BoundingPolygon p2, Vector relativeVelocity, double frameTime, Vector axis)
		{
			CollisionResult r = new CollisionResult();
			
			Projection prj1 = ProjectPolygon(p1, axis);
			Projection prj2 = ProjectPolygon(p2, axis);
			//Calculate velocity component in direction of axis
			double velAxis = axis.DotProduct(relativeVelocity), distance = 0;
			
			//Positive distance means we don't have an overlap. Negative means we have an overlap.
			distance = prj1.GetDistance(prj2);
			
			#if (DEBUG_COLLISION)
			Log.Write("Testing axis " + axis + ", relative vel: " + relativeVelocity);
			Log.Write("Projection 1: " + prj1 + " Projection 2: " + prj2);
			Log.Write("Distance " + distance);
			#endif
			
			//Round off distance if it's too small
			if (Math.Abs(distance) < Constants.MinDouble) distance = 0; 
			if (distance <= 0)
			{
				r.IsIntersecting = true;
				r.distance = -distance;
				return r;
			}
			
			#if (DEBUG_COLLISION)
			Log.Write("VelAxis: " + velAxis);
			#endif
			
			//If projection of polygon 2 is to the right of polygon 1, AND we have a positive velocity along the axis
			//OR projection of polygon 1 is to the left of polygon 2 AND we have a negative velocity along axis
			//then we might have a collision in the future. If not, the objects are either moving in separate directions
			//or they are staying still.
			if ((velAxis > 0 && prj2.Min > prj1.Max) || (velAxis < 0 && prj1.Min > prj2.Max))
			{
				r.CollisionTime = distance / Math.Abs(velAxis);
				if (r.CollisionTime < 1)
					r.WillIntersect = true;
				#if (DEBUG_COLLISION)
				Log.Write("Coll. time: " + r.CollisionTime);
				#endif
				
				return r;
			}
			
			//Won't intersect. Return with WillIntersect=false, IsIntersecting=false.
			return r;
		}
		
		/// <summary>
		/// Test collision between two objects. 
		/// </summary>
		public static void TestCollision(ICollidable o1, ICollidable o2, double frameTime)
		{
			#if (DEBUG_COLLISION)
			Log.Write("Begin collision test");
			#endif
			//Log.Write(o1.BoundingBox.ToString());
			//Log.Write(o2.BoundingBox.ToString());
			
			//Calculate relative velocity between o1 and o2, as seen from o1
			Vector relativeVelocity  = (o1.Velocity - o2.Velocity)*frameTime;
			#if (DEBUG_COLLISION)
			Log.Write("Velocity 1 " + o1.Velocity + " Velocity 2 " + o2.Velocity);
			#endif
			CollisionResult bestResult = null;
			Vector bestNormal = null;
			bool separating = false;
			
			List<Vector>[] polygons = {o1.BoundingBox.EdgeNormals, o2.BoundingBox.EdgeNormals};
			
			// Find each edge normal in the bounding polygons, which is used as axes.
			foreach (var poly in polygons)
			{
				// If the result is ever null, we have a separating axis, and we can cancel the search.
				foreach (var axis in poly)
				{
					//Vector edge = verts[i == verts.Count - 1 ? 0 : i+1] - verts[i];
					//Vector axis = new Vector(-edge.Y, edge.X);
					
					//Test for collision on one axis
					CollisionResult r = testAxis(o1.BoundingBox, o2.BoundingBox, relativeVelocity, frameTime, axis);
					
					//No separation.
					if (!r.IsIntersecting && !r.WillIntersect)
					{
						separating = true;
						break;
					}
					//Find the "best" determination of HOW the objects collides.
					//That is, what direction, and what normal was intersected first.
					else if (bestResult == null || //No previously stored result
					        //Previous result was an overlapping one, while the latest result indicate o1 and o2 will collide in the future instead, OR there is a smaller minimum translation vector.
					        (bestResult.IsIntersecting && (r.WillIntersect || bestResult.distance < r.distance)) || 
					        //Previous result was that o1 and o2 collides in the future, but this result indicates that they collide later.
					        (bestResult.WillIntersect && (r.WillIntersect && r.CollisionTime > bestResult.CollisionTime)))
					{
						bestResult = r;
						bestNormal = axis;
						
						#if (DEBUG_COLLISION)
						Log.Write("New best axis");
						#endif
					}
				}
				if (separating) break;
			}			

			if (!separating)
			{
				#if (DEBUG_COLLISION)
				Log.Write("COLLISION. Normal: " + bestNormal + " Time: " + bestResult.CollisionTime);
				#endif
				if (bestResult.IsIntersecting)
					bestResult.MinimumTranslationVector = bestNormal.Normalize() * bestResult.distance;
				else
					bestNormal.Normalize();

				bestResult.FrameTime = frameTime;
									
				collisionEvents.Add(new CollisionEvent(o1, o2, bestNormal, bestResult));
				collisionEvents.Add(new CollisionEvent(o2, o1, bestNormal, bestResult));
			}
			else
			{
				#if (DEBUG_COLLISION)
				Log.Write("NO COLLISION");
				#endif
			}
			#if (DEBUG_COLLISION)
			Log.Write("");
			#endif
		}
	}
}
