using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Tetris
{
	public class Spiel
	{
		private static DebugList<Block> GelandeteBlöcke = new DebugList<Block>();

		private int breite;
		private int höhe;
		private int tickrate;
		private Block aktuellBlock;
		private Block vorschauBlock;
		private Random zufall;
		private Musik musik;
		private int punkte = 0;
		private int level;
		private int[][] feld;

		public Spiel(int breite, int höhe, int tickrate, int level, Musik musik)
		{
			GelandeteBlöcke.Clear();

			this.breite = breite;
			this.höhe = höhe;
			this.tickrate = tickrate;
			this.level = level;
			this.musik = musik;
			zufall = new();

			//// Feld initialisieren
			feld = Enumerable.Range(0, this.höhe + 1).Select(x => new int[breite]).ToArray();

			// Musik abspielen
			Abspielen.MusikAn = true;
			Abspielen.Musik(this.musik);

			// Tickrate an das Startlevel anpassen
			for (int i = 1; i < level; i++)
			{
				this.tickrate = Math.Max(100, this.tickrate - (i * 3));
			}

			FeldZeichnen();
			UpdateScore();

			// Ersten Block und Vorschau Block generieren
			aktuellBlock = BlockGenerieren();
			vorschauBlock = BlockGenerieren();
			vorschauBlock.Position = new(breite + 3, 1);
			vorschauBlock.Zeichnen();

			Update();
		}

		private void UpdateScore()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition(breite + 3, 5);
			Console.Write($"Z {punkte}");
			Console.SetCursorPosition(breite + 3, 8);
			Console.Write($"L {level}");
		}

		private void LevelUp()
		{
			Abspielen.Sound(Sound.Levelup);
			level++;
			if (punkte % 10 == 0) level++;
			tickrate = Math.Max(100, tickrate - (level * 3));
			punkte = 0;
		}

		private bool GameOverPrüfen()
		{
			if (aktuellBlock.Position.Y < 2)
			{
				Abspielen.MusikAn = false;
				Abspielen.Sound(Sound.Gameover);
				Animation();
				Console.ReadKey();
				return true;
			}
			return false;
		}

		private void Update()
		{
			while (true)
			{
				GelandeteBlöcke = new DebugList<Block>(GelandeteBlöcke
		.Where(obj => !obj.Layout.All(row => row.All(element => element == 0)))
		.ToList());
				LandungPrüfen();

				if (aktuellBlock.Gelandet == false)
				{
					aktuellBlock.Löschen();
					Bewegung bew = aktuellBlock.Bewegen(höhe);

					// Ob der Block sich bewegen darf wird in der Kollision mit anderen Blöcken geprüft
					if (aktuellBlock.LinkeEckBewegung && bew == Bewegung.Links) aktuellBlock.Position.Y++;
					else if (aktuellBlock.RechteEckBewegung && bew == Bewegung.Rechts) aktuellBlock.Position.Y++;
					else if (bew == Bewegung.Nicht) aktuellBlock.Position.Y++;
				}
				aktuellBlock.LinksBewegen = true;
				aktuellBlock.RechtsBewegen = true;
				aktuellBlock.LinkeEckBewegung = true;
				aktuellBlock.RechteEckBewegung = true;

				SeitenKollisionPrüfen();
				aktuellBlock.Zeichnen();

				// Falls der Block gelandet ist, wird er in die Liste der gelandeten Blöcke hinzugefügt und ein neuer Block wird generiert
				if (aktuellBlock.Gelandet)
				{
					Abspielen.Sound(Sound.Landen);
					for (int LayoutY = 0; LayoutY < aktuellBlock.Layout.Length; LayoutY++)
					{
						for (int LayoutX = 0; LayoutX < aktuellBlock.Layout[LayoutY].Length; LayoutX++)
						{
							if (aktuellBlock.Layout[LayoutY][LayoutX] == 1)
							{
								feld[aktuellBlock.Position.Y + LayoutY][aktuellBlock.Position.X + LayoutX - 1] = 1;
							}
						}
					}
					if (GameOverPrüfen()) break;
					GelandeteBlöcke.Add(aktuellBlock);
					vorschauBlock.Löschen();
					aktuellBlock = new Block(breite / 2, 0, vorschauBlock.Farbe, vorschauBlock.Layout.Select(x => x.ToArray()).ToArray());
					vorschauBlock = BlockGenerieren();
					vorschauBlock.Position = new(breite + 3, 1);
					vorschauBlock.Zeichnen();
				}

				ZeilenPrüfenUndLöschen();
				Debug();
				Thread.Sleep(tickrate);
			}
		}

		private async void Animation()
		{
			for (int y = höhe; y >= 0; y--)
			{
				Console.ForegroundColor = ConsoleColor.Black;
				Thread.Sleep(70);
				for (int x = 0; x < breite; x++)
				{
					Console.SetCursorPosition(x + 1, y);
					Console.Write("█");
				}
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(1, höhe / 2);
				Console.WriteLine("Game Over!");
			}
		}

		private void FeldZeichnen()
		{
			Console.ForegroundColor = ConsoleColor.Black;

			DrawHorizontalLine(0, höhe + 1, breite + 2, "▀");

			DrawVerticalLine(0, höhe + 1, "█");
			DrawVerticalLine(breite + 1, höhe + 1, "█");

			DrawHorizontalLine(2 + breite, 4, 7, "▀");
			DrawHorizontalLine(2 + breite, 7, 7, "▀");
			DrawHorizontalLine(2 + breite, 10, 7, "▀");

			DrawVerticalLine(breite + 8, 10, "█");
		}

		private void Debug()
		{
			// zeichne das Feld
			for (int y = 0; y < feld.Length; y++)
			{
				for (int x = 0; x < feld[y].Length; x++)
				{
					Console.SetCursorPosition(breite + 10 + x, y + 1);
					Console.Write(feld[y][x]);
				}
			}
		}

		private void DrawHorizontalLine(int startX, int startY, int length, string symbol)
		{
			for (int i = 0; i < length; i++)
			{
				Console.SetCursorPosition(startX + i, startY);
				Console.Write(symbol);
			}
		}

		private void DrawVerticalLine(int x, int endY, string symbol)
		{
			for (int i = 0; i < endY; i++)
			{
				Console.SetCursorPosition(x, i);
				Console.Write(symbol);
			}
		}

		private Block BlockGenerieren()
		{
			Block block = Block.Blöcke[zufall.Next(0, Block.Blöcke.Count)];
			return new Block(breite / 2, 0, block.Farbe, block.Layout.Select(x => x.ToArray()).ToArray());
		}

		private void SeitenKollisionPrüfen()
		{
			int[] breitesteZeile = aktuellBlock.Layout.OrderByDescending(x => x.Length).First();
			int newX = Math.Max(1, Math.Min(breite - breitesteZeile.Length + 1, aktuellBlock.Position.X));

			if (aktuellBlock.Position.X + breitesteZeile.Length - 1 >= breite + 1 || aktuellBlock.Position.X < 1)
			{
				aktuellBlock.Löschen();
				aktuellBlock.Position.X = newX;
				FeldZeichnen();
			}
		}

		// Hier wird die Kollision mit anderen Blöcken geprüft
		private void LandungPrüfen()
		{
			if (aktuellBlock.Position.Y + aktuellBlock.Layout.Count() >= höhe + 1) aktuellBlock.Gelandet = true;
			if (KollisionMitAnderenBlöcken(aktuellBlock)) aktuellBlock.Gelandet = true;
		}

		private void ZeilenPrüfenUndLöschen()
		{
			for (int zeile = 0; zeile < feld.Length; zeile++)
			{
				// .All(zeile => zeile == 1) -> Prüft ob alle Werte in der Zeile 1 sind
				if (feld[zeile].All(zeile => zeile == 1))
				{
					foreach (Block gelandeterBlock in GelandeteBlöcke)
					{
						for (int LayoutY = 0; LayoutY < gelandeterBlock.Layout.Length; LayoutY++)
						{
							for (int LayoutX = 0; LayoutX < gelandeterBlock.Layout[LayoutY].Length; LayoutX++)
							{
								if (gelandeterBlock.Layout[LayoutY][LayoutX] == 1 && gelandeterBlock.Position.Y + LayoutY == zeile)
								{
									gelandeterBlock.Layout[LayoutY][LayoutX] = 0;
									feld[zeile][gelandeterBlock.Position.X + LayoutX - 1] = 0;
									Console.SetCursorPosition(gelandeterBlock.Position.X + LayoutX, gelandeterBlock.Position.Y + LayoutY);
									Console.Write(" ");
								}
							}
						}
					}
					Abspielen.Sound(Sound.Linie);
					ZeileRunterBewegen(zeile);
					punkte++;
					if (punkte % 10 == 0) LevelUp();
					UpdateScore();
				}
			}
			Trace.WriteLine("\n");
		}

		public void ZeileRunterBewegen(int zeile)
		{
			for (int index = 0; index < GelandeteBlöcke.Count; index++)
			{
				if (GelandeteBlöcke[index].Layout.All(row => row.All(element => element == 0)))
				{
					GelandeteBlöcke[index].Löschen();
					GelandeteBlöcke.Remove(GelandeteBlöcke[index]);
				}
				else if (GelandeteBlöcke[index].Position.Y + GelandeteBlöcke[index].Layout.Length - 1 <= zeile)
				{
					// Alte Position aus Feld Löschen
					SetBlockPositionInFeld(GelandeteBlöcke[index], 0);

					// Block runter bewegen
					GelandeteBlöcke[index].Löschen();
					GelandeteBlöcke[index].Position.Y++;
					GelandeteBlöcke[index].Zeichnen();

					// Neue Position in das Feld setzten
					SetBlockPositionInFeld(GelandeteBlöcke[index], 1);
				}
				// Wenn vom block nur noch ein teil über der zeile ist,
				// Kompaktieren() aufrufen
				else if (GelandeteBlöcke[index].Position.Y < zeile && GelandeteBlöcke[index].Position.Y + GelandeteBlöcke[index].Layout.Length - 1 > zeile)
				{
					SetBlockPositionInFeld(GelandeteBlöcke[index], 0);
					GelandeteBlöcke[index].Löschen();
					bool ganzLeer = GelandeteBlöcke[index].Kompaktieren();
					if (!ganzLeer)
					{
						GelandeteBlöcke[index].Zeichnen();
						// Hier kann es Abstürzen
						SetBlockPositionInFeld(GelandeteBlöcke[index], 1);
						Trace.WriteLine("Block kompaktiert: " + index);
					}
					else GelandeteBlöcke.Remove(GelandeteBlöcke[index]);
				}
			}
		}

		public static bool KollisionMitAnderenBlöcken(Block block)
		{
			bool kollision = false;
			foreach (var gelandeterBlock in GelandeteBlöcke)
			{
				// Durch das Layout des aktuellen Blocks loopen
				for (int LayoutY = 0; LayoutY < block.Layout.Length; LayoutY++)
				{
					for (int LayoutX = 0; LayoutX < block.Layout[LayoutY].Length; LayoutX++)
					{
						if (block.Layout[LayoutY][LayoutX] == 1)
						{
							Position PosUnterBlock = new(block.Position.X + LayoutX, block.Position.Y + LayoutY + 1);
							Position PoSRechtsBlock = new(block.Position.X + LayoutX + 1, block.Position.Y + LayoutY);
							Position PoSLinksBlock = new(block.Position.X + LayoutX - 1, block.Position.Y + LayoutY);
							Position PosUntenLinksBlock = new(block.Position.X + LayoutX - 1, block.Position.Y + LayoutY + 1);
							Position PosUntenRechtsBlock = new(block.Position.X + LayoutX + 1, block.Position.Y + LayoutY + 1);
							// Durch das Layout des gelandeten Blocks loopen
							for (int GelandetLayoutY = 0; GelandetLayoutY < gelandeterBlock.Layout.Length; GelandetLayoutY++)
							{
								for (int GelandetLayoutX = 0; GelandetLayoutX < gelandeterBlock.Layout[GelandetLayoutY].Length; GelandetLayoutX++)
								{
									if (gelandeterBlock.Layout[GelandetLayoutY][GelandetLayoutX] == 1)
									{
										Position PosGelandeterBlock = new(gelandeterBlock.Position.X + GelandetLayoutX, gelandeterBlock.Position.Y + GelandetLayoutY);
										if (PosUnterBlock.Equals(PosGelandeterBlock)) kollision = true;
										if (PoSRechtsBlock.Equals(PosGelandeterBlock)) block.RechtsBewegen = false;
										if (PoSLinksBlock.Equals(PosGelandeterBlock)) block.LinksBewegen = false;
										if (PosUntenLinksBlock.Equals(PosGelandeterBlock)) block.LinkeEckBewegung = false;
										if (PosUntenRechtsBlock.Equals(PosGelandeterBlock)) block.RechteEckBewegung = false;
									}
								}
							}
						}
					}
				}
			}
			return kollision;
		}

		public void SetBlockPositionInFeld(Block block, int setzen)
		{
			for (int LayoutY = 0; LayoutY < block.Layout.Length; LayoutY++)
			{
				for (int LayoutX = 0; LayoutX < block.Layout[LayoutY].Length; LayoutX++)
				{
					if (block.Layout[LayoutY][LayoutX] == 1)
					{
						feld[block.Position.Y + LayoutY][block.Position.X + LayoutX - 1] = setzen;
					}
				}
			}
		}
	}
}