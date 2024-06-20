using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tetris.SoundPlayback
{
  public class LinuxPlayer : IAudioPlayer
  {
    public bool MusicOn = true;

    public void PlaySound(Sound sound)
    {
      var soundFile = AudioFiles.GetEnumDescription(sound);
      var command = $"aplay -q {soundFile}";
      var process = System.Diagnostics.Process.Start("bash", $"-c \"{command}\"");
      process?.WaitForExit();
    }

    public void TurnOffMusic() => MusicOn = false;

    public void TurnOnMusic() => MusicOn = true;

    public async void PlayMusic(Music music)
    {
      var musicFile = AudioFiles.GetEnumDescription(music);
      var command = $"aplay -q {musicFile}";
      while (MusicOn)
      {
        var process = System.Diagnostics.Process.Start("bash", $"-c \"{command}\"");
        process?.WaitForExit();
        await Task.Delay(100);
      }

    }
  }
}
