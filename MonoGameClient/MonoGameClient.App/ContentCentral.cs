using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App;

internal static class ContentCentral
{
    static bool loaded = false;

    internal static SpriteFont ComicMono48 { get; private set; }

    internal static Texture2D LongButton1 { get; private set; }

    internal static void Load(ContentManager contentManager)
    {
        if (loaded)
        {
            return;
        }

        ComicMono48 = contentManager.Load<SpriteFont>("Fonts/ComicMono48");

        LongButton1 = contentManager.Load<Texture2D>("Assets/Buttons/longButton1");

        loaded = true;
    }
}
