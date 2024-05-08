using Tic_Tac_Toe.Content;
using Tic_Tac_Toe.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.IO;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using System.Threading;

namespace Tic_Tac_Toe
{
    public class Game1 : Game
    {
        //standard variables
        private double FpsTime;
        public string DebugString;
        public GameStates Gamestate = GameStates.Menu;
        private GameWindow _window;
        private int _playerturn, TEMPFPS, FPS, BoardSize = 3, difficulty = 1, CurrentRound = 0, MaxRounds;
        private bool DebugMode, GameOver;
        public int RoundEndScreenNum = 120, UserID;
        private string RoundWinner;
        
        //Custom Classes
        private MenuButton[] _boardSizeButtons = new MenuButton[4];
        private DataBaseManager _databaseManager;
        private ScoreManagment scrmgnt;
        private Camera _camera;
        private BackgroundManagment _backgroundManagment;
        public Player _player1, _player2;
        private GameModeSelector _modeSelector;
        private List<MenuButton> _buttons = new List<MenuButton>(); //list lol
        private List<TextBox> _textBoxs = new List<TextBox>();
        private Characters _opponent;
        private BoardManager _boardManager;

        //Drawable Content
        private Model _cellModel, cross, nought;
        private SpriteFont _font, _RoundEndFont, _textBoxFont;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D FacePlayer1, FacePlayer2;

        //Delegates and structures
        public Action<CellState?> RoundEnd;
        public readonly Matrix projection;
        private KeyboardState DelayedKstate;
        public event Action Start;
        private Vector3 mouseRotationBuffer, Target, CurrentFacing, TargetPos, CurrentPos;
        private MouseState delayedMouseState;


        public Game1() //THE MAIN CLASS, PREPARE FOR A MIGRAINE BEFORE ENTERING
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280; //Sets resolution
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _camera = new Camera(this); //new Vector3(-4, -16, 15)
            _opponent = new Characters(new Vector3(0,0,3.5f), new Vector3(0, 0f, -3.5f)); //initialise opponent class
            _boardManager = new BoardManager(this, _camera); //initialise the board manager, which later initialises the board in

            MenuInitialiseButtons();

            _textBoxFont = Content.Load<SpriteFont>("TextInput");

            _player1 = new Player(CellState.Cross, "Player 1");
            _player2 = new Player(CellState.Nought, "Player 2");
            scrmgnt = new ScoreManagment(this, Content.Load<Model>("Numbers/numbers"), _player1, _player2, _camera);
            _databaseManager = new DataBaseManager(this, new Texture2D[] { Content.Load<Texture2D>("MenuButtons/LoginTextInput"), Content.Load<Texture2D>("MenuButtons/Sign Up"), Content.Load<Texture2D>("MenuButtons/Login") }, Content.Load<SpriteFont>("TextInput"), _player1);

            _backgroundManagment = new BackgroundManagment(Content.Load<Model>("Room"), this); //not really needed... Could do with being removed?

            RoundEnd += RoundEndScreen;
            Start += _backgroundManagment.SortAssets;

            Components.Add(scrmgnt);//also list lol?
            Components.Add(_backgroundManagment);
            Components.Add(_boardManager);
            Components.Add(_camera);
            Components.Add(_databaseManager);

            _boardManager.TurnEnd += TurnManagment;
            _databaseManager.RefreshLeaderboard();

            base.Initialize();
            _window = this.Window;
        }




