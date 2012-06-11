//#undef DEBUG
using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	//A generic game object class
	public abstract class GameObjectComponent : GOComponent, ICollidable
	{
		protected Dictionary<string, BoundingPolygon> boundingPolygons;
		protected double remainingFrameTime = 1;
		protected double animationSpeedFactor = 1;
		protected double frameTime;
		protected Game game;

		private double left, right;
		private bool cachedLeft, cachedRight;
		
		//States. These may be used for animation etc.
		protected delegate void ObjectState();
		protected List<ObjectState> objectStates = new List<ObjectState>();
		protected int currentState = -1, prevState = -1;
		protected int framesInCurrentState = 0;
		
		//Construct a game object. Set controller to null if the object should be static.
		public GameObjectComponent(Game game, Dictionary<string, BoundingPolygon> boundingPolygons)
		{
			this.game = game;
			this.boundingPolygons = boundingPolygons;
			//boundingPolygon.MoveTo(Position.X, Position.Y);
			
			CanCollide = true;
			
			SetupStates();
		}
		
		protected void SetState(int newstate)
		{
			prevState = currentState;
			currentState = newstate;
			framesInCurrentState = 0;
			
			if (this is Player)
			Console.WriteLine("Setting state to " + newstate);
			//objectStates[currentState]();
		}
		
		protected abstract void SetupStates();
		
		//Add a new state to this object
		protected int AddState(ObjectState state)
		{
			objectStates.Add(state);
			return objectStates.Count-1;
		}
		
		protected void OverloadState(int stateID, ObjectState state)
		{
			objectStates[stateID] = state;
		}
		
		public void Prepare(double frameTime)
		{
			this.frameTime = frameTime;
			cachedLeft = false;
			cachedRight = false;
		}
		
		//Update this object's position and such
		public virtual void UpdatePosition(double frameTime)
		{
			//Position += Velocity*frameTime;
			//remainingFrameTime = 1;
			
			//Run the current object state
			if (currentState != -1)
				objectStates[currentState]();
		}
				
		public override void Update(double frameTime)
		{
			UpdatePosition(frameTime);
			framesInCurrentState++;
		}
		
		#region ICollidable specifics
		//Bounding box before update
		public abstract BoundingPolygon BoundingPolygon
		{
			get;
		}
		
		//Returns a bounding box covering all the potential area where collisions may occur
		Box collisionCheckArea;
		public Box GetCollisionCheckArea(double frameTime)
		{
			double dx = Math.Abs(Velocity.X)*frameTime;
			double dy = Math.Abs(Velocity.Y)*frameTime;
			collisionCheckArea.Left = BoundingPolygon.Left-dx;
			collisionCheckArea.Right = BoundingPolygon.Right+dx;
			collisionCheckArea.Top = BoundingPolygon.Top+dy;
			collisionCheckArea.Bottom = BoundingPolygon.Bottom-dy;
			
			return collisionCheckArea;
		}

		public virtual void Collide(BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
			ControllerComponent controller = (ControllerComponent)Owner.GetComponent("controller");
			if (controller != null)
				controller.HandleCollision(this, p, collisionNormal, collisionResult);
		}
		
		public virtual void Collide(ICollidable o, Vector edgeNormal, CollisionResult collisionResult)
		{
			ControllerComponent controller = (ControllerComponent)Owner.GetComponent("controller");
			if (controller != null)
				controller.HandleCollision(this, (GameObjectComponent)o, collisionResult);
		}
		#endregion

		#region Control
		
		public abstract void UpAction();		
		public abstract void LeftAction();
		public abstract void DownAction();
		public abstract void RightAction();
		
		#endregion
		
		public Sprite CurrentSprite
		{
			get 
			{
				DrawableComponent d = (DrawableComponent)Owner.GetComponent("drawable"); 
				return (Sprite)d.Renderable;
			} 
		}
		
		public void Accellerate(Vector accelVector)
		{
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			
			motion.Accelleration.Add(accelVector);
		}
		
		//Current velocity
		public Vector Velocity
		{
			get
			{
				MotionComponent physics = (MotionComponent)Owner.GetComponent("motion");
				return physics.Velocity;
			}
			protected set 
			{
				MotionComponent physics = (MotionComponent)Owner.GetComponent("motion");
				physics.Velocity.Set(value);
			}
		}
		
		//Current position
		public Vector Position
		{
			get
			{
				TransformComponent transform = (TransformComponent)Owner.GetComponent("transform");
				return transform.Position;
			}
			set
			{
				TransformComponent transform = (TransformComponent)Owner.GetComponent("transform");
				transform.Position.Set(value);
				if (BoundingPolygon != null)
					BoundingPolygon.MoveTo(transform.Position.X, transform.Position.Y);
			}
		}
		
		public double Left
		{
			get 
			{ 
				if (!cachedLeft)
				{
					left = Math.Min(BoundingPolygon.Left, BoundingPolygon.Left + Velocity.X*frameTime); 
					cachedLeft = true;
				}
				
				return left;
			}
		}
		
		public double Right
		{
			get 
			{ 
				if (!cachedRight)
				{
					right = Math.Max(BoundingPolygon.Right, BoundingPolygon.Right + Velocity.X*frameTime); 
					cachedRight = true;
				}
				
				return right;
			}
		}
	
		//Set to true to delete this object next frame
		public bool Delete
		{
			get;
			set;
		}
		
		public bool CanCollide
		{
			get;
			set;
		}
		
		public override string Family
		{
			get { return "go"; }
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}
