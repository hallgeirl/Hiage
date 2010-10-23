using System;
using Engine;

namespace Mario
{
	public class ObjectFactory
	{
		private Game game;
		
		public ObjectFactory (Game game)
		{
			this.game = game;
		}
		
		public GameObject Spawn(string objectName, Vector position, Vector velocity, WorldPhysics worldPhysics)
		{
			ObjectDescriptor obj = game.Resources.GetObjectDescriptor(objectName);
			Sprite sprite = (string.IsNullOrEmpty(obj.Sprite) ? null : new Sprite(game.Resources.GetSpriteDescriptor(obj.Sprite), game.Resources));
			ObjectPhysics objectPhysics = ObjectPhysics.DefaultObjectPhysics;
			double runSpeed = double.PositiveInfinity, maxSpeed = double.PositiveInfinity;
			int width, height;
			
			//Get any physical attributes for this object
			//delegate double GetDouble(string propname);
			
			
			objectPhysics.Elasticity = obj.GetDoubleProperty("elasticity");
			objectPhysics.Friction = obj.GetDoubleProperty("friction");
			runSpeed = obj.GetDoubleProperty("run-speed");
			maxSpeed = obj.GetDoubleProperty("max-speed");
			width = obj.GetIntProperty("width");
			height = obj.GetIntProperty("height");
			
			
			switch (obj.Type)
			{
			case "player":
				return new Player(position, velocity, sprite, game.Display.Renderer, new PlayerController(game.Input), worldPhysics, objectPhysics, runSpeed, maxSpeed);
			case "enemy":
				switch (obj.Name)
				{
				case "goomba":
					return new BasicGroundEnemy(position, velocity, sprite, game.Display.Renderer, new DumbGroundAI(), worldPhysics, objectPhysics, width, height, runSpeed, maxSpeed);
				default:
					Log.Write("Unknown enemy object name: " + obj.Name);
					break;
				}
				break;
			default:
				Log.Write("Unknown object type: " + obj.Type);
				break;
			}
			
			
			return null;
		}
	}
}
