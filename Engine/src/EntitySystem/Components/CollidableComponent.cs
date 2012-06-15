using System;
using System.Collections.Generic;

namespace Engine
{
	public class CollidableComponent : GOComponent
	{
		Dictionary<string, BoundingPolygon> boundingPolygons = new Dictionary<string, BoundingPolygon>();
		double frameTime;
		
		public CollidableComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
			if (descriptor.Attributes.ContainsKey("default"))
				SetCurrentBoundingPolygon((string)descriptor["default"]);
		}
		
		public override void Update (double frameTime)
		{
			if (Position == null) return;
			CurrentBoundingPolygon.MoveTo(Position.X, Position.Y);
			this.frameTime = frameTime;
		}
		
		public void SetCurrentBoundingPolygon(string name)
		{
			CurrentBoundingPolygon = boundingPolygons[name];
		}
		
		public void TestCollision(List<BoundingPolygon> polygons, double frameTime)
		{
			TestCollision(polygons, frameTime, -1);
		}
		
		public void TestCollision(List<BoundingPolygon> polygons, double frameTime, int axis)
		{
			CollisionResult r = CollisionManager.TestCollision(CurrentBoundingPolygon, Velocity, polygons, frameTime, axis);
			if (r.isIntersecting || r.hasIntersected)
			{
				CollisionResponseComponent cr = (CollisionResponseComponent)Owner.GetComponent("collisionresponse");
				
				if (cr != null)
					cr.RegisterCollision(r);
			}
		}
		
		public void TestCollision(CollidableComponent o2, double frameTime)
		{
			CollisionResult r = CollisionManager.TestCollision(this.CurrentBoundingPolygon, this.Velocity, o2.CurrentBoundingPolygon, o2.Velocity, frameTime);
			
			if (r.isIntersecting || r.hasIntersected)
			{
				CollisionResponseComponent cr = (CollisionResponseComponent)Owner.GetComponent("collisionresponse");
				if (cr != null)
					cr.RegisterObjectObjectCollision(r, this, o2);
			}
		}
		
		public Box GetCollisionCheckArea(double frameTime, int axis)
		{
			Box collisionCheckArea;
			if (CurrentBoundingPolygon == null)
				collisionCheckArea.Bottom = collisionCheckArea.Left = collisionCheckArea.Right = collisionCheckArea.Top = 0;
			else
			{
				double dx = 0, dy = 0;
				if (axis <= 0 && Velocity != null)
					dx = Math.Abs(Velocity.X)*frameTime;
				else if ((axis < 0 || axis == 1) && Velocity != null)
					dy = Math.Abs(Velocity.Y)*frameTime;
				
				collisionCheckArea.Left = CurrentBoundingPolygon.Left-dx;
				collisionCheckArea.Right = CurrentBoundingPolygon.Right+dx;
				collisionCheckArea.Top = CurrentBoundingPolygon.Top+dy;
				collisionCheckArea.Bottom = CurrentBoundingPolygon.Bottom-dy;
			}
			return collisionCheckArea;
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is PositionChangedMessage)
			{
				Position = ((PositionChangedMessage)message).Position;
			}
			
			if (message is VelocityChangedMessage)
			{
				Velocity = ((VelocityChangedMessage)message).Velocity;
			}
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "collidable")
				throw new LoggedException("Cannot load CollidableComponent from descriptor " + descriptor.Name);
			
			//Load bounding polygons
			foreach (ComponentDescriptor p in descriptor.Subcomponents)
			{
				BoundingPolygon poly = new BoundingPolygon();
				
				//load vertices
				foreach (ComponentDescriptor v in p.Subcomponents)
				{
					Vector vert = new Vector();
					vert.X = double.Parse(v["x"]);
					vert.Y = double.Parse(v["y"]);
					poly.AddVertex(vert);
				}
				boundingPolygons[p["id"]] = poly;
			}
		}
				
		#region Properties
		public override string Family 
		{
			get 
			{
				return "collidable";
			}
		}	
		
		
		public double Left
		{
			get 
			{ 
				if (CurrentBoundingPolygon == null) return 0;
				else return Math.Min(CurrentBoundingPolygon.Left, CurrentBoundingPolygon.Left + (Velocity != null ? Velocity.X*frameTime : 0)); }
		}
		public double Right
		{
			get 
			{ 
				if (CurrentBoundingPolygon == null) return 0;
				else return Math.Max(CurrentBoundingPolygon.Right, CurrentBoundingPolygon.Right + (Velocity != null ? Velocity.X*frameTime : 0)); 
			}
		}
		public double Bottom
		{
			get 
			{ 
				if (CurrentBoundingPolygon == null) return 0;
				else return Math.Min(CurrentBoundingPolygon.Bottom, CurrentBoundingPolygon.Bottom + (Velocity != null ? Velocity.Y*frameTime : 0)); 
			}
		}
		public double Top
		{
			get 
			{ 
				if (CurrentBoundingPolygon == null) return 0;
				return Math.Max(CurrentBoundingPolygon.Top, CurrentBoundingPolygon.Top + (Velocity != null ? Velocity.Y*frameTime : 0)); 
			}
		}
		
		public BoundingPolygon CurrentBoundingPolygon
		{
			get; 
			private set;
		}
		
		private Vector Position
		{
			get; 
			set;
		}
		
		private Vector Velocity
		{
			get;
			set;
		}
		#endregion

	}
}

