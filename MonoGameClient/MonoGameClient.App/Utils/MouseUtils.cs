using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameClient.App.Utils;

internal static class MouseUtils
{
    static MouseState current;

    static bool currentlyClicked = false;
    static Guid? currentMouseHandleHolder;

    internal static int XPos => current.X;
    internal static int YPos => current.Y;
    internal static bool LeftDown => current.LeftButton == ButtonState.Pressed;

    static Guid? mouseLeftButtonHandle;
    internal static Guid? MouseLeftButtonHandle => mouseLeftButtonHandle;

    internal static event EventHandler KeyDown;
    internal static event EventHandler KeyUp;

    internal static void Update()
    {
        current = Mouse.GetState();

        if (!currentlyClicked && LeftDown)
        {
            KeyDown?.Invoke(null, EventArgs.Empty);
        }
        else if (currentlyClicked && !LeftDown)
        {
            KeyUp?.Invoke(null, EventArgs.Empty);
        }

        currentlyClicked = LeftDown;
    }

    internal static bool LockMouseHandle(Guid id)
    {
        if (currentMouseHandleHolder is null)
        {
            currentMouseHandleHolder = id;
            return true;
        }
        return false;
    }

    internal static bool ReleaseMouseHandle(Guid id)
    {
        if (currentMouseHandleHolder is not null && currentMouseHandleHolder == id)
        {
            currentMouseHandleHolder = null;
            return true;
        }
        return false;
    }

    internal static bool IsCurrentMouseHandleHolder(Guid id)
    {
        return currentMouseHandleHolder is not null && currentMouseHandleHolder == id;
    }

    internal static bool MouseIsFree()
    {
        return currentMouseHandleHolder is null;
    }
}
