using System;

namespace Engine
{
	public class FrictionComponent : GOComponent
	{
		double friction;
		bool applyToX, applyToY;
		
		public FrictionComponent (double friction, bool applyToX, bool applyToY) : base()
		{
			this.friction = friction;
			this.applyToX = applyToX;
			this.applyToY = applyToY;
		}
		
		public override string Family {
			get {
				return "friction";
			}
		}
		
		public double Friction
		{
			get { return friction; }
		}
		
		public override void Update (double frameTime)
		{
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			if (applyToX)
			{
				if (motion.Velocity.X > friction*frameTime)
					motion.Accelleration.X -= friction;
				else if (motion.Velocity.X < -friction*frameTime)
					motion.Accelleration.X += friction;
			}
			else if (applyToY)
			{
				if (motion.Velocity.Y > friction*frameTime)
					motion.Accelleration.Y -= friction;
				else if (motion.Velocity.Y < -friction*frameTime)
					motion.Accelleration.Y += friction;
			}
		}
		
		public void ClampVelocity(Vector velocity, double frameTime)
		{
			if (applyToX)
			{
				if (velocity.X <= Friction*frameTime && velocity.X >= -Friction*frameTime)
				    velocity.X = 0;
			}
			if (applyToY)
			{
				if (velocity.Y <= Friction*frameTime && velocity.Y >= -Friction*frameTime)
				    velocity.Y = 0;
			}
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

