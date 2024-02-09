using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets;

internal interface IBaseAsset : ISimpleUpdatable, ISimpleDrawable
{
    Guid Id { get; }
}
