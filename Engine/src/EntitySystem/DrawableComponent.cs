using System;

namespace Engine
{
	public abstract class DrawableComponent : GOComponent
	{
		public DrawableComponent () : base()
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
	}
}