        private void MenuInitialiseButtons()
        {
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/ButtonTexture"), new Vector2(38, 172), 1)); //initialises start button lol
            //_buttons.Add(_settings = new MenuButton(Content.Load<Texture2D>("MenuButtons/Settings"), new Vector2(38, 265)));
            _buttons.Add(_boardSizeButtons[0] = new MenuButton(Content.Load<Texture2D>("MenuButtons/3x3"), new Vector2(414, 174), 2));
            _buttons.Add(_boardSizeButtons[1] = new MenuButton(Content.Load<Texture2D>("MenuButtons/5x5"), new Vector2(531, 174), 3));
            _buttons.Add(_boardSizeButtons[2] = new MenuButton(Content.Load<Texture2D>("MenuButtons/4x4"), new Vector2(414, 238), 4));
            _buttons.Add(_boardSizeButtons[3] = new MenuButton(Content.Load<Texture2D>("MenuButtons/9x9"), new Vector2(531, 238), 5));
            _buttons.Add(_modeSelector = new GameModeSelector(Content.Load<Texture2D>("MenuButtons/PVP"), Content.Load<Texture2D>("MenuButtons/AI"), new Vector2(38, 256), 6));
            _textBoxs.Add(new TextBox(new Vector2(416, 350), Content.Load<Texture2D>("MenuButtons/ButtonBlank"), this, 1, true, false, Content.Load<SpriteFont>("TextInput"), "Rounds: ", 107.5f, 370, 7));
            _buttons.Add(_textBoxs[0]);
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/EASY"), new Vector2(662, 177), 8));
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/MEDIUM"), new Vector2(662, 254), 9));
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/HARD"), new Vector2(662, 326), 10));
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/LeaderBoard"), new Vector2(949, 25), 11));
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/P1CustomFaceButton"), new Vector2(698, 659), 12));
            _buttons.Add(new MenuButton(Content.Load<Texture2D>("MenuButtons/P2CustomFaceButton"), new Vector2(828, 659), 13));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            
            _opponent.Model = Content.Load<Model>("Character");
            _boardManager.LoadModelBoard(Content.Load<Model>("Board"), this);
            _cellModel = Content.Load<Model>("Cell");
            _font = Content.Load<SpriteFont>("Text");
            _RoundEndFont = Content.Load<SpriteFont>("RoundEndText");
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
                        MenuUpdate();

                        DebugString = _textBoxs[0].heldText;
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


        private void MenuUpdate() //UPDATE FOR THE MENU (I can't really read this anymore)
        {

            foreach (MenuButton button in _buttons)
            {
                button.Colour = Color.White;
            }
            var mouseState = Mouse.GetState(_window);
            Rectangle MouseCollider = new Rectangle(mouseState.X, mouseState.Y, 10, 10);
            MenuButton? ButtonHovering = UserInputManagment.HoveringOverButton(MouseCollider, _buttons);
            int temp = 0;
            if (ButtonHovering != null)
            {
                temp = ButtonHovering.ID;
            }
            switch (temp)
            {
                case 1: //start button
                    {


                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            if (_modeSelector.IsChecked == true)
                            {
                                Gamestate = GameStates.PVP;
                                _playerturn = 1;
                            }
                            else
                            {
                                Gamestate = GameStates.VsAi;
                            }

                            try
                            {
                                MaxRounds = int.Parse(_textBoxs[0].heldText);
                            }
                            catch
                            {
                                MaxRounds = 3;
                            }

                            _boardManager.Inistialize(BoardSize, BoardSize, _cellModel);

                        }
                        break;
                    }

                case 0://if nothing is being hovered over
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            foreach (var textBox in _textBoxs)
                            {
                                textBox.IsChecked = false;
                            }
                        }
                        break;
                    }

