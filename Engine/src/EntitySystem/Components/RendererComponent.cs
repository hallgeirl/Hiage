using System;

namespace Engine
{
	public class RendererComponent : GOComponent
	{
		Renderer renderer;
		
		public RendererComponent(ComponentDescriptor descriptor, ResourceManager resources, Renderer renderer) : base(descriptor, resources)
		{
			this.renderer = renderer;
		}
		
		public override string Family
		{
			get { return "renderer"; }
		}
		
		public override void Update(double frameTime)
		{
			DrawableComponent drawable = (DrawableComponent)Owner.GetComponent("drawable");
			
			renderer.Render(drawable.Renderable);
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "renderer")
				throw new LoggedException("Cannot load RendererComponent from descriptor " + descriptor.Name);
		}
	}
}

