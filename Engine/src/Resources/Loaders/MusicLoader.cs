
using System;
using System.IO;
using SdlDotNet.Audio;

namespace Engine
{
	
	
	public class MusicLoader : IResourceLoader<Music>
	{
		public Music LoadResource(string filename, string name)
		{
			Music s = new Music(filename);
			
			return s;
		}
	}
}
