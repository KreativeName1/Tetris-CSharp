using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Tetris
{
	public class Block
	{
		public static List<Block> Blöcke = InitializeBlocks();

		public Position Position { get; set; }
		public ConsoleColor Color { get; set; }
		public int[][] Pattern { get; set; }
		public bool IsLanded { get; set; }
		public bool canMoveRight { get; set; } = true;
		public bool CanMoveLeft { get; set; } = true;
		public bool canMoveLeftCorner { get; set; } = true;
		public bool canMoveRightCorner { get; set; } = true;


		public Block(int x, int y, ConsoleColor color, int[][] pattern)
		{
			Position = new Position(x, y);
			Color = color;
			Pattern = pattern;
			IsLanded = false;
		}

		public Direction Move(int blockHeight)
		{
			Direction blockDirection = Direction.Nothing;
			if (Console.KeyAvailable)
			{
				ConsoleKeyInfo key = Console.ReadKey(true);

				if (key.Key == ConsoleKey.LeftArrow)
				{
					if (CanMoveLeft) Position.X--;
					blockDirection = Direction.Left;
				}
				else if (key.Key == ConsoleKey.RightArrow)
				{
					if (canMoveRight) Position.X++;
					blockDirection = Direction.Right;
				}
				else if (key.Key == ConsoleKey.DownArrow) Rotate(true, blockHeight);
				else if (key.Key == ConsoleKey.UpArrow) Rotate(false, blockHeight);
			}
			else
			{
				if (IsKeyDown(ConsoleKey.LeftArrow))
				{
					if (CanMoveLeft) Position.X--;
					blockDirection = Direction.Left;
				}
				else if (IsKeyDown(ConsoleKey.RightArrow))
				{
					if (canMoveRight) Position.X++;
					blockDirection = Direction.Right;
				}
			}
			while (Console.KeyAvailable) Console.ReadKey(true);
			return blockDirection;
		}

		public void Render()
		{
			Console.SetCursorPosition(Position.X, Position.Y);
			Console.ForegroundColor = Color;

			for (int LayoutY = 0; LayoutY < Pattern.Length; LayoutY++)
			{
				for (int LayoutX = 0; LayoutX < Pattern[LayoutY].Length; LayoutX++)
				{
					Console.SetCursorPosition(Position.X + LayoutX, Position.Y + LayoutY);
					if (Pattern[LayoutY][LayoutX] == 1) Console.Write("█");
				}
			}

			Console.ForegroundColor = ConsoleColor.White;
		}

		public void Erase()
		{
			Console.SetCursorPosition(Position.X, Position.Y);

			for (int LayoutY = 0; LayoutY < Pattern.Length; LayoutY++)
			{
				for (int LayoutX = 0; LayoutX < Pattern[LayoutY].Length; LayoutX++)
				{
					Console.SetCursorPosition(Position.X + LayoutX, Position.Y + LayoutY);
					if (Pattern[LayoutY][LayoutX] == 1) Console.Write(" ");
				}
			}
		}

		public void Rotate(bool isRotatingClockwise, int höhe)
		{
			Erase();
			int rowCount = Pattern.Length;
			int columnCount = Pattern[0].Length;
      int[][] oldPattern = Pattern.Clone() as int[][] ?? throw new Exception("Pattern is missing! Please make sure the block has a pattern inside the json file. If Missing, you can find it in the GitHub Repository.");

			int[][] rotatedPattern = new int[columnCount][];

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				rotatedPattern[columnIndex] = new int[rowCount];

				for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
				{
					if (isRotatingClockwise) rotatedPattern[columnIndex][rowIndex] = oldPattern[rowCount - rowIndex - 1][columnIndex];
					else rotatedPattern[columnIndex][rowIndex] = oldPattern[rowIndex][columnCount - columnIndex - 1];
				}
			}

			Pattern = rotatedPattern;
			//if (Spiel.KollisionMitAnderenBlöcken(this)) Layout = alt;
			//else if (Position.Y + Layout.Length > höhe) Layout = alt;
			//else Abspielen.Sound(Sound.Drehen);
		}

		public bool CompactPattern()
		{
			int emptyLinesCount = 0;

			for (int i = 0; i < Pattern.Length; i++)
			{
				if (Pattern[i].Sum() == 0) emptyLinesCount++;
			}
			if (emptyLinesCount == Pattern.Length) return true;

			int[][] compactPatterin = new int[Pattern.Length - emptyLinesCount][];

			int index = 0;
			for (int i = 0; i < Pattern.Length; i++)
			{
				if (Pattern[i].Sum() != 0)
				{
					compactPatterin[index] = Pattern[i];
					index++;
				}
			}
			Pattern = compactPatterin;

			Position.Y += emptyLinesCount;
			return false;
		}

		public static List<Block> InitializeBlocks()
		{
			List<Block>? blockList;
      if (!File.Exists("blocks.json")) throw new Exception("blocks.json not found! Please make sure the file is in the same directory as the executable. If Missing, you can find it in the GitHub Repository.");
			blockList = JsonConvert.DeserializeObject<List<Block>>(File.ReadAllText("blocks.json"), new JsonSerializerSettings
			{
				Converters = { new ConsoleColorConverter() }
			});
      if (blockList == null) throw new Exception("No blocks were found in blocks.json. Please make sure the file is not empty. If Empty, you can find the blocks in the GitHub Repository.");

			foreach (Block block in blockList) block.Position = new Position(0, 0);
			return blockList;
		}

    //TODO Replace with Version that has Linux and Mac support
		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(ConsoleKey vKey);

		private bool IsKeyDown(ConsoleKey key)
		{
			return (GetAsyncKeyState(key) & 0x8000) != 0;
		}

		public override string ToString()
		{
			string text = "";
			// Show layout
			for (int y = 0; y < Pattern.Length; y++)
			{
				for (int x = 0; x < Pattern[y].Length; x++)
				{
					text += Pattern[y][x];
				}
				text += "\n";
			}
			return text;
		}

		public override bool Equals(object? obj)
		{
			return obj is Block && obj as Block == this;
		}

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
