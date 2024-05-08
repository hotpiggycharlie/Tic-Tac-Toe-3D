using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe
{
    public class Board //LITERALLY JUST EXISTS FOR THE MODEL TO WORK LOL
    {

        private Matrix world;
        public Matrix World { get { return world; } set { world = value; } }

        private Model model;
        public Model BoardModel { get { return model; } set { model = value; } }
        public Vector3 Position;

        public Board(Model x)
        {
            Position = new Vector3(0, 2.67f, 0);
            BoardModel = x;
            world = Matrix.CreateTranslation(Position); //this just handels the board model, nothing fancy
    }




    }

}
