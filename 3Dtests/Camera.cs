using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe
{
    internal class Camera : GameComponent//inherits game component, I only learnt I could do this recently, woops lol (my code is irrepairable and my day is ruined)
    {
        private Vector3 cameraLookAt;
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Matrix View { get { return Matrix.CreateLookAt(Position, cameraLookAt, Vector3.Up); } }

        public Camera(Game game) : base(game)
        {
            this.cameraLookAt = Vector3.Zero;
            this.Position = new Vector3(0,8.5f,2);
            this.Rotation = Vector3.Zero;
        }

        private void UpdateLookAt()//Called on update loop to rebuild what the camera can see
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            cameraLookAt = Position + lookAtOffset;
        }


        public void SetPosition(Vector3 position) //method called to move camera from another class MOSTLY FOR DEBUGGING
        {
            Matrix rotate = Matrix.CreateRotationY(Rotation.Y);

            position = Vector3.Transform(position, rotate);

            Vector3 cameraView = Position + position;

            Position = cameraView;
            UpdateLookAt();
        }

        public void SetRotation(Vector3 rotation) //change the rotation of the camera, very important lol
        {
            this.Rotation = rotation;

            UpdateLookAt();

        }

        public void SetLookAt(Vector3 position)
        {
            cameraLookAt = position;
        }
    }
}
