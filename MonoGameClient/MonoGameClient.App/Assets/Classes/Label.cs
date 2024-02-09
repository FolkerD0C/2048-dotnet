using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets;

internal class Label : BoxWithFixText
{
    Rectangle sourceRectangle;

    public Label(string text, Texture2D texture, SpriteFont font, Color fontColor, Vector2 destination, Vector2 padding) : base(text, texture, font, fontColor, destination, padding)
    {
        sourceRectangle = new(0, 0, texture.Width, texture.Height);
    }

    public Label(string text, Texture2D texture, SpriteFont font, Color fontColor, Vector2 destination) : this(text, texture, font, fontColor, destination, new Vector2(20, 20))
    {
    }

    public Label(string text, Texture2D texture, SpriteFont font, Color fontColor, Vector2 destination, float padding) : this(text, texture, font, fontColor, destination, new Vector2(padding, padding))
    {
    }

    public override void Draw(SpriteBatch openSpriteBatch, GameTime gameTime)
    {
        openSpriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
        openSpriteBatch.DrawString(font, Text, textDestination, fontColor);
    }
}
