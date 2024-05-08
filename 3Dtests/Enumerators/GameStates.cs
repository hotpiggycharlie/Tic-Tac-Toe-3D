using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe.Enumerators
{
    public enum GameStates //handles the states of games, e.g. whether the game has started or what game mode was picked
{
        Menu,
        VsAi,
        PVP,
        Options, //unused, but nice to know it exists
        Animations, //unused, time limitations mostly :(
        SignInMenu
}
}
