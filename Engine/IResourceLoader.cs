
using System;

namespace Engine
{
	/// <summary>
	/// Interface for resource loaders: Texture loaders, etc.
	/// </summary>
	public interface IResourceLoader<T>
	{
		T LoadResource(string name);
	}
}
