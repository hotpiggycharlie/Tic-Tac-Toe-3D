using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tic_Tac_Toe.Content;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic.ApplicationServices;

namespace Tic_Tac_Toe
{
    internal class ScoreManagment: DrawableGameComponent //class to manage everything score related in game, models and such
    {

        private MySqlConnection connection;

        private Model _numbers;
        public Matrix World = Matrix.CreateWorld(new Vector3(0,5,0), -Vector3.UnitZ, Vector3.Up);
        public bool Drawing = true;
        Player _player1, _player2;
        Game1 _game1;
        Matrix projection;
        Camera _camera;
        Vector3 red, blue;

        public ScoreManagment(Game game, Model numModel, Player player1, Player player2, Camera cam) : base(game)
        {
            _numbers = numModel;
            _player1 = player1;
            _player2 = player2;
            _game1 = (Game1)game;
            _camera = cam;
            projection = _game1.projection;
            red = new Vector3(1, 0, 0);
            blue = new Vector3(0, 0, 1);
        }

        public ModelMesh ScoreUpdate(int score)
        {
            return _numbers.Meshes[score - 1];
        }

        public override void Draw(GameTime gameTime)
        {
            if (Drawing == true)
            {
                if (_player1.Score != 0)
                {
                    DrawModelMesh(ScoreUpdate(_player1.Score), Matrix.CreateTranslation(new Vector3(2, 4.8f, -3.5f)), red);
                    DrawModelMesh(ScoreUpdate(_player1.Score), Matrix.CreateRotationY(MathHelper.ToRadians(180f)) * Matrix.CreateTranslation(new Vector3(2, 4.8f, 3.5f)), red);
                }
                if (_player2.Score != 0)
                {
                    DrawModelMesh(ScoreUpdate(_player2.Score), Matrix.CreateTranslation(new Vector3(-2, 4.8f, -3.5f)), blue);
                    DrawModelMesh(ScoreUpdate(_player2.Score), Matrix.CreateRotationY(MathHelper.ToRadians(180f)) * Matrix.CreateTranslation(new Vector3(-2, 4.8f, 3.5f)), blue);
                }
            }
            base.Draw(gameTime);
        }



        private void DrawModelMesh(ModelMesh mesh, Matrix world, Vector3 Colour)
        {

            foreach (BasicEffect effect in mesh.Effects)
            {

                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                effect.World = world;
                effect.View = _camera.View;
                effect.Projection = projection;
                effect.DiffuseColor = Colour;
                if (effect.LightingEnabled == false)
                {
                    effect.EnableDefaultLighting();
                }
                effect.EmissiveColor = Colour*0.5f;
            }
            mesh.Draw();
        }

    }
}
