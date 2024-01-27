using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameClient.App.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets
{
    internal class Button : IDrawable
    {
        enum ButtonState
        {
            Idle,
            Hover,
            ClickDown,
            Clicked
        }

        ButtonState state;
        Vector2 destinationPosition;
        Texture2D textureAtlas;
        int height;
        int width;
        string text;
        SpriteFont font;

        readonly Guid id;
        public Guid Id => id;

        public event EventHandler Click;

        public Button(Texture2D textureAtlas, Vector2 destinationPosition, int height, int width, string text, SpriteFont font)
        {
            id = Guid.NewGuid();
            state = ButtonState.Idle;
            this.textureAtlas = textureAtlas;
            this.destinationPosition = destinationPosition;
            this.height = height;
            this.width = width;
            this.text = text;
            this.font = font;
        }

        public void Draw(SpriteBatch openSpriteBatch)
        {
            int atlasColumn;
            int atlasRow;
            switch (state)
            {
                case ButtonState.Idle:
                    {
                        atlasColumn = 0;
                        atlasRow = 0;
                        break;
                    }
                case ButtonState.Hover:
                    {
                        atlasColumn = 1;
                        atlasRow = 0;
                        break;
                    }
                case ButtonState.ClickDown:
                    {
                        atlasColumn = 0;
                        atlasRow = 1;
                        break;
                    }
                case ButtonState.Clicked:
                    {
                        atlasColumn = 1;
                        atlasRow = 1;
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid button state: " + state + " in button: " + id);
            }

            Rectangle sourceRectangle = new(textureAtlas.Width / 2 * atlasColumn, textureAtlas.Height / 2 * atlasRow, textureAtlas.Width / 2, textureAtlas.Height / 2);
            Rectangle destinationRectangle = new((int)destinationPosition.X, (int)destinationPosition.Y, width, height);

            openSpriteBatch.Draw(textureAtlas, destinationRectangle, sourceRectangle, Color.White);
        }

        public void Update()
        {
            if (state == ButtonState.ClickDown && !MouseUtils.LeftDown)
            {
                Click?.Invoke(this, EventArgs.Empty);
                state = ButtonState.Clicked;
                return;
            }

            if (MouseUtils.XPos < destinationPosition.X || MouseUtils.XPos > destinationPosition.X + width
                || MouseUtils.YPos < destinationPosition.Y || MouseUtils.YPos > destinationPosition.Y + height)
            {
                state = ButtonState.Idle;
                return;
            }

            if (MouseUtils.LeftDown)
            {
                state = ButtonState.ClickDown;
            }
            else
            {
                state = ButtonState.Hover;
            }
        }
    }
}
