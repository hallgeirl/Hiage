using System;
using System.Collections.Generic;

namespace Engine
{
	public class CollidableComponent : GOComponent
	{
		public CollidableComponent (BoundingPolygon bp)
		{
			BoundingPolygon = bp;
		}
		
		public override string Family 
		{
			get 
			{
				return "collidable";
			}
		}
		
		public override void Update (double frameTime)
		{
			TransformComponent transform = (TransformComponent)Owner.GetComponent("transform");
			BoundingPolygon.MoveTo(transform.Position.X, transform.Position.Y);
		}
		
		public BoundingPolygon BoundingPolygon
		{
			get;
			private set;
		}
		
		internal Vector Velocity
		{
			get
			{
				MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
				if (motion != null)
					return motion.Velocity;
				return new Vector(0,0);
			}
		}
		
		public void TestCollision(List<BoundingPolygon> polygons, double frameTime)
		{
			TestCollision(polygons, frameTime, -1);
		}
		
		public void TestCollision(List<BoundingPolygon> polygons, double frameTime, int axis)
		{
			CollisionResult r = CollisionManager.TestCollision(this, polygons, frameTime, axis);
			if (r.isIntersecting || r.hasIntersected)
			{
				CollisionResponseComponent cr = (CollisionResponseComponent)Owner.GetComponent("collisionresponse");
				
				cr.RegisterCollision(r);
			}
		}		
		
		public Box GetCollisionCheckArea(double frameTime)
		{
			Box collisionCheckArea;
			double dx = Math.Abs(Velocity.X)*frameTime;
			double dy = Math.Abs(Velocity.Y)*frameTime;
			collisionCheckArea.Left = BoundingPolygon.Left-dx;
			collisionCheckArea.Right = BoundingPolygon.Right+dx;
			collisionCheckArea.Top = BoundingPolygon.Top+dy;
			collisionCheckArea.Bottom = BoundingPolygon.Bottom-dy;
			
			return collisionCheckArea;
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

