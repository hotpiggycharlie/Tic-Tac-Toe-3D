using _3Dtests.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dtests
{
    class Player
{

        public string Name { get; set; }
        public int Score { get; set; }
        public CellState PlayerTeam { get; set; }

        public Player(CellState playerTeam, string name)
        {
            PlayerTeam = playerTeam;
            Score = 0;
            Name = name;
        }
    }
}
