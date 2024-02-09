using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets;

internal abstract class BoxWithFixText : ISimpleDrawable
{
    readonly string text;
    internal string Text => text;

    protected readonly Texture2D texture;
    protected readonly SpriteFont font;
    protected readonly Color fontColor;

    readonly Vector2 destination;
    internal Vector2 Destination => destination;

    readonly Vector2 padding;
    internal Vector2 Padding => padding;

    protected readonly Vector2 textDestination;

    readonly int width;
    internal int Width => width;

    readonly int height;
    internal int Height => height;

    protected readonly Rectangle destinationRectangle;

    public BoxWithFixText(string text, Texture2D texture, SpriteFont font, Color fontColor, Vector2 destination, Vector2 padding)
    {
        this.text = text;
        this.texture = texture;
        this.font = font;
        this.fontColor = fontColor;
        this.destination = destination;
        this.padding = padding;

        textDestination = new Vector2(destination.X + padding.X, destination.Y + padding.Y);
        Vector2 measurements = font.MeasureString(text);
        width = (int)(measurements.X + padding.X * 2);
        height = (int)(measurements.Y + padding.Y * 2);

        destinationRectangle = new((int)destination.X, (int)destination.Y, width, height);
    }

    public abstract void Draw(SpriteBatch openSpriteBatch, GameTime gameTime);
}
