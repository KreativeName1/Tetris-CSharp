

using Tetris.SoundPlayback;

public class WindowsPlayerFactory : IPlatformAudioFactory
{
  public IAudioPlayer CreatePlayer()
  {
    return new WindowsPlayer();
  }
}

public class LinuxPlayerFactory : IPlatformAudioFactory
{
  public IAudioPlayer CreatePlayer()
  {
    return new LinuxPlayer(); // Replace with your Linux audio player using PortAudio
  }
}
public class MacPlayerFactory : IPlatformAudioFactory
{
  public IAudioPlayer CreatePlayer()
   {
    return new MacPlayer(); // Replace with your MacOS audio player using AVAudioEngine
  }
}