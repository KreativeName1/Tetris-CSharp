using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Tetris.SoundPlayback;

namespace Tetris
{
  public class Game
  {
    private static List<Block> placedBlocks = new List<Block>();
    private readonly IAudioPlayer _player;
    private int boardWidth;
    private int boardHeight;
    private int gameSpeed;
    private Block currentPiece;
    private Block nextPiece;
    private Random randomizer;
    private Music music;
    private int score = 0;
    private int level;
    private int[][] board;

    public Game(int boardWidth, int boardHeight, int gameSpeed, int level, Music music, IAudioPlayer player)
    {

      placedBlocks.Clear();
      _player = player;

      this.boardWidth = boardWidth;
      this.boardHeight = boardHeight;
      this.gameSpeed = gameSpeed;
      this.level = level;
      this.music = music;
      randomizer = new();

      board = Enumerable.Range(0, this.boardHeight + 1).Select(x => new int[boardWidth]).ToArray();

      _player.TurnOnMusic();
      _player.PlayMusic(this.music);

      for (int i = 1; i < level; i++)
      {
        this.gameSpeed = Math.Max(100, this.gameSpeed - (i * 3));
      }

      RenderBoard();
      UpdateScore();

      // Ersten Block und Vorschau Block generieren
      currentPiece = GenerateBlock();
      nextPiece = GenerateBlock();
      nextPiece.Position = new(boardWidth + 3, 1);
      nextPiece.Render();

      GameLoop();
    }

    private void UpdateScore()
    {
      Console.ForegroundColor = ConsoleColor.White;
      Console.SetCursorPosition(boardWidth + 3, 5);
      Console.Write($"S {score}");
      Console.SetCursorPosition(boardWidth + 3, 8);
      Console.Write($"L {level}");
    }

    private void LevelUp()
    {
      _player.PlaySound(Sound.Levelup);
      level++;
      if (score % 10 == 0) level++;
      gameSpeed = Math.Max(100, gameSpeed - (level * 3));
      score = 0;
    }

    private bool IsGameOver()
    {
      if (currentPiece.Position.Y < 2)
      {
        _player.TurnOffMusic();
        _player.PlaySound(Sound.Gameover);
        PlayGameOverAnimation();
        Console.ReadKey();
        return true;
      }
      return false;
    }

    private void GameLoop()
    {
      while (true)
      {
        placedBlocks = new List<Block>(placedBlocks
    .Where(obj => !obj.Pattern.All(row => row.All(element => element == 0)))
    .ToList());
        CheckLanding();

        if (currentPiece.IsLanded == false)
        {
          currentPiece.Erase();
          Direction bew = currentPiece.Move(boardHeight);

          // Ob der Block sich bewegen darf wird in der Kollision mit anderen Blöcken geprüft
          if (currentPiece.canMoveLeftCorner && bew == Direction.Left) currentPiece.Position.Y++;
          else if (currentPiece.canMoveRightCorner && bew == Direction.Right) currentPiece.Position.Y++;
          else if (bew == Direction.Nothing) currentPiece.Position.Y++;
        }
        currentPiece.CanMoveLeft = true;
        currentPiece.canMoveRight = true;
        currentPiece.canMoveLeftCorner = true;
        currentPiece.canMoveRightCorner = true;

        CheckSideCollision();
        currentPiece.Render();

        if (currentPiece.IsLanded)
        {
          _player.PlaySound(Sound.Land);
          for (int LayoutY = 0; LayoutY < currentPiece.Pattern.Length; LayoutY++)
          {
            for (int LayoutX = 0; LayoutX < currentPiece.Pattern[LayoutY].Length; LayoutX++)
            {
              if (currentPiece.Pattern[LayoutY][LayoutX] == 1)
              {
                board[currentPiece.Position.Y + LayoutY][currentPiece.Position.X + LayoutX - 1] = 1;
              }
            }
          }
          if (IsGameOver()) break;
          placedBlocks.Add(currentPiece);
          nextPiece.Erase();
          currentPiece = new Block(boardWidth / 2, 0, nextPiece.Color, nextPiece.Pattern.Select(x => x.ToArray()).ToArray());
          nextPiece = GenerateBlock();
          nextPiece.Position = new(boardWidth + 3, 1);
          nextPiece.Render();
        }

        CheckAndDeleteLines();
        Debug();
        Thread.Sleep(gameSpeed);
      }
    }

