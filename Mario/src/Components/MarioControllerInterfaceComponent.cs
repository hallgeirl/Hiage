using System;
using Engine;

namespace Mario
{
	public class MarioControllerInterfaceComponent : ControllerInterfaceComponent
	{
		private bool onGround = true;
		public MarioControllerInterfaceComponent ()
		{
		}
		
		public override void UpAction()
		{
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			
			if (motion != null && onGround)
			{
				motion.Velocity.Y = 200;
			}
				
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
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			
			if (motion != null)
				motion.Accelleration.X -= 400;
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
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			motion.Accelleration.X += 400;
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
			if (message is InAirMessage)
				onGround = false;
			else if (message is LandedMessage)
				onGround = true;
		}
	}
}

