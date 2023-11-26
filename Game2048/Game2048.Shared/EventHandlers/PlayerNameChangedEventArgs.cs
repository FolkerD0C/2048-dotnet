using System;

namespace Game2048.Shared.EventHandlers
{
    public class PlayerNameChangedEventArgs : EventArgs
    {
        public string PlayerName { get; }

        public PlayerNameChangedEventArgs(string playerName)
        {
            PlayerName = playerName;
        }
    }
}
