using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tic_Tac_Toe
{
    class Characters //holds character models, is needed if any animation want to be done, which I'm not certain they ever will be lol, but just to be sure
    {
        private Model _model;
        public Model Model { get { return _model; }  set { _model = value; }  }

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; } set { position = value; }
        }

        private Matrix world_2;
        public Matrix World_2 { get { return world_2; } set { world_2 = value; } }

        private Matrix world_1;
        public Matrix World_1 { get { return world_1; } set { world_1 = value; } }


        public Characters(Vector3 defaultLocationP1, Vector3 defaultLocationP2)
        {
            world_2 = Matrix.CreateWorld(defaultLocationP2, Vector3.UnitX, Vector3.Up);
            world_1 = Matrix.CreateWorld(defaultLocationP1, -Vector3.UnitX, Vector3.Up);
        }


    }
}
