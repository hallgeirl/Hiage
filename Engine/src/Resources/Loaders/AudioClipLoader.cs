
using System;
using System.IO;
using SdlDotNet.Audio;

namespace Engine
{
	
	
	public class AudioClipLoader : IResourceLoader<Sound>
	{
		public Sound LoadResource(string filename, string name)
		{
			Sound s = new Sound(filename);
			
			return s;
		}
	}
}