    private void PlayGameOverAnimation()
    {
      for (int y = boardHeight; y >= 0; y--)
      {
        Console.ForegroundColor = ConsoleColor.Black;
        Thread.Sleep(70);
        for (int x = 0; x < boardWidth; x++)
        {
          Console.SetCursorPosition(x + 1, y);
          Console.Write("█");
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(1, boardHeight / 2);
        Console.WriteLine("Game Over!");
      }
    }

    private void RenderBoard()
    {
      Console.ForegroundColor = ConsoleColor.Black;

      DrawHorizontalLine(0, boardHeight + 1, boardWidth + 2, "▀");

      DrawVerticalLine(0, boardHeight + 1, "█");
      DrawVerticalLine(boardWidth + 1, boardHeight + 1, "█");

      DrawHorizontalLine(2 + boardWidth, 4, 7, "▀");
      DrawHorizontalLine(2 + boardWidth, 7, 7, "▀");
      DrawHorizontalLine(2 + boardWidth, 10, 7, "▀");

      DrawVerticalLine(boardWidth + 8, 10, "█");
    }

    private void Debug()
    {
      for (int y = 0; y < board.Length; y++)
      {
        for (int x = 0; x < board[y].Length; x++)
        {
          Console.SetCursorPosition(boardWidth + 10 + x, y + 1);
          Console.Write(board[y][x]);
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

    private Block GenerateBlock()
    {
      Block block = Block.Blöcke[randomizer.Next(0, Block.Blöcke.Count)];
      return new Block(boardWidth / 2, 0, block.Color, block.Pattern.Select(x => x.ToArray()).ToArray());
    }

    private void CheckSideCollision()
    {
      int[] breitesteZeile = currentPiece.Pattern.OrderByDescending(x => x.Length).First();
      int newX = Math.Max(1, Math.Min(boardWidth - breitesteZeile.Length + 1, currentPiece.Position.X));

      if (currentPiece.Position.X + breitesteZeile.Length - 1 >= boardWidth + 1 || currentPiece.Position.X < 1)
      {
        currentPiece.Erase();
        currentPiece.Position.X = newX;
        RenderBoard();
      }
    }

    // Hier wird die Kollision mit anderen Blöcken geprüft
    private void CheckLanding()
    {
      if (currentPiece.Position.Y + currentPiece.Pattern.Count() >= boardHeight + 1) currentPiece.IsLanded = true;
      if (CheckCollisionWithOtherBlocks(currentPiece)) currentPiece.IsLanded = true;
    }

    private void CheckAndDeleteLines()
    {
      for (int lineIndex = 0; lineIndex < board.Length; lineIndex++)
      {
        // .All(zeile => zeile == 1) -> Prüft ob alle Werte in der Zeile 1 sind
        if (board[lineIndex].All(line => line == 1))
        {
          foreach (Block currentBlock in placedBlocks)
          {
            for (int LayoutY = 0; LayoutY < currentBlock.Pattern.Length; LayoutY++)
            {
              for (int LayoutX = 0; LayoutX < currentBlock.Pattern[LayoutY].Length; LayoutX++)
              {
                if (currentBlock.Pattern[LayoutY][LayoutX] == 1 && currentBlock.Position.Y + LayoutY == lineIndex)
                {
                  currentBlock.Pattern[LayoutY][LayoutX] = 0;
                  board[lineIndex][currentBlock.Position.X + LayoutX - 1] = 0;
                  Console.SetCursorPosition(currentBlock.Position.X + LayoutX, currentBlock.Position.Y + LayoutY);
                  Console.Write(" ");
                }
              }
            }
          }
          _player.PlaySound(Sound.Linie);
          ShiftLinesDown(lineIndex);
          score++;
          if (score % 10 == 0) LevelUp();
          UpdateScore();
        }
      }
      Trace.WriteLine("\n");
    }

    public void ShiftLinesDown(int lineIndex)
    {
      for (int index = 0; index < placedBlocks.Count; index++)
      {
        if (placedBlocks[index].Pattern.All(row => row.All(element => element == 0)))
        {
          placedBlocks[index].Erase();
          placedBlocks.Remove(placedBlocks[index]);
        }
        else if (placedBlocks[index].Position.Y + placedBlocks[index].Pattern.Length - 1 <= lineIndex)
        {
          // Alte Position aus Feld Löschen
          SetBlockPositionInFeld(placedBlocks[index], 0);

          // Block runter bewegen
          placedBlocks[index].Erase();
          placedBlocks[index].Position.Y++;
          placedBlocks[index].Render();

          // Neue Position in das Feld setzten
          SetBlockPositionInFeld(placedBlocks[index], 1);
        }
        // Wenn vom block nur noch ein teil über der zeile ist,
        // Kompaktieren() aufrufen
        else if (placedBlocks[index].Position.Y < lineIndex && placedBlocks[index].Position.Y + placedBlocks[index].Pattern.Length - 1 > lineIndex)
        {
          SetBlockPositionInFeld(placedBlocks[index], 0);
          placedBlocks[index].Erase();
          bool isEmpty = placedBlocks[index].CompactPattern();
          if (!isEmpty)
          {
            placedBlocks[index].Render();
            // Hier kann es Abstürzen
            SetBlockPositionInFeld(placedBlocks[index], 1);
            Trace.WriteLine("Block kompaktiert: " + index);
          }
          else placedBlocks.Remove(placedBlocks[index]);
        }
      }
    }

    public static bool CheckCollisionWithOtherBlocks(Block block)
    {
      bool isColliding = false;
      foreach (var landedBlock in placedBlocks)
      {
        // Durch das Layout des aktuellen Blocks loopen
        for (int LayoutY = 0; LayoutY < block.Pattern.Length; LayoutY++)
        {
          for (int LayoutX = 0; LayoutX < block.Pattern[LayoutY].Length; LayoutX++)
          {
            if (block.Pattern[LayoutY][LayoutX] == 1)
            {
              Position lowerBlockPosition = new(block.Position.X + LayoutX, block.Position.Y + LayoutY + 1);
              Position rightBlockPosition = new(block.Position.X + LayoutX + 1, block.Position.Y + LayoutY);
              Position leftBlockPosition = new(block.Position.X + LayoutX - 1, block.Position.Y + LayoutY);
              Position bottomLeftBlockPosition = new(block.Position.X + LayoutX - 1, block.Position.Y + LayoutY + 1);
              Position bottomRightBlockPosition = new(block.Position.X + LayoutX + 1, block.Position.Y + LayoutY + 1);

              for (int landedLayoutY = 0; landedLayoutY < landedBlock.Pattern.Length; landedLayoutY++)
              {
                for (int landedLayoutX = 0; landedLayoutX < landedBlock.Pattern[landedLayoutY].Length; landedLayoutX++)
                {
                  if (landedBlock.Pattern[landedLayoutY][landedLayoutX] == 1)
                  {
                    Position PosGelandeterBlock = new(landedBlock.Position.X + landedLayoutX, landedBlock.Position.Y + landedLayoutY);
                    if (lowerBlockPosition.Equals(PosGelandeterBlock)) isColliding = true;
                    if (rightBlockPosition.Equals(PosGelandeterBlock)) block.canMoveRight = false;
                    if (leftBlockPosition.Equals(PosGelandeterBlock)) block.CanMoveLeft = false;
                    if (bottomLeftBlockPosition.Equals(PosGelandeterBlock)) block.canMoveLeftCorner = false;
                    if (bottomRightBlockPosition.Equals(PosGelandeterBlock)) block.canMoveRightCorner = false;
                  }
                }
              }
            }
          }
        }
      }
      return isColliding;
    }

    public void SetBlockPositionInFeld(Block block, int newValue)
    {
      for (int LayoutY = 0; LayoutY < block.Pattern.Length; LayoutY++)
      {
        for (int LayoutX = 0; LayoutX < block.Pattern[LayoutY].Length; LayoutX++)
        {
          if (block.Pattern[LayoutY][LayoutX] == 1)
          {
            board[block.Position.Y + LayoutY][block.Position.X + LayoutX - 1] = newValue;
          }
        }
      }
    }
  }
}