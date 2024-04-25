using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3Dtests
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model _model;
        private Vector3 Target, CurrentFacing;
        Board _board;
        private SpriteFont _font;
        private double FpsTime;
        private int TEMPFPS, FPS;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 10, 1), new Vector3(0, 0, 0), -Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1280f / 720, 0.1f, 100f);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _model = Content.Load<Model>("character");
            _board = new Board(Content.Load<Model>("TicTacToeBoard"));
            _font = Content.Load<SpriteFont>("DebugText");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var mouseState = Mouse.GetState();

            var kstate = Keyboard.GetState();

            TEMPFPS++;
            FpsTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FpsTime >= 1000)
            {
                FPS = TEMPFPS;
                TEMPFPS = 0;
                FpsTime = 0;
            }



            if (kstate.IsKeyDown(Keys.Space))
            {
                Target = new Vector3(0, 8, -1);
            }
            else
            {
                Target = new Vector3(0, 0, 0);
            }

            if (CurrentFacing != Target)
            {
                CurrentFacing = Vector3.SmoothStep(CurrentFacing, Target, 0.25f);
            }



            view = Matrix.CreateLookAt(new Vector3(0, 10, 1), CurrentFacing, -Vector3.UnitY);

            base.Update(gameTime);
        }

       

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"{FPS}", new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(_font, FPS.ToString(), new Vector2(0, 0), Color.Black);

            _spriteBatch.End();

            DrawModel(_model, world, view, projection);
            DrawModel(_board.BoardModel, _board.world, view, projection);
            base.Draw(gameTime);
        }


        private void DrawModel(Model model, Matrix world, Matrix view, Matrix Projection)
        {
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

    }
}