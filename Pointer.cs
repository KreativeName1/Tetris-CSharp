namespace Tetris
{
	public class Pointer
	{
		public Position pos;
		public bool Enabled;
		private int[] widthArray;
		public int index;

		public Pointer(Position pos, bool enabled, int[] widthArray)
		{
			this.pos = pos;
			this.Enabled = enabled;
			this.widthArray = widthArray;
			index = widthArray[0];
		}

		public void Render()
		{
			if (Enabled) Console.ForegroundColor = ConsoleColor.Red;
			else Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition(pos.X, pos.Y);
			Console.Write("^");
		}

		public void Erase()
		{
			Console.SetCursorPosition(pos.X, pos.Y);
			Console.Write(" ");
		}

		public void Move(ConsoleKeyInfo key)
		{
			switch (key.Key)
			{
				case ConsoleKey.LeftArrow:
					Erase();
					if (index != widthArray[0])
					{
						pos.X -= 2;
						index--;
					}
					break;

				case ConsoleKey.RightArrow:
					Erase();
					if (index != widthArray[widthArray.Length - 1])
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