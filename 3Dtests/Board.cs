using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dtests
{
    public class Board //LITERALLY JUST EXISTS FOR THE MODEL TO WORK LOL
    {

        private Matrix world;
        public Matrix World { get { return world; } set { world = value; } }

        private Model model;
        public Model BoardModel { get { return model; } set { model = value; } }

        public Board(Model x)
        {
            BoardModel = x;
            world = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(0, 8, -1)); //this just handels the board model, nothing fancy
    }




    }

}
