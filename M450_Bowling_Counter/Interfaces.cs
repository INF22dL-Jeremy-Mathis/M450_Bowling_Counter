using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public interface IInputProvider
    {
        int GetPinsKnockedDown(int pinsLeft);
        int GetPlayerCount();
        string GetPlayerName(int i);
    }

    public interface IOutputProvider
    {
        void DisplayMessage(string message);
    }
}
