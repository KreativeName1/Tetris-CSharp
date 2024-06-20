using System;
using NAudio.Wave;
using Tetris.SoundPlayback;
using System.Runtime.InteropServices;

namespace Tetris
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      try {
      Block.InitializeBlocks();
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
      IPlatformAudioFactory factory;
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        factory = new WindowsPlayerFactory();
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
          factory = new LinuxPlayerFactory();
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      {
          factory = new MacPlayerFactory();
      }
      else
      {
          throw new PlatformNotSupportedException();
      }
      Menu menu = new(factory);
      menu.StartScreen();
    }
  }
}