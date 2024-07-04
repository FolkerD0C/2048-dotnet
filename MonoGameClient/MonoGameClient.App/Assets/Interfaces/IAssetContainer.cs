using System.Collections.Generic;

namespace MonoGameClient.App.Assets;

internal interface IAssetContainer
{
    IEnumerable<BaseAsset> Children { get; }
}