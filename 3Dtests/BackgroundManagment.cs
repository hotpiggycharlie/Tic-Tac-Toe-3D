using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3Dtests
{
    internal class BackgroundManagment
{
        private Matrix world;
        private List<Model> _backGroundManagerList;
        public List<Model> BackGroundManagerList { get { return _backGroundManagerList; } set { _backGroundManagerList = value; } }

        private List<Model> _MainGameLoopBackGroundManagerList;

        public BackgroundManagment(Model MenuBack)
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

        public void DrawMain(Matrix view, Matrix Projection, Action<Model, Matrix, Matrix, Matrix> DrawModel)
        {
            foreach (var model in _backGroundManagerList)
            {
                DrawModel(model, world, view, Projection);
            }
        }

    }
}
