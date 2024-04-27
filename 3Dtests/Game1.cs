using Tic_Tac_Toe.Content;
using Tic_Tac_Toe.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Tic_Tac_Toe
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
        //private Vector3 CameraPosition = new Vector3(-4,-16,15);
        private MouseState delayedMouseState;
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
        private Camera _camera;
        public event Action Start;
        private Vector3 mouseRotationBuffer;
        private bool DebugMode;
        private KeyboardState DelayedKstate;
        ScoreManagment scrmgnt;

        public readonly Matrix projection;




        public Game1() //THE MAIN CLASS, PREPARE FOR A MIGRAINE BEFORE ENTERING
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280; //Sets resolution
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000f);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _camera = new Camera(this); //new Vector3(-4, -16, 15)
            _opponent = new Opponent(new Vector3(0, 0, -2)); //initialise opponent class
            _boardManager = new BoardManager(this, _camera); //initialise the board manager, which later initialises the board in

            MenuInitialiseButtons();

            _player1 = new Player(CellState.Cross, "Player 1");
            _player2 = new Player(CellState.Nought, "Player 2");
            scrmgnt = new ScoreManagment(this, Content.Load<Model>("Numbers/numbers"), _player1, _player2);

            _backgroundManagment = new BackgroundManagment(Content.Load<Model>("Room"), this);

            Start += _backgroundManagment.SortAssets;

            Components.Add(scrmgnt);
            Components.Add(_backgroundManagment);
            Components.Add(_boardManager);
            Components.Add(_camera);

            _boardManager.TurnEnd += TurnManagment;


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
            _boardManager.LoadModelBoard(Content.Load<Model>("Board"), this);
            _cellModel = Content.Load<Model>("Cell");
            _font = Content.Load<SpriteFont>("DebugText");
            cross = Content.Load<Model>("Cross");
            nought = Content.Load<Model>("Nought");
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
            if (kstate.IsKeyDown(Keys.LeftAlt) && !(DelayedKstate.IsKeyDown(Keys.LeftAlt))) //PRESS LEFT-ALT TO ENTER A DEBUGGING STATE, HELPED ME MASSIVELY
            {
                if (!DebugMode)
                {
                    DebugMode = true;
                    DebugString = "DEBUG MODE: FREECAM";
                    this.IsMouseVisible = false;

                }
                else
                {
                    DebugMode = false;
                }
            }
            TEMPFPS++;
            FpsTime += gameTime.ElapsedGameTime.TotalMilliseconds; //BASIC FPS COUNTER, THIS WAS MADE JUST WHEN I WAS TESTING THINGS
            if (FpsTime >= 1000)
            {
                FPS = TEMPFPS;
                TEMPFPS = 0;
                FpsTime = 0;
            }

            var mouseState = Mouse.GetState(_window); //TAKE THE MOUSE STATE
            if (!DebugMode)
            {
                if (mouseState.Y >= 600) //THIS HANDLES LOOKING UP AND DOWN USING THE MOUSE POSITION, THIS IS DONE IN A BASIC MANNER
                {
                    Target = _boardManager.Board.Position;
                }
                else if (mouseState.Y <= 100)
                {
                    Target = new Vector3(0, 7f, -3);
                }
                if (CurrentFacing != Target)
                {
                    CurrentFacing = Vector3.SmoothStep(CurrentFacing, Target, 0.15f); //THIS HANDLES THE SMOOTHING OF THE CAMERA WHEN TURNING
                }
                _camera.SetLookAt(CurrentFacing);
            }

            

            if (DebugMode)
            {
                DebugUpdateLoop(mouseState, gameTime, kstate);
                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2); //HANDLES DEBUG MODE

            }

            //; //SETS MY DELAYED MOUSE & KEYBOARD STATE AT END OF UPDATE LOOP
            DelayedKstate = kstate;
        }

        private void DebugUpdateLoop(MouseState mouseState, GameTime gameTime, KeyboardState kstate)
        {
            Vector3 moveVector = Vector3.Zero;
            float deltaX;
            float deltaY;
            if (mouseState != delayedMouseState)
            {
                deltaX = mouseState.X - (GraphicsDevice.Viewport.Width / 2);
                deltaY = mouseState.Y - (GraphicsDevice.Viewport.Height / 2);

                mouseRotationBuffer.X -= 0.1f * deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mouseRotationBuffer.Y -= 0.1f * deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                }

                if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));
                }

                _camera.SetRotation(new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)),
                    MathHelper.WrapAngle(mouseRotationBuffer.X), 0));



            }
            if (kstate.IsKeyDown(Keys.W)) //forward
                moveVector.Z = 1;

            if (kstate.IsKeyDown(Keys.S)) //back
                moveVector.Z = -1;

            if (kstate.IsKeyDown(Keys.A)) //left
                moveVector.X = 1;

            if (kstate.IsKeyDown(Keys.D)) //right
                moveVector.X = -1;

            if (kstate.IsKeyDown(Keys.LeftControl)) //down
                moveVector.Y = -1;

            if (kstate.IsKeyDown(Keys.Space)) //up
                moveVector.Y = +1;

            if (moveVector != Vector3.Zero)
            {
                moveVector *= (float)gameTime.ElapsedGameTime.TotalSeconds; //to make FPS irrelevent

                _camera.SetPosition(moveVector);
            }
        }

        private void TurnManagment(MouseState mouseState, Cell hoveringOver)
        {
            DebugString = "HOVERING";
            if (Gamestate == GameStates.VsAi)
            {
                AIRoundControl(mouseState, hoveringOver);
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released && hoveringOver.State == CellState.Empty)
                {
                    DebugString = "CLICKING";
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
            delayedMouseState = mouseState;
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
                        

                        if (Gamestate == GameStates.VsAi)
                        {
                            UpdateProcedures.DrawModel(_opponent.Model, _opponent.World, _camera.View, projection, GraphicsDevice);
                        }

                        _backgroundManagment.DrawMain(_camera.View, projection);
                        if(scrmgnt.Draw == true)
                        {
                            if (_player1.Score != 0)
                            {
                                DrawModelMesh(scrmgnt.ScoreUpdate(_player1.Score));
                            }
                            if (_player2.Score != 0)
                            {
                                DrawModelMesh(scrmgnt.ScoreUpdate(_player2.Score));
                            }
                        }



                        _spriteBatch.Begin(); //DRAWS 2D STUFF, MUST BE DRAWN LAST

                        _spriteBatch.DrawString(_font, $"{FPS}", new Vector2(0, 0), Color.Wheat);

                        _spriteBatch.DrawString(_font, $"{DebugString}", new Vector2(100, 0), Color.Red);

                        _spriteBatch.End();

                        break;
                    }
                case GameStates.Menu:
                {
                        _backgroundManagment.DrawMain(_camera.View, projection);
                        _spriteBatch.Begin();
                        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
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

        private void DrawModelMesh(ModelMesh mesh)
        {
            
            foreach (BasicEffect effect in mesh.Effects)
            {

                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                effect.World = scrmgnt.World;
                effect.View = _camera.View;
                effect.Projection = projection;
                if (effect.LightingEnabled == false)
                {
                    effect.EnableDefaultLighting();
                }
            }
            mesh.Draw();
        }


        

    }
}