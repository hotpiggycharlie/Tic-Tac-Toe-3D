using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3Dtests
{
    class GameModeSelector: MenuButton //does what it says on the tin, inherits from default menu button
{

        private Texture2D _PrimaryTexture;
        private Texture2D _SecondaryTexture;

        public GameModeSelector(Texture2D texture1, Texture2D texture2, Vector2 position): base(texture1, position)
        {
            _PrimaryTexture = texture1;
            _SecondaryTexture = texture2;
            IsChecked = true;
        }

        public void SwapCurrentTexture()
        {
            if (Texture != _PrimaryTexture)
            {
                Texture = _PrimaryTexture;
                IsChecked = true;
            }
            else
            {
                Texture = _SecondaryTexture;
                IsChecked = false;
            }
        }


    }
}
