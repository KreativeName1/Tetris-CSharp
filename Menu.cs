namespace Tetris
{
	public class Menu
	{
		private Spiel spiel;
		private int breite = 10;
		private int höhe = 20;
		private int level;
		private Musik musik;

		public void StartBild()
		{
			Console.Clear();
			Console.CursorVisible = false;
			Abspielen.MusikAn = true;
			Abspielen.Musik(Musik.Title);
			ZeichnenFarbig(ConsoleColor.Magenta, "█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█");
			ZeichnenFarbig(ConsoleColor.Magenta, "█     ", false);
			ZeichnenFarbig(ConsoleColor.Yellow, "TETRIS", false);
			ZeichnenFarbig(ConsoleColor.Magenta, "     █");
			ZeichnenFarbig(ConsoleColor.Magenta, "█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█");
			while (!Console.KeyAvailable)
			{
				Console.SetCursorPosition(2, 5);
				ZeichnenFarbig(ConsoleColor.Yellow, "<Space> drücken");
				Thread.Sleep(500);
				Console.SetCursorPosition(2, 5);
				Console.WriteLine("               ");
				Thread.Sleep(500);
			}
			Auswahl();
		}

		public void Auswahl()
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
			ConsoleColor farbe = ConsoleColor.Magenta;
			ZeichenAnPos(13, 2, "█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█", farbe);
			ZeichenAnPos(13, 3, "█               █", farbe);
			ZeichenAnPos(13, 4, "█               █", farbe);
			ZeichenAnPos(13, 5, "█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█", farbe);

			ZeichenAnPos(18, 1, "Musik", farbe);
			ZeichenAnPos(15, 3, "A B C D E F G");

			ZeichenAnPos(2, 9, "█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█", farbe);
			ZeichenAnPos(2, 10, "█                                     █", farbe);
			ZeichenAnPos(2, 11, "█                                     █", farbe);
			ZeichenAnPos(2, 12, "█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█", farbe);

			ZeichenAnPos(20, 8, "Level", farbe);
			ZeichenAnPos(4, 10, "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15");

			MusikAuswahl();
			LevelAuswahl();
			Spielen();
		}

		private void MusikAuswahl()
		{
			Zeiger zMusik = new(new Position(15, 4), true, new int[] { 1, 2, 3, 4, 5, 6, 7 });
			zMusik.Zeichnen();

			while (zMusik.Enabled)
			{
				zMusik.Bewegen(Console.ReadKey(true));
				zMusik.Zeichnen();
				musik = (Musik)zMusik.index - 1;
				Abspielen.MusikAn = false;
				Thread.Sleep(100);
				Abspielen.MusikAn = true;
				Abspielen.Musik(musik);
			}
		}

		private void LevelAuswahl()
		{
			int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			Zeiger zLevel = new(new Position(4, 11), true, array);
			zLevel.Zeichnen();
			while (zLevel.Enabled)
			{
				zLevel.Bewegen(Console.ReadKey(true));
				zLevel.Zeichnen();
			}
			level = array[zLevel.index - 1];
		}

		private void Spielen()
		{
			Console.Clear();
			Abspielen.MusikAn = false;
			Thread.Sleep(500);
			spiel = new Spiel(breite, höhe, 170, level, musik);
			StartBild();
		}

		private void ZeichenAnPos(int x, int y, string text, ConsoleColor farbe = ConsoleColor.Yellow)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = farbe;
			Console.Write(text);
			Console.ForegroundColor = ConsoleColor.White;
		}

		private void ZeichnenFarbig(ConsoleColor farbe, string text, bool newLine = true)
		{
			Console.ForegroundColor = farbe;
			if (newLine) Console.WriteLine(text);
			else Console.Write(text);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}