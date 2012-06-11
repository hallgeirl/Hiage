using System;

namespace Engine
{
	public abstract class DrawableComponent : GOComponent
	{
		public static int PlayAnimationMessage;
		static DrawableComponent()
		{
			PlayAnimationMessage = GameObject.RegisterMessage();
		}
		
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
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

