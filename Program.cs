using System;
using NAudio.Wave;

namespace Tetris
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Block.Laden();
			Menu menu = new();
			menu.StartBild();
		}
	}
}