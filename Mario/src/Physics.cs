
using System;

namespace Mario
{
	
	/// <summary>
	/// Represents per-object physical attributes, like friction and elasticity
	/// </summary>
	public class ObjectPhysics
	{
		public ObjectPhysics(double friction, double elasticity)
		{
			Friction = friction;
			Elasticity = elasticity;
		}
		
		public double Friction
		{
			get;
			set;
		}
		
		public double Elasticity
		{
			get;
			set;
		}
		
		public static ObjectPhysics DefaultObjectPhysics
		{
			get
			{
				return new ObjectPhysics(0, 0);
			}
		}
		
		public override string ToString ()
		{
			return string.Format("[ObjectPhysics: Friction={0}, Elasticity={1}]", Friction, Elasticity);
		}
	}
	
	/// <summary>
	/// Represents physics attributes of the map, like gravity.
	/// </summary>
	public class WorldPhysics
	{
		
		public WorldPhysics (double gravity, double groundFrictionFactor)
		{
			Gravity = gravity;
			GroundFrictionFactor = groundFrictionFactor;
		}
		
		/// <summary>
		/// How much gravity is there on this map?
		/// </summary>
		public double Gravity
		{
			get;
			set;
		}
		
		/// <summary>
		/// Ground friction factor (large => objects are affected in a large degree by friction, small => objects are affected in a smaller degree)
		/// </summary>
		public double GroundFrictionFactor
		{
			get;
			set;
		}
		
		public static WorldPhysics DefaultWorldPhysics
		{
			get
			{
				return new WorldPhysics(0, 1);
			}
		}
		
		public override string ToString ()
		{
			return string.Format("[WorldPhysics: Gravity={0}, GroundFrictionFactor={1}]", Gravity, GroundFrictionFactor);
		}
	}
}
