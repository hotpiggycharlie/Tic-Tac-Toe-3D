using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    internal class ScoreManagment: GameComponent //class to manage everything score related, database and all lol
    {


        private Model _numbers;
        public Matrix World = Matrix.CreateWorld(new Vector3(0,5,0), -Vector3.UnitZ, Vector3.Up);
        public bool Draw = true;

        public ScoreManagment(Game game, Model numModel, Player player1, Player player2) : base(game)
        {
            _numbers = numModel;
        }

        public ModelMesh ScoreUpdate(int score)
        {
            return _numbers.Meshes[score - 1];
        }

    }
}
