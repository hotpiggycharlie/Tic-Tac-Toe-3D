using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tic_Tac_Toe
{
    class Opponent //holds opponent models, is needed if any animation want to be done, which I'm not certain they ever will be lol, but just to be sure
    {
        private Model _model;
        public Model Model { get { return _model; }  set { _model = value; }  }

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; } set { position = value; }
        }

        private Matrix world;
        public Matrix World { get { return world; } set { world = value; } }


        public Opponent(Vector3 defaultLocation)
        {
            world = Matrix.CreateWorld(defaultLocation, Vector3.UnitX, Vector3.Up);
        }


    }
}
