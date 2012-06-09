using System;

namespace Engine
{
	public class TransformComponent : GOComponent
	{
		public TransformComponent () : base()
		{
			Position = new Vector();
		}
		
		public TransformComponent (Vector pos) : base()
		{
			Position = pos.Copy();
		}
		
		public Vector Position
		{
			get;
			set;
		}
		
		public override string Family 
		{
			get 
			{
				return "transform";
			}
		}
		
		public override void Update (double frameTime)
		{
		}
	}
}

