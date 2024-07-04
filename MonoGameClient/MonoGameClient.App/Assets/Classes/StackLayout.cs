using System.Collections.Generic;
using MonoGameClient.App.Assets.Enums;

namespace MonoGameClient.App.Assets;

internal abstract class StackLayout : MutableBaseAsset, IAssetContainer
{
    public abstract IEnumerable<BaseAsset> Children { get; }

    public readonly Orientation Orientation;

    protected StackLayout(Orientation orientation)
    {
        Orientation = orientation;;
    }
}
