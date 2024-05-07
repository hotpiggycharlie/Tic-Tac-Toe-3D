using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tic_Tac_Toe
{
    internal class MenuButton
{
        private bool _isChecked = true;

        public int ID;

        public bool IsChecked { get { return _isChecked; } set { _isChecked = value; } }

        protected Texture2D _texture;
        protected Vector2 _position;

        public Color Colour = Color.White;

        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        public MenuButton(Texture2D texture, Vector2 position, int ID)
        {
            _position = position;
            _texture = texture;
            this.ID = ID;
        }
        public Rectangle rectangle
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, rectangle, Colour);
        }

    }
}
