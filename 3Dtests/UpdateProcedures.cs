using Tic_Tac_Toe.Content;
using Tic_Tac_Toe.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace Tic_Tac_Toe
{
    static class UpdateProcedures
    {
        // THE MAIN GAME1.CS CLASS WAS TOO FULL TO READ WITH ALL THE UPDATE LOOPS, SOME ARE BEING MOVED HERE FOR LEGIBILITY'S SAKE.

        public static void MenuUpdate(List<MenuButton> _buttons, GameWindow _window,
            ref MouseState delayedMouseState, GameModeSelector _modeSelector, ref GameStates Gamestate, ref int _playerturn,
            BoardManager _boardManager, ref int BoardSize, Model _cellModel, List<TextBox> textBoxes, ref int difficulty,
            ref int maxrounds, DataBaseManager databasemanager) //UPDATE FOR THE MENU (I can't really read this anymore)
        {

            foreach (MenuButton button in _buttons)
            {
                button.Colour = Color.White;
            }
            var mouseState = Mouse.GetState(_window);
            Rectangle MouseCollider = new Rectangle(mouseState.X, mouseState.Y, 10, 10);
            MenuButton? ButtonHovering = UserInputManagment.HoveringOverButton(MouseCollider, _buttons);
            int temp = 0;
            if (ButtonHovering != null) {
                temp = ButtonHovering.ID;
            }
            switch (temp)
            {
                case 1: //start button
                    {


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

                            try
                            {
                                maxrounds = int.Parse(textBoxes[0].heldText);
                            }
                            catch
                            {
                                maxrounds = 3;
                            }

                            _boardManager.Inistialize(BoardSize, BoardSize, _cellModel);

                        }
                        break;
                    }

                case 0://if nothing is being hovered over
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            foreach (var textBox in textBoxes)
                            {
                                textBox.IsChecked = false;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            _modeSelector.SwapCurrentTexture();
                        }
                        break;
                    }
                case 2:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 3; }
                        break;
                    }
                case 3:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 5; }
                        break;
                    }
                case 4:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 4; }
                        break;
                    }
                case 5:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { BoardSize = 9; }
                        break;
                    }
                case 8:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { difficulty = 1; }
                        break;
                    }
                case 9:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { difficulty = 1; }
                        break;
                    }
                case 10:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { difficulty = 1; }
                        break;
                    }
                case 11:
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released) { databasemanager.RefreshLeaderboard(_buttons[10]); }
                        break;
                    }

            }
            if (ButtonHovering == null)
            {
                
            }
            else
            {
                foreach (TextBox txtbox in textBoxes)
                {
                    if (ButtonHovering == txtbox)
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed && delayedMouseState.LeftButton == ButtonState.Released)
                        {
                            foreach(var textBox in textBoxes)
                            {
                                if (textBox != txtbox)
                                {
                                    textBox.IsChecked = false;
                                }
                            }
                            txtbox.IsChecked = true;
                        }
                    }
                }
            }
            delayedMouseState = mouseState;


        }


        /*.____________________________________________________________________________________________________________________________________________________________________.*/

        public static void DrawModel(Model model, Matrix world, Matrix view, Matrix Projection, GraphicsDevice device)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    device.DepthStencilState = DepthStencilState.Default;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = Projection;
                    if (effect.LightingEnabled == false)
                    {
                        effect.EnableDefaultLighting();
                    }
                    effect.EmissiveColor = Vector3.Zero;
                }
                mesh.Draw();
            }
        }

        public static void DrawModel(Model model, Matrix world, Matrix view, Matrix Projection, GraphicsDevice device, Vector3 SelectedColour) //mostly for cells
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    device.DepthStencilState = DepthStencilState.Default;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = Projection;
                    if (effect.LightingEnabled == false)
                    {
                        effect.EnableDefaultLighting();
                    }
                    effect.EmissiveColor = SelectedColour;
                }
                mesh.Draw();
            }
        }


        public static bool GetKeyState(Keys key, KeyboardState k1, KeyboardState k2)
        {
            if(k1.IsKeyDown(key) && !k2.IsKeyDown(key)){
                return true;
            }
            return false;
        }

    }
}
