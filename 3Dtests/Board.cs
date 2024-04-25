using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dtests
{
    public class Board
    {


        public Matrix world { get; }


        public Model BoardModel { get; }

        public Board(Model x)
        {
            BoardModel = x;
            world = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(0, 8, -1));
    }

    }

}
