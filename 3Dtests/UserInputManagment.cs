using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe
{
    static class UserInputManagment //A collection of tools, these will be important later ;)
{


        public static Ray CalculateRayFromMouse(Vector2 MouseLocation, Matrix View, Matrix Projection, Viewport Viewport) //finds a ray from mouse in 3D space
        {                                                   //SOURCE = mouse location on camera
            Vector3 AtCamera = Viewport.Unproject(new Vector3(MouseLocation.X, MouseLocation.Y, 0f), Projection, View, Matrix.Identity);

            Vector3 FarFromCamera = Viewport.Unproject(new Vector3(MouseLocation.X, MouseLocation.Y, 1f), Projection, View, Matrix.Identity); //same exact thing, just far from camera

            Vector3 Direction = FarFromCamera - AtCamera;

            Direction.Normalize();

            return new Ray(AtCamera, Direction);
        }

        public static float? CollisionIntersection(BoundingSphere sphere, Vector2 mouseLocation, Matrix View, Matrix Projection, Viewport Viewport) //checks if a ray interacts with a collision sphere (hitbox)
        {
            Ray Mouse = CalculateRayFromMouse(mouseLocation, View, Projection, Viewport);
            return Mouse.Intersects(sphere);
        }

        public static bool MouseRayIntesects(Matrix World, Model Model, Vector2 mouseLocation, Matrix View, Matrix Projection, Viewport Viewport) //checks if a ray interacts with a collision sphere, then returns it as a bool
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                BoundingSphere sphere = mesh.BoundingSphere;
                sphere = sphere.Transform(World);
                float? collision = CollisionIntersection(sphere, mouseLocation, View, Projection, Viewport);
                if(collision != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static MenuButton? HoveringOverButton(Rectangle MouseCollider, List<MenuButton> buttons) //check to see if a menu button is being hovered over
        {
            foreach (MenuButton button in buttons)
            {
                if (MouseCollider.Intersects(button.rectangle))
                {
                    button.Colour = Color.DimGray;
                    return button;
                }
            }
            return null;
        }


    }
}
