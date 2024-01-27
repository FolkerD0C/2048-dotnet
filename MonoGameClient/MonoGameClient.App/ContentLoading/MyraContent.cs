using FontStashSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.ContentLoading;

internal static class MyraContent
{
    static DynamicSpriteFont comicMono32;
    internal static DynamicSpriteFont ComicMono32 => comicMono32;

    static bool loaded = false;

    internal static void Load()
    {
        if (loaded)
        {
            throw new InvalidOperationException("Myra content is already loaded.");
        }

        byte[] comicMonoBytes = File.ReadAllBytes(Path.Join("Content", "Fonts", "ComicMono.ttf"));
        FontSystem comicMono32Factory = new();
        comicMono32Factory.AddFont(comicMonoBytes);
        comicMono32 = comicMono32Factory.GetFont(32);


        loaded = true;
    }
}
