using Tetris.SoundPlayback;

namespace Tetris
{
	public class Menu
	{
    private readonly IAudioPlayer _player;

		private Game gameInstance;
		private int width = 10;
		private int height = 20;
		private int level;
		private Music menuMusic;

    public Menu(IPlatformAudioFactory factory)
    {
      _player = factory.CreatePlayer();
    }

		public void StartScreen()
		{
			Console.Clear();
			Console.CursorVisible = false;
			_player.TurnOnMusic();
			_player.PlayMusic(Music.Title);
			ZeichnenFarbig(ConsoleColor.Magenta, "█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█");
			ZeichnenFarbig(ConsoleColor.Magenta, "█     ", false);
			ZeichnenFarbig(ConsoleColor.Yellow, "TETRIS", false);
			ZeichnenFarbig(ConsoleColor.Magenta, "     █");
			ZeichnenFarbig(ConsoleColor.Magenta, "█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█");
			while (!Console.KeyAvailable)
			{
				Console.SetCursorPosition(2, 5);
				ZeichnenFarbig(ConsoleColor.Yellow, "Press <Space>");
				Thread.Sleep(500);
				Console.SetCursorPosition(2, 5);
				Console.WriteLine("               ");
				Thread.Sleep(500);
			}
			Selection();
		}

		public void Selection()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Yellow;
			for (int y = 0; y < 20; y++)
			{
				for (int x = 0; x < 45; x++)
				{
					Console.SetCursorPosition(x, y);
					Console.Write("▒");
				}
			}
			ConsoleColor color = ConsoleColor.Magenta;
			DrawTextAtPosition(13, 2, "█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█", color);
			DrawTextAtPosition(13, 3, "█               █", color);
			DrawTextAtPosition(13, 4, "█               █", color);
			DrawTextAtPosition(13, 5, "█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█", color);

			DrawTextAtPosition(18, 1, "Musik", color);
			DrawTextAtPosition(15, 3, "A B C D E F G");

			DrawTextAtPosition(2, 9, "█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█", color);
			DrawTextAtPosition(2, 10, "█                                     █", color);
			DrawTextAtPosition(2, 11, "█                                     █", color);
			DrawTextAtPosition(2, 12, "█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█", color);

			DrawTextAtPosition(20, 8, "Level", color);
			DrawTextAtPosition(4, 10, "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15");

			MusicSelection();
			LevelSelection();
			StartGame();
		}

		private void MusicSelection()
		{
			Pointer MusicSelector = new(new Position(15, 4), true, new int[] { 1, 2, 3, 4, 5, 6, 7 });
			MusicSelector.Render();

			while (MusicSelector.Enabled)
			{
				MusicSelector.Move(Console.ReadKey(true));
				MusicSelector.Render();
				menuMusic = (Music)MusicSelector.index - 1;
				_player.TurnOffMusic();
				Thread.Sleep(100);
        _player.TurnOnMusic();
        _player.PlayMusic(menuMusic);
			}
		}

		private void LevelSelection()
		{
			int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			Pointer zLevel = new(new Position(4, 11), true, array);
			zLevel.Render();
			while (zLevel.Enabled)
			{
				zLevel.Move(Console.ReadKey(true));
				zLevel.Render();
			}
			level = array[zLevel.index - 1];
		}

		private void StartGame()
		{
			Console.Clear();
			_player.TurnOffMusic();
			Thread.Sleep(500);
			gameInstance = new Game(width, height, 170, level, menuMusic, _player);
			StartScreen();
		}

		private void DrawTextAtPosition(int x, int y, string text, ConsoleColor color = ConsoleColor.Yellow)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = color;
			Console.Write(text);
			Console.ForegroundColor = ConsoleColor.White;
		}

		private void ZeichnenFarbig(ConsoleColor color, string text, bool newLine = true)
		{
			Console.ForegroundColor = color;
			if (newLine) Console.WriteLine(text);
			else Console.Write(text);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}