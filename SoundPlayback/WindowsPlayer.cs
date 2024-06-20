using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.SoundPlayback
{

	public class WindowsPlayer : IAudioPlayer
	{
		public bool MusicOn = true;

		public async void PlaySound(Sound sound)
		{
			using (var audioFile = new AudioFileReader(AudioFiles.GetEnumDescription(sound)))
			using (var outputDevice = new WaveOutEvent())
			{
				outputDevice.Init(audioFile);
				outputDevice.Play();
				while (outputDevice.PlaybackState == PlaybackState.Playing)
				{
					await Task.Delay(100);
				}
			}
		}
    public void TurnOffMusic() => MusicOn = false;
    public void TurnOnMusic() => MusicOn = true;

		public async void PlayMusic(Music music)
		{
			while (MusicOn)
			{
				using (var audioFile = new AudioFileReader(AudioFiles.GetEnumDescription(music)))
				using (var outputDevice = new WaveOutEvent())
				{
					outputDevice.Init(audioFile);
					outputDevice.Play();
					outputDevice.Volume = 0.6f;
					while (outputDevice.PlaybackState == PlaybackState.Playing)
					{
						if (!MusicOn) break;
						await Task.Delay(100);
					}
				}
			}
		}
	}
}