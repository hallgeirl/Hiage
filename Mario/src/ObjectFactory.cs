using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class ObjectFactory
	{
		private Game game;
		private MarioGameState state;
		
		public ObjectFactory (Game game, MarioGameState state)
		{
			this.game = game;
			this.state = state;
		}
		
		private double TryGetDoubleProperty(ObjectDescriptor obj, string prop)
		{
			double val = 0;
			try
			{
				val = obj.GetDoubleProperty(prop);
			}
			catch (KeyNotFoundException)
			{
				val = 0;
			}
			
			return val;
		}
		
		public GameObject Spawn(string objectName, Vector position, Vector velocity, WorldPhysics worldPhysics)
		{
			ObjectDescriptor obj = game.Resources.GetObjectDescriptor(objectName);
			Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
			foreach (var s in obj.Sprites)
			{
				SpriteDescriptor spriteDesc = game.Resources.GetSpriteDescriptor(s.Value);
				sprites[s.Key] = new Sprite(spriteDesc, game.Resources);
				sprites[s.Key].PlayAnimation(spriteDesc.DefaultAnimation, false);
			}
			//Sprite sprites = (string.IsNullOrEmpty(obj.Sprite) ? null : new Sprite(game.Resources.GetSpriteDescriptor(obj.Sprite), game.Resources));
			ObjectPhysics objectPhysics = ObjectPhysics.DefaultObjectPhysics;
			double runSpeed = double.PositiveInfinity, maxSpeed = double.PositiveInfinity;
			
			//Get any physical attributes for this object
			objectPhysics.Elasticity = TryGetDoubleProperty(obj, "elasticity");
			objectPhysics.Friction = TryGetDoubleProperty(obj, "friction");
							
			runSpeed = TryGetDoubleProperty(obj, "run-speed");
			maxSpeed = TryGetDoubleProperty(obj, "max-speed");
			
			//Clone the bounding polygon dictionary
			Dictionary<string, BoundingPolygon> boundingPolygons = new Dictionary<string, BoundingPolygon>();
			foreach (var key in obj.BoundingPolygons.Keys)
			{
				boundingPolygons[key] = (BoundingPolygon)obj.BoundingPolygons[key].Clone();
			}
			
			switch (obj.Type)
			{
			case "player":
				return new Player(game, position, velocity, sprites, obj.DefaultSprite, new PlayerController(game.Input), worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed, state.PlayerState);
				//return new Player(position, velocity, sprite, game.Display.Renderer, new PlayerController(game.Input), worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed, state.PlayerState);
			
			case "enemy":
				switch (obj.Name)
				{
				case "goomba":
					return new BasicGroundEnemy(game, position, velocity, sprites, obj.DefaultSprite, new DumbGroundAI(), worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed);
				default:
					Log.Write("Unknown enemy object name: " + obj.Name, Log.WARNING);
					break;
				}
				break;
				
			case "coin":
				return new Coin(game, position, sprites, obj.DefaultSprite, boundingPolygons);
			
			default:
				Log.Write("Unknown object type: " + obj.Type, Log.WARNING);
				break;
			}
			
			return null;
		}
		
		public Icon CreateIcon(string sprite, double scaling)
		{
			Sprite iconSprite = new Sprite(game.Resources.GetSpriteDescriptor(sprite), game.Resources);
			Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
			dict["icon"] = iconSprite;
			iconSprite.Scaling = scaling;
			
			return new Icon(game, dict);
		}
	}
}
