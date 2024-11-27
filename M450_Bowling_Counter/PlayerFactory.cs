using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M450_Bowling_Counter
{
    public static class PlayerFactory
    {
        public static List<Player> CreatePlayers(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            List<Player> players = new List<Player>();
            int playerCount;

            do
            {
                outputProvider.DisplayMessage("Anzahl der Spieler (minimum 1): ");
                playerCount = inputProvider.GetPlayerCount();
            } while (playerCount < 1);

            for (int i = 1; i <= playerCount; i++)
            {
                outputProvider.DisplayMessage($"Name für Spieler {i}: ");
                string name = inputProvider.GetPlayerName(i);
                players.Add(new Player(name));
                outputProvider.DisplayMessage($"\n");
            }

            return players;
        }
    }
}
