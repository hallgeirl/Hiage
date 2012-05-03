using System;
using System.Diagnostics;
using SdlDotNet.Audio;
using SdlDotNet.Core;
using System.Threading;

using Tao.Sdl;

namespace Engine
{
	public class AudioManager
	{
		ResourceManager resourceManager;
		
		public AudioManager (ResourceManager resmanager)
		{
			resourceManager = resmanager;
			MusicPlayer.EnableMusicFinishedCallback();
		}
		
		/// <summary>
		/// Play a sound clip
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
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
		
		/// <summary>
		/// Plays a music clip
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public void PlayMusic(string intro, string main)
		{
			MusicPlayer.Stop();
			if (main == null) return;
			Music m = resourceManager.GetMusic(main);
			
			if (intro != null && intro != "")
			{

				Music intro_music = resourceManager.GetMusic(intro);
				MusicPlayer.CurrentMusic = intro_music;
				MusicPlayer.Play(1);
				EventHandler<MusicFinishedEventArgs> handler = null;
				handler = delegate (object sender, MusicFinishedEventArgs args) {	Events.MusicFinished -= handler;
																					MusicPlayer.CurrentMusic = m; 
																					MusicPlayer.Play(true);
																				};
				Events.MusicFinished += handler;
			}
			else
			{
				MusicPlayer.CurrentMusic = m;
				MusicPlayer.Play(true);
			}
		}

	}
}

 