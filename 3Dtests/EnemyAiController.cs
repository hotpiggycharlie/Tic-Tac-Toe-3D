using Tic_Tac_Toe.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    static class EnemyAiController // I think I wrote more comments here than everywhere else combined lmao
    {


        public static Cell EnemyAIMove(int difficulty, Cell[,] Board, BoardManager manager)
        {
            if (difficulty == 1)
            {
                return EasyMove(Board); //random
            }
            else if (difficulty == 2)
            {
                return MediumMove(Board, manager); //block, no attack
            }
            else
            {
                return HardMove(Board, manager); //attack, no block
            }
        }

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

        public static Cell MediumMove(Cell[,] Board, BoardManager BoardManager)
        {
            int boardsize = Board.GetLength(0);
            // Check if player can win in the next move and block it
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (Board[i, j].State == CellState.Empty)
                    {
                        Board[i, j].State = CellState.Cross;
                        if (BoardManager.CheckWin() == (CellState.Cross))
                        {// Block opponent's winning move
                            return Board[i, j];
                        }
                        Board[i, j].State = CellState.Empty; // Undo move
                    }
                }
            }
            // If no winning or blocking move, make a random move
            Random rand = new Random();
            int randRow, randCol;
            do
            {
                randRow = rand.Next(0, boardsize);
                randCol = rand.Next(0, boardsize);
            } while (Board[randRow, randCol].State != CellState.Empty);
            return Board[randRow, randCol];
        }


        public static Cell HardMove(Cell[,] Board, BoardManager BoardManager)
        {
            // Check if AI can win in the next move
            int boardsize = Board.GetLength(0);
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (Board[i, j].State == CellState.Empty)
                    {
                        Board[i, j].State = CellState.Nought;
                        if (BoardManager.CheckWin() == (CellState.Nought))
                        {
                            return Board[i, j]; // AI wins
                        }
                        Board[i, j].State = CellState.Empty; // Undo move
                    }
                }
            }

            // MediumMove code:
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (Board[i, j].State == CellState.Empty)
                    {
                        Board[i, j].State = CellState.Cross;
                        if (BoardManager.CheckWin() == (CellState.Cross))
                        {
                            return Board[i, j];
                        }
                        Board[i, j].State = CellState.Empty;
                    }
                }
            }

            // Random if all else fails
            Random rand = new Random();
            int randRow, randCol;
            do
            {
                randRow = rand.Next(0, boardsize);
                randCol = rand.Next(0, boardsize);
            } while (Board[randRow, randCol].State != CellState.Empty);
            return Board[randRow, randCol];
        }

    
}

}
