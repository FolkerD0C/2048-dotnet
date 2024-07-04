using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameClient.App.Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets;

internal abstract class BaseAsset : ISimpleUpdatable, ISimpleDrawable
{
    readonly Guid id;
    internal Guid Id => id;

    protected Texture2D background;

    public Vector2 Destination { get; protected set;}

    public int Width { get; protected set;}

    public int Height { get; protected set;}

    public VerticalAlignment VerticalAlignment { get; protected set;}
    
    public HorizontalAlignment HorizontalAlignment { get; protected set;}

    public abstract void Draw(SpriteBatch openSpriteBatch, GameTime gameTime);
    public abstract void Update(GameTime gameTime);

    public BaseAsset()
    {
        id = Guid.NewGuid();
    }
}
