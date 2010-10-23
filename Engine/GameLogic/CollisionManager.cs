
using System;
using System.Collections.Generic;

namespace Engine
{
	public struct CollisionResult
	{
		public bool WillIntersect;
		//public bool IsIntersecting;
		public double CollisionTime;
		public Vector MinimumTranslationVector;
		public double FrameTime;
		
	}
	
	/// <summary>
	/// Tests for collisions.
	/// Usage: Call CollisionManager.PerformCollisionEvents() when the collision events should be performed.
	/// </summary>
	public class CollisionManager
	{
		private class CollisionEvent
		{
			enum Type
			{
				OBJECT_EDGE = 1,
				OBJECT_OBJECT = 2
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
		
		private class Projection
		{
			public Projection(double min, double max, Vector axis)
			{
				Axis = (Vector)axis.Clone();
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
			
			public void Extend(double a)
			{
				if (a < 0) Min += a;
				else Max += a;
			}
			
			public Vector Axis
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

				if (Min < p.Min && Max < p.Max)
					return p.Min - Max;
				else if (p.Min < Min && p.Max < Max)
					return Min - p.Max;
				else if (Min < p.Min && Max > p.Max)
					return Math.Max(Min - p.Max, p.Min - Max);
				else if (p.Min < Min && p.Max > Max)
					return Math.Max(p.Min - Max, Min - p.Max);
				
				return 0;
				/*
				if (Min < p.Min)
					return p.Min - Max;
				else
					return Min - p.Max;*/
			}
			
			public bool IsOverlapping(Projection p)
			{
				return ((Min >= p.Min && Min <= p.Max) || (Max >= p.Min && Max <= p.Max) || (p.Min >= Min && p.Min <= Max) || (p.Max >= Min && p.Max <= Max));
			}
			
			public override string ToString()
			{
				return "Projection(Min=" + Min + ",Max=" + Max + ",Axis=" + Axis + ")";
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
		
		private static Projection ProjectBox(BoundingBox b, Vector axis)
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
		}
		
		private static Projection ProjectLine(Vector p1, Vector p2, Vector axis)
		{
			return new Projection(Math.Min(axis.DotProduct(p1), axis.DotProduct(p2)), Math.Max(axis.DotProduct(p1), axis.DotProduct(p2)), axis);
		}
		
