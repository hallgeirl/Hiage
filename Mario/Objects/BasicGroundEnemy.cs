
using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{


	public class BasicGroundEnemy : Character
	{
		protected int dieState;
		protected Timer dieTimer = new Timer();
		
		public BasicGroundEnemy (Vector position, Vector velocity, Sprite sprite, Renderer renderer, IController controller, //GameObject attributes
		               WorldPhysics worldPhysics, ObjectPhysics objectPhysics, Dictionary<string, BoundingPolygon> boundingPolygons,	//PhysicalObject attributes
		               double runSpeed, double maxSpeed) 	//Character attributes
			: base(position, velocity, sprite, renderer, controller, worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed) 
		{
			Stompable = true;
		}
		
		protected override void SetupStates ()
		{
			base.SetupStates();
			
			dieState = AddState(delegate {
				Sprite.PlayAnimation("die", false);
			});
		}
		
		public override void Update(double frameTime)
		{
			base.Update(frameTime);
			if (dieTimer.Elapsed > 1000)
				Delete = true;
		}

		public override void UpAction ()
		{
		}

		public override void DownAction()
		{
		}

		public override void RightAction()
		{
			if (currentState != dieState)
				Accellerate(new Vector(300, 0));
		}
		
		public override void LeftAction()
		{
			if (currentState != dieState)
				Accellerate(new Vector(-300, 0));
		}
		
		public override BoundingPolygon BoundingBox 
		{
			get 
			{
				return boundingPolygons["normal"];
			}
		}
		
		public bool Stompable
		{
			get;
			private set;
		}
		
		public void Kill()
		{
			currentState = dieState;
			dieTimer.Start();
			
			Dying = true;
		}
		
		public bool Dying
		{
			get;
			private set;
		}
	}
}
