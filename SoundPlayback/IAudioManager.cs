using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tetris.SoundPlayback
{
    public interface IAudioPlayer
    {
        void PlaySound(Sound sound);
        void PlayMusic(Music music);
        void TurnOffMusic();
        void TurnOnMusic();
    }
}