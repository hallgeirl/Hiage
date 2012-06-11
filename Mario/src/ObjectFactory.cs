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
			
			GameObject go = new GameObject(obj.Type);
			
			switch (obj.Type)
			{
			case "player":
			{
				go.AddComponent(new Player(game, boundingPolygons, runSpeed, maxSpeed, state.PlayerState));
				go.AddComponent(new PlayerController(game.Input));
				go.AddComponent(new MarioControllerInterfaceComponent());
				go.AddComponent(new GravityComponent(worldPhysics.Gravity));
				go.AddComponent(new FrictionComponent(worldPhysics.GroundFrictionFactor * objectPhysics.Friction, true, false));
				
				//collision
				CollisionResponseComponent cr = new CollisionResponseComponent();
				cr.AddHandler(new PhysicalObjectCollisionHandler());
				cr.AddHandler(new InAirCollisionHandler());
				go.AddComponent(cr);
				go.AddComponent(new CollidableComponent(boundingPolygons["small-standing"]));
				
				//states
				StateMachineComponent su = new StateMachineComponent();
				go.AddComponent(su);
				su.AddState(new WalkState(su, runSpeed));
				su.AddState(new RunState(su,runSpeed));
				su.AddState(new StandState(su));
				su.AddState(new InAirState(su));

				go.BroadcastMessage(new SetStateMessage("state_stand"));
				                   
				
				break;
			}
			
			case "enemy":
				switch (obj.Name)
				{
				case "goomba":
				{
					go.AddComponent(new BasicGroundEnemy(game, boundingPolygons, runSpeed, maxSpeed));
					go.AddComponent(new DumbGroundAI());
					go.AddComponent(new GravityComponent(worldPhysics.Gravity));
					go.AddComponent(new FrictionComponent(worldPhysics.GroundFrictionFactor * objectPhysics.Friction, true, false));
				
					StateMachineComponent su = new StateMachineComponent();
					go.AddComponent(su);
					su.AddState(new WalkState(su, runSpeed));
					su.AddState(new RunState(su,runSpeed));
					su.AddState(new StandState(su));
					su.AddState(new InAirState(su));
	
					go.AddComponent(new SpeedLimitComponent(maxSpeed));
					
					//collision
					CollisionResponseComponent cr = new CollisionResponseComponent();
					cr.AddHandler(new PhysicalObjectCollisionHandler());
					cr.AddHandler(new InAirCollisionHandler());
					go.AddComponent(cr);
					go.AddComponent(new CollidableComponent(boundingPolygons["normal"]));
					break;
				}
				default:
					Log.Write("Unknown enemy object name: " + obj.Name, Log.WARNING);
					break;
				}
				break;
				
			case "coin":
				go.AddComponent(new Coin(game, boundingPolygons));
				break;
			
			case "mushroom":
				Mushroom.ItemType itemType = Mushroom.ItemType.RedMushroom;
				switch (obj.Name)
				{
				case "mushroom-red":
					itemType = Mushroom.ItemType.RedMushroom;
					break;
				case "mushroom-green":
					itemType = Mushroom.ItemType.GreenMushroom;
					break;
				}
				go.AddComponent(new Mushroom(game, boundingPolygons, itemType));
				go.AddComponent(new DumbGroundAI());
				go.AddComponent(new GravityComponent(worldPhysics.Gravity));
				go.AddComponent(new FrictionComponent(worldPhysics.GroundFrictionFactor * objectPhysics.Friction, true, false));
				break;
			default:
				Log.Write("Unknown object type: " + obj.Type, Log.WARNING);
				break;
			}
			
			go.AddComponent(new TransformComponent(position));
			go.AddComponent(new RendererComponent(game.Display.Renderer));
			go.AddComponent (new SpriteComponent(sprites[obj.DefaultSprite]));
			//go.AddComponent(new PhysicsComponent(velocity, new Vector(0,0)));
			go.AddComponent(new MotionComponent(velocity, new Vector(0,0)));
			return go;
		}
		
		public GameObject CreateIcon(string sprite, double scaling)
		{
			GameObject go = new GameObject("icon");
			Sprite iconSprite = new Sprite(game.Resources.GetSpriteDescriptor(sprite), game.Resources);
			Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
			dict["icon"] = iconSprite;
			iconSprite.Scaling = scaling;
			
			go.AddComponent(new Icon(game, dict));
			go.AddComponent(new RendererComponent(game.Display.Renderer));
			go.AddComponent(new TransformComponent());
			go.AddComponent(new SpriteComponent(iconSprite));
			
			return go;
		}
	}
}
