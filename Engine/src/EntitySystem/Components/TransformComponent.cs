using System;

namespace Engine
{
	public class TransformComponent : GOComponent
	{
		Vector position = new Vector();
		
		public TransformComponent(ComponentDescriptor descriptor, ResourceManager resources, Vector position) : base(descriptor, resources)
		{
			if (position != null)
				this.position = position.Copy();
			
			OwnerSet += delegate {
				Owner.BroadcastMessage(new PositionChangedMessage(Position));
				Owner.ComponentAdded += delegate(object sender, GOComponent component) {
					component.SendMessage(new PositionChangedMessage(Position));
				};
			};
		}
		
		public Vector Position
		{
			get { return position; }
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
				
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "transform")
				throw new LoggedException("Cannot load TransformComponent from descriptor " + descriptor.Name);	
				
			if (descriptor.Attributes.ContainsKey("x"))
			    Position.X = double.Parse(descriptor["x"]);
			if (descriptor.Attributes.ContainsKey("y"))
			    Position.Y = double.Parse(descriptor["y"]);
		}
		
	}
}

