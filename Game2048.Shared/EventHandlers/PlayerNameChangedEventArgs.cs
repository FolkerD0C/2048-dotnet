using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
