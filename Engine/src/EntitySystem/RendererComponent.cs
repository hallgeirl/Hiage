using System;

namespace Engine
{
	public class RendererComponent : GOComponent
	{
		Renderer renderer;
		
		public RendererComponent (Renderer renderer) : base()
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
	}
}

