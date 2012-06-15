using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class ObjectFactory
	{
		private Game game;
		Dictionary<string, List<GOComponent>> components = new Dictionary<string, List<GOComponent>>();
//		private MarioGameState state;
		
		public ObjectFactory (Game game, MarioGameState state)
		{
			this.game = game;
//			this.state = state;
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
		
		private GOComponent SpawnComponent(ComponentDescriptor descriptor)
		{
			switch (descriptor.Name)
			{
			case "sprites":
				return new SpriteComponent(descriptor, game.Resources, game.Display.Renderer);
			case "friction":
				return new FrictionComponent(descriptor, game.Resources, true, false);
			case "gravity":
				return new GravityComponent(descriptor, game.Resources, 500);
			case "transform":
				return new TransformComponent(descriptor, game.Resources, new Vector(0,0));
			case "motion":
				return new MotionComponent(descriptor, game.Resources);
//			case "renderer":
//				return new RendererComponent(descriptor, game.Resources, game.Display.Renderer);
			case "statemachine":
				return new StateMachineComponent(descriptor, game.Resources);
			case "speedlimit":
				return new SpeedLimitComponent(descriptor, game.Resources);
			case "collidable":
				return new CollidableComponent(descriptor, game.Resources);
			case "collisionresponse":
				return new CollisionResponseComponent(descriptor, game.Resources);
			case "physicalobjectcollisionresponse":
				return new PhysicalObjectCollisionResponseComponent(descriptor, game.Resources);
			case "mariointerface":
				return new MarioInterfaceComponent(descriptor, game.Resources);
			case "groundenemyinterface":
				return new GroundEnemyInterfaceComponent();
			case "groundai":
				return new DumbGroundAIComponent();
			case "playercontroller":
				return new PlayerController(game.Input);
			}
			return null;
		}
		
		public GameObject Spawn(string objectName, Vector position, Vector velocity, WorldPhysics worldPhysics)
		{
			ObjectDescriptor obj = game.Resources.GetObjectDescriptor(objectName);
			//Sprite sprites = (string.IsNullOrEmpty(obj.Sprite) ? null : new Sprite(game.Resources.GetSpriteDescriptor(obj.Sprite), game.Resources));
//			ObjectPhysics objectPhysics = ObjectPhysics.DefaultObjectPhysics;
			
			GameObject go = new GameObject(obj.Name);
			
			foreach (ComponentDescriptor c in obj.Components)
			{
				GOComponent goc = SpawnComponent(c);
				if (!components.ContainsKey(goc.Family))
					components[goc.Family] = new List<GOComponent>();
				components[goc.Family].Add(goc);
				go.AddComponent(goc);
			}
			
			TransformComponent tr = (TransformComponent)go.GetComponent("transform");
			if (tr != null)
				tr.Position.Set(position);
			
//			GravityComponent g = (GravityComponent)go.GetComponent("gravity");
//			if (g != null)
//				g.
			
			return go;
		}
		
		public Dictionary<string, List<GOComponent>> Components
		{
			get { return components; }
		}
		
//		
//		public GameObject CreateIcon(string sprite, double scaling)
//		{
//			GameObject go = new GameObject("icon");
//			Sprite iconSprite = new Sprite(game.Resources.GetSpriteDescriptor(sprite), game.Resources);
//			Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
//			dict["icon"] = iconSprite;
//			iconSprite.Scaling = scaling;
//			
//			go.AddComponent(new Icon(game, dict));
//			go.AddComponent(new RendererComponent(game.Display.Renderer));
//			go.AddComponent(new TransformComponent());
//			go.AddComponent(new SpriteComponent(iconSprite));
//			
//			return go;
//		}
	}
}
