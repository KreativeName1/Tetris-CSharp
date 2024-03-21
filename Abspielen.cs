using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
	public enum Sound
	{
		[Description("Sound/gameover.mp3")]
		Gameover,

		[Description("Sound/block_land.wav")]
		Landen,

		[Description("Sound/line_destroy.wav")]
		Linie,

		[Description("Sound/block_rotate.wav")]
		Drehen,

		[Description("Sound/levelup.wav")]
		Levelup,
	}

	public enum Musik
	{
		[Description("Musik/theme_a.mp3")]
		ThemeA,

		[Description("Musik/theme_b.mp3")]
		ThemeB,

		[Description("Musik/theme_c.mp3")]
		ThemeC,

		[Description("Musik/1 - Music 1.mp3")]
		ThemeD,

		[Description("Musik/2 - Music 2.mp3")]
		ThemeE,

		[Description("Musik/theme_f.mp3")]
		ThemeF,

		[Description("Musik/3 - Music 3.mp3")]
		ThemeG,

		[Description("Musik/title.mp3")]
		Title
	}

	public class Abspielen
	{
		public static bool MusikAn = true;

		public static async void Sound(Sound sound)
		{
			using (var audioFile = new AudioFileReader(GetEnumDescription(sound)))
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

		public static async void Musik(Musik musik)
		{
			while (MusikAn)
			{
				using (var audioFile = new AudioFileReader(GetEnumDescription(musik)))
				using (var outputDevice = new WaveOutEvent())
				{
					outputDevice.Init(audioFile);
					outputDevice.Play();
					outputDevice.Volume = 0.6f;
					while (outputDevice.PlaybackState == PlaybackState.Playing)
					{
						if (!MusikAn) break;
						await Task.Delay(100);
					}
				}
			}
		}

		// Methode, die die Beschreibung eines Enum-Elements zurückgibt
		private static string GetEnumDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : value.ToString();
		}
	}
}