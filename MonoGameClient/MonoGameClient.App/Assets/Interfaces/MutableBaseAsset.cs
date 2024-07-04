using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameClient.App.Assets;

internal abstract class MutableBaseAsset : BaseAsset, ISimpleResizable
{
    public abstract void Resize(int width, int height);
}