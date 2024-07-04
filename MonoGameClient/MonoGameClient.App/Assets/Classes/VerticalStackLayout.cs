using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameClient.App.Assets.Enums;

namespace MonoGameClient.App.Assets;

internal class VerticalStackLayout : StackLayout
{
    public VerticalStackLayout() : base(Orientation.Vertical)
    {
    }

    public override IEnumerable<BaseAsset> Children => throw new System.NotImplementedException();

    public override void Draw(SpriteBatch openSpriteBatch, GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }

    public override void Resize(int width, int height)
    {
        throw new System.NotImplementedException();
    }

    public override void Update(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }
}