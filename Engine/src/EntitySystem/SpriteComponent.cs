using System;

namespace Engine
{
	public class SpriteComponent : DrawableComponent
	{
		public SpriteComponent (Sprite sprite) : base()
		{
			this.sprite = sprite;
			sprite.PlayAnimation(Sprite.DEFAULT_ANIMATION, true);
		}
		
		private Sprite sprite;
		
		public override IRenderable Renderable
		{
			get { return sprite; } 
		}	
		
		public override void Update (double frameTime)
		{
			TransformComponent transform = (TransformComponent)Owner.GetComponent("transform");
			sprite.X = transform.Position.X;
			sprite.Y = transform.Position.Y;
			//CurrentSprite.Update(frameTime*animationSpeedFactor);
			
			sprite.Update(frameTime);
		}
		
	}
}

