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
    internal static SpriteFont UniFontEXMono36 { get; private set; }
    internal static SpriteFont FreeMono36 { get; private set; }

    internal static Texture2D LongButton1 { get; private set; }

    internal static Texture2D ProportionedButton1 { get; private set; }

    internal static Texture2D LongLabel1 { get; private set; }

    internal static void Load(ContentManager contentManager)
    {
        if (loaded)
        {
            return;
        }

        ComicMono48 = contentManager.Load<SpriteFont>("Fonts/ComicMono48");
        UniFontEXMono36 = contentManager.Load<SpriteFont>("Fonts/UnifontEX/Unifontexmono36");
        FreeMono36 = contentManager.Load<SpriteFont>("Fonts/FreeMono/FreeMono36");

        LongButton1 = contentManager.Load<Texture2D>("Assets/Buttons/longButton1");
        ProportionedButton1 = contentManager.Load<Texture2D>("Assets/Buttons/proportionedButton1");

        LongLabel1 = contentManager.Load<Texture2D>("Assets/Labels/longLabel1");

        loaded = true;
    }
}