		/// <summary>
		/// Test collision against a series of edges
		/// </summary>
		/// <param name="o1">
		/// A <see cref="ICollidable"/>
		/// </param>
		/// <param name="edges">
		/// A <see cref="List"/>
		/// </param>
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
						Projection prj1 = ProjectBox(o.BoundingBox, a);
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
		/// Test collision against a list of other collidable objects. 
		/// </summary>
		/// <param name="o1">
		/// A <see cref="ICollidable"/>
		/// </param>
		/// <param name="objects">
		/// A <see cref="List"/>
		/// </param>
		public static void TestCollision(ICollidable o1, ICollidable o2, double frameTime)
		{
			//Calculate relative speed between o1 and o2
			Vector relativeVelocity  = (o1.Velocity - o2.Velocity)*frameTime;
			
			//A point p in obj has the position p(t) = p(0) + v*t at any time, where v is the relative velocity
			//We want to check each axis for a collision, for all corner points.
			/*
			 * The math behind the idea:
			 * Given rectangles r1 and r2, and a relative velocity v with components x and y.
			 * r1.right(t) = r1.right(0)+v.x*t = r2.left => t = (r2.left - r1.right(0)) / v.x
			 * r1.left(t) = r1.left(0)+v.x*t = r2.right => t = (r2.right - r1.left(0)) / v.x
			 * And similar for left vs left and right vs right, and for the vertical axis
			 * If v.x == 0, then one of the object's vertical edges NEED to be within the bounds of the target rectangle to begin with. Same with v.y.
			 * */
			//double t_top_min = -1, t_bottom_min = -1, t_top_max = -1, t_bottom_max = -1;
			double timeX = -1, timeY = -1;
			Vector normal = new Vector(0,0);
		
			if (relativeVelocity.X != 0)
			{
				//Find the time of the intersection of the right edge of o1 vs the left edge of o2
				double t_right_min = (o2.BoundingBox.Left - o1.BoundingBox.Right) / relativeVelocity.X;
				//And left edge of o1 vs right edge of o2
				double t_left_min = (o2.BoundingBox.Right - o1.BoundingBox.Left) / relativeVelocity.X;
				//And same for right/left edge vs right/left edge (for when the collision is over)
				double t_right_max;// = (o2.BoundingBox.Right - o1.BoundingBox.Right) / relativeVelocity.X;
				double t_left_max;// = (o2.BoundingBox.Right - o1.BoundingBox.Right) / relativeVelocity.X;
				
				//We need to calculate the exit time differently, depending on what X-direction we're moving in
				if (relativeVelocity.X < 0)
				{
					t_left_max = (o2.BoundingBox.Left - o1.BoundingBox.Left) / relativeVelocity.X;
					t_right_max = (o2.BoundingBox.Left - o1.BoundingBox.Right) / relativeVelocity.X;
				}
				else
				{
					t_left_max = (o2.BoundingBox.Right - o1.BoundingBox.Left) / relativeVelocity.X;
					t_right_max = (o2.BoundingBox.Right - o1.BoundingBox.Right) / relativeVelocity.X;
				}

				//Check the case where they're actually inside the boundaries already
				if (o1.BoundingBox.Left < o2.BoundingBox.Right && o1.BoundingBox.Left > o2.BoundingBox.Left)
					t_left_min = 0;
				
				if (o1.BoundingBox.Right > o2.BoundingBox.Left && o1.BoundingBox.Right < o2.BoundingBox.Right)
					t_right_min = 0;
				
				//Now, check what time frames actually overlap (that is, check that t_min < t_max, and of course check if t_min > 0)
				if (t_right_min >= 0 && t_right_min < t_right_max)
				{
					timeX = t_right_min;
					normal.X = -1;
				}
				else if (t_left_min >= 0 && t_left_min < t_left_max)
				{
					timeX = t_left_min;
					normal.X = 1;
				}
			}
			else
			{
				//Check the case where they're actually inside the boundaries already
				if (o1.BoundingBox.Left <= o2.BoundingBox.Right && o1.BoundingBox.Left >= o2.BoundingBox.Left)
					timeX = 0;
				
				if (o1.BoundingBox.Right >= o2.BoundingBox.Left && o1.BoundingBox.Right <= o2.BoundingBox.Right)
					timeX = 0;
			}
			
			if (relativeVelocity.Y != 0)
			{
				//And finally the same for the y axis
				double t_top_min = (o2.BoundingBox.Bottom - o1.BoundingBox.Top) / relativeVelocity.Y;
				double t_bottom_min = (o2.BoundingBox.Top - o1.BoundingBox.Bottom) / relativeVelocity.Y;
				double t_top_max;
				double t_bottom_max;

				if (o1.BoundingBox.Top >= o2.BoundingBox.Bottom && o1.BoundingBox.Top <= o2.BoundingBox.Top)
					t_top_min = 0;
				
				if (o1.BoundingBox.Bottom <= o2.BoundingBox.Top && o1.BoundingBox.Bottom >= o2.BoundingBox.Bottom)
					t_bottom_min = 0;

				//We need to calculate the exit time differently, depending on what Y-direction we're moving in
				if (relativeVelocity.Y < 0)
				{
					t_top_max = (o2.BoundingBox.Bottom - o1.BoundingBox.Top) / relativeVelocity.Y;
					t_bottom_max = (o2.BoundingBox.Bottom - o1.BoundingBox.Bottom) / relativeVelocity.Y;
				}
				else
				{
					t_top_max = (o2.BoundingBox.Top - o1.BoundingBox.Top) / relativeVelocity.Y;
					t_bottom_max = (o2.BoundingBox.Top - o1.BoundingBox.Bottom) / relativeVelocity.Y;
				}
				
				if (t_bottom_min >= 0 && t_bottom_min < t_bottom_max)
				{
					normal.Y = 1;
					timeY = t_bottom_min;
				}
				else if (t_top_min >= 0 && t_top_min < t_top_max)
				{
					normal.Y = -1;
					timeY = t_top_min;
				}
			}
			else
			{
				if (o1.BoundingBox.Top >= o2.BoundingBox.Bottom && o1.BoundingBox.Top <= o2.BoundingBox.Top)
					timeY = 0;
				
				if (o1.BoundingBox.Bottom <= o2.BoundingBox.Top && o1.BoundingBox.Bottom >= o2.BoundingBox.Bottom)
					timeY = 0;
			}
			
			//Calculate the collision time.
			double collisionTime;
			if (timeX > timeY)
			{
				collisionTime = timeX;
				normal.Y = 0;
			}
			else
			{
				collisionTime = timeY;
				normal.X = 0;
			}
			
			//If the collision happens at time 0, o1 is inside o2, and the normal vector is undefined.
			//if (collisionTime == 0) normal = null;
			
			if (timeX >= 0 && timeY >= 0 && collisionTime < 1)
			{
				CollisionResult cr = new CollisionResult();
				cr.CollisionTime = collisionTime*frameTime;
			
				collisionEvents.Add(new CollisionEvent(o1, o2, normal, cr));
				collisionEvents.Add(new CollisionEvent(o2, o1, -normal, cr));
			}
		}
	}
}
