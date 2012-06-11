using System;

namespace Engine
{
	public class SpriteComponent : DrawableComponent
	{
		private double animationSpeedFactor = 1;
		
		public SpriteComponent (Sprite sprite) : base()
		{
			this.sprite = sprite;
			sprite.PlayAnimation(Sprite.DEFAULT_ANIMATION, true);
			
			OwnerSet += delegate {
				Owner.Subscribe(DrawableComponent.PlayAnimationMessage, this, 
				               delegate(object messageData) { sprite.PlayAnimation((string)messageData, false); });
			};
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
			
			sprite.Update(frameTime*animationSpeedFactor);
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is SetHorizontalFlipMessage)
				sprite.Flipped = ((SetHorizontalFlipMessage)message).Flipped;
			else if (message is SetAnimationSpeedFactorMessage)
				animationSpeedFactor = ((SetAnimationSpeedFactorMessage)message).AnimationSpeedFactor;
		}		
	}
}

