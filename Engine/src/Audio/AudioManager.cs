using System;
using SdlDotNet.Audio;
using SdlDotNet.Core;

namespace Engine
{
	public class AudioManager
	{
		ResourceManager resourceManager;
		
		public AudioManager (ResourceManager resmanager)
		{
			this.resourceManager = resmanager;
			MusicPlayer.EnableMusicFinishedCallback();
			Events.Quit += new EventHandler<QuitEventArgs>(delegate (object sender, QuitEventArgs args)
			                                              {
														       MusicPlayer.Stop();
													      });
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
			Music m = resourceManager.GetMusic(main);
			
			if (intro != null && intro != "")
			{

				Music intro_music = resourceManager.GetMusic(intro);
				MusicPlayer.CurrentMusic = intro_music;
				MusicPlayer.Play(1);
				Events.MusicFinished += new EventHandler<MusicFinishedEventArgs>(delegate (object sender, MusicFinishedEventArgs args) 
				                                                                { 
																					MusicPlayer.CurrentMusic = m; 
																					MusicPlayer.Play(true); 
																				});
				//MusicPlayer.QueuedMusic = m;
				
			}
			else
			{
				MusicPlayer.CurrentMusic = m;
				MusicPlayer.Play(true);
			}
		}
		
	}
}

