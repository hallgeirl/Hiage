using System;
using System.Collections.Generic;

namespace Engine
{
	public class SpriteComponent : DrawableComponent
	{
		private double animationSpeedFactor = 1;
		
		Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
		Sprite currentSprite;
				
		public SpriteComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}
		
		
		public void SetCurrentSprite(string spriteID)
		{
			currentSprite = sprites[spriteID];
		}
		
		public override IRenderable Renderable
		{
			get { return currentSprite; } 
		}	
		
		public override void Update (double frameTime)
		{
			if (currentSprite == null)
				return;
			
			if (Position != null)
			{
				currentSprite.X = Position.X;
				currentSprite.Y = Position.Y;
			}
			
			currentSprite.Update(frameTime*animationSpeedFactor);
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is SetHorizontalFlipMessage)
				currentSprite.Flipped = ((SetHorizontalFlipMessage)message).Flipped;
			else if (message is SetAnimationSpeedFactorMessage)
				animationSpeedFactor = ((SetAnimationSpeedFactorMessage)message).AnimationSpeedFactor;
			else if (message is PositionChangedMessage)
				Position = ((PositionChangedMessage)message).Position;
			else if (message is PlayAnimationMessage)
				currentSprite.PlayAnimation(((PlayAnimationMessage)message).AnimationName, false);
		}	
		
		private Vector Position
		{
			get; 
			set;
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "sprites")
				throw new LoggedException("Cannot load SpriteComponent from descriptor " + descriptor.Name);	
				
			foreach (ComponentDescriptor s in descriptor.Subcomponents)
			{
				string spriteName = "unnamed_sprite";
				if (!s.Attributes.ContainsKey("id"))
					Log.Write("Sprite in object is missing an ID. Defaulting to \"unnamed_sprite\".", Log.WARNING);
				else
					spriteName = s["id"];
				SpriteDescriptor spriteDesc = resourceManager.GetSpriteDescriptor(s.Value);
				sprites[spriteName] = new Sprite(spriteDesc, resourceManager);
				sprites[spriteName].PlayAnimation(spriteDesc.DefaultAnimation, false);	
			}
			
			if (descriptor.Attributes.ContainsKey("default"))
			    SetCurrentSprite((string)descriptor["default"]);
			
		}
	}
}

