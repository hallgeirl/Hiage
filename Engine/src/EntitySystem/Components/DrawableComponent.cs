using System;

namespace Engine
{
	public abstract class DrawableComponent : GOComponent
	{
		public DrawableComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}
		
		public override string Family
		{
			get { return "drawable"; }
		}
		
		public abstract IRenderable Renderable
		{
			get; 
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

