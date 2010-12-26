using System;
using System.Collections.Generic;
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
			
			//Get any physical attributes for this object
			objectPhysics.Elasticity = obj.GetDoubleProperty("elasticity");
			objectPhysics.Friction = obj.GetDoubleProperty("friction");
			runSpeed = obj.GetDoubleProperty("run-speed");
			maxSpeed = obj.GetDoubleProperty("max-speed");
			
			//Clone the bounding polygon dictionary
			Dictionary<string, BoundingPolygon> boundingPolygons = new Dictionary<string, BoundingPolygon>();
			foreach (var key in obj.BoundingPolygons.Keys)
			{
				boundingPolygons[key] = (BoundingPolygon)obj.BoundingPolygons[key].Clone();
			}
			
			switch (obj.Type)
			{
			case "player":
				return new Player(position, velocity, sprite, game.Display.Renderer, new PlayerController(game.Input), worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed);
			case "enemy":
				switch (obj.Name)
				{
				case "goomba":
					return new BasicGroundEnemy(position, velocity, sprite, game.Display.Renderer, new DumbGroundAI(), worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed);
				default:
					Log.Write("Unknown enemy object name: " + obj.Name, Log.WARNING);
					break;
				}
				break;
			default:
				Log.Write("Unknown object type: " + obj.Type, Log.WARNING);
				break;
			}
			
			return null;
		}
	}
}
