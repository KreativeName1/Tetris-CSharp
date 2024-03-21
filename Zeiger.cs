namespace Tetris
{
	public class Zeiger
	{
		public Position pos;
		public bool Enabled;
		private int[] breite;
		public int index;

		public Zeiger(Position pos, bool enabled, int[] breite)
		{
			this.pos = pos;
			this.Enabled = enabled;
			this.breite = breite;
			index = breite[0];
		}

		public void Zeichnen()
		{
			if (Enabled) Console.ForegroundColor = ConsoleColor.Red;
			else Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition(pos.X, pos.Y);
			Console.Write("^");
		}

		public void Löschen()
		{
			Console.SetCursorPosition(pos.X, pos.Y);
			Console.Write(" ");
		}

		public void Bewegen(ConsoleKeyInfo key)
		{
			switch (key.Key)
			{
				case ConsoleKey.LeftArrow:
					Löschen();
					if (index != breite[0])
					{
						pos.X -= 2;
						index--;
					}
					break;

				case ConsoleKey.RightArrow:
					Löschen();
					if (index != breite[breite.Length - 1])
					{
						pos.X += 2;
						index++;
					}
					break;

				case ConsoleKey.Enter:
					Enabled = false;
					break;
			}
		}
	}
}