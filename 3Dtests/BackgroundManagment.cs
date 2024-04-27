using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    internal class BackgroundManagment: DrawableGameComponent
{
        private Matrix world;
        private List<Model> _backGroundManagerList;
        public List<Model> BackGroundManagerList { get { return _backGroundManagerList; } set { _backGroundManagerList = value; } }

        private List<Model> _MainGameLoopBackGroundManagerList;

        public BackgroundManagment(Model MenuBack, Game game): base(game)
        {
            _backGroundManagerList = new List<Model>
            {
                MenuBack
            };
            world = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitX, Vector3.Up);
        }

        public void SortAssets()
        {
            _backGroundManagerList.Clear();
            _backGroundManagerList = _MainGameLoopBackGroundManagerList;
            _MainGameLoopBackGroundManagerList.Clear();
        }


        public void DrawMain(Matrix view, Matrix Projection)
        {
            foreach (var model in _backGroundManagerList)
            {
                UpdateProcedures.DrawModel(model, world, view, Projection, GraphicsDevice);
            }
        }

    }
}
