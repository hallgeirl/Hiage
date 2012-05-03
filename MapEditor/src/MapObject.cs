
using System;
using System.Collections.Generic;
using Engine;

namespace MapEditor
{
	/// <summary>
	/// Represents an object placed on the map
	/// </summary>
	public class MapObject
	{
		Sprite sprite;
		Renderer renderer;
		
		
		public MapObject (ObjectDescriptor descriptor, ResourceManager resources, Renderer renderer, Vector position)
		{
			SpriteDescriptor spriteDesc = resources.GetSpriteDescriptor(descriptor.Sprites[descriptor.DefaultSprite]);
			sprite = new Sprite(spriteDesc, resources);
			Position = position;
			this.renderer = renderer;
			
			sprite.PlayAnimation(spriteDesc.DefaultAnimation, true);
			
			Name = descriptor.Name;
			ExtraProperties = new Dictionary<string, string>();
		}
		
		public void Update(double frameTime)
		{
			sprite.X = Position.X;
			sprite.Y = Position.Y;
			sprite.Update(frameTime);
		}
		
		public void Render()
		{
			renderer.Render(sprite);
		}
		
		//Additional object properties per instance
		public Dictionary<string, string> ExtraProperties
		{
			get;
			private set;
		}

		
		public Vector Position
		{
			get;
			private set;
		}
		
		public string Name
		{
			get;
			private set;
		}
		
		public double Left
		{
			get { return sprite.X - sprite.Width / 2; }
		}
		
		public double Right
		{
			get { return sprite.X + sprite.Width / 2; }
		}

		public double Top
		{
			get { return sprite.Y + sprite.Height / 2; }
		}

		public double Bottom
		{
			get { return sprite.Y - sprite.Height / 2; }
		}

	}
}
