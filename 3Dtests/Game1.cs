using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3Dtests
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model _model, board;
        private Vector3 _position, Target, CurrentFacing;
        private float _speed, _RotationSpeed, _sprintSpeed, _defaultSpeed;
        private float RotatX, RotatY;

        private SpriteFont _font;



        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix world2 = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(0, 8, -1));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 10, 1), new Vector3(0, 0, 0), -Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _position = Vector3.Zero;
            _defaultSpeed = 1f; _sprintSpeed = 10f;
            _RotationSpeed = 0.006f;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _model = Content.Load<Model>("character");
            board = Content.Load<Model>("untitled");
            _font = Content.Load<SpriteFont>("DebugText");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here



            this.IsMouseVisible = false;
            var mouseState = Mouse.GetState();

            var kstate = Keyboard.GetState();
            /*
            if (kstate.IsKeyDown(Keys.LeftShift)) //Get sprinting, must be first
            {
                _speed = _sprintSpeed;
            }
            else
            {
                _speed = _defaultSpeed;
            }

            if (kstate.IsKeyDown(Keys.W)) //Movement basics
            {
                _position.Y += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (kstate.IsKeyDown(Keys.A))
            {
                _position.X -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (kstate.IsKeyDown(Keys.D))
            {
                _position.X += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (kstate.IsKeyDown(Keys.S))
            {
                _position.Y -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }*/

            if (kstate.IsKeyDown(Keys.Space))
            {
                Target = new Vector3(0, 8, -1);
            }
            else
            {
                Target = new Vector3(0, 0, 0);
            }

            if(CurrentFacing != Target)
            {
                if (CurrentFacing.X < Target.X)
                {
                    CurrentFacing.X += 1;
                }
                else if (CurrentFacing.X > Target.X)
                {
                    CurrentFacing.X -= 1;
                }

                if (CurrentFacing.Y < Target.Y)
                {
                    CurrentFacing.Y += 1;
                }
                else if (CurrentFacing.Y > Target.Y)
                {
                    CurrentFacing.Y -= 1;
                }

                if (CurrentFacing.Z < Target.Z)
                {
                    CurrentFacing.Z += 1;
                }
                else if (CurrentFacing.Z > Target.Z)
                {
                    CurrentFacing.Z -= 1;
                }
            }



            //RotatX = _RotationSpeed * mouseState.X;
            //RotatY = _RotationSpeed * mouseState.Y;

            //world = Matrix.CreateRotationZ(RotatX) * Matrix.CreateRotationX(-RotatY) * Matrix.CreateTranslation(_position);
            //world = Matrix.CreateTranslation(_position);

            /*Vector3 CameraDirection = Matrix.CreateFromYawPitchRoll(RotatY, RotatX, 0).Forward;
            Vector3 CamPosition = new Vector3(_position.X + 10, _position.Y, _position.Z);
            Vector3 camTarget = _model.Root.Transform.Forward + CameraDirection;*/

            /*float _camAngle = RotatX;
            Vector3 _camPosition = new Vector3(_position.X+5, _position.Y, _position.Z);
            Vector3 _camTarget = _camPosition + Matrix.CreateFromYawPitchRoll(_camAngle, 4.655f, 0).Forward;
            view = Matrix.CreateLookAt(_camPosition, _camTarget, Vector3.Up);*/

            //view = Matrix.CreateLookAt(new Vector3(_position.X + 5, _position.Y + 5, _position.Z+1), new Vector3(_position.X, _position.Y, _position.Z + 1), Vector3.UnitZ);

            view = Matrix.CreateLookAt(new Vector3(0, 10, 1), CurrentFacing, -Vector3.UnitY);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //_spriteBatch.Begin();

            //_spriteBatch.DrawString(_font, "X: " + RotatX.ToString() + " Y: " + RotatY.ToString(), new Vector2(0, 0), Color.Black);
            //_spriteBatch.DrawString(_font, view.ToString(), new Vector2(0, 0), Color.Black);

            //_spriteBatch.End();

            DrawModel(_model, world, view, projection);
            DrawModel(board, world2, view, projection);
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
                }
                mesh.Draw();
            }
        }

    }
}