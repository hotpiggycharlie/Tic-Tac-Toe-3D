using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tic_Tac_Toe
{
    class TextBox: MenuButton
    {
        public string heldText { get { return strbuilder.ToString(); } }
        private StringBuilder strbuilder = new StringBuilder();
        private int maxChars;
        private bool OnlyNumbers = false;
        private bool OnlyLetters = false;
        private SpriteFont font;
        public string InputPrompt;
        private float WrittenOffset, PromptOffset;

        public TextBox(Vector2 Position, Texture2D Texture, Game game, int MaxChars, SpriteFont font, int ID) : base(Texture, Position, ID)
        {
            game.Window.TextInput += TextUpdate;
            maxChars = MaxChars;
        }

        public TextBox(Vector2 Position, Texture2D Texture, Game game, int MaxChars, bool OnlyNumbers, bool OnlyLetters, SpriteFont font, string inputPrompt, float WrittenOffset, float PromptOffset, int ID) : base(Texture, Position, ID)
        {
            game.Window.TextInput += TextUpdate;
            maxChars = MaxChars;
            this.OnlyNumbers = OnlyNumbers;
            this.OnlyLetters = OnlyLetters;
            this.font = font;
            InputPrompt = inputPrompt;
            this.WrittenOffset = WrittenOffset;
            this.PromptOffset = PromptOffset;
        }



        private void TextUpdate(object sender, TextInputEventArgs e)
        {
            if (IsChecked)
            {
                if (e.Key == Keys.Back)
                {
                    if (strbuilder.Length > 0)
                        strbuilder.Remove(strbuilder.Length - 1, 1);
                }
                else
                {
                    if (WithinLimits(e.Character))
                    {
                        strbuilder.Append(e.Character);
                    }
                }

            }
        }

        private bool WithinLimits(Char input)
        {
            if (Char.IsLetterOrDigit(input) && Char.IsAscii(input))
            {
                if (strbuilder.ToString().Length >= maxChars)
                {
                    return false;
                }
                if (OnlyNumbers && Char.IsDigit(input))
                {
                    return true;
                }
                else if (OnlyLetters && !Char.IsDigit(input))
                {
                    return true;
                }
                else if (!OnlyLetters && !OnlyNumbers)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (heldText != "")
            {
                spriteBatch.DrawString(font, heldText, new Vector2(_position.X + WrittenOffset, _position.Y), Color.White);
            }
            spriteBatch.DrawString(font, InputPrompt, new Vector2(_position.X - PromptOffset, _position.Y), Color.White);
        }

    }
}
