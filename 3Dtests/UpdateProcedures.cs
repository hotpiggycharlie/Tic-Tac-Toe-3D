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
        //tools that are used in a lot of different places, basic things like drawing code and some user input stuff
                



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

        public static void DrawCharacter(Model model, Matrix world, Matrix view, Matrix Projection, GraphicsDevice device, Vector3 Colour, Texture2D? texture)
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
                    if (mesh.Name != "Face")
                    {
                        effect.DiffuseColor = Colour;
                    }
                    else if(texture != null)
                    {
                        effect.Texture = texture;
                    }
                }
                mesh.Draw();
            }
        }


        /*.____________________________________________________________________________________________________________________________________________________________________.*/

        public static void DrawModelEmissive(Model model, Matrix world, Matrix view, Matrix Projection, GraphicsDevice device, Vector3 SelectedColour) //mostly for cells
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


        /*.____________________________________________________________________________________________________________________________________________________________________.*/
        
        
        public static bool GetKeyState(Keys key, KeyboardState k1, KeyboardState k2)
        {
            if(k1.IsKeyDown(key) && !k2.IsKeyDown(key)){
                return true;
            }
            return false;
        }

    }
}
