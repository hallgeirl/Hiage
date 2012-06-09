using System;

namespace Engine
{
	public abstract class GOComponent
	{
		public GOComponent()
		{
		}
		
		public abstract string Family
		{
			get;
		}
		
		public abstract void Update(double frameTime);
		
		public GameObject Owner
		{
			get;
			internal set;
		}
	}
}

