using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Tetris
{
	public class Block
	{
		public static List<Block> Blöcke = Laden();

		public Position Position { get; set; }
		public ConsoleColor Farbe { get; set; }
		public int[][] Layout { get; set; }
		public bool Gelandet { get; set; }
		public bool RechtsBewegen { get; set; } = true;
		public bool LinksBewegen { get; set; } = true;
		public bool LinkeEckBewegung { get; set; } = true;
		public bool RechteEckBewegung { get; set; } = true;

		public Block(int x, int y, ConsoleColor farbe, int[][] layout)
		{
			Position = new Position(x, y);
			Farbe = farbe;
			Layout = layout;
			Gelandet = false;
		}

		public Bewegung Bewegen(int höhe)
		{
			Bewegung bew = Bewegung.Nicht;
			if (Console.KeyAvailable)
			{
				ConsoleKeyInfo key = Console.ReadKey(true);

				if (key.Key == ConsoleKey.LeftArrow)
				{
					if (LinksBewegen) Position.X--;
					bew = Bewegung.Links;
				}
				else if (key.Key == ConsoleKey.RightArrow)
				{
					if (RechtsBewegen) Position.X++;
					bew = Bewegung.Rechts;
				}
				else if (key.Key == ConsoleKey.DownArrow) Drehen(true, höhe);
				else if (key.Key == ConsoleKey.UpArrow) Drehen(false, höhe);
			}
			else
			{
				if (IsKeyDown(ConsoleKey.LeftArrow))
				{
					if (LinksBewegen) Position.X--;
					bew = Bewegung.Links;
				}
				else if (IsKeyDown(ConsoleKey.RightArrow))
				{
					if (RechtsBewegen) Position.X++;
					bew = Bewegung.Rechts;
				}
			}
			while (Console.KeyAvailable) Console.ReadKey(true);
			return bew;
		}

		public void Zeichnen()
		{
			// Zur neuen Position gehen und die Farbe setzen
			Console.SetCursorPosition(Position.X, Position.Y);
			Console.ForegroundColor = Farbe;

			// Loop durch das Layout und an der Position ein Zeichen schreiben, wenn der Wert 1 ist
			for (int LayoutY = 0; LayoutY < Layout.Length; LayoutY++)
			{
				for (int LayoutX = 0; LayoutX < Layout[LayoutY].Length; LayoutX++)
				{
					Console.SetCursorPosition(Position.X + LayoutX, Position.Y + LayoutY);
					if (Layout[LayoutY][LayoutX] == 1) Console.Write("█");
				}
			}

			Console.ForegroundColor = ConsoleColor.White;
		}

		public void Löschen()
		{
			// Position des Cursors auf die Position des Blocks setzen
			Console.SetCursorPosition(Position.X, Position.Y);

			// Loop durch die Zeilen
			for (int LayoutY = 0; LayoutY < Layout.Length; LayoutY++)
			{
				for (int LayoutX = 0; LayoutX < Layout[LayoutY].Length; LayoutX++)
				{
					// Zur Position plus Offset gehen und ein Leerzeichen schreiben, wenn der Wert 1 ist
					Console.SetCursorPosition(Position.X + LayoutX, Position.Y + LayoutY);
					if (Layout[LayoutY][LayoutX] == 1) Console.Write(" ");
				}
			}
		}

		public void Drehen(bool uhrzeigersinn, int höhe)
		{
			// Altes Löschen
			Löschen();
			int zeilen = Layout.Length;
			int spalten = Layout[0].Length;
			int[][] alt = Layout.Clone() as int[][];

			int[][] gedreht = new int[spalten][];

			// Loop durch die Spalten
			for (int spalte = 0; spalte < spalten; spalte++)
			{
				// Neue Zeile erstellen
				gedreht[spalte] = new int[zeilen];

				// Loop durch die Zeilen
				for (int zeile = 0; zeile < zeilen; zeile++)
				{
					if (uhrzeigersinn) gedreht[spalte][zeile] = alt[zeilen - zeile - 1][spalte];
					else gedreht[spalte][zeile] = alt[zeile][spalten - spalte - 1];
				}
			}

			Layout = gedreht;
			//if (Spiel.KollisionMitAnderenBlöcken(this)) Layout = alt;
			//else if (Position.Y + Layout.Length > höhe) Layout = alt;
			//else Abspielen.Sound(Sound.Drehen);
		}

		public bool Kompaktieren()
		{
			int leereZeilen = 0;
			// Loop durch die Zeilen
			for (int i = 0; i < Layout.Length; i++)
			{
				if (Layout[i].Sum() == 0) leereZeilen++;
			}
			// prüfen, ob alle weg sind
			if (leereZeilen == Layout.Length) return true;

			int[][] kompakt = new int[Layout.Length - leereZeilen][];

			int index = 0;
			// Loop durch die Zeilen
			for (int i = 0; i < Layout.Length; i++)
			{
				if (Layout[i].Sum() != 0)
				{
					kompakt[index] = Layout[i];
					index++;
				}
			}

			Layout = kompakt;

			Position.Y += leereZeilen;
			return false;
		}

		public static List<Block> Laden()
		{
			List<Block> liste = new();
			liste = JsonConvert.DeserializeObject<List<Block>>(File.ReadAllText("Blöcke.json"), new JsonSerializerSettings
			{
				Converters = { new ConsoleColorConverter() }
			});

			foreach (Block block in liste) block.Position = new Position(0, 0);
			return liste;
		}

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
			for (int y = 0; y < Layout.Length; y++)
			{
				for (int x = 0; x < Layout[y].Length; x++)
				{
					text += Layout[y][x];
				}
				text += "\n";
			}
			return text;
		}

		// equals
		public override bool Equals(object? obj)
		{
			return obj is Block && obj as Block == this;
		}
	}
}
