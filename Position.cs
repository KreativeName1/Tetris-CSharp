using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
	public class Position
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is Position))
				return false;

			Position other = (Position)obj;
			return this.X == other.X && this.Y == other.Y;
		}

		public void Delete()
		{
			Console.SetCursorPosition(X, Y);
			Console.Write(" ");
		}

		public override string ToString()
		{
			return $"X: {X}, Y: {Y}";
		}

    public override int GetHashCode()
    {
      return HashCode.Combine(X, Y);
    }
  }
}