                case 6://game mode selecter
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            _modeSelector.SwapCurrentTexture();
                        }
                        break;
                    }
                case 2://board size 3x3
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 3; }
                        break;
                    }
                case 3://board size 5x5
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 5; }
                        break;
                    }
                case 4://board size 4x4
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 4; }
                        break;
                    }
                case 5://board size 9x9
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 9; }
                        break;
                    }
                case 8://difficulty ez
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { difficulty = 1; }
                        break;
                    }
                case 9://difficulty medium
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { difficulty = 2; }
                        break;
                    }
                case 10://difficulty hard
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { difficulty = 3; }
                        break;
                    }
                case 11://leaderboard
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { _databaseManager.RefreshLeaderboard(); }
                        break;
                    }
                case 12:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            var t = new Thread((ThreadStart)(() => //HONESTLY IDK, IT CREATES A TEMPORARY THREAD WHICH SOLVES A BUG, THIS THREAD IS STA WHICH ALLOWS WINDOWS FORMS TO RUN.
                            {
                                OpenFileDialog fileDialog = new OpenFileDialog();
                                fileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
                                if (fileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    FacePlayer1 = Texture2D.FromFile(GraphicsDevice, fileDialog.FileName);
                                }
                                return;
                            }));

                            t.SetApartmentState(ApartmentState.STA);
                            t.Start();
                            t.Join();
                        }
                        break;
                    }
                case 13:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            var t = new Thread((ThreadStart)(() => //HONESTLY IDK, IT CREATES A TEMPORARY THREAD WHICH SOLVES A BUG, THIS THREAD IS STA WHICH ALLOWS WINDOWS FORMS TO RUN.
                            {
                                OpenFileDialog fileDialog = new OpenFileDialog();
                                fileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
                                if (fileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    FacePlayer2 = Texture2D.FromFile(GraphicsDevice, fileDialog.FileName);
                                }
                                return;
                            }));

                            t.SetApartmentState(ApartmentState.STA);
                            t.Start();
                            t.Join();
                        }
                        break;
                    }

            }
            if (!(ButtonHovering == null)) { //textboxes are handdled differently due to *several* bugs
                foreach (TextBox txtbox in _textBoxs)
                {
                    if (ButtonHovering == txtbox)
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            foreach (var textBox in _textBoxs)
                            {
                                if (textBox != txtbox)
                                {
                                    textBox.IsChecked = false;
                                }
                            }
                            txtbox.IsChecked = true;
                        }
                    }
                }
            }
            delayedMouseState = mouseState;


        }



        private void MainUpdateLoop(GameTime gameTime) //update loop for the game, seperate method to make things simpler, kinda (I can't read)
        {

            if (CurrentRound >= MaxRounds && GameOver == false)
            {
                if (_player1.Score > _player2.Score)
                {
                    RoundEnd.Invoke(_player1.PlayerTeam);
                    _databaseManager.IncreaseScore(_player1, _player1.Score);
                }
                else if (_player1.Score < _player2.Score)
                {
                    RoundEnd.Invoke(_player2.PlayerTeam);
                    if (!(Gamestate == GameStates.VsAi))
                    {
                        _databaseManager.IncreaseScore(_player2, _player2.Score);
                    }
                }
                else
                {
                    RoundEnd.Invoke(null);
                }
                GameOver = true;
            }

            var kstate = Keyboard.GetState();
            if (UpdateProcedures.GetKeyState(Keys.LeftAlt, kstate, DelayedKstate)) //PRESS LEFT-ALT TO ENTER A DEBUGGING STATE, HELPED ME MASSIVELY
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

            foreach (Cell cel in _boardManager.CellGrid)
            {
                if (cel.SELECTED)
                {
                    cel.SELECTED = false;
                }
            }


            var mouseState = Mouse.GetState(_window); //TAKE THE MOUSE STATE
            if (!DebugMode && Gamestate == GameStates.VsAi)
            {
                
                if (mouseState.Y >= 600) //THIS HANDLES LOOKING UP AND DOWN USING THE MOUSE POSITION, THIS IS DONE IN A BASIC MANNER
                {
                    Target = _boardManager.Board.Position;
                }
                else if (mouseState.Y <= 100)
                {
                    Target = new Vector3(0, 4.8f, -3.5f);
                }
                if (CurrentFacing != Target)
                {
                    CurrentFacing = Vector3.SmoothStep(CurrentFacing, Target, 0.15f); //THIS HANDLES THE SMOOTHING OF THE CAMERA WHEN TURNING
                }
                _camera.SetLookAt(CurrentFacing);
            }
            else if (!DebugMode)
            {
                if (mouseState.Y >= 600) //THIS HANDLES LOOKING UP AND DOWN USING THE MOUSE POSITION, THIS IS DONE IN A BASIC MANNER
                {
                    Target = _boardManager.Board.Position;
                }
                else if (mouseState.Y <= 100)
                {
                    if (_playerturn == 1)
                    {
                        Target = new Vector3(0, 4.8f, -3.5f);
                    }
                    else
                    {
                        Target = new Vector3(0, 4.8f, 3.5f);
                    }
                }
                if (CurrentFacing != Target)
                {
                    CurrentFacing = Vector3.SmoothStep(CurrentFacing, Target, 0.15f); //THIS HANDLES THE SMOOTHING OF THE CAMERA WHEN TURNING
                }

                if (_playerturn == 1)
                {
                    TargetPos = new Vector3(0, 4.8f, 3.5f);
                }
                else
                {
                    TargetPos = new Vector3(0, 4.8f, -3.5f);
                }
                if (CurrentPos != TargetPos)
                {
                    CurrentPos = Vector3.SmoothStep(CurrentPos, TargetPos, 0.25f);
                }
                _camera.Position = CurrentPos;
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



        private void DebugUpdateLoop(MouseState mouseState, GameTime gameTime, KeyboardState kstate)//loop only active for debugging
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
                            CurrentRound++;
                        }
                        else
                        {
                            _player2.Score += 1;
                            CurrentRound++;
                        }
                        RoundEnd.Invoke(WinningCell);
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
                        CurrentRound++;
                    }
                    else
                    {
                        _player2.Score += 1;
                        CurrentRound++;
                    }
                    RoundEnd.Invoke(WinningCell);
                }
                else
                {
                    if (WinningCell != null)
                    {
                        EnemyAiController.EnemyAIMove(difficulty, _boardManager.CellGrid, _boardManager).update(_player2.PlayerTeam, nought, cross);
                        WinningCell = _boardManager.CheckWin();
                        if (WinningCell == _player2.PlayerTeam)
                        {
                            _player2.Score += 1;
                            RoundEnd.Invoke(WinningCell);
                            CurrentRound++;
                        }
                    }
                    else
                    {
                        RoundEnd.Invoke(WinningCell);
                    }
                }
            }
        }



        private void RoundEndScreen(CellState? cellstate)
        {
            RoundEndScreenNum = 0;
            switch (cellstate)
            {
                case CellState.Cross: RoundWinner = "Player 1"; break;
                case CellState.Nought: RoundWinner = "Player 2"; break;
                case null: RoundWinner = ""; break;
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
                        

                        UpdateProcedures.DrawCharacter(_opponent.Model, _opponent.World_2, _camera.View, projection, GraphicsDevice, Vector3.UnitZ, FacePlayer2);
                        UpdateProcedures.DrawCharacter(_opponent.Model, _opponent.World_1, _camera.View, projection, GraphicsDevice, Vector3.UnitX, FacePlayer1);

                        _backgroundManagment.DrawMain(_camera.View, projection);

                        if (_boardManager.Initialized)
                        {
                            UpdateProcedures.DrawModel(_boardManager.Board.BoardModel, _boardManager.Board.World, _camera.View, projection, GraphicsDevice);
                            foreach (Cell cell in _boardManager.CellGrid)
                            {
                                if (cell.SELECTED == false)
                                {
                                    UpdateProcedures.DrawModel(cell.Model, cell.World, _camera.View, projection, GraphicsDevice);
                                    if (cell.Marked())
                                    {
                                        UpdateProcedures.DrawModel(cell.ExtraModel, cell.World, _camera.View, projection, GraphicsDevice);
                                    }
                                }
                                else
                                {
                                    if (cell.Marked()) { 
                                        UpdateProcedures.DrawModelEmissive(cell.Model, cell.World, _camera.View, projection, GraphicsDevice, new Vector3(1,0,0));
                                        UpdateProcedures.DrawModelEmissive(cell.ExtraModel, cell.World, _camera.View, projection, GraphicsDevice, new Vector3(1, 0, 0));
                                    }
                                    else
                                    {
                                        UpdateProcedures.DrawModelEmissive(cell.Model, cell.World, _camera.View, projection, GraphicsDevice, new Vector3(0, 1, 0));
                                    }
                                }
                            }
                        }


                        _spriteBatch.Begin(); //DRAWS 2D STUFF, MUST BE DRAWN LAST -- TURNS OUT IT ALSO LIKES TO BREAK OTHER DRAW FUNCTIONS :D
                        
                        _spriteBatch.DrawString(_font, $"{FPS}", new Vector2(0, 0), Color.Wheat);

                        _spriteBatch.DrawString(_font, $"{DebugString}", new Vector2(100, 0), Color.Red);
                        if (RoundEndScreenNum < 120 && RoundWinner != "")
                        {
                            _spriteBatch.DrawString(_RoundEndFont, $"{RoundWinner} Won the Round!", new Vector2(GraphicsDevice.Viewport.Width/2 - 376.5f, GraphicsDevice.Viewport.Height / 2), Color.Red);
                            RoundEndScreenNum++;
                        }
                        else if (RoundEndScreenNum < 120)
                        {
                            _spriteBatch.DrawString(_RoundEndFont, $"Tie", new Vector2(GraphicsDevice.Viewport.Width / 2 - 46.5f, GraphicsDevice.Viewport.Height / 2), Color.Red);
                            RoundEndScreenNum++;
                        }
                        if (RoundEndScreenNum == 119)
                        {
                            _boardManager.Reset(null);
                            if (GameOver)
                            {
                                _player1.Score = 0;
                                _player2.Score = 0;
                                CurrentRound = 0;
                                GameOver = false;
                                Gamestate = GameStates.Menu;
                            }
                        }
                        _spriteBatch.End();
                        break;
                    }
                case GameStates.Menu:
                {
                        _backgroundManagment.DrawMain(_camera.View, projection);
                        _spriteBatch.Begin();
                        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                        if (_modeSelector.IsChecked)
                        {
                            foreach (MenuButton button in _buttons)
                            {
                                if(button.ID != 8 && button.ID != 9 && button.ID != 10)//difficulty AI
                                button.Draw(_spriteBatch);
                            }
                        }
                        else
                        {
                            foreach (MenuButton button in _buttons)
                            {
                                button.Draw(_spriteBatch);
                            }
                        }
                        _spriteBatch.DrawString(_font, $"{DebugString}", Vector2.Zero, Color.Black, 0f, Vector2.Zero, 5f, SpriteEffects.None, 1);
                        _spriteBatch.DrawString(_RoundEndFont, $"You are logged in as {_databaseManager.Username}", new Vector2(20, 670), Color.DarkRed, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                        

                        try //THIS CODE REALLY UPSETS ME, BUT IT DOES WORK
                        {
                            string[,] temp = _databaseManager.LeaderboardArray;
                            for (int i = 0; i < temp.GetLength(0); i++)
                            {
                                if (!string.IsNullOrEmpty(temp[i, 0]))
                                {
                                    if (temp[i, 0].Length < 10)
                                    {
                                        _spriteBatch.DrawString(_textBoxFont, temp[i, 0], new Vector2(950, (24 * i) + 65.35f), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1);
                                    }
                                    else
                                    {
                                        //System.Windows.Forms.MessageBox.Show("trimming");
                                        string drawntext = temp[i,0].Substring(0,8);
                                        _spriteBatch.DrawString(_textBoxFont, drawntext+"...", new Vector2(950, (24 * i) + 65.35f), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1);
                                    }



                                    _spriteBatch.DrawString(_textBoxFont, temp[i, 1], new Vector2(1120, (24 * i) + 65.35f), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1);
                                    _spriteBatch.DrawString(_textBoxFont, temp[i, 2], new Vector2(1200, (24 * i) + 65.35f), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1);
                                }
                            }
                        }
                        catch
                        {

                        }
                        _spriteBatch.End();
                        break;
                }
            }
            
            base.Draw(gameTime);
        }







    }
}