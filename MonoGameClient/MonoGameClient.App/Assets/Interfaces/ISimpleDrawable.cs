using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Assets;

internal interface ISimpleDrawable
{
    void Draw(SpriteBatch openSpriteBatch, GameTime gameTime);
}
