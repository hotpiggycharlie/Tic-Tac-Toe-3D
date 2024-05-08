using Tic_Tac_Toe.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe
{

    class Cell // class for each and every cell on the board, exists mostly to spite me, also handles the model of the nought or cross it contains, this is my least favourite class
{
        private Model _model, _extraModel;
        private CellState _state;
        private Vector3 Position;
        private Matrix _world;
        private int _cellNumber;
        public bool SELECTED = false;
        public int CellNumber { get { return _cellNumber; } }
        public Matrix World { get { return _world; } }
        public Model Model { get { return _model; } }
        public Vector3 DbugPostion { get { return Position; } }
        public Model ExtraModel { get { return _extraModel; } }
        public Cell(Vector2 Coordinates, Model model)
        {
            Position = new Vector3(Coordinates.X, 0.15f, Coordinates.Y);
            _model = model;
        }

        public void GetGlobalPosition(Matrix BoardWorld)
        {
            _world = Matrix.CreateTranslation(BoardWorld.Translation + Position);
        }

        public void Corrections(float radius, int cellNum)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                mesh.BoundingSphere = new BoundingSphere(mesh.BoundingSphere.Center, radius);
            }
            _cellNumber = cellNum + 1;
        }


        public void update(CellState state, Model nought, Model Cross)
        {
            _state = state;
            if (_state == CellState.Nought)
            {
                _extraModel = nought;
            }
            else if(_state == CellState.Cross)
            {
                _extraModel = Cross;
            }
        }

        public bool Marked()
        {
            if(_state != CellState.Empty)
            {
                return true;
            }
            return false;
        }


        public CellState State { get { return _state; } set { _state = value; } }
    }
}
