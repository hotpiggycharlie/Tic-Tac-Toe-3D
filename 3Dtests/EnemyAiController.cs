using Tic_Tac_Toe.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tic_Tac_Toe
{
    static class EnemyAiController // Room for expansion here lol
{
        public static Cell EasyMove(Cell[,] Board)
        {
            while (true)
            {
                Random RNG = new Random();
                int x = RNG.Next(0, Board.GetLength(0));
                int y = RNG.Next(0, Board.GetLength(1));
                if (Board[x, y].State == CellState.Empty)
                {
                    return Board[x, y];
                }
            }
        }
}
}
