using System;
using Engine;

namespace Mario
{
	public class GroundEnemyInterfaceComponent : ControllerInterfaceComponent
	{
		bool onGround;
		public GroundEnemyInterfaceComponent ()
		{
		}
		
		public override void UpAction()
		{
				
			/*if (OnGround)
			{
				Velocity.Y = 200;
				game.Audio.PlaySound("mario-jump");
				OnGround = false;
				//Accellerate(new Vector(0,200));
			}*/
		}
		
		public override void DownAction()
		{
			/*crouching = true;
			//Position.Y -= 2;
			
			if (currentState != crouchState)
			{
				SetState(crouchState);
				
			}*/
		}
		
		public override void LeftAction()
		{
			Accelleration.X -= 400;
			/*if (OnGround)
			{
				if (!crouching)
					Accellerate(new Vector(-400, 0));
			}
			else*/
			//	Accellerate(new Vector(-200, 0));
		}
		
		public override void RightAction()
		{
			Accelleration.X += 400;
			/*if (OnGround)
			{
				if (!crouching)
					Accellerate(new Vector(400, 0));
			}
			else */
			//	Accellerate(new Vector(200, 0));
		}
		
		public override void ReceiveMessage (Message message)
		{
			base.ReceiveMessage(message);
			
			if (message is InAirMessage)
				onGround = false;
			else if (message is LandedMessage)
				onGround = true;
			else if (message is VelocityChangedMessage)
				Velocity = ((VelocityChangedMessage)message).Velocity;
			else if (message is AccellerationChangedMessage)
				Accelleration = ((AccellerationChangedMessage)message).Accelleration;
		}
		
		private Vector Accelleration
		{
			get;set;
		}
		
		private Vector Velocity
		{
			get;set;
		}
		
	}
}

