using _3Dtests.Content;
using _3Dtests.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dtests
{
    static class UpdateProcedures
{
        // THE MAIN GAME1.CS CLASS WAS TOO FULL TO READ WITH ALL THE UPDATE LOOPS, SOME ARE BEING MOVED HERE FOR LEGIBILITY'S SAKE.

        public static void MenuUpdate(List<MenuButton> _buttons, GameWindow _window, MenuButton _startButton, ref string DebugString,
            ref MouseState delayedMouseState, GameModeSelector _modeSelector, ref GameStates Gamestate,ref int _playerturn, ref Texture2D _menuBackground,
            BoardManager _boardManager,ref int BoardSize, Model _cellModel, MenuButton[] _boardSizeButtons) //UPDATE FOR THE MENU (I can't really read this anymore)
        {
            foreach (MenuButton button in _buttons)
            {
                button.Colour = Color.White;
            }
            var mouseState = Mouse.GetState(_window);
            Rectangle MouseCollider = new Rectangle(mouseState.X, mouseState.Y, 10, 10);
            MenuButton ButtonHovering = UserInputManagment.HoveringOverButton(MouseCollider, _buttons);
            if (ButtonHovering == _startButton)
            {
                DebugString += "hovering";
                if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                {
                    if (_modeSelector.IsChecked == true)
                    {
                        Gamestate = GameStates.PVP;
                        _playerturn = 1;
                    }
                    else
                    {
                        Gamestate = GameStates.VsAi;
                    }
                    _boardManager.Inistialize(BoardSize, BoardSize, _cellModel);

                }
            }
            else if (ButtonHovering == _modeSelector)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                {
                    _modeSelector.SwapCurrentTexture();
                }
            }
            else if (ButtonHovering == _boardSizeButtons[0]) { if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 3; } }
            else if (ButtonHovering == _boardSizeButtons[1]) { if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 5; } }
            else if (ButtonHovering == _boardSizeButtons[2]) { if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 4; } }
            else if (ButtonHovering == _boardSizeButtons[3]) { if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 9; } }
            else
            {
                DebugString += "False";
            }
            delayedMouseState = mouseState;

            DebugString = _startButton.rectangle.ToString() + " " + MouseCollider.ToString() + " " + mouseState.X + " " + mouseState.Y + " " + mouseState.Position.ToString();
        }


        /*.____________________________________________________________________________________________________________________________________________________________________.*/


        

    }
}
