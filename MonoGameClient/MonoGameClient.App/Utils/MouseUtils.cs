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

    internal static bool LockMouseHandle(Guid handleCandidateId)
    {
        if (mouseLeftButtonHandle == null)
        {
            mouseLeftButtonHandle = handleCandidateId;
            return true;
        }
        return false;
    }

    internal static bool ReleaseMouseHandle(Guid handleId)
    {
        if (mouseLeftButtonHandle == handleId)
        {
            mouseLeftButtonHandle = null;
            return true;
        }
        return false;
    }
}
