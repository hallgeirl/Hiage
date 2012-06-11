using System;

namespace Engine
{
	public class TransformComponent : GOComponent
	{
		public TransformComponent () : base()
		{
			Position = new Vector();
		}
		
		public TransformComponent (Vector pos) : base()
		{
			Position = pos.Copy();
		}
		
		public Vector Position
		{
			get;
			private set;
		}
		
		public override string Family 
		{
			get 
			{
				return "transform";
			}
		}
		
		public void Update(double frameTime, int axis)
		{
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			
			if (motion != null)
				Position[axis] += (motion.Velocity[axis] * frameTime); 
		}
		
		public override void Update (double frameTime)
		{
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			
			if (motion != null)
				Position.Add(motion.Velocity * frameTime); 
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

