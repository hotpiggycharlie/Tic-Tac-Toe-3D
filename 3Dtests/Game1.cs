using _3Dtests.Content;
using _3Dtests.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _3Dtests
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector3 Target, CurrentFacing;
        Opponent _opponent;
        BoardManager _boardManager;
        Model _cellModel, cross, nought;
        private SpriteFont _font;
        private double FpsTime;
        private int TEMPFPS, FPS, BoardSize = 3;
        public string DebugString;
        private Vector3 CameraPosition = new Vector3(-4,-16,15);
        MouseState delayedMouseState;
        private GameStates Gamestate = GameStates.Menu;
        private Texture2D _menuBackground;
        private MenuButton _startButton;
        private MenuButton[] _boardSizeButtons = new MenuButton[4];
        private List<MenuButton> _buttons = new List<MenuButton>(); //list lol
        private GameModeSelector _modeSelector;
        private GameWindow _window;
        private Player _player1, _player2;
        private int _playerturn;
        private BackgroundManagment _backgroundManagment;

        public event Action Start;

        private Matrix view = Matrix.CreateLookAt(new Vector3(-4, -16, 15), new Vector3(-4, -10, 15), -Vector3.UnitY);

        public readonly Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1280f / 720, 0.1f, 100f);




        public Game1() //THE MAIN CLASS, PREPARE FOR A MIGRAINE BEFORE ENTERING
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280; //Sets resolution
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _opponent = new Opponent(new Vector3(0, 0, -2)); //initialise opponent class
            _boardManager = new BoardManager(); //initialise the board manager, which later initialises the board in

            MenuInitialiseButtons();

            _player1 = new Player(CellState.Cross, "Player 1");
            _player2 = new Player(CellState.Nought, "Player 2");

            _backgroundManagment = new BackgroundManagment(Content.Load<Model>("Room"));

            Start += _backgroundManagment.SortAssets;

            base.Initialize();
            _window = this.Window;
        }

        private void MenuInitialiseButtons()
        {
            _buttons.Add(_startButton = new MenuButton(Content.Load<Texture2D>("MenuButtons/ButtonTexture"), new Vector2(38, 172))); //initialises start button lol
            //_buttons.Add(_settings = new MenuButton(Content.Load<Texture2D>("MenuButtons/Settings"), new Vector2(38, 265)));
            _buttons.Add(_boardSizeButtons[0] = new MenuButton(Content.Load<Texture2D>("MenuButtons/3x3"), new Vector2(414, 174)));
            _buttons.Add(_boardSizeButtons[1] = new MenuButton(Content.Load<Texture2D>("MenuButtons/5x5"), new Vector2(531, 174)));
            _buttons.Add(_boardSizeButtons[2] = new MenuButton(Content.Load<Texture2D>("MenuButtons/4x4"), new Vector2(414, 238)));
            _buttons.Add(_boardSizeButtons[3] = new MenuButton(Content.Load<Texture2D>("MenuButtons/9x9"), new Vector2(531, 238)));
            _buttons.Add(_modeSelector = new GameModeSelector(Content.Load<Texture2D>("MenuButtons/PVP"), Content.Load<Texture2D>("MenuButtons/AI"), new Vector2(38, 256)));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            
            _opponent.Model = Content.Load<Model>("Itbegins");
            _boardManager.LoadModelBoard(Content.Load<Model>("Board"));
            _cellModel = Content.Load<Model>("Cell");
            _font = Content.Load<SpriteFont>("DebugText");
            cross = Content.Load<Model>("Cross");
            nought = Content.Load<Model>("Nought");
            _menuBackground = Content.Load<Texture2D>("MenuButtons/MenuBack");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            switch (Gamestate) {
                case GameStates.Menu:
            {
                        UpdateProcedures.MenuUpdate( _buttons, _window, _startButton,ref DebugString,ref delayedMouseState,
                            _modeSelector,ref Gamestate,ref _playerturn,ref _menuBackground, _boardManager,ref BoardSize, _cellModel,
                            _boardSizeButtons); //just a few things to pass over... OK maybe more than I thought lol. (and this is just for the menu)
                    break;
            }
                case GameStates.PVP or GameStates.VsAi:
                    {
                        MainUpdateLoop(gameTime);
                        break;
                    }
            }
            base.Update(gameTime);
        }


        private void MainUpdateLoop(GameTime gameTime) //update loop for the game, seperate method to make things simpler, kinda (I can't read)
                                                       //EDIT: I couldn't move this into update procedures :(
                                                       //(getstates don't work outside of Game1)
        {
            var kstate = Keyboard.GetState();
            TEMPFPS++;
            FpsTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FpsTime >= 1000)
            {
                FPS = TEMPFPS;
                TEMPFPS = 0;
                FpsTime = 0;
            }

            var mouseState = Mouse.GetState(_window);

            if (mouseState.Y >= 600)
            {
                Target = new Vector3(0, 8, -1);
            }
            else if (mouseState.Y <= 100)
            {
                Target = new Vector3(-4, -10, 15);
            }
            if (CurrentFacing != Target)
            {
                CurrentFacing = Vector3.Lerp(CurrentFacing, Target, 0.25f);
            }
            view = Matrix.CreateLookAt(CameraPosition, CurrentFacing, Vector3.Down);

            if (_boardManager.Initialized)
            {
                Cell hoveringOver = _boardManager.MouseHoveringOverCell(new Vector2(mouseState.X, mouseState.Y), view, projection, this.GraphicsDevice.Viewport);
                if (hoveringOver != null)
                {
                    TurnManagment(mouseState, hoveringOver);
                }
            }
            delayedMouseState = mouseState;
        }

        private void TurnManagment(MouseState mouseState, Cell hoveringOver)
        {
            if (Gamestate == GameStates.VsAi)
            {
                AIRoundControl(mouseState, hoveringOver);
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released && hoveringOver.State == CellState.Empty)
                {
                    if (_playerturn == 1)
                    {
                        hoveringOver.update(_player1.PlayerTeam, nought, cross);
                        _playerturn++;
                    }
                    else
                    {
                        hoveringOver.update(_player2.PlayerTeam, nought, cross);
                        _playerturn--;
                    }
                    CellState? WinningCell = _boardManager.CheckWin();
                    if (WinningCell != CellState.Empty)
                    {
                        if (WinningCell == _player1.PlayerTeam)
                        {
                            _player1.Score += 1;
                        }
                        else
                        {
                            _player2.Score += 1;
                        }
                        _boardManager.Reset();
                    }
                }
            }
        }


        private void AIRoundControl(MouseState mouseState, Cell hoveringOver)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released && hoveringOver.State == CellState.Empty)
            {
                hoveringOver.update(_player1.PlayerTeam, nought, cross);
                CellState? WinningCell = _boardManager.CheckWin();
                if (WinningCell != CellState.Empty && WinningCell != null)
                {
                    if (WinningCell == _player1.PlayerTeam)
                    {
                        _player1.Score += 1;
                    }
                    else
                    {
                        _player2.Score += 1;
                    }
                    _boardManager.Reset();
                }
                else
                {
                    if (WinningCell != null)
                    {
                        EnemyAiController.EasyMove(_boardManager.CellGrid).update(_player2.PlayerTeam, nought, cross);
                        WinningCell = _boardManager.CheckWin();
                        if (WinningCell == _player2.PlayerTeam)
                        {
                            _player2.Score += 1;
                            _boardManager.Reset();
                        }
                    }
                    else
                    {
                        _boardManager.Reset();
                    }
                }
                DebugString = $"{_player1.Name}:" + _player1.Score + $" {_player2.Name}:" + _player2.Score;
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);
            // TODO: Add your drawing code here
            switch (Gamestate)
            {
                case GameStates.VsAi or GameStates.PVP:
                    {
                        _spriteBatch.Begin();

                        _spriteBatch.DrawString(_font, $"{FPS}", new Vector2(0, 0), Color.Black);

                        _spriteBatch.DrawString(_font, $"{DebugString}", new Vector2(100, 0), Color.Black);

                        _spriteBatch.End();

                        if (Gamestate == GameStates.VsAi)
                        {
                            DrawModel(_opponent.Model, _opponent.World, view, projection);
                        }

                        if (_boardManager.Initialized)
                        {
                            DrawBoard();
                        }
                        _backgroundManagment.DrawMain(view, projection, DrawModel);
                        break;
                    }
                case GameStates.Menu:
                {
                        _backgroundManagment.DrawMain(view, projection, DrawModel);
                        _spriteBatch.Begin();
                        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                        //_spriteBatch.Draw(_menuBackground, Vector2.Zero, Color.White);
                        foreach(MenuButton button in _buttons)
                        {
                            button.Draw(_spriteBatch);
                        }
                        _spriteBatch.DrawString(_font, $"{DebugString}", Vector2.Zero, Color.Black);
                        _spriteBatch.End();
                        break;
                }
            }
            
            base.Draw(gameTime);
        }


        private void DrawBoard()
        {
            DrawModel(_boardManager.Board.BoardModel, _boardManager.Board.World, view, projection);
            foreach (Cell cell in _boardManager.CellGrid)
            {
                DrawModel(cell.Model, cell.World, view, projection);
                if (cell.Marked())
                {
                    DrawModel(cell.ExtraModel, cell.World, view, projection);
                }
            }
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix Projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {

                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = Projection;
                    if (effect.LightingEnabled == false)
                    {
                        effect.EnableDefaultLighting();
                    }
                }
                mesh.Draw();
            }
        }

    }
}