using System;

namespace Engine
{
	public class RenderableChangedMessage : Message
	{
		public RenderableChangedMessage (IRenderable renderable)
		{
			Renderable = renderable;
		}
		
		public IRenderable Renderable
		{
			get;
			private set;
		}
	}
}

