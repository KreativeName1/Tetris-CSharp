using Tetris.SoundPlayback;

public class Player
{
  private readonly IAudioPlayer _player;

  public Player(IPlatformAudioFactory factory)
  {
    _player = factory.CreatePlayer();
  }

  public async Task PlaySoundAsync(Sound sound)
  {
    _player.PlaySound(sound);
  }

  public async Task PlayMusicAsync(Music music)
  {
      _player.PlayMusic(music);
  }

  public void TurnOffMusic()
  {
    _player.TurnOffMusic();
  }
  public void TurnOnMusic()
  {
    _player.TurnOnMusic();
  }
}