using System;

namespace Engine
{
	public abstract class DrawableComponent : GOComponent
	{
		protected Renderer renderer;
		public DrawableComponent(ComponentDescriptor descriptor, ResourceManager resources, Renderer renderer) : base(descriptor, resources)
		{
			OwnerSet += delegate {
				Owner.BroadcastMessage(new RenderableChangedMessage(Renderable));
				Owner.ComponentAdded += delegate(object sender, GOComponent component) {
					component.SendMessage(new RenderableChangedMessage(Renderable));
				};
			};
			this.renderer = renderer;
		}
		
		public override string Family
		{
			get { return "drawable"; }
		}
		
		IRenderable renderable;
		public IRenderable Renderable
		{
			get { return renderable; }
			protected set 
			{ 
				renderable = value; 
				if (Owner != null) 
					Owner.BroadcastMessage(new RenderableChangedMessage(renderable)); }
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is PositionChangedMessage)
				Position = ((PositionChangedMessage)message).Position;
			else if (message is ScalingChangedMessage)
				Scale = ((ScalingChangedMessage)message).Scaling;
		}	
		
		protected Vector Position
		{
			get; 
			set;
		}
		protected Vector Scale
		{
			get;
			set;
		}
		
	}
}

