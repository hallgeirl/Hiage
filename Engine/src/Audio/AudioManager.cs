using System;
using SdlDotNet.Audio;

namespace Engine
{
	public class AudioManager
	{
		ResourceManager resourceManager;
		
		public AudioManager (ResourceManager resmanager)
		{
			this.resourceManager = resmanager;
		}
		
		public void PlaySound(string name)
		{
			Sound s = resourceManager.GetAudioClip(name);
			try
			{
				s.Play();
			}
			catch(Exception)
			{
			}
		}
		
	}
}

