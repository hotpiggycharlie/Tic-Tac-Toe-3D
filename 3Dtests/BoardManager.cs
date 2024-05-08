using Tic_Tac_Toe.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    class BoardManager: DrawableGameComponent // a fancy class, it does all win checking and initialising of the board, it doesn't handle any models though
    {
        private Cell[,] boardMetaphor;
        float TotalBoardSizeX = 1.25f, TotalBoardSizeY = 1f, CellSize = 0.018f;
        private Board _board;
        private bool _initialize = false;
        public Board Board { get { return _board; } }
        public Cell[,] CellGrid { get { return boardMetaphor; } }
        public bool Initialized { get { return _initialize; } }

        public bool Playable { get; private set; }

        private Camera _camera;
        private Matrix projection;
        public Action<MouseState, Cell> TurnEnd;
        private GameWindow window;
        Game1 game1;

        public BoardManager(Game game, Camera camera) : base(game)
        {
            _camera = camera;
            game1 = (Game1)game;
            projection = game1.projection;
            window = game1.Window;
            game1.RoundEnd += Reset;
            Playable = true;
        }

        public void Inistialize(int BoardSizeX, int BoardSizeY, Model cellModel) 
        {
            float totalspaceoccupiedx = CellSize * BoardSizeX;
            float totalspaceoccupiedy = CellSize * BoardSizeY;



            float SpaceBetweenCellsX = (TotalBoardSizeX - totalspaceoccupiedx) / (BoardSizeX - 1);
            float SpaceBetweenCellsY = (TotalBoardSizeY - totalspaceoccupiedy) / (BoardSizeY - 1);

            boardMetaphor = new Cell[BoardSizeX, BoardSizeY];
            for (int i = 0; i < BoardSizeX; i++) 
            {
                for (int j = 0; j < BoardSizeY; j++)
                {
                    Vector2 Position = new Vector2(j * SpaceBetweenCellsX + j * CellSize - 0.6f, SpaceBetweenCellsY * i + i * CellSize - 0.5f); // MIN 0.7, -0.5  MAX -0.6, 0.75
                    boardMetaphor[i, j] = new Cell(Position, cellModel);
                    boardMetaphor[i, j].GetGlobalPosition(_board.World);
                    boardMetaphor[i, j].Corrections(0.15f, i * 3 + j);
                }
            }
            _initialize = true;
        }


        public void Reset(CellState? cell)
        {
            Playable = false;
            if (game1.RoundEndScreenNum == 119)
            {
                if (_initialize)
                {
                    for (int i = 0; i < boardMetaphor.GetLength(1); i++)
                    {
                        for (int j = 0; j < boardMetaphor.GetLength(0); j++)
                        {
                            boardMetaphor[i, j].State = CellState.Empty;
                        }
                    }
                }
                Playable = true;
            }
        }

        public CellState? CheckWin()
        {
            CellState cellState = CellState.Empty;

            cellState = HorizontalCheck(); //CHECK HORIZONTALLY
            if (cellState != CellState.Empty)
            {
                return cellState;
            }

            cellState = VerticalCheck(); //CHECK VERTICALLY
            if (cellState != CellState.Empty)
            {
                return cellState;
            }

            

            if (boardMetaphor.GetLength(1) == boardMetaphor.GetLength(0))
            {
                cellState = DiagonalRightToLeft(); //CHECK DIAGONAL TOP RIGHT TO BOTTOM LEFT
                if (cellState != CellState.Empty)
                {
                    return cellState;
                }

                cellState = DiagonalLeftToRight();//CHECK DIAGONAL TOP LEFT TO BOTTOM RIGHT
                if (cellState != CellState.Empty)
                {
                    return cellState;
                }
            }

            for (int i = 0; i < boardMetaphor.GetLength(0); i++)
            {
                for (int j = 0; j < boardMetaphor.GetLength(1); j++)
                {
                    if (boardMetaphor[i,j].State == CellState.Empty)
                    {
                        return CellState.Empty;
                    }
                }
            }

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            if (Initialized && Playable)
            {
                MouseState mouseState = Mouse.GetState(window);
                Cell hoveringOver = MouseHoveringOverCell(new Vector2(mouseState.X, mouseState.Y), _camera.View, projection, this.GraphicsDevice.Viewport);
                if (hoveringOver != null)
                {
                    hoveringOver.SELECTED = true;
                    TurnEnd.Invoke(mouseState, hoveringOver);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)//caused some bugs?
        {
            base.Draw(gameTime);
        }

        public void LoadModelBoard(Model model, Game game)
        {
            _board = new Board(model);
        }

        public Cell? MouseHoveringOverCell(Vector2 Mouselocation, Matrix view, Matrix projection, Viewport viewport)
        {
            bool intersection = false;
            for (int i = 0; i <= boardMetaphor.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= boardMetaphor.GetLength(1) - 1; j++)
                {
                    foreach (ModelMesh mesh in boardMetaphor[i, j].Model.Meshes)
                    {
                        if (UserInputManagment.MouseRayIntesects(boardMetaphor[i, j].World, boardMetaphor[i, j].Model, Mouselocation, view, projection, viewport))
                        {
                            intersection = true; break;
                        }

                    }
                    if (intersection)
                    {
                        return boardMetaphor[i, j];
                    }
                }
            }
            return null;
        }


        private CellState VerticalCheck()
        {
            bool win = false;
            CellState cellState = CellState.Empty;
            for (int i = 0; i < boardMetaphor.GetLength(1); i++) //VERTICAL WIN CHECKING
            {
                if (boardMetaphor[0, i].State != CellState.Empty)
                {
                    
                    for (int j = 0; j < boardMetaphor.GetLength(0); j++)
                    {
                        if (boardMetaphor[0, i].State == boardMetaphor[j, i].State)
                        {
                            win = true;
                            cellState = boardMetaphor[0, i].State;
                        }
                        else
                        {
                            win = false;
                            cellState = CellState.Empty;
                            break;
                        }
                    }
                    if (win)
                    {
                        return cellState;
                    }
                }
            }
            return cellState;
        }

        private CellState HorizontalCheck()
        {
            bool win = false;
            CellState cellState = CellState.Empty;
            for (int i = 0; i < boardMetaphor.GetLength(0); i++) //HORIZONAL WIN CHECKING
            {
                if (boardMetaphor[i, 0].State != CellState.Empty)
                {
                    for (int j = 0; j < boardMetaphor.GetLength(1); j++)
                    {
                        if (boardMetaphor[i, 0].State == boardMetaphor[i, j].State)
                        {
                            win = true;
                            cellState = boardMetaphor[i, 0].State;
                        }
                        else
                        {
                            win = false;
                            cellState = CellState.Empty;
                            break;
                        }
                    }
                    if (win)
                    {
                        return cellState;
                    }
                }
            }
            return CellState.Empty;
        }

        private CellState DiagonalRightToLeft()
        {
            if (boardMetaphor[0, 0].State != CellState.Empty) //DIAGONAL BOTTOM LEFT TO TOP RIGHT
            {
                bool DiagonalWin = false;
                for (int i = 0; i < boardMetaphor.GetLength(1); i++)
                {
                    if (boardMetaphor[i, i].State == boardMetaphor[0, 0].State)
                    {
                        DiagonalWin = true;
                    }
                    else
                    {
                        DiagonalWin = false;
                        break;
                    }
                }
                if (DiagonalWin)
                {
                    return boardMetaphor[0, 0].State;
                }
            }
            return CellState.Empty;
        }


        private CellState DiagonalLeftToRight()
        {
            int lengthx = boardMetaphor.GetLength(1) - 1;
            if (boardMetaphor[lengthx, 0].State != CellState.Empty)
            {
                bool DiagonalWin = false;
                for (int i = 0; i <= lengthx; i++)
                {
                    if (boardMetaphor[lengthx - i, i].State == boardMetaphor[lengthx, 0].State)
                    {
                        DiagonalWin = true;
                    }
                    else
                    {
                        DiagonalWin = false;
                        break;
                    }
                }
                if (DiagonalWin)
                {
                    return boardMetaphor[lengthx, 0].State;
                }
            }
            return CellState.Empty;

        }
    }
}
