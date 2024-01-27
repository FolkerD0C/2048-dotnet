using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets;

internal interface IDrawable
{
    Guid Id { get; }

    void Update();

    void Draw(SpriteBatch openSpriteBatch);
}
