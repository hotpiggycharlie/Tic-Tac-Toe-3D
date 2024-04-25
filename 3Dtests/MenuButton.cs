using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace _3Dtests
{
    internal class MenuButton: Game1
{
        private bool _isChecked = true;

        public bool IsChecked { get { return _isChecked; } set { _isChecked = value; } }

        private Texture2D _texture;
        private Vector2 _position;

        public Color Colour = Color.White;

        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        public MenuButton(Texture2D texture, Vector2 position)
        {
            _position = position;
            _texture = texture;
        }
        public Rectangle rectangle
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, rectangle, Colour);
        }

    }
}
